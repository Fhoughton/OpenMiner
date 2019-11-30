// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AmbientMusicManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner
{
  internal class AmbientMusicManager : GameObjectBase
  {
    private Map map;
    private GameInstance instance;
    private float timeSinceLastAmbience;
    private AmbientMusicWorker worker;
    private bool playNow;

    public AmbientMusicManager(GameInstance instance, Map map)
    {
      this.map = map;
      this.instance = instance;
      this.Name = "Ambient Music";
    }

    protected override void LoadContentCore(InitState state)
    {
      base.LoadContentCore(state);
      this.worker = new AmbientMusicWorker();
    }

    protected override void UnloadContentCore()
    {
      base.UnloadContentCore();
      if (this.worker == null)
        return;
      this.worker.UnloadContent();
    }

    protected override void UpdateCore(UpdateState state)
    {
      if ((double) CoreGlobals.AudioManager.MusicVolume <= 0.0 || Globals1.IsCuePlaying(CoreGlobals.AudioManager.CurrentCue))
        return;
      this.timeSinceLastAmbience += Services.ElapsedTime;
      if ((double) this.timeSinceLastAmbience <= 10.0 || !this.playNow && !this.instance.Random.RandomChanceTime(20.0))
        return;
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.worker, false, PriorityLevel.Normal);
      this.timeSinceLastAmbience = 0.0f;
      this.playNow = false;
    }

    public void ResetMusicShuffle()
    {
      if (this.worker == null)
        return;
      this.worker.Reshuffle();
      this.timeSinceLastAmbience = 8f;
      this.playNow = true;
    }
  }
}
