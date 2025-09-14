using System.Configuration;
using System.Data;
using System.Windows;
using TestHub;

namespace TestHub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the database
            Database.Initialize();

            // Project database connection
            Database.TestConnection();
        }
    }
}
