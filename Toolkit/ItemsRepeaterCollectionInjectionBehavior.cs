using Microsoft.Toolkit.Uwp.UI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Layout
{
    [ContentProperty(Name = nameof(InjectionTarget))]
    public class ItemsRepeaterCollectionInjectionBehavior : BehaviorBase<ItemsRepeater>, IEnumerable<object>
    {
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(ItemsRepeaterCollectionInjectionBehavior), new PropertyMetadata(null, OnItemsSourceChanged));

        public FrameworkElement InjectionTarget
        {
            get { return (FrameworkElement)GetValue(InjectionTargetProperty); }
            set { SetValue(InjectionTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InjectionTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InjectionTargetProperty =
            DependencyProperty.Register("InjectionTarget", typeof(FrameworkElement), typeof(ItemsRepeaterCollectionInjectionBehavior), new PropertyMetadata(null));

        public int Index { get; set; } // TODO: DP?

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsRepeaterCollectionInjectionBehavior behavior)
            {
                behavior.AssociatedObject.ItemsSource = behavior;
            }            
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (ItemsSource != null)
            {
                AssociatedObject.ItemsSource = this;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            int x = 0;
            foreach(var element in ItemsSource as IEnumerable<object>)
            {
                if (x++ == Index)
                {
                    yield return InjectionTarget;
                }

                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
