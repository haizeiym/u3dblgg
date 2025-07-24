using UnityEngine;
using UnityEditor;

/// <summary>
/// 配置测试窗口
/// 用于测试配置中的图形和球是否正确使用
/// </summary>
public class ConfigTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/配置使用测试")]
    public static void ShowWindow()
    {
        GetWindow<ConfigTestWindow>("配置使用测试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("配置使用测试", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个窗口用于测试配置中的图形和球是否正确使用", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("测试形状配置使用"))
        {
            TestShapeConfigUsage();
        }

        if (GUILayout.Button("测试球配置使用"))
        {
            TestBallConfigUsage();
        }

        if (GUILayout.Button("测试配置验证"))
        {
            TestConfigValidation();
        }

        if (GUILayout.Button("显示当前配置状态"))
        {
            ShowCurrentConfigStatus();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("测试说明:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 点击测试按钮验证配置使用");
        EditorGUILayout.LabelField("2. 查看控制台输出确认配置正确应用");
        EditorGUILayout.LabelField("3. 在编辑器中添加形状和球验证效果");
    }

    void TestShapeConfigUsage()
    {
        Debug.Log("=== 测试形状配置使用 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空！");
            return;
        }

        string[] shapeTypes = config.GetShapeTypeNames();
        Debug.Log($"配置中的形状类型数量: {shapeTypes.Length}");

        for (int i = 0; i < shapeTypes.Length; i++)
        {
            string shapeType = shapeTypes[i];
            ShapeType shapeConfig = config.GetShapeConfig(shapeType);
            
            if (shapeConfig != null)
            {
                Debug.Log($"形状[{i}]: {shapeType}");
                Debug.Log($"  配置名称: {shapeConfig.name}");
                Debug.Log($"  精灵: {shapeConfig.sprite?.name ?? "null"}");
                
                // 测试创建ShapeData
                ShapeData testShape = new ShapeData(shapeType, Vector2.zero, 0f);
                Debug.Log($"  创建的ShapeData: {testShape.shapeType}");
            }
            else
            {
                Debug.LogError($"未找到形状配置: {shapeType}");
            }
        }
    }

    void TestBallConfigUsage()
    {
        Debug.Log("=== 测试球配置使用 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空！");
            return;
        }

        string[] ballTypes = config.GetBallTypeNames();
        Debug.Log($"配置中的球类型数量: {ballTypes.Length}");

        for (int i = 0; i < ballTypes.Length; i++)
        {
            string ballType = ballTypes[i];
            BallType ballConfig = config.GetBallConfig(ballType);
            
            if (ballConfig != null)
            {
                Debug.Log($"球[{i}]: {ballType}");
                Debug.Log($"  配置名称: {ballConfig.name}");
                Debug.Log($"  颜色: {ballConfig.color}");
                Debug.Log($"  精灵: {ballConfig.sprite?.name ?? "null"}");
                
                // 测试创建BallData
                BallData testBall = new BallData(ballType, Vector2.zero);
                Debug.Log($"  创建的BallData: {testBall.ballType}");
            }
            else
            {
                Debug.LogError($"未找到球配置: {ballType}");
            }
        }
    }

    void TestConfigValidation()
    {
        Debug.Log("=== 测试配置验证 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空！");
            return;
        }

        // 测试形状配置验证
        string[] shapeTypes = config.GetShapeTypeNames();
        for (int i = 0; i < shapeTypes.Length; i++)
        {
            string shapeType = shapeTypes[i];
            ShapeType shapeConfig = config.GetShapeConfig(shapeType);
            
            if (shapeConfig != null)
            {
                Debug.Log($"✓ 形状配置验证通过: {shapeType}");
            }
            else
            {
                Debug.LogError($"✗ 形状配置验证失败: {shapeType}");
            }
        }

        // 测试球配置验证
        string[] ballTypes = config.GetBallTypeNames();
        for (int i = 0; i < ballTypes.Length; i++)
        {
            string ballType = ballTypes[i];
            BallType ballConfig = config.GetBallConfig(ballType);
            
            if (ballConfig != null)
            {
                Debug.Log($"✓ 球配置验证通过: {ballType}");
            }
            else
            {
                Debug.LogError($"✗ 球配置验证失败: {ballType}");
            }
        }
    }

    void ShowCurrentConfigStatus()
    {
        Debug.Log("=== 当前配置状态 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空！");
            return;
        }

        Debug.Log($"配置实例: {config.name}");
        Debug.Log($"形状类型数量: {config.shapeTypes?.Count ?? 0}");
        Debug.Log($"球类型数量: {config.ballTypes?.Count ?? 0}");
        Debug.Log($"背景配置数量: {config.backgroundConfigs?.Count ?? 0}");
        Debug.Log($"当前背景索引: {config.currentBackgroundIndex}");

        // 显示形状类型详情
        if (config.shapeTypes != null)
        {
            Debug.Log("形状类型详情:");
            for (int i = 0; i < config.shapeTypes.Count; i++)
            {
                var shape = config.shapeTypes[i];
                Debug.Log($"  [{i}] {shape?.name ?? "null"} (Sprite: {shape?.sprite?.name ?? "null"})");
            }
        }

        // 显示球类型详情
        if (config.ballTypes != null)
        {
            Debug.Log("球类型详情:");
            for (int i = 0; i < config.ballTypes.Count; i++)
            {
                var ball = config.ballTypes[i];
                Debug.Log($"  [{i}] {ball?.name ?? "null"} (Color: {ball?.color}, Sprite: {ball?.sprite?.name ?? "null"})");
            }
        }
    }
} 