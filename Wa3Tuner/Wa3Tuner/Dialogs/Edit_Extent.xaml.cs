using MdxLib.Model;
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
        CExtent Current = new CExtent();
        bool WholeModel = false;
        private CModel? model;
        private bool all = false;

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
        public Edit_Extent(bool wholeModel, CModel m)
        {
            if (wholeModel == false) { Close(); return; }
            WholeModel = wholeModel;
            model = m;
            InitializeComponent();
            Current = new CExtent();
            minx_.Text = "0";
            miny_.Text = "0";
            minz_.Text = "0";
            maxx_.Text = "0";
            maxy_.Text = "0";
            maxz_.Text = "0";
            bounds_.Text = "0";
        }
        public Edit_Extent(CModel currentModel, bool all)
        {
            InitializeComponent();
             model = currentModel;
            this.all =  all;
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
          
          
            bool parse1 = float.TryParse(minx_.Text, out float minx);
            bool parse2 = float.TryParse(miny_.Text, out float miny);
            bool parse3 = float.TryParse(minz_.Text, out float minz);
            bool parse4 = float.TryParse(maxx_.Text, out float maxx);
            bool parse5 = float.TryParse(maxy_.Text, out float maxy);
            bool parse6 = float.TryParse(maxz_.Text, out float maxz);
            bool parse7 = float.TryParse(bounds_.Text, out float bound);
           
                if (WholeModel)
            {
               
                if (model == null) { MessageBox.Show("Null model"); return; }
                if (parse1 && parse2 && parse3 && parse4 && parse5 && parse6 && parse7)
                {
                    if (bound < 0) { MessageBox.Show("Radius cannot be negative"); return; }
                    if (minx >= maxx) { MessageBox.Show("min x canot be equal are lower than max x"); return; }
                    if (miny >= maxy) { MessageBox.Show("min y canot be equal are lower than max y"); return; }
                    if (minz >= maxz) { MessageBox.Show("min z canot be equal are lower than max z"); return; }
                    
                        CExtent ex = new CExtent();
                        ex.Max.X = maxx;
                        ex.Max.Y = maxy;
                        ex.Max.Z = maxz;
                        ex.Min.X = minx;
                        ex.Min.Y = miny;
                        ex.Min.Z = minz;
                    ex.Radius = bound;
                      
                                              // sequences
                                     foreach (var sequence in model.Sequences) { sequence.Extent = new CExtent(ex); }
                               //geosets extents and geoset sequence extents
                             foreach (var geoset in model.Geosets)
                    {
                        geoset.Extent = new CExtent(ex);
                        if (model.Sequences.Count > 0)
                        {
                            geoset.Extents.Clear();
                            for (var i = 0; i < model.Sequences.Count; i++)
                            {
                                var x = new CGeosetExtent(model);
                                x.Extent = new CExtent(ex);
                                geoset.Extents.Add(x);
                            }
                        }
                    }
                               // model extents
                        model.Extent = new CExtent(ex);
                        DialogResult = true;
                    Close();


                }
                else { MessageBox.Show("One or more fields are invalid"); return; }

                return;
            }

           //regular extent
            if (parse1 && parse2 && parse3 && parse4 && parse5 && parse6 && parse7)
            {
                if (bound < 0) { MessageBox.Show("Radius cannot be negative"); return; }
                if (minx >= maxx) { MessageBox.Show("min x canot be equal are lower than max x"); return; }
                if (miny >= maxy) { MessageBox.Show("min y canot be equal are lower than max y"); return; }
                if (minz >= maxz) { MessageBox.Show("min z canot be equal are lower than max z"); return; }
                if (all)
                {
                    foreach (var s in model.Sequences)
                    {
                        s.Extent  .Min.X = minx; 
                        s.Extent.Min.Y = miny;
                        s.Extent.Min.Z = minz;
                        s.Extent.Max.X = maxx;
                        s.Extent.Max.Y = maxy;
                        s.Extent.Max.Z = maxz;
                        s.Extent.Radius = bound;
                       
                    }
                    DialogResult = true;
                }
                else
                {
                    Current.Min.X = minx; Current.Min.Y = miny; Current.Min.Z = minz;
                    Current.Max.X = maxx; Current.Max.Y = maxy; Current.Max.Z = maxz;
                    Current.Radius = bound;
                    DialogResult = true;
                }
               
              

            }

        }

        private void copy(object? sender, RoutedEventArgs? e)
        {
            bool parse1 = float.TryParse(minx_.Text, out float minx);
            bool parse2 = float.TryParse(miny_.Text, out float miny);
            bool parse3 = float.TryParse(minz_.Text, out float minz);
            bool parse4 = float.TryParse(maxx_.Text, out float maxx);
            bool parse5 = float.TryParse(maxy_.Text, out float maxy);
            bool parse6 = float.TryParse(maxz_.Text, out float maxz);
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

        private void paste(object? sender, RoutedEventArgs? e)
        {
            var copied = ExtentCopier.Copied;

            minx_.Text = copied.Min.X.ToString();
            miny_.Text = copied.Min.Y.ToString();
            minz_.Text = copied.Min.Z.ToString();
            maxx_.Text = copied.Max.X.ToString();
            maxy_.Text = copied.Max.Y.ToString();
            maxz_.Text = copied.Max.Z.ToString();
            bounds_.Text = copied.Radius.ToString();
        }

        private void reset(object? sender, RoutedEventArgs? e)
        {
            if (Current == null)
                return;

            if (Current.Min != null)
            {
                if (minx_ != null) minx_.Text = Current.Min.X.ToString();
                if (miny_ != null) miny_.Text = Current.Min.Y.ToString();
                if (minz_ != null) minz_.Text = Current.Min.Z.ToString();
            }

            if (Current.Max != null)
            {
                if (maxx_ != null) maxx_.Text = Current.Max.X.ToString();
                if (maxy_ != null) maxy_.Text = Current.Max.Y.ToString();
                if (maxz_ != null) maxz_.Text = Current.Max.Z.ToString();
            }

            if (bounds_ != null)
                bounds_.Text = Current.Radius.ToString();
        }




        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
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
