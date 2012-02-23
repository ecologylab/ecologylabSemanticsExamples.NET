using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSReader.ViewModels
{
  class FeedViewModel : ViewModelBase
  {

    public virtual string Creator { get; set; }

    public virtual string Subject { get; set; }

    public virtual string Description { get; set; }

    public virtual string Title { get; set; }

    public virtual DateTime Date { get; set; }

    public virtual Uri Location { get; set; }

  }
}
