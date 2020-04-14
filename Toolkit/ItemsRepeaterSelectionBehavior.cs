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

        protected override void OnAttached()
        {
            Cleanup();

            base.OnAttached();

            selectionModel = new SelectionModel();
            selectionModel.Source = AssociatedObject.ItemsSource;
            _token = AssociatedObject.RegisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, ItemsRepeater_ItemsSourceChanged);

            selectionModel.SingleSelect = this.SingleSelect;
            selectionModel.SelectionChanged += SelectionModel_SelectionChanged;
        }

        private void ItemsRepeater_ItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            selectionModel.Source = AssociatedObject.ItemsSource;

            selectionModel.ClearSelection();

            selectionModel.Select(0); // Test
        }

        private void SelectionModel_SelectionChanged(SelectionModel sender, SelectionModelSelectionChangedEventArgs args)
        {
            
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
