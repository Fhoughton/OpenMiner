// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.CircleDoubleRandomSliceBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class CircleDoubleRandomSliceBulletPattern : CircleBulletPattern
  {
    public CircleDoubleRandomSliceBulletPattern(PcgRandom random)
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
      int num1 = this.random.Next(0, 100);
      int num2 = this.random.Next(4, 9);
      for (int index = num1 - num2; index < num1 + num2; ++index)
      {
        float num3 = (float) index * ((float) Math.PI / 100f);
        direction.X = (float) Math.Cos((double) num3);
        direction.Y = (float) Math.Sin((double) num3);
        this.AddBullet(game, actor, launchPosition, direction, pattern.ColorID);
        launchPosition1.X = launchPosition.X + direction.X * 5f;
        launchPosition1.Y = launchPosition.Y + direction.Y * 5f;
        this.AddBullet(game, actor, launchPosition1, direction, pattern.ColorID);
      }
    }
  }
}
