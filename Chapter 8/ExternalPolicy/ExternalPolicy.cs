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
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace KCD.Workflow.Rules
{
	public partial class ExternalPolicy: SequenceActivity
	{
		public ExternalPolicy()
		{
			InitializeComponent();
		}

        public static DependencyProperty SourceSiteURLProperty = DependencyProperty.Register("SourceSiteURL",typeof(string),typeof(KCD.Workflow.Rules.ExternalPolicy));
        public string SourceSiteURL
        {
            get
            {
                return ((string)(base.GetValue(KCD.Workflow.Rules.ExternalPolicy.SourceSiteURLProperty)));
            }
            set
            {
                base.SetValue(KCD.Workflow.Rules.ExternalPolicy.SourceSiteURLProperty,value);
            }
        }

        public static DependencyProperty DocumentLibraryProperty =DependencyProperty.Register("DocumentLibrary",typeof(string),typeof(KCD.Workflow.Rules.ExternalPolicy));
            
        [Description("Name of Document Library storing Ruleset information"),Editor(typeof(docLibSelector), typeof(UITypeEditor))]
        public string DocumentLibrary
        {
            get
            {
                return ((string)(base.GetValue(KCD.Workflow.Rules.ExternalPolicy.DocumentLibraryProperty)));
            }
            set
            {
                base.SetValue(KCD.Workflow.Rules.ExternalPolicy.DocumentLibraryProperty,value);
            }
        }
            
        public static DependencyProperty RuleSetNameProperty =DependencyProperty.Register("RuleSetName", typeof(string),typeof(KCD.Workflow.Rules.ExternalPolicy));
        
        [Description("Name of RuleSet to Apply"),Editor(typeof(RuleSetSelector), typeof(UITypeEditor))]
        public string RuleSetName
        {
            get
            {
                return ((string)(base.GetValue(KCD.Workflow.Rules.ExternalPolicy.RuleSetNameProperty)));
            }
            set
            {
                base.SetValue(KCD.Workflow.Rules.ExternalPolicy.RuleSetNameProperty,value);
            }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            string sFullURL = this.SourceSiteURL + @"/" + this.DocumentLibrary + @"/" + this.RuleSetName + ".ruleset";
            WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
            XmlUrlResolver resolver = new XmlUrlResolver();
            System.Net.NetworkCredential myCredentials = new System.Net.NetworkCredential("Administrator", "password", "Mossb2tr");
            resolver.Credentials = myCredentials;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = resolver;
            XmlReader reader = XmlReader.Create(sFullURL, settings);
            RuleSet ruleSet = (RuleSet)serializer.Deserialize(reader);
            reader.Close();
            Activity targetActivity = Utility.GetRootWorkflow(this.Parent);
            RuleValidation validation = new RuleValidation(targetActivity.GetType(), null);
            RuleExecution execution = new RuleExecution(validation,targetActivity, context);
            ruleSet.Execute(execution);
            return ActivityExecutionStatus.Closed;
        }

        
	}

    public class Utility
    {

        public static CompositeActivity GetRootWorkflow(CompositeActivity activity)
        {
            if (activity.Parent != null)
            {
                CompositeActivity workflow = GetRootWorkflow(activity.Parent);
                return workflow;
            }
            else
            {
                return activity;
            }
        }
    }

    public class docLibSelector : UITypeEditor
    {
        IWindowsFormsEditorService frmEditor = null;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context,IServiceProvider provider, object value)
        {
            ExternalPolicy parent = (ExternalPolicy)context.Instance;
            string sSourceSiteURL = parent.SourceSiteURL;
            if (sSourceSiteURL != null)
            {
                frmEditor = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                ListBox lbDocLibs = new ListBox();
                SPSite site = new SPSite(sSourceSiteURL);
                SPWeb web = site.OpenWeb();
                foreach (SPDocumentLibrary dl in web.GetListsOfType(SPBaseType.DocumentLibrary))
                {
                    lbDocLibs.Items.Add(dl.Title);
                }
                lbDocLibs.SelectedValueChanged += new EventHandler(lbDocLibs_SelectedValueChanged);
                frmEditor.DropDownControl(lbDocLibs);
                return lbDocLibs.SelectedItem;
            }
            else
            {
                MessageBox.Show(@"You must specify a Source Site URL first!");
                return null;
            }
        }

        void lbDocLibs_SelectedValueChanged(object sender, EventArgs e)
        {
            frmEditor.CloseDropDown();
        }
    }

    public class RuleSetSelector: UITypeEditor
    {
        IWindowsFormsEditorService frmEditor = null;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context,IServiceProvider provider, object value)
        {
            ExternalPolicy parent = (ExternalPolicy)context.Instance;
            string sSourceSiteURL = parent.SourceSiteURL;
            string sDocLibName = parent.DocumentLibrary;
            if (
                (sSourceSiteURL != null)
                && (sDocLibName != null)
                )
            {
                frmEditor = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                ListBox lbRuleSets = new ListBox();
                SPSite site = new SPSite(sSourceSiteURL);
                SPWeb web = site.OpenWeb();
                SPDocumentLibrary dl = (SPDocumentLibrary)web.Lists[sDocLibName];
                CompositeActivity workflow = Utility.GetRootWorkflow(parent.Parent);
                SPQuery qry = new SPQuery();
                qry.Query = string.Format(@"<Where><Eq><FieldRef Name='WorkflowName' /><Value Type='Text'>{0}</Value></Eq></Where>",workflow.QualifiedName);
                SPListItemCollection lic = dl.GetItems(qry);
                if (lic.Count > 0)
                {
                    foreach (SPListItem li in lic)
                    {
                        lbRuleSets.Items.Add(Path.GetFileNameWithoutExtension(li.File.Name));
                    }
                }
                lbRuleSets.SelectedValueChanged += new EventHandler(lbRuleSets_SelectedValueChanged);
                frmEditor.DropDownControl(lbRuleSets);
                return lbRuleSets.SelectedItem;
            }
            else
            {
                MessageBox.Show(@"You must specify a Document Library first!");
                return null;
            }
        }

        void lbRuleSets_SelectedValueChanged(object sender, EventArgs e)
        {
            frmEditor.CloseDropDown();
        }
    }

}
