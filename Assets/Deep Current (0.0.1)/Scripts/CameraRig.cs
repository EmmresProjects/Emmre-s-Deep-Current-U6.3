using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DeepCurrent
{
    // Copying the HTML, as of the moment with fixed-height camera and arena bounds, without screen shake.
    [RequireComponent(typeof(Camera))]
    public class CameraRig : MonoBehaviour
    {
        public Camera Cam { get; private set; }

        void Awake()
        {
            DC.Cam = this;
            Cam = GetComponent<Camera>();
            if (GetComponent<UniversalAdditionalCameraData>() == null)
                gameObject.AddComponent<UniversalAdditionalCameraData>();
            Cam.orthographic = true;
            Cam.orthographicSize = DC.HalfHeight;
            Cam.clearFlags = CameraClearFlags.SolidColor;
            Cam.backgroundColor = DC.Abyss;
            Cam.nearClipPlane = 0.3f;
            Cam.farClipPlane = 100f;
            transform.position = new Vector3(0f, 0f, -10f);
            RefreshBounds();
        }

        void Update()
        {
            RefreshBounds();
        }

        void RefreshBounds()
        {
            float aspect = Screen.height > 0 ? (float)Screen.width / Screen.height : (16f / 9f);
            Arena.HalfH = Cam.orthographicSize;
            Arena.HalfW = Arena.HalfH * aspect;
        }

        public Vector3 ScreenToWorld(Vector2 screenPos)
        {
            return Cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        }
    }
}
