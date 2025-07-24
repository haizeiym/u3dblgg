using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 预览预制体生成器
/// 用于生成配置预览UI的预制体
/// </summary>
public class PreviewPrefabGenerator : EditorWindow
{
    [MenuItem("Tools/Level Editor/生成预览预制体")]
    public static void ShowWindow()
    {
        GetWindow<PreviewPrefabGenerator>("预览预制体生成器");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("预览预制体生成器", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("生成完整预览UI"))
        {
            GenerateCompletePreviewUI();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("生成预览项预制体"))
        {
            GeneratePreviewItemPrefabs();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("生成预览面板"))
        {
            GeneratePreviewPanel();
        }
    }

    void GenerateCompletePreviewUI()
    {
        // 创建主Canvas
        GameObject canvasObj = new GameObject("ConfigPreviewCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建预览面板
        GameObject panelObj = CreatePreviewPanel(canvasObj.transform);
        panelObj.name = "PreviewPanel";

        // 添加ConfigPreviewUI组件
        ConfigPreviewUI previewUI = panelObj.AddComponent<ConfigPreviewUI>();
        
        // 设置组件引用
        previewUI.previewPanel = panelObj;
        previewUI.shapePreviewContent = panelObj.transform.Find("Content/ShapeSection/ScrollView/Viewport/Content");
        previewUI.ballPreviewContent = panelObj.transform.Find("Content/BallSection/ScrollView/Viewport/Content");
        previewUI.backgroundPreviewContent = panelObj.transform.Find("Content/BackgroundSection/ScrollView/Viewport/Content");
        previewUI.closeButton = panelObj.transform.Find("Header/CloseButton").GetComponent<Button>();
        previewUI.refreshButton = panelObj.transform.Find("Header/RefreshButton").GetComponent<Button>();
        
        // 设置滚动视图引用
        previewUI.shapeScrollView = panelObj.transform.Find("Content/ShapeSection/ScrollView").GetComponent<ScrollRect>();
        previewUI.ballScrollView = panelObj.transform.Find("Content/BallSection/ScrollView").GetComponent<ScrollRect>();
        previewUI.backgroundScrollView = panelObj.transform.Find("Content/BackgroundSection/ScrollView").GetComponent<ScrollRect>();

        // 保存预制体
        string prefabPath = "Assets/Prefabs/ConfigPreviewUI.prefab";
        CreatePrefabDirectory();
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
        
        Debug.Log($"完整预览UI已生成: {prefabPath}");
        
        // 清理场景中的临时对象
        DestroyImmediate(canvasObj);
        
        // 选中生成的预制体
        Selection.activeObject = prefab;
    }

    void GeneratePreviewItemPrefabs()
    {
        CreatePrefabDirectory();

        // 生成形状预览项预制体
        GameObject shapeItem = CreatePreviewItem("ShapePreviewItem", Color.blue);
        PrefabUtility.SaveAsPrefabAsset(shapeItem, "Assets/Prefabs/ShapePreviewItem.prefab");
        DestroyImmediate(shapeItem);

        // 生成球预览项预制体
        GameObject ballItem = CreatePreviewItem("BallPreviewItem", Color.red);
        PrefabUtility.SaveAsPrefabAsset(ballItem, "Assets/Prefabs/BallPreviewItem.prefab");
        DestroyImmediate(ballItem);

        // 生成背景预览项预制体
        GameObject backgroundItem = CreatePreviewItem("BackgroundPreviewItem", Color.gray);
        PrefabUtility.SaveAsPrefabAsset(backgroundItem, "Assets/Prefabs/BackgroundPreviewItem.prefab");
        DestroyImmediate(backgroundItem);

        Debug.Log("预览项预制体已生成");
    }

    void GeneratePreviewPanel()
    {
        CreatePrefabDirectory();

        GameObject panelObj = CreatePreviewPanel(null);
        string prefabPath = "Assets/Prefabs/PreviewPanel.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(panelObj, prefabPath);
        
        Debug.Log($"预览面板已生成: {prefabPath}");
        
        DestroyImmediate(panelObj);
        Selection.activeObject = prefab;
    }

    GameObject CreatePreviewPanel(Transform parent)
    {
        GameObject panelObj = new GameObject("PreviewPanel");
        if (parent != null)
        {
            panelObj.transform.SetParent(parent);
        }

        // 添加Image组件作为背景
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // 设置RectTransform
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 创建头部
        GameObject headerObj = CreateHeader(panelObj.transform);

        // 创建内容区域
        GameObject contentObj = CreateContent(panelObj.transform);

        return panelObj;
    }

    GameObject CreateHeader(Transform parent)
    {
        GameObject headerObj = new GameObject("Header");
        headerObj.transform.SetParent(parent);

        // 设置RectTransform
        RectTransform headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.sizeDelta = new Vector2(0, 60);
        headerRect.anchoredPosition = Vector2.zero;

        // 添加背景
        Image headerImage = headerObj.AddComponent<Image>();
        headerImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        // 创建标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(headerObj.transform);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "配置预览";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 18;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, 0);
        titleRect.offsetMax = new Vector2(-120, 0);

        // 创建关闭按钮
        GameObject closeButtonObj = CreateButton(headerObj.transform, "CloseButton", "关闭", new Vector2(1, 0.5f), new Vector2(-10, 0));
        closeButtonObj.GetComponent<Button>().onClick.AddListener(() => {
            if (parent.GetComponent<ConfigPreviewUI>() != null)
            {
                parent.GetComponent<ConfigPreviewUI>().ClosePreview();
            }
        });

        // 创建刷新按钮
        GameObject refreshButtonObj = CreateButton(headerObj.transform, "RefreshButton", "刷新", new Vector2(1, 0.5f), new Vector2(-70, 0));
        refreshButtonObj.GetComponent<Button>().onClick.AddListener(() => {
            if (parent.GetComponent<ConfigPreviewUI>() != null)
            {
                parent.GetComponent<ConfigPreviewUI>().RefreshPreview();
            }
        });

        return headerObj;
    }

    GameObject CreateContent(Transform parent)
    {
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(parent);

        // 设置RectTransform
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(0, 60);
        contentRect.offsetMax = Vector2.zero;

        // 添加VerticalLayoutGroup
        VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // 创建各个内容区域
        CreateContentSection(contentObj.transform, "ShapeSection", "形状类型", Color.blue);
        CreateContentSection(contentObj.transform, "BallSection", "球类型", Color.red);
        CreateContentSection(contentObj.transform, "BackgroundSection", "背景类型", Color.gray);

        return contentObj;
    }

    void CreateContentSection(Transform parent, string name, string title, Color sectionColor)
    {
        GameObject sectionObj = new GameObject(name);
        sectionObj.transform.SetParent(parent);

        // 设置RectTransform
        RectTransform sectionRect = sectionObj.AddComponent<RectTransform>();
        sectionRect.sizeDelta = new Vector2(0, 250);

        // 添加背景
        Image sectionImage = sectionObj.AddComponent<Image>();
        sectionImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // 创建标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(sectionObj.transform);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = title;
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 16;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleLeft;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        titleRect.anchoredPosition = new Vector2(10, -15);

        // 创建滚动视图
        GameObject scrollViewObj = CreateScrollView(sectionObj.transform, title);
    }

    GameObject CreateScrollView(Transform parent, string sectionName)
    {
        GameObject scrollViewObj = new GameObject("ScrollView");
        scrollViewObj.transform.SetParent(parent);

        // 设置RectTransform
        RectTransform scrollRect = scrollViewObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.offsetMin = new Vector2(10, 10);
        scrollRect.offsetMax = new Vector2(-10, -40);

        // 添加ScrollRect组件
        ScrollRect scrollRectComponent = scrollViewObj.AddComponent<ScrollRect>();
        scrollRectComponent.horizontal = false;
        scrollRectComponent.vertical = true;
        scrollRectComponent.scrollSensitivity = 10f;

        // 创建Viewport
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollViewObj.transform);

        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0.3f);

        Mask viewportMask = viewportObj.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        // 创建Content
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform);

        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = new Vector2(5, 0);
        contentRect.offsetMax = new Vector2(-5, 0);

        // 添加ContentSizeFitter
        ContentSizeFitter contentFitter = contentObj.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 添加GridLayoutGroup
        GridLayoutGroup gridLayout = contentObj.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(80, 80);
        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 4;
        gridLayout.childAlignment = TextAnchor.UpperLeft;

        // 设置ScrollRect的content
        scrollRectComponent.content = contentRect;
        scrollRectComponent.viewport = viewportRect;

        return scrollViewObj;
    }

    GameObject CreateButton(Transform parent, string name, string text, Vector2 anchor, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);

        // 添加Image组件
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

        // 添加Button组件
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 0.9f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        button.colors = colors;

        // 设置RectTransform
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchor;
        buttonRect.anchorMax = anchor;
        buttonRect.sizeDelta = new Vector2(50, 30);
        buttonRect.anchoredPosition = position;

        // 创建文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 20;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonObj;
    }

    GameObject CreatePreviewItem(string name, Color color)
    {
        GameObject itemObj = new GameObject(name);
        
        // 添加Image组件
        Image itemImage = itemObj.AddComponent<Image>();
        itemImage.color = color;

        // 设置RectTransform
        RectTransform itemRect = itemObj.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(80, 80);

        return itemObj;
    }

    void CreatePrefabDirectory()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
    }
} 