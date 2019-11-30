// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GamerMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Screens
{
  internal class GamerMenuEntry : BlockMenuEntry
  {
    public NetworkGamer Gamer;

    public GamerMenuEntry(BlockMenuScreen screen, NetworkGamer gamer)
      : base(screen, "Gamer: " + gamer.Gamertag)
    {
      this.Gamer = gamer;
      this.ColorHighlighted = Color.DarkGray;
    }
  }
}
