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

namespace MarketingCampaignASP2
{
	public sealed partial class MarketingCampaignASPWF2: SequentialWorkflowActivity
	{
		public MarketingCampaignASPWF2()
		{
			InitializeComponent();
		}

        public Guid workflowId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();

        private string sTrafficCoordinator = default(String);
        private string sMarketingDirectorEMail = default(String);
        private bool taskCompleted = false;


        private void onSendEmail(object sender, EventArgs e)
        {
            sendEmail1.To = sMarketingDirectorEMail + "@kcdholdings.com";
            string sItemTitle = workflowProperties.Item["Name"].ToString();
            string sItemURL = workflowProperties.ItemUrl;
            sendEmail1.Body = string.Format("New Marketing Campaign: {0}. URL:{1}", sItemTitle,
            sItemURL);
        }

        private void onCreateTask(object sender, EventArgs e)
        {
            taskID = Guid.NewGuid();
taskProperties.Title = "New Marketing Campaign";
taskProperties.AssignedTo = sTrafficCoordinator;
string sItemTitle = workflowProperties.Item["Name"].ToString();
string sItemURL = workflowProperties.ItemUrl;
string sOriginator = workflowProperties.Originator;
taskProperties.Description = string.Format("New Marketing Campaign: {0}. URL:{1}",sItemTitle, sItemURL);
taskProperties.ExtendedProperties["taskinstructions"] = string.Format("Please review this proposed marketing campaign and let {0} know if there are any scheduling issues. Thanks!", sOriginator);
        }

        public Guid taskID = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties taskProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        private void taskComplete(object sender, ConditionalEventArgs e)
        {
            e.Result = !taskCompleted;

        }

        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties afterProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties beforeProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        private void onTaskChanged(object sender, ExternalDataEventArgs e)
        {
            if (afterProperties.ExtendedProperties.ContainsValue("Completed"))
            {
                taskCompleted = true;
            }
        }
	}

}
