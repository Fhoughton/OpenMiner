// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.RemoteStorageEnumerateUserSharedWorkshopFilesResult_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public struct RemoteStorageEnumerateUserSharedWorkshopFilesResult_t
  {
    public EResult m_eResult;
    public int m_nResultsReturned;
    public int m_nTotalResultCount;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    public ulong[] m_rgPublishedFileId;
  }
}
