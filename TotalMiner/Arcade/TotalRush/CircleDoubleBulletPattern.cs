// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.CircleDoubleBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class CircleDoubleBulletPattern : CircleBulletPattern
  {
    public CircleDoubleBulletPattern(PcgRandom random)
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
      for (int index = 0; index < 28; ++index)
      {
        float num = (float) index * ((float) Math.PI / 14f);
        direction.X = (float) Math.Cos((double) num);
        direction.Y = (float) Math.Sin((double) num);
        this.AddBullet(game, actor, launchPosition, direction, pattern.ColorID);
        launchPosition1.X = launchPosition.X + direction.X * 10f;
        launchPosition1.Y = launchPosition.Y + direction.Y * 10f;
        this.AddBullet(game, actor, launchPosition1, direction, pattern.ColorID);
      }
    }
  }
}
