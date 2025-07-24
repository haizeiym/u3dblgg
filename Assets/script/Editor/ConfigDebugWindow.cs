using UnityEngine;
using UnityEditor;

/// <summary>
/// 配置调试窗口
/// 用于检查配置加载状态和调试问题
/// </summary>
public class ConfigDebugWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/配置调试")]
    public static void ShowWindow()
    {
        GetWindow<ConfigDebugWindow>("配置调试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("配置调试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("检查配置状态"))
        {
            CheckConfigStatus();
        }

        if (GUILayout.Button("强制重新加载配置"))
        {
            ForceReloadConfig();
        }

        if (GUILayout.Button("创建测试配置"))
        {
            CreateTestConfig();
        }

        if (GUILayout.Button("显示配置内容"))
        {
            ShowConfigContent();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("调试信息:", EditorStyles.boldLabel);
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            EditorGUILayout.LabelField($"配置实例: 已创建");
            EditorGUILayout.LabelField($"形状类型数量: {config.shapeTypes?.Count ?? 0}");
            EditorGUILayout.LabelField($"球类型数量: {config.ballTypes?.Count ?? 0}");
            EditorGUILayout.LabelField($"背景配置数量: {config.backgroundConfigs?.Count ?? 0}");
            EditorGUILayout.LabelField($"当前背景索引: {config.currentBackgroundIndex}");
        }
        else
        {
            EditorGUILayout.LabelField("配置实例: 未创建", EditorStyles.boldLabel);
        }
    }

    void CheckConfigStatus()
    {
        Debug.Log("=== 配置状态检查 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("LevelEditorConfig.Instance 为空！");
            return;
        }

        Debug.Log($"配置实例已创建: {config.name}");
        Debug.Log($"形状类型: {config.shapeTypes?.Count ?? 0} 个");
        Debug.Log($"球类型: {config.ballTypes?.Count ?? 0} 个");
        Debug.Log($"背景配置: {config.backgroundConfigs?.Count ?? 0} 个");

        // 检查配置文件是否存在
        string configPath = System.IO.Path.Combine(Application.dataPath, "config", "level_editor_config.json");
        bool fileExists = System.IO.File.Exists(configPath);
        Debug.Log($"配置文件存在: {fileExists} ({configPath})");

        if (fileExists)
        {
            string json = System.IO.File.ReadAllText(configPath);
            Debug.Log($"配置文件内容长度: {json.Length} 字符");
            Debug.Log($"配置文件内容预览: {json.Substring(0, Mathf.Min(200, json.Length))}...");
        }
    }

    void ForceReloadConfig()
    {
        Debug.Log("=== 强制重新加载配置 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空，无法重新加载");
            return;
        }

        config.LoadConfigFromFile();
        Debug.Log("配置重新加载完成");
        
        // 刷新窗口
        Repaint();
    }

    void CreateTestConfig()
    {
        Debug.Log("=== 创建测试配置 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空，无法创建测试配置");
            return;
        }

        // 清空现有配置
        config.shapeTypes.Clear();
        config.ballTypes.Clear();
        config.backgroundConfigs.Clear();

        // 添加测试形状
        config.AddShapeType("测试圆形");
        config.AddShapeType("测试矩形");
        config.AddShapeType("测试三角形");

        // 添加测试球
        config.AddBallType("测试红球", Color.red);
        config.AddBallType("测试蓝球", Color.blue);
        config.AddBallType("测试绿球", Color.green);

        // 添加测试背景
        config.AddBackgroundConfig("测试白色背景", null, Color.white);
        config.AddBackgroundConfig("测试灰色背景", null, Color.gray);
        config.AddBackgroundConfig("测试黑色背景", null, Color.black);

        // 保存配置
        config.SaveConfigToFile();
        Debug.Log("测试配置创建完成并保存");
        
        // 刷新窗口
        Repaint();
    }

    void ShowConfigContent()
    {
        Debug.Log("=== 显示配置内容 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空");
            return;
        }

        Debug.Log("形状类型:");
        if (config.shapeTypes != null)
        {
            for (int i = 0; i < config.shapeTypes.Count; i++)
            {
                var shape = config.shapeTypes[i];
                Debug.Log($"  [{i}] {shape?.name ?? "null"} (Sprite: {shape?.sprite?.name ?? "null"})");
            }
        }

        Debug.Log("球类型:");
        if (config.ballTypes != null)
        {
            for (int i = 0; i < config.ballTypes.Count; i++)
            {
                var ball = config.ballTypes[i];
                Debug.Log($"  [{i}] {ball?.name ?? "null"} (Color: {ball?.color}, Sprite: {ball?.sprite?.name ?? "null"})");
            }
        }

        Debug.Log("背景配置:");
        if (config.backgroundConfigs != null)
        {
            for (int i = 0; i < config.backgroundConfigs.Count; i++)
            {
                var bg = config.backgroundConfigs[i];
                Debug.Log($"  [{i}] {bg?.name ?? "null"} (UseSprite: {bg?.useSprite}, Color: {bg?.backgroundColor}, Sprite: {bg?.backgroundSprite?.name ?? "null"})");
            }
        }
    }
} 