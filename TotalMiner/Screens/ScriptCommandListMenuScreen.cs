// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptCommandListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptCommandListMenuScreen : FolderListMenuScreen
  {
    private string lastPath;
    private string[] commandList;
    private Action onCancelSelected;

    public ScriptCommandListMenuScreen(
      GameInstance instance,
      Player player,
      string path,
      string[] commandList,
      ListBoxScreen.OnMenuItemSelected onSelected)
      : this(instance, player, path, commandList, onSelected, (Action) null)
    {
    }

    public ScriptCommandListMenuScreen(
      GameInstance instance,
      Player player,
      string path,
      string[] commandList,
      ListBoxScreen.OnMenuItemSelected onSelected,
      Action onCancel)
      : base(instance, player)
    {
      this.lastPath = path;
      this.commandList = commandList;
      this.onCancelSelected = onCancel;
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.ScriptBlock;
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.LoadCommands), onSelected, (EventHandler<PlayerIndexEventArgs>) null, (string) null, new EventHandler<PlayerIndexEventArgs>(this.OnHelpButtonPressed), "Command Documentation", false);
    }

    private string[] LoadCommands(string path)
    {
      if (path == null)
        path = "";
      List<string> stringList = new List<string>(this.commandList.Length);
      foreach (string command in this.commandList)
      {
        if (command.StartsWith(path))
        {
          string str1 = command.Substring(path.Length);
          int num = str1.IndexOf('\\');
          if (num >= 0)
          {
            string str2 = str1.Substring(0, num + 1);
            if (!stringList.Contains(str2))
              stringList.Add(str2);
          }
          else
            stringList.Add(str1);
        }
      }
      return stringList.ToArray();
    }

    private void OnHelpButtonPressed(object sender, PlayerIndexEventArgs e)
    {
      string command = this.MenuEntries[this.selectedEntry].Text;
      int length = command.IndexOf(' ');
      if (length >= 0)
        command = command.Substring(0, length);
      string helpText = new ScriptDocumentation().GetHelpText(command);
      if (helpText == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new HowToScreen(this.player, command + " Command Documentation", helpText), this.ControllingPlayer);
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new ScriptCommandListMenuScreen(this.instance, this.player, this.currentPath, this.commandList, this.onItemSelected);
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      if ((this.currentPath == null || this.currentPath.Length < 1) && this.onCancelSelected != null)
        this.onCancelSelected();
      base.OnCancel(playerIndex);
    }
  }
}
