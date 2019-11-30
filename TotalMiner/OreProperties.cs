// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.OreProperties
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public static class OreProperties
  {
    private static OreProperty[] ores;
    private static List<OreProperty> oreList;

    public static OreProperty[] Ores
    {
      get
      {
        if (OreProperties.ores == null)
        {
          OreProperties.ores = OreProperties.oreList.ToArray();
          OreProperties.oreList = (List<OreProperty>) null;
        }
        return OreProperties.ores;
      }
    }

    public static void AddOre(OreProperty ore)
    {
      if (OreProperties.oreList == null || ore.DepositFrequency <= 0 || (ore.DepositSize <= 0 || (double) ore.MaxDepth <= (double) ore.MinDepth))
        return;
      OreProperties.oreList.Add(ore);
    }

    public static void RemoveOre(Block blockID)
    {
      if (OreProperties.oreList == null)
        return;
      for (int index = OreProperties.oreList.Count - 1; index >= 0; --index)
      {
        if (OreProperties.oreList[index].BlockID == blockID)
          OreProperties.oreList.RemoveAt(index);
      }
    }

    public static void Initialize(BiomeType biome)
    {
      OreProperties.ores = (OreProperty[]) null;
      OreProperties.oreList = new List<OreProperty>((IEnumerable<OreProperty>) new OreProperty[21]
      {
        new OreProperty()
        {
          BlockID = Block.Scoria,
          DepositFrequency = 200,
          DepositSize = 30,
          MaxDepth = 0.6f,
          MinDepth = 0.0f
        },
        new OreProperty()
        {
          BlockID = Block.SaltBlock,
          DepositFrequency = 100,
          DepositSize = 30,
          MaxDepth = 0.5f,
          MinDepth = 0.0f
        },
        new OreProperty()
        {
          BlockID = Block.Coal,
          DepositFrequency = 500,
          DepositSize = 22,
          MaxDepth = 0.5f,
          MinDepth = 1f / 1000f
        },
        new OreProperty()
        {
          BlockID = Block.Gold,
          DepositFrequency = 350,
          DepositSize = 20,
          MaxDepth = 0.5f,
          MinDepth = 1f / 1000f
        },
        new OreProperty()
        {
          BlockID = Block.Opal,
          DepositFrequency = 230,
          DepositSize = 30,
          MaxDepth = 0.6f,
          MinDepth = 0.01f
        },
        new OreProperty()
        {
          BlockID = Block.Fluorite,
          DepositFrequency = 170,
          DepositSize = 20,
          MaxDepth = 0.6f,
          MinDepth = 0.02f
        },
        new OreProperty()
        {
          BlockID = Block.Cassiterite,
          DepositFrequency = 70,
          DepositSize = 12,
          MaxDepth = 0.2f,
          MinDepth = 0.0f
        },
        new OreProperty()
        {
          BlockID = Block.Copper,
          DepositFrequency = 70,
          DepositSize = 12,
          MaxDepth = 0.2f,
          MinDepth = 0.0f
        },
        new OreProperty()
        {
          BlockID = Block.Iron,
          DepositFrequency = 550,
          DepositSize = 25,
          MaxDepth = 0.5f,
          MinDepth = 1f / 1000f
        },
        new OreProperty()
        {
          BlockID = Block.Carbon,
          DepositFrequency = 450,
          DepositSize = 18,
          MaxDepth = 0.6f,
          MinDepth = 0.02f
        },
        new OreProperty()
        {
          BlockID = Block.Flint,
          DepositFrequency = 300,
          DepositSize = 15,
          MaxDepth = 0.7f,
          MinDepth = 1f / 1000f
        },
        new OreProperty()
        {
          BlockID = Block.Sulphur,
          DepositFrequency = 250,
          DepositSize = 30,
          MaxDepth = 0.7f,
          MinDepth = 0.1f
        },
        new OreProperty()
        {
          BlockID = Block.Greenstone,
          DepositFrequency = 250,
          DepositSize = 15,
          MaxDepth = 0.7f,
          MinDepth = 0.2f
        },
        new OreProperty()
        {
          BlockID = Block.Diamond,
          DepositFrequency = 210,
          DepositSize = 10,
          MaxDepth = 1f,
          MinDepth = 0.3f
        },
        new OreProperty()
        {
          BlockID = Block.Cyclonite,
          DepositFrequency = 200,
          DepositSize = 20,
          MaxDepth = 0.8f,
          MinDepth = 0.5f
        },
        new OreProperty()
        {
          BlockID = Block.Sapphire,
          DepositFrequency = 180,
          DepositSize = 12,
          MaxDepth = 0.9f,
          MinDepth = 0.5f
        },
        new OreProperty()
        {
          BlockID = Block.Titanium,
          DepositFrequency = 150,
          DepositSize = 15,
          MaxDepth = 0.9f,
          MinDepth = 0.6f
        },
        new OreProperty()
        {
          BlockID = Block.Obsidian,
          DepositFrequency = 115,
          DepositSize = 10,
          MaxDepth = 1f,
          MinDepth = 0.6f
        },
        new OreProperty()
        {
          BlockID = Block.Ruby,
          DepositFrequency = 170,
          DepositSize = 10,
          MaxDepth = 1f,
          MinDepth = 0.6f
        },
        new OreProperty()
        {
          BlockID = Block.Platinum,
          DepositFrequency = 170,
          DepositSize = 20,
          MaxDepth = 1f,
          MinDepth = 0.6f
        },
        new OreProperty()
        {
          BlockID = Block.Uranium,
          DepositFrequency = 105,
          DepositSize = 10,
          MaxDepth = 1f,
          MinDepth = 0.8f
        }
      });
      switch (biome)
      {
        case BiomeType.Desert:
          OreProperties.oreList.Add(new OreProperty()
          {
            BlockID = Block.SandBrick,
            DepositFrequency = 150,
            DepositSize = 40,
            MaxDepth = 0.9f,
            MinDepth = 0.0f
          });
          OreProperties.oreList.Add(new OreProperty()
          {
            BlockID = Block.Dirt,
            DepositFrequency = 80,
            DepositSize = 30,
            MaxDepth = 0.4f,
            MinDepth = 0.0f
          });
          OreProperties.oreList.Add(new OreProperty()
          {
            BlockID = Block.Basalt,
            DepositFrequency = 70,
            DepositSize = 20,
            MaxDepth = 0.4f,
            MinDepth = 0.0f
          });
          break;
        case BiomeType.Grasslands:
          OreProperties.oreList.Add(new OreProperty()
          {
            BlockID = Block.Sand,
            DepositFrequency = 70,
            DepositSize = 30,
            MaxDepth = 0.4f,
            MinDepth = 0.0f
          });
          OreProperties.oreList.Add(new OreProperty()
          {
            BlockID = Block.Basalt,
            DepositFrequency = 70,
            DepositSize = 20,
            MaxDepth = 0.4f,
            MinDepth = 0.0f
          });
          break;
      }
    }
  }
}
