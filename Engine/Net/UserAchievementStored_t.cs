// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.UserAchievementStored_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [CallbackIdentity(1103)]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct UserAchievementStored_t
  {
    public ulong m_nGameID;
    [MarshalAs(UnmanagedType.I1)]
    public bool m_bGroupAchievement;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchAchievementName;
    public uint m_nCurProgress;
    public uint m_nMaxProgress;
  }
}
