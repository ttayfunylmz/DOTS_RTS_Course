using Unity.Entities;
using UnityEngine;

public class EnemyAttackHQAuthoring : MonoBehaviour
{
    public class Baker : Baker<EnemyAttackHQAuthoring>
    {
        public override void Bake(EnemyAttackHQAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyAttackHQ
            {
                
            });
        }
    }
}

public struct EnemyAttackHQ : IComponentData
{

}