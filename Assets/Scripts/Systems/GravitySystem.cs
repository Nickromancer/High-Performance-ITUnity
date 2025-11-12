using System.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EntityManagerSimpleSystem))]
public partial struct GravitySystem : ISystem
{
    public int gravityValue;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ManagerSingeltonComponent>();
        foreach (var config in SystemAPI.Query<RefRO<configData>>())
        {
            gravityValue = config.ValueRO.gravity;
        }
    }

    public void OnDestroy(ref SystemState state)
    {
    
    }

    
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        /*
        foreach (var (rot, rotData) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotatingData>>().WithNone<StopRotatingTag>().WithAll<RotateTag>())
        {
            var xRot = quaternion.RotateX( rotData.ValueRO.Value *  Mathf.Deg2Rad * dt);
            
            rot.ValueRW.Rotation = math.mul(rot.ValueRO.Rotation, xRot);
        }
        */

        foreach (var (transform, partData) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ParticleData>>().WithAll<GravityTag>())
        {

            var pos = transform.ValueRO.Position;
            pos.y -= gravityValue * partData.ValueRO.mass * SystemAPI.Time.DeltaTime;

            if (pos.y <= 0f)
            {
                pos.y = 100;
            }

            transform.ValueRW.Position = pos;
        }
    }
}
