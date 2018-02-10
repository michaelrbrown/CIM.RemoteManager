using CIM.RemoteManager.iOS.Renderer;
using UIKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellRenderer))]
namespace CIM.RemoteManager.iOS.Renderer
{
    /// <summary>
    /// Prevents default selection background color on tap in listview rows.
    /// </summary>
    public class ViewCellRenderer : Xamarin.Forms.Platform.iOS.ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            reusableCell = base.GetCell(item, reusableCell, tv);
            reusableCell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return reusableCell;
        }
    }
}