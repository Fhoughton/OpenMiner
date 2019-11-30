// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TerrainSetupScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class TerrainSetupScreen : BlockMenuScreen
  {
    private GameProperties gameProperties;
    private BiomeParams biomeParams;
    private Action onRefresh;

    public TerrainSetupScreen(GameProperties gameProperties, Action onRefresh)
      : base("Terrain Setup", (Player) null)
    {
      this.onRefresh = onRefresh;
      this.gameProperties = gameProperties;
      this.biomeParams = gameProperties.SaveGame.Header.BiomeParams;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Biome: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Sea Level: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Ground Block: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Caves: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "------------------------"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Max Height: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Dirt Height: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Basalt Height: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "SnowLayer Height: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Snow Height: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Max Sea Depth: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Water Saturation: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Ore Density: %"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Tree Frequency: %"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Tree Density Min: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Tree Density Max: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Edit Noise Parameters"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int index2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.TerrainTypeSelectedEventHandler);
      blockMenuEntryList1[index2].SelectLeft += new EventHandler<PlayerIndexEventArgs>(this.SeaLevelSelectedLeftEventHandler);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index3 = index2;
      int num2 = index3 + 1;
      blockMenuEntryList3[index3].SelectRight += new EventHandler<PlayerIndexEventArgs>(this.SeaLevelSelectedRightEventHandler);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index4 = num2;
      int num3 = index4 + 1;
      blockMenuEntryList4[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.GroundBlockSelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index5 = num3;
      int num4 = index5 + 1;
      blockMenuEntryList5[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.CavesSelected);
      int num5 = num4 + 1;
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index6 = num5;
      int num6 = index6 + 1;
      blockMenuEntryList6[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.MaxHeightSelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index7 = num6;
      int num7 = index7 + 1;
      blockMenuEntryList7[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.DirtHeightSelected);
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index8 = num7;
      int num8 = index8 + 1;
      blockMenuEntryList8[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.BasaltHeightSelected);
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index9 = num8;
      int num9 = index9 + 1;
      blockMenuEntryList9[index9].Selected += new EventHandler<PlayerIndexEventArgs>(this.SnowLayerHeightSelected);
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index10 = num9;
      int num10 = index10 + 1;
      blockMenuEntryList10[index10].Selected += new EventHandler<PlayerIndexEventArgs>(this.SnowHeightSelected);
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index11 = num10;
      int num11 = index11 + 1;
      blockMenuEntryList11[index11].Selected += new EventHandler<PlayerIndexEventArgs>(this.MaxSeaDepthSelected);
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index12 = num11;
      int num12 = index12 + 1;
      blockMenuEntryList12[index12].Selected += new EventHandler<PlayerIndexEventArgs>(this.WaterSaturationSelected);
      List<BlockMenuEntry> blockMenuEntryList13 = blockMenuEntryList1;
      int index13 = num12;
      int num13 = index13 + 1;
      blockMenuEntryList13[index13].Selected += new EventHandler<PlayerIndexEventArgs>(this.OreDensitySelected);
      List<BlockMenuEntry> blockMenuEntryList14 = blockMenuEntryList1;
      int index14 = num13;
      int num14 = index14 + 1;
      blockMenuEntryList14[index14].Selected += new EventHandler<PlayerIndexEventArgs>(this.TreeFrequencySelected);
      List<BlockMenuEntry> blockMenuEntryList15 = blockMenuEntryList1;
      int index15 = num14;
      int num15 = index15 + 1;
      blockMenuEntryList15[index15].Selected += new EventHandler<PlayerIndexEventArgs>(this.TreeDensityMinSelected);
      List<BlockMenuEntry> blockMenuEntryList16 = blockMenuEntryList1;
      int index16 = num15;
      int num16 = index16 + 1;
      blockMenuEntryList16[index16].Selected += new EventHandler<PlayerIndexEventArgs>(this.TreeDensityMaxSelected);
      List<BlockMenuEntry> blockMenuEntryList17 = blockMenuEntryList1;
      int index17 = num16;
      int num17 = index17 + 1;
      blockMenuEntryList17[index17].Selected += new EventHandler<PlayerIndexEventArgs>(this.SetupNoiseParamsSelected);
      List<BlockMenuEntry> blockMenuEntryList18 = blockMenuEntryList1;
      int index18 = num17;
      int num18 = index18 + 1;
      blockMenuEntryList18[index18].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 408;
      this.ItemHeight = 28;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.6f;
      this.ItemsPerPage = 20;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.biomeParams.TreeDensityMax < this.biomeParams.TreeDensityMin)
        this.biomeParams.TreeDensityMax = this.biomeParams.TreeDensityMin;
      if (this.onRefresh == null)
        return;
      this.onRefresh();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        if (!this.IsPreviewableBiome)
          return base.ButtonBarHeight;
        return 30;
      }
    }

    private bool IsEditableBiome
    {
      get
      {
        BiomeType biome = this.gameProperties.SaveGame.Header.TerrainData.Biome;
        switch (biome)
        {
          case BiomeType.Flat:
          case BiomeType.Desert:
          case BiomeType.Grasslands:
          case BiomeType.SemiAlphine:
            return true;
          default:
            return biome == BiomeType.DigDeep;
        }
      }
    }

    private bool IsPreviewableBiome
    {
      get
      {
        BiomeType biome = this.gameProperties.SaveGame.Header.TerrainData.Biome;
        switch (biome)
        {
          case BiomeType.Desert:
          case BiomeType.Grasslands:
          case BiomeType.SemiAlphine:
            return true;
          default:
            return biome == BiomeType.DigDeep;
        }
      }
    }

    private void ResetToggleItems()
    {
      int num1 = 0;
      BiomeType biome = this.gameProperties.SaveGame.Header.TerrainData.Biome;
      List<MenuEntry> menuEntries1 = this.MenuEntries;
      int index1 = num1;
      int num2 = index1 + 1;
      menuEntries1[index1].Text = "Terrain: " + biome.ToString();
      string str = biome == BiomeType.Flat ? "Ground" : "Sea";
      List<MenuEntry> menuEntries2 = this.MenuEntries;
      int index2 = num2;
      int num3 = index2 + 1;
      menuEntries2[index2].Text = str + " Level: " + this.gameProperties.SaveGame.Header.TerrainData.SeaLevel.ToString();
      int num4;
      if (this.gameProperties.SaveGame.Header.GameMode == GameMode.Creative)
      {
        List<MenuEntry> menuEntries3 = this.MenuEntries;
        int index3 = num3;
        num4 = index3 + 1;
        menuEntries3[index3].Text = "Ground Block: " + this.gameProperties.SaveGame.Header.TerrainData.GroundBlock.ToString();
      }
      else
      {
        List<MenuEntry> menuEntries3 = this.MenuEntries;
        int index3 = num3;
        num4 = index3 + 1;
        menuEntries3[index3].Text = "Ground Block: Fixed";
      }
      List<MenuEntry> menuEntries4 = this.MenuEntries;
      int index4 = num4;
      int num5 = index4 + 1;
      menuEntries4[index4].Text = "Caves: " + (this.biomeParams.GenerateCaves ? "On" : "Off");
      int num6 = num5 + 1;
      List<MenuEntry> menuEntries5 = this.MenuEntries;
      int index5 = num6;
      int num7 = index5 + 1;
      menuEntries5[index5].Text = "Max Height: " + (object) this.biomeParams.MaxHeight;
      List<MenuEntry> menuEntries6 = this.MenuEntries;
      int index6 = num7;
      int num8 = index6 + 1;
      menuEntries6[index6].Text = "Dirt Height: " + (object) this.biomeParams.DirtHeight;
      List<MenuEntry> menuEntries7 = this.MenuEntries;
      int index7 = num8;
      int num9 = index7 + 1;
      menuEntries7[index7].Text = "Basalt Height: " + (object) this.biomeParams.BasaltHeight;
      List<MenuEntry> menuEntries8 = this.MenuEntries;
      int index8 = num9;
      int num10 = index8 + 1;
      menuEntries8[index8].Text = "SnowLayer Height: " + (object) this.biomeParams.SnowLayerHeight;
      List<MenuEntry> menuEntries9 = this.MenuEntries;
      int index9 = num10;
      int num11 = index9 + 1;
      menuEntries9[index9].Text = "Snow Height: " + (object) this.biomeParams.SnowHeight;
      List<MenuEntry> menuEntries10 = this.MenuEntries;
      int index10 = num11;
      int num12 = index10 + 1;
      menuEntries10[index10].Text = "Max Sea Depth: " + (object) this.biomeParams.MaxSeaDepth;
      List<MenuEntry> menuEntries11 = this.MenuEntries;
      int index11 = num12;
      int num13 = index11 + 1;
      menuEntries11[index11].Text = "Water Saturation: " + (object) this.biomeParams.WaterSaturation;
      List<MenuEntry> menuEntries12 = this.MenuEntries;
      int index12 = num13;
      int num14 = index12 + 1;
      menuEntries12[index12].Text = "Ore Density: %" + (object) this.biomeParams.OreDensity;
      List<MenuEntry> menuEntries13 = this.MenuEntries;
      int index13 = num14;
      int num15 = index13 + 1;
      menuEntries13[index13].Text = "Tree Frequency: %" + (object) this.biomeParams.TreeFrequency;
      List<MenuEntry> menuEntries14 = this.MenuEntries;
      int index14 = num15;
      int num16 = index14 + 1;
      menuEntries14[index14].Text = "Tree Density Min: " + (object) this.biomeParams.TreeDensityMin;
      List<MenuEntry> menuEntries15 = this.MenuEntries;
      int index15 = num16;
      int num17 = index15 + 1;
      menuEntries15[index15].Text = "Tree Density Max: " + (object) this.biomeParams.TreeDensityMax;
      this.MenuEntries[4].IsEnabled = false;
      if (this.gameProperties.IsNewMap && this.IsEditableBiome)
      {
        switch (this.gameProperties.SaveGame.Header.TerrainData.Biome)
        {
          case BiomeType.Flat:
            this.MenuEntries[2].IsEnabled = true;
            for (int index3 = 3; index3 < this.MenuEntries.Count - 1; ++index3)
              this.MenuEntries[index3].IsEnabled = false;
            break;
          case BiomeType.Desert:
          case BiomeType.Grasslands:
            this.MenuEntries[2].IsEnabled = false;
            this.MenuEntries[3].IsEnabled = this.gameProperties.SaveGame.Header.GameMode == GameMode.Creative;
            this.MenuEntries[5].IsEnabled = true;
            for (int index3 = 6; index3 < 11; ++index3)
              this.MenuEntries[index3].IsEnabled = false;
            break;
          case BiomeType.DigDeep:
            for (int index3 = 0; index3 < this.MenuEntries.Count - 2; ++index3)
              this.MenuEntries[index3].IsEnabled = false;
            this.MenuEntries[12].IsEnabled = true;
            this.MenuEntries[13].IsEnabled = true;
            this.MenuEntries[14].IsEnabled = true;
            this.MenuEntries[15].IsEnabled = true;
            break;
          default:
            this.MenuEntries[2].IsEnabled = false;
            this.MenuEntries[3].IsEnabled = this.gameProperties.SaveGame.Header.GameMode == GameMode.Creative;
            for (int index3 = 5; index3 < this.MenuEntries.Count - 1; ++index3)
              this.MenuEntries[index3].IsEnabled = true;
            break;
        }
      }
      else
      {
        for (int index3 = 0; index3 < this.MenuEntries.Count - 2; ++index3)
          this.MenuEntries[index3].IsEnabled = false;
        this.selectedEntry = this.MenuEntries.Count - 1;
      }
    }

    private void SetMapDimensions()
    {
      SaveMapHead header = this.gameProperties.SaveGame.Header;
      int min = header.TerrainData.Biome == BiomeType.Flat ? 0 : 100;
      int max = header.MapHeight - 100 - header.MapHeight % 100;
      if (header.GameMode == GameMode.DigDeep || header.MapHeight > 512)
        min = max = (int) header.TerrainData.SeaLevel;
      switch (header.TerrainData.GroundBlock)
      {
        case Item.SkyWorld:
        case Item.SpaceWorld:
          min = max = 0;
          break;
        case Item.NaturalWorld:
          if (min < 50)
          {
            min = 50;
            break;
          }
          break;
      }
      header.TerrainData.SeaLevel = (ushort) MyMathHelper.Clamp((int) header.TerrainData.SeaLevel, min, max);
      this.biomeParams.MaxHeight = (int) MathHelper.Clamp((float) this.biomeParams.MaxHeight, 4f, (float) (header.MapHeight - (int) header.TerrainData.SeaLevel));
    }

    public override bool HandleInput(InputState input)
    {
      if (!this.ControllingPlayer.HasValue)
        return false;
      if (this.IsPreviewableBiome && InputManager1.IsInputReleasedNew(this.ControllingPlayer, PlayerInput.EventScriptY))
      {
        SaveMapHead header = this.gameProperties.SaveGame.Header;
        this.ScreenManager.AddScreen((GameScreen) new TerrainPreviewScreen(this.gameProperties, this.onRefresh), this.ControllingPlayer);
      }
      return base.HandleInput(input);
    }

    private void GroundBlockSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen((GameInstance) null, (Player) null, new SelectItemCallBack(this.GroundBlockCallback), "Select the Ground block", BlockSelectMode.SelectingGround, Block.None, 0), new PlayerIndex?(e.PlayerIndex));
    }

    private void CavesSelected(object sender, PlayerIndexEventArgs e)
    {
      this.biomeParams.GenerateCaves = !this.biomeParams.GenerateCaves;
      this.ResetToggleItems();
    }

    private bool GroundBlockCallback(Player player, Item itemID, int notUsed, object tagData)
    {
      if (itemID == Item.None)
        return false;
      this.gameProperties.SaveGame.Header.TerrainData.GroundBlock = itemID;
      this.SetMapDimensions();
      this.ResetToggleItems();
      return true;
    }

    private void MaxHeightSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnMaxHeightEntered), this.biomeParams.MaxHeight, false), this.ControllingPlayer);
    }

    private void DirtHeightSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnDirtHeightEntered), this.biomeParams.DirtHeight, false), this.ControllingPlayer);
    }

    private void BasaltHeightSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnBasaltHeightEntered), this.biomeParams.BasaltHeight, false), this.ControllingPlayer);
    }

    private void SnowLayerHeightSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnSnowLayerHeightEntered), this.biomeParams.SnowLayerHeight, false), this.ControllingPlayer);
    }

    private void SnowHeightSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnSnowHeightEntered), this.biomeParams.SnowHeight, false), this.ControllingPlayer);
    }

    private void MaxSeaDepthSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnMaxSeaDepthEntered), this.biomeParams.MaxSeaDepth, false), this.ControllingPlayer);
    }

    private void WaterSaturationSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnWaterSaturationEntered), this.biomeParams.WaterSaturation, false), this.ControllingPlayer);
    }

    private void TreeFrequencySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnTreeFrequencyEntered), this.biomeParams.TreeFrequency, true, false), this.ControllingPlayer);
    }

    private void TreeDensityMinSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnTreeDensityMinEntered), this.biomeParams.TreeDensityMin, false), this.ControllingPlayer);
    }

    private void TreeDensityMaxSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnTreeDensityMaxEntered), this.biomeParams.TreeDensityMax, false), this.ControllingPlayer);
    }

    private void OreDensitySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnOreDensityEntered), (float) this.biomeParams.OreDensity, false, false), this.ControllingPlayer);
    }

    private void SetupNoiseParamsSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new TerrainNoiseSetupScreen(this.gameProperties, this.onRefresh), this.ControllingPlayer);
    }

    private void OnMaxHeightEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.MaxHeight = MyMathHelper.Clamp((int) number, 4, this.gameProperties.SaveGame.Header.MapHeight - (int) this.gameProperties.SaveGame.Header.TerrainData.SeaLevel);
      this.ResetToggleItems();
    }

    private void OnDirtHeightEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.DirtHeight = MyMathHelper.Clamp((int) number, 10, this.biomeParams.MaxHeight);
      this.ResetToggleItems();
    }

    private void OnBasaltHeightEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.BasaltHeight = MyMathHelper.Clamp((int) number, 10, this.biomeParams.MaxHeight);
      this.ResetToggleItems();
    }

    private void OnSnowLayerHeightEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.SnowLayerHeight = MyMathHelper.Clamp((int) number, 10, this.biomeParams.MaxHeight);
      this.ResetToggleItems();
    }

    private void OnSnowHeightEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.SnowHeight = MyMathHelper.Clamp((int) number, 10, this.biomeParams.MaxHeight);
      this.ResetToggleItems();
    }

    private void OnMaxSeaDepthEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.MaxSeaDepth = MyMathHelper.Clamp((int) number, 1, (int) this.gameProperties.SaveGame.Header.TerrainData.SeaLevel - 2);
      this.ResetToggleItems();
    }

    private void OnWaterSaturationEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.WaterSaturation = MyMathHelper.Clamp((int) number, 1, 100);
      this.ResetToggleItems();
    }

    private void OnTreeFrequencyEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.TreeFrequency = MathHelper.Clamp((float) number, 1f, 100f);
      this.ResetToggleItems();
    }

    private void OnTreeDensityMinEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.TreeDensityMin = MyMathHelper.Clamp((int) number, 1, 100);
      this.ResetToggleItems();
    }

    private void OnTreeDensityMaxEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.TreeDensityMax = MyMathHelper.Clamp((int) number, 1, 100);
      this.ResetToggleItems();
    }

    private void OnOreDensityEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.OreDensity = (int) MathHelper.Clamp((float) number, 1f, 500f);
      this.ResetToggleItems();
    }

    private void TerrainTypeSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      SaveMapHead header = this.gameProperties.SaveGame.Header;
      switch (header.TerrainData.Biome)
      {
        case BiomeType.Flat:
          header.TerrainData.Biome = BiomeType.SemiAlphine;
          header.TerrainData.GroundBlock = Item.Grass;
          header.PassiveMobs = true;
          break;
        case BiomeType.Desert:
          header.TerrainData.Biome = BiomeType.Grasslands;
          header.TerrainData.GroundBlock = Item.Grass;
          header.PassiveMobs = true;
          break;
        case BiomeType.SemiAlphine:
          header.TerrainData.Biome = BiomeType.Desert;
          header.TerrainData.GroundBlock = Item.Sand;
          header.PassiveMobs = true;
          break;
        default:
          if (header.GameMode == GameMode.Creative)
          {
            header.TerrainData.Biome = BiomeType.Flat;
            header.PassiveMobs = false;
            break;
          }
          header.TerrainData.Biome = BiomeType.SemiAlphine;
          header.TerrainData.GroundBlock = Item.Grass;
          header.PassiveMobs = true;
          break;
      }
      this.gameProperties.BiomeType = header.TerrainData.Biome;
      this.biomeParams.Initialize(header.TerrainData.Biome, 294, this.gameProperties.IsNewMap ? new int?(header.MapSeed) : new int?());
      this.SetMapDimensions();
      this.ResetToggleItems();
    }

    private void SeaLevelSelectedLeftEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.gameProperties.SaveGame.Header.TerrainData.SeaLevel -= (ushort) 50;
      this.SetMapDimensions();
      this.ResetToggleItems();
    }

    private void SeaLevelSelectedRightEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.gameProperties.SaveGame.Header.TerrainData.SeaLevel += (ushort) 50;
      this.SetMapDimensions();
      this.ResetToggleItems();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawBottomBar()
    {
      if (!this.IsPreviewableBiome)
        return;
      Rectangle rect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight + 6, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, rect.Y - 8, this.MenuRect.Width, 1), Color.Gray);
      rect.Y -= 3;
      GraphicStatics.DrawInputIcon(this.SpriteBatch, PlayerInput.EventScriptY, rect);
      this.SpriteBatch.DrawString(this.Font, "Preview Terrain", new Vector2((float) (rect.X + 32), (float) (rect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }
  }
}
