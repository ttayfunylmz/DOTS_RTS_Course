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
        public int index;
        public int x;
        public int y;
        public byte cost;
        public byte bestCost;
        public float2 vector;
    }

    private int2 targetGridPosition;

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
                    index = index,
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

        NativeArray<RefRW<GridNode>> gridNodeNativeArray
            = new NativeArray<RefRW<GridNode>>(gridSystemData.width * gridSystemData.height, Allocator.Temp);

        for(int x = 0; x < gridSystemData.width; ++x)
        {
            for(int y = 0; y < gridSystemData.height; ++y)
            {
                int index = CalculateIndex(x, y, gridSystemData.width);
                Entity gridNodeEntity = gridSystemData.gridMap.gridEntityArray[index];
                RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(gridNodeEntity);

                gridNodeNativeArray[index] = gridNode;

                gridNode.ValueRW.vector = new float2(0, 1);
                if(x == targetGridPosition.x && y == targetGridPosition.y)
                {
                    // THIS IS THE TARGET DESTINATION
                    gridNode.ValueRW.cost = 0;
                    gridNode.ValueRW.bestCost = 0;
                }
                else
                {
                    gridNode.ValueRW.cost = 1;
                    gridNode.ValueRW.bestCost = byte.MaxValue;
                }
            }
        }

        NativeQueue<RefRW<GridNode>> gridNodeOpenQueue = new NativeQueue<RefRW<GridNode>>(Allocator.Temp);
        
        RefRW<GridNode> targetGridNode = gridNodeNativeArray[CalculateIndex(targetGridPosition, gridSystemData.width)];
        gridNodeOpenQueue.Enqueue(targetGridNode);

        int safety = 1000;
        while(gridNodeOpenQueue.Count > 0)
        {
            safety--;
            if(safety < 0)
            {
                Debug.LogError("Safety Break!");
                break;
            }

            RefRW<GridNode> currentGridNode = gridNodeOpenQueue.Dequeue();

            NativeList<RefRW<GridNode>> neighbourGridNodeList
                = GetNeighbourGridNodeList(currentGridNode, gridNodeNativeArray, gridSystemData.width, gridSystemData.height);

            foreach(RefRW<GridNode> neighbourGridNode in neighbourGridNodeList)
            {
                byte newBestCost = (byte)(currentGridNode.ValueRO.bestCost + neighbourGridNode.ValueRO.cost);
                if(newBestCost < neighbourGridNode.ValueRO.bestCost)
                {
                    neighbourGridNode.ValueRW.bestCost = newBestCost;
                    neighbourGridNode.ValueRW.vector = CalculateVector
                    (
                        neighbourGridNode.ValueRO.x, neighbourGridNode.ValueRO.y,
                        currentGridNode.ValueRO.x, currentGridNode.ValueRO.y
                    );

                    gridNodeOpenQueue.Enqueue(neighbourGridNode);
                }
            }

            neighbourGridNodeList.Dispose();
        }

        gridNodeOpenQueue.Dispose();
        gridNodeNativeArray.Dispose();

        if (Input.GetMouseButtonDown(0))
        {
            float3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
            int2 mouseGridPosition = GetGridPosition(mouseWorldPosition, gridSystemData.gridNodeSize);
            if (IsValidGridPosition(mouseGridPosition, gridSystemData.width, gridSystemData.height))
            {
                int index = CalculateIndex(mouseGridPosition.x, mouseGridPosition.y, gridSystemData.width);
                Entity gridNodeEntity = gridSystemData.gridMap.gridEntityArray[index];
                RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(gridNodeEntity);
                
                targetGridPosition = mouseGridPosition;
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

    public static NativeList<RefRW<GridNode>> GetNeighbourGridNodeList(
        RefRW<GridNode> currentGridNode, 
        NativeArray<RefRW<GridNode>> gridNodeNativeArray,
        int width,
        int height)
    {
        NativeList<RefRW<GridNode>> neighbourGridNodeList = new NativeList<RefRW<GridNode>>(Allocator.Temp);

        int gridNodeX = currentGridNode.ValueRW.x;
        int gridNodeY = currentGridNode.ValueRW.y;

        int2 positionLeft = new int2(gridNodeX - 1, gridNodeY + 0);
        int2 positionRight = new int2(gridNodeX + 1, gridNodeY + 0);
        int2 positionUp = new int2(gridNodeX + 0, gridNodeY + 1);
        int2 positionDown = new int2(gridNodeX + 0, gridNodeY - 1);

        int2 positionLowerLeft = new int2(gridNodeX - 1, gridNodeY - 1);
        int2 positionLowerRight = new int2(gridNodeX + 1, gridNodeY - 1);
        int2 positionUpperLeft = new int2(gridNodeX - 1, gridNodeY + 1);
        int2 positionUpperRight = new int2(gridNodeX + 1, gridNodeY + 1);

        if(IsValidGridPosition(positionLeft, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionLeft, width)]);
        }

        if(IsValidGridPosition(positionRight, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionRight, width)]);
        }

        if(IsValidGridPosition(positionUp, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionUp, width)]);
        }

        if(IsValidGridPosition(positionDown, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionDown, width)]);
        }

        if(IsValidGridPosition(positionLowerLeft, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionLowerLeft, width)]);
        }

        if(IsValidGridPosition(positionLowerRight, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionLowerRight, width)]);
        }

        if(IsValidGridPosition(positionUpperLeft, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionUpperLeft, width)]);
        }

        if(IsValidGridPosition(positionUpperRight, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionUpperRight, width)]);
        }

        return neighbourGridNodeList;
    }

    public static float2 CalculateVector(int fromX, int fromY, int toX, int toY)
    {
        return new float2(toX, toY) - new float2(fromX, fromY);
    }

    public static int CalculateIndex(int x, int y, int width)
    {
        return x + y * width;
    }

    public static int CalculateIndex(int2 gridPosition, int width)
    {
        return CalculateIndex(gridPosition.x, gridPosition.y, width);
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
