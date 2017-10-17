using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HumanUI
{
    public class HUI_RhPickButton : Button
    {
        public List<Guid> objIDs;
        private string msg;
        private bool allowMultiple;
        private bool allowNone;
        private ObjectType filter;
        public HUI_RhPickButton(string msg, bool allowMultiple, bool allowNone, ObjectType filter)
            : base()
        {
            this.msg = msg;
            this.allowMultiple = allowMultiple;
            this.allowNone = allowNone;
            this.filter = filter;
            objIDs = new List<Guid>();
            Click += PickButtonClick;
        }

        private void PickButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            getNewGeometry();
        }

        void getNewGeometry()
        {



            ObjRef[] rhObjects = new ObjRef[1];


            try
            {
                Rhino.Commands.Result result = Rhino.Commands.Result.Cancel;
                if (!allowMultiple)
                {
                    result = Rhino.Input.RhinoGet.GetOneObject(msg, allowNone, filter, out ObjRef rhObject);
                    rhObjects[0] = rhObject;

                }
                else
                {
                    result = Rhino.Input.RhinoGet.GetMultipleObjects(msg, allowNone, filter, out rhObjects);
                }

                if (result == Rhino.Commands.Result.Cancel)
                {
                    OnPickCompleted(EventArgs.Empty);
                }
                else if (result == Rhino.Commands.Result.Success)
                {
                    objIDs = rhObjects.Select(o => o.ObjectId).ToList();
                    OnPickCompleted(EventArgs.Empty);
                    return;
                }
            }
            catch
            {

            }

        }


        protected virtual void OnPickCompleted(EventArgs e)
        {
            PickCompleted?.Invoke(this, e);
        }

        public event EventHandler PickCompleted;
    }
}
