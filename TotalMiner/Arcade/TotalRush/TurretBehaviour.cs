// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.TurretBehaviour
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class TurretBehaviour : EnemyBehaviour
  {
    public TurretBehaviour(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
      : base(game)
    {
    }

    public override void Update(Actor actor)
    {
      if (!actor.IsDestroyed)
      {
        if ((double) actor.RotationSpeed != 0.0)
        {
          actor.Rotation += actor.RotationSpeed;
          actor.Aim = MyMathHelper.RotateVector2ByAngle(new Vector2(0.0f, -1f), actor.Rotation);
        }
        this.UpdateWorldData(actor);
        this.FireWeapons(actor);
        this.CheckForCollisionWithPlayer(actor);
      }
      else
      {
        this.UpdateDestroyed(actor);
        this.UpdateWorldData(actor);
      }
    }
  }
}
