// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.BlockSelectMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class BlockSelectMenu : NewGuiMenu2
  {
    private Window mainMenuContainer;
    private BlockSelectMode mode;
    private ItemSelectModeFlag modeFlag;
    private Action<Item> itemSelected;
    private InventoryTabsPane tabsPane;
    private string name;

    public override string Name
    {
      get
      {
        return this.name;
      }
    }

    public BlockSelectMenu(
      GameInstance instance,
      Player player,
      string name,
      BlockSelectMode mode,
      Action<Item> itemSelected)
      : base(instance, player)
    {
      this.mode = mode;
      this.itemSelected = itemSelected;
      this.name = name != null ? name : "Blocks";
      switch (mode)
      {
        case BlockSelectMode.CreativeFill:
          this.modeFlag = ItemSelectModeFlag.Fill;
          break;
        case BlockSelectMode.SelectingBlockTextureForMultTextureBlock:
          this.modeFlag = ItemSelectModeFlag.MultiTexture;
          break;
      }
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private void InitMainContainer()
    {
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -150;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int x = 120;
      int y1 = 110;
      TextBox textBox = new TextBox(this.Name, x, y1, 836, 40, 0.75f, WinTextAlignX.Center, WinTextAlignY.Center);
      textBox.Colors = (Window.ColorProfile) Colors.LabelColors;
      this.canvas.AddChild((Node) textBox);
      int y2 = y1 + 60;
      Window window = this.mainMenuContainer = new Window((string) null, x, y2, this.canvas.Size.X - x * 2, 900)
      {
        Name = "mainContainer"
      };
      window.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window);
      this.tabsPane = new InventoryTabsPane((NewGuiMenu2) this, this.windowManager, new Action<InventorySlotWin, bool>(this.ItemSelected), new Func<Item, bool>(this.IsItemVisible));
      window.AddChild((Node) this.tabsPane.InitWindows(true));
    }

    public override void Open(
      WindowManager windowManager,
      Rectangle screenRect,
      Texture2D backTexture,
      NewGuiMenu prevOpen,
      Action onExit,
      TabData tabData,
      Color backColor)
    {
      base.Open(windowManager, screenRect, backTexture, prevOpen, onExit, tabData, backColor);
      this.tabsPane.Open();
    }

    public override void Close()
    {
      this.tabsPane.Close();
      base.Close();
    }

    private bool IsItemVisible(Item itemID)
    {
      if (this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.CreativeReplace)
        return itemID < Item.zLastBlockID;
      if (this.mode == BlockSelectMode.SelectingBlockForReplaceTexture)
        return this.IsItemVisibleForReplaceTexture(itemID);
      if (this.mode == BlockSelectMode.SelectingChannel)
      {
        if (itemID >= Item.ColorSmoothGray)
          return itemID <= Item.ColorBlue;
        return false;
      }
      if (this.mode == BlockSelectMode.SelectingDecal)
      {
        if (itemID > Item.None)
          return itemID < (Item) MapTM.DecalNames.Length;
        return false;
      }
      if (this.modeFlag == (ItemSelectModeFlag) 0)
        return true;
      return (Globals1.ItemData[(int) itemID].SelectFlag & this.modeFlag) > (ItemSelectModeFlag) 0;
    }

    private bool IsItemVisibleForReplaceTexture(Item itemID)
    {
      if (itemID <= Item.None || itemID >= Item.zLastBlockID || !this.instance.Map.UsesBlockTextureTable((Block) itemID))
        return false;
      Item obj = itemID;
      if ((uint) obj <= 132U)
      {
        if (obj != Item.Obsidian && obj != Item.LockedChest)
          goto label_5;
      }
      else if (obj != Item.Painting && obj != Item.LockedDoor)
        goto label_5;
      return false;
label_5:
      return true;
    }

    private void ItemSelected(InventorySlotWin win, bool isSingle)
    {
      if (this.itemSelected == null)
        return;
      this.itemSelected(win.InvItem.ItemID);
    }
  }
}
