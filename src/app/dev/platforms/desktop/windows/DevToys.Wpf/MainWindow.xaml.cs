using System.Windows;
using DevToys.Api;
using DevToys.Business.ViewModels;
using DevToys.Core.Mef;
using DevToys.MauiBlazor.Core.FileStorage;
using DevToys.MauiBlazor.Core.Languages;
using DevToys.Tools;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.Fast.Components.FluentUI.Infrastructure;
using Uno.Extensions;
using MicaWPF.Controls;
using PredefinedSettings = DevToys.Core.Settings.PredefinedSettings;

namespace DevToys.MauiBlazor;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MicaWindow
{
    internal static readonly Lazy<MefComposer> MefComposer
        = new(() => new MefComposer(
            new[] {
                typeof(DevToysToolsResourceManagerAssemblyIdentifier).Assembly,
                typeof(MainWindowViewModel).Assembly
            }));

    public MainWindow()
    {
        InitializeComponent();

        blazorWebView.BlazorWebViewInitializing += BlazorWebView_BlazorWebViewInitializing;
        blazorWebView.Initialized += BlazorWebView_Initialized;

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
#if DEBUG
            MessageBox.Show(error.ExceptionObject.ToString(), caption: "Error");
#else
            MessageBox.Show(text: "An error has occurred.", caption: "Error");
#endif

            // Log the error information (error.ExceptionObject)
        };

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWpfBlazorWebView();

        // Initialize Microsoft.Fast.Components.FluentUI
        serviceCollection.AddFluentUIComponents(options =>
        {
            Guard.IsNotNull(options);
            options.HostingModel = BlazorHostingModel.Hybrid;
        });
        serviceCollection.AddScoped<IStaticAssetService, FileBasedStaticAssetService>();

#if DEBUG
        serviceCollection.AddBlazorWebViewDeveloperTools();
        //builder.Logging.AddDebug();
#endif

        //builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
        //builder.Logging.AddFilter("System", LogLevel.Warning);
        //builder.Logging.AddFile(new FileStorage());

        serviceCollection.AddSingleton(provider => MefComposer.Value.Provider);

        GlobalExceptionHandler.UnhandledException += GlobalExceptionHandler_UnhandledException;

        //ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        //global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = loggerFactory;

        //app.Log().LogInformation("App is starting...");

        // Set the user-defined language.
        string? languageIdentifier = MefComposer.Value.Provider.Import<ISettingsProvider>().GetSetting(PredefinedSettings.Language);
        LanguageDefinition languageDefinition
            = LanguageManager.Instance.AvailableLanguages.FirstOrDefault(l => string.Equals(l.InternalName, languageIdentifier))
            ?? LanguageManager.Instance.AvailableLanguages[0];
        LanguageManager.Instance.SetCurrentCulture(languageDefinition);

        Resources.Add("services", serviceCollection.BuildServiceProvider());
    }

    private void BlazorWebView_Initialized(object? sender, EventArgs e)
    {
        blazorWebView.WebView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
    }

    private void BlazorWebView_BlazorWebViewInitializing(object sender, Microsoft.AspNetCore.Components.WebView.BlazorWebViewInitializingEventArgs e)
    {
        blazorWebView.WebView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
    }

    private static void GlobalExceptionHandler_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        //Guard.IsNotNull(app);
        //LogUnhandledException(app.Log(), (Exception)e.ExceptionObject);
    }

    [LoggerMessage(5, LogLevel.Critical, "Unhandled exception !!!    (╯°□°）╯︵ ┻━┻")]
    static partial void LogUnhandledException(ILogger logger, Exception exception);
}
