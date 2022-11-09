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
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class SelectGeometry : IExternalCommand
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

                Options geometryOptions = new Options();
                geometryOptions.DetailLevel = ViewDetailLevel.Fine;
                GeometryElement geometryElements = element.get_Geometry(geometryOptions);

                foreach(GeometryObject g in geometryElements)
                {
                    Solid solid = g as Solid;

                    int faces = 0;
                    double area = 0;

                    foreach(Face f in solid.Faces)
                    {
                        area += f.Area;
                        faces++;
                    }

                    area = UnitUtils.ConvertFromInternalUnits(area, DisplayUnitType.DUT_SQUARE_METERS);

                    TaskDialog.Show("Geometry area", string.Format("Number of faces: {0} \n Total area: {1}", faces, area));
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
