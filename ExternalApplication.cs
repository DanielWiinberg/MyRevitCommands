using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace MyRevitCommands
{
    class ExternalApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("MyRevitCommands");

            string path = Assembly.GetExecutingAssembly().Location;
            PushButtonData button = new PushButtonData("B1", "Change Location", path, "MyRevitCommands.ChangeLocation");

            RibbonPanel panel = application.CreateRibbonPanel("MyRevitCommands", "Commands");

            Uri imagePath = new Uri(@"C:\Users\dwii\Documents\CreatingRevitPlugins_Tutorial\Ex_Files_Revit_Creating_C_Sharp_Plugins\Exercise Files\05_04\Start\desk.png");
            BitmapImage icon = new BitmapImage(imagePath);

            PushButton pushButton = panel.AddItem(button) as PushButton;
            pushButton.LargeImage = icon;

            return Result.Succeeded;
        }
    }
}
