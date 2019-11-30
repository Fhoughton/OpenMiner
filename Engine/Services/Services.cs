// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Services
// Assembly: StudioForge.Engine.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F64109AA-9366-479E-934D-7CCC2CE0841F
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Services.dll

using System;
using System.IO;

namespace StudioForge.Engine
{
  public static class Services
  {
    public static IServiceProvider Instance;
    public static ICaughtExceptions ExceptionReporter;
    public static string FontPath;
    public static string ScreenManagerPath;
    public static float TotalTime;
    public static float ElapsedTime;
    public static float RealElapsedTime;
    public static bool IsRunningSlowly;

    public static T GetService<T>()
    {
      T service = (T) Services.Instance.GetService(typeof (T));
      if ((object) service == null)
        Services.LogNotFound(typeof (T));
      return service;
    }

    private static void LogNotFound(Type type)
    {
      File.AppendAllText("Log.txt", "Service: " + type.FullName + " not found");
    }
  }
}
