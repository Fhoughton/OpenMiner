// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMatchmakingPlayersResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMatchmakingPlayersResponse
  {
    public abstract IntPtr GetIntPtr();

    public abstract void AddPlayerToList(string pchName, int nScore, float flTimePlayed);

    public abstract void PlayersFailedToRespond();

    public abstract void PlayersRefreshComplete();
  }
}
