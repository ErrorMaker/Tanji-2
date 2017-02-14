using System.Windows;

using Tanji.Services;

namespace Tanji
{
    public partial class App : Application
    {
        public static IMaster Master { get; set; }

        public static bool InDesignMode()
        {
            return !(Current is App);
        }
    }
}