using MdxLib.Animator;
using MdxLib.Model;

using MdxLib.Primitives;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using W3_Texture_Finder;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;


namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentSaveLocaiton = string.Empty;
        public string CurrentSaveFolder = string.Empty;
        public CModel CurrentModel = new CModel();
        private string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private string IconsPath;
        public TextureBrowser TextureFinder;
        UVMapper Mapper;

        private ImageSource[] Textures_Loaded;
        Dictionary<string, string> IconPaths = new Dictionary<string, string>();
        List<string> Recents = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            IconsPath = System.IO.Path.Combine(AppPath, "Icons"); ;
            InitIcons();
            LoadRecents();
            // MessageBox.Show(MPQPaths.local);

            MPQFinder.Find();
            MPQHelper.Initialize();
            TextureFinder = new TextureBrowser(this, MPQHelper.Listfile_All);
            Mapper = new UVMapper();


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
        }
        public static string OpenModelFileDialog()
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





                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    CurrentModel = new CModel();
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



                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    CurrentModel = new CModel();

                    return;
                }




            }
            CurrentModel = TemporaryModel;
            RenameAllNodes();
            Box_Errors.Text = "";
            LabelDisplayInfo.Text = "";
            CurrentSaveLocaiton = FromFileName;
            CurrentSaveFolder = Path.GetDirectoryName(CurrentSaveLocaiton); ;
            MPQHelper.LocalModelFolder = CurrentSaveFolder;
            CollectTextures();
            RefreshAll();

            if (Recents.Contains(FromFileName) == false) { Recents.Add(FromFileName); SaveRecents(); RefreshRecents(); }
        }
        private void load(object sender, RoutedEventArgs e)
        {
            string FromFileName = OpenModelFileDialog();
            if (!File.Exists(FromFileName)) { return; }


            LoadModel(FromFileName);
        }
        private void RefreshAll()
        {

            ListOptions.IsEnabled = true;
            ButtonSave.IsEnabled = true;
            ButtonSaveAs.IsEnabled = true;
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
                        if (MPQHelper.FileExists(texture.FileName))
                        {
                            image.Source = MPQHelper.GetImageSource(texture.FileName);
                        }
                        else
                        {
                            string path = Path.Combine(CurrentSaveFolder, texture.FileName);
                            
                            image.Source = MPQHelper.GetImageSourceExternal(path);
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
                string looping = sequence.NonLooping ? "Nonlooping" : "looping";
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

            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                ListBoxItem Item = new ListBoxItem();
                
                int UsedMaterialIndex = CurrentModel.Materials.IndexOf(geo.Material.Object);
                TextBlock Title = new TextBlock();
               string TitleName = geo.ObjectId.ToString() + $" ({geo.Vertices.Count} vertices, {geo.Faces.Count} triangles) (material {UsedMaterialIndex})";
                Title.Text = TitleName;
                CheckBox CheckPart = new CheckBox();
                StackPanel Container = new StackPanel();
                Container.Orientation = Orientation.Horizontal;
                Container.Children.Add(CheckPart);
                Container.Children.Add(Title);
                
                Item.Content = Container;
                if (GeosetVisible.ContainsKey(geo)) { CheckPart.IsChecked = GeosetVisible[geo]; }
                else
                {
                    CheckPart.IsChecked = true;
                    GeosetVisible.Add(geo, true);
                    CheckPart.Checked += CheckedGeosetVisibility;
                    CheckPart.Unchecked += CheckedGeosetVisibility;
                }

                ListGeosets.Items.Add(Item);
            }
            CleanGeosetVisible();
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
            if (CurrentModel == null) { MessageBox.Show("Null model"); return; }
            if (System.IO.Path.GetExtension(CurrentSaveLocaiton).ToLower() == ".mdl")
            {
                string ToFileName = CurrentSaveLocaiton;
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdl();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
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

            if (System.IO.Path.GetExtension(ToFileName).ToLower() == ".mdl")
            {

                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdl();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
            }
            if (System.IO.Path.GetExtension(ToFileName).ToLower() == ".mdx")
            {

                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdx();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
            }
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
            Viewport_Main.Camera = new PerspectiveCamera
            {
                Position = new Point3D(3, 3, 3),
                LookDirection = new Vector3D(-3, -3, -3),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
            };

        }
        private void RenderGeosets(Viewport3D viewport)
        {
            DrawCube(viewport, new Point3D(0, 0, 0), 1.0, Colors.Blue);
            AdjustCam();

            return; ;
            // Clear existing children from the viewport
            viewport.Children.Clear();

            // Create a model group to hold all geosets
            var modelGroup = new Model3DGroup();

            foreach (CGeoset geoset in CurrentModel.Geosets)
            {
                // Create a mesh for the current geoset
                var mesh = new MeshGeometry3D();

                foreach (var face in geoset.Faces)
                {
                    // Add triangle vertices
                    Point3D vertex1 = new Point3D(face.Vertex1.Object.Position.X, face.Vertex1.Object.Position.Y, face.Vertex1.Object.Position.Z);
                    Point3D vertex2 = new Point3D(face.Vertex2.Object.Position.X, face.Vertex2.Object.Position.Y, face.Vertex2.Object.Position.Z);
                    Point3D vertex3 = new Point3D(face.Vertex3.Object.Position.X, face.Vertex3.Object.Position.Y, face.Vertex3.Object.Position.Z);

                    mesh.Positions.Add(vertex1);
                    mesh.Positions.Add(vertex2);
                    mesh.Positions.Add(vertex3);

                    // Add triangle indices (assume vertices are added sequentially)
                    int index = mesh.Positions.Count;
                    mesh.TriangleIndices.Add(index - 3);
                    mesh.TriangleIndices.Add(index - 2);
                    mesh.TriangleIndices.Add(index - 1);
                }

                // Create material for the geoset
                var material = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));

                // Create the geometry model
                var geometryModel = new GeometryModel3D(mesh, material);

                // Add the geometry model to the group
                modelGroup.Children.Add(geometryModel);
            }

            // Add lighting to the scene
            var light = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1));
            modelGroup.Children.Add(light);

            // Create a visual 3D for the model group
            var modelVisual = new ModelVisual3D { Content = modelGroup };

            // Add a camera if not already set
            if (viewport.Camera == null)
            {
                viewport.Camera = new PerspectiveCamera
                {
                    Position = new Point3D(0, 0, 10),
                    LookDirection = new Vector3D(0, 0, -1),
                    UpDirection = new Vector3D(0, 1, 0),
                    FieldOfView = 60
                };
            }

            // Add the model visual to the viewport
            viewport.Children.Add(modelVisual);
            AdjustCam();
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
            if (CurrentModel.Geosets.Count == 0) { Mapper.Hide(); }
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
                if (choice == 1) { } // append only
                if (choice == 2) // overwrite, dont append
                {
                    OVerwriteKeyframesForMatchingNodes(CurrentModel, TemporaryModel);
                }
                if (choice == 3)
                {
                    OVerwriteKeyframesForMatchingNodes(CurrentModel, TemporaryModel);
                }// overwrite, and append
                if (choice == 4)

                { // overwrite all
                    OverwriteWholeNodeStructure(CurrentModel, TemporaryModel);
                }
                if (choice == 5)
                {
                    ImportEmitters2AndEventObjects(CurrentModel, TemporaryModel);
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

        private void ImportEmitters2AndEventObjects(CModel currentModel, CModel temporaryModel)
        {

            foreach (INode node in temporaryModel.Nodes)
            {
                if (node is CEvent)
                {
                    CEvent ev = (CEvent)node;

                    CEvent _new = new CEvent(currentModel);
                    foreach (var item in ev.Tracks)
                    {
                        CEventTrack track = new CEventTrack(currentModel);
                        track.Time = item.Time;
                        _new.Tracks.Add(track);
                        track.Tag = item.Tag;
                    }

                }
            }
            throw new NotImplementedException();
        }

        private void OverwriteWholeNodeStructure(CModel currentModel, CModel temporaryModel)
        {
            currentModel.Nodes.Clear();
            throw new NotImplementedException();
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
        private static class IDCounter
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
            InputVector v = new InputVector(selected.PivotPoint);
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
                list.Add($"Geoset {geo.ObjectId} ({geo.Vertices.Count} vertices, {geo.Faces.Count} triangles)");
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

        }

        private CSequence GetSelectedSequence()
        {
            string s = (ListSequenes.SelectedItem as ListBoxItem).Content.ToString();
            string[] parts = s.Split('[').ToArray();
            return CurrentModel.Sequences.First(X => X.Name == parts[0].Trim());
        }

        private void EditNodeTags(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            edittags eg = new edittags(node);
            eg.ShowDialog();
            if (eg.DialogResult == true)
            {
                node.Billboarded = eg.Check_b.IsChecked == true;
                node.BillboardedLockX = eg.Check_bx.IsChecked == true;
                node.BillboardedLockY = eg.Check_by.IsChecked == true;
                node.BillboardedLockZ = eg.Check_bz.IsChecked == true;
                node.CameraAnchored = eg.Check_a.IsChecked == true;
                node.DontInheritTranslation = eg.Check_d1.IsChecked == true;
                node.DontInheritRotation = eg.Check_d2.IsChecked == true;
                node.DontInheritScaling = eg.Check_d3.IsChecked == true;
            }
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
            sb.AppendLine($"Sequences total duration: {CurrentModel.Sequences.Sum(x => x.IntervalStart + x.IntervalEnd)}");
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
            sb.AppendLine($"Total keyframes of nodes: {CountKeyframes(false, true, false)}");
            sb.AppendLine($"Total keyframes of nodes data: {CountKeyframes(false, false, true)}");
            sb.AppendLine($"Total keyframes: {CountKeyframes(true, false, false)}");
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
                count += geo.Faces.Count;
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

        private int CountKeyframes(bool others, bool nodes, bool nodesdata)
        {
            int count = 0;
            if (nodes)
            {
                foreach (INode node in CurrentModel.Nodes)
                {
                    count += node.Translation.Count;
                    count += node.Rotation.Count;
                    count += node.Scaling.Count;
                }
                return count;
            }
            if (nodesdata)
            {
                foreach (INode node in CurrentModel.Nodes)
                {
                    if (node is CAttachment)
                    {
                        CAttachment item = node as CAttachment;
                        count += item.Visibility.Count;
                    }
                    if (node is CParticleEmitter)
                    {
                        CParticleEmitter item = node as CParticleEmitter;
                        count += item.Visibility.Count;
                        count += item.EmissionRate.Count;
                        count += item.LifeSpan.Count;
                        count += item.InitialVelocity.Count;
                        count += item.Gravity.Count;
                        count += item.Longitude.Count;
                        count += item.Latitude.Count;
                    }
                    if (node is CParticleEmitter2)
                    {
                        CParticleEmitter2 item = node as CParticleEmitter2;
                        count += item.Visibility.Count;
                        count += item.Gravity.Count;
                        count += item.Width.Count;
                        count += item.Length.Count;
                        count += item.Speed.Count;
                        count += item.Variation.Count;
                        count += item.Latitude.Count;
                        count += item.EmissionRate.Count;
                    }
                    if (node is CRibbonEmitter)
                    {
                        CRibbonEmitter item = node as CRibbonEmitter;
                        count += item.Visibility.Count;
                        count += item.HeightAbove.Count;
                        count += item.HeightBelow.Count;
                        count += item.TextureSlot.Count;
                        count += item.Alpha.Count;
                        count += item.Color.Count;

                    }
                    if (node is CLight)
                    {
                        CLight item = node as CLight;
                        count += item.Visibility.Count;

                        count += item.Color.Count;
                        count += item.AmbientColor.Count;
                        count += item.Intensity.Count;
                        count += item.AmbientIntensity.Count;
                        count += item.AttenuationStart.Count;
                        count += item.AttenuationEnd.Count;

                    }
                    return count;
                }
            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                count += ga.Color.Count;
                count += ga.Alpha.Count;
            }
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    count += layer.Alpha.Count;
                    count += layer.TextureId.Count;
                }
            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                count += camera.Rotation.Count;

            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                count += ta.Translation.Count;
                count += ta.Rotation.Count;
                count += ta.Scaling.Count;
            }

            return count; ;
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
            Optimizer.Linearize = Check_Linearize.IsChecked == true;
            Optimizer.DeleteIsolatedTriangles = Check_delEmptyEO.IsChecked == true;
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
            Optimizer.AddMissingKeyframes = Check_MissingKeyframes.IsChecked == true;
            Optimizer._DetachFromNonBone = Check_DetachFromNonBone.IsChecked == true;
            Optimizer.AddOrigin = Check_AddMissingOrigin.IsChecked == true;


            // we sohudl clamp keyframes not delete them if invalid

            Optimizer.Optimize(CurrentModel);
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
            RefreshGeosetsList(); RefreshViewPort();
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
                if (st.Check_T.IsChecked == true) node.Translation.Reverse();
                if (st.Check_R.IsChecked == true) node.Rotation.Reverse();
                if (st.Check_S.IsChecked == true) node.Scaling.Reverse();

            }
        }

        private void DelNode(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            int id = node.NodeId;
            CurrentModel.Nodes.Remove(node);
            foreach (INode nod in CurrentModel.Nodes.ToList())
            {
                if (nod.Parent == null) { CurrentModel.Nodes.Remove(nod); continue; }
                if (nod.Parent.NodeId == id) { CurrentModel.Nodes.Remove(nod); continue; }

            }
            RefreshNodesTree();
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
            if (CurrentModel.Geosets.Count == 0) { Mapper.Hide(); }
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
                        node.Parent.Attach(selected);
                        RefreshNodesTree();
                    }
                }
                else { MessageBox.Show("At least one other node must be present"); return; }
            }
        }

        private void SelectAllGeosets(object sender, RoutedEventArgs e)
        {
            foreach (var item in ListGeosets.Items)
            {
                ListGeosets.SelectedItems.Add(item);
            }

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

        }

        private void clearsequencetranslations(object sender, RoutedEventArgs e)
        {

        }



        private void clearsequencerotations(object sender, RoutedEventArgs e)
        {

        }

        private void clearsequencescalings(object sender, RoutedEventArgs e)
        {

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
                    Optimizer.CalculateGeosetExtent(geoset);
                    CCollisionShape node = new CCollisionShape(CurrentModel);
                    node.Name = $"CollisionShape_{IDCounter.Next()}";
                    node.Type = ECollisionShapeType.Box;
                    node.Vertex1 = new CVector3(geoset.Extent.Min.X, geoset.Extent.Min.Y, geoset.Extent.Min.Z);
                    node.Vertex2 = new CVector3(geoset.Extent.Max.X, geoset.Extent.Max.Y, geoset.Extent.Max.Z);
                    CurrentModel.Nodes.Add(node);
                }
                RefreshNodesTree();
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
                List<CGeoset> geosets = GetSelectedGeosets();

                for (int i = 1; i < geosets.Count; i++)
                {
                    foreach (CGeosetVertex vertex in geosets[i].Vertices.ToList())
                    {
                        CGeosetVertex vertex_c = vertex;

                        geosets[i].Vertices.Remove(vertex);
                        geosets[1].Vertices.Add(vertex_c);
                    }
                    foreach (CGeosetFace face in geosets[i].Faces.ToList())
                    {
                        CGeosetFace face_C = face;
                        geosets[i].Faces.Remove(face);
                        geosets[1].Faces.Add(face);
                    }
                    foreach (var item in geosets[i].Groups.ToList())
                    {
                        var item2 = item;
                        geosets[i].Groups.Remove(item);
                        geosets[1].Groups.Add(item2);
                    }

                }
                for (int i = 1; i < geosets.Count; i++)
                {
                    DeleteGeosetAnimationOf(geosets[i]);

                }
                RefreshGeosetsList();
                RefreshGeosetAnimationsList();
            }
            else
            {
                MessageBox.Show("Select at least 2 geosets");
            }
            RefreshViewPort();
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
            InputVector iv = new InputVector();
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
            InputVector iv = new InputVector(new CVector3(100, 100, 100), "Percentage");
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X; float y = iv.Y; float z = iv.Z;
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        float x_ = vertex.Position.X * x; ;
                        float y_ = vertex.Position.Y * y;
                        float z_ = vertex.Position.Z * z;
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
            InputVector iv = new InputVector(new CVector3(0, 0, 0), "Rotation (-360-360)");
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

            foreach (CGeosetFace face in inputGeoset.Faces)
            {
                CGeosetFace _newFace = new CGeosetFace(whichModel);

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
                _newGeoset.Faces.Add(_newFace);
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
                string name = $"Geoset Animation {ga.ObjectId} of geoset {ga.Geoset.ObjectId}: Alpha: {alpha} Alphas: {ga.Alpha.Count}, Colors: {ga.Color.Count}, Uses colors: {ga.UseColor}";

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
                AttachToNewBone(ImportedGeoset);
                RefreshGeosetsList();
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


                geo = new_Geoset;


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
                AttachToNewBone(ImportedGeoset);
                RefreshGeosetsList();
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
            CGeosetGroupNode n = new CGeosetGroupNode(CurrentModel);
            n.Node.Attach(bone);
            geoset.Groups.Add(group);
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                vertex.Group.Attach(group);
            }
            RefreshNodesTree();
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
                        foreach (CGeosetFace face in geoset.Faces.ToList())
                        {
                            if (
                                geoset.Vertices.Contains(face.Vertex1.Object) == false ||
                                geoset.Vertices.Contains(face.Vertex2.Object) == false ||
                                geoset.Vertices.Contains(face.Vertex3.Object) == false


                                )
                            {
                                geoset.Faces.Remove(face);
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
            }

            Report_Node_data.Text = sb.ToString();
        }

        private void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("War3ModelTuner v1.0.8 (??/12/2024) by stan0033 built using C#, .NET 5.0, Visual Studio 2022.\n\n Would not be possible without Magos' MDXLib v1.0.4 that reads/writes Warcraft 3 MDL/MDX model format 800.");
        }

        private void EditMatTags(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                // unfinished
            }
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
                    if (MPQHelper.FileExists(texture.FileName))
                    {
                        Textures_Loaded[i]  = MPQHelper.GetImageSource(texture.FileName);
                        
                    }
                    else
                    {
                        string path = Path.Combine(CurrentSaveFolder, texture.FileName);
                        Textures_Loaded[i] = MPQHelper.GetImageSource(path);
                        
                    }
                    
                     

                }
                  if (texture.ReplaceableId == 1)
                {
                    Textures_Loaded[i] = MPQHelper.GetImageSource(TeamColor);
                   

                }
                  if (texture.ReplaceableId == 2)
                {

                    Textures_Loaded[i] = MPQHelper.GetImageSource(TeamGlow);

                }
                if (texture.ReplaceableId  > 2)
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
            // Clear existing models in the viewport
            Viewport3D viewport = Scene_Viewport;
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

            // Optionally render a ground plane

            // Render each geoset
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                if (GeosetVisible[geo] == false) { continue; }
                MeshGeometry3D mesh = new MeshGeometry3D();
                int indexOfTexture = CurrentModel.Textures.IndexOf(geo.Material.Object.Layers[0].Texture.Object);
                ImageSource texture = Textures_Loaded[indexOfTexture];
               
                bool hasAlphaTransparency = geo.Material.Object.Layers[0].FilterMode == EMaterialLayerFilterMode.Additive || geo.Material.Object.Layers[0].FilterMode == EMaterialLayerFilterMode.AdditiveAlpha;

                // Loop through each face in the geoset
                foreach (CGeosetFace face in geo.Faces)
                {
                    // Get vertex positions, normals, and texture coordinates
                    Point3D vertex1 = new Point3D(face.Vertex1.Object.Position.X, face.Vertex1.Object.Position.Y, face.Vertex1.Object.Position.Z);
                    Vector3D normal1 = new Vector3D(face.Vertex1.Object.Normal.X, face.Vertex1.Object.Normal.Y, face.Vertex1.Object.Normal.Z);
                    Point vertex1TexCoord = new Point(face.Vertex1.Object.TexturePosition.X, face.Vertex1.Object.TexturePosition.Y);

                    Point3D vertex2 = new Point3D(face.Vertex2.Object.Position.X, face.Vertex2.Object.Position.Y, face.Vertex2.Object.Position.Z);
                    Vector3D normal2 = new Vector3D(face.Vertex2.Object.Normal.X, face.Vertex2.Object.Normal.Y, face.Vertex2.Object.Normal.Z);
                    Point vertex2TexCoord = new Point(face.Vertex2.Object.TexturePosition.X, face.Vertex2.Object.TexturePosition.Y);

                    Point3D vertex3 = new Point3D(face.Vertex3.Object.Position.X, face.Vertex3.Object.Position.Y, face.Vertex3.Object.Position.Z);
                    Vector3D normal3 = new Vector3D(face.Vertex3.Object.Normal.X, face.Vertex3.Object.Normal.Y, face.Vertex3.Object.Normal.Z);
                    Point vertex3TexCoord = new Point(face.Vertex3.Object.TexturePosition.X, face.Vertex3.Object.TexturePosition.Y);

                    // Update bounding box
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex1);
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex2);
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex3);

                    // Add vertices, normals, and texture coordinates
                    mesh.Positions.Add(vertex1);
                    mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(vertex1TexCoord);

                    mesh.Positions.Add(vertex2);
                    mesh.Normals.Add(normal2);
                    mesh.TextureCoordinates.Add(vertex2TexCoord);

                    mesh.Positions.Add(vertex3);
                    mesh.Normals.Add(normal3);
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
                viewport.Children.Add(model);
            }

            // Adjust the camera to fit the geometry
            AdjustCamera(viewport, minPoint, maxPoint);
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

        private void Edutglobalsequences(object sender, RoutedEventArgs e)
        {
            EditGS_W eg = new EditGS_W(CurrentModel); eg.ShowDialog();
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
                
            }
        }
        private bool Pause = false;
        private void RefreshLayersTextureAnimList()
        {
            Combo_LayerUsedTextureAnim.Items.Clear();
            Combo_LayerUsedTextureAnim.Items.Add(new ComboBoxItem() { Content="None" });
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
            InputVector iv = new InputVector();
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
            CSequence seq = GetSelectedSequence();
            MessageBox.Show("For each node that contains animations of the first sequence, if the seocnd sequence's associates nodes contain the same amount keyframes, they will sync in time with the first sequence");
            List<string> sequences = CurrentModel.Sequences.Select(x => x.Name).ToList();
            sequences.Remove(seq.Name);
            if (sequences.Count > 1)
            {
                Selector s = new Selector(sequences);
                s.ShowDialog();
                if (s.DialogResult == true)
                {

                }
            }
            else
            {
                MessageBox.Show("At least two sequences must be present");
            }

        }

        private void changegeosetusedmaterial(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geos = GetSelectedGeosets();
            if (geos.Count == 0)
            {
                return;
            }
            List<string> materials = CurrentModel.Materials.Select(x => x.ObjectId.ToString()).ToList();
            Selector s = new Selector(materials);
            s.ShowDialog();
            if (s?.DialogResult == true)
            {
                int index = s.box.SelectedIndex;
                foreach (CGeoset geo in geos)
                {
                    geo.Material.Attach(CurrentModel.Materials[index]);
                }
                RefreshGeosetsList();
            }
        }

        private void createnode_click(object sender, RoutedEventArgs e)
        {
            createnode cr = new createnode(CurrentModel);
            cr.ShowDialog();
            if (cr.DialogResult == true)
            {
                INode SelectedNode = ListNodes.SelectedItem == null ? null : GetSeletedNode();
                NodeType type = cr.Result;
                string name = cr.ResultName;
                INode _new = null;

                if (type == NodeType.Bone) _new = new CBone(CurrentModel);
                if (type == NodeType.Helper) _new = new CHelper(CurrentModel);
                if (type == NodeType.Attachment) _new = new CAttachment(CurrentModel);
                if (type == NodeType.Ribbon) _new = new CRibbonEmitter(CurrentModel);
                if (type == NodeType.Emitter1) _new = new CRibbonEmitter(CurrentModel);
                if (type == NodeType.Emitter2) _new = new CParticleEmitter2(CurrentModel);
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
                    selected.Name = temp;
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
            Report_gsequences.Text = $"Global sequences: {CurrentModel.GlobalSequences.Count}";
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

                InputVector v = new InputVector();
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
                InputVector vector = new InputVector("Percentage");
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

                    MessageBox.Show($"to '{node.Name}' are attached geosets at indexes: " + string.Join("\n", indexes.ToArray()));
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
                InputVector vector = new InputVector("By");
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
                InputVector vector = new InputVector("Percentage");
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
            InputVector ax = new InputVector();
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

        private void showinfo(object sender, SelectionChangedEventArgs e)
        {
            if (ListOptions.SelectedIndex == 5)
            {
                ChangedInspector(null, null);
            }

        }

        private void ShowErrors()
        {
            ErrorChecker.CurrentModel = CurrentModel;
            Box_Errors.Text = ErrorChecker.Inspect(CurrentModel);
        }

        private void editvisibilitiesofgeoset(object sender, RoutedEventArgs e)
        {
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
        private bool RenderGroundPlane = false;
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
                    ea.ShowDialog();
                }
                if (node is CBone bone)
                {
                    window_editbone_data edit = new window_editbone_data(bone, CurrentModel);
                    edit.ShowDialog();
                }
                if (node is CCollisionShape cols)
                {
                    window_edit_cols edit = new window_edit_cols(cols, CurrentModel);
                    edit.ShowDialog();

                }
                if (node is CParticleEmitter emitter1)
                {
                    edit_emitter1 ei = new edit_emitter1(emitter1, CurrentModel);
                    ei.ShowDialog();

                }
                if (node is CParticleEmitter2 emitter2)
                {
                    edit_emitter2 e2 = new edit_emitter2(emitter2, CurrentModel);
                    e2.ShowDialog();

                }
                if (node is CRibbonEmitter ribbon)
                {
                    edit_ribbon er = new edit_ribbon(CurrentModel, ribbon);
                    er.ShowDialog();

                }
                if (node is CLight light)
                {
                    Edit_light el = new Edit_light(light, CurrentModel); el.ShowDialog();
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
            if (Combo_LayerUsedTexture.SelectedIndex == -1) { return; }
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                int index = List_Layers.SelectedIndex;
                int comboIndex = Combo_LayerUsedTexture.SelectedIndex;
                
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
        private bool RenderTextures = true;
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
        private void fragmentGeoset(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
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
                RefreshGeosetsList();
                RefreshGeosetAnimationsList();

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

        private void showuv(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Geosets.Count == 0) { MessageBox.Show("There are no geosets"); return; }
            Mapper.Show();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {

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

        private void EqualizeColor(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteGeoset(object sender, RoutedEventArgs e)
        {

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
            }
        }

        private void addgs(object sender, RoutedEventArgs e)
        {
            bool parse = int.TryParse(InputGSDuration.Text, out int duration);
            if (!parse) { MessageBox.Show("Expected integer"); return; }
            else
            {
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

        private void clampuvofgeoset(object sender, RoutedEventArgs e)
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
                        vertex.TexturePosition = new CVector2(X,Y);
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
                    if (geoset.Faces.Count != 2) { continue; }
                    geoset.Vertices[0].TexturePosition = new CVector2(0,0);
                    geoset.Vertices[1].TexturePosition = new CVector2(0,1);
                    geoset.Vertices[2].TexturePosition = new CVector2(1,0);
                    geoset.Vertices[3].TexturePosition = new CVector2(1,1);
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
            CBone     AttachedToBone = null;
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
                        AttachedToBone =(CBone) attachedToNode;
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

    }
}