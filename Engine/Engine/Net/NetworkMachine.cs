// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.NetworkMachine
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Net
{
  public class NetworkMachine
  {
    public List<NetworkGamer> Gamers { get; private set; }

    public NetworkMachine(List<NetworkGamer> gamers)
    {
      this.Gamers = new List<NetworkGamer>((IEnumerable<NetworkGamer>) gamers);
    }

    public void RemoveFromSession()
    {
      throw new NotImplementedException();
    }
  }
}
