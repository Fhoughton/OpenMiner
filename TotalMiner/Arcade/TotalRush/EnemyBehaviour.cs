// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.EnemyBehaviour
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class EnemyBehaviour : ActorBehaviour
  {
    public EnemyBehaviour(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
      : base(game)
    {
    }

    public override void Update(Actor actor)
    {
      if (actor.IsDestroyed)
      {
        this.UpdateDestroyed(actor);
        this.UpdateWorldData(actor);
      }
      else
      {
        actor.CurrentTargetTravelTime += 0.01666667f;
        if ((double) actor.CurrentTargetTravelTime >= (double) actor.TotalTargetTravelTime)
          this.SelectNewTargetPosition(actor);
        actor.Position = Vector2.SmoothStep(actor.OriginalPosition, actor.TargetPosition, actor.CurrentTargetTravelTime / actor.TotalTargetTravelTime);
        actor.Rotation += actor.RotationSpeed;
        this.UpdateWorldData(actor);
        this.FireWeapons(actor);
        this.UpdateExhaust(actor);
        this.CheckForCollisionWithPlayer(actor);
        --actor.Score;
      }
    }

    private void SelectNewTargetPosition(Actor actor)
    {
      actor.TargetPosition.X = (float) this.game.Random.Next((int) ((double) this.game.ScreenSize.X * 0.0500000007450581), (int) ((double) this.game.ScreenSize.X * 0.949999988079071));
      actor.TargetPosition.Y = (float) this.game.Random.Next((int) ((double) this.game.ScreenSize.Y * 0.0500000007450581), this.game.ScreenSize.Y * actor.MaxPosY / 100);
      actor.OriginalPosition = actor.Position;
      actor.CurrentTargetTravelTime = 0.0f;
      actor.TotalTargetTravelTime = ((actor.TargetPosition - actor.Position) / (actor.Speed * 60f)).Length() * (float) (1.0 + this.game.Random.NextDouble());
    }

    public override void DestroyActor(Actor actor)
    {
      if (this.game.ActorData[(int) actor.ActorType].IsBigShipKill)
        this.game.BigShipKill();
      base.DestroyActor(actor);
    }

    public override void OnSmartBombActivated(Actor actor, Actor player)
    {
      if (!actor.IsVulnerable || actor.IsDestroyed)
        return;
      ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
      this.game.AddDamageParticles(actor.ActorType, actor.WorldPosition, ParticleType.ExplosionFront, actorDataXml.ExplosionScale, 2f, Vector2.Zero, (int) ((double) actorDataXml.ExplosionScale * 20.0));
      this.TakeDamage(actor, (int) ((double) actorDataXml.SmartBombDamage * (double) this.game.DifficultyFactor) + 1);
    }

    public override void TakeDamage(Actor actor, int damagePoints)
    {
      if (actor.IsDestroyed)
        return;
      int num = (actor.Hitpoints - 1) % 2000;
      actor.Hitpoints -= damagePoints;
      if (actor.Hitpoints <= 0)
      {
        ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
        this.game.Sound.PlaySound(actorDataXml.Hitpoints >= 200 ? SoundEffectType.ShipDestroyedBig : SoundEffectType.ShipDestroyedSmall, actorDataXml.Hitpoints < 2 ? 1f : MathHelper.Lerp(0.1f, 1f, (float) Math.Min(1000, actorDataXml.Hitpoints) / 1000f));
        this.game.UpdateScore(Math.Max(actor.Score, 100));
        this.game.DestroyActor(actor);
      }
      else
      {
        if ((actor.Hitpoints - 1) % 2000 <= num)
          return;
        this.game.Sound.PlaySound(SoundEffectType.UnderExplosion);
      }
    }
  }
}
