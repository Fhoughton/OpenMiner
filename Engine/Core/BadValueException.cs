// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.BadValueException
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public class BadValueException : Exception
  {
    public string VariableName;
    public object Variable;

    public BadValueException()
      : this("Undefined variable", (object) null)
    {
    }

    public BadValueException(string variableName, object variable)
      : this(variableName, (object) null, (Exception) null)
    {
    }

    public BadValueException(string variableName, object variable, Exception innerException)
      : base("Bad value in " + variableName, innerException)
    {
      this.VariableName = variableName;
      this.Variable = variable;
    }
  }
}
