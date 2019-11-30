// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.NpcAnimContent
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner.Graphics
{
  internal class NpcAnimContent : IHasContent, IUnmanagedBuffer
  {
    public readonly ActorType ActorType;
    public NpcContentFrame[] Frames;
    private GameInstance instance;

    public long BufferSize
    {
      get
      {
        long num = 0;
        if (this.Frames != null)
        {
          foreach (NpcContentFrame frame in this.Frames)
            num += frame.BufferSize;
        }
        return num;
      }
    }

    public NpcAnimContent(GameInstance instance, ActorType actorType)
    {
      this.ActorType = actorType;
      this.instance = instance;
    }

    public void LoadContent(InitState state)
    {
      ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) this.ActorType];
      int num = actorTypeDataXml.ComNameWalk != null ? actorTypeDataXml.ComNameWalk.Length : 0;
      this.Frames = new NpcContentFrame[1 + num];
      for (int index = 0; index < this.Frames.Length; ++index)
      {
        this.Frames[index] = new NpcContentFrame();
        if (index < num / 2)
          this.Frames[index].LoadContent(this.instance, this.ActorType, actorTypeDataXml.ComNameWalk[index]);
        else if (index > num / 2)
          this.Frames[index].LoadContent(this.instance, this.ActorType, actorTypeDataXml.ComNameWalk[index - 1]);
        else
          this.Frames[index].LoadContent(this.instance, this.ActorType, actorTypeDataXml.ComName);
      }
    }

    public void UnloadContent()
    {
      foreach (NpcContentFrame frame in this.Frames)
        frame?.UnloadContent();
    }
  }
}
