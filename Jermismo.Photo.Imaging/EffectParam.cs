using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Jermismo.Photo.Imaging
{

    /// <summary>
    /// The type of effect parameter.
    /// </summary>
    public enum EffectParamType { Range, List, ButtonList, Curves, Text, LineSelect }

    /// <summary>
    /// The base effect param class.
    /// </summary>
    public abstract class EffectParamBase
    {
        public string Name { get; protected set; }
        public EffectParamType Type { get; protected set; }
    }

    /// <summary>
    /// A range of float values.
    /// </summary>
    public class RangeParam : EffectParamBase
    {
        public float Start { get; private set; }
        public float End { get; private set; }
        public float Value { get; set; }
        public RangeParam(string name, float start, float end, float value)
        {
            Name = name;
            this.Type = EffectParamType.Range;
            Start = start;
            End = end;
            Value = value;
        }
    }

    /// <summary>
    /// A list of string values.
    /// </summary>
    public class ListParam : EffectParamBase
    {
        public string[] Items { get; private set; }
        public string Selected { get; set; }
        public ListParam(string name, string[] items, string selected)
        {
            Name = name;
            this.Type = EffectParamType.List;
            Items = items;
            Selected = selected;
        }
    }

    /// <summary>
    /// Enum for which side of the button the icon goes on.
    /// </summary>
    public enum ButtonIconLayout { Left, Right }

    /// <summary>
    /// Describes a button for use in a button list.
    /// </summary>
    public class ButtonListParamItem
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public ButtonIconLayout IconLayout { get; private set; }
        public ButtonListParamItem(string name, string icon, ButtonIconLayout iconLayout)
        {
            Name = name;
            Icon = icon;
            IconLayout = iconLayout;
        }
    }

    /// <summary>
    /// A button with an icon.
    /// </summary>
    public class ButtonListParam : EffectParamBase
    {
        public ReadOnlyCollection<ButtonListParamItem> Buttons { get; private set; }
        public string Selected { get; set; }
        public ButtonListParam(string name, ButtonListParamItem[] items)
        {
            Name = name;
            this.Type = EffectParamType.ButtonList;
            Buttons = new ReadOnlyCollection<ButtonListParamItem>(items);
            Selected = string.Empty;
        }
    }

    /// <summary>
    /// Parameter for holding curves values.
    /// </summary>
    public class CurvesParam : EffectParamBase
    {
        public Point[] Lightness { get; set; }
        public Point[] Red { get; set; }
        public Point[] Green { get; set; }
        public Point[] Blue { get; set; }
        public CurvesParam()
        {
            Name = "Curves";
            this.Type = EffectParamType.Curves;
        }
    }

    /// <summary>
    /// Parameter for holding text values.
    /// </summary>
    public class TextParam : EffectParamBase
    {
        public string Text { get; set; }
        public bool ReadOnly { get; set; }
        public TextParam(string name, bool readOnly)
        {
            Name = name;
            this.Type = EffectParamType.Text;
            Text = string.Empty;
            ReadOnly = readOnly;
        }
    }

    public class LineSelectParam : EffectParamBase
    {
        public float Offset { get; set; }
        public Orientation Orientation { get; set; }
        public LineSelectParam(float offset, Orientation orientation)
        {
            Name = "LineSelect";
            this.Type = EffectParamType.LineSelect;
            Offset = offset;
            Orientation = orientation;
        }
    }

}
