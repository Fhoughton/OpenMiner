// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamAppList
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamAppList
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint GetNumInstalledApps();

    public abstract uint GetInstalledApps(ref uint pvecAppID, uint unMaxAppIDs);

    public abstract int GetAppName(uint nAppID, string pchName, int cchNameMax);

    public abstract int GetAppInstallDir(uint nAppID, string pchDirectory, int cchNameMax);

    public abstract int GetAppBuildId(uint nAppID);
  }
}
