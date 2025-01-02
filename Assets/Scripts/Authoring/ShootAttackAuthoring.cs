using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{


    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public Transform bulletspawnPositionTransform;


    public class Baker : Baker<ShootAttackAuthoring>
    {


        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
                damageAmount = authoring.damageAmount,
                attackDistance = authoring.attackDistance,
                bulletSpawnLocalPosition = authoring.bulletspawnPositionTransform.localPosition,
            });
        }


    }


}



public struct ShootAttack : IComponentData
{

    public float timer;
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public float3 bulletSpawnLocalPosition;
    public OnShootEvent onShoot;

    public struct OnShootEvent
    {
        public bool isTriggered;
        public float3 shootFromPosition;
    }

}