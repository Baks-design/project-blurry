using UnityEngine;

public class FootstepSystem : MonoBehaviour
{
    [Header("Left Foot Settings")]
    [SerializeField] AudioSource leftFoot;
    [SerializeField] AudioClip leftFootClip;
    [SerializeField, Range(0.1f, 1f)] float leftMinimumPitch = 0.5f;
    [Header("Right Foot Settings")]
    [SerializeField] AudioSource rightFoot;
    [SerializeField, InLineEditor] AudioClip rightFootClip;
    [SerializeField, Range(0.1f, 1f)] float rightMinimumPitch = 0.5f;
    [Header("Turn Around Settings")]
    [SerializeField, InLineEditor] AudioClip turnClip;
    [SerializeField, Range(0.1f, 1f)] float turnMinimumPitch = 0.5f;
    bool _nextStepLeft;

    public void Step()
    {
        var foot = _nextStepLeft ? leftFoot : rightFoot;
        var clip = _nextStepLeft ? leftFootClip : rightFootClip;
        var minPitch = _nextStepLeft ? leftMinimumPitch : rightMinimumPitch;

        foot.pitch = Random.Range(minPitch, 1f);
        foot.PlayOneShot(clip);

        _nextStepLeft = !_nextStepLeft;
    }

    public void Turn()
    {
        var pitch = Random.Range(turnMinimumPitch, 1f);

        leftFoot.pitch = pitch;
        rightFoot.pitch = pitch;

        leftFoot.PlayOneShot(turnClip);
        rightFoot.PlayOneShot(turnClip);
    }
}
//TODO: Add object pooling 