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

namespace DocCleaner
{
	public sealed partial class DocCleanerWF: SequentialWorkflowActivity
	{
		public DocCleanerWF()
		{
			InitializeComponent();
		}

        public Guid workflowId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();

        private void setBeginLog(object sender, EventArgs e)
        {
            this.hlogBegin.HistoryDescription = string.Format(@"
            Processing: {0}. Attempting to remove macros",
            this.workflowProperties.Item.Name);
        }

        private void sendErrorEmail(object sender, EventArgs e)
        {
            this.emlError.To = ((SPUser)this.forEach1.CurrentItem).Email;
            this.emlError.Body = string.Format(@"The following document was
                posted to {0} - {1}. The automatic process was unable to
                successfully remove the macro parts from the document package.
                This document will be unavailable to end users until it is
                manually processed.{2}",
            this.macroStripper.ParentList.ParentWeb.Title,
            this.macroStripper.ParentList.Title,
            this.macroStripper.FinalDocumentName);
        }

        private void sendAuthorEmail(object sender, EventArgs e)
        {
            this.emlAuthor.To = this.workflowProperties.OriginatorEmail;
            this.emlAuthor.Body = string.Format(@"The document ({0}) you
                recently posted to the KCD Holdings Client portal has encountered
                a problem. KCD policy prohibits the posting of documents with
                embedded macros. We were unable to automatically remove the
                macros from your document. Therefore it has been quarantined
                until it can be processed manually.",
            this.macroStripper.OriginalDocumentName);
        }

        private void setEndLog(object sender, EventArgs e)
        {
            string sHistoryOutcome = @"Processing complete: ";
            if (macroStripper.IsMacroFree)
            {
                sHistoryOutcome += "Macros successfully removed";
            }
            else
            {
                sHistoryOutcome += "Macros NOT successfully removed";
            }
            this.hlogEnd.HistoryDescription = sHistoryOutcome;
        }
	}

}
