// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.ActorBehaviour
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal abstract class ActorBehaviour
  {
    private static Color[] ExhaustColor = new Color[5]
    {
      Color.Red,
      Color.Orange,
      Color.Yellow,
      Color.White,
      Color.Blue
    };
    protected StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game;

    protected ActorBehaviour(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
    {
      this.game = game;
    }

    public virtual void InitializeActor(Actor actor)
    {
      actor.IsAlive = true;
      this.game.SetActorDefaults(actor, actor.ActorType);
      this.InitializeChildren(actor);
    }

    protected void InitializeChildren(Actor actor)
    {
      ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
      if (actorDataXml.ChildTypes == null)
        return;
      for (int index = 0; index < actorDataXml.ChildTypes.Length; ++index)
        this.game.CreateActor(actor, actorDataXml.ChildTypes[index], actorDataXml.ChildOffsets[index]);
    }

    public virtual void Update(Actor actor)
    {
      if (actor.IsDestroyed)
        this.UpdateDestroyed(actor);
      this.UpdateWorldData(actor);
    }

    protected void UpdateWorldData(Actor actor)
    {
      actor.WorldPosition = actor.Parent != null ? actor.Parent.WorldPosition + actor.Position : actor.Position;
      actor.HitBoxCenter.X = actor.WorldPosition.X + actor.HitBoxOffset.X * actor.Scale;
      actor.HitBoxCenter.Y = actor.WorldPosition.Y + actor.HitBoxOffset.Y * actor.Scale;
    }

    protected void ClampPosition(Actor actor)
    {
      if ((double) actor.Position.X < 4.0)
        actor.Position.X = 4f;
      else if ((double) actor.Position.X >= (double) (this.game.ScreenSize.X - 4))
        actor.Position.X = (float) (this.game.ScreenSize.X - 4);
      if ((double) actor.Position.Y < 6.0)
      {
        actor.Position.Y = 6f;
      }
      else
      {
        if ((double) actor.Position.Y < (double) (this.game.ScreenSize.Y - 6))
          return;
        actor.Position.Y = (float) (this.game.ScreenSize.Y - 6);
      }
    }

    protected void UpdateExhaust(Actor actor)
    {
      if (-actor.ExhaustTimer > 0)
        return;
      ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
      int num = (int) ((double) actorDataXml.ExhaustSpread * (double) actorDataXml.Scale * (double) actor.Scale);
      Vector2 vector2 = MyMathHelper.RotateVector2ByAngle(new Vector2((float) (this.game.Random.Next(num * 2 + 1) - num), (float) actorDataXml.ExhaustOffsetY * actorDataXml.Scale * actor.Scale), actor.Rotation);
      vector2.X = -vector2.X;
      this.game.AddParticle(actor.Position + vector2, ParticleType.Exhaust, 1f, 0.3f, vector2 * 0.05f, ActorBehaviour.ExhaustColor[this.game.Random.Next(ActorBehaviour.ExhaustColor.Length)]);
      actor.ExhaustTimer = actorDataXml.ExhaustFrequency;
    }

    protected virtual void UpdateDestroyed(Actor actor)
    {
      actor.DestroyedAge -= 0.01666667f;
      if ((double) actor.DestroyedAge < 0.0)
      {
        actor.IsAlive = false;
      }
      else
      {
        ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
        actor.Scale = (float) ((double) actor.DestroyedAge / (double) actorDataXml.DestructionTime * 0.5 + 0.5);
        if (this.game.Random.Next(6) != 0)
          return;
        Vector2 worldPosition = actor.WorldPosition;
        worldPosition.X += (float) this.game.Random.Next(actorDataXml.SrcRect.Width) - (float) actorDataXml.SrcRect.Width * 0.5f;
        worldPosition.Y += (float) this.game.Random.Next(actorDataXml.SrcRect.Height) - (float) actorDataXml.SrcRect.Height * 0.5f;
        float explosionScale = actorDataXml.ExplosionScale;
        float scale = explosionScale + (float) (this.game.Random.NextDouble() * (double) explosionScale * 0.100000001490116 - (double) explosionScale * 0.0500000007450581);
        this.game.AddDamageParticles(actor.ActorType, worldPosition, ParticleType.ExplosionBack, scale, 0.8f, Vector2.Zero, (int) ((double) scale * 4.0));
        if (!actorDataXml.IsBoss || this.game.Random.Next(6) != 0)
          return;
        this.game.Sound.PlaySound(SoundEffectType.ShipDestroyedBig, (float) (this.game.Random.NextDouble() * 0.5 + 0.200000002980232));
      }
    }

    public void FireWeapons(Actor actor)
    {
      if (actor.BulletPatterns == null || actor.BulletPatterns.Count <= 0)
        return;
      Actor weaponsTarget = this.GetWeaponsTarget();
      if (weaponsTarget != null && weaponsTarget.IsDestroyed)
        return;
      for (int i = actor.BulletPatterns.Count - 1; i >= 0; --i)
      {
        BulletPattern pattern = actor.BulletPatterns.Array[i];
        if ((double) pattern.InitialDelay > 0.0)
        {
          pattern.InitialDelay -= 0.01666667f;
          actor.BulletPatterns.Array[i] = pattern;
          if ((double) pattern.InitialDelay > 0.0)
            continue;
        }
        if (pattern.HitPointsStart == 0 || actor.Hitpoints <= pattern.HitPointsStart)
        {
          if (pattern.HitPointsEnd != 0 && actor.Hitpoints < pattern.HitPointsEnd)
          {
            actor.BulletPatterns.RemoveAt(i);
          }
          else
          {
            float num1 = pattern.BulletsPerSecond / 60f;
            int num2 = (int) num1;
            pattern.BulletOverflow += num1 - (float) num2;
            if ((double) pattern.BulletOverflow > 1.0)
            {
              ++num2;
              --pattern.BulletOverflow;
            }
            for (int index = 0; index < num2 && this.game.UnusedBullets.Count > 0; ++index)
            {
              this.game.BulletPatterns[(int) pattern.PatternType].FireBullets(this.game, actor, actor.WorldPosition, weaponsTarget, ref pattern);
              if (--pattern.SoundCounter <= 0)
              {
                this.game.Sound.PlaySound(pattern.SoundID);
                pattern.SoundCounter = pattern.SoundInstanceFreq;
              }
            }
            actor.BulletPatterns.Array[i] = pattern;
          }
        }
      }
    }

    protected virtual Actor GetWeaponsTarget()
    {
      return this.game.Actors[this.game.PlayerID];
    }

    protected void CheckForCollisionWithPlayer(Actor actor)
    {
      Actor actor1 = this.game.Actors[this.game.PlayerID];
      if (!actor1.IsVulnerable || (double) Vector2.DistanceSquared(actor.HitBoxCenter, actor1.HitBoxCenter) > (double) actor.HitBoxRadius2 + (double) actor1.HitBoxRadius2)
        return;
      this.game.DestroyActor(actor1);
    }

    public virtual void DestroyActor(Actor actor)
    {
      if (!this.game.IsPlayerMaxPower || this.game.Random.Next(actor.DropChance) == 0)
        this.game.SpawnPickup(actor.DropType, actor.WorldPosition, Vector2.Zero);
      actor.IsVulnerable = false;
      actor.IsDestroyed = true;
      ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
      actor.DestroyedAge = actorDataXml.DestructionTime;
      actor.Velocity = Vector2.Zero;
      if (actor.DestroyedHandler != null)
        actor.DestroyedHandler(actor);
      this.DestroyChildren(actor);
    }

    protected void DestroyChildren(Actor actor)
    {
      if (actor.ChildCount <= 0)
        return;
      LinkedListNode<Point> next;
      for (LinkedListNode<Point> linkedListNode = this.game.UsedActors.First; linkedListNode != null; linkedListNode = next)
      {
        next = linkedListNode.Next;
        Actor actor1 = this.game.Actors[linkedListNode.Value.X];
        if (actor1.Parent == actor)
          this.game.DestroyActor(actor1);
      }
    }

    public virtual void OnDeactivate(Actor actor)
    {
    }

    public virtual void OnSmartBombActivated(Actor actor, Actor player)
    {
    }

    public virtual void TakeDamage(Actor actor, int damagePoints)
    {
    }
  }
}
