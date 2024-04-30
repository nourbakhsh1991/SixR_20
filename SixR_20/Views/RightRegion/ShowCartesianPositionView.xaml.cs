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
using SixR_20.ViewModels.RightRegion;

namespace SixR_20.Views.RightRegion
{
    /// <summary>
    /// Interaction logic for ShowCartesianPositionView.xaml
    /// </summary>
    public partial class ShowCartesianPositionView : UserControl
    {
        public ShowCartesianPositionView(IUnityContainer container)
        {
            InitializeComponent();
            DataContext = new ShowCartesianPositionViewModel(container);
        }
    }
}
