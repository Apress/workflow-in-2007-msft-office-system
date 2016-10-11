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
using System.Xml.Serialization;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;

namespace RuleSetTestWorkflow
{
	public sealed partial class Workflow1: SequentialWorkflowActivity
	{
        int myFlagVariable = 0;
        
        public Workflow1()
		{
			InitializeComponent();
		}

        public Guid workflowId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();

        private void sendEMailA(object sender, EventArgs e)
        {
            this.sendEmail1.Body = "Branch 1 executed, so myFlagVariable = 7";
        }
        private void sendEMailB(object sender, EventArgs e)
        {
            this.sendEmail2.Body = "Branch 2 executed, so myFlagVariable <> 7";
        }

	}

}
