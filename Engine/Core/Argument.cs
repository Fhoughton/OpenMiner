// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Argument
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.Globalization;

namespace StudioForge.Engine.Core
{
  public static class Argument
  {
    public static void NotNull(object argument, string name)
    {
      if (argument == null)
        throw new ArgumentNullException(name, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Argument '{0}' cannot be null.", (object) name));
    }
  }
}
