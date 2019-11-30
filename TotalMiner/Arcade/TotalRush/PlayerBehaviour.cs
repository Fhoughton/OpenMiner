// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.PlayerBehaviour
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class PlayerBehaviour : ActorBehaviour
  {
    public PlayerBehaviour(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
      : base(game)
    {
    }

    public override void Update(Actor actor)
    {
      actor.Position.X += actor.Velocity.X;
      actor.Position.Y += actor.Velocity.Y;
      actor.WorldPosition = actor.Parent != null ? actor.Parent.WorldPosition + actor.Position : actor.Position;
      Vector2 vector2 = MyMathHelper.RotateVector2ByAngle(actor.HitBoxOffset, actor.Rotation);
      actor.HitBoxCenter.X = actor.WorldPosition.X - vector2.X * actor.Scale;
      actor.HitBoxCenter.Y = actor.WorldPosition.Y + vector2.Y * actor.Scale;
      if (!actor.IsDestroyed)
      {
        if (!actor.IsVulnerable && (double) actor.Scale < 1.0)
        {
          actor.Scale += 0.004f;
          if ((double) actor.Scale >= 1.0)
          {
            actor.Scale = 1f;
            actor.IsVulnerable = true;
          }
        }
        this.UpdateExhaust(actor);
      }
      else
        this.UpdateDestroyed(actor);
    }

    protected override Actor GetWeaponsTarget()
    {
      return (Actor) null;
    }

    public override void DestroyActor(Actor actor)
    {
      this.game.Sound.PlaySound(SoundEffectType.ShipDestroyedBig);
      float num = actor.BulletPatterns.Array[0].BulletsPerSecond / 20f;
      for (int index = 0; (double) index < (double) num && this.game.UnusedActors.Count > 5; ++index)
      {
        Vector2 velocity = new Vector2((float) (this.game.Random.NextDouble() * 6.0 - 3.0), (float) (this.game.Random.NextDouble() * 6.0 - 3.0));
        this.game.SpawnPickup(ActorType.FirePowerPickup, actor.WorldPosition + velocity * 10f, velocity);
      }
      if (actor.TailShipCount > 0)
      {
        Actor actor1 = (Actor) null;
        for (LinkedListNode<Point> linkedListNode = this.game.UsedActors.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          Actor actor2 = this.game.Actors[linkedListNode.Value.X];
          if (actor2.ActorType == ActorType.TailShip && actor2.Parent == actor && actor2.TailShipID == 1)
          {
            actor1 = actor2;
            break;
          }
        }
        if (actor1 != null)
        {
          Actor actor2 = this.game.CreateActor((Actor) null, ActorType.Player, actor1.WorldPosition);
          if (actor2 != null)
          {
            actor2.TailShipCount = actor.TailShipCount - 1;
            actor2.IsVulnerable = false;
            actor2.Scale = this.game.ActorData[2].Scale;
            this.ClampPosition(actor2);
            this.game.UpdatePower(0);
            LinkedListNode<Point> next;
            for (LinkedListNode<Point> linkedListNode = this.game.UsedActors.First; linkedListNode != null; linkedListNode = next)
            {
              next = linkedListNode.Next;
              Actor actor3 = this.game.Actors[linkedListNode.Value.X];
              if (actor3.ActorType == ActorType.TailShip && actor3.Parent == actor)
              {
                if (actor3.TailShipID == 1)
                {
                  this.game.DestroyActor(actor3);
                }
                else
                {
                  actor3.Parent = actor2;
                  --actor3.TailShipID;
                }
              }
            }
          }
        }
      }
      base.DestroyActor(actor);
    }

    public void ActivateSmartBomb(Actor actor)
    {
      this.game.ClearSurroundingBullets(ActorType.EnemyBullet, actor.WorldPosition, 300f);
      LinkedListNode<Point> next;
      for (LinkedListNode<Point> linkedListNode = this.game.UsedActors.First; linkedListNode != null; linkedListNode = next)
      {
        next = linkedListNode.Next;
        Actor actor1 = this.game.Actors[linkedListNode.Value.X];
        this.game.Behaviours[(int) actor1.ActorType].OnSmartBombActivated(actor1, actor);
      }
    }
  }
}
