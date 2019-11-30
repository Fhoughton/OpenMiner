// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveData
  {
    public Map Map;
    public SaveMapHead Header;
    public SaveGameState GameState;
    public List<SavePlayerState> PlayerState;
    public SaveArcadeState ArcadeState;
    public SaveRatingState Ratings;
    public GameSettings GameSettings;
    public List<string> SignTextCache;
    public List<CustomArray<SaveDataBlock>> Changes;
    public bool JustConverted18;
  }
}
