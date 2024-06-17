using System.IO;
using UnityEditor;
using UnityEngine;

namespace BundleLoader
{
    public class AssetBundleBuilderWindow : EditorWindow
    {
        private Texture2D xSymbol;
        private Texture2D oSymbol;
        private Texture2D background;
        private string assetBundleName = "DefaultAssetBundle";
        private bool buildAssetBundle;

        [MenuItem("Window/Asset Bundle Builder")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AssetBundleBuilderWindow), false, "Asset Bundle Builder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Assign Graphics:", EditorStyles.boldLabel);

            xSymbol = (Texture2D)EditorGUILayout.ObjectField("X Symbol", xSymbol, typeof(Texture2D), false);
            oSymbol = (Texture2D)EditorGUILayout.ObjectField("O Symbol", oSymbol, typeof(Texture2D), false);
            background = (Texture2D)EditorGUILayout.ObjectField("Background", background, typeof(Texture2D), false);

            GUILayout.Space(10);

            GUILayout.Label("Asset Bundle Settings:", EditorStyles.boldLabel);
            assetBundleName = EditorGUILayout.TextField("Asset Bundle Name", assetBundleName);

            GUILayout.Space(20);

            if (GUILayout.Button("Build Asset Bundle"))
            {
                buildAssetBundle = true;
                BuildAssetBundle();
            }
        }

        private void BuildAssetBundle()
        {
            if (xSymbol == null || oSymbol == null || background == null)
            {
                Debug.LogError("One or more required assets are missing.");
                return;
            }

            AssetBundleBuild[] builds = new AssetBundleBuild[1];
            builds[0].assetBundleName = assetBundleName;

            string[] assetNames = new string[3];
            assetNames[0] = AssetDatabase.GetAssetPath(xSymbol);
            assetNames[1] = AssetDatabase.GetAssetPath(oSymbol);
            assetNames[2] = AssetDatabase.GetAssetPath(background);

            builds[0].assetNames = assetNames;

            // Define the path where the AssetBundle will be saved (StreamingAssets folder)
            string assetBundlePath = Path.Combine(Application.streamingAssetsPath, assetBundleName);

            // Ensure the directory exists before attempting to create the bundle
            Directory.CreateDirectory(Path.GetDirectoryName(assetBundlePath));

            // Build the AssetBundle
            BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(assetBundlePath), builds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            // Refresh the AssetDatabase and clear the build flag
            AssetDatabase.Refresh();
            buildAssetBundle = false;

            // Log a message to indicate the AssetBundle build completion
            Debug.Log($"AssetBundle '{assetBundleName}' has been built and saved to: {assetBundlePath}");
        }
    }
}