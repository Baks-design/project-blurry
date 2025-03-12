using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public interface IHoverable
    {
        string Tooltip { get; set; }
        Transform TooltipTransform { get; }

        void OnHoverStart(Material _hoverMat);
        void OnHoverEnd();
    }
}