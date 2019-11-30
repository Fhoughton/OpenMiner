// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.Sound
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Audio;
using StudioForge.Engine;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class Sound
  {
    private string[] soundEffectName = new string[9]
    {
      null,
      "TotalRushPickup",
      "TotalRushPlayerBullet",
      "TotalRushUnderExplosion",
      "TotalRushShipHit",
      "TotalRushSmallShipDestroyed",
      "TotalRushBigShipDestroyed",
      "TotalRushBulletPatternCSRB",
      "TotalRushBulletPatternRS"
    };

    public void Initialize()
    {
    }

    public void PlaySound(SoundEffectType type)
    {
      this.PlaySound(type, 1f);
    }

    public void PlaySound(SoundEffectType type, float volume)
    {
      Cue cue;
      if (!CoreGlobals.AudioManager.PlaySound(this.soundEffectName[(int) type], out cue) || (double) volume >= 1.0)
        return;
      cue.SetVariable("Volume", volume);
    }
  }
}
