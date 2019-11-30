// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamUtils
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamUtils : ISteamUtils
  {
    private IntPtr m_pSteamUtils;

    public CSteamUtils(IntPtr SteamUtils)
    {
      this.m_pSteamUtils = SteamUtils;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamUtils;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamUtils == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint GetSecondsSinceAppActive()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetSecondsSinceAppActive(this.m_pSteamUtils);
    }

    public override uint GetSecondsSinceComputerActive()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetSecondsSinceComputerActive(this.m_pSteamUtils);
    }

    public override int GetConnectedUniverse()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetConnectedUniverse(this.m_pSteamUtils);
    }

    public override uint GetServerRealTime()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetServerRealTime(this.m_pSteamUtils);
    }

    public override string GetIPCountry()
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamUtils_GetIPCountry(this.m_pSteamUtils));
    }

    public override bool GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight)
    {
      this.CheckIfUsable();
      pnWidth = 0U;
      pnHeight = 0U;
      return NativeCalls.SteamAPI_ISteamUtils_GetImageSize(this.m_pSteamUtils, iImage, ref pnWidth, ref pnHeight);
    }

    public override bool GetImageRGBA(int iImage, byte[] pubDest, int nDestBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetImageRGBA(this.m_pSteamUtils, iImage, pubDest, nDestBufferSize);
    }

    public override bool GetCSERIPPort(ref uint unIP, ref char usPort)
    {
      this.CheckIfUsable();
      unIP = 0U;
      usPort = char.MinValue;
      return NativeCalls.SteamAPI_ISteamUtils_GetCSERIPPort(this.m_pSteamUtils, ref unIP, ref usPort);
    }

    public override byte GetCurrentBatteryPower()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetCurrentBatteryPower(this.m_pSteamUtils);
    }

    public override uint GetAppID()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetAppID(this.m_pSteamUtils);
    }

    public override void SetOverlayNotificationPosition(uint eNotificationPosition)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUtils_SetOverlayNotificationPosition(this.m_pSteamUtils, eNotificationPosition);
    }

    public override bool IsAPICallCompleted(ulong hSteamAPICall, ref bool pbFailed)
    {
      this.CheckIfUsable();
      pbFailed = false;
      return NativeCalls.SteamAPI_ISteamUtils_IsAPICallCompleted(this.m_pSteamUtils, hSteamAPICall, ref pbFailed);
    }

    public override int GetAPICallFailureReason(ulong hSteamAPICall)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetAPICallFailureReason(this.m_pSteamUtils, hSteamAPICall);
    }

    public override bool GetAPICallResult(
      ulong hSteamAPICall,
      IntPtr pCallback,
      int cubCallback,
      int iCallbackExpected,
      ref bool pbFailed)
    {
      this.CheckIfUsable();
      pbFailed = false;
      return NativeCalls.SteamAPI_ISteamUtils_GetAPICallResult(this.m_pSteamUtils, hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
    }

    public override void RunFrame()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUtils_RunFrame(this.m_pSteamUtils);
    }

    public override uint GetIPCCallCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetIPCCallCount(this.m_pSteamUtils);
    }

    public override void SetWarningMessageHook(SteamWarningMessageHookDelegate pFunction)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUtils_SetWarningMessageHook(this.m_pSteamUtils, pFunction);
    }

    public override bool IsOverlayEnabled()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_IsOverlayEnabled(this.m_pSteamUtils);
    }

    public override bool BOverlayNeedsPresent()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_BOverlayNeedsPresent(this.m_pSteamUtils);
    }

    public override ulong CheckFileSignature(string szFileName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_CheckFileSignature(this.m_pSteamUtils, szFileName);
    }

    public override bool ShowGamepadTextInput(
      int eInputMode,
      int eLineInputMode,
      string pchDescription,
      uint unCharMax,
      string pchExistingText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_ShowGamepadTextInput(this.m_pSteamUtils, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
    }

    public override uint GetEnteredGamepadTextLength()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetEnteredGamepadTextLength(this.m_pSteamUtils);
    }

    public override bool GetEnteredGamepadTextInput(string pchText, uint cchText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_GetEnteredGamepadTextInput(this.m_pSteamUtils, pchText, cchText);
    }

    public override string GetSteamUILanguage()
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamUtils_GetSteamUILanguage(this.m_pSteamUtils));
    }

    public override bool IsSteamRunningInVR()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUtils_IsSteamRunningInVR(this.m_pSteamUtils);
    }

    public override void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUtils_SetOverlayNotificationInset(this.m_pSteamUtils, nHorizontalInset, nVerticalInset);
    }
  }
}
