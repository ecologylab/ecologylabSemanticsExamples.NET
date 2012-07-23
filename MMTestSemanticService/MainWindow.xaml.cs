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

        MetadataServicesClient metadataServiceClient;

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

                metadataServiceClient = new MetadataServicesClient(_repositoryMetadataTranslationScope);
                metadataServiceClient.metadataDownloadComplete += MetadataDownloadComplete;
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

            foreach (var url in urls.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                metadataServiceClient.GetMetadata(url);
            }
        }

        private void MetadataDownloadComplete(object sender, MetadataServicesClient.MetadataEventArgs args)
        {
            if (args.Metadata == null)
                return;

            Console.WriteLine("downloaded metadata: " + args.Metadata.Location);   
            new AmazonProductView((args.Metadata as AmazonProduct));
        }

    }
}
