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

using System.IO;
using System.IO.Packaging;
using Microsoft.SharePoint;
using System.Xml;
using System.Runtime.Serialization;

namespace KCD.SharePoint.Activities
{
    [ToolboxItem(typeof(MacroStripperToolboxItem))]
    [ActivityValidator(typeof(MacroStripperActivityValidator))]
    [Designer(typeof(MacroStripperDesigner))]
    public partial class MacroStripperActivity: Activity
	{
		public MacroStripperActivity()
		{
			InitializeComponent();
		}

        public static DependencyProperty PayloadItemProperty =DependencyProperty.Register("PayloadItem", typeof(Microsoft.SharePoint.SPListItem),typeof(KCD.SharePoint.Activities.MacroStripperActivity));
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("List Item the Workflow is operating upon")]
        [CategoryAttribute("Configuration")]
        public SPListItem PayloadItem
        {
            get
            {
                return((SPListItem)(base.GetValue(KCD.SharePoint.Activities.MacroStripperActivity.PayloadItemProperty)));
            }
            set
            {
                base.SetValue(KCD.SharePoint.Activities.MacroStripperActivity.PayloadItemProperty, (SPListItem)value);
            }
        }

        private string _finalDocumentName;
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(false)]
        [DescriptionAttribute("Name of macro-free document in Document Library")]
        [CategoryAttribute("Configuration")]
        public string FinalDocumentName
        {
            get { return _finalDocumentName; }
            set { _finalDocumentName = value; }
        }

        private string _originalDocumentName;
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(false)]
        [DescriptionAttribute("Original name of document in Document Library")]
        [CategoryAttribute("Configuration")]
        public string OriginalDocumentName
        {
            get { return _originalDocumentName; }
            set { _originalDocumentName = value; }
        }

        private SPList _parentList;
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(false)]
        [DescriptionAttribute("SPList item containing document")]
        [CategoryAttribute("Configuration")]
        public SPList ParentList
        {
            get { return _parentList; }
            set { _parentList = value; }
        }

        private bool _isMacroFree = false;
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(false)]
        [DescriptionAttribute("Indicates whether resulting document is free of macros")]
        [CategoryAttribute("Configuration")]
        public bool IsMacroFree
        {
            get { return _isMacroFree; }
            set { _isMacroFree = value; }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
         SPFile file = PayloadItem.File;
         string sFileExtension = Path.GetExtension(file.Name);
         this.ParentList = PayloadItem.ParentList;
         if (
            (sFileExtension.ToLower() == ".docm")
            || (sFileExtension.ToLower() == ".xlsm")
            || (sFileExtension.ToLower() == ".pptm")
            )
         {
             string sNewFileExtension = string.Empty;
             switch (sFileExtension.ToLower())
             {
                 case ".docm":
                    sNewFileExtension = ".docx";
                    break;
                 case ".xlsm":
                     sNewFileExtension = ".xlsx";
                     break;
                 case ".pptm":
                     sNewFileExtension = ".pptx";
                     break;
                 default:
                    break;
             }
             try
             {
                 this.OriginalDocumentName = file.Name;
                 Stream strmFile = file.OpenBinaryStream();
                 RemoveMacros(strmFile, sFileExtension);
                 PayloadItem.ParentList.RootFolder.Files.Add( PayloadItem.Url.Replace(sFileExtension, sNewFileExtension), strmFile, PayloadItem.Properties, true);
                 PayloadItem.Delete();
                 this.FinalDocumentName = Path.GetFileName(file.Name).Replace(sFileExtension, sNewFileExtension);
                 this.IsMacroFree = true;
             }
             catch (Exception ex)
             {
                 this.FinalDocumentName = Path.GetFileName(file.Name);
                 this.IsMacroFree = false;
             }
         }
         else
         {
             this.FinalDocumentName = Path.GetFileName(file.Name);
             this.IsMacroFree = true;
         }
         return ActivityExecutionStatus.Closed;
        }

        private void RemoveMacros(Stream fs, string sFileExtension)
        {
             // Adapted from code in the Open XML File Formats Code Snippets
             // provided by Microsoft
             const string relationshipType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
             const string vbaRelationshipType = @"http://schemas.microsoft.com/office/2006/relationships/vbaProject";
             const string relationshipNamespace = @"http://schemas.openxmlformats.org/package/2006/relationships";
             const string vbaFreeRelsContentType = @"application/vnd.openxmlformats-package.relationships+xml";
             string vbaFreeContentType = string.Empty;
             Uri relsUri = null;
             switch (sFileExtension.ToLower())
             {
                 case ".docm":
                     vbaFreeContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml";
                     relsUri = new Uri("/word/_rels/document.xml.rels", UriKind.Relative);
                     break;
                 case ".xlsm":
                     vbaFreeContentType = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
                     relsUri = new Uri("/xl/_rels/workbook.xml.rels", UriKind.Relative);
                     break;
                 case ".pptm":
                     vbaFreeContentType = @"application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml";
                     relsUri = new Uri("/ppt/_rels/presentation.xml.rels", UriKind.Relative);
                     break;
                 default:
                    break;
             }
             if ((vbaFreeContentType != string.Empty) && (relsUri != null))
             {
                using (Package onePackage = Package.Open(fs, FileMode.Open, FileAccess.ReadWrite))
                {
                    PackagePart startPart = null;
                    Uri startPartUri = null;
                    foreach (System.IO.Packaging.PackageRelationship relationship in onePackage.GetRelationshipsByType(relationshipType))
                    {
                        startPartUri = PackUriHelper.ResolvePartUri(new Uri("/",
                        UriKind.Relative), relationship.TargetUri);
                        startPart = onePackage.GetPart(startPartUri);
                        break;
                    }
                    PackagePart relsPart = onePackage.GetPart(relsUri);
                    foreach (System.IO.Packaging.PackageRelationship relationship in startPart.GetRelationshipsByType(vbaRelationshipType))
                    {
                        Uri vbaUri = PackUriHelper.ResolvePartUri(startPartUri, relationship.TargetUri);
                        onePackage.DeletePart(vbaUri);
                        break;
                    }
                    NameTable nt = new NameTable();
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                    nsManager.AddNamespace("r", relationshipNamespace);
                    XmlDocument xDocRels = new XmlDocument(nt);
                    xDocRels.Load(relsPart.GetStream());
                    XmlNode vbaNode = xDocRels.SelectSingleNode(@"//r:Relationship[@Target='vbaProject.bin']", nsManager);
                    if (vbaNode != null)
                    {
                        vbaNode.ParentNode.RemoveChild(vbaNode);
                    }
                    XmlDocument xdoc = new XmlDocument(nt);
                    xdoc.Load(startPart.GetStream());
                    onePackage.DeletePart(startPart.Uri);
                    relsPart = onePackage.CreatePart(relsUri, vbaFreeRelsContentType);
                    startPart = onePackage.CreatePart(startPartUri, vbaFreeContentType);
                    xDocRels.Save(relsPart.GetStream(FileMode.Create, FileAccess.Write));
                    xdoc.Save(startPart.GetStream(FileMode.Create, FileAccess.Write));
                    onePackage.Close();
                }
             }
          }

	}

    [ActivityDesignerTheme(typeof(MacroStripperDesignerTheme))]
    public class MacroStripperDesigner : ActivityDesigner
    {

        protected override Size OnLayoutSize(ActivityDesignerLayoutEventArgs e)
        {
            base.OnLayoutSize(e);
            return new Size(200, 45);
        }

        protected override Rectangle ImageRectangle
        {
            get
            {
                Rectangle rectActivity = this.Bounds;
                Size size = new Size(20, 20);
                Rectangle rectImage = new Rectangle();
                rectImage.X = rectActivity.Left + 5;
                rectImage.Y = rectActivity.Top + ((rectActivity.Height - size.Height) / 2);
                rectImage.Width = size.Width;
                rectImage.Height = size.Height;
                return rectImage;
            }
        }

        protected override Rectangle TextRectangle
        {
            get
            {
                Rectangle rectActivity = this.Bounds;
                Size size = new Size(170, 40);
                Rectangle rectText = new Rectangle();
                rectText.X = this.ImageRectangle.Right + 5;
                rectText.Y = rectActivity.Top + 2;
                rectText.Size = size;
                return rectText;
            }
        }

        protected override void Initialize(Activity activity)
        {
            base.Initialize(activity);
            Bitmap img = KCD.SharePoint.Activities.Properties.Resources.MacroStripperImage;
            this.Image = img;
        }
    }


    public class MacroStripperDesignerTheme : ActivityDesignerTheme
    {
        public MacroStripperDesignerTheme(WorkflowTheme theme): base(theme)
        {
            BackColorStart = Color.White;
            BackColorEnd = Color.LightSlateGray;
            BackgroundStyle = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            ForeColor = Color.Black;
        }
    }

    [Serializable]
    internal class MacroStripperToolboxItem : ActivityToolboxItem
    {
        public MacroStripperToolboxItem(Type type): base(type)
        {
        }
        
        private MacroStripperToolboxItem(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
            this.Description = "Remove Macros from Office 2007 Documents";
            this.Company = "KCD Holdings, Inc.";
            this.DisplayName = "Macro Stripper";
            this.Bitmap = new Bitmap(KCD.SharePoint.Activities.Properties.Resources.MacroStripper);
        }
    }


     public class MacroStripperActivityValidator : ActivityValidator
     {
         public override ValidationErrorCollection Validate(ValidationManager manager, object obj)
         {
            ValidationErrorCollection activityErrors = base.Validate(manager,obj);
            MacroStripperActivity msa = obj as MacroStripperActivity;
            if ((null != msa) && (null != msa.Parent))
            {
                if (!msa.IsBindingSet(MacroStripperActivity.PayloadItemProperty))
                {
                    if (msa.GetValue(MacroStripperActivity.PayloadItemProperty)== null)
                    {
                        activityErrors.Add(ValidationError.
                        GetNotSetValidationError("PayloadItem"));
                    }
                }
            }
            return activityErrors;
         }
    }

}
