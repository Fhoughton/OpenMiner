// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.InputState
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StudioForge.Engine.Core
{
  public class InputState
  {
    private Buttons[] lastButtonPressed = new Buttons[1];
    private float[] lastButtonPressTimer = new float[1];
    private Keys[] lastKeyPressed = new Keys[1];
    private float[] lastKeyPressTimer = new float[1];
    public const int MaxInputs = 1;
    public readonly KeyboardState[] CurrentKeyboardStates;
    public readonly GamePadState[] CurrentGamePadStates;
    public readonly MouseState[] CurrentMouseStates;
    public readonly KeyboardState[] LastKeyboardStates;
    public readonly GamePadState[] LastGamePadStates;
    public readonly MouseState[] LastMouseStates;
    public readonly bool[] GamePadWasConnected;
    private double elapsedTime;
    private PlayerIndex lastPlayerIndex;

    public PlayerIndex LastPlayerIndex
    {
      get
      {
        return this.lastPlayerIndex;
      }
    }

    public InputState()
    {
      this.CurrentKeyboardStates = new KeyboardState[1];
      this.CurrentGamePadStates = new GamePadState[1];
      this.CurrentMouseStates = new MouseState[1];
      this.LastKeyboardStates = new KeyboardState[1];
      this.LastGamePadStates = new GamePadState[1];
      this.LastMouseStates = new MouseState[1];
      this.GamePadWasConnected = new bool[1];
      this.OverrideEscape = false;
    }

    public void Update(float elapsedTime)
    {
      this.elapsedTime = (double) elapsedTime;
      for (int index = 0; index < 1; ++index)
      {
        this.LastKeyboardStates[index] = this.CurrentKeyboardStates[index];
        this.LastGamePadStates[index] = this.CurrentGamePadStates[index];
        this.LastMouseStates[index] = this.CurrentMouseStates[index];
        this.CurrentKeyboardStates[index] = Keyboard.GetState((PlayerIndex) index);
        this.CurrentGamePadStates[index] = GamePad.GetState((PlayerIndex) index);
        this.CurrentMouseStates[index] = Mouse.GetState();
        if (this.CurrentGamePadStates[index].IsConnected)
          this.GamePadWasConnected[index] = true;
        if (this.CurrentGamePadStates[index].IsButtonUp(this.lastButtonPressed[index]))
          this.lastButtonPressTimer[index] = 0.75f;
      }
    }

    public bool IsNewKeyPress(
      Keys key,
      PlayerIndex? controllingPlayer,
      out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      bool flag1;
      if (controllingPlayer.HasValue)
      {
        playerIndex = controllingPlayer.Value;
        int index = (int) playerIndex;
        bool flag2 = this.CurrentKeyboardStates[index].IsKeyDown(key);
        bool flag3 = false;
        if (flag2)
        {
          if (key != this.lastKeyPressed[index])
          {
            this.lastKeyPressTimer[index] = 0.75f;
            this.lastKeyPressed[index] = key;
          }
          else
          {
            this.lastKeyPressTimer[index] -= Services.ElapsedTime;
            if ((double) this.lastKeyPressTimer[index] <= 0.0)
            {
              this.lastKeyPressTimer[index] = 0.1f;
              flag3 = true;
            }
          }
          if (key == Keys.Enter || key == Keys.Space)
            flag3 = false;
        }
        flag1 = flag2 && (this.LastKeyboardStates[index].IsKeyUp(key) || flag3);
      }
      else
        flag1 = this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.One), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Two), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Three), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      return flag1;
    }

    public bool IsNewKeyPress(Keys key)
    {
      return this.IsNewKeyPress(key, new PlayerIndex?(), out this.lastPlayerIndex);
    }

    public bool IsKeyPressed(Keys key)
    {
      return this.IsKeyPressed(key, new PlayerIndex?(PlayerIndex.One), out this.lastPlayerIndex);
    }

    public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (controllingPlayer.HasValue)
      {
        playerIndex = controllingPlayer.Value;
        return this.CurrentKeyboardStates[(int) playerIndex].IsKeyDown(key);
      }
      if (!this.IsKeyPressed(key, new PlayerIndex?(PlayerIndex.One), out playerIndex) && !this.IsKeyPressed(key, new PlayerIndex?(PlayerIndex.Two), out playerIndex) && !this.IsKeyPressed(key, new PlayerIndex?(PlayerIndex.Three), out playerIndex))
        return this.IsKeyPressed(key, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      return true;
    }

    private bool IsRepeatAllowed(Buttons button)
    {
      switch (button)
      {
        case Buttons.DPadUp:
        case Buttons.DPadDown:
        case Buttons.DPadLeft:
        case Buttons.DPadRight:
        case Buttons.LeftThumbstickLeft:
        case Buttons.RightThumbstickUp:
        case Buttons.RightThumbstickDown:
        case Buttons.RightThumbstickRight:
        case Buttons.RightThumbstickLeft:
        case Buttons.LeftThumbstickUp:
        case Buttons.LeftThumbstickDown:
        case Buttons.LeftThumbstickRight:
          return true;
        default:
          return false;
      }
    }

    public bool IsNewKeyRelease(
      Keys key,
      PlayerIndex? controllingPlayer,
      out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (controllingPlayer.HasValue)
      {
        playerIndex = controllingPlayer.Value;
        int index = (int) playerIndex;
        if (this.CurrentKeyboardStates[index].IsKeyUp(key))
          return this.LastKeyboardStates[index].IsKeyDown(key);
        return false;
      }
      if (!this.IsNewKeyRelease(key, new PlayerIndex?(PlayerIndex.One), out playerIndex) && !this.IsNewKeyRelease(key, new PlayerIndex?(PlayerIndex.Two), out playerIndex) && !this.IsNewKeyRelease(key, new PlayerIndex?(PlayerIndex.Three), out playerIndex))
        return this.IsNewKeyRelease(key, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      return true;
    }

    public bool IsNewButtonPress(
      Buttons button,
      PlayerIndex? controllingPlayer,
      out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      bool flag1;
      if (controllingPlayer.HasValue)
      {
        playerIndex = controllingPlayer.Value;
        int index = (int) playerIndex;
        bool flag2 = this.CurrentGamePadStates[index].IsButtonDown(button);
        bool flag3 = false;
        if (flag2 && this.IsRepeatAllowed(button))
        {
          if (button != this.lastButtonPressed[index])
          {
            this.lastButtonPressTimer[index] = 0.75f;
            this.lastButtonPressed[index] = button;
          }
          else
          {
            this.lastButtonPressTimer[index] -= Services.ElapsedTime;
            if ((double) this.lastButtonPressTimer[index] <= 0.0)
            {
              this.lastButtonPressTimer[index] = 0.1f;
              flag3 = true;
            }
          }
        }
        flag1 = flag2 && (this.LastGamePadStates[index].IsButtonUp(button) || flag3);
      }
      else
        flag1 = this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.One), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Two), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Three), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      return flag1;
    }

    public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
    {
      return this.IsNewButtonPress(button, new PlayerIndex?(playerIndex), out this.lastPlayerIndex);
    }

    public bool IsButtonDown(Buttons button, PlayerIndex playerIndex)
    {
      return this.CurrentGamePadStates[(int) playerIndex].IsButtonDown(button);
    }

    public bool IsButtonUp(Buttons button, PlayerIndex playerIndex)
    {
      return this.CurrentGamePadStates[(int) playerIndex].IsButtonUp(button);
    }

    public bool IsGamepadConnect(PlayerIndex playerIndex)
    {
      return this.CurrentGamePadStates[(int) playerIndex].IsConnected;
    }

    public bool IsNewButtonPress(Buttons button)
    {
      return this.IsNewButtonPress(button, new PlayerIndex?(), out this.lastPlayerIndex);
    }

    public bool IsNewButtonRelease(
      Buttons button,
      PlayerIndex? controllingPlayer,
      out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (controllingPlayer.HasValue)
      {
        playerIndex = controllingPlayer.Value;
        int index = (int) playerIndex;
        if (this.CurrentGamePadStates[index].IsButtonUp(button))
          return this.LastGamePadStates[index].IsButtonDown(button);
        return false;
      }
      if (!this.IsNewButtonRelease(button, new PlayerIndex?(PlayerIndex.One), out playerIndex) && !this.IsNewButtonRelease(button, new PlayerIndex?(PlayerIndex.Two), out playerIndex) && !this.IsNewButtonRelease(button, new PlayerIndex?(PlayerIndex.Three), out playerIndex))
        return this.IsNewButtonRelease(button, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      return true;
    }

    public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyRelease(Keys.Space, controllingPlayer, out playerIndex) && !this.IsNewKeyRelease(Keys.Enter, controllingPlayer, out playerIndex) && (!this.IsNewButtonRelease(Buttons.A, controllingPlayer, out playerIndex) && !this.IsNewButtonRelease(Buttons.Start, controllingPlayer, out playerIndex)))
        return InputManager.IsMouseButtonReleasedNew(controllingPlayer.Value, StudioForge.Engine.Integration.MouseButtons.LeftButton);
      return true;
    }

    public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if ((this.OverrideEscape || !this.IsNewKeyRelease(Keys.Escape, controllingPlayer, out playerIndex)) && (!this.IsNewButtonRelease(Buttons.B, controllingPlayer, out playerIndex) && !this.IsNewButtonRelease(Buttons.Back, controllingPlayer, out playerIndex)))
        return InputManager.IsMouseButtonReleasedNew(controllingPlayer.Value, StudioForge.Engine.Integration.MouseButtons.RightButton);
      return true;
    }

    public bool IsMenuXButton(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyPress(Keys.X, controllingPlayer, out playerIndex))
        return this.IsNewButtonPress(Buttons.X, controllingPlayer, out playerIndex);
      return true;
    }

    public bool IsMenuYButton(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyPress(Keys.Y, controllingPlayer, out playerIndex))
        return this.IsNewButtonPress(Buttons.Y, controllingPlayer, out playerIndex);
      return true;
    }

    public bool IsMenuUp(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyPress(Keys.Up, controllingPlayer, out this.lastPlayerIndex) && !this.IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out this.lastPlayerIndex))
        return this.IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out this.lastPlayerIndex);
      return true;
    }

    public bool IsMenuDown(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyPress(Keys.Down, controllingPlayer, out this.lastPlayerIndex) && !this.IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out this.lastPlayerIndex))
        return this.IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out this.lastPlayerIndex);
      return true;
    }

    public bool IsMenuLeft(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyPress(Keys.Left, controllingPlayer, out this.lastPlayerIndex) && (!this.IsKeyPressed(Keys.Left, controllingPlayer, out this.lastPlayerIndex) || !this.IsKeyPressed(Keys.LeftShift, controllingPlayer, out this.lastPlayerIndex)))
        return this.IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out this.lastPlayerIndex);
      return true;
    }

    public bool IsMenuRight(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (!this.IsNewKeyPress(Keys.Right, controllingPlayer, out this.lastPlayerIndex) && (!this.IsKeyPressed(Keys.Right, controllingPlayer, out this.lastPlayerIndex) || !this.IsKeyPressed(Keys.LeftShift, controllingPlayer, out this.lastPlayerIndex)))
        return this.IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out this.lastPlayerIndex);
      return true;
    }

    public bool IsPauseGame(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
        controllingPlayer = new PlayerIndex?(PlayerIndex.One);
      if (this.OverrideEscape || !this.IsNewKeyPress(Keys.Escape, controllingPlayer, out this.lastPlayerIndex))
        return this.IsNewButtonPress(Buttons.Start, controllingPlayer, out this.lastPlayerIndex);
      return true;
    }

    public bool IsPauseGame(out PlayerIndex outlastPlayerIndex)
    {
      bool flag = !this.OverrideEscape && this.IsNewKeyPress(Keys.Escape, new PlayerIndex?(), out this.lastPlayerIndex) || this.IsNewButtonPress(Buttons.Start, new PlayerIndex?(), out this.lastPlayerIndex);
      outlastPlayerIndex = this.lastPlayerIndex;
      return flag;
    }

    public bool OverrideEscape { get; set; }
  }
}
