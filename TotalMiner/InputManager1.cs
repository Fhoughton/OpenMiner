// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.InputManager1
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public static class InputManager1
  {
    public static InputProfile Profile;
    public static InputProfile OrigProfile;

    public static void PushVirtualMouse()
    {
      InputManager.PushVirtualMouse();
    }

    public static void PopVirtualMouse()
    {
      InputManager.PopVirtualMouse();
    }

    public static void Initialize(InputProfile profile)
    {
      if (profile == null)
      {
        profile = new InputProfile()
        {
          Account = "System",
          Name = ""
        };
        InputManager1.RestoreDefaults(profile);
      }
      InputManager1.OrigProfile = profile;
      InputManager1.Profile = profile.Clone(profile.Account);
      InputManager.Initialize(InputManager1.Profile.InputScheme, (int) InputManager1.Profile.MouseLookAtSmoothing);
    }

    public static void RestoreDefaults(InputProfile profile)
    {
      profile.MouseLookAtSmoothing = (byte) 6;
      profile.MouseSensitivity = 0.5f;
      profile.GamePadSensitivity = 0.8f;
      profile.GamePadInvertY = false;
      profile.GamePadRumble = true;
      if (profile.InputScheme == null)
        profile.InputScheme = new Dictionary<ushort, InputItem>(305);
      Dictionary<ushort, InputItem> inputScheme = profile.InputScheme;
      inputScheme.Clear();
      inputScheme.Add((ushort) 1, new InputItem(Keys.W, Buttons.LeftThumbstickUp)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 2, new InputItem(Keys.S, Buttons.LeftThumbstickDown)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 3, new InputItem(Keys.A, Buttons.LeftThumbstickLeft)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 4, new InputItem(Keys.D, Buttons.LeftThumbstickRight)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 5, new InputItem(Keys.Space, StudioForge.Engine.Integration.MouseButtons.None, Buttons.A));
      inputScheme.Add((ushort) 6, new InputItem(Keys.LeftControl, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftStick));
      inputScheme.Add((ushort) 7, new InputItem(Keys.Space, Buttons.A)
      {
        KeyCtrl = true
      });
      inputScheme.Add((ushort) 20, new InputItem(Keys.F, StudioForge.Engine.Integration.MouseButtons.None, Buttons.X));
      inputScheme.Add((ushort) 21, new InputItem(Keys.Space, StudioForge.Engine.Integration.MouseButtons.None, Buttons.A));
      inputScheme.Add((ushort) 22, new InputItem(Keys.LeftControl, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftStick));
      inputScheme.Add((ushort) 30, new InputItem(Keys.None, StudioForge.Engine.Integration.MouseButtons.RightButton, Buttons.LeftTrigger)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 31, new InputItem(Keys.None, StudioForge.Engine.Integration.MouseButtons.LeftButton, Buttons.RightTrigger)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 32, new InputItem(Keys.Z, StudioForge.Engine.Integration.MouseButtons.ScrollWheel, Buttons.LeftShoulder));
      inputScheme.Add((ushort) 33, new InputItem(Keys.X, StudioForge.Engine.Integration.MouseButtons.ScrollWheel, Buttons.RightShoulder));
      inputScheme.Add((ushort) 34, new InputItem(Keys.D1));
      inputScheme.Add((ushort) 35, new InputItem(Keys.D2));
      inputScheme.Add((ushort) 50, new InputItem(Keys.E, StudioForge.Engine.Integration.MouseButtons.None, Buttons.Y));
      inputScheme.Add((ushort) 51, new InputItem(Keys.Escape, Buttons.B));
      inputScheme.Add((ushort) 52, new InputItem(Keys.Tab, StudioForge.Engine.Integration.MouseButtons.None, Buttons.DPadUp));
      inputScheme.Add((ushort) 53, new InputItem(Keys.PageUp, StudioForge.Engine.Integration.MouseButtons.ScrollWheel, Buttons.RightShoulder)
      {
        EnabledMouseButton = false
      });
      inputScheme.Add((ushort) 54, new InputItem(Keys.PageDown, StudioForge.Engine.Integration.MouseButtons.ScrollWheel, Buttons.LeftShoulder)
      {
        EnabledMouseButton = false
      });
      inputScheme.Add((ushort) 80, new InputItem(Keys.Escape, Buttons.Start));
      inputScheme.Add((ushort) 81, new InputItem(Keys.Enter, Buttons.DPadDown));
      inputScheme.Add((ushort) 82, new InputItem(Keys.I, Buttons.B));
      inputScheme.Add((ushort) 83, new InputItem(Keys.U));
      inputScheme.Add((ushort) 84, new InputItem(Keys.B, StudioForge.Engine.Integration.MouseButtons.None, Buttons.DPadLeft));
      inputScheme.Add((ushort) 85, new InputItem(Keys.T, StudioForge.Engine.Integration.MouseButtons.None, Buttons.A));
      inputScheme.Add((ushort) 86, new InputItem(Keys.M, StudioForge.Engine.Integration.MouseButtons.None, Buttons.Back));
      inputScheme.Add((ushort) 87, new InputItem(Keys.F9));
      inputScheme.Add((ushort) 100, new InputItem(Keys.X, Buttons.X));
      inputScheme.Add((ushort) 101, new InputItem(Keys.Y, Buttons.Y));
      inputScheme.Add((ushort) 102, new InputItem(Keys.B, Buttons.B));
      inputScheme.Add((ushort) 120, new InputItem(Keys.None, StudioForge.Engine.Integration.MouseButtons.ScrollWheel, Buttons.RightStick)
      {
        EnabledKey = false,
        EnabledMouseButton = false,
        EnabledButton = false
      });
      inputScheme.Add((ushort) 121, new InputItem(Keys.None, StudioForge.Engine.Integration.MouseButtons.ScrollWheel, Buttons.RightStick)
      {
        EnabledKey = false,
        EnabledMouseButton = false,
        EnabledButton = false
      });
      inputScheme.Add((ushort) 122, new InputItem(Keys.M, StudioForge.Engine.Integration.MouseButtons.LeftButton, Buttons.LeftShoulder));
      inputScheme.Add((ushort) 123, new InputItem(Keys.N, StudioForge.Engine.Integration.MouseButtons.RightButton, Buttons.RightShoulder));
      inputScheme.Add((ushort) 130, new InputItem(Keys.OemMinus, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftShoulder));
      inputScheme.Add((ushort) 131, new InputItem(Keys.OemPlus, StudioForge.Engine.Integration.MouseButtons.None, Buttons.RightShoulder));
      inputScheme.Add((ushort) 140, new InputItem(Keys.C, StudioForge.Engine.Integration.MouseButtons.None, Buttons.X));
      inputScheme.Add((ushort) 141, new InputItem(Keys.L));
      inputScheme.Add((ushort) 150, new InputItem(Keys.Enter, StudioForge.Engine.Integration.MouseButtons.LeftButton, Buttons.A));
      inputScheme.Add((ushort) 200, new InputItem(Keys.Escape, Buttons.B)
      {
        EnabledKey = false
      });
      inputScheme.Add((ushort) 201, new InputItem(Keys.Left, Buttons.DPadLeft));
      inputScheme.Add((ushort) 202, new InputItem(Keys.Right, Buttons.DPadRight));
      inputScheme.Add((ushort) 203, new InputItem(Keys.Up, Buttons.DPadUp));
      inputScheme.Add((ushort) 204, new InputItem(Keys.Down, Buttons.DPadDown));
      inputScheme.Add((ushort) 205, new InputItem(Keys.PageUp, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftTrigger));
      inputScheme.Add((ushort) 206, new InputItem(Keys.PageDown, StudioForge.Engine.Integration.MouseButtons.None, Buttons.RightTrigger));
      inputScheme.Add((ushort) 207, new InputItem(Keys.OemTilde, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftShoulder));
      inputScheme.Add((ushort) 208, new InputItem(Keys.Tab, StudioForge.Engine.Integration.MouseButtons.None, Buttons.RightShoulder));
      inputScheme.Add((ushort) 209, new InputItem(Keys.Enter, StudioForge.Engine.Integration.MouseButtons.LeftButton, Buttons.A));
      inputScheme.Add((ushort) 210, new InputItem(Keys.Insert, StudioForge.Engine.Integration.MouseButtons.RightButton, Buttons.X));
      inputScheme.Add((ushort) 211, new InputItem(Keys.Scroll));
      inputScheme.Add((ushort) 212, new InputItem(Keys.None, StudioForge.Engine.Integration.MouseButtons.MiddleButton));
      inputScheme.Add((ushort) 220, new InputItem(Keys.E, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftShoulder));
      inputScheme.Add((ushort) 221, new InputItem(Keys.T, StudioForge.Engine.Integration.MouseButtons.None, Buttons.Y));
      inputScheme.Add((ushort) 222, new InputItem(Keys.D1, StudioForge.Engine.Integration.MouseButtons.None, Buttons.RightShoulder));
      inputScheme.Add((ushort) 223, new InputItem(Keys.E, StudioForge.Engine.Integration.MouseButtons.None, Buttons.Y));
      inputScheme.Add((ushort) 224, new InputItem(Keys.Enter, StudioForge.Engine.Integration.MouseButtons.None, Buttons.A));
      inputScheme.Add((ushort) 250, new InputItem(Keys.C, Buttons.Start));
      inputScheme.Add((ushort) 251, new InputItem(Keys.A, StudioForge.Engine.Integration.MouseButtons.None, Buttons.LeftTrigger));
      inputScheme.Add((ushort) 252, new InputItem(Keys.D, StudioForge.Engine.Integration.MouseButtons.None, Buttons.RightTrigger));
      inputScheme.Add((ushort) 253, new InputItem(Keys.U, Buttons.Start));
      inputScheme.Add((ushort) 280, new InputItem(Keys.X, StudioForge.Engine.Integration.MouseButtons.None, Buttons.X)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 281, new InputItem(Keys.Y, StudioForge.Engine.Integration.MouseButtons.None, Buttons.Y)
      {
        EnabledButton = false
      });
      inputScheme.Add((ushort) 300, new InputItem(Keys.Enter, Buttons.A));
      inputScheme.Add((ushort) 301, new InputItem(Keys.C, Buttons.Y));
      inputScheme.Add((ushort) 302, new InputItem(Keys.T, Buttons.X));
      inputScheme.Add((ushort) 303, new InputItem(Keys.A, Buttons.X));
      inputScheme.Add((ushort) 304, new InputItem(Keys.D, Buttons.Y));
    }

    public static Keys GetInputKey(PlayerIndex index, PlayerInput key)
    {
      return InputManager.GetInputKey(index, (ushort) key);
    }

    public static Keys GetInputKey(PlayerIndex index, GuiInput key)
    {
      return InputManager.GetInputKey(index, (ushort) key);
    }

    public static bool IsInputPressed(PlayerIndex index, PlayerInput key)
    {
      return InputManager.IsInputPressed(index, (ushort) key);
    }

    public static bool IsInputPressed(PlayerIndex index, GuiInput key)
    {
      return InputManager.IsInputPressed(index, (ushort) key);
    }

    public static bool IsInputPressed(PlayerIndex? index, PlayerInput key)
    {
      return InputManager.IsInputPressed(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputPressed(PlayerIndex? index, GuiInput key)
    {
      return InputManager.IsInputPressed(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputReleased(PlayerIndex index, PlayerInput key)
    {
      return InputManager.IsInputReleased(index, (ushort) key);
    }

    public static bool IsInputReleased(PlayerIndex index, GuiInput key)
    {
      return InputManager.IsInputReleased(index, (ushort) key);
    }

    public static bool IsInputReleased(PlayerIndex? index, PlayerInput key)
    {
      return InputManager.IsInputReleased(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputReleased(PlayerIndex? index, GuiInput key)
    {
      return InputManager.IsInputReleased(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputPressedNew(PlayerIndex index, PlayerInput key)
    {
      return InputManager.IsInputPressedNew(index, (ushort) key);
    }

    public static bool IsInputPressedNew(PlayerIndex index, GuiInput key)
    {
      if (InputManager.IsInputPressedNew(index, (ushort) key))
        return true;
      if (key == GuiInput.ExitScreen)
        return InputManager.IsButtonPressedNew(index, Buttons.Back);
      return false;
    }

    public static bool IsInputPressedNew(PlayerIndex? index, PlayerInput key)
    {
      return InputManager.IsInputPressedNew(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputPressedNew(PlayerIndex? index, GuiInput key)
    {
      if (InputManager.IsInputPressedNew(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key))
        return true;
      if (key == GuiInput.ExitScreen)
        return InputManager.IsButtonPressedNew(index.HasValue ? index.Value : PlayerIndex.One, Buttons.Back);
      return false;
    }

    public static bool IsInputReleasedNew(PlayerIndex index, PlayerInput key)
    {
      return InputManager.IsInputReleasedNew(index, (ushort) key);
    }

    public static bool IsInputReleasedNew(PlayerIndex index, GuiInput key)
    {
      return InputManager.IsInputReleasedNew(index, (ushort) key);
    }

    public static bool IsInputReleasedNew(PlayerIndex? index, PlayerInput key)
    {
      return InputManager.IsInputReleasedNew(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputReleasedNew(PlayerIndex? index, GuiInput key)
    {
      return InputManager.IsInputReleasedNew(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputChanged(PlayerIndex index, PlayerInput key)
    {
      return InputManager.IsInputChanged(index, (ushort) key);
    }

    public static bool IsInputChanged(PlayerIndex index, GuiInput key)
    {
      return InputManager.IsInputChanged(index, (ushort) key);
    }

    public static bool IsInputChanged(PlayerIndex? index, PlayerInput key)
    {
      return InputManager.IsInputChanged(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }

    public static bool IsInputChanged(PlayerIndex? index, GuiInput key)
    {
      return InputManager.IsInputChanged(index.HasValue ? index.Value : PlayerIndex.One, (ushort) key);
    }
  }
}
