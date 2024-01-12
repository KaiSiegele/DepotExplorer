using System.Windows;
using System.Diagnostics;
using Persistence;
using UserInterface;
using Tools;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (InitApplicaion())
            {
                new MainWindow().Show();
            }
            else
            {
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (databaseOpen)
            {
                Error error = new Error();
                DatabaseHandle.CloseDatabase(error);
            }
//            UserSettings.Save("settings.xml", new UserSettings());
        }

        private bool InitApplicaion()
        {
            UserSettings settings = new UserSettings();
            UserSettings.Load("settings.xml", ref settings);

            InitLog(settings.LogDirectory, settings.TraceLevel);

            return InitData(settings.DataBaseType, settings.ConnectionString);
        }

        private void InitLog(string dir, TraceLevel level)
        {
            string message = string.Empty;
            string file = Misc.CreateFilename("Log", "txt");

            if (!Log.Init(dir, file, "DepotExplorer", level, ref message))
            {
                Prompts.ShowWarning(DepotExplorer.Properties.Resources.Title, message);
            }
        }

        private bool InitData(DBType dataBaseType, string connectionString)
        {
            bool result = false;
            Error error = new Error();

            if (DatabaseHandle.CreateDatabase(dataBaseType, error))
            {
                if (DatabaseHandle.OpenDatabase(connectionString, error))
                {
                    databaseOpen = true;
                    string message = string.Empty;
                    if (Starter.InitData(true, ref message))
                    {
                        result = true;
                    }
                    else
                    {
                        Prompts.ShowError(DepotExplorer.Properties.Resources.Title, DepotExplorer.Properties.Resources.DatabaseError, message);
                    }
                }
                else
                {
                    Prompts.ShowError(DepotExplorer.Properties.Resources.Title, DepotExplorer.Properties.Resources.DatabaseError, error.Description);
                }
            }
            else
            {
                Prompts.ShowError(DepotExplorer.Properties.Resources.Title, DepotExplorer.Properties.Resources.DatabaseError, error.Description);
            }
            return result;
        }

        private bool databaseOpen = false;
    }
}
