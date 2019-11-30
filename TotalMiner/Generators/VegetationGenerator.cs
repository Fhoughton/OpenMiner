// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.VegetationGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Generators
{
  internal static class VegetationGenerator
  {
    private static VegetationGenerator.FloraModelSetup[] TreeModels = new VegetationGenerator.FloraModelSetup[17]
    {
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Fir"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Birch"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Oak Small"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Oak Med"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Oak Bent"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Oak Big"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Willow"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Pine"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Pine Big"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Small"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Small2"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Med"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Med2"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Big"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Big2"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComPack = "System",
        ComName = "Trees_Original_Maple Large"
      }
    };
    private static VegetationGenerator.FloraModelSetup[] CactusModels = new VegetationGenerator.FloraModelSetup[10]
    {
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus1"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus2"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus3"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus4"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus5"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus6"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus7"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus8"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus9"
      },
      new VegetationGenerator.FloraModelSetup()
      {
        ComName = "Flora_Cactus10"
      }
    };

    public static void ClearStaticData()
    {
      for (int index = 0; index < VegetationGenerator.TreeModels.Length; ++index)
      {
        if (VegetationGenerator.TreeModels[index].Model != null)
        {
          VegetationGenerator.TreeModels[index].Model.UnloadContent();
          VegetationGenerator.TreeModels[index].Model = (MapModel) null;
        }
      }
      for (int index = 0; index < VegetationGenerator.CactusModels.Length; ++index)
      {
        if (VegetationGenerator.CactusModels[index].Model != null)
        {
          VegetationGenerator.CactusModels[index].Model.UnloadContent();
          VegetationGenerator.CactusModels[index].Model = (MapModel) null;
        }
      }
    }

    public static ModelPlacement AddTree(
      GameInstance instance,
      Map map,
      GlobalPoint3D p,
      PcgRandom random,
      UpdateBlockMethod method,
      bool transmit)
    {
      return VegetationGenerator.AddTree(instance, map, VegetationGenerator.TreeModels, p, random, method, transmit);
    }

    public static ModelPlacement AddTree(
      GameInstance instance,
      Map map,
      VegetationGenerator.FloraModelSetup[] treeModels,
      GlobalPoint3D p,
      PcgRandom random,
      UpdateBlockMethod method,
      bool transmit)
    {
      int index = random.Next(treeModels.Length);
      VegetationGenerator.FloraModelSetup treeModel = treeModels[index];
      MapModel mapModel = treeModel.Model;
      if (mapModel == null)
      {
        mapModel = treeModel.Model = instance.SystemVoxelModelManager.LoadComponent(treeModel.ComPack, treeModel.ComName, false);
        Block block = Block.Wood;
        if (treeModel.ComName.Contains("Birch"))
          block = Block.BirchWood;
        GlobalPoint3D? blockInRow = mapModel.FindBlockInRow(block, 1);
        if (blockInRow.HasValue)
        {
          treeModel.Offset = blockInRow.Value;
          treeModel.Offset.Y = 0;
        }
        treeModels[index] = treeModel;
      }
      ModelPlacement modelPlacement = new ModelPlacement()
      {
        Model = mapModel,
        Point = p - treeModel.Offset
      };
      map.SetBlockData(p + GlobalPoint3D.Up, (byte) 0, (byte) 0, method, GamerID.Sys1, false);
      mapModel.Map.CopyTo(map, GlobalPoint3D.Zero, modelPlacement.Point, mapModel.ModelSize, GlobalPoint3D.MaxValue, GlobalPoint3D.MaxValue, 0, method, Map.CopyType.NoOverwrite, Map.CopyAccess.Restricted, GamerID.Sys1, transmit, (IProgressBar) null);
      return modelPlacement;
    }

    public static void AddTreeShadedGrass(
      Map map,
      BiomeBase biome,
      GlobalPoint3D p,
      int radius,
      PcgRandom random,
      UpdateBlockMethod method,
      bool transmit)
    {
      Vector3 vector3 = new Vector3();
      GlobalPoint3D p1 = new GlobalPoint3D();
      int num = radius * radius;
      for (p1.Z = p.Z - radius + 1; p1.Z < p.Z + radius; ++p1.Z)
      {
        for (p1.X = p.X - radius + 1; p1.X < p.X + radius; ++p1.X)
        {
          if (random.Next(100) > 10)
          {
            vector3.X = (float) (p1.X - p.X);
            vector3.Z = (float) (p1.Z - p.Z);
            vector3.Y = 0.0f;
            if ((double) vector3.LengthSquared() <= (double) num)
            {
              p1.Y = biome.GetGroundHeightGlobal(map, p1.X, p1.Z);
              if (map.GetBlockID(p1) == (byte) 1)
                map.SetBlockData(p1, (byte) 161, (byte) 0, method, GamerID.Sys1, transmit);
            }
          }
        }
      }
    }

    private static int GetSurfaceY(Map map, GlobalPoint3D p, Block match, int searchDistance)
    {
      Block block1 = Block.None;
      int y = p.Y;
      Block block2;
      if ((block2 = (Block) map.GetBlockID(p)) != Block.None)
      {
        for (++p.Y; map.IsValidPoint(p) && (block1 = (Block) map.GetBlockID(p)) != Block.None && p.Y <= y + searchDistance; ++p.Y)
          block2 = block1;
        if (block1 != Block.None || block2 != match)
          return -1;
        return p.Y - 1;
      }
      for (--p.Y; map.IsValidPoint(p) && (block1 = (Block) map.GetBlockID(p)) == Block.None && p.Y >= y - searchDistance; --p.Y)
        block2 = block1;
      if (block1 != match || block2 != Block.None)
        return -1;
      return p.Y;
    }

    public static ModelPlacement AddCactus(
      GameInstance instance,
      Map map,
      GlobalPoint3D p,
      PcgRandom random,
      UpdateBlockMethod method)
    {
      int index = random.Next(VegetationGenerator.CactusModels.Length);
      VegetationGenerator.FloraModelSetup cactusModel = VegetationGenerator.CactusModels[index];
      MapModel mapModel = cactusModel.Model;
      if (mapModel == null)
      {
        mapModel = cactusModel.Model = instance.SystemVoxelModelManager.LoadComponent("System", cactusModel.ComName, false);
        GlobalPoint3D? blockInRow = mapModel.FindBlockInRow(Block.Cactus, 1);
        if (blockInRow.HasValue)
        {
          cactusModel.Offset = blockInRow.Value;
          cactusModel.Offset.Y = 0;
        }
        VegetationGenerator.CactusModels[index] = cactusModel;
      }
      ModelPlacement modelPlacement = new ModelPlacement()
      {
        Model = mapModel,
        Point = p - cactusModel.Offset
      };
      mapModel.Map.CopyTo(map, GlobalPoint3D.Zero, modelPlacement.Point, mapModel.ModelSize, GlobalPoint3D.MaxValue, GlobalPoint3D.MaxValue, 0, method, Map.CopyType.NoOverwrite, Map.CopyAccess.Restricted, GamerID.Sys1, false, (IProgressBar) null);
      return modelPlacement;
    }

    public static void CreateFloral(MapOld map, int floralCount, IProgressBar progressBar)
    {
      for (int index = 0; index < floralCount; ++index)
      {
        progressBar.AddProgress(1f / (float) floralCount);
        VegetationGenerator.AddFloral(map);
      }
    }

    private static void AddFloral(MapOld map)
    {
      int num = 0;
      while (++num < 100)
      {
        int x = map.Random.Next(1, map.MapSize.X - 1);
        int z = map.Random.Next(1, map.MapSize.Z - 1);
        if (VegetationGenerator.PlaceFloral(map, new Point3D(x, 0, z)))
          break;
      }
    }

    private static bool PlaceFloral(MapOld map, Point3D p)
    {
      bool flag = false;
      int num1 = map.Random.Next(1, 4);
      Point3D p1 = new Point3D();
      int num2 = map.Random.Next(6);
      if (num2 > 3)
        num2 -= 2;
      for (int index1 = p.Z - num1; index1 <= p.Z + num1; ++index1)
      {
        for (int index2 = p.X - num1; index2 <= p.X + num1; ++index2)
        {
          p1.X = index2;
          p1.Z = index1;
          p1.Y = 0;
          p1 = map.Clamp(p1, 1);
          p1.Y = map.GetHeight(p1) + 1;
          if (p1.Y > map.WaterLevel + 1 && p1.Y < 0 && (BlockData.IsGrassOrDirt((Block) map.GetBlockID(p1 + Point3D.Down)) && map.Random.Next(10) > 2))
          {
            int num3 = map.Random.Next(10) == 0 ? map.Random.Next(4) : num2;
            map.SetBlockData(p1, (byte) (59 + num3), (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
            flag = true;
          }
        }
      }
      return flag;
    }

    public static void FlowerDecoration(
      Map map,
      GlobalPoint3D p,
      float chance,
      int minCount,
      int maxCount,
      int maxWidth,
      float density,
      int maxY,
      PcgRandom random,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (random.NextDouble() > (double) chance)
        return;
      Point3D chunkSize = map.ChunkSize;
      VegetationGenerator.FlowerDecorationCore(map, p.X, p.Z, maxWidth, density, maxY, random, chunkSize.X, chunkSize.Y, chunkSize.Z, map.MapBound.Min, map.MapBound.Max, method, playerID, transmit);
    }

    public static void FlowerDecoration(
      Map map,
      GlobalPoint3D chunkGlobalOffset,
      float chance,
      int minCount,
      int maxCount,
      int maxWidth,
      float density,
      int maxY,
      PcgRandom random,
      int chunkSizeX,
      int chunkSizeY,
      int chunkSizeZ,
      GlobalPoint3D mapBoundMin,
      GlobalPoint3D mapBoundMax,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (chunkGlobalOffset.Y <= (int) map.SeaLevel - chunkSizeY || chunkGlobalOffset.Y > maxY || random.NextDouble() > (double) chance)
        return;
      int num = random.Next(minCount, maxCount + 1);
      for (int index = 0; index < num; ++index)
      {
        int x = random.Next(chunkSizeX) + chunkGlobalOffset.X;
        int z = random.Next(chunkSizeZ) + chunkGlobalOffset.Z;
        VegetationGenerator.FlowerDecorationCore(map, x, z, maxWidth, density, maxY, random, chunkSizeX, chunkSizeY, chunkSizeZ, mapBoundMin, mapBoundMax, method, playerID, transmit);
      }
    }

    private static void FlowerDecorationCore(
      Map map,
      int x,
      int z,
      int maxWidth,
      float density,
      int maxY,
      PcgRandom random,
      int chunkSizeX,
      int chunkSizeY,
      int chunkSizeZ,
      GlobalPoint3D mapBoundMin,
      GlobalPoint3D mapBoundMax,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      int num1 = random.Next(2, maxWidth);
      int num2 = random.Next(6);
      if (num2 > 3)
        num2 -= 2;
      int num3 = 59;
      byte auxData = 4;
      int num4 = x - num1;
      if (num4 < mapBoundMin.X)
        num4 = mapBoundMin.X;
      int num5 = x + num1;
      if (num5 >= mapBoundMax.X)
        num5 = mapBoundMax.X - 1;
      int num6 = z - num1;
      if (num6 < mapBoundMin.Z)
        num6 = mapBoundMin.Z;
      int num7 = z + num1;
      if (num7 >= mapBoundMax.Z)
        num7 = mapBoundMax.Z - 1;
      GlobalPoint3D p2 = new GlobalPoint3D(x, 0, z);
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      for (globalPoint3D.Z = num6; globalPoint3D.Z <= num7; ++globalPoint3D.Z)
      {
        for (globalPoint3D.X = num4; globalPoint3D.X <= num5; ++globalPoint3D.X)
        {
          globalPoint3D.Y = (int) map.GetHeight(globalPoint3D);
          if (globalPoint3D.Y < maxY)
          {
            p2.Y = globalPoint3D.Y;
            if ((double) GlobalPoint3D.Distance(globalPoint3D, p2) < (double) maxWidth && BlockData.IsGrassOrDirt((Block) map.GetBlockIDNoCache(globalPoint3D)))
            {
              ++globalPoint3D.Y;
              if (map.GetBlockIDNoCache(globalPoint3D) == (byte) 0 && random.NextDouble() <= (double) density)
              {
                int num8 = random.Next(10) == 0 ? random.Next(4) : num2;
                if (VegetationGenerator.CanPlace(map, globalPoint3D, method, playerID))
                  map.SetBlockData(globalPoint3D, (byte) (num3 + num8), auxData, method, playerID, transmit)?.SetChunkFlag(ChunkFlags.LightDirty);
              }
            }
          }
        }
      }
    }

    public static void CreateGrassStones(MapOld map, int stoneCount, IProgressBar progressBar)
    {
      float increment = 1f / (float) stoneCount;
      for (int index = 0; index < stoneCount; ++index)
      {
        progressBar.AddProgress(increment);
        VegetationGenerator.AddGrassStones(map);
      }
    }

    private static void AddGrassStones(MapOld map)
    {
      int num = 0;
      while (++num < 100)
      {
        int x = map.Random.Next(1, map.MapSize.X - 1);
        int z = map.Random.Next(1, map.MapSize.Z - 1);
        if (VegetationGenerator.PlaceGrassStones(map, new Point3D(x, 0, z)))
          break;
      }
    }

    private static bool PlaceGrassStones(MapOld map, Point3D p)
    {
      bool flag = false;
      int num = map.Random.Next(0, 3);
      Point3D p1 = new Point3D();
      for (int index1 = p.Z - num; index1 <= p.Z + num; ++index1)
      {
        for (int index2 = p.X - num; index2 <= p.X + num; ++index2)
        {
          p1.X = index2;
          p1.Z = index1;
          p1.Y = 0;
          p1 = map.Clamp(p1, 1);
          p1.Y = map.GetHeight(p1) + 1;
          if (p1.Y > map.WaterLevel + 1 && p1.Y < 0 && (BlockData.IsGrassOrDirt((Block) map.GetBlockID(p1 + Point3D.Down)) && map.Random.Next(10) > 5))
          {
            --p1.Y;
            map.SetBlockData(p1, (byte) 79, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
            flag = true;
          }
        }
      }
      return flag;
    }

    public static void CreateLongGrass(MapOld map, int grassCount, IProgressBar progressBar)
    {
      float increment = 1f / (float) grassCount;
      for (int index = 0; index < grassCount; ++index)
      {
        progressBar.AddProgress(increment);
        VegetationGenerator.AddLongGrass(map);
      }
    }

    private static void AddLongGrass(MapOld map)
    {
      int num = 0;
      while (++num < 100)
      {
        int x = map.Random.Next(1, map.MapSize.X - 1);
        int z = map.Random.Next(1, map.MapSize.Z - 1);
        if (VegetationGenerator.PlaceLongGrass(map, new Point3D(x, 0, z)))
          break;
      }
    }

    private static bool PlaceLongGrass(MapOld map, Point3D p)
    {
      bool flag = false;
      int num = map.Random.Next(1, 4);
      Point3D p1 = new Point3D();
      for (int index1 = p.Z - num; index1 <= p.Z + num; ++index1)
      {
        for (int index2 = p.X - num; index2 <= p.X + num; ++index2)
        {
          p1.X = index2;
          p1.Z = index1;
          p1.Y = 0;
          p1 = map.Clamp(p1, 1);
          p1.Y = map.GetHeight(p1) + 1;
          if (p1.Y > map.WaterLevel + 1 && p1.Y < 0 && (BlockData.IsGrassOrDirt((Block) map.GetBlockID(p1 + Point3D.Down)) && map.Random.Next(10) > 5))
          {
            map.SetBlockData(p1, (byte) 112, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
            flag = true;
          }
        }
      }
      return flag;
    }

    public static void GrassDecoration(
      Map map,
      GlobalPoint3D p,
      float chance,
      int minCount,
      int maxCount,
      int maxWidth,
      float density,
      int berryBushChance,
      int maxY,
      PcgRandom random,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (random.NextDouble() > (double) chance)
        return;
      Point3D chunkSize = map.ChunkSize;
      VegetationGenerator.GrassDecorationCore(map, p.X, p.Z, maxWidth, density, berryBushChance, maxY, random, chunkSize.X, chunkSize.Y, chunkSize.Z, map.MapBound.Min, map.MapBound.Max, method, playerID, transmit);
    }

    public static void GrassDecoration(
      Map map,
      GlobalPoint3D chunkGlobalOffset,
      float chance,
      int minCount,
      int maxCount,
      int maxWidth,
      float density,
      int berryBushChance,
      int maxY,
      PcgRandom random,
      int chunkSizeX,
      int chunkSizeY,
      int chunkSizeZ,
      GlobalPoint3D mapBoundMin,
      GlobalPoint3D mapBoundMax,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (chunkGlobalOffset.Y <= (int) map.SeaLevel - chunkSizeY || chunkGlobalOffset.Y > maxY || random.NextDouble() > (double) chance)
        return;
      int num = random.Next(minCount, maxCount + 1);
      for (int index = 0; index < num; ++index)
      {
        int x = random.Next(chunkSizeX) + chunkGlobalOffset.X;
        int z = random.Next(chunkSizeZ) + chunkGlobalOffset.Z;
        VegetationGenerator.GrassDecorationCore(map, x, z, maxWidth, density, berryBushChance, maxY, random, chunkSizeX, chunkSizeY, chunkSizeZ, mapBoundMin, mapBoundMax, method, playerID, transmit);
      }
    }

    private static void GrassDecorationCore(
      Map map,
      int x,
      int z,
      int maxWidth,
      float density,
      int berryBushChance,
      int maxY,
      PcgRandom random,
      int chunkSizeX,
      int chunkSizeY,
      int chunkSizeZ,
      GlobalPoint3D mapBoundMin,
      GlobalPoint3D mapBoundMax,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      int num1 = random.Next(maxWidth / 3, maxWidth);
      byte auxData = 4;
      int num2 = x - num1;
      if (num2 < mapBoundMin.X)
        num2 = mapBoundMin.X;
      int num3 = x + num1;
      if (num3 >= mapBoundMax.X)
        num3 = mapBoundMax.X - 1;
      int num4 = z - num1;
      if (num4 < mapBoundMin.Z)
        num4 = mapBoundMin.Z;
      int num5 = z + num1;
      if (num5 >= mapBoundMax.Z)
        num5 = mapBoundMax.Z - 1;
      GlobalPoint3D p2 = new GlobalPoint3D(x, 0, z);
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      for (globalPoint3D.Z = num4; globalPoint3D.Z <= num5; ++globalPoint3D.Z)
      {
        for (globalPoint3D.X = num2; globalPoint3D.X <= num3; ++globalPoint3D.X)
        {
          globalPoint3D.Y = (int) map.GetHeight(globalPoint3D);
          if (globalPoint3D.Y < maxY)
          {
            p2.Y = globalPoint3D.Y;
            if ((double) GlobalPoint3D.Distance(globalPoint3D, p2) < (double) maxWidth - 0.400000005960464 && BlockData.IsGrassOrDirt((Block) map.GetBlockIDNoCache(globalPoint3D)))
            {
              ++globalPoint3D.Y;
              if (map.GetBlockIDNoCache(globalPoint3D) == (byte) 0 && random.NextDouble() <= (double) density && VegetationGenerator.CanPlace(map, globalPoint3D, method, playerID))
              {
                byte blockID = random.Next(berryBushChance) == 0 ? (byte) 223 : (byte) 112;
                map.SetBlockData(globalPoint3D, blockID, auxData, method, playerID, transmit)?.SetChunkFlag(ChunkFlags.LightDirty);
              }
            }
          }
        }
      }
    }

    private static bool CanPlace(
      Map map,
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      return method == UpdateBlockMethod.Generation || map.GetClearBlockResult(p, method, playerID) == ClearBlockResult.Success;
    }

    public struct FloraModelSetup
    {
      public string ComPack;
      public string ComName;
      public MapModel Model;
      public GlobalPoint3D Offset;
    }
  }
}
