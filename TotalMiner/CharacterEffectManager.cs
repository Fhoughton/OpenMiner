// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CharacterEffectManager
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public class CharacterEffectManager
  {
    private ITMActor parent;
    private ITMActor applier;
    private List<CharacterEffect> effects;

    public CharacterEffectManager(ITMActor parent, ITMActor applier)
    {
      this.parent = parent;
      this.applier = applier;
      this.effects = new List<CharacterEffect>();
    }

    public int EffectCount
    {
      get
      {
        lock (this.effects)
          return this.effects.Count;
      }
    }

    public void AddEffect(CharacterEffect effect)
    {
      lock (this.effects)
        this.effects.Add(effect);
    }

    public void DeleteEffect(string name)
    {
      lock (this.effects)
      {
        for (int index = this.effects.Count - 1; index >= 0; --index)
        {
          if (this.effects[index].Name == name)
            this.effects.RemoveAt(index);
        }
      }
    }

    public void DeleteEffect(CharacterEffect effect)
    {
      lock (this.effects)
        this.effects.Remove(effect);
    }

    public void DeleteAllEffects()
    {
      lock (this.effects)
        this.effects.Clear();
    }

    public void Update()
    {
      lock (this.effects)
      {
        for (int index = this.effects.Count - 1; index >= 0; --index)
        {
          CharacterEffect effect = this.effects[index];
          if (effect.History != null && !this.parent.HasHistory(effect.History))
          {
            this.effects.RemoveAt(index);
          }
          else
          {
            effect.Timer += Services.ElapsedTime;
            if ((double) effect.Timer >= (double) effect.Interval)
            {
              effect.Timer -= effect.Interval;
              if (!effect.Update(this.parent, this.applier))
              {
                this.effects.RemoveAt(index);
                continue;
              }
            }
            if ((double) effect.Duration > 0.0)
            {
              effect.Duration -= Services.ElapsedTime;
              if ((double) effect.Duration <= 0.0)
                this.effects.RemoveAt(index);
            }
          }
        }
      }
    }
  }
}
