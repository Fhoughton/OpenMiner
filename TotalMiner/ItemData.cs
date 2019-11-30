// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemData
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  public static class ItemData
  {
    public static bool IsEnabled(Item itemID)
    {
      ItemDataXML itemDataXml = Globals1.ItemData[(int) itemID];
      if (itemDataXml.IsValid)
        return itemDataXml.IsEnabled;
      return false;
    }

    public static string ToString(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].Name;
    }

    public static string ToString(Block blockID)
    {
      return Globals1.ItemData[(int) blockID].Name;
    }

    public static byte GetParticleLight(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].ParticleLight;
    }

    public static ushort GetBurnTime(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].BurnTime;
    }

    public static int GetMinCustBuyPrice(Item itemID)
    {
      return ItemData.GetMinCustBuyPrice(Globals1.ItemData[(int) itemID].MinCSPrice);
    }

    public static int GetMinCustBuyPrice(int minCustSellPrice)
    {
      if (minCustSellPrice >= 0)
        return (int) ((double) minCustSellPrice * 1.20000004768372);
      return minCustSellPrice;
    }

    public static int GetMinCustSellPrice(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].MinCSPrice;
    }

    public static ushort GetItemDurability(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].Durability;
    }

    public static bool HasDurability(Item itemID)
    {
      return ItemData.GetItemDurability(itemID) > (ushort) 0;
    }

    public static int GetStackSize(Item itemID)
    {
      if (ItemData.GetItemDurability(itemID) == (ushort) 0)
        return Globals1.ItemData[(int) itemID].StackSize;
      return 1;
    }

    public static bool IsSubTypeAny(Item itemID, ItemSubType subTypes)
    {
      return (Globals1.ItemTypeData[(int) itemID].SubType & subTypes) > ItemSubType.None;
    }

    public static bool IsSubTypeAny(Block blockID, ItemSubType subTypes)
    {
      return (Globals1.ItemTypeData[(int) blockID].SubType & subTypes) > ItemSubType.None;
    }

    public static bool IsSubTypeAny(byte blockID, ItemSubType subTypes)
    {
      return (Globals1.ItemTypeData[(int) blockID].SubType & subTypes) > ItemSubType.None;
    }

    public static bool IsSubType(Item itemID, ItemSubType subTypes)
    {
      return (Globals1.ItemTypeData[(int) itemID].SubType & subTypes) == subTypes;
    }

    public static bool IsItemType(Item itemID, ItemType type)
    {
      return Globals1.ItemTypeData[(int) itemID].Type == type;
    }

    public static bool IsItemTypeClass(Item itemID, ItemTypeClass type)
    {
      return Globals1.ItemTypeData[(int) itemID].Class == type;
    }

    public static bool IsItemUse(Item itemID, ItemUse use)
    {
      return Globals1.ItemTypeData[(int) itemID].Use == use;
    }

    public static bool IsBindableWeapon(Item itemID)
    {
      if (ItemData.GetItemType(itemID) == ItemType.Weapon)
        return !ItemData.IsSubTypeAny(itemID, ItemSubType.Arrow | ItemSubType.Grenade);
      return false;
    }

    public static float GetItemStrikeDamage(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].StrikeDamage;
    }

    public static float GetItemStrikeReach(Item itemID)
    {
      return Globals1.ItemData[(int) itemID].StrikeReach;
    }

    public static float GetItemSwingPauseTime(Item itemID)
    {
      return Globals1.ItemSwingTimeData[(int) itemID].Pause;
    }

    public static float GetItemSwingExtendedPauseTime(Item itemID)
    {
      return Globals1.ItemSwingTimeData[(int) itemID].ExtendedPause;
    }

    public static float GetItemSwingRetractTime(Item itemID)
    {
      return Globals1.ItemSwingTimeData[(int) itemID].RetractTime;
    }

    public static bool GetItemSwingRetractSmooth(Item itemID)
    {
      return Globals1.ItemSwingTimeData[(int) itemID].RetractSmooth;
    }

    public static float GetItemSwingTime(Item itemID)
    {
      return Globals1.ItemSwingTimeData[(int) itemID].Time;
    }

    public static bool IsItemSwingable(Item itemID)
    {
      return Globals1.ItemSwingData[(int) Globals1.ItemTypeData[(int) itemID].Swing].IsSwingable;
    }

    private static ushort GetStrikeBlockEfficiency(
      Item itemID,
      Block blockID,
      ref BlockMaterialDataXML materialData,
      ref ItemTypeClassDataXML typeClassData)
    {
      switch (Globals1.SkillData[(int) itemID].UseSkill)
      {
        case SkillType.Strength:
        case SkillType.Attack:
        case SkillType.Defence:
          return materialData.WeaponEfficiency;
        case SkillType.Digging:
          return materialData.ShovelEfficiency;
        case SkillType.Chopping:
          return materialData.HatchetEfficiency;
        case SkillType.Farming:
          switch (typeClassData.ItemClass)
          {
            case ItemTypeClass.Bronze:
              typeClassData = Globals1.ItemTypeClassData[3];
              break;
            case ItemTypeClass.Diamond:
              typeClassData = Globals1.ItemTypeClassData[7];
              break;
          }
          if (ItemData.IsSubType(itemID, ItemSubType.TillTool) || blockID == Block.Crop && ItemData.IsSubType(itemID, ItemSubType.HarvestTool))
            return materialData.ShovelEfficiency;
          return (ushort) ((double) materialData.PickEfficiency * 0.5);
        default:
          return materialData.PickEfficiency;
      }
    }

    public static ItemType GetItemType(Item itemID)
    {
      return Globals1.ItemTypeData[(int) itemID].Type;
    }

    public static ItemUse GetItemUse(Item itemID)
    {
      return Globals1.ItemTypeData[(int) itemID].Use;
    }

    public static EquipIndex GetItemEquipIndex(Item itemID)
    {
      return Globals1.ItemTypeData[(int) itemID].Equip;
    }

    public static bool CanItemBreakBlocks(Item itemID)
    {
      return Globals1.ItemTypeData[(int) itemID].Class != ItemTypeClass.CantMine;
    }

    public static bool CanItemUseKey(Item itemID)
    {
      Item obj = itemID;
      if ((uint) obj <= 140U)
      {
        if (obj != Item.LockedChest && obj != Item.LockedDoorTop)
          goto label_4;
      }
      else if (obj != Item.LockedDoorBottom && obj != Item.LockedDoor)
        goto label_4;
      return true;
label_4:
      return false;
    }

    public static Item ConvertBlockIDToItemID(Item itemID)
    {
      if (itemID > Item.zLastBlockID)
        return itemID;
      Block block = (Block) itemID;
      if ((uint) block <= 150U)
      {
        if ((uint) block <= 72U)
        {
          switch (block)
          {
            case Block.WoodDoorTop:
              break;
            case Block.SteelDoorTop:
              goto label_11;
            case Block.Rope:
              return Item.RopeIcon;
            default:
              goto label_28;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Stairs:
              return Item.StairsIcon;
            case Block.WoodDoorBottom:
              break;
            case Block.SteelDoorBottom:
              goto label_11;
            case Block.Sign:
              return Item.SignIcon;
            case Block.LitFurnace:
              return Item.Furnace;
            case Block.BedHead:
            case Block.BedFoot:
              return Item.Bed;
            case Block.Fence:
              return Item.FenceIcon;
            case Block.LockedDoorTop:
              goto label_12;
            case Block.HalfBlock:
              return Item.HalfBlockIcon;
            case Block.Ramp:
              return Item.RampIcon;
            default:
              goto label_28;
          }
        }
        return Item.WoodDoor;
label_11:
        return Item.SteelDoor;
      }
      if ((uint) block <= 158U)
      {
        if (block == Block.Cylinder)
          return Item.CylinderIcon;
        if (block == Block.Table)
          return Item.TableIcon;
        goto label_28;
      }
      else
      {
        switch (block)
        {
          case Block.Switch:
            return Item.SwitchIcon;
          case Block.Button:
            return Item.ButtonIcon;
          case Block.Stairs2:
            return Item.Stairs2Icon;
          case Block.HalfBlock2:
            return Item.HalfBlock2Icon;
          case Block.Ramp2:
            return Item.Ramp2Icon;
          case Block.LockedDoorBottom:
            break;
          default:
            goto label_28;
        }
      }
label_12:
      return Item.LockedDoor;
label_28:
      return itemID;
    }

    public static Item ConvertItemIDToBlockID(Item itemID)
    {
      if (itemID <= Item.zLastBlockID)
        return itemID;
      Item obj = itemID;
      if ((uint) obj <= 314U)
      {
        switch (obj)
        {
          case Item.WoodDoor:
            return Item.WoodDoorBottom;
          case Item.SteelDoor:
            return Item.SteelDoorBottom;
          case Item.SignIcon:
            return Item.Sign;
          case Item.Bed:
            return Item.BedFoot;
          case Item.LockedDoor:
            return Item.LockedDoorBottom;
        }
      }
      else
      {
        switch (obj)
        {
          case Item.FenceIcon:
            return Item.Fence;
          case Item.RopeIcon:
            return Item.Rope;
          case Item.StairsIcon:
            return Item.Stairs;
          case Item.HalfBlockIcon:
            return Item.HalfBlock;
          case Item.RampIcon:
            return Item.Ramp;
          case Item.CylinderIcon:
            return Item.Cylinder;
          case Item.TableIcon:
            return Item.Table;
          case Item.SwitchIcon:
            return Item.Switch;
          case Item.ButtonIcon:
            return Item.Button;
          case Item.Stairs2Icon:
            return Item.Stairs2;
          case Item.HalfBlock2Icon:
            return Item.HalfBlock2;
          case Item.Ramp2Icon:
            return Item.Ramp2;
        }
      }
      return itemID;
    }
  }
}
