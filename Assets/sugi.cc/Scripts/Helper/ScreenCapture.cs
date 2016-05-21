using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace sugi.cc
{
    public class ScreenCapture : MonoBehaviour
    {
        static ScreenCapture Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject("capture").AddComponent<ScreenCapture>();
                return _Instance;
            }
        }

        static ScreenCapture _Instance;
        public int fps = 10;
        public int superSize = 0;

        bool recording;
        int framecount;

        // Use this for initialization
        void Start()
        {
            Init();
        }

        void Init()
        {
            System.IO.Directory.CreateDirectory("Capture");
            Time.captureFramerate = fps;
            framecount = -1;
        }

        // Update is called once per frame
        void Update()
        {
            if (!recording)
                return;

            if (framecount > 0)
            {
                var name = "Capture/frame" + Time.frameCount.ToString("00000") + ".png";
                Application.CaptureScreenshot(name, superSize);
            }
            framecount++;
        }
#if UNITY_EDITOR
        [MenuItem("ScreenCapture/Shot")]
        public static void CaptureScreenShot()
        {
            System.IO.Directory.CreateDirectory("ScreenShot");
            var name = "ScreenShot/Capture.png";
            Application.CaptureScreenshot(name);
        }

        [MenuItem("ScreenCapture/Start/FPS:10")]
        public static void StartRecording10()
        {
            Instance.fps = 10;
            Instance.Init();
            Instance.recording = true;
        }

        [MenuItem("ScreenCapture/Start/FPS:30")]
        public static void StartRecording30()
        {
            Instance.fps = 30;
            Instance.Init();
            Instance.recording = true;
        }

        [MenuItem("ScreenCapture/Start/FPS:60")]
        public static void StartRecording60()
        {
            Instance.fps = 60;
            Instance.Init();
            Instance.recording = true;
        }

        [MenuItem("ScreenCapture/Stop")]
        public static void StopRecording()
        {
            Instance.recording = false;
        }
#endif
    }
}