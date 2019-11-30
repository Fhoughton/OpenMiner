// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.FrameRateCounter
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Core
{
  public class FrameRateCounter : GameObjectBase, IFrameRateCounter
  {
    private string debugString = "";
    private int frameCounter;
    private float elapsedTime;
    private int lastFrameRate;
    private int lastSpriteCalls;
    private int lastDrawCalls;

    public int FrameRate { get; private set; }

    public int SpriteCalls { get; set; }

    public int DrawCalls { get; set; }

    public int Primitives { get; set; }

    public string DebugString
    {
      get
      {
        if (this.FrameRate != this.lastFrameRate || this.SpriteCalls != this.lastSpriteCalls || this.DrawCalls != this.lastDrawCalls)
        {
          this.debugString = this.ToString();
          this.lastFrameRate = this.FrameRate;
          this.lastSpriteCalls = this.SpriteCalls;
          this.lastDrawCalls = this.DrawCalls;
        }
        return this.debugString;
      }
    }

    protected override void UpdateCore(UpdateState state)
    {
      this.elapsedTime += Services.RealElapsedTime;
      if ((double) this.elapsedTime <= 1.0)
        return;
      --this.elapsedTime;
      this.FrameRate = this.frameCounter;
      this.frameCounter = 0;
    }

    public void DrawUpdate()
    {
      this.DrawCalls = this.SpriteCalls = this.Primitives = 0;
      ++this.frameCounter;
    }

    public override string ToString()
    {
      return "Fps: " + (object) this.FrameRate + ". Sprite Calls: " + (object) this.SpriteCalls + ". Draw Calls: " + (object) this.DrawCalls + ". Primitives: " + (object) this.Primitives + " / " + (object) (this.Primitives * this.FrameRate) + ".";
    }
  }
}
