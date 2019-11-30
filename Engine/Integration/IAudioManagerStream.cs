// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IAudioManagerStream
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework.Audio;

namespace StudioForge.Engine.Integration
{
  public interface IAudioManagerStream
  {
    float SoundVolume { get; set; }

    SoundEffect LoadSoundEffectFromStream(string asset);

    int PlaySoundFromStream(string asset);

    int PlaySoundFromStream(string asset, AudioEmitter emitter);
  }
}
