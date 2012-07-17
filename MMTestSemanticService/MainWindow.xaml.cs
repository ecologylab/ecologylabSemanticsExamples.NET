using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;

using Simpl.Fundamental.Net;
using Simpl.Serialization;
using ecologylab.semantics.collecting;
using ecologylab.semantics.generated.library;
using ecologylab.semantics.metadata.builtins;
using ecologylab.semantics.metametadata;
using ecologylab.semantics.services;
using ecologylab.semantics.generated.library.products;
using ecologylab.interactive.semantics.View;

namespace MMTestSemanticService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MetadataServicesClient metadataServiceClient;

        SemanticsGlobalCollection<Document> globalColection; 

        public MainWindow()
        {
            InitializeComponent();

            globalColection = new SemanticsGlobalCollection<Document>();

            metadataServiceClient = new MetadataServicesClient(RepositoryMetadataTranslationScope.Get());

            metadataServiceClient.metadataDownloadComplete += MetadataDownloadComplete;
        }

        private async void BtnGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            string urls = UrlBox.Text;
            List<Task<Document>> extractionRequests = new List<Task<Document>>();

            foreach (var url in urls.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Console.WriteLine("Requesting async extraction of: " + url);

                ParsedUri puri = new ParsedUri(url);

                Document document = null;
                globalColection.TryGetDocument(puri, out document);

                if (document == null)
                {
                    globalColection.AddDocument(new Document(), puri);

                    Task<Document> t = metadataServiceClient.GetMetadata(url);

                    extractionRequests.Add(t);
                }
                else
                {
                    MetadataDownloadComplete(this, new MetadataServicesClient.MetadataEventArgs(document));
                }
            }

            while (extractionRequests.Count > 0)
            {
                Task<Document> completedTask = await TaskEx.WhenAny(extractionRequests);
                extractionRequests.Remove(completedTask);

                Document parsedDoc = await completedTask;
/*                if (parsedDoc == null)
                    continue;

                Expander expander = new Expander { Header = parsedDoc.Title };
                TextBox metadataXML = new TextBox { TextWrapping = TextWrapping.Wrap, MinHeight = 100 };

                var s = timeStamps[parsedDoc.Location.Value.ToString()];
                Console.WriteLine(" ---------------------------------- Time to complete: " + DateTime.Now.Subtract(s).TotalMilliseconds);
                metadataXML.Text = await TaskEx.Run(() => SimplTypesScope.Serialize(parsedDoc, StringFormat.Xml));

                expander.Content = metadataXML;
                MetadataTitleXMLContainer.Children.Add(expander);
*/ 
            }

        }

        private void MetadataDownloadComplete(object sender, MetadataServicesClient.MetadataEventArgs args)
        {
            if (args.Metadata == null)
                return;
            
            Document document = null;
            globalColection.TryGetDocument(args.Metadata.Location.Value, out document);

            if (document == null)
                globalColection.AddDocument(args.Metadata, args.Metadata.Location.Value);
            else
                globalColection.Remap(document, args.Metadata);

            // new AmazonProductView((args.Metadata as AmazonProduct));
        }

    }
}
