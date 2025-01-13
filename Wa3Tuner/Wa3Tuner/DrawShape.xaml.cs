using MdxLib.Model;
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

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for DrawShape.xaml
    /// </summary>
  
    public partial class DrawShape : Window
    {
        enum DrawMethod { Pencil, Line}
        private CModel Model;
        private DrawMethod Method = DrawMethod.Pencil;
        private bool _isDrawing = false;
        private Point _startPoint;
        private Line _currentLine;

        public DrawShape()
        {
            InitializeComponent();
        }

        public DrawShape(CModel currentModel)
        {
            this.Model = currentModel;
        }

        private void SetModelPencil(object sender, RoutedEventArgs e)
        {
            Method = DrawMethod.Pencil;
        }

        private void SetModeLine(object sender, RoutedEventArgs e)
        {
            Method = DrawMethod.Line;
        }

        private void ok(object sender, RoutedEventArgs e)
        {

        }

        private void undo(object sender, RoutedEventArgs e)
        {

        }

        private void redo(object sender, RoutedEventArgs e)
        {

        }
    }
}
