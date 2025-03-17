using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Audio
{
    public enum GroundType
    {
        None,
        Grass,
        Wood,
        Stone,
        Metal
    }

    public class AudioTag : MonoBehaviour
    {
        [SerializeField] GroundType groundTypeTag;

        public GroundType GroundTypeTag => groundTypeTag;
    }
}