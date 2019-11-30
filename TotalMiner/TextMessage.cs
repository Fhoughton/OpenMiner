// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TextMessage
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct TextMessage
  {
    public TextMsgTarget Target;
    public float Timer;
    public string Header;
    public string Message;
    public string MessageLine1;
    public string MessageRemaining;
    public byte ClanID;
    public Color Color;
    public Color BackColor;
    public Vector2 Measure;
    public Vector2 Measure2;
  }
}
