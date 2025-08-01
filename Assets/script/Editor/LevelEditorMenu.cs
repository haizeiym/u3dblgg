using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 关卡编辑器菜单
/// 提供一键配置功能，自动创建关卡编辑器UI
/// </summary>
public class LevelEditorMenu
{
    [MenuItem("Tools/Level Editor/一键配置")]
    public static void SetupLevelEditor()
    {
        // 1. 创建或获取Canvas
        Canvas mainCanvas = CreateOrGetCanvas();
        
        // 2. 创建EventSystem（如果不存在）
        CreateEventSystem();
        
        // 3. 创建关卡编辑器UI结构
        GameObject editorUI = CreateLevelEditorUI(mainCanvas);
        
        // 4. 挂载LevelEditorUI脚本
        LevelEditorUI levelEditor = editorUI.GetComponent<LevelEditorUI>();
        if (levelEditor == null)
        {
            levelEditor = editorUI.AddComponent<LevelEditorUI>();
        }
        
        // 5. 确保配置已加载（在UI构建之前）
        LoadConfiguration();
        
        // 6. 创建UI结构
        LevelEditorUIBuilder builder = new LevelEditorUIBuilder(levelEditor);
        builder.CreateUIStructure();
        
        // 7. 延迟初始化默认LevelData，确保Awake()先执行
        EditorApplication.delayCall += () => {
            InitializeDefaultLevelData(levelEditor);
            
            // 注意：事件绑定将在运行时自动调用
            Selection.activeGameObject = editorUI;
            Debug.Log("关卡编辑器配置完成！事件绑定将在运行时自动执行。");
        };
    }
    
    static Canvas CreateOrGetCanvas()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("创建新的Canvas...");
            GameObject canvasObj = new GameObject("LevelEditorCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log($"Canvas创建完成，GraphicRaycaster: {raycaster != null}");
        }
        else
        {
            Debug.Log("使用现有Canvas");
        }
        return canvas;
    }
    
    static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            Debug.Log("创建EventSystem...");
            GameObject eventSystem = new GameObject("EventSystem");
            EventSystem eventSystemComponent = eventSystem.AddComponent<EventSystem>();
            StandaloneInputModule inputModule = eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log($"EventSystem创建完成，EventSystem: {eventSystemComponent != null}, InputModule: {inputModule != null}");
        }
        else
        {
            Debug.Log("使用现有EventSystem");
        }
    }
    
    static GameObject CreateLevelEditorUI(Canvas canvas)
    {
        GameObject editorObj = new GameObject("LevelEditorUI");
        editorObj.transform.SetParent(canvas.transform, false);
        
        RectTransform editorRect = editorObj.AddComponent<RectTransform>();
        editorRect.anchorMin = Vector2.zero;
        editorRect.anchorMax = Vector2.one;
        editorRect.offsetMin = Vector2.zero;
        editorRect.offsetMax = Vector2.zero;
        
        return editorObj;
    }
    
    static void InitializeDefaultLevelData(LevelEditorUI levelEditor)
    {
        // 确保配置已加载
        LoadConfiguration();
        
        // 确保数据已初始化
        if (levelEditor.currentLevel == null)
        {
            levelEditor.currentLevel = new LevelData("新关卡");
        }
        
        // 确保至少有一个层级
        if (levelEditor.currentLevel.layers.Count == 0)
        {
            levelEditor.currentLayer = new LayerData("默认层级");
            levelEditor.currentLevel.layers.Add(levelEditor.currentLayer);
        }
        else if (levelEditor.currentLayer == null)
        {
            // 如果层级列表不为空但当前层级为null，选择第一个层级
            levelEditor.currentLayer = levelEditor.currentLevel.layers[0];
        }
        
        // 刷新UI
        levelEditor.RefreshUI();
    }
    
    /// <summary>
    /// 加载配置
    /// </summary>
    static void LoadConfiguration()
    {
        try
        {
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                // 尝试从文件加载配置
                config.LoadConfigFromFile();
                Debug.Log("一键配置：配置已加载");
                
                // 如果配置为空，初始化默认配置
                if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
                {
                    Debug.Log("一键配置：配置为空，初始化默认配置");
                    config.InitializeDefaultConfig();
                }
                
                Debug.Log($"一键配置：配置加载完成 - 形状: {config.shapeTypes.Count}, 球: {config.ballTypes.Count}, 背景: {config.backgroundConfigs.Count}");
            }
            else
            {
                Debug.LogError("一键配置：无法获取LevelEditorConfig实例");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"一键配置：配置加载失败: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
        }
    }
} 