#define GRID_DEBUG

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct GridSystem : ISystem
{
    public struct GridSystemData : IComponentData
    {
        public int width;
        public int height;
        public float gridNodeSize;
        public GridMap gridMap;
    }

    public struct GridMap
    {
        public NativeArray<Entity> gridEntityArray;
    }

    public struct GridNode : IComponentData
    {
        public int x;
        public int y;
        public byte data;
    }

#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnCreate(ref SystemState state)
    {
        int width = 20;
        int height = 10;
        float gridNodeSize = 5f;
        int totalCount = width * height;

        Entity gridNodeEntityPrefab = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<GridNode>(gridNodeEntityPrefab);

        GridMap gridMap = new GridMap();
        gridMap.gridEntityArray = new NativeArray<Entity>(totalCount, Allocator.Persistent);

        state.EntityManager.Instantiate(gridNodeEntityPrefab, gridMap.gridEntityArray);

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                int index = CalculateIndex(x, y, width);
                GridNode gridNode = new GridNode
                {
                    x = x,
                    y = y
                };
#if GRID_DEBUG
                state.EntityManager.SetName(gridMap.gridEntityArray[index], "GridNode_" + x + "_" + y);
#endif
                SystemAPI.SetComponent(gridMap.gridEntityArray[index], gridNode);
            }
        }

        state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
        state.EntityManager.SetComponentData(state.SystemHandle, new GridSystemData
        {
            width = width,
            height = height,
            gridNodeSize = gridNodeSize,
            gridMap = gridMap
        });
    }

#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnUpdate(ref SystemState state)
    {
        GridSystemData gridSystemData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);

        if (Input.GetMouseButtonDown(0))
        {
            float3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
            int2 mouseGridPosition = GetGridPosition(mouseWorldPosition, gridSystemData.gridNodeSize);
            if (IsValidGridPosition(mouseGridPosition, gridSystemData.width, gridSystemData.height))
            {
                int index = CalculateIndex(mouseGridPosition.x, mouseGridPosition.y, gridSystemData.width);
                Entity gridNodeEntity = gridSystemData.gridMap.gridEntityArray[index];
                RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(gridNodeEntity);
                gridNode.ValueRW.data = 1;
            }
        }

#if GRID_DEBUG
        GridSystemDebug.Instance?.InitializeGrid(gridSystemData);
        GridSystemDebug.Instance?.UpdateGrid(gridSystemData);
#endif
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        RefRW<GridSystemData> gridSystemData = SystemAPI.GetComponentRW<GridSystemData>(state.SystemHandle);
        gridSystemData.ValueRW.gridMap.gridEntityArray.Dispose();
    }


    public static int CalculateIndex(int x, int y, int width)
    {
        return x + y * width;
    }

    public static float3 GetWorldPosition(int x, int y, float gridNodeSize)
    {
        return new float3
        (
            x * gridNodeSize,
            0f,
            y * gridNodeSize
        );
    }

    public static int2 GetGridPosition(float3 worldPosition, float gridNodeSize)
    {
        return new int2
        (
            (int)math.floor(worldPosition.x / gridNodeSize),
            (int)math.floor(worldPosition.z / gridNodeSize)
        );
    }

    public static bool IsValidGridPosition(int2 gridPosition, int width, int height)
    {
        return
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < width &&
            gridPosition.y < height;
    }
}
