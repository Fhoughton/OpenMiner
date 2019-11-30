// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamRemoteStorage
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamRemoteStorage
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool FileWrite(string pchFile, IntPtr pvData, int cubData);

    public abstract int FileRead(string pchFile, IntPtr pvData, int cubDataToRead);

    public abstract ulong FileWriteAsync(string pchFile, IntPtr pvData, uint cubData);

    public abstract ulong FileReadAsync(string pchFile, uint nOffset, uint cubToRead);

    public abstract bool FileReadAsyncComplete(ulong hReadCall, IntPtr pvBuffer, uint cubToRead);

    public abstract bool FileForget(string pchFile);

    public abstract bool FileDelete(string pchFile);

    public abstract ulong FileShare(string pchFile);

    public abstract bool SetSyncPlatforms(string pchFile, uint eRemoteStoragePlatform);

    public abstract ulong FileWriteStreamOpen(string pchFile);

    public abstract bool FileWriteStreamWriteChunk(ulong writeHandle, IntPtr pvData, int cubData);

    public abstract bool FileWriteStreamClose(ulong writeHandle);

    public abstract bool FileWriteStreamCancel(ulong writeHandle);

    public abstract bool FileExists(string pchFile);

    public abstract bool FilePersisted(string pchFile);

    public abstract int GetFileSize(string pchFile);

    public abstract long GetFileTimestamp(string pchFile);

    public abstract uint GetSyncPlatforms(string pchFile);

    public abstract int GetFileCount();

    public abstract string GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes);

    public abstract bool GetQuota(ref int pnTotalBytes, ref int puAvailableBytes);

    public abstract bool IsCloudEnabledForAccount();

    public abstract bool IsCloudEnabledForApp();

    public abstract void SetCloudEnabledForApp(bool bEnabled);

    public abstract ulong UGCDownload(ulong hContent, uint unPriority);

    public abstract bool GetUGCDownloadProgress(
      ulong hContent,
      ref int pnBytesDownloaded,
      ref int pnBytesExpected);

    public abstract bool GetUGCDetails(
      ulong hContent,
      ref uint pnAppID,
      string ppchName,
      ref int pnFileSizeInBytes,
      out ulong pSteamIDOwner);

    public abstract int UGCRead(
      ulong hContent,
      IntPtr pvData,
      int cubDataToRead,
      uint cOffset,
      uint eAction);

    public abstract int GetCachedUGCCount();

    public abstract ulong GetCachedUGCHandle(int iCachedContent);

    public abstract ulong PublishWorkshopFile(
      string pchFile,
      string pchPreviewFile,
      uint nConsumerAppId,
      string pchTitle,
      string pchDescription,
      uint eVisibility,
      ref SteamParamStringArray_t pTags,
      uint eWorkshopFileType);

    public abstract ulong CreatePublishedFileUpdateRequest(ulong unPublishedFileId);

    public abstract bool UpdatePublishedFileFile(ulong updateHandle, string pchFile);

    public abstract bool UpdatePublishedFilePreviewFile(ulong updateHandle, string pchPreviewFile);

    public abstract bool UpdatePublishedFileTitle(ulong updateHandle, string pchTitle);

    public abstract bool UpdatePublishedFileDescription(ulong updateHandle, string pchDescription);

    public abstract bool UpdatePublishedFileVisibility(ulong updateHandle, uint eVisibility);

    public abstract bool UpdatePublishedFileTags(
      ulong updateHandle,
      ref SteamParamStringArray_t pTags);

    public abstract ulong CommitPublishedFileUpdate(ulong updateHandle);

    public abstract ulong GetPublishedFileDetails(ulong unPublishedFileId, uint unMaxSecondsOld);

    public abstract ulong DeletePublishedFile(ulong unPublishedFileId);

    public abstract ulong EnumerateUserPublishedFiles(uint unStartIndex);

    public abstract ulong SubscribePublishedFile(ulong unPublishedFileId);

    public abstract ulong EnumerateUserSubscribedFiles(uint unStartIndex);

    public abstract ulong UnsubscribePublishedFile(ulong unPublishedFileId);

    public abstract bool UpdatePublishedFileSetChangeDescription(
      ulong updateHandle,
      string pchChangeDescription);

    public abstract ulong GetPublishedItemVoteDetails(ulong unPublishedFileId);

    public abstract ulong UpdateUserPublishedItemVote(ulong unPublishedFileId, bool bVoteUp);

    public abstract ulong GetUserPublishedItemVoteDetails(ulong unPublishedFileId);

    public abstract ulong EnumerateUserSharedWorkshopFiles(
      ulong steamId,
      uint unStartIndex,
      ref SteamParamStringArray_t pRequiredTags,
      ref SteamParamStringArray_t pExcludedTags);

    public abstract ulong PublishVideo(
      uint eVideoProvider,
      string pchVideoAccount,
      string pchVideoIdentifier,
      string pchPreviewFile,
      uint nConsumerAppId,
      string pchTitle,
      string pchDescription,
      uint eVisibility,
      ref SteamParamStringArray_t pTags);

    public abstract ulong SetUserPublishedFileAction(ulong unPublishedFileId, uint eAction);

    public abstract ulong EnumeratePublishedFilesByUserAction(uint eAction, uint unStartIndex);

    public abstract ulong EnumeratePublishedWorkshopFiles(
      uint eEnumerationType,
      uint unStartIndex,
      uint unCount,
      uint unDays,
      ref SteamParamStringArray_t pTags,
      ref SteamParamStringArray_t pUserTags);

    public abstract ulong UGCDownloadToLocation(
      ulong hContent,
      string pchLocation,
      uint unPriority);
  }
}
