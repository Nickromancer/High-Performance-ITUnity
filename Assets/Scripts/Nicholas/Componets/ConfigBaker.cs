using Unity.Entities;
using UnityEngine;

class ConfigBaker : MonoBehaviour
{
    public GameObject prefab;
    public int maxPoolSize;
}

class ConfigBakerBaker : Baker<ConfigBaker>
{
    public override void Bake(ConfigBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ConfigComp
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            maxPoolSize = authoring.maxPoolSize
        });
    }
}

public struct ConfigComp : IComponentData
{
    public Entity prefab;
    public int maxPoolSize;
}
