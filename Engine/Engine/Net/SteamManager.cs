// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamManager
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class SteamManager
  {
    private uint appId;
    private SteamLobbyMatchMaker lobbyMatchmaker;
    private static SteamManager instance;
    private static int packID;

    public static SteamManager Instance
    {
      get
      {
        return SteamManager.instance;
      }
    }

    public SteamLobbyMatchMaker LobbyMatchmaker
    {
      get
      {
        return this.lobbyMatchmaker;
      }
    }

    public SteamManager(uint appId)
    {
      if (SteamManager.instance != null)
        throw new Exception();
      SteamManager.instance = this;
      this.appId = appId;
    }

    public bool Initialize(SteamWarningMessageHookDelegate messageHook)
    {
      if (!SteamAPI.Init(this.appId))
      {
        CoreGlobals.LogErrorMessage("Steam Error", "SteamAPI_Init() failed. Ensure Steam is running.");
        return false;
      }
      SteamAPI.SteamClient().SetWarningMessageHook(messageHook);
      this.lobbyMatchmaker = new SteamLobbyMatchMaker();
      SteamManager.packID = 16;
      return true;
    }
  }
}
