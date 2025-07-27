using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// 关卡编辑器UI更新器
/// 负责处理配置变更时的实时UI更新
/// </summary>
public class LevelEditorUIUpdater : IUIUpdater
{
    private LevelEditorUI editorUI;
    private LevelEditorUIBuilder uiBuilder;
    
    public LevelEditorUIUpdater(LevelEditorUI editor, LevelEditorUIBuilder builder)
    {
        editorUI = editor;
        uiBuilder = builder;
        SubscribeToConfigEvents();
    }
    
    /// <summary>
    /// 订阅配置变更事件
    /// </summary>
    void SubscribeToConfigEvents()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.OnShapeTypesChanged += OnShapeTypesChanged;
            config.OnBallTypesChanged += OnBallTypesChanged;
            config.OnBackgroundConfigsChanged += OnBackgroundConfigsChanged;
            config.OnCurrentBackgroundChanged += OnCurrentBackgroundChanged;
            config.OnConfigReloaded += OnConfigReloaded;
            
            Debug.Log("已订阅配置变更事件");
        }
        else
        {
            Debug.LogWarning("配置实例为空，无法订阅事件");
        }
    }
    
    /// <summary>
    /// 取消订阅配置变更事件
    /// </summary>
    public void UnsubscribeFromConfigEvents()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.OnShapeTypesChanged -= OnShapeTypesChanged;
            config.OnBallTypesChanged -= OnBallTypesChanged;
            config.OnBackgroundConfigsChanged -= OnBackgroundConfigsChanged;
            config.OnCurrentBackgroundChanged -= OnCurrentBackgroundChanged;
            config.OnConfigReloaded -= OnConfigReloaded;
            
            Debug.Log("已取消订阅配置变更事件");
        }
    }
    
    /// <summary>
    /// 形状类型变更处理
    /// </summary>
    void OnShapeTypesChanged()
    {
        Debug.Log("形状类型已变更，更新UI...");
        
        // 更新形状类型按钮
        UpdateShapeTypeButtons();
        
        // 更新当前选中的形状类型索引
        UpdateCurrentShapeTypeIndex();
        
        // 刷新预览UI
        RefreshPreviewUI();
    }
    
    /// <summary>
    /// 球类型变更处理
    /// </summary>
    void OnBallTypesChanged()
    {
        Debug.Log("球类型已变更，更新UI...");
        
        // 更新球类型按钮
        UpdateBallTypeButtons();
        
        // 更新当前选中的球类型索引
        UpdateCurrentBallTypeIndex();
        
        // 刷新预览UI
        RefreshPreviewUI();
    }
    
    /// <summary>
    /// 背景配置变更处理
    /// </summary>
    void OnBackgroundConfigsChanged()
    {
        Debug.Log("背景配置已变更，更新UI...");
        
        // 更新背景
        if (editorUI != null)
        {
            editorUI.ApplyBackground();
        }
        
        // 刷新预览UI
        RefreshPreviewUI();
    }
    
    /// <summary>
    /// 当前背景变更处理
    /// </summary>
    void OnCurrentBackgroundChanged()
    {
        Debug.Log("当前背景已变更，更新UI...");
        
        // 更新背景
        if (editorUI != null)
        {
            editorUI.ApplyBackground();
        }
    }
    
    /// <summary>
    /// 配置重新加载处理
    /// </summary>
    void OnConfigReloaded()
    {
        Debug.Log("配置已重新加载，更新所有UI...");
        
        // 检查editorUI是否有效
        if (editorUI == null)
        {
            Debug.LogWarning("editorUI为空，跳过UI更新");
            return;
        }
        
        // 更新所有UI组件
        UpdateShapeTypeButtons();
        UpdateBallTypeButtons();
        UpdateCurrentShapeTypeIndex();
        UpdateCurrentBallTypeIndex();
        
        // 更新背景
        if (editorUI != null)
        {
            editorUI.ApplyBackground();
        }
        
        // 刷新预览UI
        RefreshPreviewUI();
        
        Debug.Log("所有UI已更新完成");
    }
    
    /// <summary>
    /// 更新形状类型按钮
    /// </summary>
    void UpdateShapeTypeButtons()
    {
        if (editorUI?.rightPanel == null)
        {
            Debug.LogWarning("无法更新形状类型按钮：editorUI或rightPanel为空");
            return;
        }
        
        // 清除现有的形状类型按钮
        ClearShapeTypeButtons();
        
        // 重新创建形状类型按钮
        if (uiBuilder != null)
        {
            uiBuilder.CreateShapeTypeButtons(editorUI.rightPanel, new Vector2(0, 0.6f));
            Debug.Log("形状类型按钮已重新创建");
        }
        else
        {
            Debug.LogWarning("uiBuilder为空，无法重新创建形状类型按钮");
        }
    }
    
    /// <summary>
    /// 更新球类型按钮
    /// </summary>
    void UpdateBallTypeButtons()
    {
        if (editorUI?.rightPanel == null)
        {
            Debug.LogWarning("无法更新球类型按钮：editorUI或rightPanel为空");
            return;
        }
        
        // 清除现有的球类型按钮
        ClearBallTypeButtons();
        
        // 重新创建球类型按钮
        if (uiBuilder != null)
        {
            uiBuilder.CreateBallTypeButtons(editorUI.rightPanel, new Vector2(0, 0.4f));
            Debug.Log("球类型按钮已重新创建");
        }
        else
        {
            Debug.LogWarning("uiBuilder为空，无法重新创建球类型按钮");
        }
    }
    
    /// <summary>
    /// 清除形状类型按钮
    /// </summary>
    void ClearShapeTypeButtons()
    {
        if (editorUI?.rightPanel == null) return;
        
        // 查找并销毁形状类型相关的UI元素
        Transform[] children = editorUI.rightPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            // 添加null检查，避免访问已销毁的对象
            if (child != null && (child.name == "TypeLabel" || child.name.Contains("ShapeType")))
            {
                if (child.gameObject != null)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
        
        editorUI.shapeTypeButtons = null;
    }
    
    /// <summary>
    /// 清除球类型按钮
    /// </summary>
    void ClearBallTypeButtons()
    {
        if (editorUI?.rightPanel == null) return;
        
        // 查找并销毁球类型相关的UI元素
        Transform[] children = editorUI.rightPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            // 添加null检查，避免访问已销毁的对象
            if (child != null && (child.name == "BallTypeLabel" || child.name.Contains("BallType")))
            {
                if (child.gameObject != null)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
        
        editorUI.ballTypeButtons = null;
    }
    
    /// <summary>
    /// 更新当前形状类型索引
    /// </summary>
    void UpdateCurrentShapeTypeIndex()
    {
        if (editorUI == null) return;
        
        string[] shapeTypes = LevelEditorConfig.Instance.GetShapeTypeNames();
        if (shapeTypes.Length > 0)
        {
            // 如果当前索引超出范围，重置为0
            if (editorUI.currentShapeTypeIndex >= shapeTypes.Length)
            {
                editorUI.currentShapeTypeIndex = 0;
                Debug.Log($"当前形状类型索引超出范围，重置为0");
            }
        }
        else
        {
            editorUI.currentShapeTypeIndex = -1;
            Debug.Log("没有可用的形状类型");
        }
    }
    
    /// <summary>
    /// 更新当前球类型索引
    /// </summary>
    void UpdateCurrentBallTypeIndex()
    {
        if (editorUI == null) return;
        
        string[] ballTypes = LevelEditorConfig.Instance.GetBallTypeNames();
        if (ballTypes.Length > 0)
        {
            // 如果当前索引超出范围，重置为0
            if (editorUI.currentBallTypeIndex >= ballTypes.Length)
            {
                editorUI.currentBallTypeIndex = 0;
                Debug.Log($"当前球类型索引超出范围，重置为0");
            }
        }
        else
        {
            editorUI.currentBallTypeIndex = -1;
            Debug.Log("没有可用的球类型");
        }
    }
    
    /// <summary>
    /// 刷新预览UI
    /// </summary>
    void RefreshPreviewUI()
    {
        // 查找并刷新预览UI
        ConfigPreviewUI previewUI = Object.FindObjectOfType<ConfigPreviewUI>();
        if (previewUI != null)
        {
            previewUI.RefreshPreview();
            Debug.Log("预览UI已刷新");
        }
    }
} 