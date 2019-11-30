// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptListMenuScreen : FolderListMenuScreen
  {
    private bool allowEdit;
    private bool allowDelete;
    private bool isAdventureScripts;
    private bool returnToAdventureScriptScreen;
    private Script scriptToDelete;
    private string renameFolderFrom;
    private string renameFolderTo;
    private bool searchNames;

    public ScriptListMenuScreen(
      GameInstance instance,
      Player player,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected,
      bool allowDelete,
      bool allowEdit)
      : this(instance, player, path, onSelected, allowDelete, allowEdit, false, false)
    {
    }

    public ScriptListMenuScreen(
      GameInstance instance,
      Player player,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected,
      bool allowDelete,
      bool allowEdit,
      bool isAddventureScripts,
      bool returnToAdventureScriptScreen)
      : base(instance, player)
    {
      this.allowEdit = allowEdit;
      this.allowDelete = allowDelete;
      this.isAdventureScripts = isAddventureScripts;
      this.returnToAdventureScriptScreen = returnToAdventureScriptScreen;
      this.ButtonScale = 0.5f;
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.ScriptBlock;
      if (player != null)
      {
        if (!player.HasPermission(Permissions.Admin))
          allowDelete = false;
        if (!player.HasPermissionAny(Permissions.Admin | Permissions.ViewScripts))
          allowEdit = false;
      }
      this.Initialize(path, this.isAdventureScripts ? new FolderListMenuScreen.LoadFolderItems(instance.ListOfSortedAdventureScriptNames) : new FolderListMenuScreen.LoadFolderItems(instance.ListOfSortedScriptNames), onSelected, allowEdit ? new EventHandler<PlayerIndexEventArgs>(this.OnSelectXButton) : (EventHandler<PlayerIndexEventArgs>) null, allowEdit ? (player == null || player.IsAdmin ? "Edit" : "View") : (string) null, allowDelete ? new EventHandler<PlayerIndexEventArgs>(this.OnSelectYButton) : (EventHandler<PlayerIndexEventArgs>) null, allowDelete ? "Delete" : (string) null, (path == null || path.Length < 1 || path.IndexOf('\\') < 0) && allowEdit);
    }

    protected override void LoadParentScreen()
    {
      if (this.allowEdit)
        return;
      if (this.returnToAdventureScriptScreen)
        this.ScreenManager.AddScreen((GameScreen) new ScriptAdventureMenuScreen(this.instance, this.player), this.ControllingPlayer);
      else
        this.ScreenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    protected override GameScreen RestartScreenCore()
    {
      ScriptListMenuScreen scriptListMenuScreen = new ScriptListMenuScreen(this.instance, this.player, this.currentPath, this.onItemSelected, this.allowDelete, this.allowEdit, this.isAdventureScripts, this.returnToAdventureScriptScreen);
      scriptListMenuScreen.CloseOnSelect = this.CloseOnSelect;
      return (GameScreen) scriptListMenuScreen;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (currentGamePadState.IsButtonDown(Buttons.Start) && lastGamePadState.IsButtonUp(Buttons.Start))
      {
        this.CheckScriptIsUsed();
        return true;
      }
      if (currentGamePadState.IsButtonDown(Buttons.X) && lastGamePadState.IsButtonUp(Buttons.X))
      {
        if (this.IsFolderSelected && this.CanRenameFolder)
        {
          this.RenameFolder();
          return true;
        }
      }
      else
      {
        if (currentGamePadState.IsButtonDown(Buttons.LeftShoulder) && lastGamePadState.IsButtonUp(Buttons.LeftShoulder))
        {
          this.SearchName();
          return true;
        }
        if (currentGamePadState.IsButtonDown(Buttons.RightShoulder) && lastGamePadState.IsButtonUp(Buttons.RightShoulder))
        {
          this.SearchText();
          return true;
        }
      }
      return base.HandleInput(input);
    }

    private Script ScriptForCheckIsUsed
    {
      get
      {
        if (this.player != null && this.player.IsAdmin && (this.selectedEntry >= 0 && this.selectedEntry < this.MenuEntries.Count))
          return this.instance.GetScript(this.currentPath + this.MenuEntries[this.selectedEntry].Text);
        return (Script) null;
      }
    }

    private bool IsFolderSelected
    {
      get
      {
        if (this.selectedEntry >= 0 && this.selectedEntry < this.MenuEntries.Count)
          return this.MenuEntries[this.selectedEntry].Text.EndsWith("\\");
        return false;
      }
    }

    private bool CanRenameFolder
    {
      get
      {
        if (this.player != null && this.player.IsAdmin && this.allowDelete)
          return !this.allowEdit;
        return false;
      }
    }

    private void CheckScriptIsUsed()
    {
      Script scriptForCheckIsUsed = this.ScriptForCheckIsUsed;
      if (scriptForCheckIsUsed == null)
        return;
      string str = this.instance.IsScriptAssigned(scriptForCheckIsUsed);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This script is currently " + (str == null ? this.instance.GetScriptUsedBy(scriptForCheckIsUsed) ?? "not used" : "assigned to " + str), "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnSelectXButton(object sender, PlayerIndexEventArgs e)
    {
      Script script = this.instance.GetScript(this.currentPath + ((MenuEntry) sender).Text);
      if (script == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, (Action) null), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OnSelectYButton(object sender, PlayerIndexEventArgs e)
    {
      Script script = this.instance.GetScript(this.currentPath + ((MenuEntry) sender).Text);
      if (script == null)
        return;
      this.scriptToDelete = script;
      string str1 = this.instance.IsScriptAssigned(script);
      string str2 = str1 == null ? this.instance.GetScriptUsedBy(script) : "assigned to " + str1;
      if (str2 != null)
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("This script is currently " + str2 + ".\n\nConfirm Delete.", "Delete", (string) null, (string) null, "Cancel", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ConfirmDelete);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
      else
        this.ConfirmDelete((object) null, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
    }

    private void ConfirmDelete(object sender, PlayerIndexEventArgs e)
    {
      this.instance.DeleteScript(this.scriptToDelete);
      this.RestartScreen();
    }

    private void RenameFolder()
    {
      MenuEntry menuEntry = this.MenuEntries[this.selectedEntry];
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter new Folder name", "", this.currentPath + menuEntry.Text.Substring(0, menuEntry.Text.Length - 1), new AsyncCallback(this.OnFolderNameEntered), (object) null);
    }

    private void OnFolderNameEntered(IAsyncResult ar)
    {
      string str = Globals2.StripFolderName(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      MenuEntry menuEntry = this.MenuEntries[this.selectedEntry];
      string oldName = this.currentPath + menuEntry.Text.Substring(0, menuEntry.Text.Length - 1);
      if (!(str != oldName))
        return;
      this.RenameFolder(oldName, str);
    }

    private void RenameFolder(string oldName, string newName)
    {
      string str1 = oldName.EndsWith("\\") ? oldName : oldName + (object) '\\';
      string str2 = newName.EndsWith("\\") ? newName : newName + (object) '\\';
      foreach (Script script in this.instance.Scripts)
      {
        if (script.Name.StartsWith(str2) && !script.Name.StartsWith(str1))
        {
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Rename Cancelled.\n\nThere is already a folder with this name.", (string) null, (string) null, (string) null, "Close", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
          return;
        }
      }
      bool flag = false;
      foreach (Script script in this.instance.Scripts)
      {
        if (this.instance.IsScriptUsedInWorld(script))
        {
          flag = true;
          break;
        }
      }
      this.renameFolderFrom = str1;
      this.renameFolderTo = str2;
      if (flag)
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("One or more scripts in this folder are assigned or called.\nConfirming the rename will cause their assignments to be updated.", "Confirm Rename", (string) null, (string) null, "Cancel Rename", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ConfirmRenameFolder);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
      else
        this.ConfirmRenameFolder((object) null, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
    }

    private void ConfirmRenameFolder(object sender, PlayerIndexEventArgs e)
    {
      this.instance.RenameScript((Script) null, this.renameFolderFrom, this.renameFolderTo);
      this.RestartScreen();
    }

    private void SearchName()
    {
      this.searchNames = true;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the text to search names for", "", "", new AsyncCallback(this.SearchTextEntered), (object) null);
    }

    private void SearchText()
    {
      this.searchNames = false;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the text to search commands for", "", "", new AsyncCallback(this.SearchTextEntered), (object) null);
    }

    private void SearchTextEntered(IAsyncResult ar)
    {
      string text = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (text == null)
        return;
      List<Script> scriptList = this.searchNames ? this.instance.ScriptSearchNames(this.currentPath, text) : this.instance.ScriptSearchText(this.currentPath, text);
      if (scriptList.Count < 1)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("No scripts found", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        string[] data = new string[scriptList.Count];
        for (int index = 0; index < scriptList.Count; ++index)
          data[index] = scriptList[index].Name;
        this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, data, new ListBoxScreen.OnMenuItemSelected(this.OnSearchScriptSelected), false)
        {
          CloseOnSelect = false
        }, this.ControllingPlayer);
      }
    }

    private bool OnSearchScriptSelected(MenuEntry entry)
    {
      Script script = this.instance.GetScript(entry.Text);
      if (script != null)
        this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, (Action) null), this.ControllingPlayer);
      return true;
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(0, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight + 9, 20, 20);
      if (this.ScriptForCheckIsUsed != null)
      {
        destinationRectangle.X = this.MenuRect.X + this.MenuRect.Width - 362;
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.Start), new Rectangle(destinationRectangle.X, destinationRectangle.Y - 1, destinationRectangle.Width, destinationRectangle.Height), Color.White);
        this.SpriteBatch.DrawString(this.Font, "Is Used", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      }
      else if (this.IsFolderSelected && this.CanRenameFolder)
      {
        destinationRectangle.X = this.MenuRect.X + this.MenuRect.Width - 330;
        this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.X), destinationRectangle, Color.White);
        this.SpriteBatch.DrawString(this.Font, "Rename Folder", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      }
      destinationRectangle.X -= 154;
      this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.RightShoulder), new Rectangle(destinationRectangle.X, destinationRectangle.Y - 2, destinationRectangle.Width - 4, destinationRectangle.Height + 6), this.ColorWhite);
      this.SpriteBatch.DrawString(this.Font, "Search Text", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 4), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      destinationRectangle.X -= 164;
      this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.LeftShoulder), new Rectangle(destinationRectangle.X, destinationRectangle.Y - 2, destinationRectangle.Width - 4, destinationRectangle.Height + 6), this.ColorWhite);
      this.SpriteBatch.DrawString(this.Font, "Search Name", new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + 4), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      base.DrawBottomBar();
    }
  }
}
