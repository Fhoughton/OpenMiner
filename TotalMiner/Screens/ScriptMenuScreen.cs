// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private MapStrategyTM strategy;
    private Player playerToRunScript;
    private ScriptDocumentation docs;

    public ScriptMenuScreen(GameInstance instance, Player player)
      : base("Scripts Menu", player)
    {
      this.instance = instance;
      this.strategy = instance.Map.MapStrategy as MapStrategyTM;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Run Script"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, player.IsAdmin ? "Edit Script" : "View Script"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "New Script"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "New Script From Change Log"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Run Script For Another Player"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Run Single Script Command"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Adventure Scripts"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Event Driven Scripts"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "History Log"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Cancel Running Script"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int index1 = 0;
      blockMenuEntryList1[index1].IsEnabled = player.IsAdmin ? instance.ScriptCount > 0 : player.HasPermission(Permissions.Adventure) && instance.AdventureScriptCount > 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index2 = index1;
      int index3 = index2 + 1;
      blockMenuEntryList2[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.RunScriptEntrySelected);
      blockMenuEntryList1[index3].IsEnabled = instance.Scripts.Count > 0 && (player.IsAdmin || player.HasPermission(Permissions.ViewScripts));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index4 = index3;
      int index5 = index4 + 1;
      blockMenuEntryList3[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.EditScriptEntrySelected);
      blockMenuEntryList1[index5].IsEnabled = player.IsAdmin;
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index6 = index5;
      int index7 = index6 + 1;
      blockMenuEntryList4[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.NewScriptEntrySelected);
      blockMenuEntryList1[index7].IsEnabled = player.IsAdmin;
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index8 = index7;
      int index9 = index8 + 1;
      blockMenuEntryList5[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.NewScriptFromChangeLogEntrySelected);
      blockMenuEntryList1[index9].IsEnabled = player.IsAdmin && instance.Scripts.Count > 0 && NetworkManager.Instance.AllGamerCount > 1;
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index10 = index9;
      int index11 = index10 + 1;
      blockMenuEntryList6[index10].Selected += new EventHandler<PlayerIndexEventArgs>(this.RunScriptForPlayerEntrySelected);
      blockMenuEntryList1[index11].IsEnabled = player.IsAdmin;
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index12 = index11;
      int index13 = index12 + 1;
      blockMenuEntryList7[index12].Selected += new EventHandler<PlayerIndexEventArgs>(this.RunScriptSingleCommandEntrySelected);
      blockMenuEntryList1[index13].IsEnabled = player.IsAdmin && instance.Scripts.Count > 0;
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index14 = index13;
      int index15 = index14 + 1;
      blockMenuEntryList8[index14].Selected += new EventHandler<PlayerIndexEventArgs>(this.AdventureScriptsEntrySelected);
      blockMenuEntryList1[index15].IsEnabled = player.IsAdmin && instance.Scripts.Count > 0;
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index16 = index15;
      int index17 = index16 + 1;
      blockMenuEntryList9[index16].Selected += new EventHandler<PlayerIndexEventArgs>(this.EventDrivenScriptsEntrySelected);
      blockMenuEntryList1[index17].IsEnabled = player.IsAdmin;
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index18 = index17;
      int index19 = index18 + 1;
      blockMenuEntryList10[index18].Selected += new EventHandler<PlayerIndexEventArgs>(this.HistoryLogEntrySelected);
      blockMenuEntryList1[index19].IsEnabled = player.IsAdmin && instance.Scripts.Count > 0;
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index20 = index19;
      int num1 = index20 + 1;
      blockMenuEntryList11[index20].Selected += new EventHandler<PlayerIndexEventArgs>(this.CancelScriptEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index21 = num1;
      int num2 = index21 + 1;
      blockMenuEntryList12[index21].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
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

    private void RunScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      this.playerToRunScript = this.player;
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForExecute), false, false, !this.player.IsAdmin, false), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForExecute(MenuEntry script)
    {
      if (this.playerToRunScript == null)
        return true;
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) this.playerToRunScript
      };
      this.instance.ExecuteScript((string) script.Tag + script.Text, data, true);
      return false;
    }

    private void RunScriptForPlayerEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (NetworkManager.Instance.AllGamers.Count == 1)
        this.OnPlayerSelected(this.player.Gamer, false, (string) null);
      else
        this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnPlayerSelected), true, (string) null, false, false), this.ControllingPlayer);
    }

    private void RunScriptSingleCommandEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.docs == null)
        this.docs = new ScriptDocumentation();
      this.ScreenManager.AddScreen((GameScreen) new ScriptCommandListMenuScreen(this.instance, this.player, (string) null, this.docs.CommandList, new ListBoxScreen.OnMenuItemSelected(this.OnSingleCommandToInsertSelected), new Action(this.OnSingleCommandToInsertCancelled)), this.ControllingPlayer);
    }

    private bool OnSingleCommandToInsertSelected(MenuEntry command)
    {
      this.OnSingleCommandToInsertSelected(command != null ? command.Text : "");
      return true;
    }

    private void OnSingleCommandToInsertCancelled()
    {
      this.OnSingleCommandToInsertSelected("");
    }

    private void OnSingleCommandToInsertSelected(string text)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Script Console", "Enter the script command to execute", text, new AsyncCallback(this.OnScriptCommandEntered), (object) null);
    }

    private void OnScriptCommandEntered(IAsyncResult ar)
    {
      string cmd = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (cmd == null || cmd.Length <= 0)
        return;
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      Script script = new Script("temp", 1);
      Player gamer1 = (Player) null;
      string gamer2 = this.ExtractGamer(cmd, out gamer1);
      script.Commands.Add(gamer2);
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) gamer1
      };
      this.instance.ExecuteScript(script, data, true);
    }

    private string ExtractGamer(string cmd, out Player gamer)
    {
      gamer = this.player;
      foreach (NetworkGamer allGamer in this.instance.NetworkManager.AllGamers)
      {
        if (cmd.StartsWith(allGamer.Gamertag + " "))
        {
          cmd = cmd.Substring(allGamer.Gamertag.Length + 1);
          gamer = allGamer.Tag as Player;
          break;
        }
      }
      return cmd;
    }

    private void OnPlayerSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      if (gamer == null)
        return;
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      this.playerToRunScript = gamer.Tag as Player;
      this.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForExecute), false, false), this.ControllingPlayer);
    }

    private void NewScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Script script = new Script("New Script" + (this.instance.Scripts.Count + 1).ToString());
      script.Commands.Add("");
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, true, (ScriptEditScreen) null, (Action) null), this.ControllingPlayer);
    }

    private void NewScriptFromChangeLogEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Script script = new Script("New Script" + (this.instance.Scripts.Count + 1).ToString(), this.player.ChangeLog.Count);
      this.player.ChangeLog.WriteItems(script.Commands);
      script.IsChanged = script.Commands.Count > 0;
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, true, (ScriptEditScreen) null, (Action) null), this.ControllingPlayer);
    }

    private void EditScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      ScriptListMenuScreen scriptListMenuScreen = new ScriptListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForEdit), true, false);
      this.ScreenManager.AddScreen((GameScreen) scriptListMenuScreen, this.ControllingPlayer);
      scriptListMenuScreen.CloseOnSelect = false;
    }

    private bool OnScriptSelectedForEdit(MenuEntry scriptItem)
    {
      Script script = this.instance.GetScript((string) scriptItem.Tag + scriptItem.Text);
      if (script != null)
        this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, (Action) null), this.ControllingPlayer);
      return true;
    }

    private void CancelScriptEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      this.ScreenManager.AddScreen((GameScreen) new ScriptCancelListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForCancel)), this.ControllingPlayer);
    }

    private bool OnScriptSelectedForCancel(MenuEntry scriptItem)
    {
      Script script = this.instance.GetScript((string) scriptItem.Tag + scriptItem.Text);
      if (script != null && !this.instance.CancelScript(script, (Actor) null))
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This script could not be found on the execution queue", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.ControllingPlayer.Value));
      return true;
    }

    private void AdventureScriptsEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptAdventureMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    private void EventDrivenScriptsEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptEventDrivenMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    private void HistoryLogEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      List<string> clansThatHaveHistory = this.instance.GetListOfClansThatHaveHistory();
      string[] extraItemsAtTop = new string[clansThatHaveHistory.Count + 1];
      extraItemsAtTop[0] = "System";
      for (int index = 0; index < clansThatHaveHistory.Count; ++index)
        extraItemsAtTop[index + 1] = "Clan: " + clansThatHaveHistory[index];
      this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnHistoryLogGamerSelected), false, (string) null, false, false, extraItemsAtTop), this.ControllingPlayer);
    }

    private void OnHistoryLogGamerSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      Player player = (Player) null;
      string clanName = (string) null;
      if (gamer != null)
        player = gamer.Tag as Player;
      else if (text.StartsWith("Clan: "))
      {
        clanName = text.Substring(6);
        if (clanName == "")
          clanName = (string) null;
      }
      this.ScreenManager.AddScreen((GameScreen) new HistoryLogScreen(this.instance, player, clanName, (string) null), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
