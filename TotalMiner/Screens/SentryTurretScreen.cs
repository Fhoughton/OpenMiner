// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SentryTurretScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Blocks;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class SentryTurretScreen : ChestScreen
  {
    private Texture2D checkboxOn;
    private Texture2D checkboxOff;
    private SentryTurretBlock block;
    private string targetChoice;

    protected override int CoWindowHeight
    {
      get
      {
        return 302;
      }
    }

    private bool IsOwner
    {
      get
      {
        if (!this.chest.IsOwner(this.player))
          return this.player.IsGod;
        return true;
      }
    }

    public SentryTurretScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base(instance, player, p, Block.SentryTurret)
    {
      this.block = this.chest as SentryTurretBlock;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.checkboxOn = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkboxOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
      this.SetTargetChoiceString();
      this.WinRectClickPress = true;
      Rectangle r = new Rectangle(this.screenRect.X + 17, this.screenRect.Y + 38, 230, 30);
      this.AddWinRect(r, new EventHandler<EventArgs>(this.ClickTargetChoice));
      r.Y += 31;
      this.AddWinRect(r, new EventHandler<EventArgs>(this.ClickTargetOwner));
      r.Y += 31;
      this.AddWinRect(r, new EventHandler<EventArgs>(this.ClickTargetAdmins));
      r.Y += 31;
      this.AddWinRect(r, new EventHandler<EventArgs>(this.ClickTargetPlayers));
      r.Y += 31;
      this.AddWinRect(r, new EventHandler<EventArgs>(this.ClickTargetMobs));
      r.Y += 31;
      this.AddWinRect(r, new EventHandler<EventArgs>(this.ClickRequiresPower));
    }

    private void ClickTargetChoice(object sender, EventArgs e)
    {
      this.currentSlotID = -10;
      this.LiftAllButtonPressed();
    }

    private void ClickTargetOwner(object sender, EventArgs e)
    {
      this.currentSlotID = -20;
      this.LiftAllButtonPressed();
    }

    private void ClickTargetAdmins(object sender, EventArgs e)
    {
      this.currentSlotID = -30;
      this.LiftAllButtonPressed();
    }

    private void ClickTargetPlayers(object sender, EventArgs e)
    {
      this.currentSlotID = -40;
      this.LiftAllButtonPressed();
    }

    private void ClickTargetMobs(object sender, EventArgs e)
    {
      this.currentSlotID = -50;
      this.LiftAllButtonPressed();
    }

    private void ClickRequiresPower(object sender, EventArgs e)
    {
      this.currentSlotID = -60;
      this.LiftAllButtonPressed();
    }

    private void SetTargetChoiceString()
    {
      this.targetChoice = "Target: ";
      if (this.block.IsTargeting(BlockTargetTypes.Strongest))
        this.targetChoice += "Strongest";
      else if (this.block.IsTargeting(BlockTargetTypes.Weakest))
        this.targetChoice += "Weakest";
      else
        this.targetChoice += "Closest";
    }

    protected override void MoveLeft()
    {
      if (this.currentSlotID < 0)
        return;
      base.MoveLeft();
    }

    protected override void MoveRight()
    {
      if (this.currentSlotID < 0)
        return;
      base.MoveRight();
    }

    protected override void MoveUpCore()
    {
      int num = 30 + (this.pagesize - 10);
      if (this.currentSlotID < num)
        this.currentSlotID += 10;
      else if (this.IsOwner)
        this.currentSlotID = -60;
      else
        this.currentSlotID -= num;
    }

    protected override void MoveDownCore()
    {
      if (this.currentSlotID < 10 && this.IsOwner)
      {
        if (this.currentSlotID >= 0)
          this.currentSlotID = -10;
        else
          this.currentSlotID -= 10;
        if (this.currentSlotID != -70)
          return;
        this.currentSlotID = 30;
      }
      else
      {
        int num = 30 + (this.pagesize - 10);
        if (this.currentSlotID < 30)
          this.currentSlotID += num;
        else
          this.currentSlotID -= 10;
      }
    }

    protected override void LiftAllButtonPressed()
    {
      if (this.currentSlotID < 0)
      {
        if (this.currentSlotID == -10)
        {
          this.block.ToggleTargetChoice();
          this.SetTargetChoiceString();
        }
        else if (this.currentSlotID == -20)
          this.block.ToggleTargetType(BlockTargetTypes.Owner);
        else if (this.currentSlotID == -30)
          this.block.ToggleTargetType(BlockTargetTypes.Admins);
        else if (this.currentSlotID == -40)
          this.block.ToggleTargetType(BlockTargetTypes.Players);
        else if (this.currentSlotID == -50)
        {
          this.block.ToggleTargetType(BlockTargetTypes.Mobs);
        }
        else
        {
          if (this.currentSlotID != -60)
            return;
          this.block.RequiresPower = !this.block.RequiresPower;
        }
      }
      else
        base.LiftAllButtonPressed();
    }

    protected override void CheckForClear()
    {
      if (this.instance.Map.GetBlockID(this.chest.Point) == (byte) 142)
        return;
      this.ExitScreen();
    }

    protected override void OnScreenClosed()
    {
      base.OnScreenClosed();
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.chest, false);
    }

    protected override bool CanEditInventory
    {
      get
      {
        return true;
      }
    }

    protected override void DrawCoWindow()
    {
      base.DrawCoWindow();
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.screenRect.X, this.screenRect.Y + 32, this.screenRect.Width, 1), Color.Gray);
      bool flag1 = this.block.IsTargeting(BlockTargetTypes.Owner);
      bool flag2 = this.block.IsTargeting(BlockTargetTypes.Players);
      bool flag3 = this.block.IsTargeting(BlockTargetTypes.Mobs);
      bool flag4 = this.block.IsTargeting(BlockTargetTypes.Admins);
      this.block.IsTargeting(BlockTargetTypes.Strongest);
      this.block.IsTargeting(BlockTargetTypes.Weakest);
      Rectangle rectangle = new Rectangle(this.screenRect.X + 24, this.screenRect.Y + 40, 24, 24);
      rectangle.Y += 31;
      this.spriteBatch.Draw(flag1 ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      rectangle.Y += 31;
      this.spriteBatch.Draw(flag4 ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      rectangle.Y += 31;
      this.spriteBatch.Draw(flag2 ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      rectangle.Y += 31;
      this.spriteBatch.Draw(flag3 ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      rectangle.Y += 31;
      this.spriteBatch.Draw(this.block.RequiresPower ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      Vector2 vector2 = new Vector2((float) (this.screenRect.X + 24), (float) (this.screenRect.Y + 44));
      this.spriteBatch.DrawString(CoreGlobals.GameFont, this.targetChoice, vector2 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      vector2.X += 36f;
      vector2.Y += 31f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "Owner", vector2 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      vector2.Y += 31f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "Admins", vector2 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      vector2.Y += 31f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "Players", vector2 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      vector2.Y += 31f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "Enemies", vector2 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      vector2.Y += 31f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "Requires Power", vector2 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (this.currentSlotID >= 0)
        return;
      rectangle = new Rectangle(this.screenRect.X + 16, this.screenRect.Y + 36, 232, 34);
      rectangle.Y += (this.currentSlotID + 10) / -10 * 31;
      this.spriteBatch.DrawBox(rectangle, 2, Color.Yellow, 0.0f);
    }
  }
}
