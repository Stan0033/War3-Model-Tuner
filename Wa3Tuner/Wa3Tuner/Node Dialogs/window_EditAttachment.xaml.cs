using MdxLib.Model;
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
    /// Interaction logic for window_EditAttachment.xaml
    /// </summary>
    public partial class window_EditAttachment : Window
    {
        CModel Model;
        CAttachment Attachment;
        public window_EditAttachment(MdxLib.Model.INode node, CModel model)
        {
            InitializeComponent();
            Attachment = node as CAttachment;
            Model = model;
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            Attachment.Path = Path.Text.Trim();
            DialogResult = true;
        }
        private void EditVis(object sender, RoutedEventArgs e)
        {
            transformation_editor tr = new transformation_editor(Model, Attachment.Visibility, false, TransformationType.Visibility);
            tr.ShowDialog();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
