using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using TImport = TmxProcessorLib.Content.TmxData;

namespace TmxProcessorLib
{
    /// <summary>
    /// Importer simply loads the xml from a tmx file into a TmxXml class
    /// </summary>
    [ContentImporter(".tmx", DisplayName = "Tiled Map File", DefaultProcessor = "TmxProcessor")]
    public class TmxImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            context.Logger.PushFile(filename);

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(filename);

            TImport data = new TImport(filename);
            data.ImportXmlDoc(xmlDoc, context);

            return data;
        }
    }
}
