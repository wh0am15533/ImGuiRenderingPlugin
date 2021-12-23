using ImGuiNET;
using UnityEngine;

namespace Trainer.DearImGui
{
    public class ImGuiInput
    {
        int[] trackedKeys;
        public static bool WantCaptureMouse
        {
            get { return ImGui.GetIO().WantCaptureMouse; }
        }

        public static bool WantCaptureKeyboard
        {
            get { return ImGui.GetIO().WantCaptureKeyboard; }
        }

        public void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            trackedKeys = new int[] {
                io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab,
                io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.LeftArrow,
                io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.RightArrow,
                io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.UpArrow,
                io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.DownArrow,
                io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyCode.PageUp,
                io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyCode.PageDown,
                io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home,
                io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.End,
                io.KeyMap[(int)ImGuiKey.Insert] = (int)KeyCode.Insert,
                io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete,
                io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Backspace,
                io.KeyMap[(int)ImGuiKey.Space] = (int)KeyCode.Space,
                io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Return,
                io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyCode.Escape,
                io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)KeyCode.KeypadEnter,
                io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.A,
                io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.C,
                io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.V,
                io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.X,
                io.KeyMap[(int)ImGuiKey.Y] = (int)KeyCode.Y,
                io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z,
            };
        }
    
        public void Update()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            UpdateMouse(io);
            UpdateKeyboard(io);
        }

        public void UpdateMouse(ImGuiIOPtr io)
        {
            io.MouseDown[0] = Input.GetMouseButton(0);
            io.MouseDown[1] = Input.GetMouseButton(1);
            io.MouseDown[2] = Input.GetMouseButton(2);

            io.MousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            io.MouseWheel = Input.mouseScrollDelta.y;
            io.MouseWheelH = Input.mouseScrollDelta.x;
        }

        public void UpdateKeyboard(ImGuiIOPtr io)
        {
            io.AddInputCharactersUTF8(Input.inputString);

            foreach (int key in trackedKeys)
            {
                io.KeysDown[key] = Input.GetKey((KeyCode)key);
            }

            io.KeyCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            io.KeyAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            io.KeyShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            io.KeySuper = Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows)
                        || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
        }
    }
}

