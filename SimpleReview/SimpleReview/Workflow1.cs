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
using System.Xml.Serialization;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.Office.Workflow.Utility;

namespace SimpleReview
{
	public sealed partial class Workflow1: SequentialWorkflowActivity
	{
		public Workflow1()
		{
			InitializeComponent();
		}

        public Guid workflowId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();
        public Guid taskId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties taskProps = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties afterProps = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties beforeProps = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();


        private string itemtitle = default(System.String);
        private string assignto = default(System.String);
        private int currentreviewer = 0;
        private string instructions = default(System.String);
        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            this.workflowId = workflowProperties.WorkflowId;
            itemtitle = workflowProperties.Item.DisplayName;
            XmlSerializer myserializer = new XmlSerializer(typeof(myFields));
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(workflowProperties.InitiationData));
            myFields myinitf = (myFields)myserializer.Deserialize(reader);
            assignto = myinitf.assignees;
            instructions = myinitf.instructions;
        }

        private void workflownotFinished(object sender, ConditionalEventArgs e)
        {
            if (this.assignto.Split(Convert.ToChar(";")).Length < currentreviewer + 1)
            {
                e.Result = false;
            }
            else
            {
                e.Result = true;
            }
        }
        private bool isFinished = false;
        private void tasknotFinished(object sender, ConditionalEventArgs e)
        {
            e.Result = !isFinished;
        }

        private void createTask1_MethodInvoking(object sender, EventArgs e)
        {
            taskId = Guid.NewGuid();
            isFinished = false;
            taskProps.Title = "ÇëÄãÉóÅú:" + itemtitle;
            taskProps.AssignedTo = this.assignto.Split(Convert.ToChar(";"))[this.currentreviewer].ToString();
            taskProps.Description = this.instructions;
            taskProps.ExtendedProperties["instructions"] = this.instructions;
        }
        private string comments = default(System.String);
        private void onTaskChanged1_Invoked(object sender, ExternalDataEventArgs e)
        {
            this.isFinished = bool.Parse(afterProps.ExtendedProperties["isFinished"].ToString());
            this.comments = afterProps.ExtendedProperties["comments"].ToString();
        }

        private void completeTask1_MethodInvoking(object sender, EventArgs e)
        {
            this.currentreviewer++;
        }
    
	}

}
