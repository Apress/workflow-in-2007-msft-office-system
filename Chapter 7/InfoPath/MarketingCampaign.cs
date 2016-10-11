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
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.Office.Workflow.Utility;

namespace MarketingCampaign
{
	public sealed partial class MarketingCampaign: SharePointSequentialWorkflowActivity
	{
		public MarketingCampaign()
		{
			InitializeComponent();
		}

        public Guid workflowId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();
        public Guid taskID = default(System.Guid);
        public SPWorkflowTaskProperties taskProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties beforeProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties afterProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        private string sTrafficCoordinator = default(String);
        private string sMarketingDirectorEMail = default(String);
        private bool taskCompleted = false;

        private void onWorkflowActivated(object sender, ExternalDataEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InitForm));
            XmlTextReader rdrInitForm = new XmlTextReader(new System.IO.StringReader(workflowProperties.InitiationData));
            InitForm frmInit = (InitForm)serializer.Deserialize(rdrInitForm);

            sTrafficCoordinator = @"mossrtm\" + frmInit.trafficcoordinator;
            sMarketingDirectorEMail = frmInit.marketingdirectoremail;
        }

        private void onSendEmail(object sender, EventArgs e)
        {
            sendEmail1.To = sMarketingDirectorEMail + "@kcdholdings.com";
            string sItemTitle = workflowProperties.Item["Name"].ToString();
            string sItemURL = workflowProperties.ItemUrl;
            sendEmail1.Body = string.Format("New Marketing Campaign: {0}. URL:{1}",sItemTitle, sItemURL);
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
            taskProperties.ExtendedProperties["taskinstructions"] = string.Format("Please review this proposed marketing campaign and let {0} know if there are any scheduling issues.Thanks!", sOriginator);
        }


        private void taskComplete(object sender, ConditionalEventArgs e)
        {
            e.Result = !taskCompleted;
        }

       
        private void onTaskChanged(object sender, ExternalDataEventArgs e)
        {
            taskCompleted= bool.Parse(afterProperties.ExtendedProperties["taskcompleted"].ToString());
            taskProperties.ExtendedProperties["taskcomments"] = afterProperties.ExtendedProperties["taskcomments"].ToString();
        }
	}

}
