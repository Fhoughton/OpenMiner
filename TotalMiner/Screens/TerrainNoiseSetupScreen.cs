// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TerrainNoiseSetupScreen
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
  internal class TerrainNoiseSetupScreen : BlockMenuScreen
  {
    private GameProperties gameProperties;
    private BiomeParams biomeParams;
    private Action onRefresh;

    public TerrainNoiseSetupScreen(GameProperties gameProperties, Action onRefresh)
      : base("Terrain Noise Setup", (Player) null)
    {
      this.onRefresh = onRefresh;
      this.gameProperties = gameProperties;
      this.biomeParams = gameProperties.SaveGame.Header.BiomeParams;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Biome: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Big Detai Noise: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Big Detail Multiplier: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Medium Detail Noise: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Medium Detail Multiplier: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fine Detail Noise: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fine Detail Multiplier: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Total Noise Divisor: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].IsEnabled = false;
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.BigDetailNoiseSelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.BigDetailMultiplierSelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.MediumDetailNoiseSelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.MediumDetailMultiplierSelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num6;
      int num7 = index6 + 1;
      blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.FineDetailNoiseSelected);
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index7 = num7;
      int num8 = index7 + 1;
      blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.FineDetailMultiplierSelected);
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index8 = num8;
      int num9 = index8 + 1;
      blockMenuEntryList9[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.TotalNoiseDivisorSelected);
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index9 = num9;
      int num10 = index9 + 1;
      blockMenuEntryList10[index9].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 408;
      this.ItemsPerPage = 20;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
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
          case BiomeType.Desert:
          case BiomeType.SemiAlphine:
            return true;
          default:
            return biome == BiomeType.Grasslands;
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
          case BiomeType.SemiAlphine:
            return true;
          default:
            return biome == BiomeType.Grasslands;
        }
      }
    }

    private void ResetToggleItems()
    {
      int num1 = 0;
      List<MenuEntry> menuEntries1 = this.MenuEntries;
      int index1 = num1;
      int num2 = index1 + 1;
      menuEntries1[index1].Text = "Terrain: " + this.gameProperties.SaveGame.Header.TerrainData.Biome.ToString();
      List<MenuEntry> menuEntries2 = this.MenuEntries;
      int index2 = num2;
      int num3 = index2 + 1;
      menuEntries2[index2].Text = "Big Feature Noise: " + (object) this.biomeParams.BigDetailNoise;
      List<MenuEntry> menuEntries3 = this.MenuEntries;
      int index3 = num3;
      int num4 = index3 + 1;
      menuEntries3[index3].Text = "Big Feature Multiplier: " + (object) this.biomeParams.BigDetailMultiplier;
      List<MenuEntry> menuEntries4 = this.MenuEntries;
      int index4 = num4;
      int num5 = index4 + 1;
      menuEntries4[index4].Text = "Medium Feature Noise: " + (object) this.biomeParams.MediumDetailNoise;
      List<MenuEntry> menuEntries5 = this.MenuEntries;
      int index5 = num5;
      int num6 = index5 + 1;
      menuEntries5[index5].Text = "Medium Feature Multiplier: " + (object) this.biomeParams.MediumDetailMultiplier;
      List<MenuEntry> menuEntries6 = this.MenuEntries;
      int index6 = num6;
      int num7 = index6 + 1;
      menuEntries6[index6].Text = "Fine Feature Noise: " + (object) this.biomeParams.FineDetailNoise;
      List<MenuEntry> menuEntries7 = this.MenuEntries;
      int index7 = num7;
      int num8 = index7 + 1;
      menuEntries7[index7].Text = "Fine Feature Multiplier: " + (object) this.biomeParams.FineDetailMultiplier;
      List<MenuEntry> menuEntries8 = this.MenuEntries;
      int index8 = num8;
      int num9 = index8 + 1;
      menuEntries8[index8].Text = "Total Noise Divisor: " + (object) this.biomeParams.TotalNoiseDivisor;
      if (this.gameProperties.IsNewMap && this.IsEditableBiome)
        return;
      this.selectedEntry = this.MenuEntries.Count - 1;
      for (int index9 = 0; index9 < this.selectedEntry; ++index9)
        this.MenuEntries[index9].IsEnabled = false;
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

    private void BigDetailNoiseSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnBigDetailNoiseEntered), this.biomeParams.BigDetailNoise, true, false), this.ControllingPlayer);
    }

    private void BigDetailMultiplierSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnBigDetailMultiplierEntered), this.biomeParams.BigDetailMultiplier, true, false), this.ControllingPlayer);
    }

    private void MediumDetailNoiseSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnMediumDetailNoiseEntered), this.biomeParams.MediumDetailNoise, true, false), this.ControllingPlayer);
    }

    private void MediumDetailMultiplierSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnMediumDetailMultiplierEntered), this.biomeParams.MediumDetailMultiplier, true, false), this.ControllingPlayer);
    }

    private void FineDetailNoiseSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnFineDetailNoiseEntered), this.biomeParams.FineDetailNoise, true, false), this.ControllingPlayer);
    }

    private void FineDetailMultiplierSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnFineDetailMultiplierEntered), this.biomeParams.FineDetailMultiplier, true, false), this.ControllingPlayer);
    }

    private void TotalNoiseDivisorSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen((Player) null, new NumberEntered(this.OnTotalNoiseDivisorEntered), this.biomeParams.TotalNoiseDivisor, true, false), this.ControllingPlayer);
    }

    private void OnBigDetailNoiseEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.BigDetailNoise = MathHelper.Clamp((float) number, 10f, 10000f);
      this.ResetToggleItems();
    }

    private void OnBigDetailMultiplierEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.BigDetailMultiplier = MathHelper.Clamp((float) number, 1f, 100f);
      this.ResetToggleItems();
    }

    private void OnMediumDetailNoiseEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.MediumDetailNoise = MathHelper.Clamp((float) number, 1f, 1000f);
      this.ResetToggleItems();
    }

    private void OnMediumDetailMultiplierEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.MediumDetailMultiplier = MathHelper.Clamp((float) number, 0.1f, 10f);
      this.ResetToggleItems();
    }

    private void OnFineDetailNoiseEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.FineDetailNoise = MathHelper.Clamp((float) number, 0.1f, 100f);
      this.ResetToggleItems();
    }

    private void OnFineDetailMultiplierEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.FineDetailMultiplier = MathHelper.Clamp((float) number, 1f / 1000f, 10f);
      this.ResetToggleItems();
    }

    private void OnTotalNoiseDivisorEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number <= 0.0)
        return;
      this.biomeParams.TotalNoiseDivisor = MathHelper.Clamp((float) number, 1f, 100f);
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
