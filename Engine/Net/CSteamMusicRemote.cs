// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMusicRemote
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamMusicRemote : ISteamMusicRemote
  {
    private IntPtr m_pSteamMusicRemote;

    public CSteamMusicRemote(IntPtr SteamMusicRemote)
    {
      this.m_pSteamMusicRemote = SteamMusicRemote;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMusicRemote;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMusicRemote == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool RegisterSteamMusicRemote(string pchName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote(this.m_pSteamMusicRemote, pchName);
    }

    public override bool DeregisterSteamMusicRemote()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote(this.m_pSteamMusicRemote);
    }

    public override bool BIsCurrentMusicRemote()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote(this.m_pSteamMusicRemote);
    }

    public override bool BActivationSuccess(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_BActivationSuccess(this.m_pSteamMusicRemote, bValue);
    }

    public override bool SetDisplayName(string pchDisplayName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_SetDisplayName(this.m_pSteamMusicRemote, pchDisplayName);
    }

    public override bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64(this.m_pSteamMusicRemote, pvBuffer, cbBufferLength);
    }

    public override bool EnablePlayPrevious(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_EnablePlayPrevious(this.m_pSteamMusicRemote, bValue);
    }

    public override bool EnablePlayNext(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_EnablePlayNext(this.m_pSteamMusicRemote, bValue);
    }

    public override bool EnableShuffled(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_EnableShuffled(this.m_pSteamMusicRemote, bValue);
    }

    public override bool EnableLooped(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_EnableLooped(this.m_pSteamMusicRemote, bValue);
    }

    public override bool EnableQueue(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_EnableQueue(this.m_pSteamMusicRemote, bValue);
    }

    public override bool EnablePlaylists(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_EnablePlaylists(this.m_pSteamMusicRemote, bValue);
    }

    public override bool UpdatePlaybackStatus(int nStatus)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus(this.m_pSteamMusicRemote, nStatus);
    }

    public override bool UpdateShuffled(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdateShuffled(this.m_pSteamMusicRemote, bValue);
    }

    public override bool UpdateLooped(bool bValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdateLooped(this.m_pSteamMusicRemote, bValue);
    }

    public override bool UpdateVolume(float flValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdateVolume(this.m_pSteamMusicRemote, flValue);
    }

    public override bool CurrentEntryWillChange()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_CurrentEntryWillChange(this.m_pSteamMusicRemote);
    }

    public override bool CurrentEntryIsAvailable(bool bAvailable)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable(this.m_pSteamMusicRemote, bAvailable);
    }

    public override bool UpdateCurrentEntryText(string pchText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText(this.m_pSteamMusicRemote, pchText);
    }

    public override bool UpdateCurrentEntryElapsedSeconds(int nValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(this.m_pSteamMusicRemote, nValue);
    }

    public override bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt(this.m_pSteamMusicRemote, pvBuffer, cbBufferLength);
    }

    public override bool CurrentEntryDidChange()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_CurrentEntryDidChange(this.m_pSteamMusicRemote);
    }

    public override bool QueueWillChange()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_QueueWillChange(this.m_pSteamMusicRemote);
    }

    public override bool ResetQueueEntries()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_ResetQueueEntries(this.m_pSteamMusicRemote);
    }

    public override bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_SetQueueEntry(this.m_pSteamMusicRemote, nID, nPosition, pchEntryText);
    }

    public override bool SetCurrentQueueEntry(int nID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry(this.m_pSteamMusicRemote, nID);
    }

    public override bool QueueDidChange()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_QueueDidChange(this.m_pSteamMusicRemote);
    }

    public override bool PlaylistWillChange()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_PlaylistWillChange(this.m_pSteamMusicRemote);
    }

    public override bool ResetPlaylistEntries()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_ResetPlaylistEntries(this.m_pSteamMusicRemote);
    }

    public override bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_SetPlaylistEntry(this.m_pSteamMusicRemote, nID, nPosition, pchEntryText);
    }

    public override bool SetCurrentPlaylistEntry(int nID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry(this.m_pSteamMusicRemote, nID);
    }

    public override bool PlaylistDidChange()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusicRemote_PlaylistDidChange(this.m_pSteamMusicRemote);
    }
  }
}
