// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ChestScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class ChestScreen : BaseInventoryScreen
  {
    private string chestDesc = "";
    protected int page;
    protected int pagesize;
    protected string pageString;
    protected ChestBlock chest;
    protected Block blockID;

    protected override int CoWindowHeight
    {
      get
      {
        return this.pagesize / 10 * 46 + 3 + 60;
      }
    }

    protected override Inventory CoWindowInventory
    {
      get
      {
        return this.chest.Inventory;
      }
    }

    protected override Inventory CursorInventory
    {
      get
      {
        if (this.currentSlotID <= 29)
          return base.CursorInventory;
        return this.chest.Inventory;
      }
    }

    protected override InventoryItem CursorItem
    {
      get
      {
        if (this.currentSlotID <= 29)
          return base.CursorItem;
        InventoryItem inventoryItem = this.chest.Inventory[this.currentSlotID - 30 + this.page * this.pagesize];
        if (!this.chest.Inventory.AllowZeroCountItems && inventoryItem.Count <= 0)
          return InventoryItem.Empty;
        return inventoryItem;
      }
      set
      {
        if (this.currentSlotID > 29)
          this.chest.Inventory[this.currentSlotID - 30 + this.page * this.pagesize] = value;
        else
          this.inventory[this.currentSlotID] = value;
      }
    }

    protected override int CursorItemCount
    {
      get
      {
        return this.CursorItem.Count;
      }
      set
      {
        if (this.currentSlotID > 29)
          this.chest.Inventory.SetItemCount(this.currentSlotID - 30 + this.page * this.pagesize, value);
        else
          this.inventory.SetItemCount(this.currentSlotID, value);
      }
    }

    protected override bool CanEditInventory
    {
      get
      {
        if (this.blockID == Block.Chest)
          return this.player.HasPermission(Permissions.Adventure);
        return true;
      }
    }

    protected virtual int PageCount
    {
      get
      {
        return ((int) this.chest.Inventory.PackSize - 1) / this.pagesize + 1;
      }
    }

    public ChestScreen(GameInstance instance, Player player, Inventory inventory)
      : base(instance, player, (Actor) player, inventory, 0)
    {
      this.blockID = Block.None;
      this.chest = new ChestBlock(GlobalPoint3D.Zero, this.blockID, inventory);
    }

    public ChestScreen(GameInstance instance, Player player, Block blockID)
      : base(instance, player, (Actor) player, (Inventory) player.Inventory, 0)
    {
      this.blockID = blockID;
      switch (blockID)
      {
        case Block.ItemShop:
        case Block.BlockShop:
          this.chest = (ChestBlock) new ShopBlock(GlobalPoint3D.Zero, this.inventory);
          break;
        default:
          this.chest = new ChestBlock(GlobalPoint3D.Zero, blockID, this.inventory);
          break;
      }
      this.chestDesc = Utils.InsertSpacesBeforeCapitals(blockID.ToString());
    }

    public ChestScreen(GameInstance instance, Player player, GlobalPoint3D p, Block blockID)
      : base(instance, player, (Inventory) player.Inventory, 0)
    {
      this.blockID = blockID;
      this.chest = instance.MapStrategyTM.GetOrAddDataBlock(p, blockID, UpdateBlockMethod.Player, this.PlayerID, true) as ChestBlock;
      this.chestDesc = Utils.InsertSpacesBeforeCapitals(blockID.ToString());
    }

    public override void LoadContent()
    {
      if (this.chest.Inventory == null)
        this.chest.Inventory = new Inventory(50);
      this.page = 0;
      this.pagesize = (int) Math.Max((short) 10, Math.Min((short) 50, this.chest.Inventory.PackSize));
      this.pageString = (this.page + 1).ToString();
      base.LoadContent();
    }

    protected override void OnScreenClosed()
    {
      base.OnScreenClosed();
      if (!this.IsPlayerInventory)
        return;
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.chest, (this.blockID != Block.LockedChest || this.chest.Gamertag == null) && !this.chest.Inventory.HasItems());
      this.player.Raise_ChestClosed();
    }

    protected override void PrevPageButtonPressed()
    {
      if (--this.page < 0)
        this.page = this.PageCount - 1;
      this.pageString = (this.page + 1).ToString();
      if (this.PageCount <= 1)
        return;
      this.CursorMoved();
    }

    protected override void NextPageButtonPressed()
    {
      if (++this.page >= this.PageCount)
        this.page = 0;
      this.pageString = (this.page + 1).ToString();
      if (this.PageCount <= 1)
        return;
      this.CursorMoved();
    }

    protected override void MoveLeftCore()
    {
      if (this.currentSlotID % 10 == 0)
        this.currentSlotID += 9;
      else
        --this.currentSlotID;
    }

    protected override void MoveRightCore()
    {
      if ((this.currentSlotID + 1) % 10 == 0)
        this.currentSlotID -= 9;
      else
        ++this.currentSlotID;
    }

    protected override void MoveUpCore()
    {
      int num = 30 + (this.pagesize - 10);
      if (this.currentSlotID < num)
        this.currentSlotID += 10;
      else
        this.currentSlotID -= num;
    }

    protected override void MoveDownCore()
    {
      int num = 30 + (this.pagesize - 10);
      if (this.currentSlotID < 30)
        this.currentSlotID += num;
      else
        this.currentSlotID -= 10;
    }

    protected override bool IsUnliftValid
    {
      get
      {
        if (!base.IsUnliftValid)
          return false;
        if (this.currentSlotID <= 29)
          return this.inventory.ItemAllowed(this.liftedItem.ItemID);
        return this.chest.Inventory.ItemAllowed(this.liftedItem.ItemID);
      }
    }

    protected override bool SetCurrentSlotCoWindow(Point pos)
    {
      int num1 = 30;
      int num2 = (int) this.chest.Inventory.PackSize / 10;
      int num3 = 30 + 171 + 36;
      Rectangle rectangle = new Rectangle(0, 0, 46, 46);
      for (int index1 = 0; index1 < num2; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          rectangle.X = index2 * 46 + this.screenRect.X + 16;
          rectangle.Y = this.screenRect.Y + this.screenRect.Height - num3 - (index1 + 1) * 46 - 3 + 16;
          int num4 = index2 + index1 * 10 + num1;
          if (rectangle.Contains(pos))
          {
            this.currentSlotID = num4;
            return true;
          }
        }
      }
      return false;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      this.CheckForClear();
    }

    protected virtual void CheckForClear()
    {
      Block blockId = (Block) this.instance.Map.GetBlockID(this.chest.Point);
      if ((uint) blockId <= 50U)
      {
        if (blockId == Block.Bookcase || blockId == Block.Chest)
          return;
      }
      else if (blockId == Block.LockedChest || blockId == Block.Crate || blockId == Block.Safe)
        return;
      this.ExitScreen();
    }

    protected override void DrawCoWindow()
    {
      int num = 171;
      this.DrawGrid(this.pagesize / 10, num + 36, this.chest.Inventory, 30, this.page * this.pagesize, false, false);
      this.spriteBatch.DrawString(this.Font, this.chestDesc, new Vector2((float) (this.screenRect.X + 14), (float) (this.screenRect.Y + 6)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 15;
      screenRect.Y = this.screenRect.Y + this.screenRect.Height - num - 46;
      screenRect.Width = 25;
      screenRect.Height = 25;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.TransferItem, screenRect);
      this.spriteBatch.DrawString(this.Font, "Transfer", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      screenRect.X += 370;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, screenRect);
      this.spriteBatch.DrawString(this.Font, "Close", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override Rectangle GetSlotRect(int slotID)
    {
      Rectangle slotRect = base.GetSlotRect(slotID);
      if (slotID > 29)
        slotRect.Y -= 39;
      return slotRect;
    }
  }
}
