// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptEditScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Game;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptEditScreen : BlockMenuScreen
  {
    public static List<string> Clipboard = new List<string>();
    public int MinLineMarked = -1;
    public int MaxLineMarked = -1;
    private int lineMarked = -1;
    private int headerLines = 3;
    public Action OnScriptSaved;
    private Script workScript;
    private Script originalScript;
    private GameInstance instance;
    private ScriptDocumentation docs;
    private ScriptCompiler syntaxTester;
    private string playerPoint;
    private int playerPointSizeX;
    private bool openScriptMenuOnExit;
    private List<string> syntaxErrors;
    private bool b_released;
    private Process editorProcess;
    private FileSystemWatcher editorWatcher;
    private static string lastFileChanged;
    private static DateTime lastWriteTime;

    public static void SetClipboard(string s)
    {
      ScriptEditScreen.Clipboard.Clear();
      if (s == null)
        return;
      ScriptEditScreen.Clipboard.Add(s);
    }

    private int CmdIndex
    {
      get
      {
        return this.selectedEntry - this.headerLines;
      }
    }

    public int GetCmdIndex(int entryID)
    {
      return entryID - this.headerLines;
    }

    public int MenuEntryIndex(int cmdIndex)
    {
      return cmdIndex - 1 + this.headerLines;
    }

    private ScriptCompiler SyntaxTester
    {
      get
      {
        return this.syntaxTester ?? (this.syntaxTester = new ScriptCompiler(this.instance));
      }
    }

    private bool IsInsideIfBlock()
    {
      return this.IsInsideIfBlock(this.selectedEntry);
    }

    private bool IsInsideIfBlock(int line)
    {
      for (int index = line - 1; index >= this.headerLines; --index)
      {
        if (this.MenuEntries[index].Text.StartsWith("if", StringComparison.OrdinalIgnoreCase) || this.MenuEntries[index].Text.StartsWith("elseif", StringComparison.OrdinalIgnoreCase))
          return true;
        if (this.MenuEntries[index].Text.StartsWith("then", StringComparison.OrdinalIgnoreCase) || this.MenuEntries[index].Text.StartsWith("else", StringComparison.OrdinalIgnoreCase) || this.MenuEntries[index].Text.StartsWith("endif", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return false;
    }

    public ScriptEditScreen(
      GameInstance instance,
      Player player,
      Script script,
      bool openScriptMenuOnExit,
      ScriptEditScreen oldScreen,
      Action onScriptSaved)
      : base("Edit Script", player)
    {
      this.instance = instance;
      this.workScript = new Script(script);
      this.workScript.EditID = script.EditID;
      this.openScriptMenuOnExit = openScriptMenuOnExit;
      this.OnScriptSaved = onScriptSaved;
      this.originalScript = script;
      this.docs = new ScriptDocumentation();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Script Name: " + script.Name));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnScriptNameSelected);
      GraphicsDeviceManager service = Services.GetService<GraphicsDeviceManager>();
      if ((service == null || !service.IsFullScreen) && Globals2.ExternalScriptEditor.IsNotEmpty())
      {
        blockMenuEntryList[blockMenuEntryList.Count - 1].SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnExternalEditorSelected);
        blockMenuEntryList[blockMenuEntryList.Count - 1].ButtonTextX = "Open in External Editor";
      }
      if (player.IsAdmin)
      {
        blockMenuEntryList[blockMenuEntryList.Count - 1].SelectYButton += new EventHandler<PlayerIndexEventArgs>(this.OnLoadFromClipboard);
        blockMenuEntryList[blockMenuEntryList.Count - 1].ButtonTextY = "Load from Clipboard";
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Script Alias: " + script.Alias));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnScriptAliasSelected);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------------"));
      for (int i = 0; i < script.Commands.Count; ++i)
        blockMenuEntryList.Add(this.GetNewMenuEntry(i));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------------"));
      if (player.IsAdmin)
      {
        blockMenuEntryList[blockMenuEntryList.Count - 1].SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnItemInsertHandler);
        blockMenuEntryList[blockMenuEntryList.Count - 1].ButtonTextX = "Insert";
      }
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      if (oldScreen == null || script != oldScreen.workScript)
        return;
      this.selectedEntry = oldScreen.selectedEntry;
      this.itemAtTopOfPage = oldScreen.itemAtTopOfPage;
      this.lineMarked = oldScreen.lineMarked;
      this.MinLineMarked = oldScreen.MinLineMarked;
      this.MaxLineMarked = oldScreen.MaxLineMarked;
    }

    private BlockMenuEntry GetNewMenuEntry(int i)
    {
      ScriptLineMenuEntry scriptLineMenuEntry = new ScriptLineMenuEntry(this, this.workScript, i);
      scriptLineMenuEntry.SelectLeft += new EventHandler<PlayerIndexEventArgs>(this.OnItemHomeHandler);
      scriptLineMenuEntry.SelectRight += new EventHandler<PlayerIndexEventArgs>(this.OnItemInsertCommandHandler);
      if (this.player.IsAdmin)
      {
        scriptLineMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnItemSelectedHandler);
        scriptLineMenuEntry.SelectYButton += new EventHandler<PlayerIndexEventArgs>(this.OnItemDeleteHandler);
        scriptLineMenuEntry.SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnItemInsertHandler);
        scriptLineMenuEntry.ButtonTextA = "Edit";
        scriptLineMenuEntry.ButtonTextX = "Insert";
        scriptLineMenuEntry.ButtonTextY = "Delete";
      }
      return (BlockMenuEntry) scriptLineMenuEntry;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 1054;
      this.ItemHeight = 24;
      this.ItemGapY = 4;
      this.ItemTextScale = 0.5f;
      this.ItemsPerPage = this.player.GameInstance.LocalPlayerCount == 1 || this.player.GameInstance.LocalPlayerCount == 2 && Globals2.GameSettings.SplitScreenVertical ? 20 : 10;
      this.DrawLastLine = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      if (this.itemAtTopOfPage > this.selectedEntry)
        this.itemAtTopOfPage = this.selectedEntry;
      GlobalPoint3D point = this.instance.Map.GetPoint(this.player.Position + new Vector3(0.0f, 0.2f, 0.0f));
      this.playerPoint = string.Format("[{0},{1},{2}]", (object) point.X, (object) point.Y, (object) point.Z);
      this.playerPointSizeX = (int) ((double) this.Font.MeasureString(this.playerPoint).X * (double) this.ItemTextScale);
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      if (this.editorWatcher != null)
      {
        this.editorWatcher.Changed -= new FileSystemEventHandler(this.TempFileChanged);
        this.editorWatcher.Changed += new FileSystemEventHandler(ScriptEditScreen.TempFileChangedAfterInGameEditorClosed);
      }
      base.OnScreenRemovedCore();
    }

    private void RebuildScreen()
    {
      this.ItemsPerPage = this.player.GameInstance.LocalPlayerCount == 1 || this.player.GameInstance.LocalPlayerCount == 2 && Globals2.GameSettings.SplitScreenVertical ? 20 : 10;
      this.ItemsPerPage = Math.Min(Math.Max(1, this.ItemsPerPage), this.MenuEntries.Count);
      this.ResetMenuRect();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    private void FlagScriptIsChanged()
    {
      this.workScript.IsChanged = true;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      KeyboardState currentKeyboardState = input.CurrentKeyboardStates[(int) this.ControllingPlayer.Value];
      KeyboardState lastKeyboardState = input.LastKeyboardStates[(int) this.ControllingPlayer.Value];
      if (!this.b_released)
      {
        this.b_released = currentGamePadState.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Released && currentKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Escape);
        return true;
      }
      if (InputManager.IsKeyReleasedNew(this.ControllingPlayer.Value, Microsoft.Xna.Framework.Input.Keys.Escape) || InputManager.IsButtonReleasedNew(this.ControllingPlayer.Value, Buttons.B))
      {
        if (this.player.IsAdmin)
        {
          this.ScreenManager.AddScreen((GameScreen) new ScriptEditOptionsMenuScreen(this.instance, this.player, this.workScript, this), this.ControllingPlayer);
          this.b_released = false;
        }
        else
          this.ExitScreen();
        return true;
      }
      if (this.lineMarked >= 0)
      {
        if (this.selectedEntry < this.lineMarked)
        {
          this.MinLineMarked = this.selectedEntry;
          this.MaxLineMarked = this.lineMarked;
          if (this.selectedEntry < this.headerLines)
            this.MinLineMarked = this.headerLines;
        }
        else if (this.selectedEntry > this.lineMarked)
        {
          this.MaxLineMarked = this.selectedEntry;
          this.MinLineMarked = this.lineMarked;
          if (this.selectedEntry == this.MenuEntries.Count - 1)
            this.MaxLineMarked = this.MenuEntries.Count - 2;
        }
        else
          this.MinLineMarked = this.MaxLineMarked = this.selectedEntry;
      }
      if (this.MenuEntries[this.selectedEntry].IsSelectLeftHaveHandler)
      {
        if (this.player.IsAdmin)
        {
          if (currentGamePadState.Buttons.LeftShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed && lastGamePadState.Buttons.LeftShoulder == Microsoft.Xna.Framework.Input.ButtonState.Released || currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C) && lastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.C))
          {
            this.OnItemCopyHandler((object) this.MenuEntries[this.selectedEntry], new PlayerIndexEventArgs(this.ControllingPlayer.Value));
            return true;
          }
          if (currentGamePadState.Buttons.RightShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed && lastGamePadState.Buttons.RightShoulder == Microsoft.Xna.Framework.Input.ButtonState.Released || currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V) && lastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.V))
          {
            this.OnItemPasteHandler((object) this.MenuEntries[this.selectedEntry], new PlayerIndexEventArgs(this.ControllingPlayer.Value));
            return true;
          }
          if (currentGamePadState.Buttons.LeftStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed && lastGamePadState.Buttons.LeftStick == Microsoft.Xna.Framework.Input.ButtonState.Released || currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && lastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.LeftShift))
          {
            if (this.lineMarked >= 0)
              this.ClearTextSelection();
            else
              this.lineMarked = this.MinLineMarked = this.MaxLineMarked = this.selectedEntry;
          }
        }
        if ((InputManager.IsButtonReleasedNew(this.ControllingPlayer.Value, Buttons.Start) || InputManager.IsKeyReleasedNew(this.ControllingPlayer.Value, Microsoft.Xna.Framework.Input.Keys.Space)) && this.OpenTargetScript(this.MenuEntries[this.selectedEntry].Text))
        {
          this.b_released = false;
          return true;
        }
        if (currentGamePadState.Buttons.RightStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed && lastGamePadState.Buttons.RightStick == Microsoft.Xna.Framework.Input.ButtonState.Released || InputManager.IsKeyReleasedNew(this.ControllingPlayer.Value, Microsoft.Xna.Framework.Input.Keys.F1))
        {
          string command = this.MenuEntries[this.selectedEntry].Text;
          int length = command.IndexOf(' ');
          if (length >= 0)
            command = command.Substring(0, length);
          string helpText = new ScriptDocumentation().GetHelpText(command);
          if (helpText != null)
          {
            this.ScreenManager.AddScreen((GameScreen) new HowToScreen(this.player, command + " Command Documentation", helpText, 0.6f), this.ControllingPlayer);
            this.b_released = false;
          }
          return true;
        }
      }
      if (this.MenuEntries[this.selectedEntry].IsSelectXButtonHaveHandler && currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Insert) && lastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Insert))
      {
        this.InsertLine("");
        return true;
      }
      if (this.MenuEntries[this.selectedEntry].IsSelectYButtonHaveHandler && currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete) && lastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Delete))
      {
        this.DeleteLine();
        return true;
      }
      if (currentGamePadState.Buttons.Back != Microsoft.Xna.Framework.Input.ButtonState.Pressed || (lastGamePadState.Buttons.Back != Microsoft.Xna.Framework.Input.ButtonState.Released || !this.workScript.IsChanged))
        return base.HandleInput(input);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You have made changes. Use the B menu", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      return true;
    }

    private void ClearTextSelection()
    {
      this.lineMarked = this.MinLineMarked = this.MaxLineMarked = -1;
    }

    private void OnItemSelectedHandler(object sender, PlayerIndexEventArgs e)
    {
      string defaultText = this.workScript.Commands[this.CmdIndex].Substring(0, Math.Min((int) byte.MaxValue, this.workScript.Commands[this.CmdIndex].Length));
      MenuEntry menuEntry = this.MenuEntries[this.selectedEntry];
      float itemTextScale = this.ItemTextScale;
      Rectangle lastHighLightRect = menuEntry.LastHighLightRect;
      lastHighLightRect.X += 76;
      lastHighLightRect.Width -= 76;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, (string) null, (string) null, defaultText, new AsyncCallback(this.OnCommandEntered), (object) this.selectedEntry, lastHighLightRect, itemTextScale, false);
    }

    private void OnCommandEntered(IAsyncResult ar)
    {
      string name = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (name == null)
        return;
      string str = Globals2.StripBadChars(name);
      this.MenuEntries[(int) ar.AsyncState].Text = this.workScript.Commands[this.CmdIndex] = str;
      this.FlagScriptIsChanged();
    }

    private void OnItemInsertHandler(object sender, PlayerIndexEventArgs e)
    {
      this.InsertLine("");
    }

    private void InsertLine(string text)
    {
      this.workScript.Commands.Insert(this.CmdIndex, text);
      this.FlagScriptIsChanged();
      BlockMenuEntry newMenuEntry = this.GetNewMenuEntry(this.CmdIndex);
      newMenuEntry.LoadContent();
      this.MenuEntries.Insert(this.selectedEntry, (MenuEntry) newMenuEntry);
      this.RebuildScreen();
    }

    private void OnItemInsertCommandHandler(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptCommandListMenuScreen(this.instance, this.player, (string) null, this.IsInsideIfBlock() ? this.docs.ConditionalList : this.docs.CommandList, new ListBoxScreen.OnMenuItemSelected(this.OnCommandToInsertSelected)), this.ControllingPlayer);
      this.b_released = false;
    }

    private void OnItemHomeHandler(object sender, PlayerIndexEventArgs e)
    {
      this.selectedEntry = 0;
      if (this.selectedEntry >= this.itemAtTopOfPage)
        return;
      this.itemAtTopOfPage = this.selectedEntry;
    }

    private void OnItemCopyHandler(object sender, PlayerIndexEventArgs e)
    {
      if (this.lineMarked >= 0)
      {
        ScriptEditScreen.Clipboard.Clear();
        for (int minLineMarked = this.MinLineMarked; minLineMarked <= this.MaxLineMarked && minLineMarked >= 0 && minLineMarked < this.MenuEntries.Count; ++minLineMarked)
          ScriptEditScreen.Clipboard.Add(this.MenuEntries[minLineMarked].Text);
      }
      else
        ScriptEditScreen.SetClipboard(this.MenuEntries[this.selectedEntry].Text);
    }

    private void OnItemPasteHandler(object sender, PlayerIndexEventArgs e)
    {
      for (int index = ScriptEditScreen.Clipboard.Count - 1; index >= 0; --index)
        this.InsertLine(ScriptEditScreen.Clipboard[index]);
    }

    private bool OnCommandToInsertSelected(MenuEntry command)
    {
      if (command != null && this.player.IsAdmin)
        this.InsertLine(command.Text);
      return true;
    }

    private void OnItemDeleteHandler(object sender, PlayerIndexEventArgs e)
    {
      this.DeleteLine();
    }

    private void DeleteLine()
    {
      if (this.lineMarked >= 0)
      {
        if (this.selectedEntry > this.MinLineMarked)
          this.selectedEntry = this.MinLineMarked;
        for (int minLineMarked = this.MinLineMarked; minLineMarked <= this.MaxLineMarked; --this.MaxLineMarked)
        {
          this.workScript.Commands.RemoveAt(this.GetCmdIndex(minLineMarked));
          this.MenuEntries.RemoveAt(minLineMarked);
        }
        this.ClearTextSelection();
      }
      else
      {
        this.workScript.Commands.RemoveAt(this.CmdIndex);
        this.MenuEntries.RemoveAt(this.selectedEntry);
      }
      this.FlagScriptIsChanged();
      this.RebuildScreen();
    }

    private bool OpenTargetScript(string command)
    {
      string calledScriptName = ScriptCompiler.GetCalledScriptName(command);
      if (calledScriptName.IsEmpty())
        return false;
      this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, this.instance.GetScript(calledScriptName) ?? new Script(calledScriptName), false, (ScriptEditScreen) null, this.OnScriptSaved), this.ControllingPlayer);
      return true;
    }

    private bool OnErrorSelected(MenuEntry item)
    {
      int num1 = item.Text.IndexOf("Line: ");
      int num2 = item.Text.IndexOf(",", num1 + 6);
      int result;
      if (int.TryParse(item.Text.Substring(num1 + 6, num2 - num1 - 6), out result) && result > 0 && result <= this.workScript.Commands.Count)
        this.SetSelectedEntry(this.MenuEntryIndex(result));
      return true;
    }

    public void OnSaveAndExitSelected()
    {
      if (this.workScript.IsChanged)
      {
        this.instance.CancelScript(this.originalScript.Name, (Actor) null);
        if (Globals2.GameProperties.SaveGame.DirNumber > 0)
          MapSaver.SaveScript(Globals2.GameProperties.SaveGame.MapFilePath, this.workScript);
        this.originalScript.LootTables = (Dictionary<int, LootTable>) null;
        this.instance.AddOrOverwriteScript(this.originalScript.Name, this.workScript, true);
        if (this.OnScriptSaved != null)
          this.OnScriptSaved();
        this.syntaxErrors = this.SyntaxTester.TestScript(this.workScript, 1);
        if (this.HasErrorMessage(this.syntaxErrors))
        {
          this.originalScript.Name = this.workScript.Name;
          MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Syntax errors have been detected in this script so it\ncannot be compiled correctly and will not execute as expected.\n", "Show errors and return to Edit screen", (string) null, (string) null, "Exit anyway", CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
          messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnExitWithErrors);
          messageBoxScreenTm.ButtonB += new EventHandler<PlayerIndexEventArgs>(this.OnExitScreen);
          this.instance.AddScreen((GameScreen) messageBoxScreenTm, this.player);
          return;
        }
      }
      this.ExitEditScreen();
    }

    private void OnExitWithErrors(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, this.syntaxErrors.ToArray(), (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnErrorSelected), (string) null, (EventHandler<PlayerIndexEventArgs>) null, (string) null, (EventHandler<PlayerIndexEventArgs>) null, false, 0.5f, 1152), this.ControllingPlayer);
    }

    private void OnExitScreen(object sender, PlayerIndexEventArgs e)
    {
      this.ExitEditScreen();
    }

    public void ExitEditScreen()
    {
      this.ExitScreen();
      if (!this.openScriptMenuOnExit)
        return;
      this.ScreenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    private bool HasErrorMessage(List<string> testList)
    {
      if (testList != null)
      {
        foreach (string test in testList)
        {
          if (test.Contains(", Error: "))
            return true;
        }
      }
      return false;
    }

    private void OnLoadFromClipboard(object sender, PlayerIndexEventArgs e)
    {
      if (!System.Windows.Forms.Clipboard.ContainsText(TextDataFormat.Text))
        return;
      string[] strArray = System.Windows.Forms.Clipboard.GetText(TextDataFormat.Text).Split('\n');
      this.workScript.Commands.Clear();
      this.MenuEntries.RemoveRange(this.headerLines, this.MenuEntries.Count - this.headerLines);
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (index > 0 || !strArray[index].StartsWith("Script Name: "))
        {
          this.workScript.Commands.Add(Globals2.StripBadChars(strArray[index].Trim()));
          this.MenuEntries.Add((MenuEntry) this.GetNewMenuEntry(this.workScript.Commands.Count - 1));
        }
      }
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------------"));
      if (this.player.IsAdmin)
      {
        this.MenuEntries[this.MenuEntries.Count - 1].SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnItemInsertHandler);
        this.MenuEntries[this.MenuEntries.Count - 1].ButtonTextX = "Insert";
      }
      this.FlagScriptIsChanged();
      this.RebuildScreen();
    }

    private void OnExternalEditorSelected(object sender, PlayerIndexEventArgs e)
    {
      if (Globals2.ExternalScriptEditor == null)
        return;
      string str = "temp" + Script.NameEditID(this.workScript) + ".txt";
      using (Stream stream = FileSystem.OpenWrite("Temp\\" + str))
      {
        using (StreamWriter streamWriter = new StreamWriter(stream))
        {
          streamWriter.WriteLine("Script Name: " + this.workScript.Name);
          for (int line = 0; line < this.workScript.Commands.Count; ++line)
          {
            string command = this.workScript.Commands[line];
            if (this.IsInsideIfBlock(line))
              streamWriter.WriteLine("  " + command);
            else
              streamWriter.WriteLine(command);
          }
        }
      }
      if (!this.player.IsAdmin)
        return;
      if (this.editorProcess != null)
      {
        BaseGame.KillProcess(this.editorProcess);
        BaseGame.DisposeFileWatcher(this.editorWatcher);
      }
      this.editorProcess = Process.Start(new ProcessStartInfo()
      {
        Arguments = FileSystem.RootPath + "Temp\\" + str,
        FileName = Globals2.ExternalScriptEditor,
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = true
      });
      this.editorProcess.EnableRaisingEvents = true;
      this.editorProcess.Exited += new EventHandler(ScriptEditScreen.EditorProcExited);
      BaseGame.AddProcess(this.editorProcess);
      this.editorWatcher = new FileSystemWatcher();
      this.editorWatcher.Path = FileSystem.RootPath + "Temp\\";
      this.editorWatcher.NotifyFilter = NotifyFilters.LastWrite;
      this.editorWatcher.Filter = str;
      this.editorWatcher.EnableRaisingEvents = true;
      this.editorWatcher.Changed += new FileSystemEventHandler(this.TempFileChanged);
      BaseGame.AddFileWatcher(this.editorWatcher);
    }

    private static void EditorProcExited(object sender, EventArgs e)
    {
      Process proc = sender as Process;
      BaseGame.RemoveProcess(proc);
      FileSystemWatcher fileWatcher = BaseGame.GetFileWatcher(proc.StartInfo.Arguments);
      if (fileWatcher == null)
        return;
      BaseGame.DisposeFileWatcher(fileWatcher);
    }

    private static bool DuplicateChangeRaised(string filename)
    {
      DateTime lastWriteTime = File.GetLastWriteTime(filename);
      bool flag = filename == ScriptEditScreen.lastFileChanged && lastWriteTime == ScriptEditScreen.lastWriteTime;
      ScriptEditScreen.lastFileChanged = filename;
      ScriptEditScreen.lastWriteTime = lastWriteTime;
      return flag;
    }

    private void TempFileChanged(object sender, FileSystemEventArgs e)
    {
      string fullPath = e.FullPath;
      if (ScriptEditScreen.DuplicateChangeRaised(fullPath))
        return;
      if (!File.Exists(fullPath))
        return;
      try
      {
        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          using (StreamReader streamReader = new StreamReader((Stream) fileStream))
          {
            this.workScript.Commands.Clear();
            this.MenuEntries.RemoveRange(this.headerLines, this.MenuEntries.Count - this.headerLines);
            int num = 0;
            while (!streamReader.EndOfStream)
            {
              string str = streamReader.ReadLine();
              if (num++ > 0 || !str.StartsWith("Script Name: "))
              {
                this.workScript.Commands.Add(Globals2.StripBadChars(str.Trim()));
                this.MenuEntries.Add((MenuEntry) this.GetNewMenuEntry(this.workScript.Commands.Count - 1));
              }
            }
            this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------------"));
            if (this.player.IsAdmin)
            {
              this.MenuEntries[this.MenuEntries.Count - 1].SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnItemInsertHandler);
              this.MenuEntries[this.MenuEntries.Count - 1].ButtonTextX = "Insert";
            }
            this.FlagScriptIsChanged();
            this.RebuildScreen();
          }
        }
      }
      catch (IOException ex)
      {
      }
    }

    private static void TempFileChangedAfterInGameEditorClosed(object sender, FileSystemEventArgs e)
    {
      string fullPath = e.FullPath;
      if (ScriptEditScreen.DuplicateChangeRaised(fullPath) || !File.Exists(fullPath))
        return;
      int editId = Script.GetEditID(fullPath);
      if (editId < 0)
        return;
      Script script = GameInstance.Instance.GetScript(editId);
      if (script == null)
        return;
      try
      {
        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          using (StreamReader streamReader = new StreamReader((Stream) fileStream))
          {
            script.Commands.Clear();
            int num = 0;
            while (!streamReader.EndOfStream)
            {
              string str = streamReader.ReadLine();
              if (num++ > 0 || !str.StartsWith("Script Name: "))
                script.Commands.Add(Globals2.StripBadChars(str.Trim()));
            }
            script.IsChanged = true;
          }
        }
      }
      catch (IOException ex)
      {
      }
    }

    private void OnScriptNameSelected(object sender, PlayerIndexEventArgs e)
    {
      if (!this.player.IsAdmin)
        return;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter Script Name", "", this.workScript.Name, new AsyncCallback(this.OnScriptNameEntered), (object) null, this.MenuEntries[0], false);
    }

    private void OnScriptNameEntered(IAsyncResult ar)
    {
      string str = Globals2.StripFolderName(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty() || !(str != this.workScript.Name))
        return;
      if (this.instance.GetScript(str) != null)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Name: " + str + "\nis already used by another script name or script alias.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        this.workScript.Name = str;
        this.MenuEntries[0].Text = "Script Name: " + str;
        this.FlagScriptIsChanged();
      }
    }

    private void OnScriptAliasSelected(object sender, PlayerIndexEventArgs e)
    {
      if (!this.player.IsAdmin)
        return;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter Script Alias", "", this.workScript.Alias, new AsyncCallback(this.OnScriptAliasEntered), (object) null, this.MenuEntries[1], false);
    }

    private void OnScriptAliasEntered(IAsyncResult ar)
    {
      string scriptName = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (scriptName == null || !(scriptName != this.workScript.Alias))
        return;
      if (this.instance.GetScript(scriptName) != null)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Alias: " + scriptName + "\nis already used by another script name or script alias.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        this.workScript.Alias = scriptName;
        this.MenuEntries[1].Text = "Script Alias: " + scriptName;
        this.FlagScriptIsChanged();
      }
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + 8, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      int num = 3;
      float scale = 0.5f;
      string buttonTextA = this.MenuEntries[this.selectedEntry].ButtonTextA;
      if (!buttonTextA.IsEmpty())
      {
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureA, destinationRectangle, this.ColorWhite);
        float x = (float) (destinationRectangle.X + destinationRectangle.Width + 8);
        float y = (float) (destinationRectangle.Y + num);
        this.SpriteBatch.DrawString(this.Font, buttonTextA, new Vector2(x, y), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X = (int) ((double) x + (double) this.Font.MeasureString(buttonTextA).X * (double) scale + 18.0);
      }
      string buttonTextX = this.MenuEntries[this.selectedEntry].ButtonTextX;
      if (!buttonTextX.IsEmpty())
      {
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureX, destinationRectangle, this.ColorWhite);
        float x = (float) (destinationRectangle.X + destinationRectangle.Width + 8);
        float y = (float) (destinationRectangle.Y + num);
        this.SpriteBatch.DrawString(this.Font, buttonTextX, new Vector2(x, y), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X = (int) ((double) x + (double) this.Font.MeasureString(buttonTextX).X * (double) scale + 18.0);
      }
      string buttonTextY = this.MenuEntries[this.selectedEntry].ButtonTextY;
      if (!buttonTextY.IsEmpty())
      {
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, this.ColorWhite);
        float x = (float) (destinationRectangle.X + destinationRectangle.Width + 8);
        float y = (float) (destinationRectangle.Y + num);
        this.SpriteBatch.DrawString(this.Font, buttonTextY, new Vector2(x, y), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X = (int) ((double) x + (double) this.Font.MeasureString(buttonTextY).X * (double) scale + 18.0);
      }
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureB, destinationRectangle, this.ColorWhite);
      float x1 = (float) (destinationRectangle.X + destinationRectangle.Width + 8);
      float y1 = (float) (destinationRectangle.Y + num);
      this.SpriteBatch.DrawString(this.Font, this.player.IsAdmin ? "Menu" : "Exit", new Vector2(x1, y1), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      destinationRectangle.X += 86;
      if (this.MenuEntries[this.selectedEntry].IsSelectLeftHaveHandler)
      {
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.DPadLeft), destinationRectangle, this.ColorWhite);
        this.SpriteBatch.DrawString(this.Font, "Home", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 8), (float) (destinationRectangle.Y + num)), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X += 92;
      }
      if (this.MenuEntries[this.selectedEntry].IsSelectRightHaveHandler)
      {
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.DPadRight), destinationRectangle, this.ColorWhite);
        float x2 = (float) (destinationRectangle.X + destinationRectangle.Width + 8);
        float y2 = (float) (destinationRectangle.Y + num);
        this.SpriteBatch.DrawString(this.Font, this.player.IsAdmin ? "Insert Command" : "View Commands", new Vector2(x2, y2), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X += 182;
      }
      if (this.MenuEntries[this.selectedEntry].IsSelectLeftHaveHandler && this.player.IsAdmin)
      {
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.LeftShoulder), new Rectangle(destinationRectangle.X + 2, destinationRectangle.Y - 2, destinationRectangle.Width - 4, destinationRectangle.Height + 6), this.ColorWhite);
        this.SpriteBatch.DrawString(this.Font, "Copy", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 4), (float) (destinationRectangle.Y + num)), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X += 84;
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.RightShoulder), new Rectangle(destinationRectangle.X + 6, destinationRectangle.Y - 2, destinationRectangle.Width - 4, destinationRectangle.Height + 6), this.ColorWhite);
        this.SpriteBatch.DrawString(this.Font, "Paste", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 10), (float) (destinationRectangle.Y + num)), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X += 98;
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.RightStick), destinationRectangle, this.ColorWhite);
        this.SpriteBatch.DrawString(this.Font, "Help", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 10), (float) (destinationRectangle.Y + num)), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        destinationRectangle.X += 82;
      }
      this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.LeftTrigger), new Rectangle(destinationRectangle.X + 6, destinationRectangle.Y - 2, destinationRectangle.Width - 4, destinationRectangle.Height + 6), this.ColorWhite);
      this.SpriteBatch.DrawString(this.Font, "Page Up/Dn", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 10), (float) (destinationRectangle.Y + num)), this.ColorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      if (!this.player.IsAdmin)
        return;
      this.SpriteBatch.DrawString(this.Font, this.playerPoint, new Vector2((float) (this.MenuRect.X + this.MenuRect.Width - 8 - this.playerPointSizeX), (float) (this.MenuRect.Y + 3)), this.ColorWhite, 0.0f, Vector2.Zero, this.ItemTextScale, SpriteEffects.None, 1f);
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
