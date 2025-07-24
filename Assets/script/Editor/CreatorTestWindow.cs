using UnityEngine;
using UnityEditor;

/// <summary>
/// 创建器测试窗口
/// 专门用于测试UI更新器创建器的功能
/// </summary>
public class CreatorTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/创建器测试")]
    public static void ShowWindow()
    {
        GetWindow<CreatorTestWindow>("创建器测试");
    }

    void OnGUI()
    {
        GUILayout.Label("UI更新器创建器测试", EditorStyles.boldLabel);
        
        if (GUILayout.Button("测试Editor创建器"))
        {
            TestEditorCreator();
        }
        
        if (GUILayout.Button("测试反射工厂"))
        {
            TestReflectionFactory();
        }
        
        if (GUILayout.Button("测试类型查找"))
        {
            TestTypeFinding();
        }
        
        if (GUILayout.Button("测试构造函数"))
        {
            TestConstructors();
        }
        
        if (GUILayout.Button("完整创建器测试"))
        {
            FullCreatorTest();
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
                
                // 测试接口方法
                var method = typeof(IUIUpdater).GetMethod("UnsubscribeFromConfigEvents");
                if (method != null)
                {
                    method.Invoke(uiUpdater, null);
                    Debug.Log("✓ 接口方法调用成功");
                }
            }
            else
            {
                Debug.LogError("✗ Editor创建器创建UI更新器失败");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Editor创建器测试失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
        }
    }
    
    void TestReflectionFactory()
    {
        Debug.Log("=== 测试反射工厂 ===");
        
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
                Debug.Log("✓ 反射工厂创建UI更新器成功");
                Debug.Log($"  类型: {uiUpdater.GetType().Name}");
                Debug.Log($"  接口实现: {uiUpdater is IUIUpdater}");
            }
            else
            {
                Debug.LogError("✗ 反射工厂创建UI更新器失败");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 反射工厂测试失败: {e.Message}");
        }
    }
    
    void TestTypeFinding()
    {
        Debug.Log("=== 测试类型查找 ===");
        
        var types = new string[]
        {
            "UIUpdaterCreator, Assembly-CSharp-Editor",
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
    
    void TestConstructors()
    {
        Debug.Log("=== 测试构造函数 ===");
        
        try
        {
            var uiBuilderType = System.Type.GetType("LevelEditorUIBuilder, Assembly-CSharp-Editor");
            var uiUpdaterType = System.Type.GetType("LevelEditorUIUpdater, Assembly-CSharp-Editor");
            
            if (uiBuilderType != null)
            {
                var constructors = uiBuilderType.GetConstructors();
                Debug.Log($"UI构建器构造函数数量: {constructors.Length}");
                
                foreach (var ctor in constructors)
                {
                    var parameters = ctor.GetParameters();
                    Debug.Log($"  构造函数参数数量: {parameters.Length}");
                    foreach (var param in parameters)
                    {
                        Debug.Log($"    参数: {param.ParameterType.Name} {param.Name}");
                    }
                }
            }
            
            if (uiUpdaterType != null)
            {
                var constructors = uiUpdaterType.GetConstructors();
                Debug.Log($"UI更新器构造函数数量: {constructors.Length}");
                
                foreach (var ctor in constructors)
                {
                    var parameters = ctor.GetParameters();
                    Debug.Log($"  构造函数参数数量: {parameters.Length}");
                    foreach (var param in parameters)
                    {
                        Debug.Log($"    参数: {param.ParameterType.Name} {param.Name}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 构造函数测试失败: {e.Message}");
        }
    }
    
    void FullCreatorTest()
    {
        Debug.Log("=== 完整创建器测试 ===");
        
        TestTypeFinding();
        TestConstructors();
        TestEditorCreator();
        TestReflectionFactory();
        
        Debug.Log("=== 完整创建器测试完成 ===");
    }
} 