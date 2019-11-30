// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.NetworkSessionProperties
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public class NetworkSessionProperties
  {
    private int?[] properties = new int?[8];

    public int? this[int index]
    {
      get
      {
        return this.properties[index];
      }
      set
      {
        this.properties[index] = value;
      }
    }
  }
}
