using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class SpawnAmountSlider : MonoBehaviour
{
    public Slider particleSpawnAmountSlider;
    public TMP_InputField particleSpawnAmountInput;
    public float particleSpawnAmount;

    private EntityManager _manager;
    private EntityQuery _configQuery;
    private Entity _config;

    void Start()
    {
        // Ensure world exists (it may not if ECS hasn't initialized yet)
        if (World.DefaultGameObjectInjectionWorld == null)
        {
            Debug.LogWarning("No ECS World exists yet. SpawnAmountSlider will wait.");
            return;
        }

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Cache query
        _configQuery = _manager.CreateEntityQuery(typeof(ConfigComp));

        // Listeners
        particleSpawnAmountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        particleSpawnAmountInput.onValueChanged.AddListener(OnInputFieldChanged);

        particleSpawnAmountInput.text = particleSpawnAmountSlider.value.ToString();
    }

    public void OnSliderValueChanged(float value)
    {
        particleSpawnAmount = value;

        particleSpawnAmountInput.SetTextWithoutNotify(value.ToString());

        UpdateConfigSpawnAmount();
    }

    public void OnInputFieldChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            result = Mathf.Clamp(result,
                particleSpawnAmountSlider.minValue,
                particleSpawnAmountSlider.maxValue);

            particleSpawnAmount = result;

            particleSpawnAmountSlider.SetValueWithoutNotify(result);
        }

        UpdateConfigSpawnAmount();
    }

    public void UpdateConfigSpawnAmount()
    {
        // If world isn't ready, skip
        if (World.DefaultGameObjectInjectionWorld == null)
            return;

        // Manager is safe now
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Skip if no singleton exists yet
        if (!_configQuery.HasSingleton<ConfigComp>())
            return;

        // Get singleton
        _config = _configQuery.GetSingletonEntity();

        var data = _manager.GetComponentData<ConfigComp>(_config);
        data.maxParticlesAmount = (int)particleSpawnAmount;
        _manager.SetComponentData(_config, data);
    }
}
