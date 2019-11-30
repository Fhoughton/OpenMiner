// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptLineMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptLineMenuEntry : BlockMenuEntry
  {
    private ScriptEditScreen scriptEditScreen;
    private Script script;
    private int index;

    public ScriptLineMenuEntry(ScriptEditScreen screen, Script script, int index)
      : base((BlockMenuScreen) screen, script.Commands[index])
    {
      this.scriptEditScreen = screen;
      this.script = script;
      this.index = index;
    }

    public override Vector2 HighlightBarOffset
    {
      get
      {
        return new Vector2(this.script.IsInConditionalBlock(this.index) ? -70f : -50f, 0.0f);
      }
    }

    public override void Draw(Vector2 position, int entryID, bool isSelected)
    {
      int num = 50;
      this.index = this.scriptEditScreen.GetCmdIndex(entryID);
      if (this.script.IsInConditionalBlock(this.index))
        num += 20;
      if (entryID >= this.scriptEditScreen.MinLineMarked && entryID <= this.scriptEditScreen.MaxLineMarked)
        this.DrawHighLight(position + new Vector2((float) num, 0.0f), Color.Yellow * 0.5f);
      this.Screen.SpriteBatch.DrawString(this.Screen.Font, (this.index + 1).ToString() + ":", position + this.TextOffset + new Vector2(0.0f, 7f), Color.LightCyan, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      base.Draw(position + new Vector2((float) num, 0.0f), entryID, isSelected);
    }
  }
}
