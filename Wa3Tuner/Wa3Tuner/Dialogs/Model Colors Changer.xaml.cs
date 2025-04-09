using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Model_Colors_Changer.xaml
    /// </summary>
    public partial class Model_Colors_Changer : Window
    {
        CModel model;
        private List<Vector3> Colors;
        public Model_Colors_Changer(CModel m)
        {
            InitializeComponent();
            model = m;
            Fill();
            FillColors();
        }

        private void FillColors()
        {
            Colors = new List<Vector3>
            {
                new Vector3(255, 0, 0),
                new Vector3(0, 0, 255),
                new Vector3(0, 128, 128),
                new Vector3(128, 0, 128),
                new Vector3(255, 255, 0),
                new Vector3(255, 165, 0),
                new Vector3(255, 192, 203),
                new Vector3(128, 128, 128),
                new Vector3(1, 50, 32),
                new Vector3(150, 75, 0)
            };
        }

        private void Fill()
        {
            Combo.Items.Add(new ComboBoxItem() { Content = "Red"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Blue"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Teal"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Purple"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Yellow"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Orange"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Green"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Pink"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Gray"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Light Blue"});
            Combo.Items.Add(new ComboBoxItem() { Content = "Dark Green" });
            Combo.Items.Add(new ComboBoxItem() { Content = "Brown"});
          
            Combo.Items.Add(new ComboBoxItem() { Content = "Black"});
            Combo.Items.Add(new ComboBoxItem() { Content = "White"});
            Combo.SelectedIndex = 0;    
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { ok(null,null); }
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            int index = Combo.SelectedIndex;
            bool alpha = c1.IsChecked == true;
            bool alphaAnimated = c2.IsChecked == true;
            bool tc = c3.IsChecked == true;
            bool tg = c4.IsChecked == true;
            bool stars = c5.IsChecked == true;
            bool glows = c6.IsChecked == true;
            bool seek = SEEK.IsChecked == true;

            if (!alpha && !alphaAnimated && !tc && !tg && !stars && !glows)
            {
                MessageBox.Show("Select at least one option");return;
            }

            ColorCollecton.Init();

            if (alpha || alphaAnimated)
            {
                foreach (var ga in model.GeosetAnimations)
                {
                    
                    if (ga.Color.Animated && ga.UseColor && alphaAnimated)
                    {
                        foreach (var kf in ga.Color)
                        {
                            bool changed = false;
                            Vector3 value = Calculator.BGRnToRGB_Vector(kf.Value);

                            for (int i = 0; i < Colors.Count; i++)
                            {
                                if (Colors[i] == value)
                                {
                                    ga.Color.MakeStatic(Calculator.RGB_Vector_to_BGR(Colors[i]));
                                    changed = true;
                                    break;
                                }

                            }
                            if (!changed && seek)
                            {
                               
                                    Vector3 c = ColorHelper.FindClosestColor(value, Colors);
                                    ga.Color.MakeStatic(Calculator.RGB_Vector_to_BGR(c));

                                

                            }
                        }
                       
                       
                    }
                    else if (ga.Color.Static && alpha && ga.UseColor)
                    {
                        Vector3 value = Calculator.BGRnToRGB_Vector(ga.Color.GetValue());
                        bool changed = false;
                        for (int i = 0; i < Colors.Count; i++)
                        {
                            if (Colors[i] == value)
                            {
                                ga.Color.MakeStatic(Calculator.RGB_Vector_to_BGR(Colors[i]));
                                changed = true;
                                break;
                            }

                        }
                        if (!changed)
                        {
                            if (seek)
                            {
                                // take selected color
                                var selected = Colors[index];

                                Vector3 c = ColorHelper.FindClosestColor(Colors[index], Colors);
                                ga.Color.MakeStatic(Calculator.RGB_Vector_to_BGR(c));

                            }
                             
                        }
                       
                    }
                }
            }

            if (tc)
            {
                foreach (var texture in model.Textures)
                {
                    if (texture.FileName.Length == 0) { continue; }
                    for (int i = 0; i < ColorCollecton.TC.Count; i++)
                    {
                        if (texture.FileName == ColorCollecton.TC[i])
                        {
                            texture.FileName = ColorCollecton.TC[index];
                        }
                    }
                   
                }
            }
            if (tg)
            {
                foreach (var texture in model.Textures)
                {
                    if (texture.FileName.Length == 0) { continue; }
                    for (int i = 0; i < ColorCollecton.TG.Count; i++)
                    {
                        if (texture.FileName == ColorCollecton.TG[i])
                        {
                            texture.FileName = ColorCollecton.TG[index];
                        }
                    }

                }
            }
            if (stars)
            {
                foreach (var texture in model.Textures)
                {
                    if (texture.FileName.Length == 0) { continue; }
                    for (int i = 0; i < ColorCollecton.Stars.Count; i++)
                    {
                        if (texture.FileName == ColorCollecton.Stars[i])
                        {
                            texture.FileName = ColorCollecton.Stars[index];
                        }
                    }

                }
            }
            if (glows)
            {
                foreach (var texture in model.Textures)
                {
                    if (texture.FileName.Length == 0) { continue; }
                    for (int i = 0; i < ColorCollecton.Glows.Count; i++)
                    {
                        if (texture.FileName == ColorCollecton.Glows[i])
                        {
                            texture.FileName = ColorCollecton.Glows[index];
                        }
                    }

                }
            }

            ColorCollecton.Free();
            DialogResult = true;
        }
    }
}
