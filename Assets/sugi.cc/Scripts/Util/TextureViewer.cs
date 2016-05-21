using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace sugi.cc
{
    public class TextureViewer : MonoBehaviour
    {
        public static KeyCode showKey = KeyCode.T;

        bool show;
        int viewingIndex;
        Texture[] textures;
        Rect windowRect = Rect.MinMaxRect(0, 0, 512f, 512f);
        Texture currentTex;

        void SetCurrentTex()
        {
            viewingIndex = (int)Mathf.Repeat(viewingIndex, textures.Length);
            currentTex = textures[viewingIndex];
            windowRect.width = currentTex.width + 32;
            windowRect.height = currentTex.height + 32;
        }
        void Update()
        {
            if (show)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    viewingIndex--;
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    viewingIndex++;
                SetCurrentTex();
            }

            if (!Input.GetKeyDown(showKey))
                return;
            show = !show;
            Cursor.visible = show;
            if (show)
            {
                textures = FindObjectsOfType<Texture>();
                SetCurrentTex();
            }
        }
        void OnGUI()
        {
            if (!show)
                return;
            if (currentTex == null)
            {
                textures = FindObjectsOfType<Texture>();
                SetCurrentTex();
            }
            windowRect = GUI.Window(1, windowRect, OnWindow, currentTex.name);
        }
        void OnWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(currentTex);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
