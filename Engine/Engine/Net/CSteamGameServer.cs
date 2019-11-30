// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamGameServer
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamGameServer : ISteamGameServer
  {
    private IntPtr m_pSteamGameServer;

    public CSteamGameServer(IntPtr SteamGameServer)
    {
      this.m_pSteamGameServer = SteamGameServer;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamGameServer;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamGameServer == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool InitGameServer(
      uint unIP,
      char usGamePort,
      char usQueryPort,
      uint unFlags,
      uint nGameAppId,
      string pchVersionString)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_InitGameServer(this.m_pSteamGameServer, unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
    }

    public override void SetProduct(string pszProduct)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetProduct(this.m_pSteamGameServer, pszProduct);
    }

    public override void SetGameDescription(string pszGameDescription)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetGameDescription(this.m_pSteamGameServer, pszGameDescription);
    }

    public override void SetModDir(string pszModDir)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetModDir(this.m_pSteamGameServer, pszModDir);
    }

    public override void SetDedicatedServer(bool bDedicated)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetDedicatedServer(this.m_pSteamGameServer, bDedicated);
    }

    public override void LogOn(string pszToken)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_LogOn(this.m_pSteamGameServer, pszToken);
    }

    public override void LogOnAnonymous()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_LogOnAnonymous(this.m_pSteamGameServer);
    }

    public override void LogOff()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_LogOff(this.m_pSteamGameServer);
    }

    public override bool BLoggedOn()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_BLoggedOn(this.m_pSteamGameServer);
    }

    public override bool BSecure()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_BSecure(this.m_pSteamGameServer);
    }

    public override ulong GetSteamID()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_GetSteamID(this.m_pSteamGameServer);
    }

    public override bool WasRestartRequested()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_WasRestartRequested(this.m_pSteamGameServer);
    }

    public override void SetMaxPlayerCount(int cPlayersMax)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetMaxPlayerCount(this.m_pSteamGameServer, cPlayersMax);
    }

    public override void SetBotPlayerCount(int cBotplayers)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetBotPlayerCount(this.m_pSteamGameServer, cBotplayers);
    }

    public override void SetServerName(string pszServerName)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetServerName(this.m_pSteamGameServer, pszServerName);
    }

    public override void SetMapName(string pszMapName)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetMapName(this.m_pSteamGameServer, pszMapName);
    }

    public override void SetPasswordProtected(bool bPasswordProtected)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetPasswordProtected(this.m_pSteamGameServer, bPasswordProtected);
    }

    public override void SetSpectatorPort(char unSpectatorPort)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetSpectatorPort(this.m_pSteamGameServer, unSpectatorPort);
    }

    public override void SetSpectatorServerName(string pszSpectatorServerName)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetSpectatorServerName(this.m_pSteamGameServer, pszSpectatorServerName);
    }

    public override void ClearAllKeyValues()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_ClearAllKeyValues(this.m_pSteamGameServer);
    }

    public override void SetKeyValue(string pKey, string pValue)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetKeyValue(this.m_pSteamGameServer, pKey, pValue);
    }

    public override void SetGameTags(string pchGameTags)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetGameTags(this.m_pSteamGameServer, pchGameTags);
    }

    public override void SetGameData(string pchGameData)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetGameData(this.m_pSteamGameServer, pchGameData);
    }

    public override void SetRegion(string pszRegion)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetRegion(this.m_pSteamGameServer, pszRegion);
    }

    public override bool SendUserConnectAndAuthenticate(
      uint unIPClient,
      IntPtr pvAuthBlob,
      uint cubAuthBlobSize,
      ref ulong pSteamIDUser)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_SendUserConnectAndAuthenticate(this.m_pSteamGameServer, unIPClient, pvAuthBlob, cubAuthBlobSize, ref pSteamIDUser);
    }

    public override ulong CreateUnauthenticatedUserConnection()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_CreateUnauthenticatedUserConnection(this.m_pSteamGameServer);
    }

    public override void SendUserDisconnect(ulong steamIDUser)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SendUserDisconnect(this.m_pSteamGameServer, steamIDUser);
    }

    public override bool BUpdateUserData(ulong steamIDUser, string pchPlayerName, uint uScore)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_BUpdateUserData(this.m_pSteamGameServer, steamIDUser, pchPlayerName, uScore);
    }

    public override uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
    {
      this.CheckIfUsable();
      pcbTicket = 0U;
      return NativeCalls.SteamAPI_ISteamGameServer_GetAuthSessionTicket(this.m_pSteamGameServer, pTicket, cbMaxTicket, ref pcbTicket);
    }

    public override uint BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_BeginAuthSession(this.m_pSteamGameServer, pAuthTicket, cbAuthTicket, steamID);
    }

    public override void EndAuthSession(ulong steamID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_EndAuthSession(this.m_pSteamGameServer, steamID);
    }

    public override void CancelAuthTicket(uint hAuthTicket)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_CancelAuthTicket(this.m_pSteamGameServer, hAuthTicket);
    }

    public override uint UserHasLicenseForApp(ulong steamID, uint appID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_UserHasLicenseForApp(this.m_pSteamGameServer, steamID, appID);
    }

    public override bool RequestUserGroupStatus(ulong steamIDUser, ulong steamIDGroup)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_RequestUserGroupStatus(this.m_pSteamGameServer, steamIDUser, steamIDGroup);
    }

    public override void GetGameplayStats()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_GetGameplayStats(this.m_pSteamGameServer);
    }

    public override ulong GetServerReputation()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_GetServerReputation(this.m_pSteamGameServer);
    }

    public override uint GetPublicIP()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_GetPublicIP(this.m_pSteamGameServer);
    }

    public override bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, char srcPort)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_HandleIncomingPacket(this.m_pSteamGameServer, pData, cbData, srcIP, srcPort);
    }

    public override int GetNextOutgoingPacket(
      IntPtr pOut,
      int cbMaxOut,
      ref uint pNetAdr,
      ref char pPort)
    {
      this.CheckIfUsable();
      pNetAdr = 0U;
      pPort = char.MinValue;
      return NativeCalls.SteamAPI_ISteamGameServer_GetNextOutgoingPacket(this.m_pSteamGameServer, pOut, cbMaxOut, ref pNetAdr, ref pPort);
    }

    public override void EnableHeartbeats(bool bActive)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_EnableHeartbeats(this.m_pSteamGameServer, bActive);
    }

    public override void SetHeartbeatInterval(int iHeartbeatInterval)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_SetHeartbeatInterval(this.m_pSteamGameServer, iHeartbeatInterval);
    }

    public override void ForceHeartbeat()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamGameServer_ForceHeartbeat(this.m_pSteamGameServer);
    }

    public override ulong AssociateWithClan(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_AssociateWithClan(this.m_pSteamGameServer, steamIDClan);
    }

    public override ulong ComputeNewPlayerCompatibility(ulong steamIDNewPlayer)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServer_ComputeNewPlayerCompatibility(this.m_pSteamGameServer, steamIDNewPlayer);
    }
  }
}
