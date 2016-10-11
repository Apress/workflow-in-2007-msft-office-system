using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.Office.Workflow.Utility;

namespace ResetTasks
{
	public sealed partial class ResetTasks: StateMachineWorkflowActivity
	{
		public ResetTasks()
		{
			InitializeComponent();
		}

        public SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();

        private void InitCreateTask(object sender, EventArgs e)
        {
            this.taskID = Guid.NewGuid();
            this.updateDueDates(null, null);
            this.taskProperties.AssignedTo = GetCurrentReviewers();
            this.taskProperties.Title = "Marketing Campaign Review";
        }

        public Guid taskID = default(System.Guid);
        public SPWorkflowTaskProperties taskProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties afterProps = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties beforeProps = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        private void updateDueDates(object sender, EventArgs e)
        {
            Random rand = new Random();
            this.taskProperties.DueDate = DateTime.Now.AddDays(rand.Next(5, 15));
        }

        private string GetCurrentReviewers()
        {
            return @"mossrtm\marketingreviewer";
        }
	}

}
