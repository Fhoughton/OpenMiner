// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.FavoritesListChanged_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [CallbackIdentity(502)]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct FavoritesListChanged_t
  {
    public uint m_nIP;
    public uint m_nQueryPort;
    public uint m_nConnPort;
    public uint m_nAppID;
    public uint m_nFlags;
    [MarshalAs(UnmanagedType.I1)]
    public bool m_bAdd;
    public uint m_unAccountId;
  }
}
