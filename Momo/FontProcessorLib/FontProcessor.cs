using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


namespace FontProcessorLib
{
    [ContentProcessor(DisplayName = "Font")]
    public class FontProcessor : ContentProcessor<FontContent, FontContent>
    {
        ContentProcessorContext context;


        public override FontContent Process(FontContent input, ContentProcessorContext context)
        {
            this.context = context;

            //for (int i = 0; i < input.m_typeface.m_pages.Count; ++i)
            //{
            //    FontContent.Page page = input.m_typeface.m_pages[i];
            //    ExternalReference<TextureContent> er = new ExternalReference<TextureContent>(page.m_textureFileName, input.Identity);
            //    context.BuildAsset<TextureContent, TextureContent>(er, typeof(TextureProcessor).Name, input.OpaqueData, null, page.m_textureAssetName);
            //    page.m_texture = er;
            //}

            return input;
        }
    }
}
