using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Layout
{
    public class ItemsRepeaterToggleSelectionAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            if (sender is FrameworkElement element)
            {
                var new_value = !ItemsRepeaterSelectionBehavior.GetIsSelected(element);

                ItemsRepeaterSelectionBehavior.SetIsSelected(element, new_value);

                return new_value;
            }

            return null;
        }
    }
}
