using UnityEngine;
using Unity.Entities;

public class CameraToECS : MonoBehaviour
{
    EntityQuery _cameraQuery;

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
    }

    void Update()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        var camEntity = entityManager.CreateEntityQuery(typeof(CameraComp)).GetSingletonEntity();

        entityManager.SetComponentData(camEntity, new CameraComp
        {
            position = transform.position,
            rotation = transform.up
        });
    }
}
