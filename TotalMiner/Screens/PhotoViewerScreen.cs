// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PhotoViewerScreen
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
  internal class PhotoViewerScreen : GameScreen
  {
    private int photoID;
    private PhotoInfo info;
    private Texture2D photo;
    private bool isFullScreen;

    public PhotoViewerScreen(int photoID)
      : this(photoID, (Texture2D) null)
    {
    }

    public PhotoViewerScreen(int photoID, Texture2D photo)
    {
      this.photoID = photoID;
      this.photo = photo;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.info = GraphicStatics.PhotoData.ReadPhotoInfo(this.photoID);
      if (this.photo != null)
        return;
      this.photo = GraphicStatics.PhotoData.LoadPhoto(this.photoID, PhotoFileType.PhotoImage);
    }

    public override bool HandleInput(InputState input)
    {
      if (this.ControllingPlayer.HasValue)
      {
        if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen) || InputManager.IsButtonReleasedNew(this.ControllingPlayer.Value, Buttons.A))
        {
          this.ExitScreen();
          return true;
        }
        if (InputManager.IsButtonPressedNew(this.ControllingPlayer.Value, Buttons.X) || InputManager1.IsInputReleasedNew(this.ControllingPlayer.Value, GuiInput.SelectItem))
        {
          this.isFullScreen = !this.isFullScreen;
          return true;
        }
      }
      return base.HandleInput(input);
    }

    protected override void DrawCore()
    {
      int num1 = 300;
      int num2 = this.photo != null ? this.photo.Width : 500;
      int num3 = this.photo != null ? this.photo.Height : 500;
      int rectWidth = num2 + num1;
      int rectHeight = num3;
      if (this.isFullScreen)
      {
        rectWidth = this.GraphicsDevice.Viewport.Width;
        rectHeight = rectWidth / num2 * num3;
      }
      Rectangle rectangle = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, rectWidth, rectHeight);
      if (!this.isFullScreen)
      {
        rectangle.Width -= num1;
        rectangle.X += 10;
        rectangle.Y -= 2;
      }
      this.ScreenManager.SpriteBatch.Begin();
      this.ScreenManager.SpriteBatch.Draw(CoreGlobals.BlankTexture, this.GraphicsDevice.Viewport.Rectangle(), Color.Black);
      if (!this.isFullScreen)
        this.ScreenManager.SpriteBatch.DrawBox(rectangle.Expand(4), 4, Color.White, 0.0f);
      if (this.photo != null)
        this.ScreenManager.SpriteBatch.Draw(this.photo, rectangle, Color.White);
      if (!this.isFullScreen)
      {
        rectangle.X += rectangle.Width + 4;
        rectangle.Width = num1;
        this.ScreenManager.SpriteBatch.DrawFilledBox(rectangle.Expand(4), 4, Color.White, Color.Black);
        this.ScreenManager.SpriteBatch.DrawString(this.Font, "Map Name:\n\n\nMap Owner:\n\n\nPhotographer:", new Vector2((float) (rectangle.X + 10), (float) (rectangle.Y + 10)), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
        this.ScreenManager.SpriteBatch.DrawString(this.Font, "\n" + this.info.MapName + "\n\n\n" + this.info.MapOwner + "\n\n\n" + this.info.Photographer, new Vector2((float) (rectangle.X + 10), (float) (rectangle.Y + 10)), Color.Yellow, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
        this.ScreenManager.SpriteBatch.Draw(CoreGlobals.ButtonTextureX, new Vector2((float) (rectangle.X + 10), (float) (rectangle.Y + rectangle.Height - 40)), new Rectangle?(), Color.White, 0.0f, Vector2.Zero, new Vector2(0.4f), SpriteEffects.None, 0.0f);
        this.ScreenManager.SpriteBatch.DrawString(this.Font, "Fullscreen", new Vector2((float) (rectangle.X + 50), (float) (rectangle.Y + rectangle.Height - 38)), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      }
      this.ScreenManager.SpriteBatch.End();
    }
  }
}
