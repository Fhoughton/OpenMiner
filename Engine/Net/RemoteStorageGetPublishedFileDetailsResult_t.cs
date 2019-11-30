// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.RemoteStorageGetPublishedFileDetailsResult_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public struct RemoteStorageGetPublishedFileDetailsResult_t
  {
    public EResult m_eResult;
    public ulong m_nPublishedFileId;
    public uint m_nCreatorAppID;
    public uint m_nConsumerAppID;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchTitle;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchDescription;
    public ulong m_hFile;
    public ulong m_hPreviewFile;
    public ulong m_ulSteamIDOwner;
    public uint m_rtimeCreated;
    public uint m_rtimeUpdated;
    public ERemoteStoragePublishedFileVisibility m_eVisibility;
    public bool m_bBanned;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchTags;
    public bool m_bTagsTruncated;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260, ArraySubType = UnmanagedType.I1)]
    public char[] m_pchFileName;
    public int m_nFileSize;
    public int m_nPreviewFileSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchURL;
    public EWorkshopFileType m_eFileType;
    public bool m_bAcceptedForUse;
  }
}
