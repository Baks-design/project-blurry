using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class Hoverable : MonoBehaviour, IHoverable
    {
        [SerializeField, Self] MeshRenderer meshRenderer;
        [SerializeField] Transform tooltipTransform;
        [SerializeField] string tooltip;
        Material myMat;

        public Material MyMaterial => myMat;
        public MeshRenderer MeshRenderer => meshRenderer;
        public Transform TooltipTransform => tooltipTransform;
        public string Tooltip
        {
            get => tooltip;
            set => tooltip = value;
        }

        void Start() => myMat = meshRenderer.sharedMaterial;

        public void OnHoverStart(Material hoverMat) => meshRenderer.material = hoverMat;

        public void OnHoverEnd() => meshRenderer.material = myMat;
    }
}