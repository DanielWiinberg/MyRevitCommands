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
    class SetParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                Reference pickedObj = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if (pickedObj == null)
                {
                    return Result.Failed;
                }

                Element element = doc.GetElement(pickedObj.ElementId);

                Parameter parameter = element.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM);

                TaskDialog.Show("Pameter value", string.Format("Parameter storage type: {0} \n Value: {1}", 
                    parameter.StorageType.ToString(),
                    parameter.AsValueString()));

                using (Transaction trans = new Transaction(doc, "Set parameter"))
                {
                    trans.Start();

                    parameter.Set(UnitUtils.ConvertToInternalUnits(1500, DisplayUnitType.DUT_MILLIMETERS));

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
