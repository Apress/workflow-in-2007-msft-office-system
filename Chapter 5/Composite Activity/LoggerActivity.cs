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

using Microsoft.SharePoint.WorkflowActions;

namespace KCD.Sharepoint.Activities.Composite
{

    public partial class LoggerActivity : SequenceActivity, IActivityEventListener<ActivityExecutionStatusChangedEventArgs>
	{
		public LoggerActivity()
		{
			InitializeComponent();
		}

        public static DependencyProperty ConditionProperty = System.Workflow.ComponentModel.DependencyProperty.Register("Condition", typeof(ActivityCondition), typeof(LoggerActivity));

        [Description("This is the description which appears in the Property Browser")]
        [Category("This is the category which will be displayed in the Property Browser")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ActivityCondition Condition
        {
            get
            {
                return ((ActivityCondition)(base.GetValue(LoggerActivity.ConditionProperty)));
            }
            set
            {
                base.SetValue(LoggerActivity.ConditionProperty, value);
            }
        }

        public static DependencyProperty RunningProperty = System.Workflow.ComponentModel.DependencyProperty.Register("Running", typeof(bool), typeof(LoggerActivity));

        [Description("This is the description which appears in the Property Browser")]
        [Category("This is the category which will be displayed in the Property Browser")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Running
        {
            get
            {
                return ((bool)(base.GetValue(LoggerActivity.RunningProperty)));
            }
            set
            {
                base.SetValue(LoggerActivity.RunningProperty, value);
            }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            this.Running = true;
            bool ActivityStarted = false;
            if (this.Condition.Evaluate(this, executionContext))
            {
                for (int i = 0; i < this.Activities.Count; i++)
                {
                    if ((string)this.Activities[i].UserData["logger"] == "logger")
                    {
                        LogToHistoryListActivity logger = (LogToHistoryListActivity)this.Activities[i];
                        logger.HistoryDescription = string.Format(@"Begin 
                            Activity Execution:{0} with {1} enabled Children", 
                            this.QualifiedName, 
                            this.EnabledActivities.Count.ToString());
                        break;
                    }
                }
            }

            for (int childNum = 0; childNum < this.EnabledActivities.Count; 
                childNum++)
            {
                Activity child = this.EnabledActivities[childNum] as Activity;
                if (null != child)
                {
                    child.RegisterForStatusChange(Activity.ClosedEvent, this);
                    executionContext.ExecuteActivity(child);
                    ActivityStarted = true;
                }
            }
            return ActivityStarted ? ActivityExecutionStatus.Executing : ActivityExecutionStatus.Closed;
        }


        void IActivityEventListener<ActivityExecutionStatusChangedEventArgs>.OnEvent(object sender, ActivityExecutionStatusChangedEventArgs e)
        {
            ActivityExecutionContext context = sender as 
                ActivityExecutionContext;
            if (e.ExecutionStatus == ActivityExecutionStatus.Closed)
            {
                e.Activity.UnregisterForStatusChange(Activity.ClosedEvent, this);
                LoggerActivity lgr = context.Activity as LoggerActivity;
                bool finished = true;
                for (int childNum = 0; childNum < lgr.EnabledActivities.Count; 
                    childNum++)
                {
                    Activity child = lgr.EnabledActivities[childNum];
                    if ((child.ExecutionStatus != ActivityExecutionStatus.
                        Initialized) && (child.ExecutionStatus != 
                        ActivityExecutionStatus.Closed))
                        finished = false;
                }
                if (finished)
                    context.CloseActivity();
            }
        }

        protected override ActivityExecutionStatus Cancel(ActivityExecutionContext executionContext)
        {
            if (null == executionContext) throw new ArgumentNullException("executionContext");

            bool cancelled = true;

            // Check all children to ensure that they are cancelled
            for (int childNum = 0; childNum < this.EnabledActivities.Count; childNum++)
            {
                Activity child = this.EnabledActivities[childNum];

                if (child.ExecutionStatus == ActivityExecutionStatus.Executing)
                {
                    executionContext.CancelActivity(child);
                    cancelled = false;
                }
                else if ((child.ExecutionStatus == ActivityExecutionStatus.Canceling) || (child.ExecutionStatus == ActivityExecutionStatus.Faulting))
                    cancelled = false;
            }

            return cancelled ? ActivityExecutionStatus.Canceling : ActivityExecutionStatus.Closed;
        }
	}
}
