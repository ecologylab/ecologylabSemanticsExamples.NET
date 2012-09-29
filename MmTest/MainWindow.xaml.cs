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

using Simpl.Fundamental.Net;
using Simpl.Serialization;
using Ecologylab.Semantics.Collecting;
using Ecologylab.Semantics.Generated.Library;
using Ecologylab.Semantics.MetadataNS.Builtins;
using Ecologylab.Semantics.MetaMetadataNS;
using Ecologylab.Semantics.Services;

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

            //var a = new MetadataServicesClient();
            //Console.Write("a" + a);
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _semanticsSessionScope = await SemanticsSessionScope.InitAsync(
                                            RepositoryMetadataTranslationScope.Get(),
                                            MetaMetadataRepositoryInit.DefaultRepositoryLocation);
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeception: " + ex.StackTrace);
                
            }
            
            BtnGetMetadata.IsEnabled = true;
        }


        private async void BtnGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            //            string serializedMetadata = UrlBox.Text;
            //            Object o = RepositoryMetadataTranslationScope.Get().Deserialize(serializedMetadata, StringFormat.Xml);
            //            StringBuilder sb = new StringBuilder();
            //            SimplTypesScope.Serialize(o, sb, StringFormat.Xml);
            //            String reserializedMetadatata = sb.ToString();
            //            Console.WriteLine(reserializedMetadatata);
            //Expander expander = new Expander();
            //TextBox metadataXML = new TextBox { TextWrapping = TextWrapping.Wrap, MinHeight = 100 };
            //metadataXML.Text = reserializedMetadatata;

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

            while (extractionRequests.Count > 0)
            {
                Task<Document> completedTask = await Task.WhenAny(extractionRequests);
                extractionRequests.Remove(completedTask);

                Document parsedDoc = await completedTask;
                if (parsedDoc == null)
                    continue;

                Expander expander = new Expander {Header = parsedDoc.Title};
                TextBox metadataXML = new TextBox {TextWrapping = TextWrapping.Wrap, MinHeight = 100};

                var s = timeStamps[parsedDoc.Location.Value];
                Console.WriteLine(" ---------------------------------- Time to complete: " +
                                  DateTime.Now.Subtract(s).TotalMilliseconds);
                metadataXML.Text = await Task.Run(() => SimplTypesScope.Serialize(parsedDoc, StringFormat.Xml));

                expander.Content = metadataXML;
                MetadataTitleXMLContainer.Children.Add(expander);
            }

        }
    }
}
