using Unity.Entities;
using UnityEngine;


public class GravityTagBaker : MonoBehaviour 
{
       class baker : Baker<GravityTagBaker>
       {
              public override void Bake(GravityTagBaker authoring)
              {
                     var entity = GetEntity(TransformUsageFlags.Dynamic);
                     AddComponent<GravityTag>(entity);
              }
       }
}

public struct GravityTag : IComponentData
{

}
