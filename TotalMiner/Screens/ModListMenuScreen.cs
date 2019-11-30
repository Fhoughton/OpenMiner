// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ModListMenuScreen
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
  internal class ModListMenuScreen : BlockMenuScreen
  {
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private Action<bool> onExit;
    private bool changed;

    public ModListMenuScreen()
      : this((Action<bool>) null)
    {
    }

    public ModListMenuScreen(Action<bool> onExit)
      : base("Mod List Menu", (Player) null)
    {
      this.onExit = onExit;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (FileSystem.IsDirExist(ModManager.ModsPath))
      {
        foreach (string dir in FileSystem.GetDirs(ModManager.ModsPath))
        {
          string name = dir.Substring(dir.LastIndexOf('\\') + 1);
          blockMenuEntryList.Add((BlockMenuEntry) new ModMenuEntry((BlockMenuScreen) this, name + ": ", ModManager.IsActiveMod(name)));
          blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ModEntrySelected);
        }
      }
      if (blockMenuEntryList.Count == 0)
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "No mods found"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.ItemsPerPage = 15;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 32, 0, this.arrowTexture.Width, this.arrowTexture.Height);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.onExit == null)
        return;
      this.onExit(this.changed);
    }

    private void ModEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      ModMenuEntry modMenuEntry = sender as ModMenuEntry;
      string name = modMenuEntry.Text.Substring(0, modMenuEntry.Text.IndexOf(':'));
      if (ModManager.IsActiveMod(name))
      {
        ModManager.UnloadMod(name);
        modMenuEntry.IsActive = false;
        this.changed = true;
      }
      else
      {
        string errorMessage;
        if (ModManager.LoadMod(name, out errorMessage) != null)
        {
          modMenuEntry.IsActive = true;
          this.changed = true;
        }
        else
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This mod could not be loaded: " + name + "\n" + errorMessage, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.ControllingPlayer.Value));
      }
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
