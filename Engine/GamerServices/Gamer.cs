// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GamerServices.Gamer
// Assembly: StudioForge.Engine.GamerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EA07B8F-6C00-417B-9E82-CD1E4EB140B6
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GamerServices.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.GamerServices
{
  public class Gamer
  {
    public object Tag;
    protected GamerStates gamerState;

    public GamerStates GamerState
    {
      get
      {
        return this.gamerState;
      }
    }

    public PlayerIndex PlayerIndex { get; private set; }

    public GamerID ID { get; private set; }

    public string Gamertag { get; private set; }

    public bool IsReady { get; set; }

    public bool IsTalking { get; protected set; }

    public Gamer(GamerID id, string gamerTag)
      : this(id, gamerTag, PlayerIndex.One)
    {
    }

    public Gamer(GamerID id, string gamerTag, PlayerIndex playerIndex)
    {
      this.ID = id;
      this.Gamertag = gamerTag;
      this.PlayerIndex = playerIndex;
    }

    public void AddGamerState(GamerStates states)
    {
      this.gamerState |= states;
    }
  }
}
