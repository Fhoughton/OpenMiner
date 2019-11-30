// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.HealthEffect
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.API;
using System;

namespace StudioForge.TotalMiner
{
  public class HealthEffect : CharacterEffect
  {
    public int Points;

    protected override bool UpdateCore(ITMActor receiver, ITMActor applier)
    {
      if (!receiver.IsDeadOrInactiveOrDisabled)
      {
        if (this.Points > 0)
        {
          receiver.Health = Math.Min(receiver.Health + (float) this.Points, receiver.MaxHealth);
        }
        else
        {
          double damageAndDisplay = (double) receiver.TakeDamageAndDisplay(DamageType.Effect, (float) -this.Points, Vector3.Zero, applier, Item.None, SkillType.None);
        }
      }
      return true;
    }
  }
}
