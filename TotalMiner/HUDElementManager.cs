// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.HUDElementManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class HUDElementManager
  {
    private List<HUDElement> elements;

    public List<HUDElement> HUDElements
    {
      get
      {
        return this.elements;
      }
    }

    public HUDElementManager()
    {
      this.elements = new List<HUDElement>();
    }

    private HUDElement GetElement(string name)
    {
      return this.GetElement(name, -1);
    }

    private HUDElement GetElement(string name, int index)
    {
      lock (this.elements)
      {
        foreach (HUDElement element in this.elements)
        {
          if ((index < 0 || index == element.Index) && element.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            return element;
        }
      }
      return (HUDElement) null;
    }

    public void AddShape(
      string name,
      int index,
      Rectangle rect,
      Color color,
      HUDElementProps props)
    {
      HUDElement element = this.GetElement(name, index);
      if (element != null)
      {
        HUDRect hudRect = element as HUDRect;
        if (hudRect == null)
          return;
        hudRect.Rect = rect;
        hudRect.Color = color;
        hudRect.Props = props;
      }
      else
      {
        HUDRect hudRect = new HUDRect();
        hudRect.Name = name;
        hudRect.Color = color;
        hudRect.Props = props;
        hudRect.Rect = rect;
        hudRect.Index = index;
        HUDElement hudElement = (HUDElement) hudRect;
        lock (this.elements)
          this.elements.Add(hudElement);
      }
    }

    public void AddText(
      string name,
      string text,
      Vector2 pos,
      float scale,
      float rot,
      Color color,
      HUDElementProps props)
    {
      HUDElement element = this.GetElement(name);
      if (element != null)
      {
        HUDText hudText = element as HUDText;
        if (hudText == null)
          return;
        hudText.HUDString = text;
        hudText.Position = pos;
        hudText.Scale = scale;
        hudText.Rotation = rot;
        hudText.Color = color;
        hudText.Props = props;
      }
      else
      {
        HUDText hudText = new HUDText();
        hudText.Name = name;
        hudText.HUDString = text;
        hudText.Position = pos;
        hudText.Scale = scale;
        hudText.Rotation = rot;
        hudText.Color = color;
        hudText.Props = props;
        HUDElement hudElement = (HUDElement) hudText;
        lock (this.elements)
          this.elements.Add(hudElement);
      }
    }

    public void AddCounter(
      string name,
      History history,
      string historyKey,
      Vector2 pos,
      float scale,
      Color color,
      HUDElementProps props)
    {
      HUDElement element = this.GetElement(name);
      if (element != null)
      {
        HUDCounter hudCounter = element as HUDCounter;
        if (hudCounter == null)
          return;
        hudCounter.History = history;
        hudCounter.HistoryKey = historyKey;
        hudCounter.Position = pos;
        hudCounter.Scale = scale;
        element.Color = color;
        hudCounter.Props = props;
      }
      else
      {
        HUDCounter hudCounter = new HUDCounter();
        hudCounter.Name = name;
        hudCounter.History = history;
        hudCounter.HistoryKey = historyKey;
        hudCounter.Position = pos;
        hudCounter.Scale = scale;
        hudCounter.Color = color;
        hudCounter.Props = props;
        HUDElement hudElement = (HUDElement) hudCounter;
        lock (this.elements)
          this.elements.Add(hudElement);
      }
    }

    public void AddBar(
      string name,
      History history,
      string historyKey,
      int maxValue,
      Rectangle rect,
      float scale,
      Color color,
      HUDElementProps props)
    {
      HUDElement element = this.GetElement(name);
      if (element != null)
      {
        HUDProgressBar hudProgressBar = element as HUDProgressBar;
        if (hudProgressBar == null)
          return;
        hudProgressBar.History = history;
        hudProgressBar.HistoryKey = historyKey;
        hudProgressBar.MaxValue = maxValue;
        hudProgressBar.Rect = rect;
        hudProgressBar.Scale = scale;
        hudProgressBar.Color = color;
        hudProgressBar.Props = props;
      }
      else
      {
        HUDProgressBar hudProgressBar = new HUDProgressBar();
        hudProgressBar.Name = name;
        hudProgressBar.History = history;
        hudProgressBar.HistoryKey = historyKey;
        hudProgressBar.MaxValue = maxValue;
        hudProgressBar.Rect = rect;
        hudProgressBar.Scale = scale;
        hudProgressBar.Color = color;
        hudProgressBar.Props = props;
        HUDElement hudElement = (HUDElement) hudProgressBar;
        lock (this.elements)
          this.elements.Add(hudElement);
      }
    }

    public void RemoveElement(string name)
    {
      this.RemoveElement(name, -1);
    }

    public void RemoveElement(string name, int index)
    {
      lock (this.elements)
      {
        for (int index1 = this.elements.Count - 1; index1 >= 0; --index1)
        {
          if ((index < 0 || index == this.elements[index1].Index) && this.elements[index1].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            this.elements.RemoveAt(index1);
        }
      }
    }
  }
}
