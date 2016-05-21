using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using DataUI;

namespace sugi.cc
{
    public class SettingManager : MonoBehaviour
    {
        public static void AddSettingMenu(Setting setting, string filePath)
        {
            setting.LoadSettingFromFile(filePath);
            setting.dataEditor = new FieldEditor(setting);
            if (!Instance.settings.Contains(setting))
            {
                Instance.settings.Add(setting);
                Instance.settings = Instance.settings.OrderBy(b => b.filePath).ToList();
            }
        }

        #region instance

        public static SettingManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject("SettingManager").AddComponent<SettingManager>();
                return _Instance;
            }
        }

        static SettingManager _Instance;

        #endregion

        public static KeyCode EditKey = KeyCode.E;

        List<Setting> settings = new List<Setting>();
        Setting currentSetting;
        bool edit;
        Rect windowRect = Rect.MinMaxRect(0, 0, Math.Min(Screen.width, 1024f), Math.Min(Screen.height, 768f));
        Vector2 scroll;

        public void HideGUI()
        {
            edit = false;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(EditKey))
            {
                edit = !edit;
                Cursor.visible = edit;
            }
        }

        void OnGUI()
        {
            if (!edit)
                return;
            windowRect = GUI.Window(GetInstanceID(), windowRect, OnWindow, "Settings");
        }

        void OnWindow(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            settings.ForEach(setting =>
            {
                if (setting.edit)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16f);
                    GUILayout.BeginVertical();
                    GUILayout.Label(setting.filePath);
                    setting.OnGUIFunc();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save and Close"))
                        setting.SaveAndClose();
                    if (GUILayout.Button("Cancel"))
                        setting.CancelAndClose();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                else if (GUILayout.Button(setting.filePath))
                    setting.edit = true;
            });
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        void OnRenderObject()
        {
            settings.ForEach(setting =>
            {
                setting.OnRenderObjectFunc(Camera.current);
            });
        }

        [System.Serializable]
        public abstract class Setting
        {
            public FieldEditor dataEditor { get; set; }

            public string filePath { get; set; }

            public bool edit { get; set; }

            public void LoadSettingFromFile(string path)
            {
                filePath = path;
                Helper.LoadJsonFile(this, filePath);
                OnLoad();
            }

            public void Save()
            {
                Helper.SaveJsonFile(this, filePath);
            }

            public void SaveAndClose()
            {
                Save();
                edit = false;
                OnClose();
            }

            public void CancelAndClose()
            {
                Helper.LoadJsonFile(this, filePath);
                dataEditor = new FieldEditor(this);
                edit = false;
                OnClose();
            }

            public virtual void OnGUIFunc()
            {
                dataEditor.OnGUI();
            }

            public virtual void OnRenderObjectFunc(Camera cam) { }

            protected virtual void OnLoad()
            {
            }

            protected virtual void OnClose()
            {
            }
        }
    }
}