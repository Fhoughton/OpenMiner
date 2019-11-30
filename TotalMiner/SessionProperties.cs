// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SessionProperties
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner
{
  public class SessionProperties
  {
    public int ExeVersion;
    public SessionType SessionType;
    public NetworkSessionState SessionState;
    public string MapName;
    public string OwnerName;
    public string HostName;
    public GameMode GameMode;
    public MapAttribute Attribute;
    public int CurrentPlayerCount;
    public float RatingAvgStars;
    public int RatingsCount;
    public bool SkillsEnabled;
    public bool SkillsLocal;
    public bool CombatEnabled;
    public Permissions DefaultPermission;
    public int ModsEnabledCount;

    public static void Copy(SessionProperties from, SessionProperties to)
    {
      to.SessionType = from.SessionType;
      to.SessionState = from.SessionState;
      to.ExeVersion = from.ExeVersion;
      to.MapName = from.MapName;
      to.OwnerName = from.OwnerName;
      to.HostName = from.HostName;
      to.GameMode = from.GameMode;
      to.Attribute = from.Attribute;
      to.CurrentPlayerCount = from.CurrentPlayerCount;
      to.RatingAvgStars = from.RatingAvgStars;
      to.RatingsCount = from.RatingsCount;
      to.SkillsEnabled = from.SkillsEnabled;
      to.SkillsLocal = from.SkillsLocal;
      to.CombatEnabled = from.CombatEnabled;
      to.DefaultPermission = from.DefaultPermission;
      to.ModsEnabledCount = from.ModsEnabledCount;
    }
  }
}
