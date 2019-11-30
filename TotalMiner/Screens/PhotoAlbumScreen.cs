// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PhotoAlbumScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class PhotoAlbumScreen : GameScreen
  {
    private int pagesize = 6;
    private int selected;
    private int currentAttempt;
    private bool inputCleared;
    private List<PhotoTag> photos;
    private Action<int> photoSelected;
    private PhotoLoader loader;
    private bool isPhotosLoading;
    private bool runOnFinished;

    private int currentPhotoCount
    {
      get
      {
        lock (this.photos)
          return this.photos.Count;
      }
    }

    private Texture2D GetPhoto(int i)
    {
      lock (this.photos)
        return i < 0 || i >= this.photos.Count ? (Texture2D) null : this.photos[i].Texture;
    }

    public PhotoAlbumScreen()
      : this((Action<int>) null)
    {
    }

    public PhotoAlbumScreen(Action<int> photoSelected)
    {
      this.photoSelected = photoSelected;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.photos = new List<PhotoTag>();
      this.loader = new PhotoLoader();
      this.LoadPage(0, false);
      this.selected = 0;
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      this.ClearPhotos();
    }

    private void ClearPhotos()
    {
      lock (this.photos)
      {
        for (int index = 0; index < this.photos.Count; ++index)
        {
          if (this.photos[index].Texture != null)
            this.photos[index].Texture.Dispose();
        }
        this.photos.Clear();
      }
    }

    private void LoadPage(int startID, bool backwards)
    {
      lock (this.photos)
        this.photos.Clear();
      this.LoadPhotos(startID, backwards);
      this.selected = 0;
    }

    private void LoadPhotos(int startID, bool backwards)
    {
      this.loader.End(0);
      this.loader.Start(startID, backwards, PhotoFileType.PhotoImage, new PhotoLoaded(this.OnPhotoLoaded), new Action(this.OnFinish), (ShouldLoadPhoto) null);
      this.isPhotosLoading = true;
    }

    private bool OnPhotoLoaded(int photoID, Texture2D texture, bool backwards)
    {
      this.currentAttempt = photoID;
      if (texture == null)
        return true;
      lock (this.photos)
      {
        PhotoTag photoTag = new PhotoTag()
        {
          PhotoID = photoID,
          Texture = texture
        };
        if (backwards)
          this.photos.Insert(0, photoTag);
        else
          this.photos.Add(photoTag);
        return this.photos.Count < this.pagesize;
      }
    }

    private void OnFinish()
    {
      this.runOnFinished = true;
    }

    private void OnFinishMainThread()
    {
      this.runOnFinished = false;
      if (!this.isPhotosLoading)
        return;
      this.isPhotosLoading = false;
      if (Globals2.GetPhotoCount() == 0)
        this.ExitScreen();
      if (this.photos.Count == 0)
      {
        this.selected = 0;
        this.LoadPage(0, false);
      }
      else
      {
        if (this.selected < this.photos.Count)
          return;
        this.selected = this.photos.Count - 1;
      }
    }

    public override bool HandleInput(InputState input)
    {
      if (!this.ControllingPlayer.HasValue || !this.HandleInputCore(input))
        return false;
      Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
      return true;
    }

    private bool HandleInputCore(InputState input)
    {
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.isPhotosLoading = false;
        if (this.loader != null)
          this.loader.End(0);
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorUp))
      {
        if (this.selected - 3 >= 0 || this.selected + 3 < this.currentPhotoCount)
        {
          this.selected -= 3;
          if (this.selected < 0)
            this.selected += 6;
          return true;
        }
      }
      else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorDown))
      {
        this.selected += 3;
        if (this.selected >= this.pagesize)
          this.selected -= 6;
        else if (this.selected >= this.currentPhotoCount)
        {
          this.selected -= 3;
          return false;
        }
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorRight))
      {
        ++this.selected;
        if (this.selected >= this.currentPhotoCount)
          this.selected = 0;
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorLeft))
      {
        --this.selected;
        if (this.selected < 0)
          this.selected = this.currentPhotoCount - 1;
        return true;
      }
      if (!this.isPhotosLoading)
      {
        if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.SelectItem))
        {
          lock (this.photos)
          {
            if (this.selected >= 0)
            {
              if (this.selected < this.photos.Count)
              {
                int photoId = this.photos[this.selected].PhotoID;
                if (this.photoSelected != null)
                {
                  this.photoSelected(photoId);
                  this.ExitScreen();
                }
                else
                {
                  Texture2D photo = this.GetPhoto(this.selected);
                  if (photo != null)
                    this.ScreenManager.AddScreen((GameScreen) new PhotoViewerScreen(photoId, photo), this.ControllingPlayer);
                }
              }
            }
          }
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown))
        {
          lock (this.photos)
          {
            if (this.photos.Count > 0)
            {
              int startID = this.photos[this.photos.Count - 1].PhotoID + 1;
              if (this.photos.Count < this.pagesize)
                startID = 0;
              this.LoadPage(startID, false);
            }
            else
              this.LoadPage(0, false);
          }
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp))
        {
          if (Globals2.GetPhotoCount() > this.pagesize)
          {
            lock (this.photos)
            {
              if (this.photos.Count > 0)
              {
                int startID = this.photos[0].PhotoID - 1;
                if (startID < 0)
                  startID = (int) byte.MaxValue;
                this.LoadPage(startID, true);
              }
              else
                this.LoadPage(0, false);
            }
          }
          return true;
        }
        if (InputManager.IsButtonPressedNew(this.ControllingPlayer.Value, Buttons.Y))
        {
          lock (this.photos)
          {
            if (this.photos.Count > 0)
            {
              int photoId = this.photos[this.photos.Count - 1].PhotoID;
              Globals2.DeletePhoto(this.photos[this.selected].PhotoID);
              this.photos.RemoveAt(this.selected);
              this.LoadPhotos(photoId + 1, false);
            }
          }
          return true;
        }
      }
      return base.HandleInput(input);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      this.inputCleared = !this.otherScreenHasFocus && !coveredByOtherScreen;
      if (!this.runOnFinished)
        return;
      this.OnFinishMainThread();
    }

    protected override void DrawCore()
    {
      Rectangle rectangle = new Rectangle(0, 0, 288, 288);
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.Draw(CoreGlobals.BlankTexture, this.GraphicsDevice.Viewport.Rectangle(), Color.Black);
      int index1 = 0;
      for (int index2 = 0; index2 < 2; ++index2)
      {
        for (int index3 = 0; index3 < 3; ++index3)
        {
          rectangle.X = 200 + index3 * (rectangle.Width + 20);
          rectangle.Y = 60 + index2 * (rectangle.Height + 20);
          lock (this.photos)
          {
            if (index1 < this.photos.Count)
            {
              if (this.photos[index1].Texture != null)
              {
                Rectangle rect = rectangle.Expand(4);
                spriteBatch.DrawBox(rect, 4, this.selected == index1 ? Color.Yellow : Color.White, 0.0f);
                spriteBatch.Draw(this.photos[index1].Texture, rectangle, Color.White);
                string text = this.photos[index1].PhotoID.ToString();
                Vector2 vector2 = this.Font.MeasureString(text) * 0.8f;
                spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(rectangle.X + 4, rectangle.Y + 4, (int) ((double) vector2.X + 8.0), (int) ((double) vector2.Y - 8.0)), Color.Black * 0.5f);
                spriteBatch.DrawString(this.Font, text, new Vector2((float) (rectangle.X + 8), (float) (rectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
                if (this.selected == index1)
                {
                  int num = 4;
                  spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - 40 + num, rectangle.Width, 40 - num), Color.Black * 0.5f);
                  spriteBatch.Draw(CoreGlobals.ButtonTextureA, new Rectangle(rectangle.X + 12, rectangle.Y + rectangle.Height - 36 + num, 24, 24), Color.White);
                  spriteBatch.DrawString(this.Font, "View", new Vector2((float) (rectangle.X + 44), (float) (rectangle.Y + rectangle.Height - 34 + num)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
                  spriteBatch.Draw(CoreGlobals.ButtonTextureY, new Rectangle(rectangle.X + rectangle.Width - 120, rectangle.Y + rectangle.Height - 36 + num, 24, 24), Color.White);
                  spriteBatch.DrawString(this.Font, "Delete", new Vector2((float) (rectangle.X + rectangle.Width - 120 + 32), (float) (rectangle.Y + rectangle.Height - 34 + num)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
                }
              }
            }
          }
          ++index1;
        }
      }
      if (this.isPhotosLoading)
      {
        spriteBatch.DrawFilledBox(new Rectangle(497, 340, 310, 35), 2, Color.White, Color.DarkSlateGray);
        spriteBatch.DrawStringCentered(CoreGlobals.GameFont, "Loading: " + (object) this.currentAttempt + "... Please wait...", 346f, Color.White, 0.5f);
      }
      spriteBatch.End();
    }
  }
}
