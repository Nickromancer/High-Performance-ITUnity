using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct CollisionSystem : ISystem
{
    private ComponentLookup<ParticleTag> particles;
    private ComponentLookup<EnvironmentTag> environment;
    public EntityCommandBuffer ECB;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComponent>();
        particles = state.GetComponentLookup<ParticleTag>();
        environment = state.GetComponentLookup<EnvironmentTag>(true);
        ECB = new EntityCommandBuffer(state.WorldUpdateAllocator);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        particles.Update(ref state);
        environment.Update(ref state);
        var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
    .CreateCommandBuffer(state.WorldUnmanaged);

        state.Dependency = new CollisionJob
        {
            allParticles = particles,
            allEnvironments = environment,
            ecb = ECB,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);


    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

[BurstCompile]
public struct CollisionJob : ICollisionEventsJob
{
    public ComponentLookup<ParticleTag> allParticles;
    [ReadOnly] public ComponentLookup<EnvironmentTag> allEnvironments;

    public EntityCommandBuffer ecb;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        if (allParticles.HasComponent(entityA) && allEnvironments.HasComponent(entityB))
        {
            Debug.Log("happening");
            ecb.SetComponent(entityA, new ParticleTag { fallen = true });
        }
        else if (allParticles.HasComponent(entityB))
        {
            Debug.Log("kinda happening");
            ecb.SetComponent(entityA, new ParticleTag { fallen = true });
        }
    }
}
