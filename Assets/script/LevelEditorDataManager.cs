using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡编辑器数据管理器
/// 处理关卡数据相关的逻辑
/// </summary>
public class LevelEditorDataManager
{
    private LevelEditorUI editorUI;
    private LevelEditorUIManager uiManager;
    
    public LevelEditorDataManager(LevelEditorUI editor)
    {
        editorUI = editor;
        uiManager = new LevelEditorUIManager(editor);
        InitializeLevel();
    }
    
    void InitializeLevel()
    {
        if (editorUI.currentLevel == null)
        {
            editorUI.currentLevel = new LevelData("新关卡");
            editorUI.currentLayer = new LayerData("层级1");
            editorUI.currentLevel.layers.Add(editorUI.currentLayer);
        }
    }
    
    public void AddLayer()
    {
        LayerData newLayer = new LayerData($"层级{editorUI.currentLevel.layers.Count + 1}");
        editorUI.currentLevel.layers.Add(newLayer);
        editorUI.currentLayer = newLayer;
        editorUI.RefreshUI();
        ClearEditArea();
    }
    
    public void DeleteLayer()
    {
        if (editorUI.currentLevel.layers.Count > 1)
        {
            editorUI.currentLevel.layers.Remove(editorUI.currentLayer);
            editorUI.currentLayer = editorUI.currentLevel.layers[0];
            editorUI.RefreshUI();
        }
    }
    
    public void AddShape()
    {
        if (editorUI.currentLayer != null)
        {
            string[] types = LevelEditorConfig.Instance.GetShapeTypeNames();
            if (editorUI.currentShapeTypeIndex >= 0 && editorUI.currentShapeTypeIndex < types.Length)
            {
                string shapeType = types[editorUI.currentShapeTypeIndex];
                ShapeData newShape = new ShapeData(shapeType, Vector2.zero, 0f);
                editorUI.currentLayer.shapes.Add(newShape);
                GameObject newShapeObj = uiManager.CreateShapeObject(newShape);
                
                // 自动选中新创建的形状
                if (newShapeObj != null)
                {
                    ShapeController controller = newShapeObj.GetComponent<ShapeController>();
                    if (controller != null)
                    {
                        editorUI.SelectShape(controller);
                        Debug.Log($"成功创建形状: {shapeType}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"形状类型索引超出范围: {editorUI.currentShapeTypeIndex}");
            }
        }
        else
        {
            Debug.LogWarning("当前没有可用的层级");
        }
    }
    
    public void AddBall()
    {
        if (editorUI.selectedShape != null)
        {
            ShapeData shapeData = editorUI.selectedShape.GetShapeData();
            if (shapeData != null)
            {
                BallData newBall = new BallData("红球", Vector2.zero);
                shapeData.balls.Add(newBall);
                
                // 将球创建在选中的形状内部
                GameObject ballObj = uiManager.CreateBallObject(newBall);
                if (ballObj != null)
                {
                    // 将球设置为形状的子对象
                    ballObj.transform.SetParent(editorUI.selectedShape.transform, false);
                    
                    Debug.Log($"成功添加球到形状: {shapeData.shapeType}");
                }
                else
                {
                    Debug.LogError("创建球对象失败");
                }
            }
        }
        else
        {
            Debug.LogWarning("请先选择一个形状，然后点击添加球");
        }
    }
    
    public void ExportLevel()
    {
        string json = LevelDataExporter.SaveToJson(editorUI.currentLevel);
        Debug.Log("导出的JSON数据：\n" + json);
        
        // 复制到剪贴板
        GUIUtility.systemCopyBuffer = json;
        Debug.Log("关卡数据已复制到剪贴板");
    }
    
    void ClearEditArea()
    {
        // 清除编辑区域的所有对象
        if (editorUI.editAreaContent)
        {
            foreach (Transform child in editorUI.editAreaContent)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
} 