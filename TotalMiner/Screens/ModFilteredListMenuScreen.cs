// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ModFilteredListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ModFilteredListMenuScreen : BlockMenuScreen
  {
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private Func<string, bool> isActiveMod;
    private Func<string, bool> onSelected;

    public ModFilteredListMenuScreen(Func<string, bool> onSelected, ModFilter modFilter)
      : this(onSelected, modFilter, (Func<string, bool>) null)
    {
    }

    public ModFilteredListMenuScreen(
      Func<string, bool> onSelected,
      ModFilter modFilter,
      Func<string, bool> isActiveMod)
      : base("Mod List Menu", (Player) null)
    {
      this.onSelected = onSelected;
      this.isActiveMod = isActiveMod != null ? isActiveMod : new Func<string, bool>(this.IsActiveMod);
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (FileSystem.IsDirExist(ModManager.ModsPath))
      {
        foreach (string dir in FileSystem.GetDirs(ModManager.ModsPath))
        {
          string name = dir.Substring(dir.LastIndexOf('\\') + 1);
          if (ModManager.Matches(name, modFilter))
          {
            blockMenuEntryList.Add((BlockMenuEntry) new ModMenuEntry((BlockMenuScreen) this, name + ": ", isActiveMod(name)));
            blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ModEntrySelected);
          }
        }
      }
      if (blockMenuEntryList.Count == 0)
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "No mods found"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    private bool IsActiveMod(string modName)
    {
      return ModManager.IsActiveMod(modName);
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.ItemsPerPage = 18;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 32, 0, this.arrowTexture.Width, this.arrowTexture.Height);
    }

    private void ModEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.onSelected == null)
        return;
      ModMenuEntry modMenuEntry = sender as ModMenuEntry;
      if (!this.onSelected(modMenuEntry.Text.Substring(0, modMenuEntry.Text.IndexOf(':'))))
        return;
      this.ExitScreen();
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
      Rectangle rectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - 36, 24, 24);
      if (this.itemAtTopOfPage > 0)
      {
        this.arrowRect.Y = this.MenuRect.Y + 8;
        this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
      }
      if (this.itemAtTopOfPage + this.ItemsPerPage >= this.MenuEntries.Count)
        return;
      this.arrowRect.Y = this.MenuRect.Y + this.MenuRect.Height - 25;
      this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, Color.White);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
