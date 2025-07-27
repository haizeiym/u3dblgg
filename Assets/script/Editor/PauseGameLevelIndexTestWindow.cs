using UnityEngine;
using UnityEditor;

public class PauseGameLevelIndexTestWindow : EditorWindow
{
    // [MenuItem("Tools/Level Editor/Test Pause Game Level Index")]
    public static void ShowWindow()
    {
        GetWindow<PauseGameLevelIndexTestWindow>("暂停游戏关卡索引测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("暂停游戏关卡索引测试", EditorStyles.boldLabel);
        
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
        if (GUILayout.Button("模拟暂停游戏时的配置重新加载"))
        {
            TestPauseGameConfigReload();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("模拟组件重新初始化"))
        {
            TestComponentReinitialization();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("测试单例重新创建"))
        {
            TestSingletonRecreation();
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
        
        // 显示配置状态
        EditorGUILayout.LabelField("配置状态:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("形状类型数量:", config.shapeTypes.Count.ToString());
        EditorGUILayout.LabelField("球类型数量:", config.ballTypes.Count.ToString());
        EditorGUILayout.LabelField("背景配置数量:", config.backgroundConfigs.Count.ToString());
    }
    
    void TestPauseGameConfigReload()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"测试前关卡索引: {beforeIndex}");
        
        // 模拟暂停游戏时的配置重新加载
        config.LoadConfigFromFile();
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"测试后关卡索引: {afterIndex}");
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 暂停游戏时关卡索引保持成功！");
        }
        else
        {
            Debug.LogError($"❌ 暂停游戏时关卡索引被重置: {beforeIndex} -> {afterIndex}");
        }
    }
    
    void TestComponentReinitialization()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"组件重新初始化前关卡索引: {beforeIndex}");
        
        // 模拟组件重新初始化时的配置加载
        // 这里模拟BackgroundManager和ConfigPreviewUI的Start方法逻辑
        if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
        {
            Debug.Log("模拟组件重新初始化: 配置为空，重新加载");
            config.LoadConfigFromFile();
        }
        else
        {
            Debug.Log("模拟组件重新初始化: 配置已存在，跳过重新加载");
        }
        
        int afterIndex = config.GetLevelIndex();
        Debug.Log($"组件重新初始化后关卡索引: {afterIndex}");
        
        if (beforeIndex == afterIndex)
        {
            Debug.Log("✅ 组件重新初始化时关卡索引保持成功！");
        }
        else
        {
            Debug.LogError($"❌ 组件重新初始化时关卡索引被重置: {beforeIndex} -> {afterIndex}");
        }
    }
    
    void TestSingletonRecreation()
    {
        var config = LevelEditorConfig.Instance;
        int beforeIndex = config.GetLevelIndex();
        Debug.Log($"单例重新创建前关卡索引: {beforeIndex}");
        
        // 模拟单例重新创建（通过反射清空_instance）
        var field = typeof(LevelEditorConfig).GetField("_instance", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (field != null)
        {
            field.SetValue(null, null);
            Debug.Log("已清空单例实例");
            
            // 重新获取实例
            var newConfig = LevelEditorConfig.Instance;
            int afterIndex = newConfig.GetLevelIndex();
            Debug.Log($"单例重新创建后关卡索引: {afterIndex}");
            
            if (beforeIndex == afterIndex)
            {
                Debug.Log("✅ 单例重新创建时关卡索引保持成功！");
            }
            else
            {
                Debug.LogError($"❌ 单例重新创建时关卡索引被重置: {beforeIndex} -> {afterIndex}");
            }
        }
        else
        {
            Debug.LogError("无法访问_instance字段");
        }
    }
} 