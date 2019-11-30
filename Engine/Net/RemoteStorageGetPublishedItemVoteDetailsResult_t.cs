// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.RemoteStorageGetPublishedItemVoteDetailsResult_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public struct RemoteStorageGetPublishedItemVoteDetailsResult_t
  {
    public EResult m_eResult;
    public ulong m_unPublishedFileId;
    public int m_nVotesFor;
    public int m_nVotesAgainst;
    public int m_nReports;
    public float m_fScore;
  }
}
