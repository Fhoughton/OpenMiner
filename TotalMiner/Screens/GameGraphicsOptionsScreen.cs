// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GameGraphicsOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GameGraphicsOptionsScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private GameSettings settings;
    private SliderValue viewDistance;
    private SliderValue textureSmoothing;

    public GameGraphicsOptionsScreen(GameInstance instance, Player player)
      : base("Graphics Options", player)
    {
      GameGraphicsOptionsScreen graphicsOptionsScreen = this;
      this.instance = instance;
      this.settings = Globals2.GameSettings;
      this.viewDistance = new SliderValue()
      {
        Value = this.settings.ViewDistance,
        Range = 1f
      };
      this.textureSmoothing = new SliderValue()
      {
        Value = this.settings.TextureSmoothing,
        Range = 1f
      };
      List<BlockMenuEntry> items = new List<BlockMenuEntry>();
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "View Distance: ", this.viewDistance, 288));
      items.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Texture Smoothing: ", this.textureSmoothing, 288));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Rebuild Local Light"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "View Clan Banners"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clear All Pickups (Reduce Lag)"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Adjust HUD Position"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int index1 = 0;
      items[index1].IsEnabled = false;
      List<BlockMenuEntry> blockMenuEntryList1 = items;
      int index2 = index1;
      int num1 = index2 + 1;
      blockMenuEntryList1[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        graphicsOptionsScreen.settings.SplitScreenVertical = !graphicsOptionsScreen.settings.SplitScreenVertical;
        graphicsOptionsScreen.ScreenManager.ExitAllPlayerScreens();
        instance.ResetPlayerViewports();
      });
      List<BlockMenuEntry> blockMenuEntryList2 = items;
      int index3 = num1;
      int index4 = index3 + 1;
      blockMenuEntryList2[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ShaderDetail = this.settings.ShaderDetail != ShaderDetail.Low ? ShaderDetail.Low : ShaderDetail.High;
        this.ResetToggleItems();
      });
      items[index4].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        graphicsOptionsScreen.viewDistance.Value = MathHelper.Clamp(graphicsOptionsScreen.viewDistance.Value - 0.025f, 0.1f, 1f);
        graphicsOptionsScreen.settings.ViewDistance = graphicsOptionsScreen.viewDistance.Value;
        instance.OnViewDistanceChanged();
      });
      List<BlockMenuEntry> blockMenuEntryList3 = items;
      int index5 = index4;
      int index6 = index5 + 1;
      blockMenuEntryList3[index5].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        graphicsOptionsScreen.viewDistance.Value = MathHelper.Clamp(graphicsOptionsScreen.viewDistance.Value + 0.025f, 0.25f, 1f);
        graphicsOptionsScreen.settings.ViewDistance = graphicsOptionsScreen.viewDistance.Value;
        instance.OnViewDistanceChanged();
      });
      items[index6].IsEnabled = GraphicStatics.TexturePack.BlockTextureSize() > 16;
      items[index6].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.textureSmoothing.Value = MathHelper.Clamp(this.textureSmoothing.Value - 0.1f, 0.0f, 1f);
        this.settings.TextureSmoothing = this.textureSmoothing.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList4 = items;
      int index7 = index6;
      int index8 = index7 + 1;
      blockMenuEntryList4[index7].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.textureSmoothing.Value = MathHelper.Clamp(this.textureSmoothing.Value + 0.1f, 0.0f, 1f);
        this.settings.TextureSmoothing = this.textureSmoothing.Value;
      });
      items[index8].IsEnabled = GraphicStatics.TexturePack.BlockTextureSize() > 16;
      List<BlockMenuEntry> blockMenuEntryList5 = items;
      int index9 = index8;
      int num2 = index9 + 1;
      blockMenuEntryList5[index9].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        //closure_0.settings.UseMipMaps = !closure_0.settings.UseMipMaps;
        //items[3].IsEnabled = closure_0.settings.UseMipMaps;
        //closure_0.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList6 = items;
      int index10 = num2;
      int index11 = index10 + 1;
      blockMenuEntryList6[index10].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        graphicsOptionsScreen.settings.OldskoolLight = !graphicsOptionsScreen.settings.OldskoolLight;
        instance.FlagMapChunksForMeshReload();
        graphicsOptionsScreen.ResetToggleItems();
      });
      items[index11].IsEnabled = player.IsAdmin;
      List<BlockMenuEntry> blockMenuEntryList7 = items;
      int index12 = index11;
      int num3 = index12 + 1;
      blockMenuEntryList7[index12].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        instance.RebuildLocalLight(player);
        graphicsOptionsScreen.ScreenManager.ExitAllPlayerScreens(graphicsOptionsScreen.ControllingPlayer);
      });
      List<BlockMenuEntry> blockMenuEntryList8 = items;
      int index13 = num3;
      int num4 = index13 + 1;
      blockMenuEntryList8[index13].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ViewClouds = !this.settings.ViewClouds;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList9 = items;
      int index14 = num4;
      int num5 = index14 + 1;
      blockMenuEntryList9[index14].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.FloraAnimation = !this.settings.FloraAnimation;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList10 = items;
      int index15 = num5;
      int num6 = index15 + 1;
      blockMenuEntryList10[index15].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ToolTips = !this.settings.ToolTips;
        GameScreen.SetToolTips(this.settings.ToolTips);
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList11 = items;
      int index16 = num6;
      int num7 = index16 + 1;
      blockMenuEntryList11[index16].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ViewSounds = !this.settings.ViewSounds;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList12 = items;
      int index17 = num7;
      int index18 = index17 + 1;
      blockMenuEntryList12[index17].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.ScreenManager.AddScreen((GameScreen) new ViewClanBannerScreen(), this.ControllingPlayer);
        this.ExitScreen();
      });
      items[index18].IsEnabled = player.IsAdmin;
      List<BlockMenuEntry> blockMenuEntryList13 = items;
      int index19 = index18;
      int num8 = index19 + 1;
      blockMenuEntryList13[index19].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        instance.ClearAllParticles(true);
        graphicsOptionsScreen.ScreenManager.ExitAllPlayerScreens(graphicsOptionsScreen.ControllingPlayer);
      });
      List<BlockMenuEntry> blockMenuEntryList14 = items;
      int index20 = num8;
      int num9 = index20 + 1;
      blockMenuEntryList14[index20].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.ScreenManager.ExitAllPlayerScreens();
        this.ScreenManager.AddScreen((GameScreen) new HUDAdjustScreen(), this.ControllingPlayer);
      });
      items[items.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) items.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = this.settings.SplitScreenVertical ? "Split Screen: Vertical" : "Split Screen: Horizontal";
      this.MenuEntries[1].Text = "Shader Detail: " + this.settings.ShaderDetail.ToString();
      this.MenuEntries[4].Text = "Texture Smoothing: " + (this.settings.UseMipMaps ? "On" : "Off");
      this.MenuEntries[5].Text = this.settings.OldskoolLight ? "Old Skool Light: On" : "Old Skool Light: Off";
      this.MenuEntries[7].Text = this.settings.ViewClouds ? "Show Clouds: On" : "Show Clouds: Off";
      this.MenuEntries[8].Text = this.settings.FloraAnimation ? "Flora Animation: On" : "Flora Animation: Off";
      this.MenuEntries[9].Text = this.settings.ToolTips ? "Tooltips: On" : "Tooltips: Off";
      this.MenuEntries[10].Text = this.settings.ViewSounds ? "Show Sounds: On" : "Show Sounds: Off";
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 574;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
