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
    class EditElement : IExternalCommand
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

                using(Transaction trans = new Transaction(doc, "Edit element"))
                {
                    trans.Start();

                    XYZ translationVector = new XYZ(10, 10, 0);

                    ElementTransformUtils.MoveElement(doc, element.Id, translationVector);

                    LocationPoint elementLocationPoint = element.Location as LocationPoint;
                    if(elementLocationPoint == null) { return Result.Failed; }

                    Line rotationAxis = Line.CreateBound(elementLocationPoint.Point, new XYZ(elementLocationPoint.Point.X, elementLocationPoint.Point.Y, elementLocationPoint.Point.Z + 1));

                    double angle = 45 * Math.PI / 180;

                    ElementTransformUtils.RotateElement(doc, element.Id, rotationAxis, angle);

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
