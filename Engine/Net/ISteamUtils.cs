// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamUtils
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamUtils
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint GetSecondsSinceAppActive();

    public abstract uint GetSecondsSinceComputerActive();

    public abstract int GetConnectedUniverse();

    public abstract uint GetServerRealTime();

    public abstract string GetIPCountry();

    public abstract bool GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight);

    public abstract bool GetImageRGBA(int iImage, byte[] pubDest, int nDestBufferSize);

    public abstract bool GetCSERIPPort(ref uint unIP, ref char usPort);

    public abstract byte GetCurrentBatteryPower();

    public abstract uint GetAppID();

    public abstract void SetOverlayNotificationPosition(uint eNotificationPosition);

    public abstract bool IsAPICallCompleted(ulong hSteamAPICall, ref bool pbFailed);

    public abstract int GetAPICallFailureReason(ulong hSteamAPICall);

    public abstract bool GetAPICallResult(
      ulong hSteamAPICall,
      IntPtr pCallback,
      int cubCallback,
      int iCallbackExpected,
      ref bool pbFailed);

    public abstract void RunFrame();

    public abstract uint GetIPCCallCount();

    public abstract void SetWarningMessageHook(SteamWarningMessageHookDelegate hook);

    public abstract bool IsOverlayEnabled();

    public abstract bool BOverlayNeedsPresent();

    public abstract ulong CheckFileSignature(string szFileName);

    public abstract bool ShowGamepadTextInput(
      int eInputMode,
      int eLineInputMode,
      string pchDescription,
      uint unCharMax,
      string pchExistingText);

    public abstract uint GetEnteredGamepadTextLength();

    public abstract bool GetEnteredGamepadTextInput(string pchText, uint cchText);

    public abstract string GetSteamUILanguage();

    public abstract bool IsSteamRunningInVR();

    public abstract void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset);
  }
}
