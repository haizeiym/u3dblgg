using UnityEngine;

/// <summary>
/// UI更新器工厂（运行时版本）
/// 用于创建UI更新器实例，通过反射访问Editor中的类
/// </summary>
public static class UIUpdaterFactory
{
    /// <summary>
    /// 创建UI更新器实例
    /// </summary>
    /// <param name="editorUI">LevelEditorUI实例</param>
    /// <returns>UI更新器实例</returns>
    public static IUIUpdater CreateUIUpdater(LevelEditorUI editorUI)
    {
        if (editorUI == null)
        {
            Debug.LogError("LevelEditorUI实例为空，无法创建UI更新器");
            return null;
        }

        try
        {
            Debug.Log("开始创建UI更新器...");
            
            // 通过反射创建UI构建器和UI更新器
            var uiBuilderType = System.Type.GetType("LevelEditorUIBuilder, Assembly-CSharp-Editor");
            var uiUpdaterType = System.Type.GetType("LevelEditorUIUpdater, Assembly-CSharp-Editor");
            
            Debug.Log($"UI构建器类型查找结果: {uiBuilderType != null}");
            Debug.Log($"UI更新器类型查找结果: {uiUpdaterType != null}");
            
            if (uiBuilderType == null)
            {
                Debug.LogError("未找到LevelEditorUIBuilder类型");
                return null;
            }
            
            if (uiUpdaterType == null)
            {
                Debug.LogError("未找到LevelEditorUIUpdater类型");
                return null;
            }
            
            // 检查构造函数
            var uiBuilderConstructors = uiBuilderType.GetConstructors();
            var uiUpdaterConstructors = uiUpdaterType.GetConstructors();
            
            Debug.Log($"UI构建器构造函数数量: {uiBuilderConstructors.Length}");
            Debug.Log($"UI更新器构造函数数量: {uiUpdaterConstructors.Length}");
            
            foreach (var ctor in uiBuilderConstructors)
            {
                var parameters = ctor.GetParameters();
                Debug.Log($"UI构建器构造函数参数: {parameters.Length}");
                foreach (var param in parameters)
                {
                    Debug.Log($"  参数类型: {param.ParameterType.Name}");
                }
            }
            
            foreach (var ctor in uiUpdaterConstructors)
            {
                var parameters = ctor.GetParameters();
                Debug.Log($"UI更新器构造函数参数: {parameters.Length}");
                foreach (var param in parameters)
                {
                    Debug.Log($"  参数类型: {param.ParameterType.Name}");
                }
            }
            
            // 创建UI构建器
            Debug.Log("尝试创建UI构建器...");
            var uiBuilder = System.Activator.CreateInstance(uiBuilderType, editorUI);
            if (uiBuilder == null)
            {
                Debug.LogError("UI构建器创建失败");
                return null;
            }
            Debug.Log($"UI构建器创建成功，类型: {uiBuilder.GetType().Name}");
            
            // 创建UI更新器
            Debug.Log("尝试创建UI更新器...");
            var uiUpdater = System.Activator.CreateInstance(uiUpdaterType, editorUI, uiBuilder);
            if (uiUpdater == null)
            {
                Debug.LogError("UI更新器创建失败");
                return null;
            }
            Debug.Log($"UI更新器创建成功，类型: {uiUpdater.GetType().Name}");
            
            // 验证接口实现
            if (uiUpdater is IUIUpdater)
            {
                Debug.Log("UI更新器正确实现了IUIUpdater接口");
            }
            else
            {
                Debug.LogError("UI更新器未实现IUIUpdater接口");
                return null;
            }
            
            Debug.Log("UI更新器已通过反射工厂创建成功");
            return (IUIUpdater)uiUpdater;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UI更新器创建失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
            return null;
        }
    }
} 