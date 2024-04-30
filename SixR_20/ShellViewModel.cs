using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using SixR_20.Annotations;
using SixR_20.Bootstrapper;
using SixR_20.Models;
using SixR_20.ViewModels;

namespace SixR_20
{
    class ShellViewModel : BaseViewModel
    {
        public ICommand keyDown { get; internal set; }
        public ICommand keyUp { get; internal set; }
        private string _headerText;
        private int _mainRegionColspan = 1;
        private int _mainRegionRowspan = 1;
        private int _topGridColspan = 2;
        private Thickness _mainRegionThickness = new Thickness(10, 10, 5, 5);
        private GridLength _leftGridWidth = new GridLength(0);
        public int MainRegionColspan
        {
            get { return _mainRegionColspan; }
            set { _mainRegionColspan = value; OnPropertyChanged(nameof(MainRegionColspan)); }
        }

        public int TopGridColspan
        {
            get { return _topGridColspan; }
            set { _topGridColspan = value; OnPropertyChanged(nameof(TopGridColspan)); }
        }
        public int MainRegionRowspan
        {
            get { return _mainRegionRowspan; }
            set { _mainRegionRowspan = value; OnPropertyChanged(nameof(MainRegionRowspan)); }
        }
        public string HeaderText
        {
            get { return _headerText; }
            set { _headerText = value; OnPropertyChanged(nameof(HeaderText)); }
        }
        public Thickness MainRegionThickness
        {
            get { return _mainRegionThickness; }
            set { _mainRegionThickness = value; OnPropertyChanged(nameof(MainRegionThickness)); }
        }
        public GridLength LeftGridWidth
        {
            get { return _leftGridWidth; }
            set { _leftGridWidth = value; OnPropertyChanged(nameof(LeftGridWidth)); }
        }
        public ShellViewModel(IUnityContainer container)
        {
            this.Initialize(container);
        }
        ~ShellViewModel()
        {
           // Application.Current.Dispatcher.Invoke(new Action(() =>
           // {
                BeckhoffContext.Controller.SetRegisterCtrWord(new ushort[] { 7, 7, 7, 7, 7, 7 });
                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetControlWord);
                BeckhoffContext.Controller.SetModesOfOperation(1);
                BeckhoffContext.Controller.SetCommand((int)CommandsEnum.SetMode);
           // }));
        }
        private void Initialize(IUnityContainer container)
        {
            MainRegionColspan = 1;
            MainRegionRowspan = 1;
            this.Container = container;
            var eventAggregator = container.Resolve<IEventAggregator>();
            var viewRequestedEvent = eventAggregator.GetEvent<ViewRequestedEvent>();
            viewRequestedEvent.Subscribe(this.ViewRequestedEventHandler, true);
        }

        private void ViewRequestedEventHandler(string s)
        {
            dynamic Command = JsonConvert.DeserializeObject(s);
            if (Command.command == "SetMainRegionColspan")
            {
                MainRegionColspan = Command.MainRegionColspan;
            }
            else if (Command.command == "SetMainRegionRowspan")
            {
                MainRegionRowspan = Command.MainRegionRowspan;
            }
            else if (Command.command == "SetMainRegionThickness")
            {
                MainRegionThickness = new Thickness(
                    (int)Command.MainRegionThickness.left,
                    (int)Command.MainRegionThickness.top,
                    (int)Command.MainRegionThickness.right,
                    (int)Command.MainRegionThickness.bottom);
            }
            else if (Command.command == "HideLeftGrid")
            {
                LeftGridWidth = new GridLength(0);
                TopGridColspan = 3;
                HeaderText = "Self Test Mode";
            }
            else if (Command.command == "ShowLeftGrid")
            {
                LeftGridWidth = new GridLength(2.5, GridUnitType.Star);
                TopGridColspan = 3;
                HeaderText = "Position Mode";
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    BeckhoffContext.Controller.PropertyChanged += Controller_PropertyChanged;
                }));
            }
            else if (Command.command == "ChangeHeader")
            {
                HeaderText = Command.Text.ToString();
            }

        }

        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BufferNumber":
                    if (BeckhoffContext.Controller.CurrentCommand == 4 && BeckhoffContext.modeOfGUI!=2)
                    {
                        BeckhoffContext.Controller.FillBuffer(BeckhoffContext.Controller.BufferNumber);
                        // _ctrlr.ReadBuffer(_ctrlr.BufferNumber);

                    }
                    if (BeckhoffContext.Controller.CurrentCommand == 13)
                    {
                        //BeckhoffContext.Controller.FillNextJogBuffer(BeckhoffContext.Controller.BufferNumber);
                    }
                    break;
            }
        }
    }
}

