using UnityEngine;

public class ParticleAmountSingleton : MonoBehaviour
{
    public static ParticleAmountSingleton instance;
    public float amount;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
