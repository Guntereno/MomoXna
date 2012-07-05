using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MomoMap.Content
{
    internal class ModelInst
    {
        private string m_modelName;
        private Matrix m_worldMatrix;

        internal ModelInst(string modelName, Matrix worldMatrix)
        {
            m_modelName = modelName;
            m_worldMatrix = worldMatrix;
        }

        internal void Write(ContentWriter output)
        {
            output.Write(m_modelName);
            output.WriteObject<Matrix>(m_worldMatrix);
        }
    }
}
