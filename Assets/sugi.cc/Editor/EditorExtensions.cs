using UnityEngine;
using UnityEditor;
using System.IO;

namespace sugi.cc
{
    public class EditorExtensions
    {
        [MenuItem("Assets/Select All/Materials")]
        public static void SelectAllMaterial()
        {
            Selection.objects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
        }

        [MenuItem("Assets/Select All/Meshes")]
        public static void SelectAllMesh()
        {
            Selection.objects = Selection.GetFiltered(typeof(Mesh), SelectionMode.DeepAssets);
        }

        [MenuItem("Assets/Select All/Textures")]
        public static void SelectAllTexture()
        {
            Selection.objects = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
        }

        [MenuItem("Assets/Select All/Shaders")]
        public static void SelectAllShader()
        {
            Selection.objects = Selection.GetFiltered(typeof(Shader), SelectionMode.DeepAssets);
        }

        [MenuItem("Assets/Copy and Create New.asset")]
        public static void CopyObject()
        {
            var o = Selection.activeObject;
            if (o == null)
                return;
            var newObj = Object.Instantiate<Object>(o);
            var path = AssetDatabase.GetAssetPath(o);
            path = Path.GetDirectoryName(path);
            AssetDatabase.CreateAsset(newObj, path + '/' + o.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = newObj;
        }

        [MenuItem("Assets/Create/Default Folders")]
        public static void CreateFolders()
        {
            var ao = Selection.activeObject;
            if (ao == null) return;
            var path = AssetDatabase.GetAssetPath(ao);
            if (AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(path, "Materials");
                AssetDatabase.CreateFolder(path, "Scenes");
                AssetDatabase.CreateFolder(path, "Scripts");
                AssetDatabase.CreateFolder(path, "Shaders");
                AssetDatabase.CreateFolder(path, "Textures");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}