// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamScreenshots
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamScreenshots
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight);

    public abstract uint AddScreenshotToLibrary(
      string pchFilename,
      string pchThumbnailFilename,
      int nWidth,
      int nHeight);

    public abstract void TriggerScreenshot();

    public abstract void HookScreenshots(bool bHook);

    public abstract bool SetLocation(uint hScreenshot, string pchLocation);

    public abstract bool TagUser(uint hScreenshot, ulong steamID);

    public abstract bool TagPublishedFile(uint hScreenshot, ulong unPublishedFileID);
  }
}
