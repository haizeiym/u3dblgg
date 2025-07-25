using UnityEngine;
using UnityEditor;

public class LevelIndexPersistenceTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/Test Level Index Persistence")]
    public static void ShowWindow()
    {
        GetWindow<LevelIndexPersistenceTestWindow>("关卡索引持久性测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("关卡索引持久性测试", EditorStyles.boldLabel);
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            EditorGUILayout.HelpBox("无法获取配置实例", MessageType.Error);
            return;
        }
        
        EditorGUILayout.Space();
        
        // 显示当前索引
        int currentIndex = config.GetLevelIndex();
        EditorGUILayout.LabelField("当前关卡索引:", currentIndex.ToString());
        
        EditorGUILayout.Space();
        
        // 测试按钮
        if (GUILayout.Button("测试配置加载（模拟重启）"))
        {
            TestConfigLoading();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("测试默认配置初始化"))
        {
            TestDefaultConfigInitialization();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("增加关卡索引"))
        {
            int newIndex = config.IncrementLevelIndex();
            Debug.Log($"关卡索引已增加到: {newIndex}");
            Repaint();
        }
        
        EditorGUILayout.Space();
        
        // 手动设置索引
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("设置索引:");
        int setIndex = EditorGUILayout.IntField(currentIndex);
        if (setIndex != currentIndex && GUILayout.Button("设置"))
        {
            config.SetLevelIndex(setIndex);
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 保存和重新加载按钮
        if (GUILayout.Button("保存配置"))
        {
            config.SaveConfigToFile();
            Debug.Log("配置已保存");
        }
        
        if (GUILayout.Button("重新加载配置"))
        {
            config.LoadConfigFromFile();
            Debug.Log("配置已重新加载");
            Repaint();
        }
        
        EditorGUILayout.Space();
        
        // 显示配置文件路径
        EditorGUILayout.LabelField("配置文件路径:", "Assets/config/level_editor_config.json");
        
        if (GUILayout.Button("打开配置文件"))
        {
            #if UNITY_EDITOR
            var configAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/config/level_editor_config.json");
            if (configAsset != null)
            {
                UnityEditor.Selection.activeObject = configAsset;
                UnityEditor.EditorGUIUtility.PingObject(configAsset);
            }
            #endif
        }
    }
    
    void TestConfigLoading()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"测试前关卡索引: {beforeIndex}");
        
        // 模拟重启时的配置加载
        config.LoadConfigFromFile();
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"测试后关卡索引: {afterIndex}");
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 关卡索引保持成功！");
        }
        else
        {
            Debug.LogError($"❌ 关卡索引被重置: {beforeIndex} -> {afterIndex}");
        }
    }
    
    void TestDefaultConfigInitialization()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"初始化前关卡索引: {beforeIndex}");
        
        // 测试默认配置初始化
        config.InitializeDefaultConfig();
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"初始化后关卡索引: {afterIndex}");
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 默认配置初始化保持关卡索引成功！");
        }
        else
        {
            Debug.LogError($"❌ 默认配置初始化重置了关卡索引: {beforeIndex} -> {afterIndex}");
        }
    }
} 