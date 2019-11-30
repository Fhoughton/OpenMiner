// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamLobbyMatchMaker
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class SteamLobbyMatchMaker
  {
    private ELobbyType LobbyPublicity = ELobbyType.k_ELobbyTypePublic;
    private const int MAX_LOBBY_MEMBERS = 24;
    private ulong currentLobbyID;
    private SteamCallResult<LobbyMatchList_t> callResultLobbyMatchList;
    private SteamCallResult<LobbyCreated_t> callResultLobbyCreated;
    private SteamCallResult<LobbyEnter_t> callResultLobbyEnter;
    private SteamCallback<GameLobbyJoinRequested_t> callbackJoinRequest;
    private SteamCallback<LobbyChatMsg_t> callbackLobbyChatMsg;
    private SteamCallback<LobbyChatUpdate_t> callbackLobbyChatUpdate;
    private SteamCallback<LobbyDataUpdate_t> callbackLobbyDataUpdate;

    public ulong CurrentLobbyID
    {
      get
      {
        return this.currentLobbyID;
      }
      private set
      {
        this.currentLobbyID = value;
      }
    }

    public void Initialize()
    {
      this.callResultLobbyMatchList = new SteamCallResult<LobbyMatchList_t>(new SteamCallResult<LobbyMatchList_t>.DispatchDelegate(this.OnLobbyMatchList));
      this.callResultLobbyCreated = new SteamCallResult<LobbyCreated_t>(new SteamCallResult<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
      this.callResultLobbyEnter = new SteamCallResult<LobbyEnter_t>(new SteamCallResult<LobbyEnter_t>.DispatchDelegate(this.OnLobbyJoined));
      this.callbackJoinRequest = new SteamCallback<GameLobbyJoinRequested_t>(new SteamCallback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnInvite), false);
      this.callbackLobbyChatMsg = new SteamCallback<LobbyChatMsg_t>(new SteamCallback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChatMessage), false);
      this.callbackLobbyChatUpdate = new SteamCallback<LobbyChatUpdate_t>(new SteamCallback<LobbyChatUpdate_t>.DispatchDelegate(this.OnLobbyChatUpdate), false);
      this.callbackLobbyDataUpdate = new SteamCallback<LobbyDataUpdate_t>(new SteamCallback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyDataUpdate), false);
    }

    public void CreateLobby()
    {
      if (this.CurrentLobbyID != 0UL)
        this.LeaveCurrentLobby();
      this.callResultLobbyCreated.Set(SteamAPI.SteamMatchmaking().CreateLobby(this.LobbyPublicity, 24), new SteamCallResult<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
    }

    public void LeaveCurrentLobby()
    {
      if (this.CurrentLobbyID == 0UL)
        return;
      SteamAPI.SteamMatchmaking().LeaveLobby(this.CurrentLobbyID);
    }

    public void RefreshHostStatus()
    {
      if (this.CurrentLobbyID == 0UL)
      {
        CoreGlobals.LogErrorMessage("Steam Error", "Refresh Host Status error, Lobby ID is Zero");
      }
      else
      {
        SteamAPI.SteamMatchmaking().GetLobbyOwner(this.CurrentLobbyID);
        long steamId = (long) SteamAPI.SteamUser().GetSteamID();
      }
    }

    private void OnLobbyMatchList(LobbyMatchList_t lobbyMatchList, bool ioFailure)
    {
    }

    private void sortFoundLobbies(LobbyMatchList_t lobbyMatchList)
    {
    }

    private void OnLobbyCreated(LobbyCreated_t lobbyCreated, bool ioFailure)
    {
      if (ioFailure)
      {
        CoreGlobals.LogErrorMessage("Steam error", "Failed to create Lobby");
      }
      else
      {
        this.currentLobbyID = lobbyCreated.m_ulSteamIDLobby;
        this.RefreshHostStatus();
        SteamAPI.SteamMatchmaking().SetLobbyData(this.currentLobbyID, "Lobby", SteamAPI.SteamFriends().GetPersonaName());
        SteamAPI.SteamMatchmaking().SetLobbyOwner(this.currentLobbyID, SteamAPI.SteamUser().GetSteamID());
      }
    }

    private void OnLobbyJoined(LobbyEnter_t lobbyEnter, bool ioFailure)
    {
      if (ioFailure)
      {
        CoreGlobals.LogErrorMessage("Steam error", "Joining Lobby Failed");
      }
      else
      {
        this.currentLobbyID = lobbyEnter.m_ulSteamIDLobby;
        this.ConnectToLobbyMembers(lobbyEnter.m_ulSteamIDLobby);
        long lobbyOwner = (long) SteamAPI.SteamMatchmaking().GetLobbyOwner(lobbyEnter.m_ulSteamIDLobby);
        SteamAPI.SteamMatchmaking().GetLobbyData(lobbyEnter.m_ulSteamIDLobby, "Lobby");
        this.RefreshHostStatus();
      }
    }

    private void ConnectToLobbyMembers(ulong currentLobbyID)
    {
      throw new Exception();
    }

    private void OnInvite(GameLobbyJoinRequested_t args)
    {
    }

    private void OnLobbyChatMessage(LobbyChatMsg_t lobbyChatMsg)
    {
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t lobbyChatUpdate)
    {
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t lobbyDataUpdate)
    {
      if (lobbyDataUpdate.m_bSuccess == (byte) 0)
        return;
      CoreGlobals.LogInfoMessage("Steam Message", "Received a lobby data update.");
    }
  }
}
