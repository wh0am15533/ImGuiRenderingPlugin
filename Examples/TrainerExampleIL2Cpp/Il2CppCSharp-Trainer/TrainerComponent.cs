﻿
#region[Refs]
using System;
//using UnityEngine.InputSystem; // New Unity InputSystem
using UnityEngine;
using ImGuiNET;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        private GameObject imguihook = null;
        private DearImGui.ImGuiDemoWindow mainWindow = null;

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

            // Find cimgui.dll and set path
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string file = Directory.GetFiles(rootPath, "cimgui.dll", SearchOption.AllDirectories).FirstOrDefault();
            if (file != null || file != string.Empty) { var res = SetDllDirectory(System.IO.Path.GetDirectoryName(file)); }
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

            // Init ImGui Simple
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
                    if (camera != null) { mainWindow = camera.gameObject.AddComponent<DearImGui.ImGuiDemoWindow>(); }

                    // Subscribe to Layout Event (Cleaner, Safe)- Where you place your window layout code.
                    //DearImGui.ImGuiDemoWindow.Layout += OnLayout;
                    mainWindow.Layout += OnLayout;
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

            #region[Examples]

            // Usage Manual: https://pthom.github.io/imgui_manual_online/manual/imgui_manual.html

            //ImGui.Text("Hello, world!");                                      // Display some text (you can use a format string too)
            //ImGui.Checkbox("ImGui Demo Window", ref _showImGuiDemoWindow);    // Edit bools storing our windows open/close state
            //ImGui.SliderFloat("float", ref _f, 0, 1, _f.ToString("0.000"));   // Edit 1 float using a slider from 0.0f to 1.0f    
            //ImGui.ColorEdit3("clear color", ref _clearColor);                 // Edit 3 floats representing a color
            //ImGui.DragInt("Draggable Int", ref _dragInt);
            /*
            //float framerate = ImGui.GetIO().Framerate;
            //ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");   

            // 2. Show another simple window. In most cases you will use an explicit Begin/End pair to name your windows.
            if (_showAnotherWindow)
            {
                ImGui.Begin("Another Window", ref _showAnotherWindow);
                ImGui.Text("Hello from another window!");
                if (ImGui.Button("Close Me"))
                    _showAnotherWindow = false;
                ImGui.End();
            }

            // 3. Show the ImGui demo window. Most of the sample code is in ImGui.ShowDemoWindow(). Read its code to learn more about Dear ImGui!
            if (_showImGuiDemoWindow)
            {
                // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
                // Here we just want to make the demo initial state a bit more friendly!
                ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
            }

            if (ImGui.TreeNode("Tabs"))
            {
                if (ImGui.TreeNode("Basic"))
                {
                    ImGuiTabBarFlags tab_bar_flags = ImGuiTabBarFlags.None;
                    if (ImGui.BeginTabBar("MyTabBar", tab_bar_flags))
                    {
                        if (ImGui.BeginTabItem("Avocado"))
                        {
                            ImGui.Text("This is the Avocado tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Broccoli"))
                        {
                            ImGui.Text("This is the Broccoli tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Cucumber"))
                        {
                            ImGui.Text("This is the Cucumber tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        ImGui.EndTabBar();
                    }
                    ImGui.Separator();
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Advanced & Close Button"))
                {
                    // Expose a couple of the available flags. In most cases you may just call BeginTabBar() with no flags (0).
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_Reorderable", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.Reorderable);
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_AutoSelectNewTabs", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.AutoSelectNewTabs);
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_NoCloseWithMiddleMouseButton", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);
                    if ((s_tab_bar_flags & (uint)ImGuiTabBarFlags.FittingPolicyMask) == 0)
                        s_tab_bar_flags |= (uint)ImGuiTabBarFlags.FittingPolicyDefault;
                    if (ImGui.CheckboxFlags("ImGuiTabBarFlags_FittingPolicyResizeDown", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.FittingPolicyResizeDown))
                s_tab_bar_flags &= ~((uint)ImGuiTabBarFlags.FittingPolicyMask ^ (uint)ImGuiTabBarFlags.FittingPolicyResizeDown);
                    if (ImGui.CheckboxFlags("ImGuiTabBarFlags_FittingPolicyScroll", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.FittingPolicyScroll))
                s_tab_bar_flags &= ~((uint)ImGuiTabBarFlags.FittingPolicyMask ^ (uint)ImGuiTabBarFlags.FittingPolicyScroll);

                    // Tab Bar
                    string[] names = { "Artichoke", "Beetroot", "Celery", "Daikon" };

                    for (int n = 0; n < s_opened.Length; n++)
                    {
                        if (n > 0) { ImGui.SameLine(); }
                        ImGui.Checkbox(names[n], ref s_opened[n]);
                    }

                    // Passing a bool* to BeginTabItem() is similar to passing one to Begin(): the underlying bool will be set to false when the tab is closed.
                    if (ImGui.BeginTabBar("MyTabBar", (ImGuiTabBarFlags)s_tab_bar_flags))
                    {
                        for (int n = 0; n < s_opened.Length; n++)
                            if (s_opened[n] && ImGui.BeginTabItem(names[n], ref s_opened[n]))
                            {
                                ImGui.Text($"This is the {names[n]} tab!");
                                if ((n & 1) != 0)
                                    ImGui.Text("I am an odd tab.");
                                ImGui.EndTabItem();
                            }
                        ImGui.EndTabBar();
                    }
                    ImGui.Separator();
                    ImGui.TreePop();
                }
                ImGui.TreePop();
            }

            ImGuiIOPtr io = ImGui.GetIO();
            SetThing(out io.DeltaTime, 2f);

            if (_showMemoryEditor)
            {
                _memoryEditor.Draw("Memory Editor", _memoryEditorData, _memoryEditorData.Length);
            }
            */

            // ------------------------------------------------------------------------------------------------------------------------------------------------
            // ref: https://github.com/ocornut/imgui/tree/512c54bbc062c41c74f8a8bd8ff1fd6bebd1e6d0
            // ref: https://github.com/ocornut/imgui/blob/master/imgui_demo.cpp
            /*
            // Create a window called "My First Tool", with a menu bar.
            ImGui::Begin("My First Tool", &my_tool_active, ImGuiWindowFlags_MenuBar);
            if (ImGui::BeginMenuBar())
            {
                if (ImGui::BeginMenu("File"))
                {
                    if (ImGui::MenuItem("Open..", "Ctrl+O")) { *//* Do stuff *//*
                }
                if (ImGui.MenuItem("Save", "Ctrl+S")) { *//* Do stuff *//* }
                    if (ImGui.MenuItem("Close", "Ctrl+W")) { my_tool_active = false; }
                    ImGui.EndMenu();
                }
                ImGui::EndMenuBar();
            }

            // Edit a color (stored as ~4 floats)
            ImGui::ColorEdit4("Color", my_color);

            // Plot some values
            const float my_values[] = { 0.2f, 0.1f, 1.0f, 0.5f, 0.9f, 2.2f };
            ImGui::PlotLines("Frame Times", my_values, IM_ARRAYSIZE(my_values));

            // Display contents in a scrolling region
            ImGui::TextColored(ImVec4(1, 1, 0, 1), "Important Stuff");
            ImGui::BeginChild("Scrolling");
            for (int n = 0; n < 50; n++)
                ImGui::Text("%04d: Some text", n);
            ImGui::EndChild();
            ImGui::End();
            */
            #endregion
        }
        public void OnDisable()
        {
            //DearImGui.ImGuiDemoWindow.Layout -= OnLayout;
            mainWindow.Layout -= OnLayout;
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
