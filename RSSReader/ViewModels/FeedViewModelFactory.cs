using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ecologylab.semantics.generated.library;
using ecologylab.semantics.generated.library.rss;
using ecologylab.semantics.metadata.builtins;

namespace RSSReader.ViewModels
{
  class FeedViewModelFactory
  {

    public static IEnumerable<FeedViewModel> GetFeedViewModels(Document feedDoc)
    {
      if (feedDoc is Rss)
        return GetFeedViewModels((feedDoc as Rss).Channel.Items);
      else if (feedDoc is Rss22)
        return GetFeedViewModels((feedDoc as Rss22).Items);
      return null;
    }

    private static IEnumerable<FeedViewModel> GetFeedViewModels(IEnumerable<Item> items)
    {
      var viewModels = new ObservableCollection<RssItemFeedViewModel>();
      foreach (var item in items)
      {
        viewModels.Add(new RssItemFeedViewModel(item));
      }
      return viewModels;
    }

  }
}
