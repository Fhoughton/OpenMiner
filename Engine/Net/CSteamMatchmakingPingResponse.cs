// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMatchmakingPingResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamMatchmakingPingResponse : ISteamMatchmakingPingResponse
  {
    private IntPtr m_pSteamMatchmakingPingResponse;

    public CSteamMatchmakingPingResponse(IntPtr SteamMatchmakingPingResponse)
    {
      this.m_pSteamMatchmakingPingResponse = SteamMatchmakingPingResponse;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMatchmakingPingResponse;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMatchmakingPingResponse == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override void ServerResponded(IntPtr server)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingPingResponse_ServerResponded(this.m_pSteamMatchmakingPingResponse, server);
    }

    public override void ServerFailedToRespond()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingPingResponse_ServerFailedToRespond(this.m_pSteamMatchmakingPingResponse);
    }
  }
}
