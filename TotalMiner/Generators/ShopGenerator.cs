// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.ShopGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Generators
{
  internal static class ShopGenerator
  {
    private static PcgRandom random;

    public static Point3D CreateShops(MapOld map, GameInstance instance, int seed)
    {
      ShopGenerator.random = seed == -1 ? map.Random : new PcgRandom(seed);
      TerrainData terrainData = Globals2.GameProperties.SaveGame.Header.TerrainData;
      int num1 = 5;
      int min1 = map.MapSize.X / 3;
      int max1 = map.MapSize.X - min1;
      int min2 = map.MapSize.Z / 3;
      int max2 = map.MapSize.Z - min1;
      while (true)
      {
        int num2 = 1000;
        while (--num2 > 0)
        {
          Point3D p = new Point3D(ShopGenerator.random.Next(min1, max1), 0, ShopGenerator.random.Next(min2, max2));
          p.Y = map.GetHeight(p);
          if (terrainData.Biome == BiomeType.Flat)
          {
            ShopGenerator.SetupShop(map, p, instance);
            return p;
          }
          if (p.Y > map.WaterLevel + 1 && p.Y < map.WaterLevel + num1 && ShopGenerator.IsSuitableShopBase(map, p))
          {
            ShopGenerator.SetupShop(map, p, instance);
            return p;
          }
        }
        ++num1;
      }
    }

    private static bool IsSuitableShopBase(MapOld map, Point3D p)
    {
      byte num = map.IsHellSeed() ? (byte) 32 : (map.IsMoonSeed() ? (byte) 22 : (byte) 1);
      return (int) map.GetBlockID(p) == (int) num;
    }

    private static void SetupShop(MapOld map, Point3D p, GameInstance instance)
    {
      Point3D point3D = p;
      for (p.X = point3D.X - 2; p.X < point3D.X + 3; ++p.X)
      {
        for (p.Z = point3D.Z - 1; p.Z < point3D.Z + 2; ++p.Z)
        {
          for (p.Y = point3D.Y + 3; p.Y > point3D.Y; --p.Y)
          {
            switch ((Block) map.GetBlockID(p))
            {
              case Block.Wisdom:
              case Block.Blueprint:
                continue;
              default:
                map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
                continue;
            }
          }
          map.SetBlockData(p, (byte) 25, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
          --p.Y;
          while (map.GetBlockID(p) == (byte) 0 || map.IsIcon(p))
          {
            switch ((Block) map.GetBlockID(p))
            {
              case Block.Wisdom:
              case Block.Blueprint:
                --p.Y;
                continue;
              default:
                map.SetBlockData(p, (byte) 25, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
                goto case Block.Wisdom;
            }
          }
        }
      }
      if (instance.IsLegendaryDifficulty)
        return;
      p = point3D;
      ++p.Y;
      --p.X;
      Block blockId1 = (Block) map.GetBlockID(p);
      if (blockId1 == Block.Wisdom && blockId1 == Block.Blueprint)
      {
        ++p.Y;
        map.SetBlockData(p, (byte) 64, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
        --p.Y;
      }
      else
        map.SetBlockData(p, (byte) 64, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
      p.X += 2;
      Block blockId2 = (Block) map.GetBlockID(p);
      if (blockId2 == Block.Wisdom && blockId2 == Block.Blueprint)
      {
        ++p.Y;
        map.SetBlockData(p, (byte) 65, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
        --p.Y;
      }
      else
        map.SetBlockData(p, (byte) 65, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
      --p.X;
    }
  }
}
