using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator;
using MindKey.WordCloudGenerator.Base;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using SkiaSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

namespace MindKey.WordCloudGeneratorTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> words = new Dictionary<string, int>();
        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsCountry());
            var randomizerint = RandomizerFactory.GetRandomizer(new FieldOptionsInteger() { Min = 1, Max = 100 });
            for (int i = 0; i < 100; i++)
            {
                string? key = randomizer.Generate();
                if (key != null && !words.ContainsKey(key))
                    words.Add(key, randomizerint.Generate().Value);
            }
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            IWordCloudGenerator wordCloudGenerator = new GeneticWordCloudGenerator(Configuration);
            WorkCloudParameter workCloudParameter = new WorkCloudParameter();
            workCloudParameter.Width = 500;
            workCloudParameter.Height = 500;
            workCloudParameter.WordLimit = 50;

            var wordCloudResult = await wordCloudGenerator.Generate(words, workCloudParameter);
            if (wordCloudResult != null)
            {
                imgPreview.Source = null;
                byte[] byteBuffer = Convert.FromBase64String(wordCloudResult.Image);
                File.WriteAllBytes("ff.jpg", byteBuffer.ToArray());
                var bitmap = new BitmapImage();
                //bitmap.UriSource = new Uri("ms-appdata:///local/image.jpg");
                //imgshow.Source = bitmap;
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "ff.jpg", UriKind.Absolute);
                bitmap.EndInit();
                imgPreview.Source = bitmap;

            }
        }
   
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

        }
    }
}