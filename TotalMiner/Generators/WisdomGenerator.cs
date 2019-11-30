// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.WisdomGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Generators
{
  internal class WisdomGenerator : SpecialBlockGenerator
  {
    public static void CreateWisdom(
      MapOld map,
      List<Point3D> lightPoints,
      int minLight,
      PcgRandom random)
    {
      minLight = 0;
      WisdomGenerator wisdomGenerator1 = new WisdomGenerator();
      wisdomGenerator1.LightPoints = lightPoints;
      WisdomGenerator wisdomGenerator2 = wisdomGenerator1;
      int length = Wisdom.WisdomList.Length;
      float num = (float) Wisdom.WisdomList[length - 1].Level * 1.5f;
      for (int i = 0; i < length; ++i)
      {
        WisdomItem wisdom = Wisdom.WisdomList[i];
        Vector2 range = new Vector2((float) wisdom.Level / num, (float) (wisdom.Level + 1) / num);
        wisdom.Point = (GlobalPoint3D) wisdomGenerator2.AddBlock(map, i, Block.Wisdom, range, -map.MapSize.Y + 1, new ValidatePoint(WisdomGenerator.CheckPoint), minLight, random);
      }
    }

    private static bool CheckPoint(Point3D p, int i)
    {
      return true;
    }
  }
}
