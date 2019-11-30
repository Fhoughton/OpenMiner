// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TextMessageMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class TextMessageMenuScreen : FolderListMenuScreen
  {
    private string msgText;
    private string msgRecipientText;
    private NetworkGamer msgRecipient;
    private BlockMenuEntry viewChatLogItem;

    public TextMessageMenuScreen(GameInstance instance, Player player, string path)
      : base(instance, player)
    {
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.ChatIcon;
      BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, "Send Text Message");
      blockMenuEntry1.Selected += new EventHandler<PlayerIndexEventArgs>(this.SendMenuEntrySelected);
      this.MenuEntries.Add((MenuEntry) blockMenuEntry1);
      BlockMenuEntry blockMenuEntry2 = new BlockMenuEntry((BlockMenuScreen) this, "Create Preset Message");
      blockMenuEntry2.Selected += new EventHandler<PlayerIndexEventArgs>(this.CreatePresetMenuEntrySelected);
      this.MenuEntries.Add((MenuEntry) blockMenuEntry2);
      BlockMenuEntry blockMenuEntry3;
      this.viewChatLogItem = blockMenuEntry3 = new BlockMenuEntry((BlockMenuScreen) this, "View Chat Log");
      blockMenuEntry3.Selected += new EventHandler<PlayerIndexEventArgs>(this.ViewChatLogMenuEntrySelected);
      this.MenuEntries.Add((MenuEntry) blockMenuEntry3);
      string str1 = "Presets";
      if (path.IsNotEmpty() && path.Length > 1)
        str1 = str1 + ": " + path.Substring(0, path.Length - 1);
      string str2 = str1.Length < 39 ? new string('-', 39 - str1.Length) : "";
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, str2 + " " + str1 + " " + str2));
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.GetMenuItems), new ListBoxScreen.OnMenuItemSelected(this.OnPresetSelected), (EventHandler<PlayerIndexEventArgs>) null, (string) null, new EventHandler<PlayerIndexEventArgs>(this.PresetDeleteMenuEntrySelected), "Delete", false);
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "-----------------------------------------------------------------------------"));
      BlockMenuEntry blockMenuEntry4 = new BlockMenuEntry((BlockMenuScreen) this, this.currentPath.IsNotEmpty() ? "Back" : "Close");
      blockMenuEntry4.Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.Add((MenuEntry) blockMenuEntry4);
    }

    private string[] GetMenuItems(string path)
    {
      List<string> stringList = new List<string>();
      GamertagData gamertagData = Globals2.GamertagData.GetGamertagData((Gamer) this.player.Gamer);
      if (gamertagData != null)
        stringList.AddRange((IEnumerable<string>) this.GetArrayOfSortedItems(gamertagData.TextMessagePresets, path));
      return stringList.ToArray();
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new TextMessageMenuScreen(this.instance, this.player, this.currentPath);
    }

    private void SelectGamer(Action<NetworkGamer, bool, string> action)
    {
      List<string> ofClansInTheGame = this.instance.GetListOfClansInTheGame();
      for (int index = 0; index < ofClansInTheGame.Count; ++index)
        ofClansInTheGame[index] = "Clan: " + ofClansInTheGame[index];
      ofClansInTheGame.Insert(0, "Admins");
      this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.player, action, true, (string) null, true, true, (string[]) null, ofClansInTheGame.ToArray()), this.ControllingPlayer);
    }

    private bool OnPresetSelected(MenuEntry item)
    {
      this.msgText = item.Text;
      this.SelectGamer(new Action<NetworkGamer, bool, string>(this.OnPresetSelecedForSend));
      return true;
    }

    private void OnPresetSelecedForSend(NetworkGamer gamer, bool allGamers, string text)
    {
      this.msgRecipient = gamer;
      this.msgRecipientText = text;
      this.SendTextMessage();
    }

    private void SendMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.SelectGamer(new Action<NetworkGamer, bool, string>(this.OnTextMessageRecipientSelected));
    }

    private void OnTextMessageRecipientSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      this.msgRecipient = gamer;
      this.msgRecipientText = text;
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter message", (string) null, "", new AsyncCallback(this.OnMessageEntered), (object) null);
    }

    private void OnMessageEntered(IAsyncResult ar)
    {
      this.msgText = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (!this.msgText.IsNotEmpty())
        return;
      this.SendTextMessage();
      this.ExitScreen();
    }

    private void CreatePresetMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter a preset message", (string) null, this.currentPath, new AsyncCallback(this.OnPresetEntered), (object) null);
    }

    private void ViewChatLogMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new TextMessageViewLogScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OnPresetEntered(IAsyncResult ar)
    {
      string str = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      Globals2.GamertagData.AddTextMessagePreset((Gamer) this.player.Gamer, str);
      Globals2.SaveGamertagData(true, false, true);
      this.RestartScreen();
    }

    private void SendTextMessage()
    {
      bool admin = false;
      string clan = (string) null;
      if (this.msgRecipient == null && this.msgRecipientText.IsNotEmpty())
      {
        admin = this.msgRecipientText == "Admins";
        if (!admin && this.msgRecipientText.StartsWith("Clan: "))
        {
          clan = this.msgRecipientText.Substring(6);
          if (clan == "")
            clan = (string) null;
        }
      }
      this.instance.SendTextMessage(this.msgRecipient, this.player, clan, admin, this.msgText);
    }

    private void PresetDeleteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.selectedEntry <= 2 || this.selectedEntry >= this.MenuEntries.Count - 2)
        return;
      Globals2.GamertagData.RemoveTextMessagePreset((Gamer) this.player.Gamer, this.currentPath + this.MenuEntries[this.selectedEntry].Text);
      Globals2.SaveGamertagData(true, false, true);
      this.RestartScreen();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      this.viewChatLogItem.IsEnabled = this.instance.ChatLog.Count > 0;
    }

    protected override void DrawEntry(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      this.DrawItemTextures = entryID > 2 && entryID < this.MenuEntries.Count - 2;
      base.DrawEntry(menuEntry, entryID, position, isSelected);
    }
  }
}
