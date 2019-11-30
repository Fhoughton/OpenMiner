// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.gameserveritem_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public struct gameserveritem_t
  {
    public servernetadr_t m_NetAdr;
    public int m_nPing;
    public bool m_bHadSuccessfulResponse;
    public bool m_bDoNotRefresh;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
    public char[] m_szGameDir;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
    public char[] m_szMap;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
    public char[] m_szGameDescription;
    public uint m_nAppID;
    public int m_nPlayers;
    public int m_nMaxPlayers;
    public int m_nBotPlayers;
    public bool m_bPassword;
    public bool m_bSecure;
    public uint m_ulTimeLastPlayed;
    public int m_nServerVersion;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
    public char[] m_szServerName;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
    public char[] m_szGameTags;
    public ulong m_steamID;
  }
}
