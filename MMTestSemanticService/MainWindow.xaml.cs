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
        SemanticsSessionScope _semanticsSessionScope;

        SemanticsGlobalCollection<Document> downloadedDocumentCollection;

        //MetadataServicesClient metadataServiceClient;

        public MainWindow()
        {
            InitializeComponent();

            BtnGetMetadata.IsEnabled = false;
            Loaded += MainWindow_Loaded;
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SimplTypesScope _repositoryMetadataTranslationScope = RepositoryMetadataTranslationScope.Get();

                _semanticsSessionScope = await SemanticsSessionScope.InitAsync(
                                            _repositoryMetadataTranslationScope,
                                            MetaMetadataRepositoryInit.DEFAULT_REPOSITORY_LOCATION);

                //metadataServiceClient = new MetadataServicesClient(_repositoryMetadataTranslationScope);
                //metadataServiceClient.metadataDownloadComplete += MetadataDownloadComplete;
                downloadedDocumentCollection = new SemanticsGlobalCollection<Document>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeception: " + ex.StackTrace);

            }

            BtnGetMetadata.IsEnabled = true;
        }

        private async void BtnGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            string urls = UrlBox.Text;
            List<DocumentClosure> documentCollection = new List<DocumentClosure>();

            foreach (var url in urls.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Document document = _semanticsSessionScope.GetOrConstructDocument(new ParsedUri(url));
                DocumentClosure documentClosure = document.GetOrConstructClosure(_semanticsSessionScope.MetadataServicesClient, downloadedDocumentCollection);

                documentCollection.Add(documentClosure);
            }

            foreach (var documentClosure in documentCollection)
            {
                documentClosure.Continuations += this.MetadataDownloadComplete;
                documentClosure.GetMetadata();
            }
        }

        private void MetadataDownloadComplete(object sender, DocumentClosureEventArgs args)
        {
            if (args.DocumentClosure.Document == null)
                return;

            Console.WriteLine("downloaded metadata: " + args.DocumentClosure.Document.Location);
            MetadataTitleXMLContainer.Children.Add(new MetadataBrowserEditorView(args.DocumentClosure.Document));
        }

    }
}
