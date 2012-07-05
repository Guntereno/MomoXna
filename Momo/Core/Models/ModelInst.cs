using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Momo.Core.Models
{
    public class ModelInst
    {
        public Model Model { get; set; }
        public Matrix WorldMatrix { get; set; }

        public ModelInst(Model model, Matrix worldMatrix)
        {
            Model = model;
            WorldMatrix = worldMatrix;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * WorldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.TextureEnabled = true;
                    effect.PreferPerPixelLighting = true;
                }

                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
