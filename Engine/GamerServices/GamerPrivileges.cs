// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GamerServices.GamerPrivileges
// Assembly: StudioForge.Engine.GamerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EA07B8F-6C00-417B-9E82-CD1E4EB140B6
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GamerServices.dll

namespace StudioForge.Engine.GamerServices
{
  public class GamerPrivileges
  {
    public GamerPrivilegeSetting AllowCommunication
    {
      get
      {
        return GamerPrivilegeSetting.Everyone;
      }
    }

    public bool AllowOnlineSessions
    {
      get
      {
        return true;
      }
    }

    public GamerPrivilegeSetting AllowProfileViewing
    {
      get
      {
        return GamerPrivilegeSetting.Blocked;
      }
    }

    public bool AllowPurchaseContent
    {
      get
      {
        return false;
      }
    }

    public bool AllowTradeContent
    {
      get
      {
        return false;
      }
    }

    public GamerPrivilegeSetting AllowUserCreatedContent
    {
      get
      {
        return GamerPrivilegeSetting.Blocked;
      }
    }
  }
}
