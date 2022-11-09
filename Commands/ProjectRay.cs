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
    class ProjectRay : IExternalCommand
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

                LocationPoint locationPoint = element.Location as LocationPoint;
                if(locationPoint == null) { return Result.Failed; }

                XYZ p1 = locationPoint.Point;

                XYZ rayDirection = new XYZ(0, 0, 1);

                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Roofs);
                ReferenceIntersector referenceIntersector = new ReferenceIntersector(filter, FindReferenceTarget.All, (View3D)doc.ActiveView);

                ReferenceWithContext contextReference = referenceIntersector.FindNearest(p1, rayDirection);
                Reference hitReference = contextReference.GetReference();

                XYZ intersectionPoint = hitReference.GlobalPoint;

                double distance = p1.DistanceTo(intersectionPoint);
                distance = UnitUtils.ConvertFromInternalUnits(distance, DisplayUnitType.DUT_MILLIMETERS);

                TaskDialog.Show("Raycasting", string.Format("Distance to roof: {0}", distance));

            }catch(Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
