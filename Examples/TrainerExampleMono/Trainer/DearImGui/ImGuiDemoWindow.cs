using UnityEngine;
using ImGuiNET;
using System;

namespace Trainer.DearImGui
{
    public class ImGuiDemoWindow : MonoBehaviour
    {
        #region[Declarations]

        // Event Support
        public static event Action Layout;
        internal static void DoLayout() => Layout?.Invoke();

        // Cannot be Null, Else OnPostRender won't fire
        public Camera cam = null;

        #endregion

        public void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
            //cam.depth = 100f;
        }

        public void Update()
        {
            DoLayout();
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
