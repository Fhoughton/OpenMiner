// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ActorEventArgs
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal class ActorEventArgs
  {
    public Actor Actor;
    public Actor Target;
    public Item Weapon;

    public ActorEventArgs(Actor actor, Actor target, Item weapon)
    {
      this.Actor = actor;
      this.Target = target;
      this.Weapon = weapon;
    }
  }
}
