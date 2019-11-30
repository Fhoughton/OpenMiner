// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NewGameBiomeSelectMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class NewGameBiomeSelectMenuScreen : BlockMenuScreen
  {
    private bool IsCreative
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Creative;
      }
    }

    public NewGameBiomeSelectMenuScreen()
      : base("New World", (Player) null)
    {
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      if (this.IsCreative)
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Flat"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Desert"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Grasslands"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Semi-Alpine"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      if (this.IsCreative)
        blockMenuEntryList1[num1++].Selected += new EventHandler<PlayerIndexEventArgs>(this.FlatSelected);
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.DesertSelected);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.GrasslandsSelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.SemiAlpineSelected);
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 287;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenAddedCore()
    {
      base.OnScreenAddedCore();
      if (Globals2.AutoStartMap >= 0)
        return;
      this.FlatSelected((object) null, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
    }

    private void FlatSelected(object sender, PlayerIndexEventArgs e)
    {
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      TerrainData terrainData = header.TerrainData;
      Globals2.GameProperties.BiomeType = terrainData.Biome = BiomeType.Flat;
      header.PassiveMobs = false;
      header.RegionSize = new Point3D(256, 256, 256);
      header.TotalMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      header.CurrentMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen((GameInstance) null, (Player) null, new SelectItemCallBack(this.GroundBlockSelected), "Select the Ground block", BlockSelectMode.SelectingGround, Block.None, 0), new PlayerIndex?(e.PlayerIndex));
    }

    private void SemiAlpineSelected(object sender, PlayerIndexEventArgs e)
    {
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      TerrainData terrainData = header.TerrainData;
      Globals2.GameProperties.BiomeType = terrainData.Biome = BiomeType.SemiAlphine;
      terrainData.GroundBlock = Item.Grass;
      header.PassiveMobs = true;
      header.RegionSize = new Point3D(512, 512, 512);
      header.TotalMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      header.CurrentMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    private void DesertSelected(object sender, PlayerIndexEventArgs e)
    {
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      TerrainData terrainData = header.TerrainData;
      Globals2.GameProperties.BiomeType = terrainData.Biome = BiomeType.Desert;
      terrainData.GroundBlock = Item.Sand;
      header.PassiveMobs = true;
      header.RegionSize = new Point3D(512, 512, 512);
      header.TotalMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      header.CurrentMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    private void GrasslandsSelected(object sender, PlayerIndexEventArgs e)
    {
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      TerrainData terrainData = header.TerrainData;
      Globals2.GameProperties.BiomeType = terrainData.Biome = BiomeType.Grasslands;
      terrainData.GroundBlock = Item.Grass;
      header.PassiveMobs = true;
      header.RegionSize = new Point3D(512, 512, 512);
      header.TotalMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      header.CurrentMapBound = new BoxInt()
      {
        Min = new GlobalPoint3D(0, 0, 0),
        Max = new GlobalPoint3D(1024, 512, 1024)
      };
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    public bool GroundBlockSelected(Player player, Item blockID, int notUsed, object tagData)
    {
      if (blockID == Item.None)
        return false;
      TerrainData terrainData = Globals2.GameProperties.SaveGame.Header.TerrainData;
      Globals2.GameProperties.BiomeType = terrainData.Biome = BiomeType.Flat;
      terrainData.GroundBlock = blockID;
      if (Globals2.GameProperties.IsNewMap && blockID == Item.SpaceWorld)
      {
        Globals2.GameSettings.ViewClouds = false;
        Globals2.GameSettings.FloraAnimation = false;
      }
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), this.ControllingPlayer);
      this.ExitScreen();
      return true;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
