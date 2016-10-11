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
 *                  Template for ASP.Net Workflow Forms
 *                  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * 
 * Modifications by DMann for book: Workflow in the 2007 Office System (Apress, 2007)
 * Date: 12/16/2006
 * 
 * Required Modifications:
 *  1. Update namespace to match your workflow class' namespace
 *  2: Create object for form controls
 *  3: Set field/control values from deserialized form
 *  4: Set object members based on data submitted with form.
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
using System.Xml.Serialization;


using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Workflow;


namespace MarketingCampaignASP2     // Step 1: Update Namespace
{
    public enum FormType
    {
        Association,
        Initiation,
        ModificationReviewers
    }
    public class WFDataPages : LayoutsPageBase
    {        
        //Controls
        protected SPList List;
        protected SPContentType m_ct;

        /* Step 2: Create objects for form controls.  Names and types must match ASPX page entries.
         * Objects are created here so that they are accessible from both Association and Initiation forms (which
         * both inherit from this class).  If a field is not necessary on one of the forms, simply do not declare
         * it in the ASPX file.
         * Ex: protected HtmlInputText TrafficCoordinator = new HtmlInputText();
       */

        protected HtmlInputText TrafficCoordinator = new HtmlInputText();
        protected HtmlInputText MarketingDirector = new HtmlInputText();
        
        protected string m_strQueryParams;
        protected bool m_bContentTypeTemplate = false;
        

        #  region OnLoad

        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);

            //Ensure the we get teh context variables
            EnsureRequestParamsParsed();

            //Check for permissions
           SPBasePermissions perms = SPBasePermissions.Open | SPBasePermissions.ViewPages;
            if (m_bContentTypeTemplate)
                perms |= SPBasePermissions.AddAndCustomizePages;
            else
                perms |= SPBasePermissions.ManageLists;
            Web.CheckPermissions(perms);
        }

        
            //Ensure the we get the context variables
            protected void EnsureRequestParamsParsed()
            {
                string strListID = Request.QueryString["List"];
                string strCTID = Request.QueryString["ctype"];
                if (strListID != null)
                    List = Web.Lists[new Guid(strListID)];
                if (strCTID != null)
                {
                    m_strQueryParams = "?ctype=" + strCTID;
                    if (List != null)
                    {
                        m_strQueryParams += "&List=" + strListID;
                        m_ct = List.ContentTypes[new SPContentTypeId(strCTID)];
                    }
                    else
                    {
                        m_ct = Web.ContentTypes[new SPContentTypeId(strCTID)];
                        m_bContentTypeTemplate = true;
                    }
                }
                else
                    m_strQueryParams = "?List=" + strListID;
            }
        #endregion

        #region Form Data Serialization

        // Deserializes the association xml string and populates the fields in the form
        // Should only be used for association and initiation forms, since modification
        // forms don't have the same fields
        internal void PopulatePageFromXml(string associationXml, FormType type)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FormData));
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(associationXml));
            FormData formdata = (FormData)serializer.Deserialize(reader);

            /* Step 3: Set field/control values from deserialized form
             * Both Association and Initiation forms use this method, so include a line for all fields.
             * If a field is not used on a particular form, the ASPX page will ensure that it doesn't show
             * up, even if you set a value here
            ex: 
             * TrafficCoordinator.Value = formdata.TrafficCoordinator;
            */

            TrafficCoordinator.Value = formdata.TrafficCoordinator;
            MarketingDirector.Value = formdata.MarketingDirector;

        }

        // Serializes form data into an xml string to be passed into the workflow
        internal string SerializeFormToString(FormType type)
        {
            FormData data = new FormData();
            if (type == FormType.Association)
            {
                /* Step 4a: Set object members based on data submitted with form - one entry per form field.
                 * Here we need separate entries for Association and Initiation forms as they 
                 * potentially submit different fields.
                 * Here we cover Association forms
                 * ex:
                        data.TrafficCoordinator = TrafficCoordinator.Value;
                */

                data.TrafficCoordinator = TrafficCoordinator.Value;
                data.MarketingDirector = MarketingDirector.Value;
            }
            else if (type == FormType.Initiation)
            {
                /* Step 4b: Same as 4a, except for Initiation forms
                ex:
                    data.TrafficCoordinator = TrafficCoordinator.Value;
                */

                data.TrafficCoordinator = TrafficCoordinator.Value;
                data.MarketingDirector = MarketingDirector.Value;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FormData));
                serializer.Serialize(stream, data);
                stream.Position = 0;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(bytes);
            }

        }

        #endregion

        #region Url Helper Functions

        public string strGroup = "Group";
        public string StrGetRelativeUrl(System.Web.UI.Page pgIn, string strPage, string strGrpName)
        {
            //            ULS.Assert(FValidString(strPage));
            string strUrl = StrGetWebRelativePath(pgIn) + strPage;
            // No need to UrlEncodeAsUrl strUrl as it would have already been encoded
            if (FValidString(strGrpName))
                strUrl += "?" + SPHttpUtility.UrlKeyValueEncode(strGroup) + "=" + SPHttpUtility.UrlKeyValueEncode(strGrpName);

            return strUrl;
        }
        public string StrGetWebRelativePath(System.Web.UI.Page pgIn)
        {
            string strT = SPUtility.OriginalServerRelativeRequestUrl;

            int iLastSlash = strT.LastIndexOf("/");
            if (iLastSlash > 0)
            {
                strT = SPHttpUtility.UrlPathEncode(SPHttpUtility.UrlPathDecode(strT, /*allowHashParameter*/ true),
                    /*allowHashParameter*/ true);
                iLastSlash = strT.LastIndexOf("/");
                return strT.Substring(0, iLastSlash + 1);
            }
            else
                return string.Empty;
        }
        public bool FValidString(string strIn)
        {
            return FValidString(strIn, 2048);
        }
        public bool FValidString(string strIn, uint nMaxLength)
        {
            return (strIn != null && strIn.Length > 0 && strIn.Length <= nMaxLength);
        }
        #endregion
    }
}