// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.TitleFileSystem
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.IO;

namespace StudioForge.Engine.Core
{
  public static class TitleFileSystem
  {
    private static string rootPath;

    public static string RootPath
    {
      get
      {
        return TitleFileSystem.rootPath;
      }
      set
      {
        TitleFileSystem.rootPath = value;
        if (TitleFileSystem.rootPath == null || TitleFileSystem.rootPath.EndsWith("\\"))
          return;
        TitleFileSystem.rootPath += (string) (object) '\\';
      }
    }

    public static bool IsFileExist(string path)
    {
      return File.Exists(TitleFileSystem.RootPath + path);
    }

    public static Stream OpenFile(
      string path,
      FileMode mode,
      FileAccess access,
      FileShare share)
    {
      return (Stream) File.Open(TitleFileSystem.RootPath + path, mode, access, share);
    }

    public static string[] GetDirs(string path)
    {
      return Directory.GetDirectories(TitleFileSystem.RootPath + path);
    }

    public static string[] GetFiles(string path, string pattern)
    {
      return Directory.GetFiles(TitleFileSystem.RootPath + path, pattern);
    }
  }
}
