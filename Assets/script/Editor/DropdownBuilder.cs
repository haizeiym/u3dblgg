using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dropdown配置数据
/// </summary>
[System.Serializable]
public class DropdownConfig
{
    public string label;
    public string[] options;
    public string defaultText;
    
    public DropdownConfig(string label, string[] options, string defaultText = "请选择")
    {
        this.label = label;
        this.options = options;
        this.defaultText = defaultText;
    }
}

/// <summary>
/// Dropdown构建器
/// 专门负责创建Dropdown组件
/// </summary>
public class DropdownBuilder
{
    public static GameObject CreateDropdown(GameObject parent, string label, Vector2 position)
    {
        return CreateDropdown(parent, label, position, null);
    }
    
    public static GameObject CreateDropdown(GameObject parent, string label, Vector2 position, string[] options)
    {
        GameObject dropdown = new GameObject(label + "Dropdown");
        dropdown.transform.SetParent(parent.transform, false);
        
        RectTransform dropdownRect = dropdown.AddComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(position.x, position.y);
        dropdownRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.05f);
        dropdownRect.offsetMin = Vector2.zero;
        dropdownRect.offsetMax = Vector2.zero;
        
        Image dropdownBg = dropdown.AddComponent<Image>();
        dropdownBg.color = Color.white;
        
        // 先创建模板，再创建Dropdown组件
        RectTransform templateRect = CreateDropdownTemplate(dropdown);
        var (itemImage, itemText) = CreateDropdownItem(dropdown, templateRect);
        
        Dropdown dropdownComponent = dropdown.AddComponent<Dropdown>();
        dropdownComponent.targetGraphic = dropdownBg;
        dropdownComponent.template = templateRect;
        dropdownComponent.itemText = itemText;
        dropdownComponent.itemImage = itemImage; // 关键修正
        
        CreateDropdownLabel(dropdown, dropdownComponent);
        CreateDropdownArrow(dropdown, dropdownComponent);
        
        // 确保Dropdown组件正确初始化
        dropdownComponent.RefreshShownValue();
        
        // 如果提供了选项，则配置选项
        if (options != null && options.Length > 0)
        {
            ConfigureDropdownOptions(dropdownComponent, options);
        }
        
        return dropdown;
    }
    
    static void CreateDropdownLabel(GameObject dropdown, Dropdown dropdownComponent)
    {
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(dropdown.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(5, 0);
        labelRect.offsetMax = new Vector2(-25, 0);
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "选择类型";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 14;
        labelText.color = Color.black;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        dropdownComponent.captionText = labelText;
    }
    
    static void CreateDropdownArrow(GameObject dropdown, Dropdown dropdownComponent)
    {
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.SetParent(dropdown.transform, false);
        
        RectTransform arrowRect = arrowObj.AddComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1, 0.5f);
        arrowRect.anchorMax = new Vector2(1, 0.5f);
        arrowRect.anchoredPosition = new Vector2(-10, 0);
        arrowRect.sizeDelta = new Vector2(20, 20);
        
        Image arrowImage = arrowObj.AddComponent<Image>();
        arrowImage.color = Color.black;
        
        dropdownComponent.itemImage = arrowImage;
    }
    
    static RectTransform CreateDropdownTemplate(GameObject dropdown)
    {
        // 创建模板容器
        GameObject template = new GameObject("Template");
        template.transform.SetParent(dropdown.transform, false);
        template.SetActive(false);
        
        RectTransform templateRect = template.AddComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 1);
        templateRect.anchorMax = new Vector2(1, 1);
        templateRect.offsetMin = Vector2.zero;
        templateRect.offsetMax = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0, 150);
        
        Image templateImage = template.AddComponent<Image>();
        templateImage.color = Color.white;
        
        // 创建Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(template.transform, false);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // 创建Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        return templateRect;
    }
    
    static (Image, Text) CreateDropdownItem(GameObject dropdown, RectTransform templateRect)
    {
        // 找到Content对象
        Transform content = templateRect.Find("Viewport/Content");
        if (content == null) return (null, null);
        
        GameObject itemTemplate = new GameObject("Item");
        itemTemplate.transform.SetParent(content, false);
        
        RectTransform itemRect = itemTemplate.AddComponent<RectTransform>();
        itemRect.anchorMin = new Vector2(0, 1);
        itemRect.anchorMax = new Vector2(1, 1);
        itemRect.offsetMin = Vector2.zero;
        itemRect.offsetMax = Vector2.zero;
        itemRect.sizeDelta = new Vector2(0, 30);
        
        // 创建Toggle组件
        Toggle itemToggle = itemTemplate.AddComponent<Toggle>();
        
        // 创建背景（Item Image）
        GameObject itemBg = new GameObject("Item Background");
        itemBg.transform.SetParent(itemTemplate.transform, false);
        RectTransform itemBgRect = itemBg.AddComponent<RectTransform>();
        itemBgRect.anchorMin = Vector2.zero;
        itemBgRect.anchorMax = Vector2.one;
        itemBgRect.offsetMin = Vector2.zero;
        itemBgRect.offsetMax = Vector2.zero;
        Image itemBgImage = itemBg.AddComponent<Image>();
        itemBgImage.color = Color.white;
        
        // 创建选中状态的背景（作为Toggle的graphic）
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(itemTemplate.transform, false);
        RectTransform checkmarkRect = checkmark.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0, 0.5f);
        checkmarkRect.anchoredPosition = new Vector2(10, 0);
        checkmarkRect.sizeDelta = new Vector2(20, 20);
        Image checkmarkImage = checkmark.AddComponent<Image>();
        checkmarkImage.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        // 创建文本
        GameObject itemText = new GameObject("Item Label");
        itemText.transform.SetParent(itemTemplate.transform, false);
        RectTransform itemTextRect = itemText.AddComponent<RectTransform>();
        itemTextRect.anchorMin = Vector2.zero;
        itemTextRect.anchorMax = Vector2.one;
        itemTextRect.offsetMin = new Vector2(35, 0); // 为checkmark留出空间
        itemTextRect.offsetMax = new Vector2(-5, 0);
        Text itemTextComponent = itemText.AddComponent<Text>();
        itemTextComponent.text = "选项";
        itemTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        itemTextComponent.fontSize = 14;
        itemTextComponent.color = Color.black;
        itemTextComponent.alignment = TextAnchor.MiddleLeft;
        
        // 设置Toggle组件
        itemToggle.targetGraphic = itemBgImage;
        itemToggle.graphic = checkmarkImage;
        
        return (itemBgImage, itemTextComponent);
    }
    
    /// <summary>
    /// 配置Dropdown选项
    /// </summary>
    static void ConfigureDropdownOptions(Dropdown dropdown, string[] options)
    {
        dropdown.ClearOptions();
        
        foreach (string option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        // 设置默认选择第一个选项
        if (dropdown.options.Count > 0)
        {
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }
    }
    
    /// <summary>
    /// 创建形状类型Dropdown（一键配置）
    /// </summary>
    public static GameObject CreateShapeTypeDropdown(GameObject parent, Vector2 position)
    {
        string[] shapeTypes = { "圆形", "矩形", "三角形", "菱形" };
        return CreateSimpleDropdown(parent, "形状类型", position, shapeTypes);
    }
    
    /// <summary>
    /// 创建球类型Dropdown（一键配置）
    /// </summary>
    public static GameObject CreateBallTypeDropdown(GameObject parent, Vector2 position)
    {
        string[] ballTypes = { "红球", "蓝球", "绿球", "黄球" };
        return CreateSimpleDropdown(parent, "球类型", position, ballTypes);
    }
    
    /// <summary>
    /// 使用配置创建Dropdown（一键配置）
    /// </summary>
    public static GameObject CreateDropdownWithConfig(GameObject parent, Vector2 position, DropdownConfig config)
    {
        GameObject dropdown = CreateSimpleDropdown(parent, config.label, position, config.options);
        Dropdown dropdownComponent = dropdown.GetComponent<Dropdown>();
        
        // 设置默认文本
        if (dropdownComponent.captionText != null)
        {
            dropdownComponent.captionText.text = config.defaultText;
        }
        
        // 确保Dropdown正确初始化
        dropdownComponent.RefreshShownValue();
        
        return dropdown;
    }
    
    /// <summary>
    /// 创建简化的Dropdown（更可靠的方法）
    /// </summary>
    public static GameObject CreateSimpleDropdown(GameObject parent, string label, Vector2 position, string[] options)
    {
        GameObject dropdown = new GameObject(label + "Dropdown");
        dropdown.transform.SetParent(parent.transform, false);
        
        RectTransform dropdownRect = dropdown.AddComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(position.x, position.y);
        dropdownRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.05f);
        dropdownRect.offsetMin = Vector2.zero;
        dropdownRect.offsetMax = Vector2.zero;
        
        Image dropdownBg = dropdown.AddComponent<Image>();
        dropdownBg.color = Color.white;
        
        Dropdown dropdownComponent = dropdown.AddComponent<Dropdown>();
        dropdownComponent.targetGraphic = dropdownBg;
        
        // 创建简单的模板
        GameObject template = new GameObject("Template");
        template.transform.SetParent(dropdown.transform, false);
        template.SetActive(false);
        
        RectTransform templateRect = template.AddComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 1);
        templateRect.anchorMax = new Vector2(1, 1);
        templateRect.offsetMin = Vector2.zero;
        templateRect.offsetMax = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0, 120);
        
        Image templateImage = template.AddComponent<Image>();
        templateImage.color = Color.white;
        
        // 创建Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(template.transform, false);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // 创建Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        // 创建Item
        GameObject item = new GameObject("Item");
        item.transform.SetParent(content.transform, false);
        
        RectTransform itemRect = item.AddComponent<RectTransform>();
        itemRect.anchorMin = new Vector2(0, 1);
        itemRect.anchorMax = new Vector2(1, 1);
        itemRect.offsetMin = Vector2.zero;
        itemRect.offsetMax = Vector2.zero;
        itemRect.sizeDelta = new Vector2(0, 30);
        
        Toggle itemToggle = item.AddComponent<Toggle>();
        
        // 创建Item Background
        GameObject itemBg = new GameObject("Item Background");
        itemBg.transform.SetParent(item.transform, false);
        
        RectTransform itemBgRect = itemBg.AddComponent<RectTransform>();
        itemBgRect.anchorMin = Vector2.zero;
        itemBgRect.anchorMax = Vector2.one;
        itemBgRect.offsetMin = Vector2.zero;
        itemBgRect.offsetMax = Vector2.zero;
        
        Image itemBgImage = itemBg.AddComponent<Image>();
        itemBgImage.color = Color.white;
        
        // 创建Item Label
        GameObject itemLabel = new GameObject("Item Label");
        itemLabel.transform.SetParent(item.transform, false);
        
        RectTransform itemLabelRect = itemLabel.AddComponent<RectTransform>();
        itemLabelRect.anchorMin = Vector2.zero;
        itemLabelRect.anchorMax = Vector2.one;
        itemLabelRect.offsetMin = new Vector2(5, 0);
        itemLabelRect.offsetMax = new Vector2(-5, 0);
        
        Text itemLabelText = itemLabel.AddComponent<Text>();
        itemLabelText.text = "选项";
        itemLabelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        itemLabelText.fontSize = 14;
        itemLabelText.color = Color.black;
        itemLabelText.alignment = TextAnchor.MiddleLeft;
        
        // 设置Toggle
        itemToggle.targetGraphic = itemBgImage;
        itemToggle.graphic = itemBgImage;
        
        // 设置Dropdown
        dropdownComponent.template = templateRect;
        dropdownComponent.itemText = itemLabelText;
        dropdownComponent.itemImage = itemBgImage; // 关键修正
        
        // 创建Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(dropdown.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(5, 0);
        labelRect.offsetMax = new Vector2(-25, 0);
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "请选择";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 14;
        labelText.color = Color.black;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        dropdownComponent.captionText = labelText;
        
        // 创建Arrow
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.SetParent(dropdown.transform, false);
        
        RectTransform arrowRect = arrowObj.AddComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1, 0.5f);
        arrowRect.anchorMax = new Vector2(1, 0.5f);
        arrowRect.anchoredPosition = new Vector2(-10, 0);
        arrowRect.sizeDelta = new Vector2(20, 20);
        
        Image arrowImage = arrowObj.AddComponent<Image>();
        arrowImage.color = Color.black;
        
        dropdownComponent.itemImage = arrowImage;
        
        // 配置选项
        if (options != null && options.Length > 0)
        {
            dropdownComponent.ClearOptions();
            foreach (string option in options)
            {
                dropdownComponent.options.Add(new Dropdown.OptionData(option));
            }
            dropdownComponent.value = 0;
            dropdownComponent.RefreshShownValue();
        }
        
        return dropdown;
    }
    
    /// <summary>
    /// 获取预定义的Dropdown配置
    /// </summary>
    public static DropdownConfig GetShapeTypeConfig()
    {
        return new DropdownConfig("形状类型", new string[] { "圆形", "矩形", "三角形", "菱形" }, "选择形状");
    }
    
    /// <summary>
    /// 获取球类型Dropdown配置
    /// </summary>
    public static DropdownConfig GetBallTypeConfig()
    {
        return new DropdownConfig("球类型", new string[] { "红球", "蓝球", "绿球", "黄球" }, "选择球类型");
    }
} 