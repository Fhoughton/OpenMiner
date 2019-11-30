// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.HotBarRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;

namespace StudioForge.TotalMiner.Renderers
{
  internal class HotBarRenderer
  {
    private int slotSize = 48;
    private SpriteBatchSafe spriteBatch;
    private SpriteBatchSafe spriteBatchPoint;
    private SpriteBatchSafe spriteBatchText;
    private Rectangle fullBar;
    private InventoryItem lastLeftHandItem;
    private InventoryItem lastRightHandItem;
    private string lastLeftHandItemName;
    private string lastRightHandItemName;

    public void LoadContent()
    {
      this.spriteBatch = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchText = GraphicStatics.SpriteBatchPool.GetNextItem();
    }

    public void UnloadContent()
    {
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchText);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch);
    }

    public void Draw(Player player, Player virtualPlayer)
    {
      if (!virtualPlayer.IsHotBarVisible)
        return;
      int height1 = GraphicStatics.HUDPos(player).Height;
      int width = this.slotSize + (this.slotSize - 3) * 9 + 18;
      int height2 = this.slotSize + 18;
      int num1 = GraphicStatics.DefaultViewport.Width / 2;
      int num2 = player.Settings.CompassTop ? 0 : 40;
      int y = height1 - height2 - num2;
      this.fullBar = new Rectangle(num1 - width / 2, y, width, height2);
      this.DrawFull(player, virtualPlayer);
    }

    private void DrawFull(Player player, Player virtualPlayer)
    {
      Matrix screenMatrix = player.GetScreenMatrix(ScreenForScale.HotBar);
      this.spriteBatch.BeginTM(screenMatrix);
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, screenMatrix);
      this.spriteBatchText.BeginTM(screenMatrix);
      int cursorSlot1ID = virtualPlayer.Inventory.HotBarLeftSlotID;
      int cursorSlot2ID = virtualPlayer.Inventory.HotBarRightSlotID;
      Color cursorSlot1Color;
      Color cursorSlot2Color;
      bool cursorSlot1HasPriority;
      if (virtualPlayer.Settings.WieldType == WieldType.LeftHand)
      {
        cursorSlot1Color = virtualPlayer.HotbarRightCursorColor;
        cursorSlot2Color = virtualPlayer.HotbarLeftCursorColor;
        cursorSlot1HasPriority = true;
        if (cursorSlot2ID == cursorSlot1ID)
          cursorSlot2ID = -1;
      }
      else
      {
        cursorSlot1Color = virtualPlayer.HotbarLeftCursorColor;
        cursorSlot2Color = virtualPlayer.HotbarRightCursorColor;
        cursorSlot1HasPriority = virtualPlayer.Settings.WieldType == WieldType.BothHands && virtualPlayer.HotbarLeftCursorHasVisualPriority;
        if (cursorSlot1HasPriority)
        {
          if (cursorSlot2ID == cursorSlot1ID)
            cursorSlot2ID = -1;
        }
        else if (cursorSlot1ID == cursorSlot2ID)
          cursorSlot1ID = -1;
      }
      this.DrawFrame(virtualPlayer, this.fullBar, 0, cursorSlot1ID, cursorSlot2ID, cursorSlot1Color, cursorSlot2Color, cursorSlot1HasPriority);
      this.spriteBatch.End();
      this.spriteBatchPoint.End();
      this.spriteBatchText.End();
    }

    private void DrawFrame(
      Player virtualPlayer,
      Rectangle rect,
      int itemSlotOffset,
      int cursorSlot1ID,
      int cursorSlot2ID,
      Color cursorSlot1Color,
      Color cursorSlot2Color,
      bool cursorSlot1HasPriority)
    {
      float hotBarTransparency = virtualPlayer.HotBarTransparency;
      this.spriteBatch.DrawFilledBox(rect, 4, new Color(0.1f, 0.1f, 0.1f) * hotBarTransparency, new Color(0.2f, 0.2f, 0.2f) * 0.9f * hotBarTransparency);
      ++rect.X;
      ++rect.Y;
      this.DrawGrid(virtualPlayer, rect, (Inventory) virtualPlayer.Inventory, itemSlotOffset, 10, hotBarTransparency);
      if (cursorSlot1HasPriority)
      {
        this.DrawCursor(rect, cursorSlot2ID, cursorSlot2Color * hotBarTransparency);
        this.DrawCursor(rect, cursorSlot1ID, cursorSlot1Color * hotBarTransparency);
      }
      else
      {
        this.DrawCursor(rect, cursorSlot1ID, cursorSlot1Color * hotBarTransparency);
        this.DrawCursor(rect, cursorSlot2ID, cursorSlot2Color * hotBarTransparency);
      }
      this.DrawCursorItemNames(virtualPlayer, rect, hotBarTransparency, cursorSlot1ID >= 0, cursorSlot2ID >= 0);
    }

    protected void DrawGrid(
      Player virtualPlayer,
      Rectangle rect,
      Inventory inventory,
      int itemSlotOffset,
      int gridWidth,
      float alpha)
    {
      Rectangle slotRect = new Rectangle(0, 0, this.slotSize, this.slotSize);
      for (int index = 0; index < gridWidth; ++index)
      {
        slotRect.X = index * (this.slotSize - 3) + rect.X + 8;
        slotRect.Y = rect.Y + 8;
        this.DrawSlot(slotRect.X, slotRect.Y, alpha);
        InventoryItem inventoryItem = inventory[index + itemSlotOffset];
        this.DrawItem(virtualPlayer, slotRect, inventoryItem, alpha);
      }
    }

    private void DrawCursor(Rectangle rect, int slotID, Color color)
    {
      if (slotID < 0)
        return;
      GraphicStatics.DrawCursor(this.spriteBatch, this.GetSlotRect(rect, slotID), color);
    }

    private void DrawCursorItemNames(
      Player virtualPlayer,
      Rectangle rect,
      float alpha,
      bool drawLeft,
      bool drawRight)
    {
      InventoryItem inventoryItem1 = virtualPlayer.LeftHand.HasItem ? virtualPlayer.Inventory.LeftHand : InventoryItem.Empty;
      if (inventoryItem1.ItemID == Item.Blueprint || inventoryItem1.ItemID == Item.Book || inventoryItem1.ItemID == Item.Clipboard || inventoryItem1.ItemID != this.lastLeftHandItem.ItemID || (int) inventoryItem1.Durability != (int) this.lastLeftHandItem.Durability)
      {
        this.lastLeftHandItem = inventoryItem1;
        this.lastLeftHandItemName = ItemData2.ForDisplay(virtualPlayer, inventoryItem1);
      }
      if (inventoryItem1.ItemID != Item.None)
      {
        Vector2 position = new Vector2((float) rect.X, (float) (rect.Y - 22));
        this.spriteBatch.DrawString(CoreGlobals.GameFont, this.lastLeftHandItemName, position, Color.White * alpha, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      }
      InventoryItem inventoryItem2 = virtualPlayer.RightHand.HasItem ? virtualPlayer.Inventory.RightHand : InventoryItem.Empty;
      if (inventoryItem2.ItemID == Item.Blueprint || inventoryItem2.ItemID == Item.Book || inventoryItem2.ItemID == Item.Clipboard || inventoryItem2.ItemID != this.lastRightHandItem.ItemID || (int) inventoryItem2.Durability != (int) this.lastRightHandItem.Durability)
      {
        this.lastRightHandItem = inventoryItem2;
        this.lastRightHandItemName = ItemData2.ForDisplay(virtualPlayer, inventoryItem2);
      }
      if (inventoryItem2.ItemID == Item.None)
        return;
      Vector2 position1 = new Vector2((float) (rect.X + rect.Width - 3), (float) (rect.Y - 22));
      position1.X -= CoreGlobals.GameFont.MeasureString(this.lastRightHandItemName).X * 0.5f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, this.lastRightHandItemName, position1, Color.White * alpha, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
    }

    protected void DrawSlot(int x, int y, float alpha)
    {
      Rectangle rect = new Rectangle(x, y, this.slotSize, this.slotSize);
      Color color = new Color(0.4f, 0.4f, 0.4f, 1f) * alpha;
      this.spriteBatch.DrawFilledBox(rect, 3, color, color * 0.25f);
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.X = rect.X + 3;
      destinationRectangle.Y = rect.Y + 3;
      destinationRectangle.Width = rect.Width - 6;
      destinationRectangle.Height = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black * alpha);
      destinationRectangle.Height = rect.Height - 6;
      destinationRectangle.Width = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black * alpha);
    }

    protected void DrawItem(
      Player virtualPlayer,
      Rectangle slotRect,
      InventoryItem item,
      float alpha)
    {
      if (item.ItemID == Item.None || item.Count <= 0)
        return;
      if (!virtualPlayer.CanUseItem(item.ItemID))
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, slotRect.Expand(-3), Color.DarkRed * 0.3f * alpha);
      GraphicStatics.DrawItem(this.spriteBatch, this.spriteBatchPoint, this.spriteBatchText, slotRect.X, slotRect.Y, item, true, false, alpha);
    }

    protected virtual Rectangle GetSlotRect(Rectangle rect, int slotID)
    {
      return new Rectangle(slotID * (this.slotSize - 3) + rect.X + 8, rect.Y + 8, this.slotSize, this.slotSize);
    }
  }
}
