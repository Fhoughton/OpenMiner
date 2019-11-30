// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CCallbackBase
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public struct CCallbackBase
  {
    public const byte k_ECallbackFlagsRegistered = 1;
    public const byte k_ECallbackFlagsGameServer = 2;
    public IntPtr m_vTable;
    public byte m_nCallbackFlags;
    public int m_iCallback;
  }
}
