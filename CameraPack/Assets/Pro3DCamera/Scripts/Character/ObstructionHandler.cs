using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pro3DCamera;

public class ObstructionHandler : MonoBehaviour
{

    class Obstruction
    {
        public enum State { FadingIn, FadingOut };
        public State state;
        public GameObject obj;
        public Material[] materials;
        public Color[] colors;

        public Obstruction(GameObject obj)
        {
            state = State.FadingOut;
            this.obj = obj;
            try
            {
                if (obj.GetComponent<SkinnedMeshRenderer>())
                    materials = obj.GetComponent<SkinnedMeshRenderer>().materials;
                else
                    materials = obj.GetComponent<Renderer>().materials;
            }
            catch(MissingComponentException)
            {
                Debug.LogError("No renderer on object: " + obj.name);
            }
            colors = new Color[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                colors[i] = materials[i].color;
            }
        }
    }


    public CameraDataManager dataManager;
    ObstructionHandlerData.ObstructionSettings obstructionSetting = new ObstructionHandlerData.ObstructionSettings();

    List<Obstruction> obstructions; //for holding all things that occlude the player
    RaycastHit[] obstructionHits;
    Material[] targetMaterials;
    Color[] initialTargetColors;
    Color targetColor;

    void Start()
    {
        obstructions = new List<Obstruction>();

        //initialize target materials and colors
        if (GetComponentInChildren<SkinnedMeshRenderer>())
            targetMaterials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        else
            targetMaterials = GetComponentInChildren<Renderer>().materials;
        initialTargetColors = new Color[targetMaterials.Length];

        for (int i = 0; i < targetMaterials.Length; i++)
        {
            initialTargetColors[i] = targetMaterials[i].color;
        }

    }

    void GetSettings()
    {
        obstructionSetting = dataManager.obstructionData.obstructionSet;
    }

    void FixedUpdate()
    {
        GetSettings();
        if (obstructionSetting.active)
        {
            CheckForObstructions();

            if (obstructionSetting.changeTargetColor)
            {
                HandleTargetColorWithObstructions();
            }

            if (obstructionHits != null)
            {
                CheckForOldObstructions(obstructionHits);
                HandleObstructionFading();
            }
        }
        else {
            RemoveAllRemainingObstructions();
        }
    }

    void CheckForObstructions()
    {
        Ray ray = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);

        obstructionHits = Physics.RaycastAll(ray, Vector3.Distance(Camera.main.transform.position, transform.position), obstructionSetting.obstructionLayer);

        foreach (RaycastHit hit in obstructionHits)
        {
            Obstruction o = new Obstruction(hit.collider.gameObject);
            for (int i = 0; i < o.materials.Length; i++ )
                SetMaterialBlendMode(o.materials[i], "Fade");
            if (!ObstructionExists(o))
                obstructions.Add(o);
        }
    }

    /// <summary>
    /// Sets obstructions not in the raycast anymore to fade back in to be fully opaque
    /// </summary>
    void CheckForOldObstructions(RaycastHit[] hits)
    {
        foreach (Obstruction o in obstructions)
        {
            bool obstructionStillExists = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == o.obj)
                    obstructionStillExists = true; //if this line is reached, a hit exists that is equal to an active obstruction, otherwise the obstruction does not exist anymore
            }

            if (!obstructionStillExists)
                o.state = Obstruction.State.FadingIn;
        }
    }

    /// <summary>
    /// Required to prevent redundancy in the obstructions list.
    /// </summary>
    bool ObstructionExists(Obstruction o)
    {
        foreach (Obstruction obstruction in obstructions)
        {
            if (obstruction.obj == o.obj)
                return true;
        }
        return false;
    }

    void HandleObstructionFading()
    {
        for (int i = 0; i < obstructions.Count; i++)
        {
            try
            {
                for (int j = 0; j < obstructions[i].materials.Length; j++)
                {
                    if (obstructions[i].state == Obstruction.State.FadingOut)
                    {
                        if (obstructions[i].colors[j].a > obstructionSetting.minObstructionAlpha)
                        {
                            obstructions[i].colors[j].a -= obstructionSetting.obstructionFadeSmooth * Time.deltaTime;
                            obstructions[i].materials[j].color = obstructions[i].colors[j];
                        }
                    }
                    if (obstructions[i].state == Obstruction.State.FadingIn)
                    {
                        if (obstructions[i].colors[j].a < 1.0f)
                        {
                            obstructions[i].colors[j].a += obstructionSetting.obstructionFadeSmooth * Time.deltaTime;
                            obstructions[i].materials[j].color = obstructions[i].colors[j];
                        }
                        else
                        {
                            for (int k = 0; k < obstructions[i].materials.Length; k++)
                                SetMaterialBlendMode(obstructions[i].materials[k], "Opaque");
                            obstructions.RemoveAt(i);
                        }
                    }
                }
            }
            catch (System.ArgumentOutOfRangeException) { }
        }
    }

    void RemoveAllRemainingObstructions()
    {
        for (int i = 0; i < obstructions.Count; i++)
        {
            try
            {
                for (int j = 0; j < obstructions[i].materials.Length; j++)
                {
                    if (obstructions[i].colors[j].a < 1.0f)
                    {
                        obstructions[i].colors[j].a += obstructionSetting.obstructionFadeSmooth * Time.deltaTime;
                        obstructions[i].materials[j].color = obstructions[i].colors[j];
                    }
                    else
                    {
                        for (int k = 0; k < obstructions[i].materials.Length; k++)
                            SetMaterialBlendMode(obstructions[i].materials[k], "Opaque");
                        obstructions.RemoveAt(i);
                    }
                }

            }
            catch (System.ArgumentOutOfRangeException) { }
        }
    }

    void HandleTargetColorWithObstructions()
    {
        for (int i = 0; i < targetMaterials.Length; i++)
        {
            Color newColor;
            if (obstructionHits.Length > 0)
            {
                newColor = obstructionSetting.obstructedColor * obstructionSetting.colorIntensity;
            }
            else
            {
                newColor = initialTargetColors[i];
            }
            targetMaterials[i].color = Vector4.Lerp(targetMaterials[i].color, newColor, obstructionSetting.targetFadeSmooth * Time.deltaTime);
        }
    }

    void FadeMaterialTo(Material mat, float alpha)
    {
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }

    void SetMaterialBlendMode(Material material, string BLEND_MODE)
    {
        switch (BLEND_MODE)
        {
            case "Opaque":
                material.SetFloat("_Mode", 0);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case "Fade":
                material.SetFloat("_Mode", 2); //set to fade mode
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}
