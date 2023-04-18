using DevToys.Api;

namespace DevToys.MauiBlazor.Components.GuiTool;

public partial class UIButtonPresenter : MefLayoutComponentBase
{
    [Parameter]
    public IUIButton? UIButton { get; set; }

    internal async Task OnButtonClickAsync()
    {
        Guard.IsNotNull(UIButton);
        if (UIButton.OnClickAction is not null)
        {
            await UIButton.OnClickAction.Invoke(); // TODO: Try Catch and log?
        }
    }
}
