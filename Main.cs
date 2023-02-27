using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIWritingTypeInstanceParameters
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберите элемент");
            var selectedElement = doc.GetElement(selectedRef);//приобразовываем объект в элемент
            
            if(selectedElement is FamilyInstance)
            {
                //создаем транзакцию, doc -это указываем в каком документе делаем транзакцию и называем "Set parameters"
                using (Transaction ts = new Transaction(doc, "Установить параметр"))
                {
                    ts.Start(); //начало транзакции;
                    //выполняем преобразование в модель, таким образом считывать данные из модели мы можем без транзакции,
                    //а когда мы вносим какие либо изменения: двигаем элементы, изменяем параметры,
                    //то обязательно используем транзакции иначе будет ошибка;
                    //создаем новую переменную для того, чтобы преобразовать selectedElement в FamilyInstance
                    var familyInstance = selectedElement as FamilyInstance;

                    //создаем переменную типа Parameter обращаемся к familyInstance и ищем параметер с низванием "Коментарий"
                    //или Сomments в анг. версии Revit, get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS) это для рус. версии Revit
                    Parameter commentParameter = familyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                    //обращаемся к commentParameter и записываем значение в данный параметр
                    commentParameter.Set("Тестовый коментарий");

                    //создаем следующий параметр
                    //новая переменная typeCommentsParameter обращаемся к familyInstance и обращаемся теперь к типу Sybol,
                    //потому что будем вести запись значение типа параметра
                    Parameter typeCommentsParameter = familyInstance.Symbol.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    //устанавливаем значение
                    typeCommentsParameter.Set("Тест введите коментарии");
                    ts.Commit();//фиксируем изменения
                }


            }


            return Result.Succeeded;
        }
    }
}
