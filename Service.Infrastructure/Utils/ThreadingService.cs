using System;
using System.Threading.Tasks;

namespace Service.Common.Utils
{
    /// <summary>
    /// Threading service.
    /// </summary>
    public class ThreadingService
    {
        /// <summary>
        /// Method to fire and forget the given action.
        /// Logs the error in the event of the exception, but no exception is raised.
        /// </summary>
        /// <param name="action">The Action to execute.</param>
        public static void FireAndForget(Action action)
        {
            Task.Factory.StartNew(action)
                .ContinueWith(antecedent =>
                {
                    antecedent.Exception?.Handle(ex =>
                    {

                        return true; //mark as handled
                    });
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
