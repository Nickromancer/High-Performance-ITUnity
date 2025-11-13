using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct CollisionSystem : ISystem
{
    private ComponentLookup<ParticleTag> particles;
    private ComponentLookup<EnvironmentTag> environment;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComponent>();
        particles = state.GetComponentLookup<ParticleTag>();
        environment = state.GetComponentLookup<EnvironmentTag>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        particles.Update(ref state);
        environment.Update(ref state);

        state.Dependency = new CollisionJob
        {
            allParticles = particles,
            allEnvironments = environment,
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

    public EntityCommandBuffer ECB;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;


        if (allEnvironments.HasComponent(entityB) && allEnvironments.HasComponent(entityA))
        {
            Debug.Log("happening");
            ECB.DestroyEntity(entityA);
        }
        else
        {
            Debug.Log("not happening");

        }
    }
}
