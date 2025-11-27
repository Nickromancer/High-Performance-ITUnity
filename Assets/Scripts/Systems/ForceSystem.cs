using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;


using Random = UnityEngine.Random;

partial struct ForceSystem : ISystem
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
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        
        

        foreach (var (velocity, transform) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>>().WithAll<ForceTag>())
        {
            // Example: add upward force
            float3 force = new float3(config.amountOfForceX, config.amountOfForceY, config.amountOfForceZ);

            // Apply to the linear velocity (mass etc. ignored here for simplicity)
            velocity.ValueRW.Linear += force * deltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
