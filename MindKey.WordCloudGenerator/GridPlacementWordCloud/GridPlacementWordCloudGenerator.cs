using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using SkiaSharp;
namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    public class GridPlacementWordCloudGenerator : AWordCloudGenerator
    {
        //Determine the size of the grid: The first step is to determine the size of the grid based on the available space and the desired number of cells.For example, if the available space is a 500 x 500 pixel image and we want to use a 10 x 10 grid, each cell will be 50 x 50 pixels.
        //Calculate the position of each word: Once the grid size is determined, the algorithm places each word in a cell of the grid.The position of the word is calculated by centering it horizontally and vertically in the cell. For example, if the word is "hello" and the cell is 50 x 50 pixels, the position of the word will be (25, 25) within the cell.
        //Adjust the font size: After positioning the words in the cells, the font size is adjusted to fill the available space in the cell.The font size is increased or decreased until the word fits within the cell without overlapping with neighboring words.
        //Repeat for all words: The algorithm repeats steps 2 and 3 for all the words in the input text.
        //Output the word cloud: Once all the words are positioned and the font size is adjusted, the algorithm outputs the final word cloud.

        public GridPlacementWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }

        protected override async Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            //Determine the size of the grid: The first step is to determine the size of the grid based on the available space and the desired number of cells.For example, if the available space is a 500 x 500 pixel image and we want to use a 10 x 10 grid, each cell will be 50 x 50 pixels.
            using SKBitmap bitmap = SKBitmap.Decode(MaskFilePath);
            using SKCanvas canvas = new SKCanvas(bitmap);

            var canvasHeight = parameter.Height;
            var canvasWidth = parameter.Width;
            var service = new GridPlacementService(wordCount, canvasHeight, canvasWidth, bitmap.Copy());
            service.OnProgress += ServiceOnProgress;

            var wordCloudCloud = await service.Start();
            ChangeColor(bitmap);
            foreach (var word in wordCloudCloud.WordCloudWords.Where(q => q.IsFit.HasValue && q.IsFit.Value))
            {
                canvas.DrawText(word.Text, word.DrawX, word.DrawY, word.Font, word.Paint);
            }
            using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
            var base64String = Convert.ToBase64String(data.ToArray());
            var isFitCount = wordCloudCloud.WordCloudWords.Count(q => q.IsFit.HasValue && q.IsFit.Value);
            var totalCount = wordCloudCloud.WordCount.Count;
            WordCloudResult.Status = $"Done";
            WordCloudResult.Image = base64String;
            Directory.Delete(OutputPath, true);
            return WordCloudResult;
        }

    }
}
