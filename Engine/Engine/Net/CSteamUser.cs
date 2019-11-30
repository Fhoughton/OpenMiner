// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamUser
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamUser : ISteamUser
  {
    private IntPtr m_pSteamUser;

    public CSteamUser(IntPtr SteamUser)
    {
      this.m_pSteamUser = SteamUser;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamUser;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamUser == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint GetHSteamUser()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_GetHSteamUser(this.m_pSteamUser);
    }

    public override bool BLoggedOn()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_BLoggedOn(this.m_pSteamUser);
    }

    public override ulong GetSteamID()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_GetSteamID(this.m_pSteamUser);
    }

    public override int InitiateGameConnection(
      IntPtr pAuthBlob,
      int cbMaxAuthBlob,
      ulong steamIDGameServer,
      uint unIPServer,
      char usPortServer,
      bool bSecure)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_InitiateGameConnection(this.m_pSteamUser, pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
    }

    public override void TerminateGameConnection(uint unIPServer, char usPortServer)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_TerminateGameConnection(this.m_pSteamUser, unIPServer, usPortServer);
    }

    public override void TrackAppUsageEvent(ulong gameID, int eAppUsageEvent, string pchExtraInfo)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_TrackAppUsageEvent(this.m_pSteamUser, gameID, eAppUsageEvent, pchExtraInfo);
    }

    public override bool GetUserDataFolder(string pchBuffer, int cubBuffer)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_GetUserDataFolder(this.m_pSteamUser, pchBuffer, cubBuffer);
    }

    public override void StartVoiceRecording()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_StartVoiceRecording(this.m_pSteamUser);
    }

    public override void StopVoiceRecording()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_StopVoiceRecording(this.m_pSteamUser);
    }

    public override EVoiceResult GetAvailableVoice(
      out uint pcbCompressed,
      out uint pcbUncompressed,
      uint nUncompressedVoiceDesiredSampleRate)
    {
      this.CheckIfUsable();
      pcbCompressed = 0U;
      pcbUncompressed = 0U;
      return NativeCalls.SteamAPI_ISteamUser_GetAvailableVoice(this.m_pSteamUser, out pcbCompressed, out pcbUncompressed, nUncompressedVoiceDesiredSampleRate);
    }

    public override EVoiceResult GetVoice(
      bool bWantCompressed,
      byte[] pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      bool bWantUncompressed,
      byte[] pUncompressedDestBuffer,
      uint cbUncompressedDestBufferSize,
      out uint nUncompressBytesWritten,
      uint nUncompressedVoiceDesiredSampleRate)
    {
      this.CheckIfUsable();
      nBytesWritten = 0U;
      nUncompressBytesWritten = 0U;
      return NativeCalls.SteamAPI_ISteamUser_GetVoice(this.m_pSteamUser, bWantCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, bWantUncompressed, pUncompressedDestBuffer, cbUncompressedDestBufferSize, out nUncompressBytesWritten, nUncompressedVoiceDesiredSampleRate);
    }

    public override EVoiceResult DecompressVoice(
      byte[] pCompressed,
      uint cbCompressed,
      byte[] pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      uint nDesiredSampleRate)
    {
      this.CheckIfUsable();
      nBytesWritten = 0U;
      return NativeCalls.SteamAPI_ISteamUser_DecompressVoice(this.m_pSteamUser, pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, nDesiredSampleRate);
    }

    public override uint GetVoiceOptimalSampleRate()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_GetVoiceOptimalSampleRate(this.m_pSteamUser);
    }

    public override uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
    {
      this.CheckIfUsable();
      pcbTicket = 0U;
      return NativeCalls.SteamAPI_ISteamUser_GetAuthSessionTicket(this.m_pSteamUser, pTicket, cbMaxTicket, ref pcbTicket);
    }

    public override uint BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_BeginAuthSession(this.m_pSteamUser, pAuthTicket, cbAuthTicket, steamID);
    }

    public override void EndAuthSession(ulong steamID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_EndAuthSession(this.m_pSteamUser, steamID);
    }

    public override void CancelAuthTicket(uint hAuthTicket)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_CancelAuthTicket(this.m_pSteamUser, hAuthTicket);
    }

    public override uint UserHasLicenseForApp(ulong steamID, uint appID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_UserHasLicenseForApp(this.m_pSteamUser, steamID, appID);
    }

    public override bool BIsBehindNAT()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_BIsBehindNAT(this.m_pSteamUser);
    }

    public override void AdvertiseGame(ulong steamIDGameServer, uint unIPServer, char usPortServer)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUser_AdvertiseGame(this.m_pSteamUser, steamIDGameServer, unIPServer, usPortServer);
    }

    public override ulong RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_RequestEncryptedAppTicket(this.m_pSteamUser, pDataToInclude, cbDataToInclude);
    }

    public override bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
    {
      this.CheckIfUsable();
      pcbTicket = 0U;
      return NativeCalls.SteamAPI_ISteamUser_GetEncryptedAppTicket(this.m_pSteamUser, pTicket, cbMaxTicket, ref pcbTicket);
    }

    public override int GetGameBadgeLevel(int nSeries, bool bFoil)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_GetGameBadgeLevel(this.m_pSteamUser, nSeries, bFoil);
    }

    public override int GetPlayerSteamLevel()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_GetPlayerSteamLevel(this.m_pSteamUser);
    }

    public override ulong RequestStoreAuthURL(string pchRedirectURL)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUser_RequestStoreAuthURL(this.m_pSteamUser, pchRedirectURL);
    }
  }
}
