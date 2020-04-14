using Microsoft.Toolkit.Uwp.UI.Behaviors;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
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
        private SelectionModel _selectionModel;
        private bool _selectionChanging = false;
        private long? _token = null;
        private IndexPath _previousSelection;

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
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(ItemsRepeaterSelectionBehavior), new PropertyMetadata(false, IsSelected_PropertyChanged));

        protected override void OnAttached()
        {
            Cleanup();

            base.OnAttached();

            _selectionModel = new SelectionModel(); // TODO: Expose
            _selectionModel.Source = AssociatedObject.ItemsSource;
            _selectionModel.SingleSelect = this.SingleSelect;

            _selectionModel.SelectionChanged += SelectionModel_SelectionChanged;

            _token = AssociatedObject.RegisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, ItemsRepeater_ItemsSourceChanged);
            AssociatedObject.ElementPrepared += AssociatedObject_ElementPrepared;
        }        

        private void ItemsRepeater_ItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            _selectionModel.Source = AssociatedObject.ItemsSource;

            _selectionModel.ClearSelection();
        }

        private static void IsSelected_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                // Find our behavior
                var repeater = element.FindAscendant<ItemsRepeater>();
                if (repeater == null)
                {
                    return;
                }

                var selectionBehavior = Interaction.GetBehaviors(repeater).FirstOrDefault(behavior => behavior.GetType() == typeof(ItemsRepeaterSelectionBehavior)) as ItemsRepeaterSelectionBehavior;
                if (selectionBehavior == null || selectionBehavior._selectionChanging)
                {
                    return;
                }

                var index = repeater.GetElementIndex(d as UIElement);

                // TODO: Multi-select
                if (e.NewValue as bool? == true)
                {
                    selectionBehavior._selectionModel.Select(index);
                }
                else
                {
                    selectionBehavior._selectionModel.Deselect(index);
                }
            }
        }

        private void SelectionModel_SelectionChanged(SelectionModel sender, SelectionModelSelectionChangedEventArgs args)
        {
            _selectionChanging = true;
            if (SingleSelect)
            {
                // If we had a previously selected item, we need to unselect it first.
                if (_previousSelection != null)
                {
                    var item = AssociatedObject.TryGetElement(_previousSelection.GetAt(0));

                    if (item != null)
                    {
                        SetIsSelected(item, false);
                    }
                }

                // Select our new item
                if (_selectionModel.SelectedItem != null)
                {
                    var item = AssociatedObject.TryGetElement(_selectionModel.SelectedIndex.GetAt(0));

                    if (item != null)
                    {
                        SetIsSelected(item, true);
                    }
                }
                
                _previousSelection = _selectionModel.SelectedIndex;
            }

            _selectionChanging = false;
        }

        private void AssociatedObject_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            _selectionChanging = true;
            if (SingleSelect && _selectionModel.SelectedIndex.GetAt(0) == args.Index) // Single Selection, Non-Nested Case
            {
                SetIsSelected(args.Element, true);
            }
            else
            {
                SetIsSelected(args.Element, false);
            }
            _selectionChanging = false;
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

            if (_selectionModel != null)
            {
                _selectionModel.SelectionChanged -= SelectionModel_SelectionChanged;
                _selectionModel = null;
            }
        }
        #pragma warning restore CS8305 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}
