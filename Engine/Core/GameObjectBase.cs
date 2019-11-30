// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.GameObjectBase
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public abstract class GameObjectBase : IGameObject, IHasUpdate, IRecycled, IHasInitialization, IHasContent
  {
    private string name;
    public bool IsEnabledField;

    public string Name
    {
      get
      {
        if (this.name == null)
          this.name = "<Unnamed>";
        return this.name;
      }
      set
      {
        this.name = value;
        if (this.name != null && this.name.Length >= 1)
          return;
        this.name = "<Unnamed>";
      }
    }

    public List<IGameObject> Children { get; protected set; }

    public bool IsEnabled
    {
      get
      {
        return this.IsEnabledField;
      }
      set
      {
        this.IsEnabledField = value;
      }
    }

    public bool IsRecyclable { get; set; }

    public void Initialize()
    {
      this.InitializeCore((InitState) null);
    }

    public void Initialize(InitState state)
    {
      this.InitializeCore(state);
    }

    protected virtual void InitializeCore(InitState state)
    {
    }

    public void LoadContent()
    {
      this.LoadContentCore((InitState) null);
    }

    public void LoadContent(InitState state)
    {
      this.LoadContentCore(state);
    }

    protected virtual void LoadContentCore(InitState state)
    {
    }

    public void UnloadContent()
    {
      this.UnloadContentCore();
    }

    protected virtual void UnloadContentCore()
    {
    }

    public bool HandleInput(InputState input, PlayerIndex playerIndex)
    {
      return this.HandleInputCore(input, playerIndex);
    }

    protected virtual bool HandleInputCore(InputState input, PlayerIndex playerIndex)
    {
      return false;
    }

    public void Update()
    {
      this.UpdateCore((UpdateState) null);
    }

    public void Update(UpdateState state)
    {
      this.UpdateCore(state);
    }

    protected virtual void UpdateCore(UpdateState state)
    {
    }
  }
}
