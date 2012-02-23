using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using Simpl.Fundamental.Net;
using Simpl.Serialization;
using ecologylab.semantics.collecting;
using ecologylab.semantics.generated.library;
using ecologylab.semantics.metadata.builtins;
using ecologylab.semantics.metametadata;

namespace MmTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SemanticsSessionScope _semanticsSessionScope;

        public MainWindow()
        {
            InitializeComponent();

            BtnGetMetadata.IsEnabled = false;
            Loaded += MainWindow_Loaded;
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _semanticsSessionScope = await SemanticsSessionScope.InitAsync(
                                            RepositoryMetadataTranslationScope.Get(),
                                            MetaMetadataRepositoryInit.DEFAULT_REPOSITORY_LOCATION);
            
            BtnGetMetadata.IsEnabled = true;
        }


        private async void BtnGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            string urls = UrlBox.Text;
            List<Task<Document>> extractionRequests = new List<Task<Document>>();
            Dictionary<ParsedUri, DateTime> timeStamps = new Dictionary<ParsedUri, DateTime>();
            foreach (var s in urls.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Console.WriteLine("Requesting async extraction of: " + s);

                ParsedUri puri = new ParsedUri(s);
                {
                    Task<Document> t = _semanticsSessionScope.GetDocument(puri);
                    timeStamps.Add(puri, DateTime.Now);
                    //Alternate, if you want the document here:
                    //Document doc = await _semanticsSessionScope.GetDocument(puri);

                    extractionRequests.Add(t);
                    
                }
            }
            
            while(extractionRequests.Count > 0)
            {
                Task<Document> completedTask = await TaskEx.WhenAny(extractionRequests);
                extractionRequests.Remove(completedTask);

                Document parsedDoc = await completedTask;
                if (parsedDoc == null)
                    continue;

                Expander expander = new Expander { Header = parsedDoc.Title };
                TextBox metadataXML = new TextBox {TextWrapping = TextWrapping.Wrap, MinHeight = 100};

                var s = timeStamps[parsedDoc.Location.Value];
                Console.WriteLine(" ---------------------------------- Time to complete: " + DateTime.Now.Subtract(s).TotalMilliseconds);
                metadataXML.Text = await TaskEx.Run(() => SimplTypesScope.Serialize(parsedDoc, StringFormat.Xml));

                expander.Content = metadataXML;
                MetadataTitleXMLContainer.Children.Add(expander);
                
                
            }
        }
    }
}
