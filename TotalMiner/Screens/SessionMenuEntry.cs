// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SessionMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Net;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class SessionMenuEntry : BlockMenuEntry
  {
    private string gameModeText = "";
    private string ratingText = "";
    private string attribText = "";
    private string sizeText = "";
    private string playersText = "";
    public readonly IAvailableNetworkSession Session;
    private float rating;
    private int myrating;
    private bool skillsEnabled;
    private int filesize;
    private int playerCount;
    private bool gotQualityOfService;
    private double pingTime;
    private Texture2D checkon;
    private Texture2D checkoff;
    private SessionType type;
    private ServerEntry serverEntry;
    private Texture2D starTexture;
    private Texture2D favTexture;
    private Permissions defaultPermissions;
    private BlockMenuScreen screen;

    public ServerEntry ServerEntry
    {
      get
      {
        return this.serverEntry;
      }
    }

    public float Rating
    {
      get
      {
        return this.rating;
      }
    }

    public float MyRating
    {
      get
      {
        return (float) this.myrating;
      }
    }

    public int FileSize
    {
      get
      {
        return this.filesize;
      }
    }

    public bool SkillsEnabled
    {
      get
      {
        return this.skillsEnabled;
      }
    }

    public int PlayerCount
    {
      get
      {
        return this.playerCount;
      }
    }

    public string AttributeText
    {
      get
      {
        return this.attribText;
      }
    }

    public string GameModeText
    {
      get
      {
        return this.gameModeText;
      }
    }

    public SessionMenuEntry(BlockMenuScreen screen, IAvailableNetworkSession session)
      : base(screen, ((SessionProperties) session.SessionProperties).HostName)
    {
      this.screen = screen;
      this.Session = session;
      this.ColorHighlighted = Color.DarkGray;
      this.gameModeText = "Unknown";
      this.attribText = "Unknown";
      this.ratingText = "(0)";
      this.sizeText = "Unknown";
      this.playersText = "1";
      this.BuildPropertyText();
    }

    private SessionProperties sessionProperties
    {
      get
      {
        return (SessionProperties) this.Session.SessionProperties;
      }
    }

    private void BuildPropertyText()
    {
      this.type = this.sessionProperties.SessionType;
      this.gameModeText = this.sessionProperties.GameMode.ToString();
      this.rating = this.sessionProperties.RatingAvgStars;
      this.ratingText = string.Format("({0})", (object) this.sessionProperties.RatingsCount);
      this.skillsEnabled = this.sessionProperties.SkillsEnabled;
      MapAttribute attribute = this.sessionProperties.Attribute;
      this.attribText = attribute == MapAttribute.WorkInProgress ? "WIP" : attribute.ToString();
      this.defaultPermissions = this.sessionProperties.DefaultPermission;
      this.playerCount = this.sessionProperties.CurrentPlayerCount;
      this.playersText = this.playerCount.ToString();
    }

    protected override void LoadContentCore()
    {
      base.LoadContentCore();
      this.checkon = CoreGlobals.Content.Load<Texture2D>("Textures\\checkboxon");
      this.checkoff = CoreGlobals.Content.Load<Texture2D>("Textures\\checkboxoff");
      this.starTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\RatingStar");
      this.favTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\Heart");
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      Color color = this.IsEnabled ? (isSelected ? this.ColorSelected : this.ColorUnselected) : this.ColorDisabled;
      color = new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      Vector2 vector2 = this.Screen.ItemFont.MeasureString(this.Text) * this.Screen.ItemTextScale;
      Vector2 pos = position + this.TextOffset;
      pos.Y = (float) (((double) this.Height - (double) vector2.Y) / 2.0 + (double) position.Y - 1.0);
      this.DrawItem(pos, color);
      this.DrawTexture(position, color);
    }

    private void DrawItem(Vector2 pos, Color color)
    {
      float scale = 0.6f;
      pos.Y += 4f;
      if (this.type == SessionType.ShareComPack || this.type == SessionType.SharePhoto)
      {
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += 238f;
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, "  ---", pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      }
      else
      {
        Rectangle destinationRectangle;
        if (this.serverEntry.IsFavourite)
        {
          destinationRectangle = new Rectangle((int) pos.X - 24, (int) pos.Y + 4, 16, 16);
          this.Screen.SpriteBatch.Draw(this.favTexture, destinationRectangle, Color.White);
        }
        if (this.serverEntry.MapName == null)
        {
          this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }
        else
        {
          this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text.ToUpper(), pos + new Vector2(0.0f, -8f), Color.DarkKhaki, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
          this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.serverEntry.MapName.ToUpper(), pos + new Vector2(0.0f, 12f), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
        }
        pos.X += 238f;
        if (this.myrating == 0 && ((double) this.rating == 0.0 || this.ratingText == "(0)"))
        {
          this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, "   ---", pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += 90f;
        }
        else
        {
          if (this.myrating == 0)
          {
            if ((double) this.rating != 0.0)
              this.DrawRating(pos + new Vector2(0.0f, 6f), this.rating);
          }
          else
          {
            this.DrawRating(pos + new Vector2(0.0f, -4f), this.rating);
            this.DrawRating(pos + new Vector2(0.0f, 16f), (float) this.myrating);
          }
          pos.X += 88f;
          this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.ratingText, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }
        pos.X += 72f;
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.gameModeText, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += 114f;
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.attribText, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= 10f;
        if (this.type == SessionType.Play)
        {
          pos.X += 178f;
          pos.Y -= 9f;
          this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.playersText, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.Y += 9f;
          destinationRectangle = new Rectangle((int) pos.X - 26, (int) pos.Y + 19, 12, 12);
          this.Screen.SpriteBatch.Draw((this.defaultPermissions & Permissions.Adventure) == Permissions.Adventure ? this.checkon : this.checkoff, destinationRectangle, Color.White);
          destinationRectangle.X += 14;
          this.Screen.SpriteBatch.Draw((this.defaultPermissions & Permissions.Edit) == Permissions.Edit ? this.checkon : this.checkoff, destinationRectangle, Color.White);
          destinationRectangle.X += 14;
          this.Screen.SpriteBatch.Draw((this.defaultPermissions & Permissions.Creative) == Permissions.Creative ? this.checkon : this.checkoff, destinationRectangle, Color.White);
          destinationRectangle.X += 14;
          this.Screen.SpriteBatch.Draw((this.defaultPermissions & Permissions.Fly) == Permissions.Fly ? this.checkon : this.checkoff, destinationRectangle, Color.White);
          destinationRectangle.X += 14;
          this.Screen.SpriteBatch.Draw((this.defaultPermissions & Permissions.Map) == Permissions.Map ? this.checkon : this.checkoff, destinationRectangle, Color.White);
          pos.X += 70f;
          destinationRectangle = new Rectangle((int) pos.X + 1, (int) pos.Y, 24, 24);
          this.Screen.SpriteBatch.Draw(this.skillsEnabled ? this.checkon : this.checkoff, destinationRectangle, Color.White);
          pos.X += 56f;
        }
        else
          pos.X += 206f;
        pos.Y -= 6f;
        pos.X += 4f;
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.sizeText, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, scale * 0.8f, SpriteEffects.None, 0.0f);
        pos.Y += 18f;
        if (!this.gotQualityOfService)
        {
          try
          {
            QualityOfService qualityOfService = this.Session.QualityOfService;
            if (qualityOfService.IsAvailable)
            {
              this.pingTime = qualityOfService.AverageRoundtripTime.TotalMilliseconds;
              this.gotQualityOfService = true;
            }
          }
          catch (InvalidOperationException ex)
          {
          }
          catch (Exception ex)
          {
            Services.ExceptionReporter.ReportExceptionCaught(49, ex);
          }
        }
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, "Net: " + (this.gotQualityOfService ? (this.pingTime == (double) ushort.MaxValue ? "error" : string.Format("{0:0} ms", (object) this.pingTime)) : "n/a"), pos, Color.Orange, 0.0f, Vector2.Zero, scale * 0.8f, SpriteEffects.None, 0.0f);
      }
    }

    private void DrawRating(Vector2 pos, float rating)
    {
      Rectangle destinationRectangle = new Rectangle((int) pos.X, (int) pos.Y, 16, 16);
      for (int index = 1; index < 6; ++index)
      {
        this.Screen.SpriteBatch.Draw(this.starTexture, destinationRectangle, (double) index <= (double) rating ? Color.Yellow : Color.LightGray);
        if ((double) index > (double) rating && (double) (index - 1) < (double) rating)
        {
          destinationRectangle.Width = (int) (((double) rating - (double) (index - 1)) * (double) destinationRectangle.Width);
          Rectangle rectangle = new Rectangle(0, 0, (int) ((double) destinationRectangle.Width / 16.0 * (double) this.starTexture.Width), this.starTexture.Height);
          this.Screen.SpriteBatch.Draw(this.starTexture, destinationRectangle, new Rectangle?(rectangle), Color.Yellow);
          destinationRectangle.Width = 16;
        }
        destinationRectangle.X += 16;
      }
    }
  }
}
