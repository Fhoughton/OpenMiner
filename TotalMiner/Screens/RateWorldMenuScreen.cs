// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.RateWorldMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class RateWorldMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private byte rating;
    private Texture2D starTexture;
    private Action callBack;

    public RateWorldMenuScreen(GameInstance instance, Player player, Action callBack)
      : base("Rate World", player)
    {
      this.instance = instance;
      this.callBack = callBack;
      this.rating = player.SaveState.RatingStars;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Please take a moment to Rate this World"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Rate World: "));
      blockMenuEntryList[1].SelectLeft += new EventHandler<PlayerIndexEventArgs>(this.RateTheWorldLeft);
      blockMenuEntryList[1].SelectRight += new EventHandler<PlayerIndexEventArgs>(this.RateTheWorldRight);
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.selectedEntry = 1;
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 622;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.starTexture = this.content.Load<Texture2D>("Textures\\RatingStar");
    }

    private void RateTheWorldLeft(object sender, PlayerIndexEventArgs e)
    {
      this.rating = (byte) MyMathHelper.Clamp((int) this.rating - 1, 1, 5);
    }

    private void RateTheWorldRight(object sender, PlayerIndexEventArgs e)
    {
      this.rating = (byte) MyMathHelper.Clamp((int) this.rating + 1, 1, 5);
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      if ((int) this.rating != (int) this.player.SaveState.RatingStars && this.player.Gamer != null)
      {
        this.player.SaveState.RatingStars = this.rating;
        NetworkManager.Instance.SendRatingVote(this.rating, this.player.Gamer.ID);
        if (Globals2.GamertagData.AddServerRating(this.player.SignedInGamer, this.rating))
          this.player.Raise_RatedWorld(this.player);
      }
      base.OnCancel(playerIndex);
      this.callBack();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawMenuExtra()
    {
      base.DrawMenuExtra();
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + 194, this.MenuRect.Y + this.ItemHeight + this.ItemGapY + 10, 24, 24);
      for (int index = 1; index < 6; ++index)
      {
        this.SpriteBatch.Draw(this.starTexture, destinationRectangle, index <= (int) this.rating ? Color.Yellow : Color.LightGray);
        destinationRectangle.X += 30;
      }
    }
  }
}
