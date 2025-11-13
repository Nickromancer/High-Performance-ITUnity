using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class SpawnerBaker : MonoBehaviour
{
    public float width;
    public float depth;
    public float delay;

    public float timer;
}

class SpawnerBakerBaker : Baker<SpawnerBaker>
{
    public override void Bake(SpawnerBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SpawnerComponent
        {
            width = authoring.width,
            depth = authoring.depth,
            delay = authoring.delay,
            timer = authoring.timer
        });
    }
}

public struct SpawnerComponent : IComponentData
{
    public float width;
    public float depth;
    public float delay;

    public float timer;
}
