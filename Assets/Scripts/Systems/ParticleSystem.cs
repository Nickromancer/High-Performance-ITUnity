using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct ParticleSystem : ISystem
{
    private float time;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        time = 0;
        state.RequireForUpdate<ParticleSystemTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        time += Time.deltaTime;
        foreach (var velocity in SystemAPI.Query<RefRW<PhysicsVelocity>>().WithAll<ParticleTag>())
        {
            velocity.ValueRW.Linear = new float3(0, Physics.gravity.y, 0);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
