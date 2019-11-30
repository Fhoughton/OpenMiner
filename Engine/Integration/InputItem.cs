// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.InputItem
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework.Input;

namespace StudioForge.Engine.Integration
{
  public struct InputItem
  {
    public Buttons Button;
    public Keys Key;
    public bool KeyCtrl;
    public bool KeyShift;
    public bool KeyAlt;
    public MouseButtons MouseButton;
    public bool MouseCtrl;
    public bool MouseShift;
    public bool MouseAlt;
    public bool EnabledButton;
    public bool EnabledKey;
    public bool EnabledMouseButton;

    public InputItem(Keys key)
    {
      this.Key = key;
      this.KeyCtrl = this.KeyShift = this.KeyAlt = this.MouseCtrl = this.MouseShift = this.MouseAlt = false;
      this.MouseButton = MouseButtons.None;
      this.Button = (Buttons) 0;
      this.EnabledKey = true;
      this.EnabledButton = false;
      this.EnabledMouseButton = false;
    }

    public InputItem(MouseButtons mouseButton)
    {
      this.Key = Keys.None;
      this.KeyCtrl = this.KeyShift = this.KeyAlt = this.MouseCtrl = this.MouseShift = this.MouseAlt = false;
      this.MouseButton = mouseButton;
      this.Button = (Buttons) 0;
      this.EnabledKey = false;
      this.EnabledButton = false;
      this.EnabledMouseButton = true;
    }

    public InputItem(Buttons button)
    {
      this.Key = Keys.None;
      this.KeyCtrl = this.KeyShift = this.KeyAlt = this.MouseCtrl = this.MouseShift = this.MouseAlt = false;
      this.MouseButton = MouseButtons.None;
      this.Button = button;
      this.EnabledKey = false;
      this.EnabledButton = true;
      this.EnabledMouseButton = false;
    }

    public InputItem(Keys key, MouseButtons mouseButton)
    {
      this.Key = key;
      this.KeyCtrl = this.KeyShift = this.KeyAlt = this.MouseCtrl = this.MouseShift = this.MouseAlt = false;
      this.MouseButton = mouseButton;
      this.Button = (Buttons) 0;
      this.EnabledKey = true;
      this.EnabledButton = false;
      this.EnabledMouseButton = true;
    }

    public InputItem(Keys key, Buttons button)
    {
      this.Key = key;
      this.KeyCtrl = this.KeyShift = this.KeyAlt = this.MouseCtrl = this.MouseShift = this.MouseAlt = false;
      this.MouseButton = MouseButtons.None;
      this.Button = button;
      this.EnabledKey = true;
      this.EnabledButton = true;
      this.EnabledMouseButton = false;
    }

    public InputItem(Keys key, MouseButtons mouseButton, Buttons button)
    {
      this.Key = key;
      this.KeyCtrl = this.KeyShift = this.KeyAlt = this.MouseCtrl = this.MouseShift = this.MouseAlt = false;
      this.MouseButton = mouseButton;
      this.Button = button;
      this.EnabledKey = true;
      this.EnabledButton = true;
      this.EnabledMouseButton = true;
    }
  }
}
