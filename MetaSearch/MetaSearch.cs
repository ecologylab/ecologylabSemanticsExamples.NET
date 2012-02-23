using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;
using Simpl.Serialization;
using Simpl.Serialization.Attributes;
using ecologylab.semantics.generated.library.search;

namespace MetaSearch
{
  [SimplTag("meta_search")]
  internal class MetaSearch : ElementState
  {

    private MMDExtractionBrowser _browser;

    private List<Search> _searches;

    [SimplCollection("result")] private List<SearchResult> _results;

    public MetaSearch()
    {
      _browser = new MMDExtractionBrowser();
      _searches = new List<Search>();
    }

    public async void Search(List<string> urls)
    {
      foreach (string url in urls)
      {
        var search = (Search) await _browser.ExtractMetadata(new ParsedUri(url));
        _searches.Add(search);
      }

      int i = 0;
      while (true)
      {
        bool resultsAdded = false;
        for (int j = 0; j < _searches.Count; ++j)
        {
          List<SearchResult> searchResults = _searches[j].SearchResults;
          if (searchResults != null && i < searchResults.Count)
          {
            _results.Add(searchResults[i]);
            resultsAdded = true;
          }
        }
        if (resultsAdded)
          ++i;
        else
          break;
      }
    }

    private static void Main(string[] args)
    {
      if (args.Length < 2)
      {
        Console.Error.WriteLine("args: <output-html-file-path> <search-url> [<search-url> ...]");
        Environment.Exit(-1);
      }

      String outFilePath = args[0];
      var urls = new List<string>();
      for (int i = 1; i < args.Length; ++i)
      {
        string url = args[i].Trim();
        if (url != "//")
          urls.Add(url);
      }

      var metaSearch = new MetaSearch();
      metaSearch.Search(urls);
      SimplTypesScope.Serialize(metaSearch, new StreamWriter("metaSearch.xml"), StringFormat.Xml);
    }

  }
}
