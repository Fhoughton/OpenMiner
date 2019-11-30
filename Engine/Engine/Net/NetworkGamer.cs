// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.NetworkGamer
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using StudioForge.Engine.GamerServices;
using System.Collections.Generic;

namespace StudioForge.Engine.Net
{
  public class NetworkGamer : Gamer
  {
    public bool IsGuest
    {
      get
      {
        return (this.gamerState & GamerStates.Guest) > (GamerStates) 0;
      }
    }

    public bool IsHost
    {
      get
      {
        return (this.gamerState & GamerStates.Host) > (GamerStates) 0;
      }
    }

    public bool IsLocal
    {
      get
      {
        return (this.gamerState & GamerStates.Local) > (GamerStates) 0;
      }
    }

    public NetworkMachine Machine { get; internal set; }

    public bool IsPrivateSlot { get; private set; }

    public bool HasLeftSession { get; private set; }

    public NetworkGamer(GamerID id, string gamerTag)
      : base(id, gamerTag)
    {
      this.Machine = new NetworkMachine(new List<NetworkGamer>()
      {
        this
      });
    }
  }
}
