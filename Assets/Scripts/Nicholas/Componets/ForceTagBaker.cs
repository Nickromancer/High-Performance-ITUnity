using Unity.Entities;
using UnityEngine;


public class ForceTagBaker : MonoBehaviour 
{
       class baker : Baker<ForceTagBaker>
       {
              public override void Bake(ForceTagBaker authoring)
              {
                     var entity = GetEntity(TransformUsageFlags.Dynamic);
                     AddComponent<ForceTag>(entity);
              }
       }
}

public struct ForceTag : IComponentData
{

}
