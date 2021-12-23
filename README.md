# ImGui Rendering Plugin

[Dear ImGui](https://github.com/ocornut/imgui) within Unity injected at runtime, using [ImGui.NET](https://github.com/mellinoe/ImGui.NET) and the [Unity Low-level native plug-in interface](https://docs.unity3d.com/Manual/NativePluginInterface.html). 

Includes Plugins Patching tool to patch the game to preload the plugin for newer Unity versions, otherwise the plugin will never get the required UnityInterfaces pointer from Unity. Run this with path to gme data folder if the API check fails when you load the plugin.

Example projects for both IL2Cpp and Mono are included. They were designed to support BepInEx, however you can use other loader/injection methods.

![alt text](https://i.imgur.com/TRN03cZ.png)

![alt text](https://i.imgur.com/Kd8qAcW.png)
