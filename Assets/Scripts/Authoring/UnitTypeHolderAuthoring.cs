using Unity.Entities;
using UnityEngine;

public class UnitTypeHolderAuthoring : MonoBehaviour
{

    public UnitTypeSO.UnitType unitType;


    public class Baker : Baker<UnitTypeHolderAuthoring>
    {


        public override void Bake(UnitTypeHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitTypeHolder
            {
                unitType = authoring.unitType
            });
        }
    }

}



public struct UnitTypeHolder : IComponentData
{


    public UnitTypeSO.UnitType unitType;



}