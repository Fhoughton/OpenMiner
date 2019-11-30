// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.ShopMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Storage;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class ShopMenu : NewGuiMenu2
  {
    private InventoryTabsPane tabsPane;
    private InventoryPane invPane;
    private InventoryBodyPane invBodyPane;
    private Inventory actorInventory;
    private ShopBlock shopBlock;
    private bool lastFiniteResources;

    public override string Name
    {
      get
      {
        return "Shop";
      }
    }

    private bool IsPlayerInventory
    {
      get
      {
        return this.actorInventory == this.player.Inventory;
      }
    }

    private bool IsPlayerShop
    {
      get
      {
        return this.shopBlock != null;
      }
    }

    private bool IsShopOwner
    {
      get
      {
        if (this.shopBlock != null)
          return this.shopBlock.IsOwner(this.player);
        return false;
      }
    }

    public ShopMenu(GameInstance instance, Player player, GlobalPoint3D p)
      : base(instance, player)
    {
      this.shopBlock = (ShopBlock) null;
      this.actorInventory = (Inventory) player.Inventory;
    }

    public ShopMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
      this.actorInventory = (Inventory) player.Inventory;
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
      this.lastFiniteResources = this.instance.IsFiniteResources;
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
      int num = 870;
      TextBox textBox1 = new TextBox("Player Inventory", x, y1, 836, 40, 0.75f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox1.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
      this.canvas.AddChild((Node) textBox1);
      TextBox textBox2 = new TextBox(this.Name + " Inventory", x + num, y1, 836, 40, 0.75f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      textBox2.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
      this.canvas.AddChild((Node) textBox2);
      int y2 = y1 + 60;
      Window window1 = new Window((string) null, x, y2 + 84, this.canvas.Size.X - x * 2, 900)
      {
        Name = "invContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window1);
      this.invPane = new InventoryPane((NewGuiMenu2) this, this.actorInventory, 10, new Point(80, 80), InventorySlotWinFlags.ShowQuantity | InventorySlotWinFlags.ShowSellPrice, new Action<InventorySlotWin, bool>(this.OnInventoryItemSelected));
      this.invPane.DragEndHandler = new Window.WindowDragHandler(this.DragEndActorInventory);
      Window window2 = this.invPane.InitWindows();
      window1.AddChild((Node) window2);
      this.invPane.RefreshInventoryWindowItems();
      window2.AdjustSizeToContainAllChildren();
      bool flag = !this.instance.IsCreativeMode || !this.player.IsAdmin;
      TextBox textBox3 = new TextBox(flag ? "Sell All" : "Clear All", (int) window1.Position.X, y2 + 38, 140, 34, 0.6f)
      {
        TextAlignX = WinTextAlignX.Center
      };
      if (flag)
        textBox3.ClickHandler += new Window.WindowHandler(this.ClickSellAll);
      else
        textBox3.ClickHandler += new Window.WindowHandler(this.ClickClearAll);
      this.canvas.AddChild((Node) textBox3);
      this.actorInventory = (Inventory) this.player.Inventory;
      this.invBodyPane = new InventoryBodyPane((NewGuiMenu2) this, this.actorInventory, new Point(80, 80), InventorySlotWinFlags.ShowQuantity | InventorySlotWinFlags.ShowSellPrice, new Action<InventorySlotWin, bool>(this.OnInventoryItemSelected));
      this.invBodyPane.DragEndHandler = new Window.WindowDragHandler(this.DragEndActorInventory);
      Window window3 = this.invBodyPane.InitWindows();
      window3.Position.X = 216f;
      window3.Position.Y = (float) ((double) window2.Position.Y + (double) window2.Size.Y + 42.0);
      window1.AddChild((Node) window3);
      this.invBodyPane.RefreshInventoryWindowItems();
      Window window4 = new Window((string) null, x + num, y2, this.canvas.Size.X - x * 2, 900)
      {
        Name = "shopContainer"
      };
      window4.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window4);
      this.tabsPane = new InventoryTabsPane((NewGuiMenu2) this, this.parentScreen.WindowManager, new Action<InventorySlotWin, bool>(this.OnShopItemSelected), new Func<Item, bool>(this.IsItemValidForShop));
      this.tabsPane.DragEndHandler = new Window.WindowDragHandler(this.DragEndShopInventory);
      window4.AddChild((Node) this.tabsPane.InitWindows(false));
      window4.AdjustSizeToContainAllChildrenDeep();
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
      if (this.lastFiniteResources == this.instance.IsFiniteResources)
        return;
      this.invPane.RefreshInventoryWindowItems();
      this.invBodyPane.RefreshInventoryWindowItems();
      this.tabsPane.InventoryPane.RefreshInventoryWindowItems();
      this.lastFiniteResources = this.instance.IsFiniteResources;
    }

    public override void Close()
    {
      this.tabsPane.Close();
      base.Close();
    }

    private bool IsItemValidForShop(Item itemID)
    {
      if (this.shopBlock != null && !this.shopBlock.Inventory.HasItem(itemID))
        return false;
      return Globals1.ItemData[(int) itemID].MinCSPrice >= 0;
    }

    private void ClickClearAll(object sender, WindowEventArgs e)
    {
      this.actorInventory.ClearItems();
      this.invPane.RefreshInventoryWindowItems();
      this.invBodyPane.RefreshInventoryWindowItems();
    }

    private void ClickSellAll(object sender, WindowEventArgs e)
    {
      bool flag = false;
      for (int slotID = 0; slotID < (int) this.actorInventory.PackSize && slotID < this.actorInventory.Count; ++slotID)
      {
        InventoryItem inventoryItem = this.actorInventory[slotID];
        if (inventoryItem.ItemID != Item.GoldPieces)
          flag |= this.TradeCore(this.invPane.GetInvWin(slotID), inventoryItem.Count, false);
      }
      if (flag)
        Sounds.PlaySound(Item.GoldPieces, ItemSoundType.Use);
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    private void OnInventoryItemSelected(InventorySlotWin win, bool isSingle)
    {
      InventoryItem invItem = win.InvItem;
      if (invItem.Count <= 0)
        return;
      int qty = isSingle ? 1 : invItem.Count;
      this.Trade(win, qty, false);
    }

    private void OnShopItemSelected(InventorySlotWin win, bool isSingle)
    {
      InventoryItem invItem = win.InvItem;
      if (invItem.Count <= 0)
        return;
      int qty = isSingle ? 1 : ItemData.GetStackSize(invItem.ItemID);
      this.Trade(win, qty, true);
    }

    public void DragEndActorInventory(object sender, WindowDragEventArgs e)
    {
      InventorySlotWin window = e.Window as InventorySlotWin;
      if (window == null)
        return;
      InventorySlotWin hovered = e.Hovered as InventorySlotWin;
      if (hovered == null)
        return;
      InventoryItem invItem1 = window.InvItem;
      int slotId = hovered.SlotID;
      if (slotId >= (int) hovered.Inventory.PackSize && (EquipIndex) (slotId - (int) hovered.Inventory.PackSize) != Globals1.ItemTypeData[(int) invItem1.ItemID].Equip - (byte) 1)
        return;
      int num1 = e.Window != e.DraggingProxy ? ((InventorySlotWin) e.DraggingProxy).InvItem.Count : invItem1.Count;
      if (num1 <= 0)
        return;
      if (hovered.Inventory != window.Inventory)
      {
        this.Trade(window, num1, false);
      }
      else
      {
        InventoryItem invItem2 = hovered.InvItem;
        if (invItem1.ItemID != invItem2.ItemID)
        {
          if (e.Window == e.DraggingProxy)
          {
            window.Inventory.SwapItem(window.SlotID, hovered.SlotID);
            window.Refresh(this.player);
            hovered.Refresh(this.player);
          }
          else
          {
            if (invItem2.ItemID != Item.None)
              return;
            invItem1.Count -= num1;
            window.InvItem = invItem1;
            invItem1.Count = num1;
            hovered.InvItem = invItem1;
            window.Refresh(this.player);
            hovered.Refresh(this.player);
          }
        }
        else
        {
          if (invItem1.Count <= 0 || ItemData.HasDurability(invItem1.ItemID))
            return;
          int num2 = Math.Min(num1, ItemData.GetStackSize(invItem1.ItemID) - invItem2.Count);
          if (num2 <= 0)
            return;
          invItem2.Count += num2;
          invItem1.Count -= num2;
          hovered.InvItem = invItem2;
          window.InvItem = invItem1;
          window.Refresh(this.player);
          hovered.Refresh(this.player);
        }
      }
    }

    public void DragEndShopInventory(object sender, WindowDragEventArgs e)
    {
      InventorySlotWin window = e.Window as InventorySlotWin;
      if (window == null)
        return;
      InventoryItem invItem1 = window.InvItem;
      if (invItem1.ItemID == Item.None)
        return;
      InventorySlotWin hovered = e.Hovered as InventorySlotWin;
      if (hovered == null || hovered.Inventory == window.Inventory)
        return;
      int slotId = hovered.SlotID;
      if (slotId >= (int) hovered.Inventory.PackSize && (EquipIndex) (slotId - (int) hovered.Inventory.PackSize) != Globals1.ItemTypeData[(int) invItem1.ItemID].Equip - (byte) 1)
        return;
      InventoryItem invItem2 = hovered.InvItem;
      if (invItem2.Count != 0 && invItem1.ItemID != invItem2.ItemID)
        return;
      int stackSize = ItemData.GetStackSize(invItem1.ItemID);
      int qty = Math.Min(stackSize, stackSize - invItem2.Count);
      if (qty <= 0)
        return;
      this.Trade(window, qty, true);
    }

    private void Trade(InventorySlotWin win, int qty, bool buy)
    {
      if (this.TradeCore(win, qty, buy))
      {
        win.Refresh(this.player);
        Sounds.PlaySound(Item.GoldPieces, ItemSoundType.Use);
      }
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    private bool TradeCore(InventorySlotWin win, int qty, bool buy)
    {
      bool flag = false;
      InventoryItem invItem = win.InvItem;
      if (!this.IsItemLocked(invItem.ItemID) && this.CanTradeItem(invItem.ItemID, buy))
      {
        if (!this.IsPlayerShop && (!this.instance.IsFiniteResources || this.instance.IsCreativeMode && this.player != null && this.player.IsAdmin) || this.player != null && this.player.IsGodOrTester)
        {
          flag = this.InfiniteTrade(win, qty, buy);
          if (flag)
            this.invPane.RefreshInventoryWindowItems();
        }
        else
        {
          flag = this.FiniteTrade(win, qty, buy);
          if (flag)
          {
            this.invPane.RefreshInventoryWindowItems();
            this.tabsPane.InventoryPane.RefreshInventoryWindowItems();
          }
        }
      }
      return flag;
    }

    private bool InfiniteTrade(InventorySlotWin win, int qty, bool buy)
    {
      InventoryItem invItem = win.InvItem;
      if (this.IsPlayerShop || !buy)
        qty = Math.Min(invItem.Count, qty);
      if (qty <= 0)
        return false;
      if (buy)
        return this.InfiniteBuy(invItem, qty);
      return this.ReturnToShop(win, qty);
    }

    private bool InfiniteBuy(InventoryItem item, int qty)
    {
      if (item.MaxDurability != (ushort) 0 && qty != 1)
        return false;
      if (this.IsPlayerInventory)
        return this.player.AddToInventory(item.ItemID, qty) > 0;
      return this.actorInventory.AddToInventory(item.ItemID, qty) > 0;
    }

    private bool ReturnToShop(InventorySlotWin win, int qty)
    {
      bool flag = false;
      InventoryItem invItem = win.InvItem;
      if (this.shopBlock == null || this.shopBlock.Inventory.HasItem(invItem.ItemID))
      {
        invItem.Count -= qty;
        win.InvItem = invItem;
        flag = true;
      }
      return flag;
    }

    private bool FiniteTrade(InventorySlotWin win, int qty, bool buy)
    {
      InventoryItem invItem = win.InvItem;
      if (invItem.ItemID == Item.GoldPieces)
        return false;
      if (this.IsPlayerShop || !buy)
      {
        qty = Math.Min(invItem.Count, qty);
        if (this.IsPlayerShop && !buy)
        {
          ushort maxDurability = invItem.MaxDurability;
          if (maxDurability > (ushort) 0 && (int) invItem.Durability < (int) maxDurability)
          {
            this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: Cannot stock a damaged item", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.player.PlayerIndex));
            return false;
          }
        }
      }
      if (qty > 0)
      {
        int price = this.GetPrice(invItem, buy);
        if (price >= 0)
        {
          if (buy)
            return this.Buy(win, qty, price);
          if (price > 0 || this.IsShopOwner && invItem.ItemID == Item.GoldPieces)
            return this.Sell(win, qty, price);
        }
      }
      return false;
    }

    private bool Buy(InventorySlotWin win, int qty, int price)
    {
      bool flag = false;
      InventoryItem invItem = win.InvItem;
      if ((invItem.MaxDurability == (ushort) 0 || qty == 1) && (this.IsShopOwner || (long) this.player.GoldCoinsOnPerson >= (long) price * (long) qty))
      {
        qty = !this.IsPlayerInventory ? this.actorInventory.AddToInventory(invItem.ItemID, qty) : this.player.AddToInventory(invItem.ItemID, qty);
        if (qty > 0)
        {
          if (this.IsPlayerShop)
          {
            win.Inventory.DecrementItem(invItem.ItemID, qty);
            if (!this.IsShopOwner)
              win.Inventory.IncrementItem(Item.GoldPieces, price * qty);
          }
          if (!this.IsShopOwner)
          {
            this.actorInventory.DecrementItem(Item.GoldPieces, price * qty);
            if (this.IsPlayerInventory)
              this.player.Raise_ItemTraded(invItem.ItemID, qty, price * qty, false);
          }
          flag = true;
        }
      }
      return flag;
    }

    private bool Sell(InventorySlotWin win, int qty, int price)
    {
      InventoryItem invItem = win.InvItem;
      int num = qty;
      if (win.Inventory.HasItem(invItem.ItemID))
      {
        while (qty > 0)
        {
          bool flag = !this.IsPlayerShop || win.Inventory.ItemCount(Item.GoldPieces) >= price;
          if (this.IsShopOwner || flag && (this.IsPlayerInventory ? this.player.AddToInventory(Item.GoldPieces, price) : this.actorInventory.AddToInventory(Item.GoldPieces, price)) > 0)
          {
            if (this.IsPlayerShop)
            {
              if (!this.IsShopOwner)
                win.Inventory.DecrementItem(Item.GoldPieces, price);
              win.Inventory.IncrementItem(invItem.ItemID, 1);
            }
            --qty;
            --invItem.Count;
          }
          else
            break;
        }
        win.InvItem = invItem;
        if (!this.IsShopOwner && num > qty && this.IsPlayerInventory)
          this.player.Raise_ItemTraded(invItem.ItemID, num - qty, price * (num - qty), true);
      }
      return num > qty;
    }

    private int GetPrice(InventoryItem item, bool buy)
    {
      if (this.IsShopOwner && item.ItemID == Item.GoldPieces)
        return 0;
      int itemId = (int) item.ItemID;
      Player player = this.shopBlock != null ? this.instance.GetPlayer(this.shopBlock.Gamertag) : (Player) null;
      PriceList priceList = !this.IsPlayerShop || this.shopBlock.PriceList == null ? player?.DefaultPriceList : this.shopBlock.PriceList;
      int num;
      if (buy)
      {
        num = priceList != null ? (priceList.Prices[itemId].ForSale ? priceList.Prices[itemId].Sell : -1) : ItemData.GetMinCustBuyPrice(item.ItemID);
      }
      else
      {
        num = priceList != null ? (priceList.Prices[itemId].ForSale ? priceList.Prices[itemId].FinalBuy : -1) : ItemData.GetMinCustSellPrice(item.ItemID);
        if (item.MaxDurability > (ushort) 0 && item.ItemID != Item.Book)
          num = (int) ((double) num * ((double) item.Durability / (double) item.MaxDurability));
      }
      return num;
    }

    private bool IsItemLocked(Item itemID)
    {
      if (this.instance.IsItemLocked(itemID))
        return !this.player.IsGodOrTester;
      return false;
    }

    private bool CanTradeItem(Item itemID, bool buy)
    {
      if (itemID == Item.SkeletonKey && !this.IsPlayerShop && buy)
        return this.player.IsAdmin;
      return true;
    }
  }
}
