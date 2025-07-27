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
    public Button createLevelButton; // 新增：创建关卡按钮
    public InputField levelNameInput; // 新增：关卡名称输入框
    
    [Header("Center Panel - Edit Area")]
    public Transform editAreaContent;
    public Image editAreaBackground; // 新增：编辑区背景
    public Button addShapeButton;
    public Button addBallButton;
    public Button deleteShapeButton; // 新增：删除形状按钮
    public Button deleteBallButton; // 新增：删除球按钮
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
    public Button importButton; // 新增：导入按钮
    
    [Header("Fixed Positions")]
    public Button addFixedPositionButton; // 新增：添加固定位置按钮
    public Button clearFixedPositionsButton; // 新增：清除固定位置按钮
    public Button showFixedPositionsButton; // 新增：显示固定位置按钮
    
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
        InitializeDefaultData();
    }
    
    void Start()
    {
        // 在运行时初始化关卡名称
        if (Application.isPlaying)
        {
            InitializeDefaultData();
        }
        
        // 在运行时创建类型按钮
        CreateTypeButtons();
        
        // 在运行时自动绑定事件
        SetupEventListeners();
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
    
    /// <summary>
    /// 设置所有事件监听器（只在运行时调用）
    /// </summary>
    public void SetupEventListeners()
    {
        // 只在运行时调用
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Debug.LogWarning("SetupEventListeners只能在运行时调用，编辑器模式下跳过");
            return;
        }
        #endif
        
        Debug.Log("开始设置事件监听器...");
        
        // 检查UI组件是否已创建
        if (!CheckUIComponents())
        {
            Debug.LogWarning("UI组件未完全创建，跳过事件绑定");
            return;
        }
        
        // 左侧面板按钮事件
        SetupLeftPanelEvents();
        
        // 工具栏按钮事件
        SetupToolbarEvents();
        
        // 属性面板事件
        SetupPropertyEvents();
        
        // 形状类型和球类型按钮事件
        SetupTypeButtonEvents();
        
        Debug.Log("事件监听器设置完成");
    }
    
    /// <summary>
    /// 检查UI组件是否已创建
    /// </summary>
    bool CheckUIComponents()
    {
        bool allComponentsExist = true;
        
        // 检查主要按钮
        if (addLayerButton == null)
        {
            Debug.LogWarning("添加层级按钮未创建");
            allComponentsExist = false;
        }
        
        if (deleteLayerButton == null)
        {
            Debug.LogWarning("删除层级按钮未创建");
            allComponentsExist = false;
        }
        
        if (createLevelButton == null)
        {
            Debug.LogWarning("创建关卡按钮未创建");
            allComponentsExist = false;
        }
        
        if (levelNameInput == null)
        {
            Debug.LogWarning("关卡名称输入框未创建");
            allComponentsExist = false;
        }
        
        if (addShapeButton == null)
        {
            Debug.LogWarning("添加形状按钮未创建");
            allComponentsExist = false;
        }
        
        if (addBallButton == null)
        {
            Debug.LogWarning("添加球按钮未创建");
            allComponentsExist = false;
        }
        
        if (deleteShapeButton == null)
        {
            Debug.LogWarning("删除形状按钮未创建");
            allComponentsExist = false;
        }
        
        if (deleteBallButton == null)
        {
            Debug.LogWarning("删除球按钮未创建");
            allComponentsExist = false;
        }
        
        if (exportButton == null)
        {
            Debug.LogWarning("导出按钮未创建");
            allComponentsExist = false;
        }
        
        if (importButton == null)
        {
            Debug.LogWarning("导入按钮未创建");
            allComponentsExist = false;
        }
        
        // 检查固定位置按钮
        if (addFixedPositionButton == null)
        {
            Debug.LogWarning("添加固定位置按钮未创建");
            allComponentsExist = false;
        }
        
        if (clearFixedPositionsButton == null)
        {
            Debug.LogWarning("清除固定位置按钮未创建");
            allComponentsExist = false;
        }
        
        if (showFixedPositionsButton == null)
        {
            Debug.LogWarning("显示固定位置按钮未创建");
            allComponentsExist = false;
        }
        
        // 检查面板
        if (leftPanel == null)
        {
            Debug.LogWarning("左侧面板未创建");
            allComponentsExist = false;
        }
        
        if (centerPanel == null)
        {
            Debug.LogWarning("中央面板未创建");
            allComponentsExist = false;
        }
        
        if (rightPanel == null)
        {
            Debug.LogWarning("右侧面板未创建");
            allComponentsExist = false;
        }
        
        if (allComponentsExist)
        {
            Debug.Log("✓ 所有UI组件已创建");
        }
        else
        {
            Debug.LogError("✗ 部分UI组件未创建，无法绑定事件");
        }
        
        return allComponentsExist;
    }
    
    /// <summary>
    /// 延迟设置事件监听器（当UI组件可能未完全创建时使用）
    /// </summary>
    public void SetupEventListenersDelayed()
    {
        #if UNITY_EDITOR
        StartCoroutine(SetupEventListenersCoroutine());
        #endif
    }
    
    #if UNITY_EDITOR
    System.Collections.IEnumerator SetupEventListenersCoroutine()
    {
        Debug.Log("等待UI组件创建完成...");
        
        // 等待最多10帧，确保UI组件创建完成
        int maxFrames = 10;
        int currentFrame = 0;
        
        while (currentFrame < maxFrames)
        {
            yield return null;
            currentFrame++;
            
            if (CheckUIComponents())
            {
                Debug.Log($"UI组件在第{currentFrame}帧创建完成");
                SetupEventListeners();
                yield break;
            }
        }
        
        Debug.LogError("等待超时，UI组件仍未完全创建");
    }
    #endif
    
    /// <summary>
    /// 设置左侧面板事件
    /// </summary>
    void SetupLeftPanelEvents()
    {
        if (addLayerButton != null)
        {
            addLayerButton.onClick.AddListener(OnAddLayerClicked);
            Debug.Log("添加层级按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("添加层级按钮为空");
        }
        
        if (deleteLayerButton != null)
        {
            deleteLayerButton.onClick.AddListener(OnDeleteLayerClicked);
            Debug.Log("删除层级按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("删除层级按钮为空");
        }
        
        if (createLevelButton != null)
        {
            createLevelButton.onClick.AddListener(OnCreateLevelClicked);
            Debug.Log("创建关卡按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("创建关卡按钮为空");
        }
        
        if (levelNameInput != null)
        {
            levelNameInput.onValueChanged.AddListener(OnLevelNameChanged);
            Debug.Log("关卡名称输入框事件绑定成功");
        }
        else
        {
            Debug.LogError("关卡名称输入框为空");
        }
    }
    
    /// <summary>
    /// 设置工具栏事件
    /// </summary>
    void SetupToolbarEvents()
    {
        if (addShapeButton != null)
        {
            addShapeButton.onClick.AddListener(OnAddShapeClicked);
            Debug.Log("添加形状按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("添加形状按钮为空");
        }
        
        if (addBallButton != null)
        {
            addBallButton.onClick.AddListener(OnAddBallClicked);
            Debug.Log("添加球按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("添加球按钮为空");
        }
        
        if (deleteShapeButton != null)
        {
            deleteShapeButton.onClick.AddListener(OnDeleteShapeClicked);
            Debug.Log("删除形状按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("删除形状按钮为空");
        }
        
        if (deleteBallButton != null)
        {
            deleteBallButton.onClick.AddListener(OnDeleteBallClicked);
            Debug.Log("删除球按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("删除球按钮为空");
        }
        
        if (backgroundButton != null)
        {
            backgroundButton.onClick.AddListener(OnBackgroundClicked);
            Debug.Log("背景按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("背景按钮为空");
        }
        
        if (previewButton != null)
        {
            previewButton.onClick.AddListener(OnPreviewClicked);
            Debug.Log("预览按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("预览按钮为空");
        }
    }
    
    /// <summary>
    /// 设置属性面板事件
    /// </summary>
    void SetupPropertyEvents()
    {
        if (positionXSlider != null)
        {
            positionXSlider.onValueChanged.AddListener(OnPositionXChanged);
            Debug.Log("位置X滑块事件绑定成功");
        }
        
        if (positionYSlider != null)
        {
            positionYSlider.onValueChanged.AddListener(OnPositionYChanged);
            Debug.Log("位置Y滑块事件绑定成功");
        }
        
        if (rotationSlider != null)
        {
            rotationSlider.onValueChanged.AddListener(OnRotationChanged);
            Debug.Log("旋转滑块事件绑定成功");
        }
        
        if (exportButton != null)
        {
            exportButton.onClick.AddListener(OnExportClicked);
            Debug.Log("导出按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("导出按钮为空");
        }
        
        if (importButton != null)
        {
            importButton.onClick.AddListener(OnImportClicked);
            Debug.Log("导入按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("导入按钮为空");
        }
        
        // 固定位置按钮事件
        if (addFixedPositionButton != null)
        {
            addFixedPositionButton.onClick.AddListener(OnAddFixedPositionClicked);
            Debug.Log("添加固定位置按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("添加固定位置按钮为空");
        }
        
        if (clearFixedPositionsButton != null)
        {
            clearFixedPositionsButton.onClick.AddListener(OnClearFixedPositionsClicked);
            Debug.Log("清除固定位置按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("清除固定位置按钮为空");
        }
        
        if (showFixedPositionsButton != null)
        {
            showFixedPositionsButton.onClick.AddListener(OnShowFixedPositionsClicked);
            Debug.Log("显示固定位置按钮事件绑定成功");
        }
        else
        {
            Debug.LogError("显示固定位置按钮为空");
        }
    }
    
    /// <summary>
    /// 设置类型按钮事件
    /// </summary>
    void SetupTypeButtonEvents()
    {
        // 形状类型按钮事件
        if (shapeTypeButtons != null)
        {
            for (int i = 0; i < shapeTypeButtons.Length; i++)
            {
                int index = i; // 捕获循环变量
                if (shapeTypeButtons[i] != null)
                {
                    shapeTypeButtons[i].onClick.AddListener(() => OnShapeTypeButtonClicked(index));
                    Debug.Log($"形状类型按钮[{index}]事件绑定成功");
                }
            }
        }
        else
        {
            Debug.LogWarning("形状类型按钮数组为空，跳过事件绑定");
        }
        
        // 球类型按钮事件
        if (ballTypeButtons != null)
        {
            for (int i = 0; i < ballTypeButtons.Length; i++)
            {
                int index = i; // 捕获循环变量
                if (ballTypeButtons[i] != null)
                {
                    ballTypeButtons[i].onClick.AddListener(() => OnBallTypeButtonClicked(index));
                    Debug.Log($"球类型按钮[{index}]事件绑定成功");
                }
            }
        }
        else
        {
            Debug.LogWarning("球类型按钮数组为空，跳过事件绑定");
        }
    }
    
    // 事件处理方法
    void OnAddLayerClicked()
    {
        Debug.Log("添加层级按钮被点击！");
        AddLayer();
    }
    
    void OnDeleteLayerClicked()
    {
        Debug.Log("删除层级按钮被点击！");
        DeleteLayer();
    }
    
    void OnCreateLevelClicked()
    {
        Debug.Log("创建关卡按钮被点击！");
        CreateLevel();
    }
    
    void OnLevelNameChanged(string newName)
    {
        Debug.Log($"关卡名称已修改为: {newName}");
        UpdateLevelName(newName);
    }
    
    void OnAddShapeClicked()
    {
        Debug.Log("添加形状按钮被点击！");
        
        // 添加安全检查
        if (dataManager == null)
        {
            Debug.LogError("dataManager为空，无法添加形状");
            return;
        }
        
        try
        {
            AddShape();
            Debug.Log("添加形状操作完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"添加形状时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
        }
    }
    
    void OnAddBallClicked()
    {
        Debug.Log("添加球按钮被点击！");
        
        // 添加安全检查
        if (dataManager == null)
        {
            Debug.LogError("dataManager为空，无法添加球");
            return;
        }
        
        try
        {
            AddBall();
            Debug.Log("添加球操作完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"添加球时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
        }
    }
    
    void OnDeleteShapeClicked()
    {
        Debug.Log("删除形状按钮被点击！");
        
        if (selectedShape == null)
        {
            Debug.LogWarning("没有选中的形状，无法删除");
            return;
        }
        
        if (dataManager == null)
        {
            Debug.LogError("dataManager为空，无法删除形状");
            return;
        }
        
        try
        {
            DeleteShape();
            Debug.Log("删除形状操作完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"删除形状时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
        }
    }
    
    void OnDeleteBallClicked()
    {
        Debug.Log("删除球按钮被点击！");
        
        if (selectedBall == null)
        {
            Debug.LogWarning("没有选中的球，无法删除");
            return;
        }
        
        if (dataManager == null)
        {
            Debug.LogError("dataManager为空，无法删除球");
            return;
        }
        
        try
        {
            DeleteBall();
            Debug.Log("删除球操作完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"删除球时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
        }
    }
    
    void OnBackgroundClicked()
    {
        Debug.Log("背景按钮被点击！");
        SwitchBackground();
    }
    
    void OnPreviewClicked()
    {
        Debug.Log("预览按钮被点击！");
        ShowConfigPreview();
    }
    
    void OnImportClicked()
    {
        Debug.Log("导入关卡按钮被点击！");
        ImportLevel();
    }
    
    void OnExportClicked()
    {
        Debug.Log("导出按钮被点击！");
        ExportLevel();
    }
    
    void OnAddFixedPositionClicked()
    {
        Debug.Log("添加固定位置按钮被点击！");
        
        // 检查是否选中了形状
        if (selectedShape == null)
        {
            Debug.LogWarning("请先选中一个形状");
            return;
        }
        
        // 弹出坐标输入测试窗口
        #if UNITY_EDITOR
        // 直接调用固定位置窗口
        try
        {
            var assembly = System.Reflection.Assembly.Load("Assembly-CSharp-Editor");
            var windowType = assembly.GetType("CoordinateInputTestWindow");
            if (windowType != null)
            {
                var openWindowMethod = windowType.GetMethod("OpenWindow", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (openWindowMethod != null)
                {
                    openWindowMethod.Invoke(null, null);
                    Debug.Log("已打开固定位置编辑器窗口");
                }
                else
                {
                    Debug.LogError("未找到OpenWindow方法");
                }
            }
            else
            {
                Debug.LogError("未找到CoordinateInputTestWindow类型");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"打开固定位置编辑器窗口失败: {e.Message}");
        }
        #else
        // 运行时直接使用形状当前位置添加固定位置
        Vector2 currentPosition = selectedShape.ShapeData.position;
        AddFixedPosition(currentPosition);
        Debug.Log($"运行时模式：已使用形状当前位置添加固定位置: {currentPosition}");
        #endif
    }
    
    void OnClearFixedPositionsClicked()
    {
        Debug.Log("清除固定位置按钮被点击！");
        ClearFixedPositions();
    }
    
    void OnShowFixedPositionsClicked()
    {
        Debug.Log("显示固定位置按钮被点击！");
        ShowFixedPositions();
    }
    
    /// <summary>
    /// 形状类型按钮点击事件
    /// </summary>
    void OnShapeTypeButtonClicked(int index)
    {
        var config = LevelEditorConfig.Instance;
        if (config != null && index < config.shapeTypes.Count)
        {
            string shapeType = config.shapeTypes[index].name;
            Debug.Log($"点击形状类型按钮: {shapeType} (索引: {index})");
            
            // 更新当前形状类型索引
            currentShapeTypeIndex = index;
            
            // 更新按钮状态（选中/未选中）
            UpdateShapeTypeButtonStates(index);
            
            // 检查是否有选中的形状，如果有则更新类型
            if (selectedShape != null)
            {
                UpdateShapeType(index);
                Debug.Log($"形状类型已更新为: {shapeType}");
            }
            else
            {
                Debug.Log($"已选择形状类型: {shapeType}（用于新建形状）");
            }
        }
    }
    
    /// <summary>
    /// 球类型按钮点击事件
    /// </summary>
    void OnBallTypeButtonClicked(int index)
    {
        var config = LevelEditorConfig.Instance;
        if (config != null && index < config.ballTypes.Count)
        {
            string ballType = config.ballTypes[index].name;
            Debug.Log($"点击球类型按钮: {ballType} (索引: {index})");
            
            // 更新当前球类型索引
            currentBallTypeIndex = index;
            
            // 更新按钮状态（选中/未选中）
            UpdateBallTypeButtonStates(index);
            
            // 检查是否有选中的球，如果有则更新类型
            if (selectedBall != null)
            {
                UpdateBallType(index);
                Debug.Log($"球类型已更新为: {ballType}");
            }
            else
            {
                Debug.Log($"已选择球类型: {ballType}（用于新建球）");
            }
        }
    }
    
    /// <summary>
    /// 更新形状类型按钮状态
    /// </summary>
    void UpdateShapeTypeButtonStates(int selectedIndex)
    {
        if (shapeTypeButtons != null)
        {
            for (int i = 0; i < shapeTypeButtons.Length; i++)
            {
                if (shapeTypeButtons[i] != null)
                {
                    Image buttonBg = shapeTypeButtons[i].GetComponent<Image>();
                    if (buttonBg != null)
                    {
                        if (i == selectedIndex)
                        {
                            buttonBg.color = new Color(0.2f, 0.6f, 1f, 1f); // 选中状态
                        }
                        else
                        {
                            buttonBg.color = new Color(0.4f, 0.4f, 0.4f, 1f); // 未选中状态
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 更新球类型按钮状态
    /// </summary>
    void UpdateBallTypeButtonStates(int selectedIndex)
    {
        if (ballTypeButtons != null)
        {
            for (int i = 0; i < ballTypeButtons.Length; i++)
            {
                if (ballTypeButtons[i] != null)
                {
                    Image buttonBg = ballTypeButtons[i].GetComponent<Image>();
                    if (buttonBg != null)
                    {
                        if (i == selectedIndex)
                        {
                            buttonBg.color = new Color(0.2f, 0.6f, 1f, 1f); // 选中状态
                        }
                        else
                        {
                            buttonBg.color = new Color(0.4f, 0.4f, 0.4f, 1f); // 未选中状态
                        }
                    }
                }
            }
        }
    }
    
    void InitializeDefaultData()
    {
        // 加载配置
        LoadConfiguration();
        
        // 如果没有初始化数据，创建默认数据
        if (currentLevel == null)
        {
            // 使用配置中的索引创建默认关卡名称
            int defaultIndex = LevelEditorConfig.Instance.GetLevelIndex();
            string defaultLevelName = $"LevelConfig_{defaultIndex}";
            currentLevel = new LevelData(defaultLevelName);
        }
        else
        {
            // 如果关卡已存在，但在运行时，也要确保关卡名称使用正确的格式
            if (Application.isPlaying)
            {
                int defaultIndex = LevelEditorConfig.Instance.GetLevelIndex();
                string correctLevelName = $"LevelConfig_{defaultIndex}";
                if (currentLevel.levelName != correctLevelName)
                {
                    currentLevel.levelName = correctLevelName;
                    Debug.Log($"运行时更新关卡名称: {correctLevelName}");
                }
            }
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
        
        // 初始化关卡名称显示
        if (levelNameInput != null)
        {
            levelNameInput.text = currentLevel.levelName;
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
                // 只在配置为空时才重新加载，避免重置levelIndex
                if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
                {
                    Debug.Log("配置为空，重新加载");
                    config.LoadConfigFromFile();
                    
                    // 如果配置仍然为空，初始化默认配置
                    if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
                    {
                        Debug.Log("配置为空，初始化默认配置");
                        // 保存当前的levelIndex，避免被重置
                        int savedLevelIndex = config.GetLevelIndex();
                        config.InitializeDefaultConfig();
                        // 确保levelIndex不被重置
                        config.SetLevelIndex(savedLevelIndex);
                        Debug.Log($"保持levelIndex: {savedLevelIndex}");
                    }
                }
                else
                {
                    Debug.Log("配置已存在，跳过重新加载");
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
    
    public void CreateLevel()
    {
        if (dataManager != null) dataManager.CreateLevel();
    }
    
    public void UpdateLevelName(string newName)
    {
        if (dataManager != null) dataManager.UpdateLevelName(newName);
    }
    
    public void AddShape()
    {
        if (dataManager != null) dataManager.AddShape();
    }
    
    public void AddBall()
    {
        if (dataManager != null) dataManager.AddBall();
    }
    
    public void DeleteShape()
    {
        if (dataManager != null) dataManager.DeleteShape();
    }
    
    public void DeleteBall()
    {
        if (dataManager != null) dataManager.DeleteBall();
    }
    
    public void AddFixedPosition()
    {
        if (selectedShape == null)
        {
            Debug.LogWarning("请先选中一个形状");
            return;
        }
        
        // 获取鼠标在编辑区的位置
        Vector2 mousePosition = GetMousePositionInEditArea();
        
        // 检查位置是否有效
        if (mousePosition == Vector2.zero && editAreaBackground != null)
        {
            // 如果获取失败，使用形状当前位置作为默认位置
            mousePosition = selectedShape.ShapeData.position;
            Debug.Log("使用形状当前位置作为固定位置");
        }
        
        selectedShape.ShapeData.AddFixedPosition(mousePosition);
        
        Debug.Log($"已为形状 '{selectedShape.ShapeData.shapeType}' 添加固定位置: {mousePosition}");
        RefreshUI();
    }
    
    /// <summary>
    /// 在指定位置添加固定位置
    /// </summary>
    public void AddFixedPosition(Vector2 position)
    {
        if (selectedShape == null)
        {
            Debug.LogWarning("请先选中一个形状");
            return;
        }
        
        selectedShape.ShapeData.AddFixedPosition(position);
        
        Debug.Log($"已为形状 '{selectedShape.ShapeData.shapeType}' 添加固定位置: {position}");
        RefreshUI();
    }
    
    public void ClearFixedPositions()
    {
        if (selectedShape == null)
        {
            Debug.LogWarning("请先选中一个形状");
            return;
        }
        
        selectedShape.ShapeData.ClearFixedPositions();
        Debug.Log($"已清除形状 '{selectedShape.ShapeData.shapeType}' 的所有固定位置");
        RefreshUI();
    }
    
    public void ShowFixedPositions()
    {
        if (selectedShape == null)
        {
            Debug.LogWarning("请先选中一个形状");
            return;
        }
        
        ShapeData shapeData = selectedShape.ShapeData;
        if (shapeData.HasFixedPositions())
        {
            string positions = "";
            for (int i = 0; i < shapeData.fixedPositions.Count; i++)
            {
                positions += $"位置{i + 1}: {shapeData.fixedPositions[i]}\n";
            }
            Debug.Log($"形状 '{shapeData.shapeType}' 的固定位置:\n{positions}");
        }
        else
        {
            Debug.Log($"形状 '{shapeData.shapeType}' 没有配置固定位置");
        }
    }
    
    /// <summary>
    /// 获取鼠标在编辑区的位置
    /// </summary>
    private Vector2 GetMousePositionInEditArea()
    {
        if (editAreaBackground == null)
        {
            Debug.LogWarning("编辑区背景为空，无法获取鼠标位置");
            return Vector2.zero;
        }
            
        // 获取鼠标屏幕位置
        Vector3 mouseScreenPos = Input.mousePosition;
        
        // 尝试转换为编辑区的本地坐标
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            editAreaBackground.rectTransform, 
            mouseScreenPos, 
            null, 
            out localPoint))
        {
            Debug.Log($"成功获取鼠标位置: 屏幕坐标={mouseScreenPos}, 本地坐标={localPoint}");
            return localPoint;
        }
        
        // 如果转换失败，尝试使用世界坐标转换
        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            editAreaBackground.rectTransform,
            mouseScreenPos,
            null,
            out worldPoint))
        {
            Vector2 localFromWorld = editAreaBackground.rectTransform.InverseTransformPoint(worldPoint);
            Debug.Log($"通过世界坐标获取鼠标位置: 屏幕坐标={mouseScreenPos}, 世界坐标={worldPoint}, 本地坐标={localFromWorld}");
            return localFromWorld;
        }
        
        Debug.LogWarning($"无法将鼠标位置转换为编辑区坐标: 屏幕坐标={mouseScreenPos}");
        return Vector2.zero;
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
    
    public void OnPositionXChanged(float value)
    {
        if (uiManager != null) uiManager.UpdatePositionX(value);
    }
    
    public void OnPositionYChanged(float value)
    {
        if (uiManager != null) uiManager.UpdatePositionY(value);
    }
    
    public void OnRotationChanged(float value)
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
    /// 在运行时创建形状类型和球类型按钮
    /// </summary>
    void CreateTypeButtons()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("CreateTypeButtons只能在运行时调用");
            return;
        }
        
        Debug.Log("开始创建类型按钮...");
        
        // 确保配置已加载
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("无法获取配置实例，无法创建类型按钮");
            return;
        }
        
        // 创建形状类型按钮
        CreateShapeTypeButtons();
        
        // 创建球类型按钮
        CreateBallTypeButtons();
        
        Debug.Log("类型按钮创建完成");
    }
    
    /// <summary>
    /// 创建形状类型按钮
    /// </summary>
    void CreateShapeTypeButtons()
    {
        if (rightPanel == null)
        {
            Debug.LogError("右侧面板为空，无法创建形状类型按钮");
            return;
        }
        
        // 从配置中获取形状类型
        var config = LevelEditorConfig.Instance;
        string[] shapeTypes = config.GetShapeTypeNames();
        Debug.Log($"创建 {shapeTypes.Length} 个形状类型按钮");
        
        shapeTypeButtons = new Button[shapeTypes.Length];
        
        for (int i = 0; i < shapeTypes.Length; i++)
        {
            // 创建按钮
            GameObject buttonObj = new GameObject($"ShapeTypeButton_{i}");
            buttonObj.transform.SetParent(rightPanel.transform, false);
            
            // 添加组件
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            Button button = buttonObj.AddComponent<Button>();
            
            // 设置按钮位置和大小
            buttonRect.anchorMin = new Vector2(0.05f, 0.5f - (i * 0.03f));
            buttonRect.anchorMax = new Vector2(0.45f, 0.53f - (i * 0.03f));
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // 设置按钮外观
            buttonImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            
            // 创建文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            Text text = textObj.AddComponent<Text>();
            
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            text.text = shapeTypes[i];
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 12;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            // 保存按钮引用
            shapeTypeButtons[i] = button;
        }
        
        // 设置默认选中状态
        if (shapeTypeButtons.Length > 0)
        {
            UpdateShapeTypeButtonStates(currentShapeTypeIndex);
        }
    }
    
    /// <summary>
    /// 创建球类型按钮
    /// </summary>
    void CreateBallTypeButtons()
    {
        if (rightPanel == null)
        {
            Debug.LogError("右侧面板为空，无法创建球类型按钮");
            return;
        }
        
        // 从配置中获取球类型
        var config = LevelEditorConfig.Instance;
        string[] ballTypes = config.GetBallTypeNames();
        Debug.Log($"创建 {ballTypes.Length} 个球类型按钮");
        
        ballTypeButtons = new Button[ballTypes.Length];
        
        for (int i = 0; i < ballTypes.Length; i++)
        {
            // 创建按钮
            GameObject buttonObj = new GameObject($"BallTypeButton_{i}");
            buttonObj.transform.SetParent(rightPanel.transform, false);
            
            // 添加组件
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            Button button = buttonObj.AddComponent<Button>();
            
            // 设置按钮位置和大小
            buttonRect.anchorMin = new Vector2(0.55f, 0.5f - (i * 0.03f));
            buttonRect.anchorMax = new Vector2(0.95f, 0.53f - (i * 0.03f));
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // 设置按钮外观
            buttonImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            
            // 创建文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            Text text = textObj.AddComponent<Text>();
            
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            text.text = ballTypes[i];
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 12;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            // 保存按钮引用
            ballTypeButtons[i] = button;
        }
        
        // 设置默认选中状态
        if (ballTypeButtons.Length > 0)
        {
            UpdateBallTypeButtonStates(currentBallTypeIndex);
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