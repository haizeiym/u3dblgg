using UnityEngine;
using UnityEditor;

/// <summary>
/// 工厂测试窗口
/// 专门用于测试UI更新器工厂的功能
/// </summary>
public class FactoryTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/工厂测试")]
    public static void ShowWindow()
    {
        GetWindow<FactoryTestWindow>("工厂测试");
    }

    void OnGUI()
    {
        GUILayout.Label("UI更新器工厂测试", EditorStyles.boldLabel);
        
        if (GUILayout.Button("测试工厂创建"))
        {
            TestFactoryCreation();
        }
        
        if (GUILayout.Button("测试Editor创建器"))
        {
            TestEditorCreator();
        }
        
        if (GUILayout.Button("测试类型查找"))
        {
            TestTypeFinding();
        }
        
        if (GUILayout.Button("测试实例创建"))
        {
            TestInstanceCreation();
        }
        
        if (GUILayout.Button("测试接口调用"))
        {
            TestInterfaceCall();
        }
        
        if (GUILayout.Button("完整工厂测试"))
        {
            FullFactoryTest();
        }
    }
    
    void TestFactoryCreation()
    {
        Debug.Log("=== 测试工厂创建 ===");
        
        try
        {
            var editorUI = Object.FindObjectOfType<LevelEditorUI>();
            if (editorUI == null)
            {
                Debug.LogWarning("未找到LevelEditorUI实例，请确保场景中有LevelEditorUI组件");
                return;
            }
            
            var uiUpdater = UIUpdaterFactory.CreateUIUpdater(editorUI);
            if (uiUpdater != null)
            {
                Debug.Log("✓ 工厂创建UI更新器成功");
                Debug.Log($"  类型: {uiUpdater.GetType().Name}");
                Debug.Log($"  接口实现: {uiUpdater is IUIUpdater}");
            }
            else
            {
                Debug.LogError("✗ 工厂创建UI更新器失败");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 工厂创建测试失败: {e.Message}");
        }
    }
    
    void TestEditorCreator()
    {
        Debug.Log("=== 测试Editor创建器 ===");
        
        try
        {
            var editorUI = Object.FindObjectOfType<LevelEditorUI>();
            if (editorUI == null)
            {
                Debug.LogWarning("未找到LevelEditorUI实例，请确保场景中有LevelEditorUI组件");
                return;
            }
            
            var uiUpdater = UIUpdaterCreator.CreateUIUpdater(editorUI);
            if (uiUpdater != null)
            {
                Debug.Log("✓ Editor创建器创建UI更新器成功");
                Debug.Log($"  类型: {uiUpdater.GetType().Name}");
                Debug.Log($"  接口实现: {uiUpdater is IUIUpdater}");
            }
            else
            {
                Debug.LogError("✗ Editor创建器创建UI更新器失败");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Editor创建器测试失败: {e.Message}");
        }
    }
    
    void TestTypeFinding()
    {
        Debug.Log("=== 测试类型查找 ===");
        
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
                Debug.Log($"  类型名称: {type.Name}");
                Debug.Log($"  命名空间: {type.Namespace}");
            }
            else
            {
                Debug.LogError($"✗ 未找到类型: {typeName}");
            }
        }
    }
    
    void TestInstanceCreation()
    {
        Debug.Log("=== 测试实例创建 ===");
        
        try
        {
            var editorUI = Object.FindObjectOfType<LevelEditorUI>();
            if (editorUI == null)
            {
                Debug.LogWarning("未找到LevelEditorUI实例");
                return;
            }
            
            var uiBuilderType = System.Type.GetType("LevelEditorUIBuilder, Assembly-CSharp-Editor");
            if (uiBuilderType != null)
            {
                var uiBuilder = System.Activator.CreateInstance(uiBuilderType, editorUI);
                if (uiBuilder != null)
                {
                    Debug.Log("✓ UI构建器创建成功");
                    
                    var uiUpdaterType = System.Type.GetType("LevelEditorUIUpdater, Assembly-CSharp-Editor");
                    if (uiUpdaterType != null)
                    {
                        var uiUpdater = System.Activator.CreateInstance(uiUpdaterType, editorUI, uiBuilder);
                        if (uiUpdater != null)
                        {
                            Debug.Log("✓ UI更新器创建成功");
                            Debug.Log($"  类型: {uiUpdater.GetType().Name}");
                        }
                        else
                        {
                            Debug.LogError("✗ UI更新器创建失败");
                        }
                    }
                    else
                    {
                        Debug.LogError("✗ 未找到UI更新器类型");
                    }
                }
                else
                {
                    Debug.LogError("✗ UI构建器创建失败");
                }
            }
            else
            {
                Debug.LogError("✗ 未找到UI构建器类型");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 实例创建测试失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
        }
    }
    
    void TestInterfaceCall()
    {
        Debug.Log("=== 测试接口调用 ===");
        
        try
        {
            var editorUI = Object.FindObjectOfType<LevelEditorUI>();
            if (editorUI == null)
            {
                Debug.LogWarning("未找到LevelEditorUI实例");
                return;
            }
            
            var uiUpdater = UIUpdaterFactory.CreateUIUpdater(editorUI);
            if (uiUpdater != null)
            {
                // 测试接口方法
                var unsubscribeMethod = typeof(IUIUpdater).GetMethod("UnsubscribeFromConfigEvents");
                if (unsubscribeMethod != null)
                {
                    unsubscribeMethod.Invoke(uiUpdater, null);
                    Debug.Log("✓ 接口方法调用成功");
                }
                else
                {
                    Debug.LogError("✗ 未找到接口方法");
                }
            }
            else
            {
                Debug.LogError("✗ UI更新器创建失败，无法测试接口调用");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 接口调用测试失败: {e.Message}");
        }
    }
    
    void FullFactoryTest()
    {
        Debug.Log("=== 完整工厂测试 ===");
        
        TestTypeFinding();
        TestInstanceCreation();
        TestFactoryCreation();
        TestEditorCreator();
        TestInterfaceCall();
        
        Debug.Log("=== 完整工厂测试完成 ===");
    }
} 