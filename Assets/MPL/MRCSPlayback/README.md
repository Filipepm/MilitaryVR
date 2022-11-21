Introduction 
============

Unity plugin for Video Hologram (HCap) playback. This contains built binaries of the native plugin libraries
for playing back the HCap file format, called SVF, as well as the C# scripts that directly interface to the native plugin.

Unity version 2017.3 or higher is required. The latest version tested with is 2018.2.

This plugin supports 32-bit and 64-bit Windows and UWP, as well as Android, macOS and iOS.
Note that if you are using other 3rd party plugins in your project that also overrides
the UnityAppController, you will have to resolve the conflict by subclassing one to the other,
or copying the implementations over from one to the other.

##### Magic Leap
The Magic Leap plugin was built and tested with Lumin SDK v0.21.0 and Unity 2019.1.0f2

##### Android
- API version 21 or higher is required, as is support for OpenGL ES 3.0 or higher and the GL\_OES\_EGL\_image\_external extension.
- Note that the plugin does *not* support Vulkan. You may need to uncheck the `Auto Graphics API` option in the Unity Android settings and ensure that `OpenGLES3` is at the top of the `Graphics APIs` list to prevent Unity from defaulting to Vulkan.

##### iOS
Supports iOS 9.0 and higher and requires support for Metal graphics API.

##### macOS
Supports macOS 10.13 and higher and requires support for Metal graphics API. 

Note that the "Metal Editor Support" option must be enabled in for the plugin to function properly in the Unity Editor on macOS. 


The native plugin is built with Visual Studio 2017 and requires the Microsoft Visual C++ Redistributable for Visual Studio 2017,
which can be obtained from <https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads>


Unity Plugin Organization
==========================

#### Assets/SVFUnityPlugin

All of this folder's content is what goes into SVFUnityPlugin Unity package. Contains the native Unity plugin
libraries that support playback of Video Hologram content as well as any relevant C# scripts.

- Platforms

  Native libraries for iOS, Android, macOS and [64/32]-bit UWP/Windows apps

- Prefabs/HoloVideoExamplePrefab

  Prefab object created with Scripts/HoloVideoObject.cs and Scripts/SVFUnityPluginInterop.cs.
  Use for a quick start project. Drag and drop in to scene and specify .mp4 path here

- Prefabs/HoloVideoPreviewPrefab

  A version of HoloVideoExamplePrefab that generates a preview in the editor window
  (Requires playing the project once)

- Scripts/HVConductor

  Used for async opening/closing in a linear sequence of videos.

- Shaders & Materials

 For demonstration, unlit (with shadow) and lit (with emissive mix) surface shaders are included.
 When making your own custom shaders, you must have a "\_MainTexture" field on your shader.


# Using HoloVideo prefab
Drag and drop SVFUnityPlugin/Prefabs/HoloVideoExamplePrefab into the scene and specify the video path under 'Url' field. This may be relative path from *StreamingAssets* directory or absolute path.

Only sequential playback is supported.

Playback speed may be adjusted by calling SetClockScale function. Note that changing the clock scale once playback has started does not
have the same effect as a typical fast-forward or slow-motion control. The next frame of content to be played is determined by the elapsed
time since the beginning of playback multiplied by the clock scale. For example, after one second of playback of 30fps content, setting
the clock scale to 2.0 does not mean that playback will continue at frame 31, but rather frame 60 will be played next.

# Asynchronous loading of videos on background thread
Opening a new HoloVideo on the main thread can briefly cause frame drops while it's being initialized.
To mitigate this, we can use HVConductor. This feature will require using .NET 4.6 runtime which is only 
available in Unity 2017 and newer, and is considered experimental. To use it, add the HoloVideoObjects to
a sequence in HVConductor's Sequence List. A sequence will be loaded/unloaded and played together. 
GoToNextSequence() plays next set of videos and starts asynchronously loading the next sequence.

**FOR UWP:** Add USE\_ASYNC to Scripting Define Symbols under Player Settings even if you changed the script runtime to 4.6.

# HoloVideo Object Settings

### General Settings

The following settings are useful:

- **Audio Disabled** - disable playback of any audio tracks embedded in the SVF content.
- **Auto Looping** - restart playback at the beginning of the SVF content after reaching the end.
- **Should Auto Play** - Enable to start the video playback on Start

# Additional Notes

### iOS Plugin Registration

Due to the nature of the Unity native rendering plugin system on iOS, the plugin entrypoints must be manually registered with Unity
at app start-up. This is handled by the SVFUnityPluginAppController class located in SVFUnityPlugin/Platforms/iOS, which 
subclasses the default UnityAppController implementation. This file will be automatically included in the Xcode project exported by
Unity when deploying an iOS build. If a non-standard build configuration is used (such as embedding the Unity Xcode project into another
container application) care should be taken to ensure same registration step is executed in the final application or the plugin will fail
to start.

### git LFS
If you're obtaining this directly from the HCap team's git repository, note that our git repo uses the Large File Storage 
(LFS) extension. Make sure that you have obtained and installed git-lfs before cloning the repo.
We've had good results with git versions 2.10 and newer, combined with git-lfs versions 1.5.6 or 2.0.1. If you have an
older version of git and run into problems during the clone, try updating your git version.

### Enabling Editor Coroutines
The geometry in Editor Scene view does not update if Game view is not visible without an experimental Editor Coroutines plugin
available in the package manager. Install that plugin and add USE\_EDITOR\_COROUTINE to Scripting Define Symbols under Player Settings
if you prefer working with just Scene view visible.

# Current changelog

## 1.4.6 - 2020-02-20

Fixed a bug with not being able to open external files on Android 10

## 1.4.5 - 2019-08-22

A memory leak has been fixed when destroying or re-using HoloVideoObject instances without explicitly calling Close(). This affected Windows and UWP platforms only.

## 1.4.4 - 2019-07-29

Support has been added for playing captures from the Android obb (APK Expansion) file when the Unity "Split Application Binary" option is enabled.

## 1.4.3 - 2019-06-6

Support for Magic Leap "Unity Play Mode" has been added for Windows only. A graphics card which supports the WGL_NV_DX_interop
OpenGL extension is required. Also note that the 'compute normals' feature has not been implemented for this mode so rendering 
artifacts may be observed for scenes that rely on this feature in Unity Play Mode.

## 1.4.3 - 2019-06-12
Fixed crash on Android if attempting to open an invalid file
Fixed HoloVideoObject always destroying underlying HCapObject when it can reuse
Made Editor Coroutines plugin optional for use in Scene view updates

## 1.4.2 - 2019-05-14

- Fixed DisplayFrame() not working
- Fixed Pause not properly updating IsPlaying status

## 1.4.1 - 2019-04-30

- Fixed a bug where opening a new capture on an existing HoloVideoObject instance would not play correctly on Android.
- Fixed a bug where texture atlas seams would sometimes be visible on non-power-of-2 captures on Android.
- Fixed geometry not updating in Unity in Editor's Scene if Game view is not visible

## 1.4.0 - 2019-04-18

ARM64 binaries are now included for Android. Versions of libSVFUnityPlugin.so for ARM and ARM64 architectures are now located in 
Platforms/Android/Plugins/ARM and Platforms/Android/Plugins/ARM64 respectively. The existing libSVFUnityPlugin.so located at 
Platforms/Android/Plugins from earlier versions of the plugin should be manually deleted when upgrading.

## 1.3.7 - 2019-03-08

The SVFOpenInfo.UseKeyedMutex option mentioned in the 1.3.6 release notes should no longer be necessary and it's use is now
discouraged for optimal performance and application memory footprint.

## 1.3.6 - 2019-01-18

Fixed a bug where captures would occasionally display an incorrect/corrupt texture on Windows (Desktop and UWP), 
particularly when system/device was under heavy load.

On some systems this fix may result in reduced performance. To revert to the previous behavior (at the risk of possible
texture sync issues) set SVFOpenInfo.UseKeyedMutex = false on each HoloVideoObject instance.

## 1.3.4 - 2019-01-2

Captures would sometimes display a completely white texture at the start of playback, this has been fixed in this release.

## 1.3.3 - 2018-12-6

Preliminary support has been added for Magic Leap in this release.

## 1.3.1 - 2018-11-14

Clock scale settings made before opening a capture had no effect previously, this has been fixed in this release.

## 1.3.0 - 2018-09-25

Breaking changes have been made in this release: 'SVF Draw mode' has been removed as has 'Cached mode'.

When updating to version 1.3.0 from a previous release the file CameraViews.cs (in SVFUnityPlugin/Scripts) should be deleted as it is 
no longer part of the plugin and will cause build errors if left in the project.


