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
    public Button addShapeButton;
    public Button addBallButton;
    
    [Header("Right Panel - Properties")]
    public InputField nameInput;
    public Slider positionXSlider;
    public Slider positionYSlider;
    public Slider rotationSlider;
    public Button[] shapeTypeButtons; // 替换Dropdown为按钮数组
    public Button exportButton;
    
    [Header("Prefabs")]
    public GameObject shapePrefab;
    public GameObject ballPrefab;
    
    // 数据
    public LevelData currentLevel;
    public LayerData currentLayer;
    public ShapeController selectedShape;
    public BallController selectedBall;
    public int currentShapeTypeIndex = 0; // 0:圆形, 1:矩形, 2:三角形, 3:菱形
    
    // UI管理器
    private LevelEditorUIManager uiManager;
    private LevelEditorDataManager dataManager;
    
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
    }
    
    void SetupEventListeners()
    {
        if (addLayerButton) addLayerButton.onClick.AddListener(AddLayer);
        if (deleteLayerButton) deleteLayerButton.onClick.AddListener(DeleteLayer);
        if (addShapeButton) addShapeButton.onClick.AddListener(AddShape);
        if (addBallButton) addBallButton.onClick.AddListener(AddBall);
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
        // 如果没有初始化数据，创建默认数据
        if (currentLevel == null)
        {
            currentLevel = new LevelData("新关卡");
            currentLayer = new LayerData("默认层级");
            currentLevel.layers.Add(currentLayer);
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
} 