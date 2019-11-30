// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.CircleSmallRandomBlobsBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class CircleSmallRandomBlobsBulletPattern : CircleBulletPattern
  {
    public CircleSmallRandomBlobsBulletPattern(PcgRandom random)
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
      Vector2 direction = new Vector2();
      Vector2 launchPosition1 = new Vector2();
      int min = (int) (4.0 * (double) game.DifficultyFactor);
      int max = (int) (8.0 * (double) game.DifficultyFactor);
      for (int index1 = 0; index1 < 24; ++index1)
      {
        float num1 = (float) index1 * 0.2617994f;
        direction.X = (float) Math.Cos((double) num1);
        direction.Y = (float) Math.Sin((double) num1);
        int num2 = this.random.Next(min, max);
        for (int index2 = 0; index2 < num2; ++index2)
        {
          launchPosition1.X = launchPosition.X + (float) (this.random.NextDouble() * 20.0 - 10.0);
          launchPosition1.Y = launchPosition.Y + (float) (this.random.NextDouble() * 20.0 - 10.0);
          this.AddBullet(game, actor, launchPosition1, direction, pattern.ColorID);
        }
      }
    }
  }
}
