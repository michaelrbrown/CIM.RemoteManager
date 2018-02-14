using System;
using System.Collections;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Controls
{
    /// <summary>
    /// Repeater view.
    /// </summary>
    public class RepeaterView : StackLayout
    {
        /// <summary>
        /// The item template property.
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty =
                BindableProperty.Create(
                    "ItemTemplate",
                    typeof(DataTemplate),
                    typeof(RepeaterView),
                    null,
                    propertyChanged: (bindable, value, newValue) =>
                        Populate(bindable));

        /// <summary>
        /// The items source property.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(RepeaterView),
                null,
                BindingMode.OneWay,
                propertyChanged: (bindable, value, newValue) =>
                  Populate(bindable));

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the item template.
        /// </summary>
        /// <value>The item template.</value>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)this.GetValue(ItemTemplateProperty);
            set => this.SetValue(ItemTemplateProperty, value);
        }

        /// <summary>
        /// Populate the specified bindable.
        /// </summary>
        /// <returns>The populate.</returns>
        /// <param name="bindable">Bindable.</param>
        private static void Populate(BindableObject bindable)
        {
            var repeater = (RepeaterView)bindable;

            // Clean
            repeater.Children.Clear();

            // Only populate once both properties are received
            if (repeater.ItemsSource == null ||
                repeater.ItemTemplate == null)
                return;

            foreach (var viewModel in repeater.ItemsSource)
            {
                var content = repeater.ItemTemplate.CreateContent();
                if (!(content is View) && !(content is ViewCell))
                {
                    throw new Exception(
                          $"Invalid visual object {nameof(content)}");
                }

                var view = content is View ? content as View :
                                             ((ViewCell)content).View;
                view.BindingContext = viewModel;

                repeater.Children.Add(view);
            }
        }
    }
}
