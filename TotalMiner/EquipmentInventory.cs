// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.EquipmentInventory
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.Storage;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class EquipmentInventory : Inventory
  {
    public int HotBarLeftSlotID = -1;
    public int HotBarRightSlotID = -1;

    public int HeadIndex
    {
      get
      {
        return (int) this.EquipIndexStart;
      }
    }

    public int NeckIndex
    {
      get
      {
        return (int) this.EquipIndexStart + 1;
      }
    }

    public int BodyIndex
    {
      get
      {
        return (int) this.EquipIndexStart + 2;
      }
    }

    public int LegsIndex
    {
      get
      {
        return (int) this.EquipIndexStart + 3;
      }
    }

    public int FeetIndex
    {
      get
      {
        return (int) this.EquipIndexStart + 4;
      }
    }

    public int LeftSideIndex
    {
      get
      {
        return (int) this.EquipIndexStart + 5;
      }
    }

    public int RightSideIndex
    {
      get
      {
        return (int) this.EquipIndexStart + 6;
      }
    }

    public int LeftHandIndex
    {
      get
      {
        return this.HotBarLeftSlotID;
      }
    }

    public int RightHandIndex
    {
      get
      {
        return this.HotBarRightSlotID;
      }
    }

    public int GetEquipSlotID(Item itemID)
    {
      return this.GetEquipSlotID(Globals1.ItemTypeData[(int) itemID].Equip);
    }

    public int GetEquipSlotID(EquipIndex equipIndex)
    {
      switch (equipIndex)
      {
        case EquipIndex.None:
          return 0;
        case EquipIndex.LeftHand:
          return this.HotBarLeftSlotID;
        case EquipIndex.RightHand:
          return this.HotBarRightSlotID;
        default:
          return (int) ((byte) this.EquipIndexStart + equipIndex - (byte) 1);
      }
    }

    public InventoryItem Head
    {
      get
      {
        return this.GetItem(this.HeadIndex);
      }
      set
      {
        this.InsertItem((ushort) this.HeadIndex, value);
      }
    }

    public InventoryItem Neck
    {
      get
      {
        return this.GetItem(this.NeckIndex);
      }
      set
      {
        this.InsertItem((ushort) this.NeckIndex, value);
      }
    }

    public InventoryItem Body
    {
      get
      {
        return this.GetItem(this.BodyIndex);
      }
      set
      {
        this.InsertItem((ushort) this.BodyIndex, value);
      }
    }

    public InventoryItem Legs
    {
      get
      {
        return this.GetItem(this.LegsIndex);
      }
      set
      {
        this.InsertItem((ushort) this.LegsIndex, value);
      }
    }

    public InventoryItem Feet
    {
      get
      {
        return this.GetItem(this.FeetIndex);
      }
      set
      {
        this.InsertItem((ushort) this.FeetIndex, value);
      }
    }

    public InventoryItem LeftSide
    {
      get
      {
        return this.GetItem(this.LeftSideIndex);
      }
      set
      {
        this.InsertItem((ushort) this.LeftSideIndex, value);
      }
    }

    public InventoryItem RightSide
    {
      get
      {
        return this.GetItem(this.RightSideIndex);
      }
      set
      {
        this.InsertItem((ushort) this.RightSideIndex, value);
      }
    }

    public InventoryItem LeftHand
    {
      get
      {
        return this.GetItem(this.LeftHandIndex);
      }
      set
      {
        this.InsertItem((ushort) this.LeftHandIndex, value);
      }
    }

    public InventoryItem RightHand
    {
      get
      {
        return this.GetItem(this.RightHandIndex);
      }
      set
      {
        this.InsertItem((ushort) this.RightHandIndex, value);
      }
    }

    public bool HasAmmunition(Item itemID)
    {
      if (ItemData.IsSubType(itemID, ItemSubType.GrenadeLauncher))
        return this.IsEquippedInHand(ItemSubType.Grenade);
      if (ItemData.IsSubType(itemID, ItemSubType.Bow))
        return this.IsEquippedInHand(ItemSubType.Arrow);
      return true;
    }

    public bool IsEquippedInHand(Item itemID)
    {
      if (this.LeftHand.ItemID != itemID)
        return this.RightHand.ItemID == itemID;
      return true;
    }

    public bool IsEquippedInHand(ItemSubType subType)
    {
      if (!ItemData.IsSubTypeAny(this.LeftHand.ItemID, subType))
        return ItemData.IsSubTypeAny(this.RightHand.ItemID, subType);
      return true;
    }

    protected override bool IsItemEquipped(int index)
    {
      if (index != this.LeftHandIndex)
        return index == this.RightHandIndex;
      return true;
    }

    public EquipmentInventory(int packSize, int equipSize, int tempSize)
      : base(packSize, equipSize, tempSize)
    {
    }

    protected override void CreateItemArray()
    {
      this.items = new List<InventoryItem>((int) this.PackSize + (int) this.EquipSize + (int) this.TempSize);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version <= 145)
        return;
      this.HotBarLeftSlotID = (int) reader.ReadUInt16();
      this.HotBarRightSlotID = (int) reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((ushort) this.HotBarLeftSlotID);
      writer.Write((ushort) this.HotBarRightSlotID);
    }

    public override void LoadFromSaveData(SaveInventoryState data)
    {
      base.LoadFromSaveData(data);
      if (data == null)
        return;
      this.HotBarLeftSlotID = (int) data.HotBarLeftID;
      this.HotBarRightSlotID = (int) data.HotBarRightID;
    }
  }
}
