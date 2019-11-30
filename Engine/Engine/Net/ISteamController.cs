// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamController
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamController
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool Init();

    public abstract bool Shutdown();

    public abstract void RunFrame();

    public abstract int GetConnectedControllers(ulong[] handlesOut);

    public abstract bool ShowBindingPanel(ulong controllerHandle);

    public abstract ulong GetActionSetHandle(string pszActionSetName);

    public abstract void ActivateActionSet(ulong controllerHandle, ulong actionSetHandle);

    public abstract ulong GetCurrentActionSet(ulong controllerHandle);

    public abstract ulong GetDigitalActionHandle(string pszActionName);

    public abstract ControllerDigitalActionData_t GetDigitalActionData(
      ulong controllerHandle,
      ulong digitalActionHandle);

    public abstract int GetDigitalActionOrigins(
      ulong controllerHandle,
      ulong actionSetHandle,
      ulong digitalActionHandle,
      EControllerActionOrigin[] originsOut);

    public abstract ulong GetAnalogActionHandle(string pszActionName);

    public abstract ControllerAnalogActionData_t GetAnalogActionData(
      ulong controllerHandle,
      ulong analogActionHandle);

    public abstract int GetAnalogActionOrigins(
      ulong controllerHandle,
      ulong actionSetHandle,
      ulong analogActionHandle,
      EControllerActionOrigin[] originsOut);

    public abstract void StopAnalogActionMomentum(ulong controllerHandle, ulong eAction);

    public abstract void TriggerHapticPulse(
      ulong controllerHandle,
      ESteamControllerPad eTargetPad,
      ushort usDurationMicroSec);
  }
}
