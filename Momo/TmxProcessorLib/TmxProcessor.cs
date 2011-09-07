using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


using TInput = TmxProcessorLib.Content.TmxData;
using TOutput = TmxProcessorLib.Content.TmxData;

namespace TmxProcessorLib
{

    /// <summary>
    /// Processer parses the TmxXml and creates a new MapData object
    /// </summary>
    [ContentProcessor(DisplayName = "TmxProcessorLib.TmxProcessor")]
    public class TmxProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            input.Process(context);
            return input;
        }
    }
}