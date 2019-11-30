// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.QualityOfService
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class QualityOfService
  {
    public TimeSpan AverageRoundtripTime
    {
      get
      {
        return TimeSpan.MinValue;
      }
    }

    public int BytesPerSecondDownstream
    {
      get
      {
        return 0;
      }
    }

    public int BytesPerSecondUpstream
    {
      get
      {
        return 0;
      }
    }

    public bool IsAvailable
    {
      get
      {
        return true;
      }
    }

    public TimeSpan MinimumRoundtripTime
    {
      get
      {
        return TimeSpan.MinValue;
      }
    }
  }
}
