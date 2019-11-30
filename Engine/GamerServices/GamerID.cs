// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GamerServices.GamerID
// Assembly: StudioForge.Engine.GamerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EA07B8F-6C00-417B-9E82-CD1E4EB140B6
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GamerServices.dll

namespace StudioForge.Engine.GamerServices
{
  public struct GamerID
  {
    public static GamerID Sys1 = new GamerID((short) -1);
    public static GamerID Sys2 = new GamerID((short) -2);
    public static GamerID Sys3 = new GamerID((short) -3);
    public readonly short ID;

    public GamerID(short id)
    {
      this.ID = id;
    }

    public GamerID(GamerID id)
    {
      this.ID = id.ID;
    }

    public static bool operator ==(GamerID id1, GamerID id2)
    {
      return (int) id1.ID == (int) id2.ID;
    }

    public static bool operator ==(GamerID id1, short id2)
    {
      return (int) id1.ID == (int) id2;
    }

    public static bool operator !=(GamerID id1, GamerID id2)
    {
      return (int) id1.ID != (int) id2.ID;
    }

    public static bool operator !=(GamerID id1, short id2)
    {
      return (int) id1.ID != (int) id2;
    }

    public static bool Equals(GamerID id1, GamerID id2)
    {
      return (int) id1.ID == (int) id2.ID;
    }

    public override bool Equals(object obj)
    {
      return (int) this.ID == (int) ((GamerID) obj).ID;
    }

    public override int GetHashCode()
    {
      return this.ID.GetHashCode();
    }

    public bool IsGamer
    {
      get
      {
        return this.ID > (short) 0;
      }
    }
  }
}
