// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.PlayerTailShipBulletPattern
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class PlayerTailShipBulletPattern : BulletPatternBase
  {
    public PlayerTailShipBulletPattern(PcgRandom random)
      : base(random)
    {
    }

    protected override void AddBullet(
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game,
      Actor actor,
      Vector2 launchPosition,
      Vector2 direction,
      int colorID)
    {
      if (game.UnusedBullets.Count <= 0)
        return;
      int i = game.UnusedBullets.Pop();
      game.UsedBullets.AddLast(game.GetListNode(i));
      Bullet bullet = game.Bullets[i];
      bullet.ColorIndex = game.Random.Next(TotalRushRenderer.MaxBulletColors);
      bullet.ActorType = ActorType.PlayerBullet;
      ActorDataXML actorDataXml = game.ActorData[(int) bullet.ActorType];
      bullet.Scale = actorDataXml.Scale;
      bullet.HitBoxRadius = actorDataXml.HitBoxRadius;
      bullet.HitBoxRadius2 = actorDataXml.HitBoxRadius * actorDataXml.HitBoxRadius;
      bullet.Velocity.X = direction.X;
      bullet.Velocity.Y = direction.Y;
      bullet.Velocity.X += (float) (game.Random.NextDouble() * 0.25 - 0.125);
      bullet.Velocity.Y += (float) (game.Random.NextDouble() * 0.25 - 0.125);
      bullet.Velocity.X *= game.PlayerBulletData.Speed.X;
      bullet.Velocity.Y *= game.PlayerBulletData.Speed.Y;
      bullet.Position.X = actor.WorldPosition.X + bullet.Velocity.X * 0.5f;
      bullet.Position.Y = actor.WorldPosition.Y + bullet.Velocity.Y * 0.5f;
      bullet.Rotation = MyMathHelper.WrapAngle((float) Math.Atan2((double) bullet.Velocity.Y, (double) bullet.Velocity.X) + 1.570796f);
      game.Bullets[i] = bullet;
    }

    protected override Vector2 SelectNewBulletDirection(
      Actor actor,
      Vector2 launchPosition,
      Actor target)
    {
      return new Vector2(0.0f, -1f);
    }
  }
}
