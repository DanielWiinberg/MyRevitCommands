using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace MyRevitCommands
{
    [Transaction(TransactionMode.Manual)]
    class PlaceView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            ViewSheet sheet = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Sheets)
                .Cast<ViewSheet>()
                .First(i => i.Name == "Test sheet");

            Element plan = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .First(i => i.Name == "First created plan");

            BoundingBoxUV bBox = sheet.Outline; //Hvordan findes en liste af de forskellige metoder og properties der kan kaldes her?
            double x = (bBox.Max.U + bBox.Min.U) / 2;
            double y = (bBox.Max.V + bBox.Min.V) / 2;

            XYZ centerPoint = new XYZ(x, y, 0);


            try
            {
                using (Transaction trans = new Transaction(doc, "Place view"))
                {
                    trans.Start();

                    Viewport viewport = Viewport.Create(doc, sheet.Id, plan.Id, centerPoint);

                    trans.Commit();
                }

            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
    
}
