// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMusicRemote
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMusicRemote
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool RegisterSteamMusicRemote(string pchName);

    public abstract bool DeregisterSteamMusicRemote();

    public abstract bool BIsCurrentMusicRemote();

    public abstract bool BActivationSuccess(bool bValue);

    public abstract bool SetDisplayName(string pchDisplayName);

    public abstract bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength);

    public abstract bool EnablePlayPrevious(bool bValue);

    public abstract bool EnablePlayNext(bool bValue);

    public abstract bool EnableShuffled(bool bValue);

    public abstract bool EnableLooped(bool bValue);

    public abstract bool EnableQueue(bool bValue);

    public abstract bool EnablePlaylists(bool bValue);

    public abstract bool UpdatePlaybackStatus(int nStatus);

    public abstract bool UpdateShuffled(bool bValue);

    public abstract bool UpdateLooped(bool bValue);

    public abstract bool UpdateVolume(float flValue);

    public abstract bool CurrentEntryWillChange();

    public abstract bool CurrentEntryIsAvailable(bool bAvailable);

    public abstract bool UpdateCurrentEntryText(string pchText);

    public abstract bool UpdateCurrentEntryElapsedSeconds(int nValue);

    public abstract bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength);

    public abstract bool CurrentEntryDidChange();

    public abstract bool QueueWillChange();

    public abstract bool ResetQueueEntries();

    public abstract bool SetQueueEntry(int nID, int nPosition, string pchEntryText);

    public abstract bool SetCurrentQueueEntry(int nID);

    public abstract bool QueueDidChange();

    public abstract bool PlaylistWillChange();

    public abstract bool ResetPlaylistEntries();

    public abstract bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText);

    public abstract bool SetCurrentPlaylistEntry(int nID);

    public abstract bool PlaylistDidChange();
  }
}
