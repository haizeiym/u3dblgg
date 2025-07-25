using UnityEngine;
using UnityEditor;

public class LevelIndexTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/Test Level Index")]
    public static void ShowWindow()
    {
        GetWindow<LevelIndexTestWindow>("关卡索引测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("关卡索引测试", EditorStyles.boldLabel);
        
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
        
        // 生成关卡名称
        string levelName = $"LevelConfig_{currentIndex}";
        EditorGUILayout.LabelField("生成的关卡名称:", levelName);
        
        EditorGUILayout.Space();
        
        // 保存配置按钮
        if (GUILayout.Button("保存配置"))
        {
            config.SaveConfigToFile();
            Debug.Log("配置已保存");
        }
        
        EditorGUILayout.Space();
        
        // 重新加载配置按钮
        if (GUILayout.Button("重新加载配置"))
        {
            config.LoadConfigFromFile();
            Debug.Log("配置已重新加载");
            Repaint();
        }
    }
} 