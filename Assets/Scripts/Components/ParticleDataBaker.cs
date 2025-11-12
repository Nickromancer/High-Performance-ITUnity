using Unity.Entities;
using UnityEngine;

public class ParticleDataBaker : MonoBehaviour
{
    public float mass;
    
    class baker : Baker<ParticleDataBaker>
    {
        public override void Bake(ParticleDataBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent( entity, new ParticleData
            {
                mass = authoring.mass
            });
        }
    }
}

public struct ParticleData : IComponentData, IEnableableComponent
{
    public float mass; 
}