using UnityEngine;
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

        public ImGuiDemoWindow(IntPtr ptr) : base(ptr)
        {

        }

        public void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
            //cam.depth = 100f;
        }

        // These basically just update the Coroutine for Rendering
        public void Update()
        {
            DoLayout();
            Tools.Il2CppCoroutine.Process();
        }
        public void FixedUpdate()
        {
            Tools.Il2CppCoroutine.ProcessWaitForFixedUpdate();
        }
        public void OnPostRender()
        {
            Tools.Il2CppCoroutine.ProcessWaitForEndOfFrame();
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