// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TotalPlayerSkillMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.TotalMiner.Storage;

namespace StudioForge.TotalMiner.Screens
{
  internal class TotalPlayerSkillMenuEntry : SkillMenuEntry
  {
    public TotalPlayerSkillMenuEntry(
      SkillsScreen screen,
      CharacterSkillsData skillsData,
      HighScoreData highScores)
      : base(screen, skillsData, -1, "Total", highScores)
    {
    }

    protected override void BuildText()
    {
      int totalLevel = this.skillsData.TotalLevel;
      if ((double) totalLevel != (double) this.lastLevel)
      {
        this.lastLevel = (float) totalLevel;
        this.levelText = totalLevel.ToString();
        this.levelColor = Color.White;
      }
      double totalXp = this.skillsData.TotalXP;
      if (totalXp != this.lastXP)
      {
        this.lastXP = totalXp;
        this.XPText = string.Format("{0:N0}", (object) (long) this.lastXP);
        this.levelColor = Color.White;
      }
      if (this.highScores == null || this.localRankText != null || (this.globalRankText != null || this.skillsScreen == null))
        return;
      this.LoadRankings(this.highScores.IsGamertagBanned(this.skillsScreen.Gamertag));
    }

    protected override void LoadRankings(bool isBanned)
    {
      int totalLevel = this.skillsData.TotalLevel;
      this.localRankText = this.player == null || this.player.GameInstance == null ? (string) null : this.player.GameInstance.GetSkillTotalRankLocal(totalLevel).ToString();
      this.globalRankText = this.highScores.HighScores.Count <= 1 || isBanned ? (string) null : string.Format("{0}/{1}", (object) Globals2.GamertagData.GetSkillTotalRank(this.highScores, totalLevel), (object) this.highScores.HighScores.Count);
    }

    protected override bool DrawNextLevel
    {
      get
      {
        return false;
      }
    }

    protected override bool DrawXP
    {
      get
      {
        return true;
      }
    }

    protected override void DrawTexture(Vector2 position, Color color)
    {
    }
  }
}
