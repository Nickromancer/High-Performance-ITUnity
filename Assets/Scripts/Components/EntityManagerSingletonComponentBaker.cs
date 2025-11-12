using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EntityManagerSingletonComponentBaker : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public bool hasBuffer;
    [Range(1, 10000)]
    public int spawnAmount;

    [Range(0, 200)]
    public int width;

    [Range(0, 200)]
    public int length;

    class baker : Baker<EntityManagerSingletonComponentBaker>
    {
        public override void Bake(EntityManagerSingletonComponentBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ManagerSingeltonComponent
            {
                prefabToSpawn = GetEntity(authoring.prefabToSpawn, TransformUsageFlags.Dynamic),
                hasBuffer = authoring.hasBuffer,
                spawnAmount = authoring.spawnAmount,
                width = authoring.width,
                length = authoring.length,
            });

        }
    }
}


public struct ManagerSingeltonComponent : IComponentData
{
    public Entity prefabToSpawn;
    public bool hasBuffer;
    public int spawnAmount;
    public int width;
    public int length;
} 

