// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamRemoteStorage
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamRemoteStorage : ISteamRemoteStorage
  {
    private IntPtr m_pSteamRemoteStorage;

    public CSteamRemoteStorage(IntPtr SteamRemoteStorage)
    {
      this.m_pSteamRemoteStorage = SteamRemoteStorage;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamRemoteStorage;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamRemoteStorage == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool FileWrite(string pchFile, IntPtr pvData, int cubData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileWrite(this.m_pSteamRemoteStorage, pchFile, pvData, cubData);
    }

    public override int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileRead(this.m_pSteamRemoteStorage, pchFile, pvData, cubDataToRead);
    }

    public override ulong FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileWriteAsync(this.m_pSteamRemoteStorage, pchFile, pvData, cubData);
    }

    public override ulong FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileReadAsync(this.m_pSteamRemoteStorage, pchFile, nOffset, cubToRead);
    }

    public override bool FileReadAsyncComplete(ulong hReadCall, IntPtr pvBuffer, uint cubToRead)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete(this.m_pSteamRemoteStorage, hReadCall, pvBuffer, cubToRead);
    }

    public override bool FileForget(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileForget(this.m_pSteamRemoteStorage, pchFile);
    }

    public override bool FileDelete(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileDelete(this.m_pSteamRemoteStorage, pchFile);
    }

    public override ulong FileShare(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileShare(this.m_pSteamRemoteStorage, pchFile);
    }

    public override bool SetSyncPlatforms(string pchFile, uint eRemoteStoragePlatform)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_SetSyncPlatforms(this.m_pSteamRemoteStorage, pchFile, eRemoteStoragePlatform);
    }

    public override ulong FileWriteStreamOpen(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen(this.m_pSteamRemoteStorage, pchFile);
    }

    public override bool FileWriteStreamWriteChunk(ulong writeHandle, IntPtr pvData, int cubData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk(this.m_pSteamRemoteStorage, writeHandle, pvData, cubData);
    }

    public override bool FileWriteStreamClose(ulong writeHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileWriteStreamClose(this.m_pSteamRemoteStorage, writeHandle);
    }

    public override bool FileWriteStreamCancel(ulong writeHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel(this.m_pSteamRemoteStorage, writeHandle);
    }

    public override bool FileExists(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FileExists(this.m_pSteamRemoteStorage, pchFile);
    }

    public override bool FilePersisted(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_FilePersisted(this.m_pSteamRemoteStorage, pchFile);
    }

    public override int GetFileSize(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetFileSize(this.m_pSteamRemoteStorage, pchFile);
    }

    public override long GetFileTimestamp(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetFileTimestamp(this.m_pSteamRemoteStorage, pchFile);
    }

    public override uint GetSyncPlatforms(string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetSyncPlatforms(this.m_pSteamRemoteStorage, pchFile);
    }

    public override int GetFileCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetFileCount(this.m_pSteamRemoteStorage);
    }

    public override string GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes)
    {
      this.CheckIfUsable();
      pnFileSizeInBytes = 0;
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamRemoteStorage_GetFileNameAndSize(this.m_pSteamRemoteStorage, iFile, ref pnFileSizeInBytes));
    }

    public override bool GetQuota(ref int pnTotalBytes, ref int puAvailableBytes)
    {
      this.CheckIfUsable();
      pnTotalBytes = 0;
      puAvailableBytes = 0;
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetQuota(this.m_pSteamRemoteStorage, ref pnTotalBytes, ref puAvailableBytes);
    }

    public override bool IsCloudEnabledForAccount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount(this.m_pSteamRemoteStorage);
    }

    public override bool IsCloudEnabledForApp()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp(this.m_pSteamRemoteStorage);
    }

    public override void SetCloudEnabledForApp(bool bEnabled)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp(this.m_pSteamRemoteStorage, bEnabled);
    }

    public override ulong UGCDownload(ulong hContent, uint unPriority)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UGCDownload(this.m_pSteamRemoteStorage, hContent, unPriority);
    }

    public override bool GetUGCDownloadProgress(
      ulong hContent,
      ref int pnBytesDownloaded,
      ref int pnBytesExpected)
    {
      this.CheckIfUsable();
      pnBytesDownloaded = 0;
      pnBytesExpected = 0;
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress(this.m_pSteamRemoteStorage, hContent, ref pnBytesDownloaded, ref pnBytesExpected);
    }

    public override bool GetUGCDetails(
      ulong hContent,
      ref uint pnAppID,
      string ppchName,
      ref int pnFileSizeInBytes,
      out ulong pSteamIDOwner)
    {
      this.CheckIfUsable();
      pnAppID = 0U;
      ppchName = "";
      pnFileSizeInBytes = 0;
      pSteamIDOwner = 0UL;
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetUGCDetails(this.m_pSteamRemoteStorage, hContent, ref pnAppID, ppchName, ref pnFileSizeInBytes, ref pSteamIDOwner);
    }

    public override int UGCRead(
      ulong hContent,
      IntPtr pvData,
      int cubDataToRead,
      uint cOffset,
      uint eAction)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UGCRead(this.m_pSteamRemoteStorage, hContent, pvData, cubDataToRead, cOffset, eAction);
    }

    public override int GetCachedUGCCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetCachedUGCCount(this.m_pSteamRemoteStorage);
    }

    public override ulong GetCachedUGCHandle(int iCachedContent)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle(this.m_pSteamRemoteStorage, iCachedContent);
    }

    public override ulong PublishWorkshopFile(
      string pchFile,
      string pchPreviewFile,
      uint nConsumerAppId,
      string pchTitle,
      string pchDescription,
      uint eVisibility,
      ref SteamParamStringArray_t pTags,
      uint eWorkshopFileType)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_PublishWorkshopFile(this.m_pSteamRemoteStorage, pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, ref pTags, eWorkshopFileType);
    }

    public override ulong CreatePublishedFileUpdateRequest(ulong unPublishedFileId)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest(this.m_pSteamRemoteStorage, unPublishedFileId);
    }

    public override bool UpdatePublishedFileFile(ulong updateHandle, string pchFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile(this.m_pSteamRemoteStorage, updateHandle, pchFile);
    }

    public override bool UpdatePublishedFilePreviewFile(ulong updateHandle, string pchPreviewFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile(this.m_pSteamRemoteStorage, updateHandle, pchPreviewFile);
    }

    public override bool UpdatePublishedFileTitle(ulong updateHandle, string pchTitle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle(this.m_pSteamRemoteStorage, updateHandle, pchTitle);
    }

    public override bool UpdatePublishedFileDescription(ulong updateHandle, string pchDescription)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription(this.m_pSteamRemoteStorage, updateHandle, pchDescription);
    }

    public override bool UpdatePublishedFileVisibility(ulong updateHandle, uint eVisibility)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility(this.m_pSteamRemoteStorage, updateHandle, eVisibility);
    }

    public override bool UpdatePublishedFileTags(
      ulong updateHandle,
      ref SteamParamStringArray_t pTags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags(this.m_pSteamRemoteStorage, updateHandle, ref pTags);
    }

    public override ulong CommitPublishedFileUpdate(ulong updateHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate(this.m_pSteamRemoteStorage, updateHandle);
    }

    public override ulong GetPublishedFileDetails(ulong unPublishedFileId, uint unMaxSecondsOld)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails(this.m_pSteamRemoteStorage, unPublishedFileId, unMaxSecondsOld);
    }

    public override ulong DeletePublishedFile(ulong unPublishedFileId)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_DeletePublishedFile(this.m_pSteamRemoteStorage, unPublishedFileId);
    }

    public override ulong EnumerateUserPublishedFiles(uint unStartIndex)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles(this.m_pSteamRemoteStorage, unStartIndex);
    }

    public override ulong SubscribePublishedFile(ulong unPublishedFileId)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_SubscribePublishedFile(this.m_pSteamRemoteStorage, unPublishedFileId);
    }

    public override ulong EnumerateUserSubscribedFiles(uint unStartIndex)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles(this.m_pSteamRemoteStorage, unStartIndex);
    }

    public override ulong UnsubscribePublishedFile(ulong unPublishedFileId)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile(this.m_pSteamRemoteStorage, unPublishedFileId);
    }

    public override bool UpdatePublishedFileSetChangeDescription(
      ulong updateHandle,
      string pchChangeDescription)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(this.m_pSteamRemoteStorage, updateHandle, pchChangeDescription);
    }

    public override ulong GetPublishedItemVoteDetails(ulong unPublishedFileId)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails(this.m_pSteamRemoteStorage, unPublishedFileId);
    }

    public override ulong UpdateUserPublishedItemVote(ulong unPublishedFileId, bool bVoteUp)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote(this.m_pSteamRemoteStorage, unPublishedFileId, bVoteUp);
    }

    public override ulong GetUserPublishedItemVoteDetails(ulong unPublishedFileId)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails(this.m_pSteamRemoteStorage, unPublishedFileId);
    }

    public override ulong EnumerateUserSharedWorkshopFiles(
      ulong steamId,
      uint unStartIndex,
      ref SteamParamStringArray_t pRequiredTags,
      ref SteamParamStringArray_t pExcludedTags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(this.m_pSteamRemoteStorage, steamId, unStartIndex, ref pRequiredTags, ref pExcludedTags);
    }

    public override ulong PublishVideo(
      uint eVideoProvider,
      string pchVideoAccount,
      string pchVideoIdentifier,
      string pchPreviewFile,
      uint nConsumerAppId,
      string pchTitle,
      string pchDescription,
      uint eVisibility,
      ref SteamParamStringArray_t pTags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_PublishVideo(this.m_pSteamRemoteStorage, eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, ref pTags);
    }

    public override ulong SetUserPublishedFileAction(ulong unPublishedFileId, uint eAction)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction(this.m_pSteamRemoteStorage, unPublishedFileId, eAction);
    }

    public override ulong EnumeratePublishedFilesByUserAction(uint eAction, uint unStartIndex)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(this.m_pSteamRemoteStorage, eAction, unStartIndex);
    }

    public override ulong EnumeratePublishedWorkshopFiles(
      uint eEnumerationType,
      uint unStartIndex,
      uint unCount,
      uint unDays,
      ref SteamParamStringArray_t pTags,
      ref SteamParamStringArray_t pUserTags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(this.m_pSteamRemoteStorage, eEnumerationType, unStartIndex, unCount, unDays, ref pTags, ref pUserTags);
    }

    public override ulong UGCDownloadToLocation(
      ulong hContent,
      string pchLocation,
      uint unPriority)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation(this.m_pSteamRemoteStorage, hContent, pchLocation, unPriority);
    }
  }
}
