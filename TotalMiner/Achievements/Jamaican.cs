// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Jamaican
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Jamaican : Unlockable
  {
    public Jamaican(Player player)
      : base(player, ActorType.Jamaican, "Use the Rasta block with the\nCreative Fill feature.", new GameMode[1]
      {
        GameMode.Creative
      }, new GameDifficulty[4]
      {
        GameDifficulty.Peaceful,
        GameDifficulty.Easy,
        GameDifficulty.Normal,
        GameDifficulty.Legendary
      }, new NetworkSessionType[3]
      {
        NetworkSessionType.Local,
        NetworkSessionType.SystemLink,
        NetworkSessionType.PlayerMatch
      })
    {
    }

    protected override void HookEvents()
    {
      this.player.CreativeFill += new BlockEventHandler(this.OnCreativeFill);
    }

    protected override void UnhookEvents()
    {
      this.player.CreativeFill -= new BlockEventHandler(this.OnCreativeFill);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.JamaicanUnlocked;
      }
    }

    private void OnCreativeFill(object sender, BlockEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.BlockID != Block.Rasta)
        return;
      this.player.UnlockData.JamaicanUnlocked = true;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    public override bool HasProgress
    {
      get
      {
        return true;
      }
    }

    public override List<string> ProgressList
    {
      get
      {
        List<string> list = new List<string>();
        this.AddReqsMetProgress(list);
        list.Add(string.Format("Completed a Creative Fill operation using the Rasta block: {0}", (object) this.player.UnlockData.JamaicanUnlocked));
        return list;
      }
    }
  }
}
