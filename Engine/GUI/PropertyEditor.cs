// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.PropertyEditor
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StudioForge.Engine.GUI
{
  public class PropertyEditor : Window
  {
    public static TextBox.ColorProfile PropertyLabelColors;
    public static DataField.ColorProfile PropertyFieldColors;
    private object target;
    private IPropertyEditorControl controller;
    private Action<ITextInputWindow, object> onValidated;

    public PropertyEditor()
    {
    }

    public PropertyEditor(
      string name,
      int x,
      int y,
      int width,
      int height,
      object target,
      Action<ITextInputWindow, object> onValidated)
      : base(name, x, y, width, height)
    {
      this.target = target;
      this.onValidated = onValidated;
      this.controller = target as IPropertyEditorControl;
      this.InitChildWindows();
    }

    private void InitChildWindows()
    {
      Type type = this.target.GetType();
      int num = 24;
      int y1 = 0;
      TextBox textBox1 = new TextBox(type.Name, 0, y1, this.Size.X, num);
      textBox1.TextAlignX = WinTextAlignX.Left;
      textBox1.TextScale = 0.5f;
      textBox1.BorderThickness = 1;
      TextBox textBox2 = textBox1;
      textBox2.Colors = (Window.ColorProfile) PropertyEditor.PropertyLabelColors;
      this.AddChild((Node) textBox2);
      int y2 = y1 + (num + 1);
      foreach (FieldInfo field in type.GetFields())
      {
        if (field.IsPublic)
        {
          string name = field.Name;
          bool flag = true;
          bool isEditable = true;
          bool isCSV = false;
          object[] customAttributes = field.GetCustomAttributes(typeof (PropertyEditorFieldAttribute), true);
          if (customAttributes != null && customAttributes.Length > 0)
          {
            PropertyEditorFieldAttribute editorFieldAttribute = customAttributes[0] as PropertyEditorFieldAttribute;
            if (editorFieldAttribute != null)
            {
              if (editorFieldAttribute.Name != null && editorFieldAttribute.Name.Length > 0)
                name = editorFieldAttribute.Name;
              flag = editorFieldAttribute.IsVisible;
              isEditable = editorFieldAttribute.IsEditable;
              isCSV = editorFieldAttribute.IsCSV;
            }
          }
          if (flag)
          {
            this.AddProperty(name, y2, num, (object) field, isEditable, isCSV);
            y2 += num + 1;
          }
        }
      }
      foreach (PropertyInfo property in type.GetProperties())
      {
        if (property.GetSetMethod() != (MethodInfo) null)
        {
          string name = property.Name;
          bool flag = true;
          bool isEditable = true;
          bool isCSV = false;
          object[] customAttributes = property.GetCustomAttributes(typeof (PropertyEditorFieldAttribute), true);
          if (customAttributes != null && customAttributes.Length > 0)
          {
            PropertyEditorFieldAttribute editorFieldAttribute = customAttributes[0] as PropertyEditorFieldAttribute;
            if (editorFieldAttribute != null)
            {
              if (editorFieldAttribute.Name != null && editorFieldAttribute.Name.Length > 0)
                name = editorFieldAttribute.Name;
              flag = editorFieldAttribute.IsVisible;
              isEditable = editorFieldAttribute.IsEditable;
              isCSV = editorFieldAttribute.IsCSV;
            }
          }
          if (flag)
          {
            this.AddProperty(name, y2, num, (object) property, isEditable, isCSV);
            y2 += num + 1;
          }
        }
      }
      this.Size = new Point(this.Size.X, y2 - 2);
    }

    private void AddProperty(string name, int y, int h, object tag, bool isEditable, bool isCSV)
    {
      int width = 200;
      TextBox textBox1 = new TextBox(name, 0, y, width, h);
      textBox1.TextAlignX = WinTextAlignX.Left;
      textBox1.TextScale = 0.5f;
      textBox1.BorderThickness = 1;
      TextBox textBox2 = textBox1;
      textBox2.Colors = (Window.ColorProfile) PropertyEditor.PropertyLabelColors;
      this.AddChild((Node) textBox2);
      PropertyEditor.TypeData typeData = new PropertyEditor.TypeData()
      {
        FieldInfo = tag,
        IsEditable = isEditable
      };
      FieldInfo fieldInfo = tag as FieldInfo;
      string text;
      if (fieldInfo != (FieldInfo) null)
      {
        typeData.Value = fieldInfo.GetValue(this.target);
        typeData.Type = fieldInfo.FieldType;
        text = this.controller.ToString(fieldInfo.Name, typeData.Value) ?? this.ToString(typeData.Value);
      }
      else
      {
        PropertyInfo propertyInfo = tag as PropertyInfo;
        if (!(propertyInfo != (PropertyInfo) null))
          return;
        typeData.Value = propertyInfo.GetValue(this.target, (object[]) null);
        typeData.Type = propertyInfo.PropertyType;
        text = this.controller.ToString(propertyInfo.Name, typeData.Value) ?? this.ToString(typeData.Value);
      }
      int num = this.Size.X - width;
      Type[] genericArguments = typeData.Type.GetGenericArguments();
      bool flag = typeData.Type.IsEnum || genericArguments != null && genericArguments.Length > 0 && genericArguments[0].IsEnum;
      TextBox textBox3;
      if (flag || typeData.Value is bool || typeData.Type.Equals(typeof (bool?)))
      {
        DropDown dropDown1 = new DropDown(text, width - 2, y, num + 1, h, 400);
        dropDown1.TextAlignX = WinTextAlignX.Left;
        dropDown1.TextScale = 0.5f;
        dropDown1.BorderThickness = 1;
        dropDown1.Tag = (object) typeData;
        dropDown1.SortComparison = new Comparison<string>(Utils.SortStringNoneAtTop);
        DropDown dropDown2 = dropDown1;
        dropDown2.AddFlags(Window.WinFlags.KeepItemsSorted);
        if (isEditable)
        {
          if (flag)
          {
            dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateEnum);
            object[] customAttributes = typeData.Type.GetCustomAttributes(typeof (FlagsAttribute), true);
            if (!isCSV)
              isCSV = customAttributes != null && customAttributes.Length > 0;
          }
          else if (typeData.Value is bool || typeData.Type.Equals(typeof (bool?)))
            dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateBool);
          else if (genericArguments != null && genericArguments.Length > 0)
          {
            dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateTypes);
            isCSV = true;
          }
          ((ITextInputWindow) dropDown2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateInput);
        }
        if (isCSV)
        {
          dropDown2.HasFlagsAttribute = true;
          dropDown2.GetNewInputHandler = (GetTextInputHander) null;
        }
        dropDown2.Colors = (Window.ColorProfile) PropertyEditor.PropertyFieldColors;
        textBox3 = (TextBox) dropDown2;
        this.AddChild((Node) dropDown2);
      }
      else
      {
        DataField dataField1 = new DataField(text, width - 2, y, num + 1, h);
        dataField1.TextAlignX = WinTextAlignX.Left;
        dataField1.TextScale = 0.5f;
        dataField1.BorderThickness = 1;
        dataField1.Tag = (object) typeData;
        DataField dataField2 = dataField1;
        if (isEditable)
        {
          dataField2.GetNewInputHandler = (GetTextInputHander) null;
          ((ITextInputWindow) dataField2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateInput);
          dataField2.IsNumeric = this.IsNumeric(typeData.Type);
        }
        dataField2.Colors = (Window.ColorProfile) PropertyEditor.PropertyFieldColors;
        textBox3 = (TextBox) dataField2;
        this.AddChild((Node) dataField2);
      }
      if (this.controller != null)
      {
        this.controller.SetPropertyEditorDefaults(name, (Window) textBox3);
        textBox3.IsEnabled = this.controller.IsPropertyEnabled(name);
      }
      else
        textBox3.IsEnabled = true;
      if (textBox3.IsEnabled)
        return;
      textBox3.Text = (string) null;
    }

    private bool IsNumeric(Type type)
    {
      return type.IsEquivalentTo(typeof (byte)) || type.IsEquivalentTo(typeof (short)) || (type.IsEquivalentTo(typeof (ushort)) || type.IsEquivalentTo(typeof (int))) || (type.IsEquivalentTo(typeof (uint)) || type.IsEquivalentTo(typeof (long)) || (type.IsEquivalentTo(typeof (ulong)) || type.IsEquivalentTo(typeof (float)))) || (type.IsEquivalentTo(typeof (double)) || type.IsEquivalentTo(typeof (Vector2)) || (type.IsEquivalentTo(typeof (Vector3)) || type.IsEquivalentTo(typeof (Vector4))) || type.IsEquivalentTo(typeof (Color)));
    }

    private void PopulateTypes(Window win, List<string> list, string filter)
    {
      list.Clear();
      PropertyEditor.TypeData tag = win.Tag as PropertyEditor.TypeData;
      if (tag == null)
        return;
      Type[] genericArguments = tag.Type.GetGenericArguments();
      if (genericArguments == null || genericArguments.Length <= 0)
        return;
      list.AddRange((IEnumerable<string>) Utils.BuildEnumStringArray(genericArguments[0], filter));
    }

    private void PopulateEnum(Window win, List<string> list, string filter)
    {
      list.Clear();
      PropertyEditor.TypeData tag = win.Tag as PropertyEditor.TypeData;
      if (tag == null)
        return;
      Type[] genericArguments = tag.Type.GetGenericArguments();
      foreach (string buildEnumString in Utils.BuildEnumStringArray(genericArguments == null || genericArguments.Length <= 0 ? tag.Type : genericArguments[0], filter))
      {
        if (!buildEnumString.StartsWith("zLast") && buildEnumString != "zCount")
          list.Add(buildEnumString);
      }
    }

    private void PopulateBool(Window win, List<string> list, string filter)
    {
      list.Clear();
      if (this.IsNullableType(((PropertyEditor.TypeData) win.Tag).Type))
        list.Add("Default");
      list.Add("False");
      list.Add("True");
    }

    private void ValidateInput(ITextInputWindow win)
    {
      TextBox textBox = win as TextBox;
      if (textBox == null)
        return;
      PropertyEditor.TypeData tag = textBox.Tag as PropertyEditor.TypeData;
      if (tag == null)
        return;
      FieldInfo fieldInfo1 = tag.FieldInfo as FieldInfo;
      if (fieldInfo1 != (FieldInfo) null)
      {
        string finalText;
        object dataToSet = this.GetDataToSet(fieldInfo1.FieldType, fieldInfo1.Name, textBox.Text, out finalText);
        fieldInfo1.SetValue(this.target, dataToSet);
        textBox.Text = finalText;
      }
      else
      {
        PropertyInfo fieldInfo2 = tag.FieldInfo as PropertyInfo;
        if (fieldInfo2 != (PropertyInfo) null)
        {
          string finalText;
          object dataToSet = this.GetDataToSet(fieldInfo2.PropertyType, fieldInfo2.Name, textBox.Text, out finalText);
          fieldInfo2.SetValue(this.target, dataToSet, (object[]) null);
          textBox.Text = finalText;
        }
      }
      if (this.onValidated != null)
        this.onValidated(win, this.target);
      if (this.controller == null)
        return;
      this.EnablePropertiesAfterValidation();
    }

    private void EnablePropertiesAfterValidation()
    {
      Type type = this.target.GetType();
      foreach (FieldInfo field in type.GetFields())
      {
        if (field.IsPublic)
        {
          TextBox propertyWindow = this.GetPropertyWindow(field.Name) as TextBox;
          if (propertyWindow != null)
          {
            propertyWindow.IsEnabled = this.controller.IsPropertyEnabled(field.Name);
            if (!propertyWindow.IsEnabled)
            {
              propertyWindow.Text = (string) null;
            }
            else
            {
              object data = field.GetValue(this.target);
              propertyWindow.Text = this.controller.ToString(field.Name, data);
              if (propertyWindow.IsTextEmpty)
                propertyWindow.Text = this.ToString(data);
            }
          }
        }
      }
      foreach (PropertyInfo property in type.GetProperties())
      {
        if (property.GetSetMethod() != (MethodInfo) null)
        {
          TextBox propertyWindow = this.GetPropertyWindow(property.Name) as TextBox;
          if (propertyWindow != null)
          {
            propertyWindow.IsEnabled = this.controller.IsPropertyEnabled(property.Name);
            if (!propertyWindow.IsEnabled)
            {
              propertyWindow.Text = (string) null;
            }
            else
            {
              object data = property.GetValue(this.target, (object[]) null);
              propertyWindow.Text = this.controller.ToString(property.Name, data);
              if (propertyWindow.IsTextEmpty)
                propertyWindow.Text = this.ToString(data);
            }
          }
        }
      }
    }

    private Window GetPropertyWindow(string propertyName)
    {
      for (Window window = this.firstChild as Window; window != null; window = window.NextSibling as Window)
      {
        PropertyEditor.TypeData tag = window.Tag as PropertyEditor.TypeData;
        if (tag != null)
        {
          FieldInfo fieldInfo1 = tag.FieldInfo as FieldInfo;
          if (fieldInfo1 != (FieldInfo) null && fieldInfo1.Name == propertyName)
            return window;
          PropertyInfo fieldInfo2 = tag.FieldInfo as PropertyInfo;
          if (fieldInfo2 != (PropertyInfo) null && fieldInfo2.Name == propertyName)
            return window;
        }
      }
      return (Window) null;
    }

    private object GetDataToSet(Type type, string name, string text, out string finalText)
    {
      object obj = (object) null;
      finalText = text;
      if (this.controller != null)
        obj = this.controller.Validate(name, text, out finalText);
      if (obj == null)
        obj = this.GetValue(type, ref finalText);
      Type conversionType = this.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
      if (obj is string)
        obj = Convert.ChangeType(obj, conversionType);
      return obj;
    }

    private bool IsNullableType(Type type)
    {
      if (type.IsGenericType)
        return type.GetGenericTypeDefinition().Equals(typeof (Nullable<>));
      return false;
    }

    private bool HasInterface(Type type, Type i)
    {
      foreach (Type type1 in type.FindInterfaces((TypeFilter) null, (object) null))
      {
        if (type1.Equals(i))
          return true;
      }
      return false;
    }

    private string ToString(object value)
    {
      if (value is Vector2)
      {
        Vector2 vector2 = (Vector2) value;
        return string.Format("{0}, {1}", (object) vector2.X, (object) vector2.Y);
      }
      if (value is Vector3)
      {
        Vector3 vector3 = (Vector3) value;
        return string.Format("{0}, {1}, {2}", (object) vector3.X, (object) vector3.Y, (object) vector3.Z);
      }
      if (value is Vector4)
      {
        Vector4 vector4 = (Vector4) value;
        return string.Format("{0}, {1}, {2}, {3}", (object) vector4.X, (object) vector4.Y, (object) vector4.Z, (object) vector4.W);
      }
      IList list = value as IList;
      if (list != null)
      {
        string str = "";
        for (int index = 0; index < list.Count; ++index)
        {
          str += list[index].ToString();
          if (index < list.Count - 1)
            str += ", ";
        }
        return str;
      }
      if (value == null)
        return "";
      return value.ToString();
    }

    private object GetValue(Type type, ref string text)
    {
      if (type == (Type) null || type == typeof (string))
        return (object) text;
      if (this.IsNullableType(type) && (text == null || text == "" || text.Equals("default", StringComparison.OrdinalIgnoreCase)))
        return (object) null;
      Type[] genericArguments = type.GetGenericArguments();
      if (genericArguments != null && genericArguments.Length > 0)
        type = genericArguments[0];
      if (type.IsEnum)
        return Utils.GetEnumFromString(type, text);
      if (type == typeof (bool))
      {
        bool result;
        bool.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (byte))
      {
        byte result;
        byte.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (short))
      {
        short result;
        short.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (ushort))
      {
        ushort result;
        ushort.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (int))
      {
        int result;
        int.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (uint))
      {
        uint result;
        uint.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (long))
      {
        long result;
        long.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (ulong))
      {
        ulong result;
        ulong.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (float))
      {
        float result;
        float.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (double))
      {
        double result;
        double.TryParse(text, out result);
        text = result.ToString();
        return (object) result;
      }
      if (type == typeof (Vector2))
      {
        float result1 = 0.0f;
        int length = text.IndexOf(',');
        float result2;
        if (length >= 0)
        {
          float.TryParse(text.Substring(0, length), out result2);
          if (length < text.Length - 1)
            float.TryParse(text.Substring(length + 1), out result1);
        }
        else
          float.TryParse(text, out result2);
        Vector2 vector2 = new Vector2(result2, result1);
        text = vector2.X.ToString() + ", " + vector2.Y.ToString();
        return (object) vector2;
      }
      if (type == typeof (Vector3))
      {
        float result1 = 0.0f;
        float result2 = 0.0f;
        int length = text.IndexOf(',');
        float result3;
        if (length >= 0)
        {
          float.TryParse(text.Substring(0, length), out result3);
          int num = text.IndexOf(',', length + 1);
          if (num >= 0)
          {
            float.TryParse(text.Substring(length + 1, num - (length + 1)), out result1);
            if (num < text.Length - 1)
              float.TryParse(text.Substring(num + 1), out result2);
          }
        }
        else
          float.TryParse(text, out result3);
        Vector3 vector3 = new Vector3(result3, result1, result2);
        text = vector3.X.ToString() + ", " + vector3.Y.ToString() + ", " + vector3.Z.ToString();
        return (object) vector3;
      }
      if (type == typeof (Vector4))
      {
        float result1 = 0.0f;
        float result2 = 0.0f;
        float result3 = 0.0f;
        int length = text.IndexOf(',');
        float result4;
        if (length >= 0)
        {
          float.TryParse(text.Substring(0, length), out result4);
          int num1 = text.IndexOf(',', length + 1);
          if (num1 >= 0)
          {
            float.TryParse(text.Substring(length + 1, num1 - (length + 1)), out result1);
            int num2 = text.IndexOf(',', num1 + 1);
            if (num2 >= 0)
            {
              float.TryParse(text.Substring(num1 + 1, num2 - (num1 + 1)), out result2);
              if (num2 < text.Length - 1)
                float.TryParse(text.Substring(num2 + 1), out result3);
            }
          }
        }
        else
          float.TryParse(text, out result4);
        Vector4 vector4 = new Vector4(result4, result1, result2, result3);
        text = vector4.X.ToString() + ", " + vector4.Y.ToString() + ", " + vector4.Z.ToString() + ", " + vector4.W.ToString();
        return (object) vector4;
      }
      if (type == typeof (List<int>))
        return (object) this.GetEnumList<int>(text);
      return (object) type;
    }

    private List<T> GetEnumList<T>(string text) where T : struct
    {
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = Utils.BreakIntoLines((SpriteFont) null, 0, 0.0f, text, false, new char[1]
      {
        ','
      });
      List<T> objList = new List<T>();
      foreach (string str in strArray)
      {
        T? enumFromString = Utils.GetEnumFromString<T>(str.Trim());
        if (enumFromString.HasValue)
          objList.Add(enumFromString.Value);
      }
      return objList;
    }

    static PropertyEditor()
    {
      TextBox.ColorProfile colorProfile1 = new TextBox.ColorProfile();
      colorProfile1.BackDisabledColor = new Color(200, 200, 200, (int) byte.MaxValue);
      colorProfile1.BackClickColor = Color.White;
      colorProfile1.BackColor = Color.White;
      colorProfile1.BackHoverColor = Color.White;
      colorProfile1.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile1.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile1.TextColor = TextBox.DefaultColorProfile.TextColor;
      PropertyEditor.PropertyLabelColors = colorProfile1;
      DataField.ColorProfile colorProfile2 = new DataField.ColorProfile();
      colorProfile2.BackDisabledColor = new Color(200, 200, 200, (int) byte.MaxValue);
      colorProfile2.BackClickColor = Color.White;
      colorProfile2.BackColor = Color.White;
      colorProfile2.BackHoverColor = Color.White;
      colorProfile2.BorderColor = DataField.DefaultColorProfile.BorderColor;
      colorProfile2.ForeColor = DataField.DefaultColorProfile.ForeColor;
      colorProfile2.TextColor = DataField.DefaultColorProfile.TextColor;
      colorProfile2.BackInputColor = DataField.DefaultColorProfile.BackInputColor;
      colorProfile2.BackSelectedTextColor = DataField.DefaultColorProfile.BackSelectedTextColor;
      PropertyEditor.PropertyFieldColors = colorProfile2;
    }

    public class TypeData
    {
      public object Value;
      public Type Type;
      public object FieldInfo;
      public bool IsEditable;
      public bool HasFlagsAttribute;
    }
  }
}
