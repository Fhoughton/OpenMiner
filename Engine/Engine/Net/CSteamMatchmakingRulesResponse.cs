// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMatchmakingRulesResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamMatchmakingRulesResponse : ISteamMatchmakingRulesResponse
  {
    private IntPtr m_pSteamMatchmakingRulesResponse;

    public CSteamMatchmakingRulesResponse(IntPtr SteamMatchmakingRulesResponse)
    {
      this.m_pSteamMatchmakingRulesResponse = SteamMatchmakingRulesResponse;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMatchmakingRulesResponse;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMatchmakingRulesResponse == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override void RulesResponded(string pchRule, string pchValue)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingRulesResponse_RulesResponded(this.m_pSteamMatchmakingRulesResponse, pchRule, pchValue);
    }

    public override void RulesFailedToRespond()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingRulesResponse_RulesFailedToRespond(this.m_pSteamMatchmakingRulesResponse);
    }

    public override void RulesRefreshComplete()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingRulesResponse_RulesRefreshComplete(this.m_pSteamMatchmakingRulesResponse);
    }
  }
}
