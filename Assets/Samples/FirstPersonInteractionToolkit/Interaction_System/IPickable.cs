using UnityEngine;

public interface IPickable
{
    bool IsPicked { get; set; }
    bool IsHolded { get; set; }
    bool IsReleased { get; set; }
    Rigidbody Rigid { get; set; }

    void OnPickUp();
    void OnHold();
    void OnRelease();
}