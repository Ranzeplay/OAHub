using OAHub.Base.Models.WorkflowModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Services
{
    public interface IValidationService
    {
        bool IsProjectExists(string projectId, out Project project);

        bool IsJobExists(string jobId, out Job job);

        bool IsStepExists(string jobId, string stepId, out Step step);
    }
}
