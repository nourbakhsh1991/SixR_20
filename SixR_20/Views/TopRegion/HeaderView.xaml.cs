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
using SixR_20.ViewModels.Shared;
using SixR_20.ViewModels.TopRegion;

namespace SixR_20.Views.TopRegion
{
    /// <summary>
    /// Interaction logic for HeaderView.xaml
    /// </summary>
    public partial class HeaderView : UserControl
    {
        public HeaderView(IUnityContainer container)
        {
            InitializeComponent();
            DataContext = new HeaderViewModel(container);
        }
    }
}
