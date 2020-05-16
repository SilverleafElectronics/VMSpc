using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VMSpc.Extensions.UI
{
    public static class GridExtensions
    {
        public static void AddMultiple(this ColumnDefinitionCollection columnDefinitionCollection, params ColumnDefinition[] columnDefinitions)
        {
            foreach (var definition in columnDefinitions)
            {
                columnDefinitionCollection.Add(definition);
            }
        }

        public static void AddMultiple(this RowDefinitionCollection RowDefinitionCollection, params RowDefinition[] RowDefinitions)
        {
            foreach (var definition in RowDefinitions)
            {
                RowDefinitionCollection.Add(definition);
            }
        }
    }
}
