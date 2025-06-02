using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Wa3Tuner.Dialogs
{
    public partial class TextColorizer_Window : Window
    {
        private List<TextChunk> Chunks = new List<TextChunk>();
        private string CurrentText = "";

        public TextColorizer_Window()
        {
            InitializeComponent();
        }

        class TextChunk
        {
            public int FromPosition;
            public int ToPosition;
            public Color Color;
            public TextChunk(int from, int to, Color color)
            {
                this.Color = color;
                this.FromPosition = from;
                this.ToPosition = to;
            }
        }

        private void CopyTexts(object? sender, RoutedEventArgs? e)
        {
            string final = GenerateFinalTextForWarcraft3();
            Clipboard.SetText(final);
        }

        private string GenerateFinalTextForWarcraft3()
        {
            RecalculateChunks();
            string finalText = "";
            int lastIndex = 0;

            foreach (var chunk in Chunks)
            {
                if (chunk.FromPosition > lastIndex)
                {
                    finalText += CurrentText.Substring(lastIndex, chunk.FromPosition - lastIndex);
                }

                string hexColor = $"|cFF{chunk.Color.R:X2}{chunk.Color.G:X2}{chunk.Color.B:X2}";
                finalText += $"{hexColor}{CurrentText.Substring(chunk.FromPosition, chunk.ToPosition - chunk.FromPosition)}|r";
                lastIndex = chunk.ToPosition;
            }

            if (lastIndex < CurrentText.Length)
            {
                finalText += CurrentText.Substring(lastIndex);
            }

            return finalText;
        }

        private void RecalculateChunks()
        {
            Chunks.Sort((a, b) => a.FromPosition.CompareTo(b.FromPosition));
            List<TextChunk> mergedChunks = new List<TextChunk>();

            foreach (var chunk in Chunks)
            {
                if (mergedChunks.Count > 0 && mergedChunks[^1].ToPosition >= chunk.FromPosition)
                {
                    mergedChunks[^1].ToPosition = Math.Max(mergedChunks[^1].ToPosition, chunk.ToPosition);
                }
                else
                {
                    mergedChunks.Add(chunk);
                }
            }

            Chunks = mergedChunks;
        }

        private void SetTextInRichTextBox2(object? sender, TextChangedEventArgs e)
        {
            RefreshRichTextBox();
        }

        private void RefreshRichTextBox()
        {
            if (MainTextBox == null) return;
            CurrentText = GetRawText();
            if (CurrentText.Length == 0) return;

            RecalculateChunks();
            MainTextBox.Document.Blocks.Clear();

            if (Chunks.Count == 0)
            {
                MainTextBox.Document.Blocks.Add(new Paragraph(new Run(CurrentText)));
                return;
            }
           

            Paragraph paragraph = new Paragraph();
            int lastIndex = 0;

            foreach (var chunk in Chunks)
            {
                if (chunk.FromPosition < 0 || chunk.ToPosition >= CurrentText.Length) continue;
                if (chunk.FromPosition > lastIndex)
                {
                    paragraph.Inlines.Add(new Run(CurrentText.Substring(lastIndex, chunk.FromPosition - lastIndex)));
                }

                Run coloredRun = new Run(CurrentText.Substring(chunk.FromPosition, chunk.ToPosition - chunk.FromPosition))
                {
                    Foreground = new SolidColorBrush(chunk.Color)
                };
                paragraph.Inlines.Add(coloredRun);

                lastIndex = chunk.ToPosition;
            }

            if (lastIndex < CurrentText.Length)
            {
                paragraph.Inlines.Add(new Run(CurrentText.Substring(lastIndex)));
            }

            MainTextBox.Document.Blocks.Add(paragraph);
        }

        private void SetColor1(object? sender, RoutedEventArgs? e)
        {
            if (GetSelectedTextLength() == 0) return;
            ApplySelectedColor(ButtonColor1Select.Background);
        }

        private void SetColor2(object? sender, RoutedEventArgs? e)
        {
            if (GetSelectedTextLength() == 0) return;
            ApplySelectedColor(ButtonColor2Select.Background);
        }

        public int GetSelectedTextLength()
        {
            if (MainTextBox == null || MainTextBox.Selection.IsEmpty)
                return 0;

            TextRange selectionRange = new TextRange(MainTextBox.Selection.Start, MainTextBox.Selection.End);
            return selectionRange.Text.Length;
        }

        private void ApplySelectedColor(Brush selectedBrush)
        {
            if (!(selectedBrush is SolidColorBrush solidBrush))
                return;

            if (MainTextBox.Selection.IsEmpty)
                return;

            int start = GetTextPosition(MainTextBox.Selection.Start);
            int end = GetTextPosition(MainTextBox.Selection.End);

            if (start == end) return; // No actual selection

            if (Chunks.Any(x => x.FromPosition == start && x.ToPosition == end)) return;

            Chunks.Add(new TextChunk(start, end, solidBrush.Color));
            RefreshRichTextBox();
        }

        private int GetTextPosition(TextPointer position)
        {
            TextRange range = new TextRange(MainTextBox.Document.ContentStart, position);
            return range.Text.Length;
        }

        private void SelectColor1(object? sender, RoutedEventArgs? e)
        {
            Brush newColor = ColorPickerHandler.Pick(ButtonColor1Select.Background);
            ButtonColor1Select.Background = newColor;
        }

        private void SelectColor2(object? sender, RoutedEventArgs? e)
        {
            Brush newColor = ColorPickerHandler.Pick(ButtonColor2Select.Background);
            ButtonColor2Select.Background = newColor;
        }

        private void ClearAllColors(object? sender, RoutedEventArgs? e)
        {
            Chunks.Clear();
            RefreshRichTextBox();
        }

        private void ClearSelection(object? sender, RoutedEventArgs? e)
        {
            if (MainTextBox.Selection.IsEmpty) return;

            int start = GetTextPosition(MainTextBox.Selection.Start);
            int end = GetTextPosition(MainTextBox.Selection.End);

            Chunks.RemoveAll(chunk => chunk.FromPosition >= start && chunk.ToPosition <= end);
            RefreshRichTextBox();
        }

        public string GetRawText()
        {
            return GetRichTextBoxText(MainTextBox);
        }

        public string GetRichTextBoxText(RichTextBox rtb)
        {
            if (rtb == null || rtb.Document == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (Block block in rtb.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run run)
                        {
                            sb.Append(run.Text);
                        }
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private void ClearSeletion(object? sender, RoutedEventArgs? e)
        {

        }

        private void SetGradient(object? sender, RoutedEventArgs? e)
        {
            if (GetSelectedTextLength() == 0) return;
            ClearChunksBetween(MainTextBox.Selection.Start, MainTextBox.Selection.End);

            RefreshRichTextBox();
        }

        private void ClearChunksBetween(TextPointer start, TextPointer end)
        {
            if (start == null || end == null) return;

            int from = start.DocumentStart.GetOffsetToPosition(start);
            int to = start.DocumentStart.GetOffsetToPosition(end);

            Chunks.RemoveAll(x => x.FromPosition >= from && x.ToPosition <= to);
        }

        private void Window_KeyDown(object? sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            
        }
    }
}
