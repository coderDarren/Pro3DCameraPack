using UnityEngine;
using System.Collections;

namespace Pro3DCamera
{
	[CreateAssetMenu()]
	public class ObstructionHandlerData : ScriptableObject {

		[System.Serializable]
        public class ObstructionSettings
	    {
	        public LayerMask obstructionLayer;
	        public float obstructionFadeSmooth = 1.5f;
	        public float minObstructionAlpha = 0.15f;
	        public bool changeTargetColor = true;
	        public Color obstructedColor = Color.red;
	        public float colorIntensity = 6;
	        public float targetFadeSmooth = 4;
	        public bool active = true;
	    }

	    public ObstructionSettings obstructionSet = new ObstructionSettings();
	}
}
