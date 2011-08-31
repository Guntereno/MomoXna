using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Fonts
{
    //[StructLayout(LayoutKind.Sequential)]
    public struct TextVertexDeclaration : IVertexType
    {
        Vector2 vertexPosition;
        Vector2 vertexTextureCoordinate;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        //The constructor for the custom vertex. This allows similar 
        //initialization of custom vertex arrays as compared to arrays of a 
        //standard vertex type, such as VertexPositionColor.
        public TextVertexDeclaration(Vector2 pos, Vector2 textureCoordinate)
        {
            vertexPosition = pos;
            vertexTextureCoordinate = textureCoordinate;
        }

        //Public methods for accessing the components of the custom vertex.
        public Vector2 Position
        {
            get { return vertexPosition; }
            set { vertexPosition = value; }
        }

        public Vector2 TextureCoordinate
        {
            get { return vertexTextureCoordinate; }
            set { vertexTextureCoordinate = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
