using UnityEngine;

/// <summary>
/// UI更新器创建器（Editor版本）
/// 在Editor中直接创建UI更新器，避免反射
/// </summary>
public static class UIUpdaterCreator
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
            Debug.Log("开始创建UI更新器（Editor版本）...");
            
            // 直接创建UI构建器和UI更新器
            var uiBuilder = new LevelEditorUIBuilder(editorUI);
            Debug.Log($"UI构建器创建成功，类型: {uiBuilder.GetType().Name}");
            
            var uiUpdater = new LevelEditorUIUpdater(editorUI, uiBuilder);
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
            
            Debug.Log("UI更新器已通过Editor创建器创建成功");
            return uiUpdater;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UI更新器创建失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
            return null;
        }
    }
} 