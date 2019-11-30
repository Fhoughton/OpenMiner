// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.FileListBoxScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class FileListBoxScreen : ListBoxScreen
  {
    public FileListBoxScreen(Player player, List<string> data)
      : base(player, data, (ListBoxScreen.OnMenuItemSelected) null, false)
    {
    }

    public FileListBoxScreen(Player player, string[] data)
      : base(player, data, (ListBoxScreen.OnMenuItemSelected) null, false)
    {
    }

    public override bool HandleInput(InputState input)
    {
      if (!input.IsNewButtonPress(Buttons.Y, this.ControllingPlayer.Value))
        return base.HandleInput(input);
      this.DeleteSelectedFile();
      return true;
    }

    private void DeleteSelectedFile()
    {
      string path = this.MenuEntries[this.selectedEntry].Text;
      int length = path.IndexOf("  / ");
      if (length >= 0)
        path = path.Substring(0, length);
      FileSystem.DeleteFile(path);
      this.MenuEntries.RemoveAt(this.selectedEntry);
    }
  }
}
