# ImGui Rendering Plugin

[Dear ImGui](https://github.com/ocornut/imgui) within Unity thanks to [ImGui.NET](https://github.com/mellinoe/ImGui.NET) and the [Unity Low-level native plug-in interface](https://docs.unity3d.com/Manual/NativePluginInterface.html). 

To build use premake on the PluginSource/source folder. Built assemblies will be placed into the unity project on build.

# TODO (Help Required)

 - [x] DX11 renderer.
 - [x] Clean up the DX11 renderer.
 - [x] Figure out how calling ImGui callbacks is going to work, as they currently crash unity.
 - [x] Cleanup the project structure as a whole. (.gitignore is uploading things it shouldn't, ImGui.NET should be a submodule etc.)
 - [x] DX12 renderer.
 - [ ] Vulkan renderer.
 - [ ] Metal renderer.
 - [x] OpenGL renderer.
 - [ ] OpenGLES renderer.
 - [ ] Add VR support, by allowing ImGui to be rendered onto a Unity texture, and displayed within 3D space.
