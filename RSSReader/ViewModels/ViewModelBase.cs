using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RSSReader.ViewModels
{
  class ViewModelBase : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

  }
}
