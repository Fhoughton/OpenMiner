// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.InputManager
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public static class InputManager
  {
    public static bool IsUsingGamePad;
    public static Texture2D KeysTexture;
    private static PlayerIndex playerIndex;
    private static MouseState mouse;
    private static MouseState prevMouse;
    private static KeyboardState keyboard;
    private static KeyboardState prevKeyboard;
    private static GamePadState gamePad;
    private static GamePadState prevGamePad;
    private static Point mousePosition;
    private static Vector2 mousePositionVec;
    private static Vector2 mousePositionDelta;
    private static Vector2 mousePositionDeltaSmoothed;
    private static Vector2[] mouseSmoothed;
    private static int mouseDiffZeroCount;
    private static int mouseSmoothIndex;
    private static int newScrollWheelPress;
    private static Dictionary<ushort, InputItem> scheme;
    private static int useVirtualMouse;

    public static Dictionary<ushort, InputItem> Scheme
    {
      get
      {
        return InputManager.scheme;
      }
    }

    public static bool UseVirtualMouse
    {
      get
      {
        return InputManager.useVirtualMouse > 0;
      }
    }

    public static void PushVirtualMouse()
    {
      ++InputManager.useVirtualMouse;
    }

    public static void PopVirtualMouse()
    {
      if (--InputManager.useVirtualMouse >= 0)
        return;
      InputManager.useVirtualMouse = 0;
    }

    public static void Initialize(Dictionary<ushort, InputItem> inputScheme, int mouseSmoothCount)
    {
      InputManager.scheme = inputScheme;
      InputManager.mouseSmoothed = mouseSmoothCount > 0 ? new Vector2[mouseSmoothCount] : (Vector2[]) null;
      InputManager.mouseDiffZeroCount = 2147483646;
    }

    public static void ResetInputs()
    {
      InputManager.prevMouse = InputManager.mouse;
      InputManager.prevKeyboard = InputManager.keyboard;
      InputManager.prevGamePad = InputManager.gamePad;
    }

    public static void SetPlayerIndex(PlayerIndex index)
    {
      InputManager.playerIndex = index;
    }

    public static InputItem GetInputItem(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      InputManager.scheme.TryGetValue(key, out inputItem);
      return inputItem;
    }

    public static bool IsInputPressed(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      if (InputManager.scheme.TryGetValue(key, out inputItem))
      {
        if (inputItem.Button != (Buttons) 0 && InputManager.IsButtonPressed(playerIndex, inputItem.Button))
          return true;
        if (inputItem.MouseButton != StudioForge.Engine.Integration.MouseButtons.None && InputManager.IsMouseButtonPressed(playerIndex, inputItem.MouseButton))
          return (inputItem.MouseAlt ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftAlt) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightAlt) ? 1 : 0)) && (inputItem.MouseCtrl ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftControl) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightControl) ? 1 : 0)) && (inputItem.MouseShift ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftShift) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightShift) ? 1 : 0));
        if (inputItem.Key != Keys.None && InputManager.IsKeyPressed(playerIndex, inputItem.Key) && (!inputItem.KeyAlt || !InputManager.IsKeyReleased(playerIndex, Keys.LeftAlt) || !InputManager.IsKeyReleased(playerIndex, Keys.RightAlt)) && ((!inputItem.KeyCtrl || !InputManager.IsKeyReleased(playerIndex, Keys.LeftControl) || !InputManager.IsKeyReleased(playerIndex, Keys.RightControl)) && (!inputItem.KeyShift || !InputManager.IsKeyReleased(playerIndex, Keys.LeftShift) || !InputManager.IsKeyReleased(playerIndex, Keys.RightShift))))
          return true;
      }
      return false;
    }

    public static bool IsInputPressedNew(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      if (InputManager.scheme.TryGetValue(key, out inputItem))
      {
        if (inputItem.Button != (Buttons) 0 && InputManager.IsButtonPressedNew(playerIndex, inputItem.Button))
          return true;
        if (inputItem.MouseButton != StudioForge.Engine.Integration.MouseButtons.None && InputManager.IsMouseButtonPressedNew(playerIndex, inputItem.MouseButton))
          return (inputItem.MouseAlt ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftAlt) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightAlt) ? 1 : 0)) && (inputItem.MouseCtrl ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftControl) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightControl) ? 1 : 0)) && (inputItem.MouseShift ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftShift) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightShift) ? 1 : 0));
        if (inputItem.Key != Keys.None && InputManager.IsKeyPressedNew(playerIndex, inputItem.Key) && (!inputItem.KeyAlt || !InputManager.IsKeyReleased(playerIndex, Keys.LeftAlt) || !InputManager.IsKeyReleased(playerIndex, Keys.RightAlt)) && ((!inputItem.KeyCtrl || !InputManager.IsKeyReleased(playerIndex, Keys.LeftControl) || !InputManager.IsKeyReleased(playerIndex, Keys.RightControl)) && (!inputItem.KeyShift || !InputManager.IsKeyReleased(playerIndex, Keys.LeftShift) || !InputManager.IsKeyReleased(playerIndex, Keys.RightShift))))
          return true;
      }
      return false;
    }

    public static bool IsInputReleased(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      return InputManager.scheme.TryGetValue(key, out inputItem) && (inputItem.Button != (Buttons) 0 && InputManager.IsButtonReleased(playerIndex, inputItem.Button) || inputItem.MouseButton != StudioForge.Engine.Integration.MouseButtons.None && !InputManager.IsMouseButtonPressed(playerIndex, inputItem.MouseButton) || inputItem.Key != Keys.None && InputManager.IsKeyReleased(playerIndex, inputItem.Key));
    }

    public static bool IsInputReleasedNew(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      if (InputManager.scheme.TryGetValue(key, out inputItem))
      {
        if (inputItem.Button != (Buttons) 0 && InputManager.IsButtonReleasedNew(playerIndex, inputItem.Button))
          return true;
        if (inputItem.MouseButton != StudioForge.Engine.Integration.MouseButtons.None && InputManager.IsMouseButtonReleasedNew(playerIndex, inputItem.MouseButton))
          return (inputItem.MouseAlt ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftAlt) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightAlt) ? 1 : 0)) && (inputItem.MouseCtrl ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftControl) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightControl) ? 1 : 0)) && (inputItem.MouseShift ? 1 : 0) == (InputManager.IsKeyPressed(playerIndex, Keys.LeftShift) ? 1 : (InputManager.IsKeyPressed(playerIndex, Keys.RightShift) ? 1 : 0));
        if (inputItem.Key != Keys.None && InputManager.IsKeyReleasedNew(playerIndex, inputItem.Key) && (!inputItem.KeyAlt || !InputManager.IsKeyReleased(playerIndex, Keys.LeftAlt) || !InputManager.IsKeyReleased(playerIndex, Keys.RightAlt)) && ((!inputItem.KeyCtrl || !InputManager.IsKeyReleased(playerIndex, Keys.LeftControl) || !InputManager.IsKeyReleased(playerIndex, Keys.RightControl)) && (!inputItem.KeyShift || !InputManager.IsKeyReleased(playerIndex, Keys.LeftShift) || !InputManager.IsKeyReleased(playerIndex, Keys.RightShift))))
          return true;
      }
      return false;
    }

    public static bool IsInputChanged(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      return InputManager.scheme.TryGetValue(key, out inputItem) && (inputItem.Button != (Buttons) 0 && InputManager.IsButtonChanged(playerIndex, inputItem.Button) || inputItem.MouseButton != StudioForge.Engine.Integration.MouseButtons.None && InputManager.IsMouseButtonChanged(playerIndex, inputItem.MouseButton) || inputItem.Key != Keys.None && InputManager.IsKeyChanged(playerIndex, inputItem.Key));
    }

    public static void SetMousePos(int x, int y)
    {
      InputManager.mousePosition.X = x;
      InputManager.mousePosition.Y = y;
      Mouse.SetPosition(x, y);
    }

    public static void GetMouseMouseState(
      PlayerIndex playerIndex,
      out MouseState state,
      out MouseState prevState)
    {
      state = InputManager.mouse;
      prevState = InputManager.prevMouse;
    }

    public static Point GetMousePos(PlayerIndex playerIndex)
    {
      return InputManager.mousePosition;
    }

    public static Point GetMousePos(PlayerIndex? playerIndex)
    {
      return InputManager.mousePosition;
    }

    public static Vector2 GetMousePosDelta(PlayerIndex playerIndex)
    {
      return InputManager.mousePositionDelta;
    }

    public static Vector2 GetMousePosDelta(PlayerIndex? playerIndex)
    {
      return InputManager.mousePositionDelta;
    }

    public static Vector2 GetMousePosDeltaSmoothed(PlayerIndex playerIndex)
    {
      return InputManager.mousePositionDeltaSmoothed;
    }

    public static Vector2 GetMousePosDeltaSmoothed(PlayerIndex? playerIndex)
    {
      return InputManager.mousePositionDeltaSmoothed;
    }

    public static int GetMouseWheelDelta(PlayerIndex playerIndex)
    {
      return InputManager.mouse.ScrollWheelValue - InputManager.prevMouse.ScrollWheelValue;
    }

    public static int GetMouseWheelDelta(PlayerIndex? playerIndex)
    {
      return InputManager.mouse.ScrollWheelValue - InputManager.prevMouse.ScrollWheelValue;
    }

    public static bool IsMouseMoved(PlayerIndex playerIndex)
    {
      if ((double) InputManager.mousePositionDelta.X == 0.0)
        return (double) InputManager.mousePositionDelta.Y != 0.0;
      return true;
    }

    public static bool IsMouseButtonPressed(PlayerIndex playerIndex, StudioForge.Engine.Integration.MouseButtons button)
    {
      switch (button)
      {
        case StudioForge.Engine.Integration.MouseButtons.LeftButton:
          return InputManager.mouse.LeftButton == ButtonState.Pressed;
        case StudioForge.Engine.Integration.MouseButtons.RightButton:
          return InputManager.mouse.RightButton == ButtonState.Pressed;
        case StudioForge.Engine.Integration.MouseButtons.MiddleButton:
          return InputManager.mouse.MiddleButton == ButtonState.Pressed;
        case StudioForge.Engine.Integration.MouseButtons.ScrollWheel:
          return InputManager.mouse.ScrollWheelValue != InputManager.prevMouse.ScrollWheelValue;
        case StudioForge.Engine.Integration.MouseButtons.XButton1:
          return InputManager.mouse.XButton1 == ButtonState.Pressed;
        case StudioForge.Engine.Integration.MouseButtons.XButton2:
          return InputManager.mouse.XButton2 == ButtonState.Pressed;
        default:
          return false;
      }
    }

    public static bool IsMouseButtonPressedNew(PlayerIndex playerIndex, StudioForge.Engine.Integration.MouseButtons button)
    {
      switch (button)
      {
        case StudioForge.Engine.Integration.MouseButtons.LeftButton:
          if (InputManager.mouse.LeftButton == ButtonState.Pressed)
            return InputManager.prevMouse.LeftButton == ButtonState.Released;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.RightButton:
          if (InputManager.mouse.RightButton == ButtonState.Pressed)
            return InputManager.prevMouse.RightButton == ButtonState.Released;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.MiddleButton:
          if (InputManager.mouse.MiddleButton == ButtonState.Pressed)
            return InputManager.prevMouse.MiddleButton == ButtonState.Released;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.ScrollWheel:
          if (InputManager.mouse.ScrollWheelValue != InputManager.prevMouse.ScrollWheelValue)
            return InputManager.prevMouse.ScrollWheelValue == InputManager.newScrollWheelPress;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.XButton1:
          if (InputManager.mouse.XButton1 == ButtonState.Pressed)
            return InputManager.prevMouse.XButton1 == ButtonState.Released;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.XButton2:
          if (InputManager.mouse.XButton2 == ButtonState.Pressed)
            return InputManager.prevMouse.XButton2 == ButtonState.Released;
          return false;
        default:
          return false;
      }
    }

    public static bool IsMouseButtonReleased(PlayerIndex playerIndex, StudioForge.Engine.Integration.MouseButtons button)
    {
      switch (button)
      {
        case StudioForge.Engine.Integration.MouseButtons.LeftButton:
          return InputManager.mouse.LeftButton == ButtonState.Released;
        case StudioForge.Engine.Integration.MouseButtons.RightButton:
          return InputManager.mouse.RightButton == ButtonState.Released;
        case StudioForge.Engine.Integration.MouseButtons.MiddleButton:
          return InputManager.mouse.MiddleButton == ButtonState.Released;
        case StudioForge.Engine.Integration.MouseButtons.ScrollWheel:
          return InputManager.mouse.ScrollWheelValue == InputManager.prevMouse.ScrollWheelValue;
        case StudioForge.Engine.Integration.MouseButtons.XButton1:
          return InputManager.mouse.XButton1 == ButtonState.Released;
        case StudioForge.Engine.Integration.MouseButtons.XButton2:
          return InputManager.mouse.XButton2 == ButtonState.Released;
        default:
          return false;
      }
    }

    public static bool IsMouseButtonReleasedNew(PlayerIndex playerIndex, StudioForge.Engine.Integration.MouseButtons button)
    {
      switch (button)
      {
        case StudioForge.Engine.Integration.MouseButtons.LeftButton:
          if (InputManager.mouse.LeftButton == ButtonState.Released)
            return InputManager.prevMouse.LeftButton == ButtonState.Pressed;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.RightButton:
          if (InputManager.mouse.RightButton == ButtonState.Released)
            return InputManager.prevMouse.RightButton == ButtonState.Pressed;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.MiddleButton:
          if (InputManager.mouse.MiddleButton == ButtonState.Released)
            return InputManager.prevMouse.MiddleButton == ButtonState.Pressed;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.ScrollWheel:
          if (InputManager.mouse.ScrollWheelValue == InputManager.prevMouse.ScrollWheelValue)
            return InputManager.prevMouse.ScrollWheelValue != InputManager.newScrollWheelPress;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.XButton1:
          if (InputManager.mouse.XButton1 == ButtonState.Released)
            return InputManager.prevMouse.XButton1 == ButtonState.Pressed;
          return false;
        case StudioForge.Engine.Integration.MouseButtons.XButton2:
          if (InputManager.mouse.XButton2 == ButtonState.Released)
            return InputManager.prevMouse.XButton2 == ButtonState.Pressed;
          return false;
        default:
          return false;
      }
    }

    public static bool IsMouseButtonChanged(PlayerIndex playerIndex, StudioForge.Engine.Integration.MouseButtons button)
    {
      switch (button)
      {
        case StudioForge.Engine.Integration.MouseButtons.LeftButton:
          return InputManager.mouse.LeftButton != InputManager.prevMouse.LeftButton;
        case StudioForge.Engine.Integration.MouseButtons.RightButton:
          return InputManager.mouse.RightButton != InputManager.prevMouse.RightButton;
        case StudioForge.Engine.Integration.MouseButtons.MiddleButton:
          return InputManager.mouse.MiddleButton != InputManager.prevMouse.MiddleButton;
        case StudioForge.Engine.Integration.MouseButtons.ScrollWheel:
          return InputManager.mouse.ScrollWheelValue != InputManager.prevMouse.ScrollWheelValue;
        case StudioForge.Engine.Integration.MouseButtons.XButton1:
          return InputManager.mouse.XButton1 != InputManager.prevMouse.XButton1;
        case StudioForge.Engine.Integration.MouseButtons.XButton2:
          return InputManager.mouse.XButton2 != InputManager.prevMouse.XButton2;
        default:
          return false;
      }
    }

    public static StudioForge.Engine.Integration.MouseButtons GetMouseButtonPressed(
      PlayerIndex playerIndex)
    {
      if (InputManager.mouse.LeftButton == ButtonState.Pressed)
        return StudioForge.Engine.Integration.MouseButtons.LeftButton;
      if (InputManager.mouse.MiddleButton == ButtonState.Pressed)
        return StudioForge.Engine.Integration.MouseButtons.MiddleButton;
      if (InputManager.mouse.RightButton == ButtonState.Pressed)
        return StudioForge.Engine.Integration.MouseButtons.RightButton;
      if (InputManager.mouse.XButton1 == ButtonState.Pressed)
        return StudioForge.Engine.Integration.MouseButtons.XButton1;
      if (InputManager.mouse.XButton2 == ButtonState.Pressed)
        return StudioForge.Engine.Integration.MouseButtons.XButton2;
      return InputManager.mouse.ScrollWheelValue != InputManager.prevMouse.ScrollWheelValue ? StudioForge.Engine.Integration.MouseButtons.ScrollWheel : StudioForge.Engine.Integration.MouseButtons.None;
    }

    public static StudioForge.Engine.Integration.MouseButtons GetMouseButtonPressedNew(
      PlayerIndex playerIndex)
    {
      if (InputManager.mouse.LeftButton == ButtonState.Pressed && InputManager.prevMouse.LeftButton == ButtonState.Released)
        return StudioForge.Engine.Integration.MouseButtons.LeftButton;
      if (InputManager.mouse.MiddleButton == ButtonState.Pressed && InputManager.prevMouse.MiddleButton == ButtonState.Released)
        return StudioForge.Engine.Integration.MouseButtons.MiddleButton;
      if (InputManager.mouse.RightButton == ButtonState.Pressed && InputManager.prevMouse.RightButton == ButtonState.Released)
        return StudioForge.Engine.Integration.MouseButtons.RightButton;
      if (InputManager.mouse.XButton1 == ButtonState.Pressed && InputManager.prevMouse.XButton1 == ButtonState.Released)
        return StudioForge.Engine.Integration.MouseButtons.XButton1;
      if (InputManager.mouse.XButton2 == ButtonState.Pressed && InputManager.prevMouse.XButton2 == ButtonState.Released)
        return StudioForge.Engine.Integration.MouseButtons.XButton2;
      return InputManager.mouse.ScrollWheelValue != InputManager.prevMouse.ScrollWheelValue && InputManager.prevMouse.ScrollWheelValue == InputManager.newScrollWheelPress ? StudioForge.Engine.Integration.MouseButtons.ScrollWheel : StudioForge.Engine.Integration.MouseButtons.None;
    }

    public static Keys[] GetPressedKeys(PlayerIndex playerIndex)
    {
      return InputManager.keyboard.GetPressedKeys();
    }

    public static Keys[] GetPressedKeysPrev(PlayerIndex playerIndex)
    {
      return InputManager.prevKeyboard.GetPressedKeys();
    }

    public static bool IsKeyPressed(PlayerIndex playerIndex, Keys key)
    {
      return InputManager.keyboard.IsKeyDown(key);
    }

    public static bool IsKeyReleased(PlayerIndex playerIndex, Keys key)
    {
      return InputManager.keyboard.IsKeyUp(key);
    }

    public static bool IsKeyPressedNew(PlayerIndex playerIndex, Keys key)
    {
      if (InputManager.keyboard.IsKeyDown(key))
        return InputManager.prevKeyboard.IsKeyUp(key);
      return false;
    }

    public static bool IsKeyReleasedNew(PlayerIndex playerIndex, Keys key)
    {
      if (InputManager.keyboard.IsKeyUp(key))
        return InputManager.prevKeyboard.IsKeyDown(key);
      return false;
    }

    public static bool IsKeyChanged(PlayerIndex playerIndex, Keys key)
    {
      return InputManager.keyboard[key] != InputManager.prevKeyboard[key];
    }

    public static Keys GetNumKeyPressedNew(PlayerIndex playerIndex)
    {
      foreach (Keys pressedKey1 in InputManager.keyboard.GetPressedKeys())
      {
        if (pressedKey1 >= Keys.D0 && pressedKey1 <= Keys.D9)
        {
          foreach (Keys pressedKey2 in InputManager.prevKeyboard.GetPressedKeys())
          {
            if (pressedKey2 == pressedKey1)
              return Keys.None;
          }
          return pressedKey1;
        }
      }
      return Keys.None;
    }

    public static Keys GetInputKey(PlayerIndex playerIndex, ushort key)
    {
      InputItem inputItem;
      if (InputManager.scheme.TryGetValue(key, out inputItem))
        return inputItem.Key;
      return Keys.None;
    }

    public static GamePadState GetGamepadState(PlayerIndex playerIndex)
    {
      return InputManager.gamePad;
    }

    public static void GetGamepadState(
      PlayerIndex playerIndex,
      out GamePadState state,
      out GamePadState prevState)
    {
      state = InputManager.gamePad;
      prevState = InputManager.prevGamePad;
    }

    public static Vector2 GetGamepadLeftStick(PlayerIndex playerIndex)
    {
      return InputManager.gamePad.ThumbSticks.Left;
    }

    public static Vector2 GetGamepadRightStick(PlayerIndex playerIndex)
    {
      return InputManager.gamePad.ThumbSticks.Right;
    }

    public static bool IsButtonPressed(PlayerIndex playerIndex, Buttons button)
    {
      return InputManager.gamePad.IsButtonDown(button);
    }

    public static bool IsButtonReleased(PlayerIndex playerIndex, Buttons button)
    {
      return InputManager.gamePad.IsButtonUp(button);
    }

    public static bool IsButtonChanged(PlayerIndex playerIndex, Buttons button)
    {
      return InputManager.gamePad.IsButtonUp(button) != InputManager.prevGamePad.IsButtonUp(button);
    }

    public static bool IsButtonPressedNew(PlayerIndex playerIndex, Buttons button)
    {
      if (InputManager.gamePad.IsButtonDown(button))
        return InputManager.prevGamePad.IsButtonUp(button);
      return false;
    }

    public static bool IsButtonReleasedNew(PlayerIndex playerIndex, Buttons button)
    {
      if (InputManager.gamePad.IsButtonUp(button))
        return InputManager.prevGamePad.IsButtonDown(button);
      return false;
    }

    public static Buttons GetButtonPressed(PlayerIndex playerIndex)
    {
      if (InputManager.gamePad.Buttons.A == ButtonState.Pressed)
        return Buttons.A;
      if (InputManager.gamePad.Buttons.B == ButtonState.Pressed)
        return Buttons.B;
      if (InputManager.gamePad.Buttons.X == ButtonState.Pressed)
        return Buttons.X;
      if (InputManager.gamePad.Buttons.Y == ButtonState.Pressed)
        return Buttons.Y;
      if (InputManager.gamePad.DPad.Down == ButtonState.Pressed)
        return Buttons.DPadDown;
      if (InputManager.gamePad.DPad.Left == ButtonState.Pressed)
        return Buttons.DPadLeft;
      if (InputManager.gamePad.DPad.Right == ButtonState.Pressed)
        return Buttons.DPadRight;
      if (InputManager.gamePad.DPad.Up == ButtonState.Pressed)
        return Buttons.DPadUp;
      if (InputManager.gamePad.Buttons.Back == ButtonState.Pressed)
        return Buttons.Back;
      if (InputManager.gamePad.Buttons.BigButton == ButtonState.Pressed)
        return Buttons.BigButton;
      if (InputManager.gamePad.Buttons.LeftShoulder == ButtonState.Pressed)
        return Buttons.LeftShoulder;
      if (InputManager.gamePad.Buttons.LeftStick == ButtonState.Pressed)
        return Buttons.LeftStick;
      if (InputManager.gamePad.Buttons.RightShoulder == ButtonState.Pressed)
        return Buttons.RightShoulder;
      if (InputManager.gamePad.Buttons.RightStick == ButtonState.Pressed)
        return Buttons.RightStick;
      return InputManager.gamePad.Buttons.Start == ButtonState.Pressed ? Buttons.Start : (Buttons) 0;
    }

    public static Buttons GetButtonPressedNew(PlayerIndex playerIndex)
    {
      if (InputManager.gamePad.Buttons.A == ButtonState.Pressed && InputManager.prevGamePad.Buttons.A == ButtonState.Released)
        return Buttons.A;
      if (InputManager.gamePad.Buttons.B == ButtonState.Pressed && InputManager.prevGamePad.Buttons.B == ButtonState.Released)
        return Buttons.B;
      if (InputManager.gamePad.Buttons.X == ButtonState.Pressed && InputManager.prevGamePad.Buttons.X == ButtonState.Released)
        return Buttons.X;
      if (InputManager.gamePad.Buttons.Y == ButtonState.Pressed && InputManager.prevGamePad.Buttons.Y == ButtonState.Released)
        return Buttons.Y;
      if (InputManager.gamePad.DPad.Down == ButtonState.Pressed && InputManager.prevGamePad.DPad.Down == ButtonState.Released)
        return Buttons.DPadDown;
      if (InputManager.gamePad.DPad.Left == ButtonState.Pressed && InputManager.prevGamePad.DPad.Left == ButtonState.Released)
        return Buttons.DPadLeft;
      if (InputManager.gamePad.DPad.Right == ButtonState.Pressed && InputManager.prevGamePad.DPad.Right == ButtonState.Released)
        return Buttons.DPadRight;
      if (InputManager.gamePad.DPad.Up == ButtonState.Pressed && InputManager.prevGamePad.DPad.Up == ButtonState.Released)
        return Buttons.DPadUp;
      if (InputManager.gamePad.Buttons.Back == ButtonState.Pressed && InputManager.prevGamePad.Buttons.Back == ButtonState.Released)
        return Buttons.Back;
      if (InputManager.gamePad.Buttons.BigButton == ButtonState.Pressed && InputManager.prevGamePad.Buttons.BigButton == ButtonState.Released)
        return Buttons.BigButton;
      if (InputManager.gamePad.Buttons.LeftShoulder == ButtonState.Pressed && InputManager.prevGamePad.Buttons.LeftShoulder == ButtonState.Released)
        return Buttons.LeftShoulder;
      if (InputManager.gamePad.Buttons.LeftStick == ButtonState.Pressed && InputManager.prevGamePad.Buttons.LeftStick == ButtonState.Released)
        return Buttons.LeftStick;
      if (InputManager.gamePad.Buttons.RightShoulder == ButtonState.Pressed && InputManager.prevGamePad.Buttons.RightShoulder == ButtonState.Released)
        return Buttons.RightShoulder;
      if (InputManager.gamePad.Buttons.RightStick == ButtonState.Pressed && InputManager.prevGamePad.Buttons.RightStick == ButtonState.Released)
        return Buttons.RightStick;
      return InputManager.gamePad.Buttons.Start == ButtonState.Pressed && InputManager.prevGamePad.Buttons.Start == ButtonState.Released ? Buttons.Start : (Buttons) 0;
    }

    public static void Update()
    {
      int scrollWheelValue = InputManager.prevMouse.ScrollWheelValue;
      InputManager.prevMouse = InputManager.mouse;
      InputManager.prevKeyboard = InputManager.keyboard;
      InputManager.prevGamePad = InputManager.gamePad;
      InputManager.mouse = Mouse.GetState();
      InputManager.keyboard = Keyboard.GetState();
      InputManager.gamePad = GamePad.GetState(InputManager.playerIndex);
      if (InputManager.IsGamePadInUse)
        InputManager.IsUsingGamePad = true;
      else if (InputManager.IsMouseInUse || InputManager.IsKeyboardInUse)
      {
        InputManager.IsUsingGamePad = false;
        if (InputManager.prevMouse.ScrollWheelValue == scrollWheelValue)
          InputManager.newScrollWheelPress = scrollWheelValue;
      }
      Vector2 left = InputManager.gamePad.ThumbSticks.Left;
      if (InputManager.useVirtualMouse > 0 && ((double) left.X != 0.0 || (double) left.Y != 0.0))
      {
        int num = 10;
        InputManager.mousePositionDelta.X = InputManager.gamePad.ThumbSticks.Left.X * (float) num;
        InputManager.mousePositionDelta.Y = InputManager.gamePad.ThumbSticks.Left.Y * (float) num;
        InputManager.mousePositionVec.X += InputManager.mousePositionDelta.X;
        InputManager.mousePositionVec.Y -= InputManager.mousePositionDelta.Y;
        InputManager.SetMousePos((int) InputManager.mousePositionVec.X, (int) InputManager.mousePositionVec.Y);
      }
      else
      {
        InputManager.mousePositionDelta.X = (float) (InputManager.mouse.X - InputManager.prevMouse.X);
        InputManager.mousePositionDelta.Y = (float) (InputManager.mouse.Y - InputManager.prevMouse.Y);
        InputManager.mousePosition.X = InputManager.mouse.X;
        InputManager.mousePosition.Y = InputManager.mouse.Y;
        InputManager.mousePositionVec.X = (float) InputManager.mouse.X;
        InputManager.mousePositionVec.Y = (float) InputManager.mouse.Y;
        if (InputManager.useVirtualMouse >= 1)
          return;
        InputManager.mousePositionDeltaSmoothed.X = InputManager.mousePositionDelta.X;
        InputManager.mousePositionDeltaSmoothed.Y = InputManager.mousePositionDelta.Y;
        if (InputManager.mouseSmoothed != null)
        {
          if ((double) InputManager.mousePositionDelta.X == 0.0 && (double) InputManager.mousePositionDelta.Y == 0.0)
            ++InputManager.mouseDiffZeroCount;
          if (InputManager.mouseDiffZeroCount > 2)
          {
            InputManager.mouseSmoothIndex = 0;
            InputManager.mouseDiffZeroCount = 0;
          }
          else
          {
            if (InputManager.mouseSmoothIndex < InputManager.mouseSmoothed.Length)
              ++InputManager.mouseSmoothIndex;
            for (int index = 0; index < InputManager.mouseSmoothIndex - 1; ++index)
            {
              Vector2 vector2 = InputManager.mouseSmoothed[index + 1];
              InputManager.mousePositionDeltaSmoothed.X += vector2.X;
              InputManager.mousePositionDeltaSmoothed.Y += vector2.Y;
              InputManager.mouseSmoothed[index] = vector2;
            }
            InputManager.mouseSmoothed[InputManager.mouseSmoothIndex - 1] = InputManager.mousePositionDelta;
            InputManager.mousePositionDeltaSmoothed.X /= (float) InputManager.mouseSmoothIndex;
            InputManager.mousePositionDeltaSmoothed.Y /= (float) InputManager.mouseSmoothIndex;
            InputManager.mouseDiffZeroCount = 0;
          }
        }
        Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
        Mouse.SetPosition(viewport.X + viewport.Width / 2, viewport.Y + viewport.Height / 2);
        InputManager.mouse = Mouse.GetState();
      }
    }

    private static bool IsGamePadInUse
    {
      get
      {
        if (!(InputManager.gamePad.ThumbSticks.Left != Vector2.Zero) && (!(InputManager.gamePad.ThumbSticks.Right != Vector2.Zero) && !(InputManager.gamePad.Buttons != InputManager.prevGamePad.Buttons) && !(InputManager.gamePad.DPad != InputManager.prevGamePad.DPad)))
          return InputManager.gamePad.Triggers != InputManager.prevGamePad.Triggers;
        return true;
      }
    }

    private static bool IsMouseInUse
    {
      get
      {
        if (InputManager.mouse.LeftButton != ButtonState.Pressed && InputManager.mouse.MiddleButton != ButtonState.Pressed && (InputManager.mouse.RightButton != ButtonState.Pressed && InputManager.mouse.ScrollWheelValue == InputManager.prevMouse.ScrollWheelValue) && InputManager.mouse.XButton1 != ButtonState.Pressed)
          return InputManager.mouse.XButton2 == ButtonState.Pressed;
        return true;
      }
    }

    private static bool IsKeyboardInUse
    {
      get
      {
        return InputManager.keyboard != InputManager.prevKeyboard;
      }
    }
  }
}
