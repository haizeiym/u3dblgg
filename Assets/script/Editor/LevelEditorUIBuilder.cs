using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

/// <summary>
/// 关卡编辑器UI构建器
/// 负责创建关卡编辑器的UI结构，不处理事件绑定
/// </summary>
public class LevelEditorUIBuilder
{
    private LevelEditorUI levelEditor;
    
    public LevelEditorUIBuilder(LevelEditorUI editor)
    {
        levelEditor = editor;
    }
    
    /// <summary>
    /// 确保配置已加载
    /// </summary>
    void EnsureConfigLoaded()
    {
        try
        {
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                // 尝试从文件加载配置
                config.LoadConfigFromFile();
                Debug.Log("LevelEditorUIBuilder: 配置已加载");
                
                // 如果配置为空，初始化默认配置
                if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
                {
                    Debug.Log("LevelEditorUIBuilder: 配置为空，初始化默认配置");
                    config.InitializeDefaultConfig();
                }
                
                Debug.Log($"LevelEditorUIBuilder: 配置加载完成 - 形状: {config.shapeTypes.Count}, 球: {config.ballTypes.Count}, 背景: {config.backgroundConfigs.Count}");
            }
            else
            {
                Debug.LogError("LevelEditorUIBuilder: 无法获取LevelEditorConfig实例");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LevelEditorUIBuilder: 配置加载失败: {e.Message}");
        }
    }
    
    public void CreateUIStructure()
    {
        Debug.Log("开始创建UI结构...");
        
        CreateLeftPanel();
        CreateCenterPanel();
        CreateRightPanel();
        CreatePrefabs();
        
        Debug.Log("UI结构创建完成");
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
        Debug.Log("创建左侧面板按钮...");
        
        // 创建添加层级按钮
        GameObject addButton = new GameObject("添加层级Button");
        addButton.transform.SetParent(parent.transform, false);
        
        RectTransform addButtonRect = addButton.AddComponent<RectTransform>();
        addButtonRect.anchorMin = new Vector2(0, 0.9f);
        addButtonRect.anchorMax = new Vector2(0.15f, 0.95f);
        addButtonRect.offsetMin = Vector2.zero;
        addButtonRect.offsetMax = Vector2.zero;
        
        Image addButtonBg = addButton.AddComponent<Image>();
        addButtonBg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        addButtonBg.raycastTarget = true;
        
        Button addButtonComponent = addButton.AddComponent<Button>();
        addButtonComponent.targetGraphic = addButtonBg;
        addButtonComponent.interactable = true;
        addButtonComponent.transition = Selectable.Transition.ColorTint;
        addButtonComponent.navigation = new Navigation() { mode = Navigation.Mode.None };
        
        levelEditor.addLayerButton = addButtonComponent;
        
        // 创建按钮文本
        GameObject addTextObj = new GameObject("Text");
        addTextObj.transform.SetParent(addButton.transform, false);
        
        RectTransform addTextRect = addTextObj.AddComponent<RectTransform>();
        addTextRect.anchorMin = Vector2.zero;
        addTextRect.anchorMax = Vector2.one;
        addTextRect.offsetMin = Vector2.zero;
        addTextRect.offsetMax = Vector2.zero;
        
        Text addTextComponent = addTextObj.AddComponent<Text>();
        addTextComponent.text = "添加层级";
        addTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        addTextComponent.fontSize = 20;
        addTextComponent.color = Color.white;
        addTextComponent.alignment = TextAnchor.MiddleCenter;
        
        // 创建删除层级按钮
        GameObject deleteButton = new GameObject("删除层级Button");
        deleteButton.transform.SetParent(parent.transform, false);
        
        RectTransform deleteButtonRect = deleteButton.AddComponent<RectTransform>();
        deleteButtonRect.anchorMin = new Vector2(0, 0.85f);
        deleteButtonRect.anchorMax = new Vector2(0.15f, 0.9f);
        deleteButtonRect.offsetMin = Vector2.zero;
        deleteButtonRect.offsetMax = Vector2.zero;
        
        Image deleteButtonBg = deleteButton.AddComponent<Image>();
        deleteButtonBg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        deleteButtonBg.raycastTarget = true;
        
        Button deleteButtonComponent = deleteButton.AddComponent<Button>();
        deleteButtonComponent.targetGraphic = deleteButtonBg;
        deleteButtonComponent.interactable = true;
        deleteButtonComponent.transition = Selectable.Transition.ColorTint;
        deleteButtonComponent.navigation = new Navigation() { mode = Navigation.Mode.None };
        
        levelEditor.deleteLayerButton = deleteButtonComponent;
        
        // 创建按钮文本
        GameObject deleteTextObj = new GameObject("Text");
        deleteTextObj.transform.SetParent(deleteButton.transform, false);
        
        RectTransform deleteTextRect = deleteTextObj.AddComponent<RectTransform>();
        deleteTextRect.anchorMin = Vector2.zero;
        deleteTextRect.anchorMax = Vector2.one;
        deleteTextRect.offsetMin = Vector2.zero;
        deleteTextRect.offsetMax = Vector2.zero;
        
        Text deleteTextComponent = deleteTextObj.AddComponent<Text>();
        deleteTextComponent.text = "删除层级";
        deleteTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        deleteTextComponent.fontSize = 20;
        deleteTextComponent.color = Color.white;
        deleteTextComponent.alignment = TextAnchor.MiddleCenter;
        
        Debug.Log("左侧面板按钮创建完成");
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
        
        GameObject addShapeButton = UIComponentBuilder.CreateButton(toolbar, "添加形状", new Vector2(0.05f, 0));
        levelEditor.addShapeButton = addShapeButton.GetComponent<Button>();
        
        GameObject deleteShapeButton = UIComponentBuilder.CreateButton(toolbar, "删除形状", new Vector2(0.2f, 0));
        levelEditor.deleteShapeButton = deleteShapeButton.GetComponent<Button>();
        
        GameObject addBallButton = UIComponentBuilder.CreateButton(toolbar, "添加球", new Vector2(0.35f, 0));
        levelEditor.addBallButton = addBallButton.GetComponent<Button>();
        
        GameObject deleteBallButton = UIComponentBuilder.CreateButton(toolbar, "删除球", new Vector2(0.5f, 0));
        levelEditor.deleteBallButton = deleteBallButton.GetComponent<Button>();
        
        // 创建背景按钮
        GameObject backgroundButton = UIComponentBuilder.CreateButton(toolbar, "背景", new Vector2(0.65f, 0));
        levelEditor.backgroundButton = backgroundButton.GetComponent<Button>();
        
        // 创建预览按钮
        GameObject previewButton = UIComponentBuilder.CreateButton(toolbar, "预览", new Vector2(0.8f, 0));
        levelEditor.previewButton = previewButton.GetComponent<Button>();
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
        
        // 形状类型按钮（只在运行时创建）
        if (Application.isPlaying)
        {
            CreateShapeTypeButtons(parent, new Vector2(0, 0.5f));
        }
        // 球类型按钮（只在运行时创建）
        if (Application.isPlaying)
        {
            CreateBallTypeButtons(parent, new Vector2(0, 0.4f));
        }
        
        // 导入导出按钮
        GameObject importButton = UIComponentBuilder.CreateButton(parent, "导入关卡", new Vector2(0, 0.15f));
        levelEditor.importButton = importButton.GetComponent<Button>();
        
        GameObject exportButton = UIComponentBuilder.CreateButton(parent, "导出JSON", new Vector2(0, 0.1f));
        levelEditor.exportButton = exportButton.GetComponent<Button>();
    }
    
    /// <summary>
    /// 创建形状类型按钮列表
    /// </summary>
    public void CreateShapeTypeButtons(GameObject parent, Vector2 position)
    {
        // 确保配置已加载
        EnsureConfigLoaded();
        
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
        
        // 从配置中获取形状类型
        string[] shapeTypes = LevelEditorConfig.Instance.GetShapeTypeNames();
        Debug.Log($"获取到 {shapeTypes.Length} 个形状类型");
        levelEditor.shapeTypeButtons = new Button[shapeTypes.Length];
        
        for (int i = 0; i < shapeTypes.Length; i++)
        {
            // 使用UIComponentBuilder创建简单按钮
            Vector2 buttonPos = new Vector2(position.x, position.y - 0.03f - (i * 0.03f));
            GameObject buttonObj = UIComponentBuilder.CreateButton(parent, shapeTypes[i], buttonPos);
            
            // 调整按钮大小
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 20);
            
            levelEditor.shapeTypeButtons[i] = buttonObj.GetComponent<Button>();
        }
    }
    
    public void CreateBallTypeButtons(GameObject parent, Vector2 position)
    {
        Debug.Log("开始创建球类型按钮...");
        
        // 确保配置已加载
        EnsureConfigLoaded();
        
        // 创建标签
        GameObject labelObj = new GameObject("BallTypeLabel");
        labelObj.transform.SetParent(parent.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(position.x, position.y);
        labelRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.02f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "球类型";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 12;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        // 从配置中获取球类型
        string[] ballTypes = LevelEditorConfig.Instance.GetBallTypeNames();
        Debug.Log($"获取到 {ballTypes.Length} 个球类型");
        levelEditor.ballTypeButtons = new Button[ballTypes.Length];
        
        for (int i = 0; i < ballTypes.Length; i++)
        {
            Vector2 buttonPos = new Vector2(position.x, position.y - 0.03f - (i * 0.03f));
            GameObject buttonObj = UIComponentBuilder.CreateButton(parent, ballTypes[i], buttonPos);
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 20);
            
            levelEditor.ballTypeButtons[i] = buttonObj.GetComponent<Button>();
        }
        
        Debug.Log($"球类型按钮创建完成，共创建 {ballTypes.Length} 个按钮");
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
} 