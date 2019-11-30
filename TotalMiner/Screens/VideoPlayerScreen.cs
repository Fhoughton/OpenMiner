// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.VideoPlayerScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner.Screens
{
  internal class VideoPlayerScreen : MinerToolScreen
  {
    private string videoAsset;
    private Video video;
    private VideoPlayer videoPlayer;
    private Texture2D videoTexture;
    private bool started;

    public VideoPlayerScreen(Player player, string videoAsset)
      : base(player)
    {
      this.videoAsset = videoAsset;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.content = (IContentManager) new ContentManager(CoreGlobals.Content.ServiceProvider, "Content");
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.Font = CoreGlobals.GameFont;
      this.video = this.content.Load<Video>(this.videoAsset);
      this.videoPlayer = new VideoPlayer();
      this.videoPlayer.IsLooped = false;
      this.screenRect = MyExtensions.CenterOfViewport(this.video.Width, this.video.Height);
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        if (this.videoPlayer.State != MediaState.Stopped)
          this.videoPlayer.Stop();
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
        return true;
      }
      if (this.videoPlayer.State != MediaState.Stopped && InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.SelectItem))
      {
        if (this.videoPlayer.State == MediaState.Paused)
          this.videoPlayer.Resume();
        else
          this.videoPlayer.Pause();
        return true;
      }
      if (currentGamePadState.Buttons.X != ButtonState.Pressed || lastGamePadState.Buttons.X != ButtonState.Released)
        return base.HandleInput(input);
      if (this.videoPlayer.State != MediaState.Stopped)
        this.videoPlayer.Stop();
      this.started = false;
      return true;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.started || this.videoPlayer.State != MediaState.Stopped)
        return;
      this.videoPlayer.Play(this.video);
      this.started = true;
    }

    protected override void DrawCore()
    {
      this.spriteBatch.Begin();
      if (this.videoPlayer.State != MediaState.Stopped)
        this.videoTexture = this.videoPlayer.GetTexture();
      if (this.videoTexture != null)
      {
        this.spriteBatch.Begin();
        this.spriteBatch.Draw(this.videoTexture, this.screenRect, Color.White);
        this.spriteBatch.Draw(CoreGlobals.ButtonTextureA, new Rectangle(this.screenRect.X + 20, this.screenRect.Y + this.screenRect.Height - 42, 24, 24), Color.White);
        this.spriteBatch.DrawString(this.Font, this.videoPlayer.State == MediaState.Paused ? "Resume" : "Pause", new Vector2((float) (this.screenRect.X + 50), (float) (this.screenRect.Y + this.screenRect.Height - 40)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
        this.spriteBatch.Draw(CoreGlobals.ButtonTextureX, new Rectangle(this.screenRect.X + 300, this.screenRect.Y + this.screenRect.Height - 42, 24, 24), Color.White);
        this.spriteBatch.DrawString(this.Font, "Replay", new Vector2((float) (this.screenRect.X + 330), (float) (this.screenRect.Y + this.screenRect.Height - 40)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
        this.spriteBatch.Draw(CoreGlobals.ButtonTextureB, new Rectangle(this.screenRect.X + this.screenRect.Width - 120, this.screenRect.Y + this.screenRect.Height - 42, 24, 24), Color.White);
        this.spriteBatch.DrawString(this.Font, "Close", new Vector2((float) (this.screenRect.X + this.screenRect.Width - 90), (float) (this.screenRect.Y + this.screenRect.Height - 40)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
        this.spriteBatch.End();
      }
      this.spriteBatch.End();
    }
  }
}
