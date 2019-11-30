// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.NetworkSessionJoinException
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  [Serializable]
  public class NetworkSessionJoinException : NetworkException
  {
    public NetworkSessionJoinError JoinError { get; private set; }

    public NetworkSessionJoinException()
    {
    }

    public NetworkSessionJoinException(string message)
      : base(message)
    {
    }

    public NetworkSessionJoinException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public NetworkSessionJoinException(string message, NetworkSessionJoinError joinError)
      : base(message)
    {
      this.JoinError = joinError;
    }
  }
}
