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
    }
    
    Sprite CreateCircleSprite()
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
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = rectTransform.anchoredPosition - eventData.position;
        SetSelected(true);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
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
} 