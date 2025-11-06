using Unity.Entities;
using UnityEngine;

    
public class StopRotatingTagBaker : MonoBehaviour
{
    public float timer = 1f; 
    class baker : Baker<StopRotatingTagBaker>
    {
        public override void Bake(StopRotatingTagBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new StopRotatingTag
            {
                timer = authoring.timer
            }); 
        }
    }
}


    public struct StopRotatingTag : IComponentData
    {
        public float timer; 
    }

