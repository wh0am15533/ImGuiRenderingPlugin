using System;
using System.Collections;
using System.Runtime.InteropServices;
//using UnityEngine.Rendering; // For RP/URP/HDRP
using UnityEngine;

namespace Trainer.DearImGui
{
    public class ImGuiPluginHook : MonoBehaviour
    {
        #region[Declarations]

        #region[P/Invokes]

        #region[System]

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static public extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32")]
        static public extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        #endregion

        #region[Plugin]

        [DllImport("ImGuiRenderingPlugin")]
        public static extern IntPtr GetUnityInterfacesPtr();

        [DllImport("ImGuiRenderingPlugin")]
        public static extern void UnityPluginLoad(System.IntPtr unityInterfacesptr);
        [DllImport("ImGuiRenderingPlugin")]
        public static extern void UnityPluginUnload();

        [DllImport("ImGuiRenderingPlugin")]
        private static extern System.IntPtr GetRenderEventFunc();

        private delegate void DebugCallback(string message, int type);
        [DllImport("ImGuiRenderingPlugin")]
        private static extern void RegisterDebugCallback(DebugCallback callback);

        [DllImport("ImGuiRenderingPlugin")]
        public static extern System.IntPtr GenerateImGuiFontTexture(System.IntPtr pixels, int width, int height, int bytesPerPixel);

        [DllImport("ImGuiRenderingPlugin")]
        public static extern void SendImGuiDrawCommands(ImGuiNET.ImDrawDataPtr ptr);

        [DllImport("ImGuiRenderingPlugin")]
        public static extern void CheckAPI();

        [DllImport("ImGuiRenderingPlugin")]
        public static extern void FlipMatrix();

        #endregion

        #endregion

        public static ImGuiPluginHook instance = null;

        private static ImGuiController _controller = null;

        // Used for Alternate Plugin Loading Method
        //private static bool _triedLoadingStubPlugin = false;
        //private static IntPtr _unityInterfacePtr = IntPtr.Zero;

        #endregion

        private void Awake()
        {
            instance = this;

            if (_controller == null)
            {
                _controller = new ImGuiController();
            }
        }

        private void OnApplicationQuit()
        {
            _controller = null;
        }

        private void Start()
        {
            RegisterDebugCallback(new DebugCallback(DebugMethod));

            #region[Fallback LoadPlugin if UnityPluginLoad didn't get called]
            /* 
            // ref: UnityNativeTool - Works in runtime but need to use an existing game plugin and replace it with StubLluiPlugin.dll.
            // Get the UnityInterfacesPtr first, without it we can't do anything
            try
            {
                _unityInterfacePtr = GetUnityInterfacesPtr();
                if (_unityInterfacePtr == IntPtr.Zero) { throw new Exception($"{nameof(GetUnityInterfacesPtr)} returned null"); } else { Debug.Log("UnityInferacesPtr = " + _unityInterfacePtr.ToString()); }
            }
            catch (DllNotFoundException) { Debug.LogWarning("StubLluiPlugin not found. UnityPluginLoad and UnityPluginUnload callbacks won't fire."); }
            finally { _triedLoadingStubPlugin = true; }
            
            // Now that we got the Ptr we can call UnityPluginLoad
            //Debug.Log("Calling UnityPluginLoad...");
            UnityPluginLoad(_unityInterfacePtr);
            */
            #endregion

            CheckAPI(); // Sanity Check

            StartCoroutine("CallPluginAtEndOfFrames");
        }

        private WaitForEndOfFrame frameWait = new WaitForEndOfFrame();
        private IEnumerator CallPluginAtEndOfFrames()
        {
            yield return frameWait;
            _controller.RecreateFontDeviceTexture(true);

            while (true)
            {
                //At the end of the frame, have ImGui render before invoking the draw on the GPU.
                yield return frameWait;
                _controller.Render();
                GL.IssuePluginEvent(GetRenderEventFunc(), 1);
            }
        }

        private void Update()
        {
            // FlipMatrix
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift) && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.M) && Event.current.type == EventType.KeyDown)
            {
                try
                {
                    // Some Games use a invert Y channel, this flips it if you need to. Per Game.
                    // Careful, a games scene change can flip based on game coding, assign a hotkey to this.
                    FlipMatrix();
                }
                catch (Exception e) { Debug.LogError("ERROR: " + e.Message); }

                Event.current.Use();
            }

            // Reassigns ImGuiWindow on Scene Changes, Since the Orig Obj gets Destroyed
            Camera cam = Camera.main;
            if (cam == null) { cam = Camera.current; }
            if (cam != null)
            {
                if (!cam.gameObject.HasComponent<DearImGui.ImGuiDemoWindow>()) { cam.gameObject.AddComponent<DearImGui.ImGuiDemoWindow>(); }
            }

            _controller.Update();
        }

        [MonoPInvokeCallback(typeof(DebugCallback))]
        private static void DebugMethod(string message, int type)
        {
            switch (type)
            {
                case 1:
                    Debug.Log("IMGUI: " + message);
                    break;
                case 2:
                    Debug.LogWarning("IMGUI: " + message);
                    break;
                case 3:
                    Debug.LogError("IMGUI: " + message);
                    break;
            }
        }
    }
}

#region[For SRP/URP/HDRP]

// ref: https://answers.unity.com/questions/1545858/onpostrender-is-not-called.html
/*
void OnEnable()
{
    RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
}
void OnDisable()
{
    RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
}
private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
{
    OnPostRender();
}
*/

#endregion