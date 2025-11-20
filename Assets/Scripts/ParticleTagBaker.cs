using Unity.Entities;
using UnityEngine;

class ParticleTagBaker : MonoBehaviour
{
    class baker : Baker<ParticleTagBaker>
    {
        public override void Bake(ParticleTagBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ParticleTag>(entity);
        }
    }

}

public struct ParticleTag : IComponentData
{
    public bool fallen;
}