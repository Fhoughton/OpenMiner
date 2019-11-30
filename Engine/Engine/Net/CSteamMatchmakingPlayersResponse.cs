// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMatchmakingPlayersResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamMatchmakingPlayersResponse : ISteamMatchmakingPlayersResponse
  {
    private IntPtr m_pSteamMatchmakingPlayersResponse;

    public CSteamMatchmakingPlayersResponse(IntPtr SteamMatchmakingPlayersResponse)
    {
      this.m_pSteamMatchmakingPlayersResponse = SteamMatchmakingPlayersResponse;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMatchmakingPlayersResponse;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMatchmakingPlayersResponse == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override void AddPlayerToList(string pchName, int nScore, float flTimePlayed)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingPlayersResponse_AddPlayerToList(this.m_pSteamMatchmakingPlayersResponse, pchName, nScore, flTimePlayed);
    }

    public override void PlayersFailedToRespond()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingPlayersResponse_PlayersFailedToRespond(this.m_pSteamMatchmakingPlayersResponse);
    }

    public override void PlayersRefreshComplete()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingPlayersResponse_PlayersRefreshComplete(this.m_pSteamMatchmakingPlayersResponse);
    }
  }
}
