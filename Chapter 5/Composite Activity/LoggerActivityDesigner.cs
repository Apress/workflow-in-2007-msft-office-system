using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;

namespace KCD.Sharepoint.Activities.Composite
{

    public class LoggerActivityDesigner : SequentialActivityDesigner
    {
        public override bool CanInsertActivities(HitTestInfo insertLocation,
            ReadOnlyCollection<Activity> activitiesToInsert)
        {
            return insertLocation.MapToIndex() != 0;
        }

        public override bool CanMoveActivities(HitTestInfo moveLocation,
            ReadOnlyCollection<Activity> activitiesToMove)
        {
            return moveLocation.MapToIndex() != 0;
        }


        public override bool CanRemoveActivities(ReadOnlyCollection<Activity>
            activitiesToRemove)
        {
            foreach (Activity a in activitiesToRemove)
            {
                if ((string)a.UserData["logger"] == "logger")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
