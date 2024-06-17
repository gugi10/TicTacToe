using UnityEditor;
using UnityEngine;

namespace BundleLoader
{
    [CustomEditor(typeof(SpriteRenderAssetLoader))]
    public class SpriteRenderAssetLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Name of sprite on this SpriteRenderer\n will be loaded from bundle", EditorStyles.boldLabel);
            DrawDefaultInspector();
        }
    }
}

