// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.WindowEventArgs
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.GUI
{
  public struct WindowEventArgs
  {
    public readonly WindowManager WindowManager;
    public readonly Window Window;
    public readonly Window Hovered;
    public readonly Point MousePosition;
    public readonly PlayerIndex PlayerIndex;
    public readonly bool KeyboardRaised;
    public object Tag;

    public static WindowEventArgs Empty
    {
      get
      {
        return new WindowEventArgs();
      }
    }

    public WindowEventArgs(
      WindowManager windowManager,
      Window window,
      Window hovered,
      Point mousePosition,
      bool keyboardRaised)
    {
      this.WindowManager = windowManager;
      this.Window = window;
      this.Hovered = hovered;
      this.MousePosition = mousePosition;
      this.KeyboardRaised = keyboardRaised;
      this.PlayerIndex = windowManager != null ? windowManager.PlayerIndex : PlayerIndex.One;
      this.Tag = (object) null;
    }
  }
}
