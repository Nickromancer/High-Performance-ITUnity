using Unity.Entities;
using UnityEngine;

public class RotatingDataBaker : MonoBehaviour
{
    public float rotatingValue;

    class baker : Baker<RotatingDataBaker>
    {
        public override void Bake(RotatingDataBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RotatingData
            {
                Value = authoring.rotatingValue
            });
        }
    }
}

public struct RotatingData : IComponentData, IEnableableComponent
{
    public float Value;
}