// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GenerateOreOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GenerateOreOptionsScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private SliderValue density;
    private Action<GamerID, GenerateOptions> callback;

    public GenerateOreOptionsScreen(
      GameInstance instance,
      Player player,
      GenerateOptions options,
      Action<GamerID, GenerateOptions> callback)
      : base("Options", player)
    {
      GenerateOreOptionsScreen oreOptionsScreen = this;
      this.instance = instance;
      this.callback = callback;
      this.density = new SliderValue()
      {
        Value = options.Density,
        Range = 1f
      };
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, string.Format("Measure: {0} x {1} x {2}", (object) options.AreaSize.X, (object) options.AreaSize.Y, (object) options.AreaSize.Z)));
      blockMenuEntryList.Add((BlockMenuEntry) new SliderMenuEntry((BlockMenuScreen) this, player, "Density: ", this.density, 240));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Generate"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Cancel"));
      blockMenuEntryList[1].SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.density.Value = MathHelper.Clamp(this.density.Value - 0.025f, 0.1f, 1f));
      blockMenuEntryList[1].SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.density.Value = MathHelper.Clamp(this.density.Value + 0.025f, 0.1f, 1f));
      blockMenuEntryList[2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (callback == null)
          return;
        options.Density = oreOptionsScreen.density.Value;
        callback(player.GamerID, options);
        oreOptionsScreen.ExitScreen();
      });
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      blockMenuEntryList[0].IsEnabled = false;
      this.selectedEntry = 1;
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 478;
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
