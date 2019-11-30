// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMatchmakingServerListResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMatchmakingServerListResponse
  {
    public abstract IntPtr GetIntPtr();

    public abstract void ServerResponded(uint hRequest, int iServer);

    public abstract void ServerFailedToRespond(uint hRequest, int iServer);

    public abstract void RefreshComplete(uint hRequest, uint response);
  }
}
