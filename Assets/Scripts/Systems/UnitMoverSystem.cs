using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    public const float REACHED_TARGET_POSITION_DISTANCE_SQ = 2f;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSystem.GridSystemData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridSystem.GridSystemData gridSystemData = SystemAPI.GetSingleton<GridSystem.GridSystemData>();

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<TargetPositionPathQueued> targetPositionPathQueued,
            EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled,
            RefRW<FlowFieldPathRequest> flowFieldPathRequest,
            EnabledRefRW<FlowFieldPathRequest> flowFieldPathRequestEnabled,
            RefRW<UnitMover> unitMover)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<TargetPositionPathQueued>,
                    EnabledRefRW<TargetPositionPathQueued>,
                    RefRW<FlowFieldPathRequest>,
                    EnabledRefRW<FlowFieldPathRequest>,
                    RefRW<UnitMover>>().WithPresent<FlowFieldPathRequest>())
        {
            RaycastInput raycastInput = new RaycastInput
            {
                Start = localTransform.ValueRO.Position,
                End = targetPositionPathQueued.ValueRO.targetPosition,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PATHFINDING_WALLS_LAYER,
                    GroupIndex = 0
                }
            };

            if (!collisionWorld.CastRay(raycastInput))
            {
                // DID NOT HIT ANYTHING, NO WALLS IN BETWEEN
                unitMover.ValueRW.targetPosition = targetPositionPathQueued.ValueRO.targetPosition;
            }
            else
            {
                // THERE'S A WALL IN BETWEEN
                flowFieldPathRequest.ValueRW.targetPosition = targetPositionPathQueued.ValueRO.targetPosition;
                flowFieldPathRequestEnabled.ValueRW = true;
            }

            targetPositionPathQueuedEnabled.ValueRW = false;
        }

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<FlowFieldFollower> flowFieldFollower,
            EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled,
            RefRW<UnitMover> unitMover)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<FlowFieldFollower>,
                    EnabledRefRW<FlowFieldFollower>,
                    RefRW<UnitMover>>())
        {
            int2 gridPosition = GridSystem.GetGridPosition(localTransform.ValueRO.Position, gridSystemData.gridNodeSize);
            int index = GridSystem.CalculateIndex(gridPosition, gridSystemData.width);
            Entity gridNodeEntity = gridSystemData.gridMapArray[flowFieldFollower.ValueRW.gridIndex].gridEntityArray[index];
            GridSystem.GridNode gridNode = SystemAPI.GetComponent<GridSystem.GridNode>(gridNodeEntity);
            float3 gridNodeMoveVector = GridSystem.GetWorldMovementVector(gridNode.vector);

            if (GridSystem.IsWall(gridNode))
            {
                gridNodeMoveVector = flowFieldFollower.ValueRO.lastMovedVector;
            }
            else
            {
                flowFieldFollower.ValueRW.lastMovedVector = gridNodeMoveVector;
            }

            unitMover.ValueRW.targetPosition
                = GridSystem.GetWorldCenterPosition(gridPosition.x, gridPosition.y, gridSystemData.gridNodeSize)
                + gridNodeMoveVector * (gridSystemData.gridNodeSize * 2f);

            if (math.distance(localTransform.ValueRO.Position, flowFieldFollower.ValueRO.targetPosition) < gridSystemData.gridNodeSize)
            {
                // TARGET DESTINATION
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                flowFieldFollowerEnabled.ValueRW = false;
            }

            RaycastInput raycastInput = new RaycastInput
            {
                Start = localTransform.ValueRO.Position,
                End = flowFieldFollower.ValueRO.targetPosition,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PATHFINDING_WALLS_LAYER,
                    GroupIndex = 0
                }
            };

            if (!collisionWorld.CastRay(raycastInput))
            {
                // DID NOT HIT ANYTHING, NO WALLS IN BETWEEN
                unitMover.ValueRW.targetPosition = flowFieldFollower.ValueRO.targetPosition;
                flowFieldFollowerEnabled.ValueRW = false;
            }
        }

        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        unitMoverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    public void Execute(ref LocalTransform localTransform,
        ref UnitMover unitMover,
        ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        float reachedTargetDistanceSq = UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ;
        if (math.lengthsq(moveDirection) <= reachedTargetDistanceSq)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            unitMover.isMoving = false;
            return;
        }

        unitMover.isMoving = true;

        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation =
            math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, math.up()),
                unitMover.rotationSpeed * deltaTime);

        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}