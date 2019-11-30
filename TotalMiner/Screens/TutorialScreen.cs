// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TutorialScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Screens
{
  internal class TutorialScreen : MinerToolScreen
  {
    private int currentInstruction;
    private string[] instructions;
    private string buttonAText;
    private GameInstance instance;

    public TutorialScreen(GameInstance instance, Player player)
      : base(player)
    {
      this.instance = instance;
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.screenRect = new Rectangle(140, (this.GraphicsDevice.Viewport.Height - 450) / 2, 1000, 450);
      base.LoadContent();
      this.buttonAText = "Next";
      this.currentInstruction = 0;
      this.instructions = new string[2];
      this.instructions[0] = "Welcome to #Miner#!\r\n\r\nThis tutorial will give you an introduction to the game.\r\n\r\nThe main objective of #Miner# is to get rich by mining for valuable items like metals and gems.\r\nYou can sell the items you find at the local shop. You'll get pocket money for some items and\r\na small fortune for others.\r\n\r\nSome items are also very useful because you can use them to craft equipment. The better \r\nequipment you have, the better you'll be able to mine.\r\n\r\nValuable items are usually found deeper underground. You'll need good equipment to go deep.\r\n\r\nThere are also archealogical artifacts scattered around the map. Left there by people from a \r\ntime immemoriable, these artifacts contain wisdom and will also fetch a good price at the local\r\nmuseum.";
      this.instructions[1] = "Continuing:\r\n";
    }

    public override bool HandleInput(InputState input)
    {
      PlayerIndex playerIndex;
      if (this.currentInstruction == this.instructions.Length - 1 && input.IsMenuSelect(this.ControllingPlayer, out playerIndex) || input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
      {
        this.ExitScreen();
        return true;
      }
      if (!input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
        return base.HandleInput(input);
      ++this.currentInstruction;
      if (this.currentInstruction == this.instructions.Length - 1)
        this.buttonAText = "Close";
      return true;
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      Color color1 = Color.White * ((float) this.TransitionAlpha / (float) byte.MaxValue);
      Color fillColor = Color.Black * (float) ((double) this.TransitionAlpha / (double) byte.MaxValue * 0.800000011920929);
      Color color2 = Color.Gray * ((float) this.TransitionAlpha / (float) byte.MaxValue);
      this.SpriteBatch.Begin();
      this.SpriteBatch.DrawRoundedFilledBox(this.screenRect, 2, color1, fillColor);
      int x = this.screenRect.X + 30;
      int num = this.screenRect.Y + this.screenRect.Height - 44;
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.screenRect.X + 2, num - 4, this.screenRect.Width - 4, 1), color1);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.screenRect.X + 2, num - 3, this.screenRect.Width - 4, 1), color2);
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureA, new Rectangle(x, num + 7, 24, 24), color1);
      this.SpriteBatch.DrawString(this.Font, this.buttonAText, new Vector2((float) (x + 35), (float) (num + 9)), color1, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
      this.SpriteBatch.End();
      Vector2 position = new Vector2((float) (this.screenRect.X + 30), (float) (this.screenRect.Y + 12));
      this.SpriteBatch.Begin();
      this.SpriteBatch.DrawString(this.Font, this.instructions[this.currentInstruction], position, color1, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
      this.SpriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
