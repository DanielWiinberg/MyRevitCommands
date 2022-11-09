using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    class ExternalDBApplication : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            application.DocumentChanged -= ElementChangedEvent;

            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            try
            {
                application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(ElementChangedEvent);

            }catch(Exception e)
            {
                return ExternalDBApplicationResult.Failed;
            }
            
            return ExternalDBApplicationResult.Succeeded;
        }

        public void ElementChangedEvent(object sender, DocumentChangedEventArgs args)
        {
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementId elementId = args.GetModifiedElementIds(filter).First();
            string name = args.GetTransactionNames().First();

            TaskDialog.Show("Modified element", elementId.ToString() + " changed by: " + name);
        }
    }
}
