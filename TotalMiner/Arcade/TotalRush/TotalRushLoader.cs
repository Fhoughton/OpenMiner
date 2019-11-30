// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.TotalRushLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class TotalRushLoader : IThreadWorkItem
  {
    private StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game;

    public string Name
    {
      get
      {
        return nameof (TotalRushLoader);
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

    public void Initialize(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game)
    {
      this.game = game;
    }

    public void Update()
    {
      ActorDataXML[] actorData = Utils.Deserialize1<ActorDataXML[]>("Content\\Arcade\\TotalRush\\ActorData.xml");
      LevelDataXML[] levelData = Utils.Deserialize1<LevelDataXML[]>("Content\\Arcade\\TotalRush\\LevelData.xml");
      ParticleDataXML[] particleData = Utils.Deserialize1<ParticleDataXML[]>("Content\\Arcade\\TotalRush\\ParticleData.xml");
      this.game.OnDataLoaded(actorData, particleData, levelData);
    }
  }
}
