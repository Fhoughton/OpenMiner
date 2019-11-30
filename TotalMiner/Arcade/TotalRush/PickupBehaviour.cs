// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.PickupBehaviour
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class PickupBehaviour : ActorBehaviour
  {
    public PickupBehaviour(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
      : base(game)
    {
    }

    public override void Update(Actor actor)
    {
      if (!actor.IsDestroyed)
      {
        actor.Position.X += actor.Velocity.X + actor.Velocity2.X;
        actor.Position.Y += actor.Velocity.Y + actor.Velocity2.Y;
        actor.Velocity2.X *= 0.97f;
        if ((double) actor.Velocity2.X < 0.00999999977648258 && (double) actor.Velocity2.X > -0.00999999977648258)
          actor.Velocity2.X = 0.0f;
        actor.Velocity2.Y *= 0.97f;
        if ((double) actor.Velocity2.Y < 0.00999999977648258 && (double) actor.Velocity2.Y > -0.00999999977648258)
          actor.Velocity2.Y = 0.0f;
        actor.Rotation += actor.RotationSpeed;
        this.UpdateWorldData(actor);
        Actor actor1 = this.game.Actors[this.game.PlayerID];
        if (actor1.IsDestroyed)
          return;
        float num1 = Vector2.Distance(actor.WorldPosition, actor1.WorldPosition);
        if ((double) num1 <= (double) actor.HitBoxRadius)
        {
          this.OnPickup(actor, actor1);
          this.game.DestroyActor(actor);
        }
        else
        {
          float num2 = 25f / num1;
          actor.Velocity = Vector2.Normalize(actor1.WorldPosition - actor.WorldPosition) * num2;
        }
      }
      else
      {
        this.UpdateDestroyed(actor);
        this.UpdateWorldData(actor);
      }
    }

    private void OnPickup(Actor actor, Actor player)
    {
      ActorType actorType = actor.ActorType;
      if (actorType == ActorType.TailShipPickup && player.TailShipCount > 4)
        actorType = ActorType.FirePowerPickup;
      switch (actorType)
      {
        case ActorType.FirePowerPickup:
          if (this.game.IsPlayerMaxPower)
          {
            this.game.ClearSurroundingBullets(ActorType.EnemyBullet, player.WorldPosition, 120f);
            break;
          }
          this.game.UpdatePower(10);
          break;
        case ActorType.TailShipPickup:
          Actor actor1 = this.game.CreateActor(player, ActorType.TailShip, Vector2.Zero);
          actor1.TailShipID = ++player.TailShipCount;
          actor1.Position = TailShipBehaviour.GetTailShipTargetPosition(this.game, actor1);
          break;
      }
      this.game.Sound.PlaySound(SoundEffectType.Pickup);
    }
  }
}
