using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace SixR_20.Library
{
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private TextEditor _editor;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
        {
            _editor = editor;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Caret; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(
                    new SolidColorBrush(Color.FromArgb(0x40, 0, 0, 0xFF)), null,
                    new Rect(rect.Location, new Size(textView.ActualWidth - 32, rect.Height)));
            }
        }
    }
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        public int curtLine;
        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.LineNumber != curtLine)
                return;
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index = 0;
            //while ((index = text.IndexOf("AvalonEdit", start)) >= 0)
            //{

            base.ChangeLinePart(
                lineStartOffset, // startOffset
                lineStartOffset + text.Length, // endOffset

                (VisualLineElement element) => {
                        // This lambda gets called once for every VisualLineElement
                        // between the specified offsets.
                        Typeface tf = element.TextRunProperties.Typeface;
                        // Replace the typeface with a modified version of
                        // the same typeface
                        element.TextRunProperties.SetBackgroundBrush(new SolidColorBrush(Colors.DimGray));
                    element.TextRunProperties.SetTypeface(new Typeface(
                                    tf.FontFamily,
                                    FontStyles.Italic,
                                    FontWeights.Bold,
                                    tf.Stretch
                                ));
                });
            start = index + 1; // search for next occurrence
            //}
        }
    }
}
