// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GameOptionsScreen
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
  internal class GameOptionsScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private bool oldCombat;
    private bool oldClanProtection;
    private int oldCombatDiff;
    private GameDifficulty oldDifficulty;
    private SliderValue soundEffectVolume;
    private SliderValue musicVolume;
    private bool[] itemsEnabled;

    public GameOptionsScreen(GameInstance instance, Player player)
      : base("Options", player)
    {
      GameOptionsScreen gameOptionsScreen = this;
      this.instance = instance;
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
      this.itemsEnabled = new bool[Globals1.ItemData.Length];
      for (int index = 0; index < Globals1.ItemData.Length; ++index)
        this.itemsEnabled[index] = Globals1.ItemData[index].IsEnabled;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Graphics Options"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Notification Options"));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Sound FX Volume: ", this.soundEffectVolume, 264));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Music Volume: ", this.musicVolume, 264));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      if (!instance.IsAvatarDesigner)
      {
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Item Options"));
      }
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int index1 = 0;
      blockMenuEntryList1[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new TexturePackMenuScreen(instance, player), gameOptionsScreen.ControllingPlayer);
        gameOptionsScreen.ExitScreen();
      });
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index2 = index1;
      int num1 = index2 + 1;
      blockMenuEntryList2[index2].IsEnabled = player != null && player.IsGod || instance == null || !instance.IsAvatarDesigner;
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index3 = num1;
      int num2 = index3 + 1;
      blockMenuEntryList3[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new GameGraphicsOptionsScreen(instance, player), gameOptionsScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index4 = num2;
      int index5 = index4 + 1;
      blockMenuEntryList4[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ScreenManager.AddScreen((GameScreen) new GameNotificationOptionsScreen(Globals2.GameSettings), this.ControllingPlayer));
      blockMenuEntryList1[index5].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.soundEffectVolume.Value = MathHelper.Clamp(this.soundEffectVolume.Value - 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.SoundVolume = this.soundEffectVolume.Value;
        TotalMinerGame.Instance.AudioManagerFiles.SoundVolume = CoreGlobals.AudioManager.SoundVolume;
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index6 = index5;
      int index7 = index6 + 1;
      blockMenuEntryList5[index6].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.soundEffectVolume.Value = MathHelper.Clamp(this.soundEffectVolume.Value + 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.SoundVolume = this.soundEffectVolume.Value;
        TotalMinerGame.Instance.AudioManagerFiles.SoundVolume = CoreGlobals.AudioManager.SoundVolume;
      });
      blockMenuEntryList1[index7].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.musicVolume.Value = MathHelper.Clamp(this.musicVolume.Value - 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.MusicVolume = this.musicVolume.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index8 = index7;
      int num3 = index8 + 1;
      blockMenuEntryList6[index8].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.musicVolume.Value = MathHelper.Clamp(this.musicVolume.Value + 0.05f, 0.0f, 1f);
        CoreGlobals.AudioManager.MusicVolume = this.musicVolume.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index9 = num3;
      int index10 = index9 + 1;
      blockMenuEntryList7[index9].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        instance.ResetMusicShuffle();
        gameOptionsScreen.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(gameOptionsScreen.ControllingPlayer.Value));
      });
      if (!instance.IsAvatarDesigner)
      {
        blockMenuEntryList1[index10].IsEnabled = !instance.IsPeacefulMode && !instance.IsLegendaryDifficulty && player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
        int index11 = index10;
        int index12 = index11 + 1;
        blockMenuEntryList8[index11].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
        {
          int num4;
          switch (Globals2.GameProperties.SaveGame.Header.GameDifficulty)
          {
            case GameDifficulty.Peaceful:
              num4 = 3;
              break;
            case GameDifficulty.Legendary:
              return;
            case GameDifficulty.Easy:
              num4 = 1;
              break;
            default:
              num4 = Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Survival ? 3 : 0;
              break;
          }
          GameDifficulty gameDifficulty = (GameDifficulty) num4;
          if (gameDifficulty == GameDifficulty.Peaceful && instance != null)
          {
            string canEnablePeaceful = instance.CanEnablePeaceful;
            if (canEnablePeaceful != null)
            {
              gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot enable Peaceful mode because " + canEnablePeaceful, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), gameOptionsScreen.ControllingPlayer);
              return;
            }
          }
          Globals2.GameProperties.SaveGame.Header.GameDifficulty = gameDifficulty;
          Globals2.GameProperties.SaveGame.Header.CombatEnabled = gameDifficulty != GameDifficulty.Peaceful;
          gameOptionsScreen.ResetToggleItems();
        });
        blockMenuEntryList1[index12].IsEnabled = instance.IsCreativeMode && player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
        int index13 = index12;
        int index14 = index13 + 1;
        blockMenuEntryList9[index13].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
        {
          if (instance.IsCombatEnabled && instance != null)
          {
            string canDisableCombat = instance.CanDisableCombat;
            if (canDisableCombat != null)
            {
              gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot disable Combat because " + canDisableCombat, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), gameOptionsScreen.ControllingPlayer);
              return;
            }
          }
          Globals2.GameProperties.SaveGame.Header.CombatEnabled = !Globals2.GameProperties.SaveGame.Header.CombatEnabled;
          gameOptionsScreen.ResetToggleItems();
        });
        blockMenuEntryList1[index14].IsEnabled = player.IsAdmin && Globals2.GameProperties.SaveGame.Header.SkillsLocal;
        List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
        int index15 = index14;
        int index16 = index15 + 1;
        blockMenuEntryList10[index15].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(gameOptionsScreen.OnXPMultiplierEntered), Globals2.GameProperties.SaveGame.Header.XPMultiplier, true, false), new PlayerIndex?(gameOptionsScreen.ControllingPlayer.Value)));
        blockMenuEntryList1[index16].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
        int index17 = index16;
        int index18 = index17 + 1;
        blockMenuEntryList11[index17].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(gameOptionsScreen.OnCombatLevelDifferenceEntered), (int) Globals2.GameProperties.SaveGame.Header.CombatLevelDifference, false), gameOptionsScreen.ControllingPlayer));
        blockMenuEntryList1[index18].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
        int index19 = index18;
        int index20 = index19 + 1;
        blockMenuEntryList12[index19].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
        {
          Globals2.GameProperties.SaveGame.Header.ClanProtection = !Globals2.GameProperties.SaveGame.Header.ClanProtection;
          this.ResetToggleItems();
        });
        blockMenuEntryList1[index20].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList13 = blockMenuEntryList1;
        int index21 = index20;
        index10 = index21 + 1;
        blockMenuEntryList13[index21].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => gameOptionsScreen.ScreenManager.AddScreen((GameScreen) new ItemOptionsScreen(instance, player, Block.ItemShop), gameOptionsScreen.ControllingPlayer));
      }
      blockMenuEntryList1[index10].IsEnabled = instance.IsHost && !Globals2.GameProperties.IsSystemMap;
      List<BlockMenuEntry> blockMenuEntryList14 = blockMenuEntryList1;
      int index22 = index10;
      int num5 = index22 + 1;
      blockMenuEntryList14[index22].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        int num4;
        if ((num4 = (int) (Globals2.GameSettings.AutoSave + 1)) == 4)
          num4 = 0;
        Globals2.GameSettings.AutoSave = (AutoSaveSetting) num4;
        gameOptionsScreen.ResetToggleItems();
        instance.OnAutoSaveChanged();
      });
      List<BlockMenuEntry> blockMenuEntryList15 = blockMenuEntryList1;
      int index23 = num5;
      int num6 = index23 + 1;
      blockMenuEntryList15[index23].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        Globals2.UseOldMenu = !Globals2.UseOldMenu;
        this.ResetToggleItems();
        TextFileParser.WriteBool("game.ini", "OldMenu", Globals2.UseOldMenu);
      });
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
      this.oldCombat = Globals2.GameProperties.SaveGame.Header.CombatEnabled;
      this.oldDifficulty = Globals2.GameProperties.SaveGame.Header.GameDifficulty;
      this.oldCombatDiff = (int) Globals2.GameProperties.SaveGame.Header.CombatLevelDifference;
      this.oldClanProtection = Globals2.GameProperties.SaveGame.Header.ClanProtection;
    }

    private void OnXPMultiplierEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      Globals2.GameProperties.SaveGame.Header.XPMultiplier = MathHelper.Clamp((float) Math.Round(number, 2), 0.01f, 100f);
      this.ResetToggleItems();
    }

    private void OnCombatLevelDifferenceEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      Globals2.GameProperties.SaveGame.Header.CombatLevelDifference = (short) number;
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = "Texture Pack: " + Globals2.GameProperties.SaveGame.Header.TexturePack;
      this.MenuEntries[5].Text = "Reset Music Shuffle";
      if (!this.instance.IsAvatarDesigner)
      {
        this.MenuEntries[6].Text = "Game Difficulty: " + Globals2.GameProperties.SaveGame.Header.GameDifficulty.ToString();
        this.MenuEntries[7].Text = this.instance.IsCombatEnabled ? "Toggle Combat: On" : "Toggle Combat: Off";
        this.MenuEntries[8].Text = "XP Multiplier: " + Globals2.GameProperties.SaveGame.Header.XPMultiplier.ToString("N2");
        short combatLevelDifference = Globals2.GameProperties.SaveGame.Header.CombatLevelDifference;
        this.MenuEntries[9].Text = "Combat Level Difference: " + (combatLevelDifference == (short) 0 ? "Inactive" : combatLevelDifference.ToString());
        this.MenuEntries[10].Text = Globals2.GameProperties.SaveGame.Header.ClanProtection ? "Clan Protection: On" : "Clan Protection: Off";
      }
      this.MenuEntries[this.MenuEntries.Count - 3].Text = "Auto Save: " + Utils.InsertSpacesBeforeCapitals(Globals2.GameSettings.AutoSave.ToString());
      this.MenuEntries[this.MenuEntries.Count - 2].Text = "Default Menu: " + (Globals2.UseOldMenu ? "Old" : "New");
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
      if (this.instance.IsAvatarDesigner)
        return;
      this.MenuEntries[10].ToolTip.Text = "If Clan Protection is On, then members of the same clan cannot attack each other.";
      this.MenuEntries[11].ToolTip.Text = "Enable or disable individual item use in the world.";
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.instance == null || Globals2.GameProperties == null || Globals2.GameProperties.SaveGame == null)
        return;
      bool flag1 = false;
      if (this.oldCombat != Globals2.GameProperties.SaveGame.Header.CombatEnabled)
      {
        flag1 = true;
        this.instance.AddNotification(this.player, " has " + (this.oldCombat ? "disabled" : "enabled") + " Combat", NotifyRecipient.Remote);
      }
      if (this.oldDifficulty != Globals2.GameProperties.SaveGame.Header.GameDifficulty)
      {
        flag1 = true;
        this.instance.AddNotification(this.player, " has changed Difficulty to " + Globals2.GameProperties.SaveGame.Header.GameDifficulty.ToString(), NotifyRecipient.Remote);
      }
      if (this.oldCombatDiff != (int) Globals2.GameProperties.SaveGame.Header.CombatLevelDifference)
      {
        flag1 = true;
        this.instance.AddNotification(this.player, " has changed Combat Level Difference to " + Globals2.GameProperties.SaveGame.Header.CombatLevelDifference.ToString(), NotifyRecipient.Remote);
      }
      if (this.oldClanProtection != Globals2.GameProperties.SaveGame.Header.ClanProtection)
      {
        flag1 = true;
        this.instance.AddNotification(this.player, " has " + (this.oldClanProtection ? "disabled" : "enabled") + " Clan Protection", NotifyRecipient.Remote);
      }
      if (flag1)
        NetworkManager.Instance.SendGamePropertiesNonVital();
      bool flag2 = false;
      for (int index = 0; index < Globals1.ItemData.Length; ++index)
      {
        if (this.itemsEnabled[index] != Globals1.ItemData[index].IsEnabled)
        {
          flag2 = true;
          break;
        }
      }
      if (!flag2)
        return;
      NetworkManager.Instance.SendGlobalItemData();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
