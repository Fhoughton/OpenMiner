// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GlobalGameOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GlobalGameOptionsScreen : BlockMenuScreen
  {
    private GlobalGamerSettings gamerSettings;
    private GameSettings settings;
    private SliderValue soundEffectVolume;
    private SliderValue musicVolume;

    public GlobalGameOptionsScreen(PlayerIndex playerIndex)
      : base("Global Game Options", (Player) null)
    {
      this.gamerSettings = Globals2.GamertagData.GetGlobalGamerSettings(playerIndex);
      this.settings = this.gamerSettings.GameSettings;
      this.soundEffectVolume = new SliderValue()
      {
        Value = this.settings.SoundVolume,
        Range = 1f
      };
      this.musicVolume = new SliderValue()
      {
        Value = this.settings.MusicVolume,
        Range = 1f
      };
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Graphics Options"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Notification Options"));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, this.player, "Sound FX Volume: ", this.soundEffectVolume, 264));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, this.player, "Music Volume: ", this.musicVolume, 264));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ScreenManager.AddScreen((GameScreen) new GlobalGraphicsOptionsScreen(this.ControllingPlayer.Value), this.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int index3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ScreenManager.AddScreen((GameScreen) new GameNotificationOptionsScreen(this.settings), this.ControllingPlayer));
      blockMenuEntryList1[index3].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.soundEffectVolume.Value = MathHelper.Clamp(this.soundEffectVolume.Value - 0.05f, 0.0f, 1f);
        this.settings.SoundVolume = this.soundEffectVolume.Value;
        CoreGlobals.AudioManager.SoundVolume = this.soundEffectVolume.Value;
        TotalMinerGame.Instance.AudioManagerFiles.SoundVolume = CoreGlobals.AudioManager.SoundVolume;
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index4 = index3;
      int index5 = index4 + 1;
      blockMenuEntryList4[index4].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.soundEffectVolume.Value = MathHelper.Clamp(this.soundEffectVolume.Value + 0.05f, 0.0f, 1f);
        this.settings.SoundVolume = this.soundEffectVolume.Value;
        CoreGlobals.AudioManager.SoundVolume = this.soundEffectVolume.Value;
        TotalMinerGame.Instance.AudioManagerFiles.SoundVolume = CoreGlobals.AudioManager.SoundVolume;
      });
      blockMenuEntryList1[index5].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.musicVolume.Value = MathHelper.Clamp(this.musicVolume.Value - 0.05f, 0.0f, 1f);
        this.settings.MusicVolume = this.musicVolume.Value;
        CoreGlobals.AudioManager.MusicVolume = this.musicVolume.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index6 = index5;
      int num3 = index6 + 1;
      blockMenuEntryList5[index6].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.musicVolume.Value = MathHelper.Clamp(this.musicVolume.Value + 0.05f, 0.0f, 1f);
        this.settings.MusicVolume = this.musicVolume.Value;
        CoreGlobals.AudioManager.MusicVolume = this.musicVolume.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index7 = num3;
      int num4 = index7 + 1;
      blockMenuEntryList6[index7].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        int num5;
        if ((num5 = (int) (this.settings.AutoSave + 1)) > 3)
          num5 = 0;
        this.settings.AutoSave = (AutoSaveSetting) num5;
        this.ResetToggleItems();
      });
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[4].Text = "Auto Save: " + Utils.InsertSpacesBeforeCapitals(this.settings.AutoSave.ToString());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 574;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
