// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.MenuEntry
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.Engine.GameState
{
  public class MenuEntry
  {
    public bool ProportionateEntryTexture = true;
    public Vector2 TextOffsetEx = new Vector2();
    public Color ColorHighlighted = Color.Yellow;
    public MenuScreen Screen;
    private float selectionFade;
    public Texture2D EntryTexture;
    public string EntryTextureName;
    public Rectangle? EntryTextureRect;
    public Rectangle? EntryTextureSrcRect;
    public string ButtonTextB;
    public string ButtonTextA;
    public string ButtonTextX;
    public string ButtonTextY;
    public object Tag;
    public bool OverrideColor;
    public Color ColorOverride;
    public ToolTip ToolTip;
    protected Rectangle lastHighLightRect;

    public virtual bool IsToolTipEnabled
    {
      get
      {
        if (this.ToolTip != null)
          return this.ToolTip.IsValid;
        return false;
      }
    }

    public string Text { get; set; }

    public bool IsEnabled { get; set; }

    public Color ColorSelected { get; set; }

    public Color ColorUnselected { get; set; }

    public Color ColorDisabled { get; set; }

    public Color ColorShadow { get; set; }

    public int Height
    {
      get
      {
        return this.Screen.ItemHeight + this.Screen.ItemGapY;
      }
    }

    public virtual Vector2 TextOffset
    {
      get
      {
        return new Vector2(16f + this.TextOffsetEx.X, this.TextOffsetEx.Y);
      }
    }

    public virtual Vector2 HighlightBarOffset
    {
      get
      {
        return new Vector2(0.0f, 0.0f);
      }
    }

    public Rectangle LastHighLightRect
    {
      get
      {
        return this.lastHighLightRect;
      }
    }

    public event EventHandler<PlayerIndexEventArgs> Selected;

    public event EventHandler<PlayerIndexEventArgs> SelectUp;

    public event EventHandler<PlayerIndexEventArgs> SelectDown;

    public event EventHandler<PlayerIndexEventArgs> SelectLeft;

    public event EventHandler<PlayerIndexEventArgs> SelectRight;

    public event EventHandler<PlayerIndexEventArgs> SelectXButton;

    public event EventHandler<PlayerIndexEventArgs> SelectYButton;

    public bool IsSelectedHaveHandler
    {
      get
      {
        return this.Selected != null;
      }
    }

    public bool IsSelectUpHaveHandler
    {
      get
      {
        return this.SelectUp != null;
      }
    }

    public bool IsSelectDownHaveHandler
    {
      get
      {
        return this.SelectDown != null;
      }
    }

    public bool IsSelectLeftHaveHandler
    {
      get
      {
        return this.SelectLeft != null;
      }
    }

    public bool IsSelectRightHaveHandler
    {
      get
      {
        return this.SelectRight != null;
      }
    }

    public bool IsSelectXButtonHaveHandler
    {
      get
      {
        return this.SelectXButton != null;
      }
    }

    public bool IsSelectYButtonHaveHandler
    {
      get
      {
        return this.SelectYButton != null;
      }
    }

    protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
    {
      if (this.Selected == null)
        return;
      this.Selected((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    protected internal virtual void OnSelectUp(PlayerIndex playerIndex)
    {
      if (this.SelectUp == null)
        return;
      this.SelectUp((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    protected internal virtual void OnSelectDown(PlayerIndex playerIndex)
    {
      if (this.SelectDown == null)
        return;
      this.SelectDown((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    protected internal virtual void OnSelectLeft(PlayerIndex playerIndex)
    {
      if (this.SelectLeft == null)
        return;
      this.SelectLeft((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    protected internal virtual void OnSelectRight(PlayerIndex playerIndex)
    {
      if (this.SelectRight == null)
        return;
      this.SelectRight((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    protected internal virtual void OnSelectXButton(PlayerIndex playerIndex)
    {
      if (this.SelectXButton == null)
        return;
      this.SelectXButton((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    protected internal virtual void OnSelectYButton(PlayerIndex playerIndex)
    {
      if (this.SelectYButton == null)
        return;
      this.SelectYButton((object) this, new PlayerIndexEventArgs(playerIndex));
    }

    public MenuEntry(MenuScreen screen, string text)
      : this(screen, text, (string) null)
    {
    }

    public MenuEntry(MenuScreen screen, string text, string textureName)
    {
      this.Screen = screen;
      this.Text = text;
      this.IsEnabled = true;
      this.ColorSelected = Color.Yellow;
      this.ColorUnselected = Color.White;
      this.ColorDisabled = Color.Gray;
      this.ColorShadow = Color.Black;
      this.EntryTextureName = textureName;
    }

    public void LoadContent()
    {
      if (!this.EntryTextureName.IsEmpty())
        this.EntryTexture = CoreGlobals.Content.Load<Texture2D>(this.EntryTextureName);
      this.LoadContentCore();
      this.ToolTip = new ToolTip(this.Screen.ItemFont);
    }

    protected virtual void LoadContentCore()
    {
    }

    public void UnloadContent()
    {
      this.UnloadContentCore();
    }

    protected virtual void UnloadContentCore()
    {
    }

    public virtual void Update(MenuScreen screen, bool isSelected)
    {
      float num = Services.ElapsedTime * 4f;
      if (isSelected)
        this.selectionFade = Math.Min(this.selectionFade + num, 1f);
      else
        this.selectionFade = Math.Max(this.selectionFade - num, 0.0f);
    }

    public virtual void Draw(Vector2 position, int index, bool isSelected)
    {
      Color itemColor = this.GetItemColor(isSelected);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      if (!this.Text.IsEmpty())
      {
        Vector2 measure = this.Screen.ItemFont.MeasureString(this.Text) * this.Screen.ItemTextScale;
        Vector2 pos = position + this.TextOffset;
        pos.Y = (float) (((double) this.Height - (double) measure.Y) / 2.0 + (double) position.Y + 1.0);
        if (this.ColorShadow.A > (byte) 0)
          this.DrawText(this.Text, pos + Vector2.One, this.ColorShadow * ((float) this.Screen.TransitionAlpha / (float) byte.MaxValue), measure);
        this.DrawText(this.Text, pos, itemColor, measure);
      }
      this.DrawTexture(position, itemColor);
      this.DrawExtra(position, itemColor, this.Screen.ItemTextScale);
    }

    protected virtual void DrawText(string text, Vector2 pos, Color color, Vector2 measure)
    {
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, text, pos, color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }

    protected virtual Color GetItemColor(bool isSelected)
    {
      Color color = this.IsEnabled ? (this.OverrideColor ? this.ColorOverride : (isSelected ? this.ColorSelected : this.ColorUnselected)) : this.ColorDisabled;
      return new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
    }

    public virtual Rectangle GetHighLightRect(Vector2 position)
    {
      return new Rectangle();
    }

    protected virtual void DrawHighLight(Vector2 position, Color color)
    {
    }

    protected virtual void DrawTexture(Vector2 position, Color color)
    {
      if (this.EntryTexture == null)
        return;
      Rectangle rectangle = new Rectangle();
      Rectangle destinationRectangle = new Rectangle(0, 0, this.EntryTexture.Width, this.EntryTexture.Height);
      if (this.EntryTextureRect.HasValue)
      {
        destinationRectangle = this.EntryTextureRect.Value;
        destinationRectangle.X += (int) position.X;
        destinationRectangle.Y += (int) position.Y;
        rectangle = destinationRectangle;
      }
      else
      {
        rectangle.Width = rectangle.Height = this.Height - 4;
        rectangle.X = (int) ((double) position.X - (double) rectangle.Width - 15.0);
        rectangle.Y = (int) ((double) position.Y + 2.0);
        if (this.ProportionateEntryTexture)
        {
          if (destinationRectangle.Width > rectangle.Width)
          {
            destinationRectangle.Height = (int) ((double) destinationRectangle.Height * (double) rectangle.Width / (double) destinationRectangle.Width);
            destinationRectangle.Width = rectangle.Width;
          }
          if (destinationRectangle.Height > rectangle.Height)
          {
            destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) rectangle.Height / (double) destinationRectangle.Height);
            destinationRectangle.Height = rectangle.Height;
          }
          destinationRectangle.X = rectangle.X + (rectangle.Width - destinationRectangle.Width) / 2;
          destinationRectangle.Y = rectangle.Y + (rectangle.Height - destinationRectangle.Height) / 2;
        }
        else
          destinationRectangle = rectangle;
      }
      if (this.Screen.DrawItemTextureBorder)
        this.Screen.SpriteBatch.DrawBox(new Rectangle(rectangle.X - 2, rectangle.Y - 2, rectangle.Width + 4, rectangle.Height + 4), 1, color, 0.0f);
      this.Screen.SpriteBatch.Draw(this.EntryTexture, destinationRectangle, this.EntryTextureSrcRect, color);
    }

    protected virtual void DrawExtra(Vector2 position, Color color, float scale)
    {
    }
  }
}
