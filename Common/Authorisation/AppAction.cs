using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Authorisation
{
    public static class AppAction
    {
        public const string Create = nameof(Create);
        public const string Read = nameof(Read);
        public const string Delete = nameof(Delete);
        public const string Update = nameof(Update);
        // export action, ...

        public static IReadOnlyList<string> FullActions {get;} 
            = new ReadOnlyCollection<string>(new[] 
            {
                Create, Read, Delete, Update        
            });

        public static IReadOnlyList<string> GetActions(params string[] actions)
        {
            var actionResult = new List<string>();

            foreach (var action in actions)
            {
                if (FullActions.Contains(action))
                {
                    actionResult.Add(action);
                }
            }

            return actionResult;
        }
    }
}
