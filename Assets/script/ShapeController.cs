using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 形状控制器
/// 负责形状的显示、交互和数据管理
/// </summary>
public class ShapeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Image image;
    private ShapeData shapeData;
    private LevelEditorUI editorUI;
    private bool isSelected = false;
    private Vector2 dragOffset;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();
        
        if (image == null)
            image = gameObject.AddComponent<Image>();
        
        SetupDefaultAppearance();
    }
    
    public void Initialize(ShapeData data, LevelEditorUI ui)
    {
        shapeData = data;
        editorUI = ui;
        UpdateVisual();
    }
    
    void SetupDefaultAppearance()
    {
        if (image != null)
        {
            image.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            // 不生成纹理，避免卡死
            image.sprite = null;
        }
        
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(50, 50);
        }
    }
    
    void UpdateShapeAppearance()
    {
        if (image == null)
        {
            Debug.LogWarning("ShapeController: image组件为空");
            return;
        }
        
        if (shapeData == null)
        {
            Debug.LogWarning("ShapeController: shapeData为空");
            return;
        }
        
        Debug.Log($"更新形状外观: {shapeData.shapeType}");
        
        try
        {
            // 从配置中获取形状配置
            ShapeType config = LevelEditorConfig.Instance.GetShapeConfig(shapeData.shapeType);
            if (config != null)
            {
                if (config.sprite != null)
                {
                    // 使用配置中的自定义图片
                    image.sprite = config.sprite;
                    // 设置图片大小为原始尺寸
                    if (config.sprite.rect.width > 0 && config.sprite.rect.height > 0)
                    {
                        image.rectTransform.sizeDelta = new Vector2(config.sprite.rect.width, config.sprite.rect.height);
                    }
                    Debug.Log($"使用配置图片: {config.name}");
                }
                else
                {
                    // 使用默认生成的图片（添加异常处理）
                    SetDefaultShapeSprite(shapeData.shapeType);
                }
            }
            else
            {
                Debug.LogWarning($"未找到形状配置: {shapeData.shapeType}，使用默认圆形");
                SetDefaultShapeSprite("圆形");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UpdateShapeAppearance异常: {e.Message}");
            // 发生异常时使用最简单的默认设置
            SetSimpleDefaultAppearance();
        }
    }
    
    /// <summary>
    /// 设置默认形状精灵（带异常处理）
    /// </summary>
    void SetDefaultShapeSprite(string shapeType)
    {
        try
        {
            switch (shapeType)
            {
                case "圆形":
                    image.sprite = ShapeSpriteGenerator.CreateCircleSprite();
                    Debug.Log("使用默认圆形精灵");
                    break;
                case "矩形":
                    image.sprite = ShapeSpriteGenerator.CreateRectangleSprite();
                    Debug.Log("使用默认矩形精灵");
                    break;
                case "三角形":
                    image.sprite = ShapeSpriteGenerator.CreateTriangleSprite();
                    Debug.Log("使用默认三角形精灵");
                    break;
                case "菱形":
                    image.sprite = ShapeSpriteGenerator.CreateDiamondSprite();
                    Debug.Log("使用默认菱形精灵");
                    break;
                default:
                    Debug.LogWarning($"未知的形状类型: {shapeType}，使用默认圆形");
                    image.sprite = ShapeSpriteGenerator.CreateCircleSprite();
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SetDefaultShapeSprite异常: {e.Message}");
            SetSimpleDefaultAppearance();
        }
    }
    
    /// <summary>
    /// 设置简单的默认外观（不生成纹理，避免卡死）
    /// </summary>
    void SetSimpleDefaultAppearance()
    {
        if (image != null)
        {
            // 使用纯色，不生成纹理
            image.sprite = null;
            image.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        }
        
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(50, 50);
        }
        
        Debug.Log("使用简单默认外观（避免纹理生成卡死）");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 检查当前层级是否激活
        if (editorUI != null && editorUI.currentLayer != null && !editorUI.currentLayer.isActive)
        {
            Debug.LogWarning("无法拖拽：当前层级未激活");
            return;
        }
        
        dragOffset = rectTransform.anchoredPosition - eventData.position;
        SetSelected(true);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 检查当前层级是否激活
        if (editorUI != null && editorUI.currentLayer != null && !editorUI.currentLayer.isActive)
        {
            return;
        }
        
        Vector2 newPosition = eventData.position + dragOffset;
        rectTransform.anchoredPosition = newPosition;
        
        if (shapeData != null)
        {
            shapeData.position = newPosition;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // 拖拽结束时的处理
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查当前层级是否激活
        if (editorUI != null && editorUI.currentLayer != null && !editorUI.currentLayer.isActive)
        {
            Debug.LogWarning("无法选择：当前层级未激活");
            return;
        }
        
        SetSelected(true);
        if (editorUI != null)
        {
            editorUI.SelectShape(this);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (image != null)
        {
            if (selected)
            {
                image.color = new Color(1f, 1f, 0f, 0.8f); // 黄色高亮
            }
            else
            {
                image.color = new Color(0.8f, 0.8f, 0.8f, 0.8f); // 默认颜色
            }
        }
    }
    
    public void SetRotation(float rotation)
    {
        if (rectTransform != null)
        {
            rectTransform.localEulerAngles = new Vector3(0, 0, rotation);
        }
        
        if (shapeData != null)
        {
            shapeData.rotation = rotation;
        }
    }
    
    public void SetPosition(Vector2 position)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }
        
        if (shapeData != null)
        {
            shapeData.position = position;
        }
    }
    
    public ShapeData GetShapeData()
    {
        return shapeData;
    }
    
    // 添加公共属性以便外部访问
    public ShapeData ShapeData
    {
        get { return shapeData; }
        set { shapeData = value; }
    }
    
    public void UpdateVisual()
    {
        if (shapeData != null && rectTransform != null)
        {
            rectTransform.anchoredPosition = shapeData.position;
            rectTransform.localEulerAngles = new Vector3(0, 0, shapeData.rotation);
            UpdateShapeAppearance();
        }
    }
} 