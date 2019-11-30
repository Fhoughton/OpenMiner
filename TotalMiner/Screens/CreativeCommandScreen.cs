// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal abstract class CreativeCommandScreen : BlockMenuScreen
  {
    protected MapTM map;
    protected GameInstance instance;
    protected CreativeOperationData data;
    protected string statusText;
    protected int markerCount;
    protected int excludeCount;
    protected int baseItemCount;

    protected virtual bool IsBoundUsed
    {
      get
      {
        return true;
      }
    }

    protected virtual bool IsClearMarkersUsed
    {
      get
      {
        return false;
      }
    }

    protected abstract bool IsPercentUsed { get; }

    protected virtual bool IsSeedUsed
    {
      get
      {
        return this.IsPercentUsed;
      }
    }

    protected virtual string SelectBlockText
    {
      get
      {
        return "Select Block";
      }
    }

    protected virtual string BoundText
    {
      get
      {
        if (this.markerCount < 2)
          return "Region: Undefined";
        return string.Format("Region: [{0}, {1}, {2}] [{3}, {4}, {5}]", (object) this.data.Min.X, (object) this.data.Min.Y, (object) this.data.Min.Z, (object) this.data.Max.X, (object) this.data.Max.Y, (object) this.data.Max.Z);
      }
    }

    protected virtual string MeasureText
    {
      get
      {
        if (this.markerCount < 2)
          return "Measure: Undefined";
        return string.Format("Measure: {0} x {1} x {2} - {3} blocks", (object) (this.data.Max.X - this.data.Min.X + 1), (object) (this.data.Max.Y - this.data.Min.Y + 1), (object) (this.data.Max.Z - this.data.Min.Z + 1), (object) this.RegionSizeInBlocks);
      }
    }

    protected virtual string ExcludeBoundText
    {
      get
      {
        if (this.excludeCount < 2)
          return "Exclude: Unused";
        return string.Format("Exclude: [{0}, {1}, {2}] [{3}, {4}, {5}]", (object) this.data.XMin.X, (object) this.data.XMin.Y, (object) this.data.XMin.Z, (object) this.data.XMax.X, (object) this.data.XMax.Y, (object) this.data.XMax.Z);
      }
    }

    protected virtual BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeFill;
      }
    }

    protected int RegionSizeInBlocks
    {
      get
      {
        return (this.data.Max.X - this.data.Min.X + 1) * (this.data.Max.Z - this.data.Min.Z + 1) * (this.data.Max.Y - this.data.Min.Y + 1);
      }
    }

    protected int RegionSizeInBlocks2D
    {
      get
      {
        return (this.data.Max.X - this.data.Min.X + 1) * (this.data.Max.Z - this.data.Min.Z + 1);
      }
    }

    public CreativeCommandScreen(GameInstance instance, Player player, CreativeOperationData data)
      : base("Creative Command", player)
    {
      this.instance = instance;
      this.map = data.Map;
      this.data = data;
      List<BlockMenuEntry> items = new List<BlockMenuEntry>();
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Command: "));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Status: "));
      if (this.IsBoundUsed)
      {
        items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Region: "));
        items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Measure: "));
        items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Exclude: "));
      }
      if (this.IsPercentUsed)
        items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Percent:"));
      if (this.IsSeedUsed)
        items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Seed:"));
      if (this.IsClearMarkersUsed)
        items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clear Markers:"));
      items[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectCommand);
      items[1].ColorOverride = Color.Red;
      this.baseItemCount = 2;
      if (this.IsBoundUsed)
        this.baseItemCount = 5;
      if (this.IsPercentUsed)
        items[this.baseItemCount++].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterPercent);
      if (this.IsSeedUsed)
        items[this.baseItemCount++].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterSeed);
      if (this.IsClearMarkersUsed)
        items[this.baseItemCount++].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterClearMarkers);
      this.AddParamItems(items);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Go"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Cancel"));
      items[items.Count - 2].ColorOverride = Color.Red;
      items[items.Count - 2].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnExecuteSelected);
      items[items.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) items.ToArray());
      this.RefreshItemText();
      this.selectedEntry = items.Count - 2;
    }

    protected abstract void AddParamItems(List<BlockMenuEntry> items);

    protected void RefreshItemText()
    {
      this.markerCount = 0;
      this.excludeCount = 0;
      lock (this.map.MapStrategyTM.MarkerBlocks)
      {
        foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.map.MapStrategyTM.MarkerBlocks)
        {
          if (markerBlock.GamerID == this.data.GamerID)
          {
            if (markerBlock.Exclude)
              ++this.excludeCount;
            else
              ++this.markerCount;
          }
        }
      }
      this.data.IsValid = true;
      this.statusText = "No Errors, ready to go.";
      this.BuildMessageText();
      this.MenuEntries[1].OverrideColor = !this.data.IsValid;
      this.MenuEntries[this.MenuEntries.Count - 2].OverrideColor = !this.data.IsValid;
      this.MenuEntries[0].Text = "Command: " + this.data.Desc;
      this.MenuEntries[1].Text = "Status: " + this.statusText;
      this.baseItemCount = 2;
      if (this.IsBoundUsed)
      {
        this.MenuEntries[this.baseItemCount++].Text = this.BoundText;
        this.MenuEntries[this.baseItemCount++].Text = this.MeasureText;
        this.MenuEntries[this.baseItemCount++].Text = this.ExcludeBoundText;
      }
      if (this.IsPercentUsed)
        this.MenuEntries[this.baseItemCount++].Text = "Percent: " + this.data.Percent.ToString();
      if (this.IsSeedUsed)
        this.MenuEntries[this.baseItemCount++].Text = "Seed: " + (this.data.Percent == (byte) 100 ? "Not used" : (this.data.Seed == 0 ? "Random" : this.data.Seed.ToString()));
      if (this.IsClearMarkersUsed)
        this.MenuEntries[this.baseItemCount++].Text = "Clear Markers: " + (this.data.ClearMarkers ? "Yes" : "No");
      this.RefreshItemTextCore();
      this.UpdateDefaults();
    }

    protected abstract void UpdateDefaults();

    protected abstract void RefreshItemTextCore();

    protected abstract void BuildMessageText();

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = this.HighlightRectWidth;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected virtual int HighlightRectWidth
    {
      get
      {
        return 576;
      }
    }

    protected void OnSelectCommand(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new CreativeCommandsMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    public void OnSelectBlock(object sender, PlayerIndexEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.OnBlockSelected), this.SelectBlockText, this.BlockSelectMode), this.player);
    }

    protected virtual bool OnBlockSelected(Player player, Block block)
    {
      this.data.BlockID = (byte) block;
      this.RefreshItemText();
      return true;
    }

    protected virtual void OnEnterPercent(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnPercentEntered), (int) this.data.Percent, false), this.ControllingPlayer);
    }

    private void OnPercentEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      if (value < 1.0 || value > 100.0)
        value = 100.0;
      this.data.Percent = (byte) value;
      this.RefreshItemText();
    }

    protected virtual void OnEnterSeed(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnSeedEntered), this.data.Seed, false), this.ControllingPlayer);
    }

    private void OnSeedEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.Seed = (int) MathHelper.Clamp((float) (int) value, 0.0f, (float) int.MaxValue);
      this.RefreshItemText();
    }

    protected void OnEnterClearMarkers(object sender, PlayerIndexEventArgs e)
    {
      this.data.ClearMarkers = !this.data.ClearMarkers;
      this.RefreshItemText();
    }

    protected void OnExecuteSelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.data.IsValid)
      {
        if (!this.OnExecuteCore())
          return;
        this.ExitScreen();
      }
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Some setup is invalid", this.ControllingPlayer.Value);
    }

    protected abstract bool OnExecuteCore();

    protected void SendNetworkCommand()
    {
      NetworkManager.Instance.SendCreativeCommand(this.data.Command, (Block) this.data.BlockID, (Block) this.data.BlockID1, (Block) this.data.BlockID2, this.data.Percent, this.data.Seed, this.data.ClearMarkers, this.data.Desc, this.data.Point, this.data.Min, this.data.Max, this.data.XMin, this.data.XMax, this.data.GamerID, this.data.Data);
    }

    protected bool WillCommandAffectNoEditZone()
    {
      BoundingBox box1 = Globals2.GetBox(this.data.Min, this.data.Max, 0.01f);
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
      {
        for (int index = 0; index < mapStrategy.Zones.Count; ++index)
        {
          Zone zone = mapStrategy.Zones[index];
          if (zone.HasZoneType(ZoneType.NoEdit))
          {
            BoundingBox box2 = Globals2.GetBox(zone.Min, zone.Max, 0.01f);
            if (box2.Intersects(box1))
            {
              BoundingBox box3 = Globals2.GetBox(this.data.XMin, this.data.XMax, 0.01f);
              bool flag = (double) box3.Min.X <= (double) box2.Min.X && (double) box3.Max.X >= (double) box2.Max.X && (double) box3.Min.Z <= (double) box2.Min.Z && (double) box3.Max.Z >= (double) box2.Max.Z;
              if (zone.HasZoneType(ZoneType.Spawn) || !flag && this.player != null && !this.player.IsAdmin && (zone.BuilderType == ZoneBuilderType.None || zone.BuilderType == ZoneBuilderType.Player && this.player.Gamertag != zone.Builder || zone.BuilderType == ZoneBuilderType.Clan && this.player.ClanName != zone.Builder))
                return true;
            }
          }
        }
      }
      return false;
    }

    protected bool IsPlayerInsideRegion(byte blockID, bool allowNoClip)
    {
      if (!this.map.IsBlockPassable(blockID))
      {
        foreach (Gamer allEnabledGamer in NetworkManager.Instance.AllEnabledGamers)
        {
          Player tag = allEnabledGamer.Tag as Player;
          if (tag != null && (!allowNoClip || !this.instance.IsCreativeMode || !this.player.IsAdmin))
          {
            GlobalPoint3D point1 = this.map.GetPoint(tag.Position);
            GlobalPoint3D point2 = this.map.GetPoint(tag.EyePosition);
            if (this.IsInsideRegion(point1) || this.IsInsideRegion(point2))
              return true;
          }
        }
      }
      return false;
    }

    private bool IsInsideRegion(GlobalPoint3D p)
    {
      if (p.X >= this.data.Min.X && p.Y >= this.data.Min.Y && (p.Z >= this.data.Min.Z && p.X <= this.data.Max.X) && p.Y <= this.data.Max.Y)
        return p.Z <= this.data.Max.Z;
      return false;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
