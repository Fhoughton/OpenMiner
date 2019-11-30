// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.CrossRotatedBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class CrossRotatedBulletPattern : BulletPatternBase
  {
    public CrossRotatedBulletPattern(PcgRandom random)
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
      float num = 1.570796f;
      Vector2 v = new Vector2(0.0f, -1f);
      float rotation = actor.Rotation;
      Vector2 launchPosition1 = new Vector2();
      for (int index = 0; index < 4; ++index)
      {
        Vector2 direction = Vector2.Normalize(MyMathHelper.RotateVector2ByAngle(v, rotation));
        direction.X = -direction.X;
        launchPosition1.X = launchPosition.X + direction.X * 10f;
        launchPosition1.Y = launchPosition.Y + direction.Y * 10f;
        this.AddBullet(game, actor, launchPosition1, direction, pattern.ColorID);
        rotation += num;
      }
    }
  }
}
