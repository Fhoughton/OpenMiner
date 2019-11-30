// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BehaviourListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.AI;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class BehaviourListMenuScreen : FolderListMenuScreen
  {
    private string lastPath;
    private bool allowDelete;
    private Action onCancelSelected;
    private BehaviourTreeType treeType;

    public BehaviourListMenuScreen(
      GameInstance instance,
      Player player,
      BehaviourTreeType treeType,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected)
      : this(instance, player, treeType, path, onSelected, (Action) null, false, false)
    {
    }

    public BehaviourListMenuScreen(
      GameInstance instance,
      Player player,
      BehaviourTreeType treeType,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected,
      Action onCancel,
      bool allowDelete,
      bool includeNone)
      : base(instance, player)
    {
      this.lastPath = path;
      this.allowDelete = allowDelete;
      this.treeType = treeType;
      this.onCancelSelected = onCancel;
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.ScriptBlock;
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.LoadEntries), onSelected, (EventHandler<PlayerIndexEventArgs>) null, (string) null, allowDelete ? new EventHandler<PlayerIndexEventArgs>(this.OnDelete) : (EventHandler<PlayerIndexEventArgs>) null, allowDelete ? "Delete" : "", includeNone);
    }

    private string[] LoadEntries(string path)
    {
      if (path == null)
        path = "";
      List<string> behaviourTreeNames = this.GetBehaviourTreeNames();
      List<string> stringList = new List<string>(behaviourTreeNames.Count);
      foreach (string str1 in behaviourTreeNames)
      {
        if (str1.StartsWith(path))
        {
          string str2 = str1.Substring(path.Length);
          int num = str2.IndexOf('\\');
          if (num >= 0)
          {
            string str3 = str2.Substring(0, num + 1);
            if (!stringList.Contains(str3))
              stringList.Add(str3);
          }
          else
            stringList.Add(str2);
        }
      }
      stringList.Sort(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
      return stringList.ToArray();
    }

    private List<string> GetBehaviourTreeNames()
    {
      List<string> stringList = new List<string>(Globals1.BehaviourTrees.Count);
      for (int index = 0; index < Globals1.BehaviourTrees.Count; ++index)
      {
        if (Globals1.BehaviourTrees[index].TreeType == this.treeType)
          stringList.Add(Globals1.BehaviourTrees[index].Name);
      }
      return stringList;
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new BehaviourListMenuScreen(this.instance, this.player, this.treeType, this.currentPath, this.onItemSelected, (Action) null, this.allowDelete, false);
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      if ((this.currentPath == null || this.currentPath.Length < 1) && this.onCancelSelected != null)
        this.onCancelSelected();
      base.OnCancel(playerIndex);
    }

    private void OnDelete(object sender, PlayerIndexEventArgs e)
    {
      MenuEntry menuEntry = sender as MenuEntry;
      if (menuEntry == null)
        return;
      Globals1.DeleteBehaviourTree(this.treeType, (string) menuEntry.Tag + menuEntry.Text);
      Globals1.SaveBehaviourTrees();
      this.RestartScreen();
    }
  }
}
