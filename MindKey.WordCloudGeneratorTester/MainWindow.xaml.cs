using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator;
using MindKey.WordCloudGenerator.Base;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MindKey.WordCloudGeneratorTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> words = new Dictionary<string, int>();
        List<Type> WordCloudGenerators = new List<Type>();
        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            var v = new GeneticWordCloudGenerator(configuration);
            this.SizeChanged += MainWindow_SizeChanged;

            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            WordCloudGenerators = types
            .Where(x => typeof(IWordCloudGenerator).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => x).ToList();
            foreach (var item in WordCloudGenerators)
            {
                cmbGenerator.Items.Add(item.Name);
            }
            cmbGenerator.SelectedIndex = 0;
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsCountry());
            var randomizerint = RandomizerFactory.GetRandomizer(new FieldOptionsInteger() { Min = 1, Max = 20 });
            for (int i = 0; i < 100; i++)
            {
                string? key = randomizer.Generate();
                if (key != null && !words.ContainsKey(key))
                    words.Add(key, randomizerint.Generate().Value);
            }
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public int CanvasHeight { get; set; }
        public int CanvasWidth { get; set; }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            IWordCloudGenerator wordCloudGenerator = (IWordCloudGenerator)Activator.CreateInstance(WordCloudGenerators[cmbGenerator.SelectedIndex], Configuration);
            wordCloudGenerator.OnProgress += WordCloudGeneratorOnProgress;
            WorkCloudParameter workCloudParameter = new WorkCloudParameter();
            workCloudParameter.Width = CanvasWidth;
            workCloudParameter.Height = CanvasHeight;
            workCloudParameter.WordLimit = 50;
            var wordCloudResult = await wordCloudGenerator.Generate(words, workCloudParameter);
            if (wordCloudResult != null)
            {
                imgPreview.Source = null;
                byte[] byteBuffer = Convert.FromBase64String(wordCloudResult.Image);
                File.WriteAllBytes("ff.jpg", byteBuffer.ToArray());
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "ff.jpg", UriKind.Absolute);
                bitmap.EndInit();
                imgPreview.Source = bitmap;
                lblStatus.Content = wordCloudResult.Status;
            }


        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasWidth = (int)gridImage.ActualWidth;
            CanvasHeight = (int)gridImage.ActualHeight;
        }

        private void WordCloudGeneratorOnProgress(object? sender, WorkCloudResult e)
        {
            imgPreview.Source = null;
            byte[] byteBuffer = Convert.FromBase64String(e.Image);
            File.WriteAllBytes("ff.jpg", byteBuffer.ToArray());
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "ff.jpg", UriKind.Absolute);
            bitmap.EndInit();
            imgPreview.Source = bitmap;
            lblStatus.Content = e.Status;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

        }
    }
}