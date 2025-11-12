using Unity.Entities;
using UnityEngine;

public class configDataBaker : MonoBehaviour
{
    public int gravityValue;
    public float minMass;
    public float maxMass;
    
    class baker : Baker<configDataBaker>
    {
        public override void Bake(configDataBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent( entity, new configData
            {
                gravity = authoring.gravityValue,
                minMass = authoring.minMass,
                maxMass = authoring.maxMass,
            });
        }
    }
}

public struct configData : IComponentData, IEnableableComponent
{
    public int gravity; 
    public float minMass;
    public float maxMass;
}