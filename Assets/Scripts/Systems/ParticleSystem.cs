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
        state.RequireForUpdate<SpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComp>();
        var cam = SystemAPI.GetSingleton<CameraComp>();
        time += Time.deltaTime;

        if (config.mode == Mode.MainThread)
        {

            foreach (var trans in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<ParticleTag>())
            {
                float4 quat = default;
                LookAt(trans.ValueRW.Position, cam.position, ref quat);
                trans.ValueRW.Rotation = new quaternion(quat);
            }
        }
        else if (config.mode == Mode.Scheduled)
        {
            var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

            state.Dependency = new RotateAndGravityParticles
            {
                ecb = ECB,
                cam = cam,

            }.Schedule(state.Dependency);
        }
        else if (config.mode == Mode.ScheduledParallel)
        {
            var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            state.Dependency = new RotateAndGravityParticlesParallel
            {
                ecb = ECB,
                cam = cam,

            }.Schedule(state.Dependency);
        }
    }

    [BurstCompile]
    public static void LookAt(in float3 sourcePoint, in float3 destPoint, ref float4 result)
    {
        float3 forwardVector = math.normalize(destPoint - sourcePoint);

        float dot = math.dot(new float3(0, 0, 1), forwardVector);

        if (math.abs(dot - (-1.0f)) < 0.000001f)
        {
            result = new float4(0, 1, 0, 3.1415926535897932f);
            return;
        }
        if (math.abs(dot - (1.0f)) < 0.000001f)
        {
            result = new float4(0, 0, 0, 1);
            return;
        }

        float rotAngle = (float)math.acos(dot);
        float3 rotAxis = math.cross(new float3(0, 0, 1), forwardVector);
        rotAxis = math.normalize(rotAxis);
        CreateFromAxisAngle(rotAxis, rotAngle, ref result);
    }

    // just in case you need that function also
    [BurstCompile]
    public static void CreateFromAxisAngle(in float3 axis, float angle, ref float4 result)
    {
        float halfAngle = angle * .5f;
        float s = (float)math.sin(halfAngle);
        result.x = axis.x * s;
        result.y = axis.y * s;
        result.z = axis.z * s;
        result.w = (float)math.cos(halfAngle);
    }


    [BurstCompile, WithAll(typeof(ParticleTag))]
    public partial struct RotateAndGravityParticles : IJobEntity
    {
        public EntityCommandBuffer ecb;
        public CameraComp cam;

        public void Execute(Entity e, ref LocalTransform trans)
        {
            float4 quat = default;
            LookAt(trans.Position, cam.position, ref quat);
            trans.Rotation = new quaternion(quat);
        }
    }

    [BurstCompile, WithAll(typeof(ParticleTag))]
    public partial struct RotateAndGravityParticlesParallel : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public CameraComp cam;

        public void Execute([ChunkIndexInQuery] int key, Entity e, ref LocalTransform trans)
        {
            float4 quat = default;
            LookAt(trans.Position, cam.position, ref quat);
            trans.Rotation = new quaternion(quat);
        }
    }
}
