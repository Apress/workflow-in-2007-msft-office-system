using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using Microsoft.SharePoint.WorkflowActions;

namespace KCD.Sharepoint.Activities.Composite
{
    
    [Serializable]
    internal class LoggerActivityToolboxItem : ActivityToolboxItem
    {
        public LoggerActivityToolboxItem(Type type): base(type)
        {
        }

        private LoggerActivityToolboxItem(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        protected override IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            System.Workflow.ComponentModel.CompositeActivity activity = new LoggerActivity();

            LogToHistoryListActivity logger = new LogToHistoryListActivity();
            logger.Name = "logger";
            logger.UserData["logger"] = "logger";
            activity.Activities.Add(logger);
           
            return new IComponent[] { activity };
        }
    }
}