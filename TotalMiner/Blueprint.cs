// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blueprint
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using System;

namespace StudioForge.TotalMiner
{
  internal class Blueprint
  {
    private static InventoryItem[,] tempItemArray1 = new InventoryItem[3, 3];
    private static InventoryItem[,] tempItemArray2 = new InventoryItem[3, 3];
    public bool IsValid;
    public bool IsEnabled;
    public BlueprintCraftType CraftType;
    public InventoryItem Result;
    public short ID;
    public short SortID;
    public string Description;
    public Vector2 Depth;
    public bool IsOrigValid;
    public bool IsDefault;
    public bool IsUnearthed;
    public bool IsGenerated;
    public GlobalPoint3D Point;
    private InventoryItem[] items;
    private InventoryItem m11;
    private InventoryItem m12;
    private InventoryItem m13;
    private InventoryItem m21;
    private InventoryItem m22;
    private InventoryItem m23;
    private InventoryItem m31;
    private InventoryItem m32;
    private InventoryItem m33;

    public InventoryItem[] Items
    {
      get
      {
        InventoryItem[] items = this.items;
        if (items != null)
          return items;
        return this.items = new InventoryItem[9]
        {
          this.m11,
          this.m12,
          this.m13,
          this.m21,
          this.m22,
          this.m23,
          this.m31,
          this.m32,
          this.m33
        };
      }
    }

    public void SetItem(int i, InventoryItem item)
    {
      if (i < 0 || i >= 9)
        return;
      if (this.items != null)
        this.items[i] = item;
      switch (i)
      {
        case 0:
          this.m11 = item;
          break;
        case 1:
          this.m12 = item;
          break;
        case 2:
          this.m13 = item;
          break;
        case 3:
          this.m21 = item;
          break;
        case 4:
          this.m22 = item;
          break;
        case 5:
          this.m23 = item;
          break;
        case 6:
          this.m31 = item;
          break;
        case 7:
          this.m32 = item;
          break;
        case 8:
          this.m33 = item;
          break;
      }
    }

    public int GetItemSlotCount(Item item, int slotID)
    {
      return this.Items[slotID].Count;
    }

    public int GetItemSlotsCount(Item item)
    {
      int num = 0;
      foreach (InventoryItem inventoryItem in this.Items)
      {
        if (inventoryItem.ItemID == item)
          ++num;
      }
      return num;
    }

    public int GetItemTotalCount(Item item)
    {
      int num = 0;
      foreach (InventoryItem inventoryItem in this.Items)
      {
        if (inventoryItem.ItemID == item)
          num += inventoryItem.Count;
      }
      return num;
    }

    public int GetCostOfComponents(bool ignoreItemDataPrice)
    {
      int num1 = 0;
      for (int index = 0; index < this.Items.Length; ++index)
      {
        Item itemId = this.Items[index].ItemID;
        int val1 = ItemData.GetMinCustBuyPrice(itemId);
        if (val1 >= 0)
        {
          int costOfComponents = Blueprints.GetCostOfComponents(itemId, ignoreItemDataPrice);
          switch (itemId)
          {
            case Item.BucketOfWater:
            case Item.BucketOfLava:
            case Item.BucketOfMilk:
              val1 -= Blueprints.GetCostOfComponents(Item.Bucket, ignoreItemDataPrice);
              break;
            case Item.BottleOfWater:
            case Item.BottleOfMilk:
              val1 -= Blueprints.GetCostOfComponents(Item.Bottle, ignoreItemDataPrice);
              break;
          }
          if (ignoreItemDataPrice && costOfComponents > 0)
            val1 = 0;
          float num2 = 1f;
          ushort durability = this.Items[index].Durability;
          if (durability > (ushort) 0)
            num2 = (float) ((int) ItemData.GetItemDurability(itemId) / (int) durability);
          num1 += (int) ((double) (Math.Max(val1, costOfComponents) * this.Items[index].Count) / (double) num2);
        }
      }
      return (int) Math.Round((double) num1 / (double) this.Result.Count);
    }

    public int GetComponentCraftDepth()
    {
      int val1 = 0;
      for (int index = 0; index < this.Items.Length; ++index)
        val1 = Math.Max(val1, this.GetComponentCraftDepthCore(this.Items[index].ItemID, 0));
      return val1;
    }

    private int GetComponentCraftDepthCore(Item itemID, int depth)
    {
      if (itemID != Item.None)
      {
        ++depth;
        Blueprint blueprint = Blueprints.GetBlueprint(itemID);
        if (blueprint != null)
          depth += blueprint.GetComponentCraftDepth();
      }
      return depth;
    }

    public bool IsMatch(
      BlueprintCraftType type,
      InventoryItem r11,
      InventoryItem r12,
      InventoryItem r13,
      InventoryItem r21,
      InventoryItem r22,
      InventoryItem r23,
      InventoryItem r31,
      InventoryItem r32,
      InventoryItem r33)
    {
      if (this.CraftType != type)
        return false;
      Blueprint.GetPattern(this, Blueprint.tempItemArray1);
      Blueprint.GetPattern(r11, r12, r13, r21, r22, r23, r31, r32, r33, Blueprint.tempItemArray2);
      return Blueprint.ComparePatterns(Blueprint.tempItemArray1, Blueprint.tempItemArray2);
    }

    public bool IsMatch(Blueprint bp)
    {
      if (this.CraftType != bp.CraftType)
        return false;
      Blueprint.GetPattern(this, Blueprint.tempItemArray1);
      Blueprint.GetPattern(bp, Blueprint.tempItemArray2);
      return Blueprint.ComparePatterns(Blueprint.tempItemArray1, Blueprint.tempItemArray2);
    }

    public Blueprint(BlueprintXML data, short ID)
      : this(data.CraftType, data.IsValid, (byte) 0, data.Depth, ID, data.SortID, data.Result.ItemID, data.Result.Count, new InventoryItem(data.Material11), new InventoryItem(data.Material12), new InventoryItem(data.Material13), new InventoryItem(data.Material21), new InventoryItem(data.Material22), new InventoryItem(data.Material23), new InventoryItem(data.Material31), new InventoryItem(data.Material32), new InventoryItem(data.Material33))
    {
      this.IsDefault = data.IsDefault;
    }

    public Blueprint(
      BlueprintCraftType craftType,
      bool isValid,
      byte level,
      Vector2 depth,
      short id,
      short sortID,
      Item result,
      int count,
      InventoryItem r11,
      InventoryItem r12,
      InventoryItem r13,
      InventoryItem r21,
      InventoryItem r22,
      InventoryItem r23,
      InventoryItem r31,
      InventoryItem r32,
      InventoryItem r33)
    {
      this.CraftType = craftType;
      this.IsValid = isValid;
      this.IsOrigValid = isValid;
      this.Depth = depth;
      this.ID = id;
      this.SortID = sortID;
      this.Result = new InventoryItem(result, count);
      this.m11 = r11;
      this.m12 = r12;
      this.m13 = r13;
      this.m21 = r21;
      this.m22 = r22;
      this.m23 = r23;
      this.m31 = r31;
      this.m32 = r32;
      this.m33 = r33;
      this.BuildDescription();
    }

    public BlueprintXML ConvertToXML()
    {
      return new BlueprintXML()
      {
        ItemID = this.Result.ItemID,
        IsValid = this.IsValid,
        CraftType = this.CraftType,
        IsDefault = this.IsDefault,
        Depth = this.Depth,
        SortID = this.SortID,
        Result = new InventoryItemNDXML(this.Result),
        Material11 = new InventoryItemXML(this.m11),
        Material12 = new InventoryItemXML(this.m12),
        Material13 = new InventoryItemXML(this.m13),
        Material21 = new InventoryItemXML(this.m21),
        Material22 = new InventoryItemXML(this.m22),
        Material23 = new InventoryItemXML(this.m23),
        Material31 = new InventoryItemXML(this.m31),
        Material32 = new InventoryItemXML(this.m32),
        Material33 = new InventoryItemXML(this.m33)
      };
    }

    private static InventoryItem DefaultItem(Item itemID)
    {
      return new InventoryItem(itemID, itemID == Item.None ? 0 : 1);
    }

    public void BuildDescription()
    {
      string sb = this.CraftType != BlueprintCraftType.Crafting ? (!this.IsFood(this.Result.ItemID) ? this.AppendCountAndItemName("Smelt", this.Result.Count, this.Result.ItemID) + " from" : this.AppendCountAndItemName("Cook", this.Result.Count, this.Result.ItemID) + " using") : this.AppendCountAndItemName("Craft", this.Result.Count, this.Result.ItemID) + " from";
      InventoryItem[] items = this.Items;
      int length = sb.Length;
      bool flag1 = true;
      for (int i = 0; i < items.Length; ++i)
      {
        InventoryItem inventoryItem = items[i];
        if (inventoryItem.ItemID != Item.None && !this.IsAlreadyCounted(items, i))
        {
          bool flag2 = this.IsLastItem(items, i);
          int itemTotalCount = this.GetItemTotalCount(inventoryItem.ItemID);
          if (!flag1 && flag2)
            sb += " and";
          sb = this.AppendCountAndItemName(sb, itemTotalCount, inventoryItem.ItemID);
          if (!flag2)
            sb += (string) (object) ',';
          flag1 = false;
        }
      }
      this.Description = sb + (object) '.';
    }

    private string AppendCountAndItemName(string sb, int count, Item itemID)
    {
      sb += " ";
      string str = ItemData.ToString(itemID);
      if (this.IsFood(itemID) && this.CraftType == BlueprintCraftType.Furnace && str.StartsWith("Cooked "))
        str = str.Substring(7);
      if (count > 1)
        sb = sb + (object) count + " ";
      else if (Globals1.ItemData[(int) itemID].Plural != PluralType.None)
        sb += this.IsVowel(str[0]) ? "an " : "a ";
      sb += str;
      return this.AppendPlural(sb, itemID, count);
    }

    private string AppendPlural(string sb, Item itemID, int count)
    {
      if (count > 1)
      {
        switch (Globals1.ItemData[(int) itemID].Plural)
        {
          case PluralType.S:
            sb += "s";
            break;
          case PluralType.ES:
            sb += "es";
            break;
        }
      }
      return sb;
    }

    private bool IsFood(Item itemID)
    {
      return Globals1.ItemTypeData[(int) itemID].Type == ItemType.Food;
    }

    private bool IsLastItem(InventoryItem[] items, int i)
    {
      Item itemId = items[i].ItemID;
      for (int i1 = i + 1; i1 < items.Length; ++i1)
      {
        if (items[i1].ItemID != Item.None && items[i1].ItemID != itemId && !this.IsAlreadyCounted(items, i1))
          return false;
      }
      return true;
    }

    private bool IsAlreadyCounted(InventoryItem[] items, int i)
    {
      Item itemId = items[i].ItemID;
      for (int index = 0; index < i; ++index)
      {
        if (items[index].ItemID == itemId)
          return true;
      }
      return false;
    }

    private bool IsVowel(char c)
    {
      switch (char.ToLower(c))
      {
        case 'a':
        case 'e':
        case 'i':
        case 'o':
        case 'u':
          return true;
        default:
          return false;
      }
    }

    private static void GetPattern(Blueprint bp, InventoryItem[,] itemArray)
    {
      Blueprint.GetPattern(bp.m11, bp.m12, bp.m13, bp.m21, bp.m22, bp.m23, bp.m31, bp.m32, bp.m33, itemArray);
    }

    private static void GetPattern(
      InventoryItem r11,
      InventoryItem r12,
      InventoryItem r13,
      InventoryItem r21,
      InventoryItem r22,
      InventoryItem r23,
      InventoryItem r31,
      InventoryItem r32,
      InventoryItem r33,
      InventoryItem[,] itemArray)
    {
      Blueprint.BuildItemArray(r11, r12, r13, r21, r22, r23, r31, r32, r33, itemArray);
    }

    private static bool ComparePatterns(InventoryItem[,] p1, InventoryItem[,] p2)
    {
      if (p1 == null || p2 == null || (p1.GetLength(0) != p2.GetLength(0) || p1.GetLength(1) != p2.GetLength(1)))
        return false;
      return Blueprint.IsMatch(p1, p2);
    }

    private static bool IsMatch(InventoryItem[,] p1, InventoryItem[,] p2)
    {
      int length1 = p1.GetLength(0);
      int length2 = p1.GetLength(1);
      for (int index1 = 0; index1 < length1; ++index1)
      {
        for (int index2 = 0; index2 < length2; ++index2)
        {
          if (p1[index1, index2].ItemID != p2[index1, index2].ItemID || p2[index1, index2].Count < p1[index1, index2].Count)
            return false;
        }
      }
      return true;
    }

    private static void BuildItemArray(
      InventoryItem r11,
      InventoryItem r12,
      InventoryItem r13,
      InventoryItem[,] result)
    {
      Blueprint.BuildItemArray(r11, r12, r13, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, result);
    }

    private static void BuildItemArray(
      InventoryItem r11,
      InventoryItem r12,
      InventoryItem r13,
      InventoryItem r21,
      InventoryItem r22,
      InventoryItem r23,
      InventoryItem r31,
      InventoryItem r32,
      InventoryItem r33,
      InventoryItem[,] result)
    {
      if (r11.ItemID == Item.None && r21.ItemID == Item.None && r31.ItemID == Item.None)
      {
        if (r12.ItemID == Item.None && r22.ItemID == Item.None && r32.ItemID == Item.None)
        {
          result[0, 0] = r13;
          result[0, 1] = r23;
          result[0, 2] = r33;
          result[1, 0] = InventoryItem.Empty;
          result[1, 1] = InventoryItem.Empty;
          result[1, 2] = InventoryItem.Empty;
          result[2, 0] = InventoryItem.Empty;
          result[2, 1] = InventoryItem.Empty;
          result[2, 2] = InventoryItem.Empty;
        }
        else
        {
          result[0, 0] = r12;
          result[1, 0] = r13;
          result[2, 0] = InventoryItem.Empty;
          result[0, 1] = r22;
          result[1, 1] = r23;
          result[2, 1] = InventoryItem.Empty;
          result[0, 2] = r32;
          result[1, 2] = r33;
          result[2, 2] = InventoryItem.Empty;
        }
      }
      else
      {
        result[0, 0] = r11;
        result[1, 0] = r12;
        result[2, 0] = r13;
        result[0, 1] = r21;
        result[1, 1] = r22;
        result[2, 1] = r23;
        result[0, 2] = r31;
        result[1, 2] = r32;
        result[2, 2] = r33;
      }
    }

    public void ReduceCraftItems(Inventory inventory, int inventorySlotIDOffset)
    {
      Inventory inventory1 = inventory;
      int index1 = inventorySlotIDOffset;
      InventoryItem[,] tempItemArray1 = Blueprint.tempItemArray1;
      InventoryItem[,] tempItemArray2 = Blueprint.tempItemArray2;
      Blueprint.BuildItemArray(inventory1[index1], inventory1[index1 + 1], inventory1[index1 + 2], inventory1[index1 + 3], inventory1[index1 + 4], inventory1[index1 + 5], inventory1[index1 + 6], inventory1[index1 + 7], inventory1[index1 + 8], tempItemArray1);
      Blueprint.BuildItemArray(this.m11, this.m12, this.m13, this.m21, this.m22, this.m23, this.m31, this.m32, this.m33, tempItemArray2);
      for (int index2 = 0; index2 < 3; ++index2)
      {
        for (int index3 = 0; index3 < 3; ++index3)
        {
          InventoryItem inventoryItem1 = tempItemArray1[index3, index2];
          InventoryItem inventoryItem2 = tempItemArray2[index3, index2];
          if (inventoryItem1.Count > 0)
          {
            int count = 0;
            if (inventoryItem2.Durability > (ushort) 0)
            {
              inventoryItem1.Durability -= Math.Min(inventoryItem1.Durability, inventoryItem2.Durability);
              if (inventoryItem1.Durability == (ushort) 0)
                count = 1;
            }
            else
              count = inventoryItem2.Count;
            if (count > 0)
            {
              switch (inventoryItem1.ItemID)
              {
                case Item.BucketOfWater:
                case Item.BucketOfLava:
                case Item.BucketOfMilk:
                  inventory.AddToInventory(Item.Bucket, count);
                  break;
                case Item.BottleOfWater:
                case Item.BottleOfMilk:
                  inventory.AddToInventory(Item.Bottle, count);
                  break;
              }
              inventoryItem1.Count -= count;
            }
            tempItemArray1[index3, index2] = inventoryItem1;
          }
        }
      }
      if (inventory1[index1].ItemID == Item.None && inventory1[index1 + 3].ItemID == Item.None && inventory1[index1 + 6].ItemID == Item.None)
      {
        if (inventory1[index1 + 1].ItemID == Item.None && inventory1[index1 + 4].ItemID == Item.None && inventory1[index1 + 7].ItemID == Item.None)
        {
          inventory1[index1 + 2] = tempItemArray1[0, 0];
          inventory1[index1 + 5] = tempItemArray1[0, 1];
          inventory1[index1 + 8] = tempItemArray1[0, 2];
        }
        else
        {
          inventory1[index1 + 1] = tempItemArray1[0, 0];
          inventory1[index1 + 2] = tempItemArray1[1, 0];
          inventory1[index1 + 4] = tempItemArray1[0, 1];
          inventory1[index1 + 5] = tempItemArray1[1, 1];
          inventory1[index1 + 7] = tempItemArray1[0, 2];
          inventory1[index1 + 8] = tempItemArray1[1, 2];
        }
      }
      else
      {
        inventory1[index1] = tempItemArray1[0, 0];
        inventory1[index1 + 1] = tempItemArray1[1, 0];
        inventory1[index1 + 2] = tempItemArray1[2, 0];
        inventory1[index1 + 3] = tempItemArray1[0, 1];
        inventory1[index1 + 4] = tempItemArray1[1, 1];
        inventory1[index1 + 5] = tempItemArray1[2, 1];
        inventory1[index1 + 6] = tempItemArray1[0, 2];
        inventory1[index1 + 7] = tempItemArray1[1, 2];
        inventory1[index1 + 8] = tempItemArray1[2, 2];
      }
    }

    public void ReduceSmeltItems(Inventory inventory)
    {
      Inventory inventory1 = inventory;
      InventoryItem[,] tempItemArray1 = Blueprint.tempItemArray1;
      InventoryItem[,] tempItemArray2 = Blueprint.tempItemArray2;
      int index1 = 1;
      Blueprint.BuildItemArray(inventory1[index1], inventory1[1 + index1], inventory1[2 + index1], tempItemArray1);
      Blueprint.BuildItemArray(this.m11, this.m12, this.m13, tempItemArray2);
      for (int index2 = 0; index2 < 3; ++index2)
      {
        InventoryItem inventoryItem1 = tempItemArray1[index2, 0];
        InventoryItem inventoryItem2 = tempItemArray2[index2, 0];
        if (inventoryItem1.Count > 0)
        {
          int count = 0;
          if (inventoryItem2.Durability > (ushort) 0)
          {
            inventoryItem1.Durability -= Math.Min(inventoryItem1.Durability, inventoryItem2.Durability);
            if (inventoryItem1.Durability == (ushort) 0)
              count = 1;
          }
          else
            count = inventoryItem2.Count;
          if (count > 0)
          {
            switch (inventoryItem1.ItemID)
            {
              case Item.BucketOfWater:
              case Item.BucketOfLava:
              case Item.BucketOfMilk:
                inventory.AddToInventory(Item.Bucket, count);
                break;
              case Item.BottleOfWater:
              case Item.BottleOfMilk:
                inventory.AddToInventory(Item.Bottle, count);
                break;
            }
            inventoryItem1.Count -= count;
          }
          tempItemArray1[index2, 0] = inventoryItem1;
        }
      }
      if (inventory1[index1].ItemID == Item.None)
      {
        if (inventory1[1 + index1].ItemID == Item.None)
        {
          inventory1[2 + index1] = tempItemArray1[0, 0];
        }
        else
        {
          inventory1[1 + index1] = tempItemArray1[0, 0];
          inventory1[2 + index1] = tempItemArray1[1, 0];
        }
      }
      else
      {
        inventory1[index1] = tempItemArray1[0, 0];
        inventory1[1 + index1] = tempItemArray1[1, 0];
        inventory1[2 + index1] = tempItemArray1[2, 0];
      }
    }
  }
}
