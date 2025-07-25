using UnityEngine;
using UnityEditor;

public class RuntimeLevelNameTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/Test Runtime Level Name")]
    public static void ShowWindow()
    {
        GetWindow<RuntimeLevelNameTestWindow>("运行时关卡名称测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("运行时关卡名称测试", EditorStyles.boldLabel);
        
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
        
        // 显示预期的关卡名称
        string expectedLevelName = $"LevelConfig_{currentIndex}";
        EditorGUILayout.LabelField("预期的关卡名称:", expectedLevelName);
        
        EditorGUILayout.Space();
        
        // 测试按钮
        if (GUILayout.Button("模拟运行时初始化"))
        {
            SimulateRuntimeInitialization();
        }
        
        EditorGUILayout.Space();
        
        // 查找场景中的LevelEditorUI
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI != null)
        {
            EditorGUILayout.LabelField("场景中的LevelEditorUI:", "已找到");
            EditorGUILayout.LabelField("当前关卡名称:", levelEditorUI.currentLevel?.levelName ?? "无");
            
            if (GUILayout.Button("强制更新关卡名称"))
            {
                ForceUpdateLevelName(levelEditorUI);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("场景中未找到LevelEditorUI", MessageType.Warning);
        }
        
        EditorGUILayout.Space();
        
        // 保存配置按钮
        if (GUILayout.Button("保存配置"))
        {
            config.SaveConfigToFile();
            Debug.Log("配置已保存");
        }
    }
    
    void SimulateRuntimeInitialization()
    {
        var config = LevelEditorConfig.Instance;
        int currentIndex = config.GetLevelIndex();
        string expectedLevelName = $"LevelConfig_{currentIndex}";
        
        Debug.Log($"模拟运行时初始化:");
        Debug.Log($"配置索引: {currentIndex}");
        Debug.Log($"预期关卡名称: {expectedLevelName}");
        
        // 查找场景中的LevelEditorUI
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI != null)
        {
            Debug.Log($"当前关卡名称: {levelEditorUI.currentLevel?.levelName ?? "无"}");
            
            if (levelEditorUI.currentLevel != null)
            {
                string oldName = levelEditorUI.currentLevel.levelName;
                levelEditorUI.currentLevel.levelName = expectedLevelName;
                Debug.Log($"关卡名称已更新: {oldName} -> {expectedLevelName}");
            }
        }
    }
    
    void ForceUpdateLevelName(LevelEditorUI levelEditorUI)
    {
        var config = LevelEditorConfig.Instance;
        int currentIndex = config.GetLevelIndex();
        string expectedLevelName = $"LevelConfig_{currentIndex}";
        
        if (levelEditorUI.currentLevel != null)
        {
            string oldName = levelEditorUI.currentLevel.levelName;
            levelEditorUI.currentLevel.levelName = expectedLevelName;
            Debug.Log($"强制更新关卡名称: {oldName} -> {expectedLevelName}");
            
            // 更新UI显示
            if (levelEditorUI.levelNameInput != null)
            {
                levelEditorUI.levelNameInput.text = expectedLevelName;
                Debug.Log("UI输入框已更新");
            }
        }
    }
} 