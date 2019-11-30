// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.FolderListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal abstract class FolderListMenuScreen : ListBoxScreen
  {
    protected Item ItemFolderIcon = Item.FolderIcon;
    protected GameInstance instance;
    protected string currentPath;
    protected ListBoxScreen.OnMenuItemSelected onItemSelected;
    protected Item ItemFileIcon;
    private FolderListMenuScreen.LoadFolderItems loadList;
    private EventHandler<PlayerIndexEventArgs> onXButtonAction;
    private EventHandler<PlayerIndexEventArgs> onYButtonAction;
    private string onXButtonText;
    private string onYButtonText;
    private string initialSelectedItem;

    protected FolderListMenuScreen(GameInstance instance, Player player)
      : base(player)
    {
      this.instance = instance;
      this.DrawItemTextures = true;
      this.DrawItemTextureBorder = false;
    }

    protected void Initialize(
      string path,
      FolderListMenuScreen.LoadFolderItems loadList,
      ListBoxScreen.OnMenuItemSelected onSelected,
      EventHandler<PlayerIndexEventArgs> onXButtonAction,
      string onXButtonText,
      EventHandler<PlayerIndexEventArgs> onYButtonAction,
      string onYButtonText,
      bool includeNoneOption)
    {
      if (path != null && !path.EndsWith("\\"))
      {
        int num = path.LastIndexOf('\\');
        if (num >= 0)
        {
          this.initialSelectedItem = path.Substring(num + 1);
          path = path.Substring(0, num + 1);
        }
        else
        {
          this.initialSelectedItem = path;
          path = (string) null;
        }
      }
      this.currentPath = path;
      this.loadList = loadList;
      this.onItemSelected = onSelected;
      this.onXButtonAction = onXButtonAction;
      this.onYButtonAction = onYButtonAction;
      this.onXButtonText = onXButtonText;
      this.onYButtonText = onYButtonText;
      this.includeNoneOption = includeNoneOption;
      this.InitializeCore(loadList(path), this.initialSelectedItem, new ListBoxScreen.OnMenuItemSelected(this.OnItemSelected), onXButtonText, onXButtonAction, onYButtonText, onYButtonAction, includeNoneOption, false, 0.0f, 0);
    }

    protected override void ItemInitialized(MenuEntry item, int entryID)
    {
      Item obj = this.ItemFileIcon;
      TexturePack texturePack = GraphicStatics.TexturePack;
      if (item.Text.EndsWith("\\"))
      {
        if (this.ShouldRemoveButtonXFromFolders)
        {
          item.SelectXButton -= this.onXButtonAction;
          item.ButtonTextX = (string) null;
        }
        if (this.ShouldRemoveButtonYFromFolders)
        {
          item.SelectYButton -= this.onYButtonAction;
          item.ButtonTextY = (string) null;
        }
        item.EntryTextureSrcRect = new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(this.ItemFolderIcon));
        obj = this.ItemFolderIcon;
      }
      else
        item.EntryTextureSrcRect = new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(this.ItemFileIcon));
      item.EntryTexture = obj == Item.None ? (Texture2D) null : (obj > Item.zLastBlockID ? GraphicStatics.TexturePack.ItemTexture : GraphicStatics.TexturePack.BlockTexture);
      item.EntryTextureRect = new Rectangle?(new Rectangle(12, 7, 18, 18));
      item.TextOffsetEx.X += 12f;
    }

    protected virtual bool ShouldRemoveButtonXFromFolders
    {
      get
      {
        return true;
      }
    }

    protected virtual bool ShouldRemoveButtonYFromFolders
    {
      get
      {
        return true;
      }
    }

    protected void RestartScreen()
    {
      this.ExitScreen();
      this.ScreenManager.AddScreen(this.RestartScreenCore(), this.ControllingPlayer);
    }

    protected virtual void LoadParentScreen()
    {
    }

    protected abstract GameScreen RestartScreenCore();

    protected string[] GetArrayOfSortedItems(List<string> items, string path)
    {
      if (path == null)
        path = "";
      List<string> stringList = new List<string>(items.Count);
      for (int index = 0; index < items.Count; ++index)
      {
        string str1 = items[index];
        if (str1.StartsWith(path))
        {
          string str2 = str1.Substring(path.Length);
          int num = str2.IndexOf('\\');
          if (num >= 0)
          {
            string str3 = str2.Substring(0, num + 1);
            if (!stringList.Contains(str3))
              stringList.Add(str3);
          }
          else
            stringList.Add(str2);
        }
      }
      stringList.Sort(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
      return stringList.ToArray();
    }

    private bool OnItemSelected(MenuEntry item)
    {
      if (item == null)
      {
        if (this.onItemSelected != null)
          return this.onItemSelected((MenuEntry) null);
      }
      else
      {
        if (item.Text.EndsWith("\\"))
        {
          this.currentPath += item.Text;
          this.RestartScreen();
          return false;
        }
        if (this.onItemSelected != null)
        {
          item.Tag = (object) this.currentPath;
          return this.onItemSelected(item);
        }
      }
      return true;
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      if (this.currentPath == null || this.currentPath.Length < 1)
      {
        this.ExitScreen();
        this.LoadParentScreen();
      }
      else
      {
        string str = this.currentPath;
        if (str.EndsWith("\\"))
          str = str.Substring(0, str.Length - 1);
        int num = str.LastIndexOf('\\');
        this.currentPath = num < 0 ? "" : str.Substring(0, num + 1);
        this.RestartScreen();
      }
    }

    protected override void DrawEntry(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      string text = menuEntry.Text;
      string str = text;
      if (str.EndsWith("\\"))
        str = str.Substring(0, str.Length - 1);
      menuEntry.Text = str;
      base.DrawEntry(menuEntry, entryID, position, isSelected);
      menuEntry.Text = text;
    }

    internal delegate string[] LoadFolderItems(string path);
  }
}
