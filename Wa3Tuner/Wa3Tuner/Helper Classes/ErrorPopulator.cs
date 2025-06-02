using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace Wa3Tuner.Helper_Classes
{
    public static class ErrorPopulator
    {

        public static void ReportErrorsWithText(
       RichTextBox box,
       string unusedText,
       string warningText,
       string criticalText,
       string errorText)
        {
            box.Document.Blocks.Clear();

            if (string.IsNullOrEmpty(unusedText) && string.IsNullOrEmpty(warningText) &&
                string.IsNullOrEmpty(criticalText) && string.IsNullOrEmpty(errorText))
            {
                var paragraph = new Paragraph(new Run("ALL OK."));
                box.Document.Blocks.Add(paragraph);
                return;
            }

            AddCategory(box, "Unused", Colors.Green, unusedText);
            AddCategory(box, "Warning", Colors.Goldenrod, warningText);
            AddCategory(box, "Severe", Colors.Orange, criticalText);
            AddCategory(box, "Errors", Colors.Red, errorText);
        }


        private static void AddCategory(RichTextBox box, string title, Color color, string categoryText)
        {
            var titlePara = new Paragraph();

            // Count non-empty lines in the category text
            int count = 0;
            if (!string.IsNullOrWhiteSpace(categoryText))
            {
                count = categoryText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
            }

            // Compose title with count, e.g. "Unused (3)"
            var fullTitle = $"{title} ({count})";

            var underline = new Underline();
            var titleRun = new Run(fullTitle)
            {
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(color)
            };

            underline.Inlines.Add(titleRun);
            titlePara.Inlines.Add(underline);

            box.Document.Blocks.Add(titlePara);

            if (count > 0)
            {
                var lines = categoryText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var para = new Paragraph(new Run(line));
                    box.Document.Blocks.Add(para);
                }
            }
        }
    }
}
