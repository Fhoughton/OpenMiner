// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ZoneMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ZoneMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private MapStrategyTM strategy;

    public ZoneMenuScreen(GameInstance instance, Player player)
      : base("Zone Menu", player)
    {
      this.instance = instance;
      this.strategy = instance.Map.MapStrategy as MapStrategyTM;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "New Zone"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Edit Zones"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.AddMenuEntrySelected);
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.EditMenuEntrySelected);
      blockMenuEntryList[2].Selected += new EventHandler<PlayerIndexEventArgs>(this.ViewZonesMenuEntrySelected);
      blockMenuEntryList[3].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[2].Text = "View Zones: " + (Globals2.GameSettings.ViewZones ? "On" : "Off");
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 290;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void AddMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.Gamer == null || (!this.ControllingPlayer.HasValue || this.strategy == null))
        return;
      if (this.strategy.MarkerBlocks.Count < 2)
        this.instance.CreativeModeHelper.ShowInvalidMarkerCountError(this.player.GamerID);
      else
        Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Zone Name", "Enter the Zone Name", "", new AsyncCallback(this.ZoneNameEntered), (object) null);
    }

    private void ZoneNameEntered(IAsyncResult ar)
    {
      string name = Globals2.StripFolderName(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (name.Length <= 0)
        return;
      if (this.strategy.GetZone(name) != null)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: A Zone already exists with this Name", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        GlobalPoint3D min;
        GlobalPoint3D max;
        this.instance.CreativeModeHelper.GetMinMax(this.player.Gamer.ID, out min, out max);
        this.instance.CreativeModeHelper.RemoveMarkers(this.player.Gamer.ID, true);
        Zone zone = new Zone(name, ZoneType.None, min, max);
        this.strategy.AddZone(zone);
        NetworkManager.Instance.SendZone(zone);
        this.ScreenManager.AddScreen((GameScreen) new ZoneEditScreen(this.instance, this.player, (string) null, (Action<Zone>) null), this.ControllingPlayer);
        this.ExitScreen();
      }
    }

    private void EditMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ZoneEditScreen(this.instance, this.player, (string) null, (Action<Zone>) null), this.ControllingPlayer);
    }

    private void ViewZonesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameSettings.ViewZones = !Globals2.GameSettings.ViewZones;
      this.ResetToggleItems();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
