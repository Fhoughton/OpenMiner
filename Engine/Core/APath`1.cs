// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.APath`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.Collections;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class APath<TNode> : IEnumerable<TNode>, IEnumerable
  {
    public TNode LastStep { get; private set; }

    public APath<TNode> PreviousSteps { get; private set; }

    public double TotalCost { get; private set; }

    private APath(TNode lastStep, APath<TNode> previousSteps, double totalCost)
    {
      Argument.NotNull((object) lastStep, nameof (lastStep));
      this.LastStep = lastStep;
      this.PreviousSteps = previousSteps;
      this.TotalCost = totalCost;
    }

    public APath(TNode start)
      : this(start, (APath<TNode>) null, 0.0)
    {
    }

    public APath<TNode> AddStep(TNode step, double stepCost)
    {
      return new APath<TNode>(step, this, this.TotalCost + stepCost);
    }

    public IEnumerator<TNode> GetEnumerator()
    {
      for (APath<TNode> p = this; p != null; p = p.PreviousSteps)
        yield return p.LastStep;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
