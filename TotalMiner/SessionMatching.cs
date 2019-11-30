// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SessionMatching
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public class SessionMatching
  {
    public int ExeVersion;
    public SessionType SessionType;
    public string MapName;
    public string OwnerName;
    public string HostName;
    public StudioForge.TotalMiner.GameMode? GameMode;
    public MapAttribute? Attribute;
    public bool? SkillsEnabled;
    public bool? SkillsLocal;
    public bool? CombatEnabled;
  }
}
