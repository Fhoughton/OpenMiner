// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.FSM
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public abstract class FSM
  {
    public readonly int ExitState;
    public static bool CanHandleInput;
    private bool hasExited;

    public virtual bool IsHandlingInput
    {
      get
      {
        return false;
      }
    }

    public bool HasExited
    {
      get
      {
        return this.hasExited;
      }
      protected set
      {
        if (!this.hasExited && value)
          this.Exit();
        this.hasExited = value;
      }
    }

    public FSM(int exitState)
    {
      this.ExitState = exitState;
      this.hasExited = false;
    }

    public virtual void Init()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Draw()
    {
    }

    public virtual void DrawMessages()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void ForceExit()
    {
      this.HasExited = true;
    }
  }
}
