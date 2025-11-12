using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class SpawnerBaker : MonoBehaviour
{

}

class SpawnerBakerBaker : Baker<SpawnerBaker>
{
    public override void Bake(SpawnerBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SpawnerTag());
    }
}

public struct SpawnerTag : IComponentData
{
}
