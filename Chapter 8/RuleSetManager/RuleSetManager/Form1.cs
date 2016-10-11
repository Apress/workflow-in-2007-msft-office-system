using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Workflow.Activities.Rules.Design;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Serialization;

namespace RuleSetManager
{
    public partial class Form1 : Form
    {
        List<WorkflowData> WFDataCollection = new List<WorkflowData>();
        string sWorkflowName = string.Empty;
        string sSiteURL = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetWorkflows_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            sSiteURL = txtSiteURL.Text;
            sSiteURL.Replace(@"\","/");
            if (!sSiteURL.EndsWith("/"))
            {
                sSiteURL += "/";
            }
            SPSite site = new SPSite(sSiteURL);
            SPWeb web = site.OpenWeb();
            WorkflowData wfd = null;
            SPWorkflowTemplateCollection wftc = web.WorkflowTemplates;
            string sAssemblyName = string.Empty;
            foreach (SPWorkflowTemplate wft in wftc)
            {
                sAssemblyName = GetAssemblyName(wft.Id.ToString());
                if (sAssemblyName != string.Empty)
                {
                    Assembly a = Assembly.Load(sAssemblyName);
                    foreach (AssemblyName mod in a.GetReferencedAssemblies())
                    {
                        if (mod.FullName.ToLower().Contains("externalpolicy"))
                        {
                            wfd = new WorkflowData();
                             wfd.Name = wft.Name;
                             wfd.ID = wft.Id;
                             WFDataCollection.Add(wfd);
                             lbWorkflows.Items.Add(wfd.Name);
                        }
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void cmboExistingRuleSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboExistingRulesets.SelectedIndex == 0)
            {
                btnRuleSet.Text = "Create RuleSet";
                lblRuleSetName.Enabled = true;
                txtRuleSetName.Enabled = true;
            }
            else
            {
                btnRuleSet.Text = "Edit RuleSet";
                lblRuleSetName.Enabled = false;
                txtRuleSetName.Enabled = false;
            }
        }

        private string GetAssemblyName(string WFID)
        {
            string sRetVal = string.Empty;
            foreach (string sFileName in Directory.GetFiles(@"C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\TEMPLATE\FEATURES\", "workflow.xml", SearchOption.AllDirectories))
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(sFileName);
                    XmlNamespaceManager nsMgr = new XmlNamespaceManager(xDoc.NameTable);
                    nsMgr.AddNamespace("def", "http://schemas.microsoft.com/sharepoint/");
                    string wfid = xDoc.SelectSingleNode("/def:Elements/def:Workflow/@Id", nsMgr).InnerText;
                    if (wfid.ToLower() == WFID.ToLower())
                    {
                        sWorkflowName = xDoc.SelectSingleNode("/def:Elements/def:Workflow/@Name", nsMgr).InnerText;
                        sRetVal = xDoc.SelectSingleNode("/def:Elements/def:Workflow/@CodeBesideAssembly", nsMgr).InnerText;
                        break;
                    }
                }
                catch { }

            }
            return sRetVal;
        }

        private void btnRuleSet_Click(object sender, EventArgs e)
        {
            RuleSet rs = null;
            if (cmboExistingRulesets.SelectedItem.ToString() == "<New>")
            {
                if (
                    (txtRuleSetName.Text == null)
                    || (txtRuleSetName.Text == string.Empty)
                )
                {
                    MessageBox.Show("Please specify a name for the RuleSet");
                }
                else
                {
                    rs = new RuleSet(txtRuleSetName.Text);
                }
            }
            else
            {
                string sFullURL = sSiteURL + "Rules/" + cmboExistingRulesets.SelectedItem + ".ruleset";
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                XmlUrlResolver resolver = new XmlUrlResolver();
                System.Net.NetworkCredential myCredentials;
                myCredentials = new System.Net.NetworkCredential("Administrator","password", "MossRTM");
                resolver.Credentials = myCredentials;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = resolver;
                XmlReader reader = XmlReader.Create(sFullURL, settings);
                rs = (RuleSet)serializer.Deserialize(reader);
                reader.Close();
            }
            if (rs != null)
            {
                RuleSetDialog ruleSetDialog = new RuleSetDialog(WFDataCollection[lbWorkflows.SelectedIndex].AssemblyType, null, rs);
                DialogResult result = ruleSetDialog.ShowDialog();
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                if (result == DialogResult.OK)
                {
                    rs = ruleSetDialog.RuleSet;
                    if (rs.Rules.Count > 0)
                    {
                        MemoryStream ms = new MemoryStream();
                        string sFilename = rs.Name + ".ruleset";
                        XmlWriter writer2 = XmlWriter.Create(ms);
                        serializer.Serialize(writer2, rs);
                        writer2.Flush();
                        SPSite site = new SPSite(sSiteURL);
                        SPWeb web = site.OpenWeb();
                        SPDocumentLibrary dl = (SPDocumentLibrary)web.Lists["Rules"];
                        SPFile newfile = dl.RootFolder.Files.Add(sFilename, ms.ToArray(), true);
                        newfile.Item["WorkflowName"] = sWorkflowName;
                        newfile.Item.Update();
                        writer2.Close();
                        MessageBox.Show("RuleSet Saved");
                        cmboExistingRulesets.Items.Add(rs.Name);
                        cmboExistingRulesets.SelectedItem = rs.Name;
                        txtRuleSetName.Text = string.Empty;
                    }
                }
            }
        }

        private void lbWorkflows_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            cmboExistingRulesets.Items.Clear();
            pnlRuleSets.Enabled = false;
            if (WFDataCollection[lbWorkflows.SelectedIndex].AssemblyName == null)
            {
                WFDataCollection[lbWorkflows.SelectedIndex].AssemblyName = GetAssemblyName(WFDataCollection[lbWorkflows.SelectedIndex].ID.ToString());
            }
            Assembly a = Assembly.Load(WFDataCollection[lbWorkflows.SelectedIndex].AssemblyName);
            foreach (Type type in a.GetTypes())
            {
                if ((type.BaseType.Name.ToLower() == "sequentialworkflowactivity") || (type.BaseType.Name.ToLower() == "statemachineworkflowactivity"))
                {
                    WFDataCollection[lbWorkflows.SelectedIndex].AssemblyType = type;
                    break;
                }
            }
            SPSite site = new SPSite(sSiteURL);
            SPWeb web = site.OpenWeb();
            SPDocumentLibrary dl = (SPDocumentLibrary)web.Lists["Rules"];
            SPQuery qry = new SPQuery();


            qry.Query = string.Format(@"<Where><Eq><FieldRef Name='WorkflowName' /><Value Type='Text'>{0}</Value></Eq></Where>", WFDataCollection[lbWorkflows.SelectedIndex].AssemblyType.Name);
            SPListItemCollection lic = dl.GetItems(qry);
            if (lic.Count > 0)
            {
                foreach (SPListItem li in lic)
                {
                    cmboExistingRulesets.Items.Add(Path.GetFileNameWithoutExtension(li.File.Name));
                }
            }
            cmboExistingRulesets.Items.Insert(0, "<New>");
            cmboExistingRulesets.SelectedIndex = 0;
            pnlRuleSets.Enabled = true;
            Cursor.Current = Cursors.Default;
        }
            
    }


    public class WorkflowData
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private Guid _id;
        public Guid ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _assemblyName;
        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }
        private Type _assemblyType;
        public Type AssemblyType
        {
            get { return _assemblyType; }
            set { _assemblyType = value; }
        }
    }

}