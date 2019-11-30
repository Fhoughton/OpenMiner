// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SkillMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;

namespace StudioForge.TotalMiner.Screens
{
  internal class SkillMenuEntry : BlockMenuEntry
  {
    protected Color levelColor = Color.White;
    public int SkillDataIndex;
    protected CharacterSkillsData skillsData;
    protected Player player;
    protected string levelText;
    protected string XPText;
    protected string localRankText;
    protected string globalRankText;
    private string nextLevelXPText;
    protected double lastXP;
    protected float lastLevel;
    protected HighScoreData highScores;
    protected SkillsScreen skillsScreen;

    public int TotalLevel
    {
      get
      {
        if (this.skillsData == null)
          return 0;
        return this.skillsData.TotalLevel;
      }
    }

    public SkillMenuEntry(
      SkillsScreen screen,
      Player player,
      int skillDataIndex,
      HighScoreData highScores)
      : base((BlockMenuScreen) screen, Utils.InsertSpacesBeforeCapitals(player.SkillsData[skillDataIndex].SkillType.ToString()))
    {
      this.Initialize(screen, player, highScores, player.SkillsData, skillDataIndex);
    }

    public SkillMenuEntry(
      SkillsScreen screen,
      CharacterSkillsData skillsData,
      int skillDataIndex,
      HighScoreData highScores)
      : base((BlockMenuScreen) screen, Utils.InsertSpacesBeforeCapitals(skillsData[skillDataIndex].SkillType.ToString()))
    {
      this.Initialize(screen, (Player) null, highScores, skillsData, skillDataIndex);
    }

    public SkillMenuEntry(
      SkillsScreen screen,
      Player player,
      int skillDataIndex,
      string text,
      HighScoreData highScores)
      : base((BlockMenuScreen) screen, text)
    {
      this.Initialize(screen, player, highScores, player.SkillsData, skillDataIndex);
    }

    public SkillMenuEntry(
      SkillsScreen screen,
      CharacterSkillsData skillsData,
      int skillDataIndex,
      string text,
      HighScoreData highScores)
      : base((BlockMenuScreen) screen, text)
    {
      this.Initialize(screen, (Player) null, highScores, skillsData, skillDataIndex);
    }

    public void SetHighScores(HighScoreData data, bool isBanned)
    {
      this.highScores = data;
      this.LoadRankings(isBanned);
    }

    private void Initialize(
      SkillsScreen screen,
      Player player,
      HighScoreData highScores,
      CharacterSkillsData skillsData,
      int skillDataIndex)
    {
      this.skillsScreen = screen;
      this.player = player;
      this.highScores = highScores;
      this.skillsData = skillsData;
      this.SkillDataIndex = skillDataIndex;
      this.ColorHighlighted = Color.DarkGray;
      this.lastXP = -1.0;
      this.lastLevel = -1f;
      this.levelText = this.XPText = this.nextLevelXPText = "";
      this.localRankText = this.globalRankText = (string) null;
    }

    public override Vector2 TextOffset
    {
      get
      {
        return base.TextOffset + new Vector2(18f, 6f);
      }
    }

    private void SelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      this.BuildText();
      Color color = this.IsEnabled ? (isSelected ? this.ColorSelected : this.ColorUnselected) : this.ColorDisabled;
      color = new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      this.DrawItem(position, color, this.levelColor);
      this.DrawTexture(position, color);
    }

    protected virtual void BuildText()
    {
      if (this.SkillDataIndex > 0)
      {
        SkillData skillData = this.skillsData[this.SkillDataIndex];
        if (skillData.CurrentXP != this.lastXP)
        {
          this.lastXP = skillData.CurrentXP;
          this.XPText = string.Format("{0:N0}", (object) (long) this.lastXP);
          this.nextLevelXPText = string.Format("{0:N0}", (object) (skillData.XPToNextLevel + 1));
        }
        if ((double) skillData.LevelWithBonuses((Actor) this.player) != (double) this.lastLevel)
        {
          int level = skillData.Level;
          int bonusLevels = skillData.GetBonusLevels((Actor) this.player);
          this.lastLevel = (float) (level + bonusLevels);
          this.levelText = level.ToString();
          if (bonusLevels > 0)
          {
            SkillMenuEntry skillMenuEntry = this;
            skillMenuEntry.levelText = skillMenuEntry.levelText + "+" + bonusLevels.ToString();
          }
          this.levelColor = Color.White;
        }
      }
      else if (this.player != null)
      {
        int combatLevel = this.player.CombatLevel;
        if ((double) combatLevel > (double) this.lastLevel)
        {
          this.lastLevel = (float) combatLevel;
          this.levelText = combatLevel.ToString();
          this.levelColor = Color.White;
        }
      }
      else
      {
        int combatLevel = this.skillsData.CombatLevel;
        if ((double) combatLevel > (double) this.lastLevel)
        {
          this.lastLevel = (float) combatLevel;
          this.levelText = combatLevel.ToString();
          this.levelColor = Color.White;
        }
      }
      if (this.highScores == null || this.localRankText != null || (this.globalRankText != null || this.skillsScreen == null))
        return;
      this.LoadRankings(this.highScores.IsGamertagBanned(this.skillsScreen.Gamertag));
    }

    protected virtual void LoadRankings(bool isBanned)
    {
      int num = this.highScores == null || this.highScores.HighScores == null ? 0 : this.highScores.HighScores.Count;
      if (this.SkillDataIndex > 0)
      {
        SkillData skillData = this.skillsData[this.SkillDataIndex];
        this.localRankText = this.player == null || this.player.GameInstance == null ? (string) null : this.player.GameInstance.GetSkillRankLocal((SkillType) this.SkillDataIndex, this.lastXP).ToString();
        this.globalRankText = num <= 1 || isBanned ? (string) null : string.Format("{0}/{1}", (object) Globals2.GamertagData.GetSkillRank(this.highScores, (SkillType) this.SkillDataIndex, this.lastXP), (object) num);
      }
      else if (this.player != null)
      {
        int combatLevel = this.player.CombatLevel;
        this.localRankText = this.player.GameInstance != null ? this.player.GameInstance.GetSkillCombatRankLocal(combatLevel).ToString() : (string) null;
        this.globalRankText = num <= 1 || isBanned ? (string) null : string.Format("{0}/{1}", (object) Globals2.GamertagData.GetSkillCombatRank(this.highScores, combatLevel), (object) num);
      }
      else
      {
        int combatLevel = this.skillsData.CombatLevel;
        this.localRankText = (string) null;
        this.globalRankText = num <= 1 || isBanned ? (string) null : string.Format("{0}/{1}", (object) Globals2.GamertagData.GetSkillCombatRank(this.highScores, combatLevel), (object) num);
      }
    }

    protected virtual bool DrawXP
    {
      get
      {
        return this.SkillDataIndex > 0;
      }
    }

    protected virtual bool DrawNextLevel
    {
      get
      {
        return this.SkillDataIndex > 0;
      }
    }

    private void DrawItem(Vector2 position, Color color, Color levelColor)
    {
      position += this.TextOffset;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, position, color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 154f;
      float x1 = this.Screen.ItemFont.MeasureString(this.levelText).X * this.Screen.ItemTextScale;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.levelText, position - new Vector2(x1, 0.0f), levelColor, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 8f;
      if (this.DrawXP)
      {
        float x2 = (float) (160.0 - (double) this.Screen.ItemFont.MeasureString(this.XPText).X * (double) this.Screen.ItemTextScale);
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.XPText, position + new Vector2(x2, 0.0f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      }
      if (this.DrawNextLevel)
      {
        float x2 = (float) (296.0 - (double) this.Screen.ItemFont.MeasureString(this.nextLevelXPText).X * (double) this.Screen.ItemTextScale);
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.nextLevelXPText, position + new Vector2(x2, 0.0f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      }
      if (this.localRankText != null)
      {
        float x2 = (float) (388.0 - (double) this.Screen.ItemFont.MeasureString(this.localRankText).X * (double) this.Screen.ItemTextScale);
        this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.localRankText, position + new Vector2(x2, 0.0f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      }
      if (this.globalRankText == null)
        return;
      float x3 = (float) (580.0 - (double) this.Screen.ItemFont.MeasureString(this.globalRankText).X * (double) this.Screen.ItemTextScale);
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.globalRankText, position + new Vector2(x3, 0.0f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }

    protected override void DrawTexture(Vector2 position, Color color)
    {
      Rectangle destinationRectangle = new Rectangle((int) position.X + 12, (int) position.Y + 3, 24, 24);
      this.Screen.SpriteBatch.Draw(GraphicStatics.TexturePack.ItemTexture, destinationRectangle, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect((Item) (460 + this.SkillDataIndex))), color);
    }
  }
}
