using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;
public partial struct FallenParticleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComponent>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComp>();
        var location = new float3();
        foreach (var (trans, spawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<SpawnerComponent>>())
            location = trans.ValueRO.Position;

        if (config.mode == Mode.MainThread)
        {
            foreach (var (trans, spawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<SpawnerComponent>>())
            {
                float3 pos = trans.ValueRO.Position;
                location = trans.ValueRO.Position;
                foreach (var (particle, entity) in SystemAPI.Query<RefRW<ParticleTag>>().WithEntityAccess())
                {
                    if (particle.ValueRO.fallen)
                    {
                        state.EntityManager.SetComponentData(entity, new LocalTransform
                        {
                            Position = new float3(Random.Range(-spawner.ValueRW.depth + pos.x, spawner.ValueRW.depth + pos.x), trans.ValueRO.Position.y, Random.Range(-spawner.ValueRW.width + pos.z, spawner.ValueRW.width + pos.z)),
                            Rotation = trans.ValueRO.Rotation,
                            Scale = Random.Range(config.minScale, config.maxScale)
                        });

                        state.EntityManager.SetComponentData(entity, new ParticleTag { fallen = false });
                    }
                }
            }
        }
        else if (config.mode == Mode.Scheduled)
        {
            var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
            var spawner = SystemAPI.GetSingleton<SpawnerComponent>();
            state.Dependency = new fallenParticles
            {
                ecb = ECB,
                config = config,
                spawner = spawner,
                trans = location,
                Seed = (uint)Time.frameCount

            }.Schedule(state.Dependency);

            var entityCount = SystemAPI.QueryBuilder().WithAll<ParticleTag>().Build().CalculateEntityCount();

            foreach (var con in SystemAPI.Query<RefRW<ConfigComp>>())
            {
                con.ValueRW.particleAmount = entityCount;
            }
        }
        else if (config.mode == Mode.ScheduledParallel)
        {
            var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var spawner = SystemAPI.GetSingleton<SpawnerComponent>();

            state.Dependency = new fallenParticlesParallel
            {
                ecb = ECB,
                config = config,
                spawner = spawner,
                trans = location,
                Seed = (uint)Time.frameCount

            }.ScheduleParallel(state.Dependency);
        }
    }
}
public partial struct fallenParticles : IJobEntity
{
    public EntityCommandBuffer ecb;
    public ConfigComp config;
    public SpawnerComponent spawner;
    public float3 trans;
    public uint Seed;


    public void Execute(Entity e, ref ParticleTag particle)
    {
        if (particle.fallen)
        {

            Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex((uint)Seed + (uint)particle.id);
            ecb.SetComponent(e, LocalTransform.FromPositionRotationScale(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + trans.x,
                                          trans.y,
                                         (random.NextFloat() * spawner.width * 2) - spawner.depth + trans.z), new Quaternion(), math.lerp(config.minScale, config.maxScale, random.NextFloat())));

            particle.fallen = false;
        }
    }
}

[BurstCompile]
public partial struct fallenParticlesParallel : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public ConfigComp config;
    public SpawnerComponent spawner;
    public float3 trans;
    public uint Seed;


    public void Execute([ChunkIndexInQuery] int key, Entity e, ref ParticleTag particle)
    {
        if (particle.fallen)
        {
            Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex((uint)Seed + (uint)particle.id);
            ecb.SetComponent(key, e, LocalTransform.FromPositionRotationScale(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + trans.x,
                                          trans.y,
                                         (random.NextFloat() * spawner.width * 2) - spawner.depth + trans.z), new Quaternion(), math.lerp(config.minScale, config.maxScale, random.NextFloat())));
            particle.fallen = false;
        }
    }
}
