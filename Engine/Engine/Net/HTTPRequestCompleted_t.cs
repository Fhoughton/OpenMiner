// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.HTTPRequestCompleted_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public struct HTTPRequestCompleted_t
  {
    public uint m_hRequest;
    public ulong m_ulContextValue;
    public bool m_bRequestSuccessful;
    public EHTTPStatusCode m_eStatusCode;
    public uint m_unBodySize;
  }
}
