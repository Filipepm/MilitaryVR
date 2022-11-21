using UnityEngine;


namespace vls_v2
{
    public class VLS_V2_VisionController : MonoBehaviour
    {
        // YOU SHOULD ADD THE FOLLOWING OTHER LIGHT CONTROLLERS TO THIS GAMEOBJECT: FLASHER, ROTATOR, SEMIROTATOR in order to use those types.
        // Configure patterns on those scripts.
        /***********************************
        Patterns
        ***********************************/

        [Header("Pattern Setup")]
        [SerializeField]
        private PodType patternAType;
        [SerializeField]
        private PodType patternBType;
        [SerializeField]
        private PodType patternCType;
        [SerializeField]
        private PodType patternDType;
        [SerializeField]
        private PodType patternEType;
        public enum PodType
        {
            flasher, rotator, semi, colored, off
        }
        public PodType GetCurrentType(int Pattern)
        {
            switch (Pattern)
            {
                case 0:
                default:
                    return PodType.off;
                case 1:
                    return patternAType;
                case 2:
                    return patternBType;
                case 3:
                    return patternCType;
                case 4:
                    return patternDType;
                case 5:
                    return patternEType;
            }
        }
        public int GetMaxPatterns()
        {
            if (patternAType == PodType.off)
                return 0;
            else if (patternBType == PodType.off)
                return 1;
            else if (patternCType == PodType.off)
                return 2;
            else if (patternDType == PodType.off)
                return 3;
            else if (patternEType == PodType.off)
                return 4;
            else
                return 5;
        }
    }
}
