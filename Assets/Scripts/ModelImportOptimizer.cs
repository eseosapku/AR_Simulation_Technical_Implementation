#if UNITY_EDITOR
using UnityEditor;

namespace FirstResponse
{
    public class ModelImportOptimizer : AssetPostprocessor
    {
        public const string ModelsFolder = "Assets/Models/";
        public const int MaxTextureSize = 1024;

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ModelsFolder))
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.maxTextureSize = MaxTextureSize;
            importer.mipmapEnabled = true;
            importer.isReadable = false;
            importer.textureCompression = TextureImporterCompression.Compressed;

            TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = MaxTextureSize;
            android.format = TextureImporterFormat.ASTC_6x6;
            android.compressionQuality = 50;
            importer.SetPlatformTextureSettings(android);
        }

        private void OnPreprocessModel()
        {
            if (!assetPath.StartsWith(ModelsFolder) || !(assetImporter is ModelImporter importer))
            {
                return;
            }

            importer.meshCompression = ModelImporterMeshCompression.High;
            importer.isReadable = false;
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;
            importer.importCameras = false;
            importer.importLights = false;
        }
    }
}
#endif
