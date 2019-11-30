// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamGameServer
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamGameServer
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool InitGameServer(
      uint unIP,
      char usGamePort,
      char usQueryPort,
      uint unFlags,
      uint nGameAppId,
      string pchVersionString);

    public abstract void SetProduct(string pszProduct);

    public abstract void SetGameDescription(string pszGameDescription);

    public abstract void SetModDir(string pszModDir);

    public abstract void SetDedicatedServer(bool bDedicated);

    public abstract void LogOn(string pszToken);

    public abstract void LogOnAnonymous();

    public abstract void LogOff();

    public abstract bool BLoggedOn();

    public abstract bool BSecure();

    public abstract ulong GetSteamID();

    public abstract bool WasRestartRequested();

    public abstract void SetMaxPlayerCount(int cPlayersMax);

    public abstract void SetBotPlayerCount(int cBotplayers);

    public abstract void SetServerName(string pszServerName);

    public abstract void SetMapName(string pszMapName);

    public abstract void SetPasswordProtected(bool bPasswordProtected);

    public abstract void SetSpectatorPort(char unSpectatorPort);

    public abstract void SetSpectatorServerName(string pszSpectatorServerName);

    public abstract void ClearAllKeyValues();

    public abstract void SetKeyValue(string pKey, string pValue);

    public abstract void SetGameTags(string pchGameTags);

    public abstract void SetGameData(string pchGameData);

    public abstract void SetRegion(string pszRegion);

    public abstract bool SendUserConnectAndAuthenticate(
      uint unIPClient,
      IntPtr pvAuthBlob,
      uint cubAuthBlobSize,
      ref ulong pSteamIDUser);

    public abstract ulong CreateUnauthenticatedUserConnection();

    public abstract void SendUserDisconnect(ulong steamIDUser);

    public abstract bool BUpdateUserData(ulong steamIDUser, string pchPlayerName, uint uScore);

    public abstract uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

    public abstract uint BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID);

    public abstract void EndAuthSession(ulong steamID);

    public abstract void CancelAuthTicket(uint hAuthTicket);

    public abstract uint UserHasLicenseForApp(ulong steamID, uint appID);

    public abstract bool RequestUserGroupStatus(ulong steamIDUser, ulong steamIDGroup);

    public abstract void GetGameplayStats();

    public abstract ulong GetServerReputation();

    public abstract uint GetPublicIP();

    public abstract bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, char srcPort);

    public abstract int GetNextOutgoingPacket(
      IntPtr pOut,
      int cbMaxOut,
      ref uint pNetAdr,
      ref char pPort);

    public abstract void EnableHeartbeats(bool bActive);

    public abstract void SetHeartbeatInterval(int iHeartbeatInterval);

    public abstract void ForceHeartbeat();

    public abstract ulong AssociateWithClan(ulong steamIDClan);

    public abstract ulong ComputeNewPlayerCompatibility(ulong steamIDNewPlayer);
  }
}
