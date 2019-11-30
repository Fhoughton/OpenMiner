// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SkillRankMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;

namespace StudioForge.TotalMiner.Screens
{
  internal class SkillRankMenuEntry : BlockMenuEntry
  {
    public HighScoreSkillRank Rank;
    private SkillType skillType;
    private string levelText;
    private string xpText;
    private string rankText;
    private bool drawTexture;

    public SkillRankMenuEntry(
      SkillRankScreen screen,
      SkillType skillType,
      HighScoreSkillRank rank,
      bool drawTexture)
      : base((BlockMenuScreen) screen, rank.Gamertag)
    {
      this.Rank = rank;
      this.skillType = skillType;
      this.drawTexture = drawTexture;
      this.levelText = rank.Level.ToString();
      this.xpText = rank.XP.ToString("N0");
      this.rankText = rank.Rank.ToString();
    }

    public override Vector2 TextOffset
    {
      get
      {
        return base.TextOffset + new Vector2(18f, 6f);
      }
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      base.Draw(position, index, isSelected);
      Color color = Globals2.GamertagData.HighScoreData.IsGamertagBanned(this.Rank.Gamertag) ? Color.Red : this.GetItemColor(isSelected);
      float x1 = (float) (361.0 - (double) this.Screen.ItemFont.MeasureString(this.levelText).X * (double) this.Screen.ItemTextScale);
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.levelText, position + new Vector2(x1, 2f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      float x2 = (float) (544.0 - (double) this.Screen.ItemFont.MeasureString(this.xpText).X * (double) this.Screen.ItemTextScale);
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.xpText, position + new Vector2(x2, 2f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      float x3 = (float) (682.0 - (double) this.Screen.ItemFont.MeasureString(this.rankText).X * (double) this.Screen.ItemTextScale);
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.rankText, position + new Vector2(x3, 2f), color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }

    protected override void DrawTexture(Vector2 position, Color color)
    {
      if (!this.drawTexture)
        return;
      Rectangle destinationRectangle = new Rectangle((int) position.X + 12, (int) position.Y + 3, 20, 20);
      this.Screen.SpriteBatch.Draw(GraphicStatics.TexturePack.ItemTexture, destinationRectangle, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect((Item) ((byte) 204 + this.skillType))), Color.White);
    }
  }
}
