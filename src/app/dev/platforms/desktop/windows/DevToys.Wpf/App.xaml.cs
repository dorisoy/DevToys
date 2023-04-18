using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevToys.Api.Core.Theme;

namespace DevToys.MauiBlazor;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Apply the app color theme.
        DevToys.MauiBlazor.MainWindow.MefComposer.Value.Provider.Import<IThemeListener>().ApplyDesiredColorTheme();
    }
}
