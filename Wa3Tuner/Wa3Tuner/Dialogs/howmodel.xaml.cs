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
    /// Interaction logic for howmodel.xaml
    /// </summary>
    public partial class howmodel : Window
    {
        public howmodel()
        {
            InitializeComponent();
            box.Text = @"
Anatomy of a Model
A model consists of several components: geometry, textures, nodes, sequences, and cameras.
Geometry
Geometry defines the shape of a model and is composed of geosets.
    Geoset:
    A geoset contains:
        Vertices: Points in 3D space with attributes like position, normal, texture coordinates, and a reference to a matrix group.
            Position: Defined by X, Y, Z coordinates, it specifies where the vertex is located in space.
            Normal: A vector with X, Y, Z coordinates that determines how light interacts with the surface. It’s visualized as a line drawn from the vertex’s position in the direction of the normal.
            Texture Coordinates (U, V): These map the vertex to a specific point on a texture, allowing the geometry to be textured.
        Triangles: The smallest polygon type. Each triangle uses three vertices from the vertex list to form a surface.
        Matrix Groups: Collections of references to bones. These determine which vertices are attached to which bones, enabling animation.
        Sequence Extents: Cubical boundaries calculated from the geoset’s current extent and its changes during a sequence. These are essential for in-game collision detection and interactions.
Nodes
Nodes define the structure and functionality of the model. Types include:
    Bone: Vertices can only attach to bones. If the bone is animated, the connected vertices move with it, animating the model.
    Attachment: Allows other models to attach to the current model in-game.
    Collision Shape: Helps units in-game be easily hovered over and clicked.
    Helper: A placeholder node used for organizing and structuring other nodes.
    Emitters (Emitter1, Emitter2, Ribbon): Used for creating particle effects like fire, smoke, or magical trails.
    Event Object: Generates sounds (e.g., death cries) or visual sprites (e.g., footsteps or blood).
    Light: Illuminates parts of the model or its surroundings in-game, enhancing the visual atmosphere.
Textures
Without textures, a model would appear plain.
    Materials: Each geoset uses a material.
        Layers: Materials consist of one or more layers, each with specific tags that define its appearance, the texture it uses, and its texture animation.
Sequences
A sequence defines an interval during which all transformations from nodes, layers, cameras, texture animations, and geoset animations occur. This makes the model ""come to life"" by playing animations.
Cameras
Global Sequences
A global sequence is a sequene that continues to play in any sequence for its duration.
Cameras are used to define specific viewpoints:
    Commonly used for unit portraits in-game.
    Sometimes used for campaign loading screens.
";
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
