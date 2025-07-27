using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Added for List

/// <summary>
/// 关卡编辑器UI管理器
/// 负责管理UI组件和用户交互
/// </summary>
public class LevelEditorUIManager
{
    private LevelEditorUI editorUI;
    private LevelEditorUIRefresher refresher;
    
    public LevelEditorUIManager(LevelEditorUI editor)
    {
        editorUI = editor;
        refresher = new LevelEditorUIRefresher(editor);
        SetupUI();
    }
    
    void SetupUI()
    {
        SetupSliders();
        SetupBackground();
        // 形状类型按钮在LevelEditorUIBuilder中创建和设置
    }
    
    void SetupSliders()
    {
        if (editorUI.positionXSlider)
        {
            editorUI.positionXSlider.minValue = -500;
            editorUI.positionXSlider.maxValue = 500;
            editorUI.positionXSlider.onValueChanged.AddListener(UpdatePositionX);
        }
        
        if (editorUI.positionYSlider)
        {
            editorUI.positionYSlider.minValue = -300;
            editorUI.positionYSlider.maxValue = 300;
            editorUI.positionYSlider.onValueChanged.AddListener(UpdatePositionY);
        }
        
        if (editorUI.rotationSlider)
        {
            editorUI.rotationSlider.minValue = 0;
            editorUI.rotationSlider.maxValue = 360;
            editorUI.rotationSlider.onValueChanged.AddListener(UpdateRotation);
        }
    }
    
    void SetupBackground()
    {
        // 初始化背景
        if (editorUI != null)
        {
            editorUI.ApplyBackground();
        }
    }
    
    // SetupDropdowns方法已移除，形状类型按钮在LevelEditorUIBuilder中创建和设置
    
    public void RefreshUI()
    {
        refresher.RefreshLayerList();
        refresher.RefreshEditArea();
    }
    
    public void SelectShape(ShapeController shape)
    {
        if (editorUI.selectedShape != null)
        {
            editorUI.selectedShape.SetSelected(false);
        }
        editorUI.selectedShape = shape;
        editorUI.selectedBall = null;
        shape.SetSelected(true);
        UpdatePropertiesPanel();
    }
    
    public void SelectBall(BallController ball)
    {
        if (editorUI.selectedBall != null)
        {
            editorUI.selectedBall.SetSelected(false);
        }
        editorUI.selectedBall = ball;
        editorUI.selectedShape = null;
        ball.SetSelected(true);
        UpdatePropertiesPanel();
    }
    
    void UpdatePropertiesPanel()
    {
        if (editorUI.selectedShape != null)
        {
            ShapeData data = editorUI.selectedShape.GetShapeData();
            if (data != null)
            {
                if (editorUI.nameInput) editorUI.nameInput.text = data.shapeType;
                if (editorUI.positionXSlider) editorUI.positionXSlider.value = data.position.x;
                if (editorUI.positionYSlider) editorUI.positionYSlider.value = data.position.y;
                if (editorUI.rotationSlider) editorUI.rotationSlider.value = data.rotation;
                
                // 更新形状类型按钮状态
                UpdateShapeTypeButtons(data.shapeType);
            }
        }
        else if (editorUI.selectedBall != null)
        {
            BallData data = editorUI.selectedBall.GetBallData();
            if (data != null)
            {
                if (editorUI.nameInput) editorUI.nameInput.text = data.ballType;
                if (editorUI.positionXSlider) editorUI.positionXSlider.value = data.position.x;
                if (editorUI.positionYSlider) editorUI.positionYSlider.value = data.position.y;
                
                // 更新球类型按钮状态
                UpdateBallTypeButtons(data.ballType);
            }
        }
    }
    
    /// <summary>
    /// 更新形状类型按钮状态
    /// </summary>
    void UpdateShapeTypeButtons(string currentType)
    {
        if (editorUI.shapeTypeButtons == null) return;
        
        string[] types = LevelEditorConfig.Instance.GetShapeTypeNames();
        int currentIndex = System.Array.IndexOf(types, currentType);
        
        for (int i = 0; i < editorUI.shapeTypeButtons.Length; i++)
        {
            if (editorUI.shapeTypeButtons[i] != null)
            {
                Image buttonBg = editorUI.shapeTypeButtons[i].GetComponent<Image>();
                if (buttonBg != null)
                {
                    if (i == currentIndex)
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
    
    public void UpdatePositionX(float value)
    {
        if (editorUI.selectedShape != null)
        {
            ShapeData shapeData = editorUI.selectedShape.GetShapeData();
            if (shapeData != null)
            {
                Vector2 pos = shapeData.position;
                pos.x = value;
                editorUI.selectedShape.SetPosition(pos);
            }
        }
        else if (editorUI.selectedBall != null)
        {
            BallData ballData = editorUI.selectedBall.GetBallData();
            if (ballData != null)
            {
                Vector2 pos = ballData.position;
                pos.x = value;
                editorUI.selectedBall.SetPosition(pos);
            }
        }
    }
    
    public void UpdatePositionY(float value)
    {
        if (editorUI.selectedShape != null)
        {
            ShapeData shapeData = editorUI.selectedShape.GetShapeData();
            if (shapeData != null)
            {
                Vector2 pos = shapeData.position;
                pos.y = value;
                editorUI.selectedShape.SetPosition(pos);
            }
        }
        else if (editorUI.selectedBall != null)
        {
            BallData ballData = editorUI.selectedBall.GetBallData();
            if (ballData != null)
            {
                Vector2 pos = ballData.position;
                pos.y = value;
                editorUI.selectedBall.SetPosition(pos);
            }
        }
    }
    
    public void UpdateRotation(float value)
    {
        if (editorUI.selectedShape != null)
        {
            editorUI.selectedShape.SetRotation(value);
        }
    }
    
    public void UpdateType(int index)
    {
        if (editorUI.selectedShape != null)
        {
            ShapeData shapeData = editorUI.selectedShape.GetShapeData();
            if (shapeData != null)
            {
                string[] types = LevelEditorConfig.Instance.GetShapeTypeNames();
                if (index >= 0 && index < types.Length)
                {
                    string newType = types[index];
                    
                    Debug.Log($"更新形状类型: {shapeData.shapeType} -> {newType}");
                    
                    // 保存旧的形状类型，用于检查是否需要清除固定位置
                    string oldType = shapeData.shapeType;
                    
                    shapeData.shapeType = newType;
                    
                    // 如果形状类型发生变化，重新加载固定位置配置
                    if (oldType != newType)
                    {
                        // 清除旧的固定位置
                        shapeData.ClearFixedPositions();
                        
                        // 从配置文件加载新形状类型的固定位置
                        shapeData.LoadFixedPositionsFromConfig();
                        
                        Debug.Log($"已为形状 '{newType}' 重新加载固定位置配置，固定位置数量: {shapeData.fixedPositions.Count}");
                        
                        // 处理现有球的位置
                        if (shapeData.balls.Count > 0)
                        {
                            if (shapeData.HasFixedPositions())
                            {
                                // 如果有固定位置，将球移动到可用的固定位置
                                int fixedPosCount = shapeData.fixedPositions.Count;
                                for (int i = 0; i < shapeData.balls.Count; i++)
                                {
                                    if (i < fixedPosCount)
                                    {
                                        // 移动到对应的固定位置
                                        shapeData.balls[i].position = shapeData.fixedPositions[i];
                                        Debug.Log($"球 {i} 已移动到固定位置: {shapeData.fixedPositions[i]}");
                                    }
                                    else
                                    {
                                        // 如果固定位置不够，将多余的球移动到形状中心
                                        shapeData.balls[i].position = shapeData.position;
                                        Debug.Log($"球 {i} 已移动到形状中心: {shapeData.position}");
                                    }
                                }
                            }
                            else
                            {
                                // 如果没有固定位置，将所有球移动到形状中心
                                for (int i = 0; i < shapeData.balls.Count; i++)
                                {
                                    shapeData.balls[i].position = shapeData.position;
                                    Debug.Log($"球 {i} 已移动到形状中心: {shapeData.position}");
                                }
                            }
                        }
                    }
                    
                    editorUI.selectedShape.UpdateVisual();
                    
                    // 更新按钮状态以反映当前选中的类型
                    UpdateShapeTypeButtons(newType);
                    
                    // 刷新UI以确保固定位置变化能够正确显示
                    editorUI.RefreshUI();
                    
                    // 恢复形状的选中状态
                    string selectedShapeType = shapeData.shapeType;
                    Vector2 selectedShapePosition = shapeData.position;
                    float selectedShapeRotation = shapeData.rotation;
                    
                    foreach (GameObject shapeObj in editorUI.UIManager.GetAllShapeObjects())
                    {
                        ShapeController controller = shapeObj.GetComponent<ShapeController>();
                        if (controller != null && controller.ShapeData != null)
                        {
                            ShapeData currentShapeData = controller.ShapeData;
                            if (currentShapeData.shapeType == selectedShapeType &&
                                Vector2.Distance(currentShapeData.position, selectedShapePosition) < 0.1f &&
                                Mathf.Abs(currentShapeData.rotation - selectedShapeRotation) < 0.1f)
                            {
                                editorUI.SelectShape(controller);
                                Debug.Log($"已恢复形状选中状态: {selectedShapeType}");
                                break;
                            }
                        }
                    }
                    
                    Debug.Log($"形状类型更新完成: {newType}");
                }
                else
                {
                    Debug.LogWarning($"形状类型索引超出范围: {index}");
                }
            }
            else
            {
                Debug.LogWarning("选中的形状没有有效的数据");
            }
        }
        else
        {
            Debug.LogWarning("没有选中任何形状");
        }
    }
    
    public GameObject CreateShapeObject(ShapeData shapeData)
    {
        return refresher.CreateShapeObject(shapeData);
    }
    
    public GameObject CreateBallObject(BallData ballData)
    {
        return refresher.CreateBallObject(ballData);
    }
    
    /// <summary>
    /// 获取最后创建的形状对象
    /// </summary>
    public GameObject GetLastCreatedShapeObject()
    {
        return refresher.GetLastCreatedShapeObject();
    }
    
    /// <summary>
    /// 获取所有形状对象
    /// </summary>
    public List<GameObject> GetAllShapeObjects()
    {
        return refresher.GetAllShapeObjects();
    }

    public void UpdateBallType(int index)
    {
        if (editorUI.selectedBall != null)
        {
            BallData ballData = editorUI.selectedBall.GetBallData();
            if (ballData != null)
            {
                string[] types = LevelEditorConfig.Instance.GetBallTypeNames();
                if (index >= 0 && index < types.Length)
                {
                    string newType = types[index];
                    ballData.ballType = newType;
                    editorUI.selectedBall.UpdateVisual();
                    UpdateBallTypeButtons(newType);
                }
                else
                {
                    Debug.LogWarning($"球类型索引超出范围: {index}");
                }
            }
        }
    }

    void UpdateBallTypeButtons(string currentType)
    {
        if (editorUI.ballTypeButtons == null) return;
        string[] types = LevelEditorConfig.Instance.GetBallTypeNames();
        int currentIndex = System.Array.IndexOf(types, currentType);
        for (int i = 0; i < editorUI.ballTypeButtons.Length; i++)
        {
            if (editorUI.ballTypeButtons[i] != null)
            {
                var img = editorUI.ballTypeButtons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    img.color = (i == currentIndex) ? new Color(0.2f, 0.6f, 1f, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f);
                }
            }
        }
    }
} 