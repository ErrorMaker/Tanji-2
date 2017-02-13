using System.Windows;

using Tanji.Services;

namespace Tanji
{
    public partial class App : Application
    {
        public static IMaster Master { get; set; }
    }
}