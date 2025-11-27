using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public partial struct SpawnSystem : ISystem
{
    float timer;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        timer = 1;
        state.RequireForUpdate<ParticleSystemTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<ECBSingletonComponent>();

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            var e = state.EntityManager.Instantiate(ecbSingleton.prefabTospawn);
            float x = (UnityEngine.Random.Range(0, 10) + 500) * 2f;
            float y = 25 * 2f;
            float z = (UnityEngine.Random.Range(0, 10) + 500) * 2f;
            state.EntityManager.SetComponentData(e, LocalTransform.FromPosition(new float3(x, y, z)));
            timer = .1f;
        }
    }
}
