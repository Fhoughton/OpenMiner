// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.InventoryPane
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class InventoryPane
  {
    public Window.WindowDragHandler DragStartHandler;
    public Window.WindowDragHandler DragEndHandler;
    protected NewGuiMenu2 parentTab;
    protected Rectangle screenRect;
    protected Window mainWin;
    protected Inventory inventory;
    protected Point slotSize;
    protected RenderProfile renderProfile;
    private int columns;
    private InventorySlotWinFlags flags;
    private Action<InventorySlotWin, bool> itemSelected;

    public InventoryPane(
      NewGuiMenu2 parentTab,
      Inventory inventory,
      int columns,
      Point slotSize,
      InventorySlotWinFlags flags,
      Action<InventorySlotWin, bool> itemSelected)
    {
      this.parentTab = parentTab;
      this.inventory = inventory;
      this.columns = columns;
      this.slotSize = slotSize;
      this.flags = flags;
      this.itemSelected = itemSelected;
    }

    public Window InitWindows()
    {
      this.screenRect = new Rectangle(0, 0, 836, 900);
      this.renderProfile = new RenderProfile()
      {
        Sampler = SamplerState.PointClamp
      };
      Window window1 = this.mainWin = new Window((string) null, 0, 0, this.screenRect.Width, this.screenRect.Height)
      {
        Name = "main"
      };
      window1.Colors = Colors.GreenTrack;
      int x = 0;
      int y = 0;
      int num1 = 4;
      int num2 = (int) this.inventory.PackSize / this.columns;
      int num3 = x;
      int slotID = 0;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        for (int index2 = 0; index2 < this.columns; ++index2)
        {
          Window window2 = this.NewSlotWin(x, y, slotID);
          window1.AddChild((Node) window2);
          x += this.slotSize.X + num1;
          ++slotID;
        }
        y += this.slotSize.Y + num1;
        x = num3;
      }
      return window1;
    }

    protected Window NewSlotWin(int x, int y, int slotID)
    {
      InventorySlotWin inventorySlotWin = new InventorySlotWin(this.parentTab, x, y, this.slotSize.X, this.slotSize.Y, this.flags, this.inventory, slotID);
      inventorySlotWin.Colors = (Window.ColorProfile) Colors.InvIcon;
      inventorySlotWin.RenderProfile = this.renderProfile;
      return (Window) inventorySlotWin;
    }

    public void RefreshInventoryWindowItems()
    {
      for (Window window = this.mainWin.FirstChild as Window; window != null; window = window.NextSibling as Window)
      {
        InventorySlotWin inventorySlotWin = window as InventorySlotWin;
        if (inventorySlotWin != null)
        {
          window.ClickHandler -= new Window.WindowHandler(this.ClickItemSlot);
          window.RightClickHandler -= new Window.WindowHandler(this.ClickRightItemSlot);
          window.ClearFlags(Window.WinFlags.IsDragable);
          window.DragStartHandler -= this.DragStartHandler;
          window.DragEndHandler -= this.DragEndHandler;
          InventoryItem inventoryItem = this.inventory[inventorySlotWin.SlotID];
          if ((this.flags & InventorySlotWinFlags.UnlockIfLocked) > InventorySlotWinFlags.None && this.parentTab.Instance.IsItemLocked(inventoryItem.ItemID))
            this.parentTab.Instance.UnlockItem(this.parentTab.Player, inventoryItem.ItemID, true);
          if (this.parentTab.Instance.IsItemUnlocked(inventoryItem.ItemID) || this.parentTab.Player.IsGodOrTester)
          {
            window.ClickHandler += new Window.WindowHandler(this.ClickItemSlot);
            window.RightClickHandler += new Window.WindowHandler(this.ClickRightItemSlot);
            if (this.DragStartHandler != null || this.DragEndHandler != null)
            {
              window.AddFlags(Window.WinFlags.IsDragable);
              if (this.DragStartHandler != null)
                window.DragStartHandler += this.DragStartHandler;
              if (this.DragEndHandler != null)
                window.DragEndHandler += this.DragEndHandler;
            }
          }
          inventorySlotWin.Refresh(this.parentTab.Player);
        }
      }
    }

    public InventorySlotWin GetInvWin(int slotID)
    {
      for (Window window = this.mainWin.FirstChild as Window; window != null; window = window.NextSibling as Window)
      {
        InventorySlotWin inventorySlotWin = window as InventorySlotWin;
        if (inventorySlotWin != null && inventorySlotWin.SlotID == slotID)
          return inventorySlotWin;
      }
      return (InventorySlotWin) null;
    }

    private void ClickItemSlot(object sender, WindowEventArgs e)
    {
      if (this.itemSelected == null)
        return;
      InventorySlotWin window = e.Window as InventorySlotWin;
      if (window == null)
        return;
      this.itemSelected(window, true);
    }

    private void ClickRightItemSlot(object sender, WindowEventArgs e)
    {
      if (this.itemSelected == null)
        return;
      InventorySlotWin window = e.Window as InventorySlotWin;
      if (window == null)
        return;
      this.itemSelected(window, false);
    }
  }
}
