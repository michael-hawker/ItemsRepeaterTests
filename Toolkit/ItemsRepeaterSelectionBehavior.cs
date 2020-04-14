using Microsoft.Toolkit.Uwp.UI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Layout
{
    public class ItemsRepeaterSelectionBehavior : BehaviorBase<ItemsRepeater>
    {
        #pragma warning disable CS8305 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        private SelectionModel selectionModel;
        private long? _token = null;

        public bool SingleSelect { get; set; }

        public static bool GetIsSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(ItemsRepeaterSelectionBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            Cleanup();

            base.OnAttached();

            selectionModel = new SelectionModel(); // TODO: Expose
            selectionModel.Source = AssociatedObject.ItemsSource;
            selectionModel.SingleSelect = this.SingleSelect;

            selectionModel.SelectionChanged += SelectionModel_SelectionChanged;

            _token = AssociatedObject.RegisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, ItemsRepeater_ItemsSourceChanged);
            AssociatedObject.ElementPrepared += AssociatedObject_ElementPrepared;
        }        

        private void ItemsRepeater_ItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            selectionModel.Source = AssociatedObject.ItemsSource;

            selectionModel.ClearSelection();

            selectionModel.Select(2); // Test
        }

        private void SelectionModel_SelectionChanged(SelectionModel sender, SelectionModelSelectionChangedEventArgs args)
        {
            if (SingleSelect && selectionModel.SelectedItem != null)
            {
                var item = AssociatedObject.TryGetElement(selectionModel.SelectedIndex.GetAt(0));

                if (item != null)
                {
                    SetIsSelected(item, true);
                }
            }

            // TODO: Unselect on null???
        }

        private void AssociatedObject_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            if (SingleSelect && selectionModel.SelectedIndex.GetAt(0) == args.Index) // Single Selection, Non-Nested Case
            {
                SetIsSelected(args.Element, true);
            }
            else
            {
                SetIsSelected(args.Element, false);
            }
        }

        protected override void OnDetaching()
        {
            Cleanup();
            
            base.OnDetaching();
        }

        private void Cleanup()
        {
            if (AssociatedObject != null && _token != null)
            {
                AssociatedObject.ElementPrepared -= AssociatedObject_ElementPrepared;

                AssociatedObject.UnregisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, _token.Value);
                _token = null;
            }

            if (selectionModel != null)
            {
                selectionModel.SelectionChanged -= SelectionModel_SelectionChanged;
                selectionModel = null;
            }
        }
        #pragma warning restore CS8305 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}
