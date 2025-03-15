using Dsafa.WpfColorPicker;
using System;
using System.Windows.Media;

namespace Wa3Tuner
{
    internal class ColorPickerHandler
    {
        internal static Brush Pick(Brush background)
        {
            var initialColor =    Calculator.BrushToColor(background);


            var dialog = new ColorPickerDialog(initialColor);
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var newColor = dialog.Color;
                Brush converted = Calculator.ColorToBrush(newColor);
                return converted;
            }

            return background;
        }
    }
}