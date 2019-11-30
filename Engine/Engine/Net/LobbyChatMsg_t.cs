// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.LobbyChatMsg_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [CallbackIdentity(507)]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct LobbyChatMsg_t
  {
    public ulong m_ulSteamIDLobby;
    public ulong m_ulSteamIDUser;
    public byte m_eChatEntryType;
    public uint m_iChatID;
  }
}
