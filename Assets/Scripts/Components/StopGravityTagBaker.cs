using Unity.Entities;
using UnityEngine;

    
public class StopGravityTagBaker : MonoBehaviour
{
    public float timer = 1f; 
    class baker : Baker<StopGravityTagBaker>
    {
        public override void Bake(StopGravityTagBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new StopGravityTag
            {
                timer = authoring.timer
            }); 
        }
    }
}


    public struct StopGravityTag : IComponentData
    {
        public float timer; 
    }

