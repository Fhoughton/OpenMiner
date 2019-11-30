// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PermissionMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class PermissionMenuEntry : BlockMenuEntry
  {
    private Player editingPlayer;
    private Player targetPlayer;
    private Texture2D checkboxOn;
    private Texture2D checkboxOff;
    private GameInstance instance;
    private PermissionsScreen screen;

    private bool IsDefaultPermission
    {
      get
      {
        return this.targetPlayer == null;
      }
    }

    private int MaxColumns
    {
      get
      {
        return this.screen.MaxColumns;
      }
    }

    private bool CanEdit
    {
      get
      {
        bool flag = this.editingPlayer.IsHost;
        if (!flag)
          flag = this.editingPlayer.IsAdmin && this.targetPlayer != null && !this.targetPlayer.IsHost && (this.editingPlayer == this.targetPlayer || !this.targetPlayer.IsAdmin) || this.editingPlayer == this.targetPlayer && this.screen.Column == 6 && this.editingPlayer.HasPermission(Permissions.VoiceChat);
        if (!flag)
          return this.EditOverride;
        return true;
      }
    }

    private bool EditOverride
    {
      get
      {
        return this.editingPlayer.IsGod;
      }
    }

    public PermissionMenuEntry(
      PermissionsScreen screen,
      GameInstance instance,
      Player editingPlayer,
      Player targetPlayer)
      : base((BlockMenuScreen) screen, PermissionMenuEntry.GTOrDefault(targetPlayer))
    {
      PermissionMenuEntry permissionMenuEntry = this;
      this.screen = screen;
      this.instance = instance;
      this.editingPlayer = editingPlayer;
      this.targetPlayer = targetPlayer;
      this.ColorHighlighted = Color.DarkGray;
      this.SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (--screen.Column != -1)
          return;
        screen.Column = permissionMenuEntry.MaxColumns;
      });
      this.SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (++screen.Column < permissionMenuEntry.MaxColumns + 1)
          return;
        screen.Column = 0;
      });
      if (instance != null && instance.MiniGame != null)
        this.Selected += new EventHandler<PlayerIndexEventArgs>(this.CantChangeInMiniGameMessageEventHandler);
      else
        this.Selected += new EventHandler<PlayerIndexEventArgs>(this.SelectedEventHandler);
      this.checkboxOn = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkboxOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
    }

    private void SelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      if (!this.CanEdit && this.screen.Column != 8)
      {
        this.ShowCantEditPermsMessage();
      }
      else
      {
        switch (this.screen.Column)
        {
          case 0:
            this.ToggleAllPermissions();
            break;
          case 1:
            this.TogglePermission(Permissions.Adventure);
            break;
          case 2:
            this.TogglePermission(Permissions.Edit);
            break;
          case 3:
            this.TogglePermission(Permissions.Creative);
            break;
          case 4:
            this.TogglePermission(Permissions.Fly);
            break;
          case 5:
            this.TogglePermission(Permissions.Map);
            break;
          case 6:
            this.TogglePermission(Permissions.VoiceChat);
            break;
          case 7:
            this.TogglePermission(Permissions.Spectate);
            break;
          case 8:
            this.screen.ScreenManager.AddScreen((GameScreen) new PermissionEditScreen(this.instance, this.editingPlayer, this.targetPlayer), this.screen.ControllingPlayer);
            return;
        }
        NetworkManager.Instance.SendPermissions(this.IsDefaultPermission ? GamerID.Sys1 : this.targetPlayer.GamerID, this.Permission, (NetworkGamer) null);
      }
    }

    private Permissions Permission
    {
      get
      {
        if (!this.IsDefaultPermission)
          return this.targetPlayer.Permission;
        if (Globals2.GameProperties == null)
          return Permissions.None;
        return Globals2.GameProperties.SaveGame.Header.DefaultPermission;
      }
    }

    private void TogglePermission(Permissions permission)
    {
      if ((permission & Permissions.Save) != Permissions.None && !this.editingPlayer.IsHost)
        return;
      if (this.IsDefaultPermission)
      {
        Globals2.TogglDefaultPermission(permission);
      }
      else
      {
        if (this.targetPlayer.IsHost && !this.editingPlayer.IsHost)
          return;
        this.targetPlayer.TogglePermission(permission);
      }
    }

    private void ToggleAllPermissions()
    {
      if (this.IsDefaultPermission)
      {
        this.ToggleAllDefaultPermissions();
      }
      else
      {
        if (this.targetPlayer.IsHost && !this.editingPlayer.IsHost)
          return;
        this.ToggleAllPlayerPermissions();
      }
    }

    private void ToggleAllDefaultPermissions()
    {
      if (Globals2.GameProperties == null || !NetworkManager.Instance.IsHost)
        return;
      if (Globals2.HasDefaultPermission(Permissions.Edit))
        Globals2.GameProperties.SaveGame.Header.DefaultPermission = Permissions.None;
      else
        Globals2.GameProperties.SaveGame.Header.DefaultPermission = Permissions.Adventure | Permissions.Edit | Permissions.Creative | Permissions.Fly | Permissions.Map;
    }

    private void ToggleAllPlayerPermissions()
    {
      bool flag = this.targetPlayer.HasPermission(Permissions.Admin);
      if (this.targetPlayer.HasPermission(Permissions.Adventure))
      {
        this.targetPlayer.Permission = Permissions.None;
      }
      else
      {
        this.targetPlayer.Permission = Permissions.Adventure | Permissions.Edit | Permissions.Creative | Permissions.Fly | Permissions.Map;
        if (this.targetPlayer.IsHost)
          this.targetPlayer.Permission |= Permissions.Save;
      }
      if (!flag)
        return;
      this.targetPlayer.Permission |= Permissions.Admin | Permissions.Grief;
    }

    private void ShowCantEditPermsMessage()
    {
      this.Screen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You do not have permission to edit this permission", "Ok", (string) null, (string) null, (string) null, this.Screen.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.editingPlayer), this.Screen.ControllingPlayer);
    }

    private void CantChangeInMiniGameMessageEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.Screen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot edit permissions\nwhile a mini-game is in progress", "Ok", (string) null, (string) null, (string) null, this.Screen.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.editingPlayer), this.Screen.ControllingPlayer);
    }

    public override void Update(MenuScreen screen, bool isSelected)
    {
      base.Update(screen, isSelected);
      if (this.screen.Column == 8)
        this.ToolTip.Text = "Press A to see other permissions not shown on this main permission screen.";
      else
        this.ToolTip.Text = (string) null;
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      Color color = this.IsEnabled ? (isSelected ? this.ColorSelected : this.ColorUnselected) : this.ColorDisabled;
      color = new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      this.DrawItem(position, color);
      this.DrawTexture(position, color);
    }

    private static string GTOrDefault(Player player)
    {
      if (player != null)
        return player.Gamer.Gamertag;
      return ".. default ..";
    }

    private bool HasPermission(Permissions permission)
    {
      if (!this.IsDefaultPermission)
        return this.targetPlayer.HasPermission(permission);
      return Globals2.HasDefaultPermission(permission);
    }

    private void DrawItem(Vector2 position, Color color)
    {
      position.X += 32f;
      position.Y += 9f;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, PermissionMenuEntry.GTOrDefault(this.targetPlayer), position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.Y -= 2f;
      position.X += 302f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.Adventure) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 106f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.Edit) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 94f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.Creative) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 89f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.Fly) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 62f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.Map) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 78f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.VoiceChat) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 98f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.Spectate) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 109f;
      this.Screen.SpriteBatch.Draw(this.HasPermission(Permissions.SystemShops) || this.HasPermission(Permissions.ViewScripts) || (this.HasPermission(Permissions.Grief) || this.HasPermission(Permissions.Save)) || this.HasPermission(Permissions.Admin) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }

    protected override void DrawHighLight(Vector2 position, Color color)
    {
      Rectangle highlightRect = ((PanelMenuScreen) this.Screen).HighlightRect;
      highlightRect.X += (int) position.X;
      highlightRect.Y += (int) position.Y - 4;
      Color color1 = this.ColorHighlighted * ((float) this.Screen.TransitionAlpha / (float) byte.MaxValue);
      Color fillColor = this.ColorHighlighted * (float) ((double) this.Screen.TransitionAlpha / (double) byte.MaxValue * 0.5);
      int num1 = 278;
      int num2 = 138;
      int num3 = 70;
      int num4 = 120;
      int num5 = 58;
      int num6 = 66;
      int num7 = 88;
      int num8 = 122;
      int num9 = 90;
      switch (this.screen.Column)
      {
        case 1:
          highlightRect.X += num1;
          highlightRect.Width = num2;
          break;
        case 2:
          highlightRect.X += num1 + num2;
          highlightRect.Width = num3;
          break;
        case 3:
          highlightRect.X += num1 + num2 + num3;
          highlightRect.Width = num4;
          break;
        case 4:
          highlightRect.X += num1 + num2 + num3 + num4;
          highlightRect.Width = num5;
          break;
        case 5:
          highlightRect.X += num1 + num2 + num3 + num4 + num5;
          highlightRect.Width = num6;
          break;
        case 6:
          highlightRect.X += num1 + num2 + num3 + num4 + num5 + num6;
          highlightRect.Width = num7;
          break;
        case 7:
          highlightRect.X += num1 + num2 + num3 + num4 + num5 + num6 + num7;
          highlightRect.Width = num8;
          break;
        case 8:
          highlightRect.X += num1 + num2 + num3 + num4 + num5 + num6 + num7 + num8;
          highlightRect.Width = num9;
          break;
        default:
          highlightRect.Width = num1;
          break;
      }
      this.Screen.SpriteBatch.DrawFilledBox(highlightRect, 1, color1, fillColor);
    }
  }
}
