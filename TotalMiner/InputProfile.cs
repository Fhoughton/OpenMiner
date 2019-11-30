// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.InputProfile
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public class InputProfile
  {
    public string Account;
    public string Name;
    public byte MouseLookAtSmoothing;
    public float MouseSensitivity;
    public float GamePadSensitivity;
    public bool GamePadInvertY;
    public bool GamePadRumble;
    public Dictionary<ushort, InputItem> InputScheme;

    public InputProfile Clone(string account)
    {
      return new InputProfile()
      {
        Account = account,
        Name = this.Name,
        MouseLookAtSmoothing = this.MouseLookAtSmoothing,
        MouseSensitivity = this.MouseSensitivity,
        GamePadSensitivity = this.GamePadSensitivity,
        GamePadInvertY = this.GamePadInvertY,
        GamePadRumble = this.GamePadRumble,
        InputScheme = new Dictionary<ushort, InputItem>((IDictionary<ushort, InputItem>) this.InputScheme)
      };
    }
  }
}
