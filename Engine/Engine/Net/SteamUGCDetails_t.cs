// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamUGCDetails_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public struct SteamUGCDetails_t
  {
    public ulong m_nPublishedFileId;
    public EResult m_eResult;
    public EWorkshopFileType m_eFileType;
    public uint m_nCreatorAppID;
    public uint m_nConsumerAppID;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchTitle;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchDescription;
    public ulong m_ulSteamIDOwner;
    public uint m_rtimeCreated;
    public uint m_rtimeUpdated;
    public uint m_rtimeAddedToUserList;
    public ERemoteStoragePublishedFileVisibility m_eVisibility;
    public bool m_bBanned;
    public bool m_bAcceptedForUse;
    public bool m_bTagsTruncated;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchTags;
    public ulong m_hFile;
    public ulong m_hPreviewFile;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260, ArraySubType = UnmanagedType.I1)]
    public char[] m_pchFileName;
    public int m_nFileSize;
    public int m_nPreviewFileSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.I1)]
    public char[] m_rgchURL;
    public uint m_unVotesUp;
    public uint m_unVotesDown;
    public float m_flScore;
    public uint m_unNumChildren;
  }
}
