// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ZoneEditScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ZoneEditScreen : FolderListMenuScreen
  {
    public readonly int MaxColumns = 7;
    private Action<Zone> action;
    public int Column;
    private MapStrategyTM strategy;

    public ZoneEditScreen(GameInstance instance, Player player, string path, Action<Zone> action)
      : base(instance, player)
    {
      this.action = action;
      this.ItemFileIcon = Item.Marker;
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, " Zone                                 Spawn  Fly  PvP  Mobs  Edit  Other"));
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.ListOfSortedZones), new ListBoxScreen.OnMenuItemSelected(this.OnZoneItemSelectedHandler), new EventHandler<PlayerIndexEventArgs>(this.OnXButton), "Rename", new EventHandler<PlayerIndexEventArgs>(this.OnYButton), "Delete", false);
    }

    private string[] ListOfSortedZones(string path)
    {
      if (path == null)
        path = "";
      this.strategy = this.instance.MapStrategyTM;
      List<Zone> zones = this.strategy.Zones;
      List<string> stringList = new List<string>(zones.Count);
      for (int index = zones.Count - 1; index >= 0; --index)
      {
        Zone zone = zones[index];
        if (zone != null && zone.Name != null)
        {
          string str1 = zone.Name;
          if (zone.GamerID.IsGamer)
          {
            Player player = this.instance.GetPlayer(zone.GamerID);
            if (player != null)
              str1 = "Temp Zones\\" + player.Gamertag + "\\" + str1;
          }
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
      }
      stringList.Sort(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
      return stringList.ToArray();
    }

    private int SortZones(BlockMenuEntry a, BlockMenuEntry b)
    {
      return a.Text.CompareTo(b.Text);
    }

    protected override BlockMenuEntry GetNewMenuItem(string name)
    {
      Zone zone = this.GetZone(this.currentPath + name);
      if (zone == null)
        return new BlockMenuEntry((BlockMenuScreen) this, name);
      return (BlockMenuEntry) new ZoneMenuEntry(this, this.instance, this.player, zone, name);
    }

    private Zone GetZone(string name)
    {
      if (!name.StartsWith("Temp Zones\\") || name.EndsWith("\\"))
        return this.instance.MapStrategyTM.GetZone(name);
      int num1 = name.IndexOf('\\');
      int num2 = 0;
      if (num1 >= 0)
        num2 = name.IndexOf('\\', num1 + 1);
      if (num2 >= 0)
      {
        string str = name.Substring(num1 + 1, num2 - num1 - 1);
        if (str.IsNotEmpty())
        {
          Player player = this.instance.GetPlayer(str);
          if (player != null)
            return this.instance.MapStrategyTM.GetZone(name.Substring(num2 + 1), player.GamerID);
        }
      }
      return (Zone) null;
    }

    protected override void ItemInitialized(MenuEntry item, int entryID)
    {
      base.ItemInitialized(item, entryID);
      ZoneMenuEntry zoneMenuEntry = item as ZoneMenuEntry;
      if (zoneMenuEntry == null)
        return;
      Rectangle rectangle = zoneMenuEntry.EntryTextureRect.Value;
      rectangle.Y -= 4;
      zoneMenuEntry.EntryTextureRect = new Rectangle?(rectangle);
    }

    protected override bool ShouldRemoveButtonXFromFolders
    {
      get
      {
        return false;
      }
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = this.width = 792;
      this.ItemsPerPage = 14;
      this.DrawLastLine = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new ZoneEditScreen(this.instance, this.player, this.currentPath, this.action);
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    protected override void OnScreenRemovedCore()
    {
      foreach (MenuEntry menuEntry in this.MenuEntries)
      {
        ZoneMenuEntry zoneMenuEntry = menuEntry as ZoneMenuEntry;
        if (zoneMenuEntry != null && zoneMenuEntry.IsChanged)
          NetworkManager.Instance.SendZone(zoneMenuEntry.Zone);
      }
      base.OnScreenRemovedCore();
    }

    private bool OnZoneItemSelectedHandler(MenuEntry item)
    {
      ZoneMenuEntry zoneMenuEntry = item as ZoneMenuEntry;
      if (zoneMenuEntry == null)
        return true;
      zoneMenuEntry.OnSelectedEventHandler((object) item, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
      return false;
    }

    private void OnXButton(object sender, PlayerIndexEventArgs e)
    {
      string defaultText = this.currentPath + this.MenuEntries[this.selectedEntry].Text;
      if (defaultText.StartsWith("Temp Zones\\"))
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Cannot rename Temp zones", (string) null, (string) null, (string) null, "Close", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      else
        Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Rename Zone", sender is ZoneMenuEntry ? "Enter a new name for the zone" : "Enter the new path for the zones", defaultText, new AsyncCallback(this.EndShowKeyboardForRenameZone), (object) null, (MenuEntry) (sender as ZoneMenuEntry), false);
    }

    private void EndShowKeyboardForRenameZone(IAsyncResult ar)
    {
      string str1 = Globals2.StripFolderName(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      string str2 = this.currentPath + this.MenuEntries[this.selectedEntry].Text;
      if (str1.Length <= 0)
        return;
      List<Zone> zones = this.instance.MapStrategyTM.Zones;
      for (int index = zones.Count - 1; index >= 0; --index)
      {
        Zone zone = zones[index];
        if (zone.Name.StartsWith(str2))
          zone.Name = str1 + zone.Name.Substring(str2.Length, zone.Name.Length - str2.Length);
      }
      this.RestartScreen();
    }

    private void OnYButton(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Delete Zone: " + this.MenuEntries[this.selectedEntry].Text, "Yes", (string) null, (string) null, "No", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.DeleteZone);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void DeleteZone(object sender, PlayerIndexEventArgs e)
    {
      ZoneMenuEntry menuEntry = this.MenuEntries[this.selectedEntry] as ZoneMenuEntry;
      if (menuEntry == null)
        return;
      this.instance.DeleteZone(menuEntry.Zone);
      NetworkManager.Instance.SendZoneDelete(menuEntry.Zone);
      this.RestartScreen();
    }
  }
}
