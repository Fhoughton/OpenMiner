// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainCharacterMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Achievements;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainCharacterMenu : NewGuiMenu2
  {
    private Vector3 modelYPR;
    private WindowMapModel winModel;
    private bool dragStart;
    private Point dragAvatarMouseStartPos;

    public override string Name
    {
      get
      {
        return "Character";
      }
    }

    public MainCharacterMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private void InitMainContainer()
    {
      PlayerStats.Stat[] playerStatsAsText = this.player.GetPlayerStatsAsText();
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -100;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int y1 = 110;
      int width1 = 250;
      int width2 = 150;
      int height1 = 34;
      int num1 = 4;
      int num2 = 8 + playerStatsAsText.Length;
      int height2 = height1 * num2 + num1 * (num2 - 1);
      float textScale = 0.6f;
      int width3 = (int) ((double) (winRect.Height - 200) * 0.714285731315613);
      int num3 = 50;
      Window window1 = new Window((string) null, winRect.Width / 2 - (width1 + 1 + width2) - width3 / 2 - num3, y1, width1 + 1 + width2, height2)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window1);
      int y2;
      int x1 = y2 = 0;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window2;
      this.initialNavigable = window2 = (Window) new TextBox("Escape", x1, y2, width1, height1, textScale);
      window2.IsEnabled = this.player.IsEscapeEnabled;
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(this.ClickEscape);
      window1.AddChild((StudioForge.Engine.Core.Node) window2);
      int y3 = y2 + (height1 + num1);
      Window window3 = (Window) new TextBox("Skills", x1, y3, width1, height1, textScale);
      window3.IsEnabled = Globals2.GameProperties.SaveGame.Header.SkillsEnabled;
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window3.ClickHandler += new Window.WindowHandler(this.ClickSkills);
      window1.AddChild((StudioForge.Engine.Core.Node) window3);
      int y4 = y3 + (height1 + num1);
      Window window4 = (Window) new TextBox("Permissions", x1, y4, width1, height1, textScale);
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickPermissions);
      window1.AddChild((StudioForge.Engine.Core.Node) window4);
      int y5 = y4 + (height1 + num1);
      Window window5 = (Window) new TextBox("Text Message", x1, y5, width1, height1, textScale);
      window5.IsEnabled = this.player.HasPermission(Permissions.TextChat) || this.IsGodOrTester;
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window5.ClickHandler += new Window.WindowHandler(this.ClickTextMessage);
      window1.AddChild((StudioForge.Engine.Core.Node) window5);
      int y6 = y5 + (height1 + num1);
      Window window6 = (Window) new TextBox("Teleport to Player", x1, y6, width1, height1, textScale);
      window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window6.IsEnabled = this.IsGodOrTester || this.player.IsAdmin && this.instance.IsCreativeMode && this.instance.PlayerEnabledCount > 1;
      window6.ClickHandler += new Window.WindowHandler(this.ClickTeleportToPlayer);
      window1.AddChild((StudioForge.Engine.Core.Node) window6);
      int y7 = y6 + (height1 + num1);
      Window window7 = (Window) new TextBox("Teleport to Marker", x1, y7, width1, height1, textScale);
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window7.IsEnabled = this.IsGodOrTester || this.player.IsAdmin && this.instance.IsCreativeMode && this.instance.MapMarkers.Count > 0;
      window7.ClickHandler += new Window.WindowHandler(this.ClickTeleportToMarker);
      window1.AddChild((StudioForge.Engine.Core.Node) window7);
      int y8 = y7 + (height1 + num1);
      Window window8 = (Window) new TextBox("Change Log", x1, y8, width1, height1, textScale);
      window8.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window8.ClickHandler += new Window.WindowHandler(this.ClickChangeLog);
      window1.AddChild((StudioForge.Engine.Core.Node) window8);
      int y9 = y8 + (height1 + num1) + (height1 + num1);
      foreach (PlayerStats.Stat stat in playerStatsAsText)
      {
        Window window9 = (Window) new TextBox(stat.Desc, x1, y9, width1, height1, textScale);
        window9.Colors = (Window.ColorProfile) Colors.LabelColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window9);
        Window window10 = (Window) new TextBox(stat.Value.ToString(), x1 + width1 + 1, y9, width2, height1, textScale, WinTextAlignX.Right, WinTextAlignY.Center);
        window10.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window10);
        y9 += height1 + num1;
      }
      float modelScale = (float) (0.140000000596046 * (520.0 / (double) (winRect.Height - 200)));
      this.winModel = new WindowMapModel((string) null, winRect.Width / 2 - width3 / 2, 100, width3, winRect.Height - 200, modelScale);
      this.winModel.Colors = Window.TransparentColorProfile;
      this.winModel.SetYPR(this.modelYPR = new Vector3(3.141593f, 0.0f, 0.0f));
      this.winModel.SetAsset(this.player.AvatarModel.ComName);
      this.winModel.ClickDownHandler += new Window.WindowHandler(this.DragAvatar);
      this.winModel.ClickHandler += new Window.WindowHandler(this.DragEndAvatar);
      this.canvas.AddChild((StudioForge.Engine.Core.Node) this.winModel);
      List<Unlockable> unlockableList = new List<Unlockable>((IEnumerable<Unlockable>) this.player.Unlockables.UnlockableList);
      unlockableList.Sort(new Comparison<Unlockable>(this.SortUnlockablesByName));
      int num4 = 3;
      int x2 = winRect.Width / 2 + width3 / 2 + num3;
      int y10 = 110;
      int width4 = 200;
      Window window11 = new Window((string) null, x2, y10, width4 * num4 + num4 - 1, 0)
      {
        Name = "unlocksContainer"
      };
      window11.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window11);
      int y11;
      int x3 = y11 = 0;
      int num5 = 0;
      int num6 = 0;
      foreach (Unlockable unlockable in unlockableList)
      {
        if (unlockable != null && !unlockable.IsNPC)
        {
          Window window9 = (Window) new TextBox(unlockable.ActorType.ToString(), x3, y11, width4, height1, textScale);
          window9.IsEnabled = Player.IsActorTypeValidForAvatar(unlockable.ActorType);
          window9.Colors = (Window.ColorProfile) Colors.ButtonColors;
          window9.Tag = (object) unlockable;
          window9.ClickHandler += new Window.WindowHandler(this.ClickUnlockable);
          window11.AddChild((StudioForge.Engine.Core.Node) window9);
          x3 += width4 + 1;
          ++num6;
          if (++num5 == num4)
          {
            y11 += height1 + num1;
            x3 = 0;
            num5 = 0;
          }
        }
      }
      int num7 = num6 / num4;
      int num8 = height1 * num7 + num1 * (num7 - 1);
      window11.Size.Y = num8;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
    }

    private int SortUnlockablesByName(Unlockable u1, Unlockable u2)
    {
      return u1.ActorType.ToString().CompareTo(u2.ActorType.ToString());
    }

    protected override bool HandleInput()
    {
      Vector2 gamepadRightStick = InputManager.GetGamepadRightStick(this.playerIndex);
      if ((double) gamepadRightStick.X != 0.0)
      {
        this.modelYPR.X += gamepadRightStick.X * 0.1f;
        this.winModel.SetYPR(this.modelYPR);
      }
      return base.HandleInput();
    }

    private void ClickEscape(object sender, WindowEventArgs e)
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Escape\n\nUse this option if you are trapped\nunderground and cannot escape.\n\nWarning! All your items will be dropped\nbefore you are transported to the surface!", (string) null, "Yes, take me to the surface!", (string) null, "No, I'll stay where I am", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonB += (EventHandler<PlayerIndexEventArgs>) ((o, pe) => CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound));
      messageBoxScreenTm.ButtonX += (EventHandler<PlayerIndexEventArgs>) ((o, pe) =>
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuSelectSound);
        this.instance.PlayerEscape(e.PlayerIndex);
        this.ExitScreen();
      });
      this.screenManager.AddScreen((GameScreen) messageBoxScreenTm, new PlayerIndex?(this.playerIndex));
    }

    private void ClickSkills(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new SkillsScreen(this.player, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickPermissions(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new PermissionsScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickTextMessage(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new TextMessageMenuScreen(this.instance, this.player, (string) null), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickTeleportToPlayer(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnTeleportToPlayer), true, this.player.Gamer.Gamertag, false, false), new PlayerIndex?(this.playerIndex));
    }

    private void OnTeleportToPlayer(NetworkGamer gamer, bool allGamers, string text)
    {
      Player tag = gamer.Tag as Player;
      if (tag == null)
        return;
      this.player.TeleportTo((Actor) tag);
      this.ExitScreen();
    }

    private void ClickTeleportToMarker(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new MapMarkerListScreen(this.instance, this.player, new Action<string>(this.OnTeleportToMarker), true), new PlayerIndex?(this.playerIndex));
    }

    private void OnTeleportToMarker(string markerLabel)
    {
      MapMarker? mapMarker = this.instance.GetMapMarker(markerLabel);
      if (!mapMarker.HasValue)
        return;
      this.player.TeleportTo(mapMarker.Value.Point, true);
      this.ExitScreen();
    }

    private void ClickChangeLog(object sender, WindowEventArgs e)
    {
      ChangeLogScreen changeLogScreen = new ChangeLogScreen(this.player, this.player.ChangeLog);
      changeLogScreen.IsPopup = true;
      this.screenManager.AddScreen((GameScreen) changeLogScreen, new PlayerIndex?(this.playerIndex));
    }

    private void ClickUnlockable(object sender, WindowEventArgs e)
    {
      Unlockable tag = e.Window.Tag as Unlockable;
      if (tag == null)
        return;
      this.winModel.SetAsset(tag.ActorType.ToString());
      this.player.SetAvatar(this.player, tag.ActorType);
    }

    private void DragAvatar(object sender, WindowEventArgs e)
    {
      if (!this.dragStart)
      {
        this.dragAvatarMouseStartPos = e.MousePosition;
        this.dragStart = true;
      }
      else
      {
        this.modelYPR.X += (float) (e.MousePosition.X - this.dragAvatarMouseStartPos.X) * 0.01f;
        this.dragAvatarMouseStartPos = e.MousePosition;
        this.winModel.SetYPR(this.modelYPR);
      }
    }

    private void DragEndAvatar(object sender, WindowEventArgs e)
    {
      this.dragStart = false;
    }
  }
}
