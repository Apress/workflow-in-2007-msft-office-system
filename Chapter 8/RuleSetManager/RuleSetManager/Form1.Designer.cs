namespace RuleSetManager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSiteURL = new System.Windows.Forms.TextBox();
            this.btnGetWorkflows = new System.Windows.Forms.Button();
            this.lbWorkflows = new System.Windows.Forms.ListBox();
            this.pnlRuleSets = new System.Windows.Forms.Panel();
            this.cmboExistingRulesets = new System.Windows.Forms.ComboBox();
            this.btnRuleSet = new System.Windows.Forms.Button();
            this.txtRuleSetName = new System.Windows.Forms.TextBox();
            this.lblRuleSetName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlRuleSets.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL: ";
            // 
            // txtSiteURL
            // 
            this.txtSiteURL.Location = new System.Drawing.Point(45, 13);
            this.txtSiteURL.Name = "txtSiteURL";
            this.txtSiteURL.Size = new System.Drawing.Size(233, 20);
            this.txtSiteURL.TabIndex = 1;
            // 
            // btnGetWorkflows
            // 
            this.btnGetWorkflows.Location = new System.Drawing.Point(284, 10);
            this.btnGetWorkflows.Name = "btnGetWorkflows";
            this.btnGetWorkflows.Size = new System.Drawing.Size(142, 23);
            this.btnGetWorkflows.TabIndex = 2;
            this.btnGetWorkflows.Text = "Get Workflow Templates";
            this.btnGetWorkflows.UseVisualStyleBackColor = true;
            this.btnGetWorkflows.Click += new System.EventHandler(this.btnGetWorkflows_Click);
            // 
            // lbWorkflows
            // 
            this.lbWorkflows.FormattingEnabled = true;
            this.lbWorkflows.Location = new System.Drawing.Point(16, 73);
            this.lbWorkflows.Name = "lbWorkflows";
            this.lbWorkflows.Size = new System.Drawing.Size(410, 303);
            this.lbWorkflows.TabIndex = 3;
            this.lbWorkflows.SelectedIndexChanged += new System.EventHandler(this.lbWorkflows_SelectedIndexChanged);
            // 
            // pnlRuleSets
            // 
            this.pnlRuleSets.Controls.Add(this.cmboExistingRulesets);
            this.pnlRuleSets.Controls.Add(this.btnRuleSet);
            this.pnlRuleSets.Controls.Add(this.txtRuleSetName);
            this.pnlRuleSets.Controls.Add(this.lblRuleSetName);
            this.pnlRuleSets.Controls.Add(this.label2);
            this.pnlRuleSets.Enabled = false;
            this.pnlRuleSets.Location = new System.Drawing.Point(16, 399);
            this.pnlRuleSets.Name = "pnlRuleSets";
            this.pnlRuleSets.Size = new System.Drawing.Size(410, 100);
            this.pnlRuleSets.TabIndex = 4;
            // 
            // cmboExistingRulesets
            // 
            this.cmboExistingRulesets.FormattingEnabled = true;
            this.cmboExistingRulesets.Location = new System.Drawing.Point(117, 4);
            this.cmboExistingRulesets.Name = "cmboExistingRulesets";
            this.cmboExistingRulesets.Size = new System.Drawing.Size(212, 21);
            this.cmboExistingRulesets.TabIndex = 5;
            this.cmboExistingRulesets.SelectedIndexChanged += new System.EventHandler(this.cmboExistingRuleSets_SelectedIndexChanged);
            // 
            // btnRuleSet
            // 
            this.btnRuleSet.Location = new System.Drawing.Point(311, 51);
            this.btnRuleSet.Name = "btnRuleSet";
            this.btnRuleSet.Size = new System.Drawing.Size(96, 23);
            this.btnRuleSet.TabIndex = 4;
            this.btnRuleSet.Text = "Create RuleSet";
            this.btnRuleSet.UseVisualStyleBackColor = true;
            this.btnRuleSet.Click += new System.EventHandler(this.btnRuleSet_Click);
            // 
            // txtRuleSetName
            // 
            this.txtRuleSetName.Enabled = false;
            this.txtRuleSetName.Location = new System.Drawing.Point(117, 54);
            this.txtRuleSetName.Name = "txtRuleSetName";
            this.txtRuleSetName.Size = new System.Drawing.Size(188, 20);
            this.txtRuleSetName.TabIndex = 3;
            // 
            // lblRuleSetName
            // 
            this.lblRuleSetName.AutoSize = true;
            this.lblRuleSetName.Enabled = false;
            this.lblRuleSetName.Location = new System.Drawing.Point(4, 54);
            this.lblRuleSetName.Name = "lblRuleSetName";
            this.lblRuleSetName.Size = new System.Drawing.Size(107, 13);
            this.lblRuleSetName.TabIndex = 1;
            this.lblRuleSetName.Text = "New RuleSet Name: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Existing RuleSet: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 511);
            this.Controls.Add(this.pnlRuleSets);
            this.Controls.Add(this.lbWorkflows);
            this.Controls.Add(this.btnGetWorkflows);
            this.Controls.Add(this.txtSiteURL);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "RuleSet Manager";
            this.pnlRuleSets.ResumeLayout(false);
            this.pnlRuleSets.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSiteURL;
        private System.Windows.Forms.Button btnGetWorkflows;
        private System.Windows.Forms.ListBox lbWorkflows;
        private System.Windows.Forms.Panel pnlRuleSets;
        private System.Windows.Forms.ComboBox cmboExistingRulesets;
        private System.Windows.Forms.Button btnRuleSet;
        private System.Windows.Forms.TextBox txtRuleSetName;
        private System.Windows.Forms.Label lblRuleSetName;
        private System.Windows.Forms.Label label2;
    }
}

