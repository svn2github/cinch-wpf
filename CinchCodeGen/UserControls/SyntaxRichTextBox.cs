using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Documents;

namespace CinchCodeGen
{
    /// <summary>
    /// An extremely simple syntax highlighting RichTextBox
    /// This class was obtained from Tamir Khasons blog, and
    /// subsequently modified slightly
    /// </summary>
    public class SyntaxRichTextBox : RichTextBox
    {
        #region Data
        private enum TokenType { KeyWord, Other };
        private String currentFileName = String.Empty;
        private ISyntaxChecker syntaxChecker = null;
        private List<Tag> m_tags = new List<Tag>();
        #endregion

        #region Nested Types
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;
        }
        #endregion

        #region Ctor
        public SyntaxRichTextBox()
        {
            this.Background=Brushes.Black;
            this.Foreground=Brushes.White;
            this.BorderBrush=Brushes.Transparent;
            this.BorderThickness=new Thickness(0);
            this.IsReadOnly=true;
            this.Document.MinPageWidth=1500;
            this.Document.MaxPageWidth=1500;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads file contents in
        /// </summary>
        private void LoadFileContents()
        {
            using (StreamReader sr = new StreamReader(CurrentFileName))
            {
                String code = sr.ReadToEnd();

                if (code != null && code.Length > 0)
                {
                    this.Document.Blocks.Clear();
                    this.Document.Blocks.Add(
                        new Paragraph(new Run(code)));
                }
            }
        }

        /// <summary>
        /// Applies the formatting, and formats
        /// </summary>
        private void ApplyFormatting()
        {
            if (this.Document == null)
                return;

            TextRange documentRange = new TextRange(
                this.Document.ContentStart, this.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = this.Document.ContentStart;
            while (navigator.CompareTo(this.Document.ContentEnd) < 0)
            {
                TextPointerContext context = 
                    navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart 
                    && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);

                }
                navigator = navigator.GetNextContextPosition(
                    LogicalDirection.Forward);
            }
            //now format
            Format();
        }

        /// <summary>
        /// Format the selected key words found
        /// </summary>
        private void Format()
        {

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(
                    m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(
                    TextElement.ForegroundProperty, 
                    new SolidColorBrush(Colors.Orange));
            }
            m_tags.Clear();

        }

        /// <summary>
        /// Try and identify key words in the document run
        /// </summary>
        private void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | 
                    syntaxChecker.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | 
                        syntaxChecker.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        CreateTag(run, sIndex, eIndex, word);
                    }
                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            CreateTag(run, sIndex, eIndex, lastWord);
        }


        /// <summary>
        /// Creates a known tag, which will later be used for formatting
        /// </summary>
        private void CreateTag(Run run, int sIndex, int eIndex, string word)
        {
            if (syntaxChecker.IsKnownTag(word))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(
                    sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(
                    eIndex + 1, LogicalDirection.Backward);
                t.Word = word;
                m_tags.Add(t);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the file contents and applies the formatting based
        /// on the <c>ISyntaxChecker</c> passed in
        /// </summary>
        public void LoadAndAnalyzeText(ISyntaxChecker syntaxChecker)
        {
            this.syntaxChecker = syntaxChecker;
            LoadFileContents();
            TextRange range = new TextRange(this.Document.ContentStart, 
                this.Document.ContentEnd);
            range.ClearAllProperties();
            this.ApplyFormatting();
        }


        #endregion

        #region Public Properties

        public String CurrentFileName
        {
            get { return currentFileName; }
            set
            {
                currentFileName = value;
            }
        }

     

        #endregion
    }
}
