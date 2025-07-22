using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡编辑器UI构建器
/// 负责创建关卡编辑器的UI结构
/// </summary>
public class LevelEditorUIBuilder
{
    private LevelEditorUI levelEditor;
    
    public LevelEditorUIBuilder(LevelEditorUI editor)
    {
        levelEditor = editor;
    }
    
    public void CreateUIStructure()
    {
        CreateLeftPanel();
        CreateCenterPanel();
        CreateRightPanel();
        CreatePrefabs();
    }
    
    void CreateLeftPanel()
    {
        GameObject leftPanel = new GameObject("LeftPanel");
        leftPanel.transform.SetParent(levelEditor.transform, false);
        
        RectTransform leftRect = leftPanel.AddComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0, 0);
        leftRect.anchorMax = new Vector2(0.2f, 1);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = Vector2.zero;
        
        Image leftBg = leftPanel.AddComponent<Image>();
        leftBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        levelEditor.leftPanel = leftPanel;
        
        CreateLayerListContent(leftPanel);
        CreateLeftPanelButtons(leftPanel);
    }
    
    void CreateLayerListContent(GameObject parent)
    {
        GameObject content = new GameObject("LayerListContent");
        content.transform.SetParent(parent.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 0.8f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        ScrollRect scrollRect = content.AddComponent<ScrollRect>();
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.padding = new RectOffset(10, 10, 10, 10);
        
        levelEditor.levelListContent = content.transform;
    }
    
    void CreateLeftPanelButtons(GameObject parent)
    {
        GameObject addButton = UIComponentBuilder.CreateButton(parent, "添加层级", new Vector2(0, 0.9f));
        levelEditor.addLayerButton = addButton.GetComponent<Button>();
        
        GameObject deleteButton = UIComponentBuilder.CreateButton(parent, "删除层级", new Vector2(0, 0.85f));
        levelEditor.deleteLayerButton = deleteButton.GetComponent<Button>();
    }
    
    void CreateCenterPanel()
    {
        GameObject centerPanel = new GameObject("CenterPanel");
        centerPanel.transform.SetParent(levelEditor.transform, false);
        
        RectTransform centerRect = centerPanel.AddComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.2f, 0);
        centerRect.anchorMax = new Vector2(0.8f, 1);
        centerRect.offsetMin = Vector2.zero;
        centerRect.offsetMax = Vector2.zero;
        
        Image centerBg = centerPanel.AddComponent<Image>();
        centerBg.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        
        levelEditor.centerPanel = centerPanel;
        
        CreateEditAreaContent(centerPanel);
        CreateToolbar(centerPanel);
    }
    
    void CreateEditAreaContent(GameObject parent)
    {
        GameObject editContent = new GameObject("EditAreaContent");
        editContent.transform.SetParent(parent.transform, false);
        
        RectTransform editRect = editContent.AddComponent<RectTransform>();
        editRect.anchorMin = Vector2.zero;
        editRect.anchorMax = Vector2.one;
        editRect.offsetMin = new Vector2(0, 50);
        editRect.offsetMax = Vector2.zero;
        
        levelEditor.editAreaContent = editContent.transform;
    }
    
    void CreateToolbar(GameObject parent)
    {
        GameObject toolbar = new GameObject("Toolbar");
        toolbar.transform.SetParent(parent.transform, false);
        
        RectTransform toolbarRect = toolbar.AddComponent<RectTransform>();
        toolbarRect.anchorMin = new Vector2(0, 1);
        toolbarRect.anchorMax = new Vector2(1, 1);
        toolbarRect.anchoredPosition = new Vector2(0, -25);
        toolbarRect.sizeDelta = new Vector2(0, 50);
        
        Image toolbarBg = toolbar.AddComponent<Image>();
        toolbarBg.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        
        GameObject addShapeButton = UIComponentBuilder.CreateButton(toolbar, "添加形状", new Vector2(0.1f, 0));
        levelEditor.addShapeButton = addShapeButton.GetComponent<Button>();
        
        GameObject addBallButton = UIComponentBuilder.CreateButton(toolbar, "添加球", new Vector2(0.3f, 0));
        levelEditor.addBallButton = addBallButton.GetComponent<Button>();
    }
    
    void CreateRightPanel()
    {
        GameObject rightPanel = new GameObject("RightPanel");
        rightPanel.transform.SetParent(levelEditor.transform, false);
        
        RectTransform rightRect = rightPanel.AddComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.8f, 0);
        rightRect.anchorMax = new Vector2(1, 1);
        rightRect.offsetMin = Vector2.zero;
        rightRect.offsetMax = Vector2.zero;
        
        Image rightBg = rightPanel.AddComponent<Image>();
        rightBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        levelEditor.rightPanel = rightPanel;
        
        CreatePropertyControls(rightPanel);
    }
    
    void CreatePropertyControls(GameObject parent)
    {
        GameObject nameInput = UIComponentBuilder.CreateInputField(parent, "名称", new Vector2(0, 0.9f));
        levelEditor.nameInput = nameInput.GetComponent<InputField>();
        
        GameObject posXSlider = UIComponentBuilder.CreateSlider(parent, "X位置", new Vector2(0, 0.8f));
        levelEditor.positionXSlider = posXSlider.GetComponent<Slider>();
        
        GameObject posYSlider = UIComponentBuilder.CreateSlider(parent, "Y位置", new Vector2(0, 0.7f));
        levelEditor.positionYSlider = posYSlider.GetComponent<Slider>();
        
        GameObject rotSlider = UIComponentBuilder.CreateSlider(parent, "旋转", new Vector2(0, 0.6f));
        levelEditor.rotationSlider = rotSlider.GetComponent<Slider>();
        
        // 创建形状类型按钮列表（替代Dropdown）
        CreateShapeTypeButtons(parent, new Vector2(0, 0.5f));
        
        GameObject exportButton = UIComponentBuilder.CreateButton(parent, "导出JSON", new Vector2(0, 0.1f));
        levelEditor.exportButton = exportButton.GetComponent<Button>();
    }
    
    /// <summary>
    /// 创建形状类型按钮列表（使用Unity自带简单按钮）
    /// </summary>
    void CreateShapeTypeButtons(GameObject parent, Vector2 position)
    {
        // 创建标签
        GameObject labelObj = new GameObject("TypeLabel");
        labelObj.transform.SetParent(parent.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(position.x, position.y);
        labelRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.02f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "形状类型";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 12;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        // 创建形状类型按钮
        string[] shapeTypes = { "圆形", "矩形", "三角形", "菱形" };
        levelEditor.shapeTypeButtons = new Button[shapeTypes.Length];
        
        for (int i = 0; i < shapeTypes.Length; i++)
        {
            // 使用UIComponentBuilder创建简单按钮
            Vector2 buttonPos = new Vector2(position.x, position.y - 0.03f - (i * 0.03f));
            GameObject buttonObj = UIComponentBuilder.CreateButton(parent, shapeTypes[i], buttonPos);
            
            // 调整按钮大小
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 20);
            
            // 设置按钮点击事件
            int index = i; // 捕获循环变量
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnShapeTypeButtonClicked(index, shapeTypes[index]));
            
            levelEditor.shapeTypeButtons[i] = button;
        }
    }
    
    /// <summary>
    /// 形状类型按钮点击事件
    /// </summary>
    void OnShapeTypeButtonClicked(int index, string shapeType)
    {
        Debug.Log($"点击形状类型按钮: {shapeType} (索引: {index})");
        
        // 检查是否有选中的形状
        if (levelEditor.selectedShape == null)
        {
            Debug.LogWarning("没有选中任何形状，无法更改类型");
            return;
        }
        
        // 更新按钮状态（选中/未选中）
        for (int i = 0; i < levelEditor.shapeTypeButtons.Length; i++)
        {
            Image buttonBg = levelEditor.shapeTypeButtons[i].GetComponent<Image>();
            if (buttonBg != null)
            {
                if (i == index)
                {
                    buttonBg.color = new Color(0.2f, 0.6f, 1f, 1f); // 选中状态
                }
                else
                {
                    buttonBg.color = new Color(0.4f, 0.4f, 0.4f, 1f); // 未选中状态
                }
            }
        }
        
        // 调用LevelEditorUI的公共方法更新类型
        levelEditor.UpdateShapeType(index);
        
        Debug.Log($"形状类型已更新为: {shapeType}");
    }
    
    void CreatePrefabs()
    {
        GameObject shapePrefab = new GameObject("ShapePrefab");
        shapePrefab.AddComponent<RectTransform>();
        shapePrefab.AddComponent<Image>();
        shapePrefab.AddComponent<ShapeController>();
        levelEditor.shapePrefab = shapePrefab;
        
        GameObject ballPrefab = new GameObject("BallPrefab");
        ballPrefab.AddComponent<RectTransform>();
        ballPrefab.AddComponent<Image>();
        ballPrefab.AddComponent<BallController>();
        levelEditor.ballPrefab = ballPrefab;
    }
    
    /// <summary>
    /// 示例：如何添加额外的按钮（一键配置）
    /// </summary>
    void CreateAdditionalButtons(GameObject parent)
    {
        // 示例1：创建简单的按钮
        GameObject simpleButton = UIComponentBuilder.CreateButton(parent, "简单按钮", new Vector2(0, 0.4f));
        
        // 示例2：创建多个按钮
        string[] buttonTexts = { "按钮A", "按钮B", "按钮C" };
        for (int i = 0; i < buttonTexts.Length; i++)
        {
            Vector2 pos = new Vector2(0, 0.3f - (i * 0.03f));
            GameObject button = UIComponentBuilder.CreateButton(parent, buttonTexts[i], pos);
        }
    }
} 