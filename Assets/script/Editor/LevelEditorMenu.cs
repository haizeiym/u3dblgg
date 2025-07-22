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
        
        // 5. 创建UI结构
        LevelEditorUIBuilder builder = new LevelEditorUIBuilder(levelEditor);
        builder.CreateUIStructure();
        
        // 6. 延迟初始化默认LevelData，确保Awake()先执行
        EditorApplication.delayCall += () => {
            InitializeDefaultLevelData(levelEditor);
            Selection.activeGameObject = editorUI;
            Debug.Log("关卡编辑器配置完成！");
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
        // 确保数据已初始化
        if (levelEditor.currentLevel == null)
        {
            levelEditor.currentLevel = new LevelData("新关卡");
            levelEditor.currentLayer = new LayerData("默认层级");
            levelEditor.currentLevel.layers.Add(levelEditor.currentLayer);
        }
        
        // 刷新UI
        levelEditor.RefreshUI();
    }
} 