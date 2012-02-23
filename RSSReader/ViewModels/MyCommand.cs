using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RSSReader.ViewModels
{
  internal class MyCommand : ICommand
  {

    private Action<object>      _action;

    private Func<object, bool>  _checkExecutable;

    public MyCommand(Action<object> action, Func<object, bool> checkExecutable = null)
    {
      this._action = action;
      this._checkExecutable = checkExecutable;
    }

    public void Execute(object parameter)
    {
      this._action(parameter);
    }

    public bool CanExecute(object parameter)
    {
      return this._checkExecutable == null || this._checkExecutable(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

  }
}
