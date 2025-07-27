using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 配置预览UI组件
/// 在游戏运行时展示配置中的所有形状和球类型
/// </summary>
public class ConfigPreviewUI : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject previewPanel;
    public Transform shapePreviewContent;
    public Transform ballPreviewContent;
    public Transform backgroundPreviewContent;
    public Button closeButton;
    public Button refreshButton;
    
    [Header("滚动视图")]
    public ScrollRect shapeScrollView;
    public ScrollRect ballScrollView;
    public ScrollRect backgroundScrollView;
    
    [Header("预制体")]
    public GameObject shapePreviewItemPrefab;
    public GameObject ballPreviewItemPrefab;
    public GameObject backgroundPreviewItemPrefab;
    
    [Header("设置")]
    public float previewSize = 80f;
    public int gridColumns = 4;
    public float spacing = 10f;
    public bool autoRefresh = true;
    
    private List<GameObject> previewItems = new List<GameObject>();
    private GridLayoutGroup shapeGridLayout;
    private GridLayoutGroup ballGridLayout;
    private GridLayoutGroup backgroundGridLayout;
    
    // 防止重复刷新的标志
    private bool isRefreshing = false;
    
    void Start()
    {
        SetupEventListeners();
        SetupGridLayouts();
        
        // 确保配置已加载（只在首次启动时）
        if (LevelEditorConfig.Instance != null)
        {
            // 只在配置为空时才重新加载，避免重置levelIndex
            var config = LevelEditorConfig.Instance;
            if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
            {
                Debug.Log("ConfigPreviewUI: 配置为空，重新加载");
                config.LoadConfigFromFile();
            }
            else
            {
                Debug.Log("ConfigPreviewUI: 配置已存在，跳过重新加载");
            }
        }
        
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
        }
    }
    
    void SetupEventListeners()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePreview);
        }
        
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshPreview);
        }
    }
    
    /// <summary>
    /// 设置网格布局组件
    /// </summary>
    void SetupGridLayouts()
    {
        // 设置形状网格布局
        if (shapePreviewContent != null)
        {
            shapeGridLayout = shapePreviewContent.GetComponent<GridLayoutGroup>();
            if (shapeGridLayout == null)
            {
                shapeGridLayout = shapePreviewContent.gameObject.AddComponent<GridLayoutGroup>();
            }
            SetupGridLayout(shapeGridLayout);
        }
        
        // 设置球网格布局
        if (ballPreviewContent != null)
        {
            ballGridLayout = ballPreviewContent.GetComponent<GridLayoutGroup>();
            if (ballGridLayout == null)
            {
                ballGridLayout = ballPreviewContent.gameObject.AddComponent<GridLayoutGroup>();
            }
            SetupGridLayout(ballGridLayout);
        }
        
        // 设置背景网格布局
        if (backgroundPreviewContent != null)
        {
            backgroundGridLayout = backgroundPreviewContent.GetComponent<GridLayoutGroup>();
            if (backgroundGridLayout == null)
            {
                backgroundGridLayout = backgroundPreviewContent.gameObject.AddComponent<GridLayoutGroup>();
            }
            SetupGridLayout(backgroundGridLayout);
        }
    }
    
    /// <summary>
    /// 设置网格布局参数
    /// </summary>
    void SetupGridLayout(GridLayoutGroup gridLayout)
    {
        if (gridLayout != null)
        {
            gridLayout.cellSize = new Vector2(previewSize, previewSize);
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridColumns;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
        }
    }
    
    /// <summary>
    /// 显示预览面板
    /// </summary>
    public void ShowPreview()
    {
        try
        {
            if (previewPanel != null)
            {
                previewPanel.SetActive(true);
                
                // 延迟刷新，避免在激活过程中立即刷新导致卡死
                StartCoroutine(DelayedRefreshPreview());
            }
            else
            {
                Debug.LogError("previewPanel为空，无法显示预览");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"显示预览时发生异常: {e.Message}");
        }
    }
    
    /// <summary>
    /// 延迟刷新预览，避免卡死
    /// </summary>
    System.Collections.IEnumerator DelayedRefreshPreview()
    {
        // 等待一帧，确保UI完全激活
        yield return null;
        
        // 再次检查组件状态
        if (previewPanel != null && previewPanel.activeSelf && !isRefreshing)
        {
            RefreshPreview();
        }
        else if (isRefreshing)
        {
            Debug.LogWarning("延迟刷新时检测到正在刷新中，跳过");
        }
    }
    
    /// <summary>
    /// 关闭预览面板
    /// </summary>
    public void ClosePreview()
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 刷新预览内容
    /// </summary>
    public void RefreshPreview()
    {
        // 防止重复刷新
        if (isRefreshing)
        {
            Debug.LogWarning("正在刷新中，跳过重复刷新请求");
            return;
        }
        
        Debug.Log("开始刷新配置预览...");
        isRefreshing = true;
        
        try
        {
            // 检查预览面板是否激活
            if (previewPanel == null || !previewPanel.activeSelf)
            {
                Debug.LogWarning("预览面板未激活，跳过刷新");
                return;
            }
            
            // 检查配置实例
            if (LevelEditorConfig.Instance == null)
            {
                Debug.LogError("LevelEditorConfig.Instance 为空！");
                return;
            }
            
            // 检查UI组件是否有效
            if (shapePreviewContent == null || ballPreviewContent == null || backgroundPreviewContent == null)
            {
                Debug.LogWarning("预览UI组件为空，跳过刷新");
                return;
            }
            
            // 获取当前配置数据（不重新加载，避免无限循环）
            var config = LevelEditorConfig.Instance;
            if (config == null)
            {
                Debug.LogError("配置实例为空！");
                return;
            }
            
            Debug.Log($"配置数据 - 形状: {config.shapeTypes?.Count ?? 0}, 球: {config.ballTypes?.Count ?? 0}, 背景: {config.backgroundConfigs?.Count ?? 0}");
            
            // 限制预览项数量，避免创建过多导致卡死
            int maxItems = 100;
            if (previewItems.Count > maxItems)
            {
                Debug.LogWarning($"预览项数量过多({previewItems.Count})，限制为{maxItems}个");
                ClearPreviewItems();
            }
            
            ClearPreviewItems();
            CreateShapePreviews();
            CreateBallPreviews();
            CreateBackgroundPreviews();
            
            // 重置滚动位置
            ResetScrollPositions();
            
            Debug.Log($"刷新完成，共显示 {previewItems.Count} 个预览项");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"刷新配置预览时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
        }
        finally
        {
            // 确保刷新标志被重置
            isRefreshing = false;
        }
    }
    
    /// <summary>
    /// 重置所有滚动视图的位置
    /// </summary>
    void ResetScrollPositions()
    {
        if (shapeScrollView != null)
        {
            shapeScrollView.normalizedPosition = new Vector2(0, 1);
        }
        if (ballScrollView != null)
        {
            ballScrollView.normalizedPosition = new Vector2(0, 1);
        }
        if (backgroundScrollView != null)
        {
            backgroundScrollView.normalizedPosition = new Vector2(0, 1);
        }
    }
    
    /// <summary>
    /// 清除所有预览项
    /// </summary>
    void ClearPreviewItems()
    {
        foreach (var item in previewItems)
        {
            if (item != null)
            {
                try
                {
                    DestroyImmediate(item);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"销毁预览项时发生异常: {e.Message}");
                }
            }
        }
        previewItems.Clear();
    }
    
    /// <summary>
    /// 创建形状预览
    /// </summary>
    void CreateShapePreviews()
    {
        if (shapePreviewContent == null)
        {
            Debug.LogError("shapePreviewContent 为空！");
            return;
        }
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空！");
            return;
        }
        
        if (config.shapeTypes == null)
        {
            Debug.LogError("形状类型列表为空！");
            return;
        }
        
        Debug.Log($"创建形状预览，共 {config.shapeTypes.Count} 个形状类型");
        
        // 限制创建数量，避免卡死
        int maxShapes = 50;
        int createCount = Mathf.Min(config.shapeTypes.Count, maxShapes);
        
        for (int i = 0; i < createCount; i++)
        {
            var shapeType = config.shapeTypes[i];
            if (shapeType == null)
            {
                Debug.LogWarning($"形状类型[{i}]为空，跳过");
                continue;
            }
            
            try
            {
                GameObject previewItem = CreatePreviewItem(shapePreviewItemPrefab, shapePreviewContent);
                
                if (previewItem != null)
                {
                    SetupShapePreviewItem(previewItem, shapeType, i);
                    previewItems.Add(previewItem);
                    Debug.Log($"成功创建形状预览: {shapeType.name}");
                }
                else
                {
                    Debug.LogError($"创建形状预览项失败: {shapeType.name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建形状预览[{i}]时发生异常: {e.Message}");
            }
        }
        
        if (config.shapeTypes.Count > maxShapes)
        {
            Debug.LogWarning($"形状类型数量({config.shapeTypes.Count})超过限制({maxShapes})，只显示前{maxShapes}个");
        }
    }
    
    /// <summary>
    /// 创建球预览
    /// </summary>
    void CreateBallPreviews()
    {
        if (ballPreviewContent == null) return;
        
        var config = LevelEditorConfig.Instance;
        if (config == null || config.ballTypes == null) return;
        
        Debug.Log($"创建球预览，共 {config.ballTypes.Count} 个球类型");
        
        // 限制创建数量，避免卡死
        int maxBalls = 50;
        int createCount = Mathf.Min(config.ballTypes.Count, maxBalls);
        
        for (int i = 0; i < createCount; i++)
        {
            var ballType = config.ballTypes[i];
            if (ballType == null)
            {
                Debug.LogWarning($"球类型[{i}]为空，跳过");
                continue;
            }
            
            try
            {
                GameObject previewItem = CreatePreviewItem(ballPreviewItemPrefab, ballPreviewContent);
                
                if (previewItem != null)
                {
                    SetupBallPreviewItem(previewItem, ballType, i);
                    previewItems.Add(previewItem);
                    Debug.Log($"成功创建球预览: {ballType.name}");
                }
                else
                {
                    Debug.LogError($"创建球预览项失败: {ballType.name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建球预览[{i}]时发生异常: {e.Message}");
            }
        }
        
        if (config.ballTypes.Count > maxBalls)
        {
            Debug.LogWarning($"球类型数量({config.ballTypes.Count})超过限制({maxBalls})，只显示前{maxBalls}个");
        }
    }
    
    /// <summary>
    /// 创建背景预览
    /// </summary>
    void CreateBackgroundPreviews()
    {
        if (backgroundPreviewContent == null) return;
        
        var config = LevelEditorConfig.Instance;
        if (config == null || config.backgroundConfigs == null) return;
        
        Debug.Log($"创建背景预览，共 {config.backgroundConfigs.Count} 个背景配置");
        
        // 限制创建数量，避免卡死
        int maxBackgrounds = 50;
        int createCount = Mathf.Min(config.backgroundConfigs.Count, maxBackgrounds);
        
        for (int i = 0; i < createCount; i++)
        {
            var backgroundConfig = config.backgroundConfigs[i];
            if (backgroundConfig == null)
            {
                Debug.LogWarning($"背景配置[{i}]为空，跳过");
                continue;
            }
            
            try
            {
                GameObject previewItem = CreatePreviewItem(backgroundPreviewItemPrefab, backgroundPreviewContent);
                
                if (previewItem != null)
                {
                    SetupBackgroundPreviewItem(previewItem, backgroundConfig, i);
                    previewItems.Add(previewItem);
                    Debug.Log($"成功创建背景预览: {backgroundConfig.name}");
                }
                else
                {
                    Debug.LogError($"创建背景预览项失败: {backgroundConfig.name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建背景预览[{i}]时发生异常: {e.Message}");
            }
        }
        
        if (config.backgroundConfigs.Count > maxBackgrounds)
        {
            Debug.LogWarning($"背景配置数量({config.backgroundConfigs.Count})超过限制({maxBackgrounds})，只显示前{maxBackgrounds}个");
        }
    }
    
    /// <summary>
    /// 创建预览项
    /// </summary>
    GameObject CreatePreviewItem(GameObject prefab, Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError("父对象为空，无法创建预览项");
            return null;
        }
        
        if (prefab != null)
        {
            GameObject item = Instantiate(prefab, parent);
            Debug.Log($"使用预制体创建预览项: {prefab.name}");
            return item;
        }
        else
        {
            // 创建默认预览项
            GameObject item = new GameObject("PreviewItem");
            item.transform.SetParent(parent);
            
            // 添加Image组件
            Image image = item.AddComponent<Image>();
            image.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
            
            // 设置RectTransform
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(previewSize, previewSize);
            }
            
            Debug.Log("创建默认预览项");
            return item;
        }
    }
    
    /// <summary>
    /// 设置形状预览项
    /// </summary>
    void SetupShapePreviewItem(GameObject item, ShapeType shapeType, int index)
    {
        Image image = item.GetComponent<Image>();
        if (image == null) return;
        
        if (shapeType.sprite != null)
        {
            image.sprite = shapeType.sprite;
            image.color = Color.white;
        }
        else
        {
            // 设置默认颜色
            Color[] defaultColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta };
            image.color = defaultColors[index % defaultColors.Length];
        }
        
        // 添加文本标签
        AddTextLabel(item, shapeType.name);
        
        // 添加边框
        AddBorder(item, Color.black, 2f);
    }
    
    /// <summary>
    /// 设置球预览项
    /// </summary>
    void SetupBallPreviewItem(GameObject item, BallType ballType, int index)
    {
        Image image = item.GetComponent<Image>();
        if (image == null) return;
        
        if (ballType.sprite != null)
        {
            image.sprite = ballType.sprite;
            image.color = Color.white;
        }
        else
        {
            image.color = ballType.color;
        }
        
        // 添加文本标签
        AddTextLabel(item, ballType.name);
        
        // 添加边框
        AddBorder(item, Color.black, 2f);
    }
    
    /// <summary>
    /// 设置背景预览项
    /// </summary>
    void SetupBackgroundPreviewItem(GameObject item, BackgroundConfig backgroundConfig, int index)
    {
        Image image = item.GetComponent<Image>();
        if (image == null) return;
        
        if (backgroundConfig.useSprite && backgroundConfig.backgroundSprite != null)
        {
            image.sprite = backgroundConfig.backgroundSprite;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = backgroundConfig.backgroundColor;
        }
        
        // 添加文本标签
        AddTextLabel(item, backgroundConfig.name);
        
        // 添加边框
        AddBorder(item, Color.black, 2f);
    }
    
    /// <summary>
    /// 添加文本标签
    /// </summary>
    void AddTextLabel(GameObject item, string text)
    {
        // 创建文本对象
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(item.transform);
        
        // 添加Text组件
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 12;
        textComponent.color = Color.black;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        // 设置RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 0.2f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
    }
    
    /// <summary>
    /// 添加边框
    /// </summary>
    void AddBorder(GameObject item, Color borderColor, float borderWidth)
    {
        // 创建边框对象
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(item.transform);
        
        // 添加Image组件作为边框
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;
        borderImage.sprite = null;
        
        // 设置RectTransform
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        if (borderRect != null)
        {
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(-borderWidth, -borderWidth);
            borderRect.offsetMax = new Vector2(borderWidth, borderWidth);
        }
        
        // 将边框移到最底层
        borderObj.transform.SetAsFirstSibling();
    }
    
    /// <summary>
    /// 切换预览面板显示状态
    /// </summary>
    public void TogglePreview()
    {
        if (previewPanel != null)
        {
            bool isActive = previewPanel.activeSelf;
            if (isActive)
            {
                ClosePreview();
            }
            else
            {
                ShowPreview();
            }
        }
    }
    
    /// <summary>
    /// 自动刷新（如果启用）
    /// </summary>
    void Update()
    {
        if (autoRefresh && previewPanel != null && previewPanel.activeSelf)
        {
            // 可以在这里添加自动刷新逻辑
            // 比如检测配置文件变化等
        }
    }
} 