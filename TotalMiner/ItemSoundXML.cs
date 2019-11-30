// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemSoundXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public struct ItemSoundXML
  {
    public string[] Step;
    public string[] Mine;
    public string[] Dig;
    public string[] Chop;
    public string[] Use;
    public string[] UseFail;
    public string[] Hit;

    public bool HasSound
    {
      get
      {
        if (this.Step != null && this.Step.Length > 0 || this.Mine != null && this.Mine.Length > 0 || (this.Dig != null && this.Dig.Length > 0 || this.Chop != null && this.Chop.Length > 0) || this.Use != null && this.Use.Length > 0)
          return true;
        if (this.UseFail != null)
          return this.UseFail.Length > 0;
        return false;
      }
    }
  }
}
