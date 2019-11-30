// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.TrialTeleportGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Generators
{
  internal class TrialTeleportGenerator : SpecialBlockGenerator
  {
    private static List<Point3D> points;

    public static void CreateTeleports(MapOld map, List<Point3D> lightPoints, Point3D spawnPoint)
    {
      TrialTeleportGenerator.points = new List<Point3D>();
      TrialTeleportGenerator teleportGenerator1 = new TrialTeleportGenerator();
      teleportGenerator1.LightPoints = lightPoints;
      TrialTeleportGenerator teleportGenerator2 = teleportGenerator1;
      for (int i = 0; i < 10; ++i)
      {
        Point3D point3D = teleportGenerator2.AddBlock(map, i, Block.Obsidian, new Vector2(0.2f, 0.5f), -map.MapSize.Y / 2, new ValidatePoint(TrialTeleportGenerator.CheckPoint), 0, map.Random);
        TrialTeleportGenerator.points.Add(point3D);
      }
      map.SetBlockData(spawnPoint + new Point3D(3, 1, 0), (byte) 0, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
      map.SetBlockData(spawnPoint + new Point3D(3, 2, 0), (byte) 0, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
      map.SetBlockData(spawnPoint + new Point3D(3, 0, 0), (byte) 31, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
      TrialTeleportGenerator.points = (List<Point3D>) null;
    }

    private static bool CheckPoint(Point3D p, int i)
    {
      foreach (Point3D point in TrialTeleportGenerator.points)
      {
        if ((double) Point3D.DistanceSquared(p, point) < 20000.0)
          return false;
      }
      return true;
    }
  }
}
