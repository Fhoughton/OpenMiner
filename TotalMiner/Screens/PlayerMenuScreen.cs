// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PlayerMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class PlayerMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;

    public PlayerMenuScreen(GameInstance instance, Player player)
      : base("Player Menu", player)
    {
      this.instance = instance;
      this.player = player;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Options"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Text Message"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Escape"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Skills"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Permissions"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Statistics"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Change Log"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Teleport to Marker"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Teleport to Player"));
      if (instance.IsDigDeepMode && player.IsGodOrTester)
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Teleport to Blueprint"));
      if (instance.IsDigDeepMode && player.IsGodOrTester)
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Teleport to Wisdom"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OptionsMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.TextMessageMenuEntrySelected);
      blockMenuEntryList1[num3 - 1].IsEnabled = player.HasPermission(Permissions.TextChat);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.EscapeMenuEntrySelected);
      blockMenuEntryList1[num4 - 1].IsEnabled = player.IsEscapeEnabled;
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.SkillsEntrySelected);
      blockMenuEntryList1[num5 - 1].IsEnabled = Globals2.GameProperties.SaveGame.Header.SkillsEnabled;
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.PermissionsEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num6;
      int num7 = index6 + 1;
      blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.StatisticsEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index7 = num7;
      int num8 = index7 + 1;
      blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.ChangeLogEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index8 = num8;
      int num9 = index8 + 1;
      blockMenuEntryList9[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.TeleportToMarkerMenuEntrySelected);
      blockMenuEntryList1[num9 - 1].IsEnabled = player.IsGodOrTester || player.IsAdmin && instance.IsCreativeMode;
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index9 = num9;
      int num10 = index9 + 1;
      blockMenuEntryList10[index9].Selected += new EventHandler<PlayerIndexEventArgs>(this.TeleportToPlayerMenuEntrySelected);
      blockMenuEntryList1[num10 - 1].IsEnabled = player.IsGodOrTester || player.IsAdmin && instance.IsCreativeMode && instance.PlayerEnabledCount > 1;
      if (instance.IsDigDeepMode && player.IsGodOrTester)
        blockMenuEntryList1[num10++].Selected += new EventHandler<PlayerIndexEventArgs>(this.TeleportToBlueprintMenuEntrySelected);
      if (instance.IsDigDeepMode && player.IsGodOrTester)
        blockMenuEntryList1[num10++].Selected += new EventHandler<PlayerIndexEventArgs>(this.TeleportToWisdomMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index10 = num10;
      int num11 = index10 + 1;
      blockMenuEntryList11[index10].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 335;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void EscapeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Escape\n\nUse this option if you are trapped\nunderground and cannot escape.\n\nWarning! All your items will be dropped\nbefore you are transported to the surface!", (string) null, "Yes, take me to the surface!", (string) null, "No, I'll stay where I am", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonB += (EventHandler<PlayerIndexEventArgs>) ((o, pe) =>
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
      });
      messageBoxScreenTm.ButtonX += (EventHandler<PlayerIndexEventArgs>) ((o, pe) =>
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuSelectSound);
        this.instance.PlayerEscape(e.PlayerIndex);
        this.ExitScreen();
      });
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void SkillsEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new SkillsScreen(this.player, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void TextMessageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new TextMessageMenuScreen(this.instance, this.player, (string) null), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new PlayerOptionsScreen(this.instance, this.player), this.ControllingPlayer);
    }

    private void PermissionsEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new PermissionsScreen(this.instance, this.player), this.ControllingPlayer);
    }

    private void StatisticsEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new StatsScreen(this.instance, this.player, false), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void ChangeLogEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnChangeLogGamerSelected), true, (string) null, false, false), this.ControllingPlayer);
    }

    private void OnChangeLogGamerSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      Player tag = gamer.Tag as Player;
      if (tag == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new ChangeLogScreen(this.player, tag.ChangeLog), this.ControllingPlayer);
    }

    private void TeleportToPlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnTeleportPlayer), true, this.player.Gamer.Gamertag, false, false), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void TeleportToMarkerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new MapMarkerListScreen(this.instance, this.player, new Action<string>(this.OnTeleportMarker), true), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void TeleportToBlueprintMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlueprintListScreen(this.instance, this.player, new Action<string>(this.OnTeleportBlueprint)), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void TeleportToWisdomMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new WisdomListScreen(this.instance, this.player, new Action<string>(this.OnTeleportWisdom)), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OnTeleportPlayer(NetworkGamer gamer, bool allGamers, string text)
    {
      Player tag = gamer.Tag as Player;
      if (tag == null)
        return;
      this.player.TeleportTo((Actor) tag);
    }

    private void OnTeleportMarker(string markerLabel)
    {
      MapMarker? mapMarker = this.instance.GetMapMarker(markerLabel);
      if (!mapMarker.HasValue)
        return;
      this.player.TeleportTo(mapMarker.Value.Point, true);
    }

    private void OnTeleportBlueprint(string bpName)
    {
      bpName = bpName.Substring(0, bpName.IndexOf(":"));
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (bpName == blueprint.Result.ItemID.ToString())
          this.player.TeleportTo(blueprint.Point + GlobalPoint3D.Backward, false);
      }
    }

    private void OnTeleportWisdom(string text)
    {
      int result;
      if (!int.TryParse(text.Substring(0, text.IndexOf(":")), out result))
        return;
      foreach (WisdomItem wisdom in Wisdom.WisdomList)
      {
        if ((int) wisdom.ID == result)
          this.player.TeleportTo(wisdom.Point + GlobalPoint3D.Backward, false);
      }
    }

    private void BotMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.instance.GetLocalPlayer(PlayerIndex.One).IsBot = !this.instance.GetLocalPlayer(PlayerIndex.One).IsBot;
      this.ExitScreen();
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      base.OnCancel(playerIndex);
      this.ScreenManager.AddScreen((GameScreen) new PauseMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
