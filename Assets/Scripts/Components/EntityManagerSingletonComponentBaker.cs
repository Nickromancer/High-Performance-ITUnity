using Unity.Entities;
using UnityEngine;

public class EntityManagerSingletonComponentBaker : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public bool hasBuffer;
    public int spawnAmount;

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
            });

        }
    }
}


public struct ManagerSingeltonComponent : IComponentData
{
    public Entity prefabToSpawn;
    public bool hasBuffer;
    public int spawnAmount;
} 

