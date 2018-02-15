using System;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Controls
{
    /// <summary>
    /// An extended <see cref="Xamarin.Forms.ListView"/> with scroll position tracking.
    /// </summary>
    public class EnhancedListView : Xamarin.Forms.ListView
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Xamarin.Forms.ListView"/> class.
        /// </summary>
        public EnhancedListView() : base(ListViewCachingStrategy.RetainElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Xamarin.Forms.ListView"/> class.
        /// </summary>
        /// <param name="strategy">The caching strategy to use.</param>
        public EnhancedListView(ListViewCachingStrategy strategy) : base(strategy)
        {
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event that is raised after a scroll completes.
        /// </summary>
        public event EventHandler<ScrolledEventArgs> Scrolled;

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to be called after a scroll completes.
        /// </summary>
        /// <param name="args">The scroll event arguments.</param>
        public void OnScrolled(ScrolledEventArgs args)
        {
            Scrolled?.Invoke(this, args);
        }

        #endregion
    }
}