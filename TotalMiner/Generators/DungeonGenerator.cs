// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.DungeonGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Generators
{
  internal static class DungeonGenerator
  {
    private static Map map;
    private static Dictionary<MapChunk, List<BlastPoint>> blastPoints;
    private static GlobalPoint3D p;
    private static int depth;
    private static int width;
    private static int decentRate;
    private static int lightChance;
    private static PcgRandom random;
    private static TerrainGeneratorBase biome;
    private static int treasureChestMinDepth;
    private static int treasureChestChance;

    public static void CreateSurfaceDungeons(
      GameInstance instance,
      MapTM map,
      BiomeType biomeType,
      int dungeonCount,
      int minDepth,
      int maxDepth,
      int treasureChestMinDepth,
      int treasureChestChance,
      Dictionary<MapChunk, List<BlastPoint>> blastPoints,
      IProgressBar progressBar)
    {
      GlobalPoint3D mapSize = map.MapSize;
      PcgRandom random = map.Random;
      TerrainGeneratorBase biome;
      int handle;
      MapTM.GetBiome(biomeType, out biome, out handle);
      biome.Initialize(instance, map, Globals2.GameProperties.SaveGame.Header.BiomeParams);
      biome.InitializeRandom(map.Seed);
      try
      {
        for (int index = 0; index < dungeonCount; ++index)
        {
          int num = 0;
          while (num++ < 50)
          {
            GlobalPoint3D _p = new GlobalPoint3D(map.Random.Next(40, mapSize.X - 40), 0, map.Random.Next(40, mapSize.Z - 40));
            _p.Y = biome.GetGroundHeightGlobal((Map) map, _p.X, _p.Z);
            if (_p.Y >= (int) map.SeaLevel)
            {
              int _depth = map.Random.Next(minDepth, maxDepth);
              int _width = map.Random.Next(2, 5);
              int _decentRate = map.Random.Next(1, 3);
              DungeonGenerator.CreateDungeon((Map) map, _p, _depth, _width, _decentRate, 3, treasureChestMinDepth, treasureChestChance, random, blastPoints, biome);
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(56, ex);
      }
      finally
      {
        MapTM.ReleaseBiome(biomeType, biome, handle);
      }
    }

    public static void CreateLowerDungeons(
      Map map,
      int dungeonCount,
      Dictionary<MapChunk, List<BlastPoint>> blastPoints,
      IProgressBar progressBar)
    {
      GlobalPoint3D mapSize = map.MapSize;
      int max = map.MapSize.Y / 6 - 100;
      int min = mapSize.Y / 7;
      PcgRandom random = map.Random;
      for (int index = 0; index < dungeonCount; ++index)
      {
        int _depth = map.Random.Next(max / 2, max);
        GlobalPoint3D _p = new GlobalPoint3D(map.Random.Next(40, mapSize.X - 40), map.Random.Next(min, mapSize.Y - min - _depth), map.Random.Next(40, mapSize.Z - 40));
        int _width = random.Next(2, 4);
        int _decentRate = random.Next(1, 5);
        DungeonGenerator.CreateDungeon(map, _p, _depth, _width, _decentRate, 3, (int) map.SeaLevel - 200, 80, random, blastPoints, (TerrainGeneratorBase) null);
      }
    }

    public static void ClearStaticData()
    {
      DungeonGenerator.map = (Map) null;
      DungeonGenerator.biome = (TerrainGeneratorBase) null;
      if (DungeonGenerator.blastPoints == null)
        return;
      DungeonGenerator.blastPoints.Clear();
    }

    private static void CreateDungeon(
      Map _map,
      GlobalPoint3D _p,
      int _depth,
      int _width,
      int _decentRate,
      int _lightChance,
      int _treasureChestMinDepth,
      int _treasureChestChance,
      PcgRandom cdr,
      Dictionary<MapChunk, List<BlastPoint>> _blastPoints,
      TerrainGeneratorBase _biome)
    {
      DungeonGenerator.map = _map;
      DungeonGenerator.p = _p;
      DungeonGenerator.depth = _depth;
      DungeonGenerator.width = _width;
      DungeonGenerator.decentRate = _decentRate;
      DungeonGenerator.lightChance = _lightChance;
      DungeonGenerator.treasureChestMinDepth = _treasureChestMinDepth;
      DungeonGenerator.treasureChestChance = _treasureChestChance;
      DungeonGenerator.random = cdr;
      DungeonGenerator.blastPoints = _blastPoints;
      DungeonGenerator.biome = _biome;
      DungeonGenerator.ClearDungeon();
    }

    private static void ClearDungeon()
    {
      int widthChangeRate = DungeonGenerator.GetWidthChangeRate(DungeonGenerator.width);
      int num1 = DungeonGenerator.random.Next(4);
      float radians = 0.0f;
      float num2 = (float) (DungeonGenerator.random.NextDouble() * 0.5 - 0.5);
      int num3 = DungeonGenerator.p.Y - DungeonGenerator.depth;
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      GlobalPoint3D p = DungeonGenerator.p;
      GlobalPoint3D mapSize = DungeonGenerator.map.MapSize;
      float num4 = 0.0f;
      MapChunk mapChunk = (MapChunk) null;
      List<BlastPoint> blastPointList = (List<BlastPoint>) null;
      while (DungeonGenerator.p.Y > num3 && DungeonGenerator.p.Y > DungeonGenerator.map.MapBound.Min.Y + 4)
      {
        MapChunk chunk = DungeonGenerator.map.GetChunk(DungeonGenerator.p);
        if (chunk != mapChunk && !DungeonGenerator.blastPoints.TryGetValue(chunk, out blastPointList))
        {
          blastPointList = new List<BlastPoint>();
          DungeonGenerator.blastPoints.Add(chunk, blastPointList);
        }
        bool flag = false;
        if ((double) GlobalPoint3D.Distance(DungeonGenerator.p, p) > (double) num4)
        {
          if ((double) num4 > 0.0)
            flag = true;
          num4 = DungeonGenerator.random.Next(6) != 0 ? (float) (DungeonGenerator.random.Next(12) + 8) : (float) DungeonGenerator.random.Next(50, 100);
          p = DungeonGenerator.p;
        }
        Vector3 vector3 = Vector3.Normalize(DungeonGenerator.p.ToVector3() - globalPoint3D.ToVector3());
        BlastPoint blastPoint = new BlastPoint()
        {
          Point = DungeonGenerator.p,
          Direction = vector3,
          Radius = DungeonGenerator.width,
          Torch = flag
        };
        if (!flag && DungeonGenerator.random.Next(30) == 0)
          blastPoint.MobSpawn = true;
        blastPointList.Add(blastPoint);
        if (DungeonGenerator.p.Y < DungeonGenerator.treasureChestMinDepth && DungeonGenerator.random.Next(DungeonGenerator.treasureChestChance) == 0)
          DungeonGenerator.GenerateHiddenTreasureCave(DungeonGenerator.map, DungeonGenerator.p, DungeonGenerator.random.Next(4));
        globalPoint3D = DungeonGenerator.p;
        int width = DungeonGenerator.width;
        int num5 = Math.Min(DungeonGenerator.width, DungeonGenerator.random.Next(3) * DungeonGenerator.decentRate);
        DungeonGenerator.p.Y -= num5;
        radians += num2;
        Vector2 position = new Vector2(0.0f, (float) DungeonGenerator.width);
        Matrix rotationZ = Matrix.CreateRotationZ(radians);
        position = Vector2.Transform(position, rotationZ);
        DungeonGenerator.p.X += (int) position.X;
        DungeonGenerator.p.Z += (int) position.Y;
        if (DungeonGenerator.p.X < 2)
        {
          num1 = 0;
          DungeonGenerator.p.X = 2;
        }
        if (DungeonGenerator.p.Z < 2)
        {
          num1 = 0;
          DungeonGenerator.p.Z = 2;
        }
        if (DungeonGenerator.p.X > mapSize.X - 2)
        {
          num1 = 0;
          DungeonGenerator.p.X = mapSize.X - 2;
        }
        if (DungeonGenerator.p.Z > mapSize.Z - 2)
        {
          num1 = 0;
          DungeonGenerator.p.Z = mapSize.Z - 2;
        }
        if (--num1 <= 0)
        {
          num2 = (float) (DungeonGenerator.random.NextDouble() * 0.3 + 0.2);
          if (DungeonGenerator.random.Next(2) == 0)
            num2 = -num2;
          num1 = DungeonGenerator.random.Next(4);
        }
        if (--widthChangeRate <= 0)
        {
          DungeonGenerator.width += DungeonGenerator.random.Next(Math.Max(2, DungeonGenerator.width)) - Math.Max(1, DungeonGenerator.width / 2);
          int num6 = DungeonGenerator.map.MapBound.Max.Y - DungeonGenerator.map.MapBound.Min.Y;
          int max = MyMathHelper.Clamp((int) ((double) (num6 - DungeonGenerator.p.Y) / (double) num6 * 10.0), 2, 10) * 2;
          DungeonGenerator.width = MyMathHelper.Clamp(DungeonGenerator.width, 3, max);
          widthChangeRate = DungeonGenerator.GetWidthChangeRate(DungeonGenerator.width);
        }
      }
    }

    private static void GenerateHiddenTreasureCave(Map map, GlobalPoint3D p, int face)
    {
      if (p.X <= map.MapBound.Min.X + 30 || p.X >= map.MapBound.Max.X - 30 || (p.Z <= map.MapBound.Min.Z + 30 || p.Z >= map.MapBound.Max.Z - 30))
        return;
      int num = 3;
      Point3D reverseFaceOffset = DungeonGenerator.GetReverseFaceOffset(face);
      MapChunk mapChunk = (MapChunk) null;
      List<BlastPoint> blastPointList = (List<BlastPoint>) null;
      for (int index = 0; index < 4; ++index)
      {
        p += reverseFaceOffset * (num - 1) * 2;
        MapChunk chunk = map.GetChunk(p);
        if (chunk != null)
        {
          if (chunk != mapChunk && !DungeonGenerator.blastPoints.TryGetValue(chunk, out blastPointList))
          {
            blastPointList = new List<BlastPoint>();
            DungeonGenerator.blastPoints.Add(chunk, blastPointList);
          }
          blastPointList.Add(new BlastPoint()
          {
            Point = p,
            Direction = Vector3.Normalize(reverseFaceOffset.ToVector3()),
            Radius = num,
            Strength = 100f,
            TreasureChest = index == 3,
            Torch = index == 0 || index == 3
          });
        }
        ++num;
      }
    }

    private static int GetWidthChangeRate(int width)
    {
      return 1;
    }

    private static GlobalPoint3D FindDryFloor(Map map, GlobalPoint3D p)
    {
      while (map.IsValidPoint(p) && !map.IsSolid(p))
        --p.Y;
      while (map.GetBlockID(p) == (byte) 11)
        --p.X;
      ++p.Y;
      return p;
    }

    private static Point3D GetFaceOffset(int face)
    {
      switch (face)
      {
        case 1:
          return Point3D.Left;
        case 2:
          return Point3D.Right;
        case 3:
          return Point3D.Forward;
        case 4:
          return Point3D.Backward;
        default:
          return Point3D.Zero;
      }
    }

    private static Point3D GetReverseFaceOffset(int face)
    {
      switch (face)
      {
        case 1:
          return Point3D.Right;
        case 2:
          return Point3D.Left;
        case 3:
          return Point3D.Backward;
        case 4:
          return Point3D.Forward;
        default:
          return Point3D.Zero;
      }
    }
  }
}
