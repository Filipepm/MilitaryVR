# Changelog
All previous notable changes to this project will be archived in this file.

## 1.2.18 - 2018-08-28

Fixed hang when playing capture with "Auto Looping" on iOS/macOS.

## 1.2.17 - 2018-08-22

Reduced memory footprint on iOS/macOS when playing multiple captures simultaneously.

## 1.2.16 - 2018-08-03

Minor performance improvements

## 1.2.15 - 2018-08-01
IL2CPP scripting backend now works properly in UWP
HoloVideoObject's TextureBuffer is now cleaned up during Close()
HoloVideoObject now calls Close OnDestroy()

## 1.2.14 - 2018-07-23
Removed vertex format version from frame info and added isKeyFrame flag
HoloVideoObject frees up texture memory on Close now

## 1.2.12 - 2018-06-29

### OpenAsync fixes
HoloVideoObject.OpenAsync has been fixed to work properly again in this version.

### Windows "auto looping" fix
Auto looping in the Windows plugin (Desktop and UWP) could sometimes cause audio and video to fall out of sync, and in other cases cause playback to hang completely.
This has been corrected in this version.

## 1.2.10 - 2018-06-12

### Memory leak fixed on iOS
A memory leak has been fixed when creating and destroying HoloVideoObject instances.

### Hang fixed when reusing HoloVideoObject on iOS
Reusing a HoloVideoObject to play new clips on iOS was causing a hang at the start of playback.

## 1.2.7 - 2018-04-25

### End of stream reporting
End of stream notifications should now be reported correctly by HoloVideoObject.OnEndOfStreamNotify on iOS and Android when plugin is in 'Auto Looping' mode.

## 1.2.6 - 2018-04-13

### WACK Compliance
The WSA plugin libraries should now pass the Windows Application Certification Kit "Supported API test". 
Previous versions of the plugin made use of some unsupported APIs which would cause WACK validation to fail.

### Android memory leak
In a relatively obscure case where periodic updates (Unity render events) were not being invoked on an actively playing HoloVideoObject,
internal data could build up endlessly resulting the application running out of memory and crashing. This has been fixed in this release.

## 1.2.4 - 2018-04-03

### Clockscale change on the fly
This patch adds ability to properly change clockspeed in the middle of playback (previously
we only supported setting this at the beginning). This change can cause issues with videos
with audio when it goes out of sync, and may require closing and reopening of the video.
This should work on all platforms.

### Async bug fixes
Asynchronous loading/unloading had hitch issues in Unity draw mode which has been resolved.

### iOS and Android promoted to beta

## 1.2.3 - 2018-03-16
### Preview Mode (Experimental)
This version adds a feature to see a preview of the hologram in Unity's editor mode.
You can use HoloVideoPreviewPrefab to try it out. You can set the Preview Frame field
in HoloVideoPreview.cs(attached to the prefab as a child) to modify the frame that will
be previewed. It will require at least one play to capture a frame of the mesh, and
another one play to capture the texture -- this is still an experimental feature. If it
is being stubborn, try deleting the assets under Assets/SVFUnityPlugin/PreviewAssets/.
To disable it, you can disable the prefab's child game object (HoloVideoPreview).

### DisplayFrame (Previously BurstPlay)
DisplayFrame() on HoloVideoObject will now allow opening the video and pausing it in the
first X frame. The stopping frame can be set on the HoloVideoObject or as an optional
parameter to DisplayFrame(). This functionality should work on Windows, iOS, Android and
macOS while working with Unity.

## 1.2.1, 1.2.2
Undocumented iOS and Android bug fixes

## 1.2.0 - 2018-02-16

### Android support has been added to the plugin in this release
API version 21 or higher is required, as is support for OpenGL ES 3.0 or higher and the GL\_OES\_EGL\_image\_external extension.

### iOS support has been added to the plugin in this release
Support for the Metal graphics API is required.

## Older than 1.2.0

### 3D Audio
This version includes support for 3D Audio.  The location of the HCap audio source will be updated on every frame to be correct relative to the Audio Listener object in the Unity scene.
The **Audio Source Offset** setting on a Holo Video Object allows the apparent source of HCap audio to be offset from the local origin of the object.

### Flip Handedness
This setting on a Holo Video Object causes the HCap content rendering to be flipped in X, i.e. mirrored about the Y-Z plane.  In previous versions this mirroring would fail to render correctly
under some transformations of the object.  That is now fixed.  Note that this setting only has an effect in **SVF Draw** mode.

### Auto Device Detection
When rendering in **SVF Draw** mode, the **Target Device** setting on a Holo Video Object must be set correctly for the HCap to be rendered in the correct coordinate system.  
Setting this incorrectly may result in the
HCap being rendered upside-down, either in the Unity editor or in the game window.  For most uses, the *Auto Detect* setting should work, and this is the default.  If automatic detection
does not work for some reason, a specific device (Windows Mixed Reality headset, HoloLens, Vive, Oculus, etc.) can be selected to override the automatic detection.  The automatic detection
should work correctly for all known devices on any version of Unity, but the internal logic does differ from an earlier version of this package that supported Unity 5.5 only.  If, for compatibility
with an existing project, the old behavior is needed, then there is a *Unity 55 PC* setting that will reproduce the old behavior.  This should not be necessary for new projects.

### Texture Corruption Fixed
In previous versions of the plugin, playback could end on a corrupted texture on some platforms.  This has been fixed.  A previous workaround existed in the form of a setting on the Holo Video Object script in Unity called **Render Last Frames Transparent**.  This setting is still supported, but should no longer be necessary.

### Audio Device Specification
This version supports manually selecting an output audio device for HCap audio through the SVFOpenInfo.AudioDeviceId setting. The list of available devices can be obtained through the GetAudioDevices function on the _HoloVideoObject_ class.



