using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct GridSystem : ISystem
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

    [BurstCompile]
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

        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                int index = CalculateIndex(x, y, width);
                GridNode gridNode = new GridNode
                {
                    x = x,
                    y = y
                };

                state.EntityManager.SetName(gridMap.gridEntityArray[index], "GridNode_" + x + "_" + y);
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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridSystemData gridSystemData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);

        if(Input.GetKeyDown(KeyCode.T))
        {
            int index = CalculateIndex(3, 2, gridSystemData.width);
            Entity gridNodeEntity = gridSystemData.gridMap.gridEntityArray[index];
            RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(gridNodeEntity);
            gridNode.ValueRW.data = 1;
        }
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
}
