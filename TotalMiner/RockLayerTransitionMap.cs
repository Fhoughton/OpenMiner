// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.RockLayerTransitionMap
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class RockLayerTransitionMap
  {
    public byte[] Map;
    public byte Range;
    public Point Size;

    public void Generate(StudioForge.BlockWorld.Map map, byte range)
    {
      this.Range = range;
      float num1 = (float) range;
      int num2 = this.Size.X = map.MapSize.X;
      int num3 = this.Size.Y = map.MapSize.Z;
      this.Map = new byte[num2 * num3];
      int[] simplexNoisePermTable = SimplexNoise1.GetSimplexNoisePermTable(map.Seed);
      float num4 = 100f;
      float num5 = 30f;
      float num6 = 7f;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          float num7 = (float) (((double) SimplexNoise1.noise((float) index2 / num4, (float) index1 / num4, simplexNoisePermTable) * 11.0 + (double) SimplexNoise1.noise((float) index2 / num5, (float) index1 / num5, simplexNoisePermTable) * 1.10000002384186 + (double) SimplexNoise1.noise((float) index2 / num6, (float) index1 / num6, simplexNoisePermTable) * 0.109999999403954) / 11.0);
          this.Map[index2 + index1 * num3] = (byte) ((double) num7 * (double) num1);
        }
      }
    }

    public byte GetValue(int x, int z, int d)
    {
      switch (d)
      {
        case 1:
          return this.Map[(x % this.Size.Y + 1) * this.Size.X - z % this.Size.X - 1];
        case 2:
          return this.Map[(z % this.Size.Y + 1) * this.Size.X - x % this.Size.X - 1];
        case 3:
          return this.Map[(this.Size.Y - x % this.Size.Y - 1) * this.Size.X + z % this.Size.X];
        default:
          return this.Map[x % this.Size.X + z % this.Size.Y * this.Size.X];
      }
    }
  }
}
