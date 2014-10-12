using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ZUMOAPPNAME.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// list to Observable Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> col)
        {
            return new ObservableCollection<T>(col);
        }

        /// <summary>
        /// Continous live connection
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task)
        {
            task.ContinueWith(p => { var e = task.Exception; });
        }
    }
}
