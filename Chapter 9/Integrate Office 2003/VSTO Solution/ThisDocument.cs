using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;

namespace WFTaskDocument
{
    public partial class ThisDocument
    {

	string sSiteURL = string.Empty;
	string sDocLibName = string.Empty;
	string sDocName = string.Empty;
	string sFolders = string.Empty;
	string sTaskName = string.Empty;
	string sTaskURL = string.Empty;        

	private void ThisDocument_Startup(object sender, System.EventArgs e)
        {
		if (GetValues())
		{
			XmlNode xNode = CheckForWFTasks();
			if (GetValuesFromXML(xNode))
			{
				ShowTaskPane();
			}
		}
        }

	private bool GetValues()
	{
 		string sTemp = string.Empty;
 		string sFullName = string.Empty;
 		try
 		{
 			sFullName = this.FullName;
 			sDocName = this.Name;
 			if (this.SharedWorkspace.Connected)
 			{
 				if (this.SharedWorkspace.Folders.Count > 1)
 				{
 					for (int i = 1; i <= this.SharedWorkspace.Folders.Count;i++)
 					{
 						if (sFullName.IndexOf(SharedWorkspace.Folders[i].FolderName) > -1)
 						{
 							sTemp = this.SharedWorkspace.Folders[i].FolderName;
 							break;
 						}
 					}
 					if (sTemp == string.Empty)
 					{
						sTemp = sFullName.Replace(sDocName, String.Empty);
 						sTemp = sTemp.TrimEnd('/');
 						sDocLibName = sTemp.Substring(sTemp.LastIndexOf("/")+ 1);
 					}
 					else
 					{
 						sDocLibName = sTemp.Substring(0, sTemp.IndexOf('/'));
 					}
 					sSiteURL = this.FullName.Substring(0, this.FullName.IndexOf(sDocLibName));
 					sFolders = sTemp.Replace(sDocLibName, string.Empty);
 					sFolders = sFolders.Replace(sDocName, string.Empty);
 					sFolders = sFolders.Replace(sSiteURL, string.Empty);
 				}
 				else
 				{
 					sTemp = sFullName.Replace(sDocName, String.Empty);
 					sTemp = sTemp.TrimEnd('/');
 					sDocLibName = sTemp.Substring(sTemp.LastIndexOf("/") + 1);
 					sSiteURL = this.FullName.Substring(0, this.FullName.IndexOf(sDocLibName));
 				}
 				return true;
 			}
 			else
 			{
 				return false;
 			}
 		}
 		catch (Exception ex)
 		{
 			return false;
		}
 	}

	private XmlNode CheckForWFTasks()
	{
		WFTaskService.WFTask wft = new WFTaskService.WFTask();
		wft.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
		XmlNode xNode = wft.GetWFTasks(sSiteURL, sDocLibName, sFolders, sDocName);
		return xNode;
	}



	private bool GetValuesFromXML(XmlNode xNode)
	{
		try
		{
			sTaskName = xNode.SelectSingleNode("/TaskName/text()").Value;
			sTaskURL = xNode.SelectSingleNode("/TaskURL/text()").Value;
			if ((sTaskName != string.Empty)
				&& (sTaskURL != string.Empty)
			)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			return false;
		}
	}

	private void ShowTaskPane()
	{
		Label lblTaskName = new Label();
		lblTaskName.Text = sTaskName;
		Button btn = new Button();
		btn.BackColor = Color.Silver;
		btn.Text = "View Workflow Task";
		btn.Click += new System.EventHandler(btn_Click);
		this.ActionsPane.Controls.Add(lblTaskName);
		this.ActionsPane.Controls.Add(btn);
	}

	private void btn_Click(object sender, System.EventArgs e)
	{
		System.Diagnostics.Process.Start(sTaskURL);
	}


        private void ThisDocument_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisDocument_Startup);
            this.Shutdown += new System.EventHandler(ThisDocument_Shutdown);
        }
        
        #endregion
    }
}
