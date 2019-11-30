// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private bool settingsChanged;
    private int optionsIndexStart;

    private string SpawnMobText
    {
      get
      {
        if (!Globals2.GameProperties.SaveGame.Header.PassiveMobs && !Globals2.GameProperties.SaveGame.Header.EnemyMobs)
          return "Off";
        if (Globals2.GameProperties.SaveGame.Header.PassiveMobs && !Globals2.GameProperties.SaveGame.Header.EnemyMobs)
          return "Passive Only";
        return !Globals2.GameProperties.SaveGame.Header.PassiveMobs && Globals2.GameProperties.SaveGame.Header.EnemyMobs ? "Enemy Only" : "Passive And Enemy";
      }
    }

    public CreativeMenuScreen(GameInstance instance, Player player)
      : base("Creative Other", player)
    {
      this.instance = instance;
      instance.PauseGame();
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Tools"));
      if (!instance.IsAvatarDesigner)
      {
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Zones"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Scripts"));
      }
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Load Component"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Save Component"));
      if (!instance.IsAvatarDesigner)
      {
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "----------------- Options -----------------"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      }
      else
      {
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "----------------------------------"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Mark Head"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Mark Torse"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Mark Arm"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Mark Leg"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "----------------------------------"));
      }
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToolsMenuEntrySelected);
      if (!instance.IsAvatarDesigner)
      {
        List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
        int index2 = num2;
        int num3 = index2 + 1;
        blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.ZonesMenuEntrySelected);
        List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
        int index3 = num3;
        num2 = index3 + 1;
        blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.ScriptsMenuEntrySelected);
      }
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num2;
      int num4 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.LoadComponentMenuEntrySelected);
      blockMenuEntryList1[num4 - 1].IsEnabled = player.HasPermission(Permissions.Creative);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num4;
      int num5 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.SaveComponentMenuEntrySelected);
      blockMenuEntryList1[num5 - 1].IsEnabled = player.HasUnsavedComponentEquipped && player.HasPermission(Permissions.Save);
      int num6;
      if (!instance.IsAvatarDesigner)
      {
        int num3 = num5 + 1;
        this.optionsIndexStart = num3;
        List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
        int index2 = num3;
        int num7 = index2 + 1;
        blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToggleFiniteMenuEntrySelected);
        blockMenuEntryList1[num7 - 1].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
        int index3 = num7;
        int num8 = index3 + 1;
        blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToggleSkillsMenuEntrySelected);
        blockMenuEntryList1[num8 - 1].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
        int index6 = num8;
        int num9 = index6 + 1;
        blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToggleDayNightMenuEntrySelected);
        blockMenuEntryList1[num9 - 1].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
        int index7 = num9;
        int num10 = index7 + 1;
        blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToggleWeatherMenuEntrySelected);
        blockMenuEntryList1[num10 - 1].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
        int index8 = num10;
        int num11 = index8 + 1;
        blockMenuEntryList9[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.SetWindFactorMenuEntrySelected);
        blockMenuEntryList1[num11 - 1].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
        int index9 = num11;
        int num12 = index9 + 1;
        blockMenuEntryList10[index9].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToggleMobsMenuEntrySelected);
        blockMenuEntryList1[num12 - 1].IsEnabled = player.IsAdmin;
        List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
        int index10 = num12;
        num6 = index10 + 1;
        blockMenuEntryList11[index10].Selected += new EventHandler<PlayerIndexEventArgs>(this.ToggleKeepItemsOfDeathMenuEntrySelected);
        blockMenuEntryList1[num6 - 1].IsEnabled = player.IsAdmin;
      }
      else
      {
        List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
        int index2 = num5;
        int num3 = index2 + 1;
        blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.MarkHeadMenuEntrySelected);
        List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
        int index3 = num3;
        int num7 = index3 + 1;
        blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.MarkTorsoMenuEntrySelected);
        List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
        int index6 = num7;
        int num8 = index6 + 1;
        blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.MarkArmMenuEntrySelected);
        List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
        int index7 = num8;
        num6 = index7 + 1;
        blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.MarkLegMenuEntrySelected);
      }
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index11 = num6;
      int num13 = index11 + 1;
      blockMenuEntryList12[index11].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      if (this.instance.IsAvatarDesigner)
        return;
      this.MenuEntries[this.optionsIndexStart + 1].IsEnabled = this.player.IsAdmin && Globals2.GameProperties.SaveGame.Header.FiniteMode;
      this.MenuEntries[this.optionsIndexStart].Text = "Finite Resources: " + (Globals2.GameProperties.SaveGame.Header.FiniteMode ? "On" : "Off");
      this.MenuEntries[this.optionsIndexStart + 1].Text = "Skill System: " + (this.instance.IsSkillsEnabled ? "On " + (Globals2.GameProperties.SaveGame.Header.SkillsLocal ? "(Local)" : "(Global)") : "Off");
      this.MenuEntries[this.optionsIndexStart + 2].Text = "Day/Night Cycle: " + (Globals2.GameProperties.SaveGame.Header.DayNightActive ? "On" : "Off");
      this.MenuEntries[this.optionsIndexStart + 3].Text = "Weather: " + (Globals2.GameProperties.SaveGame.Header.WeatherActive ? "On" : "Off");
      this.MenuEntries[this.optionsIndexStart + 4].Text = "Wind Factor: " + (object) this.instance.Wind.WindFactor;
      this.MenuEntries[this.optionsIndexStart + 5].Text = "Natural Mobs: " + this.SpawnMobText;
      this.MenuEntries[this.optionsIndexStart + 6].Text = "Keep Items on Death: " + (Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath ? "On" : "Off");
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      if (this.instance.IsAvatarDesigner)
        return;
      this.MenuEntries[this.optionsIndexStart + 4].ToolTip.Text = "The games general wind strength is multiplied by this wind factor. A factor of zero removes wind from the game. A factor between zero and one reduces the wind strength. A factor of one leaves the wind strength the same. A factor greater than one increases the wind strength.";
    }

    private void ToolsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new CreativeCommandsMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void ItemsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Admin, true))
        this.ScreenManager.AddScreen((GameScreen) new CustomItemMenuScreen(this.instance, this.player), this.ControllingPlayer);
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void ZonesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Admin, true))
        this.ScreenManager.AddScreen((GameScreen) new ZoneMenuScreen(this.instance, this.player), this.ControllingPlayer);
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void ScriptsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermissionAny(Permissions.Admin | Permissions.ViewScripts, true))
        this.ScreenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), this.ControllingPlayer);
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void BehavioursMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Admin, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new BehaviourMenuScreen(this.instance, this.player, (NpcSpawnBlock) null), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void LoadComponentMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        if (this.instance.TotalClipboardsSizeInBytes >= (long) this.instance.TotalClipboardsSizeCapacity)
        {
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Component RAM capacity reached.\nDiscard some clipboards before loading a new component.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
        }
        else
        {
          this.ScreenManager.AddScreen((GameScreen) new LoadComponentPackScreen(this.instance, this.player, false, true), this.ControllingPlayer);
          this.ExitScreen();
        }
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void SaveComponentMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Save, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new LoadComponentPackScreen(this.instance, this.player, true, false), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void ToggleFiniteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      Globals2.GameProperties.SaveGame.Header.FiniteMode = !Globals2.GameProperties.SaveGame.Header.FiniteMode;
      this.ResetToggleItems();
      this.settingsChanged = true;
      this.instance.SkillSystemChanged(isSkillsEnabled);
    }

    private void ToggleSkillsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.instance.ToggleSkillSystem();
      this.ResetToggleItems();
      this.settingsChanged = true;
    }

    private void ToggleDayNightMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.SaveGame.Header.DayNightActive = !Globals2.GameProperties.SaveGame.Header.DayNightActive;
      this.ResetToggleItems();
      this.settingsChanged = true;
    }

    private void ToggleWeatherMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (!(Globals2.GameProperties.SaveGame.Header.WeatherActive = !Globals2.GameProperties.SaveGame.Header.WeatherActive))
        this.instance.MapStrategyTM.RemoveAllWeather();
      this.ResetToggleItems();
      this.settingsChanged = true;
    }

    private void SetWindFactorMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnWindFactorEntered), this.instance.Wind.WindFactor, true, false), this.ControllingPlayer);
    }

    private void OnWindFactorEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.instance.Wind.WindFactor = MathHelper.Clamp((float) number, 0.0f, 3f);
      this.ResetToggleItems();
      this.settingsChanged = true;
    }

    private void ToggleMobsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (!Globals2.GameProperties.SaveGame.Header.PassiveMobs && !Globals2.GameProperties.SaveGame.Header.EnemyMobs)
        Globals2.GameProperties.SaveGame.Header.PassiveMobs = true;
      else if (Globals2.GameProperties.SaveGame.Header.PassiveMobs && !Globals2.GameProperties.SaveGame.Header.EnemyMobs)
      {
        Globals2.GameProperties.SaveGame.Header.PassiveMobs = false;
        Globals2.GameProperties.SaveGame.Header.EnemyMobs = true;
      }
      else if (!Globals2.GameProperties.SaveGame.Header.PassiveMobs && Globals2.GameProperties.SaveGame.Header.EnemyMobs)
      {
        Globals2.GameProperties.SaveGame.Header.PassiveMobs = true;
        Globals2.GameProperties.SaveGame.Header.EnemyMobs = true;
      }
      else
      {
        Globals2.GameProperties.SaveGame.Header.PassiveMobs = false;
        Globals2.GameProperties.SaveGame.Header.EnemyMobs = false;
      }
      this.ResetToggleItems();
      this.settingsChanged = true;
    }

    private void ToggleKeepItemsOfDeathMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath = !Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath;
      this.ResetToggleItems();
      this.settingsChanged = true;
    }

    private void MarkHeadMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
    }

    private void MarkTorsoMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
    }

    private void MarkArmMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
    }

    private void MarkLegMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (!this.settingsChanged)
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
