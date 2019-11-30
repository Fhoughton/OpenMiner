// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamUser
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamUser
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint GetHSteamUser();

    public abstract bool BLoggedOn();

    public abstract ulong GetSteamID();

    public abstract int InitiateGameConnection(
      IntPtr pAuthBlob,
      int cbMaxAuthBlob,
      ulong steamIDGameServer,
      uint unIPServer,
      char usPortServer,
      bool bSecure);

    public abstract void TerminateGameConnection(uint unIPServer, char usPortServer);

    public abstract void TrackAppUsageEvent(ulong gameID, int eAppUsageEvent, string pchExtraInfo);

    public abstract bool GetUserDataFolder(string pchBuffer, int cubBuffer);

    public abstract void StartVoiceRecording();

    public abstract void StopVoiceRecording();

    public abstract EVoiceResult GetAvailableVoice(
      out uint pcbCompressed,
      out uint pcbUncompressed,
      uint nUncompressedVoiceDesiredSampleRate);

    public abstract EVoiceResult GetVoice(
      bool bWantCompressed,
      byte[] pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      bool bWantUncompressed,
      byte[] pUncompressedDestBuffer,
      uint cbUncompressedDestBufferSize,
      out uint nUncompressBytesWritten,
      uint nUncompressedVoiceDesiredSampleRate);

    public abstract EVoiceResult DecompressVoice(
      byte[] pCompressed,
      uint cbCompressed,
      byte[] pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      uint nDesiredSampleRate);

    public abstract uint GetVoiceOptimalSampleRate();

    public abstract uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

    public abstract uint BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID);

    public abstract void EndAuthSession(ulong steamID);

    public abstract void CancelAuthTicket(uint hAuthTicket);

    public abstract uint UserHasLicenseForApp(ulong steamID, uint appID);

    public abstract bool BIsBehindNAT();

    public abstract void AdvertiseGame(ulong steamIDGameServer, uint unIPServer, char usPortServer);

    public abstract ulong RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude);

    public abstract bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

    public abstract int GetGameBadgeLevel(int nSeries, bool bFoil);

    public abstract int GetPlayerSteamLevel();

    public abstract ulong RequestStoreAuthURL(string pchRedirectURL);
  }
}
