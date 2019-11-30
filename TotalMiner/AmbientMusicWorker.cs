// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AmbientMusicWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Audio;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class AmbientMusicWorker : IThreadWorkItem
  {
    public static string[] Cues = new string[43]
    {
      "Air Prelude",
      "Celtic Impulse",
      "Cipher2",
      "Clean Soul",
      "Cold Funk",
      "Colorless Aura",
      "Crossing the Divide",
      "Danse Macabre",
      "Darkness is Coming",
      "Decisions",
      "Deliberate Thought",
      "Desert City",
      "Double Drift",
      "Drums of the Deep",
      "Eastern Thought",
      "Finding Movement",
      "Folk Round",
      "Frozen Star",
      "Guiton Sketch",
      "Halls of the Undead",
      "Impromptu in Blue",
      "Laid Back Guitars",
      "Living Voyage",
      "Long Road Ahead",
      "Lost Frontier",
      "Music for Manatees",
      "Opium",
      "Our Story Begins",
      "Overheat",
      "Pippin the Hunchback",
      "Rumination",
      "Sancho Panza gets a Latte",
      "Side_Path",
      "Skye Cuillin",
      "Summer Day",
      "Suonatore di Liuto",
      "Supernatural",
      "Tabuk",
      "Teller of the Tales",
      "The Path of the Goblin King",
      "Tranquility Base",
      "Virtutes Instrumenti",
      "Water Lily"
    };
    private List<string> songs = new List<string>();
    private PcgRandom random;
    private Cue cue;
    private List<int> order;
    private int index;

    public string Name
    {
      get
      {
        return nameof (AmbientMusicWorker);
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

    public AmbientMusicWorker()
    {
      this.random = new PcgRandom(new Random().Next());
      this.ResetOrder();
    }

    public void UnloadContent()
    {
      if (this.cue == null || this.cue.IsPlaying)
        return;
      this.cue.Dispose();
    }

    public void Update()
    {
      try
      {
        this.UpdateCore();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(105, ex);
      }
    }

    private void UpdateCore()
    {
      if (Globals1.IsCuePlaying(this.cue) || Globals1.IsCuePlaying(CoreGlobals.AudioManager.CurrentCue))
        return;
      if (this.cue != null && !this.cue.IsDisposed)
        this.cue.Dispose();
      CoreGlobals.AudioManager.PlaySong(this.GetNextAssetToPlay(), out this.cue);
    }

    private string GetNextAssetToPlay()
    {
      if (this.index >= this.order.Count)
        this.ResetOrder();
      return AmbientMusicWorker.Cues[this.order[this.index++]];
    }

    public void Reshuffle()
    {
      this.ResetOrder();
      if (this.cue != null)
      {
        this.cue.Dispose();
        this.cue = (Cue) null;
      }
      if (CoreGlobals.AudioManager.CurrentCue == null)
        return;
      CoreGlobals.AudioManager.CurrentCue.Dispose();
    }

    private void ResetOrder()
    {
      if (this.order == null)
        this.order = new List<int>(AmbientMusicWorker.Cues.Length);
      this.order.Clear();
      for (int index1 = 0; index1 < AmbientMusicWorker.Cues.Length; ++index1)
      {
label_3:
        int num = this.random.Next(AmbientMusicWorker.Cues.Length);
        for (int index2 = 0; index2 < index1; ++index2)
        {
          if (this.order[index2] == num)
            goto label_3;
        }
        this.order.Add(num);
      }
      this.index = 0;
    }
  }
}
