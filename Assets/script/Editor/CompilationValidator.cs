using UnityEngine;
using UnityEditor;

/// <summary>
/// 编译验证器
/// 用于验证所有关键组件是否正常编译
/// </summary>
public static class CompilationValidator
{
    [MenuItem("Tools/Level Editor/验证编译")]
    public static void ValidateCompilation()
    {
        Debug.Log("=== 开始编译验证 ===");
        
        // 验证运行时类型
        ValidateRuntimeTypes();
        
        // 验证Editor类型
        ValidateEditorTypes();
        
        // 验证工厂系统
        ValidateFactorySystem();
        
        // 验证接口系统
        ValidateInterfaceSystem();
        
        Debug.Log("=== 编译验证完成 ===");
    }
    
    static void ValidateRuntimeTypes()
    {
        Debug.Log("--- 验证运行时类型 ---");
        
        var runtimeTypes = new string[]
        {
            "LevelEditorUI",
            "LevelEditorConfig", 
            "IUIUpdater",
            "UIUpdaterFactory"
        };
        
        foreach (var typeName in runtimeTypes)
        {
            var type = System.Type.GetType(typeName);
            if (type != null)
            {
                Debug.Log($"✓ 运行时类型 {typeName} 正常");
            }
            else
            {
                Debug.LogError($"✗ 运行时类型 {typeName} 未找到");
            }
        }
    }
    
    static void ValidateEditorTypes()
    {
        Debug.Log("--- 验证Editor类型 ---");
        
        var editorTypes = new string[]
        {
            "LevelEditorUIBuilder, Assembly-CSharp-Editor",
            "LevelEditorUIUpdater, Assembly-CSharp-Editor",
            "CompilationTestWindow, Assembly-CSharp-Editor",
            "FactoryTestWindow, Assembly-CSharp-Editor"
        };
        
        foreach (var typeName in editorTypes)
        {
            var type = System.Type.GetType(typeName);
            if (type != null)
            {
                Debug.Log($"✓ Editor类型 {typeName} 正常");
            }
            else
            {
                Debug.LogError($"✗ Editor类型 {typeName} 未找到");
            }
        }
    }
    
    static void ValidateFactorySystem()
    {
        Debug.Log("--- 验证工厂系统 ---");
        
        try
        {
            var editorUI = Object.FindObjectOfType<LevelEditorUI>();
            if (editorUI != null)
            {
                var uiUpdater = UIUpdaterFactory.CreateUIUpdater(editorUI);
                if (uiUpdater != null)
                {
                    Debug.Log("✓ 工厂系统正常工作");
                    Debug.Log($"  创建的UI更新器类型: {uiUpdater.GetType().Name}");
                }
                else
                {
                    Debug.LogError("✗ 工厂创建UI更新器失败");
                }
            }
            else
            {
                Debug.LogWarning("⚠ 未找到LevelEditorUI实例，跳过工厂验证");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 工厂系统验证失败: {e.Message}");
        }
    }
    
    static void ValidateInterfaceSystem()
    {
        Debug.Log("--- 验证接口系统 ---");
        
        try
        {
            var interfaceType = typeof(IUIUpdater);
            if (interfaceType != null)
            {
                Debug.Log("✓ IUIUpdater接口存在");
                
                var methods = interfaceType.GetMethods();
                Debug.Log($"  接口方法数量: {methods.Length}");
                
                foreach (var method in methods)
                {
                    Debug.Log($"  方法: {method.Name}");
                }
            }
            else
            {
                Debug.LogError("✗ IUIUpdater接口未找到");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 接口系统验证失败: {e.Message}");
        }
    }
} 