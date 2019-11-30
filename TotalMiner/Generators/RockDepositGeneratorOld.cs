// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.RockDepositGeneratorOld
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner.Generators
{
  internal static class RockDepositGeneratorOld
  {
    public static void CreateSpecialRockDeposits(MapOld map, IProgressBar progressBar)
    {
      foreach (OreProperty rock in BlockData.RockPropertiesOld)
      {
        progressBar.AddProgress(1f / (float) BlockData.RockPropertiesOld.Length);
        RockDepositGeneratorOld.CreateSpecialRockDeposits(map, rock, map.IsLightSource((byte) rock.BlockID));
      }
    }

    private static void CreateSpecialRockDeposits(MapOld map, OreProperty rock, bool needLightCalc)
    {
      for (int index = 0; index < rock.DepositFrequency * 10; ++index)
        RockDepositGeneratorOld.CreateSpecialRockDeposit(map, rock, needLightCalc);
    }

    private static void CreateSpecialRockDeposit(MapOld map, OreProperty rock, bool needLightCalc)
    {
      int x = map.Random.Next(map.MapSize.X - 4) + 2;
      int z = map.Random.Next(map.MapSize.Z - 4) + 2;
      int num1 = (int) ((double) rock.MaxDepth * (double) map.MapSize.Y - 1.0);
      int num2 = (int) ((double) rock.MinDepth * (double) map.MapSize.Y);
      int num3 = map.Random.Next(num1 - num2) + num2;
      RockDepositGeneratorOld.CreateSpecialRockDeposit(map, rock, new Point3D(x, -num3, z), needLightCalc);
    }

    private static void CreateSpecialRockDeposit(
      MapOld map,
      OreProperty rock,
      Point3D p,
      bool needLightCalc)
    {
      int num = (int) (map.Random.NextDouble() * (double) rock.DepositSize * 0.200000002980232 + (double) rock.DepositSize * 0.800000011920929);
      ushort blockData = map.BuildBlockData((byte) rock.BlockID, (byte) 0, (byte) 0);
      for (int index = 0; index < num; ++index)
      {
        p.X += map.Random.Next(3) - 1;
        p.Y += map.Random.Next(3) - 1;
        p.Z += map.Random.Next(3) - 1;
        p = map.Clamp(p, 2);
        if (p.Y >= map.WaterLevel - 10)
        {
          int height = map.GetHeight(p);
          if (p.Y > height)
            p.Y = height;
        }
        Block blockId = (Block) map.GetBlockID(p);
        if (blockId > Block.None && blockId < Block.Bedrock)
        {
          if (needLightCalc)
            map.SetBlockData(p, (byte) rock.BlockID, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
          else
            map.SetBlockDataInternal(p, blockData);
        }
      }
    }
  }
}
