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
        ZombieAttack
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;
}
