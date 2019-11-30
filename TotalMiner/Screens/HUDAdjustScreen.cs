// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.HUDAdjustScreen
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
  internal class HUDAdjustScreen : GameScreen
  {
    private float x;
    private float y;
    private Rectangle save;

    public HUDAdjustScreen()
    {
      this.save = GraphicStatics.HUDPos();
      this.x = (float) this.save.X;
      this.y = (float) this.save.Y;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      Viewport viewport = this.GraphicsDevice.Viewport;
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        GraphicStatics.SetHUDPos(this.save.X, this.save.Y);
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.SelectItem))
      {
        Globals2.SaveGlobalData();
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.MsgBoxX))
      {
        this.x = (float) (viewport.Width / 10);
        this.y = (float) (viewport.Height / 10);
      }
      float num1 = 0.0f;
      float num2 = 0.0f;
      if (InputManager1.IsInputPressed(this.ControllingPlayer, PlayerInput.MoveLeft) || InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.CursorLeft))
        num1 = 0.5f;
      else if (InputManager1.IsInputPressed(this.ControllingPlayer, PlayerInput.MoveRight) || InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.CursorRight))
        num1 = -0.5f;
      if (InputManager1.IsInputPressed(this.ControllingPlayer, PlayerInput.MoveForward) || InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.CursorUp))
        num2 = -0.5f;
      else if (InputManager1.IsInputPressed(this.ControllingPlayer, PlayerInput.MoveBackward) || InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.CursorDown))
        num2 = 0.5f;
      if (InputManager.IsKeyPressed(this.ControllingPlayer.Value, Keys.LeftControl) || InputManager.IsKeyPressed(this.ControllingPlayer.Value, Keys.RightControl) || (InputManager.IsKeyPressed(this.ControllingPlayer.Value, Keys.LeftShift) || InputManager.IsKeyPressed(this.ControllingPlayer.Value, Keys.RightShift)))
      {
        num1 *= 5f;
        num2 *= 5f;
      }
      float num3 = num1 + currentGamePadState.ThumbSticks.Left.X;
      float num4 = num2 - currentGamePadState.ThumbSticks.Left.Y;
      float num5 = num3 + currentGamePadState.ThumbSticks.Right.X;
      float num6 = num4 - currentGamePadState.ThumbSticks.Right.Y;
      this.x += num5;
      this.y += num6;
      if ((double) this.x < 0.0)
        this.x = 0.0f;
      if ((double) this.y < 0.0)
        this.y = 0.0f;
      if ((double) this.x > (double) (viewport.Width - 22))
        this.x = (float) (viewport.Width - 22);
      if ((double) this.y > (double) (viewport.Height - 22))
        this.y = (float) (viewport.Height - 22);
      GraphicStatics.SetHUDPos((int) this.x, (int) this.y);
      return base.HandleInput(input);
    }

    protected override void DrawCore()
    {
      Viewport viewport = this.GraphicsDevice.Viewport;
      this.GraphicsDevice.Viewport = GraphicStatics.DefaultViewport;
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      spriteBatch.Begin();
      Rectangle rect = new Rectangle(viewport.Width / 2 - 105, viewport.Height / 2 - 76, 210, 151);
      spriteBatch.DrawFilledBox(rect, 2, Color.White, Color.Black * 0.5f);
      GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.SelectItem, new Rectangle(rect.X + 20, rect.Y + 20, 32, 32));
      GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.MsgBoxX, new Rectangle(rect.X + 20, rect.Y + 60, 32, 32));
      GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.SelectItem, new Rectangle(rect.X + 20, rect.Y + 20, 32, 32));
      GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.ExitScreen, new Rectangle(rect.X + 20, rect.Y + 100, 32, 32));
      spriteBatch.DrawString(CoreGlobals.GameFont, "Save", new Vector2((float) (rect.X + 64), (float) (rect.Y + 14)), Color.White);
      spriteBatch.DrawString(CoreGlobals.GameFont, "Reset", new Vector2((float) (rect.X + 64), (float) (rect.Y + 54)), Color.White);
      spriteBatch.DrawString(CoreGlobals.GameFont, "Cancel", new Vector2((float) (rect.X + 64), (float) (rect.Y + 94)), Color.White);
      Point point = new Point(128, 72);
      spriteBatch.DrawFilledBox(CoreGlobals.BlankTexture, new Rectangle(GraphicStatics.HUDPos().X, GraphicStatics.HUDPos().Y, point.X, point.Y), 2, Color.Black, Color.Gray);
      spriteBatch.DrawFilledBox(CoreGlobals.BlankTexture, new Rectangle(GraphicStatics.HUDPos().X, GraphicStatics.HUDPos().Height - point.Y, point.X, point.Y), 2, Color.Black, Color.Gray);
      spriteBatch.DrawFilledBox(CoreGlobals.BlankTexture, new Rectangle(GraphicStatics.HUDPos().Width - point.X, GraphicStatics.HUDPos().Y, point.X, point.Y), 2, Color.Black, Color.Gray);
      spriteBatch.DrawFilledBox(CoreGlobals.BlankTexture, new Rectangle(GraphicStatics.HUDPos().Width - point.X, GraphicStatics.HUDPos().Height - point.Y, point.X, point.Y), 2, Color.Black, Color.Gray);
      spriteBatch.End();
      this.GraphicsDevice.Viewport = viewport;
    }
  }
}
