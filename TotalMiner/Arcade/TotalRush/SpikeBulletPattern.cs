// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.SpikeBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class SpikeBulletPattern : BulletPatternBase
  {
    public SpikeBulletPattern(PcgRandom random)
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
      Vector2 direction = Vector2.Normalize(target.WorldPosition - launchPosition);
      Vector2 launchPosition1 = launchPosition;
      int num = (int) (8.0 * (double) game.DifficultyFactor);
      for (int index = 0; index < num; ++index)
      {
        this.AddBullet(game, actor, launchPosition1, direction, pattern.ColorID);
        launchPosition1.X += direction.X * 4f;
        launchPosition1.Y += direction.Y * 4f;
      }
    }
  }
}
