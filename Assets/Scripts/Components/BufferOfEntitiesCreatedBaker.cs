using Unity.Entities;
using UnityEngine;

public class BufferOfEntitiesCreatedBaker : MonoBehaviour
{
    
    class baker : Baker<BufferOfEntitiesCreatedBaker>
    {
        public override void Bake(BufferOfEntitiesCreatedBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<ListOfEntitiesCreatedComponent>(entity);
        }
    }
}

public struct ListOfEntitiesCreatedComponent : IBufferElementData
{
    public Entity entity; 
}


