using DevToys.Api;

namespace DevToys.MauiBlazor.Components.GuiTool;

public partial class UIStackPresenter : MefLayoutComponentBase
{
    [Parameter]
    public IUIStack? UIStack { get; set; }

    public override void Dispose()
    {
        if (UIStack != null)
        {
            UIStack.IsVisibleChanged -= UIStack_IsVisibleChanged;
            UIStack.Show(); // Make it visible again so when we come back to the tool, it re-appears.
            UIStack = null;
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (UIStack != null)
        {
            UIStack.IsVisibleChanged += UIStack_IsVisibleChanged;
        }
    }

    private void UIStack_IsVisibleChanged(object? sender, EventArgs e)
    {
        this.StateHasChanged();
    }
}
