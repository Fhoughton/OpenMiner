// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.AmbientSoundsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class AmbientSoundsScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private AmbientSoundBlock block;
    private SliderValue volume;
    private SliderValue distance;
    private SliderValue loopDelay;
    private bool notPlayingMessage;

    private string LoopDelayText
    {
      get
      {
        return "Loop: " + (this.block.LoopDelayIndex == (byte) 0 ? "None" : AmbientSoundWorker.LoopDelays[(int) this.block.LoopDelayIndex].ToString() + " seconds");
      }
    }

    private string TimeText
    {
      get
      {
        return "Time: " + (this.block.DayOrNight == DayOrNight.Day ? "Day time" : (this.block.DayOrNight == DayOrNight.Night ? "Night time" : "Always"));
      }
    }

    private string PowerText
    {
      get
      {
        return "Requires Power: " + (this.block.RequiresPower ? "Yes" : "No");
      }
    }

    public AmbientSoundsScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Ambient Sound", player)
    {
      AmbientSoundsScreen ambientSoundsScreen = this;
      this.instance = instance;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.AmbientSoundBlock, UpdateBlockMethod.Player, this.PlayerID, true) as AmbientSoundBlock;
      this.volume = new SliderValue()
      {
        Value = this.block.Volume,
        Range = 1f
      };
      this.distance = new SliderValue()
      {
        Value = (float) this.block.Distance / 100f,
        Range = 1f
      };
      this.loopDelay = new SliderValue()
      {
        Value = (float) this.block.LoopDelayIndex / (float) (AmbientSoundWorker.LoopDelays.Length - 1),
        Range = 1f
      };
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Sound: " + Globals1.AmbientSoundData[this.block.SoundID].Name));
      blockMenuEntryList.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Distance: " + this.block.Distance.ToString() + " blocks", this.distance, 300));
      blockMenuEntryList.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, this.LoopDelayText, this.loopDelay, 300));
      blockMenuEntryList.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Volume: ", this.volume, 300, 12));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TimeText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.PowerText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        List<string> data = new List<string>(Globals1.AmbientSoundData.Length);
        foreach (AmbientSoundXML ambientSoundXml in Globals1.AmbientSoundData)
        {
          if (ambientSoundXml.IsValid && !ambientSoundXml.Name.Equals("none", StringComparison.OrdinalIgnoreCase))
            data.Add(ambientSoundXml.Name);
        }
        data.Sort();
        data.Insert(0, "None");
        ambientSoundsScreen.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(player, data, new ListBoxScreen.OnMenuItemSelected(ambientSoundsScreen.OnSoundSelected), false), ambientSoundsScreen.ControllingPlayer);
      });
      blockMenuEntryList[1].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ambientSoundsScreen.distance.Value = MathHelper.Clamp(ambientSoundsScreen.distance.Value - 0.05f, 0.05f, 1f);
        ambientSoundsScreen.block.Distance = (ushort) ((double) ambientSoundsScreen.distance.Value * 100.0);
        ambientSoundsScreen.MenuEntries[ambientSoundsScreen.selectedEntry].Text = "Distance: " + ambientSoundsScreen.block.Distance.ToString() + " blocks";
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[1].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ambientSoundsScreen.distance.Value = MathHelper.Clamp(ambientSoundsScreen.distance.Value + 0.05f, 0.05f, 1f);
        ambientSoundsScreen.block.Distance = (ushort) ((double) ambientSoundsScreen.distance.Value * 100.0);
        ambientSoundsScreen.MenuEntries[ambientSoundsScreen.selectedEntry].Text = "Distance: " + ambientSoundsScreen.block.Distance.ToString() + " blocks";
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[2].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        int num = AmbientSoundWorker.LoopDelays.Length - 1;
        ambientSoundsScreen.loopDelay.Value = MathHelper.Clamp(ambientSoundsScreen.loopDelay.Value - 1f / (float) num, 0.0f, 1f);
        ambientSoundsScreen.block.LoopDelayIndex = (byte) ((double) ambientSoundsScreen.loopDelay.Value * (double) num);
        ambientSoundsScreen.MenuEntries[ambientSoundsScreen.selectedEntry].Text = ambientSoundsScreen.LoopDelayText;
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[2].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        int num = AmbientSoundWorker.LoopDelays.Length - 1;
        ambientSoundsScreen.loopDelay.Value = MathHelper.Clamp(ambientSoundsScreen.loopDelay.Value + 1f / (float) num, 0.0f, 1f);
        ambientSoundsScreen.block.LoopDelayIndex = (byte) ((double) ambientSoundsScreen.loopDelay.Value * (double) num);
        ambientSoundsScreen.MenuEntries[ambientSoundsScreen.selectedEntry].Text = ambientSoundsScreen.LoopDelayText;
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[3].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ambientSoundsScreen.volume.Value = MathHelper.Clamp(ambientSoundsScreen.volume.Value - 0.1f, 0.0f, 1f);
        ambientSoundsScreen.block.Volume = ambientSoundsScreen.volume.Value;
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[3].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ambientSoundsScreen.volume.Value = MathHelper.Clamp(ambientSoundsScreen.volume.Value + 0.1f, 0.0f, 1f);
        ambientSoundsScreen.block.Volume = ambientSoundsScreen.volume.Value;
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ambientSoundsScreen.block.DayOrNight = ambientSoundsScreen.block.DayOrNight != DayOrNight.None ? (ambientSoundsScreen.block.DayOrNight != DayOrNight.Day ? DayOrNight.None : DayOrNight.Night) : DayOrNight.Day;
        ambientSoundsScreen.MenuEntries[ambientSoundsScreen.selectedEntry].Text = ambientSoundsScreen.TimeText;
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ambientSoundsScreen.block.RequiresPower = !ambientSoundsScreen.block.RequiresPower;
        ambientSoundsScreen.MenuEntries[ambientSoundsScreen.selectedEntry].Text = ambientSoundsScreen.PowerText;
        instance.AmbientSoundManager.SetBlock(ambientSoundsScreen.block);
      });
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      bool flag = player.HasPermission(Permissions.Creative);
      this.MenuEntries[0].IsEnabled = flag;
      this.MenuEntries[1].IsEnabled = flag;
      this.MenuEntries[2].IsEnabled = flag;
      this.MenuEntries[3].IsEnabled = flag;
      this.MenuEntries[4].IsEnabled = flag;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 478;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void ResetMenuRect(Rectangle rect)
    {
      if (this.block.DisplayNotPlayingMessage)
        rect.Height += 96;
      else if (this.notPlayingMessage)
        rect.Height -= 96;
      this.notPlayingMessage = this.block.DisplayNotPlayingMessage;
      base.ResetMenuRect(rect);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, this.block.SoundID == 0);
    }

    private bool OnSoundSelected(MenuEntry sound)
    {
      if (sound != null)
      {
        int index = 0;
        while (index < Globals1.AmbientSoundData.Length && (!Globals1.AmbientSoundData[index].IsValid || !(Globals1.AmbientSoundData[index].Name == sound.Text)))
          ++index;
        if (index >= Globals1.AmbientSoundData.Length)
          index = 0;
        this.block.SoundID = index;
        this.instance.AmbientSoundManager.SetBlock(this.block);
        this.MenuEntries[0].Text = "Sound: " + Globals1.AmbientSoundData[this.block.SoundID].Name;
      }
      return true;
    }

    protected override void DrawCore()
    {
      if (this.notPlayingMessage != this.block.DisplayNotPlayingMessage)
        this.ResetMenuRect(this.MenuRect);
      base.DrawCore();
    }

    protected override void DrawMenuExtra()
    {
      base.DrawMenuExtra();
      if (!this.block.DisplayNotPlayingMessage)
        return;
      this.SpriteBatch.DrawString(this.Font, "This sound is not playing because " + (object) 5 + "\nsounds are already playing in this area.", new Vector2((float) (this.MenuRect.X + 42), (float) (this.MenuRect.Y + this.MenuRect.Height - 80)), Color.Orange, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
