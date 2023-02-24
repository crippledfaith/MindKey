using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    internal class GridPlacementWordCloudGenerator : AWordCloudGenerator
    {

        //Determine the size of the grid: The first step is to determine the size of the grid based on the available space and the desired number of cells.For example, if the available space is a 500 x 500 pixel image and we want to use a 10 x 10 grid, each cell will be 50 x 50 pixels.
        //Calculate the position of each word: Once the grid size is determined, the algorithm places each word in a cell of the grid.The position of the word is calculated by centering it horizontally and vertically in the cell. For example, if the word is "hello" and the cell is 50 x 50 pixels, the position of the word will be (25, 25) within the cell.
        //Adjust the font size: After positioning the words in the cells, the font size is adjusted to fill the available space in the cell.The font size is increased or decreased until the word fits within the cell without overlapping with neighboring words.
        //Repeat for all words: The algorithm repeats steps 2 and 3 for all the words in the input text.
        //Output the word cloud: Once all the words are positioned and the font size is adjusted, the algorithm outputs the final word cloud.


        public GridPlacementWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
            WordCloudResult= new WorkCloudResult();
        }

        public override event EventHandler<WorkCloudResult>? OnProgress;

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }

        protected override async Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {

            //Determine the size of the grid: The first step is to determine the size of the grid based on the available space and the desired number of cells.For example, if the available space is a 500 x 500 pixel image and we want to use a 10 x 10 grid, each cell will be 50 x 50 pixels.
            int canvasHeight = parameter.Height;
            int canvasWidth = parameter.Width;

            return WordCloudResult;
        }
    }
}
