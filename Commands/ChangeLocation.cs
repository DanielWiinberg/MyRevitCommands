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
    [TransactionAttribute(TransactionMode.Manual)]
    class ChangeLocation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                Reference pickedObj = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if(pickedObj == null) { return Result.Failed; }

                Element element = doc.GetElement(pickedObj.ElementId);

                using(Transaction trans = new Transaction(doc, "Change location"))
                {
                    trans.Start();

                    LocationPoint locationPoint = element.Location as LocationPoint;

                    if(locationPoint == null) { return Result.Failed; }

                    XYZ oldPoint = locationPoint.Point;
                    XYZ newPoint = new XYZ(oldPoint.X + 3, oldPoint.Y + 3, oldPoint.Z);

                    locationPoint.Point = newPoint;

                    trans.Commit();
                }


            }catch(Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
