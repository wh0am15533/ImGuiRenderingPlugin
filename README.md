# ImGui Rendering Plugin - Unity Runtime

[Dear ImGui](https://github.com/ocornut/imgui) within Unity injected at runtime, using [ImGui.NET](https://github.com/mellinoe/ImGui.NET) and the [Unity Low-level native plug-in interface](https://docs.unity3d.com/Manual/NativePluginInterface.html). Although it's geared for injection at runtime, it can also be used within the Unity Editor. There's a few repo's that implement ImGui in Unity, however they only work within the Unity Editor, none support runtime injection.

Use's Dear ImGui v1.84 with docking and viewport support. At this time v1.85 breaks the input in Unity, but I'm working on that. v1.85 doesn't really include any new features but it has a bunch of touch-ups and some fixes for some issues. Working on adding ImPlot, ImGizmos, and ImNodes.

Includes Plugins Patching tool to patch the game to preload/initialize the plugin for newer Unity versions, otherwise the plugin will never get the required UnityInterfaces pointer from Unity. Without the interfaces pointer, this wouldn't work as a Native Rendering Plugin since Unity provides no way to obtain it. If you build a game, no problem, Unity will add the reference's to the build, however, when injecting into a game there's no reference to the plugin so Unity doesn't automatically call UnityPluginLoad. This tool will add those refences. Run it with path to gme data folder if the API check fails when you load the plugin. Source not included.

Example projects for both IL2Cpp and Mono are included. They were designed to support BepInEx, however you can use other loader/injection methods. Some good code example's for building window components can be found here at this excellent repo: [UImGui](https://github.com/psydack/uimgui) (If your using in the Unity Editor, I highly recommend using this project)

Testing done on Unity versions 2018.1 through Current. Require's a game using .NetStandard or .Net 4, since base libraries don't support .Net 3.5. Does not support 32bit yet. Render support includes Direct11/12, Vulkan, and OpenGL. I haven't been able to test on Vulkan or OpenGL since I don't have a device with those SDK's. Base libraries are built with NetStandard 2.0 some it shoulld support most standard .Net Framwork versions.

Note: There's a slight clipping issue in DirectX11 .cpp, something with the Scissors Rect. If someone the knows DirectX better than I, I'm open to comments.

![alt text](https://i.imgur.com/TRN03cZ.png)

![alt text](https://i.imgur.com/Kd8qAcW.png)

Usage:
   Place cimgui.dll and ImGuiRenderingPlugin.dll into the game's Data\Plugins folder (Either X86 or X86_64).
   Place Trainer.dll and System.Runtime.CompilerServices.Unsafe.dll into BepInEx\plugins folder (Some Mono Unity versions will also require System.Runtime.dll) These libs are included in the BepInEx Dependencies.rar file under the libs folder.
