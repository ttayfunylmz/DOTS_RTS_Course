using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomHolderAuthoring : MonoBehaviour
{

    public class Baker : Baker<RandomHolderAuthoring>
    {

        public override void Bake(RandomHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomHolder
            {
                random = new Unity.Mathematics.Random((uint)entity.Index)
            });
        }
    }
}



public struct RandomHolder : IComponentData
{

    public Unity.Mathematics.Random random;

}