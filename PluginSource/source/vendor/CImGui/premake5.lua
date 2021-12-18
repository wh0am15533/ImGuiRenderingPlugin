-- Premake5 file for CImGui, Damien Henderson 2020
-- In this fake vendor folder to avoid needing a fork of the repo for the submodule

CImGuiPath = "../../../cimgui/"

function MakePath(relativePath)
    return CImGuiPath .. relativePath
end
function BoilerPlate()
    language "C"
    
    objdir(MakePath("build/%{prj.name}/Obj/%{cfg.buildcfg}"))
    location(MakePath("build/%{prj.name}"))

    staticruntime "on"
    defines{"IMGUI_DISABLE_OBSOLETE_FUNCTIONS=1"}

    filter "system:windows"
        systemversion "latest"
    filter {"system:windows", "kind:SharedLib"}
        defines{"IMGUI_IMPL_API=extern \"C\" __declspec(dllexport)"}
    filter "system:not windows"
        defines{"IMGUI_IMPL_API=extern \"C\""}
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
        optimize "On"
    filter {"configurations:Debug", "system:windows"}
        targetdir(MakePath("win/debug"))
        filter {"configurations:Release", "system:windows"}
        targetdir(MakePath("win/release"))
    filter{}

    

    includedirs {MakePath(""), MakePath("imgui")}
    files {MakePath("*.cpp"), MakePath("imgui/*.cpp")}
    targetname "cimgui"
end


    project "CImGuiStatic"
        kind "StaticLib"
        BoilerPlate()        
    project "CImGuiShared"
        kind "StaticLib"
        BoilerPlate()