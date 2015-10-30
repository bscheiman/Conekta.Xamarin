using System;
using ObjCRuntime;

[assembly: LinkWith ("libDeviceCollectorLibrary.a", LinkTarget.Simulator | LinkTarget.ArmV7s | LinkTarget.Arm64, SmartLink = true, ForceLoad = true)]
