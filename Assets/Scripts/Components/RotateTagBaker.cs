using Unity.Entities;
using UnityEngine;


public class RotateTagBaker : MonoBehaviour 
{
       class baker : Baker<RotateTagBaker>
       {
              public override void Bake(RotateTagBaker authoring)
              {
                     var entity = GetEntity(TransformUsageFlags.Dynamic);
                     AddComponent<RotateTag>(entity);
              }
       }
}

public struct RotateTag : IComponentData
{

}
