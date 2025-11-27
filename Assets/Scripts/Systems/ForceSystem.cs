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

            if (config.doWhirlpool)
            {
                 // Hardcoded speed of spin (radians/second)
                float whirlpoolSpeed = 2f;

                // Hardcoded strength of the circular force
                float whirlpoolStrength = 10f;

                // Angle increases over time to make circular movement
                float angle = (float)SystemAPI.Time.ElapsedTime * whirlpoolSpeed;

                float x = math.cos(angle);
                float z = math.sin(angle);

                force = new float3(x, 0f, z) * whirlpoolStrength;
            }
            if (config.doStraightWind)
            {

            }
            if (config.doUpdraft)
            {

            }

            // Apply to the linear velocity (mass etc. ignored here for simplicity)
            velocity.ValueRW.Linear += force * deltaTime;

        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
