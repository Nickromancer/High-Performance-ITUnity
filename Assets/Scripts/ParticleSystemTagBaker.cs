using Unity.Entities;
using UnityEngine;

class ParticleSystemTagBaker : MonoBehaviour
{
    class baker : Baker<ParticleSystemTagBaker>
    {
        public override void Bake(ParticleSystemTagBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ParticleSystemTag>(entity);
        }
    }

}

public struct ParticleSystemTag : IComponentData
{
}