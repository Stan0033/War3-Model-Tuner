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
        CModel Model;
        public Transformations_viewer(INode node, CModel geo)
        {
            
            InitializeComponent();
            Model = geo;
            TextBlockColumn1.Text = "Translation";
            TextBoxColumn1.Text = GetData(node.Translation);
            TextBlockColumn2.Text = "Rotation";
            TextBoxColumn2.Text = GetData(node.Rotation);
            TextBlockColumn3.Text = "Scaling";
            TextBoxColumn3.Text = GetData(node.Scaling);
        }
        public Transformations_viewer(CTextureAnimation ta, CModel model)
        {
            InitializeComponent();
            Model = model;
            TextBlockColumn1.Text = "Translation";
            TextBlockColumn1.Text = "Rotation";
            TextBlockColumn1.Text = "Scaling";
            TextBoxColumn1.Text = GetData(ta.Translation);
            TextBoxColumn2.Text = GetData(ta.Rotation);
            TextBoxColumn3.Text = GetData(ta.Scaling);
        }
        public Transformations_viewer(CGeosetAnimation ga, CModel geo)
        {

            InitializeComponent();
            Model = geo;
            TextBlockColumn1.Text = "Alpha";
            TextBoxColumn1.Text = GetData(ga.Alpha);
            TextBlockColumn2.Text = "Color";
            TextBoxColumn2.Text = GetData(ga.Color);
        }
        public Transformations_viewer(CMaterialLayer layer, CModel geo)
        {

            InitializeComponent();
            Model = geo;
            TextBlockColumn1.Text = "Alpha";
            TextBoxColumn1.Text = GetData(layer.Alpha);
            TextBlockColumn2.Text = "TextureId";
            TextBoxColumn2.Text = GetData(layer.TextureId);
        }
        private string GetData(CAnimator<CVector3> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                sb.AppendLine($"({FindS(item.Time)}) Track {item.Time}: {item.Value.X}, {item.Value.Y}, {item.Value.Z}");
            }
            return sb.ToString();
        }
        private string GetData(CAnimator<int> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                string texture = FindTextureByID(item.Value);
                sb.AppendLine($"({FindS(item.Time)}) Track {item.Time}: {item.Value} ");
            }
            return sb.ToString();
        }

        private string FindTextureByID(int value)
        {
           foreach (CTexture t in Model.Textures)
            {
                if (t.ObjectId == value) {
                    if (t.ReplaceableId == 0)
                    {
                        return t.FileName;
                    }
                    else
                    {
                        return $"Replaceable ID {t.ReplaceableId}";
                    }
                }
            }
            return "Not Found";
        }

        private string FindS(int track)
        {
            string result = "No Sequence";
            foreach (CSequence sequenc in Model.Sequences)
            {
                if (track >= sequenc.IntervalStart &&  track <= sequenc.IntervalEnd)
                {
                    return sequenc.Name;
                }
            }
            return result;
        }
        private string GetData(CAnimator<CVector4> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                sb.AppendLine($"{FindS(item.Time)}) Track {item.Time}: {item.Value.X}, {item.Value.Y}, {item.Value.Z}, {item.Value.W}");
            }
            return sb.ToString();
        }
        private string GetData(CAnimator<float> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in animator)
            {
                sb.AppendLine($"{FindS(item.Time)}) Track {item.Time}: {item.Value}");
            }
            return sb.ToString();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
