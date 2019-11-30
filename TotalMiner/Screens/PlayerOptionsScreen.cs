// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PlayerOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class PlayerOptionsScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private SliderValue fov;
    private SliderValue sensitivity;

    public event EventHandler<EventArgs> ViewportChanged;

    private void OnViewportChanged()
    {
      if (this.ViewportChanged == null)
        return;
      this.ViewportChanged((object) this, EventArgs.Empty);
    }

    public PlayerOptionsScreen(GameInstance instance, Player player)
      : base("Options", player)
    {
      PlayerOptionsScreen playerOptionsScreen = this;
      this.instance = instance;
      this.fov = new SliderValue()
      {
        Value = player.FOVNormalized,
        Range = 1f
      };
      this.sensitivity = new SliderValue()
      {
        Value = player.Settings.GamePadSensitivity,
        Range = 1f
      };
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Field of View: ", this.fov, 296, 12));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Gamepad Sensitivity: ", this.sensitivity, 296, 12));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int index2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.HudVisible = !player.Settings.HudVisible;
        playerOptionsScreen.ResetToggleItems();
      });
      blockMenuEntryList1[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (player.Settings.MapVisible = !player.Settings.MapVisible)
          player.MiniMapRenderer.OnMapDataChanged();
        playerOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index3 = index2;
      int index4 = index3 + 1;
      blockMenuEntryList3[index3].IsEnabled = player.IsGod || !instance.IsLegendaryDifficulty && player.HasPermission(Permissions.Map);
      blockMenuEntryList1[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.BlueprintFinderVisible = !player.Settings.BlueprintFinderVisible;
        playerOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index5 = index4;
      int num2 = index5 + 1;
      blockMenuEntryList4[index5].IsEnabled = player.IsGod || instance.IsDigDeepMode && !instance.IsLegendaryDifficulty;
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index6 = num2;
      int num3 = index6 + 1;
      blockMenuEntryList5[index6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.RumbleOn = !player.Settings.RumbleOn;
        playerOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index7 = num3;
      int index8 = index7 + 1;
      blockMenuEntryList6[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.NameplateToggleSelected);
      blockMenuEntryList1[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.MobNameplateToggleSelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index9 = index8;
      int index10 = index9 + 1;
      blockMenuEntryList7[index9].IsEnabled = player.IsGod || !instance.IsAvatarDesigner;
      blockMenuEntryList1[index10].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.DisplayXPGains = !player.Settings.DisplayXPGains;
        playerOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index11 = index10;
      int num4 = index11 + 1;
      blockMenuEntryList8[index11].IsEnabled = Globals2.GameProperties.SaveGame.Header.SkillsEnabled;
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index12 = num4;
      int num5 = index12 + 1;
      blockMenuEntryList9[index12].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.InvertY = !player.Settings.InvertY;
        playerOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index13 = num5;
      int num6 = index13 + 1;
      blockMenuEntryList10[index13].Selected += new EventHandler<PlayerIndexEventArgs>(this.AutoPlacementToggleSelected);
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index14 = num6;
      int num7 = index14 + 1;
      blockMenuEntryList11[index14].Selected += new EventHandler<PlayerIndexEventArgs>(this.HotBarTransToggleSelected);
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index15 = num7;
      int num8 = index15 + 1;
      blockMenuEntryList12[index15].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.CompassTop = !player.Settings.CompassTop;
        playerOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList13 = blockMenuEntryList1;
      int index16 = num8;
      int index17 = index16 + 1;
      blockMenuEntryList13[index16].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        player.Settings.Bobbing = !player.Settings.Bobbing;
        playerOptionsScreen.ResetToggleItems();
      });
      blockMenuEntryList1[index17].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        playerOptionsScreen.fov.Value = MathHelper.Clamp(playerOptionsScreen.fov.Value - 0.05f, 0.0f, 1f);
        player.FOVNormalized = playerOptionsScreen.fov.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList14 = blockMenuEntryList1;
      int index18 = index17;
      int index19 = index18 + 1;
      blockMenuEntryList14[index18].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        playerOptionsScreen.fov.Value = MathHelper.Clamp(playerOptionsScreen.fov.Value + 0.05f, 0.0f, 1f);
        player.FOVNormalized = playerOptionsScreen.fov.Value;
      });
      blockMenuEntryList1[index19].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        playerOptionsScreen.sensitivity.Value = MathHelper.Clamp(playerOptionsScreen.sensitivity.Value - 0.05f, 0.0f, 1f);
        player.Settings.GamePadSensitivity = playerOptionsScreen.sensitivity.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList15 = blockMenuEntryList1;
      int index20 = index19;
      int num9 = index20 + 1;
      blockMenuEntryList15[index20].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        playerOptionsScreen.sensitivity.Value = MathHelper.Clamp(playerOptionsScreen.sensitivity.Value + 0.05f, 0.0f, 1f);
        player.Settings.GamePadSensitivity = playerOptionsScreen.sensitivity.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList16 = blockMenuEntryList1;
      int index21 = num9;
      int num10 = index21 + 1;
      blockMenuEntryList16[index21].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        WieldType wieldType = player.Settings.WieldType;
        if (player.Settings.WieldType == WieldType.BothHands)
          player.Settings.WieldType = WieldType.LeftHand;
        else
          ++player.Settings.WieldType;
        playerOptionsScreen.ResetToggleItems();
        player.OnWieldTypeChanged(wieldType);
      });
      List<BlockMenuEntry> blockMenuEntryList17 = blockMenuEntryList1;
      int index22 = num10;
      int num11 = index22 + 1;
      blockMenuEntryList17[index22].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      int num1 = 0;
      List<MenuEntry> menuEntries1 = this.MenuEntries;
      int index1 = num1;
      int num2 = index1 + 1;
      menuEntries1[index1].Text = this.player.Settings.HudVisible ? "Toggle HUD: On " : "Toggle HUD: Off";
      List<MenuEntry> menuEntries2 = this.MenuEntries;
      int index2 = num2;
      int num3 = index2 + 1;
      menuEntries2[index2].Text = this.player.Settings.MapVisible ? "Toggle Map: On " : "Toggle Map: Off";
      List<MenuEntry> menuEntries3 = this.MenuEntries;
      int index3 = num3;
      int num4 = index3 + 1;
      menuEntries3[index3].Text = this.player.Settings.BlueprintFinderVisible ? "Toggle Blueprint Finder: On " : "Toggle Blueprint Finder: Off";
      List<MenuEntry> menuEntries4 = this.MenuEntries;
      int index4 = num4;
      int num5 = index4 + 1;
      menuEntries4[index4].Text = this.player.Settings.RumbleOn ? "Toggle Rumble: On " : "Toggle Rumble: Off";
      List<MenuEntry> menuEntries5 = this.MenuEntries;
      int index5 = num5;
      int num6 = index5 + 1;
      menuEntries5[index5].Text = "Toggle Nameplates: " + this.player.GetNameplateSettingText();
      List<MenuEntry> menuEntries6 = this.MenuEntries;
      int index6 = num6;
      int num7 = index6 + 1;
      menuEntries6[index6].Text = "Toggle Mob Nameplates: " + this.player.GetMobNameplateSettingText();
      List<MenuEntry> menuEntries7 = this.MenuEntries;
      int index7 = num7;
      int num8 = index7 + 1;
      menuEntries7[index7].Text = "Toggle Display XP Gains: " + (this.player.Settings.DisplayXPGains ? "On" : "Off");
      List<MenuEntry> menuEntries8 = this.MenuEntries;
      int index8 = num8;
      int num9 = index8 + 1;
      menuEntries8[index8].Text = this.player.Settings.InvertY ? "Invert Y: On" : "Invert Y: Off";
      List<MenuEntry> menuEntries9 = this.MenuEntries;
      int index9 = num9;
      int num10 = index9 + 1;
      menuEntries9[index9].Text = "Auto Place Speed: " + this.player.Settings.GetAutoPlaceSettingText();
      List<MenuEntry> menuEntries10 = this.MenuEntries;
      int index10 = num10;
      int num11 = index10 + 1;
      menuEntries10[index10].Text = "Hotbar Transparency: " + this.player.Settings.GetHotbarTransparencyText();
      List<MenuEntry> menuEntries11 = this.MenuEntries;
      int index11 = num11;
      int num12 = index11 + 1;
      menuEntries11[index11].Text = "Compass: " + (this.player.Settings.CompassTop ? "Top" : "Bottom");
      List<MenuEntry> menuEntries12 = this.MenuEntries;
      int index12 = num12;
      int num13 = index12 + 1;
      menuEntries12[index12].Text = this.player.Settings.Bobbing ? "Bobbing: On" : "Bobbing: Off";
      int num14 = num13 + 2;
      List<MenuEntry> menuEntries13 = this.MenuEntries;
      int index13 = num14;
      int num15 = index13 + 1;
      menuEntries13[index13].Text = "Wield: " + this.player.Settings.WieldType.ToString();
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 478;
      this.ItemHeight = 28;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.6f;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      NetworkManager.Instance.SendPlayerSettings(this.player, (NetworkGamer) null);
    }

    private void NameplateToggleSelected(object sender, PlayerIndexEventArgs e)
    {
      this.player.ToggleNameplateSetting();
      this.ResetToggleItems();
    }

    private void MobNameplateToggleSelected(object sender, PlayerIndexEventArgs e)
    {
      this.player.ToggleMobNameplateSetting();
      this.ResetToggleItems();
    }

    private void AutoPlacementToggleSelected(object sender, PlayerIndexEventArgs e)
    {
      this.player.ToggleAutoPlace();
      this.ResetToggleItems();
    }

    private void HotBarTransToggleSelected(object sender, PlayerIndexEventArgs e)
    {
      this.player.Settings.ToggleHotbarTransparency();
      this.ResetToggleItems();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
