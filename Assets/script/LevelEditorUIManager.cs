using UnityEngine;
using UnityEngine.UI;

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
            }
        }
    }
    
    /// <summary>
    /// 更新形状类型按钮状态
    /// </summary>
    void UpdateShapeTypeButtons(string currentType)
    {
        if (editorUI.shapeTypeButtons == null) return;
        
        string[] types = { "圆形", "矩形", "三角形", "菱形" };
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
                string[] types = { "圆形", "矩形", "三角形", "菱形" };
                string newType = types[index];
                
                Debug.Log($"更新形状类型: {shapeData.shapeType} -> {newType}");
                
                shapeData.shapeType = newType;
                editorUI.selectedShape.UpdateVisual();
                
                // 更新按钮状态以反映当前选中的类型
                UpdateShapeTypeButtons(newType);
                
                Debug.Log($"形状类型更新完成: {newType}");
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
} 