using TMPro;
using Unity.Entities;
using UnityEngine;

public class ParticleCounter : MonoBehaviour
{
    public TMP_Text text;
    public Entity configObject;
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
        _config = _manager.CreateEntityQuery(typeof(ConfigComp)).GetSingletonEntity();
        text.text = "Particles: " + _manager.GetComponentData<ConfigComp>(_config).particleAmount;
    }
}
