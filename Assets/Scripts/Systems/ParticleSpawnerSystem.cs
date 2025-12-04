using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Pool;
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
        float3 location = 0;

        ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (trans, spawner) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<SpawnerComponent>>())
        {
            location = new float3(trans.ValueRO.Position);
            switch (config.environmentType)
            {
                case snowEnvironment.snowCity:
                    trans.ValueRW.Position = new float3(50f, 90f, -50f);
                    break;

                case snowEnvironment.snowForrest:
                    trans.ValueRW.Position = new float3(-50f, 90f, 50f);
                    break;

                case snowEnvironment.snowMountain:
                    trans.ValueRW.Position = new float3(50f, 90f, 50f);
                    break;

                case snowEnvironment.snowPlane:
                    trans.ValueRW.Position = new float3(-50f, 90f, -50f);
                    break;
                case snowEnvironment.movingSnowCloud:
                    float time = (float)SystemAPI.Time.ElapsedTime / 8;
                    trans.ValueRW.Position = new float3(math.cos(time) * 50f, 90f, math.sin(time) * 50f);
                    break;
            }

        }
        if (config.mode == Mode.MainThread)
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
                foreach (var (particle, entity) in SystemAPI.Query<RefRW<ParticleTag>>().WithEntityAccess())
                {
                    if (particle.ValueRO.fallen)
                    {
                        if (config.maxParticlesAmount < config.particleAmount)
                        {
                            ECB.DestroyEntity(entity);
                        }

                        state.EntityManager.SetComponentData(entity, new LocalTransform
                        {
                            Position = new float3(Random.Range(-spawner.ValueRW.depth + pos.x, spawner.ValueRW.depth + pos.x), trans.ValueRO.Position.y, Random.Range(-spawner.ValueRW.width + pos.z, spawner.ValueRW.width + pos.z)),
                            Rotation = trans.ValueRO.Rotation,
                            Scale = Random.Range(config.minScale, config.maxScale)
                        });

                        state.EntityManager.SetComponentData(entity, new ParticleTag { fallen = false });
                    }
                }

                var entityCount = SystemAPI.QueryBuilder().WithAll<ParticleTag>().Build().CalculateEntityCount();

                foreach (var con in SystemAPI.Query<RefRW<ConfigComp>>())
                {
                    con.ValueRW.particleAmount = entityCount;
                }
                spawner.ValueRW.timer -= Time.deltaTime;
            }
        }
        else if (config.mode == Mode.Scheduled)
        {
            {
                var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
                var spawner = SystemAPI.GetSingleton<SpawnerComponent>();

                state.Dependency = new spawnParticles
                {
                    ecb = ECB,
                    config = config,
                    Seed = (uint)Time.frameCount

                }.Schedule(state.Dependency);

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

        }
        else if (config.mode == Mode.ScheduledParallel)
        {
            {
                var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
                var spawner = SystemAPI.GetSingleton<SpawnerComponent>();

                state.Dependency = new spawnParticlesParallel
                {
                    ecb = ECB,
                    config = config,
                    Seed = (uint)Time.frameCount

                }.Schedule(state.Dependency);

                state.Dependency = new fallenParticlesParallel
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
            {
                float3 pos = trans.Position;
                Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex((uint)Seed);

                for (int i = 0; i < config.multiplier; i++)
                {
                    Entity e = ecb.Instantiate(config.prefab);

                    ecb.AddComponent(e, LocalTransform.FromPosition(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + pos.x,
                                               pos.y,
                                              (random.NextFloat() * spawner.width * 2) - spawner.depth + pos.z)));


                }
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
            Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex((uint)Seed);

            if (particle.fallen)
            {
                if (config.maxParticlesAmount < config.particleAmount)
                {
                    ecb.DestroyEntity(e);
                }

                ecb.SetComponent(e, LocalTransform.FromPosition(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + trans.x,
                                           trans.y,
                                          (random.NextFloat() * spawner.width * 2) - spawner.depth + trans.z)));

                ecb.SetComponent(e, new ParticleTag { fallen = false });
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
            {
                float3 pos = trans.Position;
                Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex((uint)Seed);

                for (int i = 0; i < config.multiplier; i++)
                {
                    Entity e = ecb.Instantiate(key, config.prefab);

                    ecb.AddComponent(key, e, LocalTransform.FromPosition(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + pos.x,
                                               pos.y,
                                              (random.NextFloat() * spawner.width * 2) - spawner.depth + pos.z)));
                }
            }
        }
    }

    public partial struct fallenParticlesParallel : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public ConfigComp config;
        public SpawnerComponent spawner;
        public float3 trans;
        public uint Seed;


        public void Execute([ChunkIndexInQuery] int key, Entity e, ref ParticleTag particle)
        {
            Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex((uint)Seed);

            if (particle.fallen)
            {
                if (config.maxParticlesAmount < config.particleAmount)
                {
                    ecb.DestroyEntity(key, e);
                }

                ecb.SetComponent(key, e, LocalTransform.FromPosition(new float3((random.NextFloat() * spawner.depth * 2) - spawner.depth + trans.x,
                                           trans.y,
                                          (random.NextFloat() * spawner.width * 2) - spawner.depth + trans.z)));

                ecb.SetComponent(key, e, new ParticleTag { fallen = false });
            }
        }
    }
}

