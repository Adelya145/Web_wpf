using Shoes.Model;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Shoes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentClient { get; set; }
    }

}
