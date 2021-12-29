#pragma once

#include "Unity/IUnityGraphics.h"

#include "cimgui.h"

#include <stddef.h>

struct IUnityInterfaces;

// Super-simple "graphics abstraction". This is nothing like how a proper platform abstraction layer would look like;
// all this does is a base interface for whatever our plugin sample needs. Which is only "draw some triangles"
// and "modify a texture" at this point.
//
// There are implementations of this base class for D3D9, D3D11, OpenGL etc.; see individual RenderAPI_* files.
class RenderAPI
{
public:
	virtual ~RenderAPI() { }


	// Process general event like initialization, shutdown, device loss/reset etc.
	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces) = 0;

	// Is the API using "reversed" (1.0 at near plane, 0.0 at far plane) depth buffer?
	// Reversed Z is used on modern platforms, and improves depth buffer precision.
	virtual bool GetUsesReverseZ() = 0;

	virtual ImTextureID CreateImGuiFontsTexture(void* pixels, int width, int height, int bytesPerPixel) = 0;
    virtual void ProcessImGuiCommandList(ImDrawData* drawData) = 0;

	virtual void FlipMatrix() = 0; // Add J.E
	virtual void ToggleScissors() = 0; // Add J.E
	//float matrix[4][4]; // Added J.E

	virtual void CreateResources() = 0; // Added to expose it for Scissor toggle. J.E
};


// Create a graphics API implementation instance for the given API type.
RenderAPI* CreateRenderAPI(UnityGfxRenderer apiType);

