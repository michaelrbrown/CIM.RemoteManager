using System.Collections.Generic;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Controls
{
    /// <summary>
    /// A <see cref="StackLayout"/> that can be bound to a <see cref="DataTemplate"/> and data source.
    /// </summary>
    public class BindableStackLayout : StackLayout
    {
        #region Bindable Properties

        /// <summary>
        /// Property bound to <see cref="ItemTemplate"/>.
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty =
           BindableProperty.Create(
               propertyName: nameof(ItemTemplate),
               returnType: typeof(DataTemplate),
               declaringType: typeof(BindableStackLayout),
               defaultValue: default(DataTemplate),
               propertyChanged: OnItemTemplateChanged);

        /// <summary>
        /// Property bound to <see cref="ItemsSource"/>.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                propertyName: nameof(ItemsSource),
                returnType: typeof(IEnumerable<object>),
                declaringType: typeof(BindableStackLayout),
                propertyChanged: OnItemsSourceChanged);

        #endregion

        #region Properties 

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/>.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of view models to bind to the item views.
        /// </summary>
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        #region Property Changed Callbacks

        /// <summary>
        /// Called when <see cref="ItemTemplate"/> changes.
        /// </summary>
        /// <param name="bindable">The <see cref="BindableObject"/> being changed.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = (BindableStackLayout)bindable;
            if (newValue == null)
            {
                return;
            }

            layout.PopulateItems();
        }

        /// <summary>
        /// Called when <see cref="ItemsSource"/> is changed.
        /// </summary>
        /// <param name="bindable">The <see cref="BindableObject"/> being changed.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = (BindableStackLayout)bindable;
            if (newValue == null)
            {
                return;
            }

            layout.PopulateItems();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and binds the item views based on <see cref="ItemTemplate"/>.
        /// </summary>
        private void PopulateItems()
        {
            var items = ItemsSource;
            if (items == null || ItemTemplate == null)
            {
                return;
            }

            var children = Children;
            children.Clear();

            foreach (var item in items)
            {
                children.Add(InflateView(item));
            }
        }

        /// <summary>
        /// Inflates an item view using the correct <see cref="DataTemplate"/> for the given view model.
        /// </summary>
        /// <param name="viewModel">The view model to bind the item view to.</param>
        /// <returns>The new view with the view model as its binding context.</returns>
        private View InflateView(object viewModel)
        {
            var view = (View)CreateContent(ItemTemplate, viewModel, this);
            view.BindingContext = viewModel;
            return view;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create UI content from a <see cref="DataTemplate"/> (or optionally a <see cref="DataTemplateSelector"/>).
        /// </summary>
        /// <param name="template">The <see cref="DataTemplate"/>.</param>
        /// <param name="item">The view model object.</param>
        /// <param name="container">The <see cref="BindableObject"/> that will be the parent to the content.</param>
        /// <returns>The content created by the template.</returns>
        public static object CreateContent(DataTemplate template, object item, BindableObject container)
        {
            var selector = template as DataTemplateSelector;
            if (selector != null)
            {
                template = selector.SelectTemplate(item, container);
            }

            return template.CreateContent();
        }

        #endregion
    }
}