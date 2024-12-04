using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(ShootAttackSystem))]
partial struct AnimationStateSystem : ISystem
{
    private ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        activeAnimationComponentLookup = state.GetComponentLookup<ActiveAnimation>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        activeAnimationComponentLookup.Update(ref state);
        IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new IdleWalkingAnimationStateJob
        {
            activeAnimationComponentLookup = activeAnimationComponentLookup
        };
        idleWalkingAnimationStateJob.ScheduleParallel();

        activeAnimationComponentLookup.Update(ref state);
        AimShootAnimationStateJob aimShootAnimationStateJob = new AimShootAnimationStateJob
        {
            activeAnimationComponentLookup = activeAnimationComponentLookup
        };
        aimShootAnimationStateJob.ScheduleParallel();

        activeAnimationComponentLookup.Update(ref state);
        MeleeAttackAnimationStateJob meleeAttackAnimationStateJob = new MeleeAttackAnimationStateJob
        {
            activeAnimationComponentLookup = activeAnimationComponentLookup
        };
        meleeAttackAnimationStateJob.ScheduleParallel();
    }
}

public partial struct IdleWalkingAnimationStateJob : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    public void Execute(in AnimatedMesh animatedMesh, in UnitMover unitMover, in UnitAnimations unitAnimations)
    {
        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);

        if (unitMover.isMoving)
        {
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.walkAnimationType;
        }
        else
        {
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.idleAnimationType;
        }
    }
}

public partial struct AimShootAnimationStateJob : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    public void Execute(in AnimatedMesh animatedMesh, in ShootAttack shootAttack, in UnitMover unitMover,
            in Target target, in UnitAnimations unitAnimations)
    {
        if (!unitMover.isMoving && target.targetEntity != Entity.Null)
        {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.aimAnimationType;
        }

        if (shootAttack.onShoot.isTriggered)
        {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.shootAnimationType;
        }
    }
}

public partial struct MeleeAttackAnimationStateJob : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    public void Execute(in AnimatedMesh animatedMesh, in MeleeAttack meleeAttack, in UnitAnimations unitAnimations)
    {
        if (meleeAttack.onAttacked)
        {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.meleeAttackAnimationType;
        }
    }
}