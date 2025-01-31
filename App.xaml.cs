using Microsoft.Maui.Controls;

namespace BedChangeReminder
{
    public partial class App : Application
    {
        public App(AppShell appShell)
        {
            InitializeComponent();
            MainPage = appShell;
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell()); // Use AppShell as the main UI
        //}
    }
}