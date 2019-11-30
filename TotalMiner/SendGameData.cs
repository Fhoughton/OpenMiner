// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SendGameData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Storage;

namespace StudioForge.TotalMiner
{
  internal class SendGameData : IThreadWorkItem
  {
    private NetworkGamer requester;
    private GameInstance instance;

    public string Name
    {
      get
      {
        return nameof (SendGameData);
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public void Initialize(GameInstance instance, NetworkGamer requester)
    {
      this.instance = instance;
      this.requester = requester;
    }

    public void Update()
    {
      byte[] gameData = new MapSaver().GetGameData(this.instance);
      NetworkManager.Instance.SendGameData(gameData, this.requester);
    }
  }
}
