我正在使用 Unity3D (2021+) 开发一个 **拔罐小游戏关卡编辑器**，要求在 Unity 菜单栏添加一个 **"一键配置" 按钮**（位于 `Tools/Level Editor/一键配置`），功能如下：

# 功能需求
1. **菜单功能**
   - 在菜单栏添加 `Tools/Level Editor/一键配置` 菜单。
   - 点击该菜单项时：
     - 自动创建一个基础 Canvas（如果场景中没有 Canvas）。
     - 自动创建 UI 结构（ScrollView、Button、InputField、Slider）作为关卡编辑器 UI。
     - 将 `LevelEditorUI.cs` 脚本自动挂到 Canvas 上。
     - 初始化一个默认的 LevelData 对象。

2. **关卡编辑器**
   - 保持之前的功能：关卡/层级/Shape/Ball 编辑。
   - UI 仍然基于 UGUI。

3. **代码结构**
   - 增加 `LevelEditorMenu.cs`，其中包含 `[MenuItem("Tools/Level Editor/一键配置")]` 静态方法。
   - 其余脚本：
     - `LevelData.cs`
     - `LevelEditorUI.cs`
     - `ShapeController.cs`
   - 每个类代码 **不超过 200 行**。

4. **输出要求**
   - 只输出 C# 脚本。
   - 确保 `LevelEditorMenu.cs` 可直接创建所需 UI 结构并挂载 `LevelEditorUI`。