我正在使用 Unity3D (2022+) 开发一个 **拔罐小游戏关卡编辑器**，要求如下：

# 功能与限制
1. **关卡系统**
   - 每个关卡包含多个层级（Layer）。
   - 每个层级包含若干固定图形（Shape）。
   - 每个小球（Ball）只能放置在某个 Shape 内。
   - 数据结构为：
     ```json
     {
       "关卡": {
         "层级": {
           "固定图形类型1": {
             "位置": "相对父节点",
             "旋转": "相对父节点",
             "球": {
               "球1": {
                 "位置": "相对父节点",
                 "类型": "预设类型"
               }
             }
           }
         }
       }
     }
     ```

2. **UI 交互**
   - 使用 UGUI 组件（Canvas + Button + ScrollView + InputField + Slider）。
   - 左侧：显示关卡和层级列表，可添加/删除。
   - 中间：编辑区，显示当前层级中的 Shape 和 Ball。
   - 右侧：属性面板，可编辑选中 Shape 或 Ball 的位置、旋转和类型。
   - 点击 Shape 时，高亮显示。
   - **支持拖拽和旋转：**
     - 拖拽：通过实现 `IBeginDragHandler` / `IDragHandler` 修改 `RectTransform.anchoredPosition`。
     - 旋转：通过 Slider 或按钮修改 `RectTransform.localEulerAngles.z`。

3. **导出**
   - 提供按钮导出当前关卡数据为 JSON 文件。

4. **代码结构要求**
   - 拆分 3 个主要脚本：
     - `LevelData.cs`：仅包含数据结构类（Level、Layer、Shape、Ball）。
     - `LevelEditorUI.cs`：控制 UI 和用户交互。
     - `ShapeController.cs`：单个 Shape 的拖拽和旋转逻辑。
   - 每个类文件 **有效代码不超过 200 行**。
   - 使用 `JsonUtility` 或 `Newtonsoft.Json` 序列化导出数据。

5. **输出格式**
   - 只输出 C# 代码文件。
   - 确保每个类的逻辑简洁，不超过 200 行。
   - 给出必要的 `MonoBehaviour` 示例，直接可用。