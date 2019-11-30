// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlockData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  internal static class BlockData
  {
    public static OreProperty[] RockPropertiesOld = new OreProperty[17]
    {
      new OreProperty()
      {
        BlockID = Block.Gold,
        DepositFrequency = 100,
        DepositSize = 20,
        MaxDepth = 0.5f,
        MinDepth = 0.035f
      },
      new OreProperty()
      {
        BlockID = Block.Scoria,
        DepositFrequency = 200,
        DepositSize = 40,
        MaxDepth = 0.4f,
        MinDepth = 0.0f
      },
      new OreProperty()
      {
        BlockID = Block.Iron,
        DepositFrequency = 300,
        DepositSize = 40,
        MaxDepth = 0.8f,
        MinDepth = 0.05f
      },
      new OreProperty()
      {
        BlockID = Block.Opal,
        DepositFrequency = 80,
        DepositSize = 30,
        MaxDepth = 0.6f,
        MinDepth = 0.04f
      },
      new OreProperty()
      {
        BlockID = Block.Flint,
        DepositFrequency = 90,
        DepositSize = 10,
        MaxDepth = 0.7f,
        MinDepth = 0.1f
      },
      new OreProperty()
      {
        BlockID = Block.Carbon,
        DepositFrequency = 400,
        DepositSize = 30,
        MaxDepth = 0.7f,
        MinDepth = 0.05f
      },
      new OreProperty()
      {
        BlockID = Block.Coal,
        DepositFrequency = 500,
        DepositSize = 20,
        MaxDepth = 0.5f,
        MinDepth = 0.03f
      },
      new OreProperty()
      {
        BlockID = Block.Sulphur,
        DepositFrequency = 200,
        DepositSize = 30,
        MaxDepth = 0.7f,
        MinDepth = 0.2f
      },
      new OreProperty()
      {
        BlockID = Block.Greenstone,
        DepositFrequency = 200,
        DepositSize = 20,
        MaxDepth = 0.7f,
        MinDepth = 0.3f
      },
      new OreProperty()
      {
        BlockID = Block.Diamond,
        DepositFrequency = 150,
        DepositSize = 10,
        MaxDepth = 1f,
        MinDepth = 0.3f
      },
      new OreProperty()
      {
        BlockID = Block.Cyclonite,
        DepositFrequency = 100,
        DepositSize = 20,
        MaxDepth = 0.8f,
        MinDepth = 0.4f
      },
      new OreProperty()
      {
        BlockID = Block.Titanium,
        DepositFrequency = 100,
        DepositSize = 30,
        MaxDepth = 0.9f,
        MinDepth = 0.6f
      },
      new OreProperty()
      {
        BlockID = Block.Obsidian,
        DepositFrequency = 25,
        DepositSize = 10,
        MaxDepth = 1f,
        MinDepth = 0.6f
      },
      new OreProperty()
      {
        BlockID = Block.Ruby,
        DepositFrequency = 100,
        DepositSize = 10,
        MaxDepth = 1f,
        MinDepth = 0.6f
      },
      new OreProperty()
      {
        BlockID = Block.Platinum,
        DepositFrequency = 50,
        DepositSize = 20,
        MaxDepth = 1f,
        MinDepth = 0.6f
      },
      new OreProperty()
      {
        BlockID = Block.Uranium,
        DepositFrequency = 15,
        DepositSize = 10,
        MaxDepth = 1f,
        MinDepth = 0.8f
      },
      new OreProperty()
      {
        BlockID = Block.Fluorite,
        DepositFrequency = 100,
        DepositSize = 10,
        MaxDepth = 0.6f,
        MinDepth = 0.04f
      }
    };

    public static bool IsGrassOrDirt(Block blockID)
    {
      Block block = blockID;
      if ((uint) block <= 79U)
      {
        switch (block)
        {
          case Block.Grass:
          case Block.Dirt:
          case Block.GrassyStone:
            break;
          default:
            goto label_4;
        }
      }
      else if (block != Block.GrassShaded && block != Block.TilledEarth)
        goto label_4;
      return true;
label_4:
      return false;
    }

    public static Block GetBurntBlock(Block blockID)
    {
      switch (blockID)
      {
        case Block.Grass:
        case Block.GrassyStone:
        case Block.GrassShaded:
          return Block.Dirt;
        default:
          return Block.None;
      }
    }

    public static bool IsTillable(Block blockID, Item toolID)
    {
      switch (blockID)
      {
        case Block.Grass:
        case Block.Dirt:
        case Block.GrassyStone:
        case Block.GrassShaded:
          return true;
        default:
          return false;
      }
    }

    public static void AdjustBlockDataForMove(ref MapBlock data)
    {
      if (data.BlockID != (byte) 50 || ((int) data.AuxData & 2) != 2)
        return;
      data.AuxData &= (byte) 253;
    }

    public static bool ShouldDrawPaneSection(MapTM map, Block blockID)
    {
      if (blockID == Block.None || map.BlockData[(int) blockID].IsIcon)
        return false;
      Block block = blockID;
      if ((uint) block <= 125U)
      {
        if ((uint) block <= 47U)
        {
          switch (block)
          {
            case Block.Water:
            case Block.Lava:
            case Block.Torch:
            case Block.Ladder:
              break;
            default:
              goto label_10;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Rope:
            case Block.Fire:
            case Block.ClimbingIvy:
            case Block.Book:
            case Block.Crop:
            case Block.InvisibleBarrier:
              break;
            default:
              goto label_10;
          }
        }
      }
      else if ((uint) block <= 145U)
      {
        switch (block)
        {
          case Block.Stack:
          case Block.UpsideDownStack:
          case Block.SnowLayer:
            break;
          default:
            goto label_10;
        }
      }
      else
      {
        switch (block)
        {
          case Block.Painting:
          case Block.PressurePlate:
          case Block.Switch:
          case Block.Button:
          case Block.TrapDoor:
          case Block.Stack2:
            break;
          default:
            goto label_10;
        }
      }
      return false;
label_10:
      return true;
    }
  }
}
