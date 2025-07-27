using UnityEngine;
using UnityEditor;

public class ComprehensivePauseTestWindow : EditorWindow
{
    // [MenuItem("Tools/Level Editor/Comprehensive Pause Test")]
    public static void ShowWindow()
    {
        GetWindow<ComprehensivePauseTestWindow>("全面暂停测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("全面暂停游戏测试", EditorStyles.boldLabel);
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            EditorGUILayout.HelpBox("无法获取配置实例", MessageType.Error);
            return;
        }
        
        EditorGUILayout.Space();
        
        // 显示当前状态
        int currentIndex = config.GetLevelIndex();
        EditorGUILayout.LabelField("当前关卡索引:", currentIndex.ToString());
        EditorGUILayout.LabelField("形状类型数量:", config.shapeTypes.Count.ToString());
        EditorGUILayout.LabelField("球类型数量:", config.ballTypes.Count.ToString());
        
        EditorGUILayout.Space();
        
        // 测试场景
        if (GUILayout.Button("测试场景1: 直接LoadConfigFromFile"))
        {
            TestScenario1();
        }
        
        if (GUILayout.Button("测试场景2: 模拟组件Start方法"))
        {
            TestScenario2();
        }
        
        if (GUILayout.Button("测试场景3: 模拟单例重新创建"))
        {
            TestScenario3();
        }
        
        if (GUILayout.Button("测试场景4: 模拟配置为空的情况"))
        {
            TestScenario4();
        }
        
        if (GUILayout.Button("测试场景5: 模拟异常情况"))
        {
            TestScenario5();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("增加关卡索引"))
        {
            int newIndex = config.IncrementLevelIndex();
            Debug.Log($"关卡索引已增加到: {newIndex}");
            Repaint();
        }
        
        if (GUILayout.Button("保存配置"))
        {
            config.SaveConfigToFile();
            Debug.Log("配置已保存");
        }
        
        EditorGUILayout.Space();
        
        // 显示配置文件内容
        EditorGUILayout.LabelField("配置文件内容:", EditorStyles.boldLabel);
        string configPath = System.IO.Path.Combine(Application.dataPath, "config/level_editor_config.json");
        if (System.IO.File.Exists(configPath))
        {
            string configContent = System.IO.File.ReadAllText(configPath);
            EditorGUILayout.TextArea(configContent, GUILayout.Height(100));
        }
        else
        {
            EditorGUILayout.HelpBox("配置文件不存在", MessageType.Warning);
        }
    }
    
    void TestScenario1()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"场景1测试前关卡索引: {beforeIndex}");
        
        // 直接调用LoadConfigFromFile
        config.LoadConfigFromFile();
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"场景1测试后关卡索引: {afterIndex}");
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 场景1: 直接LoadConfigFromFile保持关卡索引成功！");
        }
        else
        {
            Debug.LogError($"❌ 场景1: 直接LoadConfigFromFile重置了关卡索引: {beforeIndex} -> {afterIndex}");
        }
    }
    
    void TestScenario2()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"场景2测试前关卡索引: {beforeIndex}");
        
        // 模拟BackgroundManager和ConfigPreviewUI的Start方法逻辑
        if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
        {
            Debug.Log("场景2: 配置为空，重新加载");
            config.LoadConfigFromFile();
        }
        else
        {
            Debug.Log("场景2: 配置已存在，跳过重新加载");
        }
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"场景2测试后关卡索引: {afterIndex}");
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 场景2: 模拟组件Start方法保持关卡索引成功！");
        }
        else
        {
            Debug.LogError($"❌ 场景2: 模拟组件Start方法重置了关卡索引: {beforeIndex} -> {afterIndex}");
        }
    }
    
    void TestScenario3()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"场景3测试前关卡索引: {beforeIndex}");
        
        // 模拟单例重新创建
        var field = typeof(LevelEditorConfig).GetField("_instance", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (field != null)
        {
            field.SetValue(null, null);
            Debug.Log("场景3: 已清空单例实例");
            
            // 重新获取实例
            var newConfig = LevelEditorConfig.Instance;
            int afterIndex = newConfig.GetLevelIndex();
            Debug.Log($"场景3测试后关卡索引: {afterIndex}");
            
            if (beforeIndex == afterIndex)
            {
                Debug.Log("✅ 场景3: 单例重新创建保持关卡索引成功！");
            }
            else
            {
                Debug.LogError($"❌ 场景3: 单例重新创建重置了关卡索引: {beforeIndex} -> {afterIndex}");
            }
        }
    }
    
    void TestScenario4()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"场景4测试前关卡索引: {beforeIndex}");
        
        // 模拟配置为空的情况（通过临时清空配置）
        var originalShapes = new System.Collections.Generic.List<ShapeType>(config.shapeTypes);
        var originalBalls = new System.Collections.Generic.List<BallType>(config.ballTypes);
        
        config.shapeTypes.Clear();
        config.ballTypes.Clear();
        
        Debug.Log("场景4: 临时清空配置");
        
        // 现在调用LoadConfiguration（模拟LevelEditorUI的逻辑）
        if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
        {
            Debug.Log("场景4: 配置为空，重新加载");
            config.LoadConfigFromFile();
            
            if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
            {
                Debug.Log("场景4: 配置仍然为空，初始化默认配置");
                int savedLevelIndex = config.GetLevelIndex();
                config.InitializeDefaultConfig();
                config.SetLevelIndex(savedLevelIndex);
            }
        }
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"场景4测试后关卡索引: {afterIndex}");
        
        // 恢复原始配置
        config.shapeTypes.Clear();
        config.ballTypes.Clear();
        config.shapeTypes.AddRange(originalShapes);
        config.ballTypes.AddRange(originalBalls);
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 场景4: 配置为空的情况保持关卡索引成功！");
        }
        else
        {
            Debug.LogError($"❌ 场景4: 配置为空的情况重置了关卡索引: {beforeIndex} -> {afterIndex}");
        }
    }
    
    void TestScenario5()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"场景5测试前关卡索引: {beforeIndex}");
        
        // 模拟异常情况（通过临时删除配置文件）
        string configPath = System.IO.Path.Combine(Application.dataPath, "config/level_editor_config.json");
        string backupPath = configPath + ".backup";
        
        if (System.IO.File.Exists(configPath))
        {
            System.IO.File.Move(configPath, backupPath);
            Debug.Log("场景5: 临时删除配置文件");
        }
        
        // 尝试加载配置（会触发异常处理）
        try
        {
            config.LoadConfigFromFile();
        }
        catch (System.Exception e)
        {
            Debug.Log($"场景5: 捕获到预期的异常: {e.Message}");
        }
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"场景5测试后关卡索引: {afterIndex}");
        
        // 恢复配置文件
        if (System.IO.File.Exists(backupPath))
        {
            System.IO.File.Move(backupPath, configPath);
            Debug.Log("场景5: 已恢复配置文件");
        }
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 场景5: 异常情况保持关卡索引成功！");
        }
        else
        {
            Debug.LogError($"❌ 场景5: 异常情况重置了关卡索引: {beforeIndex} -> {afterIndex}");
        }
    }
} 