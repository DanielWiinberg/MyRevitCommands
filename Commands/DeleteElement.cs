using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    class DeleteElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                Reference pickedObj = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            
                if (pickedObj != null)
                {
                    using (Transaction trans = new Transaction(doc))
                    {
                        trans.Start();
                        doc.Delete(pickedObj.ElementId);

                        TaskDialog taskDialog = new TaskDialog("Delete Element");
                        taskDialog.MainContent = "Are you sure you want to delete the element?";
                        taskDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                        if(taskDialog.Show() == TaskDialogResult.Ok)
                        {
                            trans.Commit();
                            TaskDialog.Show("Delete", pickedObj.ElementId.ToString() + "deleted");
                        }else
                        {
                            trans.RollBack();
                            TaskDialog.Show("Delete", pickedObj.ElementId.ToString() + "not deleted");
                        }

                    }


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
