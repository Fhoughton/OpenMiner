// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamInventory
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Text;

namespace StudioForge.Engine.Net
{
  public class CSteamInventory : ISteamInventory
  {
    private IntPtr m_pSteamInventory;

    public CSteamInventory(IntPtr SteamInventory)
    {
      this.m_pSteamInventory = SteamInventory;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamInventory;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamInventory == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint GetResultStatus(int resultHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamInventory_GetResultStatus(this.m_pSteamInventory, resultHandle);
    }

    public override bool GetResultItems(int resultHandle, out SteamItemDetails_t[] pOutItemsArray)
    {
      this.CheckIfUsable();
      uint punOutItemsArraySize = 0;
      NativeCalls.SteamAPI_ISteamInventory_GetResultItems(this.m_pSteamInventory, resultHandle, (SteamItemDetails_t[]) null, ref punOutItemsArraySize);
      pOutItemsArray = new SteamItemDetails_t[(int)punOutItemsArraySize]; //Was intptr
      return NativeCalls.SteamAPI_ISteamInventory_GetResultItems(this.m_pSteamInventory, resultHandle, pOutItemsArray, ref punOutItemsArraySize);
    }

    public override uint GetResultTimestamp(int resultHandle)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamInventory_GetResultTimestamp(this.m_pSteamInventory, resultHandle);
    }

    public override bool CheckResultSteamID(int resultHandle, ulong steamIDExpected)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamInventory_CheckResultSteamID(this.m_pSteamInventory, resultHandle, steamIDExpected);
    }

    public override void DestroyResult(int resultHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamInventory_DestroyResult(this.m_pSteamInventory, resultHandle);
    }

    public override bool GetAllItems(ref int pResultHandle)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_GetAllItems(this.m_pSteamInventory, ref pResultHandle);
    }

    public override bool GetItemsByID(ref int pResultHandle, ulong[] pInstanceIDs)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_GetItemsByID(this.m_pSteamInventory, ref pResultHandle, pInstanceIDs, (uint) pInstanceIDs.Length);
    }

    public override bool SerializeResult(
      int resultHandle,
      IntPtr pOutBuffer,
      ref uint punOutBufferSize)
    {
      this.CheckIfUsable();
      punOutBufferSize = 0U;
      return NativeCalls.SteamAPI_ISteamInventory_SerializeResult(this.m_pSteamInventory, resultHandle, pOutBuffer, ref punOutBufferSize);
    }

    public override bool DeserializeResult(
      ref int pOutResultHandle,
      IntPtr pBuffer,
      uint unBufferSize,
      bool bRESERVED_MUST_BE_FALSE)
    {
      this.CheckIfUsable();
      pOutResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_DeserializeResult(this.m_pSteamInventory, ref pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
    }

    public override bool GenerateItems(
      ref int pResultHandle,
      int[] pArrayItemDefs,
      uint[] punArrayQuantity)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_GenerateItems(this.m_pSteamInventory, ref pResultHandle, pArrayItemDefs, punArrayQuantity, (uint) punArrayQuantity.Length);
    }

    public override bool GrantPromoItems(ref int pResultHandle)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_GrantPromoItems(this.m_pSteamInventory, ref pResultHandle);
    }

    public override bool AddPromoItem(ref int pResultHandle, int itemDef)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_AddPromoItem(this.m_pSteamInventory, ref pResultHandle, itemDef);
    }

    public override bool AddPromoItems(ref int pResultHandle, int[] pArrayItemDefs)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_AddPromoItems(this.m_pSteamInventory, ref pResultHandle, pArrayItemDefs, (uint) pArrayItemDefs.Length);
    }

    public override bool ConsumeItem(ref int pResultHandle, ulong itemConsume, uint unQuantity)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_ConsumeItem(this.m_pSteamInventory, ref pResultHandle, itemConsume, unQuantity);
    }

    public override bool ExchangeItems(
      ref int pResultHandle,
      int[] pArrayGenerate,
      uint[] punArrayGenerateQuantity,
      ulong[] pArrayDestroy,
      uint[] punArrayDestroyQuantity)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_ExchangeItems(this.m_pSteamInventory, ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, (uint) punArrayGenerateQuantity.Length, pArrayDestroy, punArrayDestroyQuantity, (uint) punArrayDestroyQuantity.Length);
    }

    public override bool TransferItemQuantity(
      ref int pResultHandle,
      ulong itemIdSource,
      uint unQuantity,
      ulong itemIdDest)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_TransferItemQuantity(this.m_pSteamInventory, ref pResultHandle, itemIdSource, unQuantity, itemIdDest);
    }

    public override void SendItemDropHeartbeat()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamInventory_SendItemDropHeartbeat(this.m_pSteamInventory);
    }

    public override bool TriggerItemDrop(ref int pResultHandle, int dropListDefinition)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_TriggerItemDrop(this.m_pSteamInventory, ref pResultHandle, dropListDefinition);
    }

    public override bool TradeItems(
      ref int pResultHandle,
      ulong steamIDTradePartner,
      ulong[] pArrayGive,
      uint[] pArrayGiveQuantity,
      ulong[] pArrayGet,
      uint[] pArrayGetQuantity)
    {
      this.CheckIfUsable();
      pResultHandle = 0;
      return NativeCalls.SteamAPI_ISteamInventory_TradeItems(this.m_pSteamInventory, ref pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, (uint) pArrayGiveQuantity.Length, pArrayGet, pArrayGetQuantity, (uint) pArrayGetQuantity.Length);
    }

    public override bool LoadItemDefinitions()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamInventory_LoadItemDefinitions(this.m_pSteamInventory);
    }

    public override bool GetItemDefinitionIDs(out int[] pItemDefIDs)
    {
      this.CheckIfUsable();
      uint punItemDefIDsArraySize = 0;
      NativeCalls.SteamAPI_ISteamInventory_GetItemDefinitionIDs(this.m_pSteamInventory, (int[]) null, ref punItemDefIDsArraySize);
      pItemDefIDs = new int[(int)punItemDefIDsArraySize]; //Changed from intptr
      return NativeCalls.SteamAPI_ISteamInventory_GetItemDefinitionIDs(this.m_pSteamInventory, pItemDefIDs, ref punItemDefIDsArraySize);
    }

    public override bool GetItemDefinitionProperty(
      int iDefinition,
      string pchPropertyName,
      out string pchValueBuffer)
    {
      this.CheckIfUsable();
      uint punValueBufferSize = 0;
      NativeCalls.SteamAPI_ISteamInventory_GetItemDefinitionProperty(this.m_pSteamInventory, iDefinition, pchPropertyName, (StringBuilder) null, ref punValueBufferSize);
      StringBuilder pchValueBuffer1 = new StringBuilder((int) punValueBufferSize);
      bool definitionProperty = NativeCalls.SteamAPI_ISteamInventory_GetItemDefinitionProperty(this.m_pSteamInventory, iDefinition, pchPropertyName, pchValueBuffer1, ref punValueBufferSize);
      pchValueBuffer = pchValueBuffer1.ToString();
      return definitionProperty;
    }
  }
}
