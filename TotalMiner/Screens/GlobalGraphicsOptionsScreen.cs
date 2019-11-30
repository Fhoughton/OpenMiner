// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GlobalGraphicsOptionsScreen
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
  internal class GlobalGraphicsOptionsScreen : BlockMenuScreen
  {
    private GlobalGamerSettings gamerSettings;
    private GameSettings settings;
    private SliderValue viewDistance;
    private SliderValue textureSmoothing;

    public GlobalGraphicsOptionsScreen(PlayerIndex playerIndex)
      : base("Global Graphics Options", (Player) null)
    {
      this.gamerSettings = Globals2.GamertagData.GetGlobalGamerSettings(playerIndex);
      this.settings = this.gamerSettings.GameSettings;
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
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Texture Pack: " + this.settings.TexturePack));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Window Border: " + this.settings.WindowBorder));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, this.player, "View Distance: ", this.viewDistance, 288));
      blockMenuEntryList1.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, this.player, "Texture Smoothing: ", this.textureSmoothing, 288));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ScreenManager.AddScreen((GameScreen) new TexturePackMenuScreen((GameInstance) null, (Player) null, new Action<string>(this.TexturePackSelected), true, true), this.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ScreenManager.AddScreen((GameScreen) new WindowBorderTileListScreen(new Action<string>(this.BorderSelected)), this.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.SplitScreenVertical = !this.settings.SplitScreenVertical;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int index5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ShaderDetail = this.settings.ShaderDetail != ShaderDetail.Low ? ShaderDetail.Low : ShaderDetail.High;
        this.ResetToggleItems();
      });
      blockMenuEntryList1[index5].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.viewDistance.Value = MathHelper.Clamp(this.viewDistance.Value - 0.025f, 0.1f, 1f);
        this.settings.ViewDistance = this.viewDistance.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index6 = index5;
      int index7 = index6 + 1;
      blockMenuEntryList6[index6].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.viewDistance.Value = MathHelper.Clamp(this.viewDistance.Value + 0.025f, 0.25f, 1f);
        this.settings.ViewDistance = this.viewDistance.Value;
      });
      blockMenuEntryList1[index7].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.textureSmoothing.Value = MathHelper.Clamp(this.textureSmoothing.Value - 0.1f, 0.0f, 1f);
        this.settings.TextureSmoothing = this.textureSmoothing.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index8 = index7;
      int num5 = index8 + 1;
      blockMenuEntryList7[index8].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.textureSmoothing.Value = MathHelper.Clamp(this.textureSmoothing.Value + 0.1f, 0.0f, 1f);
        this.settings.TextureSmoothing = this.textureSmoothing.Value;
      });
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index9 = num5;
      int num6 = index9 + 1;
      blockMenuEntryList8[index9].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.UseMipMaps = !this.settings.UseMipMaps;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index10 = num6;
      int num7 = index10 + 1;
      blockMenuEntryList9[index10].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.OldskoolLight = !this.settings.OldskoolLight;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index11 = num7;
      int num8 = index11 + 1;
      blockMenuEntryList10[index11].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ViewClouds = !this.settings.ViewClouds;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index12 = num8;
      int num9 = index12 + 1;
      blockMenuEntryList11[index12].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.FloraAnimation = !this.settings.FloraAnimation;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index13 = num9;
      int num10 = index13 + 1;
      blockMenuEntryList12[index13].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.ToolTips = !this.settings.ToolTips;
        this.ResetToggleItems();
      });
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = "Texture Pack: " + this.settings.TexturePack;
      this.MenuEntries[1].Text = "Window Border: " + this.settings.WindowBorder;
      this.MenuEntries[2].Text = this.settings.SplitScreenVertical ? "Split Screen: Vertical" : "Split Screen: Horizontal";
      this.MenuEntries[2].IsEnabled = false;
      this.MenuEntries[3].Text = "Shader Detail: " + this.settings.ShaderDetail.ToString();
      this.MenuEntries[6].Text = "Texture Smoothing: " + (this.settings.UseMipMaps ? "On" : "Off");
      this.MenuEntries[7].Text = this.settings.OldskoolLight ? "Old Skool Light: On" : "Old Skool Light: Off";
      this.MenuEntries[8].Text = this.settings.ViewClouds ? "Show Clouds: On" : "Show Clouds: Off";
      this.MenuEntries[9].Text = this.settings.FloraAnimation ? "Flora Animation: On" : "Flora Animation: Off";
      this.MenuEntries[10].Text = this.settings.ToolTips ? "Tooltips: On" : "Tooltips: Off";
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

    private void TexturePackSelected(string texpack)
    {
      this.settings.TexturePack = texpack == "None" ? (string) null : texpack;
      this.ResetToggleItems();
    }

    private void BorderSelected(string border)
    {
      this.settings.WindowBorder = border;
      this.ResetToggleItems();
      GraphicStatics.LoadWindowBorder(border);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
