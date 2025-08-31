using BedChangeReminder.Views.Pages;

namespace BedChangeReminder
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage)
        {
            InitializeComponent();
            Items.Add(new ShellContent
            {
                Content = mainPage,
                Title = "Home"
            });
        }
    }
}