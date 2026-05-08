using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp2.Services;
using WpfApp2.ViewModel;

namespace WpfApp2;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<ViewModels>();

        services.AddSingleton<MainWindow>(sp =>
        {
            var window = new MainWindow();

            window.DataContext =
                sp.GetRequiredService<ViewModels>();

            return window;
        });
        var serviceProvider =
            services.BuildServiceProvider();
        var mainWindow =
            serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}