using Unity.Burst;
using Unity.Entities;

partial struct ParticleSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
