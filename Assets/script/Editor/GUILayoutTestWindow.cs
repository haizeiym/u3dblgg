using UnityEngine;
using UnityEditor;

/// <summary>
/// GUILayout测试工具
/// 用于测试配置编辑器的GUILayout修复效果
/// </summary>
public class GUILayoutTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/GUILayout测试")]
    public static void ShowWindow()
    {
        GetWindow<GUILayoutTestWindow>("GUILayout测试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("GUILayout测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试配置编辑器的GUILayout修复效果", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("打开配置编辑器"))
        {
            OpenConfigEditor();
        }

        if (GUILayout.Button("创建测试数据"))
        {
            CreateTestData();
        }

        if (GUILayout.Button("测试删除操作"))
        {
            TestDeleteOperations();
        }

        if (GUILayout.Button("验证GUI状态"))
        {
            ValidateGUIState();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("测试说明:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 打开配置编辑器");
        EditorGUILayout.LabelField("2. 添加一些测试数据");
        EditorGUILayout.LabelField("3. 尝试删除项目，观察是否出现GUI错误");
        EditorGUILayout.LabelField("4. 检查控制台是否有GUILayout错误");
    }

    void OpenConfigEditor()
    {
        var configWindowType = System.Type.GetType("LevelEditorConfigWindow, Assembly-CSharp-Editor");
        if (configWindowType != null)
        {
            EditorWindow.GetWindow(configWindowType, false, "关卡编辑器配置");
            Debug.Log("配置编辑器已打开");
        }
        else
        {
            Debug.LogError("无法找到配置编辑器类型");
        }
    }

    void CreateTestData()
    {
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空");
            return;
        }

        Debug.Log("创建测试数据...");

        // 添加测试形状
        config.AddShapeType("测试形状1");
        config.AddShapeType("测试形状2");
        config.AddShapeType("测试形状3");

        // 添加测试球
        config.AddBallType("测试红球", Color.red);
        config.AddBallType("测试蓝球", Color.blue);
        config.AddBallType("测试绿球", Color.green);

        // 添加测试背景
        config.AddBackgroundConfig("测试背景1", null, Color.white);
        config.AddBackgroundConfig("测试背景2", null, Color.gray);
        config.AddBackgroundConfig("测试背景3", null, Color.black);

        // 保存配置
        config.SaveConfigToFile();
        Debug.Log("测试数据创建完成");
    }

    void TestDeleteOperations()
    {
        Debug.Log("=== 测试删除操作 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空");
            return;
        }

        // 测试删除形状
        if (config.shapeTypes.Count > 0)
        {
            Debug.Log($"删除形状: {config.shapeTypes[0].name}");
            config.shapeTypes.RemoveAt(0);
            config.TriggerShapeTypesChanged();
        }

        // 测试删除球
        if (config.ballTypes.Count > 0)
        {
            Debug.Log($"删除球: {config.ballTypes[0].name}");
            config.ballTypes.RemoveAt(0);
            config.TriggerBallTypesChanged();
        }

        // 测试删除背景
        if (config.backgroundConfigs.Count > 0)
        {
            Debug.Log($"删除背景: {config.backgroundConfigs[0].name}");
            config.backgroundConfigs.RemoveAt(0);
            config.TriggerBackgroundConfigsChanged();
        }

        // 保存配置
        config.SaveConfigToFile();
        Debug.Log("删除操作测试完成");
    }

    void ValidateGUIState()
    {
        Debug.Log("=== 验证GUI状态 ===");
        
        // 检查是否有未关闭的GUI布局
        try
        {
            // 尝试开始一个新的垂直布局
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("GUI状态正常");
            EditorGUILayout.EndVertical();
            Debug.Log("GUI状态验证通过");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GUI状态异常: {e.Message}");
        }
    }
} 