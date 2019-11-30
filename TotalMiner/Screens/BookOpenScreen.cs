// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BookOpenScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class BookOpenScreen : MinerToolScreen
  {
    private char[] delims = new char[1]{ '_' };
    private bool editPermission;
    private int slotID;
    private BookData bookData;
    private GameInstance instance;
    private Texture2D backgroundTexture;
    private string origText;
    private string[] leftText;
    private string[] rightText;
    private Texture2D leftPic;
    private Texture2D rightPic;
    private Rectangle leftPicRect;
    private Rectangle rightPicRect;
    private bool leftPicUp;
    private bool rightPicUp;
    private bool leftPicValid;
    private bool rightPicValid;
    private string leftPageString;
    private string rightPageString;
    private bool bookChanged;
    private int currPage;
    private float pageTextScale;
    private Rectangle triggerRect;
    private IAsyncResult textResult;
    private Color colorWhite;
    private Color colorBlack;
    private Color colorBrown;

    public BookOpenScreen(GameInstance instance, Player player, BookData bookData, int slotID)
      : base(player)
    {
      this.bookData = bookData;
      this.slotID = slotID;
      this.instance = instance;
      instance.BookIDConfirmed += new GameInstance.BookIDConfirmedHandler(this.BookIDConfirmed);
      if (bookData == null)
      {
        this.ExitScreen();
      }
      else
      {
        if (bookData.Title != null && bookData.Title.Length > 0)
        {
          string key = string.Format("BookRead\\{0}", (object) bookData.Title);
          player.History.AddHistory(key);
          instance.NetworkManager.SendHistoryItem(key, player);
        }
        this.TransitionOnTime = TimeSpan.FromSeconds(0.0);
        this.currPage = 0;
        this.editPermission = player.HasPermission(Permissions.Creative) && slotID >= 0;
      }
    }

    private void BookIDConfirmed(object sender, Player player, BookData book, int slotID)
    {
      if (player != this.player || slotID != this.slotID || this.bookData.ID >= (ushort) 2)
        return;
      book.Title = this.bookData.Title;
      book.Text = this.bookData.Text;
      this.bookData = book;
      this.instance.BookIDConfirmed -= new GameInstance.BookIDConfirmedHandler(this.BookIDConfirmed);
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 957, 630);
      base.LoadContent();
      this.backgroundTexture = this.content.Load<Texture2D>("Textures\\OpenBook");
      this.triggerRect = new Rectangle(0, 0, 12, 24);
      this.ResetPageTextBuffers();
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.BookIDConfirmed -= new GameInstance.BookIDConfirmedHandler(this.BookIDConfirmed);
      if (this.bookData != null && this.bookChanged)
      {
        if (this.bookData.ID == (ushort) 0)
          this.instance.AddBookData(this.bookData, this.player, (short) this.slotID, true);
        else
          this.instance.BookChanged(this.bookData);
      }
      base.OnScreenRemovedCore();
    }

    public override bool HandleInput(InputState input)
    {
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.BookEditLeftPage))
      {
        this.EditText(this.currPage);
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.BookEditRightPage))
      {
        this.EditText(this.currPage + 1);
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp))
      {
        this.PrevPageButtonPressed();
        return true;
      }
      if (!InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown))
        return base.HandleInput(input);
      this.NextPageButtonPressed();
      return true;
    }

    private void PrevPageButtonPressed()
    {
      if (this.currPage <= 0)
        return;
      this.currPage -= 2;
      this.ResetPageTextBuffers();
      Sounds.PlaySound(Item.Book, (ITMActor) this.player, false);
    }

    private void NextPageButtonPressed()
    {
      this.currPage += 2;
      this.ResetPageTextBuffers();
      Sounds.PlaySound(Item.Book, (ITMActor) this.player, false);
    }

    private void ResetPageTextBuffers()
    {
      this.pageTextScale = 0.75f;
      this.leftText = (string[]) null;
      this.rightText = (string[]) null;
      this.leftPic = (Texture2D) null;
      this.rightPic = (Texture2D) null;
      this.leftPicValid = false;
      this.rightPicValid = false;
      if (this.bookData != null && this.bookData.Text != null && this.bookData.Text.Length > this.currPage)
      {
        this.leftText = this.ParseText(this.bookData.Text[this.currPage], out this.leftPicValid, out this.leftPic, out this.leftPicRect, out this.leftPicUp);
        if (this.bookData.Text.Length > this.currPage + 1)
          this.rightText = this.ParseText(this.bookData.Text[this.currPage + 1], out this.rightPicValid, out this.rightPic, out this.rightPicRect, out this.rightPicUp);
      }
      this.leftPageString = (this.currPage + 1).ToString();
      this.rightPageString = (this.currPage + 2).ToString();
    }

    private string[] ParseText(
      string text,
      out bool picValid,
      out Texture2D pic,
      out Rectangle picRect,
      out bool picUp)
    {
      pic = (Texture2D) null;
      picRect = new Rectangle();
      picUp = false;
      picValid = false;
      if (text != null && text.Length > 10)
      {
        int length = text.ToLower().IndexOf("[picture:");
        if (length >= 0)
        {
          int num = text.IndexOf(']', length + 9);
          int result;
          if (num >= 0 && int.TryParse(text.Substring(length + 9, num - (length + 9)), out result) && result > 0)
          {
            pic = GraphicStatics.PhotoData.LoadPhoto(result, PhotoFileType.HDThumbnail);
            picRect.X = 50;
            picRect.Y = 0;
            picRect.Width = 256;
            picRect.Height = 256;
            string str = text.Substring(0, length);
            if (num < text.Length - 1)
              str += text.Substring(num + 1);
            text = str;
            picUp = length == 0;
            picValid = true;
          }
        }
      }
      return Utils.BreakIntoLines(this.Font, 390, this.pageTextScale, Globals2.SubstituteText(text, this.instance, this.player), true, this.delims);
    }

    private void EditText(int page)
    {
      if (!this.editPermission)
        return;
      this.origText = this.bookData.Text == null || this.bookData.Text.Length <= page ? (string) null : this.bookData.Text[page];
      this.textResult = Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, page % 2 == 0 ? "Enter Left Page Text" : "Enter Right Page Text", "Use _ (underscore) to separate lines (new line).", this.origText, new AsyncCallback(this.OnTextEntered), (object) page);
    }

    private void OnTextEntered(IAsyncResult ar)
    {
      string text = Guide.EndShowKeyboardInput(ar);
      int asyncState = (int) ar.AsyncState;
      ar.AsyncWaitHandle.Close();
      if (!(text != this.origText))
        return;
      this.bookData.SetText(text, asyncState);
      this.ResetPageTextBuffers();
      this.bookChanged = true;
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
      if (this.bookData == null)
        return;
      base.DrawCore();
      float num = (float) this.TransitionAlpha / (float) byte.MaxValue;
      this.colorWhite = Color.White * num;
      this.colorBlack = Color.Black * num;
      this.colorBrown = new Color(33, 22, 12, (int) byte.MaxValue) * num;
      this.spriteBatch.BeginTM(this.Matrix);
      this.spriteBatch.Draw(this.backgroundTexture, this.screenRect, this.colorWhite);
      this.DrawPage(this.leftText, 60, 10, this.leftPageString, this.leftPic, this.leftPicRect, this.leftPicUp, this.leftPicValid);
      this.DrawPage(this.rightText, 525, 760, this.rightPageString, this.rightPic, this.rightPicRect, this.rightPicUp, this.rightPicValid);
      this.spriteBatch.DrawString(this.Font, "Turn Page", new Vector2((float) (this.screenRect.X + this.screenRect.Width - 435), (float) (this.screenRect.Y + 24)), Color.Black, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      this.triggerRect.X = this.screenRect.X + this.screenRect.Width - 456;
      this.triggerRect.Y = this.screenRect.Y + 26;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.PageUp, this.triggerRect);
      if (this.editPermission)
      {
        Rectangle rect = new Rectangle(this.screenRect.X + 157, this.screenRect.Y + 564, 28, 28);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.BookEditLeftPage, rect);
        this.spriteBatch.DrawString(this.Font, "Edit Page", new Vector2((float) (rect.X + 40), (float) (rect.Y - 1)), Color.Black, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
        rect.X += 479;
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.BookEditRightPage, rect);
        this.spriteBatch.DrawString(this.Font, "Edit Page", new Vector2((float) (rect.X + 40), (float) (rect.Y - 1)), Color.Black, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      }
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    private void DrawPage(
      string[] text,
      int textOffsetX,
      int pageOffsetX,
      string pageString,
      Texture2D pagePic,
      Rectangle pagePicRect,
      bool picUp,
      bool picValid)
    {
      this.spriteBatch.DrawString(this.Font, "Page " + pageString, new Vector2((float) (this.screenRect.X + 42 + pageOffsetX), (float) (this.screenRect.Y + 24)), Color.Black, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (text == null || text.Length <= 0)
        return;
      int num = this.screenRect.Y + 70;
      if (picUp)
        num += pagePicRect.Y + pagePicRect.Height + 30;
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] != null && text[index].Length > 0)
        {
          float x = (float) (this.screenRect.X + textOffsetX);
          this.spriteBatch.DrawString(this.Font, text[index], new Vector2(x, (float) num), this.colorBlack, 0.0f, Vector2.Zero, this.pageTextScale, SpriteEffects.None, 0.0f);
        }
        num += 35;
        if (num > this.screenRect.Y + this.screenRect.Height - 60)
          break;
      }
      if (!picValid)
        return;
      pagePicRect.X += this.screenRect.X + textOffsetX;
      if (picUp)
        pagePicRect.Y += this.screenRect.Y + 70;
      else
        pagePicRect.Y += num + 20;
      this.spriteBatch.DrawBox(CoreGlobals.BlankTexture, pagePicRect.Expand(3), 2, Color.Brown, 0.0f);
      if (pagePic != null)
      {
        this.spriteBatch.End();
        this.spriteBatch.Begin(SpriteSortMode.Texture, (BlendState) null, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, this.Matrix);
        this.spriteBatch.Draw(pagePic, pagePicRect, Color.White * 0.7f);
        this.spriteBatch.End();
        this.spriteBatch.BeginTM(this.Matrix);
      }
      else
        this.spriteBatch.DrawString(this.Font, "Picture not found", new Vector2((float) (pagePicRect.X + 10), (float) (pagePicRect.X + 5)), this.colorBlack, 0.0f, Vector2.Zero, this.pageTextScale, SpriteEffects.None, 0.0f);
    }
  }
}
