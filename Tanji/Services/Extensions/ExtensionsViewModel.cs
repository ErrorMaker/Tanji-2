using System;
using Tanji.Helpers;

namespace Tanji.Services.Extensions
{
    public class ExtensionsViewModel : ObservableObject, IHaltable
    {
        public ExtensionsViewModel()
        { }

        public void Halt()
        { }
        public void Restore()
        { }
    }
}