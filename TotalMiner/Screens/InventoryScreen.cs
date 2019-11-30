// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.InventoryScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class InventoryScreen : BaseInventoryScreen
  {
    private Rectangle equipScreenRect;
    private Rectangle statsScreenRect;
    private Rectangle[] boySlotRects;
    private EquipmentInventory equipInventory;
    private bool canViewOthersInventory;
    private int currentPlayerIndex;
    private int headIndex;
    private int neckIndex;
    private int bodyIndex;
    private int legsIndex;
    private int feetIndex;
    private int leftIndex;
    private int rightIndex;
    private Player localPlayer;

    private bool IsViewingLocalPlayer
    {
      get
      {
        return this.localPlayer == this.player;
      }
    }

    protected override int CoWindowHeight
    {
      get
      {
        return 0;
      }
    }

    public InventoryScreen(GameInstance instance, Player player, Actor inventoryOwner)
      : base(instance, player, inventoryOwner, 0)
    {
      this.equipInventory = this.inventory as EquipmentInventory;
    }

    public InventoryScreen(GameInstance instance, Player player, int cursorID)
      : base(instance, player, (Actor) player, cursorID)
    {
      this.equipInventory = this.inventory as EquipmentInventory;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.screenRect.Y = 566 - this.screenRect.Height;
      this.CalcBoySlotRects();
      this.borderColor = new Color(0.1f, 0.1f, 0.1f);
      this.clientBackColor = GraphicStatics.WindowClientColor;
      this.Matrix = this.player.GetScreenMatrix(this.screenRect.Merge(this.equipScreenRect.Merge(this.statsScreenRect)));
      this.localPlayer = this.instance.GetLocalPlayer(this.ControllingPlayer.Value);
      this.canViewOthersInventory = this.localPlayer.IsGodOrTester || this.localPlayer.IsDeveloper;
      this.currentPlayerIndex = this.instance.NetworkManager.AllEnabledGamers.IndexOf(this.player.Gamer);
    }

    protected override void OnScreenRemovedCore()
    {
      this.player = this.localPlayer;
      base.OnScreenRemovedCore();
    }

    protected override void OnScreenClosed()
    {
      base.OnScreenClosed();
      this.player.Raise_InventoryClosed();
    }

    private void CalcBoySlotRects()
    {
      this.equipScreenRect = this.screenRect;
      this.equipScreenRect.Width = 173;
      this.equipScreenRect.Height = 269;
      this.equipScreenRect.X = this.screenRect.X + this.screenRect.Width - this.equipScreenRect.Width;
      this.equipScreenRect.Y = this.screenRect.Y - this.equipScreenRect.Height - 8;
      this.statsScreenRect = this.screenRect;
      this.statsScreenRect.Width = 282;
      this.statsScreenRect.Height = 164;
      this.statsScreenRect.X = this.equipScreenRect.X - this.statsScreenRect.Width - 8;
      this.statsScreenRect.Y = this.screenRect.Y - this.statsScreenRect.Height - 8;
      this.boySlotRects = new Rectangle[7];
      int x = this.equipScreenRect.X + 13 + 49;
      int y = this.equipScreenRect.Y + 14;
      int equipIndexStart = (int) this.inventory.EquipIndexStart;
      this.headIndex = this.equipInventory.HeadIndex - equipIndexStart;
      this.neckIndex = this.equipInventory.NeckIndex - equipIndexStart;
      this.bodyIndex = this.equipInventory.BodyIndex - equipIndexStart;
      this.legsIndex = this.equipInventory.LegsIndex - equipIndexStart;
      this.feetIndex = this.equipInventory.FeetIndex - equipIndexStart;
      this.leftIndex = this.equipInventory.LeftSideIndex - equipIndexStart;
      this.rightIndex = this.equipInventory.RightSideIndex - equipIndexStart;
      this.boySlotRects[this.headIndex] = new Rectangle(x, y, 49, 49);
      this.boySlotRects[this.neckIndex] = new Rectangle(x, y + 46, 49, 49);
      this.boySlotRects[this.bodyIndex] = new Rectangle(x, y + 92, 49, 49);
      this.boySlotRects[this.legsIndex] = new Rectangle(x, y + 138, 49, 49);
      this.boySlotRects[this.feetIndex] = new Rectangle(x, y + 184, 49, 49);
      this.boySlotRects[this.leftIndex] = new Rectangle(x + 46, y + 92, 49, 49);
      this.boySlotRects[this.rightIndex] = new Rectangle(x - 46, y + 92, 49, 49);
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      bool flag = currentGamePadState.Buttons.LeftStick == ButtonState.Pressed;
      if (this.IsViewingLocalPlayer && !flag)
      {
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.EquipItemLeft))
        {
          this.inventoryOwner.EquipItem(this.GetHandToEquipOverride(InventoryHand.Left), this.currentSlotID);
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.EquipItemRight))
        {
          this.inventoryOwner.EquipItem(this.GetHandToEquipOverride(InventoryHand.Right), this.currentSlotID);
          return true;
        }
        if (this.IsPlayerInventory && InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.OpenCrafting))
        {
          this.ScreenManager.AddScreen((GameScreen) new CraftingScreen(this.instance, this.player), this.ControllingPlayer);
          this.ExitScreen();
        }
      }
      if (this.canViewOthersInventory && flag)
      {
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp))
          this.ShowPrevPlayer();
        else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown))
          this.ShowNextPlayer();
      }
      return base.HandleInput(input);
    }

    private InventoryHand GetHandToEquipOverride(InventoryHand hand)
    {
      if (this.player.Settings.WieldType == WieldType.LeftHand)
        return InventoryHand.Left;
      if (this.player.Settings.WieldType == WieldType.RightHand)
        return InventoryHand.Right;
      return hand;
    }

    protected override void TransferItems()
    {
      if (this.CursorItemCount <= 0)
        return;
      if (this.currentSlotID >= (int) this.inventory.PackSize)
        this.TransferCursorItemTo(this.inventory);
      else
        this.inventoryOwner.EquipItem(InventoryHand.None, this.currentSlotID);
    }

    private void ShowPrevPlayer()
    {
      if (--this.currentPlayerIndex < 0)
        this.currentPlayerIndex = this.instance.NetworkManager.AllEnabledGamers.Count - 1;
      this.player = this.instance.NetworkManager.AllEnabledGamers[this.currentPlayerIndex].Tag as Player;
      this.inventoryOwner = (Actor) this.player;
      this.inventory = (Inventory) this.player.Inventory;
    }

    private void ShowNextPlayer()
    {
      if (++this.currentPlayerIndex >= this.instance.NetworkManager.AllEnabledGamers.Count)
        this.currentPlayerIndex = 0;
      this.player = this.instance.NetworkManager.AllEnabledGamers[this.currentPlayerIndex].Tag as Player;
      this.inventoryOwner = (Actor) this.player;
      this.inventory = (Inventory) this.player.Inventory;
    }

    protected override bool IsLiftValid
    {
      get
      {
        return this.IsViewingLocalPlayer;
      }
    }

    protected override bool IsLiftAllValid
    {
      get
      {
        return this.IsViewingLocalPlayer;
      }
    }

    protected override bool IsLiftSingleValid
    {
      get
      {
        return this.IsViewingLocalPlayer;
      }
    }

    protected override void MoveLeftCore()
    {
      if (this.currentSlotID >= (int) this.inventory.PackSize)
      {
        if (this.currentSlotID == this.equipInventory.LeftSideIndex)
          this.currentSlotID = this.equipInventory.BodyIndex;
        else if (this.currentSlotID == this.equipInventory.RightSideIndex)
        {
          this.currentSlotID = this.equipInventory.LeftSideIndex;
        }
        else
        {
          if (this.currentSlotID != this.equipInventory.HeadIndex && this.currentSlotID != this.equipInventory.NeckIndex && (this.currentSlotID != this.equipInventory.BodyIndex && this.currentSlotID != this.equipInventory.LegsIndex) && this.currentSlotID != this.equipInventory.FeetIndex)
            return;
          this.currentSlotID = this.equipInventory.RightSideIndex;
        }
      }
      else
        base.MoveLeftCore();
    }

    protected override void MoveRightCore()
    {
      if (this.currentSlotID >= (int) this.inventory.PackSize)
      {
        if (this.currentSlotID == this.equipInventory.RightSideIndex)
          this.currentSlotID = this.equipInventory.BodyIndex;
        else if (this.currentSlotID == this.equipInventory.LeftSideIndex)
        {
          this.currentSlotID = this.equipInventory.RightSideIndex;
        }
        else
        {
          if (this.currentSlotID != this.equipInventory.HeadIndex && this.currentSlotID != this.equipInventory.NeckIndex && (this.currentSlotID != this.equipInventory.BodyIndex && this.currentSlotID != this.equipInventory.LegsIndex) && this.currentSlotID != this.equipInventory.FeetIndex)
            return;
          this.currentSlotID = this.equipInventory.LeftSideIndex;
        }
      }
      else
        base.MoveRightCore();
    }

    protected override void MoveUpCore()
    {
      if (this.currentSlotID > (int) this.inventory.PackSize - 4)
        this.MoveUpEquipBox();
      else if (this.currentSlotID < (int) this.inventory.PackSize - 10)
        this.currentSlotID += 10;
      else
        this.currentSlotID -= (int) this.inventory.PackSize - 10;
    }

    private void MoveUpEquipBox()
    {
      if (this.currentSlotID == (int) this.inventory.PackSize - 3)
        this.currentSlotID = this.equipInventory.RightSideIndex;
      else if (this.currentSlotID == (int) this.inventory.PackSize - 2)
        this.currentSlotID = this.equipInventory.FeetIndex;
      else if (this.currentSlotID == (int) this.inventory.PackSize - 1)
        this.currentSlotID = this.equipInventory.LeftSideIndex;
      else if (this.currentSlotID == this.equipInventory.FeetIndex)
        this.currentSlotID = this.equipInventory.LegsIndex;
      else if (this.currentSlotID == this.equipInventory.LegsIndex)
        this.currentSlotID = this.equipInventory.BodyIndex;
      else if (this.currentSlotID == this.equipInventory.BodyIndex || this.currentSlotID == this.equipInventory.RightSideIndex || this.currentSlotID == this.equipInventory.LeftSideIndex)
        this.currentSlotID = this.equipInventory.NeckIndex;
      else if (this.currentSlotID == this.equipInventory.NeckIndex)
      {
        this.currentSlotID = this.equipInventory.HeadIndex;
      }
      else
      {
        if (this.currentSlotID != this.equipInventory.HeadIndex)
          return;
        this.currentSlotID = 8;
      }
    }

    protected override void MoveDownCore()
    {
      if (this.currentSlotID >= (int) this.inventory.PackSize)
        this.MoveDownEquipBox();
      else if (this.currentSlotID > 9)
      {
        this.currentSlotID -= 10;
      }
      else
      {
        this.currentSlotID += (int) this.inventory.PackSize - 10;
        if (this.currentSlotID == (int) this.inventory.PackSize - 2)
          this.currentSlotID = this.equipInventory.HeadIndex;
        else if (this.currentSlotID == (int) this.inventory.PackSize - 3)
        {
          this.currentSlotID = this.equipInventory.RightSideIndex;
        }
        else
        {
          if (this.currentSlotID != (int) this.inventory.PackSize - 1)
            return;
          this.currentSlotID = this.equipInventory.LeftSideIndex;
        }
      }
    }

    private void MoveDownEquipBox()
    {
      if (this.currentSlotID == this.equipInventory.HeadIndex)
        this.currentSlotID = this.equipInventory.NeckIndex;
      else if (this.currentSlotID == this.equipInventory.NeckIndex)
        this.currentSlotID = this.equipInventory.BodyIndex;
      else if (this.currentSlotID == this.equipInventory.BodyIndex || this.currentSlotID == this.equipInventory.RightSideIndex || this.currentSlotID == this.equipInventory.LeftSideIndex)
        this.currentSlotID = this.equipInventory.LegsIndex;
      else if (this.currentSlotID == this.equipInventory.LegsIndex)
      {
        this.currentSlotID = this.equipInventory.FeetIndex;
      }
      else
      {
        if (this.currentSlotID != this.equipInventory.FeetIndex)
          return;
        this.currentSlotID = (int) this.inventory.PackSize - 2;
      }
    }

    protected override bool IsUnliftValid
    {
      get
      {
        if (this.currentSlotID < (int) this.inventory.PackSize)
          return base.IsUnliftValid;
        return (EquipIndex) (this.currentSlotID - (int) this.inventory.PackSize) == Globals1.ItemTypeData[(int) this.liftedItem.ItemID].Equip - (byte) 1;
      }
    }

    protected override bool SetCurrentSlotCoWindow(Point pos)
    {
      for (int index = 0; index < this.boySlotRects.Length; ++index)
      {
        if (this.boySlotRects[index].Contains(pos))
        {
          this.currentSlotID = (int) this.inventory.EquipIndexStart + index;
          return true;
        }
      }
      return false;
    }

    protected override void DrawBorder(SpriteBatchSafe spriteBatch, Rectangle rect)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, this.Matrix);
      spriteBatch.DrawFilledBox(rect.Expand(this.borderWidth), this.borderWidth, this.borderColor, this.clientBackColor);
      spriteBatch.End();
    }

    protected override void DrawCoWindow()
    {
      this.DrawEquipPanel();
      this.DrawStatsPanel();
    }

    protected override void DrawBaseLine()
    {
      base.DrawBaseLine();
      Rectangle screenRect = this.screenRect;
      screenRect.X += this.screenRect.Width - 94;
      screenRect.Y += this.screenRect.Height - 25;
      screenRect.Width = screenRect.Height = 24;
      if (this.player.UserControls.CraftScreenFromInventoryScreen)
      {
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.OpenCrafting, screenRect);
        this.spriteBatch.DrawString(this.Font, "Craft", new Vector2((float) (screenRect.X + 31), (float) (screenRect.Y + 5)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        screenRect.X -= 100;
      }
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.EquipItemLeft, screenRect);
      this.spriteBatch.DrawString(this.Font, "Equip", new Vector2((float) (screenRect.X + 31), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    private void DrawEquipPanel()
    {
      this.spriteBatch.DrawFilledBox(this.equipScreenRect.Expand(8), 8, new Color(0.1f, 0.1f, 0.1f), new Color(0.2f, 0.2f, 0.2f) * 0.9f);
      Rectangle boySlotRect1 = this.boySlotRects[this.headIndex];
      Rectangle boySlotRect2 = this.boySlotRects[this.neckIndex];
      Rectangle boySlotRect3 = this.boySlotRects[this.bodyIndex];
      Rectangle boySlotRect4 = this.boySlotRects[this.legsIndex];
      Rectangle boySlotRect5 = this.boySlotRects[this.feetIndex];
      Rectangle boySlotRect6 = this.boySlotRects[this.leftIndex];
      Rectangle boySlotRect7 = this.boySlotRects[this.rightIndex];
      this.DrawSlot(boySlotRect1.X, boySlotRect1.Y);
      this.DrawSlot(boySlotRect2.X, boySlotRect2.Y);
      this.DrawSlot(boySlotRect7.X, boySlotRect7.Y);
      this.DrawSlot(boySlotRect3.X, boySlotRect3.Y);
      this.DrawSlot(boySlotRect6.X, boySlotRect6.Y);
      this.DrawSlot(boySlotRect4.X, boySlotRect4.Y);
      this.DrawSlot(boySlotRect5.X, boySlotRect5.Y);
      this.DrawEquipItem(boySlotRect1, this.equipInventory.HeadIndex);
      this.DrawEquipItem(boySlotRect2, this.equipInventory.NeckIndex);
      this.DrawEquipItem(boySlotRect7, this.equipInventory.RightSideIndex);
      this.DrawEquipItem(boySlotRect3, this.equipInventory.BodyIndex);
      this.DrawEquipItem(boySlotRect6, this.equipInventory.LeftSideIndex);
      this.DrawEquipItem(boySlotRect4, this.equipInventory.LegsIndex);
      this.DrawEquipItem(boySlotRect5, this.equipInventory.FeetIndex);
    }

    private void DrawEquipItem(Rectangle rect, int i)
    {
      InventoryItem inventoryItem = !this.IsItemLifted || i != this.currentSlotID ? this.inventory[i] : this.liftedItem;
      this.DrawItem(rect, i, inventoryItem, false, BaseInventoryScreen.SkillCompare.Equip);
    }

    private void DrawStatsPanel()
    {
      float scale = 0.6f;
      this.spriteBatch.DrawFilledBox(this.statsScreenRect.Expand(8), 8, new Color(0.1f, 0.1f, 0.1f), new Color(0.2f, 0.2f, 0.2f) * 0.9f);
      Vector2 vector2_1 = new Vector2((float) (this.statsScreenRect.X + 10), (float) (this.statsScreenRect.Y + 4));
      if (this.canViewOthersInventory)
      {
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.statsScreenRect.X, this.statsScreenRect.Y - 39, 280, 30), new Color(0.1f, 0.1f, 0.1f));
        this.spriteBatch.DrawString(this.Font, this.inventoryOwner != null ? this.inventoryOwner.DisplayGamertag : this.player.Gamertag, vector2_1 + TMFont.yVec - new Vector2(0.0f, 37f), Color.Yellow, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      }
      this.spriteBatch.DrawString(this.Font, "Stat Bonuses", vector2_1 + TMFont.yVec, Color.Yellow, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 24f;
      this.spriteBatch.DrawString(this.Font, "Health", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      this.spriteBatch.DrawString(this.Font, "Strength", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      this.spriteBatch.DrawString(this.Font, "Attack", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      this.spriteBatch.DrawString(this.Font, "Defense", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      this.spriteBatch.DrawString(this.Font, "Ranged", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      this.spriteBatch.DrawString(this.Font, "Looting", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num1 = 84;
      int num2 = 172;
      InventoryItem inventoryItem = this.IsItemLifted ? this.liftedItem : this.CursorItem;
      ItemCombatDataXML itemCombatDataXml = Globals1.ItemCombatData[(int) Globals1.ItemTypeData[(int) inventoryItem.ItemID].Combat];
      Color yellow = Color.Yellow;
      Color lawnGreen = Color.LawnGreen;
      Color orangeRed = Color.OrangeRed;
      vector2_1.X += 82f;
      vector2_1.Y = (float) (this.statsScreenRect.Y + 28);
      int health = (int) itemCombatDataXml.Health;
      string text1 = health.ToString();
      Vector2 vector2_2 = this.Font.MeasureString(text1);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text1, vector2_1 + TMFont.yVec + new Vector2((float) num1 - vector2_2.X, 0.0f), health >= 0 ? yellow : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num3 = this.inventoryOwner.HealthTotalItemBonus();
      string text2 = num3.ToString();
      vector2_2 = this.Font.MeasureString(text2);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text2, vector2_1 + TMFont.yVec + new Vector2((float) num2 - vector2_2.X, 0.0f), num3 >= 0 ? lawnGreen : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 24f;
      int strength = (int) itemCombatDataXml.Strength;
      string text3 = strength.ToString();
      vector2_2 = this.Font.MeasureString(text3);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text3, vector2_1 + TMFont.yVec + new Vector2((float) num1 - vector2_2.X, 0.0f), strength >= 0 ? yellow : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num4 = this.inventoryOwner.StrengthTotalItemBonus();
      string text4 = num4.ToString();
      vector2_2 = this.Font.MeasureString(text4);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text4, vector2_1 + TMFont.yVec + new Vector2((float) num2 - vector2_2.X, 0.0f), num4 >= 0 ? lawnGreen : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      int attack = (int) itemCombatDataXml.Attack;
      string text5 = attack.ToString();
      vector2_2 = this.Font.MeasureString(text5);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text5, vector2_1 + TMFont.yVec + new Vector2((float) num1 - vector2_2.X, 0.0f), attack >= 0 ? yellow : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num5 = this.inventoryOwner.AttackTotalItemBonus();
      string text6 = num5.ToString();
      vector2_2 = this.Font.MeasureString(text6);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text6, vector2_1 + TMFont.yVec + new Vector2((float) num2 - vector2_2.X, 0.0f), num5 >= 0 ? lawnGreen : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      int defence = (int) itemCombatDataXml.Defence;
      string text7 = defence.ToString();
      vector2_2 = this.Font.MeasureString(text7);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text7, vector2_1 + TMFont.yVec + new Vector2((float) num1 - vector2_2.X, 0.0f), defence >= 0 ? yellow : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num6 = this.inventoryOwner.DefenceTotalItemBonus();
      string text8 = num6.ToString();
      vector2_2 = this.Font.MeasureString(text8);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text8, vector2_1 + TMFont.yVec + new Vector2((float) num2 - vector2_2.X, 0.0f), num6 >= 0 ? lawnGreen : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      int ranged = (int) itemCombatDataXml.Ranged;
      string text9 = ranged.ToString();
      vector2_2 = this.Font.MeasureString(text9);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text9, vector2_1 + TMFont.yVec + new Vector2((float) num1 - vector2_2.X, 0.0f), ranged >= 0 ? yellow : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num7 = this.inventoryOwner.RangedTotalItemBonus();
      string text10 = num7.ToString();
      vector2_2 = this.Font.MeasureString(text10);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text10, vector2_1 + TMFont.yVec + new Vector2((float) num2 - vector2_2.X, 0.0f), num7 >= 0 ? lawnGreen : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      vector2_1.Y += 22f;
      int looting = (int) itemCombatDataXml.Looting;
      string text11 = looting.ToString();
      vector2_2 = this.Font.MeasureString(text11);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text11, vector2_1 + TMFont.yVec + new Vector2((float) num1 - vector2_2.X, 0.0f), looting >= 0 ? yellow : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      int num8 = this.inventoryOwner.LootingTotalItemBonus();
      string text12 = num8.ToString();
      vector2_2 = this.Font.MeasureString(text12);
      vector2_2.X *= scale;
      this.spriteBatch.DrawString(this.Font, text12, vector2_1 + TMFont.yVec + new Vector2((float) num2 - vector2_2.X, 0.0f), num8 >= 0 ? lawnGreen : orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
    }

    protected override Rectangle GetSlotRect(int slotID)
    {
      if (slotID >= (int) this.inventory.EquipIndexStart && slotID < (int) this.inventory.EquipIndexEnd)
        return this.boySlotRects[slotID - (int) this.inventory.PackSize];
      return base.GetSlotRect(slotID);
    }
  }
}
