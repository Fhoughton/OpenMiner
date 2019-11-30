// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.LobbyEnter_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [CallbackIdentity(504)]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct LobbyEnter_t
  {
    public ulong m_ulSteamIDLobby;
    public uint m_rgfChatPermissions;
    [MarshalAs(UnmanagedType.I1)]
    public bool m_bLocked;
    public uint m_EChatRoomEnterResponse;
  }
}
