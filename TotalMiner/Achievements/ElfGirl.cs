// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.ElfGirl
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Achievements
{
  internal class ElfGirl : Unlockable
  {
    public ElfGirl(Player player)
      : base(player, ActorType.ElfGirl, (string) null, (GameMode[]) null, (GameDifficulty[]) null, (NetworkSessionType[]) null)
    {
    }

    public override bool IsUnlocked
    {
      get
      {
        return true;
      }
    }

    public override bool IsNPC
    {
      get
      {
        return true;
      }
    }
  }
}
