// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.DrawableGameObjectBase
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Core
{
  public abstract class DrawableGameObjectBase : GameObjectBase, IDrawableGameObject, IGameObject, IHasUpdate, IRecycled, IHasInitialization, IHasContent, IHasDraw
  {
    public bool IsVisible { get; set; }

    protected override void InitializeCore(InitState state)
    {
      base.InitializeCore(state);
      this.IsVisible = true;
    }

    public void Draw()
    {
      this.DrawCore((DrawState) null);
    }

    public void Draw(DrawState state)
    {
      this.DrawCore(state);
    }

    protected abstract void DrawCore(DrawState state);
  }
}
