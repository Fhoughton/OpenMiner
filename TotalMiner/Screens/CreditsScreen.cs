// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreditsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreditsScreen : GameScreen
  {
    private static CreditsScreen.Credit[] Credits = new CreditsScreen.Credit[123]
    {
      new CreditsScreen.Credit(1.25f, Color.DarkGreen, "Studio Forge Presents:"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1.5f, Color.White, "Total Miner"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.DarkGreen, "Update Version 2.6 - PC Initial"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Yellow, "Craig Martin"),
      new CreditsScreen.Credit(1f, Color.White, "Game Design"),
      new CreditsScreen.Credit(1f, Color.White, "Programming"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Yellow, "Bob"),
      new CreditsScreen.Credit(1f, Color.White, "Forum Administrator:"),
      new CreditsScreen.Credit(1f, Color.White, "www.totalminer.org / www.totalminerforums.net"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Yellow, "Stephen W H Martin | Martindoolittle"),
      new CreditsScreen.Credit(1f, Color.White, "Graphics Artist"),
      new CreditsScreen.Credit(1f, Color.White, "Avatar Modelling"),
      new CreditsScreen.Credit(1f, Color.White, "System HD Texture Packs"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Yellow, "Steven Wilson | Baldygargoyle"),
      new CreditsScreen.Credit(1f, Color.White, "System Worlds"),
      new CreditsScreen.Credit(1f, Color.White, "System Component Packs"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Green, "Unique Content Contributors:"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Gr1mT1m3Z - Rupture HD Texture Pack"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Miss Cellany - Sound Effect Refresh"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Derfen Steve - Steam Greenlight Promotion Video"),
      new CreditsScreen.Credit(1f, Color.Yellow, "TM Clerical - Testing"),
      new CreditsScreen.Credit(1f, Color.Yellow, "MechaWho - Testing"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Green, "Community Content Contributors:"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Miss Cellany - System Terrain Components"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Builders Unit - System Tree Components"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Ugh ThatZ NaZty - Title screen Photography"),
      new CreditsScreen.Credit(1f, Color.Yellow, "Derfen Steve - Hermes Wraith Avatar"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, "Our Website: www.totalminerforums.net"),
      new CreditsScreen.Credit(0.75f, Color.Gray, "Keep up to date with future updates, gameplay help,"),
      new CreditsScreen.Credit(0.75f, Color.Gray, "community discussions and feedback."),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, "www.youtube.com/TotalMiner"),
      new CreditsScreen.Credit(1f, Color.White, "www.facebook.com/TeamTotalMiner"),
      new CreditsScreen.Credit(1f, Color.White, "email: TotalMiner@gmail.com"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Cyan, "Twitter: @TotalMiner"),
      new CreditsScreen.Credit(1f, Color.White, "twitpic.com/photos/TotalMiner"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Gray, "Original Music"),
      new CreditsScreen.Credit(1f, Color.White, "Kevin MacLeod (incompetech.com)"),
      new CreditsScreen.Credit(1f, Color.White, "Licensed under Creative Commons: By Attribution 3.0"),
      new CreditsScreen.Credit(1f, Color.White, "http://creativecommons.org/licenses/by/3.0/"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Gray, "Wisdom Quotes"),
      new CreditsScreen.Credit(1f, Color.White, "Joseph Campbell. www.jcf.org"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Green, "Inspiration:"),
      new CreditsScreen.Credit(0.9f, Color.White, "Minecraft | PC Indie"),
      new CreditsScreen.Credit(0.9f, Color.White, "Miner Dig Deep | XBLIG"),
      new CreditsScreen.Credit(0.9f, Color.White, "The Total Miner Community"),
      new CreditsScreen.Credit(0.9f, Color.White, "Love of sandbox games"),
      new CreditsScreen.Credit(0.9f, Color.White, "Technical interest"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Green, "Special thanks to:"),
      new CreditsScreen.Credit(1f, Color.White, "Matthew Silber | Tridus - Total Miner Logos"),
      new CreditsScreen.Credit(1f, Color.Yellow, "All the community members who submitted Title screen images"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, "And a warm thanks to all our great community"),
      new CreditsScreen.Credit(1f, Color.White, "and forum members who are helping us with ideas"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Yellow, "Previously: Andrew Swanger | Durandal"),
      new CreditsScreen.Credit(1f, Color.White, "Media Designer"),
      new CreditsScreen.Credit(1f, Color.White, "Public Relations"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.Yellow, "Xbox 360 Testers"),
      new CreditsScreen.Credit(1f, Color.Gray, "Thankyou all so much for your help and friendship"),
      new CreditsScreen.Credit(1f, Color.White, "Adam D Coxon | TM Clerical"),
      new CreditsScreen.Credit(1f, Color.White, "MechaWho"),
      new CreditsScreen.Credit(1f, Color.White, "v Zoidberg v"),
      new CreditsScreen.Credit(1f, Color.White, "Las3rShark | Conmaan"),
      new CreditsScreen.Credit(1f, Color.White, "Brandon | Get Inn The Jet"),
      new CreditsScreen.Credit(1f, Color.White, "Kitty | Milkshakez7z"),
      new CreditsScreen.Credit(1f, Color.White, "Miss Cellany"),
      new CreditsScreen.Credit(1f, Color.White, "Josh | Jack Of Shades"),
      new CreditsScreen.Credit(1f, Color.White, "Jared Kuse | Jeff McNewmen"),
      new CreditsScreen.Credit(1f, Color.White, "Agykoo"),
      new CreditsScreen.Credit(1f, Color.White, "Vincent Tulino | Zz Klann zZ"),
      new CreditsScreen.Credit(1f, Color.White, "Kane Robson | SwiftGeneration"),
      new CreditsScreen.Credit(1f, Color.White, "G.Aston | muDsluG i1 | muD"),
      new CreditsScreen.Credit(1f, Color.White, "Quadraphinic"),
      new CreditsScreen.Credit(1f, Color.White, "Michial \"YoungSykotic182\" Hamilton"),
      new CreditsScreen.Credit(1f, Color.White, "X B1ack W1dow X | Mason Turpin"),
      new CreditsScreen.Credit(1f, Color.White, "Steven Wilson | Baldygargoyle"),
      new CreditsScreen.Credit(1f, Color.White, "Brent Marvich | Acewingman"),
      new CreditsScreen.Credit(1f, Color.White, "Derfen Steve | John Phillips"),
      new CreditsScreen.Credit(1f, Color.White, "Mike Eves | Evesyy"),
      new CreditsScreen.Credit(1f, Color.White, "Charles Marshfield"),
      new CreditsScreen.Credit(1f, Color.White, "Nathan | Auroras Calling"),
      new CreditsScreen.Credit(1f, Color.White, "Sean | LXY Duckboy"),
      new CreditsScreen.Credit(1f, Color.White, "Rorthic"),
      new CreditsScreen.Credit(1f, Color.White, "Ian Williams | DieHardPunisher"),
      new CreditsScreen.Credit(1f, Color.White, "David F. | ACelticYankee9"),
      new CreditsScreen.Credit(1f, Color.White, "George | Magik Chicken"),
      new CreditsScreen.Credit(1f, Color.White, "Invisis | invicten"),
      new CreditsScreen.Credit(1f, Color.White, "Noble Productions | killjoy johnboy"),
      new CreditsScreen.Credit(1f, Color.White, "NightFall | PsychoVisionz"),
      new CreditsScreen.Credit(1f, Color.White, "Dan Steer"),
      new CreditsScreen.Credit(1f, Color.White, ""),
      new CreditsScreen.Credit(1f, Color.White, "...")
    };
    private SpriteBatchSafe spriteBatch;
    private SpriteFont font;
    private float elapsed;
    private bool pauseScroll;
    private Cue cue;
    private Cue backCue;
    private float volume;

    public override void LoadContent()
    {
      base.LoadContent();
      this.spriteBatch = CoreGlobals.SpriteBatch;
      this.font = CoreGlobals.GameFont;
      this.TransitionOffTime = this.TransitionOnTime = TimeSpan.FromSeconds(2.0);
      this.backCue = CoreGlobals.AudioManager.CurrentCue;
      this.volume = CoreGlobals.AudioManager.MusicVolume;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.pauseScroll)
        this.elapsed += Services.ElapsedTime;
      if (this.ScreenState == ScreenState.TransitionOn)
        CoreGlobals.AudioManager.MusicVolume = (1f - this.TransitionPosition) * this.volume;
      else if (this.ScreenState == ScreenState.Active)
      {
        if (this.cue != null)
          return;
        string asset = "Supernatural";
        CoreGlobals.AudioManager.MusicVolume = this.volume;
        CoreGlobals.AudioManager.PlaySong(asset, out this.cue);
      }
      else
      {
        if (this.ScreenState != ScreenState.TransitionOff || this.cue == null || !this.cue.IsPlaying)
          return;
        CoreGlobals.AudioManager.MusicVolume = this.TransitionPosition * this.volume;
      }
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.cue != null && !this.cue.IsDisposed)
        this.cue.Stop(AudioStopOptions.AsAuthored);
      CoreGlobals.AudioManager.MusicVolume = this.volume;
    }

    public override bool HandleInput(InputState input)
    {
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      this.pauseScroll = false;
      if (!InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.SelectItem))
        return base.HandleInput(input);
      this.pauseScroll = true;
      return true;
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      this.spriteBatch.Begin();
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, this.GraphicsDevice.Viewport.Bounds, Color.Black * this.TransitionAlphaFloat * 0.7f);
      this.spriteBatch.End();
      this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
      for (int index = 0; index < CreditsScreen.Credits.Length; ++index)
      {
        float num = (double) this.elapsed > 4.0 ? (float) (((double) this.elapsed - 4.0) * 60.0) : 0.0f;
        int y = 200 + index * 35 - (int) num;
        this.DrawString(CreditsScreen.Credits[index], y);
        if (index == CreditsScreen.Credits.Length - 1 && y < 40)
          this.ExitScreen();
      }
      this.spriteBatch.End();
    }

    private void DrawString(CreditsScreen.Credit credit, int y)
    {
      Vector2 vector2 = this.CenterText(credit.Text, y, credit.Scale);
      this.DrawString(credit.Text, credit.Color, (int) vector2.X, (int) vector2.Y, credit.Scale);
    }

    private void DrawString(string text, Color color, int x, int y, float scale)
    {
      float num1 = 100f;
      float num2 = 600f;
      if ((double) y <= (double) num1 || (double) y >= (double) num2)
        return;
      float num3 = (float) this.TransitionAlpha / (float) byte.MaxValue;
      if ((double) y < (double) num1 + 70.0)
        num3 *= (float) (((double) y - (double) num1) / 70.0);
      else if ((double) y > (double) num2 - 70.0)
        num3 *= (float) (((double) num2 - (double) y) / 70.0);
      this.DrawString(text, x, y, scale, color * num3);
    }

    private void DrawString(string text, int x, int y, float scale, Color color)
    {
      this.spriteBatch.DrawString(this.font, text, new Vector2((float) x, (float) y) + Vector2.One, Color.Black * (float) color.A, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      this.spriteBatch.DrawString(this.font, text, new Vector2((float) x, (float) y), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
    }

    private Vector2 CenterText(string text, int y, float scale)
    {
      return new Vector2((float) (((double) CoreGlobals.GraphicsDevice.Viewport.Width - (double) (this.font.MeasureString(text) * scale).X) / 2.0), (float) y);
    }

    private struct Credit
    {
      public Color Color;
      public float Scale;
      public string Text;

      public Credit(float scale, Color color, string text)
      {
        this.Scale = scale;
        this.Color = color;
        this.Text = text;
      }
    }
  }
}
