// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MultiplayerOptionsMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class MultiplayerOptionsMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private NetworkGamer gamerToKick;
    private NetworkGamer gamerToJail;
    private byte rating;
    private Texture2D starTexture;

    public MultiplayerOptionsMenuScreen(GameInstance instance, Player player)
      : base("Multiplayer Options", player)
    {
      MultiplayerOptionsMenuScreen optionsMenuScreen = this;
      this.instance = instance;
      this.rating = player.SaveState.RatingStars;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Kick Player"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Rate World: "));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Set Spawn Inventory"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Reset All Player Spawn Points"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num;
      int index2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => optionsMenuScreen.ScreenManager.AddScreen((GameScreen) new GamerListScreen(player, new Action<NetworkGamer, bool, string>(optionsMenuScreen.OnKickGamertag), true, instance.NetworkManager.Session.Host.Gamertag, false, true), optionsMenuScreen.ControllingPlayer));
      blockMenuEntryList1[index2].SelectLeft += new EventHandler<PlayerIndexEventArgs>(this.RateTheWorldLeft);
      blockMenuEntryList1[index2].SelectRight += new EventHandler<PlayerIndexEventArgs>(this.RateTheWorldRight);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index3 = index2;
      int index4 = index3 + 1;
      blockMenuEntryList3[index3].IsEnabled = !NetworkManager.Instance.IsHost && player.HasPermission(Permissions.Adventure) && Globals2.GameProperties.SaveGame.Header.Attribute != MapAttribute.WorkInProgress && player.Statistics.SecondsPlayed >= 120.0;
      blockMenuEntryList1[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        bool flag = Globals2.GamertagData.IsServerFavourite(player.SignedInGamer);
        Globals2.GamertagData.FlagServerAsFavourite(player.SignedInGamer, !flag);
        if (!flag)
          NetworkManager.Instance.SendWorldFavorited(player.GamerID);
        optionsMenuScreen.ResetMenuText();
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index5 = index4;
      int index6 = index5 + 1;
      blockMenuEntryList4[index5].IsEnabled = !NetworkManager.Instance.IsHost;
      blockMenuEntryList1[index6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        optionsMenuScreen.ScreenManager.AddScreen((GameScreen) new ShopScreen(instance, player, instance.SpawnInventory), optionsMenuScreen.ControllingPlayer);
        optionsMenuScreen.ExitScreen();
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index7 = index6;
      int index8 = index7 + 1;
      blockMenuEntryList5[index7].IsEnabled = player.IsHost && instance.IsCreativeMode;
      blockMenuEntryList1[index8].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        instance.ResetPlayerSpawnPoints();
        optionsMenuScreen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Spawn points have been reset", "Ok", (string) null, (string) null, (string) null, optionsMenuScreen.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), optionsMenuScreen.ControllingPlayer);
        optionsMenuScreen.ExitScreen();
      });
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index9 = index8;
      int index10 = index9 + 1;
      blockMenuEntryList6[index9].IsEnabled = player.IsHost;
      blockMenuEntryList1[index10].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 430;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.starTexture = this.content.Load<Texture2D>("Textures\\RatingStar");
      this.ResetMenuText();
    }

    private void ResetMenuText()
    {
      this.MenuEntries[2].Text = "Flag as Favourite: " + (Globals2.GamertagData.IsServerFavourite(this.player.SignedInGamer) ? "Yes" : "No");
    }

    private void OnKickGamertag(NetworkGamer gamer, bool allGamers, string text)
    {
      this.gamerToKick = gamer;
      Player tag = gamer.Tag as Player;
      if (tag != null && tag.IsDeveloper)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot kick developers.\n\nDo not worry, developers will never grief.", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), tag), this.ControllingPlayer);
      }
      else
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Kick: " + gamer.Gamertag + "?", (string) null, "Yes Kick!", (string) null, "No", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), tag);
        messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.OnKickGamertag);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
    }

    private void OnKickGamertag(object sender, PlayerIndexEventArgs e)
    {
      NetworkManager.Instance.KickGamer(this.gamerToKick, false);
      this.ExitScreen();
    }

    private void OnJailGamertag(NetworkGamer gamer)
    {
      this.gamerToJail = gamer;
      Player tag = gamer.Tag as Player;
      if (tag != null && tag.IsDeveloper)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot send developers to jail.", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), tag), this.ControllingPlayer);
      }
      else
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Send: " + gamer.Gamertag + " to Jail?", (string) null, "Yes!", (string) null, "No", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), tag);
        messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.OnJailGamertag);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
    }

    private void OnJailGamertag(object sender, PlayerIndexEventArgs e)
    {
      NetworkManager.Instance.SendGamerToJail(this.gamerToJail, false);
      this.ExitScreen();
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
        bool flag = this.player.SaveState.RatingStars == (byte) 0;
        this.player.SaveState.RatingStars = this.rating;
        NetworkManager.Instance.SendRatingVote(this.rating, this.player.Gamer.ID);
        Globals2.GamertagData.AddServerRating(this.player.SignedInGamer, this.rating);
        if (flag)
          this.player.Raise_RatedWorld(this.player);
      }
      base.OnCancel(playerIndex);
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
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + 168, this.MenuRect.Y + 105, 24, 24);
      for (int index = 1; index < 6; ++index)
      {
        this.SpriteBatch.Draw(this.starTexture, destinationRectangle, index <= (int) this.rating ? Color.Yellow : Color.LightGray);
        destinationRectangle.X += 30;
      }
    }
  }
}
