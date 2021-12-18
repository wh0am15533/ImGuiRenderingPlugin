# ImGui Rendering Plugin

[Dear ImGui](https://github.com/ocornut/imgui) within Unity thanks to [ImGui.NET](https://github.com/mellinoe/ImGui.NET) and the [Unity Low-level native plug-in interface](https://docs.unity3d.com/Manual/NativePluginInterface.html). 

Currently this is only a prototype showing that it can be done, but hopefully one day it will become a more robust drop in unity package for anyone to use.

To build use premake on the PluginSource/source folder. Built assemblies will be placed into the unity project on build.

ImGUI.NET is compiled within unity using an asmdef, as it uses unsafe c sharp code.


# TODO (Help Required)

 - [x] DX11 renderer.
 - [ ] Clean up the DX11 renderer.
 - [ ] Figure out how calling ImGui callbacks is going to work, as they currently crash unity.
 - [ ] Cleanup the project structure as a whole. (.gitignore is uploading things it shouldn't, ImGui.NET should be a submodule etc.)
 - [ ] DX12 renderer.
 - [ ] Vulkan renderer.
 - [ ] Metal renderer.
 - [ ] OpenGL renderer.
 - [ ] OpenGLES renderer.
 - [ ] Add VR support, by allowing ImGui to be rendered onto a Unity texture, and displayed within 3D space.
