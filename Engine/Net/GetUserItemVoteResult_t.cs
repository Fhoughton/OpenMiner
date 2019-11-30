// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.GetUserItemVoteResult_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public struct GetUserItemVoteResult_t
  {
    public ulong m_nPublishedFileId;
    public EResult m_eResult;
    public bool m_bVotedUp;
    public bool m_bVotedDown;
    public bool m_bVoteSkipped;
  }
}
