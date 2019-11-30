// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.QuestEditScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class QuestEditScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private bool oldCombat;
    private GameDifficulty oldDifficulty;
    private SliderValue soundEffectVolume;
    private SliderValue musicVolume;
    private string questName;

    public QuestEditScreen(GameInstance instance, Player player, string questName)
      : base("Edit Quest", player)
    {
      QuestEditScreen questEditScreen = this;
      this.instance = instance;
      this.questName = questName;
      this.soundEffectVolume = new SliderValue()
      {
        Value = CoreGlobals.AudioManager.SoundVolume,
        Range = 1f
      };
      this.musicVolume = new SliderValue()
      {
        Value = CoreGlobals.AudioManager.MusicVolume,
        Range = 1f
      };
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Texture Packs"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Graphics Options"));
      blockMenuEntryList.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Sound FX Volume: ", this.soundEffectVolume, 240));
      blockMenuEntryList.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Music Volume: ", this.musicVolume, 240));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        questEditScreen.ScreenManager.AddScreen((GameScreen) new TexturePackMenuScreen(instance, player), questEditScreen.ControllingPlayer);
        questEditScreen.ExitScreen();
      });
      blockMenuEntryList[1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => questEditScreen.ScreenManager.AddScreen((GameScreen) new GameGraphicsOptionsScreen(instance, player), questEditScreen.ControllingPlayer));
      blockMenuEntryList[2].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.soundEffectVolume.Value = MathHelper.Clamp(this.soundEffectVolume.Value - 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.SoundVolume = this.soundEffectVolume.Value;
      });
      blockMenuEntryList[2].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.soundEffectVolume.Value = MathHelper.Clamp(this.soundEffectVolume.Value + 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.SoundVolume = this.soundEffectVolume.Value;
      });
      blockMenuEntryList[3].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.musicVolume.Value = MathHelper.Clamp(this.musicVolume.Value - 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.MusicVolume = this.musicVolume.Value;
      });
      blockMenuEntryList[3].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.musicVolume.Value = MathHelper.Clamp(this.musicVolume.Value + 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.MusicVolume = this.musicVolume.Value;
      });
      blockMenuEntryList[4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        int num;
        switch (Globals2.GameProperties.SaveGame.Header.GameDifficulty)
        {
          case GameDifficulty.Peaceful:
            num = 3;
            break;
          case GameDifficulty.Legendary:
            return;
          case GameDifficulty.Easy:
            num = 1;
            break;
          default:
            num = Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Survival ? 3 : 0;
            break;
        }
        GameDifficulty gameDifficulty = (GameDifficulty) num;
        if (gameDifficulty == GameDifficulty.Peaceful && instance != null)
        {
          string canEnablePeaceful = instance.CanEnablePeaceful;
          if (canEnablePeaceful != null)
          {
            questEditScreen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot enable Peaceful mode because " + canEnablePeaceful, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), questEditScreen.ControllingPlayer);
            return;
          }
        }
        Globals2.GameProperties.SaveGame.Header.GameDifficulty = gameDifficulty;
        Globals2.GameProperties.SaveGame.Header.CombatEnabled = gameDifficulty != GameDifficulty.Peaceful;
        questEditScreen.ResetToggleItems();
      });
      blockMenuEntryList[5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (instance.IsCombatEnabled && instance != null)
        {
          string canDisableCombat = instance.CanDisableCombat;
          if (canDisableCombat != null)
          {
            questEditScreen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot disable Combat because " + canDisableCombat, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), questEditScreen.ControllingPlayer);
            return;
          }
        }
        Globals2.GameProperties.SaveGame.Header.CombatEnabled = !Globals2.GameProperties.SaveGame.Header.CombatEnabled;
        questEditScreen.ResetToggleItems();
      });
      blockMenuEntryList[6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        int num;
        if ((num = (int) (Globals2.GameSettings.AutoSave + 1)) == 4)
          num = 0;
        Globals2.GameSettings.AutoSave = (AutoSaveSetting) num;
        questEditScreen.ResetToggleItems();
        instance.OnAutoSaveChanged();
      });
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.MenuEntries[4].IsEnabled = !instance.IsPeacefulMode && !instance.IsLegendaryDifficulty && player.IsAdmin;
      this.MenuEntries[5].IsEnabled = instance.IsCreativeMode && player.IsAdmin;
      this.MenuEntries[6].IsEnabled = instance.IsHost && !Globals2.GameProperties.IsSystemMap;
      this.ResetToggleItems();
      this.oldCombat = Globals2.GameProperties.SaveGame.Header.CombatEnabled;
      this.oldDifficulty = Globals2.GameProperties.SaveGame.Header.GameDifficulty;
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[4].Text = "Game Difficulty: " + Globals2.GameProperties.SaveGame.Header.GameDifficulty.ToString();
      this.MenuEntries[5].Text = this.instance.IsCombatEnabled ? "Toggle Combat: On " : "Toggle Combat: Off";
      this.MenuEntries[6].Text = "Auto Save: " + Utils.InsertSpacesBeforeCapitals(Globals2.GameSettings.AutoSave.ToString());
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

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      bool flag = false;
      if (this.oldCombat != Globals2.GameProperties.SaveGame.Header.CombatEnabled)
      {
        flag = true;
        this.instance.AddNotification(this.player, " has " + (this.oldCombat ? "disabled" : "enabled") + " Combat", NotifyRecipient.Remote);
      }
      if (this.oldDifficulty != Globals2.GameProperties.SaveGame.Header.GameDifficulty)
      {
        flag = true;
        this.instance.AddNotification(this.player, " has changed Difficulty to " + Globals2.GameProperties.SaveGame.Header.GameDifficulty.ToString(), NotifyRecipient.Remote);
      }
      if (!flag)
        return;
      NetworkManager.Instance.SendGamePropertiesNonVital();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
