// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.AvailableNetworkSession
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public sealed class AvailableNetworkSession : IAvailableNetworkSession
  {
    public object SessionProperties { get; internal set; }

    public double Ping { get; internal set; }

    public NetworkSessionType SessionType { get; internal set; }

    public QualityOfService QualityOfService { get; set; }

    public AvailableNetworkSession(NetworkSessionType sessionType, object sessionProperties)
    {
      this.SessionType = sessionType;
      this.SessionProperties = sessionProperties;
    }
  }
}
