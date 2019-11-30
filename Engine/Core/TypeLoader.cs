// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.TypeLoader
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StudioForge.Engine.Core
{
  public class TypeLoader
  {
    public List<Type> LoadTypes(string filepattern)
    {
      List<Type> typeList = new List<Type>();
      foreach (FileInfo file in new DirectoryInfo(".").GetFiles(filepattern))
        typeList.AddRange((IEnumerable<Type>) this.LoadAssemblyTypes(file));
      return typeList;
    }

    private List<Type> LoadAssemblyTypes(FileInfo file)
    {
      List<Type> typeList = new List<Type>();
      foreach (Type exportedType in Assembly.Load(file.Name.Substring(0, file.Name.Length - 4)).GetExportedTypes())
        typeList.Add(exportedType);
      return typeList;
    }
  }
}
