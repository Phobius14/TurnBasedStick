using Assets.HeadStart.Core;
using Assets.HeadStart.CoreUi;

public static class ScreenData
{
    private static IUiDependency _screenDataCanvas;

    public static void SetAvailable(IUiDependency screenDataCanvas)
    {
        _screenDataCanvas = screenDataCanvas;
    }

    public static void InitDependency(object obj)
    {
        if (_screenDataCanvas == null) { return; }
        _screenDataCanvas.InitDependency(obj);
    }

    public static void Register(CoreUiObservedValue obj)
    {
        if (_screenDataCanvas == null) { return; }
        _screenDataCanvas.ListenForChanges(obj);
    }

    public static void Register(ref CoreObservedValues obj)
    {
        if (_screenDataCanvas == null) { return; }
        _screenDataCanvas.ListenForChanges(ref obj);
    }

    public static void SetUnavailable()
    {
        _screenDataCanvas = null;
    }
}
