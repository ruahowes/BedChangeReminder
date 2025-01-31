namespace BedChangeReminder
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            CurrentItem = new ShellContent
            {
                Content = new MainPage(new MainViewModel())
            };
        }

        //public AppShell(MainPage mainPage)
        //{
        //    InitializeComponent();
        //    this.Items.Add(new ShellContent { Content = mainPage });
        //}
    }
}
