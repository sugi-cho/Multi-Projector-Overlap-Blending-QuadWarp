using UnityEngine;
using sugi.cc;

[RequireComponent(typeof(Camera))]
public class CameraTargetEvent : MonoBehaviour
{
    public TextureEvent onCreateTarget;
    void Start()
    {
        var cam = GetComponent<Camera>();
        var width = cam.pixelWidth;
        var height = cam.pixelHeight;
        var targetTexture = Helper.CreateRenderTexture(width, height);
        onCreateTarget.Invoke(targetTexture);
        cam.targetTexture = targetTexture;
    }
}
