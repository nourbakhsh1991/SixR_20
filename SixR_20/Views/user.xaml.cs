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

namespace SixR_20.Views
{
    /// <summary>
    /// Interaction logic for user.xaml
    /// </summary>
    public partial class user : UserControl
    {
        public user(IUnityContainer container)
        {
            InitializeComponent();
            //((this.Parent as Shell).DataContext as ShellViewModel).HeaderText = "Position";
            DataContext=new HelloWorldViewModel(container);
        }
    }
}
