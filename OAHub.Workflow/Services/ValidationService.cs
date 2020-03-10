using OAHub.Base.Models.WorkflowModels;
using OAHub.Workflow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Services
{
    public class ValidationService : IValidationService
    {
        private readonly WorkflowDbContext _context;

        public ValidationService(WorkflowDbContext context)
        {
            _context = context;
        }

        public bool IsProjectExists(string projectId, out Project project)
        {
            project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
            return project != null;
        }

        public bool IsJobExists(string jobId, out Job job)
        {
            job = _context.Jobs.FirstOrDefault(p => p.Id == jobId);
            return job != null;
        }

        public bool IsStepExists(string jobId, string stepId, out Step step)
        {
            var job = _context.Jobs.FirstOrDefault(p => p.Id == jobId);
            step = job.GetSteps().FirstOrDefault(s => s.Id == stepId);
            return step != null;
        }
    }
}
