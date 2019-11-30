// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BookCoverScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class BookCoverScreen : MinerToolScreen
  {
    private bool newBook;
    private bool editPermission;
    private bool copyPermission;
    private int slotID;
    private BookData book;
    private GameInstance instance;
    private float titleScale;
    private string title;
    private string[] titleDrawText;
    private IAsyncResult textResult;
    private Color colorWhite;
    private Color colorBlack;
    private Color colorBrown;

    public BookCoverScreen(GameInstance instance, Player player, BookData book, int slotID)
      : base(player)
    {
      this.book = book;
      this.slotID = slotID;
      this.instance = instance;
      this.newBook = book == null;
      this.TransitionOnTime = TimeSpan.FromSeconds(0.0);
      this.titleScale = 1.5f;
      if (this.newBook && instance.BookCount < (int) ushort.MaxValue)
        this.book = book = new BookData();
      if (book != null)
      {
        if ((this.title = book.Title) != null)
          this.titleDrawText = this.title.Split('_');
        else
          this.titleDrawText = new string[3]
          {
            "",
            "",
            "Untitled "
          };
      }
      this.editPermission = player.HasPermission(Permissions.Creative) && book != null && slotID >= 0;
      this.copyPermission = this.editPermission || player.IsAdmin;
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 630, 630);
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      if (this.book != null && this.title != this.book.Title)
      {
        this.book.Title = this.title;
        if (this.newBook)
          this.instance.AddBookData(this.book, this.player, (short) this.slotID, true);
        else
          this.instance.BookChanged(this.book);
      }
      base.OnScreenRemovedCore();
    }

    public override bool HandleInput(InputState input)
    {
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.BookOpen))
      {
        this.ScreenManager.AddScreen((GameScreen) new BookOpenScreen(this.instance, this.player, this.book, this.slotID), this.ControllingPlayer);
        Sounds.PlaySound(Item.Book, ItemSoundType.Use);
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.BookEditTitle))
      {
        this.EditTitle();
        return true;
      }
      if (!this.copyPermission || !InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.BookCopy))
        return base.HandleInput(input);
      if (this.player.IsAdmin && this.instance.IsItemUnlocked(Item.ScriptBlock))
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Copy this book to:", "Another book", "A Script", (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.CopyToBook);
        messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.CopyToScript);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
      else
        this.CopyToBook((object) this, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
      return true;
    }

    private void EditTitle()
    {
      if (!this.editPermission)
        return;
      this.textResult = Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the Books Title", "Use _ (underscore) to separate lines (new line).", this.title, new AsyncCallback(this.OnTextEntered), (object) null);
    }

    private void CopyToBook(object sender, PlayerIndexEventArgs e)
    {
      if (!this.instance.CopyBook(this.player, this.book.ID))
        return;
      this.ExitScreen();
    }

    private void CopyToScript(object sender, PlayerIndexEventArgs e)
    {
      if (this.book == null || this.book.Text == null)
        return;
      Script script = new Script("New Script" + (this.instance.Scripts.Count + 1).ToString(), this.book.Text.Length + 1);
      this.book.WriteToScript(script.Commands);
      script.IsChanged = script.Commands.Count > 0;
      this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.player.PlayerIndex));
      this.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, true, (ScriptEditScreen) null, (Action) null), this.ControllingPlayer);
    }

    private void OnTextEntered(IAsyncResult ar)
    {
      string s = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (s == null || !(s != this.title))
        return;
      this.title = Utils.StripChars(s, 32, 160);
      if (this.title != null)
        this.titleDrawText = this.title.Split('_');
      else
        this.titleDrawText = new string[3]
        {
          "",
          "",
          "Untitled "
        };
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.book != null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("The maximum number of books allowed in one map has been reached", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    public override int FadeBackBufferAlpha
    {
      get
      {
        return base.FadeBackBufferAlpha / 2;
      }
    }

    protected override void DrawCore()
    {
      if (this.book == null)
        return;
      base.DrawCore();
      float num = (float) this.TransitionAlpha / (float) byte.MaxValue;
      this.colorWhite = Color.White * num;
      this.colorBlack = Color.Black * num;
      this.colorBrown = new Color(33, 22, 12, (int) byte.MaxValue) * num;
      this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, this.Matrix);
      this.spriteBatch.Draw(GraphicStatics.TexturePack.BlockTexture, this.screenRect, new Rectangle?(GraphicStatics.TexturePack.BlockSrcRects[121]), this.colorWhite);
      Rectangle rect = new Rectangle();
      if (this.titleDrawText != null && this.titleDrawText.Length > 0)
      {
        rect = new Rectangle(this.screenRect.X + 110, this.screenRect.Y + 100, this.screenRect.Width - 300, 0);
        for (int index = 0; index < this.titleDrawText.Length; ++index)
        {
          this.spriteBatch.DrawStringCentered(this.Font, this.titleDrawText[index], rect, Color.Black, this.titleScale);
          rect.Y += (int) (32.0 * (double) this.titleScale);
        }
      }
      rect = new Rectangle(this.screenRect.X + 170, this.screenRect.Y + 500, 24, 24);
      if (this.editPermission)
      {
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.BookEditTitle, rect);
        this.spriteBatch.DrawString(this.Font, "Edit Title", new Vector2((float) (rect.X + 40), (float) (rect.Y - 4)), Color.Black, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
        rect.Y += 30;
      }
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.BookOpen, rect);
      this.spriteBatch.DrawString(this.Font, "Open Book", new Vector2((float) (rect.X + 40), (float) (rect.Y - 4)), Color.Black, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      rect.Y += 30;
      if (this.copyPermission)
      {
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.BookCopy, rect);
        this.spriteBatch.DrawString(this.Font, "Copy Book", new Vector2((float) (rect.X + 40), (float) (rect.Y - 4)), Color.Black, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      }
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
