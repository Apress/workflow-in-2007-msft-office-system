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
 *                  Template for ASP.Net Workflow Association Form
 *                  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * 
 * Modifications by DMann for book: Workflow in the 2007 Office System (Apress, 2007)
 * Date: 12/16/2006
 * 
 * Required Modifications:
 *  1. Update namespace to match your workflow class' namespace
 * 
 * File should then be usable for most ASPX forms workflows.
 * 
 * Some highly customized workflows may need additional modifications.
 * 
 * See Chapter 7 for assistance using this file as a template
 * for your ASPX forms based workflows.
 ***********************************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Workflow;

namespace KCDHoldings.Templates     //Step 1: Update Namespace
{
    public class AssocForm : WFDataPages
    {
        protected string m_workflowName;
        protected bool m_allowManual;
        protected bool m_autoStartCreate;
        protected bool m_autoStartChange;
        protected Guid m_taskListId;
        protected string m_taskListName;
        protected Guid m_historyListId;
        protected string m_historyListName;
        protected bool m_setDefault;
        protected bool m_updateLists;
        protected bool m_bLockItem;
        protected SPWorkflowTemplate m_baseTemplate;
        protected SPWorkflowAssociation m_assocTemplate;
        protected HyperLink hlReturn;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (m_assocTemplate != null)
                PopulatePageFromXml((string)m_assocTemplate.AssociationData, FormType.Association);

        }
        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);
            //Get the Workflow Name
            m_workflowName = Request.Params["WorkflowName"];

            FetchAssociationInfo();
            GetTaskAndHistoryList();
        }

        public override void Validate()
        {
            base.Validate();
            IfManualNoRequiredFields();
        }

        #region Fetch Methods

        private void FetchAssociationInfo()
        {
            SPWorkflowAssociationCollection wac;
            m_baseTemplate = Web.WorkflowTemplates[new Guid(Request.Params["WorkflowDefinition"])];
            m_assocTemplate = null;

            // figure out whether or not this is associating with a content type or a list,
            // and get the association collection.  Also set the return page values.
            if (m_ct != null) // associating w/ a content type
            {
                wac = m_ct.WorkflowAssociations;
                hlReturn.Text = m_ct.Name;
                hlReturn.NavigateUrl = "ManageContentType.aspx" + m_strQueryParams;
            }
            else   // associating w/ a list
            {
                List.Permissions.CheckPermissions(SPRights.ManageListPermissions);
                wac = List.WorkflowAssociations;
                hlReturn.Text = List.Title;
                hlReturn.NavigateUrl = List.DefaultViewUrl;
            }
            if (wac == null || wac.Count < 0)
            {
                throw new SPException("No Associations Found");
            }
            //Set the general workflow properties
            m_autoStartCreate = (Request.Params["AutoStartCreate"] == "ON");
            m_autoStartChange = (Request.Params["AutoStartChange"] == "ON");
            m_allowManual = (Request.Params["AllowManual"] == "ON");
            m_bLockItem = (Request.Params["LockItem"] == "ON");
            m_setDefault = (Request.Params["SetDefault"] == "ON");
            m_updateLists = (Request.Params["UpdateLists"] == "TRUE");
            // check if workflow association already exists.
            string strGuidAssoc = Request.Params["GuidAssoc"];
            if (strGuidAssoc != string.Empty)
            {
                m_assocTemplate = wac[new Guid(strGuidAssoc)];
            }

            // Check for a duplicate name. Note that we do this both on initial load and on
            // postback. The reason we do it on postback is that some other user may have
            // added a workflow by this name since the last time we checked.
            SPWorkflowAssociation dupTemplate = wac.GetAssociationByName(m_workflowName, Web.Locale);
            if (dupTemplate != null && (m_assocTemplate == null || m_assocTemplate.Id != dupTemplate.Id))
            {
                // throw an exception if there is a template with the same name already
                throw new SPException("Duplicate workflow name is detected.");
            }
        }

        private void GetTaskAndHistoryList()
        {
            if (m_bContentTypeTemplate)
            {
                m_taskListName = Request.Params["TaskList"];
                m_historyListName = Request.Params["HistoryList"];
            }
            else
            {

                // If the user has requested that a new task or history list be created, check
                // that the name does not duplicate the name of an existing list. If it does, show
                // the user an appropriate error page.

                string taskListGuid = Request.Params["TaskList"];
                if (taskListGuid[0] != 'z') // already existing list
                {
                    m_taskListId = new Guid(taskListGuid);
                }
                else  // new list
                {
                    SPList list = null;
                    m_taskListName = taskListGuid.Substring(1);
                    try
                    {
                        list = Web.Lists[m_taskListName];
                    }
                    catch (ArgumentException)
                    {
                    }

                    if (list != null)
                        throw new SPException("A list already exists with the same name as that proposed for the new task list. Use your browser's Back button and either change the name of the workflow or select an existing task list.&lt;br&gt;");
                }

                // do the same for the history list
                string strHistoryListGuid = Request.Params["HistoryList"];
                if (strHistoryListGuid[0] != 'z') // user selected already existing list
                {
                    m_historyListId = new Guid(strHistoryListGuid);
                }
                else // user wanted a new list
                {
                    SPList list = null;

                    m_historyListName = strHistoryListGuid.Substring(1);

                    try
                    {
                        list = Web.Lists[m_historyListName];
                    }
                    catch (ArgumentException)
                    {
                    }
                    if (list != null)
                        throw new SPException("A list already exists with the same name as that proposed for the new history list. Use your browser's Back button and either change the name of the workflow or select an existing history list.&lt;br&gt;");
                }
            }
        }
        #endregion

        #region Button Click handlers

        public void BtnOK_Click(object sender, EventArgs e)
        {
            SPList taskList = null;
            SPList historyList = null;
            if (!IsValid)
                return;
            if (!m_bContentTypeTemplate)
            {
                // If the user requested a new task list, create it.
                if (m_taskListId == Guid.Empty)
                {
                    string description = string.Format("Task list for the {0} workflow.", m_workflowName);
                    m_taskListId = Web.Lists.Add(m_taskListName, description, SPListTemplateType.Tasks);
                }

                // If the user requested a new history list, create it.
                if (m_historyListId == Guid.Empty)
                {
                    string description = string.Format("History list for the {0} workflow.", m_workflowName);
                    m_historyListId = Web.Lists.Add(m_historyListName, description, SPListTemplateType.WorkflowHistory);
                }
                taskList = Web.Lists[m_taskListId];
                historyList = Web.Lists[m_historyListId];
            }

            // perform association (if it doesn't already exist)
            bool isNewAssociation;
            if (m_assocTemplate == null)
            {
                isNewAssociation = true;
                if (!m_bContentTypeTemplate)
                    m_assocTemplate = SPWorkflowAssociation.CreateListAssociation(m_baseTemplate,
                                                                  m_workflowName,
                                                                  taskList,
                                                                  historyList);
                else
                {
                    m_assocTemplate = SPWorkflowAssociation.CreateSiteContentTypeAssociation(m_baseTemplate,
                                                                  m_workflowName,
                                                                  taskList.Title,
                                                                  historyList.Title);
                }
            }
            else // modify existing template
            {
                isNewAssociation = false;
                m_assocTemplate.Name = m_workflowName;
                m_assocTemplate.SetTaskList(taskList);
                m_assocTemplate.SetHistoryList(historyList);
            }

            // set up starup parameters in the template
            m_assocTemplate.Name = m_workflowName;
            m_assocTemplate.LockItem = m_bLockItem;
            m_assocTemplate.AutoStartCreate = m_autoStartCreate;
            m_assocTemplate.AutoStartChange = m_autoStartChange;
            m_assocTemplate.AllowManual = m_allowManual;
            if (m_assocTemplate.AllowManual)
            {
                SPBasePermissions newPerms = SPBasePermissions.EmptyMask;

                if (Request.Params["ManualPermEditItemRequired"] == "ON")
                    newPerms |= SPBasePermissions.EditListItems;
                if (Request.Params["ManualPermManageListRequired"] == "ON")
                    newPerms |= SPBasePermissions.ManageLists;

                m_assocTemplate.PermissionsManual = newPerms;
            }

            // place data from form into the association template
            m_assocTemplate.AssociationData = SerializeFormToString(FormType.Association);// SerializeFormToString(m_assocTemplate.AssociationData, FormType.Association);

            // if its a content type association, add the template to the content type
            if (m_ct != null)
            {
                if (isNewAssociation)
                    m_ct.AddWorkflowAssociation(m_assocTemplate);
                else
                    m_ct.UpdateWorkflowAssociation(m_assocTemplate);
                if (m_updateLists)
                    m_ct.UpdateWorkflowAssociationsOnChildren(false);
            }
            else// Else, if this is a list association,
            {
                if (isNewAssociation) //if new association
                    List.AddWorkflowAssociation(m_assocTemplate);
                else
                    List.UpdateWorkflowAssociation(m_assocTemplate);

                if (m_assocTemplate.AllowManual && List.EnableMinorVersions)
                {
                    //In case you selected this WF to be the content approval WF (m_setDefault = true see association page)
                    if (List.DefaultContentApprovalWorkflowId != m_assocTemplate.Id && m_setDefault)
                    {
                        if (!List.EnableModeration)
                            List.EnableModeration = true;
                        List.DefaultContentApprovalWorkflowId = m_assocTemplate.Id;
                        List.Update();
                    }
                    else if (List.DefaultContentApprovalWorkflowId == m_assocTemplate.Id && !m_setDefault)
                    {
                        // Reset the DefaultContentApprovalWorkflowId
                        List.DefaultContentApprovalWorkflowId = Guid.Empty;
                        List.Update();
                    }
                }
            }
            string strUrl = StrGetRelativeUrl(this, "WrkSetng.aspx", null)
                            + m_strQueryParams;
            Response.Redirect(strUrl);
        }
        #endregion

        private void IfManualNoRequiredFields()
        {
            if (Request.Form["AutoStartCreate"] == "ON" ||
                 Request.Form["AutoStartChange"] == "ON")
            {
                /* Workflow could start automatically (i.e. with no Initiation form)
                 * so we have   to make sure we collect all required information here
                 * Set required attributes/properties here.
                        ex: Reviewers.AllowEmpty = false;
                 * (Where Reviewers is a Contact Selector control.)
                 * 
                */
            }
        }
    }
}
