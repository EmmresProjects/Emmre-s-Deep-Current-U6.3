using UnityEngine;

namespace DeepCurrent
{
    // The camera is set up in Unity (orthographic, size, background color).
    // This component only READS what the camera sees and shares the visible arena with the gameplay code.
    [RequireComponent(typeof(Camera))]
    [DefaultExecutionOrder(-90)]
    public class CameraFitter : Singleton<CameraFitter>
    {
        public Vector2 HalfExtents { get; private set; } = new Vector2(720f, 405f);
        public ArenaBounds Bounds => new ArenaBounds(HalfExtents);

        Camera cam;
        int lastW, lastH;

        protected override void Awake()
        {
            base.Awake();
            cam = GetComponent<Camera>();
            Refresh();
        }

        void Update()
        {
            if (Screen.width != lastW || Screen.height != lastH) Refresh();
        }

        void Refresh()
        {
            lastW = Screen.width;
            lastH = Screen.height;
            float aspect = lastH > 0 ? (float)lastW / lastH : 16f / 9f;
            HalfExtents = new Vector2(cam.orthographicSize * aspect, cam.orthographicSize);
        }

        public Vector3 ScreenToWorld(Vector2 screenPos)
        {
            return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        }
    }
}
