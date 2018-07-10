﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
       
        private static Guid _faxId;
        private static Guid _taskId;
        private static Guid _userId;
        private static bool prompt = true;
        
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    CreateRequiredRecords(service);


                    // Retrieve the fax.
                    Fax retrievedFax = (Fax)service.Retrieve(Fax.EntityLogicalName, _faxId, new ColumnSet(true));

                    // Create a task.
                    Task task = new Task()
                    {
                        Subject = "Follow Up: " + retrievedFax.Subject,
                        ScheduledEnd = retrievedFax.CreatedOn.Value.AddDays(7),
                    };
                    _taskId = service.Create(task);

                    // Verify that the task has been created                    
                    if (_taskId != Guid.Empty)
                    {
                        Console.WriteLine("Created a task for the fax: '{0}'.", task.Subject);
                    }


                    DeleteRequiredRecords(service, prompt);
                }
                #endregion Demonstrate
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Dynamics CRM";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }

            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }

}