using ProblemLib.API.Model;
using System;

namespace ProblemController
{
    public class ControllerServer
    {
        /// <summary>
        /// Create a new optimization request
        /// </summary>
        /// <param name="request">The request</param>
        public void Post(OptimizationRequest request)
        {

        }

        /// <summary>
        /// Stop an optimization request
        /// </summary>
        /// <param name="id">The request</param>
        public void Delete(Guid id)
        {
            
        }

        /// <summary>
        /// Get the optimization status/solution
        /// </summary>
        public OptimizationResponse Get()
        {
            throw new NotImplementedException();
        }
    }
}
