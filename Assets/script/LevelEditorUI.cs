using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 关卡编辑器UI控制器
/// 管理关卡编辑器的UI界面和用户交互
/// </summary>
public class LevelEditorUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject leftPanel;
    public GameObject centerPanel;
    public GameObject rightPanel;
    
    [Header("Left Panel - Level/Layer List")]
    public Transform levelListContent;
    public Button addLayerButton;
    public Button deleteLayerButton;
    
    [Header("Center Panel - Edit Area")]
    public Transform editAreaContent;
    public Image editAreaBackground; // 新增：编辑区背景
    public Button addShapeButton;
    public Button addBallButton;
    public Button backgroundButton; // 新增：背景切换按钮
    public Button previewButton; // 新增：预览按钮
    
    [Header("Right Panel - Properties")]
    public InputField nameInput;
    public Slider positionXSlider;
    public Slider positionYSlider;
    public Slider rotationSlider;
    public Button[] shapeTypeButtons; // 替换Dropdown为按钮数组
    public Button[] ballTypeButtons; // 新增：球类型按钮数组
    public Button exportButton;
    
    [Header("Prefabs")]
    public GameObject shapePrefab;
    public GameObject ballPrefab;
    
    [Header("Preview")]
    public ConfigPreviewUI configPreviewUI; // 新增：配置预览UI组件
    
    // 数据
    public LevelData currentLevel;
    public LayerData currentLayer;
    public ShapeController selectedShape;
    public BallController selectedBall;
    public int currentShapeTypeIndex = 0; // 0:圆形, 1:矩形, 2:三角形, 3:菱形
    public int currentBallTypeIndex = 0; // 新增：当前球类型索引
    public int currentBackgroundIndex = 0; // 新增：当前背景索引
    
    // UI管理器
    private LevelEditorUIManager uiManager;
    private LevelEditorDataManager dataManager;
    
    #if UNITY_EDITOR
    private IUIUpdater uiUpdater; // 使用接口类型
    #endif
    
    void Awake()
    {
        InitializeManagers();
        SetupEventListeners();
        InitializeDefaultData();
    }
    
    void InitializeManagers()
    {
        uiManager = new LevelEditorUIManager(this);
        dataManager = new LevelEditorDataManager(this);
        
        #if UNITY_EDITOR
        // 初始化UI更新器（在UI构建完成后）
        StartCoroutine(InitializeUIUpdater());
        #endif
    }
    
    #if UNITY_EDITOR
    System.Collections.IEnumerator InitializeUIUpdater()
    {
        // 等待一帧，确保UI构建完成
        yield return null;
        
        // 尝试使用Editor创建器
        try
        {
            var creatorType = System.Type.GetType("UIUpdaterCreator, Assembly-CSharp-Editor");
            if (creatorType != null)
            {
                var createMethod = creatorType.GetMethod("CreateUIUpdater", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (createMethod != null)
                {
                    uiUpdater = (IUIUpdater)createMethod.Invoke(null, new object[] { this });
                    Debug.Log("UI更新器已通过Editor创建器初始化");
                }
                else
                {
                    Debug.LogError("未找到CreateUIUpdater方法");
                }
            }
            else
            {
                Debug.LogWarning("未找到UIUpdaterCreator类型，尝试使用反射工厂");
                // 回退到反射工厂
                uiUpdater = UIUpdaterFactory.CreateUIUpdater(this);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Editor创建器失败: {e.Message}");
            Debug.LogWarning("回退到反射工厂");
            // 回退到反射工厂
            uiUpdater = UIUpdaterFactory.CreateUIUpdater(this);
        }
        
        if (uiUpdater != null)
        {
            Debug.Log("UI更新器初始化成功");
        }
        else
        {
            Debug.LogWarning("UI更新器初始化失败，实时更新功能将不可用");
        }
    }
    #endif
    
    void SetupEventListeners()
    {
        if (addLayerButton) addLayerButton.onClick.AddListener(AddLayer);
        if (deleteLayerButton) deleteLayerButton.onClick.AddListener(DeleteLayer);
        if (addShapeButton) addShapeButton.onClick.AddListener(AddShape);
        if (addBallButton) addBallButton.onClick.AddListener(AddBall);
        if (backgroundButton) backgroundButton.onClick.AddListener(SwitchBackground);
        if (previewButton) previewButton.onClick.AddListener(ShowConfigPreview);
        if (exportButton) exportButton.onClick.AddListener(ExportLevel);
        
        SetupPropertyListeners();
    }
    
    void SetupPropertyListeners()
    {
        if (positionXSlider) positionXSlider.onValueChanged.AddListener(OnPositionXChanged);
        if (positionYSlider) positionYSlider.onValueChanged.AddListener(OnPositionYChanged);
        if (rotationSlider) rotationSlider.onValueChanged.AddListener(OnRotationChanged);
        // 形状类型按钮的事件在LevelEditorUIBuilder中设置
    }
    
    void InitializeDefaultData()
    {
        // 加载配置
        LoadConfiguration();
        
        // 如果没有初始化数据，创建默认数据
        if (currentLevel == null)
        {
            currentLevel = new LevelData("新关卡");
        }
        
        // 确保至少有一个层级
        if (currentLevel.layers.Count == 0)
        {
            currentLayer = new LayerData("默认层级");
            currentLevel.layers.Add(currentLayer);
        }
        else if (currentLayer == null)
        {
            // 如果层级列表不为空但当前层级为null，选择第一个层级
            currentLayer = currentLevel.layers[0];
        }
    }
    
    /// <summary>
    /// 加载配置
    /// </summary>
    void LoadConfiguration()
    {
        try
        {
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                // 尝试从文件加载配置
                config.LoadConfigFromFile();
                Debug.Log("配置已加载");
                
                // 如果配置为空，初始化默认配置
                if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
                {
                    Debug.Log("配置为空，初始化默认配置");
                    config.InitializeDefaultConfig();
                }
                
                Debug.Log($"配置加载完成 - 形状: {config.shapeTypes.Count}, 球: {config.ballTypes.Count}, 背景: {config.backgroundConfigs.Count}");
            }
            else
            {
                Debug.LogError("无法获取LevelEditorConfig实例");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"配置加载失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
        }
    }
    
    public void RefreshUI()
    {
        if (uiManager != null)
        {
            uiManager.RefreshUI();
        }
    }
    
    public void AddLayer()
    {
        if (dataManager != null) dataManager.AddLayer();
    }
    
    public void DeleteLayer()
    {
        if (dataManager != null) dataManager.DeleteLayer();
    }
    
    public void AddShape()
    {
        if (dataManager != null) dataManager.AddShape();
    }
    
    public void AddBall()
    {
        if (dataManager != null) dataManager.AddBall();
    }
    
    public void ExportLevel()
    {
        if (dataManager != null) dataManager.ExportLevel();
    }

    public void ImportLevel()
    {
        if (dataManager != null) dataManager.ImportLevel();
    }
    
    public void SelectShape(ShapeController shape)
    {
        if (uiManager != null) uiManager.SelectShape(shape);
    }
    
    public void SelectBall(BallController ball)
    {
        if (uiManager != null) uiManager.SelectBall(ball);
    }
    
    void OnPositionXChanged(float value)
    {
        if (uiManager != null) uiManager.UpdatePositionX(value);
    }
    
    void OnPositionYChanged(float value)
    {
        if (uiManager != null) uiManager.UpdatePositionY(value);
    }
    
    void OnRotationChanged(float value)
    {
        if (uiManager != null) uiManager.UpdateRotation(value);
    }
    
    /// <summary>
    /// 更新形状类型（通过按钮索引）
    /// </summary>
    public void UpdateShapeType(int index)
    {
        currentShapeTypeIndex = index;
        if (uiManager != null) uiManager.UpdateType(index);
    }

    public void UpdateBallType(int index)
    {
        if (uiManager != null) uiManager.UpdateBallType(index);
    }
    
    /// <summary>
    /// 设置当前关卡的文件路径（用于覆盖保存）
    /// </summary>
    public void SetCurrentLevelFilePath(string filePath)
    {
        if (dataManager != null)
        {
            var field = typeof(LevelEditorDataManager).GetField("currentLevelFilePath", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(dataManager, filePath);
            }
        }
    }
    
    /// <summary>
    /// 显示配置预览
    /// </summary>
    public void ShowConfigPreview()
    {
        if (configPreviewUI != null)
        {
            configPreviewUI.ShowPreview();
        }
        else
        {
            Debug.LogWarning("ConfigPreviewUI组件未设置");
        }
    }
    
    /// <summary>
    /// 清空编辑区、层级列表和选中状态
    /// </summary>
    public void ClearAllUIAndSelection()
    {
        // 清除编辑区所有对象
        if (editAreaContent)
        {
            foreach (Transform child in editAreaContent)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        // 清除层级列表
        if (levelListContent)
        {
            foreach (Transform child in levelListContent)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        // 清除选中
        selectedShape = null;
        selectedBall = null;
    }
    
    /// <summary>
    /// 切换背景
    /// </summary>
    public void SwitchBackground()
    {
        var config = LevelEditorConfig.Instance;
        currentBackgroundIndex = (currentBackgroundIndex + 1) % config.backgroundConfigs.Count;
        config.SetCurrentBackground(currentBackgroundIndex);
        ApplyBackground();
        Debug.Log($"切换到背景: {currentBackgroundIndex}");
    }
    
    /// <summary>
    /// 应用背景配置
    /// </summary>
    public void ApplyBackground()
    {
        if (editAreaBackground == null) return;
        
        var config = LevelEditorConfig.Instance;
        var backgroundConfig = config.GetCurrentBackground();
        
        if (backgroundConfig != null)
        {
            if (backgroundConfig.useSprite && backgroundConfig.backgroundSprite != null)
            {
                editAreaBackground.sprite = backgroundConfig.backgroundSprite;
                editAreaBackground.color = Color.white;
                
                // 设置精灵的缩放和偏移
                editAreaBackground.rectTransform.sizeDelta = backgroundConfig.backgroundSprite.rect.size * backgroundConfig.spriteScale;
                editAreaBackground.rectTransform.anchoredPosition = backgroundConfig.spriteOffset;
            }
            else
            {
                editAreaBackground.sprite = null;
                editAreaBackground.color = backgroundConfig.backgroundColor;
            }
        }
    }
    
    void OnDestroy()
    {
        #if UNITY_EDITOR
        // 清理UI更新器
        if (uiUpdater != null)
        {
            uiUpdater.UnsubscribeFromConfigEvents();
            Debug.Log("UI更新器已清理");
        }
        #endif
    }
} 