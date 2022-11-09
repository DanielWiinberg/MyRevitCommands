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
    class ElementIntersection : IExternalCommand
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

                GeometryElement elementGeometry = element.get_Geometry(geometryOptions);

                Solid solids = null;

                foreach(GeometryObject gObj in elementGeometry)
                {
                    GeometryInstance gInst = gObj as GeometryInstance;
                    if(gInst == null) { return Result.Failed; }

                    GeometryElement gEle = gInst.GetInstanceGeometry();

                    foreach(GeometryObject gObj2 in gEle)
                    {
                        solids = gObj2 as Solid;
                    }
                }

                FilteredElementCollector collector = new FilteredElementCollector(doc);
                ElementIntersectsSolidFilter intersectFilter = new ElementIntersectsSolidFilter(solids);
                ICollection<ElementId> intersections = collector.OfCategory(BuiltInCategory.OST_Roofs).WherePasses(intersectFilter).ToElementIds();

                uiDoc.Selection.SetElementIds(intersections);

            }catch(Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
