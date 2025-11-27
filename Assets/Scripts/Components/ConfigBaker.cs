using Unity.Entities;
using UnityEngine;

class ConfigBaker : MonoBehaviour
{
    public GameObject prefab;
    [Range(0, 100000)]
    public int maxParticlesAmount;
    public int particleAmount;


    [Range(-50, 50)]
    public float amountOfForceX;
    [Range(-50, 50)]
    public float amountOfForceY;
    [Range(-50, 50)]
    public float amountOfForceZ;
    public bool doWhirlpool;
    public bool doStraightWind;
    public bool doUpdraft;
}

class ConfigBakerBaker : Baker<ConfigBaker>
{
    public override void Bake(ConfigBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ConfigComp
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            maxParticlesAmount = authoring.maxParticlesAmount,
            amountOfForceX = authoring.amountOfForceX,
            amountOfForceY = authoring.amountOfForceY,
            amountOfForceZ = authoring.amountOfForceZ,
            doWhirlpool = authoring.doWhirlpool,
            doStraightWind = authoring.doStraightWind,
            doUpdraft = authoring.doUpdraft,
        });
    }
}

public struct ConfigComp : IComponentData
{
    public Entity prefab;
    public int maxParticlesAmount;
    public int particleAmount;

    public float amountOfForceX;
    public float amountOfForceY;
    public float amountOfForceZ;
    public bool doWhirlpool;
    public bool doStraightWind;
    public bool doUpdraft;
}
