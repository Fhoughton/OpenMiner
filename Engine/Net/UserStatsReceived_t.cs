// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.UserStatsReceived_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [CallbackIdentity(1101)]
  [StructLayout(LayoutKind.Explicit, Pack = 8)]
  public struct UserStatsReceived_t
  {
    [FieldOffset(0)]
    public ulong m_nGameID;
    [FieldOffset(8)]
    public EResult m_eResult;
    [FieldOffset(12)]
    public ulong m_steamIDUser;
  }
}
