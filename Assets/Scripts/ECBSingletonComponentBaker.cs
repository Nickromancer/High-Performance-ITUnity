using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public class ECBSingletonComponentBaker : MonoBehaviour
{
    public SchedulingType SchedulingType;
    [Header("Toggles")]
    public bool shouldDestroy;
    public bool shouldRotate;
    [Header("Variable state")]
    [Range(1, 200)]
    public int spawnAmount;
    public GameObject prefabToSpawn;
    class baker : Baker<ECBSingletonComponentBaker>
    {
        public override void Bake(ECBSingletonComponentBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ECBSingletonComponent
            {
                SchedulingType = authoring.SchedulingType,
                shouldDestroy = authoring.shouldDestroy,
                shouldRotate = authoring.shouldRotate,
                spawnAmount = authoring.spawnAmount,
                prefabTospawn = GetEntity(authoring.prefabToSpawn, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct ECBSingletonComponent : IComponentData
{
    public SchedulingType SchedulingType;
    public bool shouldDestroy;
    public bool shouldRotate;
    public int spawnAmount;
    public Entity prefabTospawn;
}

public enum SchedulingType
{
    Run,
    Schedule,
    ScheduleParallel,
    ScheduleParallelEnable
}