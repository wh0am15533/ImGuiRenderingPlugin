// Example low level rendering Unity plugin

#include "PlatformBase.h"
#include "RenderAPI.h"

#include "cimgui.h"

#include <assert.h>
#include <math.h>
#include <vector>
#include <string>

typedef void(__stdcall* DebugCallback) (const char* str, int type);
DebugCallback gDebugCallback;
static void DebugInUnity(std::string message, int type)
{
	if (gDebugCallback)
	{
		gDebugCallback(message.c_str(), type);
	}
}


// --------------------------------------------------------------------------
// UnitySetInterfaces

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType);

static IUnityInterfaces* s_UnityInterfaces = NULL;
static IUnityGraphics* s_Graphics = NULL;

static ImDrawData* s_drawData = NULL;
void CleanupDrawLists(ImDrawData* drawData)
{
	// D11.JN: CmdLists are normally managed internally by imgui, but we're cloning them so we need to take responsibility for cleanup
	if (drawData->CmdLists)
	{
		for (int i = 0; i < drawData->CmdListsCount; ++i)
		{
			ImDrawList__ClearFreeMemory(drawData->CmdLists[i]);
			igMemFree(drawData->CmdLists[i]);
		}
		delete[] drawData->CmdLists;
	}
}

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	DebugInUnity("wh0am15533 IMGUI Plugin Load", 1);
	s_UnityInterfaces = unityInterfaces;

	s_Graphics = s_UnityInterfaces->Get<IUnityGraphics>();
	s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

	s_drawData = new ImDrawData();

#if SUPPORT_VULKAN
	if (s_Graphics->GetRenderer() == kUnityGfxRendererNull)
	{
		extern void RenderAPI_Vulkan_OnPluginLoad(IUnityInterfaces*);
		RenderAPI_Vulkan_OnPluginLoad(unityInterfaces);
	}
#endif // SUPPORT_VULKAN

	// Run OnGraphicsDeviceEvent(initialize) manually on plugin load	
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	CleanupDrawLists(s_drawData);
	ImDrawData_destroy(s_drawData);

	s_Graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}

#if UNITY_WEBGL
typedef void	(UNITY_INTERFACE_API * PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API * PluginUnloadFunc)();

extern "C" void	UnityRegisterRenderingPlugin(PluginLoadFunc loadPlugin, PluginUnloadFunc unloadPlugin);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterPlugin()
{
	UnityRegisterRenderingPlugin(UnityPluginLoad, UnityPluginUnload);
}
#endif

// --------------------------------------------------------------------------
// GraphicsDeviceEvent

static RenderAPI* s_CurrentAPI = NULL;
static UnityGfxRenderer s_DeviceType = kUnityGfxRendererNull;

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	// Create graphics API implementation upon initialization
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		DebugInUnity("Initializing UnityGfxDevice", 1);

		assert(s_CurrentAPI == NULL);
		s_DeviceType = s_Graphics->GetRenderer();
		if (s_DeviceType == NULL) { DebugInUnity("Failed to get s_DeviceType!", 3); }

		s_CurrentAPI = CreateRenderAPI(s_DeviceType);
		if (s_CurrentAPI == NULL) { DebugInUnity("Failed to create s_CurrentAPI!", 3); }
	}

	// Let the implementation process the device related events
	if (s_CurrentAPI)
	{
		s_CurrentAPI->ProcessDeviceEvent(eventType, s_UnityInterfaces);
	}

	// Cleanup graphics API implementation upon shutdown
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		delete s_CurrentAPI;
		s_CurrentAPI = NULL;
		s_DeviceType = kUnityGfxRendererNull;
	}
}

// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.
static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	// Unknown / unsupported graphics device type? Do nothing
	if (s_CurrentAPI == NULL)
		return;

	// Example actually call's Native DLL function's like DrawColoredTriangle

	if (s_drawData)
	{
		s_CurrentAPI->ProcessImGuiCommandList(s_drawData);
	}
}

// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.
extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}

// --------------------------------------------------------------------------
// Our Custom Functions for the Plugins

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API CheckAPI()
{
	DebugInUnity("=========================", 1);
	DebugInUnity("Internal API Check...", 1);

	std::string dType = "";
	switch (s_DeviceType)
	{
		case kUnityGfxRendererD3D11:
			dType = "DirectX D3D11";
			break;
		case kUnityGfxRendererD3D12:
			dType = "DirectX D3D12";
			break;
		case kUnityGfxRendererOpenGLCore:
			dType = "OpenGL Core";
			break;
		case kUnityGfxRendererVulkan:
			dType = "Vulkan";
			break;
		case kUnityGfxRendererNull:
			dType = "NULL";
			break;
	}

	bool errorFlag = false;
	if (s_UnityInterfaces == NULL) { errorFlag = true; DebugInUnity("s_UnityInterfaces = FAIL!", 3); } else { DebugInUnity("s_UnityInterfaces = OK!", 1); }
	if (s_Graphics == NULL) { errorFlag = true; DebugInUnity("s_Graphics = FAIL!", 3); } else { DebugInUnity("s_Graphics = OK!", 1); }
	if (s_drawData == NULL) { errorFlag = true; DebugInUnity("s_drawData = FAIL!", 3); } else { DebugInUnity("s_drawData = OK!", 1); }
	if (s_DeviceType == NULL) { errorFlag = true; DebugInUnity("s_DeviceType = FAIL!", 3); } else { DebugInUnity("s_DeviceType = " + dType, 1); }
	if (s_CurrentAPI == NULL) { errorFlag = true; DebugInUnity("s_CurrentAPI = FAIL!", 3); } else { DebugInUnity("s_CurrentAPI = OK!", 1); }

	if (errorFlag){ DebugInUnity("API Check Status = FAIL!", 3); } else { DebugInUnity("API Check Status = OK!", 1); }
	DebugInUnity("=========================", 1);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterDebugCallback(DebugCallback callback)
{
	if (callback)
	{
		gDebugCallback = callback;
	}
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SendImGuiDrawCommands(ImDrawData* inData)
{
	CleanupDrawLists(s_drawData);
	ImDrawData_Clear(s_drawData);

	if (inData)
	{
		s_drawData->Valid = inData->Valid;
		s_drawData->CmdListsCount = inData->CmdListsCount;
		s_drawData->TotalIdxCount = inData->TotalIdxCount;
		s_drawData->TotalVtxCount = inData->TotalVtxCount;
		s_drawData->DisplayPos = inData->DisplayPos;
		s_drawData->DisplaySize = inData->DisplaySize;
		s_drawData->FramebufferScale = inData->FramebufferScale;

		s_drawData->CmdLists = inData->CmdListsCount ? new ImDrawList*[inData->CmdListsCount] : NULL;

		for (int i = 0; i < inData->CmdListsCount; ++i)
		{
			s_drawData->CmdLists[i] = ImDrawList_CloneOutput(inData->CmdLists[i]);
		}
	}
}

extern "C" ImTextureID UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GenerateImGuiFontTexture(void* pixels, int width, int height, int bytesPerPixel)
{
	//DebugInUnity("GenerateImGuiFontTexture", 1);

	if (s_CurrentAPI != NULL)
	{
		try
		{
			return s_CurrentAPI->CreateImGuiFontsTexture(pixels, width, height, bytesPerPixel);
		}
		catch (const char* msg) { DebugInUnity(msg, 3); return NULL; }
	}
	else { DebugInUnity("s_CurrentAPI is NULL!", 3); return NULL; }
}

