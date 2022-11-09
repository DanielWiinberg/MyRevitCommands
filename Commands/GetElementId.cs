using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class GetElementId : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

            Document doc = uiDoc.Document;

            try
            {
                Reference pickedObj = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                ElementId elementId = pickedObj.ElementId;
                Element element = doc.GetElement(elementId);

                ElementId elementTypeId = element.GetTypeId();
                ElementType elementType = doc.GetElement(elementTypeId) as ElementType;

                if (pickedObj != null)
                {
                    TaskDialog.Show("Element Classification:", elementTypeId.ToString() + Environment.NewLine
                        + "Category: " + element.Category.Name + Environment.NewLine
                        + "Instance: " + element.Name + Environment.NewLine
                        + "Symbol: " + elementType.Name + Environment.NewLine
                        + "Family: " + elementType.FamilyName);
                }
            }
            catch(Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }


            return Result.Succeeded;
        }
    }
}
