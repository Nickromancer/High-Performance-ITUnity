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

        if (config.mode == Mode.MainThread)
        {
            foreach (var (velocity, transform) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>>().WithAll<ForceTag>())
            {
                // Example: add upward force
                float3 force = new float3(config.amountOfForceX, config.amountOfForceY, config.amountOfForceZ);

                if (config.doWhirlpool)
                {
                    // Hardcoded speed of spin (radians/second)
                    float whirlpoolSpeed = 2f;

                    // Angle increases over time to make circular movement
                    float angle = (float)SystemAPI.Time.ElapsedTime * whirlpoolSpeed;

                    // Rotate force vector around Y axis
                    float cosA = math.cos(angle);
                    float sinA = math.sin(angle);

                    force.x = force.x * cosA - force.z * sinA;
                    force.z = force.x * sinA + force.z * cosA;
                }
                if (config.doStraightWind)
                {
                    force = new float3(200f, 0f, 0f);
                }
                if (config.doUpdraft)
                {
                    force = new float3(200f, 200f, 0f);
                }

                // Apply to the linear velocity (mass etc. ignored here for simplicity)
                velocity.ValueRW.Linear += force * deltaTime;
            }
        }
        else if (config.mode == Mode.Scheduled)
        {
            var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
            var time = (float)SystemAPI.Time.ElapsedTime;
            var delta = (float)SystemAPI.Time.DeltaTime;
            state.Dependency = new ForceJob
            {
                ecb = ECB,
                config = config,
                time = time,
                delta = delta
            }.Schedule(state.Dependency);
        }
        else if (config.mode == Mode.ScheduledParallel)
        {
            var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var time = (float)SystemAPI.Time.ElapsedTime;
            var delta = (float)SystemAPI.Time.DeltaTime;
            state.Dependency = new ForceJobParallel
            {
                ecb = ECB,
                config = config,
                time = time,
                delta = delta
            }.Schedule(state.Dependency);
        }
    }

    [BurstCompile, WithAny(typeof(ForceTag))]
    public partial struct ForceJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        public ConfigComp config;
        public float time;
        public float delta;
        public void Execute(ref PhysicsVelocity velocity)
        {

            float3 force = new float3(config.amountOfForceX, config.amountOfForceY, config.amountOfForceZ);

            if (config.doWhirlpool)
            {
                // Hardcoded speed of spin (radians/second)
                float whirlpoolSpeed = 2f;

                // Angle increases over time to make circular movement
                float angle = time * whirlpoolSpeed;

                // Rotate force vector around Y axis
                float cosA = math.cos(angle);
                float sinA = math.sin(angle);

                force.x = force.x * cosA - force.z * sinA;
                force.z = force.x * sinA + force.z * cosA;
            }
            if (config.doStraightWind)
            {
                force = new float3(200f, 0f, 0f);
            }
            if (config.doUpdraft)
            {
                force = new float3(200f, 200f, 0f);
            }

            // Apply to the linear velocity (mass etc. ignored here for simplicity)
            velocity.Linear += force * delta;
        }
    }

    [BurstCompile, WithAny(typeof(ForceTag))]
    public partial struct ForceJobParallel : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public ConfigComp config;
        public float time;
        public float delta;
        public void Execute([ChunkIndexInQuery] int key, Entity e, ref PhysicsVelocity velocity)
        {

            float3 force = new float3(config.amountOfForceX, config.amountOfForceY, config.amountOfForceZ);

            if (config.doWhirlpool)
            {
                // Hardcoded speed of spin (radians/second)
                float whirlpoolSpeed = 2f;

                // Angle increases over time to make circular movement
                float angle = time * whirlpoolSpeed;

                // Rotate force vector around Y axis
                float cosA = math.cos(angle);
                float sinA = math.sin(angle);

                force.x = force.x * cosA - force.z * sinA;
                force.z = force.x * sinA + force.z * cosA;
                force.y = Physics.gravity.y;
            }
            if (config.doStraightWind)
            {
                force = new float3(2f, Physics.gravity.y, 0f);
            }
            if (config.doUpdraft)
            {
                force = new float3(2f, 5f, 0f);
            }
            var vel = velocity;
            vel.Linear += force * delta;
            // Apply to the linear velocity (mass etc. ignored here for simplicity)
            ecb.SetComponent(key, e, vel);
        }
    }
}

