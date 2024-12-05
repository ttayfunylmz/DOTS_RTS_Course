using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "ScriptableObjects/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType : byte
    {
        None,
        SoldierIdle,
        SoldierWalk,
        ZombieIdle,
        ZombieWalk,
        SoldierAim,
        SoldierShoot,
        ZombieAttack,
        ScoutIdle,
        ScoutWalk,
        ScoutShoot,
        ScoutAim
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;

    public static bool IsAnimationUninterruptable(AnimationType animationType)
    {
        return animationType switch
        {
            AnimationType.ScoutShoot or AnimationType.SoldierShoot or AnimationType.ZombieAttack => true,
            _ => false,
        };
    }
}
