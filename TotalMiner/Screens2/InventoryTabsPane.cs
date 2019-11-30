// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.InventoryTabsPane
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class InventoryTabsPane
  {
    public Window.WindowDragHandler DragStartHandler;
    public Window.WindowDragHandler DragEndHandler;
    private NewGuiMenu2 parentTab;
    private Rectangle screenRect;
    private Window mainWin;
    private Window blocksWin;
    private Window itemsWin;
    private Window activeSubTabsWin;
    private Window invWin;
    private Window tabHighLight;
    private Window subTabHighLight;
    private TextBox pageCountWin;
    private Window pageCountContainerWin;
    private InventoryPane invPane;
    private Inventory inventory;
    private int blocksTabID;
    private int itemsTabID;
    private int columns;
    private int slotID;
    private int currentPage;
    private bool morePages;
    private bool itemAddedSinceLastPad;
    private Action<InventorySlotWin, bool> itemSelected;
    private Func<Item, bool> isVisible;
    private WindowManager windowManager;
    private int bb;

    public InventoryPane InventoryPane
    {
      get
      {
        return this.invPane;
      }
    }

    private PlayerIndex PlayerIndex
    {
      get
      {
        return this.parentTab.Player.PlayerIndex;
      }
    }

    public InventoryTabsPane(
      NewGuiMenu2 parentTab,
      WindowManager windowManager,
      Action<InventorySlotWin, bool> itemSelected,
      Func<Item, bool> isVisible)
    {
      this.parentTab = parentTab;
      this.windowManager = windowManager;
      this.itemSelected = itemSelected;
      this.isVisible = isVisible;
      this.columns = 10;
      this.inventory = new Inventory(100);
    }

    public void Open()
    {
      this.windowManager.PushInputHandler(new Func<bool>(this.HandleInput));
    }

    public void Close()
    {
      this.windowManager.PopInputHandler();
    }

    public Window InitWindows(bool blocksOnly)
    {
      this.screenRect = new Rectangle(0, 0, 836, 900);
      Window window1 = this.mainWin = new Window((string) null, 0, 0, this.screenRect.Width, this.screenRect.Height)
      {
        Name = "main"
      };
      window1.Colors = Window.TransparentColorProfile;
      int x1 = 0;
      int y = 0;
      int width = 150;
      int height = 34;
      int num1 = 4;
      float textScale = 0.6f;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      this.tabHighLight = new Window((string) null, 0, height - 5, width, 5);
      this.tabHighLight.Colors = Colors.PauseMenuTabHighlight;
      this.subTabHighLight = new Window((string) null, 0, height - 5, width, 5);
      this.subTabHighLight.Colors = Colors.PauseMenuTabHighlight;
      int x2;
      if (!blocksOnly)
      {
        TextBox textBox1 = new TextBox("Blocks", x1, y, width, height, textScale);
        textBox1.Name = "blocksTab";
        Window window2 = (Window) textBox1;
        window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window2.ClickHandler += new Window.WindowHandler(this.ClickBlocksTab);
        window1.AddChild((Node) window2);
        int x3 = x1 + (width + 1);
        TextBox textBox2 = new TextBox("Items", x3, y, width, height, textScale);
        textBox2.Name = "itemsTab";
        Window window3 = (Window) textBox2;
        window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window3.ClickHandler += new Window.WindowHandler(this.ClickItemsTab);
        window1.AddChild((Node) window3);
        x2 = x3 + 540;
      }
      else
        x2 = x1 + 691;
      Window window4 = this.pageCountContainerWin = new Window((string) null, x2, y, 141, height)
      {
        IsVisible = false
      };
      window4.Colors = Window.TransparentColorProfile;
      window1.AddChild((Node) window4);
      int x4 = 0;
      Window window5 = (Window) new TextBox("-", x4, 0, 40, height, textScale);
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window5.ClickHandler += new Window.WindowHandler(this.ClickPageUp);
      this.pageCountContainerWin.AddChild((Node) window5);
      int x5 = x4 + 43;
      Window window6 = (Window) (this.pageCountWin = new TextBox("1", x5, 0, 55, height, textScale, WinTextAlignX.Center, WinTextAlignY.Center));
      window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window6.ClickHandler += new Window.WindowHandler(this.ClickPageUp);
      this.pageCountContainerWin.AddChild((Node) window6);
      Window window7 = (Window) new TextBox("+", x5 + 58, 0, 40, height, textScale);
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window7.ClickHandler += new Window.WindowHandler(this.ClickPageDown);
      this.pageCountContainerWin.AddChild((Node) window7);
      int num2 = y + (height + num1);
      this.invPane = new InventoryPane(this.parentTab, this.inventory, this.columns, new Point(80, 80), InventorySlotWinFlags.ShowBuyPrice, new Action<InventorySlotWin, bool>(this.SlotSelected));
      this.invPane.DragStartHandler = this.DragStartHandler;
      this.invPane.DragEndHandler = this.DragEndHandler;
      this.invWin = this.invPane.InitWindows();
      this.invWin.Position.Y = (float) (num2 + height + num1 + 8);
      window1.AddChild((Node) this.invWin);
      this.InitBlocksWindows();
      this.SetTab(0);
      this.SetSubTab(0);
      return window1;
    }

    private void InitBlocksWindows()
    {
      int x1 = 0;
      int y = 0;
      int width = 118;
      int height = 34;
      float textScale = 0.6f;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window1 = new Window((string) null, 0, (int) this.invWin.Position.Y - (height + 12), this.screenRect.Width, this.screenRect.Height)
      {
        Name = "blocksSub"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.mainWin.AddChild((Node) (this.activeSubTabsWin = this.blocksWin = window1));
      Window window2 = (Window) new TextBox("Natural", x1, y, width, height, textScale);
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(this.ClickBlocksNaturalTab);
      window1.AddChild((Node) window2);
      int x2 = x1 + (width + 1);
      Window window3 = (Window) new TextBox("Stone", x2, y, width, height, textScale);
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window3.ClickHandler += new Window.WindowHandler(this.ClickBlocksRockTab);
      window1.AddChild((Node) window3);
      int x3 = x2 + (width + 1);
      Window window4 = (Window) new TextBox("Ores", x3, y, width, height, textScale);
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickBlocksOresTab);
      window1.AddChild((Node) window4);
      int x4 = x3 + (width + 1);
      Window window5 = (Window) new TextBox("Flora", x4, y, width, height, textScale);
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window5.ClickHandler += new Window.WindowHandler(this.ClickBlocksFloraTab);
      window1.AddChild((Node) window5);
      int x5 = x4 + (width + 1);
      Window window6 = (Window) new TextBox("Utility", x5, y, width, height, textScale);
      window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window6.ClickHandler += new Window.WindowHandler(this.ClickBlocksUtilityTab);
      window1.AddChild((Node) window6);
      int x6 = x5 + (width + 1);
      Window window7 = (Window) new TextBox("Building", x6, y, width, height, textScale);
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window7.ClickHandler += new Window.WindowHandler(this.ClickBlocksBuildingTab);
      window1.AddChild((Node) window7);
      Window window8 = (Window) new TextBox("Colors", x6 + (width + 1), y, width, height, textScale);
      window8.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window8.ClickHandler += new Window.WindowHandler(this.ClickBlocksColorsTab);
      window1.AddChild((Node) window8);
      this.blocksTabID = 0;
      this.PopulateBlockInventory(0);
    }

    private void InitItemsWindows()
    {
      int x1 = 0;
      int y = 0;
      int width = 118;
      int height = 34;
      float textScale = 0.6f;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window1 = new Window((string) null, 0, (int) this.invWin.Position.Y - (height + 12), this.screenRect.Width, this.screenRect.Height)
      {
        Name = "itemsSub"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.mainWin.AddChild((Node) (this.activeSubTabsWin = this.itemsWin = window1));
      Window window2 = (Window) new TextBox("Tools", x1, y, width, height, textScale);
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(this.ClickItemsToolsTab);
      window1.AddChild((Node) window2);
      int x2 = x1 + (width + 1);
      Window window3 = (Window) new TextBox("Weapons", x2, y, width, height, textScale);
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window3.ClickHandler += new Window.WindowHandler(this.ClickItemsWeaponsTab);
      window1.AddChild((Node) window3);
      int x3 = x2 + (width + 1);
      Window window4 = (Window) new TextBox("Armor", x3, y, width, height, textScale);
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickItemsArmorTab);
      window1.AddChild((Node) window4);
      int x4 = x3 + (width + 1);
      Window window5 = (Window) new TextBox("Food", x4, y, width, height, textScale);
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window5.ClickHandler += new Window.WindowHandler(this.ClickItemsFoodTab);
      window1.AddChild((Node) window5);
      int x5 = x4 + (width + 1);
      Window window6 = (Window) new TextBox("Jewelry", x5, y, width, height, textScale);
      window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window6.ClickHandler += new Window.WindowHandler(this.ClickItemsJewelryTab);
      window1.AddChild((Node) window6);
      int x6 = x5 + (width + 1);
      Window window7 = (Window) new TextBox("Keys", x6, y, width, height, textScale);
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window7.ClickHandler += new Window.WindowHandler(this.ClickItemsKeysTab);
      window1.AddChild((Node) window7);
      int x7 = x6 + (width + 1);
      Window window8 = (Window) new TextBox("Other", x7, y, width, height, textScale);
      window8.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window8.ClickHandler += new Window.WindowHandler(this.ClickItemsOtherTab);
      window1.AddChild((Node) window8);
      int num = x7 + (width + 1);
      this.itemsTabID = 0;
      this.PopulateItemInventory(0);
    }

    private void SetTab(int tabID)
    {
      Window child = this.mainWin.FindChild("blocksTab");
      if (child == null)
        return;
      TextBox sibling = child.GetSibling(tabID) as TextBox;
      if (this.tabHighLight.Parent == null)
      {
        sibling.AddChild((Node) this.tabHighLight);
      }
      else
      {
        ((TextBox) this.tabHighLight.Parent).TextOffset.Y += 3f;
        this.tabHighLight.ChangeParent((Node) sibling);
      }
      sibling.TextOffset.Y -= 3f;
    }

    private void SetSubTab(int tabID)
    {
      TextBox sibling = this.activeSubTabsWin.FirstChild.GetSibling(tabID) as TextBox;
      if (this.subTabHighLight.Parent == null)
      {
        sibling.AddChild((Node) this.subTabHighLight);
      }
      else
      {
        ((TextBox) this.subTabHighLight.Parent).TextOffset.Y += 3f;
        this.subTabHighLight.ChangeParent((Node) sibling);
      }
      sibling.TextOffset.Y -= 3f;
      this.subTabHighLight.Size.X = sibling.Size.X;
      this.subTabHighLight.Position.Y = (float) (sibling.Size.Y - this.subTabHighLight.Size.Y);
    }

    private bool HandleInput()
    {
      if (InputManager1.IsInputPressedNew(this.PlayerIndex, GuiInput.PrevTab))
      {
        if (this.activeSubTabsWin == this.blocksWin)
          this.OpenBlockSubTab(this.blocksTabID == 0 ? 6 : this.blocksTabID - 1);
        else
          this.OpenItemSubTab(this.itemsTabID == 0 ? 6 : this.itemsTabID - 1);
        return true;
      }
      if (!InputManager1.IsInputPressedNew(this.PlayerIndex, GuiInput.NextTab))
        return false;
      if (this.activeSubTabsWin == this.blocksWin)
        this.OpenBlockSubTab(this.blocksTabID == 6 ? 0 : this.blocksTabID + 1);
      else
        this.OpenItemSubTab(this.itemsTabID == 6 ? 0 : this.itemsTabID + 1);
      return true;
    }

    private void ClickBlocksTab(object sender, WindowEventArgs e)
    {
      if (this.activeSubTabsWin == this.blocksWin)
        return;
      this.activeSubTabsWin.RemoveSelf();
      if (this.blocksWin == null)
        this.InitBlocksWindows();
      else
        this.mainWin.AddChild((Node) (this.activeSubTabsWin = this.blocksWin));
      this.SetTab(0);
      this.SetSubTab(0);
      this.blocksTabID = -1;
      this.OpenBlockSubTab(0);
    }

    private void ClickItemsTab(object sender, WindowEventArgs e)
    {
      if (this.activeSubTabsWin == this.itemsWin)
        return;
      this.activeSubTabsWin.RemoveSelf();
      if (this.itemsWin == null)
        this.InitItemsWindows();
      else
        this.mainWin.AddChild((Node) (this.activeSubTabsWin = this.itemsWin));
      this.SetTab(1);
      this.SetSubTab(0);
      this.itemsTabID = -1;
      this.OpenItemSubTab(0);
    }

    private void ClickPageUp(object sender, WindowEventArgs e)
    {
      if (this.currentPage <= 0)
        return;
      this.PopulateItemInventory(--this.currentPage);
    }

    private void ClickPageDown(object sender, WindowEventArgs e)
    {
      if (!this.morePages)
        return;
      this.PopulateItemInventory(++this.currentPage);
    }

    private void OpenBlockSubTab(int tabID)
    {
      if (this.blocksTabID != tabID)
      {
        this.bb = 0;
        this.blocksTabID = tabID;
        this.PopulateBlockInventory(0);
        this.SetSubTab(tabID);
      }
      else
      {
        ++this.bb;
        this.PopulateBlockInventory(0);
      }
    }

    private void OpenItemSubTab(int tabID)
    {
      if (this.itemsTabID == tabID)
        return;
      this.itemsTabID = tabID;
      this.PopulateItemInventory(0);
      this.SetSubTab(tabID);
    }

    private void ClickBlocksNaturalTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(0);
    }

    private void ClickBlocksRockTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(1);
    }

    private void ClickBlocksOresTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(2);
    }

    private void ClickBlocksFloraTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(3);
    }

    private void ClickBlocksUtilityTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(4);
    }

    private void ClickBlocksBuildingTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(5);
    }

    private void ClickBlocksColorsTab(object sender, WindowEventArgs e)
    {
      this.OpenBlockSubTab(6);
    }

    private void ClickItemsToolsTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(0);
    }

    private void ClickItemsWeaponsTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(1);
    }

    private void ClickItemsArmorTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(2);
    }

    private void ClickItemsFoodTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(3);
    }

    private void ClickItemsJewelryTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(4);
    }

    private void ClickItemsKeysTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(5);
    }

    private void ClickItemsOtherTab(object sender, WindowEventArgs e)
    {
      this.OpenItemSubTab(6);
    }

    private void SlotSelected(InventorySlotWin win, bool isSingle)
    {
      if (this.itemSelected == null)
        return;
      this.itemSelected(win, isSingle);
    }

    private void PopulateBlockInventory(int page)
    {
      this.currentPage = page;
      this.PopulateInventory((ItemInvType) (this.blocksTabID + 1), 0);
    }

    private void PopulateItemInventory(int page)
    {
      this.currentPage = page;
      this.PopulateInventory((ItemInvType) (this.itemsTabID + 8), Globals1.BlockData.Length);
    }

    private bool PopulateItem(ItemDataXML data)
    {
      if (data.IsValid && data.IsEnabled && (!data.HasItemProxy && data.ItemID != Item.MobSpawn))
        return true;
      if (this.parentTab.Player != null)
        return this.parentTab.Player.IsGod;
      return false;
    }

    private void PopulateInventory(ItemInvType invType, int startIndex)
    {
      this.pageCountWin.Text = string.Format("{0}", (object) (this.currentPage + 1));
      this.slotID = 0;
      this.inventory.ClearItems();
      int num1 = 0;
      int num2 = this.currentPage * (int) this.inventory.PackSize;
      for (int index = startIndex; index < Globals1.ItemData.Length && this.slotID < (int) this.inventory.PackSize; ++index)
      {
        ItemDataXML data = Globals1.ItemData[index];
        if (this.PopulateItem(data) && (Globals1.ItemTypeData[index].Inv == invType && (this.isVisible == null || this.isVisible(data.ItemID)) && ++num1 > num2))
          this.AddToInventoryCore(this.inventory, new InventoryItem(data.ItemID, 1), this.slotID++);
      }
      this.morePages = this.slotID >= (int) this.inventory.PackSize;
      this.pageCountContainerWin.IsVisible = this.morePages || this.currentPage > 0;
      if (this.invPane == null)
        return;
      this.invPane.RefreshInventoryWindowItems();
    }

    private int FindItem(Item itemID)
    {
      int num = Math.Min((int) this.inventory.PackSize, this.inventory.Count);
      for (int index = 0; index < num; ++index)
      {
        if (this.inventory[index].ItemID == itemID)
          return index;
      }
      return -1;
    }

    private void PadRow()
    {
      if (!this.itemAddedSinceLastPad)
        return;
      int num = this.inventory.Count % this.columns;
      if (num != 0)
        this.slotID += this.columns - num;
      this.itemAddedSinceLastPad = false;
    }

    private void PadColumn()
    {
      this.PadColumn(1);
    }

    private void PadColumn(int cols)
    {
      if (!this.itemAddedSinceLastPad)
        return;
      if (this.inventory.Count % this.columns != 0)
        this.slotID += cols;
      this.itemAddedSinceLastPad = false;
    }

    private void InsertToInventory(Item itemID, int slotID)
    {
      ItemDataXML itemDataXml = Globals1.ItemData[(int) itemID];
      if ((!itemDataXml.IsValid || !itemDataXml.IsEnabled) && (this.parentTab.Player == null || !this.parentTab.Player.IsGod) || this.isVisible != null && !this.isVisible(itemID))
        return;
      InventoryItem inventoryItem = new InventoryItem(itemID, 1);
      for (int index = Math.Min((int) this.inventory.PackSize - 1, this.inventory.Count); index > slotID; --index)
        this.inventory[index] = this.inventory[index - 1];
      this.AddToInventoryCore(this.inventory, inventoryItem, slotID);
      ++this.slotID;
    }

    private void AddToInventoryCore(Inventory inventory, InventoryItem item, int index)
    {
      inventory[index] = item;
      this.itemAddedSinceLastPad = true;
    }
  }
}
