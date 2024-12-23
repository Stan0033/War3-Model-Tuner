using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.ModelFormats;
using MdxLib.Primitives;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentSaveLocaiton = string.Empty;
        public CModel CurrentModel = new CModel();
        private string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private string IconsPath;
        Dictionary<string, string> IconPaths = new Dictionary<string, string>();
        List<string> Recents = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            IconsPath = System.IO.Path.Combine(AppPath, "Icons"); ;
            InitIcons();
            LoadRecents();
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

            CurrentSaveLocaiton = FromFileName;
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
            RefreshGeosetAnimationsList();
            RefreshViewPort();
            //   RenderGeosets(Viewport_Main);
        }
        private void RefreshTextures()
        {
            List_Textures.Items.Clear();
            foreach (CTexture texture in CurrentModel.Textures)
            {
                if (texture.ReplaceableId == 0)
                {
                    List_Textures.Items.Add(new ListBoxItem() { Content = texture.FileName });
                }
                else
                {
                    List_Textures.Items.Add(new ListBoxItem() { Content = $"Repalceable ID {texture.ReplaceableId}" });
                }
            }
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
        private void RefreshGeosetsList()
        {

            ListGeosets.Items.Clear();
            Report_Geosets.Text = $"{CurrentModel.Geosets.Count} geosets";
            foreach (CGeoset geo in CurrentModel.Geosets)
            {

                string item = geo.ObjectId.ToString() + $" ({geo.Vertices.Count} vertices, {geo.Faces.Count} triangles)";
                ListGeosets.Items.Add(new ListBoxItem() { Content = item });
            }
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

            item.Width = 200;
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
        public static void DrawCube(Viewport3D viewport, Point3D center, double size, Color color)
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
                if (CurrentModel.GeosetAnimations[i].Alpha.Static)
                {
                    CurrentModel.GeosetAnimations[i].Alpha.MakeStatic(1);
                }
            }
            RefreshGeosetAnimationsList();
        }

        private void ImportAllGeosetsOf(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are not materials. At least one material is needed to be applied to the imported geosets."); return;
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

        private void showinfo(object sender, RoutedEventArgs e)
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
            for (int i = 0; i < Stack_Optimize.Children.Count; i++)
            {
                if (Stack_Optimize.Children[i] is CheckBox)
                {
                    CheckBox c = Stack_Optimize.Children[i] as CheckBox;
                    if (c.IsEnabled) { c.IsChecked = true; }
                }
            }
        }

        private void uncheckallopts(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Stack_Optimize.Children.Count; i++)
            {
                if (Stack_Optimize.Children[i] is CheckBox)
                {
                    CheckBox c = Stack_Optimize.Children[i] as CheckBox;
                    if (c.IsEnabled) { c.IsChecked = false; }

                }
            }
        }

        private void inversecheckopts(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Stack_Optimize.Children.Count; i++)
            {
                if (Stack_Optimize.Children[i] is CheckBox)
                {
                    CheckBox c = Stack_Optimize.Children[i] as CheckBox;
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
            }
        }

        private void delTexture(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
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

        private CTexture GetSElectedTexture()
        {
            return CurrentModel.Textures[List_Textures.SelectedIndex];
        }

        private void delgeoset(object sender, RoutedEventArgs e)
        {
            foreach (var item in ListGeosets.SelectedItems)
            {
                ListBoxItem selected = item as ListBoxItem;
                string name = selected.Content.ToString();
                int id = int.Parse(name);
                CGeoset geo = CurrentModel.Geosets.First(x => x.ObjectId == id);
                CurrentModel.Geosets.Remove(geo);
            }
            RefreshGeosetsList(); RefreshViewPort();
        }

        private void creatematerialfortargettexture(object sender, RoutedEventArgs e)
        {
            if (List_Textures.SelectedItem == null) return;
            CTexture texture = GetSElectedTexture();
            CMaterial material = new CMaterial(CurrentModel);
            CMaterialLayer layer = new CMaterialLayer(CurrentModel);
            layer.Texture.Attach(texture);
            CurrentModel.Materials.Add(material);
            RefreshMaterialsList();
        }
        private CMaterial GetSelectedMAterial()
        {

            return CurrentModel.Materials[List_MAterials.SelectedIndex];
        }
        private void RefreshMaterialsList()
        {
            List_MAterials.Items.Clear();
            foreach (CMaterial material in CurrentModel.Materials)
            {
                List_MAterials.Items.Add(new ListBoxItem() { Content = material.ObjectId.ToString() + $"({material.Layers.Count} Layers)" });
            }
            Report_Mats.Text = $"{CurrentModel.Materials.Count} materials, {CurrentModel.Textures.Count} textures";

        }


        private void DelMAterial(object sender, RoutedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                CurrentModel.Materials.Remove(material);
                RefreshMaterialsList();
            }

        }

        private void creatematsforalltextures(object sender, RoutedEventArgs e)
        {
            foreach (CTexture texture in CurrentModel.Textures)
            {
                CMaterial material = new CMaterial(CurrentModel);
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                layer.Texture.Attach(texture);
                CurrentModel.Materials.Add(material);

            }
            RefreshMaterialsList();
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
        private List<CGeoset> GetSElectedGeosets()
        {
            List<CGeoset> list = new List<CGeoset>();
            for (int i = 0; i < ListGeosets.Items.Count; i++)
            {
                if (ListGeosets.SelectedItems.Contains(ListGeosets.Items[i]))
                {
                    list.Add(CurrentModel.Geosets[i]);
                }
            }
            return list;
        }

        private void createColsForTargetGeo(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {

                List<CGeoset> geosets = GetSElectedGeosets();
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
            List<CGeoset> geosets = GetSElectedGeosets();
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
                List<CGeoset> geosets = GetSElectedGeosets();

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
                    CurrentModel.Geosets.Remove(geosets[i]);
                }
                RefreshGeosetsList();
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
                List<CGeoset> geosets = GetSElectedGeosets();
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
                List<CGeoset> geosets = GetSElectedGeosets();
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
                List<CGeoset> geosets = GetSElectedGeosets();
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
            List<CGeoset> geosets = GetSElectedGeosets();
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
                List<CGeoset> geosets = GetSElectedGeosets();

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
                List<CGeoset> geosets = GetSElectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.RotateGeoset(geoset, x, y, z);
                }
            }
            RefreshViewPort();
        }

        private void aligngeosets(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSElectedGeosets();
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
                if (VertexReference.ContainsKey(face.Vertex1.Object) == true)
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
                    _newFace.Vertex1.Attach(_newVertex);
                    VertexReference.Add(face.Vertex1.Object, _newVertex);
                    _newGeoset.Vertices.Add(_newVertex);

                }
                if (VertexReference.ContainsKey(face.Vertex2.Object) == true)
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
                    _newFace.Vertex1.Attach(_newVertex);
                    VertexReference.Add(face.Vertex2.Object, _newVertex);
                    _newGeoset.Vertices.Add(_newVertex);

                }
                if (VertexReference.ContainsKey(face.Vertex3.Object) == true)
                {
                    _newFace.Vertex1.Attach(VertexReference[face.Vertex3.Object]);

                }
                else
                {
                    CGeosetVertex _newVertex = new CGeosetVertex(whichModel);
                    _newVertex.Position = new CVector3(face.Vertex3.Object.Position);
                    _newVertex.TexturePosition = new CVector2(face.Vertex3.Object.TexturePosition);
                    _newVertex.Normal = new CVector3(face.Vertex3.Object.Normal);
                    _newVertex.Group.Attach(group);
                    _newFace.Vertex1.Attach(_newVertex);
                    VertexReference.Add(face.Vertex3.Object, _newVertex);
                    _newGeoset.Vertices.Add(_newVertex);

                }
                _newGeoset.Faces.Add(_newFace);
            }
            _newGeoset.Material.Attach(whichModel.Materials[0]);



            return _newGeoset;
        }

        private void reattachToBone(object sender, RoutedEventArgs e)
        {
            List<CGeoset> geosets = GetSElectedGeosets();
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
            List<CGeoset> geosets = GetSElectedGeosets();
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
                string name = $"Geoset Animation {ga.ObjectId} of geoset {ga.Geoset.ObjectId}: Alpha: {ga.Alpha.Count}, Color: {ga.Color.Count}, Uses color: {ga.UseColor}";

                List_GeosetAnims.Items.Add(new ListBoxItem() { Content = name });
            }
            Label_GAs.Text = $"Geoset animations - {CurrentModel.GeosetAnimations.Count}";
        }

        private void AverageNormals(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosts = GetSElectedGeosets();
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
                List<CGeoset> geosts = GetSElectedGeosets();
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
            ImportTargetGeosetOne(file);


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
            ImportTargetGeosetOne(file);
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Translations: {node.Translation.Count},");
            sb.AppendLine($" Rotations: {node.Rotation.Count}, ");
            sb.AppendLine($" Scalings: {node.Scaling.Count}, ");
            sb.AppendLine($" Billboarded: {node.Billboarded}, ");
            sb.AppendLine($" BillboardedX: {node.BillboardedLockX}, ");
            sb.AppendLine($" BillboardedY: {node.BillboardedLockY}, ");
            sb.AppendLine($" BillboardedZ: {node.BillboardedLockZ}, ");
            sb.AppendLine($" CamAnchored: {node.CameraAnchored}, ");
            sb.AppendLine($" DontInheritTranslation: {node.DontInheritTranslation}, ");
            sb.AppendLine($" DontInheritRotation: {node.DontInheritRotation}, ");
            sb.AppendLine($" DontInheritScaling: {node.DontInheritScaling}, ");
            Report_Node_data.Text = sb.ToString();
        }

        private void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("War3ModelTuner v1.0.q (23/12/2024) by stan0033 built using C#, .NET 3.5, Visual Studio 2022.\n\n Would not be possible without Magos' MDXLib v1.0.4 that reads/writes Warcraft 3 MDL/MDX model format 800.");
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

            }

        }

        private void Scene_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Ensure the camera is a PerspectiveCamera
            if (Scene_Viewport.Camera is PerspectiveCamera camera)
            {
                // Get the zoom factor from the mouse wheel delta
                double zoomFactor = e.Delta > 0 ? 0.9 : 1.1; // Zoom in on scroll up, out on scroll down

                // Calculate the new position by moving the camera along its LookDirection
                Vector3D lookDirection = camera.LookDirection;
                Point3D newPosition = camera.Position + lookDirection * zoomFactor;

                // Update the camera position
                camera.Position = newPosition;
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
                Color =  Colors.White,
                Direction = new Vector3D(-1, -1, -1)
            };
            ModelVisual3D lightModel = new ModelVisual3D { Content = light };
            viewport.Children.Add(lightModel);

            // Render each geoset
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                MeshGeometry3D mesh = new MeshGeometry3D();

                foreach (CGeosetFace face in geo.Faces)
                {
                    // Get vertex positions
                    Point3D vertex1 = new Point3D(face.Vertex1.Object.Position.X, face.Vertex1.Object.Position.Y, face.Vertex1.Object.Position.Z);
                    Point3D vertex2 = new Point3D(face.Vertex2.Object.Position.X, face.Vertex2.Object.Position.Y, face.Vertex2.Object.Position.Z);
                    Point3D vertex3 = new Point3D(face.Vertex3.Object.Position.X, face.Vertex3.Object.Position.Y, face.Vertex3.Object.Position.Z);

                    // Update bounding box
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex1);
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex2);
                    UpdateBoundingBox(ref minPoint, ref maxPoint, vertex3);

                    // Add vertices
                    mesh.Positions.Add(vertex1);
                    mesh.Positions.Add(vertex2);
                    mesh.Positions.Add(vertex3);

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
                else{
                      material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
                }
                

                // Create a GeometryModel3D
                GeometryModel3D geometryModel = new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = material
                };

                // Add the model to the viewport
                ModelVisual3D model = new ModelVisual3D { Content = geometryModel };
                viewport.Children.Add(model);
            }

            // Adjust the camera to fit the geometry
            AdjustCamera(viewport, minPoint, maxPoint);
            SliderRotation.Value = 0;
            SliderZoom.Value = 0;

         
        }
        //----------------------------------------------------
        private int rotation_angle = 0;
        private int CurrentAngle
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
        private void SetSceneCameraRotationAroundCenter()
        {
            CurrentAngle = (int)SliderRotation.Value;


            UpdateCameraPosition(Scene_Viewport, lastZoom, CurrentAngle);
        }
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

        private void UpdateCameraPosition(Viewport3D viewport, int zoom, int rotation)
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


        private void SliderRotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentModel.Geosets.Count == 0) return;
            CurrentAngle = (int) SliderRotation.Value;
            AdjustRotation(CurrentAngle);
            
        }

        private void AdjustRotation(int currentAngle)
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


        private int lastZoom = 0;
        private void Zoomed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentModel.Geosets.Count == 0) return;
            int zoom = (int)SliderZoom.Value;
            if (zoom > lastZoom)
            {
                AdjustZoom(10);
             
            }
            if (zoom < lastZoom)
            {
                AdjustZoom(-10);
                
            }
           
            lastZoom = zoom;    
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
            SelectedGeosets_Collection = GetSElectedGeosets();
            RefreshViewPort();
            
        }

         
    }
}