using Unity.Entities;
using UnityEngine;

class ConfigBaker : MonoBehaviour
{
    public GameObject prefab;
    public int maxPoolSize;
    [Range(-50, 50)]
    public float amountOfForceX;
    [Range(-50, 50)]
    public float amountOfForceY;
    [Range(-50, 50)]
    public float amountOfForceZ;
}

class ConfigBakerBaker : Baker<ConfigBaker>
{
    public override void Bake(ConfigBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ConfigComp
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            maxPoolSize = authoring.maxPoolSize,
            amountOfForceX = authoring.amountOfForceX,
            amountOfForceY = authoring.amountOfForceY,
            amountOfForceZ = authoring.amountOfForceZ,
        });
    }
}

public struct ConfigComp : IComponentData
{
    public Entity prefab;
    public int maxPoolSize;
    public float amountOfForceX;
    public float amountOfForceY;
    public float amountOfForceZ;
}
