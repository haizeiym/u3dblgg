using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI组件构建器
/// 负责创建各种UI组件
/// </summary>
public class UIComponentBuilder
{
    public static GameObject CreateButton(GameObject parent, string text, Vector2 position)
    {
        GameObject button = new GameObject(text + "Button");
        button.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = button.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(position.x, position.y);
        buttonRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.05f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        Image buttonBg = button.AddComponent<Image>();
        buttonBg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        Button buttonComponent = button.AddComponent<Button>();
        buttonComponent.targetGraphic = buttonBg;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        return button;
    }
    
    public static GameObject CreateInputField(GameObject parent, string label, Vector2 position)
    {
        GameObject inputField = new GameObject(label + "Input");
        inputField.transform.SetParent(parent.transform, false);
        
        RectTransform inputRect = inputField.AddComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(position.x, position.y);
        inputRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.05f);
        inputRect.offsetMin = Vector2.zero;
        inputRect.offsetMax = Vector2.zero;
        
        Image inputBg = inputField.AddComponent<Image>();
        inputBg.color = Color.white;
        
        InputField inputComponent = inputField.AddComponent<InputField>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputField.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 0);
        textRect.offsetMax = new Vector2(-5, 0);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = "";
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.black;
        textComponent.alignment = TextAnchor.MiddleLeft;
        
        inputComponent.textComponent = textComponent;
        
        return inputField;
    }
    
    public static GameObject CreateSlider(GameObject parent, string label, Vector2 position)
    {
        GameObject slider = new GameObject(label + "Slider");
        slider.transform.SetParent(parent.transform, false);
        
        RectTransform sliderRect = slider.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(position.x, position.y);
        sliderRect.anchorMax = new Vector2(position.x + 0.15f, position.y + 0.05f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;
        
        Image sliderBg = slider.AddComponent<Image>();
        sliderBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        Slider sliderComponent = slider.AddComponent<Slider>();
        sliderComponent.targetGraphic = sliderBg;
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(slider.transform, false);
        
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(20, 20);
        
        Image handleBg = handle.AddComponent<Image>();
        handleBg.color = Color.white;
        
        sliderComponent.handleRect = handleRect;
        
        return slider;
    }
    
    public static GameObject CreateDropdown(GameObject parent, string label, Vector2 position)
    {
        return DropdownBuilder.CreateDropdown(parent, label, position);
    }
    
    public static GameObject CreateDropdown(GameObject parent, string label, Vector2 position, string[] options)
    {
        return DropdownBuilder.CreateDropdown(parent, label, position, options);
    }
    
    /// <summary>
    /// 创建形状类型Dropdown（一键配置）
    /// </summary>
    public static GameObject CreateShapeTypeDropdown(GameObject parent, Vector2 position)
    {
        return DropdownBuilder.CreateShapeTypeDropdown(parent, position);
    }
    
    /// <summary>
    /// 创建球类型Dropdown（一键配置）
    /// </summary>
    public static GameObject CreateBallTypeDropdown(GameObject parent, Vector2 position)
    {
        return DropdownBuilder.CreateBallTypeDropdown(parent, position);
    }
    
    /// <summary>
    /// 创建简化的Dropdown（更可靠的方法）
    /// </summary>
    public static GameObject CreateSimpleDropdown(GameObject parent, string label, Vector2 position, string[] options)
    {
        return DropdownBuilder.CreateSimpleDropdown(parent, label, position, options);
    }
} 