using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Xml;
using Microsoft.SharePoint.Workflow;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class MyServiceClass : System.Web.Services.WebService
{
    public MyServiceClass()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public XmlDocument GetWFTasks(string sSiteURL, string sDocLibName, string sFolder, string sDocName,string sUser)
    {
               //temp debugging code:
        if (sUser.ToLower() == @"basexp\admin")
        {
            sUser = @"mossrtm\administrator";
        }
        sUser = @"mossrtm\administrator";
        //end temp debugging code
        
        string sTaskName = string.Empty;
        string sTaskURL = string.Empty;
        XmlDocument xDoc = new XmlDocument();
        XmlElement xElem = null;
        XmlText xText = null;
        XmlNode xNode = null;
        //SPUser usr = new SPUser(
        //this.User.
        SPSite site = new SPSite(sSiteURL);
        SPWeb web = site.OpenWeb();
        SPDocumentLibrary doclib = (SPDocumentLibrary)web.Lists[sDocLibName];
        SPListItem item = null;
        foreach (SPListItem itemTemp in doclib.Items)
        {
            if (sFolder == string.Empty)
            {
                if (itemTemp.File.Name.ToLower() == sDocName.ToLower())
                {
                    item = itemTemp;
                    break;
                }
            }
            else
            {
                if ((itemTemp.Folder.Name == sFolder)
                    && (itemTemp.File.Name.ToLower() == sDocName.ToLower())
                    )
                {
                    item = itemTemp;
                    break;
                }
            }
        }
        //string sCurrentUser = this.User.Identity.Name.ToLower();
        
        string sTaskAssignedTo = string.Empty;
        for (int i = 0; i < item.Workflows.Count; i++)
        {
            SPWorkflow wf = item.Workflows[i];
            if (!wf.IsCompleted)
            {
                wf.TaskFilter = new SPWorkflowFilter(SPWorkflowState.Running,SPWorkflowState.None);
                for (int j = 0; j < wf.Tasks.Count; j++)
                {
                    SPWorkflowTask task = item.Workflows[i].Tasks[j];
                    sTaskAssignedTo = task["AssignedTo"].ToString();
                    sTaskAssignedTo = sTaskAssignedTo.Substring(sTaskAssignedTo.IndexOf('#') + 1);
                    if (sUser.ToLower() == sTaskAssignedTo.ToLower())
                    {
                        sTaskName = task.DisplayName;
                        
                        sTaskURL = sSiteURL.TrimEnd('/') + "/Lists/" + wf.TaskList.Title + "/DispForm.aspx?ID=" + task.ID.ToString();
                        break;
                    }
                }
            }
        }

        xNode = xDoc.CreateNode(XmlNodeType.XmlDeclaration, string.Empty, string.Empty);
        xDoc.AppendChild(xNode);
        xElem = xDoc.CreateElement("WorkFlowTask");
        xDoc.AppendChild(xElem);
        xElem = xDoc.CreateElement("TaskName");
        xText = xDoc.CreateTextNode(sTaskName);
        xElem.AppendChild(xText);
        xDoc.ChildNodes.Item(1).AppendChild(xElem);
        xElem = xDoc.CreateElement("TaskURL");
        xText = xDoc.CreateTextNode(sTaskURL);
        xElem.AppendChild(xText);
        xDoc.ChildNodes.Item(1).AppendChild(xElem);

        return xDoc; 
    }
    
}
