using UnityEngine;using UnityEditor;using HMLFramwork.Helpers;using System.IO;

public class AssetBundlesEditor : EditorWindow{    static BuildTarget buildTarget = BuildTarget.StandaloneWindows64;    static string _ABStoragePath ;    static string _ABPackageName = "";
    #region 创建AB包    [MenuItem("AB包工具/打AB包/Build iPhone AB包", false, 3)]
    public static void BuildiPhoneResource()
    {
        BuildTarget target;
        target = BuildTarget.iOS;
        BuildAssetResource(target);
    }
    [MenuItem("AB包工具/打AB包/Build Android AB包", false, 2)]
    public static void BuildAndroidResource()
    {
        BuildAssetResource(BuildTarget.Android);
    }
    [MenuItem("AB包工具/打AB包/Build WebGL AB包", false, 4)]
    public static void BuildWebGLResource()
    {
        BuildAssetResource(BuildTarget.WebGL);
    }
    [MenuItem("AB包工具/打AB包/Build Wins AB包/将一个或者多个文件打成一个包", false, 1)]
    public static void BuildWindowsResource()
    {
        _ABStoragePath = HMLFramwork.Helpers.PathHelper.getAssetsBundlePath;
        buildTarget = BuildTarget.StandaloneWindows64;
        EditorWindow.GetWindow(typeof(AssetBundlesEditor), true, "AB包打包工具");
    }
    [MenuItem("AB包工具/清除所有设置过的AssetBundleName")]
    static void ClearAllAssetBundlesName()    {        int length = AssetDatabase.GetAllAssetBundleNames().Length;        if (length < 1) { Debug.Log("无需清理！"); return; }        Debug.Log(string.Format("提醒：共有{0}个需要清理...", length));        string oldAssetBundleNames = "";        for (int i = 0; i < length; i++)        {            oldAssetBundleNames = AssetDatabase.GetAllAssetBundleNames()[i];            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames, true);            Debug.Log("清理" + oldAssetBundleNames[i] + "完成！");        }
    }
    private static void BuildAssetResource(BuildTarget _target)
    {
        if (_ABStoragePath.Length <= 0 || !Directory.Exists(_ABStoragePath)) return;        //1. 移除没有用的assetbundlename        AssetDatabase.RemoveUnusedAssetBundleNames();        //2. 获取当前选中的所有对象        Object[] objs = Selection.objects;         //3. 获取选中对象的资源路径        string[] itemAssets = new string[objs.Length];        for (int i = 0; i < objs.Length; i++)        {            itemAssets[i] = AssetDatabase.GetAssetPath(objs[i]); //获取对象在工程目录下的相对路径        }        //当没有给AB包名称赋值时，默认使用选中的第一个对象的名字        if (_ABPackageName.Length<1)        {            _ABPackageName = Path.GetFileName(itemAssets[0]);        }        //将选中对象一起打包        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];        buildMap[0].assetBundleName = _ABPackageName;        buildMap[0].assetNames = itemAssets;        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(_ABStoragePath, buildMap, BuildAssetBundleOptions.None, _target);        AssetDatabase.Refresh(); //刷新        if (manifest == null) Debug.LogError("打包失败：没有选择需要打包的对象...");        else Debug.Log("提醒：打包成功...");
    }
    #endregion

    private void OnGUI()
    {
        GUILayout.Label("AB包的名字（当没有赋值时，默认使用选中的第一个对象的名字）：");        _ABPackageName = EditorGUILayout.TextField(_ABPackageName);        bool isChooseStoragePath = GUILayout.Button("选择AB包存储路径（若不选择，则默认为存储在流资源文件内的AB包路径下）");        if (isChooseStoragePath)        {            _ABStoragePath = UnityEditor.EditorUtility.OpenFolderPanel("选择AB包存储路径", _ABStoragePath, "");        }        EditorGUILayout.LabelField(_ABStoragePath);        if (GUILayout.Button("确定"))        {            Debug.Log("开始查找...");            BuildAssetResource(buildTarget);        }    }
    void OnInspectorUpdate()    {        Repaint();    }
}
