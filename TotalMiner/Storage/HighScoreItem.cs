// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.HighScoreItem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.Storage
{
  internal class HighScoreItem
  {
    public int Ticks;
    public int[] XPList;

    public long TotalXP
    {
      get
      {
        long num = 0;
        for (int index = 0; index < this.XPList.Length; ++index)
          num += (long) this.XPList[index];
        return num;
      }
    }

    public int TotalLevels
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.XPList.Length; ++index)
          num += this.GetLevel((double) this.XPList[index]);
        return num;
      }
    }

    public int CombatLevel
    {
      get
      {
        return SkillData.CombatLevel((float) this.GetLevel((double) this.XPList[0]), (float) this.GetLevel((double) this.XPList[1]), (float) this.GetLevel((double) this.XPList[2]), (float) this.GetLevel((double) this.XPList[3]), (float) this.GetLevel((double) this.XPList[4]));
      }
    }

    public int GetLevel(double xp)
    {
      return SkillData.GetLevel(xp);
    }

    public HighScoreItem()
    {
    }

    public HighScoreItem(CharacterSkillsData skillData)
    {
      if (skillData == null)
        return;
      this.XPList = new int[15];
      for (int index = 0; index < this.XPList.Length; ++index)
        this.XPList[index] = (int) skillData[index + 1].CurrentXP;
    }
  }
}
