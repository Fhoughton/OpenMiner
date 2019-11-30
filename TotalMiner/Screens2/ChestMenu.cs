// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.ChestMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.Blocks;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class ChestMenu : NewGuiMenu2
  {
    private string name;
    private InventoryPane chestPane;
    private InventoryPane invPane;
    private InventoryBodyPane invBodyPane;
    private Inventory actorInventory;
    private ChestBlock chestBlock;

    public override string Name
    {
      get
      {
        return this.name;
      }
    }

    public ChestMenu(GameInstance instance, Player player, GlobalPoint3D p)
      : base(instance, player)
    {
      Block blockId = (Block) instance.Map.GetBlockID(p);
      this.name = ItemData2.ForDisplay(instance, (Item) blockId);
      this.chestBlock = instance.MapStrategyTM.GetOrAddDataBlock(p, blockId, UpdateBlockMethod.Player, player.GamerID, false) as ChestBlock;
      this.actorInventory = (Inventory) player.Inventory;
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
      int y = 110;
      int num1 = 870;
      TextBox textBox1 = new TextBox("Player Inventory", x, y, 836, 40, 0.75f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox1.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
      this.canvas.AddChild((Node) textBox1);
      TextBox textBox2 = new TextBox(this.Name + " Inventory", x + num1, y, 836, 40, 0.75f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox2.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
      this.canvas.AddChild((Node) textBox2);
      int num2 = y + 60;
      Window window1 = new Window((string) null, x, num2 + 84, this.canvas.Size.X - x * 2, 900)
      {
        Name = "invContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window1);
      this.invPane = new InventoryPane((NewGuiMenu2) this, this.actorInventory, 10, new Point(80, 80), InventorySlotWinFlags.ShowQuantity, new Action<InventorySlotWin, bool>(this.OnInventoryItemSelected));
      this.invPane.DragEndHandler = new Window.WindowDragHandler(this.DragEndInventory);
      Window window2 = this.invPane.InitWindows();
      window1.AddChild((Node) window2);
      this.invPane.RefreshInventoryWindowItems();
      window2.AdjustSizeToContainAllChildren();
      TextBox textBox3 = new TextBox("Clear All", (int) window1.Position.X, num2 + 38, 140, 34, 0.6f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox3.ClickHandler += new Window.WindowHandler(this.ClickClearAll);
      this.canvas.AddChild((Node) textBox3);
      TextBox textBox4 = new TextBox("Transfer All >>", (int) ((double) window1.Position.X + (double) window2.Position.X + (double) window2.Size.X - 200.0), num2 + 38, 200, 34, 0.6f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox4.ClickHandler += new Window.WindowHandler(this.ClickTransferInvToChest);
      this.canvas.AddChild((Node) textBox4);
      this.actorInventory = (Inventory) this.player.Inventory;
      this.invBodyPane = new InventoryBodyPane((NewGuiMenu2) this, this.actorInventory, new Point(80, 80), InventorySlotWinFlags.ShowQuantity, new Action<InventorySlotWin, bool>(this.OnInventoryItemSelected));
      this.invBodyPane.DragEndHandler = new Window.WindowDragHandler(this.DragEndInventory);
      Window window3 = this.invBodyPane.InitWindows();
      window3.Position.X = 216f;
      window3.Position.Y = (float) ((double) window2.Position.Y + (double) window2.Size.Y + 42.0);
      window1.AddChild((Node) window3);
      this.invBodyPane.RefreshInventoryWindowItems();
      Window window4 = new Window((string) null, x + num1, num2 + 84, this.canvas.Size.X - x * 2, 900)
      {
        Name = "chestContainer"
      };
      window4.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window4);
      this.chestPane = new InventoryPane((NewGuiMenu2) this, this.chestBlock.Inventory, 10, new Point(80, 80), InventorySlotWinFlags.ShowQuantity | InventorySlotWinFlags.UnlockIfLocked, new Action<InventorySlotWin, bool>(this.OnChestItemSelected));
      this.chestPane.DragEndHandler = new Window.WindowDragHandler(this.DragEndInventory);
      Window window5 = this.chestPane.InitWindows();
      window4.AddChild((Node) window5);
      this.chestPane.RefreshInventoryWindowItems();
      TextBox textBox5 = new TextBox("<< Transfer All", (int) ((double) window4.Position.X + (double) window5.Position.X), num2 + 38, 200, 34, 0.6f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox5.ClickHandler += new Window.WindowHandler(this.ClickTransferChestToInv);
      this.canvas.AddChild((Node) textBox5);
      int width = 160;
      TextBox textBox6 = new TextBox("Organize", (int) ((double) window4.Position.X + (double) window5.Position.X + (double) window5.Size.X - (double) width), num2 + 38, width, 34, 0.6f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox6.ClickHandler += new Window.WindowHandler(this.ClickOrganizeChest);
      this.canvas.AddChild((Node) textBox6);
      window4.AdjustSizeToContainAllChildrenDeep();
    }

    public override void Close()
    {
      base.Close();
      if (this.actorInventory != this.player.Inventory)
        return;
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.chestBlock, (this.instance.Map.GetBlockID(this.chestBlock.Point) != (byte) 132 || this.chestBlock.Gamertag == null) && !this.chestBlock.Inventory.HasItems());
      this.player.Raise_ChestClosed();
    }

    private void ClickClearAll(object sender, WindowEventArgs e)
    {
      this.actorInventory.ClearItems();
      this.invPane.RefreshInventoryWindowItems();
      this.invBodyPane.RefreshInventoryWindowItems();
      this.player.LeftHand.SetItem(this.actorInventory[this.player.HotBarLeftSlotID].ItemID);
      this.player.RightHand.SetItem(this.actorInventory[this.player.HotBarLeftSlotID].ItemID);
    }

    private void ClickTransferInvToChest(object sender, WindowEventArgs e)
    {
      this.actorInventory.TransferTo(this.chestBlock.Inventory);
      this.invPane.RefreshInventoryWindowItems();
      this.chestPane.RefreshInventoryWindowItems();
      this.player.LeftHand.SetItem(this.actorInventory[this.player.HotBarLeftSlotID].ItemID);
      this.player.RightHand.SetItem(this.actorInventory[this.player.HotBarLeftSlotID].ItemID);
    }

    private void ClickTransferChestToInv(object sender, WindowEventArgs e)
    {
      this.chestBlock.Inventory.TransferTo(this.actorInventory);
      this.invPane.RefreshInventoryWindowItems();
      this.chestPane.RefreshInventoryWindowItems();
    }

    private void ClickOrganizeChest(object sender, WindowEventArgs e)
    {
      this.chestBlock.Inventory.Sort();
      this.chestPane.RefreshInventoryWindowItems();
    }

    private void OnInventoryItemSelected(InventorySlotWin win, bool isSingle)
    {
      if (!this.TransferItem(win, isSingle, this.chestBlock.Inventory))
        return;
      this.chestPane.RefreshInventoryWindowItems();
    }

    private void OnChestItemSelected(InventorySlotWin win, bool isSingle)
    {
      if (!this.TransferItem(win, isSingle, this.actorInventory))
        return;
      this.invPane.RefreshInventoryWindowItems();
    }

    private bool TransferItem(InventorySlotWin win, bool isSingle, Inventory destInventory)
    {
      InventoryItem invItem = win.InvItem;
      if (invItem.Count <= 0)
        return false;
      int count = isSingle ? 1 : invItem.Count;
      invItem.Count -= destInventory.TransferTo(invItem, count);
      win.InvItem = invItem;
      win.Refresh(this.player);
      return true;
    }

    public void DragEndInventory(object sender, WindowDragEventArgs e)
    {
      InventorySlotWin window = e.Window as InventorySlotWin;
      int qty = e.Window != e.DraggingProxy ? ((InventorySlotWin) e.DraggingProxy).InvItem.Count : window.InvItem.Count;
      this.DragItemEnd(window, e.Hovered as InventorySlotWin, qty, e.Window == e.DraggingProxy);
    }

    private void DragItemEnd(
      InventorySlotWin srcWin,
      InventorySlotWin destWin,
      int qty,
      bool notProxyDrag)
    {
      if (qty <= 0 || srcWin == null || destWin == null)
        return;
      InventoryItem invItem1 = srcWin.InvItem;
      int slotId = destWin.SlotID;
      if (slotId >= (int) destWin.Inventory.PackSize && (EquipIndex) (slotId - (int) destWin.Inventory.PackSize) != Globals1.ItemTypeData[(int) invItem1.ItemID].Equip - (byte) 1)
        return;
      if (destWin.Inventory != srcWin.Inventory)
      {
        invItem1.Count -= destWin.Inventory.TransferTo(invItem1, qty, slotId);
        srcWin.InvItem = invItem1;
        srcWin.Refresh(this.player);
        destWin.Refresh(this.player);
      }
      else
      {
        InventoryItem invItem2 = destWin.InvItem;
        if (invItem1.ItemID != invItem2.ItemID)
        {
          if (notProxyDrag)
          {
            srcWin.Inventory.SwapItem(srcWin.SlotID, destWin.SlotID);
            srcWin.Refresh(this.player);
            destWin.Refresh(this.player);
          }
          else
          {
            if (invItem2.ItemID != Item.None)
              return;
            invItem1.Count -= qty;
            srcWin.InvItem = invItem1;
            invItem1.Count = qty;
            destWin.InvItem = invItem1;
            srcWin.Refresh(this.player);
            destWin.Refresh(this.player);
          }
        }
        else
        {
          if (invItem1.Count <= 0 || ItemData.HasDurability(invItem1.ItemID))
            return;
          qty = Math.Min(qty, ItemData.GetStackSize(invItem1.ItemID) - invItem2.Count);
          if (qty <= 0)
            return;
          invItem2.Count += qty;
          invItem1.Count -= qty;
          destWin.InvItem = invItem2;
          srcWin.InvItem = invItem1;
          srcWin.Refresh(this.player);
          destWin.Refresh(this.player);
        }
      }
    }
  }
}
