using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "ScriptableObjects/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType : byte
    {
        None,
        SoldierIdle,
        SoldierWalk
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;
}
