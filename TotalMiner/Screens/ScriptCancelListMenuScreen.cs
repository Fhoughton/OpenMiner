// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptCancelListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptCancelListMenuScreen : FolderListMenuScreen
  {
    public ScriptCancelListMenuScreen(
      GameInstance instance,
      Player player,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected)
      : base(instance, player)
    {
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.ScriptBlock;
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(instance.ListOfSortedQueuedScriptNames), onSelected, (EventHandler<PlayerIndexEventArgs>) null, (string) null, (EventHandler<PlayerIndexEventArgs>) null, (string) null, false);
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new ScriptCancelListMenuScreen(this.instance, this.player, this.currentPath, this.onItemSelected);
    }

    protected override void LoadParentScreen()
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }
  }
}
