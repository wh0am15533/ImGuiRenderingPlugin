using UnityEngine;
using ImGuiNET;
using System;

namespace Trainer.DearImGui
{
    public class ImGuiDemoWindow : MonoBehaviour
    {
        #region[Declarations]

        public static ImGuiDemoWindow instance;

        // Event Support - ref: https://github.com/psydack/uimgui
        public event Action Layout;
        internal static void DoLayout() => instance.Layout?.Invoke();

        // Cannot be Null, Else OnPostRender won't fire
        private Camera _cam = null;
        public Camera cam
        {
            get { return _cam; }
            set { _cam = value; }
        }

        #endregion

        public ImGuiDemoWindow()
        {
            instance = this;
        }

        public void Awake()
        {
            if (!gameObject.HasComponent<Camera>()) { cam = gameObject.AddComponent<Camera>(); }
            else { cam = gameObject.GetComponent<Camera>(); }
            cam.depth = 100f;
            cam.clearFlags = CameraClearFlags.Nothing;
        }

        public void Update()
        {
            // TODO: Add Do Global Event
            //DoLayout();
            Layout?.Invoke();
        }

        public void FixedUpdate()
        {
            
        }
        public void OnPostRender()
        {
            
        }

        public void OnDisable()
        {
            cam = null;
        }
        public void OnDestroy()
        {
            cam = null;
        }
    }
}

/* Event Usage
public void OnEnable()
{
    Layout += OnLayout;
}
public void OnDisable()
{
    Layout -= OnLayout;
}
public void OnLayout()
{
    ImGui.ShowDemoWindow();
}
*/
