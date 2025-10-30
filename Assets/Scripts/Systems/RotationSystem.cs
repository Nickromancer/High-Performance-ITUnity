using System.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct RotationSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
       state.RequireForUpdate<ManagerSingeltonComponent>();
    }

    public void OnDestroy(ref SystemState state)
    {
    
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (rot, rotData) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotatingData>>().WithNone<StopRotatingTag>().WithAll<RotateTag>())
        {
            var xRot = quaternion.RotateX( rotData.ValueRO.Value *  Mathf.Deg2Rad * dt);
            
            rot.ValueRW.Rotation = math.mul(rot.ValueRO.Rotation, xRot);
        }
        
    }
}
