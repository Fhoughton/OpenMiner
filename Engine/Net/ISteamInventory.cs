// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamInventory
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamInventory
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint GetResultStatus(int resultHandle);

    public abstract bool GetResultItems(int resultHandle, out SteamItemDetails_t[] pOutItemsArray);

    public abstract uint GetResultTimestamp(int resultHandle);

    public abstract bool CheckResultSteamID(int resultHandle, ulong steamIDExpected);

    public abstract void DestroyResult(int resultHandle);

    public abstract bool GetAllItems(ref int pResultHandle);

    public abstract bool GetItemsByID(ref int pResultHandle, ulong[] pInstanceIDs);

    public abstract bool SerializeResult(
      int resultHandle,
      IntPtr pOutBuffer,
      ref uint punOutBufferSize);

    public abstract bool DeserializeResult(
      ref int pOutResultHandle,
      IntPtr pBuffer,
      uint unBufferSize,
      bool bRESERVED_MUST_BE_FALSE);

    public abstract bool GenerateItems(
      ref int pResultHandle,
      int[] pArrayItemDefs,
      uint[] punArrayQuantity);

    public abstract bool GrantPromoItems(ref int pResultHandle);

    public abstract bool AddPromoItem(ref int pResultHandle, int itemDef);

    public abstract bool AddPromoItems(ref int pResultHandle, int[] pArrayItemDefs);

    public abstract bool ConsumeItem(ref int pResultHandle, ulong itemConsume, uint unQuantity);

    public abstract bool ExchangeItems(
      ref int pResultHandle,
      int[] pArrayGenerate,
      uint[] punArrayGenerateQuantity,
      ulong[] pArrayDestroy,
      uint[] punArrayDestroyQuantity);

    public abstract bool TransferItemQuantity(
      ref int pResultHandle,
      ulong itemIdSource,
      uint unQuantity,
      ulong itemIdDest);

    public abstract void SendItemDropHeartbeat();

    public abstract bool TriggerItemDrop(ref int pResultHandle, int dropListDefinition);

    public abstract bool TradeItems(
      ref int pResultHandle,
      ulong steamIDTradePartner,
      ulong[] pArrayGive,
      uint[] pArrayGiveQuantity,
      ulong[] pArrayGet,
      uint[] pArrayGetQuantity);

    public abstract bool LoadItemDefinitions();

    public abstract bool GetItemDefinitionIDs(out int[] pItemDefIDs);

    public abstract bool GetItemDefinitionProperty(
      int iDefinition,
      string pchPropertyName,
      out string pchValueBuffer);
  }
}
