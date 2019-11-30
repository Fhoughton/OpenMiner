// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CraftSelectBlueprintScreen
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
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class CraftSelectBlueprintScreen : MinerToolScreen
  {
    private ItemType currentType = ItemType.Block;
    private int rowHeight;
    private int rowsPerPage;
    private int scrollOffset;
    private float current;
    private float repeatTimer;
    private string buttonText;
    private Texture2D gridTexture;
    private Texture2D lockedTexture;
    private BaseInventoryScreen inventoryScreen;
    private string[][][] bpDescText;
    private Blueprint[][] sortedBlueprints;
    private SpriteBatchSafe spriteBatch1;
    private SpriteBatchSafe spriteBatch2;
    private SpriteBatchSafe spriteBatchItem;
    private Rectangle[] tabRects;
    private BlueprintCraftType craftType;
    private Color colorWhite;
    private Color colorYellow;
    private Color colorGray;

    public CraftSelectBlueprintScreen(
      BaseInventoryScreen inventoryScreen,
      BlueprintCraftType craftType)
      : base(inventoryScreen.Player)
    {
      this.inventoryScreen = inventoryScreen;
      this.craftType = craftType;
    }

    public override void LoadContent()
    {
      this.Font = CoreGlobals.GameFont;
      this.rowHeight = 96;
      this.rowsPerPage = 5;
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 936, 570);
      base.LoadContent();
      int length = 9;
      this.bpDescText = new string[length][][];
      this.sortedBlueprints = new Blueprint[length][];
      for (int type = 0; type < length; ++type)
        this.AddSortedBlueprints(type);
      this.gridTexture = this.content.Load<Texture2D>("Textures\\grid");
      this.lockedTexture = this.content.Load<Texture2D>("Textures\\locked");
      this.buttonText = "Select Blueprint.    Use the Left Stick or DPAD to scroll.";
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.spriteBatch1 = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatch2 = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchItem = GraphicStatics.SpriteBatchPool.GetNextItem();
      Vector2 vector2 = new Vector2((float) (this.screenRect.X + 8), (float) (this.screenRect.Y + 6));
      this.tabRects = new Rectangle[9]
      {
        new Rectangle((int) vector2.X, (int) vector2.Y, 100, 32),
        new Rectangle((int) vector2.X + 100, (int) vector2.Y, 92, 32),
        new Rectangle((int) vector2.X + 192, (int) vector2.Y, 92, 32),
        new Rectangle((int) vector2.X + 284, (int) vector2.Y, 128, 32),
        new Rectangle((int) vector2.X + 412, (int) vector2.Y, 100, 32),
        new Rectangle((int) vector2.X + 512, (int) vector2.Y, 102, 32),
        new Rectangle((int) vector2.X + 614, (int) vector2.Y, 88, 32),
        new Rectangle((int) vector2.X + 702, (int) vector2.Y, 92, 32),
        new Rectangle((int) vector2.X + 794, (int) vector2.Y, 112, 32)
      };
      this.AddWinRect(this.tabRects[0], new EventHandler<EventArgs>(this.ClickTab0));
      this.AddWinRect(this.tabRects[1], new EventHandler<EventArgs>(this.ClickTab1));
      this.AddWinRect(this.tabRects[2], new EventHandler<EventArgs>(this.ClickTab2));
      this.AddWinRect(this.tabRects[3], new EventHandler<EventArgs>(this.ClickTab3));
      this.AddWinRect(this.tabRects[4], new EventHandler<EventArgs>(this.ClickTab4));
      this.AddWinRect(this.tabRects[5], new EventHandler<EventArgs>(this.ClickTab5));
      this.AddWinRect(this.tabRects[6], new EventHandler<EventArgs>(this.ClickTab6));
      this.AddWinRect(this.tabRects[7], new EventHandler<EventArgs>(this.ClickTab7));
      this.AddWinRect(this.tabRects[8], new EventHandler<EventArgs>(this.ClickTab8));
    }

    private void ClickTab0(object sender, EventArgs e)
    {
      this.SetTab(0);
    }

    private void ClickTab1(object sender, EventArgs e)
    {
      this.SetTab(1);
    }

    private void ClickTab2(object sender, EventArgs e)
    {
      this.SetTab(2);
    }

    private void ClickTab3(object sender, EventArgs e)
    {
      this.SetTab(3);
    }

    private void ClickTab4(object sender, EventArgs e)
    {
      this.SetTab(4);
    }

    private void ClickTab5(object sender, EventArgs e)
    {
      this.SetTab(5);
    }

    private void ClickTab6(object sender, EventArgs e)
    {
      this.SetTab(6);
    }

    private void ClickTab7(object sender, EventArgs e)
    {
      this.SetTab(7);
    }

    private void ClickTab8(object sender, EventArgs e)
    {
      this.SetTab(8);
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch1);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch2);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchItem);
    }

    private void AddSortedBlueprints(int type)
    {
      ItemType itemType = (ItemType) (type + 1);
      int length = 0;
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (blueprint.IsValid && blueprint.CraftType == this.craftType && (ItemData.IsEnabled(blueprint.Result.ItemID) && ItemData.GetItemType(blueprint.Result.ItemID) == itemType))
          ++length;
      }
      this.sortedBlueprints[type] = new Blueprint[length];
      if (this.bpDescText[type] == null)
        this.bpDescText[type] = new string[length][];
      int num = 0;
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (blueprint.IsValid && blueprint.CraftType == this.craftType && (ItemData.IsEnabled(blueprint.Result.ItemID) && ItemData.GetItemType(blueprint.Result.ItemID) == itemType))
          this.sortedBlueprints[type][num++] = blueprint;
      }
      Array.Sort<Blueprint>(this.sortedBlueprints[type], new Comparison<Blueprint>(this.SortBlueprintsByXP));
    }

    private int SortBlueprintsByID(Blueprint x, Blueprint y)
    {
      return x.SortID.CompareTo(y.SortID);
    }

    private int SortBlueprintsByXP(Blueprint x, Blueprint y)
    {
      SkillDataXML skillDataXml1 = Globals1.SkillData[(int) x.Result.ItemID];
      SkillDataXML skillDataXml2 = Globals1.SkillData[(int) y.Result.ItemID];
      int craftReq1 = skillDataXml1.CraftReq;
      int craftReq2 = skillDataXml2.CraftReq;
      if (craftReq1 != craftReq2)
        return craftReq1.CompareTo(craftReq2);
      float craftExp1 = skillDataXml1.CraftExp;
      float craftExp2 = skillDataXml2.CraftExp;
      if ((double) craftExp1 != (double) craftExp2)
        return craftExp1.CompareTo(craftExp2);
      return x.SortID.CompareTo(y.SortID);
    }

    public override bool HandleInput(InputState input)
    {
      if (base.HandleInput(input))
        return true;
      return this.HandleInputCore(input);
    }

    private bool HandleInputCore(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.SelectItem))
      {
        Blueprint blueprint = this.sortedBlueprints[(int) (this.currentType - (byte) 1)][(int) this.current];
        if (this.IsBlueprintEnabled(blueprint))
        {
          this.inventoryScreen.BlueprintSelected(blueprint);
          this.ExitScreen();
        }
        else
          CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PrevTab))
      {
        this.SelectTabLeft();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.NextTab))
      {
        this.SelectTabRight();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp))
      {
        this.ScrollUpPage();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown))
      {
        this.ScrollDownPage();
        return true;
      }
      Vector2 left = currentGamePadState.ThumbSticks.Left;
      bool flag1 = (double) left.Y > 0.0;
      bool flag2 = (double) left.Y < 0.0;
      if (InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.CursorUp))
      {
        flag1 = true;
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorUp))
          this.repeatTimer = 0.0f;
      }
      else if (InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.CursorDown))
      {
        flag2 = true;
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorDown))
          this.repeatTimer = 0.0f;
      }
      if ((double) left.Y != 0.0 && Math.Sign(left.Y) == Math.Sign(lastGamePadState.ThumbSticks.Left.Y) || (flag1 || flag2))
      {
        this.repeatTimer -= Services.ElapsedTime;
        if ((double) this.repeatTimer > 0.0)
          return true;
      }
      int mouseWheelDelta = InputManager.GetMouseWheelDelta(this.ControllingPlayer.Value);
      if (mouseWheelDelta > 0)
        flag1 = true;
      else if (mouseWheelDelta < 0)
        flag2 = true;
      if (flag1 || flag2)
        this.repeatTimer = 0.2f;
      if (flag2)
      {
        int index = (int) (this.currentType - (byte) 1);
        if (mouseWheelDelta == 0)
        {
          if ((double) ++this.current >= (double) this.sortedBlueprints[index].Length)
          {
            this.current = 0.0f;
            this.scrollOffset = 0;
          }
        }
        else
        {
          if ((double) this.current == (double) (this.sortedBlueprints[index].Length - 1))
            return true;
          ++this.current;
        }
        if ((double) this.scrollOffset < (double) this.current - (double) (this.rowsPerPage - 1))
          this.scrollOffset = (int) this.current - (this.rowsPerPage - 1);
        if (this.scrollOffset < 0)
          this.scrollOffset = 0;
        Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
        return true;
      }
      if (flag1)
      {
        if (mouseWheelDelta != 0 && (double) this.current == 0.0)
          return true;
        --this.current;
        if ((double) this.current < 0.0)
        {
          this.current = (float) (this.sortedBlueprints[(int) (this.currentType - (byte) 1)].Length - 1);
          this.scrollOffset = (int) this.current - (this.rowsPerPage - 1);
        }
        if ((double) this.scrollOffset > (double) this.current)
          this.scrollOffset = (int) this.current;
        if (this.scrollOffset < 0)
          this.scrollOffset = 0;
        Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
        return true;
      }
      if (InputManager.IsMouseMoved(this.ControllingPlayer.Value))
      {
        Point mousePos = InputManager.GetMousePos(this.ControllingPlayer.Value);
        mousePos.X -= this.screenRect.X + 8;
        mousePos.Y -= this.screenRect.Y + 48;
        if (mousePos.X >= 0 && mousePos.Y >= 0 && (mousePos.X < this.screenRect.Width - 16 && mousePos.Y < this.rowHeight * this.rowsPerPage))
          this.current = (float) (mousePos.Y / this.rowHeight + this.scrollOffset);
      }
      return false;
    }

    private void SetTab(int tabID)
    {
      int num = (int) (this.currentType - (byte) 1);
      if (tabID == num)
        return;
      this.currentType = (ItemType) (tabID + 1);
      this.current = 0.0f;
      this.scrollOffset = 0;
    }

    private void SelectTabLeft()
    {
      int index = (int) (this.currentType - (byte) 1);
      if (index < 0 || index >= this.sortedBlueprints.Length)
        return;
      Blueprint[] sortedBlueprint = this.sortedBlueprints[index];
      int num = (int) (this.currentType - (byte) 1);
      if (num == 0)
        num = 9;
      this.currentType = (ItemType) num;
      this.current = 0.0f;
      this.scrollOffset = 0;
    }

    private void SelectTabRight()
    {
      int index = (int) (this.currentType - (byte) 1);
      if (index < 0 || index >= this.sortedBlueprints.Length)
        return;
      Blueprint[] sortedBlueprint = this.sortedBlueprints[index];
      int num = (int) (this.currentType + (byte) 1);
      if ((byte) num == (byte) 10)
        num = 1;
      this.currentType = (ItemType) num;
      this.current = 0.0f;
      this.scrollOffset = 0;
    }

    private void ScrollUpPage()
    {
      if ((double) this.current > 0.0)
      {
        this.current -= (float) (this.rowsPerPage - 1);
        if ((double) this.current < 0.0)
          this.current = 0.0f;
      }
      if ((double) this.scrollOffset > (double) this.current)
        this.scrollOffset = (int) this.current;
      if (this.scrollOffset < 0)
        this.scrollOffset = 0;
      Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
    }

    private void ScrollDownPage()
    {
      if ((double) this.current < (double) (this.sortedBlueprints[(int) (this.currentType - (byte) 1)].Length - 1))
      {
        this.current += (float) (this.rowsPerPage - 1);
        if ((double) this.current >= (double) this.sortedBlueprints[(int) (this.currentType - (byte) 1)].Length)
          this.current = (float) (this.sortedBlueprints[(int) (this.currentType - (byte) 1)].Length - 1);
      }
      if ((double) this.scrollOffset < (double) this.current - (double) (this.rowsPerPage - 1))
        this.scrollOffset = (int) this.current - (this.rowsPerPage - 1);
      if (this.scrollOffset < 0)
        this.scrollOffset = 0;
      Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
    }

    private void ValidateCursorPosition(int oldMax)
    {
      int index = (int) (this.currentType - (byte) 1);
      if (index < 0 || index >= this.sortedBlueprints.Length || (double) this.current < (double) this.sortedBlueprints[index].Length)
        return;
      int num = oldMax - this.sortedBlueprints[index].Length;
      if (num > 0)
      {
        this.current -= (float) num;
        this.scrollOffset -= num;
      }
      if ((double) this.current < 0.0)
        this.current = 0.0f;
      if (this.scrollOffset >= 0)
        return;
      this.scrollOffset = 0;
    }

    private bool IsBlueprintEnabled(Blueprint bp)
    {
      if (!bp.IsValid || !bp.IsEnabled)
        return this.player.IsGod;
      return true;
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      float num1 = (float) this.TransitionAlpha / (float) byte.MaxValue;
      this.colorWhite = Color.White * num1;
      this.colorYellow = Color.Yellow * num1;
      this.colorGray = Color.Gray * num1 * 0.5f;
      Color color1 = new Color(0.4f, 0.4f, 0.4f) * num1;
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(this.Matrix);
      Vector2 pos = new Vector2((float) (this.screenRect.X + 8), (float) (this.screenRect.Y + 12));
      this.spriteBatch.DrawFilledBox(this.tabRects[(int) (this.currentType - (byte) 1)], 1, Color.White, Color.Yellow * 0.2f);
      this.spriteBatch.DrawString(this.Font, "  Blocks    Items    Tools    Weapons    Armor    Power    Food    Decor   Jewelry", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
      int x = this.screenRect.X + 8;
      int num2 = this.screenRect.Y + this.screenRect.Height - 36;
      this.spriteBatch.Draw(CoreGlobals.ButtonTextureA, new Rectangle(x, num2 + 6, 24, 24), this.colorWhite);
      this.spriteBatch.DrawString(this.Font, this.buttonText, new Vector2((float) (x + 35), (float) (num2 + 12)) + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
      this.spriteBatch.End();
      pos.Y += 34f;
      this.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, this.Matrix);
      this.spriteBatch1.BeginTM(this.Matrix);
      this.spriteBatch2.BeginTM(this.Matrix);
      this.spriteBatchItem.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, this.Matrix);
      this.spriteBatch1.Draw(this.ScreenManager.BlankTexture, new Rectangle(this.screenRect.X + 8, this.screenRect.Y + 48, this.screenRect.Width - 16, 1), this.colorGray);
      pos = new Vector2((float) (this.screenRect.X + 8), 0.0f);
      Color color2 = new Color(0.4f, 0.4f, 0.4f) * num1;
      Rectangle rectangle1 = new Rectangle(272, 16, 16, 16);
      Rectangle rectangle2 = new Rectangle((int) pos.X + 20, (int) pos.Y + 36, 24, 24);
      Rectangle rect = new Rectangle((int) pos.X + 23, (int) pos.Y + 39, 44, 44);
      for (int scrollOffset = this.scrollOffset; scrollOffset < this.scrollOffset + this.rowsPerPage; ++scrollOffset)
      {
        int index = (int) (this.currentType - (byte) 1);
        if (index >= 0 && index < this.sortedBlueprints.Length && this.sortedBlueprints[index].Length > scrollOffset)
        {
          Blueprint blueprint = this.sortedBlueprints[index][scrollOffset];
          Item itemId = blueprint.Result.ItemID;
          pos.Y = (float) (this.screenRect.Y + 48 + (scrollOffset - this.scrollOffset) * this.rowHeight);
          bool flag = this.IsBlueprintEnabled(blueprint);
          Color color3 = flag ? this.colorWhite : color1;
          if (flag)
          {
            this.spriteBatch1.DrawString(this.Font, ItemData.ToString(itemId), pos + new Vector2(16f, 11f) + TMFont.yVec, color3, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
            rectangle2.Y = (int) pos.Y + 32;
            rect.Y = rectangle2.Y + 3;
            GraphicStatics.DrawItem(this.spriteBatch2, this.spriteBatchItem, this.spriteBatchItem, rectangle2.X, rectangle2.Y, blueprint.Result, false);
            this.spriteBatch2.DrawBox(rect, 2, color3, 0.0f);
          }
          else
            this.spriteBatch1.DrawString(this.Font, "Locked Item", pos + new Vector2(16f, 40f) + TMFont.yVec, color3, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
          this.DrawBlueprintGrid(blueprint, pos);
          this.spriteBatch1.Draw(this.ScreenManager.BlankTexture, new Rectangle(this.screenRect.X + 8, (int) ((double) pos.Y + (double) this.rowHeight), this.screenRect.Width - 16, 1), this.colorGray);
          if (flag)
          {
            bool isSkillsEnabled = this.inventoryScreen.Player.GameInstance.IsSkillsEnabled;
            float scale = isSkillsEnabled ? 0.5f : 0.6f;
            SkillDataXML skillDataXml = Globals1.SkillData[(int) itemId];
            if (this.bpDescText[index][scrollOffset] == null)
            {
              string description = blueprint.Description;
              if (isSkillsEnabled)
                description += string.Format("\nSkill: {0} ({1}). XP Earned: {2}", (object) skillDataXml.CraftSkill, (object) skillDataXml.CraftReq, (object) skillDataXml.CraftExp);
              this.bpDescText[index][scrollOffset] = Utils.BreakIntoLines(this.Font, 584, scale, description, true);
            }
            Vector2 vector2 = pos + new Vector2(310f, (float) (20.0 + (double) (3 - this.bpDescText[index][scrollOffset].Length) * 10.0));
            foreach (string text in this.bpDescText[index][scrollOffset])
            {
              this.spriteBatch1.DrawString(this.Font, text, vector2 + TMFont.yVec, color3, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
              vector2.Y += 20f;
            }
            if (isSkillsEnabled && this.player.SkillsData.GetCraftSkill(blueprint.Result.ItemID).Level < skillDataXml.CraftReq)
              this.spriteBatch1.Draw(CoreGlobals.BlankTexture, new Rectangle(this.screenRect.X + 13, (int) pos.Y + 5, this.screenRect.Width - 26, 86), Color.Red * 0.1f);
          }
          else
            this.spriteBatch1.DrawString(this.Font, "Find the blueprint to unlock this item.", pos + new Vector2(310f, 40f) + TMFont.yVec, color3, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
          if ((double) scrollOffset == (double) this.current)
          {
            Rectangle rectangle3 = new Rectangle(this.screenRect.X + 8, (int) pos.Y, this.screenRect.Width - 16, 96);
            this.spriteBatch1.Draw(CoreGlobals.BlankTexture, rectangle3, this.colorYellow * 0.1f);
            this.spriteBatch1.DrawBox(CoreGlobals.BlankTexture, rectangle3, 5, this.colorYellow * 0.5f, 0.0f);
          }
          rectangle2.Y += this.rowHeight;
          rect.Y += this.rowHeight;
        }
      }
      this.spriteBatch.End();
      this.spriteBatch1.End();
      this.spriteBatch2.End();
      this.spriteBatchItem.End();
      CoreGlobals.FrameRateCounter.SpriteCalls += 2;
    }

    private void DrawBlueprintGrid(Blueprint blueprint, Vector2 pos)
    {
      Rectangle destinationRectangle1 = new Rectangle((int) pos.X + 210, (int) ((double) pos.Y + 17.0), this.gridTexture.Width, this.gridTexture.Height);
      if (this.IsBlueprintEnabled(blueprint))
      {
        this.spriteBatch1.Draw(this.gridTexture, destinationRectangle1, this.colorWhite);
        InventoryItem[] items = blueprint.Items;
        Rectangle destinationRectangle2 = new Rectangle(0, 0, 16, 16);
        Rectangle rectangle = new Rectangle(0, 0, 16, 16);
        for (int index = 0; index < 9; ++index)
        {
          Item itemId = items[index].ItemID;
          if (itemId != Item.None)
          {
            int num1 = index % 3;
            int num2 = index / 3;
            destinationRectangle2.X = destinationRectangle1.X + 3 + index % 3 * 20;
            destinationRectangle2.Y = destinationRectangle1.Y + 3 + (2 - num2) * 20;
            rectangle = GraphicStatics.TexturePack.ItemSrcRect(itemId);
            this.spriteBatch2.Draw(GraphicStatics.TexturePack.GetTexureForItem(itemId), destinationRectangle2, new Rectangle?(rectangle), this.colorWhite);
          }
        }
      }
      else
      {
        destinationRectangle1.X += 9;
        destinationRectangle1.Y += 4;
        destinationRectangle1.Width = this.lockedTexture.Width * 2;
        destinationRectangle1.Height = this.lockedTexture.Height * 2;
        this.spriteBatch.Draw(this.lockedTexture, destinationRectangle1, this.colorWhite);
      }
    }
  }
}
