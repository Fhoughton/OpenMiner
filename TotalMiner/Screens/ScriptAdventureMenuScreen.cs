// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptAdventureMenuScreen
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
  internal class ScriptAdventureMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;

    public ScriptAdventureMenuScreen(GameInstance instance, Player player)
      : base("Scripts Menu", player)
    {
      this.instance = instance;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "What is this?"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Assign Script"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "View Assigned Scripts"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Unassign Script"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.WhatIsThisEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.AddScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.ViewScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.RemoveScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void WhatIsThisEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Admins can assign scripts as Adventure Scripts so that other non admin players\ncan run these scripts from the Script menu without requiring Admin permission.\n\nNon Admin players can only execute these scripts, they cannot view them (unless\nthey have the View Scripts permission), they cannot edit them or change them\nin any other way.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void AddScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForAdd), false, false, false, true), this.ControllingPlayer);
      this.ExitScreen();
    }

    private bool OnScriptSelectedForAdd(MenuEntry scriptItem)
    {
      string scriptName = (string) scriptItem.Tag + scriptItem.Text;
      this.instance.AddAdventureScript(this.instance.GetScript(scriptName));
      this.instance.NetworkManager.SendAdventureScript(scriptName, false);
      this.ScreenManager.AddScreen((GameScreen) new ScriptAdventureMenuScreen(this.instance, this.player), this.ControllingPlayer);
      return true;
    }

    private void ViewScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, (ListBoxScreen.OnMenuItemSelected) null, false, false, true, true), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void RemoveScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForRemove), false, false, true, true), this.ControllingPlayer);
      this.ExitScreen();
    }

    private bool OnScriptSelectedForRemove(MenuEntry scriptItem)
    {
      string scriptName = (string) scriptItem.Tag + scriptItem.Text;
      this.instance.RemoveAdventureScript(this.instance.GetScript(scriptName));
      this.instance.NetworkManager.SendAdventureScript(scriptName, true);
      this.ScreenManager.AddScreen((GameScreen) new ScriptAdventureMenuScreen(this.instance, this.player), this.ControllingPlayer);
      return true;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
