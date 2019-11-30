// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.MenuScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.GameState
{
  public abstract class MenuScreen : GameScreen
  {
    public static float DefaultTitleScale = 1.25f;
    public static Vector2 DefaultTitlePosition = new Vector2(426f, 82f);
    public static string DefaultMenuMoveSound = "MenuMove";
    public static string DefaultMenuSelectSound = "MenuSelect";
    public static string DefaultMenuCancelSound = "MenuSelect";
    public static string DefaultMenuInvalidOperationSound = "MenuInvalidOperation";
    public static Color DefaultMenuTitleStripColor = Color.White * 0.4f;
    public Color TitleColor = Color.White;
    public bool IsSelectLeftEnabled = true;
    public bool IsSelectRightEnabled = true;
    public bool IsSelectUpEnabled = true;
    public bool IsSelectDownEnabled = true;
    public string ButtonTextB = "Back";
    public float ButtonScale = 0.6f;
    public Color ColorWhite = Color.White;
    public Color ColorBlack = Color.Black;
    public int ToolTipX = 800;
    protected Vector2 titlePosition = MenuScreen.DefaultTitlePosition;
    protected float titleScale = MenuScreen.DefaultTitleScale;
    private List<MenuEntry> menuEntries = new List<MenuEntry>();
    public int ItemHeight;
    public int ItemGapY;
    public string MenuTitle;
    public string MenuSubTitle;
    public Color TitleStripColor;
    public float ItemTextScale;
    public int ItemsPerPage;
    public bool DrawItemTextures;
    public bool DrawItemTextureBorder;
    public bool DrawItemLines;
    public bool DrawTitleStrip;
    public string MenuMoveSound;
    public string MenuSelectSound;
    public string MenuCancelSound;
    public string MenuInvalidOperationSound;
    public MenuSoundState SoundState;
    public string ButtonTextA;
    public string ButtonTextX;
    public string ButtonTextY;
    public string ButtonTextLB;
    public string ButtonTextRB;
    public SpriteFont ItemFont;
    protected float transitionOffset;
    protected int itemAtTopOfPage;
    protected int selectedEntry;
    private Vector2 selectedPosition;
    public static Texture2D signedInGamerTexture;

    public string[] ItemNames
    {
      get
      {
        string[] strArray = new string[this.menuEntries.Count];
        for (int index = 0; index < this.menuEntries.Count; ++index)
          strArray[index] = this.menuEntries[index].Text;
        return strArray;
      }
    }

    protected List<MenuEntry> MenuEntries
    {
      get
      {
        return this.menuEntries;
      }
    }

    protected int SelectedEntry
    {
      get
      {
        return this.selectedEntry;
      }
      set
      {
        this.selectedEntry = value;
        this.UpdatePageExtents();
      }
    }

    protected virtual string SelectedEntryButtonTextA
    {
      get
      {
        if (this.selectedEntry >= 0 && this.selectedEntry < this.menuEntries.Count && this.menuEntries[this.selectedEntry].ButtonTextA != null)
          return this.menuEntries[this.selectedEntry].ButtonTextA;
        return this.ButtonTextA;
      }
    }

    protected virtual string SelectedEntryButtonTextX
    {
      get
      {
        if (this.selectedEntry >= 0 && this.selectedEntry < this.menuEntries.Count && this.menuEntries[this.selectedEntry].ButtonTextX != null)
          return this.menuEntries[this.selectedEntry].ButtonTextX;
        return this.ButtonTextX;
      }
    }

    protected virtual string SelectedEntryButtonTextY
    {
      get
      {
        if (this.selectedEntry >= 0 && this.selectedEntry < this.menuEntries.Count && this.menuEntries[this.selectedEntry].ButtonTextY != null)
          return this.menuEntries[this.selectedEntry].ButtonTextY;
        return this.ButtonTextY;
      }
    }

    protected virtual string SelectedEntryButtonTextB
    {
      get
      {
        if (this.selectedEntry >= 0 && this.selectedEntry < this.menuEntries.Count && this.menuEntries[this.selectedEntry].ButtonTextB != null)
          return this.menuEntries[this.selectedEntry].ButtonTextB;
        return this.ButtonTextB;
      }
    }

    protected virtual bool IsButtonXValid
    {
      get
      {
        return true;
      }
    }

    protected virtual bool IsButtonYValid
    {
      get
      {
        return true;
      }
    }

    public void EnableEntry(int i, bool enable)
    {
      if (i < 0 || i >= this.menuEntries.Count)
        return;
      this.menuEntries[i].IsEnabled = enable;
    }

    public MenuScreen(string menuTitle)
      : this(menuTitle, (string) null)
    {
    }

    public MenuScreen(string menuTitle, string subTitle)
    {
      this.MenuTitle = menuTitle;
      this.MenuSubTitle = subTitle;
      this.ItemTextScale = 1f;
      this.MenuMoveSound = MenuScreen.DefaultMenuMoveSound;
      this.MenuSelectSound = MenuScreen.DefaultMenuSelectSound;
      this.MenuCancelSound = MenuScreen.DefaultMenuCancelSound;
      this.MenuInvalidOperationSound = MenuScreen.DefaultMenuInvalidOperationSound;
      this.DrawTitleStrip = true;
      this.TitleStripColor = MenuScreen.DefaultMenuTitleStripColor;
      InputManager.PushVirtualMouse();
    }

    public override void LoadContent()
    {
      base.LoadContent();
      if (this.ItemsPerPage == 0)
        this.ItemsPerPage = this.MenuEntries.Count;
      this.ItemFont = this.Font = CoreGlobals.GameFont;
      foreach (MenuEntry menuEntry in this.MenuEntries)
      {
        menuEntry.Screen = this;
        menuEntry.LoadContent();
      }
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      foreach (MenuEntry menuEntry in this.menuEntries)
        menuEntry.UnloadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      InputManager.PopVirtualMouse();
    }

    public override bool HandleInput(InputState input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      if (base.HandleInput(input))
        return true;
      int mouseWheelDelta = InputManager.GetMouseWheelDelta(this.ControllingPlayer);
      if (mouseWheelDelta > 0)
      {
        if (this.itemAtTopOfPage > 0)
          --this.itemAtTopOfPage;
      }
      else if (mouseWheelDelta < 0 && this.itemAtTopOfPage < this.menuEntries.Count - this.ItemsPerPage)
        ++this.itemAtTopOfPage;
      Vector2 mousePosDelta = InputManager.GetMousePosDelta(this.ControllingPlayer);
      if ((double) mousePosDelta.X != 0.0 || (double) mousePosDelta.Y != 0.0 || mouseWheelDelta != 0)
      {
        Point mousePos = InputManager.GetMousePos(this.ControllingPlayer);
        Vector2 position = this.SlidePositionForTransition(this.StartPosition);
        int num = Math.Min(this.itemAtTopOfPage + this.ItemsPerPage, this.menuEntries.Count);
        for (int itemAtTopOfPage = this.itemAtTopOfPage; itemAtTopOfPage < num; ++itemAtTopOfPage)
        {
          if (this.menuEntries[itemAtTopOfPage].GetHighLightRect(position).Contains(mousePos))
          {
            this.selectedEntry = itemAtTopOfPage;
            break;
          }
          position.Y += (float) (this.ItemHeight + this.ItemGapY);
        }
        return true;
      }
      if (this.IsSelectUpEnabled && input.IsMenuUp(this.ControllingPlayer))
      {
        this.SoundState = MenuSoundState.MenuMove;
        this.OnSelectUp(input.LastPlayerIndex);
        this.PlaySound();
        return true;
      }
      if (this.IsSelectDownEnabled && input.IsMenuDown(this.ControllingPlayer))
      {
        this.SoundState = MenuSoundState.MenuMove;
        this.OnSelectDown(input.LastPlayerIndex);
        this.PlaySound();
        return true;
      }
      if (this.IsSelectLeftEnabled && input.IsMenuLeft(this.ControllingPlayer))
      {
        this.SoundState = MenuSoundState.MenuMove;
        this.OnSelectLeft(input.LastPlayerIndex);
        this.PlaySound();
        return true;
      }
      if (this.IsSelectRightEnabled && input.IsMenuRight(this.ControllingPlayer))
      {
        this.SoundState = MenuSoundState.MenuMove;
        this.OnSelectRight(input.LastPlayerIndex);
        this.PlaySound();
        return true;
      }
      PlayerIndex playerIndex1;
      if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex1))
      {
        this.SoundState = MenuSoundState.MenuSelect;
        this.OnSelectEntry(playerIndex1);
        this.PlaySound();
        return true;
      }
      if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex1))
      {
        this.SoundState = MenuSoundState.MenuCancel;
        this.OnCancel(playerIndex1);
        this.PlaySound();
        return true;
      }
      if (input.IsMenuXButton(this.ControllingPlayer, out playerIndex1))
      {
        this.SoundState = MenuSoundState.MenuSelect;
        this.OnSelectXButton(playerIndex1);
        this.PlaySound();
        return true;
      }
      if (input.IsMenuYButton(this.ControllingPlayer, out playerIndex1))
      {
        this.SoundState = MenuSoundState.MenuSelect;
        this.OnSelectYButton(playerIndex1);
        this.PlaySound();
        return true;
      }
      PlayerIndex playerIndex2 = this.ControllingPlayer.HasValue ? this.ControllingPlayer.Value : PlayerIndex.One;
      return !input.CurrentGamePadStates[(int) playerIndex2].IsEmpty();
    }

    protected void ScrollUpPage()
    {
      if (this.selectedEntry > this.itemAtTopOfPage)
      {
        this.selectedEntry = this.itemAtTopOfPage;
      }
      else
      {
        int num = this.ItemsPerPage < 2 ? 1 : this.ItemsPerPage - 1;
        this.selectedEntry -= num;
        if (this.selectedEntry < 0)
          this.selectedEntry = 0;
        this.itemAtTopOfPage -= num;
        if (this.itemAtTopOfPage >= 0)
          return;
        this.itemAtTopOfPage = 0;
      }
    }

    protected void ScrollDownPage()
    {
      if (this.selectedEntry < this.itemAtTopOfPage + this.ItemsPerPage - 1)
      {
        this.selectedEntry = this.itemAtTopOfPage + this.ItemsPerPage - 1;
        if (this.selectedEntry < this.menuEntries.Count)
          return;
        this.selectedEntry = Math.Max(0, this.menuEntries.Count - 1);
      }
      else
      {
        int num = this.ItemsPerPage > 1 ? this.ItemsPerPage - 1 : 0;
        this.selectedEntry += num;
        if (this.selectedEntry >= this.menuEntries.Count)
          this.selectedEntry = Math.Max(0, this.menuEntries.Count - 1);
        this.itemAtTopOfPage += num;
        if (this.itemAtTopOfPage <= this.menuEntries.Count - this.ItemsPerPage)
          return;
        this.itemAtTopOfPage = this.menuEntries.Count - this.ItemsPerPage;
      }
    }

    private void SelectEntryHandler(object sender, EventArgs e)
    {
      this.SoundState = MenuSoundState.MenuSelect;
      this.OnSelectEntry(this.ControllingPlayer.HasValue ? this.ControllingPlayer.Value : PlayerIndex.One);
      this.PlaySound();
    }

    protected void OnSelectEntry(PlayerIndex playerIndex)
    {
      if (this.selectedEntry >= 0 && this.selectedEntry < this.menuEntries.Count && this.menuEntries[this.selectedEntry].IsEnabled)
      {
        this.OnSelectEntryCore(playerIndex);
        if (this.menuEntries.Count <= this.selectedEntry)
          return;
        this.menuEntries[this.selectedEntry].OnSelectEntry(playerIndex);
      }
      else
        this.SoundState = MenuSoundState.MenuInvalidOperation;
    }

    protected void OnSelectUp(PlayerIndex playerIndex)
    {
      this.OnSelectUpCore(playerIndex);
      if (this.selectedEntry >= this.menuEntries.Count)
        return;
      this.menuEntries[this.selectedEntry].OnSelectUp(playerIndex);
    }

    protected void OnSelectDown(PlayerIndex playerIndex)
    {
      this.OnSelectDownCore(playerIndex);
      if (this.selectedEntry >= this.menuEntries.Count)
        return;
      this.menuEntries[this.selectedEntry].OnSelectDown(playerIndex);
    }

    protected void OnSelectLeft(PlayerIndex playerIndex)
    {
      this.OnSelectLeftCore(playerIndex);
      if (this.selectedEntry >= this.menuEntries.Count)
        return;
      this.menuEntries[this.selectedEntry].OnSelectLeft(playerIndex);
    }

    protected void OnSelectRight(PlayerIndex playerIndex)
    {
      this.OnSelectRightCore(playerIndex);
      if (this.selectedEntry >= this.menuEntries.Count)
        return;
      this.menuEntries[this.selectedEntry].OnSelectRight(playerIndex);
    }

    protected void OnSelectXButton(PlayerIndex playerIndex)
    {
      if (!this.IsButtonXValid)
        return;
      this.OnSelectXButtonCore(playerIndex);
      if (this.selectedEntry >= this.menuEntries.Count)
        return;
      this.menuEntries[this.selectedEntry].OnSelectXButton(playerIndex);
    }

    protected void OnSelectYButton(PlayerIndex playerIndex)
    {
      if (!this.IsButtonYValid)
        return;
      this.OnSelectYButtonCore(playerIndex);
      if (this.selectedEntry >= this.menuEntries.Count)
        return;
      this.menuEntries[this.selectedEntry].OnSelectYButton(playerIndex);
    }

    protected virtual void OnSelectEntryCore(PlayerIndex playerIndex)
    {
    }

    protected virtual void OnSelectLeftCore(PlayerIndex playerIndex)
    {
    }

    protected virtual void OnSelectRightCore(PlayerIndex playerIndex)
    {
    }

    protected virtual void OnSelectXButtonCore(PlayerIndex playerIndex)
    {
    }

    protected virtual void OnSelectYButtonCore(PlayerIndex playerIndex)
    {
    }

    protected virtual void OnSelectUpCore(PlayerIndex playerIndex)
    {
      int selectedEntry = this.selectedEntry;
      if (this.menuEntries.Count > 0)
      {
        int num = 0;
        do
        {
          if (--this.selectedEntry < 0)
            this.selectedEntry = this.menuEntries.Count - 1;
          if (this.selectedEntry < 0 || this.selectedEntry >= this.menuEntries.Count)
          {
            this.selectedEntry = 0;
            break;
          }
        }
        while (++num <= this.menuEntries.Count && !this.menuEntries[this.selectedEntry].IsEnabled);
      }
      this.UpdatePageExtents();
      if (this.selectedEntry != selectedEntry)
        return;
      this.SoundState = MenuSoundState.MenuInvalidOperation;
    }

    protected virtual void OnSelectDownCore(PlayerIndex playerIndex)
    {
      int selectedEntry = this.selectedEntry;
      if (this.menuEntries.Count > 0)
      {
        int num = 0;
        do
        {
          if (++this.selectedEntry >= this.menuEntries.Count)
            this.selectedEntry = 0;
          if (this.selectedEntry < 0 || this.selectedEntry >= this.menuEntries.Count)
          {
            this.selectedEntry = 0;
            break;
          }
        }
        while (++num <= this.menuEntries.Count && !this.menuEntries[this.selectedEntry].IsEnabled);
      }
      this.UpdatePageExtents();
      if (this.selectedEntry != selectedEntry)
        return;
      this.SoundState = MenuSoundState.MenuInvalidOperation;
    }

    private void UpdatePageExtents()
    {
      if (this.selectedEntry < this.itemAtTopOfPage)
        this.itemAtTopOfPage = this.selectedEntry;
      if (this.selectedEntry < this.itemAtTopOfPage + this.ItemsPerPage)
        return;
      this.itemAtTopOfPage = this.selectedEntry - Math.Max(1, this.ItemsPerPage) + 1;
    }

    public virtual void OnCancel(PlayerIndex playerIndex)
    {
      this.ExitScreen();
    }

    public void OnCancel(object sender, PlayerIndexEventArgs e)
    {
      this.OnCancel(e.PlayerIndex);
    }

    protected virtual void PlaySound()
    {
      switch (this.SoundState)
      {
        case MenuSoundState.MenuMove:
          this.PlaySound(this.MenuMoveSound);
          break;
        case MenuSoundState.MenuInvalidOperation:
          this.PlaySound(this.MenuInvalidOperationSound);
          break;
        case MenuSoundState.MenuSelect:
          this.PlaySound(this.MenuSelectSound);
          break;
        case MenuSoundState.MenuCancel:
          this.PlaySound(this.MenuCancelSound);
          break;
      }
    }

    protected void PlaySound(string asset)
    {
      this.PlaySound(asset, 1f);
    }

    protected void PlaySound(string asset, float volume)
    {
      if (asset.IsEmpty())
        return;
      CoreGlobals.AudioManager?.PlaySound(asset);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      for (int index = 0; index < this.menuEntries.Count; ++index)
      {
        if (this.menuEntries[index] != null)
        {
          bool isSelected = this.IsActive && index == this.selectedEntry;
          this.menuEntries[index].Update(this, isSelected);
        }
      }
    }

    public void SetSelectedEntry(int i)
    {
      if (i < this.itemAtTopOfPage || i >= Math.Min(this.itemAtTopOfPage + this.ItemsPerPage, this.menuEntries.Count))
      {
        if (i > this.menuEntries.Count - this.ItemsPerPage)
        {
          this.itemAtTopOfPage = this.menuEntries.Count - this.ItemsPerPage;
        }
        else
        {
          this.itemAtTopOfPage = i - this.ItemsPerPage / 2;
          if (this.itemAtTopOfPage < 0)
            this.itemAtTopOfPage = 0;
          if (this.itemAtTopOfPage + this.ItemsPerPage > this.menuEntries.Count)
            this.itemAtTopOfPage = this.menuEntries.Count - this.ItemsPerPage;
        }
      }
      this.selectedEntry = i;
    }

    protected override void DrawCore()
    {
      this.transitionOffset = (float) Math.Pow((double) this.TransitionPosition, 2.0);
      this.ColorWhite.A = this.TransitionAlpha;
      this.ColorBlack.A = this.ColorWhite.A;
      this.SpriteBatch.Begin();
      this.DrawBackground();
      this.DrawTitle();
      this.DrawMenuEntries();
      this.DrawMenuExtra();
      this.DrawBottomBar();
      this.DrawButtons((int) this.ButtonStartPosition.X);
      this.DrawToolTip();
      this.SpriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    protected virtual void DrawBackground()
    {
    }

    protected virtual void DrawToolTip()
    {
      if (!GameScreen.isToolTipsEnabledGlobal || this.selectedEntry < 0 || (this.selectedEntry >= this.menuEntries.Count || !this.menuEntries[this.selectedEntry].IsToolTipEnabled))
        return;
      Rectangle rect = this.menuEntries[this.selectedEntry].ToolTip.GetRect();
      rect.X = this.ToolTipX;
      rect.Y = (int) this.selectedPosition.Y + this.ItemHeight / 2;
      int num1 = (int) ((double) CoreGlobals.GraphicsDevice.Viewport.Width * 0.899999976158142);
      if (rect.X + rect.Width > num1 - 4)
      {
        rect.X = num1 - 4 - rect.Width + 1;
        if (rect.X < (int) ((double) this.selectedPosition.X + (double) this.ItemFont.MeasureString(this.menuEntries[this.selectedEntry].Text).X * (double) this.ItemTextScale))
          rect.Y = (int) this.selectedPosition.Y + this.ItemHeight + this.ItemGapY;
      }
      int num2 = (int) ((double) CoreGlobals.GraphicsDevice.Viewport.Height * 0.899999976158142);
      if (rect.Y + rect.Height > num2 - 4)
        rect.Y = num2 - 4 - rect.Height + 1;
      this.menuEntries[this.selectedEntry].ToolTip.Draw(this.SpriteBatch, rect);
    }

    protected virtual void DrawTitle()
    {
      if (this.MenuTitle.IsEmpty())
        return;
      Vector2 origin = this.Font.MeasureString(this.MenuTitle) / 2f;
      Color color1 = this.TitleColor * (float) this.TransitionAlpha;
      Vector2 titlePosition = this.titlePosition;
      titlePosition.Y -= this.transitionOffset * 100f;
      if (this.DrawTitleStrip)
      {
        Rectangle destinationRectangle = new Rectangle(0, (int) ((double) titlePosition.Y - 12.0 - (double) origin.Y), this.ScreenManager.Game.GraphicsDevice.Viewport.Width, (int) ((double) origin.Y * 2.0 + 20.0));
        Color color2 = this.TitleStripColor * (float) ((double) this.TitleStripColor.A / (double) byte.MaxValue * (double) this.TransitionAlpha / (double) byte.MaxValue);
        Color color3 = color2 * (float) ((double) color2.A / (double) byte.MaxValue * 2.0);
        this.SpriteBatch.Draw(this.ScreenManager.BlankTexture, new Rectangle(destinationRectangle.X, destinationRectangle.Y - 1, destinationRectangle.Width, 1), color3);
        this.SpriteBatch.Draw(this.ScreenManager.BlankTexture, new Rectangle(destinationRectangle.X, destinationRectangle.Y + destinationRectangle.Height, destinationRectangle.Width, 1), color3);
        this.SpriteBatch.Draw(this.ScreenManager.BlankTexture, destinationRectangle, color2);
      }
      this.SpriteBatch.DrawString(this.Font, this.MenuTitle, titlePosition, color1, 0.0f, origin, this.titleScale, SpriteEffects.None, 0.0f);
    }

    protected virtual void DrawMenuEntries()
    {
      Vector2 position = this.SlidePositionForTransition(this.StartPosition);
      int num = Math.Min(this.itemAtTopOfPage + this.ItemsPerPage, this.menuEntries.Count);
      this.selectedPosition = Vector2.Zero;
      for (int itemAtTopOfPage = this.itemAtTopOfPage; itemAtTopOfPage < num; ++itemAtTopOfPage)
      {
        MenuEntry menuEntry = this.menuEntries[itemAtTopOfPage];
        if (menuEntry != null)
        {
          bool isSelected = itemAtTopOfPage == this.selectedEntry;
          this.DrawEntry(menuEntry, itemAtTopOfPage, position, isSelected);
          if (isSelected)
            this.selectedPosition = position;
        }
        position.Y += (float) (this.ItemHeight + this.ItemGapY);
      }
    }

    protected virtual Vector2 SlidePositionForTransition(Vector2 position)
    {
      if (this.ScreenState == ScreenState.TransitionOn)
        position.X -= this.transitionOffset * 256f;
      else
        position.X += this.transitionOffset * 512f;
      return position;
    }

    protected virtual void DrawMenuExtra()
    {
    }

    protected virtual void DrawBottomBar()
    {
    }

    protected virtual void DrawButtons(int x)
    {
      this.DrawButton(CoreGlobals.ButtonTextureA, this.SelectedEntryButtonTextA, ref x);
      this.DrawButton(CoreGlobals.ButtonTextureX, this.SelectedEntryButtonTextX, ref x);
      this.DrawButton(CoreGlobals.ButtonTextureY, this.SelectedEntryButtonTextY, ref x);
      this.DrawButton(CoreGlobals.ButtonTextureB, this.SelectedEntryButtonTextB, ref x);
      this.DrawButton(CoreGlobals.ButtonTextureLB, this.ButtonTextLB, ref x);
      this.DrawButton(CoreGlobals.ButtonTextureRB, this.ButtonTextRB, ref x);
    }

    private void DrawButton(Texture2D texture, string text, ref int x)
    {
      if (text.IsEmpty() || texture == null)
        return;
      SpriteFont itemFont = this.ItemFont;
      Vector2 vector2 = itemFont.MeasureString(text) * this.ButtonScale;
      Rectangle destinationRectangle = new Rectangle(x, (int) this.ButtonStartPosition.Y, (int) vector2.Y, (int) vector2.Y);
      this.SpriteBatch.Draw(texture, destinationRectangle, this.ColorWhite);
      float x1 = (float) (destinationRectangle.X + destinationRectangle.Width + 10);
      float y = (float) destinationRectangle.Y;
      this.SpriteBatch.DrawString(itemFont, text, new Vector2(x1 + 1f, y + 1f), this.ColorBlack, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 1f);
      this.SpriteBatch.DrawString(itemFont, text, new Vector2(x1, y), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 1f);
      x += (int) ((double) destinationRectangle.Width + 10.0 + (double) vector2.X + 40.0);
    }

    public virtual Vector2 StartPosition
    {
      get
      {
        return Vector2.Zero;
      }
    }

    public virtual Vector2 ButtonStartPosition
    {
      get
      {
        return new Vector2(this.StartPosition.X, 0.0f);
      }
    }

    protected virtual void DrawEntry(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      menuEntry.Draw(position, entryID, isSelected);
    }
  }
}
