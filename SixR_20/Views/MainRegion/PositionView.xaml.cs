using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using SixR_20.ViewModels.MainRegion;

namespace SixR_20.Views.MainRegion
{
    /// <summary>
    /// Interaction logic for PositionView.xaml
    /// </summary>
    public partial class PositionView : UserControl
    {
        public PositionView(IUnityContainer container)
        {
            InitializeComponent();
            DataContext = new PositionViewModel(container);
        }
    }
}
