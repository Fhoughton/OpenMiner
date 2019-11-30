// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.CloseIcon
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

namespace StudioForge.Engine.GUI
{
  internal class CloseIcon : Window
  {
    public CloseIcon()
    {
    }

    public CloseIcon(string name, Window parent)
      : this(name, parent.Size.X - 12, 1, 9, 9)
    {
    }

    public CloseIcon(string name, int x, int y, int width, int height)
      : base(name, x, y, width, height)
    {
      this.BorderThickness = 1;
    }
  }
}
