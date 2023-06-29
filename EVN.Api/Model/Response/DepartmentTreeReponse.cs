using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class DepartmentTreeReponse
    {
        public virtual long deptId { get; set; }
        public virtual long orgId { get; set; }
        public virtual string shortName { get; set; }
        public virtual string name { get; set; }
        public virtual int status { get; set; }

        public virtual long? deptParent { get; set; }
        public virtual long? deptRoot { get; set; }
        public virtual int Level { get; set; }
        public virtual IEnumerable<DepartmentTreeReponse> Children { get; set; }
    }

    internal static class GenericHelpers
    {
        public static void CreateTree<K>(this IEnumerable<DepartmentTreeReponse> listTreeItems, List<DepartmentTreeReponse> listT, int deep, string deepStr, System.Linq.Expressions.Expression<Func<DepartmentTreeReponse, K>> nameGeneral)
        {
            if (listT == null) listT = new List<DepartmentTreeReponse>();
            foreach (var treeItem in listTreeItems)
            {
                DepartmentTreeReponse item = treeItem;
                Type examType = typeof(DepartmentTreeReponse);

                System.Linq.Expressions.LambdaExpression lambda = (System.Linq.Expressions.LambdaExpression)nameGeneral;
                System.Linq.Expressions.MemberExpression memberExpression;

                if (lambda.Body is System.Linq.Expressions.UnaryExpression)
                {
                    System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)(lambda.Body);
                    memberExpression = (System.Linq.Expressions.MemberExpression)(unaryExpression.Operand);
                }
                else
                {
                    memberExpression = (System.Linq.Expressions.MemberExpression)(lambda.Body);
                }
                // Change the static property value.
                char[] characters = deepStr.ToCharArray();

                PropertyInfo piShared = examType.GetProperty(((PropertyInfo)memberExpression.Member).Name);
                piShared.SetValue(item, string.Join("", characters.ToList().Select(c => new String(c, deep))) + piShared.GetValue(treeItem).ToString());
                listT.Add(item);
                CreateTree(treeItem.Children, listT, deep + 1, deepStr, nameGeneral);
            }
        }
        /// <summary>
        /// Generates tree of items from item list
        /// </summary>
        /// 
        /// <typeparam name="T">Type of item in collection</typeparam>
        /// <typeparam name="K">Type of parent_id</typeparam>
        /// 
        /// <param name="collection">Collection of items</param>
        /// <param name="id_selector">Function extracting item's id</param>
        /// <param name="parent_id_selector">Function extracting item's parent_id</param>
        /// <param name="root_id">Root element id</param>
        /// 
        /// <returns>Tree of items</returns>
        public static IEnumerable<DepartmentTreeReponse> GenerateTree<K>(
            this IEnumerable<DepartmentTreeReponse> collection,
            Func<DepartmentTreeReponse, K> id_selector,
            Func<DepartmentTreeReponse, K> parent_id_selector,
            K root_id = default(K), int level = 0)
        {
            foreach (var c in collection.Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), root_id)))
            {
                DepartmentTreeReponse item = c;
                item.Level = level;
                item.Children = collection.GenerateTree(id_selector, parent_id_selector, id_selector(c), level: ++level);
                yield return item;
            }
        }
    }
}