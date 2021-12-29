
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
        private DearImGui.ImGuiDemoWindow mainWindow = null;

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
                                mainWindow = cam.gameObject.AddComponent<DearImGui.ImGuiDemoWindow>();
                            }

                            // Subscribe to Layout Event (Cleaner, Safe)- Where you place your window layout code.
                            mainWindow.Layout += OnLayout;
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


