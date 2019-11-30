// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.WindowBorderTileListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class WindowBorderTileListScreen : BlockMenuScreen
  {
    private Action<string> onSelected;

    public WindowBorderTileListScreen(Action<string> onSelected)
      : base("Window Border Tile List", (Player) null)
    {
      this.onSelected = onSelected;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Blade1"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Blade2"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Blade3"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Circles"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "GoldBlade"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "PurpleBanner"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "OrangeBanner"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Spiders"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fuller"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    private void OnSelect(object sender, PlayerIndexEventArgs e)
    {
      this.onSelected(this.MenuEntries[this.selectedEntry].Text);
      this.ExitScreen();
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 332;
      this.ItemsPerPage = 10;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
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
