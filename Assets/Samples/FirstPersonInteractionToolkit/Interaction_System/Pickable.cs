using KBCore.Refs;
using UnityEngine;

public class Pickable : MonoBehaviour, IPickable
{
    [SerializeField, Self] Rigidbody rigid;

    public Rigidbody Rigid { get; set; }
    public bool IsPicked { get; set; }
    public bool IsHolded { get; set; }
    public bool IsReleased { get; set; }

    public void OnPickUp()
    {
        IsPicked = true;
      
    }

    public void OnHold()
    {
      
    }

    public void OnRelease()
    {
        IsPicked = false;
    }
}
// TODO: Add actions