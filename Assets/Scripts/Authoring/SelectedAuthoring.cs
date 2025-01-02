using Unity.Entities;
using UnityEngine;

public class SelectedAuthoring : MonoBehaviour
{


    public GameObject visualGameObject;
    public float showScale;


    public class Baker : Baker<SelectedAuthoring>
    {

        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected
            {
                visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                showScale = authoring.showScale,
            });
            SetComponentEnabled<Selected>(entity, false);
        }

    }

}


public struct Selected : IComponentData, IEnableableComponent
{

    public Entity visualEntity;
    public float showScale;

    public bool onSelected;
    public bool onDeselected;

}