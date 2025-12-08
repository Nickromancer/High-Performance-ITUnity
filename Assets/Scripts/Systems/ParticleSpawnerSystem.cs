using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

partial struct ParticleSpawnerSystem : ISystem
{
    EntityCommandBuffer ECB;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComp>();

        foreach (var (trans, spawner) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SpawnerComponent>>())
        {
            switch (config.environmentType)
            {
                case snowEnvironment.snowCity:
                    trans.ValueRW.Position = new float3(50f, 90f, -50f);
                    spawner.ValueRW.depth = 50;
                    spawner.ValueRW.width = 50;
                    break;

                case snowEnvironment.snowForrest:
                    trans.ValueRW.Position = new float3(-50f, 90f, 50f);
                    spawner.ValueRW.depth = 50;
                    spawner.ValueRW.width = 50;
                    break;

                case snowEnvironment.snowMountain:
                    trans.ValueRW.Position = new float3(50f, 90f, 50f);
                    spawner.ValueRW.depth = 50;
                    spawner.ValueRW.width = 50;
                    break;

                case snowEnvironment.snowPlane:
                    trans.ValueRW.Position = new float3(-50f, 90f, -50f);
                    spawner.ValueRW.depth = 50;
                    spawner.ValueRW.width = 50;
                    break;
                case snowEnvironment.movingSnowCloud:
                    float time = (float)SystemAPI.Time.ElapsedTime / 8;
                    trans.ValueRW.Position = new float3(math.cos(time) * 50f, 90f, math.sin(time) * 50f);
                    spawner.ValueRW.depth = 50;
                    spawner.ValueRW.width = 50;
                    break;
                case snowEnvironment.wholeMap:
                    trans.ValueRW.Position = new float3(0f, 90f, 0f);
                    spawner.ValueRW.depth = 100;
                    spawner.ValueRW.width = 100;
                    break;
                case snowEnvironment.None:
                    break;
            }

        }
        if (config.mode == Mode.MainThread)
        {
            if (config.maxParticlesAmount < config.particleAmount || config.multiplier == 0)
                return;
            else
            {
                foreach (var (trans, spawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<SpawnerComponent>>())
                {
                    float3 pos = trans.ValueRO.Position;
                    if (spawner.ValueRW.timer <= 0 && config.maxParticlesAmount >= config.particleAmount)
                    {
                        for (int i = 0; i < config.multiplier; i++)
                        {
                            Entity e = state.EntityManager.Instantiate(config.prefab);
                            state.EntityManager.SetComponentData(e, new LocalTransform
                            {
                                Position = new float3(Random.Range(-spawner.ValueRW.depth + pos.x, spawner.ValueRW.depth + pos.x), trans.ValueRO.Position.y, Random.Range(-spawner.ValueRW.width + pos.z, spawner.ValueRW.width + pos.z)),
                                Rotation = trans.ValueRO.Rotation,
                                Scale = Random.Range(config.minScale, config.maxScale)
                            });
                        }
                        spawner.ValueRW.timer = spawner.ValueRW.delay;

                    }


                    var entityCount = SystemAPI.QueryBuilder().WithAll<ParticleTag>().Build().CalculateEntityCount();

                    foreach (var con in SystemAPI.Query<RefRW<ConfigComp>>())
                    {
                        con.ValueRW.particleAmount = entityCount;
                    }
                    spawner.ValueRW.timer -= Time.deltaTime;
                }
            }
        }
        else if (config.mode == Mode.Scheduled)
        {
            if (config.maxParticlesAmount < config.particleAmount || config.multiplier == 0)
                return;
            else
            {
                var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

                state.Dependency = new spawnParticles
                {
                    ecb = ECB,
                    config = config,
                    Seed = (uint)Time.frameCount

                }.Schedule(state.Dependency);

                var entityCount = SystemAPI.QueryBuilder().WithAll<ParticleTag>().Build().CalculateEntityCount();

                foreach (var con in SystemAPI.Query<RefRW<ConfigComp>>())
                {
                    con.ValueRW.particleAmount = entityCount;
                }
            }

        }
        else if (config.mode == Mode.ScheduledParallel)
        {
            if (config.maxParticlesAmount < config.particleAmount || config.multiplier == 0)
            {
                return;
            }
            else
            {
                var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

                state.Dependency = new spawnParticlesParallel
                {
                    ecb = ECB,
                    config = config,
                    Seed = (uint)Time.frameCount

                }.ScheduleParallel(state.Dependency);

                var entityCount = SystemAPI.QueryBuilder().WithAll<ParticleTag>().Build().CalculateEntityCount();

                foreach (var con in SystemAPI.Query<RefRW<ConfigComp>>())
                {
                    con.ValueRW.particleAmount = entityCount;
                }
            }
        }
    }

    [BurstCompile]
    public partial struct spawnParticles : IJobEntity
    {
        public EntityCommandBuffer ecb;
        public ConfigComp config;
        public uint Seed;


        public void Execute(ref LocalTransform trans, in SpawnerComponent spawner)
        {
            float3 pos = trans.Position;
            Unity.Mathematics.Random random;
            float randomValue = 0;
            for (int i = 0; i < config.multiplier; i++)
            {
                Entity e = ecb.Instantiate(config.prefab);
                randomValue += i + Seed + e.Index;
                random = Unity.Mathematics.Random.CreateFromIndex((uint)randomValue);

                ecb.SetComponent(e, LocalTransform.FromPosition(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + pos.x,
                                           pos.y,
                                          (random.NextFloat() * spawner.width * 2) - spawner.depth + pos.z)));
                ecb.SetComponent(e, new ParticleTag { id = randomValue });
            }
        }
    }

    [BurstCompile]
    public partial struct spawnParticlesParallel : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public ConfigComp config;
        public uint Seed;


        public void Execute([ChunkIndexInQuery] int key, ref LocalTransform trans, in SpawnerComponent spawner)
        {
            float3 pos = trans.Position;
            Unity.Mathematics.Random random;
            float randomValue = 0;
            for (int i = 0; i < config.multiplier; i++)
            {
                Entity e = ecb.Instantiate(key, config.prefab);
                randomValue += i + Seed + e.Index;
                random = Unity.Mathematics.Random.CreateFromIndex((uint)randomValue);

                ecb.SetComponent(key, e, LocalTransform.FromPosition(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + pos.x,
                                           pos.y,
                                          (random.NextFloat() * spawner.width * 2) - spawner.depth + pos.z)));
                ecb.SetComponent(key, e, new ParticleTag { fallen = false, id = randomValue });
            }
        }
    }


}

