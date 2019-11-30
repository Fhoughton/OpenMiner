// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemData2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner
{
  internal static class ItemData2
  {
    public static string ForDisplay(GameInstance instance, Item item)
    {
      return ItemData2.ForDisplay(instance, new InventoryItem(item, 1));
    }

    public static string ForDisplay(Player player, InventoryItem item)
    {
      string str = ItemData2.ForDisplay(player.GameInstance, item);
      if (item.ItemID == Item.Clipboard)
        str = str + ": " + ItemData2.GetComponentNameForDisplay(player.GetClipboardModel(item));
      return str;
    }

    private static string GetComponentNameForDisplay(MapModel model)
    {
      if (model == null)
        return "Unknown";
      if (model.ComName == null)
        return "Unsaved";
      int num = model.ComName.LastIndexOf('_');
      if (num < 0)
        return model.ComName;
      return model.ComName.Substring(num + 1);
    }

    public static string ForDisplay(GameInstance instance, InventoryItem item)
    {
      switch (item.ItemID)
      {
        case Item.None:
          return "";
        case Item.Blueprint:
          string str1 = "Blueprint: ";
          int durability = (int) item.Durability;
          if (durability >= 0 && durability < Blueprints.BlueprintList.Length)
            str1 += ItemData2.ForDisplay(instance, Blueprints.BlueprintList[durability].Result);
          return str1;
        case Item.Book:
          string str2 = "Book";
          if (instance != null)
          {
            BookData bookData = instance.GetBookData(item.Durability);
            if (bookData != null && bookData.Title != null && bookData.Title.Length > 0)
              str2 = bookData.Title.Replace('_', ' ');
          }
          return str2;
        default:
          return Globals1.ItemData[(int) item.ItemID].Name;
      }
    }

    public static ushort GetBurnTime(MapTM map, GlobalPoint3D p, Item itemID)
    {
      if (itemID < Item.zLastBlockID && itemID != Item.Obsidian && !ItemData.CanItemUseKey(itemID))
      {
        Block blockID = (Block) itemID;
        itemID = (Item) map.GetBlockTextureID(blockID, map.GetBlockTextureIndexFromExistingBlock(p, itemID));
      }
      return ItemData.GetBurnTime(itemID);
    }

    public static float GetStrikeBlockPower(Actor actor, Item itemID, Block blockID)
    {
      itemID = itemID != Item.None ? ItemData.ConvertItemIDToBlockID(itemID) : Item.Hand;
      ItemTypeDataXML itemTypeDataXml = Globals1.ItemTypeData[(int) itemID];
      if (itemTypeDataXml.Class == ItemTypeClass.SledgeHammer)
        return 1f;
      BlockDataXML blockDataXml = actor.GameInstance.Map.BlockData[(int) blockID];
      BlockMaterialDataXML blockMaterialDataXml = Globals1.BlockMaterialData[(int) blockDataXml.Material];
      ItemTypeClassDataXML typeClassData = Globals1.ItemTypeClassData[(int) itemTypeDataXml.Class];
      if (typeClassData.ItemClass == ItemTypeClass.None)
        typeClassData = ItemData2.SubsituteNoTypeClass(actor, itemID, ref blockMaterialDataXml);
      if ((int) blockMaterialDataXml.Resistance > (int) typeClassData.MaxResistance || typeClassData.Power <= (ushort) 0 || blockMaterialDataXml.Resistance <= (ushort) 0)
        return 0.0f;
      ushort strikeBlockEfficiency = ItemData2.GetStrikeBlockEfficiency(itemID, blockID, ref blockMaterialDataXml, ref typeClassData);
      if (strikeBlockEfficiency == (ushort) 0)
        return 0.0f;
      float num = (float) ((double) strikeBlockEfficiency / 100.0 * ((double) blockMaterialDataXml.BaseEfficiency / 100.0));
      return Math.Min((float) typeClassData.Power / (float) blockMaterialDataXml.Resistance * num, 1f);
    }

    private static ItemTypeClassDataXML SubsituteNoTypeClass(
      Actor actor,
      Item itemID,
      ref BlockMaterialDataXML blockMaterial)
    {
      if (itemID > Item.zLastBlockID)
        return Globals1.ItemTypeClassData[2];
      if (!actor.GameInstance.IsFiniteResources && actor.HasPermissionAny(Permissions.Creative | Permissions.Admin))
        return Globals1.ItemTypeClassData[12];
      BlockMaterialDataXML blockMaterialDataXml = Globals1.BlockMaterialData[(int) actor.GameInstance.Map.BlockData[(int) itemID].Material];
      ItemTypeClassDataXML typeClassDataXml = Globals1.ItemTypeClassData[0];
      typeClassDataXml.MaxResistance = Math.Min(blockMaterialDataXml.Resistance, (ushort) 2500);
      typeClassDataXml.Power = (ushort) 160;
      return typeClassDataXml;
    }

    public static bool IsRapidSwingItem(Actor character, Item itemID)
    {
      return ItemData.IsSubType(itemID, ItemSubType.RapidSwing) || Globals1.ItemTypeData[(int) itemID].Class == ItemTypeClass.None && itemID < Item.zLastBlockID && (!character.GameInstance.IsFiniteResources && character.HasPermissionAny(Permissions.Creative | Permissions.Admin));
    }

    public static float GetStrikeBlockPowerTestBase(Block blockID)
    {
      BlockDataXML blockDataXml = Globals1.BlockData[(int) blockID];
      BlockMaterialDataXML blockMaterialDataXml = Globals1.BlockMaterialData[(int) blockDataXml.Material];
      ItemTypeClassDataXML typeClassDataXml = Globals1.ItemTypeClassData[2];
      if (typeClassDataXml.Power <= (ushort) 0 || blockMaterialDataXml.Resistance <= (ushort) 0)
        return 0.0f;
      ItemDataXML itemDataXml = Globals1.ItemData[(int) blockID];
      return Math.Min((float) typeClassDataXml.Power / (float) blockMaterialDataXml.Resistance * ((float) blockMaterialDataXml.BaseEfficiency / 100f), 1f);
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
  }
}
