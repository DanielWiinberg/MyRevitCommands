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
    class TagView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tagOrientation = TagOrientation.Horizontal;

            List<BuiltInCategory> categories = new List<BuiltInCategory>();
            categories.Add(BuiltInCategory.OST_Windows);
            categories.Add(BuiltInCategory.OST_Doors);

            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(categories);

            IList<Element> windowsAndDoors = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();

            try
            {
                using(Transaction trans = new Transaction(doc, "Tag Elements"))
                {
                    trans.Start();

                    foreach(Element ele in windowsAndDoors)
                    {
                        Reference curElement = new Reference(ele);
                        LocationPoint location = ele.Location as LocationPoint;
                        XYZ point = location.Point;

                        IndependentTag.Create(doc, doc.ActiveView.Id, curElement, true, tagMode, tagOrientation, point);
                    }

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
