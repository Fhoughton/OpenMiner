// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.PropertyToString`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public struct PropertyToString<T>
  {
    private T tvalue;
    private string svalue;
    public string Format;

    public T Value
    {
      get
      {
        return this.tvalue;
      }
      set
      {
        if (this.tvalue.Equals((object) value) && this.svalue != null)
          return;
        this.tvalue = value;
        if (this.Format == null)
          this.svalue = this.tvalue.ToString();
        else
          this.svalue = string.Format(this.Format, (object) this.tvalue);
      }
    }

    public override string ToString()
    {
      return this.svalue;
    }
  }
}
