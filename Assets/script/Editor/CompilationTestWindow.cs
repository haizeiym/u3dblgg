using UnityEngine;
using UnityEditor;

/// <summary>
/// 编译测试窗口
/// 用于验证所有组件是否正常编译和工作
/// </summary>
public class CompilationTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/编译测试")]
    public static void ShowWindow()
    {
        GetWindow<CompilationTestWindow>("编译测试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("编译测试", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个窗口用于验证所有组件是否正常编译和工作", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("测试配置系统"))
        {
            TestConfigSystem();
        }

        if (GUILayout.Button("测试UI更新器"))
        {
            TestUIUpdater();
        }

        if (GUILayout.Button("测试接口系统"))
        {
            TestInterfaceSystem();
        }

        if (GUILayout.Button("测试反射系统"))
        {
            TestReflectionSystem();
        }

        if (GUILayout.Button("测试工厂系统"))
        {
            TestFactorySystem();
        }

        if (GUILayout.Button("完整系统测试"))
        {
            FullSystemTest();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("测试结果:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("查看控制台输出获取详细测试结果");
    }

    void TestConfigSystem()
    {
        Debug.Log("=== 测试配置系统 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log("✓ 配置实例创建成功");
            Debug.Log($"  形状类型数量: {config.shapeTypes.Count}");
            Debug.Log($"  球类型数量: {config.ballTypes.Count}");
            Debug.Log($"  背景配置数量: {config.backgroundConfigs.Count}");
        }
        else
        {
            Debug.LogError("✗ 配置实例创建失败");
        }
    }

    void TestUIUpdater()
    {
        Debug.Log("=== 测试UI更新器 ===");
        
        // 测试接口
        var interfaceType = typeof(IUIUpdater);
        if (interfaceType != null)
        {
            Debug.Log("✓ IUIUpdater接口存在");
        }
        else
        {
            Debug.LogError("✗ IUIUpdater接口不存在");
        }
        
        // 测试UI更新器类型
        var uiUpdaterType = System.Type.GetType("LevelEditorUIUpdater, Assembly-CSharp-Editor");
        if (uiUpdaterType != null)
        {
            Debug.Log("✓ LevelEditorUIUpdater类型存在");
            
            // 检查是否实现了接口
            if (typeof(IUIUpdater).IsAssignableFrom(uiUpdaterType))
            {
                Debug.Log("✓ LevelEditorUIUpdater实现了IUIUpdater接口");
            }
            else
            {
                Debug.LogError("✗ LevelEditorUIUpdater未实现IUIUpdater接口");
            }
        }
        else
        {
            Debug.LogError("✗ LevelEditorUIUpdater类型不存在");
        }
    }

    void TestInterfaceSystem()
    {
        Debug.Log("=== 测试接口系统 ===");
        
        try
        {
            // 尝试创建UI更新器实例
            var uiUpdaterType = System.Type.GetType("LevelEditorUIUpdater, Assembly-CSharp-Editor");
            if (uiUpdaterType != null)
            {
                // 查找LevelEditorUI实例
                var editorUI = Object.FindObjectOfType<LevelEditorUI>();
                if (editorUI != null)
                {
                    // 尝试创建UI构建器
                    var uiBuilderType = System.Type.GetType("LevelEditorUIBuilder, Assembly-CSharp-Editor");
                    if (uiBuilderType != null)
                    {
                        var uiBuilder = System.Activator.CreateInstance(uiBuilderType, editorUI);
                        var uiUpdater = System.Activator.CreateInstance(uiUpdaterType, editorUI, uiBuilder);
                        
                        if (uiUpdater is IUIUpdater)
                        {
                            Debug.Log("✓ 接口系统工作正常");
                            Debug.Log($"  创建的UI更新器类型: {uiUpdater.GetType().Name}");
                            
                            // 测试接口方法
                            var interfaceMethod = typeof(IUIUpdater).GetMethod("UnsubscribeFromConfigEvents");
                            if (interfaceMethod != null)
                            {
                                Debug.Log("✓ 接口方法存在");
                            }
                            else
                            {
                                Debug.LogError("✗ 接口方法不存在");
                            }
                        }
                        else
                        {
                            Debug.LogError("✗ 创建的实例未实现IUIUpdater接口");
                        }
                    }
                    else
                    {
                        Debug.LogError("✗ 无法找到LevelEditorUIBuilder类型");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠ 未找到LevelEditorUI实例，跳过接口测试");
                }
            }
            else
            {
                Debug.LogError("✗ 无法找到LevelEditorUIUpdater类型");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 接口系统测试失败: {e.Message}");
        }
    }

    void TestReflectionSystem()
    {
        Debug.Log("=== 测试反射系统 ===");
        
        try
        {
            // 测试类型查找
            var types = new string[]
            {
                "LevelEditorUIBuilder, Assembly-CSharp-Editor",
                "LevelEditorUIUpdater, Assembly-CSharp-Editor",
                "IUIUpdater, Assembly-CSharp"
            };
            
            foreach (var typeName in types)
            {
                var type = System.Type.GetType(typeName);
                if (type != null)
                {
                    Debug.Log($"✓ 找到类型: {typeName}");
                }
                else
                {
                    Debug.LogError($"✗ 未找到类型: {typeName}");
                }
            }
            
            // 测试实例创建
            var uiUpdaterType = System.Type.GetType("LevelEditorUIUpdater, Assembly-CSharp-Editor");
            if (uiUpdaterType != null)
            {
                var editorUI = Object.FindObjectOfType<LevelEditorUI>();
                if (editorUI != null)
                {
                    var uiBuilderType = System.Type.GetType("LevelEditorUIBuilder, Assembly-CSharp-Editor");
                    if (uiBuilderType != null)
                    {
                        var uiBuilder = System.Activator.CreateInstance(uiBuilderType, editorUI);
                        var uiUpdater = System.Activator.CreateInstance(uiUpdaterType, editorUI, uiBuilder);
                        
                        Debug.Log("✓ 反射创建实例成功");
                        Debug.Log($"  实例类型: {uiUpdater.GetType().Name}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 反射系统测试失败: {e.Message}");
        }
    }

    void TestFactorySystem()
    {
        Debug.Log("=== 测试工厂系统 ===");
        
        try
        {
            var editorUI = Object.FindObjectOfType<LevelEditorUI>();
            if (editorUI != null)
            {
                // 测试工厂创建UI更新器
                var uiUpdater = UIUpdaterFactory.CreateUIUpdater(editorUI);
                if (uiUpdater != null)
                {
                    Debug.Log("✓ 工厂创建UI更新器成功");
                    Debug.Log($"  创建的UI更新器类型: {uiUpdater.GetType().Name}");
                    
                    // 测试接口方法
                    var method = typeof(IUIUpdater).GetMethod("UnsubscribeFromConfigEvents");
                    if (method != null)
                    {
                        Debug.Log("✓ 接口方法存在");
                        
                        // 测试方法调用
                        method.Invoke(uiUpdater, null);
                        Debug.Log("✓ 接口方法调用成功");
                    }
                    else
                    {
                        Debug.LogError("✗ 接口方法不存在");
                    }
                }
                else
                {
                    Debug.LogError("✗ 工厂创建UI更新器失败");
                }
            }
            else
            {
                Debug.LogWarning("⚠ 未找到LevelEditorUI实例，跳过工厂测试");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 工厂系统测试失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
        }
    }

    void FullSystemTest()
    {
        Debug.Log("=== 完整系统测试 ===");
        
        TestConfigSystem();
        TestUIUpdater();
        TestInterfaceSystem();
        TestReflectionSystem();
        TestFactorySystem();
        
        Debug.Log("=== 完整系统测试完成 ===");
    }
} 