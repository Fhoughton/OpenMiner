// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.HistoryLogScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class HistoryLogScreen : FolderListMenuScreen
  {
    private History history;
    private History historyOrig;
    private string clanName;

    public HistoryLogScreen(GameInstance instance, Player player, string clanName, string path)
      : base(instance, player)
    {
      this.clanName = clanName;
      this.history = player == null ? (clanName == null ? instance.History : instance.GetOrCreateClanHistory(clanName)) : player.History;
      this.historyOrig = new History(this.history);
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.Clipboard;
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.LoadHistory), this.onSelected != null ? this.onSelected : new ListBoxScreen.OnMenuItemSelected(this.OnHistorySelected), new EventHandler<PlayerIndexEventArgs>(this.OnSelectXButton), "Rename", new EventHandler<PlayerIndexEventArgs>(this.OnSelectYButton), "Delete", false);
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new HistoryLogScreen(this.instance, this.player, this.clanName, this.currentPath);
    }

    protected override bool ShouldRemoveButtonXFromFolders
    {
      get
      {
        return false;
      }
    }

    private string[] LoadHistory(string path)
    {
      lock (this.history.Table)
      {
        List<string> stringList = new List<string>(this.history.Table.Count);
        if (this.history.Table.Count > 0)
        {
          if (path == null)
            path = "";
          foreach (KeyValuePair<string, long> keyValuePair in this.history.Table)
          {
            string key = keyValuePair.Key;
            if (key.StartsWith(path))
            {
              string str1 = key.Substring(path.Length);
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
          stringList.Sort(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
        }
        stringList.Insert(0, "New");
        return stringList.ToArray();
      }
    }

    protected override BlockMenuEntry GetNewMenuItem(string name)
    {
      bool flag = this.history.HasHistory(this.currentPath + name);
      long count = flag ? this.history.GetHistory(this.currentPath + name) : 0L;
      BlockMenuEntry blockMenuEntry = flag ? (BlockMenuEntry) new HistoryLogMenuEntry(this, name, count) : new BlockMenuEntry((BlockMenuScreen) this, name);
      if (flag)
        blockMenuEntry.ButtonTextA = "Edit Count";
      return blockMenuEntry;
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.history.IsEquals(this.historyOrig))
        return;
      NetworkManager.Instance.SendHistoryTable(this.player, this.history);
    }

    private bool OnHistorySelected(MenuEntry item)
    {
      if (this.selectedEntry == 0)
      {
        this.AddNewHistory();
      }
      else
      {
        HistoryLogMenuEntry historyLogMenuEntry = item as HistoryLogMenuEntry;
        if (historyLogMenuEntry != null)
          this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.instance.GetLocalPlayer(this.ControllingPlayer.Value), new NumberEntered(this.OnCountEntered), historyLogMenuEntry.Count, true), new PlayerIndex?(this.ControllingPlayer.Value));
      }
      return false;
    }

    private void AddNewHistory()
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "New History", "Enter a name for the history entry.", this.currentPath, new AsyncCallback(this.EndShowKeyboardForNewHistory), (object) null);
    }

    private void EndShowKeyboardForNewHistory(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      string key = Globals2.StripBadChars(str);
      if (key.Length <= 0 || key.EndsWith("\\") || key.Replace('\\', char.MinValue).Length <= 0)
        return;
      this.history.AddHistory(key);
      this.RestartScreen();
    }

    private void OnCountEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      string key = this.currentPath + this.MenuEntries[this.selectedEntry].Text;
      lock (this.history.Table)
      {
        if (!this.history.Table.ContainsKey(key))
          return;
        if (number != 0.0)
          this.history.Table[key] = (long) (int) number;
        else
          this.history.Table.Remove(key);
        this.RestartScreen();
      }
    }

    private void OnSelectXButton(object sender, PlayerIndexEventArgs e)
    {
      if (this.selectedEntry <= 0)
        return;
      this.RenameHistory(sender as BlockMenuEntry);
    }

    private void RenameHistory(BlockMenuEntry item)
    {
      if (item == null)
        return;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Rename History Entry", "", this.currentPath + item.Text, new AsyncCallback(this.EndShowKeyboardForRenameHistory), (object) item);
    }

    private void EndShowKeyboardForRenameHistory(IAsyncResult ar)
    {
      string str1 = Guide.EndShowKeyboardInput(ar);
      BlockMenuEntry asyncState = (BlockMenuEntry) ar.AsyncState;
      ar.AsyncWaitHandle.Close();
      if (!str1.IsNotEmpty())
        return;
      string str2 = Globals2.StripBadChars(str1);
      if (str2.Length <= 0 || str2.Replace('\\', char.MinValue).Length <= 0)
        return;
      List<string> stringList = new List<string>();
      string str3 = this.currentPath + asyncState.Text;
      lock (this.history.Table)
      {
        foreach (KeyValuePair<string, long> keyValuePair in this.history.Table)
        {
          if (keyValuePair.Key.StartsWith(str3))
            stringList.Add(keyValuePair.Key);
        }
        bool flag = str2[str2.Length - 1] == '\\';
        foreach (string key1 in stringList)
        {
          long history = this.history.GetHistory(key1);
          this.history.Table.Remove(key1);
          string key2 = str2;
          if (key1.Length > str3.Length)
            key2 += key1.Substring(str3.Length);
          else if (flag && str2.Substring(0, str2.Length - 1).Equals(key1, StringComparison.OrdinalIgnoreCase))
            key2 += key1;
          this.history.Table.Add(key2, history);
        }
      }
      this.RestartScreen();
    }

    private void OnSelectYButton(object sender, PlayerIndexEventArgs e)
    {
      if (this.selectedEntry <= 0)
        return;
      BlockMenuEntry blockMenuEntry = (BlockMenuEntry) sender;
      List<string> stringList = new List<string>();
      lock (this.history.Table)
      {
        foreach (KeyValuePair<string, long> keyValuePair in this.history.Table)
        {
          if (keyValuePair.Key.StartsWith(this.currentPath + blockMenuEntry.Text))
            stringList.Add(keyValuePair.Key);
        }
        foreach (string key in stringList)
          this.history.Table.Remove(key);
      }
      this.RestartScreen();
    }
  }
}
