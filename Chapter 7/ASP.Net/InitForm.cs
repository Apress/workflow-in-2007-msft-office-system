//---------------------------------------------------------------------
//  This file is part of the Enterprise Content Management Starter Kit for 2007 Office System (Beta 2 TR).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
//This source code is intended only as a supplement to Microsoft
//Development Tools and/or on-line documentation.  See these other
//materials for detailed information regarding Microsoft code samples.
// 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------

/***********************************************************************
 *                  Template for ASP.Net Workflow Initiation Form
 *                  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * 
 * Modifications by DMann for book: Workflow in the 2007 Office System (Apress, 2007)
 * Date: 12/16/2006
 * 
 * Required Modifications:
 *  1. Update namespace to match your workflow class' namespace
 *  2. Add entries for Association form-only data
 * 
 * File should then be usable for most ASPX forms workflows.
 * 
 * Some highly customized workflows may need additional modifications.
 * 
 * See Chapter 7 for assistance using this file as a template
 * for your ASPX forms based workflows.
 ***********************************************************************/


using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Workflow;

namespace MarketingCampaignASP2 // Step 1: Update Namespace
{
    public class InitForm : WFDataPages
    {
        protected string m_workflowName;
        protected SPListItem m_listItem;
        protected string m_listItemName;
        protected string m_listItemUrl;
        protected SPWorkflowAssociation m_assocTemplate;
        protected SPWorkflowTemplate m_baseTemplate;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Get the Workflow Name
            m_workflowName = Request.Params["WorkflowName"];
            // The reason we do it on postback is that some
            // other user may have added a list by this name since the last time we
            // checked.
            GetAssociationInfo();
            GetListItemInfo();
            if (!IsPostBack)
            {
                /*
                 * This page potentially posts back as a result of various events, including 
                 * the people picker validating user names (see Chapter 9).
                 * We want to have different processing depending on whether this is the initial load or 
                 * a postback.
                 */
                //Load default values for controls from the Association form (if one exists) only on initial load
                PopulatePageFromXml((string)m_assocTemplate.AssociationData, FormType.Initiation);

                /*
                 * If certain values for your workflow are set ONLY in your Association form,
                 * you still need a way to carry that information around through your initiation form
                 * so that it can be used to initiate your workflow.
                 * For example, here we add a value that exists on our Association form ("CreateTasksInSerial")
                 * into ViewState so it is accessible on subsequent page loads.  This value is retrieved from
                 * the Association Form via the PopulatePageFromXml method above.
                 *
                this.ViewState["CreateTasksInSerial"] = this.createTasksInSerial;
                */
                // Step 2a: Add entries for each field that meets this criteria (should match Step 2b)

            }
            else
            {

                /*
                 * grab any values saved on initial page load and repopulate variables.
                 * This is necessary so that the values are available for workflow initiation
                
                 * EX: this.createTasksInSerial = (bool)this.ViewState["CreateTasksInSerial"];
                 */
                // Step 2b: Add entries for each field that meets this criteria (should match Step 2a)


            }

        }

        #region Fetch Methods

        private void GetAssociationInfo()
        {
            // get ID of the association template
            Guid assocTemplateId = new Guid(Request.Params["TemplateID"]);

            // fetch the association
            m_assocTemplate = List.WorkflowAssociations[assocTemplateId];
            if (m_assocTemplate == null) // its on a content type
            {
                SPContentTypeId ctid = (SPContentTypeId)m_listItem["ContentTypeId"];
                SPContentType ct = List.ContentTypes[ctid];
                m_assocTemplate = ct.WorkflowAssociations[assocTemplateId];
            }
            // make sure we found the association
            if (m_assocTemplate == null)
                throw new SPException("The requested workflow could not be found.");

            // get base template, workflow name, and form data
            m_baseTemplate = Web.WorkflowTemplates[m_assocTemplate.BaseId];
            m_workflowName = m_assocTemplate.Name;
            string m_formData = (string)m_assocTemplate.AssociationData;
        }

        private void GetListItemInfo()
        {
            // get item workflow is being run on
            m_listItem = List.GetItemById(Convert.ToInt32(Request.Params["ID"]));

            // set URL for workflow item
            if (m_listItem.File == null)
                m_listItemUrl = Web.Url + m_listItem.ParentList.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl + "?ID=" + m_listItem.ID.ToString();
            else
                m_listItemUrl = Web.Url + "/" + m_listItem.File.Url;

            // set the list item name
            if (List.BaseType == SPBaseType.DocumentLibrary)
            {
                // if this is a doc lib, remove the extension of the file
                m_listItemName = (string)m_listItem["Name"];
                int i = m_listItemName.LastIndexOf('.');
                if (i > 0)
                    m_listItemName = m_listItemName.Substring(0, i);
            }
            else
                m_listItemName = (string)m_listItem["Title"];
        }

        #endregion

        #region Button Click Handlers
        public void BtnOK_Click(object sender, EventArgs e)
        {
            string InitData = SerializeFormToString(FormType.Initiation);
            InitiateWorkflow(InitData);
        }
        #endregion

        private void InitiateWorkflow(string InitData)
        {
            try
            {
                Web.Site.WorkflowManager.StartWorkflow(m_listItem, m_assocTemplate, InitData);
            }
            catch (Exception ex)
            {
                SPException spEx = ex as SPException;

                string errorString;

                if (spEx != null && spEx.ErrorCode == -2130575205 /* SPErrorCode.TP_E_WORKFLOW_ALREADY_RUNNING */)
                    errorString = SPResource.GetString(Strings.WorkflowFailedAlreadyRunningMessage);
                else if (spEx != null && spEx.ErrorCode == -2130575339 /* SPErrorCode.TP_E_VERSIONCONFLICT */)
                    errorString = SPResource.GetString(Strings.ListVersionMismatch);
                else if (spEx != null && spEx.ErrorCode == -2130575338 /* SPErrorCode.TP_E_LISTITEMDELETED */)
                    errorString = spEx.Message;
                else
                    errorString = SPResource.GetString(Strings.WorkflowFailedStartMessage);

                SPUtility.Redirect("Error.aspx",
                    SPRedirectFlags.RelativeToLayoutsPage,
                    HttpContext.Current,
                    "ErrorText=" + SPHttpUtility.UrlKeyValueEncode(errorString));
            }

            SPUtility.Redirect(List.DefaultViewUrl, SPRedirectFlags.UseSource, HttpContext.Current);
        }
    }
}
