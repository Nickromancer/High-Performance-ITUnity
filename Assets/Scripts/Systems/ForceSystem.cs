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
            float2 force = new float2(config.amountOfForceX, config.amountOfForceZ);

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

                force = new float2(x, z) * whirlpoolStrength;
            }
            if (config.doStraightWind)
            {

            }
            if (config.doUpdraft)
            {

            }

            // Apply to the linear velocity (mass etc. ignored here for simplicity)
            velocity.ValueRW.Linear.xz += force.xy * deltaTime;
            // velocity.ValueRW.Linear.z += force * deltaTime;
            // velocity.ValueRW.y += force.y * deltaTime;

        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
