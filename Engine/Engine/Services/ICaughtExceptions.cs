// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.ICaughtExceptions
// Assembly: StudioForge.Engine.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F64109AA-9366-479E-934D-7CCC2CE0841F
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Services.dll

using System;

namespace StudioForge.Engine
{
  public interface ICaughtExceptions
  {
    void ReportExceptionCaught(int id, Exception e);
  }
}
