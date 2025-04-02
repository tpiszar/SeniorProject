
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PlatformSwitcher : EditorWindow
{
    // Define material mappings for Windows and Android
    private static MaterialMapping[] materialMappings = new MaterialMapping[]
    {
        new MaterialMapping("Assets/Materials 1/CubeBarrierMax.mat", "Assets/Materials 1/CubeNoDepth.mat"),
        new MaterialMapping("Assets/Materials 1/BarrierSphereMax.mat", "Assets/Materials 1/BarrierNoDepth.mat"),
        new MaterialMapping("Assets/Materials 1/GhoulBarrierMax.mat", "Assets/Materials 1/GhoulNoDepth.mat"),
    };

    private static string[] windowsRPPaths = new string[]
    {
        "Assets/Settings/URP-Performant.asset",
        "Assets/Settings/URP-Balanced.asset",
        "Assets/Settings/URP-HighFidelity.asset"
    };

    private static string[] androidRPPaths = new string[]
    {
        "Assets/Settings/URP-Performant Android.asset",
        "Assets/Settings/URP-Balanced Android.asset",
        "Assets/Settings/URP-HighFidelity Android.asset"
    };

    // Paths to the render pipeline assets for each platform
    private const string windowsRPPath = "Assets/Settings/URP-HighFidelity.asset";
    private const string androidRPPath = "Assets/Settings/URP-HighFidelity Android.asset";

    [MenuItem("Tools/Switch Platform/Set for Windows")]
    public static void SetWindowsMaterials()
    {
        SetMaterialsForPlatform(true);
        SwitchRenderPipelineAsset(true);
    }

    [MenuItem("Tools/Switch Platform/Set for Android")]
    public static void SetAndroidMaterials()
    {
        SetMaterialsForPlatform(false);
        SwitchRenderPipelineAsset(false);
    }

    private static void SetMaterialsForPlatform(bool isWindows)
    {
        Dictionary<Material, Material> materialReplacementMap = new Dictionary<Material, Material>();

        foreach (var mapping in materialMappings)
        {
            Material originalMaterial = AssetDatabase.LoadAssetAtPath<Material>(isWindows ? mapping.androidPath : mapping.windowsPath);
            Material newMaterial = AssetDatabase.LoadAssetAtPath<Material>(isWindows ? mapping.windowsPath : mapping.androidPath);

            if (originalMaterial != null && newMaterial != null)
            {
                materialReplacementMap[originalMaterial] = newMaterial;
            }
            else
            {
                Debug.LogWarning($"Material missing! Check paths:\n{mapping.windowsPath}\n{mapping.androidPath}");
            }
        }

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        foreach (string guid in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
                bool modified = false;

                foreach (Renderer rend in renderers)
                {
                    if (rend.sharedMaterial == null)
                        continue;

                    if (materialReplacementMap.TryGetValue(rend.sharedMaterial, out Material newMaterial))
                    {
                        rend.sharedMaterial = newMaterial;
                        modified = true;
                    }
                }

                if (modified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    Debug.Log($"Updated prefab: {prefabPath}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Material switch to {(isWindows ? "Windows" : "Android")} completed.");
    }

    private static void SwitchRenderPipelineAsset(bool isWindows)
    {
        for (int i = 0; i < windowsRPPaths.Length; i++)
        {
            string pipelinePath = isWindows ? windowsRPPaths[i] : androidRPPaths[i];
            RenderPipelineAsset pipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(pipelinePath);
            if (pipelineAsset != null)
            {
                QualitySettings.SetQualityLevel(i);
                QualitySettings.renderPipeline = pipelineAsset;
                Debug.Log("Render Pipeline asset set to: " + pipelineAsset.name);
            }
            else
            {
                Debug.LogWarning("Render Pipeline asset missing at " + pipelinePath);
            }
        }


        AssetDatabase.SaveAssets();
        Debug.Log($"Switch to {(isWindows ? "Windows" : "Android")} completed.");


    }

    private class MaterialMapping
    {
        public string windowsPath;
        public string androidPath;

        public MaterialMapping(string windowsPath, string androidPath)
        {
            this.windowsPath = windowsPath;
            this.androidPath = androidPath;
        }
    }
}

#endif