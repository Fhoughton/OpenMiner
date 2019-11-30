// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.WindowDragEventArgs
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.GUI
{
  public struct WindowDragEventArgs
  {
    public readonly WindowManager WindowManager;
    public readonly Window Window;
    public readonly Window Hovered;
    public readonly Window DraggingProxy;
    public readonly Point MousePosition;
    public readonly PlayerIndex PlayerIndex;
    public readonly bool RightButton;
    public object Tag;

    public static WindowDragEventArgs Empty
    {
      get
      {
        return new WindowDragEventArgs();
      }
    }

    public WindowDragEventArgs(WindowEventArgs e)
    {
      this.WindowManager = e.WindowManager;
      this.Window = e.Window;
      this.Hovered = e.Hovered;
      this.DraggingProxy = (Window) null;
      this.MousePosition = e.MousePosition;
      this.RightButton = false;
      this.PlayerIndex = e.PlayerIndex;
      this.Tag = e.Tag;
    }

    public WindowDragEventArgs(
      WindowManager windowManager,
      Window window,
      Window hovered,
      Window proxy,
      Point mousePosition,
      bool rightButton)
    {
      this.WindowManager = windowManager;
      this.Window = window;
      this.Hovered = hovered;
      this.DraggingProxy = proxy;
      this.MousePosition = mousePosition;
      this.RightButton = rightButton;
      this.PlayerIndex = windowManager != null ? windowManager.PlayerIndex : PlayerIndex.One;
      this.Tag = (object) null;
    }
  }
}
