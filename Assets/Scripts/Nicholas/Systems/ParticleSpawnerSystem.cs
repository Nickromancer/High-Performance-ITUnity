using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

partial struct ParticleSpawnerSystem : ISystem
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

        foreach (var (trans, spawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<SpawnerComponent>>())
        {
            float3 pos = trans.ValueRO.Position;
            if (spawner.ValueRW.timer <= 0)
            {
                Entity e = state.EntityManager.Instantiate(config.prefab);


                state.EntityManager.SetComponentData(e, new LocalTransform
                {
                    Position = new float3(Random.Range(-spawner.ValueRW.depth + pos.x, spawner.ValueRW.depth + pos.x), trans.ValueRO.Position.y, Random.Range(-spawner.ValueRW.width + pos.z, spawner.ValueRW.width + pos.z)),
                    Rotation = trans.ValueRO.Rotation,
                    Scale = trans.ValueRO.Scale
                });
                spawner.ValueRW.timer = spawner.ValueRW.delay;


            }
            foreach (var (particle, entity) in SystemAPI.Query<RefRW<ParticleTag>>().WithEntityAccess())
            {
                if (particle.ValueRO.fallen)
                {
                    state.EntityManager.SetComponentData(entity, new LocalTransform
                    {
                        Position = new float3(Random.Range(-spawner.ValueRW.depth + pos.x, spawner.ValueRW.depth + pos.x), trans.ValueRO.Position.y, Random.Range(-spawner.ValueRW.width + pos.z, spawner.ValueRW.width + pos.z)),
                        Rotation = trans.ValueRO.Rotation,
                        Scale = trans.ValueRO.Scale
                    });

                    state.EntityManager.SetComponentData(entity, new ParticleTag { fallen = false });
                }
            }
            spawner.ValueRW.timer -= Time.deltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
