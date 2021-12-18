workspace "UnityImGui"
    configurations {"Debug", "Release"}
    architecture "x64"
group "Dependencies"
    include "vendor/CImGui"
group ""
project "UnityImGuiRenderer"
    kind "SharedLib"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"

    filter {"system:windows", "configurations:Release"}
        targetdir "../win/release"
    filter {"system:windows", "configurations:Debug"}
        targetdir "../win/debug"
    filter{}

    objdir "projectfiles/"
    location "."
    files {"**.h", "**.hpp", "**.c","**.cpp"}

    includedirs{ "../cimgui"}

    defines{ "CIMGUI_DEFINE_ENUMS_AND_STRUCTS" }

    links{"cimgui"}

    filter {"system:windows", "configurations:Release"}
        libdirs{"../cimgui/win/release"}
    filter {"system:windows", "configurations:Debug"}
        libdirs{"../cimgui/win/debug"}
    filter {}

    links {"CImGuiShared"}
    filter "system:windows"
        systemversion "latest"
        links {"opengl32"}
    filter "configurations:Debug"
      symbols "On"
   filter "configurations:Release"
      optimize "On"
