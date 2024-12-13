using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridSystemDebug : MonoBehaviour
{
    public static GridSystemDebug Instance { get; private set; }

    [SerializeField] private Transform debugPrefab;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Sprite arrowSprite;

    private bool isInit;
    private GridSystemDebugSingle[,] gridSystemDebugSingleArray;

    private void Awake() 
    {
        Instance = this;    
    }

    public void InitializeGrid(GridSystem.GridSystemData gridSystemData)
    {
        if(isInit) { return; }
        isInit = true;

        gridSystemDebugSingleArray = new GridSystemDebugSingle[gridSystemData.width, gridSystemData.height];

        for(int x = 0; x < gridSystemData.width; ++x)
        {
            for(int y = 0; y < gridSystemData.height; ++y)
            {
                Transform debugTransform = Instantiate(debugPrefab);
                GridSystemDebugSingle gridSystemDebugSingle = debugTransform.GetComponent<GridSystemDebugSingle>();
                gridSystemDebugSingle.Setup(x, y, gridSystemData.gridNodeSize);

                gridSystemDebugSingleArray[x, y] = gridSystemDebugSingle;
            }
        }
    }

    public void UpdateGrid(GridSystem.GridSystemData gridSystemData)
    {
        for(int x = 0; x < gridSystemData.width; ++x)
        {
            for(int y = 0; y < gridSystemData.height; ++y)
            {
                GridSystemDebugSingle gridSystemDebugSingle = gridSystemDebugSingleArray[x, y];
                
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                int index = GridSystem.CalculateIndex(x, y, gridSystemData.width);
                Entity gridNodeEntity = gridSystemData.gridMap.gridEntityArray[index];
                GridSystem.GridNode gridNode = entityManager.GetComponentData<GridSystem.GridNode>(gridNodeEntity);

                if(gridNode.cost == 0)
                {
                    // THIS IS THE TARGET
                    gridSystemDebugSingle.SetSprite(circleSprite);
                    gridSystemDebugSingle.SetColor(Color.green);
                }
                else
                {
                    gridSystemDebugSingle.SetSprite(arrowSprite);
                    gridSystemDebugSingle.SetColor(Color.white);
                    gridSystemDebugSingle.SetSpriteRotation(
                        Quaternion.LookRotation(new float3(gridNode.vector.x, 0, gridNode.vector.y), Vector3.up));
                }
            }
        }
    }
}
