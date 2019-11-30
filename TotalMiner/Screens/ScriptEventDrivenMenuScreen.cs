// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptEventDrivenMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptEventDrivenMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;

    private string JoinWorldScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerJoin);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string LeaveWorldScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerLeave);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string PlayerDiesScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerDeath);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string PlayerRespawnScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerRespawn);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string CustomMenuScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.CustomMenu);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    public ScriptEventDrivenMenuScreen(GameInstance instance, Player player)
      : base("Scripts Menu", player)
    {
      this.instance = instance;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "What is this?"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.WhatIsThisEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.JoinScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.LeaveScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.PlayerDiesScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.PlayerRespawnScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num6;
      int num7 = index6 + 1;
      blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.CustomMenuScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index7 = num7;
      int num8 = index7 + 1;
      blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetMenuItemText();
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 672;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.MenuEntries[1].ToolTip.Text = "The selected script will be executed once for each player as they join the world and exit the loading screen.";
      this.MenuEntries[2].ToolTip.Text = "The selected script will be executed once for each player when they leave the world.";
      this.MenuEntries[3].ToolTip.Text = "The selected script will be executed immediately for a player if they die.";
      this.MenuEntries[4].ToolTip.Text = "The selected script will be executed for a player after they respawn (from death).";
      this.MenuEntries[5].ToolTip.Text = "The selected script will be executed for a player when they select the Custom menu item on the Pause Menu.";
    }

    private void ResetMenuItemText()
    {
      this.MenuEntries[1].Text = "Player Joins: " + this.JoinWorldScriptName;
      this.MenuEntries[2].Text = "Player Leaves: " + this.LeaveWorldScriptName;
      this.MenuEntries[3].Text = "Player Dies: " + this.PlayerDiesScriptName;
      this.MenuEntries[4].Text = "Player Respawn: " + this.PlayerRespawnScriptName;
      this.MenuEntries[5].Text = "Custom Menu: " + this.CustomMenuScriptName;
    }

    private void WhatIsThisEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("On this screen you can select scripts that will automatically be executed when certain game events occur.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void JoinScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForJoin), false, true), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForJoin(MenuEntry script)
    {
      this.SetScript(script != null ? (string) script.Tag + script.Text : (string) null, ScriptEvent.PlayerJoin);
      return true;
    }

    private void LeaveScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForLeave), false, true), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForLeave(MenuEntry script)
    {
      this.SetScript(script != null ? (string) script.Tag + script.Text : (string) null, ScriptEvent.PlayerLeave);
      return true;
    }

    private void PlayerDiesScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForPlayerDies), false, true), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForPlayerDies(MenuEntry script)
    {
      this.SetScript(script != null ? (string) script.Tag + script.Text : (string) null, ScriptEvent.PlayerDeath);
      return true;
    }

    private void PlayerRespawnScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForPlayerRespawn), false, true), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForPlayerRespawn(MenuEntry script)
    {
      this.SetScript(script != null ? (string) script.Tag + script.Text : (string) null, ScriptEvent.PlayerRespawn);
      return true;
    }

    private void CustomMenuScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForCustomMenu), false, true), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForCustomMenu(MenuEntry script)
    {
      this.SetScript(script != null ? (string) script.Tag + script.Text : (string) null, ScriptEvent.CustomMenu);
      return true;
    }

    private void SetScript(string scriptName, ScriptEvent e)
    {
      this.instance.SetEventScript(e, this.instance.GetScript(scriptName));
      this.instance.NetworkManager.SendEventScript(scriptName, e);
      this.ResetMenuItemText();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
