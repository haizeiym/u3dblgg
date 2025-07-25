using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背景管理器
/// 负责管理编辑区的背景显示
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [Header("背景组件")]
    public Image backgroundImage;
    
    private LevelEditorUI editorUI;
    
    void Start()
    {
        editorUI = FindObjectOfType<LevelEditorUI>();
        if (editorUI != null)
        {
            // 确保背景图片组件存在
            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }
            
            // 确保配置已加载（只在首次启动时）
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                // 只在配置为空时才重新加载，避免重置levelIndex
                if (config.shapeTypes.Count == 0 && config.ballTypes.Count == 0)
                {
                    Debug.Log("BackgroundManager: 配置为空，重新加载");
                    config.LoadConfigFromFile();
                }
                else
                {
                    Debug.Log("BackgroundManager: 配置已存在，跳过重新加载");
                }
                config.RefreshBackgroundSprites();
            }
            
            // 应用初始背景
            ApplyBackground();
        }
    }
    
    void Update()
    {
        // 监听背景变化
        if (editorUI != null)
        {
            var config = LevelEditorConfig.Instance;
            if (config != null && config.currentBackgroundIndex != editorUI.currentBackgroundIndex)
            {
                editorUI.currentBackgroundIndex = config.currentBackgroundIndex;
                ApplyBackground();
            }
        }
    }
    
    /// <summary>
    /// 应用背景配置
    /// </summary>
    public void ApplyBackground()
    {
        if (backgroundImage == null) return;
        
        var config = LevelEditorConfig.Instance;
        if (config == null) return;
        
        var backgroundConfig = config.GetCurrentBackground();
        if (backgroundConfig != null)
        {
            Debug.Log($"应用背景: {backgroundConfig.name}, useSprite: {backgroundConfig.useSprite}");
            
            if (backgroundConfig.useSprite && backgroundConfig.backgroundSprite != null)
            {
                backgroundImage.sprite = backgroundConfig.backgroundSprite;
                backgroundImage.color = Color.white;
                
                // 设置精灵的缩放和偏移
                backgroundImage.rectTransform.sizeDelta = backgroundConfig.backgroundSprite.rect.size * backgroundConfig.spriteScale;
                backgroundImage.rectTransform.anchoredPosition = backgroundConfig.spriteOffset;
                
                Debug.Log($"应用精灵背景: {backgroundConfig.backgroundSprite.name}, 大小: {backgroundConfig.backgroundSprite.rect.size}");
            }
            else
            {
                backgroundImage.sprite = null;
                backgroundImage.color = backgroundConfig.backgroundColor;
                Debug.Log($"应用颜色背景: {backgroundConfig.backgroundColor}");
            }
        }
        else
        {
            Debug.LogWarning("未找到背景配置");
        }
    }
    
    /// <summary>
    /// 切换背景
    /// </summary>
    public void SwitchBackground()
    {
        if (editorUI != null)
        {
            editorUI.SwitchBackground();
        }
    }
    
    /// <summary>
    /// 测试背景配置加载
    /// </summary>
    [ContextMenu("测试背景配置")]
    public void TestBackgroundConfig()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log($"背景配置数量: {config.backgroundConfigs.Count}");
            for (int i = 0; i < config.backgroundConfigs.Count; i++)
            {
                var bg = config.backgroundConfigs[i];
                Debug.Log($"背景 {i}: {bg.name}, useSprite: {bg.useSprite}, sprite: {(bg.backgroundSprite != null ? bg.backgroundSprite.name : "null")}, path: {bg.spritePath}");
            }
        }
    }
} 