using UnityEngine;

public class AnimationPlaying : MonoBehaviour
{
    private int _animationsToComplete = 0;
    public void OnAnimationCompleted()
    {
        _animationsToComplete--;
    }

    public void QueueAnimation()
    {
        _animationsToComplete++;
    }

    public bool IsAnimationPlaying { get { return _animationsToComplete > 0; } }

}