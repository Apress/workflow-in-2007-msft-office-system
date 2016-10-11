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
 *                  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * 
 * Modifications by DMann for book: Workflow in the 2007 Office System (Apress, 2007)
 * Date: 12/16/2006
 * 
 * Required Modifications:
 *  1. Update namespace to match your workflow class' namespace
 *  2. Add property getters/setters for each field control
 *  
 * 
 * File should then be usable for most ASPX forms workflows.
 * 
 * Some highly customized workflows may need additional modifications.
 * 
 * See Chapter 7 for assistance using this file as a template
 * for your ASPX forms based workflows.
 ***********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace KCDHoldings.Templates	//Step 1: Update Namespace
{
    /* 
     * Contacts class supports the Contact Selector control.  See chapter 9.
     */

    [Serializable()]
    public class Contacts
    {
        private List<string> contacts;

        public List<string> ContactList
        {
            get { return contacts; }
            set { contacts = value; }
        }
        public void AddContact(string contact)
        {
            if (this.contacts == null)
            {
                this.contacts = new List<string>();
            }
            this.contacts.Add(contact);
        }

    }
    

        [Serializable()]
        public class FormData
        {
            /*
             * Step 2: Add property getters/setters for each field control.
             * ex:
                    private string trafficCoordinator = default(string);
                    public string TrafficCoordinator
                    {
                        get { return this.trafficCoordinator; }
                        set { this.trafficCoordinator = value; }
                    }
             */



           /* Support for the Contact Selector control.  See chapter 9.
            
            private Contacts reviewers = default(Contacts);
            public Contacts Reviewers
            {
                get
                {
                    return this.reviewers;
                }
                set
                {
                    this.reviewers = value;
                }
            }

            public void AddContact(string contact)
            {
                if (this.reviewers == null)
                {
                    this.reviewers = new Contacts();
                }
                this.reviewers.AddContact(contact);
            }
            public string[] GetReviewers()
            {
                return this.reviewers.ContactList.ToArray();
            }

            */

        }
        
    
    public class Helper
    {
        public static FormData DeserializeFormData(string xmlString)
        {
          using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FormData));
                    FormData data = (FormData)serializer.Deserialize(stream);
                    return data;
                }
        }
    }
}
