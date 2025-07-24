using UnityEngine;
using UnityEditor;

/// <summary>
/// 事件系统测试窗口
/// 用于测试配置变更事件系统是否正常工作
/// </summary>
public class EventSystemTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/事件系统测试")]
    public static void ShowWindow()
    {
        GetWindow<EventSystemTestWindow>("事件系统测试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("事件系统测试", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个窗口用于测试配置变更事件系统是否正常工作", MessageType.Info);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("事件触发测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("测试形状类型事件"))
        {
            TestShapeTypeEvent();
        }

        if (GUILayout.Button("测试球类型事件"))
        {
            TestBallTypeEvent();
        }

        if (GUILayout.Button("测试背景配置事件"))
        {
            TestBackgroundConfigEvent();
        }

        if (GUILayout.Button("测试当前背景事件"))
        {
            TestCurrentBackgroundEvent();
        }

        if (GUILayout.Button("测试配置重新加载事件"))
        {
            TestConfigReloadedEvent();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("事件订阅测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("测试事件订阅"))
        {
            TestEventSubscription();
        }

        if (GUILayout.Button("测试事件取消订阅"))
        {
            TestEventUnsubscription();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("测试说明:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 点击按钮测试各种事件触发");
        EditorGUILayout.LabelField("2. 查看控制台输出确认事件正常工作");
        EditorGUILayout.LabelField("3. 观察UI是否响应事件更新");
    }

    void TestShapeTypeEvent()
    {
        Debug.Log("=== 测试形状类型事件 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log("触发形状类型变更事件...");
            config.TriggerShapeTypesChanged();
            Debug.Log("✓ 形状类型事件触发完成");
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    void TestBallTypeEvent()
    {
        Debug.Log("=== 测试球类型事件 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log("触发球类型变更事件...");
            config.TriggerBallTypesChanged();
            Debug.Log("✓ 球类型事件触发完成");
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    void TestBackgroundConfigEvent()
    {
        Debug.Log("=== 测试背景配置事件 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log("触发背景配置变更事件...");
            config.TriggerBackgroundConfigsChanged();
            Debug.Log("✓ 背景配置事件触发完成");
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    void TestCurrentBackgroundEvent()
    {
        Debug.Log("=== 测试当前背景事件 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log("触发当前背景变更事件...");
            config.TriggerCurrentBackgroundChanged();
            Debug.Log("✓ 当前背景事件触发完成");
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    void TestConfigReloadedEvent()
    {
        Debug.Log("=== 测试配置重新加载事件 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log("触发配置重新加载事件...");
            config.TriggerConfigReloaded();
            Debug.Log("✓ 配置重新加载事件触发完成");
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    void TestEventSubscription()
    {
        Debug.Log("=== 测试事件订阅 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            // 测试订阅事件
            config.OnShapeTypesChanged += OnShapeTypesChangedTest;
            config.OnBallTypesChanged += OnBallTypesChangedTest;
            config.OnBackgroundConfigsChanged += OnBackgroundConfigsChangedTest;
            config.OnCurrentBackgroundChanged += OnCurrentBackgroundChangedTest;
            config.OnConfigReloaded += OnConfigReloadedTest;
            
            Debug.Log("✓ 事件订阅完成");
            Debug.Log("现在触发事件来测试订阅...");
            
            // 触发事件测试订阅
            config.TriggerShapeTypesChanged();
            config.TriggerBallTypesChanged();
            config.TriggerBackgroundConfigsChanged();
            config.TriggerCurrentBackgroundChanged();
            config.TriggerConfigReloaded();
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    void TestEventUnsubscription()
    {
        Debug.Log("=== 测试事件取消订阅 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            // 取消订阅事件
            config.OnShapeTypesChanged -= OnShapeTypesChangedTest;
            config.OnBallTypesChanged -= OnBallTypesChangedTest;
            config.OnBackgroundConfigsChanged -= OnBackgroundConfigsChangedTest;
            config.OnCurrentBackgroundChanged -= OnCurrentBackgroundChangedTest;
            config.OnConfigReloaded -= OnConfigReloadedTest;
            
            Debug.Log("✓ 事件取消订阅完成");
            Debug.Log("现在触发事件来测试取消订阅...");
            
            // 触发事件测试取消订阅（应该没有输出）
            config.TriggerShapeTypesChanged();
            config.TriggerBallTypesChanged();
            config.TriggerBackgroundConfigsChanged();
            config.TriggerCurrentBackgroundChanged();
            config.TriggerConfigReloaded();
        }
        else
        {
            Debug.LogError("✗ 配置实例为空");
        }
    }

    // 测试事件处理方法
    void OnShapeTypesChangedTest()
    {
        Debug.Log("✓ 形状类型变更事件被触发");
    }

    void OnBallTypesChangedTest()
    {
        Debug.Log("✓ 球类型变更事件被触发");
    }

    void OnBackgroundConfigsChangedTest()
    {
        Debug.Log("✓ 背景配置变更事件被触发");
    }

    void OnCurrentBackgroundChangedTest()
    {
        Debug.Log("✓ 当前背景变更事件被触发");
    }

    void OnConfigReloadedTest()
    {
        Debug.Log("✓ 配置重新加载事件被触发");
    }
} 