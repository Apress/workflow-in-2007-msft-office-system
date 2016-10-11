<%-- _lcid="1033" _version="12.0.3208" _dal="1" --%>
<%-- _LocalBinding --%>
<%@ Assembly Name="MarketingCampaignASP2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7e90f7b4b8a6ed0b"%>
<%@ Page Language="C#" EnableSessionState="true" ValidateRequest="False"  Inherits="MarketingCampaignASP2.InitForm"
    MasterPageFile="~/_layouts/application.master"  %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"  Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities"  Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint" %>


<%@ Register TagPrefix="wssuc" TagName="InputFormSection"  src="/_controltemplates/InputFormSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="InputFormControl"  src="/_controltemplates/InputFormControl.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ButtonSection"  src="/_controltemplates/ButtonSection.ascx" %> 
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls"  Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral,  PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"  Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral,  PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Workflow" Namespace="Microsoft.SharePoint.Workflow"  Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral,  PublicKeyToken=71e9bce111e9429c" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<script language="JavaScript">
    function _spBodyOnLoad()
    {
        try
        {
            window.focus();
        }
        catch(e)
        {
        }
    }
    function DoValidateAndSubmit()
    {
        return true;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <asp:Literal ID="Literal1" runat="server" Text="Customize Workflow" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <%
        string strPTS = "Customize " + m_workflowName;
        SPHttpUtility.HtmlEncode(strPTS, Response.Output);
    %>
    :
    <asp:HyperLink ID="hlReturn" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderPageImage" runat="server">
    <img src="/_layouts/images/blank.gif" width="1" height="1" alt="">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
    <%
        string strPD = "Use this page to customize this instance of " + m_workflowName + ".";
        SPHttpUtility.HtmlEncode(strPD, Response.Output);
    %>
</asp:Content>


<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    
Traffic Coordinator: kcd\<input type="text" name="TrafficCoordinator" id="TrafficCoordinator" runat="server" />
<br /><br />
Marketing Director Email: <input type="text" name="MarketingDirector" id="MarketingDirector" runat="server" />@kcdholdings.com
<br /><br />

       <input type="hidden" name="WorkflowDefinition"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode(  Request.Form["WorkflowDefinition"]), Response.Output); %>'>
        <input type="hidden" name="WorkflowName" value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode(Request.Form["WorkflowName"]),Response.Output); %>'>
        <input type="hidden" name="AddToStatusMenu"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["AddToStatusMenu"]), Response.Output); %>'>
        <input type="hidden" name="AllowManual"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["AllowManual"]),Response.Output); %>'>
        <input type="hidden" name="RoleSelect"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["RoleSelect"]),Response.Output); %>'>
        <input type="hidden" name="GuidAssoc" 
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["GuidAssoc"]),Response.Output); %>'>
        <input type="hidden" name="SetDefault"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["SetDefault"]),Response.Output); %>'>
        <input type="hidden" name="HistoryList"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["HistoryList"]),Response.Output); %>'>
        <input type="hidden" name="TaskList"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["TaskList"]),Response.Output); %>'>
        <input type="hidden" name="UpdateLists"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode(Request.Form["UpdateLists"]),Response.Output); %>'>        
        <input type="hidden" name="AutoStartCreate"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["AutoStartCreate"]),Response.Output); %>'>
        <input type="hidden" name="AutoStartChange"  
value='<% SPHttpUtility.NoEncode(SPHttpUtility.HtmlEncode( Request.Form["AutoStartChange"]),Response.Output); %>'>
        
                <asp:Button runat="server" class="ms-ButtonHeightWidth" OnClick="BtnOK_Click" Text="OK" id="btnOK" />
           
   
</asp:Content>
