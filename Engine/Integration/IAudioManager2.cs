// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IAudioManager2
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace StudioForge.Engine.Integration
{
  public interface IAudioManager2 : IAudioManager, IHasUpdate, IHasContent, IUnmanagedBuffer
  {
    bool IsSongsCached { get; set; }

    bool IsRepeating { get; set; }

    float FadePeriod { get; set; }

    Song LoadSong(string asset);

    void StopSong();

    void StopSongImmediately();

    SoundEffect LoadSound(string asset);

    void QueueSound(string asset, float timeToWait);

    SoundEffectInstance PlayNewInstance(string asset);

    SoundEffectInstance PlayNewInstance(string asset, float volume);

    void Fadeout(SoundEffectInstance effect, float time);
  }
}
