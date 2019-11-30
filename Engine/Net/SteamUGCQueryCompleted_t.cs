// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamUGCQueryCompleted_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public struct SteamUGCQueryCompleted_t
  {
    public ulong m_handle;
    public EResult m_eResult;
    public uint m_unNumResultsReturned;
    public uint m_unTotalMatchingResults;
    public bool m_bCachedData;
  }
}
