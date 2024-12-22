using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for Transformations_viewer.xaml
    /// </summary>
    public partial class Transformations_viewer : Window
    {
        public Transformations_viewer(INode node)
        {
            InitializeComponent();
            TextBlockColumn1.Text = "Translation";
            TextBoxColumn1.Text = GetData(node.Translation);
            TextBlockColumn2.Text = "Rotation";
            TextBoxColumn2.Text = GetData(node.Rotation);
            TextBlockColumn3.Text = "Scaling";
            TextBoxColumn3.Text = GetData(node.Scaling);
        }
        public Transformations_viewer(CGeosetAnimation ga)
        {
            InitializeComponent();
            TextBlockColumn1.Text = "Alpha";
            TextBoxColumn1.Text = GetData(ga.Alpha);
            TextBlockColumn2.Text = "Color";
            TextBoxColumn2.Text = GetData(ga.Color);
        }
        private string GetData(CAnimator<CVector3> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                sb.AppendLine($"Track {item.Time}: {item.Value.X}, {item.Value.Y}, {item.Value.Z}");
            }
            return sb.ToString();
        }
        private string GetData(CAnimator<CVector4> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                sb.AppendLine($"Track {item.Time}: {item.Value.X}, {item.Value.Y}, {item.Value.Z}, {item.Value.W}");
            }
            return sb.ToString();
        }
        private string GetData(CAnimator<float> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                sb.AppendLine($"Track {item.Time}: {item.Value}");
            }
            return sb.ToString();
        }
    }
}
