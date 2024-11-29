using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "ScriptableObjects/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    public Mesh[] meshArray;
    public float frameTimerMax;
}
