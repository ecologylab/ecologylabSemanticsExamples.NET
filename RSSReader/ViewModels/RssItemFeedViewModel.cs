using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;
using ecologylab.semantics.generated.library.rss;
using ecologylab.semantics.metadata.scalar;

namespace RSSReader.ViewModels
{
    internal class RssItemFeedViewModel : FeedViewModel
    {

        private Item _item;

        public RssItemFeedViewModel(Item item)
        {
            this._item = item;
        }

        public override string Creator
        {
            get { return _item.DcCreator == null ? null : _item.DcCreator.Value; }
            set
            {
                _item.DcCreator.Value = value;
                this.NotifyPropertyChanged("Creator");
            }
        }

        public override string Subject
        {
            get { return _item.DcSubject == null ? _item.Categories == null ? null : CategoriesToSubjectLine(_item.Categories) : _item.DcSubject.Value; }
            set
            {
                _item.DcSubject.Value = value;
                this.NotifyPropertyChanged("Subject");
            }
        }

        private static string CategoriesToSubjectLine(List<MetadataString> categories)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < categories.Count; ++i)
            {
                sb.Append(i > 0 ? ", " : "").Append(categories[i].Value);
            }
            return sb.ToString();
        }

        public override string Description
        {
            get { return _item.DcDescription == null ? _item.Description == null ? null : _item.Description.Value : _item.DcDescription.Value; }
            set
            {
                _item.DcDescription.Value = value;
                this.NotifyPropertyChanged("Description");
            }
        }

        public override string Title
        {
            get { return _item.DcTitle == null ? _item.Title == null ? null : _item.Title.Value : _item.DcTitle.Value; }
            set
            {
                _item.DcTitle.Value = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        public override DateTime Date
        {
            get { return _item.DcDate == null ? DateTime.MinValue : _item.DcDate.Value; }
            set
            {
                _item.DcDate.Value = value;
                this.NotifyPropertyChanged("Date");
            }
        }

        public override Uri Location
        {
            get { return _item.Location == null ? _item.Link == null ? null : _item.Link.Value : _item.Location.Value; }
            set
            {
                _item.Location.Value = new ParsedUri(value.AbsoluteUri);
                this.NotifyPropertyChanged("Location");
            }
        }

    }
}