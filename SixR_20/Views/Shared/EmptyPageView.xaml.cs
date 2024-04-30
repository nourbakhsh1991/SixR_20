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
using SixR_20.ViewModels;
using SixR_20.ViewModels.Shared;

namespace SixR_20.Views.Shared
{
    /// <summary>
    /// Interaction logic for EmptyPageView.xaml
    /// </summary>
    public partial class EmptyPageView : UserControl
    {
        public EmptyPageView(IUnityContainer container)
        {
            InitializeComponent();
            DataContext = new EmptyPageViewModel(container);
        }
    }
}
