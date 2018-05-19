using UnityEngine;
using System.Collections.Generic;

namespace Pro3DCamera {
    [CreateAssetMenu()]
    public class CameraShakeData : ScriptableObject {
        

        [System.Serializable]
        public class ShakeSequence
        {
            public string name;
            public float duration;
            public float intensity;
            public float decay;
            public AnimationCurve curve_posX;
            public AnimationCurve curve_posY;
            public bool editing;

            public ShakeSequence()
            {
                curve_posX = AnimationCurve.Linear(0, 0, 1, 1);
                curve_posY = AnimationCurve.Linear(0, 0, 1, 1);
                name = "New Shake";
                duration = 1;
                editing = false;
            }

            public void CopyFromTo(ref ShakeSequence s2)
            {
                s2.duration = this.duration;
                s2.curve_posX = this.curve_posX;
                s2.curve_posY = this.curve_posY;
                s2.intensity = this.intensity;
                s2.decay = this.decay;
            }
        }
        
        public List<ShakeSequence> shakeSequences;
    }
}
