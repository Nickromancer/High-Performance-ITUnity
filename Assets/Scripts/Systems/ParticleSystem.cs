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
        foreach (var (velocity, trans) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>>().WithAll<ParticleTag>())
        {
            float3 oldVelocity = velocity.ValueRO.Linear;
            velocity.ValueRW.Linear = new float3(oldVelocity.x, Physics.gravity.y + config.amountOfForceY, oldVelocity.z);

            // velocity.ValueRW.Linear.y += 9.81f;

            // Debug.Log(velocity.ValueRW.Linear.y.ToString());

            trans.ValueRW.Rotation = LookAt(trans.ValueRW.Position, cam.position);
        }
    }

    public static Quaternion LookAt(Vector3 sourcePoint, Vector3 destPoint)
    {
        Vector3 forwardVector = Vector3.Normalize(destPoint - sourcePoint);

        float dot = Vector3.Dot(Vector3.forward, forwardVector);

        if (math.abs(dot - (-1.0f)) < 0.000001f)
        {
            return new Quaternion(Vector3.up.x, Vector3.up.y, Vector3.up.z, 3.1415926535897932f);
        }
        if (math.abs(dot - (1.0f)) < 0.000001f)
        {
            return Quaternion.identity;
        }

        float rotAngle = (float)math.acos(dot);
        Vector3 rotAxis = Vector3.Cross(Vector3.forward, forwardVector);
        rotAxis = Vector3.Normalize(rotAxis);
        return CreateFromAxisAngle(rotAxis, rotAngle);
    }

    // just in case you need that function also
    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
    {
        float halfAngle = angle * .5f;
        float s = (float)System.Math.Sin(halfAngle);
        Quaternion q;
        q.x = axis.x * s;
        q.y = axis.y * s;
        q.z = axis.z * s;
        q.w = (float)System.Math.Cos(halfAngle);
        return q;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
