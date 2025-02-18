using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Edit_Extent.xaml
    /// </summary>
   
   
    public partial class Edit_Extent : Window
    {
        CExtent Current;
        public Edit_Extent(CExtent extent)
        {
            InitializeComponent();
            Current = extent;
            minx_.Text = extent.Min.X.ToString();
            miny_.Text = extent.Min.Y.ToString();
            minz_.Text = extent.Min.Z.ToString();
            maxx_.Text = extent.Max.X.ToString();
            maxy_.Text = extent.Max.Y.ToString();
            maxz_.Text = extent.Max.Z.ToString();
            bounds_.Text = extent.Radius.ToString();    
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            bool parse1 = float.TryParse(minx_.Text, out float minx);
            bool parse2 = float.TryParse(miny_.Text, out float miny);
            bool parse3 = float.TryParse(minz_.Text, out float minz);
            bool parse4 = float.TryParse(maxx_.Text, out float maxx);
            bool parse5 = float.TryParse(maxx_.Text, out float maxy);
            bool parse6 = float.TryParse(maxx_.Text, out float maxz);
            bool parse7 = float.TryParse(bounds_.Text, out float bound);
            if (parse1 && parse2 && parse3 && parse4 && parse5 && parse6 && parse7)
            {
                if (bound < 0) { return; }
                if (minx >= maxx) { return; }
                if (miny >= maxy) { return; }
                if (minz >= maxz) { return; }
                Current.Min.X = minx; Current.Min.Y = miny; Current.Min.Z = minz;
                Current.Max.X = maxx; Current.Max.Y = maxy; Current.Max.Z = maxz;
                Current.Radius = bound;
                DialogResult = true;

            }

        }

        private void copy(object sender, RoutedEventArgs e)
        {
            bool parse1 = float.TryParse(minx_.Text, out float minx);
            bool parse2 = float.TryParse(miny_.Text, out float miny);
            bool parse3 = float.TryParse(minz_.Text, out float minz);
            bool parse4 = float.TryParse(maxx_.Text, out float maxx);
            bool parse5 = float.TryParse(maxx_.Text, out float maxy);
            bool parse6 = float.TryParse(maxx_.Text, out float maxz);
            bool parse7 = float.TryParse(bounds_.Text, out float bound);
            if (parse1 && parse2 && parse3 && parse4 && parse5 && parse6 && parse7)
            {
                CExtent temp = new CExtent();
                if (bound < 0) { return; }
                if (minx >= maxx) { return; }
                if (miny >= maxy) { return; }
                if (minz >= maxz) { return; }
                temp.Min.X = minx; temp.Min.Y = miny; temp.Min.Z = minz;
                temp.Max.X = maxx; temp.Max.Y = maxy; temp.Max.Z = maxz;
                temp.Radius = bound;
                ExtentCopier.Copy(temp);

            }
               
        }

        private void paste(object sender, RoutedEventArgs e)
        {
            

            minx_.Text = Current.Min.X.ToString();
            miny_.Text = Current.Min.Y.ToString();
            minz_.Text = Current.Min.Z.ToString();
            maxx_.Text = Current.Max.X.ToString();
            maxy_.Text = Current.Max.Y.ToString();
            maxz_.Text = Current.Max.Z.ToString();
            bounds_.Text = Current.Radius.ToString();
        }

        private void reset(object sender, RoutedEventArgs e)
        {
            minx_.Text = Current.Min.X.ToString();
            miny_.Text = Current.Min.Y.ToString();
            minz_.Text = Current.Min.Z.ToString();
            maxx_.Text = Current.Max.X.ToString();
            maxy_.Text = Current.Max.Y.ToString();
            maxz_.Text = Current.Max.Z.ToString();
            bounds_.Text = Current.Radius.ToString();
        }
    }
    public static class ExtentCopier
    {
        public static void Copy(CExtent extent)
        {
            Copied = new CExtent(extent);
        }
        public static CExtent Copied = new CExtent();
        public static void PasteTo(CExtent extent)
        {

        }
    }
}
