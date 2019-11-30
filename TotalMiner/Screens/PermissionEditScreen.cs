// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PermissionEditScreen
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
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class PermissionEditScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private Texture2D checkboxOn;
    private Texture2D checkboxOff;
    private Player editingPlayer;
    private Player targetPlayer;

    private bool IsDefaultPermission
    {
      get
      {
        return this.targetPlayer == null;
      }
    }

    private bool CanEdit
    {
      get
      {
        bool flag = this.editingPlayer.IsHost;
        if (!flag)
          flag = this.editingPlayer.IsAdmin && this.targetPlayer != null && !this.targetPlayer.IsHost && (this.editingPlayer == this.targetPlayer || !this.targetPlayer.IsAdmin) || this.editingPlayer == this.targetPlayer && this.selectedEntry == 6 && this.editingPlayer.HasPermission(Permissions.VoiceChat);
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

    public PermissionEditScreen(GameInstance instance, Player editingPlayer, Player targetPlayer)
      : base("Script", editingPlayer)
    {
      this.instance = instance;
      this.editingPlayer = editingPlayer;
      this.targetPlayer = targetPlayer;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.IsDefaultPermission ? ".. Default .." : "Player: " + targetPlayer.Gamertag));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Adventure:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Edit:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Creative:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fly:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Map:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Voice Chat:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Text Chat:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Spectate:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Shops:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "View Scripts:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Grief:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Save:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Admin:"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      for (int index = 1; index < blockMenuEntryList.Count - 1; ++index)
        blockMenuEntryList[index].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelected);
      if (this.IsDefaultPermission)
      {
        blockMenuEntryList[blockMenuEntryList.Count - 4].IsEnabled = false;
        blockMenuEntryList[blockMenuEntryList.Count - 3].IsEnabled = false;
        blockMenuEntryList[blockMenuEntryList.Count - 2].IsEnabled = false;
      }
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    private void OnSelected(object sender, PlayerIndexEventArgs e)
    {
      if (!this.CanEdit)
      {
        this.ShowCantEditPermsMessage();
      }
      else
      {
        Permissions perm = this.GetPerm(this.selectedEntry);
        if (this.IsDefaultPermission)
          Globals2.TogglDefaultPermission(perm);
        else
          this.targetPlayer.TogglePermission(perm);
        GamerID gamerID = this.IsDefaultPermission ? GamerID.Sys1 : this.targetPlayer.GamerID;
        Permissions permission = this.IsDefaultPermission ? Globals2.DefaultPermission : this.targetPlayer.Permission;
        NetworkManager.Instance.SendPermissions(gamerID, permission, (NetworkGamer) null);
      }
    }

    private void ShowCantEditPermsMessage()
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You do not have permission to edit this permission", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.editingPlayer), this.ControllingPlayer);
    }

    private void RefreshItemText()
    {
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 384;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.checkboxOn = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkboxOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
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
      this.ScreenManager.AddScreen((GameScreen) new HowToScreen(this.player, "Permissions", this.GetPermissionText(this.selectedEntry) + "\n"), this.ControllingPlayer);
      return true;
    }

    protected override void DrawEntry(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      base.DrawEntry(menuEntry, entryID, position, isSelected);
      if (entryID == 0 || entryID == this.MenuEntries.Count - 1)
        return;
      Permissions perm = this.GetPerm(entryID);
      this.SpriteBatch.Draw(!this.IsDefaultPermission && this.targetPlayer.HasPermission(perm) || this.IsDefaultPermission && Globals2.HasDefaultPermission(perm) ? this.checkboxOn : this.checkboxOff, new Rectangle((int) position.X + 200, (int) ((double) position.Y + (double) TMFont.yOff) + 8, 24, 24), Color.White);
    }

    private Permissions GetPerm(int entryID)
    {
      switch (entryID)
      {
        case 1:
          return Permissions.Adventure;
        case 2:
          return Permissions.Edit;
        case 3:
          return Permissions.Creative;
        case 4:
          return Permissions.Fly;
        case 5:
          return Permissions.Map;
        case 6:
          return Permissions.VoiceChat;
        case 7:
          return Permissions.TextChat;
        case 8:
          return Permissions.Spectate;
        case 9:
          return Permissions.SystemShops;
        case 10:
          return Permissions.ViewScripts;
        case 11:
          return Permissions.Grief;
        case 12:
          return Permissions.Save;
        case 13:
          return Permissions.Admin;
        default:
          return Permissions.None;
      }
    }

    private string GetPermissionText(int entryID)
    {
      Permissions perm = this.GetPerm(entryID);
      if ((uint) perm <= 64U)
      {
        if ((uint) perm <= 8U)
        {
          switch (perm)
          {
            case Permissions.Adventure:
              return HowToMenuScreen.AdventurePermissionText;
            case Permissions.Edit:
              return HowToMenuScreen.EditPermissionText;
            case Permissions.Creative:
              return HowToMenuScreen.CreativePermissionText;
            case Permissions.Fly:
              return HowToMenuScreen.FlyPermissionText;
          }
        }
        else
        {
          switch (perm)
          {
            case Permissions.Map:
              return HowToMenuScreen.MapPermissionText;
            case Permissions.Save:
              return HowToMenuScreen.SavePermissionText;
            case Permissions.Admin:
              return HowToMenuScreen.AdminPermissionText;
          }
        }
      }
      else if ((uint) perm <= 512U)
      {
        switch (perm)
        {
          case Permissions.Grief:
            return HowToMenuScreen.GriefPermissionText;
          case Permissions.VoiceChat:
            return HowToMenuScreen.VoiceChatPermissionText;
          case Permissions.Spectate:
            return HowToMenuScreen.SpectatePermissionText;
        }
      }
      else
      {
        switch (perm)
        {
          case Permissions.SystemShops:
            return HowToMenuScreen.ShopPermissionText;
          case Permissions.ViewScripts:
            return HowToMenuScreen.ViewScriptsPermissionText;
          case Permissions.TextChat:
            return HowToMenuScreen.TextChatPermissionText;
        }
      }
      return HowToMenuScreen.PermissionHowToText;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 120, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Info", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }
  }
}
