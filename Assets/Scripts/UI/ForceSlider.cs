using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class ForceSlider : MonoBehaviour
{
    public SliderInputPair forceX;
    public SliderInputPair forceY;
    public SliderInputPair forceZ;

    private EntityManager _manager;
    private EntityQuery _configQuery;
    private Entity _config;

    void Start()
    {

        if (World.DefaultGameObjectInjectionWorld == null)
        {
            Debug.LogWarning("No ECS World exists yet. Waiting.");
            return;
        }

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _configQuery = _manager.CreateEntityQuery(typeof(ConfigComp));

        forceX.Initialize(UpdateConfigVariableForce);
        forceY.Initialize(UpdateConfigVariableForce);
        forceZ.Initialize(UpdateConfigVariableForce);
    }

    public void UpdateConfigVariableForce()
    {
        if (World.DefaultGameObjectInjectionWorld == null)
            return;

        if (!_configQuery.HasSingleton<ConfigComp>())
            return;

        _config = _configQuery.GetSingletonEntity();

        var data = _manager.GetComponentData<ConfigComp>(_config);

        data.amountOfForceX = forceX.value;
        data.amountOfForceY = forceY.value;
        data.amountOfForceZ = forceZ.value;
        

        _manager.SetComponentData(_config, data);
    }
}

[System.Serializable]
public class SliderInputPair
{
    public Slider slider;
    public TMP_InputField input;
    public float value;

    public void Initialize(System.Action onChanged)
    {
        slider.onValueChanged.AddListener(v =>
        {
            value = v;
            input.SetTextWithoutNotify(v.ToString());
            onChanged();
        });

        input.onValueChanged.AddListener(s =>
        {
            if (float.TryParse(s, out float result))
            {
                result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
                value = result;
                slider.SetValueWithoutNotify(result);
                onChanged();
            }
        });

        // Initial sync
        input.text = slider.value.ToString();
        value = slider.value;
    }
}

