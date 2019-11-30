// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Manager`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public abstract class Manager<T> : DrawableGameObjectBase
  {
    protected List<T> childList;

    protected override void InitializeCore(InitState state)
    {
      base.InitializeCore(state);
      this.Name = typeof (T).Name + " Manager";
      this.childList = new List<T>();
    }

    public T[] ChildList
    {
      get
      {
        return this.GetChildListCore();
      }
    }

    protected virtual T[] GetChildListCore()
    {
      return this.childList.ToArray();
    }

    public T Add(T child)
    {
      if ((object) child != null)
      {
        this.childList.Add(child);
        this.AddCore(child);
      }
      return child;
    }

    public virtual void AddCore(T child)
    {
    }

    public void Remove(T child)
    {
      if ((object) child == null)
        return;
      this.RemoveCore(child);
      this.childList.Remove(child);
    }

    public void Remove(int i)
    {
      if (i < 0 || i >= this.childList.Count)
        return;
      this.RemoveCore(this.childList[i]);
      this.childList.RemoveAt(i);
    }

    public virtual void RemoveCore(T child)
    {
    }
  }
}
