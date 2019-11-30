// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.InventorySlotWin
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class InventorySlotWin : TextBox
  {
    private NewGuiMenu2 parentTab;
    private InventorySlotWinFlags flags;
    private Inventory inventory;
    private int slotID;

    public int SlotID
    {
      get
      {
        return this.slotID;
      }
    }

    public Inventory Inventory
    {
      get
      {
        return this.inventory;
      }
    }

    public InventoryItem InvItem
    {
      get
      {
        return this.inventory[this.slotID];
      }
      set
      {
        this.inventory[this.slotID] = value;
      }
    }

    public override Window GetDragProxy(WindowDragEventArgs e)
    {
      InventoryItem inventoryItem = this.inventory[this.slotID];
      if (!e.RightButton || inventoryItem.Count <= 1)
        return base.GetDragProxy(e);
      InventorySlotWin inventorySlotWin = new InventorySlotWin(this);
      inventorySlotWin.flags = this.flags;
      inventorySlotWin.slotID = 0;
      inventorySlotWin.inventory = new Inventory(1);
      inventoryItem.Count /= 2;
      inventorySlotWin.inventory[0] = inventoryItem;
      inventorySlotWin.Refresh(GameInstance.Instance.NetworkManager.GetLocalGamer(e.PlayerIndex).Tag as Player);
      return (Window) inventorySlotWin;
    }

    public InventorySlotWin(
      NewGuiMenu2 parentTab,
      int x,
      int y,
      int width,
      int height,
      InventorySlotWinFlags flags,
      Inventory inventory,
      int slotID)
      : base((string) null, x, y, width, height, 0.4f)
    {
      this.parentTab = parentTab;
      this.flags = flags;
      this.inventory = inventory;
      this.slotID = slotID;
      this.AddFlags(Window.WinFlags.BorderRounded | Window.WinFlags.UseHoverColorIfDraggedOver | Window.WinFlags.DragCopy);
      this.TextAlignX = WinTextAlignX.Center;
      this.TextAlignY = WinTextAlignY.Bottom;
      this.TextOffset.Y = -6f;
      this.BorderThickness = 1;
      this.DragEnablePressTime = 0.21f;
    }

    public InventorySlotWin(InventorySlotWin win)
      : base((TextBox) win)
    {
      this.parentTab = win.parentTab;
    }

    public void Refresh(Player player)
    {
      int num = 64;
      bool flag1 = (this.flags & InventorySlotWinFlags.ShowQuantity) > InventorySlotWinFlags.None;
      Rectangle rectangle = new Rectangle((this.Size.X - num) / 2, (this.Size.Y - num) / 2 - (flag1 ? 2 : 0), num, num);
      InventoryItem inventoryItem = this.inventory[this.slotID];
      if (inventoryItem.ItemID != Item.None)
      {
        if (this.Texture == null)
          this.LoadTexture(GraphicStatics.TexturePack.GetTexureForItem(inventoryItem.ItemID));
        else
          this.Texture.Texture = GraphicStatics.TexturePack.GetTexureForItem(inventoryItem.ItemID);
        this.Texture.DestRect = new Rectangle?(rectangle);
        this.Texture.SrRect = new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(inventoryItem.ItemID));
        bool flag2 = inventoryItem.Count > 0 && this.parentTab.Instance.IsItemLocked(inventoryItem.ItemID);
        this.Texture.TintColor = flag2 ? new Color(0.25f, 0.25f, 0.25f, 0.25f) : Color.White;
        string tip = ItemData2.ForDisplay(player, inventoryItem);
        if (flag1)
        {
          this.Text = inventoryItem.Count.ToString();
          if (inventoryItem.Durability == (ushort) 0)
            tip += string.Format(": {0:N0} " + (inventoryItem.Count > 1 ? "units" : "unit"), (object) inventoryItem.Count);
        }
        if (flag2)
        {
          tip += "\n[Locked]";
        }
        else
        {
          if (inventoryItem.ShowDurabilityBar)
            tip = !flag1 ? tip + string.Format("\nDurability: {0:N0}", (object) inventoryItem.Durability) : tip + string.Format("\nDurability: {0:N0} of {1:N0}", (object) inventoryItem.Durability, (object) Math.Max(inventoryItem.Durability, ItemData.GetItemDurability(inventoryItem.ItemID)));
          if (inventoryItem.ItemID != Item.GoldPieces)
          {
            if ((this.flags & InventorySlotWinFlags.ShowBuyPrice) > InventorySlotWinFlags.None && this.parentTab.Instance.IsFiniteResources)
              tip += string.Format("\nBuy Price: {0:N0}gp", (object) ItemData.GetMinCustBuyPrice(inventoryItem.ItemID));
            if ((this.flags & InventorySlotWinFlags.ShowSellPrice) > InventorySlotWinFlags.None && this.parentTab.Instance.IsFiniteResources)
            {
              int minCustSellPrice = ItemData.GetMinCustSellPrice(inventoryItem.ItemID);
              tip += string.Format("\nSell Price: {0:N0}gp", (object) minCustSellPrice);
              if (inventoryItem.Count > 1)
                tip += string.Format("\nTotal Sell Price: {0:N0}gp", (object) (minCustSellPrice * inventoryItem.Count));
            }
            if (!player.CanUseItem(inventoryItem.ItemID))
            {
              SkillDataXML skillDataXml = Globals1.SkillData[(int) inventoryItem.ItemID];
              if (skillDataXml.UseSkill != SkillType.None)
              {
                int level = player.SkillsData[(int) skillDataXml.UseSkill].Level;
                tip += string.Format("\nSkill: {0} {1} of {2}", (object) skillDataXml.UseSkill.ToString(), (object) level, (object) skillDataXml.UseReq);
              }
            }
          }
        }
        this.SetToolTip(tip, 0.25f);
      }
      else
      {
        if (this.Texture != null)
          this.Texture.Texture = (Texture2D) null;
        this.Text = (string) null;
        this.SetToolTip((string) null);
      }
    }

    public override void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
      InventoryItem inventoryItem = this.inventory[this.slotID];
      bool flag1 = inventoryItem.Count > 0 && (this.flags & InventorySlotWinFlags.ShowQuantity) > InventorySlotWinFlags.None;
      bool flag2 = inventoryItem.ShowDurabilityBar;
      bool flag3 = this.parentTab.Player.CanUseItem(inventoryItem.ItemID);
      if (!flag1 && !flag3)
      {
        flag1 = true;
        flag2 = false;
      }
      if (flag1)
      {
        Color color = this.Colors.BorderColor * 0.5f;
        Rectangle rectangle = new Rectangle(bound.X, bound.Y, bound.Width, 5);
        if (flag2)
        {
          ushort num = Math.Max(inventoryItem.Durability, ItemData.GetItemDurability(inventoryItem.ItemID));
          rectangle.Width = (int) ((double) inventoryItem.Durability / (double) num * (double) rectangle.Width);
          spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, Color.Green);
          if ((int) inventoryItem.Durability < (int) num)
          {
            rectangle.X += rectangle.Width;
            rectangle.Width = bound.Width - rectangle.Width;
            spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, Color.Red);
            rectangle.X = bound.X;
            rectangle.Width = bound.Width;
          }
          rectangle.Y += rectangle.Height;
          rectangle.Height = 1;
          spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, color);
        }
        rectangle.Y = bound.Y + bound.Height - 16;
        rectangle.Height = 1;
        spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, color);
        ++rectangle.Y;
        rectangle.Height = 15;
        spriteBatch.DrawGradient(rectangle, 16, 16, (flag3 ? Color.Black : Color.Red) * 0.4f, Matrix.Identity);
      }
      base.Draw(spriteBatch, bound, scale, alpha, isEnabled);
      if (inventoryItem.Count <= 0 || !this.parentTab.Instance.IsItemLocked(inventoryItem.ItemID))
        return;
      Texture2D lockedTexture = GraphicStatics.LockedTexture;
      int width = lockedTexture.Width * 2;
      int height = lockedTexture.Height * 2;
      Rectangle destinationRectangle = new Rectangle(bound.X + (bound.Width - width) / 2, bound.Y + (bound.Height - height) / 2, width, height);
      spriteBatch.Draw(lockedTexture, destinationRectangle, Color.White * 0.8f);
    }
  }
}
