// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamUGC
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamUGC
  {
    public abstract IntPtr GetIntPtr();

    public abstract ulong CreateQueryUserUGCRequest(
      uint unAccountID,
      uint eListType,
      uint eMatchingUGCType,
      uint eSortOrder,
      uint nCreatorAppID,
      uint nConsumerAppID,
      uint unPage);

    public abstract ulong CreateQueryAllUGCRequest(
      uint eQueryType,
      uint eMatchingeMatchingUGCTypeFileType,
      uint nCreatorAppID,
      uint nConsumerAppID,
      uint unPage);

    public abstract ulong CreateQueryUGCDetailsRequest(
      ref ulong pvecPublishedFileID,
      uint unNumPublishedFileIDs);

    public abstract ulong SendQueryUGCRequest(ulong handle);

    public abstract bool GetQueryUGCResult(
      ulong handle,
      uint index,
      ref SteamUGCDetails_t pDetails);

    public abstract bool GetQueryUGCPreviewURL(
      ulong handle,
      uint index,
      string pchURL,
      uint cchURLSize);

    public abstract bool GetQueryUGCMetadata(
      ulong handle,
      uint index,
      string pchMetadata,
      uint cchMetadatasize);

    public abstract bool GetQueryUGCChildren(
      ulong handle,
      uint index,
      ref ulong pvecPublishedFileID,
      uint cMaxEntries);

    public abstract bool GetQueryUGCStatistic(
      ulong handle,
      uint index,
      uint eStatType,
      ref uint pStatValue);

    public abstract uint GetQueryUGCNumAdditionalPreviews(ulong handle, uint index);

    public abstract bool GetQueryUGCAdditionalPreview(
      ulong handle,
      uint index,
      uint previewIndex,
      string pchURLOrVideoID,
      uint cchURLSize,
      ref bool pbIsImage);

    public abstract uint GetQueryUGCNumKeyValueTags(ulong handle, uint index);

    public abstract bool GetQueryUGCKeyValueTag(
      ulong handle,
      uint index,
      uint keyValueTagIndex,
      string pchKey,
      uint cchKeySize,
      string pchValue,
      uint cchValueSize);

    public abstract bool ReleaseQueryUGCRequest(ulong handle);

    public abstract bool AddRequiredTag(ulong handle, string pTagName);

    public abstract bool AddExcludedTag(ulong handle, string pTagName);

    public abstract bool SetReturnKeyValueTags(ulong handle, bool bReturnKeyValueTags);

    public abstract bool SetReturnLongDescription(ulong handle, bool bReturnLongDescription);

    public abstract bool SetReturnMetadata(ulong handle, bool bReturnMetadata);

    public abstract bool SetReturnChildren(ulong handle, bool bReturnChildren);

    public abstract bool SetReturnAdditionalPreviews(ulong handle, bool bReturnAdditionalPreviews);

    public abstract bool SetReturnTotalOnly(ulong handle, bool bReturnTotalOnly);

    public abstract bool SetLanguage(ulong handle, string pchLanguage);

    public abstract bool SetAllowCachedResponse(ulong handle, uint unMaxAgeSeconds);

    public abstract bool SetCloudFileNameFilter(ulong handle, string pMatchCloudFileName);

    public abstract bool SetMatchAnyTag(ulong handle, bool bMatchAnyTag);

    public abstract bool SetSearchText(ulong handle, string pSearchText);

    public abstract bool SetRankedByTrendDays(ulong handle, uint unDays);

    public abstract bool AddRequiredKeyValueTag(ulong handle, string pKey, string pValue);

    public abstract ulong RequestUGCDetails(ulong nPublishedFileID, uint unMaxAgeSeconds);

    public abstract ulong CreateItem(uint nConsumerAppId, uint eFileType);

    public abstract ulong StartItemUpdate(uint nConsumerAppId, ulong nPublishedFileID);

    public abstract bool SetItemTitle(ulong handle, string pchTitle);

    public abstract bool SetItemDescription(ulong handle, string pchDescription);

    public abstract bool SetItemUpdateLanguage(ulong handle, string pchLanguage);

    public abstract bool SetItemMetadata(ulong handle, string pchMetaData);

    public abstract bool SetItemVisibility(ulong handle, uint eVisibility);

    public abstract bool SetItemTags(ulong updateHandle, ref SteamParamStringArray_t pTags);

    public abstract bool SetItemContent(ulong handle, string pszContentFolder);

    public abstract bool SetItemPreview(ulong handle, string pszPreviewFile);

    public abstract bool RemoveItemKeyValueTags(ulong handle, string pchKey);

    public abstract bool AddItemKeyValueTag(ulong handle, string pchKey, string pchValue);

    public abstract ulong SubmitItemUpdate(ulong handle, string pchChangeNote);

    public abstract uint GetItemUpdateProgress(
      ulong handle,
      ref ulong punBytesProcessed,
      ref ulong punBytesTotal);

    public abstract ulong SetUserItemVote(ulong nPublishedFileID, bool bVoteUp);

    public abstract ulong GetUserItemVote(ulong nPublishedFileID);

    public abstract ulong AddItemToFavorites(uint nAppId, ulong nPublishedFileID);

    public abstract ulong RemoveItemFromFavorites(uint nAppId, ulong nPublishedFileID);

    public abstract ulong SubscribeItem(ulong nPublishedFileID);

    public abstract ulong UnsubscribeItem(ulong nPublishedFileID);

    public abstract uint GetNumSubscribedItems();

    public abstract uint GetSubscribedItems(ref ulong pvecPublishedFileID, uint cMaxEntries);

    public abstract uint GetItemState(ulong nPublishedFileID);

    public abstract bool GetItemInstallInfo(
      ulong nPublishedFileID,
      ref ulong punSizeOnDisk,
      string pchFolder,
      uint cchFolderSize,
      ref uint punTimeStamp);

    public abstract bool GetItemDownloadInfo(
      ulong nPublishedFileID,
      ref ulong punBytesDownloaded,
      ref ulong punBytesTotal);

    public abstract bool DownloadItem(ulong nPublishedFileID, bool bHighPriority);

    public abstract bool BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder);

    public abstract void SuspendDownloads(bool bSuspend);
  }
}
