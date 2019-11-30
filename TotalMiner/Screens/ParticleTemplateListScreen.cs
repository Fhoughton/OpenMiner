// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ParticleTemplateListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ParticleTemplateListScreen : FolderListMenuScreen
  {
    private bool includeNew;
    private Action<string, string> onTemplateSelected;

    public ParticleTemplateListScreen(
      GameInstance instance,
      Player player,
      string path,
      Action<string, string> onSelected,
      bool includeNew)
      : base(instance, player)
    {
      this.onTemplateSelected = onSelected;
      this.includeNew = includeNew;
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.ParticleEmitter;
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.GetMenuItems), new ListBoxScreen.OnMenuItemSelected(this.OnTemplateSelected), (EventHandler<PlayerIndexEventArgs>) null, (string) null, new EventHandler<PlayerIndexEventArgs>(this.TemplateDeleteMenuEntrySelected), "Delete", false);
    }

    private string[] GetMenuItems(string path)
    {
      List<string> items = new List<string>();
      for (int index = 1; index < Globals2.SystemParticleData.Length; ++index)
        items.Add(Globals2.SystemParticleData[index].Name);
      foreach (ParticleData particleData in Globals2.CustomParticleData)
        items.Add(particleData.Name);
      if (!this.includeNew)
        return this.GetArrayOfSortedItems(items, path);
      List<string> stringList = new List<string>((IEnumerable<string>) this.GetArrayOfSortedItems(items, path));
      stringList.Insert(0, "New Template");
      return stringList.ToArray();
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new ParticleTemplateListScreen(this.instance, this.player, this.currentPath, this.onTemplateSelected, this.includeNew);
    }

    private bool OnTemplateSelected(MenuEntry item)
    {
      if (this.onTemplateSelected != null)
        this.onTemplateSelected((string) item.Tag, item.Text);
      return true;
    }

    private void TemplateDeleteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      MenuEntry menuEntry = sender as MenuEntry;
      if (menuEntry == null)
        return;
      Globals2.DeleteParticleTemplate(this.currentPath + menuEntry.Text);
      this.RestartScreen();
    }

    protected override bool IsButtonYValid
    {
      get
      {
        return !(this.currentPath + this.MenuEntries[this.selectedEntry].Text).StartsWith("system\\", StringComparison.OrdinalIgnoreCase);
      }
    }
  }
}
