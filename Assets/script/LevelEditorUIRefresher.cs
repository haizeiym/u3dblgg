using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
            
            // 反转顺序显示，确保最新添加的层级在最上面
            for (int i = editorUI.currentLevel.layers.Count - 1; i >= 0; i--)
            {
                LayerData layer = editorUI.currentLevel.layers[i];
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
            
            // 创建水平布局
            HorizontalLayoutGroup layout = item.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleLeft;
            
            // 确保新项目添加到列表顶部
            item.transform.SetAsFirstSibling();
            
            // 层级名称按钮
            GameObject nameButton = new GameObject("NameButton");
            nameButton.transform.SetParent(item.transform, false);
            
            Button button = nameButton.AddComponent<Button>();
            Text text = nameButton.AddComponent<Text>();
            text.text = layer.layerName;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            
            // 根据层级状态设置颜色和透明度
            if (layer.isActive)
            {
                text.color = Color.black;
                text.fontStyle = FontStyle.Bold;
            }
            else
            {
                text.color = new Color(0.5f, 0.5f, 0.5f, 0.6f); // 更明显的灰色，带透明度
                text.fontStyle = FontStyle.Normal;
            }
            
            button.onClick.AddListener(() => SelectLayer(layer));
            
            // 可见性切换按钮
            GameObject visibilityButton = new GameObject("VisibilityButton");
            visibilityButton.transform.SetParent(item.transform, false);
            
            Button visButton = visibilityButton.AddComponent<Button>();
            Text visText = visibilityButton.AddComponent<Text>();
            visText.text = layer.isVisible ? "显示" : "隐藏";
            visText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            visText.fontSize = 30;
            
            // 根据层级激活状态调整可见性按钮的透明度
            if (layer.isActive)
            {
                visText.color = layer.isVisible ? Color.green : Color.gray;
            }
            else
            {
                visText.color = layer.isVisible ? 
                    new Color(0, 0.5f, 0, 0.4f) : // 半透明绿色
                    new Color(0.5f, 0.5f, 0.5f, 0.4f); // 半透明灰色
            }
            
            visButton.onClick.AddListener(() => ToggleLayerVisibility(layer));
            
            // 设置按钮大小
            RectTransform nameRect = nameButton.GetComponent<RectTransform>();
            nameRect.sizeDelta = new Vector2(100, 20);
            
            RectTransform visRect = visibilityButton.GetComponent<RectTransform>();
            visRect.sizeDelta = new Vector2(60, 20);
            
            // 为置灰的层级添加背景色
            if (!layer.isActive)
            {
                Image background = item.AddComponent<Image>();
                background.color = new Color(0.9f, 0.9f, 0.9f, 0.3f); // 浅灰色背景
                background.raycastTarget = false; // 不阻挡点击事件
            }
        }
    }
    
    void ToggleLayerVisibility(LayerData layer)
    {
        layer.isVisible = !layer.isVisible;
        RefreshLayerVisibility(layer); // 使用专门的层级可见性刷新方法
    }
    
    /// <summary>
    /// 激活指定层级（用于外部调用）
    /// </summary>
    public void ActivateLayer(LayerData layer)
    {
        SelectLayer(layer);
    }
    
    void SelectLayer(LayerData layer)
    {
        // 将所有层级置灰
        foreach (var l in editorUI.currentLevel.layers)
        {
            l.isActive = false;
        }
        
        // 激活选中的层级
        layer.isActive = true;
        editorUI.currentLayer = layer;
        
        // 刷新UI以更新颜色显示
        RefreshLayerList();
        RefreshEditArea();
        
        Debug.Log($"层级已激活: {layer.layerName}, 激活状态: {layer.isActive}");
    }
    
    public void RefreshEditArea()
    {
        ClearEditArea();
        
        // 显示所有可见层级的内容（按层级顺序，最新的在最上层）
        for (int i = 0; i < editorUI.currentLevel.layers.Count; i++)
        {
            LayerData layer = editorUI.currentLevel.layers[i];
            if (layer.isVisible)
            {
                foreach (ShapeData shape in layer.shapes)
                {
                    GameObject shapeObj = CreateShapeObject(shape);
                    
                    // 根据层级激活状态设置编辑权限
                    if (layer.isActive)
                    {
                        EnableObjectEditing(shapeObj);
                    }
                    else
                    {
                        DisableObjectEditing(shapeObj);
                    }
                    
                    // 为每个shape创建其关联的balls
                    foreach (BallData ball in shape.balls)
                    {
                        GameObject ballObj = CreateBallObject(ball, shapeObj);
                        
                        // 根据层级激活状态设置球的编辑权限
                        if (layer.isActive)
                        {
                            EnableObjectEditing(ballObj);
                        }
                        else
                        {
                            DisableObjectEditing(ballObj);
                        }
                    }
                }
            }
        }
        
        Debug.Log($"刷新编辑区完成，当前激活层级: {editorUI.currentLayer?.layerName}");
    }
    
    /// <summary>
    /// 刷新指定层级的可见性
    /// </summary>
    public void RefreshLayerVisibility(LayerData layer)
    {
        // 清除编辑区
        ClearEditArea();
        
        // 重新显示所有可见层级的内容，确保层级顺序正确
        for (int i = 0; i < editorUI.currentLevel.layers.Count; i++)
        {
            LayerData currentLayer = editorUI.currentLevel.layers[i];
            if (currentLayer.isVisible)
            {
                foreach (ShapeData shape in currentLayer.shapes)
                {
                    GameObject shapeObj = CreateShapeObject(shape);
                    
                    // 根据层级激活状态设置编辑权限
                    if (currentLayer.isActive)
                    {
                        EnableObjectEditing(shapeObj);
                    }
                    else
                    {
                        DisableObjectEditing(shapeObj);
                    }
                    
                    // 为每个shape创建其关联的balls
                    foreach (BallData ball in shape.balls)
                    {
                        GameObject ballObj = CreateBallObject(ball, shapeObj);
                        
                        // 根据层级激活状态设置球的编辑权限
                        if (currentLayer.isActive)
                        {
                            EnableObjectEditing(ballObj);
                        }
                        else
                        {
                            DisableObjectEditing(ballObj);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 禁用对象的编辑功能
    /// </summary>
    void DisableObjectEditing(GameObject obj)
    {
        // 禁用拖拽接口实现
        var dragHandlers = obj.GetComponents<MonoBehaviour>();
        foreach (var handler in dragHandlers)
        {
            if (handler is IBeginDragHandler || handler is IDragHandler || handler is IEndDragHandler)
            {
                handler.enabled = false;
            }
        }
        
        // 禁用点击事件
        Button button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.enabled = false;
        }
        
        // 禁用图像射线检测，防止阻挡下层对象的交互
        Image image = obj.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = false; // 禁用射线检测
            Color color = image.color;
            color.a = 0.5f; // 设置透明度为50%
            image.color = color;
        }
        
        // 禁用所有子对象的交互
        foreach (Transform child in obj.transform)
        {
            DisableObjectEditing(child.gameObject);
        }
    }
    
    /// <summary>
    /// 启用对象的编辑功能
    /// </summary>
    void EnableObjectEditing(GameObject obj)
    {
        // 启用拖拽接口实现
        var dragHandlers = obj.GetComponents<MonoBehaviour>();
        foreach (var handler in dragHandlers)
        {
            if (handler is IBeginDragHandler || handler is IDragHandler || handler is IEndDragHandler)
            {
                handler.enabled = true;
            }
        }
        
        // 启用点击事件
        Button button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.enabled = true;
        }
        
        // 启用图像射线检测
        Image image = obj.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true; // 启用射线检测
            Color color = image.color;
            color.a = 1.0f; // 设置透明度为100%
            image.color = color;
        }
        
        // 启用所有子对象的交互
        foreach (Transform child in obj.transform)
        {
            EnableObjectEditing(child.gameObject);
        }
    }
    
    void ClearEditArea()
    {
        // 清理所有形状对象（包括其子对象）
        foreach (GameObject obj in shapeObjects)
        {
            if (obj) 
            {
                // 销毁形状及其所有子对象
                Object.DestroyImmediate(obj);
            }
        }
        
        // 清理独立的球对象
        foreach (GameObject obj in ballObjects)
        {
            if (obj && obj.transform.parent == editorUI.editAreaContent) 
            {
                // 只销毁直接位于编辑区的球对象
                Object.DestroyImmediate(obj);
            }
        }
        
        shapeObjects.Clear();
        ballObjects.Clear();
        
        // 额外清理：确保编辑区没有任何残留对象
        if (editorUI.editAreaContent)
        {
            for (int i = editorUI.editAreaContent.childCount - 1; i >= 0; i--)
            {
                Transform child = editorUI.editAreaContent.GetChild(i);
                if (child != null)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
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
            
            // 确保新创建的对象在UI层级的最前面（最上层）
            shapeObj.transform.SetAsLastSibling();
            
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
            
            // 确保球对象在正确的层级位置
            if (parentShape == null)
            {
                // 独立的球对象放在最上层
                ballObj.transform.SetAsLastSibling();
                ballObjects.Add(ballObj);
            }
            else
            {
                // 依附于形状的球对象放在形状内部的最上层
                ballObj.transform.SetAsLastSibling();
            }
            
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