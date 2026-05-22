using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfApp2.Models;
using WpfApp2.Services;
using WpfApp2.ViewModel;

namespace WpfApp2;

public partial class App : Application
{
    string connectionString = "Data Source=DBSRV\ag2025;Initial Catalog=LeymanSE2307g1 LAB12;Integrated Security=True;TrustServerCertificate=True";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();

        services.AddDbContext<PhoneBookContext>(options =>
        options.UseSqlServer(
        "connectionString"));

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