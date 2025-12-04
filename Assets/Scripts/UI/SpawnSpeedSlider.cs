using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSpeedSlider : MonoBehaviour
{
    public TMP_Text text;
    private EntityManager _manager;
    private Entity _config;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSlideUpdateSpeed(float sliderValue)
    {
        _config = _manager.CreateEntityQuery(typeof(ConfigComp)).GetSingletonEntity();
        var data = _manager.GetComponentData<ConfigComp>(_config);
        data.multiplier = (int)sliderValue;
        _manager.SetComponentData(_config, data);
        text.text = "Spawn Speed: " + (int)sliderValue;
    }
}
