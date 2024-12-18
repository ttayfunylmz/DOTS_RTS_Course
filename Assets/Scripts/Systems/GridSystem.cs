#define GRID_DEBUG

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public partial struct GridSystem : ISystem
{
    public const int WALL_COST = byte.MaxValue;
    public const int FLOW_FIELD_MAP_COUNT = 50;

    public struct GridSystemData : IComponentData
    {
        public int width;
        public int height;
        public float gridNodeSize;
        public NativeArray<GridMap> gridMapArray;
        public int nextGridIndex;
    }

    public struct GridMap
    {
        public NativeArray<Entity> gridEntityArray;
        public int2 targetGridPosition;
        public bool isValid;
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

        NativeArray<GridMap> gridMapArray = new NativeArray<GridMap>(FLOW_FIELD_MAP_COUNT, Allocator.Persistent);

        for (int i = 0; i < FLOW_FIELD_MAP_COUNT; ++i)
        {
            GridMap gridMap = new GridMap();
            gridMap.isValid = false;
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

            gridMapArray[i] = gridMap;
        }


        state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
        state.EntityManager.SetComponentData(state.SystemHandle, new GridSystemData
        {
            width = width,
            height = height,
            gridNodeSize = gridNodeSize,
            gridMapArray = gridMapArray
        });
    }

#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnUpdate(ref SystemState state)
    {
        GridSystemData gridSystemData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);

        foreach ((
            RefRW<FlowFieldPathRequest> flowFieldPathRequest,
            EnabledRefRW<FlowFieldPathRequest> flowFieldPathRequestEnabled,
            RefRW<FlowFieldFollower> flowFieldFollower,
            EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled)
                in SystemAPI.Query<
                    RefRW<FlowFieldPathRequest>,
                    EnabledRefRW<FlowFieldPathRequest>,
                    RefRW<FlowFieldFollower>,
                    EnabledRefRW<FlowFieldFollower>>().WithPresent<FlowFieldFollower>())
        {
            int2 targetGridPosition = GetGridPosition(flowFieldPathRequest.ValueRO.targetPosition, gridSystemData.gridNodeSize);

            flowFieldPathRequestEnabled.ValueRW = false;

            bool alreadyCalculatedPath = false;
            for(int i = 0; i < FLOW_FIELD_MAP_COUNT; ++i)
            {
                if(gridSystemData.gridMapArray[i].isValid && gridSystemData.gridMapArray[i].targetGridPosition.Equals(targetGridPosition))
                {
                    // ALREADY CALCULATED PATH TO THE EXACT SAME TARGET GRID POSITION
                    flowFieldFollower.ValueRW.gridIndex = i;
                    flowFieldFollower.ValueRW.targetPosition = flowFieldPathRequest.ValueRO.targetPosition;
                    flowFieldFollowerEnabled.ValueRW = true;

                    alreadyCalculatedPath = true;
                    break;
                }
            }

            if(alreadyCalculatedPath) { continue; }

            int gridIndex = gridSystemData.nextGridIndex;
            gridSystemData.nextGridIndex = (gridSystemData.nextGridIndex + 1) % FLOW_FIELD_MAP_COUNT;
            SystemAPI.SetComponent(state.SystemHandle, gridSystemData);

            Debug.Log("Calculating Path to " + targetGridPosition + " :: " + gridIndex);
            flowFieldFollower.ValueRW.gridIndex = gridIndex;
            flowFieldFollower.ValueRW.targetPosition = flowFieldPathRequest.ValueRO.targetPosition;
            flowFieldFollowerEnabled.ValueRW = true;

            NativeArray<RefRW<GridNode>> gridNodeNativeArray
                = new NativeArray<RefRW<GridNode>>(gridSystemData.width * gridSystemData.height, Allocator.Temp);

            for (int x = 0; x < gridSystemData.width; ++x)
            {
                for (int y = 0; y < gridSystemData.height; ++y)
                {
                    int index = CalculateIndex(x, y, gridSystemData.width);
                    Entity gridNodeEntity = gridSystemData.gridMapArray[gridIndex].gridEntityArray[index];
                    RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(gridNodeEntity);

                    gridNodeNativeArray[index] = gridNode;

                    gridNode.ValueRW.vector = new float2(0, 1);
                    if (x == targetGridPosition.x && y == targetGridPosition.y)
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

            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

            for (int x = 0; x < gridSystemData.width; ++x)
            {
                for (int y = 0; y < gridSystemData.height; ++y)
                {
                    if (collisionWorld.OverlapSphere(
                        GetWorldCenterPosition(x, y, gridSystemData.gridNodeSize),
                        gridSystemData.gridNodeSize * .5f,
                        ref distanceHitList,
                        new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << GameAssets.PATHFINDING_WALLS_LAYER,
                            GroupIndex = 0
                        }))
                    {
                        // THERE IS A WALL
                        int index = CalculateIndex(x, y, gridSystemData.width);
                        gridNodeNativeArray[index].ValueRW.cost = WALL_COST;
                    }
                }
            }
            distanceHitList.Dispose();

            NativeQueue<RefRW<GridNode>> gridNodeOpenQueue = new NativeQueue<RefRW<GridNode>>(Allocator.Temp);

            RefRW<GridNode> targetGridNode = gridNodeNativeArray[CalculateIndex(targetGridPosition, gridSystemData.width)];
            gridNodeOpenQueue.Enqueue(targetGridNode);

            int safety = 1000;
            while (gridNodeOpenQueue.Count > 0)
            {
                safety--;
                if (safety < 0)
                {
                    Debug.LogError("Safety Break!");
                    break;
                }

                RefRW<GridNode> currentGridNode = gridNodeOpenQueue.Dequeue();

                NativeList<RefRW<GridNode>> neighbourGridNodeList
                    = GetNeighbourGridNodeList(currentGridNode, gridNodeNativeArray, gridSystemData.width, gridSystemData.height);

                foreach (RefRW<GridNode> neighbourGridNode in neighbourGridNodeList)
                {
                    if (neighbourGridNode.ValueRO.cost == WALL_COST)
                    {
                        // THIS IS A WALL
                        continue;
                    }

                    byte newBestCost = (byte)(currentGridNode.ValueRO.bestCost + neighbourGridNode.ValueRO.cost);
                    if (newBestCost < neighbourGridNode.ValueRO.bestCost)
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

            GridMap gridMap = gridSystemData.gridMapArray[gridIndex];
            gridMap.targetGridPosition = targetGridPosition;
            gridMap.isValid = true;
            gridSystemData.gridMapArray[gridIndex] = gridMap;
            SystemAPI.SetComponent(state.SystemHandle, gridSystemData);
        }

        if (Input.GetMouseButtonDown(0))
        {
            float3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
            int2 mouseGridPosition = GetGridPosition(mouseWorldPosition, gridSystemData.gridNodeSize);
            if (IsValidGridPosition(mouseGridPosition, gridSystemData.width, gridSystemData.height))
            {
                // int index = CalculateIndex(mouseGridPosition.x, mouseGridPosition.y, gridSystemData.width);
                // Entity gridNodeEntity = gridSystemData.gridMap.gridEntityArray[index];
                // RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(gridNodeEntity);
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
        
        for(int i = 0; i < FLOW_FIELD_MAP_COUNT; ++i)
        {
            gridSystemData.ValueRW.gridMapArray[i].gridEntityArray.Dispose();
        }

        gridSystemData.ValueRW.gridMapArray.Dispose();
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

        if (IsValidGridPosition(positionLeft, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionLeft, width)]);
        }

        if (IsValidGridPosition(positionRight, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionRight, width)]);
        }

        if (IsValidGridPosition(positionUp, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionUp, width)]);
        }

        if (IsValidGridPosition(positionDown, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionDown, width)]);
        }

        if (IsValidGridPosition(positionLowerLeft, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionLowerLeft, width)]);
        }

        if (IsValidGridPosition(positionLowerRight, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionLowerRight, width)]);
        }

        if (IsValidGridPosition(positionUpperLeft, width, height))
        {
            neighbourGridNodeList.Add(gridNodeNativeArray[CalculateIndex(positionUpperLeft, width)]);
        }

        if (IsValidGridPosition(positionUpperRight, width, height))
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

    public static float3 GetWorldCenterPosition(int x, int y, float gridNodeSize)
    {
        return new float3
        (
            x * gridNodeSize + gridNodeSize * 0.5f,
            0f,
            y * gridNodeSize + gridNodeSize * 0.5f
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

    public static float3 GetWorldMovementVector(float2 vector)
    {
        return new float3(vector.x, 0f, vector.y);
    }

    public static bool IsWall(GridNode gridNode)
    {
        return gridNode.cost == WALL_COST;
    }
}
