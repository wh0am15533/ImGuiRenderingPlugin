
#region[References]
using System;
using UnityEngine;
//using UnityEngine.InputSystem; // New Unity Input system
using ImGuiNET;
#endregion

namespace Trainer
{
    public class TrainerMenu : MonoBehaviour
    {
        #region[Declarations]

        public static TrainerMenu instance = null;
        public static BepInEx.Logging.ManualLogSource log = Trainer.BepInLoader.log;

        public static string baseDirectory = System.Environment.CurrentDirectory;

        private Rect MainWindow;
        private bool MainWindowVisible = true;

        private GameObject imguihook;

        #endregion

        private void Start()
        {
            MainWindow = new Rect(UnityEngine.Screen.width / 2 - 100, UnityEngine.Screen.height / 2 - 350, 250f, 50f);
            instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace)) { MainWindowVisible = !MainWindowVisible; } // Legacy Input
            //if (Keyboard.current.backspaceKey.wasPressedThisFrame) { MainWindowVisible = !MainWindowVisible; } // New Input System
        }

        private void OnGUI()
        {
            if (!MainWindowVisible) { return; }
                
            if (UnityEngine.Event.current.type == UnityEngine.EventType.Layout)
            {
                GUI.backgroundColor = Color.black;
                GUIStyle titleStyle = new GUIStyle(GUI.skin.window);
                titleStyle.normal.textColor = Color.green;

                //MAIN WINDOW #0
                MainWindow = new Rect(MainWindow.x, MainWindow.y, 250f, 50f);
                MainWindow = GUILayout.Window(777, MainWindow, new GUI.WindowFunction(RenderUI), "Example Trainer v1", titleStyle, new GUILayoutOption[0]);
            }

        }

        private void RenderUI(int id)
        {
            switch (id)
            {
                case 777:

                    GUI.color = Color.white;
                    if (GUILayout.Button("Init IMGUI Demo", new GUILayoutOption[0]))
                    {
                        try
                        {
                            // Create new DontDestroy GameObject for our Plugins Hook
                            imguihook = new GameObject("ImGuiPluginHook", typeof(DearImGui.ImGuiPluginHook));
                            DontDestroyOnLoad(imguihook);

                            // Add our rendering hook
                            Camera cam = Camera.main;
                            if (cam == null) { cam = Camera.current; }
                            if (cam != null)
                            {
                                cam.gameObject.AddComponent<DearImGui.ImGuiDemoWindow>();
                            }

                            // Subscribe to Layout Event (Cleaner, Safe)- Where you place your window layout code.
                            DearImGui.ImGuiDemoWindow.Layout += OnLayout;
                        }
                        catch (Exception e) { log.LogError("ERROR: " + e.Message); }
                    }

                    break;

                default:
                    break;
            }

            GUI.DragWindow();
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


