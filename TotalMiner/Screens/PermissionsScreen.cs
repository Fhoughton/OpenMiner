// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PermissionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class PermissionsScreen : BlockMenuScreen
  {
    public readonly int MaxColumns = 8;
    public int Column;

    public PermissionsScreen(GameInstance instance, Player player)
      : base("Player Permissions", player)
    {
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Player                 Adventure  Edit  Creative  Fly  Map  Voice  Spectate  Other"));
      if (player.IsHost || player.IsAdmin)
      {
        PermissionMenuEntry permissionMenuEntry1 = new PermissionMenuEntry(this, instance, player, (Player) null);
        blockMenuEntryList.Add((BlockMenuEntry) permissionMenuEntry1);
        foreach (Gamer allGamer in NetworkManager.Instance.AllGamers)
        {
          Player tag = allGamer.Tag as Player;
          if (tag != null)
          {
            PermissionMenuEntry permissionMenuEntry2 = new PermissionMenuEntry(this, instance, player, tag);
            blockMenuEntryList.Add((BlockMenuEntry) permissionMenuEntry2);
          }
        }
      }
      else
      {
        PermissionMenuEntry permissionMenuEntry = new PermissionMenuEntry(this, instance, player, player);
        blockMenuEntryList.Add((BlockMenuEntry) permissionMenuEntry);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 1078;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.ItemsPerPage = 10;
      this.DrawLastLine = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    public override bool HandleInput(InputState input)
    {
      if (!input.IsNewButtonPress(Buttons.Y, this.ControllingPlayer.Value))
        return base.HandleInput(input);
      this.ScreenManager.AddScreen((GameScreen) new HowToScreen(this.player, "Permissions", this.GetPermissionText() + "\n"), this.ControllingPlayer);
      return true;
    }

    private string GetPermissionText()
    {
      switch (this.Column)
      {
        case 1:
          return HowToMenuScreen.AdventurePermissionText;
        case 2:
          return HowToMenuScreen.EditPermissionText;
        case 3:
          return HowToMenuScreen.CreativePermissionText;
        case 4:
          return HowToMenuScreen.FlyPermissionText;
        case 5:
          return HowToMenuScreen.MapPermissionText;
        case 6:
          return HowToMenuScreen.VoiceChatPermissionText;
        case 7:
          return HowToMenuScreen.SpectatePermissionText;
        default:
          return HowToMenuScreen.PermissionHowToText;
      }
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 120, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Info", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
