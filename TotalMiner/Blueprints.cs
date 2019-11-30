// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blueprints
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal static class Blueprints
  {
    public static Blueprint[] BlueprintList;
    private static Blueprint grenadeLauncher;

    public static void InitializeBlueprints(GameInstance instance)
    {
      bool flag1 = instance != null && instance.IsHost;
      Blueprints.grenadeLauncher = (Blueprint) null;
      List<Blueprint> blueprintList = new List<Blueprint>(Globals1.BlueprintData.Length);
      for (int index = 0; index < Globals1.BlueprintData.Length; ++index)
      {
        Item itemId = Globals1.BlueprintData[index].ItemID;
        if (Globals1.ItemData[(int) itemId].IsValid)
          blueprintList.Add(new Blueprint(Globals1.BlueprintData[index], (short) index));
      }
      Blueprints.BlueprintList = blueprintList.ToArray();
      if (flag1)
        instance.BlueprintsToPlace = new List<Blueprint>(Blueprints.BlueprintList.Length);
      bool flag2 = instance == null || !instance.IsDigDeepMode;
      foreach (Blueprint blueprint1 in Blueprints.BlueprintList)
      {
        if (blueprint1.IsValid)
        {
          Blueprint blueprint2 = blueprint1;
          Blueprint blueprint3 = blueprint1;
          bool flag3;
          blueprint1.IsGenerated = flag3 = flag2 || blueprint1.IsDefault;
          int num1;
          bool flag4 = (num1 = flag3 ? 1 : 0) != 0;
          blueprint3.IsEnabled = num1 != 0;
          int num2;
          bool flag5 = (num2 = flag4 ? 1 : 0) != 0;
          blueprint2.IsUnearthed = num2 != 0;
          if (!flag5 && flag1 && (blueprint1 != Blueprints.GrenadeLauncher && blueprint1.Result.ItemID_Raw != Item.None))
            instance.BlueprintsToPlace.Add(blueprint1);
        }
      }
      if (!flag1)
        return;
      instance.BlueprintsToPlace.Sort(new Comparison<Blueprint>(Blueprints.SortBlueprintsToPlace));
    }

    private static int SortBlueprintsToPlace(Blueprint a, Blueprint b)
    {
      return a.Depth.X.CompareTo(b.Depth.X);
    }

    public static Blueprint GrenadeLauncher
    {
      get
      {
        if (Blueprints.grenadeLauncher == null)
        {
          foreach (Blueprint blueprint in Blueprints.BlueprintList)
          {
            if (blueprint.Result.ItemID == Item.GrenadeLauncher)
            {
              Blueprints.grenadeLauncher = blueprint;
              break;
            }
          }
        }
        return Blueprints.grenadeLauncher;
      }
    }

    public static int EnabledCount
    {
      get
      {
        int num = 0;
        foreach (Blueprint blueprint in Blueprints.BlueprintList)
        {
          if (blueprint.IsEnabled)
            ++num;
        }
        return num;
      }
    }

    public static Blueprint GetSmeltResult(
      Player player,
      InventoryItem r11,
      InventoryItem r12,
      InventoryItem r13)
    {
      return Blueprints.GetResult(BlueprintCraftType.Furnace, player, r11, r12, r13, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty, InventoryItem.Empty);
    }

    public static Blueprint GetResult(
      BlueprintCraftType type,
      Player player,
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
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (blueprint.IsEnabled && ItemData.IsEnabled(blueprint.Result.ItemID) && blueprint.IsMatch(type, r11, r12, r13, r21, r22, r23, r31, r32, r33) && (blueprint.IsEnabled || blueprint.IsUnearthed || player != null && player.IsGod))
          return blueprint;
      }
      return (Blueprint) null;
    }

    public static int GetCraftCount(Item itemID)
    {
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (blueprint.Result.ItemID == itemID)
          return blueprint.Result.Count;
      }
      return 0;
    }

    public static byte GetBlueprintIndex(Item itemID)
    {
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        if (Blueprints.BlueprintList[index].Result.ItemID == itemID)
          return (byte) index;
      }
      return byte.MaxValue;
    }

    public static short GetBlueprintIndex(short ID)
    {
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        if ((int) Blueprints.BlueprintList[index].ID == (int) ID)
          return (short) index;
      }
      return -1;
    }

    public static byte GetBlueprintIndex(GlobalPoint3D p)
    {
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        if (Blueprints.BlueprintList[index].Point == p)
          return (byte) index;
      }
      return byte.MaxValue;
    }

    public static Blueprint GetBlueprint(Item itemID)
    {
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        if (Blueprints.BlueprintList[index].Result.ItemID == itemID)
          return Blueprints.BlueprintList[index];
      }
      return (Blueprint) null;
    }

    public static Blueprint GetBlueprint(int id)
    {
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        if ((int) Blueprints.BlueprintList[index].ID == id)
          return Blueprints.BlueprintList[index];
      }
      return (Blueprint) null;
    }

    public static Blueprint GetBlueprint(Map map, GlobalPoint3D p)
    {
      return Blueprints.BlueprintList[(int) MapTM.GetIDFromAux(map.GetAuxFullData(p))];
    }

    public static int GetCostOfComponents(Item itemID, bool ignoreItemDataPrice)
    {
      Blueprint blueprint = Blueprints.GetBlueprint(itemID);
      if (blueprint == null)
        return 0;
      return blueprint.GetCostOfComponents(ignoreItemDataPrice);
    }

    public static int GetMaxComponentDepth()
    {
      int num = 0;
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        int componentCraftDepth = Blueprints.BlueprintList[index].GetComponentCraftDepth();
        if (componentCraftDepth > num)
          num = componentCraftDepth;
      }
      return num;
    }
  }
}
