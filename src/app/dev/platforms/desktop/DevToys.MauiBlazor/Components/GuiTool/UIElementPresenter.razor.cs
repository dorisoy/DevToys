using DevToys.Api;
using Microsoft.UI.Xaml;

namespace DevToys.MauiBlazor.Components.GuiTool;

public partial class UIElementPresenter : MefLayoutComponentBase
{
    [Parameter]
    public IUIElement? UIElement { get; set; }
}
