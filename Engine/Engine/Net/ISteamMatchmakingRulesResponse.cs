// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMatchmakingRulesResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMatchmakingRulesResponse
  {
    public abstract IntPtr GetIntPtr();

    public abstract void RulesResponded(string pchRule, string pchValue);

    public abstract void RulesFailedToRespond();

    public abstract void RulesRefreshComplete();
  }
}
