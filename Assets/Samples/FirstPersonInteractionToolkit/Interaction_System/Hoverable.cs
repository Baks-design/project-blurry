using KBCore.Refs;
using UnityEngine;

public class Hoverable : MonoBehaviour, IHoverable
{
    [SerializeField] string tooltip;
    [SerializeField] Transform tooltipTransform;
    [SerializeField, Self] MeshRenderer m_meshRenderer;
    Material m_myMat;

    public Material MyMaterial => m_myMat;
    public MeshRenderer MeshRenderer => m_meshRenderer;
    public Transform TooltipTransform => tooltipTransform;
    public string Tooltip
    {
        get => tooltip;
        set => tooltip = value;
    }

    void Start() => m_myMat = m_meshRenderer.material;

    public void OnHoverStart(Material _hoverMat) => m_meshRenderer.material = _hoverMat;

    public void OnHoverEnd() => m_meshRenderer.material = m_myMat;
}