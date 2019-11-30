// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.BulletPatternBase
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal abstract class BulletPatternBase
  {
    protected PcgRandom random;

    protected BulletPatternBase(PcgRandom random)
    {
      this.random = random;
    }

    public virtual void FireBullets(
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game,
      Actor actor,
      Vector2 launchPosition,
      Actor target,
      ref BulletPattern pattern)
    {
      this.AddBullet(game, actor, launchPosition, this.SelectNewBulletDirection(actor, launchPosition, target), pattern.ColorID);
    }

    protected virtual void AddBullet(
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
      bullet.ColorIndex = colorID == 0 ? game.Random.Next(1, TotalRushRenderer.MaxBulletColors) : colorID;
      bullet.ActorType = ActorType.EnemyBullet;
      ActorDataXML actorDataXml = game.ActorData[(int) bullet.ActorType];
      bullet.Scale = actorDataXml.Scale;
      bullet.HitBoxRadius = actorDataXml.HitBoxRadius;
      bullet.HitBoxRadius2 = actorDataXml.HitBoxRadius * actorDataXml.HitBoxRadius;
      bullet.Position.X = launchPosition.X;
      bullet.Position.Y = launchPosition.Y;
      bullet.Velocity.X = direction.X * actorDataXml.Speed.X;
      bullet.Velocity.Y = direction.Y * actorDataXml.Speed.Y;
      game.Bullets[i] = bullet;
    }

    protected virtual Vector2 SelectNewBulletDirection(
      Actor actor,
      Vector2 launchPosition,
      Actor target)
    {
      return actor.Aim;
    }
  }
}
