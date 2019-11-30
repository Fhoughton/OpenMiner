// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.FileSystem
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace StudioForge.Engine.Core
{
  public static class FileSystem
  {
    private static string rootPath;

    public static string RootPath
    {
      get
      {
        return FileSystem.rootPath;
      }
      set
      {
        FileSystem.rootPath = value;
        if (FileSystem.rootPath == null || FileSystem.rootPath.EndsWith("\\"))
          return;
        FileSystem.rootPath += (string) (object) "\\";
      }
    }

    public static bool IsFileExist(string path)
    {
      return File.Exists(FileSystem.RootPath + path);
    }

    public static void DeleteFile(string path)
    {
      if (!FileSystem.IsFileExist(path))
        return;
      File.Delete(FileSystem.RootPath + path);
    }

    public static Stream CreateFile(string path)
    {
      return (Stream) File.Create(FileSystem.RootPath + path);
    }

    public static Stream OpenFile(string path, FileMode mode)
    {
      return (Stream) File.Open(FileSystem.RootPath + path, mode);
    }

    public static Stream OpenFile(string path, FileMode mode, FileAccess access)
    {
      return (Stream) File.Open(FileSystem.RootPath + path, mode, access);
    }

    public static Stream OpenFile(
      string path,
      FileMode mode,
      FileAccess access,
      FileShare share)
    {
      return (Stream) File.Open(FileSystem.RootPath + path, mode, access, share);
    }

    public static Stream OpenRead(string path)
    {
      return (Stream) File.Open(FileSystem.RootPath + path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public static Stream OpenWrite(string path)
    {
      return (Stream) File.Open(FileSystem.RootPath + path, FileMode.Create, FileAccess.Write, FileShare.Write);
    }

    public static T Deserialize<T>(string path)
    {
      using (Stream input = FileSystem.OpenRead(path))
      {
        using (XmlReader xmlReader = XmlReader.Create(input))
          return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
      }
    }

    public static string RemoveRoot(string path)
    {
      if (FileSystem.rootPath != null && path.StartsWith(FileSystem.rootPath))
        return path.Substring(FileSystem.rootPath.Length, path.Length - FileSystem.rootPath.Length);
      return path;
    }

    public static void RemoveRoot(string[] paths)
    {
      if (FileSystem.rootPath == null)
        return;
      for (int index = 0; index < paths.Length; ++index)
        paths[index] = FileSystem.RemoveRoot(paths[index]);
    }

    public static string CurrentDir
    {
      get
      {
        return Directory.GetCurrentDirectory();
      }
    }

    public static bool IsDirExist(string path)
    {
      return Directory.Exists(FileSystem.RootPath + path);
    }

    public static void SetCurrentDir(string path)
    {
      Directory.SetCurrentDirectory(FileSystem.RootPath + path);
    }

    public static void CreateDir(string path)
    {
      if (FileSystem.IsDirExist(path))
        return;
      Directory.CreateDirectory(FileSystem.RootPath + path);
    }

    public static void DeleteDir(string path)
    {
      if (!FileSystem.IsDirExist(path))
        return;
      FileSystem.EmptyDir(path, "*");
      Directory.Delete(FileSystem.RootPath + path);
    }

    public static void DeleteDir(string path, bool recursive)
    {
      if (!FileSystem.IsDirExist(path))
        return;
      Directory.Delete(FileSystem.RootPath + path, recursive);
    }

    public static void EmptyDir(string path, string pattern)
    {
      foreach (string file in FileSystem.GetFiles(path, pattern))
        FileSystem.DeleteFile(file);
    }

    public static string[] GetDirs(string path)
    {
      if (!FileSystem.IsDirExist(path))
        return new string[0];
      string[] directories = Directory.GetDirectories(FileSystem.RootPath + path);
      FileSystem.RemoveRoot(directories);
      return directories;
    }

    public static string[] GetFiles(string path, string pattern)
    {
      if (!FileSystem.IsDirExist(path))
        return new string[0];
      string[] files = Directory.GetFiles(FileSystem.RootPath + path, pattern);
      FileSystem.RemoveRoot(files);
      return files;
    }
  }
}
