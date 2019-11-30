// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GlobalPlayerOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GlobalPlayerOptionsScreen : BlockMenuScreen
  {
    private GlobalGamerSettings gamerSettings;
    private PlayerSettings settings;
    private SliderValue fov;
    private SliderValue sensitivity;

    public GlobalPlayerOptionsScreen(PlayerIndex playerIndex)
      : base("Options", (Player) null)
    {
      this.gamerSettings = Globals2.GamertagData.GetGlobalGamerSettings(playerIndex);
      this.settings = this.gamerSettings.PlayerSettings;
      this.fov = new SliderValue()
      {
        Value = this.settings.FOVNormalized,
        Range = 1f
      };
      this.sensitivity = new SliderValue()
      {
        Value = this.settings.GamePadSensitivity,
        Range = 1f
      };
      MapChunkTM.UpdateItemOverrideNotThreaded = true;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, this.player, "Field of View: ", this.fov, 296));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, this.player, "Gamepad Sensitivity: ", this.sensitivity, 296));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.RumbleOn = !this.settings.RumbleOn;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.DisplayXPGains = !this.settings.DisplayXPGains;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.InvertY = !this.settings.InvertY;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.AutoPlacementToggleSelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.HotBarTransToggleSelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num6;
      int num7 = index6 + 1;
      blockMenuEntryList7[index6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.CompassTop = !this.settings.CompassTop;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index7 = num7;
      int index8 = index7 + 1;
      blockMenuEntryList8[index7].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.Bobbing = !this.settings.Bobbing;
        this.ResetToggleItems();
      });
      blockMenuEntryList1[index8].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.fov.Value = MathHelper.Clamp(this.fov.Value - 0.05f, 0.0f, 1f);
        this.settings.FOVNormalized = this.fov.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index9 = index8;
      int index10 = index9 + 1;
      blockMenuEntryList9[index9].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.fov.Value = MathHelper.Clamp(this.fov.Value + 0.05f, 0.0f, 1f);
        this.settings.FOVNormalized = this.fov.Value;
      });
      blockMenuEntryList1[index10].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.sensitivity.Value = MathHelper.Clamp(this.sensitivity.Value - 0.05f, 0.0f, 1f);
        this.settings.GamePadSensitivity = this.sensitivity.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index11 = index10;
      int num8 = index11 + 1;
      blockMenuEntryList10[index11].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.sensitivity.Value = MathHelper.Clamp(this.sensitivity.Value + 0.05f, 0.0f, 1f);
        this.settings.GamePadSensitivity = this.sensitivity.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index12 = num8;
      int num9 = index12 + 1;
      blockMenuEntryList11[index12].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (this.settings.WieldType == WieldType.BothHands)
          this.settings.WieldType = WieldType.LeftHand;
        else
          ++this.settings.WieldType;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index13 = num9;
      int num10 = index13 + 1;
      blockMenuEntryList12[index13].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = "Toggle Rumble: " + (this.settings.RumbleOn ? "On " : "Off");
      this.MenuEntries[1].Text = "Toggle Display XP Gains: " + (this.settings.DisplayXPGains ? "On" : "Off");
      this.MenuEntries[2].Text = "Invert Y: " + (this.settings.InvertY ? "On" : "Off");
      this.MenuEntries[3].Text = "Auto Place Speed: " + this.settings.GetAutoPlaceSettingText();
      this.MenuEntries[4].Text = "Hotbar Transparency: " + this.settings.GetHotbarTransparencyText();
      this.MenuEntries[5].Text = "Compass: " + (this.settings.CompassTop ? "Top" : "Bottom");
      this.MenuEntries[6].Text = this.settings.Bobbing ? "Bobbing: On" : "Bobbing: Off";
      this.MenuEntries[9].Text = "Wield: " + this.settings.WieldType.ToString();
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
      MapChunkTM.UpdateItemOverrideNotThreaded = false;
    }

    private void AutoPlacementToggleSelected(object sender, PlayerIndexEventArgs e)
    {
      this.settings.ToggleAutoPlace();
      this.ResetToggleItems();
    }

    private void HotBarTransToggleSelected(object sender, PlayerIndexEventArgs e)
    {
      this.settings.ToggleHotbarTransparency();
      this.ResetToggleItems();
    }

    private void ClearUnlockablesSelected(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreen messageBoxScreen = new MessageBoxScreen("This option will clear ALL of your Unlockable Avatars progress so\nall Avatars will be locked again and you will have to perform\neverything needed to unlock them again!", (string) null, "Yes I want to clear my Unlockables progress", (string) null, "Cancel - Do not clear my progress", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground));
      messageBoxScreen.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.OnClearUnlocksProgress);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreen, this.ControllingPlayer);
    }

    private void OnClearUnlocksProgress(object sender, PlayerIndexEventArgs e)
    {
      Gamer signedInGamer = Globals2.GetSignedInGamer(this.ControllingPlayer);
      if (signedInGamer == null)
        return;
      Globals2.GamertagData.GetGamertagData(signedInGamer).UnlockData = new PlayerUnlockableData(signedInGamer);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreen("Your unlockables progress has been cleared.\nIf this was a mistake then Dashboard now so it is not saved.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground)), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
