// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.GamePropertiesEventArgs
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner.Net
{
  internal class GamePropertiesEventArgs : EventArgs
  {
    public GameProperties GameProperties;

    public GamePropertiesEventArgs(GameProperties gameProperties)
    {
      this.GameProperties = gameProperties;
    }
  }
}
