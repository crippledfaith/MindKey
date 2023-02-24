using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    internal class GridPlacementWordCloudGenerator : AWordCloudGenerator
    {
        public GridPlacementWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        public override event EventHandler<WorkCloudResult>? OnProgress;

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }

        protected override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            throw new NotImplementedException();
        }
    }
}
