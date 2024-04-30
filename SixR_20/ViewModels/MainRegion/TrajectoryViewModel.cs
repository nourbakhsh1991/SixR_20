using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FirstFloor.ModernUI.Presentation;
using FontAwesome.WPF;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using SixR_20.Bootstrapper;
using SixR_20.Interpreter;
using SixR_20.Library;
using SixR_20.Models;

namespace SixR_20.ViewModels
{
    class TrajectoryViewModel : BaseViewModel
    {
        private SixRGrammerVisitor visitor;
        public bool isCodeRunning = false;
        public static AutoResetEvent SleepEvent = new AutoResetEvent(false);
        private string _gcodeText;
        private IHighlightingDefinition _textEditorHighlighter;
        public static ColorizeAvalonEdit ColoringLine = new ColorizeAvalonEdit();
        private bool _canOpenFile;
        private bool _canSaveFile;
        private bool _canCheckCode;
        private bool _canRunCode;
        private bool _canStopCode;
        public static TextEditor TextEditor;
        private Thread ExecutiveThread;
        private FontAwesome.WPF.FontAwesome _playIcon;
        public string GcodeText
        {
            get
            { return _gcodeText; }
            set { _gcodeText = value; OnPropertyChanged(nameof(GcodeText)); }
        }

        public IHighlightingDefinition TextEditorHighlighter
        {
            get { return _textEditorHighlighter; }
            set { _textEditorHighlighter = value; OnPropertyChanged(nameof(TextEditorHighlighter)); }
        }
        public ICommand OpenFileCommand { get; internal set; }
        public ICommand SaveFileCommand { get; internal set; }
        public ICommand CheckCodeCommand { get; internal set; }
        public ICommand RunCodeCommand { get; internal set; }
        public ICommand StopCodeCommand { get; internal set; }
        public ICommand TestChangeCommand { get; internal set; }
        public TrajectoryViewModel(IUnityContainer container,TextEditor editor=null)
        {
            TextEditor = editor;
            TextEditor?.TextArea.TextView.LineTransformers.Add(ColoringLine);
            this.Initialize(container);
        }
        private void Initialize(IUnityContainer container) 
        {
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("SixR_20.Resources.Gcode.Xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {

                    TextEditorHighlighter = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            HighlightingManager.Instance.RegisterHighlighting("GCODE", new string[] { ".gcode" }, TextEditorHighlighter);
            GcodeText = "int Main()\r\n\r\nend";
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            _canOpenFile = true;
            SaveFileCommand = new RelayCommand(SaveFile, CanSaveFile);
            _canSaveFile = true;
            CheckCodeCommand = new RelayCommand(CheckCode,CanCheckCode);
            _canCheckCode = true;
            RunCodeCommand = new RelayCommand(RunCode, CanRunCode);
            _canRunCode = false;
            StopCodeCommand = new RelayCommand(StopCode, CanStopCode);
            _canStopCode = false;
            TestChangeCommand =new DelegateCommand(TextEditorTextChange);
            Application.Current.Dispatcher.Invoke(new System.Action(() =>
            {
                BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
            }));
        }
        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case "Pulse":
                    if (SixRGrammerVisitor.Finished)
                        SleepEvent.Set();
                    break;
                case "GUI_Alarms":
                    var alarms = BeckhoffContext.Controller.GUI_Alarms;
                    int alarmNumber = alarms.Count(alarm => alarm != 0);
                    if (alarmNumber > 0)
                    {
                        if (ExecutiveThread != null)
                            ExecutiveThread.Abort();
                        SleepEvent.Reset();
                        isCodeRunning = false;
                    }
                    break;

            }


        }
        public void TextEditorTextChange()
        {
            _canCheckCode = true;
            _canRunCode = false;
            _canStopCode = false;
            CommandManager.InvalidateRequerySuggested();
        }
        private void OpenFile(object obj)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Gcode (*.gcode) |*.gcode"
            };
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                GcodeText = File.ReadAllText(ofd.FileName);
            }
            _canCheckCode = true;
            _canRunCode = false;
            _canStopCode = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanOpenFile(object obj)
        {
            return _canOpenFile;
        }
        private void SaveFile(object obj)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Gcode (*.gcode) |*.gcode"
            };
            var result = sfd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                File.WriteAllText(sfd.FileName, GcodeText);
            }
        }

        private bool CanSaveFile(object obj)
        {
            return _canSaveFile;
        }
        private void CheckCode(object obj)
        {
            _canCheckCode = false;
            _canRunCode = false;
            _canStopCode = false;
            CommandManager.InvalidateRequerySuggested();
            var errorListener = new ErrorListener();

            var ais = new AntlrInputStream(GcodeText);
            var cl = new SixRGrammerLexer(ais);
            var cts = new CommonTokenStream(cl);
            var cp = new SixRGrammerParser(cts);
            ErrorListener.AllErrors = "";
            cp.AddErrorListener(errorListener);
            IParseTree tree = cp.start();
            if (ErrorListener.AllErrors == "")
            {
                _canCheckCode = true;
                _canRunCode = true;
                _canStopCode = true;
                CommandManager.InvalidateRequerySuggested();
            }
            else
            {
                MessageBox.Show(ErrorListener.AllErrors, "Syntax Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanCheckCode(object obj)
        {
            return _canCheckCode;
        }
        private void RunCode(object obj)
        {
            var content = obj as FontAwesome.WPF.FontAwesome;
            if (content != null) _playIcon = content;
            _canCheckCode = false;
            CommandManager.InvalidateRequerySuggested();
            if (!isCodeRunning && ((visitor != null && !visitor.isPuased) || visitor == null))
            {
                if (content != null)
                {
                    content.Icon = FontAwesomeIcon.Pause;
                }
            }
            else if (isCodeRunning && visitor != null && !visitor.isPuased)
            {
                if (content != null)
                {
                    content.Icon = FontAwesomeIcon.Play;
                }
                visitor.isPuased = true;
                return;
            }
            else if (isCodeRunning && visitor != null && visitor.isPuased)
            {
                if (content != null)
                {
                    content.Icon = FontAwesomeIcon.Pause;
                }
                visitor.isPuased = false;
                SleepEvent.Set();
                return;
            }
            isCodeRunning = true;
            var txt = TextEditor.Text;

            var ais = new AntlrInputStream(txt);
            var cl = new SixRGrammerLexer(ais);
            var cts = new CommonTokenStream(cl);
            var cp = new SixRGrammerParser(cts);
            IParseTree tree = cp.start();
            visitor = new SixRGrammerVisitor(BeckhoffContext.Controller, BeckhoffContext.Traj) { RunCode = true};
            ExecutiveThread = new Thread(() =>
            {

                visitor.Visit(tree);
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    isCodeRunning = false;
                    ColoringLine.curtLine = -1;
                    TextEditor.TextArea.TextView.Redraw();
                    if (content != null)
                    {
                        content.Icon = FontAwesomeIcon.Play;
                    }
                    _canCheckCode = true;
                    CommandManager.InvalidateRequerySuggested();
                    SleepEvent.Reset();
                }));
            });
            ExecutiveThread.Start();
        }

        private bool CanRunCode(object obj)
        {
            return _canRunCode;
        }
        private void StopCode(object obj)
        {
            ColoringLine.curtLine = -1;
            TextEditor.TextArea.TextView.Redraw();
            if (isCodeRunning && !visitor.isStoped)
            {
                visitor.isStoped = true;
                SleepEvent.Reset();
                isCodeRunning = false;
                if (ExecutiveThread != null)
                    ExecutiveThread.Abort();
                if (_playIcon != null)
                {
                    _playIcon.Icon = FontAwesomeIcon.Play;
                }
                _canCheckCode = true;
                CommandManager.InvalidateRequerySuggested();
                return;
            }
            if (isCodeRunning && visitor.isStoped)
            {
                visitor.isStoped = false;
                return;
            }
        }

        private bool CanStopCode(object obj)
        {
            return _canStopCode;
        }
    }
}
