using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Form.Core.Configuration;
using Sitecore.Form.Core.Pipelines.FormSubmit;
using Sitecore.StringExtensions;
using Sitecore.WFFM.Abstractions.Actions;
using Sitecore.WFFM.Abstractions.Dependencies;
using System.Linq;

namespace Sitecore.Support.Form.Core.Pipelines.FormSubmit
{
    public class FormatMessage
    {
        protected virtual string ClienMessage
        {
            get
            {
                return DependenciesManager.ResourceManager.Localize("FAILED_SUBMIT");
            }
        }

        public void Process(SubmittedFormFailuresArgs failedArgs)
        {
            Assert.IsNotNull(failedArgs, "args");
            Assert.IsNotNull(failedArgs.FormID, "FormID");
            DependenciesManager.Logger.Warn("Web Forms for Marketers: an exception: {0} has occured while trying to execute an action.".FormatWith(new object[]
            {
                failedArgs.ErrorMessage
            }), this);
            for (int i = 0; i < failedArgs.Failures.Count<ExecuteResult.Failure>(); i++)
            {
                if (Sitecore.Form.Core.Configuration.Settings.HideInnerError && !failedArgs.Failures[i].IsCustom)
                {
                    if (!string.IsNullOrEmpty(failedArgs.ErrorMessage))
                    {
                        failedArgs.Failures[i].ErrorMessage = failedArgs.ErrorMessage;
                        return;
                    }
                    Database database = Factory.GetDatabase(failedArgs.Database, false);
                    if (database != null)
                    {
                        Item item = database.GetItem(failedArgs.FormID);
                        if (item != null)
                        {
                            string text = item[FormIDs.SaveActionFailedMessage];
                            if (!string.IsNullOrEmpty(text))
                            {
                                failedArgs.Failures[i].ErrorMessage = text;
                                return;
                            }
                        }
                        Item item2 = database.GetItem(FormIDs.SubmitErrorId);
                        if (item2 != null)
                        {
                            string text2 = item2["Value"];
                            if (!string.IsNullOrEmpty(text2))
                            {
                                failedArgs.Failures[i].ErrorMessage = text2;
                                return;
                            }
                        }
                    }
                    failedArgs.Failures[i].ErrorMessage = this.ClienMessage;
                }
            }
        }
    }
}