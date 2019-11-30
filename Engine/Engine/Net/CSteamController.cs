// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamController
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamController : ISteamController
  {
    private IntPtr m_pSteamController;

    public CSteamController(IntPtr SteamController)
    {
      this.m_pSteamController = SteamController;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamController;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamController == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool Init()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_Init(this.m_pSteamController);
    }

    public override bool Shutdown()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_Shutdown(this.m_pSteamController);
    }

    public override void RunFrame()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamController_RunFrame(this.m_pSteamController);
    }

    public override int GetConnectedControllers(ulong[] handlesOut)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetConnectedControllers(this.m_pSteamController, handlesOut);
    }

    public override bool ShowBindingPanel(ulong controllerHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_ShowBindingPanel(this.m_pSteamController, controllerHandle);
    }

    public override ulong GetActionSetHandle(string pszActionSetName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetActionSetHandle(this.m_pSteamController, pszActionSetName);
    }

    public override void ActivateActionSet(ulong controllerHandle, ulong actionSetHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamController_ActivateActionSet(this.m_pSteamController, controllerHandle, actionSetHandle);
    }

    public override ulong GetCurrentActionSet(ulong controllerHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetCurrentActionSet(this.m_pSteamController, controllerHandle);
    }

    public override ulong GetDigitalActionHandle(string pszActionName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetDigitalActionHandle(this.m_pSteamController, pszActionName);
    }

    public override ControllerDigitalActionData_t GetDigitalActionData(
      ulong controllerHandle,
      ulong digitalActionHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetDigitalActionData(this.m_pSteamController, controllerHandle, digitalActionHandle);
    }

    public override int GetDigitalActionOrigins(
      ulong controllerHandle,
      ulong actionSetHandle,
      ulong digitalActionHandle,
      EControllerActionOrigin[] originsOut)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetDigitalActionOrigins(this.m_pSteamController, controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
    }

    public override ulong GetAnalogActionHandle(string pszActionName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetAnalogActionHandle(this.m_pSteamController, pszActionName);
    }

    public override ControllerAnalogActionData_t GetAnalogActionData(
      ulong controllerHandle,
      ulong analogActionHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetAnalogActionData(this.m_pSteamController, controllerHandle, analogActionHandle);
    }

    public override int GetAnalogActionOrigins(
      ulong controllerHandle,
      ulong actionSetHandle,
      ulong analogActionHandle,
      EControllerActionOrigin[] originsOut)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamController_GetAnalogActionOrigins(this.m_pSteamController, controllerHandle, actionSetHandle, analogActionHandle, originsOut);
    }

    public override void StopAnalogActionMomentum(ulong controllerHandle, ulong eAction)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamController_StopAnalogActionMomentum(this.m_pSteamController, controllerHandle, eAction);
    }

    public override void TriggerHapticPulse(
      ulong controllerHandle,
      ESteamControllerPad eTargetPad,
      ushort usDurationMicroSec)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamController_TriggerHapticPulse(this.m_pSteamController, controllerHandle, eTargetPad, usDurationMicroSec);
    }
  }
}
