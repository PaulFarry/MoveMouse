namespace MoveMouse
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// <remarks>
        /// Mouse Icon from <a href="https://www.flaticon.com/free-icons/mouse-clicker" title="mouse clicker icons">Mouse clicker icons created by designvector10 - Flaticon</a>
        /// </remarks>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());

        }
    }
}