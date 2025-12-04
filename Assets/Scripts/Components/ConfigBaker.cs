using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public enum snowEnvironment
{
    snowCity,
    snowForrest,
    snowMountain,
    snowPlane,
    movingSnowCloud
}

public enum Mode
{
    MainThread,
    Scheduled,
    ScheduledParallel
}

public class ConfigBaker : MonoBehaviour
{
    public GameObject prefab;
    [Range(0, 100000)]
    public int maxParticlesAmount;
    public int particleAmount;
    [Range(0, 10)]
    public float minScale;
    [Range(0, 10)]
    public float maxScale;
    [Range(-50, 50)]
    public float amountOfForceX;
    [Range(-50, 50)]
    public float amountOfForceY;
    [Range(-50, 50)]
    public float amountOfForceZ;
    public bool doWhirlpool;
    public bool doStraightWind;
    public bool doUpdraft;
    public int multiplier;
    public Mode mode;



    public snowEnvironment environmentType;

    //var downDown = muEnum.snowCity;
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
            minScale = authoring.minScale,
            maxScale = authoring.maxScale,
            amountOfForceX = authoring.amountOfForceX,
            amountOfForceY = authoring.amountOfForceY,
            amountOfForceZ = authoring.amountOfForceZ,
            doWhirlpool = authoring.doWhirlpool,
            doStraightWind = authoring.doStraightWind,
            doUpdraft = authoring.doUpdraft,
            environmentType = authoring.environmentType,
            multiplier = authoring.multiplier,
            mode = authoring.mode


        });
    }
}

public struct ConfigComp : IComponentData
{
    public Entity prefab;
    public int maxParticlesAmount;
    public int particleAmount;
    public float minScale;
    public float maxScale;

    public float amountOfForceX;
    public float amountOfForceY;
    public float amountOfForceZ;
    public bool doWhirlpool;
    public bool doStraightWind;
    public bool doUpdraft;
    public snowEnvironment environmentType;
    public int multiplier;
    public Mode mode;


}
