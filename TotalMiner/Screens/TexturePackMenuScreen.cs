// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TexturePackMenuScreen
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
  internal class TexturePackMenuScreen : BlockMenuScreen
  {
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private GameInstance instance;
    private Action<string> callback;

    public TexturePackMenuScreen(GameInstance instance, Player player)
      : this(instance, player, (Action<string>) null, false, false)
    {
    }

    public TexturePackMenuScreen(
      GameInstance instance,
      Player player,
      Action<string> callback,
      bool isPopup,
      bool includeNoneOption)
      : base("Texture Packs", player)
    {
      this.instance = instance;
      this.callback = callback;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      this.IsPopup = isPopup;
      if (includeNoneOption)
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "None"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Original"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Original Remade"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Original HD"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Original Autumn HD"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Original Winter HD"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Original Spring HD"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Steampunk HD"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Rupture HD by Gr1mT1m3Z"));
      foreach (string file in TitleFileSystem.GetFiles(CoreGlobals.Content.RootDirectory + "\\Textures\\", "tp_*.png"))
      {
        int num1 = file.ToLower().IndexOf("tp_");
        int num2 = file.ToLower().IndexOf(".png");
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, file.Substring(num1 + 3, num2 - (num1 + 3))));
      }
      foreach (string file in TitleFileSystem.GetFiles(CoreGlobals.Content.RootDirectory + "\\Textures\\", "tp_*.xnb"))
      {
        int num1 = file.ToLower().IndexOf("tp_");
        int num2 = file.ToLower().IndexOf(".xnb");
        string text = file.Substring(num1 + 3, num2 - (num1 + 3));
        bool flag = false;
        foreach (MenuEntry menuEntry in blockMenuEntryList)
        {
          if (menuEntry.Text == text)
          {
            flag = true;
            break;
          }
        }
        if (!flag && (!text.StartsWith("AvatarPalette") || player != null && player.IsGod))
          blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, text));
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      for (int index = 0; index < blockMenuEntryList.Count - 1; ++index)
        blockMenuEntryList[index].Selected += new EventHandler<PlayerIndexEventArgs>(this.TexturePackSelected);
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.ItemsPerPage = 14;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 32, 0, this.arrowTexture.Width, this.arrowTexture.Height);
    }

    private void TexturePackSelected(object sender, PlayerIndexEventArgs e)
    {
      MenuEntry menuEntry = sender as MenuEntry;
      if (menuEntry != null)
      {
        if (this.instance != null)
          this.instance.LoadTexturePack(menuEntry.Text, true, false, true);
        else if (this.callback != null)
          this.callback(menuEntry.Text);
      }
      if (!this.IsPopup)
        this.ScreenManager.ExitAllPlayerScreens();
      else
        this.ExitScreen();
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
      Rectangle rectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - 36, 24, 24);
      if (this.itemAtTopOfPage > 0)
      {
        this.arrowRect.Y = this.MenuRect.Y + 64;
        this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
      }
      if (this.itemAtTopOfPage + this.ItemsPerPage >= this.MenuEntries.Count)
        return;
      this.arrowRect.Y = this.MenuRect.Y + this.MenuRect.Height - 80;
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
