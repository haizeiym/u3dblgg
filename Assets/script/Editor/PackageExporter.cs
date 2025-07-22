using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Unity包导出工具
/// 将关卡编辑器相关文件打包成.unitypackage文件
/// </summary>
public class PackageExporter : EditorWindow
{
    private string packageName = "LevelEditor";
    private string packageVersion = "1.0.0";
    private string exportPath = "";
    private bool includeScenes = true;
    private bool includePrefabs = true;
    private bool includeConfig = true;
    private bool includeTextures = true; // 包含纹理资源
    private bool includeSavedLevels = false; // 默认不包含用户数据

    [MenuItem("Tools/Level Editor/导出Unity包")]
    public static void ShowWindow()
    {
        GetWindow<PackageExporter>("Unity包导出");
    }

    void OnEnable()
    {
        // 设置默认导出路径
        exportPath = Path.Combine(Application.dataPath, "../Exports");
        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }
    }

    void OnGUI()
    {
        try
        {
            EditorGUILayout.LabelField("关卡编辑器 Unity包导出", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 包信息
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("包信息", EditorStyles.boldLabel);
            packageName = EditorGUILayout.TextField("包名称:", packageName);
            packageVersion = EditorGUILayout.TextField("版本号:", packageVersion);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 导出路径
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("导出设置", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            exportPath = EditorGUILayout.TextField("导出路径:", exportPath);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.SaveFolderPanel("选择导出路径", exportPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    exportPath = selectedPath;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 包含选项
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("包含内容", EditorStyles.boldLabel);
            includeScenes = EditorGUILayout.Toggle("包含场景文件", includeScenes);
            includePrefabs = EditorGUILayout.Toggle("包含预制体", includePrefabs);
            includeConfig = EditorGUILayout.Toggle("包含配置文件", includeConfig);
            includeTextures = EditorGUILayout.Toggle("包含纹理资源", includeTextures);
            includeSavedLevels = EditorGUILayout.Toggle("包含已保存关卡", includeSavedLevels);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 导出按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出Unity包", GUILayout.Height(30)))
            {
                ExportPackage();
            }
            if (GUILayout.Button("打开导出目录", GUILayout.Width(100), GUILayout.Height(30)))
            {
                OpenExportDirectory();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // 说明
            EditorGUILayout.HelpBox(
                "导出说明:\n" +
                "• 脚本文件: 自动包含所有关卡编辑器脚本\n" +
                "• 场景文件: 包含示例场景（可选）\n" +
                "• 预制体: 包含UI预制体（可选）\n" +
                "• 配置文件: 包含默认配置（可选）\n" +
                "• 纹理资源: 包含纹理图片资源（可选）\n" +
                "• 已保存关卡: 包含用户创建的关卡（可选）",
                MessageType.Info
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PackageExporter OnGUI Error: {e.Message}");
            EditorGUILayout.HelpBox($"界面渲染错误: {e.Message}", MessageType.Error);
        }
    }

    void ExportPackage()
    {
        try
        {
            // 构建包含的文件列表
            string[] assets = BuildAssetList();

            if (assets.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "没有找到要导出的文件", "确定");
                return;
            }

            // 生成文件名
            string fileName = $"{packageName}_v{packageVersion}.unitypackage";
            string fullPath = Path.Combine(exportPath, fileName);

            // 导出包
            AssetDatabase.ExportPackage(assets, fullPath, ExportPackageOptions.Recurse);

            // 显示结果
            EditorUtility.DisplayDialog("导出成功", 
                $"Unity包已导出到:\n{fullPath}\n\n包含 {assets.Length} 个文件", "确定");

            Debug.Log($"Unity包导出成功: {fullPath}");
            Debug.Log($"包含文件数量: {assets.Length}");

            // 打开导出目录
            OpenExportDirectory();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"导出失败: {e.Message}");
            EditorUtility.DisplayDialog("导出失败", $"错误: {e.Message}", "确定");
        }
    }

    string[] BuildAssetList()
    {
        System.Collections.Generic.List<string> assets = new System.Collections.Generic.List<string>();

        // 始终包含脚本文件
        string scriptPath = "Assets/script";
        if (Directory.Exists(scriptPath))
        {
            assets.Add(scriptPath);
        }

        // 可选包含场景文件
        if (includeScenes)
        {
            string scenesPath = "Assets/Scenes";
            if (Directory.Exists(scenesPath))
            {
                assets.Add(scenesPath);
            }
        }

        // 可选包含预制体
        if (includePrefabs)
        {
            string prefabsPath = "Assets/Prefabs";
            if (Directory.Exists(prefabsPath))
            {
                assets.Add(prefabsPath);
            }
        }

        // 可选包含配置文件
        if (includeConfig)
        {
            string configPath = "Assets/config";
            if (Directory.Exists(configPath))
            {
                assets.Add(configPath);
            }
        }

        // 可选包含纹理资源
        if (includeTextures)
        {
            string texturesPath = "Assets/Textures";
            if (Directory.Exists(texturesPath))
            {
                assets.Add(texturesPath);
            }
        }

        // 可选包含已保存关卡
        if (includeSavedLevels)
        {
            string savedLevelsPath = "Assets/SavedLevels";
            if (Directory.Exists(savedLevelsPath))
            {
                assets.Add(savedLevelsPath);
            }
        }

        return assets.ToArray();
    }

    void OpenExportDirectory()
    {
        if (Directory.Exists(exportPath))
        {
            EditorUtility.RevealInFinder(exportPath);
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出目录不存在", "确定");
        }
    }
} 