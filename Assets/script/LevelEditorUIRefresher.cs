using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 关卡编辑器UI刷新器
/// 负责UI的刷新和更新操作
/// </summary>
public class LevelEditorUIRefresher
{
    private LevelEditorUI editorUI;
    private List<GameObject> shapeObjects = new List<GameObject>();
    private List<GameObject> ballObjects = new List<GameObject>();
    
    public LevelEditorUIRefresher(LevelEditorUI editor)
    {
        editorUI = editor;
    }
    
    public void RefreshLayerList()
    {
        if (editorUI.levelListContent)
        {
            foreach (Transform child in editorUI.levelListContent)
            {
                Object.Destroy(child.gameObject);
            }
            
            foreach (LayerData layer in editorUI.currentLevel.layers)
            {
                CreateLayerListItem(layer);
            }
        }
    }
    
    void CreateLayerListItem(LayerData layer)
    {
        if (editorUI.levelListContent)
        {
            GameObject item = new GameObject("LayerItem");
            item.transform.SetParent(editorUI.levelListContent, false);
            
            Button button = item.AddComponent<Button>();
            Text text = item.AddComponent<Text>();
            text.text = layer.layerName;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.color = Color.black;
            
            button.onClick.AddListener(() => SelectLayer(layer));
        }
    }
    
    void SelectLayer(LayerData layer)
    {
        editorUI.currentLayer = layer;
        RefreshEditArea();
    }
    
    public void RefreshEditArea()
    {
        ClearEditArea();
        foreach (ShapeData shape in editorUI.currentLayer.shapes)
        {
            GameObject shapeObj = CreateShapeObject(shape);
            // 为每个shape创建其关联的balls
            foreach (BallData ball in shape.balls)
            {
                CreateBallObject(ball, shapeObj);
            }
        }
    }
    
    void ClearEditArea()
    {
        foreach (GameObject obj in shapeObjects)
        {
            if (obj) Object.Destroy(obj);
        }
        foreach (GameObject obj in ballObjects)
        {
            if (obj) Object.Destroy(obj);
        }
        shapeObjects.Clear();
        ballObjects.Clear();
    }
    
    public GameObject CreateShapeObject(ShapeData shapeData)
    {
        if (editorUI.shapePrefab && editorUI.editAreaContent)
        {
            GameObject shapeObj = Object.Instantiate(editorUI.shapePrefab, editorUI.editAreaContent);
            ShapeController controller = shapeObj.GetComponent<ShapeController>();
            if (controller)
            {
                controller.Initialize(shapeData, editorUI);
            }
            shapeObjects.Add(shapeObj);
            return shapeObj;
        }
        return null;
    }
    
    public GameObject CreateBallObject(BallData ballData, GameObject parentShape = null)
    {
        if (editorUI.ballPrefab && editorUI.editAreaContent)
        {
            // 如果指定了父形状，则将球创建在形状内部，否则创建在编辑区域
            Transform parent = parentShape != null ? parentShape.transform : editorUI.editAreaContent;
            
            GameObject ballObj = Object.Instantiate(editorUI.ballPrefab, parent);
            BallController controller = ballObj.GetComponent<BallController>();
            if (controller)
            {
                controller.Initialize(ballData, editorUI);
            }
            ballObjects.Add(ballObj);
            return ballObj;
        }
        return null;
    }
    
    /// <summary>
    /// 获取最后创建的形状对象
    /// </summary>
    public GameObject GetLastCreatedShapeObject()
    {
        if (shapeObjects.Count > 0)
        {
            return shapeObjects[shapeObjects.Count - 1];
        }
        return null;
    }
} 