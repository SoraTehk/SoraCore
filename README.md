# SoraCore
Version: **x**(Major).**y**(Minor).**z**(Fixes)

## How to install (via UPM)
1. Install [MyBox](https://github.com/Deadcows/MyBox) into the project
2. Install this

## Dependencies (tested version, might work for lower)
* MyBox v1.7.0: https://github.com/Deadcows/MyBox
* Addressable v1.19.18: https://docs.unity3d.com/Packages/com.unity.addressables@lastest
* Input System v1.2.0: https://docs.unity3d.com/Packages/com.unity.inputsystem@lastest
* Cinemachine v2.8.4: https://docs.unity3d.com/Packages/com.unity.cinemachine@lastest

## Features
* Game Manager: Change game state, pause, resume, quit
* GameObject Manager: Create, destroy, pool, preload, callback.
  * Audio Manager: Play audio anywhere with MixerGroup & AudioConfiguration.
  * Level Manager: Handle scene loading more easily (add support for subscenes)
* UI Manager: Currently handle way to switch between UI Tookit documents.
* CameraFlyController: Mimic the scene view for runtime testing.