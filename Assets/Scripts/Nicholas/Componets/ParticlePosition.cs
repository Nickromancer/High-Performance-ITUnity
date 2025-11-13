using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(0)]
public struct ParticlePosition : IBufferElementData
{
    public float3 Value;
}
