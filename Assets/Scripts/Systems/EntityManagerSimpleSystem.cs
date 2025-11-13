using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;

using Random = UnityEngine.Random;

public partial struct EntityManagerSimpleSystem : ISystem
{
    public float minMass;
    public float maxMass;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ManagerSingeltonComponent>();

        foreach (var config in SystemAPI.Query<RefRO<configData>>())
        {
            minMass = config.ValueRO.minMass;
            maxMass = config.ValueRO.maxMass;
        }
    }

    public void OnUpdate(ref SystemState state)
    {
        /*             state.Enabled = false;
         */
        var singleton = SystemAPI.GetSingleton<ManagerSingeltonComponent>();
        //var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        //if (singleton.ExampleType != EntityManagerExample.Simple) return;


        foreach (var (singelton, entity) in SystemAPI.Query<RefRW<ManagerSingeltonComponent>>().WithEntityAccess())
        {

            int n = singelton.ValueRO.spawnAmount;
            int width = singelton.ValueRO.width;
            int length = singelton.ValueRO.length;

            for (int i = 0; i < n; i++)
            {
                var e = state.EntityManager.Instantiate(singelton.ValueRO.prefabToSpawn);
                float x = Random.Range(0, width);
                float y = 100;
                float z = Random.Range(0, length);

                float scale = Random.Range(1, 4);
                //float3 scale = new float3(Random.Range(1f,3f),Random.Range(1f,3f),Random.Range(1f,3f));





                state.EntityManager.SetComponentData(e, LocalTransform.FromPositionRotationScale(new float3(x, y, z), quaternion.identity, scale));


                //state.EntityManager.SetComponentData(e, LocalTransform.FromScale(scale));
            }

            // Version that spawns in a cube of cubes
            /*
            int n = singelton.ValueRO.spawnAmount; 
            for (int i = 0; i < n*n*n ; i++)
            {
                    var e = state.EntityManager.Instantiate(singelton.ValueRO.prefabToSpawn);
                    float x = (i % n) * 2f;
                    float y = ((i / n) % n) * 2f;
                    float z = (i / (n * n)) * 2f;

                    state.EntityManager.SetComponentData(e, LocalTransform.FromPosition(new float3(x, y, z)));
            }; 
            */

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

        foreach (var entity in SystemAPI.Query<RefRW<PhysicsMass>>())
        {
            entity.ValueRW.InverseMass = 1f / Random.Range(minMass, maxMass);
        }

        //ecb.Playback(state.EntityManager);
    }
}

