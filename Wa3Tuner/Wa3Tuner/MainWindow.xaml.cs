using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
 
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
 
using W3_Texture_Finder;
using Wa3Tuner.Dialogs;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
namespace Wa3Tuner
{
    enum AnimatorMode
    {
        Translate, Rotate, Scale
    }
    enum AnimatorAxis
    {
        X, Y, Z,
        U
    }
    enum UVEditMode { Move, Rotate, Scale }
    enum GeometryEditMode { Geosets, Rigging, Animator, UVMapper }
    enum CopiedKeyframe { Translation, Rotation, SCaling, all }
    enum UVLockType
    {
        None, U, V
    }
    public enum Axes { X, Y, Z }
    enum WorkMode
    {
        Select, Vertices, Edges, Faces
    }
    enum RiggingAction { Add, Remove, ClearAdd }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool RenderGroundPlane = false;
        private bool RenderTextures = true;
        private bool RenderShading = true;
        private bool RenderCollisionShapes = false;
        private bool RenderGround = false;
        private bool RenderSkinning = false;
        private bool RenderGeosetExtents = false;
        private bool RenderGeosetExtentSphere = false;
        private bool RenderNodes = false;
        private bool RenderGeometry = false;

        public string CurrentSaveLocaiton = string.Empty;
        public string CurrentSaveFolder = string.Empty;
        public CModel CurrentModel = new CModel();
        private string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private string IconsPath;
        List<string> TeamColorPaths;
        List<string> TeamGlows;
        Dictionary<MenuItem, int> TeamReference;
        int CurrentTeamColor = 0;
        public TextureBrowser TextureFinder;

        NodeMaker NodeCollection = new NodeMaker();
        private ImageSource[] Textures_Loaded;
        Dictionary<string, string> IconPaths = new Dictionary<string, string>();
        List<string> Recents = new List<string>();
        BitmapSource GroundTexture;
        public MainWindow()
        {
            Pause = true;
            InitializeComponent();
            Initialize();
            LoadGroundTexture();
            Pause = false;
        }
        private void LoadGroundTexture()
        {
            string path = Path.Combine(AppPath, "icons\\grass.png");

            GroundTexture = AppHelper.LoadBitmapImage(path);


        }
        public void Initialize()
        {
            IconsPath = System.IO.Path.Combine(AppPath, "Icons"); ;
            InitIcons();
            LoadRecents();
            // MessageBox.Show(MPQPaths.local);
            MPQFinder.Find();
            MPQHelper.Initialize();
            TextureFinder = new TextureBrowser(this, MPQHelper.Listfile_All);

            InitializeTeamColorPaths();
            //  NodeCollection = new NodeMaker();
        }
        private void InitializeTeamColorPaths()
        {
            var Brushes = new List<Brush>
        {
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Teal),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.Yellow),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Gray),
            new SolidColorBrush(Colors.LightBlue),
            new SolidColorBrush(Colors.DarkGreen),
            new SolidColorBrush(Colors.Brown),
            new SolidColorBrush(Colors.Maroon),
            new SolidColorBrush(Colors.Navy),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#40E0D0")), // Turquoise
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE82EE")), // Violet
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5DEB3")), // Wheat
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDAB9")), // Peach
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#98FF98")), // Mint
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6E6FA")), // Lavender
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F353B")), // Coal (dark gray)
            new SolidColorBrush(Colors.Snow),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#50C878")), // Emerald
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2B48C")), // Peanut (Tan)
            new SolidColorBrush(Colors.Black)
        };
            TeamColorPaths = new List<string>();
            TeamGlows = new List<string>();
            TeamReference = new Dictionary<MenuItem, int>();
            bool hasReforged = MPQHelper.FileExists(($"ReplaceableTextures\\TeamColor\\TeamColor20.blp"));
            int max = hasReforged ? 24 : 12;
            for (int i = 0; i < max; i++)
            {
                string num = i < 10 ? "0" + i.ToString() : i.ToString();
                TeamColorPaths.Add($"ReplaceableTextures\\TeamColor\\TeamColor{num}.blp");
                TeamGlows.Add($"ReplaceableTextures\\TeamGlow\\TeamGlow{num}.blp");
                MenuItem item = new MenuItem() { Header = "", Background = Brushes[i] };
                item.Click += ClickedTeamColor;
                MenuTeamColor.Items.Add(item);
                TeamReference.Add(item, i);
            }
            TeamColorPaths.Add("Textures\\Black32.blp");
            TeamGlows.Add("Textures\\Black32.blp");
        }
        private void ClickedTeamColor(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            CurrentTeamColor = TeamReference[item];
            RefreshViewPort();
        }
        public MainWindow(string file) // for opening by double click
        {
            InitializeComponent();
            Initialize();
            LoadModel(file);
        }

        private void RenderNodesInViewport(Viewport3D viewport)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                float x = node.PivotPoint.X;
                float y = node.PivotPoint.Y;
                float z = node.PivotPoint.Z;

                // Default color is gray
                byte r = 192, g = 192, b = 192;
                if (node is CAttachment) { r = 255; g = 255; b = 255; } // White
                if (node is CBone) { r = 0; g = 0; b = 0; } // Black
                if (node is CLight) { b = 0; } // Yellow
                if (node is CCollisionShape) { r = 0; g = 0; } // Blue
                if (node is CRibbonEmitter) { g = 0; b = 0; } // Red
                if (node is CParticleEmitter) { r = 0; b = 0; } // Green
                if (node is CParticleEmitter2) { r = 128; g = 0; b = 128; } // Purple
                if (node is CEvent) { r = 173; g = 216; b = 230; } // Light Blue
                if (node is CHelper) { r = 209; g = 139; b = 123; } // Light Brown

                // Create a cube geometry
                MeshGeometry3D cube = new MeshGeometry3D();
                double size = 5.0;
                Point3D p0 = new Point3D(x - size, y - size, z - size);
                Point3D p1 = new Point3D(x + size, y - size, z - size);
                Point3D p2 = new Point3D(x + size, y + size, z - size);
                Point3D p3 = new Point3D(x - size, y + size, z - size);
                Point3D p4 = new Point3D(x - size, y - size, z + size);
                Point3D p5 = new Point3D(x + size, y - size, z + size);
                Point3D p6 = new Point3D(x + size, y + size, z + size);
                Point3D p7 = new Point3D(x - size, y + size, z + size);

                cube.Positions.Add(p0); cube.Positions.Add(p1); cube.Positions.Add(p2);
                cube.Positions.Add(p3); cube.Positions.Add(p4); cube.Positions.Add(p5);
                cube.Positions.Add(p6); cube.Positions.Add(p7);

                // Define triangles for each face
                int[] indices = {
            0, 1, 2, 2, 3, 0, // Front
            1, 5, 6, 6, 2, 1, // Right
            5, 4, 7, 7, 6, 5, // Back
            4, 0, 3, 3, 7, 4, // Left
            3, 2, 6, 6, 7, 3, // Top
            4, 5, 1, 1, 0, 4  // Bottom
        };
                foreach (int i in indices)
                {
                    cube.TriangleIndices.Add(i);
                }

                // Define the material color
                Color color = Color.FromRgb(r, g, b);
                DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(color));

                // Create a 3D model
                GeometryModel3D model = new GeometryModel3D(cube, material);

                // Add the model to the viewport
                ModelVisual3D visual = new ModelVisual3D { Content = model };
                viewport.Children.Add(visual);
            }
        }

        private void LoadRecents()
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "recents.txt");
            if (File.Exists(file))
            {
                foreach (string line in File.ReadAllLines(file))
                {
                    Recents.Add(line);
                }
            }
            RefreshRecents();
        }
        private void RefreshRecents()
        {
            Item_Recents.Items.Clear();
            foreach (string recent in Recents)
            {
                MenuItem item = new MenuItem();
                item.Header = recent;
                item.Click += OpenRecent;
                Item_Recents.Items.Add(item);
            }
            MenuItem clear = new MenuItem();
            clear.Header = "Clear";
            clear.Click += clearRecents;
            Item_Recents.Items.Add(clear);
        }
        private void SaveRecents()
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "recents.txt");
            File.WriteAllText(file, "");
            File.WriteAllLines(file, Recents.ToArray());
            RefreshRecents();
        }
        private void OpenRecent(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string name = item.Header.ToString();
            LoadModel(name);
        }
        private void InitIcons()
        {
            IconPaths.Add(nameof(CBone), System.IO.Path.Combine(IconsPath, "bone.png"));
            IconPaths.Add(nameof(CAttachment), System.IO.Path.Combine(IconsPath, "attach.png"));
            IconPaths.Add(nameof(CCollisionShape), System.IO.Path.Combine(IconsPath, "cols.png"));
            IconPaths.Add(nameof(CParticleEmitter), System.IO.Path.Combine(IconsPath, "emitter1.png"));
            IconPaths.Add(nameof(CParticleEmitter2), System.IO.Path.Combine(IconsPath, "emitter2.png"));
            IconPaths.Add(nameof(CEvent), System.IO.Path.Combine(IconsPath, "event.png"));
            IconPaths.Add(nameof(CHelper), System.IO.Path.Combine(IconsPath, "info.png"));
            IconPaths.Add(nameof(CLight), System.IO.Path.Combine(IconsPath, "light.png"));
            IconPaths.Add(nameof(CRibbonEmitter), System.IO.Path.Combine(IconsPath, "emitter3.png"));
            IconPaths.Add("lock", System.IO.Path.Combine(IconsPath, "Lock.png"));
            IconPaths.Add("unlock", System.IO.Path.Combine(IconsPath, "Unlocked.png"));
            IconPaths.Add("ground", System.IO.Path.Combine(IconsPath, "grass.png"));
            SetBackgroundImage(ButtonLockV, IconPaths["unlock"]);
            SetBackgroundImage(ButtonLockU, IconPaths["unlock"]);

        }
        public void SetBackgroundImage(Button button, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || button == null)
                return;

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            brush.Stretch = Stretch.UniformToFill; // Ensures the image fills the button properly

            button.Background = brush;
        }
        public string OpenModelFileDialog()
        {
            // Create an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Model File",
                Filter = "Model Files (*.mdl;*.mdx)|*.mdl;*.mdx",
                DefaultExt = ".mdl",
                CheckFileExists = true,
                CheckPathExists = true
            };
            // Show the dialog and get the result
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            // Return null if no file was selected
            return null;
        }
        private void LoadModel(string FromFileName)
        {
            if (!File.Exists(FromFileName))
            {
                MessageBox.Show("File does not exist"); return;
            }
            //CurrentModel = 
            CModel TemporaryModel = new CModel();
            string extension = System.IO.Path.GetExtension(FromFileName).ToLower();
            if (extension == ".mdx")
            {
                try
                {
                    using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdx();
                        ModelFormat.Load(FromFileName, Stream, TemporaryModel);
                        CurrentSaveLocaiton = FromFileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    // CurrentModel = new CModel();
                    return;
                }
            }
            if (extension == ".mdl")
            {
                try
                {
                    using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdl();
                        ModelFormat.Load(FromFileName, Stream, TemporaryModel);
                        CurrentSaveLocaiton = FromFileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    //  CurrentModel = new CModel();
                    return;
                }
            }
            CurrentModel = TemporaryModel;
            MultiplyAlphasForEmitter2_MDL();
            RenameAllNodes();
            Optimizer.RemoveInvalidGeosetAnimations(CurrentModel);
            Box_Errors.Text = "";
            LabelDisplayInfo.Text = "";
            CurrentSaveLocaiton = FromFileName;
            CurrentSaveFolder = Path.GetDirectoryName(CurrentSaveLocaiton); ;
            MPQHelper.LocalModelFolder = CurrentSaveFolder;
            CollectTextures();
            RefreshAll();
            CParticleEmitter2 emitter = CurrentModel.Nodes[0] as CParticleEmitter2;
            if (Recents.Contains(FromFileName) == false) { Recents.Add(FromFileName); SaveRecents(); RefreshRecents(); }
        }
        private void load(object sender, RoutedEventArgs e)
        {
            string FromFileName = OpenModelFileDialog();
            if (!File.Exists(FromFileName)) { return; }
            LoadModel(FromFileName);
        }
        private void MultiplyAlphasForEmitter2_MDL()
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 emitter = (CParticleEmitter2)node;
                    emitter.Segment1.Alpha *= 255;
                    emitter.Segment2.Alpha *= 255;
                    emitter.Segment3.Alpha *= 255;
                }
            }
        }
        private void DivideAlphasForEmitter2_MDL()
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 emitter = (CParticleEmitter2)node;
                    emitter.Segment1.Alpha /= 255;
                    emitter.Segment2.Alpha /= 255;
                    emitter.Segment3.Alpha /= 255;
                }
            }
        }
        private void RefreshAll()
        {
            ListOptions.IsEnabled = true;
            Title = "War3 Model Tuner - " + CurrentSaveLocaiton;
            RefreshGeosetsList();
            RefreshNodesTree();
            RefreshSequencesList();
            RefreshTextures();
            RefreshMaterialsList();
            RefreshTextureAnims();
            RefreshGlobalSequencesList();
            List_Layers.Items.Clear();
            RefreshGeosetAnimationsList();
            RefreshLayersTextureList();
            RefreshLayersTextureAnimList();
            RefreshViewPort();
            //   RenderGeosets(Viewport_Main);
        }
        public void RefreshTextures()
        {
            List_Textures.Items.Clear();
            foreach (CTexture texture in CurrentModel.Textures)
            {
                if (texture.ReplaceableId == 0)
                {
                    ListBoxItem item = new ListBoxItem() { Content = texture.FileName };
                    Image image = new Image();
                    if (texture.ReplaceableId == 0)
                    {
                        if (Path.GetExtension(texture.FileName).ToLower() == ".blp")
                        {
                            image.Source = MPQHelper.GetImageSource(texture.FileName);
                            if (image.Source == null)
                            {
                                string path = Path.Combine(CurrentSaveFolder, texture.FileName);
                                image.Source = MPQHelper.GetImageSourceExternal(path);
                            }

                        }
                        else
                        {
                            MessageBox.Show($"Could not load {texture.FileName}, because it is not a BLP image");
                            image.Source = MPQHelper.GetImageSource(White);

                        }

                    }
                    if (texture.ReplaceableId == 1)
                    {
                        image.Source = MPQHelper.GetImageSource(TeamColor);
                    }
                    if (texture.ReplaceableId == 2)
                    {
                        image.Source = MPQHelper.GetImageSource(TeamGlow);
                    }
                    if (texture.ReplaceableId > 2)
                    {
                        image.Source = MPQHelper.GetImageSource(White);
                    }
                    item.ToolTip = image;
                    List_Textures.Items.Add(item);
                }
                else
                {
                    string name = $"Repalceable ID {texture.ReplaceableId}";
                    if (texture.ReplaceableId == 1) name = "Team Color";
                    if (texture.ReplaceableId == 2) name = "Team Glow";
                    List_Textures.Items.Add(new ListBoxItem() { Content = name });
                }
            }
            LabelTextues.Text = $"Textures - {CurrentModel.Textures.Count}";
        }
        private void RefreshSequencesList()
        {
            ListSequenes.Items.Clear();
            Report_sequences.Text = $"{CurrentModel.Sequences.Count} sequences";
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                string looping = sequence.NonLooping ? "Nonlooping" : "Looping";
                string data = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}] ({looping})";
                ListSequenes.Items.Add(new ListBoxItem() { Content = data });
            }
        }
        private void CleanGeosetVisible()
        {
            foreach (var geoset in GeosetVisible)
            {
                if (CurrentModel.Geosets.Contains(geoset.Key) == false)
                {
                    GeosetVisible.Remove(geoset.Key);
                }
            }
        }
        private Dictionary<CGeoset, bool> GeosetVisible = new Dictionary<CGeoset, bool>();
        private void RefreshGeosetsList()
        {
            ListGeosets.Items.Clear();
            Report_Geosets.Text = $"{CurrentModel.Geosets.Count} geosets";
            CleanGeosetVisible();
            GeosetVisible.Clear();
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                ListBoxItem Item = new ListBoxItem();
                int UsedMaterialIndex = CurrentModel.Materials.IndexOf(geo.Material.Object);
                TextBlock Title = new TextBlock();
                string TitleName = geo.ObjectId.ToString() + $" ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles) (material {UsedMaterialIndex})";
                Title.Text = TitleName;
                CheckBox CheckPart = new CheckBox();
                StackPanel Container = new StackPanel();
                Container.Orientation = Orientation.Horizontal;
                Container.Children.Add(CheckPart);
                Container.Children.Add(Title);
                CheckPart.IsChecked = true;
                GeosetVisible.Add(geo, true);
                CheckPart.Checked += (object sender, RoutedEventArgs e) => { GeosetVisible[geo] = true; RefreshViewPort(); };
                CheckPart.Unchecked += (object sender, RoutedEventArgs e) => { GeosetVisible[geo] = false; RefreshViewPort(); };
                Item.Content = Container;
                ListGeosets.Items.Add(Item);
            }
        }
        private void CheckedGeosetVisibility(object sender, EventArgs e)
        {
            if (sender is CheckBox == false) { return; }
            int index = 0;
            ListBoxItem item = null;
            CheckBox CheckedBox = sender as CheckBox;
            for (int i = 0; i < ListGeosets.Items.Count; i++)
            {
                ListBoxItem box = ListGeosets.Items[i] as ListBoxItem;
                StackPanel p = box.Content as StackPanel;
                CheckBox c = p.Children[0] as CheckBox;
                if (c == CheckedBox) { item = box; index = i; break; }
            }
            if (item == null) { return; }
            bool visible = CheckedBox.IsChecked == true;
            CGeoset geoset = CurrentModel.Geosets[index];
            GeosetVisible[geoset] = visible;
            RefreshViewPort();
        }
        private CGeoset GetGEosetOfCheckbox(CheckBox c)
        {
            string start = c.Content.ToString().Split(" ")[0];
            int id = int.Parse(start);
            return CurrentModel.Geosets.First(X => X.ObjectId == id);
        }
        private void RefreshNodesTree()
        {
            ListNodes.Items.Clear();
            Report_Nodes.Text = $"{CurrentModel.Nodes.Count} nodes";
            foreach (INode node in CurrentModel.Nodes)
            {
                if (!hasParent(node))
                { // root
                    TreeViewItem item = GetTreeViewItem(node);
                    ListNodes.Items.Add(item);
                    if (HasChildren(node))
                    {
                        AddChildren(node, item, new HashSet<INode>());
                    }
                }
            }
        }
        private void AddChildren(INode inputNode, TreeViewItem item, HashSet<INode> visited)
        {
            if (visited.Contains(inputNode)) return;
            visited.Add(inputNode);
            foreach (INode targetNode in CurrentModel.Nodes)
            {
                if (targetNode.Parent?.Node == inputNode)
                {
                    TreeViewItem newItem = GetTreeViewItem(targetNode);
                    item.Items.Add(newItem);
                    if (HasChildren(targetNode))
                    {
                        AddChildren(targetNode, newItem, visited);
                    }
                }
            }
        }
        private bool HasChildren(INode inputNode)
        {
            foreach (INode targetNode in CurrentModel.Nodes)
            {
                if (targetNode.Parent?.Node == inputNode)
                {
                    return true;
                }
            }
            return false;
        }
        public TreeViewItem GetTreeViewItem(INode node)
        {
            TreeViewItem item = new TreeViewItem();
            item.Width = 250;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            //-----------------------------------
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            //-----------------------------------
            Image icon = new Image();
            icon.Height = 16; icon.Width = 16;
            icon.Source = GetImageForNodeType(node);
            icon.Margin = new Thickness(0, 0, 5, 0);
            //-----------------------------------
            TextBlock text = new TextBlock(); text.Text = node.Name;
            text.Foreground = Brushes.Black; text.Height = 20;
            //-----------------------------------
            panel.Children.Add(icon);
            panel.Children.Add(text);
            //-----------------------------------
            item.Header = panel;
            return item;
        }
        private ImageSource GetImageForNodeType(INode node)
        {
            if (node is CBone) return LoadImageSource(IconPaths[nameof(CBone)]);
            if (node is CAttachment) return LoadImageSource(IconPaths[nameof(CAttachment)]);
            if (node is CCollisionShape) return LoadImageSource(IconPaths[nameof(CCollisionShape)]);
            if (node is CHelper) return LoadImageSource(IconPaths[nameof(CHelper)]);
            if (node is CParticleEmitter) return LoadImageSource(IconPaths[nameof(CParticleEmitter)]);
            if (node is CParticleEmitter2) return LoadImageSource(IconPaths[nameof(CParticleEmitter2)]);
            if (node is CRibbonEmitter) return LoadImageSource(IconPaths[nameof(CRibbonEmitter)]);
            if (node is CEvent) return LoadImageSource(IconPaths[nameof(CEvent)]);
            if (node is CLight) return LoadImageSource(IconPaths[nameof(CLight)]);
            throw new Exception("unknown type of node");
        }
        private ImageSource LoadImageSource(string filePath)
        {
            if (filePath.Length == 0)
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file was not found.", filePath);
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Ensure thread safety
                return bitmapImage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load image.", ex);
            }
        }
        private void save(object sender, RoutedEventArgs e)
        {
            if (CurrentSaveLocaiton.Length == 0) { MessageBox.Show("Empty save path"); return; }
            if (
               (System.IO.Path.GetExtension(CurrentSaveLocaiton) == ".mdx" ||
                System.IO.Path.GetExtension(CurrentSaveLocaiton) == ".mdl") == false
                ) { MessageBox.Show("Invalid extension"); return; }
            if (CurrentModel == null) { MessageBox.Show("Null model"); return; }
            if (System.IO.Path.GetExtension(CurrentSaveLocaiton).ToLower() == ".mdl")
            {
                string ToFileName = CurrentSaveLocaiton;
                DivideAlphasForEmitter2_MDL();
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdl();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
                FileCleaner.CleanFile(ToFileName);
                MultiplyAlphasForEmitter2_MDL();
            }
            if (System.IO.Path.GetExtension(CurrentSaveLocaiton).ToLower() == ".mdx")
            {
                string ToFileName = CurrentSaveLocaiton;
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdx();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
            }
        }
        public string ShowSaveFileDialog()
        {
            // Create a SaveFileDialog instance
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "Model Files (*.mdl;*.mdx)|*.mdl;*.mdx", // Filter for .mdl and .mdx
                DefaultExt = ".mdl", // Default extension
                AddExtension = true  // Automatically add the extension
            };
            // Show the dialog and check the result
            bool? result = saveFileDialog.ShowDialog();
            return result == true ? saveFileDialog.FileName : string.Empty;
        }
        private void saveas(object sender, RoutedEventArgs e)
        {
            if (CurrentModel == null) { MessageBox.Show("Null model"); return; }
            string ToFileName = ShowSaveFileDialog();
            CurrentSaveLocaiton = ToFileName; save(null, null);
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Title = AppHelper.Name + " - " + CurrentSaveLocaiton;
        }

        public static void DrawCube(Viewport3D viewport, Point3D center, double size, System.Windows.Media.Color color)
        {
            double halfSize = size / 2;
            // Define the cube's vertices
            Point3D[] vertices = new Point3D[]
            {
            new Point3D(center.X - halfSize, center.Y - halfSize, center.Z - halfSize), // 0
            new Point3D(center.X + halfSize, center.Y - halfSize, center.Z - halfSize), // 1
            new Point3D(center.X + halfSize, center.Y + halfSize, center.Z - halfSize), // 2
            new Point3D(center.X - halfSize, center.Y + halfSize, center.Z - halfSize), // 3
            new Point3D(center.X - halfSize, center.Y - halfSize, center.Z + halfSize), // 4
            new Point3D(center.X + halfSize, center.Y - halfSize, center.Z + halfSize), // 5
            new Point3D(center.X + halfSize, center.Y + halfSize, center.Z + halfSize), // 6
            new Point3D(center.X - halfSize, center.Y + halfSize, center.Z + halfSize), // 7
            };
            // Define the cube's faces (triangles)
            int[] indices = new int[]
            {
            0, 1, 2,  0, 2, 3, // Front
            4, 6, 5,  4, 7, 6, // Back
            0, 4, 5,  0, 5, 1, // Bottom
            2, 6, 7,  2, 7, 3, // Top
            0, 7, 4,  0, 3, 7, // Left
            1, 5, 6,  1, 6, 2  // Right
            };
            // Create a mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            foreach (var vertex in vertices)
            {
                mesh.Positions.Add(vertex);
            }
            foreach (var index in indices)
            {
                mesh.TriangleIndices.Add(index);
            }
            // Create material
            Material material = new DiffuseMaterial(new SolidColorBrush(color));
            // Create a 3D model
            GeometryModel3D geometryModel = new GeometryModel3D(mesh, material);
            // Create a model visual
            ModelVisual3D modelVisual = new ModelVisual3D
            {
                Content = geometryModel
            };
            // Add the model to the viewport
            viewport.Children.Add(modelVisual);
        }
        public void AdjustCam()
        {
            Scene_Viewport.Camera = new PerspectiveCamera
            {
                Position = new Point3D(3, 3, 3),
                LookDirection = new Vector3D(-3, -3, -3),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
            };
        }
        private void delcameras(object sender, RoutedEventArgs e)
        {
            CurrentModel.Cameras.Clear();
        }
        private void delsequences(object sender, RoutedEventArgs e)
        {
            CurrentModel.Sequences.Clear();
            RefreshSequencesList();
        }
        private void delgss(object sender, RoutedEventArgs e)
        {
            CurrentModel.GlobalSequences.Clear();
        }
        private void delgeosets(object sender, RoutedEventArgs e)
        {
            CurrentModel.Geosets.Clear();
            RefreshGeosetsList();
        }
        private void deltxa(object sender, RoutedEventArgs e)
        {
            CurrentModel.TextureAnimations.Clear();
        }
        private void resetallgas(object sender, RoutedEventArgs e)
        {
        }
        private void MakeAllGAAlphaStatic(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                CurrentModel.GeosetAnimations[i].Alpha.Clear();
                CurrentModel.GeosetAnimations[i].Alpha.MakeStatic(1);
            }
            RefreshGeosetAnimationsList();
        }
        private void ImportAllGeosetsOf(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no materials. At least one material is needed to be applied to the imported geosets."); return;
            }
            {
                CModel TemporaryModel = GetTemporaryModel();
                if (TemporaryModel == null) { return; }
                foreach (CGeoset geoset in TemporaryModel.Geosets)
                {
                    CGeoset new_Geoset = DuplicateGeogeset(geoset, CurrentModel);
                    CurrentModel.Geosets.Add(new_Geoset);
                }
                RefreshGeosetsList();
                RefreshNodesTree();
            }
        }
        private CModel GetTemporaryModel(string file = "")
        {
            CModel model = new CModel();
            string FromFileName = file;
            if (FromFileName == "")
            {
                FromFileName = OpenModelFileDialog();
            }
            if (FromFileName != null)
            {
                //CurrentModel = 
                string extension = System.IO.Path.GetExtension(FromFileName).ToLower();
                if (extension == ".mdx")
                {
                    try
                    {
                        using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            var ModelFormat = new MdxLib.ModelFormats.CMdx();
                            ModelFormat.Load(FromFileName, Stream, model);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                        CurrentModel = new CModel();
                        return null;
                    }
                }
                if (extension == ".mdl")
                {
                    try
                    {
                        using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            var ModelFormat = new MdxLib.ModelFormats.CMdl();
                            ModelFormat.Load(FromFileName, Stream, model);
                            RefreshAll();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                        CurrentModel = new CModel();
                        return null;
                    }
                }
            }
            return
                model;
        }
        private void ImportAllNodesOf(object sender, RoutedEventArgs e)
        {
            import_nodes_choice i = new import_nodes_choice();
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                int choice = i.selected;
                CModel TemporaryModel = GetTemporaryModel();
                if (TemporaryModel == null) { return; }
                if (choice == 1)
                {
                    AppendDifferentNodes(CurrentModel, TemporaryModel);
                    RefreshNodesTree();
                } // append only
                if (choice == 2) // overwrite, dont append
                {
                    OVerwriteKeyframesForMatchingNodes(CurrentModel, TemporaryModel);
                }
                if (choice == 3) // overwrite, and append
                {
                    OVerwriteKeyframesForMatchingNodes(CurrentModel, TemporaryModel);
                    AppendDifferentNodes(CurrentModel, TemporaryModel);
                    RefreshNodesTree();
                }
                if (choice == 4)
                { // overwrite all
                    OverwriteWholeNodeStructure(CurrentModel, TemporaryModel);
                }
                if (choice == 5)
                {
                    ImportEmitters2AndEventObjects(CurrentModel, TemporaryModel);
                    RefreshNodesTree();
                }
                {
                    CurrentModel.Nodes.Clear();
                    foreach (INode node in TemporaryModel.Nodes)
                    {
                        CurrentModel.Nodes.Add(node);
                    }
                }
                RefreshNodesTree();
            }
        }
        private void AppendDifferentNodes(CModel currentModel, CModel temporaryModel)
        {
            foreach (INode node in temporaryModel.Nodes)
            {
                if (currentModel.Nodes.Any(x => x.Name.ToLower() == node.Name.ToLower()) == false)
                {
                    currentModel.Nodes.Add(NodeCloner.Clone(node, currentModel));
                }
            }
            throw new NotImplementedException();
        }
        private void ImportEmitters2AndEventObjects(CModel currentModel, CModel temporaryModel)
        {
            List<INode> nodes = new List<INode>();
            foreach (INode node in temporaryModel.Nodes)
            {
                if (node is CParticleEmitter || node is CParticleEmitter2 || node is CRibbonEmitter || node is CEvent)
                {
                    nodes.Add(NodeCloner.Clone(node, currentModel));
                }
            }
            foreach (INode node in nodes)
            {
                currentModel.Nodes.Add(node);
            }
        }
        private void OverwriteWholeNodeStructure(CModel currentModel, CModel temporaryModel)
        {
            currentModel.Nodes.Clear();
            Dictionary<INode, INode> NodeReferenceOldNew = new Dictionary<INode, INode>();
            foreach (INode node in temporaryModel.Nodes)
            {
                NodeReferenceOldNew.Add(node, NodeCloner.Clone(node, currentModel));
            }
            foreach (INode node in temporaryModel.Nodes)
            {
                if (node.Parent != null && node.Parent.ObjectId != -1)
                {
                    NodeReferenceOldNew[node].Parent.Attach(NodeReferenceOldNew[node.Parent.Node]);
                }
            }
            RefreshNodesTree();
        }
        private void OVerwriteKeyframesForMatchingNodes(CModel currentModel, CModel temporaryModel)
        {
            foreach (INode externalNode in temporaryModel.Nodes)
            {
                if (currentModel.Nodes.Any(x => x.Name.ToLower() == externalNode.Name.ToLower()))
                {
                    INode matchingNode = currentModel.Nodes.First(x => x.Name.ToLower() == externalNode.Name.ToLower());
                    matchingNode.Translation.Clear();
                    matchingNode.Rotation.Clear();
                    matchingNode.Scaling.Clear();
                    foreach (var item in externalNode.Translation)
                    {
                        CAnimatorNode<CVector3> keyframe = new CAnimatorNode<CVector3>(item.Time, new CVector3(item.Value));
                        matchingNode.Translation.Add(keyframe);
                    }
                    foreach (var item in externalNode.Rotation)
                    {
                        CAnimatorNode<CVector4> keyframe = new CAnimatorNode<CVector4>(item.Time, new CVector4(item.Value));
                        matchingNode.Rotation.Add(keyframe);
                    }
                    foreach (var item in externalNode.Scaling)
                    {
                        CAnimatorNode<CVector3> keyframe = new CAnimatorNode<CVector3>(item.Time, new CVector3(item.Value));
                        matchingNode.Scaling.Add(keyframe);
                    }
                    // now the data
                    if (externalNode is CLight)
                    {
                        CLight ExternalData = (CLight)externalNode;
                        CLight matchingData = (CLight)matchingNode;
                        // Clear properties in matchingData
                        matchingData.Visibility.Clear();
                        matchingData.Color.Clear();
                        matchingData.AmbientColor.Clear();
                        matchingData.Intensity.Clear();
                        matchingData.AmbientIntensity.Clear();
                        matchingData.AttenuationStart.Clear();
                        matchingData.AttenuationEnd.Clear();
                        // Transfer Visibility
                        if (ExternalData.Visibility.Static)
                        {
                            matchingData.Visibility.MakeStatic(ExternalData.Visibility.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Visibility)
                            {
                                matchingData.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Color
                        if (ExternalData.Color.Static)
                        {
                            matchingData.Color.MakeStatic(ExternalData.Color.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Color)
                            {
                                matchingData.Color.Add(new CAnimatorNode<CVector3>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer AmbientColor
                        if (ExternalData.AmbientColor.Static)
                        {
                            matchingData.AmbientColor.MakeStatic(ExternalData.AmbientColor.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.AmbientColor)
                            {
                                matchingData.AmbientColor.Add(new CAnimatorNode<CVector3>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Intensity
                        if (ExternalData.Intensity.Static)
                        {
                            matchingData.Intensity.MakeStatic(ExternalData.Intensity.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Intensity)
                            {
                                matchingData.Intensity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer AmbientIntensity
                        if (ExternalData.AmbientIntensity.Static)
                        {
                            matchingData.AmbientIntensity.MakeStatic(ExternalData.AmbientIntensity.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.AmbientIntensity)
                            {
                                matchingData.AmbientIntensity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer AttenuationStart
                        if (ExternalData.AttenuationStart.Static)
                        {
                            matchingData.AttenuationStart.MakeStatic(ExternalData.AttenuationStart.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.AttenuationStart)
                            {
                                matchingData.AttenuationStart.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer AttenuationEnd
                        if (ExternalData.AttenuationEnd.Static)
                        {
                            matchingData.AttenuationEnd.MakeStatic(ExternalData.AttenuationEnd.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.AttenuationEnd)
                            {
                                matchingData.AttenuationEnd.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                    }
                    if (externalNode is CParticleEmitter)
                    {
                        CParticleEmitter ExternalData = (CParticleEmitter)externalNode;
                        CParticleEmitter matchingData = (CParticleEmitter)matchingNode;
                        // Clear properties in matchingData
                        matchingData.Visibility.Clear();
                        matchingData.EmissionRate.Clear();
                        matchingData.LifeSpan.Clear();
                        matchingData.InitialVelocity.Clear();
                        matchingData.Gravity.Clear();
                        matchingData.Longitude.Clear();
                        matchingData.Latitude.Clear();
                        // Transfer Visibility
                        if (ExternalData.Visibility.Static)
                        {
                            matchingData.Visibility.MakeStatic(ExternalData.Visibility.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Visibility)
                            {
                                matchingData.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer EmissionRate
                        if (ExternalData.EmissionRate.Static)
                        {
                            matchingData.EmissionRate.MakeStatic(ExternalData.EmissionRate.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.EmissionRate)
                            {
                                matchingData.EmissionRate.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer LifeSpan
                        if (ExternalData.LifeSpan.Static)
                        {
                            matchingData.LifeSpan.MakeStatic(ExternalData.LifeSpan.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.LifeSpan)
                            {
                                matchingData.LifeSpan.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer InitialVelocity
                        if (ExternalData.InitialVelocity.Static)
                        {
                            matchingData.InitialVelocity.MakeStatic(ExternalData.InitialVelocity.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.InitialVelocity)
                            {
                                matchingData.InitialVelocity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Gravity
                        if (ExternalData.Gravity.Static)
                        {
                            matchingData.Gravity.MakeStatic(ExternalData.Gravity.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Gravity)
                            {
                                matchingData.Gravity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Longitude
                        if (ExternalData.Longitude.Static)
                        {
                            matchingData.Longitude.MakeStatic(ExternalData.Longitude.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Longitude)
                            {
                                matchingData.Longitude.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Latitude
                        if (ExternalData.Latitude.Static)
                        {
                            matchingData.Latitude.MakeStatic(ExternalData.Latitude.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Latitude)
                            {
                                matchingData.Latitude.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                    }
                    if (externalNode is CParticleEmitter2)
                    {
                        CParticleEmitter2 ExternalData = (CParticleEmitter2)externalNode;
                        CParticleEmitter2 matchingData = (CParticleEmitter2)matchingNode;
                        // Clear properties in matchingData
                        matchingData.Visibility.Clear();
                        matchingData.EmissionRate.Clear();
                        matchingData.Speed.Clear();
                        matchingData.Variation.Clear();
                        matchingData.Gravity.Clear();
                        matchingData.Latitude.Clear();
                        matchingData.Width.Clear();
                        matchingData.Length.Clear();
                        // Transfer Visibility
                        if (ExternalData.Visibility.Static)
                        {
                            matchingData.Visibility.MakeStatic(ExternalData.Visibility.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Visibility)
                            {
                                matchingData.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer EmissionRate
                        if (ExternalData.EmissionRate.Static)
                        {
                            matchingData.EmissionRate.MakeStatic(ExternalData.EmissionRate.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.EmissionRate)
                            {
                                matchingData.EmissionRate.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Speed
                        if (ExternalData.Speed.Static)
                        {
                            matchingData.Speed.MakeStatic(ExternalData.Speed.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Speed)
                            {
                                matchingData.Speed.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Variation
                        if (ExternalData.Variation.Static)
                        {
                            matchingData.Variation.MakeStatic(ExternalData.Variation.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Variation)
                            {
                                matchingData.Variation.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Gravity
                        if (ExternalData.Gravity.Static)
                        {
                            matchingData.Gravity.MakeStatic(ExternalData.Gravity.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Gravity)
                            {
                                matchingData.Gravity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Latitude
                        if (ExternalData.Latitude.Static)
                        {
                            matchingData.Latitude.MakeStatic(ExternalData.Latitude.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Latitude)
                            {
                                matchingData.Latitude.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Width
                        if (ExternalData.Width.Static)
                        {
                            matchingData.Width.MakeStatic(ExternalData.Width.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Width)
                            {
                                matchingData.Width.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Length
                        if (ExternalData.Length.Static)
                        {
                            matchingData.Length.MakeStatic(ExternalData.Length.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Length)
                            {
                                matchingData.Length.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                    }
                    if (externalNode is CRibbonEmitter)
                    {
                        CRibbonEmitter ExternalData = (CRibbonEmitter)externalNode;
                        CRibbonEmitter matchingData = (CRibbonEmitter)matchingNode;
                        // Clear properties in matchingData
                        matchingData.Visibility.Clear();
                        matchingData.HeightAbove.Clear();
                        matchingData.HeightBelow.Clear();
                        matchingData.Color.Clear();
                        matchingData.TextureSlot.Clear();
                        // Transfer Visibility
                        if (ExternalData.Visibility.Static)
                        {
                            matchingData.Visibility.MakeStatic(ExternalData.Visibility.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Visibility)
                            {
                                matchingData.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer HeightAbove
                        if (ExternalData.HeightAbove.Static)
                        {
                            matchingData.HeightAbove.MakeStatic(ExternalData.HeightAbove.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.HeightAbove)
                            {
                                matchingData.HeightAbove.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer HeightBelow
                        if (ExternalData.HeightBelow.Static)
                        {
                            matchingData.HeightBelow.MakeStatic(ExternalData.HeightBelow.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.HeightBelow)
                            {
                                matchingData.HeightBelow.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer Color
                        if (ExternalData.Color.Static)
                        {
                            matchingData.Color.MakeStatic(ExternalData.Color.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.Color)
                            {
                                matchingData.Color.Add(new CAnimatorNode<CVector3>(keyframe.Time, keyframe.Value));
                            }
                        }
                        // Transfer TextureSlot
                        if (ExternalData.TextureSlot.Static)
                        {
                            matchingData.TextureSlot.MakeStatic(ExternalData.TextureSlot.GetValue());
                        }
                        else
                        {
                            foreach (var keyframe in ExternalData.TextureSlot)
                            {
                                matchingData.TextureSlot.Add(new CAnimatorNode<int>(keyframe.Time, keyframe.Value));
                            }
                        }
                    }
                }
            }
        }
        private void ImportAnimations(object sender, RoutedEventArgs e)
        {
            bool AndSequences = false;
            MessageBoxResult result = MessageBox.Show(
       "This will replace the keyframes of existing duplicate nodes. Do you want to also generate the missing sequences?",
       "Confirmation",
       MessageBoxButton.YesNo,
       MessageBoxImage.Question
   );
            if (result == MessageBoxResult.Yes)
            {
                AndSequences = true;
            }
            CModel TemporaryModel = GetTemporaryModel();
            if (TemporaryModel == null) { return; }
            if (AndSequences)
            {
                RefreshSequencesList();
            }
        }
        private bool hasParent(INode inputNode)
        {
            foreach (INode targetNode in CurrentModel.Nodes.ToList())
            {
                if (inputNode.Parent.Node == targetNode)
                {
                    return true;
                }
            }
            return false;
        }
        private void RemoveAllLights(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CLight)
                {
                    CLight cLight = (CLight)node;
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void RemoveEmitters1(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CParticleEmitter)
                {
                    CParticleEmitter cLight = (CParticleEmitter)node;
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void RemoveEmitters2(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 cLight = (CParticleEmitter2)node;
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void RemoveAllAttachments(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CAttachment)
                {
                    CAttachment cLight = (CAttachment)node;
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void RemoAllCOLS(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CCollisionShape)
                {
                    CCollisionShape cLight = (CCollisionShape)node;
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void removeAllHelpers(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CHelper)
                {
                    CHelper cLight = (CHelper)node;
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void RemoveAllEventObjects(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                if (node is CEvent)
                {
                    if (HasChildren(node) == false)
                    {
                        CurrentModel.Nodes.Remove(node);
                    }
                }
            }
        }
        private void RemoveAllAnimations(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                node.Translation.Clear();
                node.Rotation.Clear();
                node.Scaling.Clear();
            }
        }
        private string GetNodeName()
        {
            TreeViewItem item = (ListNodes.SelectedItem as TreeViewItem);
            StackPanel s = item.Header as StackPanel;
            TextBlock t = (TextBlock)s.Children[1];
            return t.Text;
        }
        private string GetNodeNameAnimator()
        {
            TreeViewItem item = (List_Bones_Animator.SelectedItem as TreeViewItem);
            StackPanel s = item.Header as StackPanel;
            TextBlock t = (TextBlock)s.Children[1];
            return t.Text;
        }
        private void RenameNode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                string name = GetNodeName();
                INode node = CurrentModel.Nodes.First(x => x.Name == name);
                Input i = new Input(name);
                i.ShowDialog();
                if (i.DialogResult == true)
                {
                    string newName = i.Result;
                    if (CurrentModel.Nodes.Any(x => x.Name == newName))
                    {
                        MessageBox.Show("There is a already a nod with this name"); return;
                    }
                    node.Name = newName;
                    RenameSelectedNodeItem(newName);
                }
            }
        }
        private void RenameSelectedNodeItem(string name)
        {
            if (ListNodes.SelectedItem != null)
            {
                TreeViewItem item = ListNodes.SelectedItem as TreeViewItem;
                StackPanel s = item.Header as StackPanel;
                TextBlock t = s.Children[1] as TextBlock;
                t.Text = name;
            }
        }
        public static class IDCounter
        {
            private static int counter = 0;
            public static int Next() { counter++; return counter; }
            public static string Next_()
            {
                counter++; return counter.ToString();
            }
        }
        private void RenameAllNodes()
        {
            List<string> ExistingNames = new List<string>();
            foreach (INode node in CurrentModel.Nodes)
            {
                if (ExistingNames.Contains(node.Name))
                {
                    node.Name = node.Name + "_" + IDCounter.Next_();
                }
                ExistingNames.Add(node.Name);
            }
        }
        private INode GetSeletedNode()
        {
            string name = GetNodeName();
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void SetPivotPoint(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode selected = GetSeletedNode();
            InputVector v = new InputVector(AllowedValue.Both, selected.PivotPoint);
            v.ShowDialog();
            if (v.DialogResult == true)
            {
                selected.PivotPoint = new MdxLib.Primitives.CVector3(v.X, v.Y, v.Z);
            }
        }
        private CGeoset GetGeoset(string input)
        {
            int id = int.Parse(input);
            return CurrentModel.Geosets.First(x => x.ObjectId == id);
        }
        private List<string> GetGoesetStringItems()
        {
            List<string> list = new List<string>();
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                list.Add($"Geoset {geo.ObjectId} ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles)");
            }
            return list;
        }
        private void Setpiv(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode selected = GetSeletedNode();
            List<string> ids = GetGoesetStringItems();
            if (ids.Count == 0) { return; }
            Selector s = new Selector(ids);
            s.ShowDialog();
            if (s.DialogResult == true)
            {
                int index = s.box.SelectedIndex;
                CGeoset geo = GetGeoset(s.Selected);
                CVector3 centroid = Calculator.GetCentroidOfGeoset(geo);
                selected.PivotPoint = centroid;
            }
        }
        private void reverseSequence(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence s = GetSelectedSequence();
            ReverseSequence(s);
        }
        private void ReverseSequence(CSequence sequence)
        {
            //nodes
            foreach (INode node in CurrentModel.Nodes)
            {
                ReverseKeyframesForSequence(node.Translation, sequence);
                ReverseKeyframesForSequence(node.Rotation, sequence);
                ReverseKeyframesForSequence(node.Scaling, sequence);
                if (node is CParticleEmitter emitter)
                {
                    ReverseKeyframesForSequence(emitter.LifeSpan, sequence);
                    ReverseKeyframesForSequence(emitter.EmissionRate, sequence);
                    ReverseKeyframesForSequence(emitter.Gravity, sequence);
                    ReverseKeyframesForSequence(emitter.Visibility, sequence);
                    ReverseKeyframesForSequence(emitter.InitialVelocity, sequence);
                    ReverseKeyframesForSequence(emitter.Longitude, sequence);
                    ReverseKeyframesForSequence(emitter.Latitude, sequence);
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    ReverseKeyframesForSequence(emitter2.Gravity, sequence);
                    ReverseKeyframesForSequence(emitter2.Width, sequence);
                    ReverseKeyframesForSequence(emitter2.Length, sequence);
                    ReverseKeyframesForSequence(emitter2.Speed, sequence);
                    ReverseKeyframesForSequence(emitter2.Latitude, sequence);
                    ReverseKeyframesForSequence(emitter2.Visibility, sequence);
                    ReverseKeyframesForSequence(emitter2.Variation, sequence);
                    ReverseKeyframesForSequence(emitter2.EmissionRate, sequence);
                }
                if (node is CLight light)
                {
                    ReverseKeyframesForSequence(light.Color, sequence);
                    ReverseKeyframesForSequence(light.AmbientColor, sequence);
                    ReverseKeyframesForSequence(light.AmbientIntensity, sequence);
                    ReverseKeyframesForSequence(light.AttenuationEnd, sequence);
                    ReverseKeyframesForSequence(light.AttenuationStart, sequence);
                }
                if (node is CRibbonEmitter ribbon)
                {

                    ReverseKeyframesForSequence(ribbon.Color, sequence);
                    ReverseKeyframesForSequence(ribbon.Alpha, sequence);
                    ReverseKeyframesForSequence(ribbon.Visibility, sequence);
                    ReverseKeyframesForSequence(ribbon.HeightAbove, sequence);
                    ReverseKeyframesForSequence(ribbon.HeightBelow, sequence);
                    ReverseKeyframesForSequence(ribbon.TextureSlot, sequence);
                }
            }
            // geoset animations
            foreach (var gs in CurrentModel.GeosetAnimations)
            {
                ReverseKeyframesForSequence(gs.Alpha, sequence);
                ReverseKeyframesForSequence(gs.Color, sequence);
            }
            // layers
            foreach (var material in CurrentModel.Materials)
            {
                foreach (var layer in material.Layers)
                {
                    ReverseKeyframesForSequence(layer.Alpha, sequence);
                    ReverseKeyframesForSequence(layer.TextureId, sequence);
                }
            }

            // cameras
            foreach (var cam in CurrentModel.Cameras)
            {
                ReverseKeyframesForSequence(cam.TargetTranslation, sequence);
                ReverseKeyframesForSequence(cam.Translation, sequence);
                ReverseKeyframesForSequence(cam.Rotation, sequence);
            }
            //texture animations
            foreach (var ta in CurrentModel.TextureAnimations)
            {
                ReverseKeyframesForSequence(ta.Scaling, sequence);
                ReverseKeyframesForSequence(ta.Translation, sequence);
                ReverseKeyframesForSequence(ta.Rotation, sequence);
            }

        }

        private void ReverseKeyframesForSequence(CAnimator<CVector3> animator, CSequence sequence)
        {
            if (animator.Static) { return; }
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<CVector3>> keyframes = new List<CAnimatorNode<CVector3>>();

            // First, collect all keyframes for the sequence
            foreach (var item in animator)
            {
                if (item.Time >= sequence.IntervalStart && item.Time <= sequence.IntervalEnd)
                {
                    keyframes.Add(item);
                }
            }

            // Second, reverse the data while keeping the time the same
            if (keyframes.Count > 1)
            {
                for (int i = 0; i < keyframes.Count; i++)
                {
                    int reverseIndex = keyframes.Count - 1 - i;
                    keyframes[i].Value = keyframes[reverseIndex].Value;
                    keyframes[i].OutTangent = keyframes[reverseIndex].InTangent;
                    keyframes[i].InTangent = keyframes[reverseIndex].OutTangent;
                }
            }
        }

        private void ReverseKeyframesForSequence(CAnimator<CVector4> animator, CSequence sequence)
        {
            if (animator.Static) { return; }
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<CVector4>> keyframes = new List<CAnimatorNode<CVector4>>();

            // First, collect all keyframes for the sequence
            foreach (var item in animator)
            {
                if (item.Time >= sequence.IntervalStart && item.Time <= sequence.IntervalEnd)
                {
                    keyframes.Add(item);
                }
            }

            // Second, reverse the data while keeping the time the same
            if (keyframes.Count > 1)
            {
                for (int i = 0; i < keyframes.Count; i++)
                {
                    int reverseIndex = keyframes.Count - 1 - i;
                    keyframes[i].Value = keyframes[reverseIndex].Value;
                    keyframes[i].OutTangent = keyframes[reverseIndex].InTangent;
                    keyframes[i].InTangent = keyframes[reverseIndex].OutTangent;
                }
            }
        }
        private void ReverseKeyframesForSequence(CAnimator<float> animator, CSequence sequence)
        {
            if (animator.Static) { return; }
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<float>> keyframes = new List<CAnimatorNode<float>>();

            // First, collect all keyframes for the sequence
            foreach (var item in animator)
            {
                if (item.Time >= sequence.IntervalStart && item.Time <= sequence.IntervalEnd)
                {
                    keyframes.Add(item);
                }
            }

            // Second, reverse the data while keeping the time the same
            if (keyframes.Count > 1)
            {
                for (int i = 0; i < keyframes.Count; i++)
                {
                    int reverseIndex = keyframes.Count - 1 - i;
                    keyframes[i].Value = keyframes[reverseIndex].Value;
                    keyframes[i].OutTangent = keyframes[reverseIndex].InTangent;
                    keyframes[i].InTangent = keyframes[reverseIndex].OutTangent;
                }
            }
        }
        private void ReverseKeyframesForSequence(CAnimator<int> animator, CSequence sequence)
        {
            if (animator.Static) { return; }
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<int>> keyframes = new List<CAnimatorNode<int>>();

            // First, collect all keyframes for the sequence
            foreach (var item in animator)
            {
                if (item.Time >= sequence.IntervalStart && item.Time <= sequence.IntervalEnd)
                {
                    keyframes.Add(item);
                }
            }

            // Second, reverse the data while keeping the time the same
            if (keyframes.Count > 1)
            {
                for (int i = 0; i < keyframes.Count; i++)
                {
                    int reverseIndex = keyframes.Count - 1 - i;
                    keyframes[i].Value = keyframes[reverseIndex].Value;
                    keyframes[i].OutTangent = keyframes[reverseIndex].InTangent;
                    keyframes[i].InTangent = keyframes[reverseIndex].OutTangent;
                }
            }
        }
        private CSequence GetSelectedSequence()
        {
            string s = (ListSequenes.SelectedItem as ListBoxItem).Content.ToString();
            string[] parts = s.Split('[').ToArray();
            return CurrentModel.Sequences.First(X => X.Name == parts[0].Trim());
        }
        private CSequence GetSelectedSequenceAnimator()
        {

            return CurrentModel.Sequences[List_Sequences_Animator.SelectedIndex];
        }

        private void switchLooping(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence s = GetSelectedSequence();
                s.NonLooping = !s.NonLooping;
                RefreshSequencesList();
            }
        }
        private void CopySQData(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences"); return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (CSequence s in CurrentModel.Sequences)
            {
                sb.AppendLine($"{s.IntervalStart}: {{ {s.Name}: Start }}"); ;
                sb.AppendLine($"{s.IntervalEnd}: {{ {s.Name}: End }}"); ;
            }
            Clipboard.SetText(sb.ToString());
        }
        private void ShowGaps(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences"); return;
            }
            List<Interval> intervals = new List<Interval>();
            foreach (CSequence s in CurrentModel.Sequences) { intervals.Add(new Interval(s.IntervalStart, s.IntervalEnd)); }
            string gaps = GetGaps(intervals);
            MessageBox.Show("Gaps between sequences up to 999,999:\n\n" + gaps);
        }
        private string GetGaps(List<Interval> intervals)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int limit = 999_999;
            // Sort intervals by their starting point for easier gap detection
            intervals.Sort((a, b) => a.From.CompareTo(b.From));
            // Initialize a variable to track the end of the last interval
            int lastEnd = 0;
            foreach (Interval interval in intervals)
            {
                // If there's a gap between lastEnd and the current interval's start
                if (interval.From > lastEnd + 1)
                {
                    int gapStart = lastEnd + 1;
                    int gapEnd = Math.Min(interval.From - 1, limit);
                    stringBuilder.AppendLine($"Gap: {gapStart}-{gapEnd}");
                    // Break early if the gap end exceeds the limit
                    if (gapEnd == limit)
                        break;
                }
                // Update lastEnd to the maximum of its current value or the current interval's end
                lastEnd = Math.Max(lastEnd, interval.To);
                // Stop processing if the lastEnd exceeds the limit
                if (lastEnd >= limit)
                    break;
            }
            // Check if there's a gap between the last interval and the limit
            if (lastEnd < limit)
            {
                int gapStart = lastEnd + 1;
                int gapEnd = limit;
                stringBuilder.AppendLine($"Gap: {gapStart}-{gapEnd}");
            }
            return stringBuilder.ToString();
        }
        private class Interval { public int From; public int To; public Interval(int from, int to) { From = from; To = to; } }
        private void showinfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Model name: {CurrentModel.Name}");
            sb.AppendLine($"Sequences: {CurrentModel.Sequences.Count}");
            sb.AppendLine($"Sequences total duration: {CurrentModel.Sequences.Sum(x => x.IntervalStart + x.IntervalEnd).ToString("N0")}");
            sb.AppendLine($"Global sequences: {CurrentModel.GlobalSequences.Count}");
            sb.AppendLine($"Geosets: {CurrentModel.Geosets.Count}");
            sb.AppendLine($"Geoset Animations: {CurrentModel.GeosetAnimations.Count}");
            sb.AppendLine($"Materials: {CurrentModel.Materials.Count}");
            sb.AppendLine($"Materials Layers: {CurrentModel.Materials.Sum(x => x.Layers.Count)}");
            sb.AppendLine($"Nodes: {CurrentModel.Nodes.Count}");
            sb.AppendLine($"Bones: {CurrentModel.Nodes.Count(x => x is CBone)}");
            sb.AppendLine($"Helpers: {CurrentModel.Nodes.Count(x => x is CHelper)}");
            sb.AppendLine($"Collision Shapes: {CurrentModel.Nodes.Count(x => x is CCollisionShape)}");
            sb.AppendLine($"Emitters 1: {CurrentModel.Nodes.Count(x => x is CParticleEmitter)}");
            sb.AppendLine($"Emitters 2: {CurrentModel.Nodes.Count(x => x is CParticleEmitter2)}");
            sb.AppendLine($"Attachments: {CurrentModel.Nodes.Count(x => x is CAttachment)}");
            sb.AppendLine($"Ribbons: {CurrentModel.Nodes.Count(x => x is CRibbonEmitter)}");
            sb.AppendLine($"Lights: {CurrentModel.Nodes.Count(x => x is CLight)}");
            sb.AppendLine($"Event objects: {CurrentModel.Nodes.Count(x => x is CEvent)}");
            sb.AppendLine($"Longest sequence: {getLongestSequence()}");
            sb.AppendLine($"Shortest sequence: {getShotestSequence()}");
            sb.AppendLine($"Longest global sequence: {GetLongestGS()}");
            sb.AppendLine($"Shortest global sequence: {GetShotestGS()}");
            sb.AppendLine($"Textures: {CurrentModel.Textures.Count}");
            sb.AppendLine($"Texture Animations: {CurrentModel.TextureAnimations.Count}");
            sb.AppendLine($"Cameras: {CurrentModel.Cameras.Count}");
            int[] cats = CountKeyframes();
            sb.AppendLine($"Total keyframes of nodes: {cats[0]}");
            sb.AppendLine($"Total keyframes of nodes data: {cats[1]}");
            sb.AppendLine($"Other keyframes: {cats[2]}");
            sb.AppendLine($"All keyframes: {cats[3]}");
            sb.AppendLine($"Triangles: {CountTriangles()}");
            sb.AppendLine($"Vertices: {CountVertices()}");
            LabelDisplayInfo.Text = sb.ToString();
            // MessageBox.Show(sb.ToString());
        }
        private int GetLongestGS()
        {
            int max = 0;
            foreach (CGlobalSequence gs in CurrentModel.GlobalSequences)
            {
                max = Math.Max(gs.Duration, max);
            }
            return max;
        }
        private int GetShotestGS()
        {
            int min = 0;
            foreach (CGlobalSequence gs in CurrentModel.GlobalSequences)
            {
                min = Math.Min(gs.Duration, min);
            }
            return min;
        }
        private int CountTriangles()
        {
            int count = 0;
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                count += geo.Triangles.Count;
            }
            return count;
        }
        private int CountVertices()
        {
            int count = 0;
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                count += geo.Vertices.Count;
            }
            return count;
        }
        private int[] CountKeyframes()
        {
            int nodes = 0;
            int datas = 0;
            int others = 0;
            int all = 0;
            int[] result = new int[4];
            //nodes
            foreach (INode node in CurrentModel.Nodes)
            {
                nodes += node.Translation.Count;
                nodes += node.Rotation.Count;
                nodes += node.Scaling.Count;

            }

            // node data
            foreach (INode node in CurrentModel.Nodes)
            {
                if (node is CAttachment)
                {
                    CAttachment item = node as CAttachment;
                    datas += item.Visibility.Count;
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter item = node as CParticleEmitter;
                    datas += item.Visibility.Count;
                    datas += item.EmissionRate.Count;
                    datas += item.LifeSpan.Count;
                    datas += item.InitialVelocity.Count;
                    datas += item.Gravity.Count;
                    datas += item.Longitude.Count;
                    datas += item.Latitude.Count;
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 item = node as CParticleEmitter2;
                    datas += item.Visibility.Count;
                    datas += item.Gravity.Count;
                    datas += item.Width.Count;
                    datas += item.Length.Count;
                    datas += item.Speed.Count;
                    datas += item.Variation.Count;
                    datas += item.Latitude.Count;
                    datas += item.EmissionRate.Count;
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter item = node as CRibbonEmitter;
                    datas += item.Visibility.Count;
                    datas += item.HeightAbove.Count;
                    datas += item.HeightBelow.Count;
                    datas += item.TextureSlot.Count;
                    datas += item.Alpha.Count;
                    datas += item.Color.Count;
                }
                if (node is CLight)
                {
                    CLight item = node as CLight;
                    datas += item.Visibility.Count;
                    datas += item.Color.Count;
                    datas += item.AmbientColor.Count;
                    datas += item.Intensity.Count;
                    datas += item.AmbientIntensity.Count;
                    datas += item.AttenuationStart.Count;
                    datas += item.AttenuationEnd.Count;
                }

            }

            //others

            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                others += ga.Color.Count;
                others += ga.Alpha.Count;
            }
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    others += layer.Alpha.Count;
                    others += layer.TextureId.Count;
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                others += camera.Rotation.Count;
                others += camera.Translation.Count;
                others += camera.TargetTranslation.Count;
            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                others += ta.Translation.Count;
                others += ta.Rotation.Count;
                others += ta.Scaling.Count;
            }

            result[0] = nodes;
            result[1] = datas;
            result[2] = others;
            result[3] = nodes + datas + others;
            return result;
        }
        private string getShotestSequence()
        {
            if (CurrentModel.Sequences.Count == 0) return "N/A";
            else
            {
                int low = 999999;
                string name = "";
                foreach (CSequence sequenc in CurrentModel.Sequences)
                {
                    if (sequenc.IntervalEnd - sequenc.IntervalStart < low) { name = sequenc.Name; }
                    low = Math.Min(sequenc.IntervalEnd - sequenc.IntervalStart, low);
                }
                return $"{name} - {low}";
            }
        }
        private string getLongestSequence()
        {
            if (CurrentModel.Sequences.Count == 0) return "N/A";
            else
            {
                int low = 0;
                string name = "";
                foreach (CSequence sequenc in CurrentModel.Sequences)
                {
                    if (sequenc.IntervalEnd - sequenc.IntervalStart > low) { name = sequenc.Name; }
                    low = Math.Max(sequenc.IntervalEnd - sequenc.IntervalStart, low);
                }
                return $"{name} - {low}";
            }
        }
        private void rootall(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                node.Parent.Detach();
            }
            RefreshNodesTree();
        }
        private void removeAllNodes(object sender, RoutedEventArgs e)
        {
            CurrentModel.Nodes.Clear();
        }
        private void ImportTextures(object sender, RoutedEventArgs e)
        {
            CModel TemporaryModel = GetTemporaryModel();
            if (TemporaryModel == null) { return; }
            foreach (CTexture tx in TemporaryModel.Textures)
            {
                if (CurrentModel.Textures.Any(x => x.FileName == tx.FileName) == false)
                {
                    CTexture newTexture = new CTexture(CurrentModel);
                    newTexture.FileName = tx.FileName;
                    newTexture.WrapWidth = tx.WrapWidth;
                    newTexture.WrapHeight = tx.WrapHeight;
                    newTexture.ReplaceableId = tx.ReplaceableId;
                    CurrentModel.Textures.Add(newTexture);
                }
            }
            RefreshTextures();
        }
        private void masscreeatesequences(object sender, RoutedEventArgs e)
        {
            Mass_Create_Sequences ms = new Mass_Create_Sequences(CurrentModel);
            ms.ShowDialog();
            if (ms.DialogResult == true) { RefreshSequencesList(); }
        }
        private void createTexture(object sender, RoutedEventArgs e)
        {
            Input i = new Input("");
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                string path = i.Result;
                if (CurrentModel.Textures.Any(x => x.FileName == path))
                {
                    MessageBox.Show("There is already a texture with that path"); return;
                }
                CTexture texture = new CTexture(CurrentModel);
                texture.FileName = path;
                CurrentModel.Textures.Add(texture);
                RefreshTextures();
                RefreshLayersTextureList();
            }
        }
        private void optimize(object sender, RoutedEventArgs e)
        {
            Scene_Viewport.Children.Clear();
            Optimizer.Linearize = Check_Linearize.IsChecked == true;
            Optimizer.DeleteIsolatedTriangles = check_delISolatedTriangles.IsChecked == true;
            Optimizer.DeleteIsolatedVertices = check_delISolatedVertices.IsChecked == true;
            Optimizer.Delete0LengthSequences = Check_Delete0LengthSequences.IsChecked == true;
            Optimizer.Delete0LengthGlobalSequences = Check_Delete0LengthGSequences.IsChecked == true;
            Optimizer.DeleteUnAnimatedSequences = Check_DeleteUnanimatedSequences.IsChecked == true;
            Optimizer.DeleteUnusedGlobalSequences = Check_delunusedgs.IsChecked == true;
            Optimizer.DeleteUnusedBones = Check_ConvertBones.IsChecked == true;
            Optimizer.DeleteUnusedHelpers = Check_DeleteUnusedHelpers.IsChecked == true;
            Optimizer.DeleteUnusedMAterials = Check_DeleteUnusedMaterials.IsChecked == true;
            Optimizer.DeleteUnusedTextures = Check_DeleteUnusedTextures.IsChecked == true;
            Optimizer.DeleteUnusedTextureAnimations = Check_DeleteUnusedTextureAnimations.IsChecked == true;
            Optimizer.DeleteUnusedKeyframes = Check_DeleteUnusedKeyframes.IsChecked == true;
            Optimizer.MergeGeosets = Check_MergeGeosets.IsChecked == true;
            Optimizer.DelUnusedMatrixGroups = Check_DeleteUnusedMatrixGroups.IsChecked == true;
            Optimizer.CalculateExtents = Check_CalculateExtents.IsChecked == true;
            Optimizer.MakeVisibilitiesNone = Check_VisInterp.IsChecked == true;
            Optimizer.AddMissingVisibilities = Check_AddMissingVisibilities.IsChecked == true;
            Optimizer.ClampUVs = Check_ClampUVs.IsChecked == true;
            Optimizer.ClampLights = Check_ClampLights.IsChecked == true;
            Optimizer.DeleteDuplicateGAs = Check_DeleteDuplciateGAs.IsChecked == true;
            Optimizer.AddMissingGAs = Check_AddMissingGeosetAnims.IsChecked == true;
            Optimizer.SetAllStaticGAS = Check_SetAllStaticGAs.IsChecked == true;
            Optimizer.ClampKeyframes = Check_ClampKeyframes.IsChecked == true;
            Optimizer.DeleteIdenticalAdjascentKEyframes = Check_DeleteIdenticalAdjascentKEyframes.IsChecked == true;
            Optimizer.Check_DeleteIdenticalAdjascentKEyframes_times = Check_DeleteIdenticalAdjascentKEyframes_times.IsChecked == true;
            Optimizer.DeleteSimilarSimilarKEyframes = Check_DeleteSimilarSimilarKEyframes.IsChecked == true;
           // Optimizer.ReducematrixGruops = Check_ReducematrixGruops.IsChecked == true;
            // missing opening/closing keyframes
            if (MissingKeyframesCheck1.IsChecked == true)
            {
                Optimizer.StretchKeyframes = true;
            }
            else if (MissingKeyframesCheck2.IsChecked == true)
            {
                Optimizer.OtherMissingKeyframes = MissingKeyframesCheck2.IsChecked == true;
                Optimizer.AddDefaultMissingOpeningKeyframes = Check_AddDefaultMissingOpeningKeyframes.IsChecked == true;
                Optimizer.AddDefaultMissingClosingKeyframes = Check_AddDefaultMissingClosingKeyframes.IsChecked == true;

                Optimizer.MoveFirstKeyframeToStart = Check_MoveFirstKeyferameToStart.IsChecked == true;
                Optimizer.MoveLastKeyframeToEnd = Check_MoveLastKeyframeToEnd.IsChecked == true;

                Optimizer.DuplicateFirstKEyframeToStart = DuplicateFirstKeyframeToStart.IsChecked == true;
                Optimizer.DuplicateLastKeyframeToEnd = Check_DuplicateLastKeyframeToEnd.IsChecked == true;
            }


            Optimizer._DetachFromNonBone = Check_DetachFromNonBone.IsChecked == true;
            Optimizer.DleteOverlapping1 = check_delOverlapping1.IsChecked == true;
            Optimizer.DleteOverlapping2 = check_delOverlapping2.IsChecked == true;
            Optimizer.ClampNormals = Check_ClampNormals.IsChecked == true;
            Optimizer.DeleteTrianglesWithNoArea = check_noArea.IsChecked == true;
            Optimizer.MergeIdenticalVertices = check_mergevertices.IsChecked == true;
            Optimizer.DeleteDuplicateGEosets = Check_DeleteDuplciateGeosets.IsChecked == true;
            Optimizer.MergeTextures = check_mergeTextures.IsChecked == true;
            Optimizer.MergeMAtertials = check_mergeMaterials.IsChecked == true;
            Optimizer.MergeTAs = check_mergeTAs.IsChecked == true;
            Optimizer.MergeLayers = check_mergeLayers.IsChecked == true;
            Optimizer.MinimizeMatrixGroups = Check_Minimiematrixgroups.IsChecked == true;
            Optimizer.FixNoMatrixGroups = Check_FixNoMatrixGroups.IsChecked == true;
            Optimizer.Optimize(CurrentModel);
            CollectTextures();
            RefreshAll();
            MessageBox.Show("Done optimizing");
        }
        private void checkallopts(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ListOptOptions.Children.Count; i++)
            {
                if (ListOptOptions.Children[i] is CheckBox)
                {
                    CheckBox c = ListOptOptions.Children[i] as CheckBox;
                    if (c.IsEnabled) { c.IsChecked = true; }
                }
            }
        }
        private void uncheckallopts(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ListOptOptions.Children.Count; i++)
            {
                if (ListOptOptions.Children[i] is CheckBox)
                {
                    CheckBox c = ListOptOptions.Children[i] as CheckBox;
                    if (c.IsEnabled) { c.IsChecked = false; }
                }
            }
        }
        private void inversecheckopts(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ListOptOptions.Children.Count; i++)
            {
                if (ListOptOptions.Children[i] is CheckBox)
                {
                    CheckBox c = ListOptOptions.Children[i] as CheckBox;
                    if (c.IsEnabled) { c.IsChecked = c.IsChecked == true ? false : true; ; }
                }
            }
        }
        private void reload(object sender, RoutedEventArgs e)
        {
            if (CurrentSaveLocaiton.Length != 0)
            {
                LoadModel(CurrentSaveLocaiton);
            }
        }
        private void showunanimatedseq(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences"); return;
            }
            Optimizer.Model = CurrentModel;
            List<string> list = new List<string>();
            foreach (CSequence sequence in CurrentModel.Sequences.ToList())
            {
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;
                if (Optimizer.IntervalAnimated(from, to) == false)
                {
                    list.Add(sequence.Name);
                }
            }
            if (list.Count > 0)
            {
                MessageBox.Show($"these sequences are not animated:\n\n" + string.Join("\n", list.ToArray()));
            }
            else
            {
                MessageBox.Show("No un-animated sequences");
            }
        }
        private void DelAllGeosets(object sender, RoutedEventArgs e)
        {
            CurrentModel.Geosets.Clear();
            RefreshGeosetsList();
            RefreshViewPort();
        }
        private void clearAllnodetrans(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                node.Translation.Clear();
                node.Scaling.Clear();
                node.Rotation.Clear();
            }
        }
        private void clearnodeTrans(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            select_Transformations st = new select_Transformations();
            st.ShowDialog();
            if (st.DialogResult == true)
            {
                if (st.Check_T.IsChecked == true) node.Translation.Clear();
                if (st.Check_R.IsChecked == true) node.Rotation.Clear();
                if (st.Check_S.IsChecked == true) node.Scaling.Clear();
            }
        }
        private void reversenodetr(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            select_Transformations st = new select_Transformations();
            st.ShowDialog();
            if (st.DialogResult == true)
            {
                if (st.Check_T.IsChecked == true) node.Translation.Reverse();
                if (st.Check_R.IsChecked == true) node.Rotation.Reverse();
                if (st.Check_S.IsChecked == true) node.Scaling.Reverse();
            }
        }
        private void loaded(object sender, RoutedEventArgs e)
        {
            //  List<int> list = new List<int>() { 1,2,4,3};
            //   MessageBox.Show(string.Join(" ", list.Select(x=>x.ToString()) .ToArray()));
            //   list.Sort();
            //   MessageBox.Show(string.Join(" ", list.Select(x=>x.ToString()) .ToArray()));
        }
        private void SpaceTras(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            select_Transformations st = new select_Transformations();
            st.ShowDialog();
            if (st.DialogResult == true)
            {
                if (st.Check_T.IsChecked == true)
                {
                    foreach (CSequence sequence in CurrentModel.Sequences)
                    {
                        SpaceTransformation(sequence, node.Translation);
                    }
                }
                if (st.Check_R.IsChecked == true)
                {
                    foreach (CSequence sequence in CurrentModel.Sequences)
                    {
                        SpaceTransformation(sequence, node.Rotation);
                    }
                }
                if (st.Check_S.IsChecked == true)
                {
                    foreach (CSequence sequence in CurrentModel.Sequences)
                    {
                        SpaceTransformation(sequence, node.Scaling);
                    }
                }
            }
        }
        private void SpaceTransformation(CSequence sequence, CAnimator<CVector3> animator)
        {
            int from = sequence.IntervalStart;
            int to = sequence.IntervalEnd;
            List<CAnimatorNode<CVector3>> isolated = animator.Where(x => x.Time >= from && x.Time <= to).ToList();
            if (isolated.Count <= 1)
            {
                return; // Cannot space less than 2 keyframes
            }
            if (isolated.Count > to - from)
            {
                MessageBox.Show($"Sequence {sequence.Name}: Translation: Keyframes are more than the interval");
                return;
            }
            // Calculate the interval for spacing
            int totalDuration = to - from;
            int spacing = totalDuration / (isolated.Count - 1);
            // Adjust the keyframes to be equally spaced
            for (int i = 0; i < isolated.Count; i++)
            {
                int newTime = from + i * spacing;
                CVector3 value = isolated[i].Value; // Retain the original value
                isolated[i] = new CAnimatorNode<CVector3>(newTime, value);
            }
        }
        private void SpaceTransformation(CSequence sequence, CAnimator<CVector4> animator)
        {
            int from = sequence.IntervalStart;
            int to = sequence.IntervalEnd;
            List<CAnimatorNode<CVector4>> isolated = animator.Where(x => x.Time >= from && x.Time <= to).ToList();
            if (isolated.Count <= 1)
            {
                return; // Cannot space less than 2 keyframes
            }
            if (isolated.Count > to - from)
            {
                MessageBox.Show($"Sequence {sequence.Name}: Translation: Keyframes are more than the interval");
                return;
            }
            // Calculate the interval for spacing
            int totalDuration = to - from;
            int spacing = totalDuration / (isolated.Count - 1);
            // Adjust the keyframes to be equally spaced
            for (int i = 0; i < isolated.Count; i++)
            {
                int newTime = from + i * spacing;
                CVector4 value = isolated[i].Value; // Retain the original value
                isolated[i] = new CAnimatorNode<CVector4>(newTime, value);
            }
        }
        private void DelNode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            if (NodeHasAttachedVertices(node))
            {
                MessageBox.Show("There are vertices attached to this node. Re-attach them to another first."); return;
            }
            int id = node.NodeId;
            CurrentModel.Nodes.Remove(node);
            foreach (INode nod in CurrentModel.Nodes.ToList())
            {
                if (nod.Parent == null) { CurrentModel.Nodes.Remove(nod); continue; }
                if (nod.Parent.NodeId == id) { CurrentModel.Nodes.Remove(nod); continue; }
            }
            RefreshNodesTree();
        }

        private bool NodeHasAttachedVertices(INode node)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    if (vertex.Group != null)
                    {
                        if (vertex.Group.Object != null)
                        {
                            foreach (var gnode in vertex.Group.Object.Nodes)
                            {
                                if (gnode.Node.Node == node) { return true; }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void CreateAtts1(object sender, RoutedEventArgs e)
        {
            List<string> names = new List<string>()
            {
                "Sprite First Ref", "Sprite Second Ref", "Sprite Third Ref", "Sprite Fourth Ref", "Sprite RallyPoint Ref", "Mount Ref"
            };
            foreach (string name in names)
            {
                if (CurrentModel.Nodes.Any(x => x.Name.ToLower() == name.ToLower()) == false)
                {
                    INode node = new CAttachment(CurrentModel);
                    node.Name = name;
                    CurrentModel.Nodes.Add(node);
                }
            }
            RefreshNodesTree();
        }
        private void Createatts2(object sender, RoutedEventArgs e)
        {
            List<string> names = new List<string>()
            {
                "Head Ref", "Chest Ref", "Hand Left Ref", "Hand Right Ref", "Foot Left Ref", "Foot Right Ref", "Overhead Ref",
                "Weapon Right Ref", "Weapon Left Ref"
            };
            foreach (string name in names)
            {
                if (CurrentModel.Nodes.Any(x => x.Name.ToLower() == name.ToLower()) == false)
                {
                    INode node = new CAttachment(CurrentModel);
                    node.Name = name;
                    CurrentModel.Nodes.Add(node);
                }
            }
            RefreshNodesTree();
        }
        private void movetoroot(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            node.Parent.Detach();
            RefreshNodesTree();
        }
        public void ClearAnimations(bool andSequence)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence sequence = GetSelectedSequence();
            int from = sequence.IntervalStart;
            int to = sequence.IntervalEnd;
            if (andSequence)
            {
                ListSequenes.Items.Remove(ListSequenes.SelectedItem);
                CurrentModel.Sequences.Remove(sequence);
            }
            foreach (INode node in CurrentModel.Nodes)
            {
                foreach (var item in node.Translation.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { node.Translation.Remove(item); }
                }
                foreach (var item in node.Scaling.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { node.Scaling.Remove(item); }
                }
                foreach (var item in node.Rotation.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { node.Rotation.Remove(item); }
                }
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    foreach (var item in element.Visibility.ToList())
                    {
                        if (item.Time >= from && item.Time <= to) { element.Visibility.Remove(item); }
                    }
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                }
            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                foreach (var item in ga.Alpha.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { ga.Alpha.Remove(item); }
                }
                foreach (var item in ga.Color.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { ga.Color.Remove(item); }
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                foreach (var item in camera.Rotation.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { camera.Rotation.Remove(item); }
                }
            }
            foreach (CMaterial material in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    foreach (var item in layer.Alpha.ToList())
                    {
                        if (item.Time >= from && item.Time <= to) { layer.Alpha.Remove(item); }
                    }
                    foreach (var item in layer.TextureId.ToList())
                    {
                        if (item.Time >= from && item.Time <= to) { layer.TextureId.Remove(item); }
                    }
                }
            }
            foreach (CTextureAnimation animation in CurrentModel.TextureAnimations)
            {
                foreach (var item in animation.Translation.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { animation.Translation.Remove(item); }
                }
                foreach (var item in animation.Rotation.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { animation.Rotation.Remove(item); }
                }
                foreach (var item in animation.Scaling.ToList())
                {
                    if (item.Time >= from && item.Time <= to) { animation.Scaling.Remove(item); }
                }
            }
        }
        private void delseqalong(object sender, RoutedEventArgs e)
        {
            ClearAnimations(true);
        }
        private void ClearSequenceAnimations(object sender, RoutedEventArgs e)
        {
            ClearAnimations(false);
        }
        private void createTextures(object sender, RoutedEventArgs e)
        {
            Mass_Create_Textures mass_Create_Textures = new Mass_Create_Textures();
            mass_Create_Textures.ShowDialog();
            if (mass_Create_Textures.DialogResult == true)
            {
                foreach (string path in mass_Create_Textures.Paths)
                {
                    CTexture tex = new CTexture(CurrentModel);
                    tex.ReplaceableId = 0;
                    tex.FileName = path;
                    CurrentModel.Textures.Add(tex);
                    if (mass_Create_Textures.Check_also.IsChecked == true)
                    {
                        CMaterial mat = new CMaterial(CurrentModel);
                        CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                        layer.Texture.Attach(tex);
                        mat.Layers.Add(layer);
                        CurrentModel.Materials.Add(mat);
                    }
                }
                RefreshTextures();
                RefreshLayersTextureList();
                CollectTextures();
            }
        }
        private void delTexture(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                if (texture.HasReferences)
                {
                    if (askDelete())
                    {
                        List_Textures.Items.Remove(List_Textures.SelectedItem);
                        foreach (CMaterial material in CurrentModel.Materials)
                        {
                            foreach (CMaterialLayer layer in material.Layers)
                            {
                                if (layer.Texture.Object == texture) { layer.Texture.Detach(); }
                            }
                        }
                        CurrentModel.Textures.Remove(texture);
                    }
                }
                else
                {
                    List_Textures.Items.Remove(List_Textures.SelectedItem);
                    foreach (CMaterial material in CurrentModel.Materials)
                    {
                        foreach (CMaterialLayer layer in material.Layers)
                        {
                            if (layer.Texture.Object == texture) { layer.Texture.Detach(); }
                        }
                    }
                    CurrentModel.Textures.Remove(texture);
                }
            }
        }
        private CTexture GetSElectedTexture()
        {
            return CurrentModel.Textures[List_Textures.SelectedIndex];
        }
        private void delgeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            List<CGeoset> geosets = GetSelectedGeosets();
            foreach (CGeoset geoset in geosets)
            {
                if (CurrentModel.GeosetAnimations.Any(x => x.Geoset.Object == geoset))
                {
                    CGeosetAnimation ga = CurrentModel.GeosetAnimations.First(x => x.Geoset.Object == geoset);
                    CurrentModel.GeosetAnimations.Remove(ga);
                }
                CurrentModel.Geosets.Remove(geoset);
            }
            RefreshGeosetsList();
            RefreshGeosetAnimationsList();
            RefreshViewPort();

        }
        private void creatematerialfortargettexture(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem == null) return;
            CTexture texture = GetSElectedTexture();
            CMaterial material = new CMaterial(CurrentModel);
            CMaterialLayer layer = new CMaterialLayer(CurrentModel);
            layer.Texture.Attach(texture);
            material.Layers.Add(layer);
            CurrentModel.Materials.Add(material);
            RefreshMaterialsList();
            CollectTextures();
        }
        private CMaterial GetSelectedMAterial()
        {
            return CurrentModel.Materials[List_MAterials.SelectedIndex];
        }
        public void RefreshMaterialsList()
        {
            List_MAterials.Items.Clear();
            foreach (CMaterial material in CurrentModel.Materials)
            {
                List_MAterials.Items.Add(new ListBoxItem() { Content = material.ObjectId.ToString() + $" ({material.Layers.Count} Layers)" });
            }
            Label_Materials.Text = $"Materials - {CurrentModel.Materials.Count}";
        }
        private void RefreshTextureAnims()
        {
            List_TextureAnims.Items.Clear();
            LabelTextueAnims.Text = $"Texture Animations {CurrentModel.TextureAnimations.Count}";
            foreach (var item in CurrentModel.TextureAnimations)
            {
                List_TextureAnims.Items.Add(new ListBoxItem() { Content = item.ObjectId });
            }
        }
        private void DelMAterial(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                if (material.HasReferences)
                {
                    if (askDelete())
                    {
                        CurrentModel.Materials.Remove(material);
                        RefreshMaterialsList();
                    }
                }
                else
                {
                    CurrentModel.Materials.Remove(material);
                    RefreshMaterialsList();
                }
            }
        }
        private void creatematsforalltextures(object sender, RoutedEventArgs e)
        {
            foreach (CTexture texture in CurrentModel.Textures)
            {
                CMaterial material = new CMaterial(CurrentModel);
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                material.Layers.Add(layer);
                layer.Texture.Attach(texture);
                CurrentModel.Materials.Add(material);
            }
            RefreshMaterialsList();
            CollectTextures();
        }
        private INode GetNode(string name)
        {
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void movetargetnodeundernode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
                nodes.Remove(node.Name);
                if (nodes.Count > 0)
                {
                    Selector s = new Selector(nodes);
                    s.ShowDialog();
                    if (s.DialogResult == true)
                    {
                        INode selected = GetNode(s.Selected);

                        if (node.Parent.Node == selected)
                        {
                            MessageBox.Show("The node is already under the selected node"); return;

                        }
                        if (selected.Parent.Node == node)
                        {
                            MessageBox.Show("Cannot place the parent node under its child node!"); return;
                        }
                        node.Parent.Attach(selected);
                        RefreshNodesTree();
                    }
                }
                else { MessageBox.Show("At least one other node must be present"); return; }
            }
        }
        private void SelectAllGeosets(object sender, RoutedEventArgs e)
        {
            Pause = true;
            foreach (var item in ListGeosets.Items)
            {
                ListGeosets.SelectedItems.Add(item);
            }
            Pause = false;
            RefreshViewPort();
        }
        private void DeselectAllGeosets(object sender, RoutedEventArgs e)
        {
            ListGeosets.SelectedItems.Clear();
        }
        private void InvertSelectGeosets(object sender, RoutedEventArgs e)
        {
            List<object> unselected = new List<object>();
            foreach (var item0 in ListGeosets.Items)
            {
                if (ListGeosets.SelectedItems.Contains(item0) == false)
                {
                    unselected.Add(item0);
                }
            }
            ListGeosets.SelectedItems.Clear();
            foreach (object item in unselected)
            {
                ListGeosets.SelectedItems.Add(item);
            }
        }
        private void clearRecents(object sender, RoutedEventArgs e)
        {
            Recents.Clear();
            SaveRecents();
        }
        private void CreateNewNode(object sender, RoutedEventArgs e)
        {
        }
        private void MergeSelectedSequences(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequence();
                List<string> sequences = CurrentModel.Sequences.Select(X => X.Name).ToList();
                sequences.Remove(sequence.Name);
                Selector s = new Selector(sequences);
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    CSequence selected = CurrentModel.Sequences.First(x => x.Name == s.Selected);
                    if (sequence.IntervalEnd != selected.IntervalStart + 1)
                    {
                        MessageBox.Show("These sequences are not back to back"); return;
                    }
                    sequence.IntervalEnd = selected.IntervalEnd;
                    CurrentModel.Sequences.Remove(selected);
                    RefreshSequencesList();
                }
            }
        }
        private void SplitSequences(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequence();
                List<string> sequences = CurrentModel.Sequences.Select(X => X.Name).ToList();
                sequences.Remove(sequence.Name);
                Selector s = new Selector(sequences);
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    CSequence selected = CurrentModel.Sequences.First(x => x.Name == s.Selected);
                    Input i = new Input("", "New name");
                    string newName = "";
                    i.ShowDialog();
                    if (i.DialogResult == true)
                    {
                        if (CurrentModel.Sequences.Any(x => x.Name.ToLower() == i.Result))
                        {
                            MessageBox.Show("There is already a sequence with this name"); return;
                        }
                        newName = i.Result;
                        int final = 0;
                        Input finalInput = new Input("", "Split at");
                        finalInput.ShowDialog();
                        if (finalInput.DialogResult == true)
                        {
                            bool parsed = int.TryParse(finalInput.Result, out final);
                            if (parsed)
                            {
                                if (final > sequence.IntervalStart && final < sequence.IntervalEnd)
                                {
                                    int temp = sequence.IntervalEnd;
                                    sequence.IntervalEnd = final;
                                    CSequence newsequence = new CSequence(CurrentModel);
                                    newsequence.IntervalStart = final + 1;
                                    newsequence.IntervalEnd = temp;
                                    newsequence.Name = newName;
                                    CurrentModel.Sequences.Add(newsequence);
                                    RefreshSequencesList();
                                }
                                else
                                {
                                    MessageBox.Show("Value not between start and end of the sequence"); return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("not an integer"); return;
                            }
                        }
                    }
                    sequence.IntervalEnd = selected.IntervalEnd;
                    CurrentModel.Sequences.Remove(selected);
                    RefreshSequencesList();
                }
            }
        }
        private void SpaceKeyframes(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequence();
                select_Transformations st = new select_Transformations();
                st.ShowDialog();
                if (st.DialogResult == true)
                {
                    if (st.Check_T.IsChecked == true)
                    {
                    }
                    if (st.Check_R.IsChecked == true)
                    {
                    }
                    if (st.Check_S.IsChecked == true)
                    {
                    }
                }
            }
        }
        private void ClearAllAnimationsOfSequence(object sender, RoutedEventArgs e)
        {
            //unfinished
        }
        private void clearsequencetranslations(object sender, RoutedEventArgs e)
        { //unfinished
        }
        private void clearsequencerotations(object sender, RoutedEventArgs e)
        { //unfinished
        }
        private void clearsequencescalings(object sender, RoutedEventArgs e)
        { //unfinished
        }
        private List<CGeoset> GetSelectedGeosets()
        {
            List<CGeoset> list = new List<CGeoset>();
            foreach (var item in ListGeosets.SelectedItems)
            {
                int index = ListGeosets.Items.IndexOf(item);
                list.Add(CurrentModel.Geosets[index]);
            }
            return list;
        }
        private void createColsForTargetGeo(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    CVector3 centroid = new CVector3();
                    Optimizer.CalculateGeosetExtent(geoset);
                    CCollisionShape node = new CCollisionShape(CurrentModel);
                    node.Name = $"CollisionShape_{IDCounter.Next()}";
                    node.Type = ECollisionShapeType.Box;
                    node.Vertex1 = new CVector3(geoset.Extent.Min.X, geoset.Extent.Min.Y, geoset.Extent.Min.Z);
                    node.Vertex2 = new CVector3(geoset.Extent.Max.X, geoset.Extent.Max.Y, geoset.Extent.Max.Z);
                    node.PivotPoint = centroid;
                    CurrentModel.Nodes.Add(node);
                }
                RefreshNodesTree();
                RefreshViewPort();
            }
        }
        private void putonground_click(object sender, RoutedEventArgs e)
        {
            putonground pg = new putonground();
            pg.ShowDialog();
            List<CGeoset> geosets = GetSelectedGeosets();
            if (pg.DialogResult == true)
            {
                if (pg.Check_1.IsChecked == true)
                {
                    foreach (CGeoset geoset in geosets)
                    {
                        Calculator.PutOnGround(geoset);
                    }
                }
                if (pg.Check_2.IsChecked == true)
                {
                    Calculator.PutOnGroundTogether(geosets);
                }
                if (pg.Check_3.IsChecked == true)
                {
                    foreach (CGeoset geoset in CurrentModel.Geosets)
                    {
                        Calculator.PutOnGround(geoset);
                    }
                }
                if (pg.Check_4.IsChecked == true)
                {
                    List<CGeoset> all = new List<CGeoset>();
                    foreach (CGeoset geo in CurrentModel.Geosets)
                    {
                        all.Add(geo);
                    }
                    Calculator.PutOnGroundTogether(all);
                }
            }
            RefreshViewPort();
        }
        private void MergeGeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 1)
            {
                Scene_Viewport.Children.Clear();
                Pause = true;
                List<CGeoset> geosets = GetSelectedGeosets();
                if (geosets.Count <= 1) { return; }
                CGeoset First = geosets[0];
                for (int i = 1; i < geosets.Count; i++)
                {
                    Calculator.TransferGeosetData(First, geosets[i], CurrentModel);
                }
                // delete  
                for (int i = 1; i < geosets.Count; i++)
                {
                    DeleteGeosetAnimationOf(geosets[i]);
                    CurrentModel.Geosets.Remove(geosets[i]);
                }
                Pause = false;
                RefreshGeosetsList();
                RefreshGeosetAnimationsList();
                RefreshViewPort();
            }
            else
            {
                MessageBox.Show("Select at least 2 geosets");
            }
        }
        private void negate(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    bool onX = ax.Check_x.IsChecked == true;
                    bool onY = ax.Check_y.IsChecked == true;
                    bool onZ = ax.Check_z.IsChecked == true;
                    foreach (CGeoset geoset in geosets)
                    {
                        foreach (CGeosetVertex vertex in geoset.Vertices)
                        {
                            float x = onX ? -vertex.Position.X : vertex.Position.X;
                            float y = onY ? -vertex.Position.Y : vertex.Position.Y;
                            float z = onZ ? -vertex.Position.Z : vertex.Position.Z;
                            vertex.Position = new CVector3(x, y, z);
                        }
                    }
                }
            }
            RefreshViewPort();
        }
        private void TranslateGeoserts(object sender, RoutedEventArgs e)
        {
            InputVector iv = new InputVector(AllowedValue.Both);
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X; float y = iv.Y; float z = iv.Z;
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        float x_ = vertex.Position.X + x; ;
                        float y_ = vertex.Position.Y + y;
                        float z_ = vertex.Position.Z + z;
                        vertex.Position = new CVector3(x_, y_, z_);
                    }
                }
            }
            RefreshViewPort();
        }
        private void scalegeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            InputVector iv = new InputVector(AllowedValue.Positive, new CVector3(100, 100, 100), "Percentage");
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X; float y = iv.Y; float z = iv.Z;
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        float x_ = vertex.Position.X * x / 100; ;
                        float y_ = vertex.Position.Y * y / 100;
                        float z_ = vertex.Position.Z * z / 100;
                        vertex.Position = new CVector3(x_, y_, z_);
                    }
                }
            }
            RefreshViewPort();
        }
        private void Centergeosets(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSelectedGeosets();
            if (geosets.Count == 0) { return; }
            axis_selector ax = new axis_selector();
            ax.ShowDialog();
            if (ax.DialogResult == true)
            {
                bool onX = ax.Check_x.IsChecked == true;
                bool onY = ax.Check_y.IsChecked == true;
                bool onZ = ax.Check_z.IsChecked == true;
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.CenterGeoset(geoset, onX, onZ, onZ);
                }
            }
            RefreshViewPort();
        }
        private void scaleToFit(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) { return; }
            SCALETOFIT sf = new SCALETOFIT();
            sf.ShowDialog();
            if (sf.DialogResult == true)
            {
                CExtent extent = sf.Extent;
                bool each = sf.Each;
                List<CGeoset> geosets = GetSelectedGeosets();
                if (each)
                {
                    foreach (CGeoset geoset in geosets)
                    {
                        Calculator.ScaleToFitInExtent(extent, geoset);
                    }
                }
                else
                {
                    Calculator.ScaleToFitInExtent(extent, geosets);
                }
            }
            RefreshViewPort();
        }
        private void RotateGeosets(object sender, RoutedEventArgs e)
        {
            InputVector iv = new InputVector(AllowedValue.Both, new CVector3(0, 0, 0), "Rotation (-360-360)");
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X; float y = iv.Y; float z = iv.Z;
                if (x > 360 || x < -360) { MessageBox.Show("A values for rotation must be between -360 and 360"); return; }
                if (y > 360 || y < -360) { MessageBox.Show("A values for rotation must be between -360 and 360"); return; }
                if (z > 360 || z < -360) { MessageBox.Show("A values for rotation must be between -360 and 360"); return; }
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.RotateGeoset(geoset, x, y, z);
                }
            }
            RefreshViewPort();
        }
        private void aligngeosets(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSelectedGeosets();
            if (geosets.Count < 2) { MessageBox.Show("Select 2 or more geosets"); return; }
            axis_selector ax = new axis_selector();
            ax.ShowDialog();
            if (ax.DialogResult == true)
            {
                bool onX = ax.Check_x.IsChecked == true;
                bool onY = ax.Check_y.IsChecked == true;
                bool onZ = ax.Check_z.IsChecked == true;
                Calculator.AlignGeosets(geosets, onX, onY, onZ);
            }
            RefreshViewPort();
        }
        private CGeoset DuplicateGeogeset(CGeoset inputGeoset, CModel whichModel)
        {
            CGeoset _newGeoset = new CGeoset(whichModel);
            CBone bone = new CBone(CurrentModel);
            bone.Name = $"ImportedGeosetBone_{IDCounter.Next()}";
            CGeosetGroup group = new CGeosetGroup(whichModel);
            CGeosetGroupNode gnode = new CGeosetGroupNode(whichModel);
            gnode.Node.Attach(bone);
            group.Nodes.Add(gnode);
            _newGeoset.Groups.Add(group);
            whichModel.Nodes.Add(bone);
            Dictionary<CGeosetVertex, CGeosetVertex> VertexReference = new Dictionary<CGeosetVertex, CGeosetVertex>();
            foreach (CGeosetTriangle face in inputGeoset.Triangles)
            {
                CGeosetTriangle _newFace = new CGeosetTriangle(whichModel);
                // Vertex 1
                if (VertexReference.ContainsKey(face.Vertex1.Object))
                {
                    _newFace.Vertex1.Attach(VertexReference[face.Vertex1.Object]);
                }
                else
                {
                    CGeosetVertex _newVertex = new CGeosetVertex(whichModel);
                    _newVertex.Position = new CVector3(face.Vertex1.Object.Position);
                    _newVertex.TexturePosition = new CVector2(face.Vertex1.Object.TexturePosition);
                    _newVertex.Normal = new CVector3(face.Vertex1.Object.Normal);
                    _newVertex.Group.Attach(group);
                    _newFace.Vertex1.Attach(_newVertex);  // Attach to Vertex1 slot
                    VertexReference.Add(face.Vertex1.Object, _newVertex);
                    _newGeoset.Vertices.Add(_newVertex);
                }
                // Vertex 2
                if (VertexReference.ContainsKey(face.Vertex2.Object))
                {
                    _newFace.Vertex2.Attach(VertexReference[face.Vertex2.Object]);
                }
                else
                {
                    CGeosetVertex _newVertex = new CGeosetVertex(whichModel);
                    _newVertex.Position = new CVector3(face.Vertex2.Object.Position);
                    _newVertex.TexturePosition = new CVector2(face.Vertex2.Object.TexturePosition);
                    _newVertex.Normal = new CVector3(face.Vertex2.Object.Normal);
                    _newVertex.Group.Attach(group);
                    _newFace.Vertex2.Attach(_newVertex);  // Attach to Vertex2 slot
                    VertexReference.Add(face.Vertex2.Object, _newVertex);
                    _newGeoset.Vertices.Add(_newVertex);
                }
                // Vertex 3
                if (VertexReference.ContainsKey(face.Vertex3.Object))
                {
                    _newFace.Vertex3.Attach(VertexReference[face.Vertex3.Object]);
                }
                else
                {
                    CGeosetVertex _newVertex = new CGeosetVertex(whichModel);
                    _newVertex.Position = new CVector3(face.Vertex3.Object.Position);
                    _newVertex.TexturePosition = new CVector2(face.Vertex3.Object.TexturePosition);
                    _newVertex.Normal = new CVector3(face.Vertex3.Object.Normal);
                    _newVertex.Group.Attach(group);
                    _newFace.Vertex3.Attach(_newVertex);  // Attach to Vertex3 slot
                    VertexReference.Add(face.Vertex3.Object, _newVertex);
                    _newGeoset.Vertices.Add(_newVertex);
                }
                // Add the face to the new geoset
                _newGeoset.Triangles.Add(_newFace);
            }
            // Attach the material to the new geoset
            _newGeoset.Material.Attach(whichModel.Materials[0]);
            return _newGeoset;
        }
        private void reattachToBone(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSelectedGeosets();
            if (geosets.Count < 1) { MessageBox.Show("Select 1 or more geosets"); return; }
            List<string> bones = CurrentModel.Bones.Select(x => x.Name).ToList();
            if (bones.Count > 0)
            {
                Selector s = new Selector(bones);
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    INode node = CurrentModel.Nodes.First(x => x.Name == s.Selected);
                    foreach (CGeoset geoset in geosets)
                    {
                        geoset.Groups.Clear();
                        CGeosetGroup group = new CGeosetGroup(CurrentModel);
                        CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                        gnode.Node.Attach(node);
                        group.Nodes.Add(gnode);
                        geoset.Groups.Add(group);
                        foreach (CGeosetVertex vertex in geoset.Vertices)
                        {
                            vertex.Group.Detach();
                            vertex.Group.Attach(group);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("There are no bones"); return;
            }
        }
        private void PullTogether(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSelectedGeosets();
            if (geosets.Count != 2) { MessageBox.Show("Select 2  geosets"); return; }
            axis_selector ax = new axis_selector();
            ax.ShowDialog();
            if (ax.DialogResult == true)
            {
                bool onX = ax.Check_x.IsChecked == true;
                bool onY = ax.Check_y.IsChecked == true;
                bool onZ = ax.Check_z.IsChecked == true;
                if (onX && onY && onZ) { MessageBox.Show("Only one axes!"); return; }
                if (!onX && !onY && !onZ) { return; }
                Calculator.PullTogether(geosets, onX, onY, onZ);
            }
            RefreshViewPort();
        }
        private void renamesequence(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence seq = GetSelectedSequence();
                Input i = new Input(seq.Name);
                i.ShowDialog();
                if (i.DialogResult == true)
                {
                    string n = i.Result;
                    if (CurrentModel.Sequences.Any(x => x.Name.ToLower() == n.ToLower()))
                    {
                        MessageBox.Show("There is already a sequence with this name");
                        return;
                    }
                    seq.Name = CapitalizeWords(n);
                    RefreshSequencesList();
                }
            }
        }
        public static string CapitalizeWords(string input)
        {
            if (input.Length == 0) return input;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }
        private bool TrackExistsInSEquences(int track, CSequence exclude)
        {
            foreach (CSequence seq in CurrentModel.Sequences)
            {
                if (seq == exclude) continue;
                if (track >= seq.IntervalStart && track <= seq.IntervalEnd) { return true; }
            }
            return false;
        }
        private void ResizeTargetSequenceKeyframes(int from, int oldTo, int newTo, CAnimator<CVector3> animator)
        {
            if (animator.Count < 2) { return; }
            // Isolate the keyframes within the old range
            List<CAnimatorNode<CVector3>> isolated = new List<CAnimatorNode<CVector3>>();
            foreach (var item in animator)
            {
                if (item.Time >= from && item.Time <= oldTo)
                {
                    isolated.Add(item);
                }
            }
            // Validate isolated keyframes
            if (isolated.Count < 2)
            {
                return; // Not enough keyframes to resize
            }
            if (isolated.Count > newTo - from)
            {
                MessageBox.Show("More keyframes than interval. Cannot resize.");
                return;
            }
            // Perform the resizing
            for (int i = 0; i < isolated.Count; i++)
            {
                var keyframe = isolated[i];
                // Calculate new time using linear interpolation
                int newTime = from + (int)((float)(keyframe.Time - from) / (oldTo - from) * (newTo - from));
                // Create a new keyframe with the updated time and original value
                var tempValue = keyframe.Value;
                isolated[i] = new CAnimatorNode<CVector3>(newTime, tempValue);
            }
            // Optionally: Sort the isolated list if needed
            isolated.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
        private void ResizeTargetSequenceKeyframes(int from, int oldTo, int newTo, CAnimator<int> animator)
        {
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<int>> isolated = new List<CAnimatorNode<int>> { };
            foreach (var item in animator) { if (item.Time >= from && item.Time <= oldTo) { isolated.Add(item); } }
            if (isolated.Count < 2) { return; }
            if (isolated.Count > newTo - from) { MessageBox.Show("More keyframes than interval. Cannot resize."); return; }
            // Perform the resizing
            for (int i = 0; i < isolated.Count; i++)
            {
                var keyframe = isolated[i];
                // Calculate new time using linear interpolation
                int newTime = from + (int)((float)(keyframe.Time - from) / (oldTo - from) * (newTo - from));
                // Create a new keyframe with the updated time and original value
                var tempValue = keyframe.Value;
                isolated[i] = new CAnimatorNode<int>(newTime, tempValue);
            }
            // Optionally: Sort the isolated list if needed
            isolated.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
        private void ResizeTargetSequenceKeyframes(int from, int oldTo, int newTo, CAnimator<float> animator)
        {
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<float>> isolated = new List<CAnimatorNode<float>> { };
            foreach (var item in animator) { if (item.Time >= from && item.Time <= oldTo) { isolated.Add(item); } }
            if (isolated.Count < 2) { return; }
            if (isolated.Count > newTo - from) { MessageBox.Show("More keyframes than interval. Cannot resize."); return; }
            // Perform the resizing
            for (int i = 0; i < isolated.Count; i++)
            {
                var keyframe = isolated[i];
                // Calculate new time using linear interpolation
                int newTime = from + (int)((float)(keyframe.Time - from) / (oldTo - from) * (newTo - from));
                // Create a new keyframe with the updated time and original value
                var tempValue = keyframe.Value;
                isolated[i] = new CAnimatorNode<float>(newTime, tempValue);
            }
            // Optionally: Sort the isolated list if needed
            isolated.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
        private void ResizeTargetSequenceKeyframes(int from, int oldTo, int newTo, CAnimator<CVector4> animator)
        {
            if (animator.Count < 2) { return; }
            List<CAnimatorNode<CVector4>> isolated = new List<CAnimatorNode<CVector4>> { };
            foreach (var item in animator) { if (item.Time >= from && item.Time <= oldTo) { isolated.Add(item); } }
            if (isolated.Count < 2) { return; }
            if (isolated.Count > newTo - from) { MessageBox.Show("More keyframes than interval. Cannot resize."); return; }
            // Perform the resizing
            for (int i = 0; i < isolated.Count; i++)
            {
                var keyframe = isolated[i];
                // Calculate new time using linear interpolation
                int newTime = from + (int)((float)(keyframe.Time - from) / (oldTo - from) * (newTo - from));
                // Create a new keyframe with the updated time and original value
                var tempValue = keyframe.Value;
                isolated[i] = new CAnimatorNode<CVector4>(newTime, tempValue);
            }
            // Optionally: Sort the isolated list if needed
            isolated.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
        private void resizeSequence(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem == null) return;
            CSequence cSequence = GetSelectedSequence();
            resizeSequence_window rw = new resizeSequence_window(cSequence);
            rw.ShowDialog();
            if (rw.DialogResult == true)
            {
                int from = cSequence.IntervalStart;
                int to = rw.Result;
                int oldTo = cSequence.IntervalEnd;
                if (TrackExistsInSEquences(to, cSequence) == false)
                {
                    MessageBox.Show("The new ending track exists in another sequence"); return;
                }
                else
                {
                    cSequence.IntervalEnd = to;
                    foreach (INode node in CurrentModel.Nodes)
                    {
                        ResizeTargetSequenceKeyframes(from, oldTo, to, node.Translation);
                        ResizeTargetSequenceKeyframes(from, oldTo, to, node.Rotation);
                        ResizeTargetSequenceKeyframes(from, oldTo, to, node.Scaling);
                        if (node is CParticleEmitter)
                        {
                            CParticleEmitter item = node as CParticleEmitter;
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.EmissionRate);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.LifeSpan);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.InitialVelocity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Gravity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Longitude);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Latitude);
                        }
                        if (node is CParticleEmitter2)
                        {
                            CParticleEmitter2 item = node as CParticleEmitter2;
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Gravity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Width);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Length);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Speed);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Variation);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Latitude);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.EmissionRate);
                        }
                        if (node is CRibbonEmitter)
                        {
                            CRibbonEmitter item = node as CRibbonEmitter;
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.HeightAbove);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.HeightBelow);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.TextureSlot);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Alpha);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Color);
                        }
                        if (node is CLight)
                        {
                            CLight item = node as CLight;
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Color);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.AmbientColor);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Intensity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.AmbientIntensity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.AttenuationStart);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.AttenuationEnd);
                        }
                    }
                    foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
                    {
                        ResizeTargetSequenceKeyframes(from, oldTo, to, ga.Color);
                        ResizeTargetSequenceKeyframes(from, oldTo, to, ga.Alpha);
                    }
                    foreach (CMaterial mat in CurrentModel.Materials)
                    {
                        foreach (CMaterialLayer layer in mat.Layers)
                        {
                            ResizeTargetSequenceKeyframes(from, oldTo, to, layer.Alpha);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, layer.TextureId);
                        }
                    }
                    foreach (CCamera camera in CurrentModel.Cameras)
                    {
                        ResizeTargetSequenceKeyframes(from, oldTo, to, camera.Rotation);
                    }
                    foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
                    {
                        ResizeTargetSequenceKeyframes(from, oldTo, to, ta.Translation);
                        ResizeTargetSequenceKeyframes(from, oldTo, to, ta.Rotation);
                        ResizeTargetSequenceKeyframes(from, oldTo, to, ta.Scaling);
                    }
                }
            }
        }
        private bool SequenceNameTaken(string name)
        {
            return CurrentModel.Sequences.Any(x => x.Name.ToLower() == name.ToLower());
        }
        private void DuplicateSEquenceWithMAtchingkeyframes(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                // new sequence name and interval
                CSequence sequence = GetSelectedSequence();
                Input i = new Input("", "Duplicated sequence name");
                i.Width = 300;
                i.ShowDialog();
                if (i.DialogResult == true)
                {
                    string name = i.Result;
                    if (SequenceNameTaken(name))
                    {
                        MessageBox.Show("This name is taken"); return;
                    }
                    else
                    {
                        int[] _new = FindNextInternal(sequence.IntervalStart, sequence.IntervalEnd);
                        if (_new[0] != _new[1])
                        {
                            DuplicateSequence(sequence.IntervalStart, sequence.IntervalEnd, _new[0], _new[1]);
                            CSequence duplicated = new CSequence(CurrentModel);
                            duplicated.IntervalStart = _new[0];
                            duplicated.IntervalEnd = _new[0];
                            CurrentModel.Sequences.Add(duplicated);
                            duplicated.Name = name;
                        }
                        else
                        {
                            MessageBox.Show("Could not find a free interval for a new sequence"); return;
                        }
                    }
                }
                // Calculator.DuplicateSequence(sequence, CurrentModel);
                RefreshSequencesList();
            }
        }
        int[] FindNextInternal(int from, int to)
        {
            int interval = from + 1 + to;
            List<int> ends = CurrentModel.Sequences.Select(x => x.IntervalEnd).ToList();
            ends = ends.OrderBy(x => x).ToList();
            int free = ends[ends.Count - 1] + 1;
            if (free + interval < 999999)
            {
                return new int[] { free, free + interval };
            }
            return new int[] { 0, 0 };
        }
        private void DuplicateSequence(int oldFrom, int oldTo, int newFrom, int newTo)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, node.Translation);
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, node.Rotation);
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, node.Scaling);
                if (node is CParticleEmitter)
                {
                    CParticleEmitter item = node as CParticleEmitter;
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.EmissionRate);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.LifeSpan);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.InitialVelocity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Gravity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Longitude);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 item = node as CParticleEmitter2;
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Gravity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Width);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Length);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Speed);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Variation);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Latitude);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.EmissionRate);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter item = node as CRibbonEmitter;
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.HeightAbove);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.HeightBelow);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.TextureSlot);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Alpha);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Color);
                }
                if (node is CLight)
                {
                    CLight item = node as CLight;
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Color);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.AmbientColor);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Intensity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.AmbientIntensity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.AttenuationStart);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.AttenuationEnd);
                }
            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, ga.Color);
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, ga.Alpha);
            }
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, layer.Alpha);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, layer.TextureId);
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, camera.Rotation);
            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, ta.Translation);
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, ta.Rotation);
                DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, ta.Scaling);
            }
            Optimizer.RearrangeKeyframes(CurrentModel);
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<CVector3> animator)
        {
            // Isolate the keyframes within the old range
            List<CAnimatorNode<CVector3>> isolated = new List<CAnimatorNode<CVector3>>();
            foreach (var item in animator)
            {
                if (item.Time >= oldFrom && item.Time <= oldTo)
                {
                    isolated.Add(item);
                }
            }
            // Duplicate keyframes onto the new sequence
            foreach (var item in isolated)
            {
                // Calculate the new time using linear interpolation
                int newTime = newFrom + (int)((float)(item.Time - oldFrom) / (oldTo - oldFrom) * (newTo - newFrom));
                // Create a copy of the value
                CVector3 valueCopy = new CVector3(item.Value);
                // Create a new keyframe
                var newNode = new CAnimatorNode<CVector3>(newTime, valueCopy);
                // Add the new keyframe to the animator
                animator.Add(newNode);
            }
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<CVector4> animator)
        {
            // Isolate the keyframes within the old range
            List<CAnimatorNode<CVector4>> isolated = new List<CAnimatorNode<CVector4>>();
            foreach (var item in animator)
            {
                if (item.Time >= oldFrom && item.Time <= oldTo)
                {
                    isolated.Add(item);
                }
            }
            // Duplicate keyframes onto the new sequence
            foreach (var item in isolated)
            {
                // Calculate the new time using linear interpolation
                int newTime = newFrom + (int)((float)(item.Time - oldFrom) / (oldTo - oldFrom) * (newTo - newFrom));
                // Create a copy of the value
                CVector4 valueCopy = new CVector4(item.Value);
                // Create a new keyframe
                var newNode = new CAnimatorNode<CVector4>(newTime, valueCopy);
                // Add the new keyframe to the animator
                animator.Add(newNode);
            }
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<float> animator)
        {
            // Isolate the keyframes within the old range
            List<CAnimatorNode<float>> isolated = new List<CAnimatorNode<float>>();
            foreach (var item in animator)
            {
                if (item.Time >= oldFrom && item.Time <= oldTo)
                {
                    isolated.Add(item);
                }
            }
            // Duplicate keyframes onto the new sequence
            foreach (var item in isolated)
            {
                // Calculate the new time using linear interpolation
                int newTime = newFrom + (int)((float)(item.Time - oldFrom) / (oldTo - oldFrom) * (newTo - newFrom));
                // Create a copy of the value
                float valueCopy = item.Value;
                // Create a new keyframe
                var newNode = new CAnimatorNode<float>(newTime, valueCopy);
                // Add the new keyframe to the animator
                animator.Add(newNode);
            }
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<int> animator)
        {
            // Isolate the keyframes within the old range
            List<CAnimatorNode<int>> isolated = new List<CAnimatorNode<int>>();
            foreach (var item in animator)
            {
                if (item.Time >= oldFrom && item.Time <= oldTo)
                {
                    isolated.Add(item);
                }
            }
            // Duplicate keyframes onto the new sequence
            foreach (var item in isolated)
            {
                // Calculate the new time using linear interpolation
                int newTime = newFrom + (int)((float)(item.Time - oldFrom) / (oldTo - oldFrom) * (newTo - newFrom));
                // Create a copy of the value
                int valueCopy = item.Value;
                // Create a new keyframe
                var newNode = new CAnimatorNode<int>(newTime, valueCopy);
                // Add the new keyframe to the animator
                animator.Add(newNode);
            }
        }
        private List<string> GetGAsList(int skip)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                if (i == skip) continue;
                list.Add(CurrentModel.GeosetAnimations[i].ObjectId.ToString());
            }
            return list;
        }
        private void equalizegas_alpha(object sender, RoutedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem == null) { return; }
            if (CurrentModel.GeosetAnimations.Count > 1)
            {
                List<string> list = GetGAsList(List_GeosetAnims.SelectedIndex);
                Selector s = new Selector(list, "Based on");
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    int id = int.Parse(s.Selected);
                    CGeosetAnimation source = CurrentModel.GeosetAnimations.First(x => x.ObjectId == id);
                    foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
                    {
                        if (ga == source) continue;
                        ga.Alpha.Clear();
                        if (source.Alpha.Static == true || source.Alpha.Count == 0)
                        {
                            ga.Alpha.MakeStatic(1);
                        }
                        else
                        {
                            foreach (var item in source.Alpha)
                            {
                                var item2 = new CAnimatorNode<float>(item);
                                ga.Alpha.Add(item2);
                            }
                        }
                    }
                    RefreshGeosetAnimationsList();
                    RefreshViewPort();
                }
            }
        }
        private void equalizegas_color(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.GeosetAnimations.Count > 1)
            {
                List<string> list = GetGAsList(List_GeosetAnims.SelectedIndex);
                Selector s = new Selector(list, "Based on");
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    int id = int.Parse(s.Selected);
                    CGeosetAnimation source = CurrentModel.GeosetAnimations.First(x => x.ObjectId == id);
                    foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
                    {
                        if (ga == source) continue;
                        ga.Color.Clear();
                        if (source.Alpha.Static == true || source.Alpha.Count == 0)
                        {
                            ga.Color.MakeStatic(new CVector3(source.Color.GetValue().X, source.Color.GetValue().Y, source.Color.GetValue().Z));
                        }
                        else
                        {
                            foreach (var item in source.Color)
                            {
                                var item2 = new CAnimatorNode<CVector3>(item);
                                ga.Color.Add(item2);
                            }
                        }
                    }
                    RefreshGeosetAnimationsList();
                }
            }
        }
        private CGeosetAnimation GetSelectedGeosetAnimation()
        {
            int index = List_GeosetAnims.SelectedIndex;
            return CurrentModel.GeosetAnimations[index];
        }
        private void MakeGAUseColor(object sender, RoutedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation selected = GetSelectedGeosetAnimation();
                selected.UseColor = !selected.UseColor;
                RefreshGeosetAnimationsList();
            }
        }
        private void RefreshGeosetAnimationsList()
        {
            List_GeosetAnims.Items.Clear();
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                string alpha = ga.Alpha.Static ? ga.Alpha.GetValue().ToString() : "N/A";
                string geoset_id = "";
                if (ga.Geoset == null || ga.Geoset.Object == null)
                {
                    geoset_id = "NULL";
                }
                else
                {
                    geoset_id = ga.Geoset.ObjectId.ToString();
                }
                string name = $"Geoset Animation {ga.ObjectId} of geoset {geoset_id}: Alpha: {alpha} Alphas: {ga.Alpha.Count}, Colors: {ga.Color.Count}, Uses colors: {ga.UseColor}";
                List_GeosetAnims.Items.Add(new ListBoxItem() { Content = name });
            }
            Label_GAs.Text = $"{CurrentModel.GeosetAnimations.Count} Geoset animations";
        }
        private void AverageNormals(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosts = GetSelectedGeosets();
                foreach (CGeoset ge in geosts)
                {
                    Calculator.AverageNormals(ge);
                }
            }
        }
        private void DuplicateGeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosts = GetSelectedGeosets();
                foreach (CGeoset ge in geosts)
                {
                    CurrentModel.Geosets.Add(DuplicateGeogeset(ge, CurrentModel));
                }
            }
            RefreshGeosetsList();
            RefreshViewPort();
        }
        private void ViewNodeTransformations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                Transformations_viewer tv = new Transformations_viewer(node, CurrentModel);
                tv.ShowDialog();
            }
        }
        private void ViewGaTransformatons(object sender, RoutedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                Transformations_viewer tv = new Transformations_viewer(ga, CurrentModel);
                tv.ShowDialog();
            }
        }
        private void ImportTargetGeosetOne(string file)
        {
            if (File.Exists(file))
            {
                CModel TemporaryModel = GetTemporaryModel(file);
                if (TemporaryModel == null) { return; }
                CGeoset new_Geoset = DuplicateGeogeset(TemporaryModel.Geosets[0], CurrentModel);
                CMaterial new_material = GetTeamGlowMaterial();
                new_Geoset.Material.Attach(new_material);
                CurrentModel.Geosets.Add(new_Geoset);
                RefreshAll();
            }
            else
            {
                MessageBox.Show($"{file} was not found");
            }
        }
        private void CreateHeroGlow(object sender, RoutedEventArgs e)
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "geosets\\Hero_Glow.mdx");
            CGeoset ImportedGeoset = GetImportedGeoset(file);
            if (ImportedGeoset != null)
            {
                ImportedGeoset.Material.Attach(GetHerGlowMaterial());
                CurrentModel.Geosets.Add(ImportedGeoset);
                // AttachToNewBone(ImportedGeoset);
                RefreshGeosetsList();
                RefreshNodesTree();
            }
            CollectTextures();
            RefreshViewPort();
        }
        private CMaterial GetHerGlowMaterial()
        {
            if (CurrentModel.Textures.Any(x => x.ReplaceableId == 2) == false)
            {
                CTexture texture = new CTexture(CurrentModel);
                texture.ReplaceableId = 2;
                CurrentModel.Textures.Add(texture);
                RefreshMaterialsList();
                CMaterial mat = new CMaterial(CurrentModel);
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                layer.Texture.Attach(texture);
                mat.Layers.Add(layer);
                CurrentModel.Materials.Add(mat);
                RefreshMaterialsList();
                return mat;
            }
            else
            {
                CTexture texture = CurrentModel.Textures.First(x => x.ReplaceableId == 2);
                bool found = false;
                foreach (CMaterial mat in CurrentModel.Materials)
                {
                    foreach (CMaterialLayer layer in mat.Layers)
                    {
                        if (layer.Texture.Object == texture) { return mat; }
                    }
                }
                if (!found)
                {
                    CMaterial mat = new CMaterial(CurrentModel);
                    CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                    layer.Texture.Attach(texture);
                    mat.Layers.Add(layer);
                    CurrentModel.Materials.Add(mat);
                    RefreshMaterialsList();
                    return mat;
                }
            }
            return null;
        }
        private CMaterial GetTeamColorMaterial()
        {
            if (CurrentModel.Textures.Any(x => x.ReplaceableId == 1) == false)
            {
                CTexture texture = new CTexture(CurrentModel);
                texture.ReplaceableId = 1;
                CurrentModel.Textures.Add(texture);
                RefreshMaterialsList();
                CMaterial mat = new CMaterial(CurrentModel);
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                layer.Texture.Attach(texture);
                mat.Layers.Add(layer);
                CurrentModel.Materials.Add(mat);
                RefreshMaterialsList();
                return mat;
            }
            else
            {
                CTexture texture = CurrentModel.Textures.First(x => x.ReplaceableId == 1);
                bool found = false;
                foreach (CMaterial mat in CurrentModel.Materials)
                {
                    foreach (CMaterialLayer layer in mat.Layers)
                    {
                        if (layer.Texture.Object == texture) { return mat; }
                    }
                }
                if (!found)
                {
                    CMaterial mat = new CMaterial(CurrentModel);
                    CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                    layer.Texture.Attach(texture);
                    mat.Layers.Add(layer);
                    CurrentModel.Materials.Add(mat);
                    RefreshMaterialsList();
                    return mat;
                }
            }
            return null;
        }
        private CGeosetGroup GetGeosetGroup()
        {
            CGeosetGroup group = new CGeosetGroup(CurrentModel);
            string bone_Name = "GeneratedBone_" + IDCounter.Next_();
            //-------------------------
            CBone bone = new CBone(CurrentModel);
            bone.Name = bone_Name;
            CurrentModel.Nodes.Add(bone);
            //-------------------------
            CGeosetGroupNode gNode = new CGeosetGroupNode(CurrentModel);
            gNode.Node.Attach(bone);
            //-------------------------
            group.Nodes.Add(gNode);
            return group;
        }
        private CGeoset GetImportedGeoset(string file)
        {
            CGeoset geo = null;
            if (File.Exists(file))
            {
                CModel TemporaryModel = GetTemporaryModel(file);
                if (TemporaryModel == null) { return null; }
                CGeoset new_Geoset = DuplicateGeogeset(TemporaryModel.Geosets[0], CurrentModel);
                CMaterial new_material = GetTeamGlowMaterial();
                new_Geoset.Material.Attach(new_material);
                // new_Geoset.Groups.Add(GetGeosetGroup( ));
                geo = new_Geoset;
                // RefreshGeosetsList();
                //  RefreshNodesTree();
            }
            else
            {
                MessageBox.Show($"{file} was not found");
            }
            return geo;
        }
        private CMaterial GetTeamGlowMaterial()
        {
            CMaterial material = new CMaterial(CurrentModel);
            CMaterialLayer layer = new CMaterialLayer(CurrentModel);
            CTexture texture = new CTexture(CurrentModel);
            texture.ReplaceableId = 2;
            layer.Texture.Attach(texture);
            material.Layers.Add(layer);
            CurrentModel.Textures.Add(texture);
            CurrentModel.Materials.Add(material);
            return material;
        }
        private void refreshnodes(object sender, RoutedEventArgs e)
        {
            RefreshNodesTree();
        }
        private void importtbg(object sender, RoutedEventArgs e)
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "geosets\\Team_Background.mdx");
            CGeoset ImportedGeoset = GetImportedGeoset(file);
            if (ImportedGeoset != null)
            {
                ImportedGeoset.Material.Attach(GetHerGlowMaterial());
                CurrentModel.Geosets.Add(ImportedGeoset);
                //  AttachToNewBone(ImportedGeoset);
                RefreshGeosetsList();
                RefreshNodesTree();
            }
            CollectTextures();
            RefreshViewPort();
        }
        private void AttachToNewBone(CGeoset geoset)
        {
            CBone bone = new CBone(CurrentModel);
            bone.Name = "ImportedGeosetBone_" + IDCounter.Next();
            geoset.Groups.Clear();
            CGeosetGroup group = new CGeosetGroup(CurrentModel);
            CGeosetGroupNode groupNode = new CGeosetGroupNode(CurrentModel);
            groupNode.Node.Attach(bone);
            group.Nodes.Add(groupNode);
            geoset.Groups.Add(group);
            CurrentModel.Nodes.Add(bone);
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                vertex.Group.Attach(group);
            }
        }
        private void DelBonesGeometry(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                if (node is CBone)
                {
                    foreach (CGeoset geoset in CurrentModel.Geosets)
                    {
                        foreach (CGeosetVertex vertex in geoset.Vertices.ToList())
                        {
                            if (vertex.Group.Object.Nodes.Any(x => x.Node.NodeId == node.NodeId))
                            {
                                geoset.Vertices.Remove(vertex);
                            }
                        }
                        foreach (CGeosetTriangle face in geoset.Triangles.ToList())
                        {
                            if (
                                geoset.Vertices.Contains(face.Vertex1.Object) == false ||
                                geoset.Vertices.Contains(face.Vertex2.Object) == false ||
                                geoset.Vertices.Contains(face.Vertex3.Object) == false
                                )
                            {
                                geoset.Triangles.Remove(face);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select a bone"); return;
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        private void ListNodes_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            ButtonEditTranslations.Content = $"Translations ({node.Translation.Count})"; ;
            ButtonEditRotations.Content = $"Rotations ({node.Rotation.Count})"; ;
            ButtonEditScalings.Content = $"Scalings ({node.Scaling.Count})"; ;
            Check_Billlboarded.IsChecked = node.Billboarded;
            Check_Billlboardedx.IsChecked = node.BillboardedLockX;
            Check_Billlboardedy.IsChecked = node.BillboardedLockY;
            Check_Billlboardedz.IsChecked = node.BillboardedLockZ;
            Check_cameraAnchored.IsChecked = node.CameraAnchored;
            Check_dontInhTranslation.IsChecked = node.DontInheritTranslation;
            Check_dontInhRotation.IsChecked = node.DontInheritRotation;
            Check_DontinhScaling.IsChecked = node.DontInheritScaling;
            InfoPivotPointNode.Text = $" Pivot: {node.PivotPoint.X}, {node.PivotPoint.Y}, {node.PivotPoint.Z} ";
            StringBuilder sb = new StringBuilder();
            if (node is CCollisionShape cols)
            {
                sb.AppendLine($" Type: {cols.Type}");
                sb.AppendLine($" Minimum Extents: {cols.Vertex1.X}, {cols.Vertex1.Y}, {cols.Vertex1.Z} ");
                sb.AppendLine($" Maximum Extents: {cols.Vertex2.X}, {cols.Vertex2.Y}, {cols.Vertex2.Z} ");
                sb.AppendLine($" Radius: {cols.Radius}");
            }
            if (node is CAttachment att)
            {
                sb.AppendLine($" Path: {att.Path}");
                sb.AppendLine($" Visibilities: {att.Visibility.Count}");
            }
            if (node is CBone bone)
            {
                sb.AppendLine($"Geoset ID: {bone.Geoset.ObjectId}");
                sb.AppendLine($"Geoset Animation ID: {bone.GeosetAnimation.ObjectId}");
                sb.AppendLine(GetAttachedVerticesCountOfBone(bone));
            }
            Report_Node_data.Text = sb.ToString();
        }

        private string GetAttachedVerticesCountOfBone(CBone bone)
        {
            int count = 0;
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    foreach (var gnode in vertex.Group.Object.Nodes)
                    {
                        if (gnode.Node.Node == bone) { count++; }
                    }

                }
            }
            return $"Attached vertices: {count}";
        }

        private void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("War3ModelTuner v1.17 (18/02/2025) by stan0033 built using C#, .NET 5.0, Visual Studio 2022.\n\n Would not be possible without Magos' MDXLib v1.0.4 that reads/writes Warcraft 3 MDL/MDX model format 800.");
        }
        private void Checked_MatFullRes(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.FullResolution = Check_MatFS.IsChecked == true;
            }
        }
        private void Checked_MatSort(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.SortPrimitivesFarZ = Check_MatSort.IsChecked == true;
            }
        }
        private void Checked_MatCC(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.ConstantColor = Check_MatCC.IsChecked == true;
            }
        }
        private void List_MAterials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                Check_MatCC.IsChecked = material.ConstantColor;
                Check_MatFS.IsChecked = material.FullResolution;
                Check_MatSort.IsChecked = material.SortPrimitivesFarZ;
                FillLayers(List_MAterials.SelectedIndex);
                List_Layers.SelectedIndex = 0;
                PRiortyPlaneInput.Text = material.PriorityPlane.ToString();
            }
        }
        private void FillLayers(int index)
        {
            List_Layers.Items.Clear();
            for (int i = 0; i < CurrentModel.Materials[index].Layers.Count; i++)
            {
                CMaterialLayer layer = CurrentModel.Materials[index].Layers[i];
                string texture = layer.TextureId.Static ? layer.Texture.ObjectId.ToString() : "N/A";
                string alpha = layer.Alpha.Static ? (layer.Alpha.GetValue() * 100).ToString() : "N/A";
                string info = $": Texture: {texture} Alpha: {alpha}  Alphas: {layer.Alpha.Count}, textureIDs: {layer.TextureId.Count}";
                List_Layers.Items.Add(new ListBoxItem() { Content = i + info });
            }
        }
        private void Scene_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int zoomStep = 10; // Define the zoom step size
            if (e.Delta > 0)
            {
                AdjustZoom(zoomStep); // Zoom in
            }
            else if (e.Delta < 0)
            {
                AdjustZoom(-zoomStep); // Zoom out
            }
        }
        private string TeamColor = "ReplaceableTextures\\TeamColor\\TeamColor00.blp";
        private string TeamGlow = "ReplaceableTextures\\TeamGlow\\TeamGlow00.blp";
        private string White = "Textures\\white.blp";
        private void CollectTextures()
        {
            if (CurrentModel.Textures.Count == 0) { return; }
            Textures_Loaded = new ImageSource[CurrentModel.Textures.Count];
            for (int i = 0; i < CurrentModel.Textures.Count; i++)
            {
                CTexture texture = CurrentModel.Textures[i];
                if (texture.ReplaceableId == 0)
                {
                    if (Path.GetExtension(texture.FileName).ToLower() == ".blp")
                    {
                        Textures_Loaded[i] = MPQHelper.GetImageSource(texture.FileName);

                    }
                    else
                    {
                        MessageBox.Show($"Could not load {texture.FileName}, because it is not a BLP image");
                        Textures_Loaded[i] = MPQHelper.GetImageSource(White);

                    }


                }
                if (texture.ReplaceableId == 1)
                {
                    Textures_Loaded[i] = MPQHelper.GetImageSource(TeamColorPaths[CurrentTeamColor]);
                }
                if (texture.ReplaceableId == 2)
                {
                    Textures_Loaded[i] = MPQHelper.GetImageSource(TeamGlows[CurrentTeamColor]);
                }
                if (texture.ReplaceableId > 2)
                {
                    Textures_Loaded[i] = MPQHelper.GetImageSource(White);
                }
            }
        }
        private void refreshalllists(object sender, RoutedEventArgs e)
        {
            RefreshAll();
        }
        private void RefreshViewPort()
        {
            if (!RenderEnabled) { Scene_Viewport.Children.Clear(); return; }
            CollectTextures();
            // Clear existing models in the viewport
            Viewport3D viewport = Scene_Viewport;
            if (viewport == null) { return; }
            viewport.Children.Clear();
            // Initialize bounding box variables
            Point3D minPoint = new Point3D(double.MaxValue, double.MaxValue, double.MaxValue);
            Point3D maxPoint = new Point3D(double.MinValue, double.MinValue, double.MinValue);
            // Add a light source
            DirectionalLight light = new DirectionalLight
            {
                Color = Colors.White,
                Direction = new Vector3D(-1, -1, -1)
            };
            ModelVisual3D lightModel = new ModelVisual3D { Content = light };
            viewport.Children.Add(lightModel);

            // Render each geoset


            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                if (GeosetVisible[geo] == false) { continue; }
                MeshGeometry3D mesh = new MeshGeometry3D();
                // render the last layer's texture of the material
                int last = geo.Material.Object.Layers.Count - 1;
                int indexOfTexture = CurrentModel.Textures.IndexOf(geo.Material.Object.Layers[last].Texture.Object);
                ImageSource texture = Textures_Loaded[indexOfTexture];
                bool hasAlphaTransparency = geo.Material.Object.Layers[0].FilterMode == EMaterialLayerFilterMode.Additive || geo.Material.Object.Layers[0].FilterMode == EMaterialLayerFilterMode.AdditiveAlpha;
                // Loop through each face in the geoset
                foreach (CGeosetTriangle face in geo.Triangles)
                {
                    // Get vertex positions, normals, and texture coordinates
                    Point3D vertex1 = new Point3D(face.Vertex1.Object.Position.X, face.Vertex1.Object.Position.Y, face.Vertex1.Object.Position.Z);
                    Point3D vertex2 = new Point3D(face.Vertex2.Object.Position.X, face.Vertex2.Object.Position.Y, face.Vertex2.Object.Position.Z);
                    Point3D vertex3 = new Point3D(face.Vertex3.Object.Position.X, face.Vertex3.Object.Position.Y, face.Vertex3.Object.Position.Z);

                    Vector3D normal3 = new Vector3D(face.Vertex3.Object.Normal.X, face.Vertex3.Object.Normal.Y, face.Vertex3.Object.Normal.Z);
                    Vector3D normal2 = new Vector3D(face.Vertex2.Object.Normal.X, face.Vertex2.Object.Normal.Y, face.Vertex2.Object.Normal.Z);
                    Vector3D normal1 = new Vector3D(face.Vertex1.Object.Normal.X, face.Vertex1.Object.Normal.Y, face.Vertex1.Object.Normal.Z);

                    Point vertex1TexCoord = new Point(face.Vertex1.Object.TexturePosition.X, face.Vertex1.Object.TexturePosition.Y);
                    Point vertex2TexCoord = new Point(face.Vertex2.Object.TexturePosition.X, face.Vertex2.Object.TexturePosition.Y);
                    Point vertex3TexCoord = new Point(face.Vertex3.Object.TexturePosition.X, face.Vertex3.Object.TexturePosition.Y);



                    // Update bounding box
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex1);
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex2);
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex3);
                    // Add vertices, normals, and texture coordinates
                    mesh.Positions.Add(vertex1);
                    if (RenderShading) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(vertex1TexCoord);
                    mesh.Positions.Add(vertex2);
                    if (RenderShading) mesh.Normals.Add(normal2);
                    mesh.TextureCoordinates.Add(vertex2TexCoord);
                    mesh.Positions.Add(vertex3);
                    if (RenderShading) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(vertex3TexCoord);
                    // Add triangle indices
                    int baseIndex = mesh.Positions.Count - 3;
                    mesh.TriangleIndices.Add(baseIndex);
                    mesh.TriangleIndices.Add(baseIndex + 1);
                    mesh.TriangleIndices.Add(baseIndex + 2);
                }
                // Create a material for the geoset
                Material material;
                if (SelectedGeosets_Collection.Contains(geo))
                {
                    material = new DiffuseMaterial(new SolidColorBrush(Colors.Yellow));
                }
                else
                {
                    if (!RenderTextures)
                    {
                        material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
                    }
                    else
                    {
                        // Handle transparency logic based on the hasAlphaTransparency flag
                        if (hasAlphaTransparency)
                        {
                            // Use an ImageBrush for the texture and set Opacity to allow transparency
                            ImageBrush imageBrush = new ImageBrush(texture)
                            {
                                Opacity = 0.5 // Set transparency here, adjust as needed
                            };
                            // Create a material using the ImageBrush for transparency
                            material = new DiffuseMaterial(imageBrush);
                        }
                        else
                        {
                            // If no alpha transparency, use the diffuse material with the texture
                            material = new DiffuseMaterial(new ImageBrush(texture));
                        }
                    }
                }
                // Create a GeometryModel3D
                GeometryModel3D geometryModel = new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = material,
                    BackMaterial = material, // Ensures the material is used for both sides of the geometry
                };
                // Enable double-sided rendering by setting BackFaceCulling to None
                geometryModel.Material = material;
                geometryModel.BackMaterial = material;  // Apply the same material to both sides
                                                        // Add the model to the viewport
                ModelVisual3D model = new ModelVisual3D { Content = geometryModel };
                if (RenderGeometry) viewport.Children.Add(model);
                if (RenderGeosetExtents) RenderGeosetBoxAction(Scene_Viewport, geo);
                if (RenderGeosetExtentSphere) { RenderGeosetSphereAction(Scene_Viewport, geo); }
            }

            // Adjust the camera to fit the geometry

            AdjustCamera(viewport, minPoint, maxPoint);
            if (RenderCollisionShapes) RenderCollisionShapesAction(Scene_Viewport);
            if (RenderNodes) RenderNodesInViewport(Scene_Viewport);

            if (RenderGround) DrawGroundPlane(Scene_Viewport, 1000, GroundTexture);
        }

        private void RenderGeosetSphereAction(Viewport3D viewport, CGeoset geo)
        {
            float radius = geo.Extent.Radius;
            if (radius <= 0) return;

            var sphereGeometry = new MeshGeometry3D();
            const int segments = 32;
            const int rings = 16;
            var centroid = Calculator.GetCentroidOfGeoset(geo);
            Point3D pivot = new Point3D(centroid.X, centroid.Y, centroid.Z);

            // Generate sphere vertices
            for (int y = 0; y <= rings; y++)
            {
                double theta = Math.PI * y / rings;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int x = 0; x <= segments; x++)
                {
                    double phi = 2 * Math.PI * x / segments;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    float xCoord = (float)(radius * cosPhi * sinTheta);
                    float yCoord = (float)(radius * cosTheta);
                    float zCoord = (float)(radius * sinPhi * sinTheta);

                    var position = new Point3D(
                        xCoord + pivot.X,
                        yCoord + pivot.Y,
                        zCoord + pivot.Z);

                    sphereGeometry.Positions.Add(position);

                    // Calculate and add normals
                    Vector3D normal = new Vector3D(xCoord, yCoord, zCoord);
                    normal.Normalize();
                    sphereGeometry.Normals.Add(normal);
                }
            }

            // Generate sphere triangles
            for (int y = 0; y < rings; y++)
            {
                for (int x = 0; x < segments; x++)
                {
                    int first = y * (segments + 1) + x;
                    int second = first + segments + 1;

                    sphereGeometry.TriangleIndices.Add(first);
                    sphereGeometry.TriangleIndices.Add(second);
                    sphereGeometry.TriangleIndices.Add(first + 1);

                    sphereGeometry.TriangleIndices.Add(second);
                    sphereGeometry.TriangleIndices.Add(second + 1);
                    sphereGeometry.TriangleIndices.Add(first + 1);
                }
            }

            AddGeometryToViewport(viewport, sphereGeometry, 0, 255, 0);
        }
        private void RenderGeosetBoxAction(Viewport3D viewport, CGeoset geo)
        {

            RenderBoxGeoset(viewport, geo);


        }

        private void RenderBoxGeoset(Viewport3D viewport, CGeoset geoset)
        {

            Point3D pivot = new Point3D();//0
            float minX = geoset.Extent.Min.X;
            float minY = geoset.Extent.Min.Y;
            float minZ = geoset.Extent.Min.Z;
            float maxX = geoset.Extent.Max.X;
            float maxY = geoset.Extent.Max.Y;
            float maxZ = geoset.Extent.Max.Z;

            if (!BoxHasArea(minX, minY, minZ, maxX, maxY, maxZ))
                return;

            var boxGeometry = new MeshGeometry3D();

            var corners = new[]
            {
        new Point3D(minX, minY, minZ) + (Vector3D)pivot, // 0
        new Point3D(maxX, minY, minZ) + (Vector3D)pivot, // 1
        new Point3D(maxX, maxY, minZ) + (Vector3D)pivot, // 2
        new Point3D(minX, maxY, minZ) + (Vector3D)pivot, // 3
        new Point3D(minX, minY, maxZ) + (Vector3D)pivot, // 4
        new Point3D(maxX, minY, maxZ) + (Vector3D)pivot, // 5
        new Point3D(maxX, maxY, maxZ) + (Vector3D)pivot, // 6
        new Point3D(minX, maxY, maxZ) + (Vector3D)pivot  // 7
    };

            foreach (var corner in corners)
                boxGeometry.Positions.Add(corner);

            // Corrected indices for all six faces
            int[] faceIndices = {
        0, 1, 2, 0, 2, 3,  // Front
        4, 6, 5, 4, 7, 6,  // Back
        0, 4, 5, 0, 5, 1,  // Bottom
        2, 6, 7, 2, 7, 3,  // Top
        0, 3, 7, 0, 7, 4,  // Left
        1, 5, 6, 1, 6, 2   // Right
    };

            foreach (int index in faceIndices)
                boxGeometry.TriangleIndices.Add(index);

            // Add normals for shading
            for (int i = 0; i < 8; i++)
            {
                Vector3D normal = new Vector3D(corners[i].X, corners[i].Y, corners[i].Z);
                normal.Normalize();
                boxGeometry.Normals.Add(normal);
            }

            AddGeometryToViewport(viewport, boxGeometry, 0, 255, 0);
        }


        //----------------------------------------------------
        private void RenderCollisionShapesAction(Viewport3D viewport)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CCollisionShape cols)
                {
                    if (cols.Type == ECollisionShapeType.Box)
                    {
                        RenderBox(viewport, cols);
                    }
                    else if (cols.Type == ECollisionShapeType.Sphere)
                    {
                        RenderSphere(viewport, cols);
                    }
                }
            }

        }

        private void RenderSphere(Viewport3D viewport, CCollisionShape cols)
        {
            float radius = cols.Radius;
            if (radius <= 0) return;

            var sphereGeometry = new MeshGeometry3D();
            const int segments = 32;
            const int rings = 16;
            Point3D pivot = new Point3D(cols.PivotPoint.X, cols.PivotPoint.Y, cols.PivotPoint.Z);

            // Generate sphere vertices
            for (int y = 0; y <= rings; y++)
            {
                double theta = Math.PI * y / rings;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int x = 0; x <= segments; x++)
                {
                    double phi = 2 * Math.PI * x / segments;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    float xCoord = (float)(radius * cosPhi * sinTheta);
                    float yCoord = (float)(radius * cosTheta);
                    float zCoord = (float)(radius * sinPhi * sinTheta);

                    var position = new Point3D(
                        xCoord + pivot.X,
                        yCoord + pivot.Y,
                        zCoord + pivot.Z);

                    sphereGeometry.Positions.Add(position);

                    // Calculate and add normals
                    Vector3D normal = new Vector3D(xCoord, yCoord, zCoord);
                    normal.Normalize();
                    sphereGeometry.Normals.Add(normal);
                }
            }

            // Generate sphere triangles
            for (int y = 0; y < rings; y++)
            {
                for (int x = 0; x < segments; x++)
                {
                    int first = y * (segments + 1) + x;
                    int second = first + segments + 1;

                    sphereGeometry.TriangleIndices.Add(first);
                    sphereGeometry.TriangleIndices.Add(second);
                    sphereGeometry.TriangleIndices.Add(first + 1);

                    sphereGeometry.TriangleIndices.Add(second);
                    sphereGeometry.TriangleIndices.Add(second + 1);
                    sphereGeometry.TriangleIndices.Add(first + 1);
                }
            }

            AddGeometryToViewport(viewport, sphereGeometry, 0, 0, 255);
        }

        private void RenderBox(Viewport3D viewport, CCollisionShape cols)
        {
            Point3D pivot = new Point3D(cols.PivotPoint.X, cols.PivotPoint.Y, cols.PivotPoint.Z);
            float minX = cols.Vertex1.X;
            float minY = cols.Vertex1.Y;
            float minZ = cols.Vertex1.Z;
            float maxX = cols.Vertex2.X;
            float maxY = cols.Vertex2.Y;
            float maxZ = cols.Vertex2.Z;

            if (!BoxHasArea(minX, minY, minZ, maxX, maxY, maxZ))
                return;

            var boxGeometry = new MeshGeometry3D();

            var corners = new[]
            {
        new Point3D(minX, minY, minZ) + (Vector3D)pivot, // 0
        new Point3D(maxX, minY, minZ) + (Vector3D)pivot, // 1
        new Point3D(maxX, maxY, minZ) + (Vector3D)pivot, // 2
        new Point3D(minX, maxY, minZ) + (Vector3D)pivot, // 3
        new Point3D(minX, minY, maxZ) + (Vector3D)pivot, // 4
        new Point3D(maxX, minY, maxZ) + (Vector3D)pivot, // 5
        new Point3D(maxX, maxY, maxZ) + (Vector3D)pivot, // 6
        new Point3D(minX, maxY, maxZ) + (Vector3D)pivot  // 7
    };

            foreach (var corner in corners)
                boxGeometry.Positions.Add(corner);

            // Corrected indices for all six faces
            int[] faceIndices = {
        0, 1, 2, 0, 2, 3,  // Front
        4, 6, 5, 4, 7, 6,  // Back
        0, 4, 5, 0, 5, 1,  // Bottom
        2, 6, 7, 2, 7, 3,  // Top
        0, 3, 7, 0, 7, 4,  // Left
        1, 5, 6, 1, 6, 2   // Right
    };

            foreach (int index in faceIndices)
                boxGeometry.TriangleIndices.Add(index);

            // Add normals for shading
            for (int i = 0; i < 8; i++)
            {
                Vector3D normal = new Vector3D(corners[i].X, corners[i].Y, corners[i].Z);
                normal.Normalize();
                boxGeometry.Normals.Add(normal);
            }

            AddGeometryToViewport(viewport, boxGeometry, 0, 0, 255);
        }

        private bool BoxHasArea(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            return (maxX - minX) > float.Epsilon && (maxY - minY) > float.Epsilon && (maxZ - minZ) > float.Epsilon;
        }

        private void AddGeometryToViewport(Viewport3D viewport, MeshGeometry3D geometry, byte r, byte g, byte b)
        {
            var material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(100, r, g, b)));
            var model = new GeometryModel3D(geometry, material);
            model.BackMaterial = material;

            var visual = new ModelVisual3D { Content = model };
            viewport.Children.Add(visual);
        }

        //----------------------------------------------------
        private float rotation_angle = 0;
        private float CurrentAngle
        {
            get { return rotation_angle; }
            set
            {
                if (value >= 360) { rotation_angle = 0; } // Wrap around when exceeding 360 degrees
                else if (value < 0) { rotation_angle = 359; } // Wrap around when going below 0 degrees
                else { rotation_angle = value; }
            }
        }
        //----------------------------------------------------
        private void UpdateBoundingBox(ref Point3D minPoint, ref Point3D maxPoint, Point3D vertex)
        {
            minPoint.X = Math.Min(minPoint.X, vertex.X);
            minPoint.Y = Math.Min(minPoint.Y, vertex.Y);
            minPoint.Z = Math.Min(minPoint.Z, vertex.Z);
            maxPoint.X = Math.Max(maxPoint.X, vertex.X);
            maxPoint.Y = Math.Max(maxPoint.Y, vertex.Y);
            maxPoint.Z = Math.Max(maxPoint.Z, vertex.Z);
        }
        private void AdjustCamera(Viewport3D viewport, Point3D minPoint, Point3D maxPoint)
        {
            // Calculate the center and size of the bounding box
            Point3D center = new Point3D(
                (minPoint.X + maxPoint.X) / 2,
                (minPoint.Y + maxPoint.Y) / 2,
                (minPoint.Z + maxPoint.Z) / 2
            );
            Vector3D size = new Vector3D(
                maxPoint.X - minPoint.X,
                maxPoint.Y - minPoint.Y,
                maxPoint.Z - minPoint.Z
            );
            // Calculate the diagonal of the bounding box
            double boundingDiagonal = size.Length;
            // Calculate the camera distance to fit the entire geometry
            double distance = boundingDiagonal / (2 * Math.Tan(45 * Math.PI / 360)); // Assuming a 45° field of view
            // Create a new PerspectiveCamera
            PerspectiveCamera camera = new PerspectiveCamera
            {
                Position = new Point3D(center.X, center.Y, center.Z + distance),
                LookDirection = new Vector3D(0, 0, -distance),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 45 // Adjust this value if necessary
            };
            // Assign the new camera to the viewport
            viewport.Camera = camera;
            double zoom = (camera.Position - new Point3D(0, 0, 0)).Length;
            UpdateCameraPosition(viewport, (int)zoom, CurrentAngle);
        }
        private void AdjustZoom(double zoomFactor)
        {
            if (CurrentModel.Geosets.Count == 0) { return; }
            //MessageBox.Show("called");
            if (Scene_Viewport.Camera is PerspectiveCamera camera)
            {
                // MessageBox.Show("called");
                // Normalize the LookDirection vector to ensure consistent movement
                Vector3D lookDirection = camera.LookDirection;
                lookDirection.Normalize();
                // Calculate the new position by scaling the look direction with zoomFactor
                Point3D newPosition = new Point3D(
                    camera.Position.X + lookDirection.X * zoomFactor,
                    camera.Position.Y + lookDirection.Y * zoomFactor,
                    camera.Position.Z + lookDirection.Z * zoomFactor
                );
                // Update the camera's position
                camera.Position = newPosition;
            }
        }
        private double Elevation = 0;
        private void UpdateCameraPosition(Viewport3D viewport, int zoom, float rotation)
        {
            try
            {
                if (viewport.Camera is PerspectiveCamera camera)
                {
                    // Define the center of the scene (default at origin)
                    Point3D center = new Point3D(0, 0, 0);
                    // Convert rotation angle to radians
                    double rotationRadians = Math.PI * rotation / 180.0;
                    // Calculate the new position in the X-Y plane
                    double x = center.X + zoom * Math.Cos(rotationRadians);
                    double y = center.Y + zoom * Math.Sin(rotationRadians);
                    double z = center.Z; // Keep Z constant for rotation around Z-axis
                    // Update the camera's position
                    camera.Position = new Point3D(x, y, z);
                    // Ensure the camera looks at the center of the scene
                    camera.LookDirection = new Vector3D(center.X - x, center.Y - y, center.Z - z);
                    // Correctly set the camera's UpDirection to always point along the Z-axis
                    camera.UpDirection = new Vector3D(0, 0, 1);
                }
            }
            catch { return; }
        }
        private void AdjustRotation(float currentAngle)
        {
            Viewport3D viewport = Scene_Viewport;
            if (viewport.Camera is PerspectiveCamera camera)
            {
                // Define the zoom level (distance from the center)
                // You can make this dynamic or retrieve it from the current camera position
                double zoom = (camera.Position - new Point3D(0, 0, 0)).Length;
                UpdateCameraPosition(viewport, (int)zoom, currentAngle);
            }
        }
        private void setModelName(object sender, RoutedEventArgs e)
        {
            Input i = new Input(CurrentModel.Name);
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                CurrentModel.Name = i.Result;
            }
        }
        private List<CGeoset> SelectedGeosets_Collection = new List<CGeoset>();
        private void SelectedGeosets(object sender, SelectionChangedEventArgs e)
        {
            SelectedGeosets_Collection = GetSelectedGeosets();
            RefreshViewPort();
        }

        private void SelectedLayer(object sender, SelectionChangedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                Check_twosided.IsChecked = layer.TwoSided;
                Check_Unshaded.IsChecked = layer.Unshaded;
                Check_Unfogged.IsChecked = layer.Unfogged;
                Check_sf.IsChecked = layer.SphereEnvironmentMap;
                Check_NoDepthSet.IsChecked = layer.NoDepthSet;
                Check_NoDepthTest.IsChecked = layer.NoDepthTest;
                Combo_FilterModeLayer.SelectedIndex = (int)layer.FilterMode;
                // Pause = true;
                ;
                Combo_LayerUsedTexture.SelectedIndex = CurrentModel.Textures.IndexOf(layer.Texture.Object);
                // Pause = false;
                Combo_LayerUsedTextureAnim.SelectedIndex = 0;
                if (layer.TextureAnimation != null)
                {
                    if (CurrentModel.TextureAnimations.Contains(layer.TextureAnimation.Object))
                    {
                        Combo_LayerUsedTextureAnim.SelectedIndex =
                           CurrentModel.TextureAnimations.IndexOf(layer.TextureAnimation.Object) + 1;
                    }
                }
            }
        }
        private bool Pause = false;
        private void RefreshLayersTextureAnimList()
        {
            Combo_LayerUsedTextureAnim.Items.Clear();
            Combo_LayerUsedTextureAnim.Items.Add(new ComboBoxItem() { Content = "None" });
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                Combo_LayerUsedTextureAnim.Items.Add(new ComboBoxItem() { Content = ta.ObjectId.ToString() });
            }
            Combo_LayerUsedTextureAnim.SelectedIndex = 0;
        }
        private void RefreshLayersTextureList()
        {
            Combo_LayerUsedTexture.Items.Clear();
            foreach (CTexture texture in CurrentModel.Textures)
            {
                string name = texture.ReplaceableId == 0 ? texture.FileName : "Replaceable ID" + texture.ReplaceableId.ToString();
                Combo_LayerUsedTexture.Items.Add(new ComboBoxItem() { Content = name });
            }
        }
        private void RefreshLayersList()
        {
            if (List_MAterials.SelectedItem != null)
            {
                FillLayers(List_MAterials.SelectedIndex);
            }
        }
        private void DelLayer(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                if (mat.Layers.Count == 1) { MessageBox.Show("Cannot delete the only remaining layer"); return; }
                mat.Layers.RemoveAt(List_Layers.SelectedIndex);
                RefreshLayersList();
            }
        }
        private void Checked_Unshaded(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.Unshaded = Check_Unshaded.IsChecked == true;
            }
        }
        private void Checked_Unfogged(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.Unfogged = Check_Unfogged.IsChecked == true;
            }
        }
        private void Checked_twosided(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.TwoSided = Check_twosided.IsChecked == true;
            }
        }
        private void Checked_sf(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.SphereEnvironmentMap = Check_sf.IsChecked == true;
            }
        }
        private void Checked_NoDepthTest(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.NoDepthTest = Check_NoDepthTest.IsChecked == true;
            }
        }
        private void Checked_NoDepthSet(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.NoDepthSet = Check_NoDepthSet.IsChecked == true;
            }
        }
        private void viewLayerTransformations(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                Transformations_viewer tv = new Transformations_viewer(layer, CurrentModel);
                tv.ShowDialog();
            }
        }
        private void DeleteTextureAnim(object sender, RoutedEventArgs e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = CurrentModel.TextureAnimations[List_TextureAnims.SelectedIndex];
                if (ta.HasReferences)
                {
                    if (askDelete())
                    {
                        CurrentModel.TextureAnimations.Remove(ta);
                        RefreshTextureAnims();
                    }
                }
                else
                {
                    CurrentModel.TextureAnimations.Remove(ta);
                    RefreshTextureAnims();
                }
            }
        }
        private void viewTextureAnims(object sender, RoutedEventArgs e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                Transformations_viewer tv = new Transformations_viewer(CurrentModel.TextureAnimations[List_TextureAnims.SelectedIndex], CurrentModel);
                tv.ShowDialog();
            }
        }
        private INode CopiedNode;
        private void copyNodeTranslations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                CopiedNode = GetSeletedNode();
            }
            else { MessageBox.Show("Select a node"); }
        }
        private void pastenodetranslations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                if (CopiedNode == null)
                {
                    MessageBox.Show("No copied node"); return;
                }
                if (CurrentModel.Nodes.Contains(CopiedNode) == false)
                {
                    MessageBox.Show("The copied not no longer exists"); return;
                }
                if (CopiedNode == node) { MessageBox.Show("The copies and the target nodes cannot be the same"); return; }
                transformation_selector ts = new transformation_selector();
                ts.ShowDialog();
                if (ts.DialogResult == true)
                {
                    bool t = ts.C1.IsChecked == true;
                    bool r = ts.C2.IsChecked == true;
                    bool s = ts.C3.IsChecked == true;
                    if (t)
                    {
                        node.Translation.Clear();
                        foreach (var item in CopiedNode.Translation)
                        {
                            CVector3 copieedVector = new CVector3(item.Value);
                            CAnimatorNode<CVector3> copy = new CAnimatorNode<CVector3>(item.Time, copieedVector);
                            node.Translation.Add(copy);
                        }
                    }
                    if (s)
                    {
                        node.Scaling.Clear();
                        foreach (var item in CopiedNode.Scaling)
                        {
                            CVector3 copieedVector = new CVector3(item.Value);
                            CAnimatorNode<CVector3> copy = new CAnimatorNode<CVector3>(item.Time, copieedVector);
                            node.Scaling.Add(copy);
                        }
                    }
                    if (r)
                    {
                        node.Rotation.Clear();
                        foreach (var item in CopiedNode.Rotation)
                        {
                            CVector4 copieedVector = new CVector4(item.Value);
                            CAnimatorNode<CVector4> copy = new CAnimatorNode<CVector4>(item.Time, copieedVector);
                            node.Rotation.Add(copy);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select a node");
                }
            }
        }
        private void NegateNodePositon(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    float X = ax.Check_x.IsChecked == true ? -node.PivotPoint.X : node.PivotPoint.X;
                    float Y = ax.Check_y.IsChecked == true ? -node.PivotPoint.Y : node.PivotPoint.Y;
                    float Z = ax.Check_z.IsChecked == true ? -node.PivotPoint.Z : node.PivotPoint.Z;
                    node.PivotPoint = new CVector3(X, Y, Z);
                }
            }
        }
        private void Tranlateallnodes(object sender, RoutedEventArgs e)
        {
            InputVector iv = new InputVector(AllowedValue.Both);
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X;
                float y = iv.Y;
                float z = iv.Z;
                foreach (INode node in CurrentModel.Nodes)
                {
                    float cx = node.PivotPoint.X + x;
                    float cy = node.PivotPoint.Y + y;
                    float cz = node.PivotPoint.Z + z;
                    node.PivotPoint = new CVector3(cx, cy, cz);
                }
            }
        }
        private void syncwhith(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence FirstSequence = GetSelectedSequence();
            MessageBox.Show("For each node that contains animations of the first sequence, if the seocnd sequence's associates nodes contain the same amount keyframes, they will sync in time with the first sequence");
            List<string> sequences = CurrentModel.Sequences.Select(x => x.Name).ToList();
            sequences.Remove(FirstSequence.Name);
            if (sequences.Count > 1)
            {
                Selector s = new Selector(sequences);
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    CSequence SecondSequence = CurrentModel.Sequences.First(x => x.Name == s.Selected);
                    if (FirstSequence == SecondSequence) { MessageBox.Show("First and scond sequence must not be the same"); return; }
                    SyncSequences(FirstSequence, SecondSequence);
                }
            }
            else
            {
                MessageBox.Show("At least two sequences must be present");
            }
        }
        private void SyncSequences(CSequence firstSequence, CSequence secondSequence)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                List<CAnimatorNode<CVector3>> Translations_Sequence1 = node.Translation.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<CVector3>> Translations_Sequence2 = node.Translation.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (Translations_Sequence1.Count == Translations_Sequence2.Count)
                {
                    Synchronize(Translations_Sequence1, Translations_Sequence2);
                }
                List<CAnimatorNode<CVector3>> Scalings_Sequence1 = node.Scaling.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<CVector3>> Salings_Sequence2 = node.Scaling.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (Translations_Sequence1.Count == Translations_Sequence2.Count)
                {
                    Synchronize(Scalings_Sequence1, Salings_Sequence2);
                }
                List<CAnimatorNode<CVector4>> Rotations_Sequence1 = node.Rotation.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<CVector4>> Rotations_Sequence2 = node.Rotation.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (Rotations_Sequence1.Count == Rotations_Sequence2.Count)
                {
                    Synchronize(Rotations_Sequence1, Rotations_Sequence2);
                }
                if (node is CAttachment attachment)
                {
                    List<CAnimatorNode<float>> list1 = attachment.Visibility.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                    List<CAnimatorNode<float>> list2 = attachment.Visibility.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                    if (Translations_Sequence1.Count == Translations_Sequence2.Count)
                    {
                        Synchronize(list1, list2);
                    }
                }
                if (node is CLight light)
                {
                    if (light.Color.Static == false && light.Color.Count > 1)
                    {
                        List<CAnimatorNode<CVector3>> list1 = light.Color.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<CVector3>> list2 = light.Color.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (light.AmbientColor.Static == false && light.AmbientColor.Count > 1)
                    {
                        List<CAnimatorNode<CVector3>> list1 = light.AmbientColor.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<CVector3>> list2 = light.AmbientColor.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (light.Intensity.Static == false && light.Intensity.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = light.Intensity.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = light.Intensity.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (light.AmbientIntensity.Static == false && light.AmbientIntensity.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = light.AmbientIntensity.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = light.AmbientIntensity.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (light.AttenuationStart.Static == false && light.AttenuationStart.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = light.AttenuationStart.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = light.AttenuationStart.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (light.AttenuationEnd.Static == false && light.AttenuationEnd.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = light.AttenuationEnd.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = light.AttenuationEnd.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (light.Visibility.Static == false && light.Visibility.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = light.Visibility.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = light.Visibility.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                }
                if (node is CParticleEmitter emitter)
                {
                    if (emitter.Visibility.Static == false && emitter.Visibility.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.Visibility.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.Visibility.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.EmissionRate.Static == false && emitter.EmissionRate.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.EmissionRate.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.EmissionRate.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.LifeSpan.Static == false && emitter.LifeSpan.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.LifeSpan.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.LifeSpan.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.Gravity.Static == false && emitter.Gravity.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.Gravity.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.Gravity.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.Longitude.Static == false && emitter.Longitude.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.Longitude.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.Longitude.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.Latitude.Static == false && emitter.Latitude.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.Latitude.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.Latitude.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.Visibility.Static == false && emitter.Visibility.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.Visibility.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.Visibility.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter.InitialVelocity.Static == false && emitter.InitialVelocity.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter.InitialVelocity.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter.InitialVelocity.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    if (emitter2.Visibility.Static == false && emitter2.Visibility.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Visibility.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Visibility.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.Speed.Static == false && emitter2.Speed.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Speed.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Speed.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.Gravity.Static == false && emitter2.Gravity.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Gravity.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Gravity.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.Variation.Static == false && emitter2.Variation.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Variation.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Variation.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.Latitude.Static == false && emitter2.Latitude.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Latitude.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Latitude.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.Width.Static == false && emitter2.Width.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Width.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Width.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.Length.Static == false && emitter2.Length.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.Length.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.Length.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (emitter2.EmissionRate.Static == false && emitter2.EmissionRate.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = emitter2.EmissionRate.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = emitter2.EmissionRate.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                }
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Visibility.Static == false && ribbon.Visibility.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = ribbon.Visibility.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = ribbon.Visibility.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (ribbon.Color.Static == false && ribbon.Color.Count > 1)
                    {
                        List<CAnimatorNode<CVector3>> list1 = ribbon.Color.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<CVector3>> list2 = ribbon.Color.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (ribbon.Alpha.Static == false && ribbon.Alpha.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = ribbon.Alpha.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = ribbon.Alpha.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (ribbon.TextureSlot.Static == false && ribbon.TextureSlot.Count > 1)
                    {
                        List<CAnimatorNode<int>> list1 = ribbon.TextureSlot.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<int>> list2 = ribbon.TextureSlot.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (ribbon.HeightAbove.Static == false && ribbon.HeightAbove.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = ribbon.HeightAbove.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = ribbon.HeightAbove.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                    if (ribbon.HeightBelow.Static == false && ribbon.HeightBelow.Count > 1)
                    {
                        List<CAnimatorNode<float>> list1 = ribbon.HeightBelow.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> list2 = ribbon.HeightBelow.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (list1.Count == list2.Count)
                        {
                            Synchronize(list1, list2);
                        }
                    }
                }
            }
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.Alpha.Static == false)
                    {
                        List<CAnimatorNode<float>> alphas1 = layer.Alpha.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<float>> alphas2 = layer.Alpha.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (alphas1.Count == alphas2.Count)
                        {
                            Synchronize(alphas1, alphas2);
                        }
                    }
                    if (layer.TextureId.Static == false)
                    {
                        List<CAnimatorNode<int>> t1 = layer.TextureId.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                        List<CAnimatorNode<int>> t2 = layer.TextureId.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                        if (t1.Count == t2.Count)
                        {
                            Synchronize(t1, t2);
                        }
                    }
                }
            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                if (ga.Alpha.Static == false)
                {
                    List<CAnimatorNode<float>> alphas1 = ga.Alpha.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                    List<CAnimatorNode<float>> alphas2 = ga.Alpha.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                    if (alphas1.Count == alphas2.Count)
                    {
                        Synchronize(alphas1, alphas2);
                    }
                }
                if (ga.Color.Static == false && ga.UseColor)
                {
                    List<CAnimatorNode<CVector3>> list1 = ga.Color.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                    List<CAnimatorNode<CVector3>> list2 = ga.Color.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                    if (list1.Count == list2.Count)
                    {
                        Synchronize(list1, list2);
                    }
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                if (camera.Rotation.Static == false)
                {
                }
                List<CAnimatorNode<float>> list1 = camera.Rotation.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<float>> list2 = camera.Rotation.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (list1.Count == list2.Count)
                {
                    Synchronize(list1, list2);
                }
            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                List<CAnimatorNode<CVector3>> Translations_Sequence1 = ta.Translation.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<CVector3>> Translations_Sequence2 = ta.Translation.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (Translations_Sequence1.Count == Translations_Sequence2.Count)
                {
                    Synchronize(Translations_Sequence1, Translations_Sequence2);
                }
                List<CAnimatorNode<CVector3>> Scalings_Sequence1 = ta.Scaling.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<CVector3>> Salings_Sequence2 = ta.Scaling.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (Translations_Sequence1.Count == Translations_Sequence2.Count)
                {
                    Synchronize(Scalings_Sequence1, Salings_Sequence2);
                }
                List<CAnimatorNode<CVector4>> Rotations_Sequence1 = ta.Rotation.Where(x => x.Time >= firstSequence.IntervalStart && x.Time <= firstSequence.IntervalEnd).ToList();
                List<CAnimatorNode<CVector4>> Rotations_Sequence2 = ta.Rotation.Where(x => x.Time >= secondSequence.IntervalStart && x.Time <= secondSequence.IntervalEnd).ToList();
                if (Translations_Sequence1.Count == Translations_Sequence2.Count)
                {
                    Synchronize(Rotations_Sequence1, Rotations_Sequence2);
                }
            }
        }
        public List<CAnimatorNode<CVector3>> Synchronize(List<CAnimatorNode<CVector3>> list1, List<CAnimatorNode<CVector3>> list2)
        {
            if (list2.Count == 0) { return list2; }
            List<CAnimatorNode<CVector3>> result = new List<CAnimatorNode<CVector3>>();
            // Calculate the relative time gaps in list1
            for (int i = 0; i < list1.Count; i++)
            {
                int newTime;
                if (i == 0)
                {
                    // First keyframe matches exactly
                    newTime = list1[i].Time;
                }
                else
                {
                    // Compute the relative gap and apply it to list2's starting time
                    int gap = list1[i].Time - list1[i - 1].Time;
                    newTime = result[i - 1].Time + gap;
                }
                // Create a new node with synchronized time and the original value from list2
                CVector3 newValue = new CVector3(list2[i].Value);
                result.Add(new CAnimatorNode<CVector3>(newTime, newValue));
            }
            return result;
        }
        public List<CAnimatorNode<CVector4>> Synchronize(List<CAnimatorNode<CVector4>> list1, List<CAnimatorNode<CVector4>> list2)
        {
            if (list2.Count == 0) { return list2; }
            List<CAnimatorNode<CVector4>> result = new List<CAnimatorNode<CVector4>>();
            // Calculate the relative time gaps in list1
            for (int i = 0; i < list1.Count; i++)
            {
                int newTime;
                if (i == 0)
                {
                    // First keyframe matches exactly
                    newTime = list1[i].Time;
                }
                else
                {
                    // Compute the relative gap and apply it to list2's starting time
                    int gap = list1[i].Time - list1[i - 1].Time;
                    newTime = result[i - 1].Time + gap;
                }
                // Create a new node with synchronized time and the original value from list2
                CVector4 newValue = new CVector4(list2[i].Value);
                result.Add(new CAnimatorNode<CVector4>(newTime, newValue));
            }
            return result;
        }
        public List<CAnimatorNode<int>> Synchronize(List<CAnimatorNode<int>> list1, List<CAnimatorNode<int>> list2)
        {
            if (list2.Count == 0) { return list2; }
            List<CAnimatorNode<int>> result = new List<CAnimatorNode<int>>();
            // Calculate the relative time gaps in list1
            for (int i = 0; i < list1.Count; i++)
            {
                int newTime;
                if (i == 0)
                {
                    // First keyframe matches exactly
                    newTime = list1[i].Time;
                }
                else
                {
                    // Compute the relative gap and apply it to list2's starting time
                    int gap = list1[i].Time - list1[i - 1].Time;
                    newTime = result[i - 1].Time + gap;
                }
                // Create a new node with synchronized time and the original value from list2
                int newValue = list2[i].Value;
                result.Add(new CAnimatorNode<int>(newTime, newValue));
            }
            return result;
        }
        public List<CAnimatorNode<float>> Synchronize(List<CAnimatorNode<float>> list1, List<CAnimatorNode<float>> list2)
        {
            if (list2.Count == 0) { return list2; }
            List<CAnimatorNode<float>> result = new List<CAnimatorNode<float>>();
            // Calculate the relative time gaps in list1
            for (int i = 0; i < list1.Count; i++)
            {
                int newTime;
                if (i == 0)
                {
                    // First keyframe matches exactly
                    newTime = list1[i].Time;
                }
                else
                {
                    // Compute the relative gap and apply it to list2's starting time
                    int gap = list1[i].Time - list1[i - 1].Time;
                    newTime = result[i - 1].Time + gap;
                }
                // Create a new node with synchronized time and the original value from list2
                float newValue = list2[i].Value;
                result.Add(new CAnimatorNode<float>(newTime, newValue));
            }
            return result;
        }
        private void changegeosetusedmaterial(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geos = GetSelectedGeosets();
            if (geos.Count == 0)
            {
                return;
            }
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("No materials");
                return;
            }
            List<string> materials = GetMaterialsList();
            Selector s = new Selector(materials);
            s.ShowDialog();
            if (s?.DialogResult == true)
            {
                int index = s.box.SelectedIndex;
                foreach (CGeoset geo in geos)
                {
                    geo.Material.Attach(CurrentModel.Materials[index]);
                }
                CollectTextures();
                RefreshGeosetsList();
            }
        }
        private List<string> GetMaterialsList()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < CurrentModel.Materials.Count; i++)
            {
                string name = "";
                if (CurrentModel.Materials[i].Layers[0].Texture.Object.ReplaceableId == 0)
                {
                    name = CurrentModel.Materials[i].Layers[0].Texture.Object.FileName;
                }
                else
                {
                    name = $"Replaceable ID {CurrentModel.Materials[i].Layers[0].Texture.Object.ReplaceableId}";
                }
                list.Add($"{CurrentModel.Materials[i].ObjectId}: {name}");
            }
            return list; ;
        }
        private void createnode_click(object sender, RoutedEventArgs e)
        {
            createnode cr = new createnode(CurrentModel);
            cr.ShowDialog();
            if (cr.DialogResult == true)
            {
                INode SelectedNode = ListNodes.SelectedItem == null ? null : GetSeletedNode();
                INode _new = null;
                NodeType type = cr.Result;
                string name = cr.ResultName;
                if (type == NodeType.Bone) _new = new CBone(CurrentModel);
                if (type == NodeType.Helper) _new = new CHelper(CurrentModel);
                if (type == NodeType.Attachment) _new = new CAttachment(CurrentModel);
                if (type == NodeType.Ribbon)
                {
                    if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences"); return; }
                    _new = new CRibbonEmitter(CurrentModel);
                }
                if (type == NodeType.Emitter1)
                { if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences"); return; } _new = new CRibbonEmitter(CurrentModel); }
                if (type == NodeType.Emitter2)
                {
                    if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences"); return; }
                    _new = new CParticleEmitter2(CurrentModel); CParticleEmitter2 node = (CParticleEmitter2)_new;
                    node.FilterMode = EParticleEmitter2FilterMode.None;
                }
                if (type == NodeType.Cols) _new = new CCollisionShape(CurrentModel);
                if (type == NodeType.Light) _new = new CLight(CurrentModel);
                if (type == NodeType.Event)
                {
                    edit_eventobject ew = new edit_eventobject(CurrentModel);
                    ew.ShowDialog();
                    if (ew.DialogResult == true) { RefreshNodesTree(); }
                    return;
                }
                _new.Name = name;
                CurrentModel.Nodes.Add(_new);
                if (SelectedNode != null && cr.Check_parent.IsChecked == true)
                {
                    _new.Parent.Attach(SelectedNode);
                }
                RefreshNodesTree();
            }
        }
        private void flattengeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    bool x = ax.Check_x.IsChecked == true;
                    bool y = ax.Check_y.IsChecked == true;
                    bool z = ax.Check_z.IsChecked == true;
                    if (x && y && z) { MessageBox.Show("Only one axis!"); return; }
                    if (x && y) { MessageBox.Show("Only one axis!"); return; }
                    if (x && z) { MessageBox.Show("Only one axis!"); return; }
                    if (y && z) { MessageBox.Show("Only one axis!"); return; }
                    foreach (CGeoset geo in geosets)
                    {
                        if (geo.Vertices.Count > 1)
                        {
                            CVector3 pos1 = geo.Vertices[0].Position;
                            for (int i = 1; i < geo.Vertices.Count; i++)
                            {
                                float px = x ? pos1.X : geo.Vertices[i].Position.X;
                                float py = y ? pos1.Y : geo.Vertices[i].Position.Y;
                                float pz = z ? pos1.Z : geo.Vertices[i].Position.Z;
                                geo.Vertices[i].Position = new CVector3(px, py, pz);
                            }
                        }
                    }
                }
                RefreshViewPort();
            }
        }
        private CSequence AskSequenceName(CSequence except)
        {
            List<string> names = CurrentModel.Sequences.Select(x => x.Name).ToList();
            names.Remove(except.Name);
            Selector s = new Selector(names);
            s.ShowDialog();
            if (s.DialogResult == true)
            {
                return CurrentModel.Sequences.First(x => x.Name == s.Selected);
            }
            return null;
        }
        private void swapnames(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null && CurrentModel.Sequences.Count > 1)
            {
                CSequence selected = GetSelectedSequence();
                CSequence target = AskSequenceName(selected);
                if (target != null)
                {
                    string temp = selected.Name;
                    selected.Name = target.Name;
                    target.Name = temp; // Fix: Assign temp to target.Name
                    RefreshSequencesList();
                }
            }
        }

        private void newsequence_(object sender, RoutedEventArgs e)
        {
            newsequence ns = new newsequence(CurrentModel);
            ns.ShowDialog();
            if (ns.DialogResult == true)
            {
                RefreshSequencesList();
            }
        }
        private void RefreshGlobalSequencesList()
        {
            Report_gsequences.Text = $"{CurrentModel.GlobalSequences.Count} Global sequences";
            ListGSequenes.Items.Clear();
            foreach (var gs in CurrentModel.GlobalSequences)
            {
                ListGSequenes.Items.Add($"{gs.ObjectId}: {gs.Duration}");
            }
        }
        private void Centergeosetsat(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                InputVector v = new InputVector(AllowedValue.Both);
                if (v.ShowDialog() == true)
                {
                    float x = v.X;
                    float y = v.Y;
                    float z = v.Z;
                    foreach (CGeoset geoset in geosets)
                    {
                        Calculator.CenterGeoset(geoset, x, y, z);
                    }
                }
            }
        }
        private void SetUsedLayerTexture(object sender, SelectionChangedEventArgs e)
        {
            if (Pause) { return; }
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                int indx = Combo_LayerUsedTexture.SelectedIndex;
                // MessageBox.Show("Selected " + indx.ToString());
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.Texture.Detach();
                layer.Texture.Attach(CurrentModel.Textures[indx]);
                //  MessageBox.Show($"attaching {CurrentModel.Textures[indx].FileName}");
                if (layer.Texture.Object == null) { MessageBox.Show("Failed"); }
                CollectTextures();
                RefreshViewPort();
            }
        }
        private List<INode> GetNodes(List<string> list)
        {
            List<INode> nodes = new List<INode>();
            foreach (string item in list)
            {
                nodes.Add(CurrentModel.Nodes.First(x => x.Name == item));
            }
            return nodes;
        }
        private void alignnodes(object sender, RoutedEventArgs e)
        {
            if (ListNodes.Items.Count < 2) { return; }
            List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
            Multiselector s = new Multiselector(nodes);
            s.ShowDialog();
            if (s.DialogResult == true)
            {
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    bool x = ax.x;
                    bool y = ax.y;
                    bool z = ax.z;
                    List<INode> nodesList = GetNodes(s.selected);
                    INode first = nodesList[0];
                    for (int i = 1; i < nodesList.Count; i++)
                    {
                        float xp = x ? nodesList[0].PivotPoint.X : nodesList[i].PivotPoint.X;
                        float yp = y ? nodesList[0].PivotPoint.Y : nodesList[i].PivotPoint.Y;
                        float zp = z ? nodesList[0].PivotPoint.Z : nodesList[i].PivotPoint.Z;
                        CVector3 vector = new CVector3(xp, yp, zp);
                        nodesList[i].PivotPoint = vector;
                    }
                }
            }
        }
        private void scalenodes(object sender, RoutedEventArgs e)
        {
            if (ListNodes.Items.Count < 2) { return; }
            List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
            Multiselector s = new Multiselector(nodes);
            s.ShowDialog();
            if (s.DialogResult == true)
            {
                List<INode> nodesList = GetNodes(s.selected);
                InputVector vector = new InputVector(AllowedValue.Both, "Percentage");
                vector.ShowDialog();
                if (vector.DialogResult == true)
                {
                    float x = vector.X; float y = vector.Y; float z = vector.Z;
                    foreach (INode node in nodesList)
                    {
                        float xp = node.PivotPoint.X * x / 100;
                        float yp = node.PivotPoint.Y * y / 100;
                        float zp = node.PivotPoint.Y * z / 100;
                    }
                }
            }
        }
        private void geosetInfobone(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                List<string> indexes = new List<string>();
                bool found = false;
                for (int i = 0; i < CurrentModel.Geosets.Count; i++)
                {
                    CGeoset geo = CurrentModel.Geosets[i];
                    foreach (var group in geo.Groups)
                    {
                        foreach (var gnode in group.Nodes)
                        {
                            if (gnode.Node.ObjectId == node.ObjectId)
                            {
                                indexes.Add(i.ToString());
                                found = true;
                                break;
                            }
                            if (found) { found = false; break; }
                        }
                    }
                }
                if (indexes.Count > 0)
                {
                    MessageBox.Show($"to '{node.Name}' are attached geosets at indexes:\n " + string.Join("\n", indexes.ToArray()));
                }
                else
                {
                    MessageBox.Show($"{node.Name} is free");
                }
            }
        }
        private void shiftnodetranslations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                InputVector vector = new InputVector(AllowedValue.Both, "By");
                vector.ShowDialog();
                if (vector.DialogResult == true)
                {
                    float x = vector.X; float y = vector.Y; float z = vector.Z;
                    for (int i = 0; i < node.Translation.Count; i++)
                    {
                        CAnimatorNode<CVector3> item = node.Translation[i];
                        float xp = item.Value.X + x; float yp = item.Value.Y + y; float zp = item.Value.Z + z;
                        int time = item.Time;
                        item = new CAnimatorNode<CVector3>(time, new CVector3(xp, yp, zp));
                    }
                }
            }
        }
        private void scalekeyframesofnode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                InputVector vector = new InputVector(AllowedValue.Both, "Percentage");
                vector.ShowDialog();
                if (vector.DialogResult == true)
                {
                    float x = vector.X; float y = vector.Y; float z = vector.Z;
                    for (int i = 0; i < node.Translation.Count; i++)
                    {
                        CAnimatorNode<CVector3> item = node.Translation[i];
                        float xp = item.Value.X * x / 100; float yp = item.Value.Y * y / 100; float zp = item.Value.Z * z / 100;
                        int time = item.Time;
                        item = new CAnimatorNode<CVector3>(time, new CVector3(xp, yp, zp));
                    }
                }
            }
        }
        private void CenterAllNodes(object sender, RoutedEventArgs e)
        {
            axis_selector ax = new axis_selector();
            ax.ShowDialog();
            if (ax.DialogResult == true)
            {
                bool x = ax.x;
                bool y = ax.y;
                bool z = ax.z;
                if (CurrentModel.Nodes.Count == 1)
                {
                    float xp = x ? 0 : CurrentModel.Nodes[0].PivotPoint.X;
                    float yp = y ? 0 : CurrentModel.Nodes[0].PivotPoint.Y;
                    float zp = z ? 0 : CurrentModel.Nodes[0].PivotPoint.Z;
                    CurrentModel.Nodes[0].PivotPoint = new CVector3(xp, yp, zp);
                }
                if (CurrentModel.Nodes.Count > 1)
                {
                    CVector3 centroid = Calculator.GetCentroidOfNodes(CurrentModel.Nodes);
                    foreach (var node in CurrentModel.Nodes)
                    {
                        float xp = node.PivotPoint.X - (x ? centroid.X : 0);
                        float yp = node.PivotPoint.Y - (y ? centroid.Y : 0);
                        float zp = node.PivotPoint.Z - (z ? centroid.Z : 0);
                        node.PivotPoint = new CVector3(xp, yp, zp);
                    }
                }
            }
        }
        private void RotateAllNodesCollectively(object sender, RoutedEventArgs e)
        {
            InputVector ax = new InputVector(AllowedValue.Both);
            ax.ShowDialog();
            if (ax.DialogResult == true)
            {
                float rotationX = ax.X;
                float rotationY = ax.Y;
                float rotationZ = ax.Z;
                // Validate rotation degrees
                if (rotationX < -360 || rotationX > 360 ||
                    rotationY < -360 || rotationY > 360 ||
                    rotationZ < -360 || rotationZ > 360)
                {
                    MessageBox.Show("Invalid degrees for rotation. Use values between -360 and 360.");
                    return;
                }
                if (CurrentModel.Nodes.Count > 1)
                {
                    // Get the centroid of the nodes
                    CVector3 centroid = Calculator.GetCentroidOfNodes(CurrentModel.Nodes);
                    foreach (var node in CurrentModel.Nodes)
                    {
                        // Translate PivotPoint to origin (centroid)
                        float xp = node.PivotPoint.X - centroid.X;
                        float yp = node.PivotPoint.Y - centroid.Y;
                        float zp = node.PivotPoint.Z - centroid.Z;
                        // Apply rotations (convert degrees to radians)
                        float radX = rotationX * (float)Math.PI / 180;
                        float radY = rotationY * (float)Math.PI / 180;
                        float radZ = rotationZ * (float)Math.PI / 180;
                        // Rotate around X axis
                        float newY = yp * (float)Math.Cos(radX) - zp * (float)Math.Sin(radX);
                        float newZ = yp * (float)Math.Sin(radX) + zp * (float)Math.Cos(radX);
                        yp = newY;
                        zp = newZ;
                        // Rotate around Y axis
                        float newX = xp * (float)Math.Cos(radY) + zp * (float)Math.Sin(radY);
                        newZ = -xp * (float)Math.Sin(radY) + zp * (float)Math.Cos(radY);
                        xp = newX;
                        zp = newZ;
                        // Rotate around Z axis
                        newX = xp * (float)Math.Cos(radZ) - yp * (float)Math.Sin(radZ);
                        newY = xp * (float)Math.Sin(radZ) + yp * (float)Math.Cos(radZ);
                        xp = newX;
                        yp = newY;
                        // Translate PivotPoint back to its original position
                        xp += centroid.X;
                        yp += centroid.Y;
                        zp += centroid.Z;
                        // Update the node's PivotPoint
                        node.PivotPoint = new CVector3(xp, yp, zp);
                    }
                }
            }
        }
        private void reversenodekeyframesrotations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                for (int i = 0; i < node.Rotation.Count; i++)
                {
                    node.Rotation[i] = Calculator.ReverseVector4(node.Rotation[i]);
                }
            }
        }
        private void ChangedInspector(object sender, SelectionChangedEventArgs e)
        {
            if (TC_Inspector.SelectedIndex == 0)
            {
                showinfo();
            }
            if (TC_Inspector.SelectedIndex == 1)
            {
                ShowErrors();
            }
        }
        private void ChangedTabEvent(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl tc && tc == ListOptions)
            {
                if (ListOptions.SelectedIndex == 5)
                {
                    ChangedInspector(null, null);
                }

                e.Handled = true;
            }
        }

        private bool CheckSkinning()
        {

            foreach (var geoset in CurrentModel.Geosets)
            {
                if (geoset.Vertices.Count != geoset.Groups.Count)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < geoset.Vertices.Count; i++)
                    {
                        if (geoset.Vertices[i].Group.Object != geoset.Groups[i]) { return false; }
                    }
                }

            }
            return true;
        }

        private void RefreshGeosetsInUVMapper()
        {
            List_UV_Geosets.Items.Clear();
            foreach (var geoset in CurrentModel.Geosets)
            {
                List_UV_Geosets.Items.Add(new ListBoxItem() { Content = geoset.ObjectId });
            }
        }



        private void RefreshGeosetsListRigging()
        {
            ListGeosetsRiggings.Children.Clear();
            foreach (var geoset in CurrentModel.Geosets)
            {
                CheckBox c = new CheckBox();
                c.Content = geoset.ObjectId.ToString();
                // unfinished
                ListGeosetsRiggings.Children.Add(c);
            }

        }

        private void RefreshBonesInRigging()
        {
            ListBonesRiggings.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CBone) ListBonesRiggings.Items.Add(new ListBoxItem() { Content = node.Name });
            }
        }

        private void ShowErrors()
        {
            ErrorChecker.CurrentModel = CurrentModel;
            Box_Errors.Text = ErrorChecker.Inspect(CurrentModel);
        }
        private void editvisibilitiesofgeoset(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences"); return; }
            if (ListGeosets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            List<CGeoset> geosets = GetSelectedGeosets();
            editvisibilities_window ew = new editvisibilities_window(CurrentModel, geosets[0]);
            ew.ShowDialog();
            if (ew.DialogResult == true)
            {
                RefreshGeosetAnimationsList();
            }
        }
        private void createprimitiveshape(object sender, RoutedEventArgs e)
        {
            //  MenuPrimitiveShape.ContextMenu.IsEnabled = true;
        }
        private void movelayerup(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                int index = List_Layers.SelectedIndex;
                if (index == 0) { return; }
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[index];
                var temp = mat.Layers[index - 1];
                mat.Layers[index] = mat.Layers[index - 1];
                mat.Layers[index] = temp;
                RefreshLayersList();
            }
        }
        private void movelayerdown(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                int index = List_Layers.SelectedIndex;
                CMaterial mat = GetSelectedMAterial();
                if (index == mat.Layers.Count - 1) { return; }
                CMaterialLayer layer = mat.Layers[index];
                var temp = mat.Layers[index + 1];
                mat.Layers[index + 1] = mat.Layers[index];
                mat.Layers[index] = temp;
                RefreshLayersList();
            }
        }
        private void createnewlayer(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Textures.Count == 0) { MessageBox.Show("There are no textures"); return; }
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                layer.Alpha.MakeStatic(1);
                layer.Texture.Attach(CurrentModel.Textures[0]);
                mat.Layers.Add(layer);
                RefreshLayersList();
            }
        }
        private void opentargetfolder(object sender, RoutedEventArgs e)
        {
            if (File.Exists(CurrentSaveLocaiton))
            {
                OpenFileLocation(CurrentSaveLocaiton);
            }
        }
        static void OpenFileLocation(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("File path is null or empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(filePath))
            {
                MessageBox.Show("The specified file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string folderPath = System.IO.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Unable to determine the folder path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                Process.Start("explorer.exe", $"\"{folderPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CreateMAterial(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Textures.Count == 0) { MessageBox.Show("No textures"); return; }
            CMaterial mat = new CMaterial(CurrentModel);
            CMaterialLayer later = new CMaterialLayer(CurrentModel);
            later.Texture.Attach(CurrentModel.Textures[0]);
            mat.Layers.Add(later);
            CurrentModel.Materials.Add(mat);
            RefreshMaterialsList();
        }
        private void createTC(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Textures.Any(x => x.ReplaceableId == 1) == false)
            {
                CTexture textue = new CTexture(CurrentModel);
                textue.ReplaceableId = 1;
                CurrentModel.Textures.Add(textue);
                RefreshTextures();
                RefreshLayersTextureList();
            }
            else { MessageBox.Show("There is already TC Texture"); }
        }
        private void createTG(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Textures.Any(x => x.ReplaceableId == 2) == false)
            {
                CTexture textue = new CTexture(CurrentModel);
                textue.ReplaceableId = 2;
                CurrentModel.Textures.Add(textue);
                RefreshTextures();
            }
            else { MessageBox.Show("There is already TG Texture"); }
        }


        private void rendergroundplane(object sender, RoutedEventArgs e)
        {
            RefreshViewPort();
        }
        private void ga_editalphas(object sender, RoutedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                transformation_editor tr = new transformation_editor(CurrentModel, ga.Alpha, true, TransformationType.Alpha);
                tr.ShowDialog();
            }
        }
        private void ga_editcolors(object sender, RoutedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                transformation_editor tr = new transformation_editor(CurrentModel, ga.Color, true, TransformationType.Color);
                tr.ShowDialog();
            }
        }
        private void editnodetranslations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_editor tr = new transformation_editor(CurrentModel, node.Translation, false, TransformationType.Translation);
                tr.ShowDialog();
            }
        }
        private void editnoderotations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_editor tr = new transformation_editor(CurrentModel, node.Rotation, false);
                tr.ShowDialog();
            }
        }
        private void editnodescalings(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_editor tr = new transformation_editor(CurrentModel, node.Scaling, false, TransformationType.Scaling);
                tr.ShowDialog();
            }
        }
        private void editlayeralpha(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                transformation_editor tr = new transformation_editor(CurrentModel, layer.Alpha, true, TransformationType.Alpha);
                tr.ShowDialog();
            }
        }
        private void editlayertextureid(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                transformation_editor tr = new transformation_editor(CurrentModel, layer.TextureId, false);
                tr.ShowDialog();
            }
        }
        private void ta_edit_tr(object sender, RoutedEventArgs e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = GetSelectedTextureAnim();
                transformation_editor tr = new transformation_editor(CurrentModel, ta.Translation, false, TransformationType.Translation);
                tr.ShowDialog();
            }
        }
        private CTextureAnimation GetSelectedTextureAnim()
        {
            return CurrentModel.TextureAnimations[List_TextureAnims.SelectedIndex];
        }
        private void ta_edit_ro(object sender, RoutedEventArgs e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = GetSelectedTextureAnim();
                transformation_editor tr = new transformation_editor(CurrentModel, ta.Rotation, false);
                tr.ShowDialog();
            }
        }
        private void ta_edit_scaling(object sender, RoutedEventArgs e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = GetSelectedTextureAnim();
                transformation_editor tr = new transformation_editor(CurrentModel, ta.Scaling, false, TransformationType.Scaling);
                tr.ShowDialog();
            }
        }
        private void EditNodeData(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                if (node is CAttachment attachment)
                {
                    window_EditAttachment ea = new window_EditAttachment(node, CurrentModel);
                    ea.ShowDialog(); return;
                }
                if (node is CBone bone)
                {
                    window_editbone_data edit = new window_editbone_data(bone, CurrentModel);
                    edit.ShowDialog(); return;
                }
                if (node is CCollisionShape cols)
                {
                    window_edit_cols edit = new window_edit_cols(cols, CurrentModel);
                    edit.ShowDialog();
                    if (RenderCollisionShapes) { RefreshViewPort(); }
                    return;
                }
                if (node is CParticleEmitter emitter1)
                {
                    edit_emitter1 ei = new edit_emitter1(emitter1, CurrentModel);
                    ei.ShowDialog(); return;
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    edit_emitter2 e2 = new edit_emitter2(emitter2, CurrentModel);
                    e2.ShowDialog();
                    return;
                }
                if (node is CRibbonEmitter ribbon)
                {
                    edit_ribbon er = new edit_ribbon(CurrentModel, ribbon);
                    er.ShowDialog();
                    return;
                }
                if (node is CLight light)
                {
                    Edit_light el = new Edit_light(light, CurrentModel);
                    el.ShowDialog();
                    return;
                }
                if (node is CEvent ev)
                {
                    edit_eventobject edit = new edit_eventobject(CurrentModel, ev);
                    edit.ShowDialog();
                    if (edit.DialogResult == true)
                    {
                        ChangeNameOfSelectedNode(ev.Name);
                    }
                }
            }
        }
        private void subdivide(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (var geoset in geosets)
                {
                    Calculator.SubdivideGeoset(geoset, CurrentModel);
                }
            }
            RefreshGeosetsList();
        }
        private void ChangeNameOfSelectedNode(string name)
        {
            TreeViewItem item = ListNodes.SelectedItem as TreeViewItem;
            StackPanel panel = item.Header as StackPanel;
            TextBlock t = panel.Children[1] as TextBlock;
            t.Text = name;
        }
        private void explain(object sender, RoutedEventArgs e)
        {
            howmodel h = new howmodel();
            h.ShowDialog();
        }
        private void Checked_Billlboarded(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.Billboarded = Check_Billlboarded.IsChecked == true;
            }
        }
        private void Checked_Billlboardedx(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.BillboardedLockX = Check_Billlboardedx.IsChecked == true;
            }
        }
        private void Checked_Billlboardedy(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.BillboardedLockY = Check_Billlboardedy.IsChecked == true;
            }
        }
        private void Checked_Billlboardedz(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.BillboardedLockZ = Check_Billlboardedz.IsChecked == true;
            }
        }
        private void Checked_cameraAnchored(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.CameraAnchored = Check_cameraAnchored.IsChecked == true;
            }
        }
        private void Checked_dontInhRotation(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.DontInheritRotation = Check_dontInhRotation.IsChecked == true;
            }
        }
        private void Checked_DontinhScaling(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.DontInheritScaling = Check_cameraAnchored.IsChecked == true;
            }
        }
        private void Checked_dontInhTranslation(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.DontInheritTranslation = Check_dontInhTranslation.IsChecked == true;
            }
        }
        private void SetUsedLayerTextureAnim(object sender, SelectionChangedEventArgs e)
        {
            if (Combo_LayerUsedTextureAnim.SelectedIndex == -1) { return; }
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                int index = List_Layers.SelectedIndex;
                int comboIndex = Combo_LayerUsedTextureAnim.SelectedIndex;
                if (comboIndex == 0)
                {
                    mat.Layers[index].TextureAnimation.Detach();
                }
                else
                {
                    mat.Layers[index].TextureAnimation.Attach(CurrentModel.TextureAnimations[comboIndex - 1]);
                }
            }
        }
        private void createta(object sender, RoutedEventArgs e)
        {
            CTextureAnimation ta = new CTextureAnimation(CurrentModel);
            CurrentModel.TextureAnimations.Add(ta);
            RefreshTextureAnims();
            RefreshLayersTextureAnimList();
        }
        private bool _isRotating = false; // Track if rotation is active
        private Point _lastMousePosition; // Store the last mouse position
        private void RotateMoved(object sender, MouseEventArgs e)
        {
            if (!_isRotating)
                return;
            // Get the current mouse position
            Point currentMousePosition = e.GetPosition((UIElement)sender);
            // Calculate the change in horizontal position
            double deltaX = currentMousePosition.X - _lastMousePosition.X;
            // Update the rotation angle based on movement
            CurrentAngle += (float)(deltaX * 0.5); // Adjust sensitivity as needed
            // Update the last mouse position
            _lastMousePosition = currentMousePosition;
            // Apply the rotation
            AdjustRotation(CurrentAngle);
        }
        private void RotateOn(object sender, MouseButtonEventArgs e)
        {
            _isRotating = true;
            // Capture the mouse for consistent input
            UIElement element = (UIElement)sender;
            element.CaptureMouse();
            // Store the initial mouse position
            _lastMousePosition = e.GetPosition(element);
        }
        private void RotateOff(object sender, MouseButtonEventArgs e)
        {
            _isRotating = false;
            // Release the mouse capture
            UIElement element = (UIElement)sender;
            element.ReleaseMouseCapture();
        }
        private void SetLayerFilterMode(object sender, SelectionChangedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                int index = List_Layers.SelectedIndex;
                mat.Layers[index].FilterMode = (EMaterialLayerFilterMode)Combo_FilterModeLayer.SelectedIndex;
            }
        }
        private void scr(object sender, RoutedEventArgs e)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            string folder = System.IO.Path.Combine(dir, "Screenshots");
            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filename = System.IO.Path.Combine(folder, "SCreenshot_" + DateTime.Now.ToString("dd mm yyyy hh mm ss") + ".png");
            CaptureScreenshot(App_window, filename);
        }
        public static void CaptureScreenshot(UIElement visual, string filePath)
        {
            // Get the dimensions of the visual
            double width = visual.RenderSize.Width;
            double height = visual.RenderSize.Height;
            // Render the visual to a RenderTargetBitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);
            visual.Measure(new Size(width, height));
            visual.Arrange(new Rect(new Size(width, height)));
            renderBitmap.Render(visual);
            // Encode the bitmap as a PNG
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            // Save to the specified file path
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(filePath));
        }
        private void scr2(object sender, RoutedEventArgs e)
        {
            if (ListOptions.SelectedIndex != 1) { MessageBox.Show("Select geosets tab"); return; }
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            string folder = System.IO.Path.Combine(dir, "Screenshots");
            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filename = System.IO.Path.Combine(folder, "SCreenshot_" + DateTime.Now.ToString("dd mm yyyy hh mm ss") + ".png");
            CaptureScreenshot(Scene_Viewport, filename);
        }
        private void findTexture(object sender, RoutedEventArgs e)
        {
            TextureFinder.ShowDialog();
        }

        private void ToggleTextures(object sender, RoutedEventArgs e)
        {
            if (Scene_Viewport == null) { return; }
            RenderTextures = !RenderTextures;
            RefreshViewPort();
        }
        private void createcube(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            CGeoset cube = Calculator.CreateCube(CurrentModel);
            cube.Material.Attach(CurrentModel.Materials[0]);
            CreateBoneForGeoset(cube);
            CurrentModel.Geosets.Add(cube);
            RefreshGeosetsList();
            CollectTextures();
            RefreshGeosetsList();
            RefreshNodesTree();
            RefreshViewPort();
        }
        private void CreateBoneForGeoset(CGeoset geoset)
        {
            CBone bone = new CBone(CurrentModel);
            bone.Name = "CubeBone_" + IDCounter.Next_();
            CGeosetGroup group = new CGeosetGroup(CurrentModel);
            CGeosetGroupNode node = new CGeosetGroupNode(CurrentModel);
            node.Node.Attach(bone);
            group.Nodes.Add(node);
            geoset.Groups.Add(group);
            CurrentModel.Nodes.Add(bone);
        }
        private void createcyllinder(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            Input i = new Input("Sides");
            i.ShowDialog();
            bool parse = int.TryParse(i.Result, out int sides);
            if (!parse) { MessageBox.Show("Inout not and integer"); return; }
            else
            {
                if (sides < 3) { MessageBox.Show("Sides cannot be less than 3"); return; }
                CGeoset cyllinder = Calculator.CreateCylinder(CurrentModel, 1, 2, sides);
                CreateBoneForGeoset(cyllinder);
                CurrentModel.Geosets.Add(cyllinder);
                cyllinder.Material.Attach(CurrentModel.Materials[0]);
                CollectTextures();
                RefreshGeosetsList();
                RefreshNodesTree();
                RefreshViewPort();
            }
        }
        private void createsphere(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
        }
        private void createcone(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            Input i = new Input("Sides");
            i.ShowDialog();
            bool parse = int.TryParse(i.Result, out int sides);
            if (!parse) { MessageBox.Show("Inout not and integer"); return; }
            else
            {
                if (sides < 3) { MessageBox.Show("Sides cannot be less than 3"); return; }
                CGeoset cone = Calculator.CreateCone(CurrentModel, 1, 2, sides);
                CreateBoneForGeoset(cone);
                cone.Material.Attach(CurrentModel.Materials[0]);
                CurrentModel.Geosets.Add(cone);
                CollectTextures();
                RefreshGeosetsList();
                RefreshNodesTree();
                RefreshViewPort();
            }
        }
        private void DeleteGeosetAnimationOf(CGeoset geoset)
        {
            foreach (CGeosetAnimation a in CurrentModel.GeosetAnimations.ToList())
            {
                if (a.Geoset.Object == geoset)
                {
                    CurrentModel.GeosetAnimations.Remove(a);
                }
            }
        }
        private void FragmentTrianglesIntoGeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                Pause = true;
                List<CGeoset> geosets = GetSelectedGeosets();
                List<CGeoset> fragments = new List<CGeoset>();
                foreach (CGeoset geoset in geosets)
                {
                    fragments.AddRange(Calculator.Fragment(geoset, CurrentModel));
                    DeleteGeosetAnimationOf(geoset);
                    CurrentModel.Geosets.Remove(geoset);
                }
                foreach (CGeoset geoset in fragments)
                {
                    CurrentModel.Geosets.Add(geoset);
                }
                Pause = false;
                RefreshAll();
            }
        }
        private void WeldAll(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.WeldAll(geoset, CurrentModel);
                }
                RefreshGeosetsList();
            }
        }


        private void flattengeosetsside(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                FlattenSide fs = new FlattenSide();
                fs.ShowDialog();
                if (fs.DialogResult == true)
                {
                    Side side = fs.side;
                    foreach (CGeoset geoset in geosets)
                    {
                        Calculator.FlattenSide(geoset, side);
                    }
                }
                RefreshViewPort();
            }
        }
        private void Simplify(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.Simplify(geoset, CurrentModel);
                }
            }
        }


        private void showgm(object sender, RoutedEventArgs e)
        {
            ButtonGeosetOptions.ContextMenu.IsOpen = true;
        }
        private void showgam(object sender, RoutedEventArgs e)
        {
            Buttongam.ContextMenu.IsOpen = true;
        }
        private void SelectedGS(object sender, SelectionChangedEventArgs e)
        {
            if (ListGSequenes.SelectedItem != null)
            {
                CGlobalSequence gs = CurrentModel.GlobalSequences[ListGSequenes.SelectedIndex];
                InputGSDuration.Text = gs.Duration.ToString();
                StringBuilder sb = new StringBuilder();
                foreach (INode node in CurrentModel.Nodes)
                {
                    if (
                        node.Translation.GlobalSequence.Object == gs ||
                        node.Rotation.GlobalSequence.Object == gs ||
                        node.Scaling.GlobalSequence.Object == gs)
                    {
                        sb.AppendLine(node.Name);
                    }
                }
            }
        }
        private void addgs(object sender, RoutedEventArgs e)
        {
            bool parse = int.TryParse(InputGSDuration.Text, out int duration);
            if (!parse) { MessageBox.Show("Expected integer"); return; }
            else
            {
                if (duration<= 0) { MessageBox.Show("Expected positive integer");return; }
                if (CurrentModel.GlobalSequences.Any(x => x.Duration == duration))
                {
                    MessageBox.Show("There is already a global sequence with this duration"); return;
                }
                CGlobalSequence gs = new CGlobalSequence(CurrentModel);
                gs.Duration = duration;
                CurrentModel.GlobalSequences.Add(gs);
                RefreshGlobalSequencesList();
            }
        }
        private void editgs(object sender, RoutedEventArgs e)
        {
            if (ListGSequenes.SelectedItem != null)
            {
                CGlobalSequence gs = new CGlobalSequence(CurrentModel);
                bool parse = int.TryParse(InputGSDuration.Text, out int duration);
                if (!parse) { MessageBox.Show("Expected integer"); return; }
                else
                {
                    gs.Duration = duration;
                    RefreshGlobalSequencesList();
                }
            }
        }
        private void delgs(object sender, RoutedEventArgs e)
        {
            if (ListGSequenes.SelectedItem != null)
            {
                CGlobalSequence gs = CurrentModel.GlobalSequences[ListGSequenes.SelectedIndex];
                if (gs.HasReferences)
                {
                    if (askDelete())
                    {
                        CurrentModel.GlobalSequences.RemoveAt(ListGSequenes.SelectedIndex);
                        RefreshGlobalSequencesList();
                    }
                }
            }
        }
        private bool askDelete()
        {
            // Show a MessageBox with Yes/No options
            MessageBoxResult result = MessageBox.Show(
                "This object is referenced. Still delete?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            // Return true if the user clicked Yes, false otherwise
            return result == MessageBoxResult.Yes;
        }

        private void negateusofgeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        float X = vertex.TexturePosition.X;
                        float Y = vertex.TexturePosition.Y;
                        X = -X;
                        vertex.TexturePosition = new CVector2(X, Y);
                    }
                }
            }
            RefreshViewPort();
        }
        private void negavsofgeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        float X = vertex.TexturePosition.X;
                        float Y = vertex.TexturePosition.Y;
                        Y = -Y;
                        vertex.TexturePosition = new CVector2(X, Y);
                    }
                }
            }
            RefreshViewPort();
        }
        private void fituvgeoet(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    if (geoset.Triangles.Count != 2) { continue; }
                    geoset.Vertices[0].TexturePosition = new CVector2(0, 0);
                    geoset.Vertices[1].TexturePosition = new CVector2(0, 1);
                    geoset.Vertices[2].TexturePosition = new CVector2(1, 0);
                    geoset.Vertices[3].TexturePosition = new CVector2(1, 1);
                }
            }
            RefreshViewPort();
        }
        private void swapusvsgeosets(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        float X = vertex.TexturePosition.X;
                        float Y = vertex.TexturePosition.Y;
                        float temp = X;
                        X = Y;
                        Y = temp;
                        vertex.TexturePosition = new CVector2(X, Y);
                    }
                }
            }
            RefreshViewPort();
        }
        private void gotogeosettexture(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                int index = CurrentModel.Materials.IndexOf(geosets[0].Material.Object);
                List_MAterials.SelectedIndex = index;
                ListOptions.SelectedIndex = 4;
            }
            else
            {
                MessageBox.Show("Select a single geoset");
            }
        }
        private void gotogeosetbone(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) { return; }
            List<CGeoset> geosets = GetSelectedGeosets();
            CBone AttachedToBone = null;
            bool same = true;
            foreach (CGeoset geoset in geosets)
            {
                foreach (CGeosetVertex vertex in geoset.Vertices)
                {
                    INode attachedToNode = vertex.Group.Object.Nodes[0].Node.Node;
                    if (AttachedToBone != null)
                    {
                        if (attachedToNode != AttachedToBone)
                        {
                            same = false;
                        }
                    }
                    else
                    {
                        AttachedToBone = (CBone)attachedToNode;
                    }
                }
                end:
                if (same)
                {
                    string name = AttachedToBone.Name;
                    SelectNodeByName(name);
                }
            }
        }
        private void SelectNodeByName(string name)
        {
            foreach (var item in ListNodes.Items)
            {
                if (item is TreeViewItem node)
                {
                    if (SelectNodeByNameRecursive(node, name))
                        break; // Exit once the desired node is found and selected
                }
            }
            ListOptions.SelectedIndex = 3;
        }
        private bool SelectNodeByNameRecursive(TreeViewItem node, string name)
        {
            // Ensure the node's header is correctly structured
            if (node.Header is StackPanel container)
            {
                TextBlock nameContainer = container.Children.OfType<TextBlock>().FirstOrDefault();
                if (nameContainer != null && nameContainer.Text == name)
                {
                    node.IsSelected = true;
                    node.IsExpanded = true; // Expand the node
                    node.BringIntoView(); // Ensures the node is visible
                    return true; // Node found and selected
                }
            }
            // Check nested nodes
            foreach (var child in node.Items)
            {
                if (child is TreeViewItem childNode)
                {
                    // Recursively check child nodes
                    if (SelectNodeByNameRecursive(childNode, name))
                    {
                        node.IsExpanded = true; // Ensure the parent node is expanded
                        return true; // Stop once the node is found
                    }
                }
            }
            return false; // Node not found in this branch
        }
        private void negatenormals(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (var geoset in geosets)
                {
                    foreach (var vertex in geoset.Vertices)
                    {
                        float x = vertex.Normal.X;
                        float y = vertex.Normal.Y;
                        float z = vertex.Normal.Z;
                        vertex.Normal = new CVector3(-x, -y, -z);
                    }
                }
            }
        }
        private void makegassameasga(object sender, RoutedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                foreach (CGeosetAnimation anim in CurrentModel.GeosetAnimations)
                {
                    if (anim == ga) { continue; }
                    anim.UseColor = ga.UseColor;
                    anim.DropShadow = ga.DropShadow;
                    if (ga.Alpha.Static)
                    {
                        anim.Alpha.MakeStatic(ga.Alpha.GetValue());
                    }
                    else
                    {
                        anim.Alpha.Clear();
                        foreach (var item in ga.Alpha)
                        {
                            anim.Alpha.Add(new CAnimatorNode<float>(item.Time, item.Value));
                        }
                    }
                    if (ga.Color.Static)
                    {
                        anim.Color.MakeStatic(new CVector3(ga.Color.GetValue()));
                    }
                    else
                    {
                        foreach (var item in ga.Color)
                        {
                            anim.Color.Add(new CAnimatorNode<CVector3>(item.Time, new CVector3(item.Value)));
                        }
                    }
                }
                RefreshGeosetAnimationsList();
            }
        }
        private void ImportSequences(object sender, RoutedEventArgs e)
        {
            CModel TemporaryModel = GetTemporaryModel();
            if (TemporaryModel == null) { return; }
            CurrentModel.Sequences.Clear();
            List<CSequence> importedSequences = new List<CSequence>();
            foreach (CSequence sequence in TemporaryModel.Sequences)
            {
                CSequence imported = new CSequence(CurrentModel);
                imported.Name = sequence.Name;
                imported.IntervalStart = sequence.IntervalStart;
                imported.IntervalEnd = sequence.IntervalEnd;
                imported.NonLooping = sequence.NonLooping;
                imported.MoveSpeed = sequence.MoveSpeed;
                imported.Rarity = sequence.Rarity;
                importedSequences.Add(imported);
            }
            importedSequences = importedSequences.OrderBy(x => x.IntervalStart).ToList();
            foreach (CSequence sequence in importedSequences)
            {
                CurrentModel.Sequences.Add(sequence);
            }
            RefreshSequencesList();
        }
        private void Box_Errors_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
        }
        private void reportwbaaits(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequence();
                List<string> names = new List<string>();
                foreach (INode node in CurrentModel.Nodes)
                {
                    if (node.Translation.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd))
                    {
                        names.Add($"The {node.GetType().Name} '{node.Name}'"); continue; ;
                    }
                    if (node.Rotation.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd))
                    {
                        names.Add($"The {node.GetType().Name} '{node.Name}'"); continue; ;
                    }
                    if (node.Scaling.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd))
                    {
                        names.Add($"The {node.GetType().Name} '{node.Name}'"); continue; ;
                    }
                }
                if (names.Count == 0)
                {
                    MessageBox.Show("No nodes are animated in this sequence"); return;
                }
                else
                {
                }
                MessageBox.Show($"These nodes are animated in the sequence '{sequence.Name}':\n\n" + string.Join("\n", names));
            }
        }
        private void Checked_WW(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                texture.WrapWidth = Check_WW.IsChecked == true;
            }
        }
        private void Checked_WH(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                texture.WrapHeight = Check_WH.IsChecked == true;
            }
        }
        private void SelectedTexture(object sender, SelectionChangedEventArgs e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                Check_WH.IsChecked = texture.WrapHeight;
                Check_WW.IsChecked = texture.WrapWidth;
            }
        }
        private void SelectedSequence(object sender, SelectionChangedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequence();
                StringBuilder sb = new StringBuilder();
                foreach (INode node in CurrentModel.Nodes)
                {
                    if (
                        node.Translation.ContainsSequence(sequence) ||
                        node.Rotation.ContainsSequence(sequence) ||
                        node.Scaling.ContainsSequence(sequence)
                        )
                    {
                        sb.AppendLine(node.Name);
                    }
                }
                Report_Sequence_UsedNodes.Text = sb.ToString();
            }
        }
        private void Hotkeys(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.S) save(null, null);
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && e.Key == Key.S) saveas(null, null);
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.O) load(null, null);
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.R) reload(null, null);
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.F) refreshalllists(null, null);
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.T)
            {
                bool c = Menuitem_Textured.IsChecked == true;
                Menuitem_Textured.IsChecked = !c;
            }
        }
        private void explain2(object sender, RoutedEventArgs e)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("For rotations, this app uses euler(x, y, z) instead of quaternion (x, y, z, w).");
            messageBuilder.AppendLine("For scalings, this app uses 0+ percentage instead of normalized percentage.");
            messageBuilder.AppendLine("For alphas, this app uses 0-100 percentage instead of normalized percentage.");
            messageBuilder.AppendLine("For colors, this app uses standard RGB instead of normalized reversed RGB.");
            messageBuilder.AppendLine("For visibility, this app uses 'visible' and 'invisible' instead of 1 and 0.");
            messageBuilder.AppendLine("Int and float transformations remain the same.");
            MessageBox.Show(messageBuilder.ToString());
        }
        private void CreatePresets(object sender, RoutedEventArgs e)
        {
            presets p = new presets(CurrentModel);
            p.ShowDialog();
            if (p.DialogResult == true)
            {
                RefreshSequencesList();
            }
        }
        private void removesequencesfromnodetransfromations(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                if (CurrentModel.Sequences.Count == 0)
                {
                    MessageBox.Show("There are no sequences"); return;
                }
                INode node = GetSeletedNode();
                transformation_selector ts = new transformation_selector();
                ts.ShowDialog();
                if (ts.DialogResult == true)
                {
                    bool t = ts.C1.IsChecked == true;
                    bool r = ts.C2.IsChecked == true;
                    bool s = ts.C3.IsChecked == true;
                    if (!t && !r && !s) { MessageBox.Show("Select at least one"); return; }
                    List<string> sequencesNames = CurrentModel.Sequences.Select(X => X.Name).ToList();
                    Selector sl = new Selector(sequencesNames, "Sequences", true);
                    if (sl.DialogResult == true)
                    {
                        List<CSequence> sequences = GetSequencesFromStringList(sl.SelectedList);
                        foreach (CSequence sequence in sequences)
                        {
                            if (t) node.Translation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
                            if (r) node.Rotation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
                            if (s) node.Scaling.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
                        }
                    }
                }
            }
        }
        public List<CSequence> GetSequencesFromStringList(List<string> names)
        {
            List<CSequence> list = new List<CSequence>();
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                if (names.Contains(sequence.Name)) { list.Add(sequence); }
            }
            return list;
        }
        private void duplicatenode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                INode clone = NodeCloner.Clone(node, CurrentModel);
                clone.Name = clone.Name = "_" + IDCounter.Next_();
                CurrentModel.Nodes.Add(clone);
                RefreshNodesTree();
            }
        }
        private void resetkeyframes(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_selector ts = new transformation_selector();
                ts.ShowDialog();
                if (ts.DialogResult == true)
                {
                    bool t = ts.C1.IsChecked == true;
                    bool r = ts.C2.IsChecked == true;
                    bool s = ts.C3.IsChecked == true;
                    if (t)
                    {
                        for (int i = 0; i < node.Translation.Count; i++)
                        {
                            var item = node.Translation[i];
                            item = new CAnimatorNode<CVector3>(item.Time, new CVector3(0, 0, 0));
                        }
                    }
                    if (r)
                    {
                        for (int i = 0; i < node.Rotation.Count; i++)
                        {
                            var item = node.Rotation[i];
                            item = new CAnimatorNode<CVector4>(item.Time, new CVector4(0, 0, 0, 0));
                        }
                    }
                    if (s)
                    {
                        for (int i = 0; i < node.Scaling.Count; i++)
                        {
                            var item = node.Scaling[i];
                            item = new CAnimatorNode<CVector3>(item.Time, new CVector3(0, 0, 0));
                        }
                    }
                }
            }
        }
        private void nodepresets_click(object sender, RoutedEventArgs e)
        {
        }
        private void createpixies(object sender, RoutedEventArgs e)
        {
            INode cloned = NodeCloner.Clone(NodeCollection.ItemPixie, CurrentModel);
            HandleRequiredTexture((CParticleEmitter2)cloned);
            cloned.Name = "ItemPixies_" + IDCounter.Next_(); ;
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
        }
        private void createdust(object sender, RoutedEventArgs e)
        {
            INode cloned = NodeCloner.Clone(NodeCollection.Dust, CurrentModel);
            cloned.Name = "Dust_" + IDCounter.Next_(); ;
            HandleRequiredTexture((CParticleEmitter2)cloned);
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
        }
        private void HandleRequiredTexture(CParticleEmitter2 cloned)
        {
            if (cloned.RequiredTexturePath.Length > 0)
            {

                if (CurrentModel.Textures.Any(x => x.FileName == cloned.RequiredTexturePath))
                {
                    CTexture texture = CurrentModel.Textures.First(x => x.FileName == cloned.RequiredTexturePath);
                    cloned.Texture.Attach(texture);

                    RefreshTextures();
                }
                else
                {
                    CTexture _new = new CTexture(CurrentModel);
                    _new.FileName = cloned.RequiredTexturePath;
                    CurrentModel.Textures.Add(_new);
                    RefreshTextures();
                    cloned.Texture.Attach(_new);
                }
            }
        }
        private void createsmoke(object sender, RoutedEventArgs e)
        {
            INode cloned = NodeCloner.Clone(NodeCollection.Smoke, CurrentModel);
            HandleRequiredTexture((CParticleEmitter2)cloned);
            cloned.Name = "Smoke_" + IDCounter.Next_(); ;
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
        }
        public static void FilterFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist.");
                return;
            }
            try
            {
                // Read all bytes from the file
                byte[] fileBytes = File.ReadAllBytes(filePath);
                // Define valid characters as a string
                string validCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\t\n\r !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
                // Convert the valid characters to a HashSet of bytes for fast lookup
                var validBytes = new HashSet<byte>(validCharacters.Select(c => (byte)c));
                // Filter the bytes and replace '\r' with '\n'
                byte[] filteredBytes = fileBytes
                    .Where(b => validBytes.Contains(b))
                    .Select(b => b == (byte)'\r' ? (byte)'\n' : b)
                    .ToArray();
                // Write the filtered bytes back to the file
                File.WriteAllBytes(filePath, filteredBytes);
                Console.WriteLine("File has been filtered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private void cameras(object sender, RoutedEventArgs e)
        {
            cameraManager cm = new cameraManager(CurrentModel);
            cm.ShowDialog();
        }
        private void findnode(object sender, RoutedEventArgs e)
        {
            Input i = new Input("");
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                string quiry = i.Result;
                if (CurrentModel.Nodes.Any(x => x.Name.ToLower().Contains(quiry)))
                {
                    string FullName = CurrentModel.Nodes.First(x => x.Name.ToLower().Contains(quiry)).Name;
                    SelectNodeByName(FullName);
                }
            }
        }
        private void Checked_MatSort2(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.SortPrimitivesNearZ = Check_MatSort2.IsChecked == true;
            }
        }
        private void reattachallToBone(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Nodes.Count(x => x is CBone) == 0)
            {
                MessageBox.Show("There are no bones"); return;
            }
            List<string> bones = CurrentModel.Nodes.Where(x => x is CBone).Select(x => x.Name).ToList();
            Selector s = new Selector(bones);
            s.ShowDialog();
            if (s.DialogResult == true)
            {
                CBone bone = CurrentModel.Nodes.First(x => x.Name == s.Selected) as CBone;
                if (bone != null)
                {
                    int id = bone.ObjectId;
                    foreach (CGeoset geo in CurrentModel.Geosets)
                    {
                        geo.Groups.Clear();
                        CGeosetGroup group = new CGeosetGroup(CurrentModel);
                        CGeosetGroupNode node = new CGeosetGroupNode(CurrentModel);
                        node.Node.Attach(bone);
                        group.Nodes.Add(node);
                        geo.Groups.Add(group);
                        foreach (CGeosetVertex vertex in geo.Vertices)
                        {
                            vertex.Group.Attach(group);
                        }
                    }
                }
            }
        }
        private void delallgas(object sender, RoutedEventArgs e)
        {
            CurrentModel.GeosetAnimations.Clear();
            List_GeosetAnims.Items.Clear();
            Label_GAs.Text = "0 Geoset Animations";
        }
        private void createmissinggas(object sender, RoutedEventArgs e)
        {
            foreach (CGeoset geoset in CurrentModel.Geosets)
            {
                if (CurrentModel.GeosetAnimations.Any(x => x.Geoset.Object == geoset) == false)
                {
                    CGeosetAnimation _new = new CGeosetAnimation(CurrentModel);
                    _new.Geoset.Attach(geoset);
                    _new.Alpha.MakeStatic(1);
                    CurrentModel.GeosetAnimations.Add(_new);
                }
            }
            RefreshGeosetAnimationsList();
        }
        private void separate(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                Pause = true;
                Scene_Viewport.Children.Clear();
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    List<List<CGeosetTriangle>> triangleGroups = Calculator.CollectTriangleGroups(geoset);
                    if (triangleGroups.Count > 1)
                    {
                        for (int i = 0; i < triangleGroups.Count; i++) // Process all groups
                        {
                            List<CGeosetTriangle> faces = triangleGroups[i];
                            Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
                            CGeoset newGeoset = new CGeoset(CurrentModel);
                            foreach (var face in faces)
                            {
                                CGeosetTriangle newFace = new CGeosetTriangle(CurrentModel);
                                // Process Vertex1
                                if (!reference.TryGetValue(face.Vertex1.Object, out var newVertex1))
                                {
                                    newVertex1 = new CGeosetVertex(CurrentModel);
                                    Calculator.CopyVertex(face.Vertex1.Object, newVertex1);
                                    reference[face.Vertex1.Object] = newVertex1;
                                    newGeoset.Vertices.Add(newVertex1);
                                }
                                newFace.Vertex1.Attach(newVertex1);
                                // Process Vertex2
                                if (!reference.TryGetValue(face.Vertex2.Object, out var newVertex2))
                                {
                                    newVertex2 = new CGeosetVertex(CurrentModel);
                                    Calculator.CopyVertex(face.Vertex2.Object, newVertex2);
                                    reference[face.Vertex2.Object] = newVertex2;
                                    newGeoset.Vertices.Add(newVertex2);
                                }
                                newFace.Vertex2.Attach(newVertex2);
                                // Process Vertex3
                                if (!reference.TryGetValue(face.Vertex3.Object, out var newVertex3))
                                {
                                    newVertex3 = new CGeosetVertex(CurrentModel);
                                    Calculator.CopyVertex(face.Vertex3.Object, newVertex3);
                                    reference[face.Vertex3.Object] = newVertex3;
                                    newGeoset.Vertices.Add(newVertex3);
                                }
                                newFace.Vertex3.Attach(newVertex3);
                                newGeoset.Triangles.Add(newFace);
                            }
                            // Assign the material
                            newGeoset.Material.Attach(geoset.Material.Object);
                            CGeosetGroup newGroup = new CGeosetGroup(CurrentModel);
                            Calculator.CopyGroup(geoset.Groups[0], newGroup, CurrentModel);
                            newGeoset.Groups.Add(newGroup);
                            foreach (var vertex in newGeoset.Vertices)
                            {
                                vertex.Group.Attach(newGroup);
                            }
                            CurrentModel.Geosets.Add(newGeoset);
                        }
                    }
                }
                RefreshGeosetsList();
                Pause = false;
                RefreshViewPort();
            }
        }
        private void drawshape(object sender, RoutedEventArgs e)
        {
            DrawShape ds = new DrawShape(CurrentModel);
            if (ds.ShowDialog() == true)
            {
                RefreshGeosetsList();
            }
        }
        private void extrudedpolygon(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0 || CurrentModel.Textures.Count == 0)
            {
                MessageBox.Show("There are no materials. At least one material is needed, to be applied to a generated geoset."); return;

            }
            CreatePolygonWindow cpw = new CreatePolygonWindow(CurrentModel);
            cpw.ShowDialog();
            if (cpw.DialogResult == true)
            {
                RefreshGeosetsList();
                RefreshViewPort();
                RefreshNodesTree();
            }
        }
        private void FragmentTrianglesInGeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (var geoset in geosets)
                {
                    foreach (CGeosetTriangle triangle in geoset.Triangles)
                    {
                        CGeosetVertex vertex1 = new CGeosetVertex(CurrentModel); Calculator.CopyVertex(triangle.Vertex1.Object, vertex1);
                        CGeosetVertex vertex2 = new CGeosetVertex(CurrentModel); Calculator.CopyVertex(triangle.Vertex2.Object, vertex2);
                        CGeosetVertex vertex3 = new CGeosetVertex(CurrentModel); Calculator.CopyVertex(triangle.Vertex3.Object, vertex3);
                        triangle.Vertex1.Attach(vertex1);
                        triangle.Vertex2.Attach(vertex2);
                        triangle.Vertex3.Attach(vertex3);
                    }
                    Calculator.CleanFreeVertices(geoset);
                }
            }
            RefreshGeosetsList();
        }
        private void FragmentFacesInGeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                List<CGeoset> ModifiedGeosets = new List<CGeoset>();
                foreach (var geoset in geosets)
                {
                    List<List<CGeosetTriangle>> list = FlatCalculator.CollectFlatSurfaces(geoset);
                    CGeoset fragmented = Calculator.ReAddTriangles(list, CurrentModel, geoset);
                    ModifiedGeosets.Add(fragmented);
                }
                Pause = true;
                foreach (var geoset in geosets)
                {
                    CurrentModel.Geosets.Remove(geoset);
                }
                foreach (var geoset in ModifiedGeosets)
                {
                    CurrentModel.Geosets.Add(geoset);
                }
                RefreshGeosetsList();
                Pause = false;
                RefreshViewPort();
            }
        }
        private void FragmentFacesIntoGeosets(object sender, RoutedEventArgs e)
        {
            Scene_Viewport.Children.Clear();
            Pause = true;
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (var geoset in geosets)
                {
                    List<List<CGeosetTriangle>> list = FlatCalculator.CollectFlatSurfaces(geoset);
                    if (list.Count <= 1) { continue; }
                    foreach (var collection in list)
                    {
                        CGeoset _new = Calculator.GeosetFromTriangles(collection, CurrentModel, geoset);
                        CurrentModel.Geosets.Add(_new);
                    }
                    CurrentModel.Geosets.Remove(geoset);
                }
            }
            RefreshGeosetsList();
            Pause = false;
            RefreshViewPort();
        }
        private void load_from_mpqs(object sender, RoutedEventArgs e)
        {
            ModelBrowser browser = new ModelBrowser();
            if (browser.ShowDialog() == true)
            {
                MPQHelper.Export(browser.Selected, AppHelper.TemporaryModelLocation);
                LoadModel(AppHelper.TemporaryModelLocation);
            }
        }

        private void ToggleShading(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            RenderShading = Menuitem_Shading.IsChecked == true;
            RefreshViewPort();
        }
        private void ToggleCols(object sender, RoutedEventArgs e)
        {
            RenderCollisionShapes = Menuitem_Cols.IsChecked == true;

            RefreshViewPort();
        }

        private void RotateGeosetsTogether(object sender, RoutedEventArgs e)
        {
            InputVector iv = new InputVector(AllowedValue.Both, new CVector3(0, 0, 0), "Rotation (-360-360)");
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X; float y = iv.Y; float z = iv.Z;
                if (x > 360 || x < -360) { MessageBox.Show("A values for rotation must be between -360 and 360"); return; }
                if (y > 360 || y < -360) { MessageBox.Show("A values for rotation must be between -360 and 360"); return; }
                if (z > 360 || z < -360) { MessageBox.Show("A values for rotation must be between -360 and 360"); return; }
                List<CGeoset> geosets = GetSelectedGeosets();
                if (geosets.Count == 1)
                {
                    Calculator.RotateGeoset(geosets[0], x, y, z);
                }
                else if (geosets.Count > 1)
                {
                    Calculator.RotateGeosetsTogether(geosets, x, y, z);
                }

            }
            RefreshViewPort();
        }

        private void Clampuvofgeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        Calculator.ClampUV(vertex.TexturePosition);
                    }
                }
            }
            RefreshViewPort();
        }

        private void ListGSequenes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switchLooping(null, null);
        }
        #region Animator
        private INode SelectedNodeInAnimator;
        private void SelectedSEquenceInAnimator(object sender, SelectionChangedEventArgs e)
        {

            if (List_Sequences_Animator.SelectedItem != null && List_Bones_Animator.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequenceAnimator();
                SelectedNodeInAnimator = GetSelectedNodeInanimator();

                List<int> tracks = new List<int>();
                for (int i = sequence.IntervalStart; i <= sequence.IntervalEnd; i++)
                {
                    if (
                        SelectedNodeInAnimator.Translation.NodeList.Any(x => x.Time == i) ||
                        SelectedNodeInAnimator.Rotation.NodeList.Any(x => x.Time == i) ||
                        SelectedNodeInAnimator.Scaling.NodeList.Any(x => x.Time == i)

                        )
                    {
                        tracks.Add(i);
                    }
                }
                List_Keyframes_Animator.Items.Clear();
                foreach (int track in tracks)
                {
                    List_Keyframes_Animator.Items.Add(new ListBoxItem() { Content = track.ToString() });
                }
                if (tracks.Count > 0)
                {
                    InputCurrentFrame.Text = tracks[0].ToString();
                    LoadKeyframeInViewport(tracks[0]);
                }
                else
                {
                    InputCurrentFrame.Text = sequence.IntervalStart.ToString();
                    LoadKeyframeInViewport(sequence.IntervalStart);
                }
            }
            else
            {
                List_Keyframes_Animator.Items.Clear();
            }
        }

        private INode GetSelectedNodeInanimator()
        {
            string name = (List_Bones_Animator.SelectedItem as ListBoxItem).Content.ToString();
            return CurrentModel.Nodes.First(x => x.Name == name);

        }

        private void ResetKeyframeTranslations(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteKeyframe(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = List_Keyframes_Animator.SelectedItem as ListBoxItem;
            int time = int.Parse(item.Content.ToString());
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CHelper || node is CBone)
                {
                    node.Translation.NodeList.RemoveAll(x => x.Time == time);
                    node.Rotation.NodeList.RemoveAll(x => x.Time == time);
                    node.Scaling.NodeList.RemoveAll(x => x.Time == time);
                }
            }
        }

        private void ResetKeyframeRotations(object sender, RoutedEventArgs e)
        {

        }

        private void ResetKeyframeScalings(object sender, RoutedEventArgs e)
        {

        }

        AnimatorMode Animator_Mode = AnimatorMode.Translate;
        AnimatorAxis Animator_Axis = AnimatorAxis.X;
        private void SetAnimatorTranslate(object sender, RoutedEventArgs e)
        {
            Animator_Mode = AnimatorMode.Translate;
            HighlightAnimatorModeButton(ButtonAnimatorT);


        }
        private void HighlightAnimatorModeButton(Button b)
        {
            ButtonAnimatorT.Background = Brushes.LightGray;
            ButtonAnimatorR.Background = Brushes.LightGray;
            ButtonAnimatorS.Background = Brushes.LightGray;
            ButtonAnimatorU.Background = Brushes.LightGray;
            b.Background = Brushes.Yellow;
        }
        private void HighlightAxisModeButton(Button b)
        {
            ButtonAnimatorX.Background = Brushes.LightGray;
            ButtonAnimatorY.Background = Brushes.LightGray;
            ButtonAnimatorZ.Background = Brushes.LightGray;
            ButtonAnimatorU.Background = Brushes.LightGray;
            b.Background = Brushes.Yellow;
        }
        private void SetAnimatorRotate(object sender, RoutedEventArgs e)
        {
            Animator_Mode = AnimatorMode.Rotate;
            HighlightAnimatorModeButton(ButtonAnimatorR);
        }

        private void SetAnimatorScale(object sender, RoutedEventArgs e)
        {
            Animator_Mode = AnimatorMode.Scale;
            HighlightAnimatorModeButton(ButtonAnimatorS);
        }

        private void SetAnimatorAxisX(object sender, RoutedEventArgs e)
        {
            Animator_Axis = AnimatorAxis.X;
            HighlightAxisModeButton(ButtonAnimatorX);
        }

        private void SetAnimatorAxisY(object sender, RoutedEventArgs e)
        {
            Animator_Axis = AnimatorAxis.Y;
            HighlightAxisModeButton(ButtonAnimatorY);
        }

        private void SetAnimatorAxisZ(object sender, RoutedEventArgs e)
        {
            Animator_Axis = AnimatorAxis.Z;
            HighlightAxisModeButton(ButtonAnimatorZ);
        }



        private void SelectedKeyframeInAnimator(object sender, SelectionChangedEventArgs e)
        {
            if (List_Keyframes_Animator.SelectedItem == null) return;
            ListBoxItem item = List_Keyframes_Animator.SelectedItem as ListBoxItem;
            int time = int.Parse(item.Content.ToString());
            InputCurrentFrame.Text = time.ToString();
            LoadKeyframeInViewport(time);

        }

        private void LoadKeyframeInViewport(int time)
        {
            throw new NotImplementedException();
        }

        private void RefreshBonesInAnimator()
        {
            if (List_Bones_Animator == null) { return; }
            List_Bones_Animator.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {


                if (NodesVisibleInAnimator[NodeType.Bone] && node is CBone) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Helper] && node is CHelper) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Emitter1] && node is CParticleEmitter) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Emitter2] && node is CParticleEmitter2) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Light] && node is CLight) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Event] && node is CEvent) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Cols] && node is CCollisionShape) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Ribbon] && node is CRibbonEmitter) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Attachment] && node is CAttachment) List_Bones_Animator.Items.Add(new ListBoxItem() { Content = node.Name });

            }
        }
        private void RefreshSequencesInAnimator()
        {
            List_Sequences_Animator.Items.Clear();
            foreach (var sequence in CurrentModel.Sequences)
            {
                string name = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]";
                List_Sequences_Animator.Items.Add(new ListBoxItem() { Content = name });
            }
        }
        private void SetAnimatorAxisU(object sender, RoutedEventArgs e)
        {
            Animator_Axis = AnimatorAxis.U;
            HighlightAxisModeButton(ButtonAnimatorU);
        }
        private int CurrentlySelectedTrack = -1;
        private void gototrack(object sender, RoutedEventArgs e)
        {
            if (List_Sequences_Animator.SelectedItem == null)
            {
                MessageBox.Show("Select a sequence from the list of sequences", "Invalid request"); return;
            }
            else
            {
                CSequence selected = GetSelectedSequenceAnimator();
                bool isInt = int.TryParse(InputCurrentFrame.Text, out int value);
                if (isInt)
                {
                    if (value >= selected.IntervalStart && value <= selected.IntervalEnd)
                    {
                        CurrentlySelectedTrack = value;
                        LoadKeyframeInViewport(CurrentlySelectedTrack);
                    }
                    else
                    {
                        MessageBox.Show("This track is not within the interval of the selected sequence", "Invalid request");

                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Expected integer", "Invalid request"); return;
                }
            }
        }
        #endregion
        private void LoadGeometryFromKeyframe(int track)
        {

        }

        private void DetachBoneFromSelectedVertices(object sender, RoutedEventArgs e)
        {
            if (ListAttachedToRiggings.SelectedItem == null)
            {
                MessageBox.Show("Nothing is selected in the list of attached bones", "Invalid request"); return;
            }
            else
            {
                if (ListAttachedToRiggings.Items.Count == 1) { MessageBox.Show("Cannot detach if there is only one item in the list"); return; }

                List<CGeosetVertex> vertices = GetSelectedVertices();
                if (vertices.Count == 0) { MessageBox.Show("Select vertices in the viewport", "Wait"); return; }
                INode node = GetSelectedAttachedNode();
                EditMatrixGroup(RiggingAction.Remove, node, vertices);

                ListAttachedToRiggings.Items.Remove(ListAttachedToRiggings.SelectedItem);
                // DetachBoneFromVertices(vertices, node);
            }
        }

        private void DetachBoneFromVertices(List<CGeosetVertex> vertices, INode node)
        {
            foreach (CGeosetVertex vertex in vertices)
            {
                var group = vertex.Group.Object;
                List<CGeosetGroupNode> gnodes = new();
                foreach (var gnode in group.Nodes)
                {
                    if (gnode.Node.Node == node)
                    {
                        gnodes.Add(gnode);
                    }
                }
                foreach (var gnode in gnodes)
                {
                    group.Nodes.Remove(gnode);
                }
            }
        }

        private INode GetSelectedAttachedNode()
        {
            string name = (ListAttachedToRiggings.SelectedItem as ListBoxItem).Content.ToString();
            return CurrentModel.Nodes.First(x => x.Name == name);
        }

        private void CreateGroupForEachVertex()
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                //unfinished
            }
        }
        private void ClearAndATtach(object sender, RoutedEventArgs e)
        {
            if (ListBonesRiggings.SelectedItem == null)
            {
                MessageBox.Show("First select a bone from the list of bones", "Invalid request"); return;
            }
            else
            {
                INode node = GetSelectedNodeInanimator();
                List<CGeosetVertex> vertices = GetSelectedVertices();
                if (vertices.Count == 0) { MessageBox.Show("Select vertices in the viewport"); return; }
                EditMatrixGroup(RiggingAction.ClearAdd, node, vertices);
                ListAttachedToRiggings.Items.Clear();
                ListAttachedToRiggings.Items.Add(new ListBoxItem() { Content = node.Name });
            }
        }

        private void ChangeAttachment(List<CGeosetVertex> vertices, INode node, bool clear)
        {
            foreach (var vertex in vertices)
            {
                var group = vertex.Group.Object;
                if (clear) group.Nodes.Clear();
                if (GroupHasNode(group, node) == false)
                {
                    CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                    gnode.Node.Attach(node);
                    group.Nodes.Add(gnode);


                }
            }
        }

        private bool GroupHasNode(CGeosetGroup group, INode node)
        {
            foreach (var gnode in group.Nodes)
            {
                if (gnode.Node.Node == node) { return true; }
            }
            return false;
        }

        private List<CGeosetVertex> GetSelectedVertices()
        {
            List<CGeosetVertex> list = new List<CGeosetVertex>();
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    if (vertex.Selected)
                    {
                        list.Add(vertex);
                    }
                }
            }
            return list;
        }

        private void AddAttachRiggin(object sender, RoutedEventArgs e)
        {
            if (ListBonesRiggings.SelectedItem == null)
            {
                MessageBox.Show("First select a bone from the list of bones", "Invalid request"); return;
            }
            else
            {
                if (ListAttachedToRiggings.Items.Count >= 4)
                {
                    MessageBox.Show("Not allowed to attach more than 4 bones to a vertex!"); return;
                }
                else
                {

                    INode node = GetSelectedNodeInanimator();
                    List<CGeosetVertex> vertices = GetSelectedVertices();
                    if (vertices.Count == 0) { MessageBox.Show("Select vertices in the viewport"); return; }
                    EditMatrixGroup(RiggingAction.Add, node, vertices);

                    ListAttachedToRiggings.Items.Add(new ListBoxItem() { Content = node.Name });

                }
            }
        }

        private void ReverseAllSequences(object sender, RoutedEventArgs e)
        {
            foreach (var sequence in CurrentModel.Sequences)
            {
                ReverseSequence(sequence);
            }
        }

        private void CopyKeyframe(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void PasteAsNewKeyframe(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void PasteOvKeyframe(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void makeallGAUseColor(object sender, RoutedEventArgs e)
        {
            foreach (var ga in CurrentModel.GeosetAnimations) ga.UseColor = true;
            RefreshGeosetAnimationsList();
        }

        private void makeallGAUseColorNOT(object sender, RoutedEventArgs e)
        {
            foreach (var ga in CurrentModel.GeosetAnimations) ga.UseColor = false;
            RefreshGeosetAnimationsList();
        }

        private void selectalluv(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void unselectuv(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void inverseselectuv(object sender, RoutedEventArgs e)
        {//unfinished

        }

        private void projectuv(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void swapuv2(object sender, RoutedEventArgs e)
        {//unfinished

        }

        private void negatevs(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void negateus(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void flattenu(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void flattenv(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void swapuvs(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void rotateuv90(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void EnterU(object sender, KeyEventArgs e)
        {
            bool parsed = float.TryParse(InputU.Text, out float u);
            if (parsed)
            {
                if (u >= 0 && u <= 1)
                {

                }
            }
        }

        private void EnterV(object sender, KeyEventArgs e)
        {
            bool parsed = float.TryParse(InputV.Text, out float v);
            if (parsed)
            {
                if (v >= 0 && v <= 1)
                {

                }
            }
        }
        private Vector2 CopiedUV = new Vector2();
        private void CopyUV(object sender, RoutedEventArgs e)
        {
            bool one = float.TryParse(InputU.Text, out float u);
            bool two = float.TryParse(InputV.Text, out float v);
            Vector2 nv = new Vector2();
            if (one) { nv.X = u; }
            if (two) { nv.Y = v; }
            CopiedUV = nv;
        }

        private void PasteUV(object sender, RoutedEventArgs e)
        {
            InputU.Text = CopiedUV.X.ToString();
            InputV.Text = CopiedUV.Y.ToString();
            // unfinished
        }

        private UVEditMode UVMode = UVEditMode.Move;
        private void SetSelectedUV(Button b)
        {
            ButtonMoveUV.Background = Brushes.LightGray;
            ButtonRotateUV.Background = Brushes.LightGray;
            ButtonScaleUV.Background = Brushes.LightGray;
            b.Background = Brushes.Yellow;
        }
        private void SetUVMove(object sender, RoutedEventArgs e)
        {
            UVMode = UVEditMode.Move;
            SetSelectedUV(ButtonMoveUV);
        }

        private void SEtUVRotate(object sender, RoutedEventArgs e)
        {
            UVMode = UVEditMode.Rotate;
            SetSelectedUV(ButtonRotateUV);
        }

        private void SetUVScale(object sender, RoutedEventArgs e)
        {


            UVMode = UVEditMode.Scale;
            SetSelectedUV(ButtonScaleUV);
        }


        //--------------------------------------------------
        // ANIMATOR
        //--------------------------------------------------
        private void Animator_ClearTranslations(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            node.Translation.Clear();
            RefreshAnimatorData();
        }

        private void RefreshAnimatorData()
        {

            RefreshBonesInAnimator();
            RefreshSequencesInAnimator();
            UnselectAllVerticesAnimator();
            List_Keyframes_Animator.Items.Clear();
            InputCurrentFrame.Text = "-1";
            //throw new NotImplementedException();
        }

        private void UnselectAllVerticesAnimator()
        {
            // throw new NotImplementedException();
        }

        private INode GetSeletedNodeInAnimator()
        {
            string name = GetNodeNameAnimator();
            return CurrentModel.Nodes.First(x => x.Name == name);
        }

        private void Animator_ClearRotations(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            node.Rotation.Clear();
            RefreshAnimatorData();
        }

        private void Animator_ClearScalings(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            node.Scaling.Clear();
            RefreshAnimatorData();
        }
        private int Animator_GetTrack()
        {
            bool parsed = int.TryParse(InputCurrentFrame.Text, out int value);
            if (parsed) return value;
            return -1;
        }
        private void Animator_ClearTranslations_Track(object sender, RoutedEventArgs e)
        {
            int track = Animator_GetTrack();
            if (track != -1)
            {
                INode node = GetSeletedNodeInAnimator();
                node.Translation.NodeList.RemoveAll(x => x.Time == track);
                RefreshAnimatorData();
            }
        }

        private void Animator_ClearRotations_Track(object sender, RoutedEventArgs e)
        {
            int track = Animator_GetTrack();
            if (track != -1)
            {
                INode node = GetSeletedNodeInAnimator();
                node.Rotation.NodeList.RemoveAll(x => x.Time == track);
                RefreshAnimatorData();
            }
        }

        private void Animator_ClearScalings_Track(object sender, RoutedEventArgs e)
        {
            int track = Animator_GetTrack();
            if (track != -1)
            {
                INode node = GetSeletedNodeInAnimator();
                node.Scaling.NodeList.RemoveAll(x => x.Time == track);
                RefreshAnimatorData();
            }
        }

        private void Animator_ResetTranslations(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            foreach (var kf in node.Translation)
            {
                kf.Value.X = 0;
                kf.Value.Y = 0;
                kf.Value.Z = 0;
            }
        }

        private void Animator_ResetRotations(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            foreach (var kf in node.Rotation)
            {
                kf.Value.X = 0;
                kf.Value.Y = 0;
                kf.Value.Z = 0;
                kf.Value.W = 1;
            }
        }

        private void Animator_ResetScalings(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            foreach (var kf in node.Scaling)
            {
                kf.Value.X = 1;
                kf.Value.Y = 1;
                kf.Value.Z = 1;
            }
        }

        private void Animator_ResetTranslations_Bone(object sender, RoutedEventArgs e)
        {

            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            int track = Animator_GetTrack();
            if (track != -1)
            {
                var key = node.Translation.NodeList.First(x => x.Time == track);
                key.Value.X = 0;
                key.Value.Y = 0;
                key.Value.Z = 0;
            }
        }

        private void Animator_ResetRorations_Bone(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            int track = Animator_GetTrack();
            if (track != -1)
            {
                var key = node.Rotation.NodeList.First(x => x.Time == track);
                key.Value.X = 0;
                key.Value.Y = 0;
                key.Value.Z = 0;
                key.Value.W = 1;
            }
        }

        private void Animator_ResetScalings_Bone(object sender, RoutedEventArgs e)
        {
            if (List_Bones_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            int track = Animator_GetTrack();
            if (track != -1)
            {
                var key = node.Scaling.NodeList.First(x => x.Time == track);
                key.Value.X = 1;
                key.Value.Y = 1;
                key.Value.Z = 1;
            }
        }

        private void gotoprevtrack(object sender, RoutedEventArgs e)
        {

        }

        private void gotonexttrack(object sender, RoutedEventArgs e)
        {

        }

        UVLockType UVMapperLock = UVLockType.None;
        private void LockU(object sender, RoutedEventArgs e)
        {
            if (UVMapperLock == UVLockType.None)
            {
                SetBackgroundImage(ButtonLockU, IconPaths["lock"]);
                UVMapperLock = UVLockType.U;
            }
            else if (UVMapperLock == UVLockType.U)
            {
                SetBackgroundImage(ButtonLockU, IconPaths["unlock"]);
                UVMapperLock = UVLockType.None;
            }
            else if (UVMapperLock == UVLockType.V)
            {
                SetBackgroundImage(ButtonLockU, IconPaths["lock"]);
                SetBackgroundImage(ButtonLockV, IconPaths["unlock"]);
                UVMapperLock = UVLockType.U;
            }
        }

        private void LockV(object sender, RoutedEventArgs e)
        {
            if (UVMapperLock == UVLockType.None)
            {
                SetBackgroundImage(ButtonLockV, IconPaths["lock"]);
                UVMapperLock = UVLockType.V;
            }
            else if (UVMapperLock == UVLockType.V)
            {
                SetBackgroundImage(ButtonLockV, IconPaths["unlock"]);
                UVMapperLock = UVLockType.None;
            }
            else if (UVMapperLock == UVLockType.U)
            {
                SetBackgroundImage(ButtonLockV, IconPaths["lock"]);
                SetBackgroundImage(ButtonLockU, IconPaths["unlock"]);
                UVMapperLock = UVLockType.V;
            }
        }

        private void ToggleGround(object sender, RoutedEventArgs e)
        {
            RenderGround = Menuitem_ground.IsChecked == true;
            RefreshViewPort();
        }
        private void DrawGroundPlane(Viewport3D viewport, int edgeLength, BitmapSource texture)
        {
            // Create a 3D model for the ground plane
            GeometryModel3D groundPlane = CreateGroundPlaneGeometry(edgeLength);

            // Apply texture as material (make it two-sided)
            DiffuseMaterial material = new DiffuseMaterial(new ImageBrush(texture));
            groundPlane.Material = material;

            // Make the material two-sided
            groundPlane.BackMaterial = material;

            // Create a ModelVisual3D to display in the Viewport3D
            ModelVisual3D groundModel = new ModelVisual3D();
            groundModel.Content = groundPlane;

            // Add the ground model to the viewport
            viewport.Children.Add(groundModel);
        }

        private GeometryModel3D CreateGroundPlaneGeometry(int edgeLength)
        {
            // Define the vertices of the ground plane (a square)
            Point3DCollection positions = new Point3DCollection
    {
       new Point3D(-edgeLength / 2.0, 0, -edgeLength / 2.0), // bottom-left
        new Point3D(edgeLength / 2.0, 0, -edgeLength / 2.0),  // bottom-right
        new Point3D(edgeLength / 2.0, 0, edgeLength / 2.0),   // top-right
        new Point3D(-edgeLength / 2.0, 0, edgeLength / 2.0),  // top-left
    };

            // Define the triangle indices (two triangles for the square)
            Int32Collection triangleIndices = new Int32Collection
    {
        0, 1, 2,  // First triangle
        0, 2, 3   // Second triangle
    };

            // Create the geometry for the ground plane
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = positions,
                TriangleIndices = triangleIndices
            };

            // Set texture coordinates for the plane to properly map the texture
            mesh.TextureCoordinates = new PointCollection
    {
        new Point(0, 0), // bottom-left
        new Point(1, 0), // bottom-right
        new Point(1, 1), // top-right
        new Point(0, 1)  // top-left
    };

            // Make the plane face upwards (along the XZ plane)
            Transform3DGroup transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90)));

            // Return a GeometryModel3D with the mesh
            GeometryModel3D groundPlane = new GeometryModel3D(mesh, null)
            {
                Transform = transformGroup
            };

            return groundPlane;
        }

        private void createCols2ForTargetGeo(object sender, RoutedEventArgs e)
        {

            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    CVector3 centroid = Calculator.GetCentroidOfGeoset(geoset);
                    //Point3D centroidP = new Point3D(centroid.X, centroid.Y, centroid.Z);
                    // Point3D Farthest = Calculator.GetFarthestPoint(geoset);
                    float distance = Calculator.GetFarthestDistance(geoset);
                    CCollisionShape cs = new CCollisionShape(CurrentModel);
                    cs.Name = "GeneratedCollisionShape_" + IDCounter.Next_();
                    cs.Type = ECollisionShapeType.Sphere;
                    cs.Radius = distance;
                    cs.PivotPoint = centroid;
                    CurrentModel.Nodes.Add(cs);
                    RefreshNodesTree();
                    if (RenderCollisionShapes) RefreshViewPort();
                }
            }
            //--------------------------------------------------
            //--------------------------------------------------
            //--------------------------------------------------
        }

        private void closemodel(object sender, RoutedEventArgs e)
        {
            CurrentModel = new CModel();
            CurrentSaveFolder = "";
            CurrentSaveLocaiton = "";

            refreshalllists(null, null);
            Title = AppHelper.Name;
        }

        private void saveasCopy(object sender, RoutedEventArgs e)
        {
            if (CurrentSaveLocaiton
                .Length > 0 && Directory.Exists(CurrentSaveFolder))
            {
                string temp = CurrentSaveLocaiton;
                CurrentSaveLocaiton = AppendTimestampToFilePath(CurrentSaveLocaiton);
                save(null, null);
                CurrentSaveLocaiton = temp;
            }
        }

        public static string AppendTimestampToFilePath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            // Get current date and time
            DateTime now = DateTime.Now;
            string dayWithSuffix = AddDaySuffix(now.Day);
            string timestamp = $"{dayWithSuffix} {now:MMMM yyyy HH-mm-ss}";

            string newFileName = $"{fileNameWithoutExt} {timestamp}{extension}";
            return Path.Combine(directory ?? "", newFileName);
        }

        private static string AddDaySuffix(int day)
        {
            string suffix;
            if (day == 11 || day == 12 || day == 13)
            {
                suffix = "th";
            }
            else if (day.ToString().EndsWith("1"))
            {
                suffix = "st";
            }
            else if (day.ToString().EndsWith("2"))
            {
                suffix = "nd";
            }
            else if (day.ToString().EndsWith("3"))
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }
            return day + suffix;
        }

        private void InputUVGrid_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool input = int.TryParse(InputUVGrid.Text, out int value);
            if (input)
            {
                if (value >= 0 && value < 256)
                {
                    SetUVCanvasGrid(value);
                }
                else
                {
                    SetUVCanvasGrid(0);
                }
            }
            else
            {
                SetUVCanvasGrid(0);
            }
        }

        private void SetUVCanvasGrid(int value)
        {
            //unfinished
            //  throw new NotImplementedException();
        }

        private void newtexturefrominput(object sender, RoutedEventArgs e)
        {
            Input i = new Input("");
            if (i.ShowDialog() == true)
            {
                string text = i.box.Text.Trim();
                if (text.Length == 0) { MessageBox.Show("Empty input"); return; }
                if (MPQHelper.FileExists(text))
                {
                    CTexture texture = new CTexture(CurrentModel);
                    texture.FileName = text;
                    CurrentModel.Textures.Add(texture);
                    RefreshTextures();
                }
                else
                {
                    MessageBox.Show($"The texture at '{text}' was not found. Not added."); return;
                }
            }
        }

        Dictionary<NodeType, bool> NodesVisibleInAnimator = new Dictionary<NodeType, bool>()
         {
             { NodeType.Bone, true},
             { NodeType.Helper, true},
             { NodeType.Cols, true},
             { NodeType.Emitter1, true},
             { NodeType.Emitter2, true},
             { NodeType.Ribbon, true},
             { NodeType.Event, true},
             { NodeType.Attachment, true},
             { NodeType.Light, true},
         };
        private void SVR_B(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Bone] = Check_SVR_B.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_H(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Helper] = Check_SVR_H.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_E1(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Emitter1] = Check_SVR_E1.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_E2(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Emitter2] = Check_SVR_E2.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_R(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Ribbon] = Check_SVR_R.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_A(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Attachment] = Check_SVR_A.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_C(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Cols] = Check_SVR_C.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_O(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Event] = Check_SVR_O.IsChecked == true;
            RefreshBonesInAnimator();
        }

        private void SVR_L(object sender, RoutedEventArgs e)
        {
            NodesVisibleInAnimator[NodeType.Light] = Check_SVR_L.IsChecked == true;
            RefreshBonesInRigging();
        }

        private void ShowMenuNodesRigging(object sender, RoutedEventArgs e)
        {
            ButtonRiggingNodes.ContextMenu.IsOpen = true;
        }

        private void Animator_CopyNodeTranslation(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void Animator_CopyNodeRotation(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void Animator_CopyNodeScaling(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void Animator_PasteNodeMerge(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void Animator_PasteNodeOverwrite(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void SplitMatrixGroups(object sender, RoutedEventArgs e)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                List<CGeosetGroup> groups = new List<CGeosetGroup>();
                foreach (var vertex in geoset.Vertices)
                {
                    CGeosetGroup group = CloneGroup(vertex.Group.Object);
                    vertex.Group.Attach(group);
                    groups.Add(group);
                }
                geoset.Groups.Clear();
                foreach (var group in groups) geoset.Groups.Add(group);
            }
            ButtonSplitGroups.IsEnabled = false;
            ButtonAddAttach.IsEnabled = true;
            ButtonClearAttach.IsEnabled = true;
            Detach.IsEnabled = true;

        }

        private CGeosetGroup CloneGroup(CGeosetGroup original)
        {
            CGeosetGroup group = new CGeosetGroup(CurrentModel);
            foreach (var item in group.Nodes)
            {
                CGeosetGroupNode node = new CGeosetGroupNode(CurrentModel);
                node.Node.Attach(item.Node.Node);
                group.Nodes.Add(node);
            }
            return group;
        }

        private void ToggleSkinning(object sender, RoutedEventArgs e)
        {
            RenderSkinning = Menuitem_skinning.IsChecked == true;
        }

        private void CopyKeyframeT(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void CopyKeyframeR(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void CopyKeyframeS(object sender, RoutedEventArgs e)
        {
            //unfinished
        }
        private List<CBone> ListBones_Rigging = new List<CBone>();
        private void CollectBonesForRigging()
        {
            ListBones_Rigging.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CBone bone)
                {

                    ListBones_Rigging.Add(bone);
                }
            }
        }
        private void SelectedBoneInRigging(object sender, SelectionChangedEventArgs e)
        {
            if (ListBonesRiggings.SelectedItem == null) return;
            CBone bone = getselectedBoneInRigging();
            foreach (var node in ListBones_Rigging)
            {
                node.IsSelected = false;
            }
            bone.IsSelected = true;
        }

        private CBone getselectedBoneInRigging()
        {
            string name = (ListBonesRiggings.SelectedItem as ListBoxItem).Content.ToString();

            return ListBones_Rigging.First(x => x.Name == name);
        }

        private void SEtCeilings(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Sequences.Count > 0)
            {
                Input i = new Input("Max Z");
                if (i.DialogResult == true)
                {
                    bool parssed = float.TryParse(i.Result, out float value);
                    if (parssed)
                    {
                        foreach (var sequence in CurrentModel.Sequences)
                        {
                            sequence.Extent.Max.Z = value;
                        }
                        foreach (var geoset in CurrentModel.Geosets)
                        {
                            foreach (var extent in geoset.Extents)
                            {
                                extent.Extent.Max.Z = value;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Expected integer or float", "Invalid input");
                    }
                }

            }
            else
            {
                MessageBox.Show("There are no sequences");
            }
        }

        private void scalegeosetsTogether(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            InputVector iv = new InputVector(AllowedValue.Positive, new CVector3(100, 100, 100), "Percentage");
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X; float y = iv.Y; float z = iv.Z;
                List<CGeoset> geosets = GetSelectedGeosets();
                List<CGeosetVertex> vertices = GetVerticesOfGeosets(geosets);
                foreach (var vertex in vertices)
                {
                    vertex.Position.X *= x;
                    vertex.Position.Y *= y;
                    vertex.Position.Z *= z;
                }


            }
        }

        private List<CGeosetVertex> GetVerticesOfGeosets(List<CGeoset> geosets)
        {
            List<CGeosetVertex> vertices = new List<CGeosetVertex>();
            foreach (var geoset in geosets)
            {
                vertices.AddRange(geoset.Vertices);
            }

            return vertices;
        }

        private void rotateeachCP(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            List<CGeoset> geosets = GetSelectedGeosets();
            InputVector vector = new InputVector(AllowedValue.Both);
            if (vector.ShowDialog() == true)
            {
                float x = vector.X;
                float y = vector.Y;
                float z = vector.Z;
                if (RotationInRange(x, y, z) == false) { MessageBox.Show("Values not in range -26 - 360"); }
                foreach (var geoset in geosets)
                {
                    Calculator.RotateGeoset(geoset, x, y, z);
                }
                RefreshViewPort();
            }


        }
        private bool RotationInRange(float x, float y, float z)
        {
            if (x < -360) return false;
            if (x > 360) return false;
            if (y < -360) return false;
            if (y > 360) return false;
            if (z < -360) return false;
            if (z > 360) return false;
            return true;
        }

        private void rotateallCP(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            List<CGeoset> geosets = GetSelectedGeosets();
            InputVector vector = new InputVector(AllowedValue.Both);
            if (vector.ShowDialog() == true)
            {
                float x = vector.X;
                float y = vector.Y;
                float z = vector.Z;
                if (RotationInRange(x, y, z) == false) { MessageBox.Show("Values not in range -26 - 360"); }
                Calculator.RotateGeosetsTogether(geosets, x, y, z);

            }
        }

        private void SelectionChanged_Geosets(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl tc && tc == Tabs_Geosets)
            {


                if (Tabs_Geosets.SelectedIndex == 2) // rigging
                {
                    RefreshBonesInRigging();
                    RefreshGeosetsListRigging();
                    CollectBonesForRigging();
                    ListAttachedToRiggings.Items.Clear();
                    // clear selection in rigging
                    foreach (var bone in ListBones_Rigging) bone.IsSelected = false;

                    bool skinningOK = CheckSkinning();
                    ButtonSplitGroups.IsEnabled = !skinningOK;
                    ButtonAddAttach.IsEnabled = skinningOK;
                    ButtonClearAttach.IsEnabled = skinningOK;
                    Detach.IsEnabled = skinningOK;
                    if (!skinningOK) MessageBox.Show("For easier working with bone-vertex relationships, each vertex must have its own matrix group. You can merge similars later. Click on 'Split groups' before using the rigging editor.");


                }
                else if (Tabs_Geosets.SelectedIndex == 3) // animator
                {

                    RefreshAnimatorData();
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // UVMAPPER
                {

                    RefreshGeosetsInUVMapper();
                }
                e.Handled = true;
            }
        }

        private void SetWorkModeVertices(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void SetWorkModeTriangles(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void SetWorkModeEdges(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void SetWorkModeSelect(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void MenuItemMinus1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus10_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus10_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus50_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus50_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus100_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus100_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus1Percent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus1Percent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus5Percent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus5Percent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemMinus10Percent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemPlus10Percent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemZeroPercent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemHundredPercent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExportGeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            string savePath = SaveTGeoFileDialog();
            if (savePath.Length > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                string data = GeosetExporter.Write(geosets[0]);
                File.WriteAllText(savePath, data);
            }
        }

        private void ImportGeoset(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no materials. At least one material is needed to be applied to an imported geoset."); return;
            }
            string openPath = OpenTGeoFileDialog();
            if (openPath.Length > 0)
            {
                CGeoset imported = GeosetExporter.Read(openPath, CurrentModel);

                ImportGeosetDialog finalize = new ImportGeosetDialog(CurrentModel);
                finalize.ShowDialog();
                if (finalize.DialogResult == true)
                {
                    CGeosetGroup group = new CGeosetGroup(CurrentModel);
                    CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                    gnode.Node.Attach(finalize.SelectedNode);
                    group.Nodes.Add(gnode);
                    imported.Groups.Add(group);
                    imported.Material.Attach(finalize.SelectedMaterial);
                    CurrentModel.Geosets.Add(imported);
                    RefreshGeosetsList();
                    RefreshViewPort();
                }


            }
        }

        public string OpenTGeoFileDialog()
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "TGeo Files (*.tgeo)|*.tgeo",
                // Set initial directory (optional)
                InitialDirectory = @"C:\"
            };

            // Show the dialog and check if the user selected a file
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }
        public string SaveTGeoFileDialog()
        {
            // Get the user's Documents folder as a default location
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Create a SaveFileDialog instance for saving files
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "TGeo Files (*.tgeo)|*.tgeo",
                // Set the default file name (optional)
                FileName = "newfile.tgeo",
                // Set initial directory to the user's Documents folder
                InitialDirectory = initialDirectory
            };

            // Show the dialog and check if the user selected a file
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return saveFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }

        private void SetPriorityPlane(object sender, TextChangedEventArgs e)
        {
            bool parsed = int.TryParse(PRiortyPlaneInput.Text, out int value);

            if (parsed && List_MAterials.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                mat.PriorityPlane = value;
            }
        }

        private void ReattachNodeGeometryToAnotherBone(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode Selectednode = GetSeletedNode();
                if (!NodeHasAttachedVertices(Selectednode))
                {
                    MessageBox.Show("Nothing is attached to this node");
                }
                else
                {
                    List<string> bones = CurrentModel.Nodes.Where(y => y is CBone).Select(x => x.Name).ToList();
                    bones.Remove(Selectednode.Name);
                    if (bones.Count == 0)
                    {
                        MessageBox.Show("There are no bones available"); return;
                    }
                    Selector s = new Selector(bones);
                    s.ShowDialog();
                    if (s.DialogResult == true)
                    {
                        int index = s.box.SelectedIndex;
                        INode selected = CurrentModel.Nodes.First(x => x.Name == bones[index]);

                        foreach (var geoset in CurrentModel.Geosets)
                        {
                            foreach (var vertex in geoset.Vertices)
                            {
                                CGeosetGroupNode groupNode = null;
                                foreach (var gnode in vertex.Group.Object.Nodes)
                                {
                                    if (gnode.Node.Node == Selectednode)
                                    {
                                        groupNode = gnode; break;
                                    }
                                }
                                if (groupNode != null)
                                {
                                    groupNode.Node.Attach(selected);
                                }
                            }

                        }
                    }
                }
            }
        }

        private void EditMatrixGroup(RiggingAction action, INode node, List<CGeosetVertex> vertices)
        {
            switch (action)
            {
                case RiggingAction.Add:
                    foreach (var vertex in vertices)
                    {
                        bool has = false;
                        foreach (var gnode in vertex.Group.Object.Nodes.ToList())
                        {
                            if (gnode.Node.Node == node)
                            {
                                has = true; break;
                            }
                        }
                        if (!has)
                        {
                            CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                            gnode.Node.Attach(node);
                            vertex.Group.Object.Nodes.Add(gnode);
                        }
                    }



                    break;
                case RiggingAction.Remove:
                    foreach (var vertex in vertices)
                    {
                        foreach (var gnode in vertex.Group.Object.Nodes.ToList())
                        {
                            if (gnode.Node.Node == node)
                            {
                                vertex.Group.Object.Nodes.Remove(gnode);
                            }
                        }
                    }
                    break;
                case RiggingAction.ClearAdd:
                    foreach (var vertex in vertices)
                    {
                        vertex.Group.Object.Nodes.Clear();
                        CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                        gnode.Node.Attach(node);
                        vertex.Group.Object.Nodes.Add(gnode);
                    }
                    break;
            }
        }

        private void SetSequenceLoop(object sender, MouseButtonEventArgs e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence sequence = GetSelectedSequence();
            sequence.NonLooping = !sequence.NonLooping;
            RefreshSequencesList();
        }

        private void ToggleGAE(object sender, RoutedEventArgs e)
        {
            RenderGeosetExtents = Menuitem_GAE.IsChecked == true;
            RefreshViewPort();
        }

        private void ToggleGAES(object sender, RoutedEventArgs e)
        {
            RenderGeosetExtentSphere = Menuitem_GAES.IsChecked == true;
            RefreshViewPort();
        }

        private void ToggleNodes(object sender, RoutedEventArgs e)
        {
            RenderNodes = Menuitem_Nodes.IsChecked == true;
            RefreshViewPort();
        }

        private void ToggleGeometry(object sender, RoutedEventArgs e)
        {
            RenderGeometry = Menuitem_Geometry.IsChecked == true;
            RefreshViewPort();
        }
        private CVector3 CopiedPivotPoint = new CVector3();
        private void CopyPivot(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                CopiedPivotPoint = new CVector3(selected.PivotPoint);
            }
        }

        private void PastePivot(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint = new CVector3(CopiedPivotPoint);

            }
        }

        private void SetSamePPAsParent(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint = new CVector3(selected.Parent.Node.PivotPoint);

            }
        }


        private CVector3 GetPolarOffsetPoint(CVector3 Point, float Distance, Axes axes, float angle)
        {
            // Convert angle to radians
            float radians = angle * (float)Math.PI / 180f;

            // Offset based on the selected axis
            switch (axes)
            {
                case Axes.X:
                    return new CVector3(Point.X,
                                        Point.Y + Distance * (float)Math.Cos(radians),
                                        Point.Z + Distance * (float)Math.Sin(radians));
                case Axes.Y:
                    return new CVector3(Point.X + Distance * (float)Math.Cos(radians),
                                        Point.Y,
                                        Point.Z + Distance * (float)Math.Sin(radians));
                case Axes.Z:
                    return new CVector3(Point.X + Distance * (float)Math.Cos(radians),
                                        Point.Y + Distance * (float)Math.Sin(radians),
                                        Point.Z);
                default:
                    return Point;
            }
        }

        private void CenterAtItsAttachedVertices(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                List<CGeosetVertex> attached = GetAttachedVerticesToNode(selected);
                CVector3 centroid = Calculator.GetCentroidOfVertices(attached);
                selected.PivotPoint = new CVector3(centroid);

            }
        }
        private List<CGeosetVertex> GetAttachedVerticesToNode(INode node)
        {
            List<CGeosetVertex> list = new List<CGeosetVertex>();
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    if (vertex.Group == null || vertex.Group.Object == null) { continue; }
                    foreach (var gnode in vertex.Group.Object.Nodes)
                    {
                        if (gnode.Node.Node == node) { list.Add(vertex); }
                    }

                }

            }
            return list;
        }

        private void REsetNode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint = new CVector3();

            }
        }

        private void NegatePPX(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.X = -selected.PivotPoint.X;

            }
        }

        private void NegatePPY(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.Y = -selected.PivotPoint.Y;

            }
        }

        private void NegatePPZ(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.Z = -selected.PivotPoint.Z;

            }
        }

        private void NegatePPA(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.X = -selected.PivotPoint.X;
                selected.PivotPoint.Y = -selected.PivotPoint.Y;
                selected.PivotPoint.Z = -selected.PivotPoint.Z;

            }

        }

        private void SetPolarOffsetPP(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                PivoPointOffsetWkndow w = new();
                if (w.ShowDialog() == true)
                {
                    CVector3 result = GetPolarOffsetPoint(w.Point, w.Distance, w.axes, w.Angle);
                    selected.PivotPoint = new CVector3(result);
                }


            }
        }

        private void ResetAllnodepp(object sender, RoutedEventArgs e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint = new CVector3();
        }

        private void negateallnodes_x(object sender, RoutedEventArgs e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint.X = -node.PivotPoint.X;
        }

        private void negateallnodes_y(object sender, RoutedEventArgs e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint.Y = -node.PivotPoint.Y;
        }

        private void negateallnodes_z(object sender, RoutedEventArgs e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint.Z = -node.PivotPoint.Z;
        }

        private void negateallnodes_all(object sender, RoutedEventArgs e)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                node.PivotPoint.X = -node.PivotPoint.X;
                node.PivotPoint.Y = -node.PivotPoint.Y;
                node.PivotPoint.Z = -node.PivotPoint.Z;
            }
        }

        private void alignallnodes_x(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Nodes.Count > 1)
            {
                var first = CurrentModel.Nodes[0];
                for (int i = 1; i < CurrentModel.Nodes.Count; i++)
                {
                    CurrentModel.Nodes[i].PivotPoint.X = first.PivotPoint.X;
                }
            }
        }

        private void alignallnodes_y(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Nodes.Count > 1)
            {
                var first = CurrentModel.Nodes[0];
                for (int i = 1; i < CurrentModel.Nodes.Count; i++)
                {
                    CurrentModel.Nodes[i].PivotPoint.X = first.PivotPoint.Y;
                }
            }
        }

        private void alignallnodes_z(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Nodes.Count > 1)
            {
                var first = CurrentModel.Nodes[0];
                for (int i = 1; i < CurrentModel.Nodes.Count; i++)
                {
                    CurrentModel.Nodes[i].PivotPoint.X = first.PivotPoint.Z;
                }
            }
        }



        private void SetDistanceBetween2Geosets(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Geosets.Count < 2)
            {
                MessageBox.Show("There must be at least 2 geosets");
                return;
            }
            if (ListGeosets.SelectedItems.Count != 2)
            {
                MessageBox.Show("Select exactly 2 geosets");
                return;
            }

            List<CGeoset> geosets = GetSelectedGeosets();
            PushGeosets ps = new PushGeosets();

            if (ps.ShowDialog() == true)
            {
                bool first = ps.CheckFirst.IsChecked == true; // push first from second
                bool second = ps.CheckSecond.IsChecked == true; // push second from first
                bool both = ps.CheckBoth.IsChecked == true; // push both from each other

                InputVector vector = new InputVector(AllowedValue.Both);
                if (vector.ShowDialog() == true)
                {
                    float x = vector.X;
                    float y = vector.Y;
                    float z = vector.Z;

                    if (first || both)
                    {
                        foreach (var vertex in geosets[0].Vertices)
                        {
                            vertex.Position = new CVector3(
                                vertex.Position.X + x,
                                vertex.Position.Y + y,
                                vertex.Position.Z + z
                            );
                        }
                    }

                    if (second || both)
                    {
                        foreach (var vertex in geosets[1].Vertices)
                        {
                            vertex.Position = new CVector3(
                                vertex.Position.X - x,
                                vertex.Position.Y - y,
                                vertex.Position.Z - z
                            );
                        }
                    }
                }
            }
            RefreshGeosetsList();
        }

        private void setGeosetUnselectable(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                if (geosets.Count == 1)
                {
                    Boolean_Window bw = new Boolean_Window("Unselectable");
                    bw.Val.IsChecked = geosets[0].Unselectable;
                    if (bw.ShowDialog() == true)
                    {
                        geosets[0].Unselectable = bw.Val.IsChecked == true;
                    }
                }
                else
                {
                    Boolean_Window bw = new Boolean_Window("Unselectable");
                    bw.Val.IsChecked = geosets[0].Unselectable;
                    if (bw.ShowDialog() == true)
                    {
                        bool checked_ = bw.Val.IsChecked == true;
                        foreach (var geoset in geosets)
                        {
                            geoset.Unselectable = checked_;
                        }

                    }
                }
            }

        }

        private void setGeosetSelection(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSelectedGeosets();
            if (geosets.Count == 1)
            {
                Input i = new(geosets[0].SelectionGroup.ToString(), "Selection group");
                if (i.ShowDialog() == true)
                {
                    bool parsed = int.TryParse(i.Result, out int result);
                    if (parsed) { geosets[0].SelectionGroup = result; }
                }
            }
            else
            {
                Input i = new("Selection group");
                if (i.ShowDialog() == true)
                {
                    bool parsed = int.TryParse(i.Result, out int result);
                    foreach (var geoset in geosets)
                    {
                        geoset.SelectionGroup = result;
                    }

                }
            }
        }

        private void rootchildren(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();

                RootChildrenOf(node);
                RefreshNodesTree();

            }
        }
        void RootChildrenOf(INode node)
        {
            foreach (var nod in CurrentModel.Nodes)
            {
                if (nod.Parent.Node == node)
                {
                    nod.Parent.Detach();
                }
            }
        }
        void RootChildrenOf_All(INode node)
        {
            foreach (var nod in CurrentModel.Nodes)
            {
                if (nod.Parent.Node == node)
                {
                    nod.Parent.Detach();
                    RootChildrenOf_All(nod);
                }
            }
        }
        private void rootchildreninfinite(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();

                RootChildrenOf_All(node);
                RefreshNodesTree();
            }
        }

        private void setallnodespoint(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Nodes.Count > 0)
            {
                InputVector vector = new InputVector(AllowedValue.Both);
                if (vector.ShowDialog() == true)
                {
                    foreach (var node in CurrentModel.Nodes)
                    {
                        node.PivotPoint = new CVector3(vector.X, vector.Y, vector.Z);
                    }
                }
            }
        }

        private void setGeosetExtent(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                Edit_Extent ee = new Edit_Extent(geosets[0].Extent);
                ee.ShowDialog();
            }
        }

        private void setGeosetExtents(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {

                List<CGeoset> geosets = GetSelectedGeosets();
                if (geosets[0].Extents.Count == 0) { return; }
                GeosetExtents gx = new GeosetExtents(geosets[0], CurrentModel.Sequences.Select(x => x).ToList());
                gx.ShowDialog();
            }
        }

        private void setSEquenceExtent(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                Edit_Extent ee = new Edit_Extent(sequence.Extent);
                ee.ShowDialog();
            }
        }

        private void setRarity(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                Input i = new Input(sequence.Rarity.ToString());
                if (i.ShowDialog() == true)
                {
                    bool parsed = float.TryParse(i.Result, out float value);
                    sequence.Rarity = value;
                }

            }
        }

        private void setMovespeed(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                Input i = new Input(sequence.MoveSpeed.ToString());
                if (i.ShowDialog() == true)
                {
                    bool parsed = float.TryParse(i.Result, out float value);
                    sequence.MoveSpeed = value;
                }

            }
        }
        bool RenderEnabled = true;
        private void S62(object sender, RoutedEventArgs e)
        {
            RenderEnabled = Menuitem_Enabled.IsChecked == true;
            if (RenderEnabled) { RefreshViewPort(); }
        }

        private void leavesequenceatframe(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                List<int> Keyframes = GetKeyframesOfSequence(sequence);
                if (Keyframes.Count >= 2)
                {
                    Selector s = new Selector(Keyframes.Select(x=>x.ToString()).ToList());
                    if (s.ShowDialog() == true)
                    {
                        int selected = int.Parse(s.Selected);
                        LeaveSequenceAtFrame_Finalize(sequence, selected);
                    }
                }
                else
                {
                    MessageBox.Show("A sequence must have at lesat 2 keyframes for this action"); return;
                }
            }
        }

        private void LeaveSequenceAtFrame_Finalize(CSequence sequence, int selected)
        {
            int from = sequence.IntervalStart;
            int to = sequence.IntervalEnd;
            int keep = selected;
            foreach (INode node in CurrentModel.Nodes)
            {
                LeaveSequenceAtFrame_Single(from, to, keep, node.Translation);
                LeaveSequenceAtFrame_Single(from, to, keep, node.Rotation);
                LeaveSequenceAtFrame_Single(from, to, keep, node.Scaling);
                if (node is CParticleEmitter)
                {
                    CParticleEmitter item = node as CParticleEmitter;
                    LeaveSequenceAtFrame_Single(from, to, keep, item.EmissionRate);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.LifeSpan);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.InitialVelocity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Gravity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Longitude);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 item = node as CParticleEmitter2;
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Gravity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Width);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Length);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Speed);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Variation);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Latitude);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.EmissionRate);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter item = node as CRibbonEmitter;
                    LeaveSequenceAtFrame_Single(from, to, keep, item.HeightAbove);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.HeightBelow);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.TextureSlot);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Alpha);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Color);
                }
                if (node is CLight)
                {
                    CLight item = node as CLight;
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Color);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.AmbientColor);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Intensity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.AmbientIntensity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.AttenuationStart);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.AttenuationEnd);
                }
            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                LeaveSequenceAtFrame_Single(from, to, keep, ga.Color);
                LeaveSequenceAtFrame_Single(from, to, keep, ga.Alpha);
            }
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    LeaveSequenceAtFrame_Single(from, to, keep, layer.Alpha);
                    LeaveSequenceAtFrame_Single(from, to, keep, layer.TextureId);
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                LeaveSequenceAtFrame_Single(from, to, keep, camera.Rotation);
            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                LeaveSequenceAtFrame_Single(from, to, keep, ta.Translation);
                LeaveSequenceAtFrame_Single(from, to, keep, ta.Rotation);
                LeaveSequenceAtFrame_Single(from, to, keep, ta.Scaling);
            }
        }

        private void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<float> animator)
        {
            if (animator.NodeList.Any(x => x.Time == keep))
            {
                var item = animator.NodeList.First(x => x.Time == keep);
                animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to);


                animator.Add(item);
                animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
            }
            else { animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to); }

        }
        private void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<CVector4> animator)
        {
            if (animator.NodeList.Any(x => x.Time == keep))
            {
                var item = animator.NodeList.First(x => x.Time == keep);
                animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to);


                animator.Add(item);
                animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
            }
            else { animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to); }

        }
        private void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<CVector3> animator)
        {
            if (animator.NodeList.Any(x => x.Time == keep))
            {
                var item = animator.NodeList.First(x => x.Time == keep);
                animator.NodeList.RemoveAll(x=>x.Time >= from && x.Time <= to);


                animator.Add(item);
                animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
            }
            else { animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to); }
            

        }
        private void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<int> animator)
        {
            if (animator.NodeList.Any(x => x.Time == keep))
            {
                var item = animator.NodeList.First(x => x.Time == keep);
                animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to);


                animator.Add(item);
                animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
            }
            else { animator.NodeList.RemoveAll(x => x.Time >= from && x.Time <= to); }

        }
        private void GetKeyframes(List<int> list_, CAnimator<int> animator) {  foreach (var frame in animator) {  if (!list_.Contains(frame.Time)) list_.Add(frame.Time); }  }
        private void GetKeyframes(List<int> list_, CAnimator<float> animator) {  foreach (var frame in animator) {  if (!list_.Contains(frame.Time)) list_.Add(frame.Time); }  }
        private void GetKeyframes(List<int> list_, CAnimator<CVector3> animator) {  foreach (var frame in animator) {  if (!list_.Contains(frame.Time)) list_.Add(frame.Time); }  }
        private void GetKeyframes(List<int> list_, CAnimator<CVector4> animator) {  foreach (var frame in animator) {  if (!list_.Contains(frame.Time)) list_.Add(frame.Time); }  }
        private List<int> GetKeyframesOfSequence(CSequence sequence)
        {
            List<int> list = new List<int>();
            foreach (INode node in CurrentModel.Nodes)
            {
                GetKeyframes(list, node.Translation);
                GetKeyframes(list, node.Rotation);
                GetKeyframes(list, node.Scaling);
                if (node is CParticleEmitter)
                {
                    CParticleEmitter item = node as CParticleEmitter;
                    GetKeyframes(list, item.EmissionRate);
                    GetKeyframes(list, item.LifeSpan);
                    GetKeyframes(list, item.InitialVelocity);
                    GetKeyframes(list, item.Gravity);
                    GetKeyframes(list, item.Longitude);
                    GetKeyframes(list, item.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 item = node as CParticleEmitter2;
                    GetKeyframes(list, item.Gravity);
                    GetKeyframes(list, item.Width);
                    GetKeyframes(list, item.Length);
                    GetKeyframes(list, item.Speed);
                    GetKeyframes(list, item.Variation);
                    GetKeyframes(list, item.Latitude);
                    GetKeyframes(list, item.EmissionRate);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter item = node as CRibbonEmitter;
                    GetKeyframes(list, item.HeightAbove);
                    GetKeyframes(list, item.HeightBelow);
                    GetKeyframes(list, item.TextureSlot);
                    GetKeyframes(list, item.Alpha);
                    GetKeyframes(list, item.Color);
                }
                if (node is CLight)
                {
                    CLight item = node as CLight;
                    GetKeyframes(list, item.Color);
                    GetKeyframes(list, item.AmbientColor);
                    GetKeyframes(list, item.Intensity);
                    GetKeyframes(list, item.AmbientIntensity);
                    GetKeyframes(list, item.AttenuationStart);
                    GetKeyframes(list, item.AttenuationEnd);
                }
            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                GetKeyframes(list, ga.Color);
                GetKeyframes(list, ga.Alpha);
            }
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    GetKeyframes(list, layer.Alpha);
                    GetKeyframes(list, layer.TextureId);
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                GetKeyframes(list, camera.Rotation);
            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                GetKeyframes(list, ta.Translation);
                GetKeyframes(list, ta.Rotation);
                GetKeyframes(list, ta.Scaling);
            }
            return list;
        }

        private void deleteinbetweenkeyframes(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;

                foreach (INode node in CurrentModel.Nodes)
                {
                    LeaveStartEnd(from, to,   node.Translation);
                    LeaveStartEnd(from, to,    node.Rotation);
                    LeaveStartEnd(from, to, node.Scaling);
                    if (node is CParticleEmitter)
                    {
                        CParticleEmitter item = node as CParticleEmitter;
                        LeaveStartEnd(from, to, item.EmissionRate);
                        LeaveStartEnd(from, to, item.LifeSpan);
                        LeaveStartEnd(from, to, item.InitialVelocity);
                        LeaveStartEnd(from, to, item.Gravity);
                        LeaveStartEnd(from, to, item.Longitude);
                        LeaveStartEnd(from, to, item.Latitude);
                    }
                    if (node is CParticleEmitter2)
                    {
                        CParticleEmitter2 item = node as CParticleEmitter2;
                        LeaveStartEnd(from, to, item.Gravity);
                        LeaveStartEnd(from, to, item.Width);
                        LeaveStartEnd(from, to, item.Length);
                        LeaveStartEnd(from, to, item.Speed);
                        LeaveStartEnd(from, to, item.Variation);
                        LeaveStartEnd(from, to, item.Latitude);
                        LeaveStartEnd(from, to, item.EmissionRate);
                    }
                    if (node is CRibbonEmitter)
                    {
                        CRibbonEmitter item = node as CRibbonEmitter;
                        LeaveStartEnd(from, to, item.HeightAbove);
                        LeaveStartEnd(from, to, item.HeightBelow);
                        LeaveStartEnd(from, to, item.TextureSlot);
                        LeaveStartEnd(from, to, item.Alpha);
                        LeaveStartEnd(from, to, item.Color);
                    }
                    if (node is CLight)
                    {
                        CLight item = node as CLight;
                        LeaveStartEnd(from, to, item.Color);
                        LeaveStartEnd(from, to, item.AmbientColor);
                        LeaveStartEnd(from, to, item.Intensity);
                        LeaveStartEnd(from, to, item.AmbientIntensity);
                        LeaveStartEnd(from, to, item.AttenuationStart);
                        LeaveStartEnd(from, to, item.AttenuationEnd);
                    }
                }
                foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
                {
                    LeaveStartEnd(from, to, ga.Color);
                    LeaveStartEnd(from, to, ga.Alpha);
                }
                foreach (CMaterial mat in CurrentModel.Materials)
                {
                    foreach (CMaterialLayer layer in mat.Layers)
                    {
                        LeaveStartEnd(from, to, layer.Alpha);
                        LeaveStartEnd(from, to, layer.TextureId);
                    }
                }
                foreach (CCamera camera in CurrentModel.Cameras)
                {
                    LeaveStartEnd(from, to, camera.Rotation);
                }
                foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
                {
                    LeaveStartEnd(from, to, ta.Translation);
                    LeaveStartEnd(from, to, ta.Rotation);
                    LeaveStartEnd(from, to, ta.Scaling);
                }

            }
        }

        private void LeaveStartEnd(int from, int to, CAnimator<CVector3> animator)  {  animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to);  }
        private void LeaveStartEnd(int from, int to, CAnimator<CVector4> animator)  {  animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to);  }
        private void LeaveStartEnd(int from, int to, CAnimator<float> animator)  {  animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to);  }
        private void LeaveStartEnd(int from, int to, CAnimator<int> animator)  {  animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to);  }

        private void CentergeosetAtNode(object sender, RoutedEventArgs e)
        {
            List<CGeoset> gesoets = GetSelectedGeosets();
            if (gesoets.Count > 0)
            {
                List<string> nodes = CurrentModel.Nodes.Select(x=>x.Name).ToList();
                Selector s = new Selector(nodes);
                s.ShowDialog();
                if (s.ShowDialog() == true)
                {
                    INode node = CurrentModel.Nodes[s.box.SelectedIndex];
                    var pos = node.PivotPoint;
                    foreach (var geoset in gesoets)
                    {
                        Calculator.CenterGeoset(geoset, pos.X, pos.Y, pos.Z);
                    }
                    
                }
            }
        }

        private void spreadv(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSelectedGeosets();
            if (geosets.Count > 0)
            {
                SpreadVerticesIWindow sw = new SpreadVerticesIWindow();
                if (sw.ShowDialog() == true)
                {
                    float distance = sw.Distance;
                    float threshold = sw.Threshold;
                    foreach (var geoset in geosets)
                    {
                        SpreadVertices(geoset, threshold, distance);
                    }
                }
                
            }
        }
        private void SpreadVertices(CGeoset geoset, float Threshold, float SetDistance)
        {
            // collect all groups of overlapping vertices
          var lists=   Calculator.FindOverlappingVertexGroups(geoset, Threshold);
            // then for each group, spread them from their collective centroid, based on the given distance

            foreach (var list in lists)
            {
                SpreadVertexGroup(list, SetDistance);
            }
        }

        private void SpreadVertexGroup(List<CGeosetVertex> list, float distance)
        {
            if (list.Count == 0)
            {
                return;
            }

            CVector3 centroid = Calculator.GetCentroidOfVertices(list);

            foreach (CGeosetVertex vertex in list)
            {
                CVector3 direction = new CVector3(
                    vertex.Position.X - centroid.X,
                    vertex.Position.Y - centroid.Y,
                    vertex.Position.Z - centroid.Z
                );

                float magnitude = Calculator. GetVectorMagnitude(direction);

                if (magnitude == 0)
                {
                    // If the vertex is exactly at the centroid, move it along an arbitrary axis
                    direction = new CVector3(1, 0, 0);
                    magnitude = 1; // Prevent division by zero
                }

                // Normalize manually
                CVector3 normalizedDirection = new CVector3(
                    direction.X / magnitude,
                    direction.Y / magnitude,
                    direction.Z / magnitude
                );

                // Push outward by the given distance
                vertex.Position = new CVector3(
                    vertex.Position.X + normalizedDirection.X * distance,
                    vertex.Position.Y + normalizedDirection.Y * distance,
                    vertex.Position.Z + normalizedDirection.Z * distance
                );
            }
        }

        private void editVisallseq(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Geosets.Count == 0) { return; }
            if (CurrentModel.Sequences.Count == 0) { return; }
            vis_v v = new vis_v(CurrentModel); v.ShowDialog();
        }
        private CMaterial CreateMaterialForTargetTexture(CTexture texture)
        {
            CMaterial mat = new CMaterial(CurrentModel);
            CMaterialLayer layer = new CMaterialLayer(CurrentModel);
            layer.TwoSided = true;
            layer.Unshaded = true;
            layer.Texture.Attach(texture);
            mat.Layers.Add(layer);
            return mat;
        }
        private void createplane_t(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem != null)
            {
                axis_picker ax = new axis_picker("Facing");
                if (ax.ShowDialog() == true)
                {
                    var selected = ax.axis;

                    CTexture texture = CurrentModel.Textures[List_Textures.SelectedIndex];
                   
                        var material = CreateMaterialForTargetTexture(texture);
                        CGeoset plane = CreatePlane(material, selected);
                        CurrentModel.Geosets.Add(plane);
                        RefreshGeosetsList();
                        RefreshViewPort();
                    
                }
            }
        }

        private CGeoset CreatePlane(CMaterial whichMaterial, Axes PlaneFacingWhichAxes)
        {
            CGeoset geoset = new CGeoset(CurrentModel);
            geoset.Material.Attach(whichMaterial);

            CGeosetVertex vertex1 = new CGeosetVertex(CurrentModel);
            CGeosetVertex vertex2 = new CGeosetVertex(CurrentModel);
            CGeosetVertex vertex3 = new CGeosetVertex(CurrentModel);
            CGeosetVertex vertex4 = new CGeosetVertex(CurrentModel);

            CGeosetTriangle triangle1 = new CGeosetTriangle(CurrentModel);
            CGeosetTriangle triangle2 = new CGeosetTriangle(CurrentModel);

            triangle1.Vertex1.Attach(vertex1);
            triangle1.Vertex2.Attach(vertex2);
            triangle1.Vertex3.Attach(vertex3);

            triangle2.Vertex1.Attach(vertex1);
            triangle2.Vertex2.Attach(vertex3);
            triangle2.Vertex3.Attach(vertex4);

            geoset.Vertices.Add(vertex1);
            geoset.Vertices.Add(vertex2);
            geoset.Vertices.Add(vertex3);
            geoset.Vertices.Add(vertex4);

            geoset.Triangles.Add(triangle1);
            geoset.Triangles.Add(triangle2);

            // Set positions, normals, and texture coordinates according to the selected axis
            switch (PlaneFacingWhichAxes)
            {
                case Axes.X:
                    vertex1.Position = new CVector3(0, -1, -1);
                    vertex2.Position = new CVector3(0, 1, -1);
                    vertex3.Position = new CVector3(0, 1, 1);
                    vertex4.Position = new CVector3(0, -1, 1);

                    vertex1.Normal = vertex2.Normal = vertex3.Normal = vertex4.Normal = new CVector3(1, 0, 0);
                    break;

                case Axes.Y:
                    vertex1.Position = new CVector3(-1, 0, -1);
                    vertex2.Position = new CVector3(1, 0, -1);
                    vertex3.Position = new CVector3(1, 0, 1);
                    vertex4.Position = new CVector3(-1, 0, 1);

                    vertex1.Normal = vertex2.Normal = vertex3.Normal = vertex4.Normal = new CVector3(0, 1, 0);
                    break;

                case Axes.Z:
                    vertex1.Position = new CVector3(-1, -1, 0);
                    vertex2.Position = new CVector3(1, -1, 0);
                    vertex3.Position = new CVector3(1, 1, 0);
                    vertex4.Position = new CVector3(-1, 1, 0);

                    vertex1.Normal = vertex2.Normal = vertex3.Normal = vertex4.Normal = new CVector3(0, 0, 1);
                    break;
            }

            // Assign texture coordinates (UV mapping)
            vertex1.TexturePosition = new CVector2(0, 0);
            vertex2.TexturePosition = new CVector2(1, 0);
            vertex3.TexturePosition = new CVector2(1, 1);
            vertex4.TexturePosition = new CVector2(0, 1);

            return geoset;
        }


    }
}


      