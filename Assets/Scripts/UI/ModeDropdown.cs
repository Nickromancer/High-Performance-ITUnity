using TMPro;
using UnityEngine;
using Unity.Entities;

public class ModeDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private EntityManager _manager;
    private EntityQuery _configQuery;
    private Entity _config;

    void Start()
    {
        if (World.DefaultGameObjectInjectionWorld == null)
        {
            Debug.LogWarning("No ECS World exists yet. EnvironmentDropdown will wait.");
            return;
        }

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _configQuery = _manager.CreateEntityQuery(typeof(ConfigComp));

        // Listen for changes
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int index)
    {
        UpdateConfigEnvironment(index);
    }

    private void UpdateConfigEnvironment(int selectedIndex)
    {
        if (World.DefaultGameObjectInjectionWorld == null)
            return;

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (!_configQuery.HasSingleton<ConfigComp>())
            return;

        _config = _configQuery.GetSingletonEntity();

        var data = _manager.GetComponentData<ConfigComp>(_config);

        // Convert dropdown index → enum
        data.mode = (Mode)selectedIndex;

        _manager.SetComponentData(_config, data);
    }
}

