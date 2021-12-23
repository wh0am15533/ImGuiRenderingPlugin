
#region[Refs]
using System;
//using UnityEngine.InputSystem; // New Unity InputSystem
using UnityEngine;
using ImGuiNET;

#endregion

namespace Trainer
{
    public class TrainerComponent : MonoBehaviour
    {
        #region[Declarations]

        #region[Trainer]

        public static GameObject obj = null;
        public static TrainerComponent instance;
        private static bool initialized = false;
        private static BepInEx.Logging.ManualLogSource log;

        #endregion

        private GameObject imguihook = null;


        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            var component = new TrainerComponent(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<TrainerComponent>()).Pointer);
            return obj;
        }

        public TrainerComponent(IntPtr ptr) : base(ptr)
        {
            log = BepInExLoader.log;
            instance = this;
        }

        private static void Initialize()
        {
            log.LogWarning("ImGui Example v1 IL2CPP - wh0am15533");

            log.LogWarning(" ");
            log.LogWarning("HotKeys:");
            log.LogWarning("   RightControl + E  = Init IMGUI Demo Window");
            log.LogWarning("   RightControl + M  = Invert Window if Upside-down");
            log.LogWarning(" ");

            initialized = true;
        }

        public void Update()
        {
            if (!initialized) { Initialize(); }

            // Init ImGui
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift) && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.E) && Event.current.type == EventType.KeyDown)
            {
                try
                {
                    // Create new DontDestroy GameObject for our Plugins Hook
                    imguihook = DearImGui.ImGuiPluginHook.Create("ImGuiPluginHook");
                    DontDestroyOnLoad(imguihook);

                    // Add our rendering hook
                    Camera camera = Camera.main;
                    if (camera == null) { camera = Camera.current; }
                    if (camera != null) { camera.gameObject.AddComponent<DearImGui.ImGuiDemoWindow>(); }

                    // Subscribe to Layout Event (Cleaner, Safe)- Where you place your window layout code.
                    DearImGui.ImGuiDemoWindow.Layout += OnLayout;
                }
                catch (Exception e) { log.LogError("ERROR: " + e.Message); }

                Event.current.Use();
            }

        }


        #region[ImGui Rendering - Make your Window Stuff Here]

        //private static int _counter = 0;
        //private static int _dragInt = 0;
        public void OnLayout()
        {
            ImGui.ShowDemoWindow();

            /*
            // 1. Show a simple window. ref: https://github.com/mellinoe/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram/Program.cs
            // Tip: if we don't call ImGui.BeginWindow()/ImGui.EndWindow() the widgets automatically appears in a window called "Debug".
            {
                ImGui.Begin("Example Trainer");
                ImGui.Text("Hello, world!");                                        // Display some text (you can use a format string too)
                //ImGui.SliderFloat("float", ref _f, 0, 1, _f.ToString("0.000"));   // Edit 1 float using a slider from 0.0f to 1.0f    
                //ImGui.ColorEdit3("clear color", ref _clearColor);                 // Edit 3 floats representing a color

                ImGui.Text($"Mouse position: {ImGui.GetMousePos().ToString()}");

                if (ImGui.Button("Button")) // Buttons return true when clicked (NB: most widgets return true when edited/activated)
                    _counter++;
                ImGui.SameLine(0, -1);
                ImGui.Text($"counter = {_counter}");

                ImGui.DragInt("Draggable Int", ref _dragInt);

                float framerate = ImGui.GetIO().Framerate;
                ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
                ImGui.End();
            }
            */
        }
        public void OnDisable()
        {
            DearImGui.ImGuiDemoWindow.Layout -= OnLayout;
        }

        #endregion

    }

    #region[Extensions]
    
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject flag) where T : Component
        {
            if (flag == null)
                return false;
            return flag.GetComponent<T>() != null;
        }
    }

    #endregion
}

/* Place on a Canvas
imguihook = DearImGui.ImGuiPluginHook.Create("ImGuiPluginHook");
DontDestroyOnLoad(imguihook);

GameObject imguiwindow = new GameObject("ImGuiWindow");
Camera camera = imguiwindow.AddComponent<Camera>();
//camera.orthographic = true;
camera.depth = 100.0f;

GameObject canvas = createUICanvas(imguihook, camera);

imguiwindow.transform.parent = canvas.transform;
imguiwindow.AddComponent<DearImGui.ImGuiWindow>();


public GameObject createUICanvas(GameObject parent, Camera cam)
{
    // Create a new Canvas Object with required components
    GameObject CanvasGO = new GameObject("ImGuiPluginCanvas");
    CanvasGO.transform.parent = parent.transform;
    //Object.DontDestroyOnLoad(CanvasGO);

    Canvas canvas = new Canvas(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<Canvas>()).Pointer);

    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    //canvas.renderMode = RenderMode.ScreenSpaceCamera;
    //canvas.worldCamera = cam;
    canvas.sortingOrder = 10000;

    UnityEngine.UI.CanvasScaler cs = new UnityEngine.UI.CanvasScaler(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.UI.CanvasScaler>()).Pointer);

    cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
    cs.referencePixelsPerUnit = 100f;
    cs.referenceResolution = new Vector2(1024f, 788f);

    UnityEngine.UI.GraphicRaycaster gr = new UnityEngine.UI.GraphicRaycaster(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.UI.GraphicRaycaster>()).Pointer);

    return CanvasGO;
}


*/
