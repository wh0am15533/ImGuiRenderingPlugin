
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
// Our Custom Functions for the Plugin

// ref: https://answers.unity.com/questions/1184386/calling-unitypluginload-manually.html
extern "C" uint64_t UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetUnityInterfacesPtr()
{
	return reinterpret_cast<uint64_t>(s_UnityInterfaces);
}
/*
extern "C" IUnityInterfaces* UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetUnityInterfacesPtr()
{
	return s_UnityInterfaces;
}
*/

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API CheckAPI()
{
	DebugInUnity("wh0am15533 IMGUI Plugin", 1);
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

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API FlipMatrix()
{
	DebugInUnity("Flip Matrix", 1);
	s_CurrentAPI->FlipMatrix();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ToggleScissors()
{
	DebugInUnity("Toggle Scissors", 1);
	s_CurrentAPI->ToggleScissors();
	s_CurrentAPI->CreateResources();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetScissorRect()
{
	for (int i = 0; i < s_drawData->CmdListsCount; ++i)
	{
		// Test Scissor Clip Rect Size J.E
		const ImDrawList* cmd_list = s_drawData->CmdLists[i];
		for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
		{
			const ImDrawCmd* pcmd = &cmd_list->CmdBuffer.Data[cmd_i];
			if (pcmd->UserCallback)
			{
				//pcmd->UserCallback(cmd_list, pcmd);
			}
			else
			{
				char left[10]; //size of the number
				sprintf(left, "%g", pcmd->ClipRect.x);

				char top[10];
				sprintf(top, "%g", pcmd->ClipRect.y);

				char right[10];
				sprintf(right, "%g", pcmd->ClipRect.z);

				char bottom[10];
				sprintf(bottom, "%g", pcmd->ClipRect.w);

				char str1[80];
				strcpy(str1, "Left: ");
				strcat(str1, left);
				strcat(str1, " Top: ");
				strcat(str1, top);
				strcat(str1, " Right: ");
				strcat(str1, right);
				strcat(str1, " Bottom: ");
				strcat(str1, bottom);

				DebugInUnity(str1, 1);
			}
		}
	}
}

/*
extern "C" uint64_t UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetMatrix()
{
	DebugInUnity("Getting Matrix", 1);

	float L = 0.0f;
	float R = s_drawData->DisplaySize.x;
	float B = s_drawData->DisplaySize.y;
	float T = 0.0f;

	float depth = 0.7f;
	float finalDepth = s_CurrentAPI->GetUsesReverseZ() ? 1.0f - depth : depth;
	float mvpInvertY[4][4] =
	{
		{ 2.0f / (R - L), 0.0f, 0.0f, 0.0f },
		{ 0.0f, -(2.0f / (T - B)), 0.0f, 0.0f },
		{ 0.0f, 0.0f, 1.0f, 0.0f },
		{ (R + L) / (L - R), -((T + B) / (B - T)), finalDepth, 1.0f },
	};

	char lineOne[35];
	strcpy(lineOne, "Line 1: ");
	char lineOneVal1[10];
	sprintf(lineOneVal1, "%g", 2.0f / (R - L));
	strcat(lineOne, lineOneVal1);
	strcat(lineOne, "f, 0.0f, 0.0f, 0.0f");
	DebugInUnity(lineOne, 2);

	char lineTwo[35];
	strcpy(lineTwo, "Line 2: 0.0f, ");
	char lineTwoVal2[10];
	sprintf(lineTwoVal2, "%g", 2.0f / (T - B));//-(2.0f / (T - B))
	strcat(lineTwo, lineTwoVal2);
	strcat(lineTwo, "f, 0.0f, 0.0f");
	DebugInUnity(lineTwo, 2);

	DebugInUnity("Line 3: 0.0f, 0.0f, 1.0f, 0.0f", 2);

	char lineFour[35];
	strcpy(lineFour, "Line 4: ");
	char lineFourVal1[10];
	sprintf(lineFourVal1, "%g", (R + L) / (L - R));
	strcat(lineFour, lineFourVal1);
	strcat(lineFour, "f, ");
	char lineFourVal2[10];
	sprintf(lineFourVal2, "%g", -((T + B) / (B - T)));
	strcat(lineFour, lineFourVal2);
	strcat(lineFour, "f, ");
	char lineFourVal3[10];
	sprintf(lineFourVal3, "%g", finalDepth);
	strcat(lineFour, lineFourVal3);
	strcat(lineFour, "f, 1.0f");
	DebugInUnity(lineFour, 2);

	// Orig Matrix
	float mvp[4][4] =
	{
		{ 2.0f / (R - L), 0.0f, 0.0f, 0.0f },
		{ 0.0f, 2.0f / (T - B), 0.0f, 0.0f },
		{ 0.0f, 0.0f, 0.5f, 0.0f },
		{ (R + L) / (L - R), (T + B) / (B - T), 0.5f, 1.0f },
	};

	char olineOne[35];
	strcpy(olineOne, "Line 1: ");
	char olineOneVal1[10];
	sprintf(olineOneVal1, "%g", 2.0f / (R - L));
	strcat(olineOne, olineOneVal1);
	strcat(olineOne, "f, 0.0f, 0.0f, 0.0f");
	DebugInUnity(olineOne, 2);

	char olineTwo[35];
	strcpy(olineTwo, "Line 2: 0.0f, ");
	char olineTwoVal2[10];
	sprintf(olineTwoVal2, "%g", 2.0f / (T - B));
	strcat(olineTwo, olineTwoVal2);
	strcat(olineTwo, "f, 0.0f, 0.0f");
	DebugInUnity(olineTwo, 2);

	DebugInUnity("Line 3: 0.0f, 0.0f, 0.5f, 0.0f", 2);

	char olineFour[35];
	strcpy(olineFour, "Line 4: ");
	char olineFourVal1[10];
	sprintf(olineFourVal1, "%g", (R + L) / (L - R));
	strcat(olineFour, olineFourVal1);
	strcat(olineFour, "f, ");
	char olineFourVal2[10];
	sprintf(olineFourVal2, "%g", (T + B) / (B - T));
	strcat(olineFour, olineFourVal2);
	strcat(olineFour, "f, 0.5f, 1.0f");
	DebugInUnity(olineFour, 2);

	return reinterpret_cast<uint64_t>(s_CurrentAPI->matrix);
}
*/