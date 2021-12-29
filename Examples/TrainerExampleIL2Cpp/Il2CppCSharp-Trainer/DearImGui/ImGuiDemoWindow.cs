﻿using UnityEngine;
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

        public ImGuiDemoWindow(IntPtr ptr) : base(ptr)
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

        // These basically just update the Coroutine for Rendering
        public void Update()
        {
            // TODO: Add Do Global Event
            //DoLayout();
            Layout?.Invoke();

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