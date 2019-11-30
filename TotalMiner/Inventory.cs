// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Inventory
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class Inventory : ITMInventory
  {
    public static readonly InventoryItem EmptyItem = new InventoryItem();
    public List<short> ItemsChanged = new List<short>();
    public short PackSize;
    public short EquipSize;
    public short TempSize;
    public bool AllowZeroCountItems;
    public bool HasItemsChanged;
    public bool SuspendItemsChangedTransmission;
    protected List<InventoryItem> items;
    private bool internalItemChangedFlag;

    short ITMInventory.PackSize
    {
      get
      {
        return this.PackSize;
      }
    }

    short ITMInventory.EquipSize
    {
      get
      {
        return this.EquipSize;
      }
    }

    short ITMInventory.TempSize
    {
      get
      {
        return this.TempSize;
      }
    }

    short ITMInventory.TotalSize
    {
      get
      {
        return this.TotalSize;
      }
    }

    short ITMInventory.EquipIndexStart
    {
      get
      {
        return this.EquipIndexStart;
      }
    }

    short ITMInventory.EquipIndexEnd
    {
      get
      {
        return this.EquipIndexEnd;
      }
    }

    short ITMInventory.TempIndexStart
    {
      get
      {
        return this.TempIndexStart;
      }
    }

    short ITMInventory.TempIndexEnd
    {
      get
      {
        return this.TempIndexEnd;
      }
    }

    List<InventoryItem> ITMInventory.Items
    {
      get
      {
        return this.items;
      }
    }

    int ITMInventory.ItemCount(StudioForge.TotalMiner.Item itemID)
    {
      return this.ItemCount(itemID);
    }

    void ITMInventory.Clear()
    {
      this.ClearItems();
    }

    int ITMInventory.DecrementItem(StudioForge.TotalMiner.Item itemID, int qty)
    {
      return this.DecrementItem(itemID, qty);
    }

    int ITMInventory.FindItem(StudioForge.TotalMiner.Item itemID)
    {
      return this.FindItem(itemID);
    }

    int ITMInventory.FindItem(StudioForge.TotalMiner.Item itemID, bool mustBeUnequipped)
    {
      return this.FindItem(itemID, mustBeUnequipped);
    }

    int ITMInventory.FindItem(
      int fromSlotID,
      int toSlotID,
      StudioForge.TotalMiner.Item itemID,
      bool mustBeUnequipped)
    {
      return this.FindItem(fromSlotID, toSlotID, itemID, mustBeUnequipped);
    }

    int ITMInventory.FindItem(StudioForge.TotalMiner.Item itemID, int count, ushort durability)
    {
      return this.FindItem(itemID, count, durability);
    }

    int ITMInventory.FindItem(ItemType itemType)
    {
      return this.FindItem(itemType);
    }

    int ITMInventory.FindItem(ItemSubType itemSubType)
    {
      return this.FindItem(itemSubType);
    }

    int ITMInventory.FindItemHighestValue(ItemType itemType)
    {
      return this.FindItemHighestValue(itemType);
    }

    int ITMInventory.FindItemHighestValue(ItemSubType itemSubType)
    {
      return this.FindItemHighestValue(itemSubType);
    }

    void ITMInventory.ReadState(BinaryReader reader, int version)
    {
      this.ReadState(reader, version);
    }

    void ITMInventory.WriteState(BinaryWriter writer)
    {
      this.WriteState(writer);
    }

    public event EventHandler ItemsCleared;

    public event IntEventHandler ItemCleared;

    public event InventoryEventHandler ItemChanged;

    private void Raise_ItemChanged(InventoryItem old, int slotID)
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, new InventoryItemEventArgs(old, slotID));
    }

    private void Raise_ItemsCleared()
    {
      if (this.ItemsCleared == null)
        return;
      this.ItemsCleared((object) this, EventArgs.Empty);
    }

    private void Raise_ItemCleared(int itemID)
    {
      if (this.ItemCleared == null)
        return;
      this.ItemCleared((object) this, new IntEventArgs(itemID));
    }

    public short TotalSize
    {
      get
      {
        return (short) ((int) this.PackSize + (int) this.EquipSize + (int) this.TempSize);
      }
    }

    public short EquipIndexStart
    {
      get
      {
        return this.PackSize;
      }
    }

    public short EquipIndexEnd
    {
      get
      {
        return (short) ((int) this.EquipIndexStart + (int) this.EquipSize);
      }
    }

    public short TempIndexStart
    {
      get
      {
        return (short) ((int) this.PackSize + (int) this.EquipSize);
      }
    }

    public short TempIndexEnd
    {
      get
      {
        return (short) ((int) this.TempIndexStart + (int) this.TempSize);
      }
    }

    public int Count
    {
      get
      {
        return this.items.Count;
      }
    }

    public virtual bool ItemAllowed(StudioForge.TotalMiner.Item item)
    {
      return true;
    }

    public int GetTotalValue()
    {
      int num = 0;
      foreach (InventoryItem inventoryItem in this.items)
      {
        int purchaseValue = inventoryItem.PurchaseValue;
        if (purchaseValue > 0)
          num += Math.Max(1, purchaseValue / 3);
      }
      return num;
    }

    public int GetItemCount(StudioForge.TotalMiner.Item itemID)
    {
      int num = 0;
      foreach (InventoryItem inventoryItem in this.items)
      {
        if (inventoryItem.ItemID == itemID)
          num += inventoryItem.Count;
      }
      return num;
    }

    public int GetEquippedItemCount(StudioForge.TotalMiner.Item itemID)
    {
      int num = 0;
      for (int equipIndexStart = (int) this.EquipIndexStart; equipIndexStart <= (int) this.EquipIndexEnd; ++equipIndexStart)
      {
        if (this.items.Count > equipIndexStart)
        {
          InventoryItem inventoryItem = this.items[equipIndexStart];
          if (inventoryItem.ItemID == itemID)
            num += inventoryItem.Count;
        }
      }
      return num;
    }

    public int TotalPackItemsCount
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.items.Count && index < (int) this.PackSize; ++index)
          num += this.items[index].Count;
        return num;
      }
    }

    public int TotalEquipItemsCount
    {
      get
      {
        int num = 0;
        for (int packSize = (int) this.PackSize; packSize < this.items.Count && packSize < (int) this.PackSize + (int) this.EquipSize; ++packSize)
          num += this.items[packSize].Count;
        return num;
      }
    }

    public int TotalTempItemsCount
    {
      get
      {
        int num = 0;
        for (int index = (int) this.PackSize + (int) this.EquipSize; index < this.items.Count && index < (int) this.PackSize + (int) this.EquipSize + (int) this.TempSize; ++index)
          num += this.items[index].Count;
        return num;
      }
    }

    public int LastItemIndex
    {
      get
      {
        int num = -1;
        for (int index = 0; index < this.items.Count; ++index)
        {
          if (this.items[index].ItemID != StudioForge.TotalMiner.Item.None)
            num = index;
        }
        return num;
      }
    }

    public bool HasFreeSlot
    {
      get
      {
        if (this.items.Count < (int) this.PackSize)
          return true;
        for (int index = 0; index < this.items.Count; ++index)
        {
          if (this.items[index].ItemID == StudioForge.TotalMiner.Item.None)
            return true;
        }
        return false;
      }
    }

    public InventoryItem this[int index]
    {
      get
      {
        return this.GetItem(index);
      }
      set
      {
        this.InsertItem((ushort) index, value);
      }
    }

    protected InventoryItem GetItem(int index)
    {
      if (index >= 0 && index < this.items.Count)
        return this.items[index];
      return Inventory.EmptyItem;
    }

    public int GetRandomItem(PcgRandom random)
    {
      int max = 0;
      for (int index = 0; index < (int) this.PackSize; ++index)
      {
        if (this.GetItem(index).Count > 0)
          ++max;
      }
      if (max > 0)
      {
        int num = random.Next(max);
        for (int index = 0; index < (int) this.PackSize; ++index)
        {
          if (this.GetItem(index).Count > 0 && num-- == 0)
            return index;
        }
      }
      return -1;
    }

    public Inventory(int packSize)
      : this(packSize, 0, 0)
    {
    }

    public Inventory(int packSize, int equipSize, int tempSize)
      : this(packSize, equipSize, tempSize, false)
    {
    }

    public Inventory(int packSize, int equipSize, int tempSize, bool allowZeroCountItems)
    {
      this.AllowZeroCountItems = allowZeroCountItems;
      this.PackSize = (short) packSize;
      this.EquipSize = (short) equipSize;
      this.TempSize = (short) tempSize;
      this.CreateItemArray();
    }

    public Inventory(Inventory inventory)
    {
      this.PackSize = inventory.PackSize;
      this.EquipSize = inventory.EquipSize;
      this.TempSize = inventory.TempSize;
      this.AllowZeroCountItems = inventory.AllowZeroCountItems;
      this.items = new List<InventoryItem>((IEnumerable<InventoryItem>) inventory.items);
    }

    protected virtual void CreateItemArray()
    {
      this.items = new List<InventoryItem>(0);
    }

    public Inventory(short packSize, short equipSize, short tempSize, Inventory inventory)
    {
      this.PackSize = packSize;
      this.EquipSize = equipSize;
      this.TempSize = tempSize;
      this.AllowZeroCountItems = inventory.AllowZeroCountItems;
      this.items = new List<InventoryItem>(inventory.items.Count);
      this.internalItemChangedFlag = false;
      for (int index = 0; index < inventory.items.Count; ++index)
      {
        InventoryItem inventoryItem = inventory.items[index];
        if (this.ItemAllowed(inventoryItem.ItemID))
        {
          this.items.Add(inventoryItem);
          this.FlagItemChanged(InventoryItem.Empty, this.items.Count - 1);
        }
      }
      if (!this.internalItemChangedFlag)
        return;
      this.HasItemsChanged = true;
    }

    public void FlagItemChanged(InventoryItem old, int slotID)
    {
      if (!this.ItemsChanged.Contains((short) slotID))
        this.ItemsChanged.Add((short) slotID);
      this.internalItemChangedFlag = true;
      this.Raise_ItemChanged(old, slotID);
    }

    public void TransferTo(Inventory destInv)
    {
      int num = Math.Min(this.items.Count, (int) this.PackSize);
      for (int index = 0; index < num; ++index)
      {
        InventoryItem transferItem = this.items[index];
        if (transferItem.ItemID != StudioForge.TotalMiner.Item.None)
        {
          transferItem.Count -= destInv.TransferTo(transferItem);
          this.items[index] = transferItem;
        }
      }
    }

    public int TransferTo(InventoryItem transferItem)
    {
      return this.TransferTo(transferItem, transferItem.Count);
    }

    public int TransferTo(InventoryItem transferItem, int count)
    {
      int num1 = count;
      int stackSize = ItemData.GetStackSize(transferItem.ItemID);
      this.internalItemChangedFlag = false;
      if (stackSize > 1)
      {
        for (int slotID = 0; count > 0 && slotID < this.items.Count && slotID < (int) this.PackSize; ++slotID)
        {
          InventoryItem inventoryItem = this.items[slotID];
          if (transferItem.ItemID == inventoryItem.ItemID && inventoryItem.Count < stackSize)
          {
            int num2 = Math.Min(count, stackSize - inventoryItem.Count);
            inventoryItem.Count += num2;
            if (this.ItemAllowed(inventoryItem.ItemID))
            {
              InventoryItem old = this.items[slotID];
              this.items[slotID] = inventoryItem;
              this.FlagItemChanged(old, slotID);
              count -= num2;
            }
          }
        }
      }
      for (int slotID = 0; count > 0 && slotID < (int) this.PackSize; ++slotID)
      {
        while (slotID >= this.items.Count)
          this.items.Add(InventoryItem.Empty);
        InventoryItem inventoryItem = this.items[slotID];
        if (stackSize > 1 && inventoryItem.ItemID == transferItem.ItemID && inventoryItem.Count < stackSize)
        {
          int num2 = Math.Min(count, stackSize - inventoryItem.Count);
          inventoryItem.Count += num2;
          if (this.ItemAllowed(inventoryItem.ItemID))
          {
            InventoryItem old = this.items[slotID];
            this.items[slotID] = inventoryItem;
            this.FlagItemChanged(old, slotID);
            count -= num2;
          }
        }
        else if (inventoryItem.Count == 0 && this.CanUseSlot(slotID, transferItem.ItemID))
        {
          int num2 = Math.Min(stackSize, count);
          inventoryItem.ItemID = transferItem.ItemID;
          inventoryItem.Count = num2;
          inventoryItem.Durability = transferItem.Durability;
          if (this.ItemAllowed(inventoryItem.ItemID))
          {
            InventoryItem old = this.items[slotID];
            this.items[slotID] = inventoryItem;
            this.FlagItemChanged(old, slotID);
            count -= num2;
          }
        }
      }
      if (this.internalItemChangedFlag)
        this.HasItemsChanged = true;
      return num1 - count;
    }

    public int TransferTo(InventoryItem srcItem, int count, int slotID)
    {
      if (!this.ItemAllowed(srcItem.ItemID))
        return 0;
      while (this.items.Count <= slotID)
        this.items.Add(InventoryItem.Empty);
      InventoryItem inventoryItem = this.items[slotID];
      if (inventoryItem.ItemID != StudioForge.TotalMiner.Item.None && inventoryItem.ItemID != srcItem.ItemID)
        return 0;
      int stackSize = ItemData.GetStackSize(srcItem.ItemID);
      if (inventoryItem.Count >= stackSize)
        return 0;
      int num = Math.Min(count, stackSize - inventoryItem.Count);
      if (num < 1)
        return 0;
      this.internalItemChangedFlag = false;
      inventoryItem.ItemID = srcItem.ItemID;
      inventoryItem.Count += num;
      inventoryItem.Durability = srcItem.Durability;
      InventoryItem old = this.items[slotID];
      this.items[slotID] = inventoryItem;
      this.FlagItemChanged(old, slotID);
      if (this.internalItemChangedFlag)
        this.HasItemsChanged = true;
      return num;
    }

    protected virtual bool CanUseSlot(int slotID, StudioForge.TotalMiner.Item itemID)
    {
      return true;
    }

    public int FindOrGetFreeSlotForItem(StudioForge.TotalMiner.Item itemID)
    {
      return this.FindOrGetFreeSlotForItem(new InventoryItem(itemID));
    }

    public int FindOrGetFreeSlotForItem(InventoryItem item)
    {
      return this.FindOrGetFreeSlotForItem(item, (int) this.PackSize, false);
    }

    public int FindOrGetFreeSlotForItem(InventoryItem item, bool ignoreMax)
    {
      return this.FindOrGetFreeSlotForItem(item, (int) this.PackSize, ignoreMax);
    }

    public int FindOrGetFreeSlotForItem(InventoryItem item, int maxIndexToSearchTo, bool ignoreMax)
    {
      if (item.MaxDurability == (ushort) 0 || ignoreMax)
      {
        int num = ignoreMax ? 0 : ItemData.GetStackSize(item.ItemID);
        if (maxIndexToSearchTo > (int) this.PackSize)
          maxIndexToSearchTo = (int) this.PackSize;
        for (int index = 0; index < this.items.Count && index < maxIndexToSearchTo; ++index)
        {
          InventoryItem inventoryItem = this.items[index];
          if ((this.AllowZeroCountItems ? inventoryItem.ItemID_Raw : inventoryItem.ItemID) == item.ItemID && (ignoreMax || inventoryItem.Count < num))
            return index;
        }
      }
      return this.FindOrAddItem(item.ItemID, maxIndexToSearchTo);
    }

    public int FindItem(StudioForge.TotalMiner.Item itemID)
    {
      return this.FindItem(itemID, false);
    }

    public int FindItem(StudioForge.TotalMiner.Item itemID, bool mustBeUnequipped)
    {
      return this.FindItem(0, (int) this.PackSize, itemID, mustBeUnequipped);
    }

    public int FindItem(int fromSlotID, int toSlotID, StudioForge.TotalMiner.Item itemID, bool mustBeUnequipped)
    {
      if (fromSlotID < 0)
        fromSlotID = 0;
      for (int index = fromSlotID; index < this.items.Count && index < toSlotID; ++index)
      {
        InventoryItem inventoryItem = this.items[index];
        StudioForge.TotalMiner.Item obj = this.AllowZeroCountItems ? inventoryItem.ItemID_Raw : inventoryItem.ItemID;
        if (itemID == obj && (inventoryItem.Count > 0 || this.AllowZeroCountItems || itemID == StudioForge.TotalMiner.Item.None) && (!mustBeUnequipped || !this.IsItemEquipped(index)))
          return index;
      }
      if (itemID != StudioForge.TotalMiner.Item.None || this.items.Count >= (int) this.PackSize)
        return -1;
      this.items.Add(InventoryItem.Empty);
      return this.items.Count - 1;
    }

    public int FindItem(ItemType itemType)
    {
      for (int index = 0; index < this.items.Count && index < (int) this.PackSize; ++index)
      {
        if (ItemData.IsItemType(this.items[index].ItemID, itemType))
          return index;
      }
      return -1;
    }

    public int FindItemHighestValue(ItemType itemType)
    {
      int num1 = int.MinValue;
      int num2 = -1;
      for (int index = 0; index < this.items.Count && index < (int) this.PackSize; ++index)
      {
        InventoryItem inventoryItem = this.items[index];
        if (ItemData.IsItemType(inventoryItem.ItemID, itemType))
        {
          int minCustBuyPrice = ItemData.GetMinCustBuyPrice(inventoryItem.ItemID);
          if (minCustBuyPrice > num1)
          {
            num1 = minCustBuyPrice;
            num2 = index;
          }
        }
      }
      return num2;
    }

    public int FindItem(ItemSubType itemSubType)
    {
      for (int index = 0; index < this.items.Count && index < (int) this.PackSize; ++index)
      {
        if (ItemData.IsSubTypeAny(this.items[index].ItemID, itemSubType))
          return index;
      }
      return -1;
    }

    public int FindItemHighestValue(ItemSubType itemSubType)
    {
      int num1 = int.MinValue;
      int num2 = -1;
      for (int index = 0; index < this.items.Count && index < (int) this.PackSize; ++index)
      {
        InventoryItem inventoryItem = this.items[index];
        if (ItemData.IsSubTypeAny(inventoryItem.ItemID, itemSubType))
        {
          int minCustBuyPrice = ItemData.GetMinCustBuyPrice(inventoryItem.ItemID);
          if (minCustBuyPrice > num1)
          {
            num1 = minCustBuyPrice;
            num2 = index;
          }
        }
      }
      return num2;
    }

    protected virtual bool IsItemEquipped(int index)
    {
      return false;
    }

    public int FindItem(StudioForge.TotalMiner.Item itemID, int count, ushort durability)
    {
      for (short index = 0; (int) index < this.items.Count && (int) index < (int) this.PackSize; ++index)
      {
        InventoryItem inventoryItem = this.items[(int) index];
        if (inventoryItem.ItemID == itemID && inventoryItem.Count == count && (int) inventoryItem.Durability == (int) durability)
          return (int) index;
      }
      return -1;
    }

    public int FindOrAddItem(StudioForge.TotalMiner.Item itemID)
    {
      return this.FindOrAddItem(itemID, (int) this.PackSize);
    }

    public int FindOrAddItem(StudioForge.TotalMiner.Item itemID, int maxIndexToSearchTo)
    {
      int slotID = -1;
      do
      {
        slotID = this.FindItem(slotID + 1, maxIndexToSearchTo, StudioForge.TotalMiner.Item.None, false);
      }
      while (slotID >= 0 && !this.CanUseSlot(slotID, itemID));
      if (slotID < 0 && this.items.Count < maxIndexToSearchTo)
      {
        this.items.Add(InventoryItem.Empty);
        slotID = (int) (short) (this.items.Count - 1);
      }
      return slotID;
    }

    public bool HasItem(StudioForge.TotalMiner.Item item)
    {
      return this.FindItem(item) >= 0;
    }

    public bool HasItems()
    {
      for (short index = 0; (int) index < this.items.Count && (int) index < (int) this.PackSize; ++index)
      {
        InventoryItem inventoryItem = this.items[(int) index];
        if ((this.AllowZeroCountItems ? inventoryItem.ItemID_Raw : inventoryItem.ItemID) != StudioForge.TotalMiner.Item.None)
          return true;
      }
      return false;
    }

    public bool HasItem(ItemSubType subType, bool exact)
    {
      for (short index = 0; (int) index < this.items.Count && (int) index < (int) this.PackSize; ++index)
      {
        InventoryItem inventoryItem = this.items[(int) index];
        StudioForge.TotalMiner.Item itemID = this.AllowZeroCountItems ? inventoryItem.ItemID_Raw : inventoryItem.ItemID;
        if ((exact ? (ItemData.IsSubType(itemID, subType) ? 1 : 0) : (ItemData.IsSubTypeAny(itemID, subType) ? 1 : 0)) != 0)
          return true;
      }
      return false;
    }

    public int ItemCount(StudioForge.TotalMiner.Item itemID)
    {
      return this.ItemCountCore(itemID, 0, (int) this.PackSize);
    }

    public int ItemEquipCount(StudioForge.TotalMiner.Item itemID)
    {
      return this.ItemCountCore(itemID, (int) this.EquipIndexStart, (int) this.EquipIndexEnd);
    }

    public int ItemTempCount(StudioForge.TotalMiner.Item itemID)
    {
      return this.ItemCountCore(itemID, (int) this.TempIndexStart, (int) this.TempIndexEnd);
    }

    protected int ItemCountCore(StudioForge.TotalMiner.Item itemID, int start, int end)
    {
      int num = 0;
      for (int index = start; index < this.items.Count && index < end; ++index)
      {
        InventoryItem inventoryItem = this.items[index];
        if (inventoryItem.ItemID == itemID)
          num += inventoryItem.Count;
      }
      return num;
    }

    public void SetItemCount(int slotID, int count)
    {
      if (slotID < 0 || slotID >= this.items.Count)
        return;
      InventoryItem inventoryItem = this.items[slotID];
      InventoryItem old = inventoryItem;
      bool flag = count != inventoryItem.Count;
      inventoryItem.Count = count;
      this.items[slotID] = inventoryItem;
      if (!flag)
        return;
      this.FlagItemChanged(old, slotID);
      this.HasItemsChanged = true;
    }

    public void SetAllItemCounts(ushort count)
    {
      this.internalItemChangedFlag = false;
      for (int slotID = 0; slotID < this.items.Count && slotID < (int) this.PackSize; ++slotID)
      {
        InventoryItem inventoryItem;
        InventoryItem old = inventoryItem = this.items[slotID];
        bool flag = (int) count != inventoryItem.Count;
        inventoryItem.Count = (int) count;
        this.items[slotID] = inventoryItem;
        if (flag)
          this.FlagItemChanged(old, slotID);
      }
      if (!this.internalItemChangedFlag)
        return;
      this.HasItemsChanged = true;
    }

    public void SetItemDurability(int slotID, ushort durability)
    {
      if (slotID < 0 || slotID >= this.items.Count)
        return;
      InventoryItem inventoryItem = this.items[slotID];
      InventoryItem old = inventoryItem;
      bool flag = (int) durability != (int) inventoryItem.Durability;
      inventoryItem.Durability = durability;
      this.items[slotID] = inventoryItem;
      if (!flag)
        return;
      this.FlagItemChanged(old, slotID);
      this.HasItemsChanged = true;
    }

    protected void InsertItem(ushort slotID, InventoryItem item)
    {
      if ((int) slotID >= (int) this.TotalSize || !this.ItemAllowed(item.ItemID))
        return;
      if ((int) slotID >= this.items.Count)
      {
        if (item.ItemID_Raw == StudioForge.TotalMiner.Item.None || item.Count <= 0)
          return;
        while (this.items.Count < (int) slotID)
          this.items.Add(Inventory.EmptyItem);
        this.items.Add(item);
        this.FlagItemChanged(Inventory.EmptyItem, this.items.Count - 1);
        this.HasItemsChanged = true;
      }
      else
      {
        InventoryItem old = this.items[(int) slotID];
        this.items[(int) slotID] = item;
        if (old.ItemID_Raw == item.ItemID_Raw && old.Count == item.Count && (int) old.Durability == (int) item.Durability)
          return;
        this.FlagItemChanged(old, (int) slotID);
        this.HasItemsChanged = true;
      }
    }

    public void SwapItem(int slotID1, int slotID2)
    {
      int num = (int) this.PackSize + (int) this.EquipSize + (int) this.TempSize;
      if (slotID1 == slotID2 || slotID1 < 0 || (slotID1 >= num || slotID2 < 0) || slotID2 >= num)
        return;
      InventoryItem inventoryItem = this[slotID1];
      this[slotID1] = this[slotID2];
      this[slotID2] = inventoryItem;
    }

    public virtual int DecrementItem(int slotID)
    {
      return this.DecrementItem(slotID, 1);
    }

    public int DecrementItem(int slotID, int count)
    {
      if (slotID < 0 || slotID >= this.items.Count || this.items[slotID].Count <= 0)
        return 0;
      InventoryItem inventoryItem = this.items[slotID];
      InventoryItem old = inventoryItem;
      inventoryItem.Count -= Math.Min(count, inventoryItem.Count);
      this.items[slotID] = inventoryItem;
      this.FlagItemChanged(old, slotID);
      this.HasItemsChanged = true;
      if (inventoryItem.Count != 0)
        return inventoryItem.Count;
      this.OnItemCleared(slotID);
      return 0;
    }

    public ushort DecrementItemDurability(int slotID, ushort count)
    {
      if (slotID < 0 || slotID >= this.items.Count || this.items[slotID].Durability <= (ushort) 0)
        return 0;
      InventoryItem inventoryItem = this.items[slotID];
      InventoryItem old = inventoryItem;
      inventoryItem.Durability -= Math.Min(count, inventoryItem.Durability);
      this.items[slotID] = inventoryItem;
      this.FlagItemChanged(old, slotID);
      this.HasItemsChanged = true;
      return inventoryItem.Durability;
    }

    public int DecrementItem(StudioForge.TotalMiner.Item itemID, int count)
    {
      return this.DecrementItem(itemID, count, false);
    }

    public int DecrementItem(StudioForge.TotalMiner.Item itemID, int count, bool includeBodySlots)
    {
      if (itemID == StudioForge.TotalMiner.Item.None || count == 0)
        return 0;
      for (int slotID = 0; slotID < this.items.Count && slotID < (int) this.PackSize && count > 0; ++slotID)
      {
        InventoryItem inventoryItem = this.items[slotID];
        if (inventoryItem.ItemID == itemID && inventoryItem.Count < ItemData.GetStackSize(inventoryItem.ItemID))
        {
          InventoryItem old = inventoryItem;
          int num = Math.Min(inventoryItem.Count, count);
          count -= num;
          inventoryItem.Count -= num;
          this.items[slotID] = inventoryItem;
          this.FlagItemChanged(old, slotID);
          this.HasItemsChanged = true;
          this.OnItemCleared(slotID);
        }
      }
      int num1 = includeBodySlots ? this.items.Count : Math.Min(this.items.Count, (int) this.PackSize);
      for (int slotID = 0; slotID < num1 && count > 0; ++slotID)
      {
        if (this.items[slotID].ItemID == itemID)
        {
          InventoryItem inventoryItem = this.items[slotID];
          InventoryItem old = inventoryItem;
          int num2 = Math.Min(inventoryItem.Count, count);
          count -= num2;
          inventoryItem.Count -= num2;
          this.items[slotID] = inventoryItem;
          this.FlagItemChanged(old, slotID);
          this.HasItemsChanged = true;
          this.OnItemCleared(slotID);
        }
      }
      return count;
    }

    public void IncrementItem(StudioForge.TotalMiner.Item itemID, int count)
    {
      if (itemID == StudioForge.TotalMiner.Item.None || count < 1)
        return;
      for (int slotID = 0; slotID < this.items.Count; ++slotID)
      {
        InventoryItem inventoryItem = this.items[slotID];
        if (inventoryItem.ItemID_Raw == itemID && (this.AllowZeroCountItems || inventoryItem.Count > 0))
        {
          InventoryItem old = inventoryItem;
          inventoryItem.Count += count;
          this.items[slotID] = inventoryItem;
          this.FlagItemChanged(old, slotID);
          this.HasItemsChanged = true;
          break;
        }
      }
    }

    public void ClearItem(StudioForge.TotalMiner.Item itemID)
    {
      for (int slotID = 0; slotID < this.items.Count; ++slotID)
      {
        InventoryItem old = this.items[slotID];
        if (old.ItemID == itemID)
        {
          this.items[slotID] = Inventory.EmptyItem;
          this.FlagItemChanged(old, slotID);
          this.OnItemCleared(slotID);
        }
      }
      this.Raise_ItemCleared((int) itemID);
    }

    public void ClearItems()
    {
      this.ClearItems(this.items.Count);
    }

    private void ClearItems(int count)
    {
      for (int slotID = 0; slotID < count && slotID < this.items.Count; ++slotID)
      {
        InventoryItem old = this.items[slotID];
        if (old.ItemID_Raw != StudioForge.TotalMiner.Item.None)
        {
          this.items[slotID] = Inventory.EmptyItem;
          this.FlagItemChanged(old, slotID);
          this.OnItemCleared(slotID);
          this.Raise_ItemCleared((int) old.ItemID);
        }
      }
    }

    private void OnItemCleared(int slotID)
    {
      this.internalItemChangedFlag = false;
      while (this.items.Count > 0)
      {
        InventoryItem inventoryItem = this.items[this.items.Count - 1];
        if (inventoryItem.ItemID_Raw == StudioForge.TotalMiner.Item.None || !this.AllowZeroCountItems && inventoryItem.ItemID == StudioForge.TotalMiner.Item.None)
        {
          this.items.RemoveAt(this.items.Count - 1);
          if (inventoryItem.ItemID != StudioForge.TotalMiner.Item.None)
            this.internalItemChangedFlag = true;
        }
        else
          break;
      }
      if (!this.internalItemChangedFlag)
        return;
      this.HasItemsChanged = true;
    }

    public void CopyFromWithPreClear(Inventory inventory)
    {
      this.items.Clear();
      this.internalItemChangedFlag = false;
      int num = (int) this.PackSize + (int) this.EquipSize + (int) this.TempSize;
      for (int index = 0; index < inventory.items.Count && index < num; ++index)
      {
        InventoryItem inventoryItem = inventory.items[index];
        if (this.ItemAllowed(inventoryItem.ItemID))
        {
          this.items.Add(inventoryItem);
          this.FlagItemChanged(InventoryItem.Empty, this.items.Count - 1);
        }
      }
      if (!this.internalItemChangedFlag)
        return;
      this.HasItemsChanged = true;
    }

    public void CopyFrom(Inventory inventory)
    {
      for (int index = 0; index < inventory.items.Count; ++index)
      {
        InventoryItem inventoryItem = inventory.items[index];
        if (this.ItemAllowed(inventoryItem.ItemID))
          this.AddToInventory(inventoryItem);
      }
    }

    public void CopyFrom(Inventory inventory, InventoryItem item)
    {
      if (item.ItemID == StudioForge.TotalMiner.Item.None || !this.ItemAllowed(item.ItemID))
        return;
      InventoryItem inventoryItem1 = new InventoryItem(item.ItemID);
      for (int index = 0; index < inventory.items.Count; ++index)
      {
        InventoryItem inventoryItem2 = inventory.items[index];
        if (inventoryItem2.ItemID == item.ItemID)
        {
          inventoryItem1.Count = Math.Min(item.Count, inventoryItem2.Count);
          int inventory1 = this.AddToInventory(inventoryItem1);
          item.Count -= inventory1;
          if (item.Count < 1 || inventory1 == 0)
            break;
        }
      }
    }

    public void MoveFrom(Inventory inventory)
    {
      for (int slotID = 0; slotID < inventory.items.Count; ++slotID)
      {
        InventoryItem inventoryItem = inventory.items[slotID];
        if (inventoryItem.ItemID != StudioForge.TotalMiner.Item.None && this.ItemAllowed(inventoryItem.ItemID))
        {
          int inventory1 = this.AddToInventory(inventoryItem);
          inventory.DecrementItem(slotID, inventory1);
        }
      }
    }

    public void MoveFrom(Inventory inventory, InventoryItem item)
    {
      if (item.ItemID == StudioForge.TotalMiner.Item.None || !this.ItemAllowed(item.ItemID))
        return;
      for (int slotID = 0; slotID < inventory.items.Count; ++slotID)
      {
        InventoryItem inventoryItem1 = inventory.items[slotID];
        if (inventoryItem1.ItemID == item.ItemID)
        {
          InventoryItem inventoryItem2 = inventoryItem1;
          inventoryItem2.Count = Math.Min(item.Count, inventoryItem1.Count);
          int inventory1 = this.AddToInventory(inventoryItem2);
          item.Count -= inventory1;
          inventory.DecrementItem(slotID, inventory1);
          if (item.Count < 1 || inventory1 == 0)
            break;
        }
      }
    }

    public int AddToInventory(StudioForge.TotalMiner.Item itemID)
    {
      return this.AddToInventory(new InventoryItem(itemID, 1));
    }

    public int AddToInventory(StudioForge.TotalMiner.Item itemID, int count)
    {
      return this.AddToInventory(new InventoryItem(itemID)
      {
        Count = count
      });
    }

    public int AddToInventory(InventoryItem item)
    {
      return this.AddToInventory(item, true);
    }

    public int AddToInventory(InventoryItem item, bool clipStackSize)
    {
      int num1 = 0;
      if (item.ItemID != StudioForge.TotalMiner.Item.None && this.ItemAllowed(item.ItemID))
      {
        this.internalItemChangedFlag = false;
        while (item.Count > 0)
        {
          int getFreeSlotForItem = this.FindOrGetFreeSlotForItem(item, !clipStackSize);
          if (getFreeSlotForItem >= 0)
          {
            InventoryItem inventoryItem = this.items[getFreeSlotForItem];
            InventoryItem old = inventoryItem;
            bool flag = inventoryItem.ItemID != item.ItemID || inventoryItem.Count != item.Count || (int) inventoryItem.Durability != (int) item.Durability;
            inventoryItem.ItemID = item.ItemID;
            int num2 = clipStackSize ? Math.Min(item.Count, ItemData.GetStackSize(item.ItemID) - inventoryItem.Count) : item.Count;
            item.Count -= num2;
            num1 += num2;
            inventoryItem.Count += num2;
            inventoryItem.Durability = item.Durability;
            this.items[getFreeSlotForItem] = inventoryItem;
            if (flag)
              this.FlagItemChanged(old, getFreeSlotForItem);
          }
          else
            break;
        }
        if (this.internalItemChangedFlag)
          this.HasItemsChanged = true;
      }
      return num1;
    }

    public void Sort()
    {
      int capacity = Math.Min(this.items.Count, (int) this.PackSize);
      List<InventoryItem> inventoryItemList = new List<InventoryItem>(capacity);
      for (int index1 = 0; index1 < capacity; ++index1)
      {
        InventoryItem inventoryItem1 = this.items[index1];
        for (int index2 = 0; index2 < inventoryItemList.Count; ++index2)
        {
          InventoryItem inventoryItem2 = inventoryItemList[index2];
          if (inventoryItem2.ItemID == inventoryItem1.ItemID && inventoryItem2.Durability == (ushort) 0 && inventoryItem2.Count < ItemData.GetStackSize(inventoryItem1.ItemID))
          {
            int num = Math.Min(inventoryItem1.Count, ItemData.GetStackSize(inventoryItem1.ItemID) - inventoryItem2.Count);
            inventoryItem2.Count += num;
            inventoryItem1.Count -= num;
            inventoryItemList[index2] = inventoryItem2;
          }
        }
        inventoryItemList.Add(inventoryItem1);
      }
      inventoryItemList.Sort(new Comparison<InventoryItem>(this.SortItemsByItemType));
      for (int index = 0; index < capacity; ++index)
        this.items[index] = inventoryItemList[index];
    }

    private int SortItemsByItemType(InventoryItem a, InventoryItem b)
    {
      StudioForge.TotalMiner.Item itemId1 = a.ItemID;
      StudioForge.TotalMiner.Item itemId2 = b.ItemID;
      if (itemId1 == StudioForge.TotalMiner.Item.None)
        return 1;
      if (itemId2 == StudioForge.TotalMiner.Item.None)
        return -1;
      if (itemId1 == itemId2)
      {
        if (a.Count != b.Count)
          return b.Count.CompareTo(a.Count);
        return a.Durability.CompareTo(b.Durability);
      }
      if (itemId1 == StudioForge.TotalMiner.Item.GoldPieces)
        return -1;
      if (itemId2 == StudioForge.TotalMiner.Item.GoldPieces)
        return 1;
      ItemTypeDataXML itemTypeDataXml1 = Globals1.ItemTypeData[(int) itemId1];
      ItemTypeDataXML itemTypeDataXml2 = Globals1.ItemTypeData[(int) itemId2];
      if (itemTypeDataXml1.Class != itemTypeDataXml2.Class)
        return itemTypeDataXml2.Class.CompareTo((object) itemTypeDataXml1.Class);
      if (itemTypeDataXml1.Inv != itemTypeDataXml2.Inv)
        return itemTypeDataXml1.Inv.CompareTo((object) itemTypeDataXml2.Inv);
      return itemId1.CompareTo((object) itemId2);
    }

    public void UpdateChanges(List<short> slotIDList, List<InventoryItem> itemList)
    {
      this.internalItemChangedFlag = false;
      for (int index = 0; index < slotIDList.Count; ++index)
      {
        int slotId = (int) slotIDList[index];
        while (this.items.Count <= slotId)
          this.items.Add(new InventoryItem());
        InventoryItem old = this.items[slotId];
        this.items[slotId] = itemList[index];
        this.FlagItemChanged(old, slotId);
      }
      if (!this.internalItemChangedFlag)
        return;
      this.HasItemsChanged = true;
    }

    public void ClearItemsChanged()
    {
      this.ItemsChanged.Clear();
      this.HasItemsChanged = false;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.ReadStateCore(reader, version);
    }

    protected virtual void ReadStateCore(BinaryReader reader, int version)
    {
      this.PackSize = reader.ReadInt16();
      if (version > 145)
        this.EquipSize = reader.ReadInt16();
      if (version > 145)
        this.TempSize = reader.ReadInt16();
      if (version < 146)
      {
        int num1 = (int) reader.ReadInt16();
      }
      if (version < 146)
      {
        int num2 = (int) reader.ReadInt16();
      }
      this.AllowZeroCountItems = reader.ReadBoolean();
      this.items.Clear();
      this.ItemsChanged.Clear();
      this.HasItemsChanged = false;
      int num3 = (int) this.PackSize + (int) this.EquipSize + (int) this.TempSize;
      int num4 = (int) reader.ReadUInt16();
      for (int index = 0; index < num4; ++index)
      {
        int num5 = (int) reader.ReadUInt16();
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.ItemID = (StudioForge.TotalMiner.Item) reader.ReadUInt16();
        inventoryItem.Count = reader.ReadInt32();
        inventoryItem.Durability = reader.ReadUInt16();
        if (inventoryItem.ItemID == StudioForge.TotalMiner.Item.Bedrock || !ItemData.IsEnabled(inventoryItem.ItemID))
          num5 = num3;
        if (num5 < num3)
        {
          while (this.items.Count < num5)
            this.items.Add(new InventoryItem());
          this.items.Add(inventoryItem);
        }
      }
    }

    public void WriteState(BinaryWriter writer)
    {
      this.WriteStateCore(writer);
    }

    protected virtual void WriteStateCore(BinaryWriter writer)
    {
      writer.Write(this.PackSize);
      writer.Write(this.EquipSize);
      writer.Write(this.TempSize);
      writer.Write(this.AllowZeroCountItems);
      ushort num = 0;
      try
      {
        for (int index = 0; index < this.items.Count; ++index)
        {
          if ((this.AllowZeroCountItems ? this.items[index].ItemID_Raw : this.items[index].ItemID) != StudioForge.TotalMiner.Item.None)
            ++num;
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(120, ex);
        writer.Write((ushort) 0);
        return;
      }
      writer.Write(num);
      try
      {
        for (int index = 0; index < this.items.Count; ++index)
        {
          if (num > (ushort) 0)
          {
            InventoryItem inventoryItem = this.items[index];
            if ((this.AllowZeroCountItems ? inventoryItem.ItemID_Raw : inventoryItem.ItemID) != StudioForge.TotalMiner.Item.None)
            {
              this.WriteInventoryItem(writer, (ushort) index, inventoryItem);
              --num;
            }
          }
          else
            break;
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(120, ex);
      }
      while (num-- > (ushort) 0)
        this.WriteInventoryItem(writer, (ushort) ((uint) this.PackSize + (uint) this.TempSize + (uint) this.EquipSize), InventoryItem.Empty);
    }

    protected void WriteInventoryItem(BinaryWriter writer, ushort slotID, InventoryItem item)
    {
      StudioForge.TotalMiner.Item obj = this.AllowZeroCountItems ? item.ItemID_Raw : item.ItemID;
      writer.Write(slotID);
      writer.Write((ushort) obj);
      writer.Write(item.Count);
      writer.Write(item.Durability);
    }

    public virtual void LoadFromSaveData(SaveInventoryState data)
    {
      if (data == null)
        return;
      this.PackSize = data.PackSize;
      this.EquipSize = data.EquipSize;
      this.TempSize = data.TempSize;
      this.AllowZeroCountItems = data.AllowZeroCountItems;
      if (this.items == null)
        this.items = new List<InventoryItem>();
      this.ClearItems();
      foreach (SaveInventoryItem saveInventoryItem in data.Items)
        this.InsertItem(saveInventoryItem.SlotID, new InventoryItem()
        {
          ItemID = (StudioForge.TotalMiner.Item) saveInventoryItem.ItemID,
          Count = saveInventoryItem.Count,
          Durability = saveInventoryItem.Durability
        });
      this.ItemsChanged.Clear();
      this.HasItemsChanged = false;
    }
  }
}
