// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamUnifiedMessages
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamUnifiedMessages : ISteamUnifiedMessages
  {
    private IntPtr m_pSteamUnifiedMessages;

    public CSteamUnifiedMessages(IntPtr SteamUnifiedMessages)
    {
      this.m_pSteamUnifiedMessages = SteamUnifiedMessages;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamUnifiedMessages;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamUnifiedMessages == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override ulong SendMethod(
      string pchServiceMethod,
      IntPtr pRequestBuffer,
      uint unRequestBufferSize,
      ulong unContext)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUnifiedMessages_SendMethod(this.m_pSteamUnifiedMessages, pchServiceMethod, pRequestBuffer, unRequestBufferSize, unContext);
    }

    public override bool GetMethodResponseInfo(
      ulong hHandle,
      ref uint punResponseSize,
      ref uint peResult)
    {
      this.CheckIfUsable();
      punResponseSize = 0U;
      peResult = 0U;
      return NativeCalls.SteamAPI_ISteamUnifiedMessages_GetMethodResponseInfo(this.m_pSteamUnifiedMessages, hHandle, ref punResponseSize, ref peResult);
    }

    public override bool GetMethodResponseData(
      ulong hHandle,
      IntPtr pResponseBuffer,
      uint unResponseBufferSize,
      bool bAutoRelease)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUnifiedMessages_GetMethodResponseData(this.m_pSteamUnifiedMessages, hHandle, pResponseBuffer, unResponseBufferSize, bAutoRelease);
    }

    public override bool ReleaseMethod(ulong hHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUnifiedMessages_ReleaseMethod(this.m_pSteamUnifiedMessages, hHandle);
    }

    public override bool SendNotification(
      string pchServiceNotification,
      IntPtr pNotificationBuffer,
      uint unNotificationBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUnifiedMessages_SendNotification(this.m_pSteamUnifiedMessages, pchServiceNotification, pNotificationBuffer, unNotificationBufferSize);
    }
  }
}
