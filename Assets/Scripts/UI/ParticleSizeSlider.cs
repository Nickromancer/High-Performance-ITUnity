using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class SpawnSizeSlider : MonoBehaviour
{
    // --- Your two pairs ---
    public SliderInputPair particleSizeMin;
    public SliderInputPair particleSizeMax;

    private EntityManager _manager;
    private EntityQuery _configQuery;
    private Entity _config;

    void Start()
    {
        // ECS world check
        if (World.DefaultGameObjectInjectionWorld == null)
        {
            Debug.LogWarning("No ECS World exists yet. Waiting.");
            return;
        }

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _configQuery = _manager.CreateEntityQuery(typeof(ConfigComp));

        // Initialize both pairs
        particleSizeMin.Initialize(UpdateConfigVariableParticleSize);
        particleSizeMax.Initialize(UpdateConfigVariableParticleSize);
    }

    public void UpdateConfigVariableParticleSize()
    {
        if (World.DefaultGameObjectInjectionWorld == null)
            return;

        if (!_configQuery.HasSingleton<ConfigComp>())
            return;

        _config = _configQuery.GetSingletonEntity();

        var data = _manager.GetComponentData<ConfigComp>(_config);

        // Apply values from both slider/input pairs
        data.minScale = particleSizeMin.value;
        data.maxScale = particleSizeMax.value;

        _manager.SetComponentData(_config, data);
    }
}


