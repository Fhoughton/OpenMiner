// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.SpikePincerBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class SpikePincerBulletPattern : BulletPatternBase
  {
    public SpikePincerBulletPattern(PcgRandom random)
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
      float angle = MyMathHelper.GetAngle(launchPosition, target.WorldPosition);
      float num1 = MyMathHelper.WrapAngle(angle - 0.1745329f);
      float num2 = MyMathHelper.WrapAngle(angle + 0.1745329f);
      Vector2 direction1 = new Vector2();
      direction1.X = (float) Math.Cos((double) num1);
      direction1.Y = (float) Math.Sin((double) num1);
      Vector2 direction2 = new Vector2();
      direction2.X = (float) Math.Cos((double) num2);
      direction2.Y = (float) Math.Sin((double) num2);
      Vector2 launchPosition1 = launchPosition;
      Vector2 launchPosition2 = launchPosition;
      int num3 = (int) (8.0 * (double) game.DifficultyFactor);
      for (int index = 0; index < num3; ++index)
      {
        this.AddBullet(game, actor, launchPosition1, direction1, pattern.ColorID);
        this.AddBullet(game, actor, launchPosition2, direction2, pattern.ColorID);
        launchPosition1.X += direction1.X * 4f;
        launchPosition1.Y += direction1.Y * 4f;
        launchPosition2.X += direction2.X * 4f;
        launchPosition2.Y += direction2.Y * 4f;
      }
    }
  }
}
