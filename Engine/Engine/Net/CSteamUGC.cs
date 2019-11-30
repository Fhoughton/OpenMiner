// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamUGC
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamUGC : ISteamUGC
  {
    private IntPtr m_pSteamUGC;

    public CSteamUGC(IntPtr SteamUGC)
    {
      this.m_pSteamUGC = SteamUGC;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamUGC;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamUGC == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override ulong CreateQueryUserUGCRequest(
      uint unAccountID,
      uint eListType,
      uint eMatchingUGCType,
      uint eSortOrder,
      uint nCreatorAppID,
      uint nConsumerAppID,
      uint unPage)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_CreateQueryUserUGCRequest(this.m_pSteamUGC, unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
    }

    public override ulong CreateQueryAllUGCRequest(
      uint eQueryType,
      uint eMatchingeMatchingUGCTypeFileType,
      uint nCreatorAppID,
      uint nConsumerAppID,
      uint unPage)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_CreateQueryAllUGCRequest(this.m_pSteamUGC, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
    }

    public override ulong CreateQueryUGCDetailsRequest(
      ref ulong pvecPublishedFileID,
      uint unNumPublishedFileIDs)
    {
      this.CheckIfUsable();
      pvecPublishedFileID = 0UL;
      return NativeCalls.SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest(this.m_pSteamUGC, ref pvecPublishedFileID, unNumPublishedFileIDs);
    }

    public override ulong SendQueryUGCRequest(ulong handle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SendQueryUGCRequest(this.m_pSteamUGC, handle);
    }

    public override bool GetQueryUGCResult(
      ulong handle,
      uint index,
      ref SteamUGCDetails_t pDetails)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCResult(this.m_pSteamUGC, handle, index, ref pDetails);
    }

    public override bool GetQueryUGCPreviewURL(
      ulong handle,
      uint index,
      string pchURL,
      uint cchURLSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCPreviewURL(this.m_pSteamUGC, handle, index, pchURL, cchURLSize);
    }

    public override bool GetQueryUGCMetadata(
      ulong handle,
      uint index,
      string pchMetadata,
      uint cchMetadatasize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCMetadata(this.m_pSteamUGC, handle, index, pchMetadata, cchMetadatasize);
    }

    public override bool GetQueryUGCChildren(
      ulong handle,
      uint index,
      ref ulong pvecPublishedFileID,
      uint cMaxEntries)
    {
      this.CheckIfUsable();
      pvecPublishedFileID = 0UL;
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCChildren(this.m_pSteamUGC, handle, index, ref pvecPublishedFileID, cMaxEntries);
    }

    public override bool GetQueryUGCStatistic(
      ulong handle,
      uint index,
      uint eStatType,
      ref uint pStatValue)
    {
      this.CheckIfUsable();
      pStatValue = 0U;
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCStatistic(this.m_pSteamUGC, handle, index, eStatType, ref pStatValue);
    }

    public override uint GetQueryUGCNumAdditionalPreviews(ulong handle, uint index)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews(this.m_pSteamUGC, handle, index);
    }

    public override bool GetQueryUGCAdditionalPreview(
      ulong handle,
      uint index,
      uint previewIndex,
      string pchURLOrVideoID,
      uint cchURLSize,
      ref bool pbIsImage)
    {
      this.CheckIfUsable();
      pbIsImage = false;
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview(this.m_pSteamUGC, handle, index, previewIndex, pchURLOrVideoID, cchURLSize, ref pbIsImage);
    }

    public override uint GetQueryUGCNumKeyValueTags(ulong handle, uint index)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags(this.m_pSteamUGC, handle, index);
    }

    public override bool GetQueryUGCKeyValueTag(
      ulong handle,
      uint index,
      uint keyValueTagIndex,
      string pchKey,
      uint cchKeySize,
      string pchValue,
      uint cchValueSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag(this.m_pSteamUGC, handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize);
    }

    public override bool ReleaseQueryUGCRequest(ulong handle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_ReleaseQueryUGCRequest(this.m_pSteamUGC, handle);
    }

    public override bool AddRequiredTag(ulong handle, string pTagName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_AddRequiredTag(this.m_pSteamUGC, handle, pTagName);
    }

    public override bool AddExcludedTag(ulong handle, string pTagName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_AddExcludedTag(this.m_pSteamUGC, handle, pTagName);
    }

    public override bool SetReturnKeyValueTags(ulong handle, bool bReturnKeyValueTags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetReturnKeyValueTags(this.m_pSteamUGC, handle, bReturnKeyValueTags);
    }

    public override bool SetReturnLongDescription(ulong handle, bool bReturnLongDescription)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetReturnLongDescription(this.m_pSteamUGC, handle, bReturnLongDescription);
    }

    public override bool SetReturnMetadata(ulong handle, bool bReturnMetadata)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetReturnMetadata(this.m_pSteamUGC, handle, bReturnMetadata);
    }

    public override bool SetReturnChildren(ulong handle, bool bReturnChildren)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetReturnChildren(this.m_pSteamUGC, handle, bReturnChildren);
    }

    public override bool SetReturnAdditionalPreviews(ulong handle, bool bReturnAdditionalPreviews)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetReturnAdditionalPreviews(this.m_pSteamUGC, handle, bReturnAdditionalPreviews);
    }

    public override bool SetReturnTotalOnly(ulong handle, bool bReturnTotalOnly)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetReturnTotalOnly(this.m_pSteamUGC, handle, bReturnTotalOnly);
    }

    public override bool SetLanguage(ulong handle, string pchLanguage)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetLanguage(this.m_pSteamUGC, handle, pchLanguage);
    }

    public override bool SetAllowCachedResponse(ulong handle, uint unMaxAgeSeconds)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetAllowCachedResponse(this.m_pSteamUGC, handle, unMaxAgeSeconds);
    }

    public override bool SetCloudFileNameFilter(ulong handle, string pMatchCloudFileName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetCloudFileNameFilter(this.m_pSteamUGC, handle, pMatchCloudFileName);
    }

    public override bool SetMatchAnyTag(ulong handle, bool bMatchAnyTag)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetMatchAnyTag(this.m_pSteamUGC, handle, bMatchAnyTag);
    }

    public override bool SetSearchText(ulong handle, string pSearchText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetSearchText(this.m_pSteamUGC, handle, pSearchText);
    }

    public override bool SetRankedByTrendDays(ulong handle, uint unDays)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetRankedByTrendDays(this.m_pSteamUGC, handle, unDays);
    }

    public override bool AddRequiredKeyValueTag(ulong handle, string pKey, string pValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_AddRequiredKeyValueTag(this.m_pSteamUGC, handle, pKey, pValue);
    }

    public override ulong RequestUGCDetails(ulong nPublishedFileID, uint unMaxAgeSeconds)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_RequestUGCDetails(this.m_pSteamUGC, nPublishedFileID, unMaxAgeSeconds);
    }

    public override ulong CreateItem(uint nConsumerAppId, uint eFileType)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_CreateItem(this.m_pSteamUGC, nConsumerAppId, eFileType);
    }

    public override ulong StartItemUpdate(uint nConsumerAppId, ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_StartItemUpdate(this.m_pSteamUGC, nConsumerAppId, nPublishedFileID);
    }

    public override bool SetItemTitle(ulong handle, string pchTitle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemTitle(this.m_pSteamUGC, handle, pchTitle);
    }

    public override bool SetItemDescription(ulong handle, string pchDescription)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemDescription(this.m_pSteamUGC, handle, pchDescription);
    }

    public override bool SetItemUpdateLanguage(ulong handle, string pchLanguage)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemUpdateLanguage(this.m_pSteamUGC, handle, pchLanguage);
    }

    public override bool SetItemMetadata(ulong handle, string pchMetaData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemMetadata(this.m_pSteamUGC, handle, pchMetaData);
    }

    public override bool SetItemVisibility(ulong handle, uint eVisibility)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemVisibility(this.m_pSteamUGC, handle, eVisibility);
    }

    public override bool SetItemTags(ulong updateHandle, ref SteamParamStringArray_t pTags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemTags(this.m_pSteamUGC, updateHandle, ref pTags);
    }

    public override bool SetItemContent(ulong handle, string pszContentFolder)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemContent(this.m_pSteamUGC, handle, pszContentFolder);
    }

    public override bool SetItemPreview(ulong handle, string pszPreviewFile)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetItemPreview(this.m_pSteamUGC, handle, pszPreviewFile);
    }

    public override bool RemoveItemKeyValueTags(ulong handle, string pchKey)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_RemoveItemKeyValueTags(this.m_pSteamUGC, handle, pchKey);
    }

    public override bool AddItemKeyValueTag(ulong handle, string pchKey, string pchValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_AddItemKeyValueTag(this.m_pSteamUGC, handle, pchKey, pchValue);
    }

    public override ulong SubmitItemUpdate(ulong handle, string pchChangeNote)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SubmitItemUpdate(this.m_pSteamUGC, handle, pchChangeNote);
    }

    public override uint GetItemUpdateProgress(
      ulong handle,
      ref ulong punBytesProcessed,
      ref ulong punBytesTotal)
    {
      this.CheckIfUsable();
      punBytesProcessed = 0UL;
      punBytesTotal = 0UL;
      return NativeCalls.SteamAPI_ISteamUGC_GetItemUpdateProgress(this.m_pSteamUGC, handle, ref punBytesProcessed, ref punBytesTotal);
    }

    public override ulong SetUserItemVote(ulong nPublishedFileID, bool bVoteUp)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SetUserItemVote(this.m_pSteamUGC, nPublishedFileID, bVoteUp);
    }

    public override ulong GetUserItemVote(ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetUserItemVote(this.m_pSteamUGC, nPublishedFileID);
    }

    public override ulong AddItemToFavorites(uint nAppId, ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_AddItemToFavorites(this.m_pSteamUGC, nAppId, nPublishedFileID);
    }

    public override ulong RemoveItemFromFavorites(uint nAppId, ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_RemoveItemFromFavorites(this.m_pSteamUGC, nAppId, nPublishedFileID);
    }

    public override ulong SubscribeItem(ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_SubscribeItem(this.m_pSteamUGC, nPublishedFileID);
    }

    public override ulong UnsubscribeItem(ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_UnsubscribeItem(this.m_pSteamUGC, nPublishedFileID);
    }

    public override uint GetNumSubscribedItems()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetNumSubscribedItems(this.m_pSteamUGC);
    }

    public override uint GetSubscribedItems(ref ulong pvecPublishedFileID, uint cMaxEntries)
    {
      this.CheckIfUsable();
      pvecPublishedFileID = 0UL;
      return NativeCalls.SteamAPI_ISteamUGC_GetSubscribedItems(this.m_pSteamUGC, ref pvecPublishedFileID, cMaxEntries);
    }

    public override uint GetItemState(ulong nPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_GetItemState(this.m_pSteamUGC, nPublishedFileID);
    }

    public override bool GetItemInstallInfo(
      ulong nPublishedFileID,
      ref ulong punSizeOnDisk,
      string pchFolder,
      uint cchFolderSize,
      ref uint punTimeStamp)
    {
      this.CheckIfUsable();
      punSizeOnDisk = 0UL;
      punTimeStamp = 0U;
      return NativeCalls.SteamAPI_ISteamUGC_GetItemInstallInfo(this.m_pSteamUGC, nPublishedFileID, ref punSizeOnDisk, pchFolder, cchFolderSize, ref punTimeStamp);
    }

    public override bool GetItemDownloadInfo(
      ulong nPublishedFileID,
      ref ulong punBytesDownloaded,
      ref ulong punBytesTotal)
    {
      this.CheckIfUsable();
      punBytesDownloaded = 0UL;
      punBytesTotal = 0UL;
      return NativeCalls.SteamAPI_ISteamUGC_GetItemDownloadInfo(this.m_pSteamUGC, nPublishedFileID, ref punBytesDownloaded, ref punBytesTotal);
    }

    public override bool DownloadItem(ulong nPublishedFileID, bool bHighPriority)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_DownloadItem(this.m_pSteamUGC, nPublishedFileID, bHighPriority);
    }

    public override bool BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUGC_BInitWorkshopForGameServer(this.m_pSteamUGC, unWorkshopDepotID, pszFolder);
    }

    public override void SuspendDownloads(bool bSuspend)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamUGC_SuspendDownloads(this.m_pSteamUGC, bSuspend);
    }
  }
}
