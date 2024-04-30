using System;
using System.Windows;
using System.Windows.Input;

namespace SixR_20.Commands
{
    class M1MoveCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (parameter != null && parameter.ToString() != "")
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            MessageBox.Show("HI!");
        }

        public event EventHandler CanExecuteChanged;
    }
}
