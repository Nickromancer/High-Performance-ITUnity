using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


    public partial struct EntityManagerSimpleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ManagerSingeltonComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var singleton = SystemAPI.GetSingleton<ManagerSingeltonComponent>();
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            //if (singleton.ExampleType != EntityManagerExample.Simple) return;
            
            foreach (var (singelton, entity) in SystemAPI.Query<RefRW<ManagerSingeltonComponent>>().WithEntityAccess())
            {

                    int n = singelton.ValueRO.spawnAmount; 
                    for (int i = 0; i < n*n*n ; i++)
                    {
                            var e = state.EntityManager.Instantiate(singelton.ValueRO.prefabToSpawn);
                            float x = (i % n) * 2f;
                            float y = ((i / n) % n) * 2f;
                            float z = (i / (n * n)) * 2f;
                
                            state.EntityManager.SetComponentData(e, LocalTransform.FromPosition(new float3(x, y, z)));
                    }; 
                
                
                /*if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Entity e = state.EntityManager.Instantiate(singelton.ValueRO.prefabToSpawn);
                    state.EntityManager.SetComponentData(e, new LocalTransform
                    {
                        Position = new float3(0, 1, 0),
                        Rotation = quaternion.identity,
                        Scale = 1
                    });
                }*/
            }
            
            foreach (var (trans, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess().WithAll<RotateTag>())
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ecb.RemoveComponent<RotateTag>(entity);
                }
         
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    trans.ValueRW.Scale = trans.ValueRO.Scale + 1; 
                }
            }
            foreach (var (trans, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithEntityAccess().WithNone<RotateTag>().WithAll<RotatingData>())
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ecb.AddComponent<RotateTag>(entity);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ecb.DestroyEntity(entity);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }

