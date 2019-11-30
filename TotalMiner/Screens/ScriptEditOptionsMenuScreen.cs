// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptEditOptionsMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptEditOptionsMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private Script script;
    private ScriptCompiler syntaxTester;
    private ScriptEditScreen scriptScreen;
    private Script.ReplaceTextType replaceTextType;
    private ListBoxScreen diagnosticsScreen;
    private string origReplaceText;
    private string newReplaceText;

    public int MenuEntryIndex(int cmdIndex)
    {
      return this.scriptScreen.MenuEntryIndex(cmdIndex);
    }

    private ScriptCompiler SyntaxTester
    {
      get
      {
        return this.syntaxTester ?? (this.syntaxTester = new ScriptCompiler(this.instance));
      }
    }

    public ScriptEditOptionsMenuScreen(
      GameInstance instance,
      Player player,
      Script script,
      ScriptEditScreen scriptScreen)
      : base("Script Options", player)
    {
      this.IsPopup = true;
      this.instance = instance;
      this.script = script;
      this.scriptScreen = scriptScreen;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (player.IsAdmin)
      {
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Save and Exit"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSaveAndExitSelected);
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "--------------------------------------"));
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Test Script Syntax"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTestScriptSelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Copy To New Script"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnCopySelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Replace Text"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnReplaceTextSelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Replace Text (Selected Text)"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnReplaceTextSelectedSelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Replace Text (All Scripts in Folder)"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnReplaceTextFolderSelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Replace Text (All Scripts)"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnReplaceTextAllSelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Search for Text (All Scripts)"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSearchTextAllSelected);
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Is Script Used"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnIsScriptUsedSelected);
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Diagnostics"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnDiagnosticsHandler);
        blockMenuEntryList[blockMenuEntryList.Count - 1].IsEnabled = player.IsAdmin;
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "General Help"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnGeneralHelpSelected);
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "--------------------------------------"));
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Abort Edit (and discard any changes)"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnAbortSelected);
      }
      else
      {
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Exit"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnAbortSelected);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 432;
      base.LoadContent();
      this.Font = this.ItemFont = CoreGlobals.GameFont;
    }

    public override bool HandleInput(InputState input)
    {
      return base.HandleInput(input);
    }

    private void OnAbortSelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.script.IsChanged)
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Abort edit and discard all changes?", "Confirm discard and abort", (string) null, (string) null, "Abort the Abort! Do not Abort!", CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnAbortScreen);
      }
      else
        this.AbortScreen();
    }

    private void OnAbortScreen(object sender, PlayerIndexEventArgs e)
    {
      this.AbortScreen();
    }

    private void AbortScreen()
    {
      this.ExitScreen();
      this.scriptScreen.ExitEditScreen();
    }

    private void OnSaveAndExitSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ExitScreen();
      this.scriptScreen.OnSaveAndExitSelected();
    }

    private void OnTestScriptSelected(object sender, PlayerIndexEventArgs e)
    {
      List<string> stringList = this.SyntaxTester.TestScript(this.script, 0);
      if (stringList != null && stringList.Count > 0)
      {
        this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, stringList.ToArray(), (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnErrorSelected), (string) null, (EventHandler<PlayerIndexEventArgs>) null, (string) null, (EventHandler<PlayerIndexEventArgs>) null, false, 0.5f, 1152), this.ControllingPlayer);
      }
      else
      {
        this.ExitScreen();
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("No syntax errors found", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
    }

    private bool OnErrorSelected(MenuEntry item)
    {
      int num1 = item.Text.IndexOf("Line: ");
      int num2 = item.Text.IndexOf(",", num1 + 6);
      int result;
      if (int.TryParse(item.Text.Substring(num1 + 6, num2 - num1 - 6), out result) && result > 0 && result <= this.script.Commands.Count)
        this.scriptScreen.SetSelectedEntry(this.MenuEntryIndex(result));
      this.ExitScreen();
      return true;
    }

    private void OnDiagnosticsHandler(object sender, PlayerIndexEventArgs e)
    {
      this.ExitScreen();
      int num1 = 0;
      string[] data;
      int num2;
      if (this.script.ExecutionCount >= 3)
      {
        data = new string[9]
        {
          string.Format("Script: {0}", (object) this.script.Name),
          string.Format("Number of times executed: {0}", (object) this.script.ExecutionCount),
          string.Format("Last execution time in milliseconds: {0:N4}", (object) ((double) this.script.LastExecutionTicks / (double) Globals1.StopWatchFreq * 1000.0)),
          string.Format("Average execution time in milliseconds: {0:N4}", (object) ((double) this.script.TotalExecutionTicks / (double) Globals1.StopWatchFreq * 1000.0 / (double) (this.script.ExecutionCount - 1))),
          string.Format("Total execution time in milliseconds: {0:N4}", (object) ((double) this.script.TotalExecutionTicks / (double) Globals1.StopWatchFreq * 1000.0)),
          null,
          null,
          null,
          "Clear Diagnostic Data"
        };
        num2 = 5;
      }
      else
      {
        data = new string[6]
        {
          string.Format("Script: {0}", (object) this.script.Name),
          string.Format("Number of times executed: {0}", (object) this.script.ExecutionCount),
          null,
          null,
          null,
          null
        };
        num2 = 2;
        data[5] = "Timings are only displayed after 3 or more executions.";
      }
      int ramUsedScriptCode = this.script.RAMUsedScriptCode;
      string[] strArray1 = data;
      int index1 = num2;
      int num3 = index1 + 1;
      string str1 = string.Format("RAM used by script source code: {0} bytes", (object) ramUsedScriptCode);
      strArray1[index1] = str1;
      string[] strArray2 = data;
      int index2 = num3;
      int num4 = index2 + 1;
      string str2 = string.Format("RAM used by script byte code: {0} bytes", (object) this.script.RAMUsedByteCode);
      strArray2[index2] = str2;
      string[] strArray3 = data;
      int index3 = num4;
      num1 = index3 + 1;
      string str3 = string.Format("Total RAM used: {0} bytes", (object) (ramUsedScriptCode + this.script.RAMUsedByteCode));
      strArray3[index3] = str3;
      this.ScreenManager.AddScreen((GameScreen) (this.diagnosticsScreen = new ListBoxScreen(this.player, data, new ListBoxScreen.OnMenuItemSelected(this.OnDiagnosticsItemSelected), false)), this.ControllingPlayer);
    }

    private bool OnDiagnosticsItemSelected(MenuEntry item)
    {
      if (item.Text == "Clear Diagnostic Data")
      {
        this.script.ExecutionCount = 0;
        this.script.LastExecutionTicks = 0L;
        this.script.TotalExecutionTicks = 0L;
        this.diagnosticsScreen.ExitScreen();
      }
      return false;
    }

    private void OnCopySelected(object sender, PlayerIndexEventArgs e)
    {
      string str = this.script.Path + "Script" + (this.instance.Scripts.Count + 1).ToString();
      if (this.script.Name != str)
      {
        Script script = new Script(this.script);
        script.Name = str;
        script.Alias = "";
        script.IsChanged = true;
        this.ExitScreen();
        this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, this.scriptScreen != null ? this.scriptScreen.OnScriptSaved : (Action) null), this.ControllingPlayer);
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Not Allowed\nRename this Script First", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnReplaceTextSelected(object sender, PlayerIndexEventArgs e)
    {
      this.replaceTextType = Script.ReplaceTextType.Script;
      this.EnterReplaceText(e.PlayerIndex);
    }

    private void OnReplaceTextSelectedSelected(object sender, PlayerIndexEventArgs e)
    {
      this.replaceTextType = Script.ReplaceTextType.SelectedText;
      this.EnterReplaceText(e.PlayerIndex);
    }

    private void OnReplaceTextFolderSelected(object sender, PlayerIndexEventArgs e)
    {
      this.replaceTextType = Script.ReplaceTextType.Folder;
      this.EnterReplaceText(e.PlayerIndex);
    }

    private void OnReplaceTextAllSelected(object sender, PlayerIndexEventArgs e)
    {
      this.replaceTextType = Script.ReplaceTextType.AllScripts;
      this.EnterReplaceText(e.PlayerIndex);
    }

    private void EnterReplaceText(PlayerIndex playerIndex)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the text to replace", "", "", new AsyncCallback(this.ReplaceTextEntered), (object) playerIndex);
    }

    private void ReplaceTextEntered(IAsyncResult ar)
    {
      this.origReplaceText = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (!this.origReplaceText.IsNotEmpty())
        return;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the replacement text", "", "", new AsyncCallback(this.ReplacementTextEntered), (object) null);
    }

    private void ReplacementTextEntered(IAsyncResult ar)
    {
      this.newReplaceText = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (this.newReplaceText == null)
        return;
      this.newReplaceText = Globals2.StripBadChars(this.newReplaceText);
      if (!(this.newReplaceText != this.origReplaceText))
        return;
      if (this.replaceTextType == Script.ReplaceTextType.Folder || this.replaceTextType == Script.ReplaceTextType.AllScripts)
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Warning: You are about to replace text over multiple scripts\nDo you wish to continue", "Yes, continue.", "No, cancel", (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, new PlayerIndex?(this.player.PlayerIndex));
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ReplaceTextOk);
      }
      else
        this.ReplaceTextCore(this.origReplaceText, this.newReplaceText);
    }

    private void ReplaceTextOk(object sender, PlayerIndexEventArgs e)
    {
      this.ReplaceTextCore(this.origReplaceText, this.newReplaceText);
    }

    private void ReplaceTextCore(string oldText, string newText)
    {
      int count;
      int scriptCount;
      this.instance.ScriptReplaceText(this.replaceTextType, this.script, this.scriptScreen, oldText, newText, out count, out scriptCount);
      this.scriptScreen.ExitScreen();
      this.ExitScreen();
      this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, this.script, true, this.scriptScreen, this.scriptScreen != null ? this.scriptScreen.OnScriptSaved : (Action) null), this.ControllingPlayer);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM(string.Format("Replaced {0} " + (count != 1 ? "instances" : "instance") + " of {1}\nwith {2} in {3} script(s).", (object) count, (object) oldText, (object) newText, (object) scriptCount), "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnSearchTextAllSelected(object sender, PlayerIndexEventArgs e)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the text to search for", "", "", new AsyncCallback(this.SearchTextEntered), (object) e.PlayerIndex);
    }

    private void SearchTextEntered(IAsyncResult ar)
    {
      string text = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (text == null)
        return;
      List<Script> scriptList = this.instance.ScriptSearchText((string) null, text);
      if (scriptList.Count < 1)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("No scripts found", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        string[] data = new string[scriptList.Count];
        for (int index = 0; index < scriptList.Count; ++index)
          data[index] = scriptList[index].Name;
        this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, data, new ListBoxScreen.OnMenuItemSelected(this.OnSearchScriptSelected), false)
        {
          CloseOnSelect = false
        }, this.ControllingPlayer);
      }
    }

    private bool OnSearchScriptSelected(MenuEntry entry)
    {
      Script script = this.instance.GetScript(entry.Text);
      if (script != null)
        this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, this.scriptScreen != null ? this.scriptScreen.OnScriptSaved : (Action) null), this.ControllingPlayer);
      return true;
    }

    private void OnIsScriptUsedSelected(object sender, PlayerIndexEventArgs e)
    {
      string str = this.instance.IsScriptAssigned(this.script);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This script is currently " + (str == null ? this.instance.GetScriptUsedBy(this.script) ?? "not used" : "assigned to " + str), "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnGeneralHelpSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ExitScreen();
      string helpText = new ScriptDocumentation().GetHelpText("General Help");
      if (helpText == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new HowToScreen(this.player, "General Help", helpText, 0.6f), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
