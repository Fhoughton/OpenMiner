// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.StarBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class StarBulletPattern : BulletPatternBase
  {
    private static Vector2[] vectors = new Vector2[8]
    {
      new Vector2(-1f, 0.0f),
      Vector2.Normalize(new Vector2(-1f, -1f)),
      new Vector2(0.0f, -1f),
      Vector2.Normalize(new Vector2(1f, -1f)),
      new Vector2(1f, 0.0f),
      Vector2.Normalize(new Vector2(1f, 1f)),
      new Vector2(0.0f, 1f),
      Vector2.Normalize(new Vector2(-1f, 1f))
    };

    public StarBulletPattern(PcgRandom random)
      : base(random)
    {
    }

    public override void FireBullets(
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game,
      Actor actor,
      Vector2 launchPosition,
      Actor target,
      ref BulletPattern pattern)
    {
      ++pattern.Data1;
      if (pattern.Data1 % 15 >= 13)
        return;
      Vector2 launchPosition1 = new Vector2();
      for (int index = 0; index < StarBulletPattern.vectors.Length; ++index)
      {
        Vector2 vector = StarBulletPattern.vectors[index];
        launchPosition1.X = launchPosition.X + vector.X * 8f;
        launchPosition1.Y = launchPosition.Y + vector.Y * 8f;
        this.AddBullet(game, actor, launchPosition1, vector, pattern.ColorID);
      }
    }
  }
}
