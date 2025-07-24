/// <summary>
/// UI更新器接口
/// 用于解耦运行时脚本和编辑器脚本
/// </summary>
public interface IUIUpdater
{
    /// <summary>
    /// 取消订阅配置变更事件
    /// </summary>
    void UnsubscribeFromConfigEvents();
} 