// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.AtTargetAndRotateParentBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class AtTargetAndRotateParentBulletPattern : AtTargetBulletPattern
  {
    public AtTargetAndRotateParentBulletPattern(PcgRandom random)
      : base(random)
    {
    }

    protected override Vector2 SelectNewBulletDirection(
      Actor actor,
      Vector2 launchPosition,
      Actor target)
    {
      Vector2 v2 = base.SelectNewBulletDirection(actor, launchPosition, target);
      actor.Rotation = MyMathHelper.GetAngle(Vector2.Zero, v2);
      return v2;
    }
  }
}
