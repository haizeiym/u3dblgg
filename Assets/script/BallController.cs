using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Image image;
    private BallData ballData;
    private LevelEditorUI editorUI;
    private bool isSelected = false;
    private Vector2 dragOffset;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
        }
        
        SetupDefaultAppearance();
    }
    
    public void Initialize(BallData data, LevelEditorUI ui)
    {
        ballData = data;
        editorUI = ui;
        UpdateVisual();
    }
    
    void SetupDefaultAppearance()
    {
        if (image)
        {
            image.color = Color.red;
            image.raycastTarget = true;
        }
        
        if (rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(30, 30);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        }
    }
    
    public void UpdateVisual()
    {
        if (ballData != null && rectTransform != null)
        {
            rectTransform.anchoredPosition = ballData.position;
            UpdateBallAppearance();
        }
    }
    
    void UpdateBallAppearance()
    {
        if (image == null || ballData == null) return;
        
        try
        {
            // 从配置中获取球配置
            BallType config = LevelEditorConfig.Instance.GetBallConfig(ballData.ballType);
            if (config != null)
            {
                // 使用配置中的颜色
                image.color = config.color;
                
                if (config.sprite != null)
                {
                    // 使用配置中的自定义图片
                    image.sprite = config.sprite;
                    // 设置图片大小为原始尺寸
                    if (config.sprite.rect.width > 0 && config.sprite.rect.height > 0)
                    {
                        image.rectTransform.sizeDelta = new Vector2(config.sprite.rect.width, config.sprite.rect.height);
                    }
                }
                else
                {
                    // 使用默认圆形精灵
                    image.sprite = CreateCircleSprite();
                }
            }
            else
            {
                // 使用默认颜色和精灵
                SetDefaultBallAppearance();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UpdateBallAppearance异常: {e.Message}");
            SetDefaultBallAppearance();
        }
    }
    
    /// <summary>
    /// 设置默认球外观
    /// </summary>
    void SetDefaultBallAppearance()
    {
        if (image == null || ballData == null) return;
        
        try
        {
            switch (ballData.ballType)
            {
                case "红球":
                    image.color = Color.red;
                    break;
                case "蓝球":
                    image.color = Color.blue;
                    break;
                case "绿球":
                    image.color = Color.green;
                    break;
                case "黄球":
                    image.color = Color.yellow;
                    break;
                default:
                    image.color = Color.red;
                    break;
            }
            
            image.sprite = CreateCircleSprite();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SetDefaultBallAppearance异常: {e.Message}");
            // 发生异常时使用最简单的设置
            if (image != null)
            {
                image.color = Color.red;
                image.sprite = null; // 不使用纹理，避免卡死
            }
        }
    }
    
    Sprite CreateCircleSprite()
    {
        try
        {
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            
            Vector2 center = new Vector2(16, 16);
            float radius = 14;
            
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * 32 + x] = distance <= radius ? Color.white : Color.clear;
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CreateCircleSprite异常: {e.Message}");
            return null;
        }
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
        
        if (ballData != null)
        {
            ballData.position = newPosition;
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
            editorUI.SelectBall(this);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (image != null)
        {
            if (selected)
            {
                // 添加高亮边框效果
                Color highlightColor = image.color;
                highlightColor.a = 1f;
                image.color = highlightColor;
            }
            else
            {
                // 恢复正常透明度
                Color normalColor = image.color;
                normalColor.a = 0.8f;
                image.color = normalColor;
            }
        }
    }
    
    public void SetPosition(Vector2 position)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }
        
        if (ballData != null)
        {
            ballData.position = position;
        }
    }
    
    public BallData GetBallData()
    {
        return ballData;
    }
    
    // 添加公共属性以便外部访问
    public BallData BallData
    {
        get { return ballData; }
        set { ballData = value; }
    }
} 