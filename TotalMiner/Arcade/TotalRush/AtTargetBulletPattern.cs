// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.AtTargetBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class AtTargetBulletPattern : BulletPatternBase
  {
    public AtTargetBulletPattern(PcgRandom random)
      : base(random)
    {
    }

    protected override Vector2 SelectNewBulletDirection(
      Actor actor,
      Vector2 launchPosition,
      Actor target)
    {
      Vector2 vector2 = Vector2.Normalize(target.WorldPosition - launchPosition);
      float num = 1f;
      vector2.X += (float) (this.random.NextDouble() * (double) num * 2.0) - num;
      vector2.Y += (float) (this.random.NextDouble() * (double) num * 2.0) - num;
      vector2.Normalize();
      return vector2;
    }
  }
}
