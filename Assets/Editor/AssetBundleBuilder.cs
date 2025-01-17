using UnityEditor;
using System.IO;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
        // �����, ���� ����� ����������� Asset Bundles
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // ���������� ���� Asset Bundles
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
