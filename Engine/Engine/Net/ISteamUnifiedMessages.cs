// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamUnifiedMessages
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamUnifiedMessages
  {
    public abstract IntPtr GetIntPtr();

    public abstract ulong SendMethod(
      string pchServiceMethod,
      IntPtr pRequestBuffer,
      uint unRequestBufferSize,
      ulong unContext);

    public abstract bool GetMethodResponseInfo(
      ulong hHandle,
      ref uint punResponseSize,
      ref uint peResult);

    public abstract bool GetMethodResponseData(
      ulong hHandle,
      IntPtr pResponseBuffer,
      uint unResponseBufferSize,
      bool bAutoRelease);

    public abstract bool ReleaseMethod(ulong hHandle);

    public abstract bool SendNotification(
      string pchServiceNotification,
      IntPtr pNotificationBuffer,
      uint unNotificationBufferSize);
  }
}
