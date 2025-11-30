using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

class CameraBaker : MonoBehaviour
{
    public float3 position;
    public float3 rotation;
}

class CameraBakerBaker : Baker<CameraBaker>
{
    public override void Bake(CameraBaker authoring)
    {
        var e = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(e, new CameraComp { position = 0, rotation = 0 });
    }
}

struct CameraComp : IComponentData
{
    public float3 position;
    public float3 rotation;
}
