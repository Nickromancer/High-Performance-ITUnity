using Unity.Entities;
using UnityEngine;

class EnvironmentTagBaker : MonoBehaviour
{

}

class EnvironmentTagBakerBaker : Baker<EnvironmentTagBaker>
{
    public override void Bake(EnvironmentTagBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EnvironmentTag());
    }
}

public struct EnvironmentTag : IComponentData
{
}
