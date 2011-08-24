using System;
using System.Diagnostics;



namespace Momo.Core.Shaders
{
    public static class ParameterSemantic
    {
        // --------------------------------------------------------------------
        // -- Public Enums
        // --------------------------------------------------------------------
        public enum Type
        {
            kUndefined = -1,

            // Texture paramaters.
            kTexture1 = 0,
            kTexture2,
            kTexture3,

            // Object parameters.
            kObjectWorldMat,
            kObjectWorldInverseMat,
            kObjectWorldInverseTransposeMat,

            // Camera parameters.
            kCameraWorldPosition,
            kCameraWorldDirection,
            kCameraViewProjMat,
            kCameraViewProjInverseMat,

            // The number of parameters in this enum.
            kCount
        }


        // --------------------------------------------------------------------
        // -- Static Private Members
        // --------------------------------------------------------------------
        private static string[] ms_names = new string[(int)Type.kCount] {
            "TEXTURE1",
            "TEXTURE2",
            "TEXTURE3",

            "OBJECT_WORLD_MATRIX",
            "OBJECT_WORLD_INVERSE_MATRIX",
            "OBJECT_WORLD_INVERSE_TRANSPOSE_MATRIX",

            "CAMERA_WORLD_POSITION",
            "CAMERA_WORLD_DIRECTION",
            "CAMERA_VIEW_PROJECTION_MATRIX",
            "CAMERA_VIEW_PROJECTION_INVERSE_MATRIX",
        };


        // --------------------------------------------------------------------
        // -- public static Methods
        // --------------------------------------------------------------------
        public static string GetSemanticName(Type type)
        {
            System.Diagnostics.Debug.Assert((int)type < ms_names.Length, "ShaderSemantics: Semantic name not set for type specified");
            return ms_names[(int)type];
        }
    }
}
