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
            
            // 可见性切换按钮 - 优化版本
            GameObject visibilityButton = new GameObject("VisibilityButton");
            visibilityButton.transform.SetParent(item.transform, false);
            
            // 添加背景图片组件
            Image visButtonBg = visibilityButton.AddComponent<Image>();
            Button visButton = visibilityButton.AddComponent<Button>();
            
            // 设置按钮背景
            if (layer.isVisible)
            {
                // 显示状态：绿色背景 + 眼睛图标
                visButtonBg.color = new Color(0.2f, 0.8f, 0.2f, 1f); // 鲜艳的绿色
                visButtonBg.sprite = CreateEyeIcon(true); // 创建眼睛图标
            }
            else
            {
                // 隐藏状态：红色背景 + 斜杠眼睛图标
                visButtonBg.color = new Color(0.8f, 0.2f, 0.2f, 1f); // 鲜艳的红色
                visButtonBg.sprite = CreateEyeIcon(false); // 创建斜杠眼睛图标
            }
            
            // 根据层级激活状态调整按钮透明度
            if (!layer.isActive)
            {
                Color currentColor = visButtonBg.color;
                visButtonBg.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f); // 半透明
            }
            
            visButton.onClick.AddListener(() => ToggleLayerVisibility(layer));
            
            // 设置按钮大小
            RectTransform nameRect = nameButton.GetComponent<RectTransform>();
            nameRect.sizeDelta = new Vector2(100, 20);
            
            RectTransform visRect = visibilityButton.GetComponent<RectTransform>();
            visRect.sizeDelta = new Vector2(25, 25); // 正方形按钮，更紧凑
            
            // 为置灰的层级添加背景色
            if (!layer.isActive)
            {
                Image background = item.AddComponent<Image>();
                background.color = new Color(0.9f, 0.9f, 0.9f, 0.3f); // 浅灰色背景
                background.raycastTarget = false; // 不阻挡点击事件
            }
        }
    }
    
    /// <summary>
    /// 创建眼睛图标精灵
    /// </summary>
    Sprite CreateEyeIcon(bool isVisible)
    {
        // 创建32x32的纹理
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        // 默认透明背景
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        if (isVisible)
        {
            // 绘制眼睛图标（显示状态）
            DrawEyeIcon(pixels, Color.white);
        }
        else
        {
            // 绘制斜杠眼睛图标（隐藏状态）
            DrawEyeIcon(pixels, Color.white);
            DrawSlashIcon(pixels, Color.red);
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // 创建精灵
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    
    /// <summary>
    /// 绘制眼睛图标
    /// </summary>
    void DrawEyeIcon(Color[] pixels, Color color)
    {
        // 眼睛轮廓（椭圆形）
        for (int y = 8; y < 24; y++)
        {
            for (int x = 6; x < 26; x++)
            {
                float dx = (x - 16) / 10f;
                float dy = (y - 16) / 8f;
                if (dx * dx + dy * dy <= 1f)
                {
                    int index = y * 32 + x;
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = color;
                    }
                }
            }
        }
        
        // 眼睛瞳孔（圆形）
        for (int y = 12; y < 20; y++)
        {
            for (int x = 12; x < 20; x++)
            {
                float dx = (x - 16) / 4f;
                float dy = (y - 16) / 4f;
                if (dx * dx + dy * dy <= 1f)
                {
                    int index = y * 32 + x;
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = Color.black;
                    }
                }
            }
        }
        
        // 眼睛高光（小圆点）
        for (int y = 13; y < 16; y++)
        {
            for (int x = 13; x < 16; x++)
            {
                float dx = (x - 14.5f) / 1.5f;
                float dy = (y - 14.5f) / 1.5f;
                if (dx * dx + dy * dy <= 1f)
                {
                    int index = y * 32 + x;
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = Color.white;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 绘制斜杠图标
    /// </summary>
    void DrawSlashIcon(Color[] pixels, Color color)
    {
        // 绘制从左上到右下的斜杠
        for (int i = 0; i < 32; i++)
        {
            int x = i;
            int y = 31 - i;
            
            // 绘制粗斜杠（3像素宽）
            for (int offset = -1; offset <= 1; offset++)
            {
                int drawX = x + offset;
                int drawY = y + offset;
                
                if (drawX >= 0 && drawX < 32 && drawY >= 0 && drawY < 32)
                {
                    int index = drawY * 32 + drawX;
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = color;
                    }
                }
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
        // 保存当前选中的对象信息
        ShapeData selectedShapeData = editorUI.selectedShape?.ShapeData;
        BallData selectedBallData = editorUI.selectedBall?.BallData;
        
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
        
        // 恢复选中状态
        RestoreSelection(selectedShapeData, selectedBallData);
        
        Debug.Log($"刷新编辑区完成，当前激活层级: {editorUI.currentLayer?.layerName}");
    }
    
    /// <summary>
    /// 恢复选中状态
    /// </summary>
    private void RestoreSelection(ShapeData selectedShapeData, BallData selectedBallData)
    {
        if (selectedShapeData != null)
        {
            // 查找对应的形状对象并重新选中
            foreach (GameObject shapeObj in shapeObjects)
            {
                ShapeController controller = shapeObj.GetComponent<ShapeController>();
                if (controller != null && controller.ShapeData != null)
                {
                    // 使用形状类型和位置来匹配，而不是引用比较
                    ShapeData currentShapeData = controller.ShapeData;
                    if (currentShapeData.shapeType == selectedShapeData.shapeType &&
                        Vector2.Distance(currentShapeData.position, selectedShapeData.position) < 0.1f &&
                        Mathf.Abs(currentShapeData.rotation - selectedShapeData.rotation) < 0.1f)
                    {
                        editorUI.SelectShape(controller);
                        Debug.Log($"已恢复形状选中状态: {selectedShapeData.shapeType} (位置: {selectedShapeData.position})");
                        break;
                    }
                }
            }
        }
        
        if (selectedBallData != null)
        {
            // 查找对应的球对象并重新选中
            foreach (GameObject ballObj in ballObjects)
            {
                BallController controller = ballObj.GetComponent<BallController>();
                if (controller != null && controller.BallData != null)
                {
                    // 使用球类型和位置来匹配，而不是引用比较
                    BallData currentBallData = controller.BallData;
                    if (currentBallData.ballType == selectedBallData.ballType &&
                        Vector2.Distance(currentBallData.position, selectedBallData.position) < 0.1f)
                    {
                        editorUI.SelectBall(controller);
                        Debug.Log($"已恢复球选中状态: {selectedBallData.ballType} (位置: {selectedBallData.position})");
                        break;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 刷新指定层级的可见性
    /// </summary>
    public void RefreshLayerVisibility(LayerData layer)
    {
        // 保存当前选中的对象信息
        ShapeData selectedShapeData = editorUI.selectedShape?.ShapeData;
        BallData selectedBallData = editorUI.selectedBall?.BallData;
        
        // 更新层级列表中的按钮外观
        UpdateLayerVisibilityButton(layer);
        
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
        
        // 恢复选中状态
        RestoreSelection(selectedShapeData, selectedBallData);
    }
    
    /// <summary>
    /// 更新层级可见性按钮的外观
    /// </summary>
    void UpdateLayerVisibilityButton(LayerData layer)
    {
        if (editorUI.levelListContent == null) return;
        
        // 查找对应的层级列表项
        foreach (Transform child in editorUI.levelListContent)
        {
            // 查找包含该层级名称的按钮
            Button nameButton = child.GetComponentInChildren<Button>();
            if (nameButton != null)
            {
                Text nameText = nameButton.GetComponent<Text>();
                if (nameText != null && nameText.text == layer.layerName)
                {
                    // 找到对应的层级项，更新其可见性按钮
                    Button visButton = child.Find("VisibilityButton")?.GetComponent<Button>();
                    if (visButton != null)
                    {
                        Image visButtonBg = visButton.GetComponent<Image>();
                        if (visButtonBg != null)
                        {
                            // 更新按钮背景和图标
                            if (layer.isVisible)
                            {
                                // 显示状态：绿色背景 + 眼睛图标
                                visButtonBg.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                                visButtonBg.sprite = CreateEyeIcon(true);
                            }
                            else
                            {
                                // 隐藏状态：红色背景 + 斜杠眼睛图标
                                visButtonBg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
                                visButtonBg.sprite = CreateEyeIcon(false);
                            }
                            
                            // 根据层级激活状态调整按钮透明度
                            if (!layer.isActive)
                            {
                                Color currentColor = visButtonBg.color;
                                visButtonBg.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
                            }
                        }
                    }
                    break;
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
        try
        {
            if (editorUI.shapePrefab == null)
            {
                Debug.LogError("shapePrefab为空，无法创建形状对象");
                return null;
            }
            
            if (editorUI.editAreaContent == null)
            {
                Debug.LogError("editAreaContent为空，无法创建形状对象");
                return null;
            }
            
            if (shapeData == null)
            {
                Debug.LogError("shapeData为空，无法创建形状对象");
                return null;
            }
            
            Debug.Log($"开始创建形状对象: {shapeData.shapeType}");
            
            GameObject shapeObj = Object.Instantiate(editorUI.shapePrefab, editorUI.editAreaContent);
            if (shapeObj == null)
            {
                Debug.LogError("Instantiate返回null，创建形状对象失败");
                return null;
            }
            
            ShapeController controller = shapeObj.GetComponent<ShapeController>();
            if (controller == null)
            {
                Debug.LogError("新创建的形状对象没有ShapeController组件");
                Object.DestroyImmediate(shapeObj);
                return null;
            }
            
            controller.Initialize(shapeData, editorUI);
            
            // 确保新创建的对象在UI层级的最前面（最上层）
            shapeObj.transform.SetAsLastSibling();
            
            shapeObjects.Add(shapeObj);
            Debug.Log($"形状对象创建成功: {shapeData.shapeType}");
            return shapeObj;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CreateShapeObject异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
            return null;
        }
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
    
    /// <summary>
    /// 获取所有形状对象
    /// </summary>
    public List<GameObject> GetAllShapeObjects()
    {
        return new List<GameObject>(shapeObjects);
    }
} 