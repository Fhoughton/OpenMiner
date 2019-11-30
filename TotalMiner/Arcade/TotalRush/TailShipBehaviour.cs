// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.TailShipBehaviour
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class TailShipBehaviour : ActorBehaviour
  {
    public static float GetTailShipSpace(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
    {
      ActorDataXML actorDataXml = game.ActorData[2];
      return (float) ((double) actorDataXml.SrcRect.Height * (double) actorDataXml.Scale + 3.0);
    }

    public static Vector2 GetTailShipTargetPosition(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game, Actor actor)
    {
      Vector2 vector2 = MyMathHelper.RotateVector2ByAngle(new Vector2(0.0f, -1f), actor.Parent.Rotation);
      float tailShipSpace = TailShipBehaviour.GetTailShipSpace(game);
      return vector2 * new Vector2((float) ((double) actor.TailShipID * (double) tailShipSpace + 8.0), (float) ((double) -actor.TailShipID * (double) tailShipSpace - 8.0));
    }

    public TailShipBehaviour(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
      : base(game)
    {
    }

    public override void Update(Actor actor)
    {
      if (!actor.IsDestroyed)
      {
        Vector2 vector2_1 = TailShipBehaviour.GetTailShipTargetPosition(this.game, actor) - actor.Position;
        if ((double) vector2_1.X != 0.0 || (double) vector2_1.Y != 0.0)
        {
          Vector2 vector2_2 = Vector2.Normalize(vector2_1);
          actor.Velocity = vector2_2 * actor.Speed * 0.75f;
          actor.Position.X += actor.Velocity.X;
          actor.Position.Y += actor.Velocity.Y;
        }
        this.UpdateWorldData(actor);
        this.FireWeapons(actor);
      }
      else
      {
        this.UpdateDestroyed(actor);
        this.UpdateWorldData(actor);
      }
    }

    protected override Actor GetWeaponsTarget()
    {
      return (Actor) null;
    }

    public override void OnSmartBombActivated(Actor actor, Actor player)
    {
      if (actor.Parent != player || actor.TailShipID != player.TailShipCount)
        return;
      this.game.DestroyActor(actor);
      --player.TailShipCount;
    }
  }
}
