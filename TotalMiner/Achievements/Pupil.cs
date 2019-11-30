// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Pupil
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Pupil : Unlockable
  {
    public Pupil(Player player)
      : base(player, ActorType.Pupil, "Read every How To instruction.", (GameMode[]) null, (GameDifficulty[]) null, (NetworkSessionType[]) null)
    {
    }

    protected override void HookEvents()
    {
      this.player.ReadHowTo += new IntEventHandler(this.OnReadHowTo);
    }

    protected override void UnhookEvents()
    {
      this.player.ReadHowTo -= new IntEventHandler(this.OnReadHowTo);
    }

    public override bool IsUnlocked
    {
      get
      {
        foreach (bool[] flagArray in this.player.UnlockData.PupilHowToRead)
        {
          foreach (bool flag in flagArray)
          {
            if (!flag)
              return false;
          }
        }
        return true;
      }
    }

    private void OnReadHowTo(object sender, IntEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      this.player.UnlockData.PupilHowToRead[e.Value >> 16][e.Value & (int) ushort.MaxValue] = true;
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
        int num = 0;
        foreach (bool[] flagArray in this.player.UnlockData.PupilHowToRead)
        {
          foreach (bool flag in flagArray)
          {
            if (!flag)
              ++num;
          }
        }
        list.Add(string.Format("How To's not read: {0}", (object) num));
        return list;
      }
    }
  }
}
