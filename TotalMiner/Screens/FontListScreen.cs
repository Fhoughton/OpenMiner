// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.FontListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class FontListScreen : BlockMenuScreen
  {
    public FontListScreen()
      : base("Font List", (Player) null)
    {
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Default"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Georgia"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Khmer UI"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Leelawadee"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Meiryo UI"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Miramonte"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Miriam"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Tahoma"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Trebuchet MS"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Verdana"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Wasco Sans"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelect);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    private void OnSelect(object sender, PlayerIndexEventArgs e)
    {
      string text = ((MenuEntry) sender).Text;
      try
      {
        this.Font = this.ItemFont = this.ScreenManager.GameFont = CoreGlobals.Content.Load<SpriteFont>(Services.FontPath + text);
        CoreGlobals.ClearReferenceCache();
      }
      catch (Exception ex)
      {
      }
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
