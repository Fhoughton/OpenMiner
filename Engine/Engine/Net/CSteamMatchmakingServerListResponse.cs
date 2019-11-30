// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMatchmakingServerListResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamMatchmakingServerListResponse : ISteamMatchmakingServerListResponse
  {
    private IntPtr m_pSteamMatchmakingServerListResponse;

    public CSteamMatchmakingServerListResponse(IntPtr SteamMatchmakingServerListResponse)
    {
      this.m_pSteamMatchmakingServerListResponse = SteamMatchmakingServerListResponse;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMatchmakingServerListResponse;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMatchmakingServerListResponse == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override void ServerResponded(uint hRequest, int iServer)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServerListResponse_ServerResponded(this.m_pSteamMatchmakingServerListResponse, hRequest, iServer);
    }

    public override void ServerFailedToRespond(uint hRequest, int iServer)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServerListResponse_ServerFailedToRespond(this.m_pSteamMatchmakingServerListResponse, hRequest, iServer);
    }

    public override void RefreshComplete(uint hRequest, uint response)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServerListResponse_RefreshComplete(this.m_pSteamMatchmakingServerListResponse, hRequest, response);
    }
  }
}
