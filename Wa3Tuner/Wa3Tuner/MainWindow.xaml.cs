using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using SharpGL;

using SharpGL.SceneGraph.Assets;

using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

using W3_Texture_Finder;
using Wa3Tuner.Dialogs;
using Wa3Tuner.Helper_Classes;
using Wa3Tuner.Helper_Classes.Parsers;

using static Wa3Tuner.Helper_Classes.PathManager;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

using Ray = Wa3Tuner.Helper_Classes.Ray;


using System.Globalization;
using System.Xml.Linq;
using Path = System.IO.Path;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls.Primitives;
using System.Collections;
using SharpGL.SceneGraph.Primitives;
using Whim_GEometry_Editor.Misc;
using System.Windows.Documents;
namespace Wa3Tuner
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //--------------------------------------------------------------------------------
        //---VALUES
        //--------------------------------------------------------------------------------

        private bool CanDrag = false;
        private bool MaximizeOnStart = false;
        private float ZoomIncrement = 1;
        private bool OptimizeOnSave = false;
        private bool Saved = false;
        private bool ColorizeTransformations = false;
        private string DefaultAuthor = "";
        MiniUV? MiniUVMapper;
        public string CurrentSaveLocation = string.Empty;
        public string CurrentSaveFolder = string.Empty;
        public CModel CurrentModel = new CModel();
        private string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private string IconsPath = "";
        List<string> TeamColorPaths = new();
        List<string> TeamGlows = new();
        private ModifyMode modifyMode_current = ModifyMode.Translate;
        private CGeosetAnimation? CopiedAnimation;
        private double LastClickPositionX = 0;
        private double LastClickPositionY = 0;
        private CSequence? CurrentlySelectedSequenceInAnimator;
        private debug_console_w Debug_Console = new debug_console_w();
        private Gradual_Visibility_Maker GradualVisibilityMaker;
        //UVMapper mapper = new UVMapper();
        private Axes axisMode = Axes.X;
        Dictionary<MenuItem, int> TeamReference = new();
        int CurrentTeamColor = 0;
        public TextureBrowser? TextureFinder;
        System.Timers.Timer? PlayTimer;

        private INode? CopiedNode;
        Dictionary<string, string> IconPaths = new Dictionary<string, string>();
        List<string> Recents = new();
        Texture? GroundTexture;
        mSelectionMode SelectionMode = mSelectionMode.Clear;
        private INode? SelectedNodeInAnimator;
        private CVector3? CopiedPathNodePosition;

        private bool ViewingPaths = false;
        private INode? Animator_CopiedNode;
        public static CSequence? DuplicatingFromSequence;
        public static CSequence? DuplicatingToSequence;
        private CVector3? CopiedTrianglePosition1;
        private CVector3? CopiedTrianglePosition2;
        private CVector3? CopiedTrianglePosition3;
        private CSequence? CopiedAnimatorSequence;
        private TransformationType CopiedAnimatorSequenceType;
        private CSequence? CopiedSequence;
        private CGeoset? CopiedRigGeoset;
        private PlayMode playMode = PlayMode.Default;
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        public MainWindow(debug_console_w debug_Console, string file = "")
        {
            Pause = true;
            InitializeComponent();
            Initialize();
            if (file.Length > 0) LoadModel(file);
            Pause = false;
            Debug_Console = debug_Console;
        }
        private bool CalledInitialize = false;
        public void Initialize()
        {
            if (CalledInitialize) return;
            CalledInitialize = true;

            IconsPath = System.IO.Path.Combine(AppPath, "Icons"); ;
            InitIcons();
            LoadRecents();
            // MessageBox.Show(MPQPaths.local);
            MPQFinder.Find();
            MPQHelper.Initialize();
            TextureFinder = new TextureBrowser(this, MPQHelper.Listfile_All);
            InitializeTeamColorPaths();
            NodeCollection = new NodeMaker();
            InitializePlayTimer();
            FillRenderItemsCollection();
            GradualVisibilityMaker = new(CurrentModel, this);
        }
        private void InitializePlayTimer()
        {
            PlayTimer = new System.Timers.Timer();
            PlayTimer.Interval = 1;
            PlayTimer.Enabled = false;
            PlayTimer.Elapsed += PlayTimer_Cycle;
        }


        private void PlayTimer_Cycle(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RefreshAnimatedVertexAndNodePositionsForRendering(Playback.Next);
                Label_Playback_currentTrack.Text = Playback.Track.ToString();
            });
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
            TeamColorPaths = new();
            TeamGlows = new();
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
        private void ClickedTeamColor(object? sender, EventArgs e)
        {
            MenuItem? item = sender as MenuItem;
            if (item == null) return;
            CurrentTeamColor = TeamReference[item];
            CollectTexturesOpenGL();
        }



        private void LoadRecents()
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "Paths\\recents.txt");
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
            string file = System.IO.Path.Combine(local, "Paths\\recents.txt");
            File.WriteAllText(file, "");
            File.WriteAllLines(file, Recents.ToArray());
            RefreshRecents();
        }
        private void OpenRecent(object? sender, EventArgs e)
        {
            MenuItem? item = sender as MenuItem; if (item == null) return;
            string? name = item.Header.ToString();
            if (name == null) { MessageBox.Show("Null string"); return; }
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
            IconPaths.Add(nameof(CHelper), System.IO.Path.Combine(IconsPath, "helper.png"));
            IconPaths.Add(nameof(CLight), System.IO.Path.Combine(IconsPath, "light.png"));
            IconPaths.Add(nameof(CRibbonEmitter), System.IO.Path.Combine(IconsPath, "emitter3.png"));
            IconPaths.Add("lock", System.IO.Path.Combine(IconsPath, "Lock.png"));
            IconPaths.Add("unlock", System.IO.Path.Combine(IconsPath, "Unlocked.png"));
            IconPaths.Add("ground", System.IO.Path.Combine(IconsPath, "grass.png"));
            ButtonNegateVertices.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "negate.png")) };
            ButtonNegateTriangles.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "negate.png")) };
            ButtonCreateTriangle.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "triangle.png")) };
            ButtonDeleteVertex.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "delete.png")) };
            ButtonDelTriangles.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "delete.png")) };
            ButtonUncouple.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "uncouple.png")) };
            ButtonWeld.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "weld.png")) };


            ButtonCollapseV.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "collapse.png")) };
            ButtonCollapseTriangleVertices.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "collapse.png")) };


            ButtonCollapseVG.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "collapse_g.png")) };
            ButtonCollapseTriangleVertices2.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "collapse_g.png")) };


            ButtonCeiling.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "ceiling.png")) };
            ButtonFloor.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "floor.png")) };
            ButtonFlattenV.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "flatten.png")) };
            ButtonFlattenTriangles.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "flatten.png")) };
            ButtonDisperse.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "disperse.png")) };
            ButtonMirror.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "mirror.png")) };
            ButtonSplitVertex.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "split_v.png")) };
            ButtonArrangeV.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "arrange.png")) };
            ButtonSwapVertices.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "swap_v.png")) };
            ButtonEditTrianglesUV.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "uv.png")) };
            ButtonSubdivide.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "subdivide.png")) };
            ButtonInset.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "inset.png")) };
            ButtonInsetConnected.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "inset2.png")) };
            ButtonMirror.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "mirror.png")) };
            ButtonMirrorTriangles.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "mirror.png")) };
            ButtonSimplify.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "simplify.png")) };
            ButtonSwaptriangles.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "swap_t.png")) };
            ButtonReattachVertex.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "swaptv.png")) };
        }
        public static void SetBackgroundImage(Button button, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || button == null)
                return;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            brush.Stretch = Stretch.UniformToFill; // Ensures the image fills the button properly
            button.Background = brush;
        }
        private void LoadModel(string? FromFileName)
        {
            if (FromFileName == null) return;
            if (!File.Exists(FromFileName))
            {
                MessageBox.Show("File was not found at that location"); return;
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
                        CurrentSaveLocation = FromFileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    // CurrentModel = new CModel();
                    return;
                }
            }
            else if (extension == ".mdl")
            {
                try
                {
                    using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdl();
                        ModelFormat.Load(FromFileName, Stream, TemporaryModel);
                        CurrentSaveLocation = FromFileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    //  CurrentModel = new CModel();
                    return;
                }
            }

            else if (extension == ".json")
            {
                CModel? temp = JsonFormat.Load(); if (temp == null) return;
                if (temp != null) { CurrentModel = temp; }
            }

            CurrentModel = TemporaryModel;
            MultiplyAlphasForEmitter2_MDL();
            RenameAllNodes();
            Optimizer.RemoveInvalidGeosetAnimations(CurrentModel);
            Optimizer.HandleInvalidTextures(CurrentModel);
            Optimizer.ArrangeGEosetAnimations(CurrentModel);

            Box_Errors.Document.Blocks.Clear();
            LabelDisplayInfo.Text = "";
            CurrentSaveLocation = FromFileName;
            string? folder = Path.GetDirectoryName(CurrentSaveLocation);
            CurrentSaveFolder = folder == null ? string.Empty : folder;
            MPQHelper.LocalModelFolder = CurrentSaveFolder;

            RefreshRenderData(null, null);
            RefreshAll();
            CParticleEmitter2? emitter = CurrentModel.Nodes[0] as CParticleEmitter2;
            if (Recents.Contains(FromFileName) == false) { Recents.Add(FromFileName); SaveRecents(); RefreshRecents(); }
            SetSaved(false);
        }

        private void SetSaved(bool v)
        {
            Saved = v;
            RefreshTitle();
        }

        private void CalculateGeosetExtents()
        {
            foreach (var geoset in CurrentModel.Geosets) Optimizer.CalculateGeosetExtent(geoset);
            SetSaved(false);
        }
        private void load(object? sender, RoutedEventArgs? e)
        {
            string? FromFileName = FileSeeker.OpenModelFileDialog();
            if (!File.Exists(FromFileName)) { return; }
            LoadModel(FromFileName);
        }
        private void MultiplyAlphasForEmitter2_MDL()
        {
            return;
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
        private void RefreshTitle()
        {
            if (Saved) Title = "War3 Model Tuner - " + CurrentSaveLocation + $" ({CurrentModel.Name})";
            else Title = "War3 Model Tuner - " + CurrentSaveLocation + $" ({CurrentModel.Name}) [Unsaved]";
        }
        private void RefreshAll()
        {
            ListOptions.IsEnabled = true;
            RefreshTitle();
            RefreshGeosetsList();
            RefreshNodesTree();
            RefreshSequencesList();
            RefreshTexturesList();
            RefreshMaterialsList();
            RefreshTextureAnims();
            RefreshGlobalSequencesList();
            List_Layers.Items.Clear();
            RefreshGeosetAnimationsList();
            RefreshLayersTextureList();
            RefreshLayersTextureAnimList();

            RefreshPath_ModelNodes_List();
            RefreshSequencesList_Paths();
            //   RenderGeosets(Viewport_Main);
        }
        public void RefreshTexturesList()
        {
            List_Textures.Items.Clear();
            foreach (CTexture texture in CurrentModel.Textures)
            {
                ListBoxItem item = new ListBoxItem() { Content = texture.FileName };
                Image image = new Image();
                string name = $"Repalceable ID {texture.ReplaceableId}";
                if (texture.ReplaceableId == 0)
                {

                    if (texture.ReplaceableId == 0)
                    {
                        name = texture.FileName;
                        string extension = System.IO.Path.GetExtension(texture.FileName).ToLower();

                        if (extension == ".blp")
                        {
                            image.Source = MPQHelper.GetImageSource(texture.FileName);
                            if (image.Source == null)
                            {
                                string? path = PathMaker.Make(CurrentSaveFolder, texture.FileName);
                                image.Source = MPQHelper.GetImageSourceExternal(path);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Could not load {texture.FileName}, because it is not a BLP image");
                            image.Source = MPQHelper.GetImageSource(White);
                        }
                    }

                }

                else if (texture.ReplaceableId == 1)
                {
                    image.Source = MPQHelper.GetImageSource(TeamColor);
                    name = "Team Color";
                }
                else if (texture.ReplaceableId == 2)
                {
                    image.Source = MPQHelper.GetImageSource(TeamGlow);
                    name = "Team Glow";
                }
                else if (texture.ReplaceableId > 2)
                {
                    image.Source = MPQHelper.GetImageSource(White);
                }





                item = new ListBoxItem() { Content = name };
                item.ToolTip = image;

                List_Textures.Items.Add(item);
            }
            LabelTextues.Text = $"Textures - {CurrentModel.Textures.Count}";
        }
        public void RefreshSequencesList()
        {
            ListSequenes.Items.Clear();
            Report_sequences.Text = $"{CurrentModel.Sequences.Count} sequences";
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                string looping = sequence.NonLooping ? "Nonlooping" : "Looping";
                string data = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}] ({looping})";

                Brush bg = GetColorOfSequenceItem(sequence);

                ListSequenes.Items.Add(new ListBoxItem() { Content = data, Background = bg });
            }
        }
        private Brush GetColorOfSequenceItem(CSequence seq)
        {
            if (!ColorizeTransformations) { return Brushes.Transparent; }
            bool t = false;
            bool r = false;
            bool s = false;

            foreach (var item in CurrentModel.Nodes)
            {
                if (item.Translation.Any(x => x.Time >= seq.IntervalStart && x.Time <= seq.IntervalEnd))
                    t = true;
                if (item.Rotation.Any(x => x.Time >= seq.IntervalStart && x.Time <= seq.IntervalEnd))
                    r = true;
                if (item.Scaling.Any(x => x.Time >= seq.IntervalStart && x.Time <= seq.IntervalEnd))
                    s = true;
            }

            // Count active transformation types
            var active = new List<Color>();
            if (t) active.Add(Colors.Green);
            if (r) active.Add(Colors.Yellow);
            if (s) active.Add(Colors.Red);

            // No transformations at all
            if (active.Count == 0)
                return Brushes.Transparent;

            // Only one transformation – return solid color
            if (active.Count == 1)
                return new SolidColorBrush(active[0]);

            // Two or more – return horizontal gradient
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0) // Horizontal
            };

            double step = 1.0 / (active.Count - 1);
            for (int i = 0; i < active.Count; i++)
            {
                brush.GradientStops.Add(new GradientStop(active[i], i * step));
            }

            return brush;
        }

        private Brush GetColorOfNode(INode seq)
        {
            if (ColorizeTransformations) { return Brushes.Transparent; }
            bool t = false;
            bool r = false;
            bool s = false;


            if (seq.Translation.Any()) t = true;
            if (seq.Rotation.Any()) r = true;
            if (seq.Scaling.Any()) s = true;

            // Count active transformation types
            var active = new List<Color>();
            if (t) active.Add(Colors.Green);
            if (r) active.Add(Colors.Yellow);
            if (s) active.Add(Colors.Red);

            // No transformations at all
            if (active.Count == 0)
                return Brushes.Transparent;

            // Only one transformation – return solid color
            if (active.Count == 1)
                return new SolidColorBrush(active[0]);

            // Two or more – return horizontal gradient
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0) // Horizontal
            };

            double step = 1.0 / (active.Count - 1);
            for (int i = 0; i < active.Count; i++)
            {
                brush.GradientStops.Add(new GradientStop(active[i], i * step));
            }

            return brush;
        }

        private void RefreshSequencesList_Player()
        {
            //tobeused
            ListSequences_Play.Items.Clear();
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                string looping = sequence.NonLooping ? "Nonlooping" : "Looping";
                string data = $"{sequence.Name} ({looping}) [{sequence.IntervalStart} - {sequence.IntervalEnd}]";
                ListSequences_Play.Items.Add(new ListBoxItem() { Content = data });
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
        public void RefreshGeosetsList()
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
                string TitleName = geo.ObjectId.ToString() + $" ({geo.Name}) ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles) (material {UsedMaterialIndex}) (Groups {geo.Groups.Count})";
                Title.Text = TitleName;
                CheckBox CheckPart = new CheckBox();
                StackPanel Container = new StackPanel();
                Item.MouseLeave += (object? sender, MouseEventArgs e) =>
                {
                    if (Item.ToolTip is ToolTip toolTip)
                    {
                        toolTip.IsOpen = false;
                    }
                };
                Container.Orientation = Orientation.Horizontal;
                Container.Children.Add(CheckPart);
                Container.Children.Add(Title);
                CheckPart.IsChecked = true;
                //GeosetVisible.Add(geo, true);
                CheckPart.Checked += (object? sender, RoutedEventArgs? e) => geo.isVisible = true; ;
                CheckPart.Unchecked += (object? sender, RoutedEventArgs? e) => geo.isVisible = false; ;
                Item.Content = Container;
                ListGeosets.Items.Add(Item);
            }
        }
        private void CheckedGeosetVisibility(object? sender, EventArgs? e)
        {
            if (sender is CheckBox == false) { return; }
            int index = 0;
            ListBoxItem? item = null;
            CheckBox? CheckedBox = sender as CheckBox;
            if (CheckedBox == null) return;
            for (int i = 0; i < ListGeosets.Items.Count; i++)
            {

                ListBoxItem? box = ListGeosets.Items[i] as ListBoxItem; if (box == null) return;
                StackPanel? p = box.Content as StackPanel; if (p == null) return;
                CheckBox? c = p.Children[0] as CheckBox; if (c == null) return;
                if (c == CheckedBox) { item = box; index = i; break; }
            }
            if (item == null) { return; }
            bool visible = CheckedBox.IsChecked == true;
            CGeoset geoset = CurrentModel.Geosets[index];
            GeosetVisible[geoset] = visible;

        }
        public void RefreshNodesTree()
        {
            ListNodes.Items.Clear();
            Report_Nodes.Text = $"{CurrentModel.Nodes.Count} nodes";
            foreach (INode node in CurrentModel.Nodes)
            {

                if (!hasParent(node))
                { // root
                    TreeViewItem item = GetTreeViewItem(node);
                    Brush bg = GetColorOfNode(node);
                    item.Background = bg;
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
                    Brush bg = !ColorizeTransformations ? Brushes.Transparent : GetColorOfNode(targetNode);
                    newItem.Background = bg;
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
            item.Width = 450;
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
        private static ImageSource LoadImageSource(string filePath)
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
        private void SaveModel(object? sender, RoutedEventArgs? e)
        {
            if (CurrentSaveLocation.Length == 0) { saveas(null, null); return; }
            string ext = System.IO.Path.GetExtension(CurrentSaveLocation).ToLower();
            if (
               (ext == ".mdx" || ext == ".mdl" || ext == ".json") == false
                ) { MessageBox.Show("Invalid extension"); return; }
            if (CurrentModel == null) { MessageBox.Show("Null model"); return; }
            if (OptimizeOnSave) OptimizeModelForSaving();
            if (ext == ".mdl")
            {
                string ToFileName = CurrentSaveLocation;
                DivideAlphasForEmitter2_MDL();
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdl();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
                FileCleaner.CleanFile(ToFileName);
                MultiplyAlphasForEmitter2_MDL();
            }
            if (ext == ".mdx")
            {
                string ToFileName = CurrentSaveLocation;
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdx();
                    ModelFormat.Save(ToFileName, Stream, CurrentModel);
                }
            }
            if (ext == ".json")
            {
                JsonFormat.Save(CurrentModel);
            }
            if (OptimizeOnSave) refreshalllists(null, null);
            SetSaved(true);
        }
        private void OptimizeModelForSaving()
        {
            Optimizer.FalseAll();
            Optimizer.DeleteUnusedGlobalSequences = true;
            Optimizer.DeleteUnusedBones = true;
            Optimizer.DeleteUnusedHelpers = true;
            Optimizer.DeleteUnusedMAterials = true;
            Optimizer.DeleteUnusedTextures = true;
            Optimizer.DeleteUnusedTextureAnimations = true;
            Optimizer.DeleteUnusedKeyframes = true;
            Optimizer.DelUnusedMatrixGroups = true;
            Optimizer.FixNoMatrixGroups = true;
            Optimizer.Optimize(CurrentModel);

        }
        private void saveas(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel == null) { MessageBox.Show("Null model"); return; }
            string ToFileName = FileSeeker.ShowSaveFileDialog();
            if (ToFileName.Length == 0) return;
            CurrentSaveLocation = ToFileName; SaveModel(null, null);
            RefreshTitle();
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
            System.Windows.Media.Media3D.Material material = new DiffuseMaterial(new SolidColorBrush(color));
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

        private void delcameras(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.Cameras.Clear();
            SetSaved(false);
        }
        private void delsequences(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.Sequences.Clear();
            RefreshSequencesList(); SetSaved(false);
        }
        private void delgss(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.GlobalSequences.Clear();
            RefreshGlobalSequencesList();
            SetSaved(false);
        }
        private void delgeosets(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.Geosets.Clear(); SetSaved(false);
            RefreshGeosetsList();
        }
        private void deltxa(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.TextureAnimations.Clear(); SetSaved(false)
               ; RefreshTextureAnims();
        }
        private void resetallgas(object? sender, RoutedEventArgs? e)
        {
            foreach (var gs in CurrentModel.GeosetAnimations)
            {
                gs.Alpha.MakeStatic(1);
            }
            SetSaved(false);
        }
        private void MakeAllGAAlphaStatic(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                CurrentModel.GeosetAnimations[i].Alpha.Clear();
                CurrentModel.GeosetAnimations[i].Alpha.MakeStatic(1);
            }
            RefreshGeosetAnimationsList(); SetSaved(false);
        }
        private void ImportAllGeosetsOf(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no materials. At least one material is needed to be applied to the imported geosets."); return;
            }
            {
                CModel? TemporaryModel = GetTemporaryModel();
                if (TemporaryModel == null) { return; }
                Pause = true;
                foreach (CGeoset geoset in TemporaryModel.Geosets)
                {
                    CGeoset new_Geoset = DuplicateGeogeset(geoset, CurrentModel);
                    CurrentModel.Geosets.Add(new_Geoset);
                }
                RefreshGeosetsList();
                RefreshNodesTree();
                RefreshRenderData(null, null);
                Pause = false;
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List(); SetSaved(false);
            }
        }
        private bool CheckSaved()
        {
            if (!Saved)
            {
                var Result = MessageBox.Show(
                    "The model is not saved. Save?",
                    "Wait...",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (Result == MessageBoxResult.Yes)
                {
                    SaveModel(null, null);
                    return true;
                }
                if (Result == MessageBoxResult.No)
                {
                    return true;
                }
                if (Result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }
        private CModel? GetTemporaryModel(string file = "")
        {
            CModel model = new CModel();
            string? FromFileName = file;
            if (FromFileName == "")
            {
                FromFileName = FileSeeker.OpenModelFileDialog();
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
        private void ImportAllNodesOf(object? sender, RoutedEventArgs? e)
        {
            import_nodes_choice i = new import_nodes_choice();
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                int choice = i.selected;
                CModel? TemporaryModel = GetTemporaryModel();
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
                RefreshNodesTree(); SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
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
        private static void OVerwriteKeyframesForMatchingNodes(CModel currentModel, CModel temporaryModel)
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
        private void ImportAnimations(object? sender, RoutedEventArgs? e)
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
            CModel? TemporaryModel = GetTemporaryModel();
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
        private void RemoveAllLights(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void RemoveEmitters1(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void RemoveEmitters2(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void RemoveAllAttachments(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void RemoAllCOLS(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void removeAllHelpers(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void RemoveAllEventObjects(object? sender, RoutedEventArgs? e)
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
            RefreshNodesTree();
            SetSaved(false);
        }
        private void RemoveAllAnimations(object? sender, RoutedEventArgs? e)
        {
            foreach (INode node in CurrentModel.Nodes.ToList())
            {
                node.Translation.Clear();
                node.Rotation.Clear();
                node.Scaling.Clear();
            }
            SetSaved(false);
        }
        private string GetNodeName()
        {
            TreeViewItem? item = (ListNodes.SelectedItem as TreeViewItem); if (item == null) return string.Empty;
            StackPanel? s = item.Header as StackPanel; if (s == null) return string.Empty;
            TextBlock? t = (TextBlock)s.Children[1]; if (t == null) return string.Empty;
            return t.Text;
        }
        private string GetNodeNameAnimator()
        {
            TreeViewItem? item = (List_Nodes_Animator.SelectedItem as TreeViewItem); if (item == null) return string.Empty;
            StackPanel? s = item.Header as StackPanel; if (s == null) return string.Empty;
            TextBlock? t = (TextBlock)s.Children[1]; if (t == null) return string.Empty;
            return t.Text;
        }
        private void RenameNode(object? sender, RoutedEventArgs? e)
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
                TreeViewItem? item = ListNodes.SelectedItem as TreeViewItem; if (item == null) return;
                StackPanel? s = item.Header as StackPanel; if (s == null) return;
                TextBlock? t = s.Children[1] as TextBlock; if (t == null) return;
                t.Text = name;
            }
        }
        private void RenameAllNodes()
        {
            List<string> ExistingNames = new();
            foreach (INode node in CurrentModel.Nodes)
            {
                if (ExistingNames.Contains(node.Name))
                {
                    node.Name = node.Name + "_" + IDCounter.Next_;
                }
                ExistingNames.Add(node.Name);
            }
        }
        private INode GetSeletedNode()
        {
            string name = GetNodeName();
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void SetPivotPoint(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode selected = GetSeletedNode();
            InputVector v = new InputVector(AllowedValue.Both, selected.PivotPoint);
            v.ShowDialog();
            if (v.DialogResult == true)
            {
                selected.PivotPoint = new MdxLib.Primitives.CVector3(v.X, v.Y, v.Z);
            }
            SetSaved(false);
        }
        private CGeoset GetGeoset(string input)
        {
            int id = int.Parse(input);
            return CurrentModel.Geosets.First(x => x.ObjectId == id);
        }
        private List<string> GetGoesetStringItems()
        {
            List<string> list = new();
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                list.Add($"Geoset {geo.ObjectId} ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles)");
            }
            return list;
        }
        private void Setpiv(object? sender, RoutedEventArgs? e)
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
                CGeoset geo = CurrentModel.Geosets[s.box.SelectedIndex];
                CVector3 centroid = Calculator.GetCentroidOfGeoset(geo);
                selected.PivotPoint = centroid;
            }
        }
        private void reverseSequence(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence s = GetSelectedSequence();
            ReverseSequence(s); SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
        }
        private CSequence GetSelectedSequence()
        {

            return CurrentModel.Sequences[ListSequenes.SelectedIndex];
        }
        private CSequence GetSelectedSequenceAnimator()
        {
            return CurrentModel.Sequences[List_Sequences_Animator.SelectedIndex];
        }
        private void switchLooping(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence s = GetSelectedSequence();
                s.NonLooping = !s.NonLooping;
                RefreshSequencesList();
            }
            SetSaved(false);
        }
        private void CopySQData(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with"); return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (CSequence s in CurrentModel.Sequences)
            {
                sb.AppendLine($"{s.IntervalStart}: {{ {s.Name}: Start }}"); ;
                sb.AppendLine($"{s.IntervalEnd}: {{ {s.Name}: End }}"); ;
            }
            TextViewer tw = new TextViewer(sb.ToString());
            tw.ShowDialog();

        }
        private void ShowGaps(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with"); return;
            }
            List<Interval> intervals = new List<Interval>();
            foreach (CSequence s in CurrentModel.Sequences) { intervals.Add(new Interval(s.IntervalStart, s.IntervalEnd)); }
            string gaps = GetGaps(intervals);

            TextViewer tw = new TextViewer("Gaps between sequences up to 999,999:\n\n" + gaps);
            tw.ShowDialog();
        }
        private static string GetGaps(List<Interval> intervals)
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
            sb.AppendLine($"Geoset Matrix Groups: {CurrentModel.Geosets.Sum(x => x.Groups.Count)}");
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
            int edges = 0;
            foreach (var g in CurrentModel.Geosets) edges += g.CountEdges();
            sb.AppendLine($"Edges: {edges}");
            LabelDisplayInfo.Text = sb.ToString();

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
                    CAttachment? item = node as CAttachment; if (item == null) continue;
                    datas += item.Visibility.Count;
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter? item = node as CParticleEmitter; if (item == null) continue;
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
                    CParticleEmitter2? item = node as CParticleEmitter2; if (item == null) continue;
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
                    CRibbonEmitter? item = node as CRibbonEmitter; if (item == null) continue;
                    datas += item.Visibility.Count;
                    datas += item.HeightAbove.Count;
                    datas += item.HeightBelow.Count;
                    datas += item.TextureSlot.Count;
                    datas += item.Alpha.Count;
                    datas += item.Color.Count;
                }
                if (node is CLight)
                {
                    CLight? item = node as CLight; if (item == null) continue;
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
        private void rootall(object? sender, RoutedEventArgs? e)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                node.Parent.Detach();
            }
            RefreshNodesTree(); SetSaved(false);
        }
        private void removeAllNodes(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.Nodes.Clear();
            RefreshNodesTree(); SetSaved(false);
        }
        private void ImportTextures(object? sender, RoutedEventArgs? e)
        {
            CModel? TemporaryModel = GetTemporaryModel();
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
            RefreshTexturesList(); SetSaved(false);
        }
        private void masscreeatesequences(object? sender, RoutedEventArgs? e)
        {
            Mass_Create_Sequences ms = new Mass_Create_Sequences(CurrentModel);
            ms.ShowDialog();
            if (ms.DialogResult == true) { RefreshSequencesList(); SetSaved(false); History.Sequences.Add(CurrentModel); }
        }
        private void createTexture(object? sender, RoutedEventArgs? e)
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
                RefreshTexturesList();
                RefreshLayersTextureList(); SetSaved(false);
            }
        }
        private void CollectOptimizationSettings()
        {
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
            Optimizer.AddOrigin = Check_AddMissingOrigin.IsChecked == true;
            Optimizer.TimeMiddle = Check_ClampTimeMiddle.IsChecked == true;
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
            Optimizer.SplitGeosets = Check_Splitgeosets.IsChecked == true && Check_Minimiematrixgroups.IsChecked == true;
            Optimizer.DeduplicateMatrixGroups = Check_duplmgn.IsChecked == true;
        }
        private void optimize(object? sender, RoutedEventArgs? e)
        {
            //Scene_Viewport3D.Children.Clear();
            CollectOptimizationSettings();
            Optimizer.Optimize(CurrentModel);

            CollectTexturesOpenGL();
            RefreshAll();
            MessageBox.Show("Done optimizing");
            SetSaved(false);
        }
        private void checkallopts(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < ListOptOptions.Children.Count; i++)
            {
                if (ListOptOptions.Children[i] is CheckBox)
                {
                    CheckBox? c = ListOptOptions.Children[i] as CheckBox; if (c == null) return;
                    if (c.IsEnabled) { c.IsChecked = true; }
                }
            }
        }
        private void uncheckallopts(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < ListOptOptions.Children.Count; i++)
            {
                if (ListOptOptions.Children[i] is CheckBox)
                {
                    CheckBox? c = ListOptOptions.Children[i] as CheckBox; if (c == null) continue; ;
                    if (c.IsEnabled) { c.IsChecked = false; }
                }
            }
        }
        private void inversecheckopts(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < ListOptOptions.Children.Count; i++)
            {
                if (ListOptOptions.Children[i] is CheckBox)
                {
                    CheckBox? c = ListOptOptions.Children[i] as CheckBox; if (c == null) return;
                    if (c.IsEnabled) { c.IsChecked = c.IsChecked == true ? false : true; ; }
                }
            }
        }
        private void reload(object? sender, RoutedEventArgs? e)
        {
            if (CurrentSaveLocation.Length != 0)
            {
                LoadModel(CurrentSaveLocation);
            }
        }
        private void showunanimatedseq(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with"); return;
            }
            Optimizer.Model = CurrentModel;
            List<string> list = new();
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
                TextViewer tw = new TextViewer($"these sequences are not animated:\n\n" + string.Join("\n", list.ToArray()));
                tw.ShowDialog();
            }
            else
            {
                MessageBox.Show("No un-animated sequences");
            }
        }
        private void DelAllGeosets(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.Geosets.Clear();
            RefreshGeosetsList();

        }
        private void clearAllnodetrans(object? sender, RoutedEventArgs? e)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                node.Translation.Clear();
                node.Scaling.Clear();
                node.Rotation.Clear();
            }
        }
        private void clearnodeTrans(object? sender, RoutedEventArgs? e)
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
        private void reversenodetr(object? sender, RoutedEventArgs? e)
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
        private void loaded(object? sender, RoutedEventArgs? e)
        {
            Initialize();
            LoadSettings();
            Textbox_HistoryLimit.Text = "200";
            LoadGroundTextureImage();
            LoadPlayBackIcons();
            Radio_PlayDef.IsChecked = true;
        }
        private void LoadPlayBackIcons()
        {
            IconLoader2.LoadIconToImageControl(IconPlay, "tick");
            IconLoader2.LoadIconToImageControl(IconDontPlay, "cross");
        }
        private void SpaceTras(object? sender, RoutedEventArgs? e)
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
        private static void SpaceTransformation(CSequence sequence, CAnimator<CVector3> animator)
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
        private static void SpaceTransformation(CSequence sequence, CAnimator<CVector4> animator)
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
        private void DelNode(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            if (NodeHasAttachedVertices(node))
            {
                MessageBox.Show("There are vertices attached to this node. Re-attach them to another first."); return;
            }
            if (node is CCollisionShape) CurrentModel.CalculateCollisionShapeEdges(); //CalculateModelLines();
            int id = node.NodeId;
            CurrentModel.Nodes.Remove(node);
            foreach (INode nod in CurrentModel.Nodes.ToList())
            {
                if (nod.Parent == null) { CurrentModel.Nodes.Remove(nod); continue; }
                if (nod.Parent.NodeId == id) { CurrentModel.Nodes.Remove(nod); continue; }
            }
            RefreshNodesTree();
            RefreshSequencesList_Paths();
            RefreshPath_ModelNodes_List(); SetSaved(false);
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
        private void CreateAtts1(object? sender, RoutedEventArgs? e)
        {
            List<string> names = new()
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
            RefreshNodesTree(); SetSaved(false);
        }
        private void Createatts2(object? sender, RoutedEventArgs? e)
        {
            List<string> names = new()
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
            RefreshNodesTree(); SetSaved(false);
        }
        private void movetoroot(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();
            node.Parent.Detach();
            RefreshNodesTree(); SetSaved(false);
        }
        public void ClearAnimations(CSequence sequence, bool andSequence)
        {

            int from = sequence.IntervalStart;
            int to = sequence.IntervalEnd;
            if (andSequence)
            {
                ListSequenes.Items.Remove(ListSequenes.SelectedItem);
                CurrentModel.Sequences.Remove(sequence);
            }
            foreach (INode node in CurrentModel.Nodes)
            {
                RemoveSequenceFromAnimator(sequence, node.Translation);
                RemoveSequenceFromAnimator(sequence, node.Scaling);
                RemoveSequenceFromAnimator(sequence, node.Rotation);

                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;

                    RemoveSequenceFromAnimator(sequence, element.Visibility);
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    RemoveSequenceFromAnimator(sequence, element.Visibility);
                    RemoveSequenceFromAnimator(sequence, element.Longitude);
                    RemoveSequenceFromAnimator(sequence, element.Latitude);
                    RemoveSequenceFromAnimator(sequence, element.EmissionRate);
                    RemoveSequenceFromAnimator(sequence, element.InitialVelocity);
                    RemoveSequenceFromAnimator(sequence, element.InitialVelocity);
                    RemoveSequenceFromAnimator(sequence, element.Gravity);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    RemoveSequenceFromAnimator(sequence, element.Visibility);
                    RemoveSequenceFromAnimator(sequence, element.Width);
                    RemoveSequenceFromAnimator(sequence, element.Length);
                    RemoveSequenceFromAnimator(sequence, element.Latitude);
                    RemoveSequenceFromAnimator(sequence, element.Speed);
                    RemoveSequenceFromAnimator(sequence, element.Gravity);
                    RemoveSequenceFromAnimator(sequence, element.Variation);


                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    RemoveSequenceFromAnimator(sequence, element.Visibility);
                    RemoveSequenceFromAnimator(sequence, element.HeightAbove);
                    RemoveSequenceFromAnimator(sequence, element.HeightBelow);
                    RemoveSequenceFromAnimator(sequence, element.Color);

                    RemoveSequenceFromAnimator(sequence, element.Alpha);
                    RemoveSequenceFromAnimator(sequence, element.TextureSlot);
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    RemoveSequenceFromAnimator(sequence, element.Visibility);

                    RemoveSequenceFromAnimator(sequence, element.Color);
                    RemoveSequenceFromAnimator(sequence, element.AmbientColor);
                    RemoveSequenceFromAnimator(sequence, element.Intensity);
                    RemoveSequenceFromAnimator(sequence, element.AmbientIntensity);
                    RemoveSequenceFromAnimator(sequence, element.AttenuationStart);
                    RemoveSequenceFromAnimator(sequence, element.AttenuationEnd);
                }

            }
            foreach (CGeosetAnimation ga in CurrentModel.GeosetAnimations)
            {
                RemoveSequenceFromAnimator(sequence, ga.Alpha);
                RemoveSequenceFromAnimator(sequence, ga.Color);

            }
            foreach (CCamera camera in CurrentModel.Cameras)
            {
                RemoveSequenceFromAnimator(sequence, camera.Translation);
                RemoveSequenceFromAnimator(sequence, camera.TargetTranslation);
                RemoveSequenceFromAnimator(sequence, camera.Rotation);
            }
            foreach (CMaterial material in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    RemoveSequenceFromAnimator(sequence, layer.Alpha);
                    RemoveSequenceFromAnimator(sequence, layer.TextureId);
                }
            }
            foreach (CTextureAnimation ta in CurrentModel.TextureAnimations)
            {
                RemoveSequenceFromAnimator(sequence, ta.Translation);
                RemoveSequenceFromAnimator(sequence, ta.Rotation);
                RemoveSequenceFromAnimator(sequence, ta.Scaling);

            }
            SetSaved(false);
        }

        private static void RemoveSequenceFromAnimator(CSequence sequence, CAnimator<float> animator)
        {
            if (animator.Animated == false) { return; }
            if (animator.Count == 0) { return; }
            foreach (var node in animator.NodeList.ToList())
            {
                if (node.Time >= sequence.IntervalStart && node.Time <= sequence.IntervalEnd)
                {
                    animator.Remove(node);
                }
            }
        }
        private static void RemoveSequenceFromAnimator(CSequence sequence, CAnimator<CVector3> animator)
        {
            if (animator.Animated == false) { return; }
            if (animator.Count == 0) { return; }
            foreach (var node in animator.NodeList.ToList())
            {
                if (node.Time >= sequence.IntervalStart && node.Time <= sequence.IntervalEnd)
                {
                    animator.Remove(node);
                }
            }
        }
        private static void RemoveSequenceFromAnimator(CSequence sequence, CAnimator<CVector4> animator)
        {
            if (animator.Animated == false) { return; }
            if (animator.Count == 0) { return; }
            foreach (var node in animator.NodeList.ToList())
            {
                if (node.Time >= sequence.IntervalStart && node.Time <= sequence.IntervalEnd)
                {
                    animator.Remove(node);
                }
            }
        }
        private static void RemoveSequenceFromAnimator(CSequence sequence, CAnimator<int> animator)
        {
            if (animator.Animated == false) { return; }
            if (animator.Count == 0) { return; }
            foreach (var node in animator.NodeList.ToList())
            {
                if (node.Time >= sequence.IntervalStart && node.Time <= sequence.IntervalEnd)
                {
                    animator.Remove(node);
                }
            }
        }

        private void delseqalong(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence sequence = GetSelectedSequence();
            ClearAnimations(sequence, true);

            RefreshSequencesList_Paths();
            RefreshPath_ModelNodes_List();
            RefreshSequencesList();
            SetSaved(false);
            History.Sequences.Add(CurrentModel);
        }

        private void AddNewMissingVisibilities(CSequence sequence)
        {
            foreach (var ga in CurrentModel.GeosetAnimations)
            {
                AddMissingVisbility(sequence, ga.Alpha);

            }
            foreach (var node in CurrentModel.Nodes)
            {

            }
            foreach (var material in CurrentModel.Materials)
            {
                foreach (var layer in material.Layers)
                {
                    AddMissingVisbility(sequence, layer.Alpha);
                }
            }
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CAttachment a) { AddMissingVisbility(sequence, a.Visibility); }
                if (node is CLight c) { AddMissingVisbility(sequence, c.Visibility); }
                if (node is CParticleEmitter e) { AddMissingVisbility(sequence, e.Visibility); }
                if (node is CParticleEmitter2 e2) { AddMissingVisbility(sequence, e2.Visibility); }
                if (node is CRibbonEmitter r) { AddMissingVisbility(sequence, r.Visibility); }

            }
        }
        private static void AddMissingVisbility(CSequence sequence, CAnimator<float> a)
        {
            if (a.Animated == false) { return; }
            if (a.Count == 0) { return; }
            var c = a.NodeList.ToList();
            foreach (var f in c)
            {
                f.Time = sequence.IntervalStart;
                f.Value = 1f;
                a.Add(f);
            }

            a.NodeList = a.NodeList.OrderBy(x => x.Time).ToList();
        }
        private void ClearSequenceAnimations(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence sequence = GetSelectedSequence();
            ClearAnimations(sequence, false); SetSaved(false);
            RefreshSequencesList();
        }
        private void createTextures(object? sender, RoutedEventArgs? e)
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
                RefreshTexturesList();
                RefreshLayersTextureList();

                CollectTexturesOpenGL();
                SetSaved(false);
            }
        }
        private void delTexture(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                if (CurrentModel.Materials.Count > 0 && CurrentModel.Textures.Count == 1)
                {
                    MessageBox.Show("Cannot delete when there is one remaing texture and   material/s");
                    return;
                }
                if (TextureIsUsed(texture))
                {
                    if (askDelete())
                    {
                        List_Textures.Items.Remove(List_Textures.SelectedItem);
                        foreach (CMaterial material in CurrentModel.Materials)
                        {
                            foreach (CMaterialLayer layer in material.Layers)
                            {
                                if (layer.Texture.Object == texture) { layer.Texture.Attach(CurrentModel.Textures[0]); }
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
                            if (layer.Texture.Object == texture) { layer.Texture.Attach(CurrentModel.Textures[0]); }
                        }
                    }
                    CurrentModel.Textures.Remove(texture);
                }
                RefreshLayersTextureList();
                SelectedLayer(null, null);
                SetSaved(false);
                //  RefreshUsedTextureInCombobox();
            }
        }
        private void RefreshUsedTextureInCombobox()
        {
            var material = GetSelectedMAterial();
            var layer = material.Layers[List_Layers.SelectedIndex];
            var index = CurrentModel.Textures.IndexOf(layer.Texture.Object);
            RefreshLayersTextureList();
            Combo_LayerUsedTexture.SelectedIndex = index;
        }
        private bool TextureIsUsed(CTexture texture)
        {
            foreach (CMaterial material in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (layer.Texture.Object == texture) { return true; }
                }
            }
            return false;
        }

        private CTexture GetSElectedTexture()
        {
            return CurrentModel.Textures[List_Textures.SelectedIndex];
        }
        private void delgeoset(object? sender, RoutedEventArgs? e)
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

            SetSaved(false);
        }
        private void creatematerialfortargettexture(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem == null) return;
            CTexture texture = GetSElectedTexture();
            CMaterial material = new CMaterial(CurrentModel);
            CMaterialLayer layer = new CMaterialLayer(CurrentModel);
            layer.Texture.Attach(texture);
            material.Layers.Add(layer);
            CurrentModel.Materials.Add(material);
            RefreshMaterialsList();

            CollectTexturesOpenGL();
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
            if (List_Layers.SelectedItem != null && List_MAterials.SelectedItem != null)
            {
                var material = GetSelectedMAterial();
                var layer = material.Layers[List_Layers.SelectedIndex];
                if (CurrentModel.TextureAnimations.Contains(layer.TextureAnimation.Object))
                {
                    int index = CurrentModel.TextureAnimations.IndexOf(layer.TextureAnimation.Object);
                    Combo_LayerUsedTextureAnim.SelectedIndex = index + 1;
                }
                else
                {
                    Combo_LayerUsedTextureAnim.SelectedIndex = 0;
                }
            }
        }
        private void DelMAterial(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                if (CurrentModel.Geosets.Count > 0 && CurrentModel.Materials.Count == 1)
                {
                    MessageBox.Show("Cannot delete the only remaining material if there are geosets"); return;
                }
                CMaterial material = GetSelectedMAterial();
                if (material.HasReferences)
                {
                    if (askDelete())
                    {
                        CurrentModel.Materials.Remove(material);
                        ReapplyMaterialsToEmptyGeosets();
                        RefreshMaterialsList();
                        RefreshLayersList();
                    }
                }
                else
                {
                    CurrentModel.Materials.Remove(material);
                    RefreshMaterialsList();
                    RefreshLayersList();
                }
                SetSaved(false);
            }
        }
        private void creatematsforalltextures(object? sender, RoutedEventArgs? e)
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

            CollectTexturesOpenGL();
        }
        internal void ReapplyMaterialsToEmptyGeosets()
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                if (CurrentModel.Materials.Contains(geoset.Material.Object) == false)
                {
                    geoset.Material.Attach(CurrentModel.Materials[0]);
                }
            }
        }
        private INode GetNode(string name)
        {
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void movetargetnodeundernode(object? sender, RoutedEventArgs? e)
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
                    if (s.Selected == null) return;

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
            else { MessageBox.Show("Select a node"); return; }
        }
        private void SelectAllGeosets(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in ListGeosets.Items)
            {
                ListGeosets.SelectedItems.Add(item);
            }
            Pause = false;

        }
        private void DeselectAllGeosets(object? sender, RoutedEventArgs? e)
        {
            ListGeosets.SelectedItems.Clear();
        }
        private void InvertSelectGeosets(object? sender, RoutedEventArgs? e)
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
        private void clearRecents(object? sender, RoutedEventArgs? e)
        {
            Recents.Clear();
            SaveRecents();
        }

        private void MergeSelectedSequences(object? sender, RoutedEventArgs? e)
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
                    History.Sequences.Add(CurrentModel);
                }
            }
        }
        private void SplitSequences(object? sender, RoutedEventArgs? e)
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
                    RefreshSequencesList(); SetSaved(false);
                }
            }
        }

        private void ClearAllAnimationsOfSequence(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Translation.Clear();
                node.Scaling.Clear();
                node.Rotation.Clear();
            }
            SetSaved(false);

        }
        private void clearsequencetranslations(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes) node.Translation.Clear();
            SetSaved(false);
        }
        private void clearsequencerotations(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes) node.Rotation.Clear();
            SetSaved(false);
        }
        private void clearsequencescalings(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes) node.Scaling.Clear();
            SetSaved(false);
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
        private void createColsForTargetGeo(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    CVector3 centroid = new CVector3();
                    Optimizer.CalculateGeosetExtent(geoset);
                    CCollisionShape node = new CCollisionShape(CurrentModel);
                    node.Name = $"CollisionShape_{IDCounter.Next}";
                    node.Type = ECollisionShapeType.Box;
                    node.Vertex1 = new CVector3(geoset.Extent.Min.X, geoset.Extent.Min.Y, geoset.Extent.Min.Z);
                    node.Vertex2 = new CVector3(geoset.Extent.Max.X, geoset.Extent.Max.Y, geoset.Extent.Max.Z);
                    node.PivotPoint = centroid;
                    CurrentModel.Nodes.Add(node);
                }
                RefreshNodesTree();

            }
        }

        private void MergeGeosets(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 1)
            {

                Pause = true;
                List<CGeoset> SelectedGeosets = GetSelectedGeosets();
                if (SelectedGeosets.Count <= 1) { return; }
                CGeoset First = SelectedGeosets[0];
                for (int i = 1; i < SelectedGeosets.Count; i++)
                {
                    Calculator.TransferGeosetData(First, SelectedGeosets[i], CurrentModel);
                }
                // delete  
                for (int i = 1; i < SelectedGeosets.Count; i++)
                {
                    DeleteGeosetAnimationOf(SelectedGeosets[i]);
                    CurrentModel.Geosets.Remove(SelectedGeosets[i]);
                }
                Optimizer.MinimizeMatrixGroups_Geoset(First);
                Pause = false;
                RefreshGeosetsList();
                RefreshGeosetAnimationsList();

            }
            else
            {
                MessageBox.Show("Select at least 2 geosets");
            }
        }
        private void negate(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void TranslateGeoserts(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void scalegeosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void CenterSelectionEach(object? sender, RoutedEventArgs? e)
        {
            if (Tabs_Geosets.SelectedIndex == 0) // geosets
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
                    SetSaved(false);
                }

            }
            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    bool onX = ax.Check_x.IsChecked == true;
                    bool onY = ax.Check_y.IsChecked == true;
                    bool onZ = ax.Check_z.IsChecked == true;


                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).ToList();
                    if (triangles.Count == 1)
                    {
                        vertices[0].Position.X = onX ? 0 : vertices[0].Position.X;
                        vertices[0].Position.Y = onY ? 0 : vertices[0].Position.Y;
                        vertices[0].Position.Z = onZ ? 0 : vertices[0].Position.Z;
                    }
                    else if (triangles.Count > 1)
                    {
                        Axes axes = Axes.X;
                        if (onY) axes = Axes.Y;
                        if (onZ) axes = Axes.Z;
                        Calculator.CenterVertices(vertices, axes);
                    }
                }

            }
            else if (Tabs_Geosets.SelectedIndex == 2 || Tabs_Geosets.SelectedIndex == 3) // vertices
            {
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    bool onX = ax.Check_x.IsChecked == true;
                    bool onY = ax.Check_y.IsChecked == true;
                    bool onZ = ax.Check_z.IsChecked == true;
                    var vertices = GetSelectedVertices();
                    if (vertices.Count == 1)
                    {
                        vertices[0].Position.X = onX ? 0 : vertices[0].Position.X;
                        vertices[0].Position.Y = onY ? 0 : vertices[0].Position.Y;
                        vertices[0].Position.Z = onZ ? 0 : vertices[0].Position.Z;
                    }
                    else if (vertices.Count > 1)
                    {
                        Axes axes = Axes.X;
                        if (onY) axes = Axes.Y;
                        if (onZ) axes = Axes.Z;
                        Calculator.CenterVertices(vertices, axes);
                    }
                }

            }
            if (Tabs_Geosets.SelectedIndex == 6) // nodes
            {
                axis_selector ax = new axis_selector();
                ax.ShowDialog();
                if (ax.DialogResult == true)
                {
                    bool onX = ax.Check_x.IsChecked == true;
                    bool onY = ax.Check_y.IsChecked == true;
                    bool onZ = ax.Check_z.IsChecked == true;
                    var nodes = GetSelectedNodes_NodeEditor();
                    if (nodes.Count == 1)
                    {
                        nodes[0].PivotPoint.X = onX ? 0 : nodes[0].PivotPoint.X;
                        nodes[0].PivotPoint.Y = onY ? 0 : nodes[0].PivotPoint.Y;
                        nodes[0].PivotPoint.Z = onZ ? 0 : nodes[0].PivotPoint.Z;
                    }
                    else if (nodes.Count > 1)
                    {
                        Axes axes = Axes.X;
                        if (onY) axes = Axes.Y;
                        if (onZ) axes = Axes.Z;

                        Calculator.centerNodes(nodes, axes);
                    }
                }



            }
        }
        private void scaleToFit(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void RotateGeosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void aligngeosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private CGeoset DuplicateGeogeset(CGeoset inputGeoset, CModel whichModel)
        {
            CGeoset _newGeoset = new CGeoset(whichModel);
            CBone bone = new CBone(CurrentModel);
            bone.Name = $"ImportedGeosetBone_{IDCounter.Next}";
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
        private void reattachToBone(object? sender, RoutedEventArgs? e)
        {
            var geosets = GetSelectedGeosets();
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
                SetSaved(false);
            }
            else
            {
                MessageBox.Show("There are no bones"); return;
            }
        }
        private void PullTogether(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void renamesequence(object? sender, RoutedEventArgs? e)
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
                    SetSaved(false);
                    History.Sequences.Add(CurrentModel);
                }
            }
        }
        public static string CapitalizeWords(string input)
        {
            if (input.Length == 0) return input;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }
        private bool TrackExistsInSEquences(int track, CSequence? exclude)
        {
            foreach (CSequence seq in CurrentModel.Sequences)
            {
                if (exclude != null) { if (seq == exclude) continue; }
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
            SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
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
            SetSaved(false);
        }
        private void resizeSequence(object? sender, RoutedEventArgs? e)
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
                if (TrackExistsInSEquences(to, cSequence) == true)
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
                            CParticleEmitter? item = node as CParticleEmitter; if (item == null) return;
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.EmissionRate);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.LifeSpan);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.InitialVelocity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Gravity);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Longitude);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Latitude);
                        }
                        if (node is CParticleEmitter2)
                        {
                            CParticleEmitter2? item = node as CParticleEmitter2; if (item == null) return;
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
                            CRibbonEmitter? item = node as CRibbonEmitter; if (item == null) return;
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.HeightAbove);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.HeightBelow);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.TextureSlot);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Alpha);
                            ResizeTargetSequenceKeyframes(from, oldTo, to, item.Color);
                        }
                        if (node is CLight)
                        {
                            CLight? item = node as CLight; if (item == null) return;
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
                    RefreshSequencesList();
                    History.Sequences.Add(CurrentModel);
                }
                SetSaved(false);
            }

        }
        private bool SequenceNameTaken(string name)
        {
            return CurrentModel.Sequences.Any(x => x.Name.ToLower() == name.ToLower());
        }
        private void DuplicateSEquenceWithMAtchingkeyframes(object? sender, RoutedEventArgs? e)
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
                            History.Sequences.Add(CurrentModel);
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
                    CParticleEmitter? item = node as CParticleEmitter; if (item == null) continue;
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.EmissionRate);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.LifeSpan);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.InitialVelocity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Gravity);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Longitude);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2? item = node as CParticleEmitter2; if (item == null) continue;
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
                    CRibbonEmitter? item = node as CRibbonEmitter; if (item == null) continue;
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.HeightAbove);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.HeightBelow);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.TextureSlot);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Alpha);
                    DuplicateKeyframes(oldFrom, oldTo, newFrom, newTo, item.Color);
                }
                if (node is CLight)
                {
                    CLight? item = node as CLight; if (item == null) continue;
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
            SetSaved(false);
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<CVector3>? animator)
        {
            if (animator == null) return;
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
            SetSaved(false);
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<CVector4>? animator)
        {
            if (animator == null) return;
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
            SetSaved(false);
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<float>? animator)
        {
            // Isolate the keyframes within the old range
            List<CAnimatorNode<float>> isolated = new List<CAnimatorNode<float>>();
            if (animator == null) return;
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
            SetSaved(false);
        }
        private void DuplicateKeyframes(int oldFrom, int oldTo, int newFrom, int newTo, CAnimator<int>? animator)
        {
            // Isolate the keyframes within the old range
            List<CAnimatorNode<int>> isolated = new List<CAnimatorNode<int>>();
            if (animator == null) return;
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
            SetSaved(false);
        }
        private List<string> GetGAsList(int skip)
        {
            List<string> list = new();
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                if (i == skip) continue;
                list.Add(CurrentModel.GeosetAnimations[i].ObjectId.ToString());
            }
            return list;
        }
        private void equalizegas_alpha(object? sender, RoutedEventArgs? e)
        {
            if (List_GeosetAnims.SelectedItem == null) { return; }
            if (CurrentModel.GeosetAnimations.Count > 1)
            {
                List<string> list = GetGAsList(List_GeosetAnims.SelectedIndex);
                Selector s = new(list, "Based on");
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    int id = 0;
                    if (s.Selected != null) { id = int.Parse(s.Selected); }

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
                    SetSaved(false);
                }
            }
        }
        private void equalizegas_color(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.GeosetAnimations.Count > 1)
            {
                List<string> list = GetGAsList(List_GeosetAnims.SelectedIndex);
                Selector s = new Selector(list, "Based on");
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    int id = 0;
                    if (s.Selected != null) { int.Parse(s.Selected); }
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
                    SetSaved(false);
                }
            }
        }
        private CGeosetAnimation GetSelectedGeosetAnimation()
        {
            int index = List_GeosetAnims.SelectedIndex;
            return CurrentModel.GeosetAnimations[index];
        }

        public void RefreshGeosetAnimationsList()
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
        private void AverageNormals(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosts = GetSelectedGeosets();
                foreach (CGeoset ge in geosts)
                {
                    Calculator.AverageNormals(ge);
                }
            }
            SetSaved(false);
        }
        private void DuplicateGeoset(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosts = GetSelectedGeosets();
                Pause = true;
                foreach (CGeoset ge in geosts)
                {
                    CurrentModel.Geosets.Add(DuplicateGeogeset(ge, CurrentModel));
                }
                RefreshRenderData(null,null);
                Pause = false;
            }
            RefreshGeosetsList();
            SetSaved(false);
        }
        private void ViewNodeTransformations(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                Transformations_viewer tv = new Transformations_viewer(node, CurrentModel);
                tv.ShowDialog();
            }
        }
        private void ViewGaTransformatons(object? sender, RoutedEventArgs? e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                Transformations_viewer tv = new Transformations_viewer(ga, CurrentModel);
                tv.ShowDialog();
            }
        }

        private void CreateHeroGlow(object? sender, RoutedEventArgs? e)
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "geosets\\Hero_Glow.mdx");
            CGeoset? ImportedGeoset = GetImportedGeoset(file);
            if (ImportedGeoset != null)
            {
                ImportedGeoset.Material.Attach(GetHerGlowMaterial());
                CurrentModel.Geosets.Add(ImportedGeoset);
                // AttachToNewBone(ImportedGeoset);
                RefreshGeosetsList();
                RefreshNodesTree();
            }

            CollectTexturesOpenGL();
            RefreshSequencesList_Paths();
            RefreshPath_ModelNodes_List();
            SetSaved(false);
        }
        private CMaterial? GetHerGlowMaterial()
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
                layer.TwoSided = true;
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

        private CGeoset? GetImportedGeoset(string file)
        {
            CGeoset? geo = null;
            if (File.Exists(file))
            {
                CModel? TemporaryModel = GetTemporaryModel(file);
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
        private void refreshnodes(object? sender, RoutedEventArgs? e)
        {
            RefreshNodesTree();
        }
        private void importtbg(object? sender, RoutedEventArgs? e)
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(local, "geosets\\Team_Background.mdx");
            CGeoset? ImportedGeoset = GetImportedGeoset(file);
            if (ImportedGeoset != null)
            {
                ImportedGeoset.Material.Attach(GetHerGlowMaterial());
                CurrentModel.Geosets.Add(ImportedGeoset);
                //  AttachToNewBone(ImportedGeoset);
                RefreshGeosetsList();
                RefreshNodesTree();
            }

            CollectTexturesOpenGL();
            RefreshSequencesList_Paths();
            RefreshPath_ModelNodes_List();
            SetSaved(false);
        }
        private void AttachToNewBone(CGeoset geoset)
        {
            CBone bone = new CBone(CurrentModel);
            bone.Name = "ImportedGeosetBone_" + IDCounter.Next;
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
        private void DelBonesGeometry(object? sender, RoutedEventArgs? e)
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
                    SetSaved(false);
                }
                else
                {
                    MessageBox.Show("Select a bone"); return;
                }

            }
        }
        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (CheckSaved() == true)
            {
                Environment.Exit(0);
            }

        }
        private void ListNodes_SelectedItemChanged(object? sender, RoutedPropertyChangedEventArgs<object>? e)
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
                sb.AppendLine(GetVisibilitySequences(att.Visibility));
            }
            if (node is CBone bone)
            {
                sb.AppendLine($"Geoset ID: {bone.Geoset.ObjectId}");
                sb.AppendLine($"Geoset Animation ID: {bone.GeosetAnimation.ObjectId}");
                sb.AppendLine(GetAttachedVerticesCountOfBone(bone));
                sb.AppendLine(GetAttachedGeosetsForBoneAsText(bone));
            }
            if (node is CParticleEmitter emitter)
            {
                sb.AppendLine(GetVisibilitySequences(emitter.Visibility));
            }
            if (node is CParticleEmitter2 emitter2)
            {
                sb.AppendLine(GetVisibilitySequences(emitter2.Visibility));
            }
            if (node is CLight light)
            {
                sb.AppendLine(GetVisibilitySequences(light.Visibility));
            }
            if (node is CRibbonEmitter ribbon)
            {
                sb.AppendLine(GetVisibilitySequences(ribbon.Visibility));



            }
            if (node is CEvent eventObj)
            {
                sb.AppendLine(GetEvent_tracks(eventObj));
            }
            Report_Node_data.Text = sb.ToString();
        }
        private string? GetEvent_tracks(CEvent eventObj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var track in eventObj.Tracks.ObjectList)
            {
                CSequence? sq = GetSequenceFromTrack(track.Time);

                if (sq == null) { sb.AppendLine($"{track.Time} [null sequence]"); continue; }
                string s = sq.Name;
                sb.AppendLine($"{track.Time} [{s}]");
            }
            return sb.ToString();
        }

        private string? GetAttachedGeosetsForBoneAsText(CBone bone)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Attached geoset/s (or parts of):");
            List<int> ids = new List<int>();
            List<string> names = new();
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    bool found = false;
                    foreach (var gnode in vertex.Group.Object.Nodes)
                    {
                        if (gnode.Node.Node == bone)
                        {
                            if (ids.Contains(geoset.ObjectId) == false)
                            {
                                ids.Add(geoset.ObjectId);
                                names.Add(geoset.Name.Length == 0 ? geoset.ObjectId.ToString() : geoset.Name);
                            }
                            found = true; break;

                        }
                        if (found) { break; }
                    }
                }
            }
            for (int i = 0; i < ids.Count; i++)
            {
                if (names[i].Length == 0)
                {
                    sb.AppendLine(ids[i].ToString());
                }
                else
                {
                    sb.AppendLine(names[i]);
                }

            }

            return sb.ToString();
        }

        private string GetVisibilitySequences(CAnimator<float> animator)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Visible in:");

            Dictionary<CSequence, string> sequenceVisibility = new Dictionary<CSequence, string>();

            if (animator.Static) // If static, determine visibility once
            {
                float val = animator.GetValue();
                return val == 1 ? "Always Visible" : "Always Invisible";
            }

            foreach (var sequence in CurrentModel.Sequences)
            {
                sequenceVisibility[sequence] = "??"; // Default to "Not Set"
            }

            foreach (var item in animator)
            {
                CSequence? sequence = GetSequenceFromTrack(item.Time);
                if (sequence == null) continue;

                string visibility = item.Value >= 0.5f ? "✓" : "";

                if (sequenceVisibility.ContainsKey(sequence) && sequenceVisibility[sequence] == "??")
                {
                    sequenceVisibility[sequence] = visibility;
                }
            }

            foreach (var kvp in sequenceVisibility)
            {
                sb.AppendLine($"{kvp.Key.Name}: {kvp.Value}");
            }

            return sb.ToString();
        }


        private CSequence? GetSequenceFromTrack(int time)
        {
            return CurrentModel.Sequences.FirstOrDefault(x => time >= x.IntervalStart && time <= x.IntervalEnd);
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
        private void about(object? sender, RoutedEventArgs? e)
        {
            about_w w = new about_w(); w.ShowDialog();
        }
        private void Checked_MatFullRes(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.FullResolution = Check_MatFS.IsChecked == true;
            }
        }
        private void Checked_MatSort(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.SortPrimitivesFarZ = Check_MatSort.IsChecked == true;
            }
        }
        private void Checked_MatCC(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.ConstantColor = Check_MatCC.IsChecked == true;
            }
        }
        private void List_MAterials_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
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

        private float minimumZoom = 5;
        private float distance = CameraControl.eyeX; // Camera's initial distance from the center
        private void Scene_MouseWheelGL(object? sender, MouseWheelEventArgs e)
        {
            if (CurrentSceneInteraction == SceneInteractionState.Modify)
            {

                ModifySelectionByDragging(modifyMode_current, axisMode, e.Delta > 0);
                return;
            }
            // Compute the direction vector from the eye to the center (0, 0, 0)
            Vector3 direction = new Vector3(CameraControl.eyeX, CameraControl.eyeY, CameraControl.eyeZ);
            // Normalize the direction vector
            direction = Vector3.Normalize(direction);
            // Adjust the zoom amount based on the mouse wheel movement
            if (e.Delta > 0) // Zoom in
            {
                if (distance <= minimumZoom) { return; }
                distance -= CameraControl.ZoomIncrement; // Decrease distance (zoom in)
            }
            else // Zoom out
            {
                distance += CameraControl.ZoomIncrement; // Increase distance (zoom out)
            }
            // Update the camera's position based on the new distance
            CameraControl.eyeX = direction.X * distance;
            CameraControl.eyeY = direction.Y * distance;
            CameraControl.eyeZ = direction.Z * distance;
            CheckForNaNCamera();
            RefreshViewportCameraDEbugInfo();
        }

        private void ModifySelectionByDragging(ModifyMode modifyMode_current, Axes axisMode, bool positive)
        {
            ModelInfoLabel.Text = modifyMode_current.ToString();
            if (modifyMode_current == ModifyMode.Scale)
            {
                ScaleSelection(positive, axisMode, false);
            }

            if (modifyMode_current == ModifyMode.ScaleEach)
            {
                ScaleSelection(positive, axisMode, true);
            }
            else if (modifyMode_current == ModifyMode.Rotate)
            {
                float val = GetVal(positive);
                if (axisMode == Axes.X)
                {
                    ModifyByX(val);

                }
                else if (axisMode == Axes.Y)
                {
                    ModifyByY(val);
                }
                else if (axisMode == Axes.Z)
                {
                    ModifyByZ(val);
                }
            }
            else if (modifyMode_current == ModifyMode.Translate)
            {
                if (axisMode == Axes.X)
                {
                    if (positive) MoveForawrad(null, null);
                    else MoveBackward(null, null);
                }
                else if (axisMode == Axes.Y)
                {
                    if (positive) MoveRight(null, null);
                    else MoveLeft(null, null);
                }
                else if (axisMode == Axes.Z)
                {
                    if (positive) MoveUp(null, null);
                    else MoveDown(null, null);
                }
            }
            else if (modifyMode_current == ModifyMode.DragUV_U)
            {
                DragUVu(positive);
            }
            else if (modifyMode_current == ModifyMode.DragUV_V)
            {
                DragUVv(positive);
            }
            else if (modifyMode_current == ModifyMode.ScaleUV)
            {
                ScaleUV(positive, true, true);
            }
            else if (modifyMode_current == ModifyMode.ScaleUVv)
            {
                ScaleUV(positive, false, true);
            }
            else if (modifyMode_current == ModifyMode.ScaleUVu)
            {
                ScaleUV(positive, true, false);
            }
            else if (modifyMode_current == ModifyMode.RotateNormals)
            {
                RotateNormals(positive);
            }
            else if (modifyMode_current == ModifyMode.RotateEach)
            {
                RotateEach(axisMode, positive);
            }
            else if (modifyMode_current == ModifyMode.ScaleEach)
            {
                ScaleEach(axisMode, positive);
            }
        }

        private void Sculpt(bool push)
        {
            throw new NotImplementedException();
        }
        private void SculptPersonal(bool push)
        {
            throw new NotImplementedException();
        }
        private void ScaleEach(Axes axisMode, bool positive)
        {

            if (Tabs_Geosets.SelectedIndex == 0)
            {

                var g = GetSelectedGeosets();
                float x = GetFloat(InputManualTransformAmount);
                if (axisMode == Axes.X)
                {

                    foreach (var geoset in g)
                    {
                        Calculator.ScaleGeoset(geoset, Calculator.GetCentroidOfGeoset(geoset), new CVector3(x, 1, 1));
                    }

                }
                if (axisMode == Axes.Y)
                {

                    foreach (var geoset in g)
                    {
                        Calculator.ScaleGeoset(geoset, Calculator.GetCentroidOfGeoset(geoset), new CVector3(0, x, 1));
                    }

                }
                if (axisMode == Axes.Z)
                {

                    foreach (var geoset in g)
                    {
                        Calculator.ScaleGeoset(geoset, Calculator.GetCentroidOfGeoset(geoset), new CVector3(0, 0, x));
                    }

                }
            }
        }

        private void RotateEach(Axes axisMode, bool positive)
        {
            if (Tabs_Geosets.SelectedIndex == 0)
            {

                var g = GetSelectedGeosets();
                float x = GetFloat(InputManualTransformAmount);
                if (axisMode == Axes.X)
                {

                    foreach (var geoset in g)
                    {
                        Calculator.RotateGeosetAroundPivotPoint(Calculator.GetCentroidOfGeoset(geoset), geoset, x, 0, 0);
                    }

                }
                if (axisMode == Axes.Y)
                {

                    foreach (var geoset in g)
                    {
                        Calculator.RotateGeosetAroundPivotPoint(Calculator.GetCentroidOfGeoset(geoset), geoset, 0, x, 0);
                    }

                }
                if (axisMode == Axes.Z)
                {

                    foreach (var geoset in g)
                    {
                        Calculator.RotateGeosetAroundPivotPoint(Calculator.GetCentroidOfGeoset(geoset), geoset, 0, 0, x);
                    }

                }
            }

        }

        private void RotateNormals(bool positive)
        {
            var geosets = GetSelectedGeosets();
            float change = positive ? 0.00277777777f : -0.00277777777f; // 1%

            foreach (var geose in geosets)
            {
                foreach (var vertex in geose.Vertices)
                {
                    float x = axisMode == Axes.X ? vertex.Normal.X + change : 0;
                    float y = axisMode == Axes.Y ? vertex.Normal.Y + change : 0;
                    float z = axisMode == Axes.Z ? vertex.Normal.Z + change : 0;
                    if (x > -1 && x < 1) { vertex.Normal.X += change; }
                    if (y > -1 && y < 1) { vertex.Normal.Y += change; }
                    if (z > -1 && z < 1) { vertex.Normal.Z += change; }

                }
            }
        }

        private void ScaleUV(bool positive, bool u, bool v)
        {
            var geosets = GetSelectedGeosets();
            float change = positive ? 0.01f : -0.01f; ; // 1%
            foreach (var geose in geosets)
            {
                foreach (var vertex in geose.Vertices)
                {
                    if (u)
                    {
                        if (vertex.TexturePosition.X > 0)
                        {
                            vertex.TexturePosition.X += vertex.TexturePosition.X * change;
                        }
                    }
                    if (v)
                    {
                        if (vertex.TexturePosition.Y > 0)
                        {
                            vertex.TexturePosition.Y += vertex.TexturePosition.Y * change;
                        }
                    }

                }
            }
        }

        private void DragUVv(bool positive)
        {
            float change = positive ? 0.005f : -0.005f; ;
            if (Tabs_Geosets.SelectedIndex == 0)
            {
                var geosets = GetSelectedGeosets();

                foreach (var geose in geosets)
                {
                    foreach (var vertex in geose.Vertices)
                    {
                        vertex.TexturePosition.X += change;
                    }
                }
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                var traingles = getSelectedTriangles();
                if (traingles.Count == 0) return;
                var vertices = traingles.SelectMany(x => x.Vertices).Distinct().ToList();
                foreach (var vertex in vertices)
                {
                    vertex.TexturePosition.X += change;
                }
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // vetices
            {
                var vertices = GetSelectedVertices();
                foreach (var vertex in vertices)
                {
                    vertex.TexturePosition.X += change;
                }
            }
        }

        private void DragUVu(bool positive)
        {
            float change = positive ? 0.005f : -0.005f; ;
            if (Tabs_Geosets.SelectedIndex == 0)
            {
                var geosets = GetSelectedGeosets();

                foreach (var geose in geosets)
                {
                    foreach (var vertex in geose.Vertices)
                    {
                        vertex.TexturePosition.Y += change;
                    }
                }
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                var traingles = getSelectedTriangles();
                if (traingles.Count == 0) return;
                var vertices = traingles.SelectMany(x => x.Vertices).Distinct().ToList();
                foreach (var vertex in vertices)
                {
                    vertex.TexturePosition.Y += change;
                }
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // vetices
            {
                var vertices = GetSelectedVertices();
                foreach (var vertex in vertices)
                {
                    vertex.TexturePosition.Y += change;
                }
            }
        }

        private float GetVal(bool positive)
        {
            float g = 0;
            bool b = float.TryParse(InputManualTransformAmount.Text, out float v);
            g = b ? v : g;
            return positive ? g : -g;
        }

        private void CheckForNaNCamera()
        {
            if (
                  float.IsNaN(CameraControl.eyeX) ||
                  float.IsNaN(CameraControl.eyeY) ||
                  float.IsNaN(CameraControl.eyeZ) ||
                   float.IsNaN(CameraControl.CenterX) ||
                  float.IsNaN(CameraControl.CenterY) ||
                  float.IsNaN(CameraControl.CenterZ) ||
                  float.IsNaN(CameraControl.UpX) ||
                  float.IsNaN(CameraControl.UpY) ||
                  float.IsNaN(CameraControl.UpZ)
                  )
            {
                view_front(null, null);
            }
        }
        private string TeamColor = "ReplaceableTextures\\TeamColor\\TeamColor00.blp";
        private string TeamGlow = "ReplaceableTextures\\TeamGlow\\TeamGlow00.blp";
        private string White = "Textures\\white.blp";
        private string Black = "UI\\Glues\\SinglePlayer\\HumanCampaign3D\\Black32.blp";

        private void refreshalllists(object? sender, RoutedEventArgs? e)
        {
            RefreshAll();
        }


        //----------------------------------------------------


        //----------------------------------------------------



        private void setModelName(object? sender, RoutedEventArgs? e)
        {
            Input i = new Input(CurrentModel.Name);
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                CurrentModel.Name = i.Result;
                RefreshTitle();
            }
        }
        private List<CGeoset> SelectedGeosetsList = new List<CGeoset>();
        private void SelectedGeosets(object? sender, SelectionChangedEventArgs? e)
        {
            SelectedGeosetsList = GetSelectedGeosets();
            foreach (var geoset in CurrentModel.Geosets) geoset.isSelected = SelectedGeosetsList.Contains(geoset);
            //RefreshViewPort();
            if (ListGeosets.SelectedItems.Count == 1)
            {
                int index = ListGeosets.SelectedIndex;
                var geoset = CurrentModel.Geosets[index];
                var ga = CurrentModel.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == geoset);
                if (ga != null)
                {
                    List_GeosetAnims.SelectedIndex = CurrentModel.GeosetAnimations.IndexOf(ga);
                }
            }
            FillManualValuesForSelectedGeosts();
        }



        private void FillManualValuesForSelectedGeosts()
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            var geosets = GetSelectedGeosets();
            if (geosets.Count == 1)
            {
                var centroid = Calculator.GetCentroidOfGeoset(geosets[0]);
                InputAnimatorX.Text = centroid.X.ToString();
                InputAnimatorY.Text = centroid.Y.ToString();
                InputAnimatorZ.Text = centroid.Z.ToString();
            }
            else
            {
                var centroid = Calculator.GetCentroidOfGeosets(geosets);
                InputAnimatorX.Text = centroid.X.ToString();
                InputAnimatorY.Text = centroid.Y.ToString();
                InputAnimatorZ.Text = centroid.Z.ToString();
            }
        }
        public void SelectedLayer(object? sender, SelectionChangedEventArgs? e)
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
                Pause = true;
                CheckLayerTextureID.IsChecked = layer.TextureId.Animated;
                ButtonTextureID.IsEnabled = layer.TextureId.Animated;
                CheckLayerAlpha.IsChecked = layer.Alpha.Animated;
                ButtonLayerAlpha.IsEnabled = layer.Alpha.Animated; ;
                InputLayerAlpha.Text = layer.Alpha.Static ? (layer.Alpha.GetValue() * 100f).ToString() : "";
                InputLayerAlpha.IsEnabled = layer.Alpha.Static;
                Pause = false;
                Combo_LayerUsedTexture.SelectedIndex = CurrentModel.Textures.IndexOf(layer.Texture.Object);
                Combo_LayerUsedTexture.IsEnabled = layer.TextureId.Static;
                InputLayerAlpha.IsEnabled = layer.Alpha.Static;
                ButtonLayerAlpha.Content = $"Edit ({layer.Alpha.Count})";
                ButtonTextureID.Content = $"Edit ({layer.TextureId.Count})";
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
        public void RefreshLayersTextureList()
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
        private void DelLayer(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                if (mat.Layers.Count == 1) { MessageBox.Show("Cannot delete the only remaining layer"); return; }
                mat.Layers.RemoveAt(List_Layers.SelectedIndex);
                RefreshLayersList();
            }
        }
        private void Checked_Unshaded(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.Unshaded = Check_Unshaded.IsChecked == true;
            }
        }
        private void Checked_Unfogged(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.Unfogged = Check_Unfogged.IsChecked == true;
            }
        }
        private void Checked_twosided(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.TwoSided = Check_twosided.IsChecked == true;
            }
        }
        private void Checked_sf(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.SphereEnvironmentMap = Check_sf.IsChecked == true;
            }
        }
        private void Checked_NoDepthTest(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.NoDepthTest = Check_NoDepthTest.IsChecked == true;
            }
        }
        private void Checked_NoDepthSet(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                layer.NoDepthSet = Check_NoDepthSet.IsChecked == true;
            }
        }
        private void viewLayerTransformations(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                Transformations_viewer tv = new Transformations_viewer(layer, CurrentModel);
                tv.ShowDialog();
            }
        }
        private void DeleteTextureAnim(object? sender, RoutedEventArgs? e)
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
        private void viewTextureAnims(object? sender, RoutedEventArgs? e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                Transformations_viewer tv = new Transformations_viewer(CurrentModel.TextureAnimations[List_TextureAnims.SelectedIndex], CurrentModel);
                tv.ShowDialog();
            }
        }

        private void copyNodeTranslations(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                CopiedNode = GetSeletedNode();
            }
            else { MessageBox.Show("Select a node"); }
        }
        private void pastenodetranslations(object? sender, RoutedEventArgs? e)
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
        private void NegateNodePositon(object? sender, RoutedEventArgs? e)
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
        private void Tranlateallnodes(object? sender, RoutedEventArgs? e)
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
        private void syncwhith(object? sender, RoutedEventArgs? e)
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
        public static List<CAnimatorNode<CVector3>> Synchronize(List<CAnimatorNode<CVector3>> list1, List<CAnimatorNode<CVector3>> list2)
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
        public static List<CAnimatorNode<CVector4>> Synchronize(List<CAnimatorNode<CVector4>> list1, List<CAnimatorNode<CVector4>> list2)
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
        public static List<CAnimatorNode<int>> Synchronize(List<CAnimatorNode<int>> list1, List<CAnimatorNode<int>> list2)
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
        public static List<CAnimatorNode<float>> Synchronize(List<CAnimatorNode<float>> list1, List<CAnimatorNode<float>> list2)
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
        private void changegeosetusedmaterial(object? sender, RoutedEventArgs? e)
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
            Selector s = new Selector(materials, "Selector - change used material");
            s.ShowDialog();
            if (s?.DialogResult == true)
            {
                Pause = true;
                int index = s.box.SelectedIndex;
                foreach (CGeoset geo in geos)
                {
                    geo.Material.Attach(CurrentModel.Materials[index]);
                }

                CollectTexturesOpenGL();
                RefreshGeosetsList();
                Pause = false;
            }
        }
        private List<string> GetMaterialsList()
        {
            List<string> list = new();
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
        private void createnode_click(object? sender, RoutedEventArgs? e)
        {
            createnode cr = new createnode(CurrentModel);
            cr.ShowDialog();
            if (cr.DialogResult == true)
            {
                INode? SelectedNode = ListNodes.SelectedItem == null ? null : GetSeletedNode();
                INode? _new = null;
                NodeType type = cr.Result;
                string name = cr.ResultName;
                if (type == NodeType.Bone) _new = new CBone(CurrentModel);
                if (type == NodeType.Helper) _new = new CHelper(CurrentModel);
                if (type == NodeType.Attachment) _new = new CAttachment(CurrentModel);
                if (type == NodeType.Ribbon)
                {
                    if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; }
                    _new = new CRibbonEmitter(CurrentModel);
                }
                if (type == NodeType.Emitter1)
                { if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; } _new = new CRibbonEmitter(CurrentModel); }
                if (type == NodeType.Emitter2)
                {
                    if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; }
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
                if (_new == null) return;
                _new.Name = name;
                CurrentModel.Nodes.Add(_new);
                if (SelectedNode != null && cr.Check_parent.IsChecked == true)
                {
                    _new.Parent.Attach(SelectedNode);
                }
                RefreshNodesTree();
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
            }
        }
        private void flattengeosets(object? sender, RoutedEventArgs? e)
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

            }
        }
        private CSequence? AskSequenceName(CSequence except)
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
        private void swapnames(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null && CurrentModel.Sequences.Count > 1)
            {
                CSequence? selected = GetSelectedSequence();
                CSequence? target = AskSequenceName(selected);
                if (target != null)
                {
                    (target.Name, selected.Name) = (selected.Name, target.Name);
                    RefreshSequencesList();
                    History.Sequences.Add(CurrentModel);
                }
            }
        }
        private void newsequence_(object? sender, RoutedEventArgs? e)
        {
            newsequence ns = new newsequence(CurrentModel);
            ns.ShowDialog();
            if (ns.DialogResult == true)
            {
                if (ns.CreatedSequence == null) { return; }

                RefreshPath_ModelNodes_List();
                AddNewMissingVisibilities(ns.CreatedSequence);
                History.Sequences.Add(CurrentModel);
                RefreshSequencesList();
                RefreshSequencesList_Paths();
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
        private void Centergeosetsat(object? sender, RoutedEventArgs? e)
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
                    SetSaved(false);
                }
            }
        }
        private void SetUsedLayerTexture(object? sender, SelectionChangedEventArgs? e)
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

                CollectTexturesOpenGL();

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
        private void alignnodes(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.Items.Count < 2) { return; }
            List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
            Multiselector_Window s = new Multiselector_Window(nodes);
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
                    SetSaved(false);
                }
            }
        }
        private void scalenodes(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.Items.Count < 2) { return; }
            List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
            Multiselector_Window s = new Multiselector_Window(nodes);
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
                SetSaved(false);
            }
        }
        private void geosetInfobone(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                List<string> indexes = new();
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
        private void shiftnodetranslations(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void scalekeyframesofnode(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void CenterAllNodes(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void RotateAllNodesCollectively(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void reversenodekeyframesrotations(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                for (int i = 0; i < node.Rotation.Count; i++)
                {
                    node.Rotation[i] = Calculator.ReverseVector4(node.Rotation[i]);
                }
                SetSaved(false);
            }
        }
        private void ChangedInspector(object? sender, SelectionChangedEventArgs? e)
        {
            if (TC_Inspector.SelectedIndex == 0)
            {
                showinfo();
            }
            if (TC_Inspector.SelectedIndex == 1)
            {
                // ShowErrors();
            }
        }
        private async void ChangedTabEvent(object? sender, SelectionChangedEventArgs? e)
        {
            if (e == null) return;
            if (e.OriginalSource is TabControl tc && tc == ListOptions)
            {
                if (ListOptions.SelectedIndex == 5)
                {
                    ChangedInspector(null, null);
                }
                if (ListOptions.SelectedIndex == 0)
                {
                    await DelayedAction(500, () => InitializeSharpGL(Scene_ViewportGL, EventArgs.Empty));
                }
                e.Handled = true;
            }
        }
        static async Task DelayedAction(int milliseconds, Action action)
        {
            await Task.Delay(milliseconds);
            action?.Invoke();
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
        private void RefreshGeosetsListRigging()
        {
            Listbox_Geosets_Rigging.Items.Clear();
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = true;

                CheckBox c = new CheckBox();
                c.IsChecked = true;
                c.Content = GetGeosetName(geoset);
                c.Checked += (sender, e) => { geoset.isVisible = true; };
                c.Unchecked += (sender, e) => { geoset.isVisible = false; };
                Listbox_Geosets_Rigging.Items.Add(new ListBoxItem() { Content = c, Width = 250, HorizontalAlignment = HorizontalAlignment.Left });
            }
        }
        private static string GetGeosetName(CGeoset geo)
        {
            return $"{geo.ObjectId} ({geo.Name}) (vertices {geo.Vertices.Count} triangles {geo.Triangles.Count})";
        }
        private void RefreshBonesInRigging()
        {
            ListBonesRiggings.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CBone) ListBonesRiggings.Items.Add(new ListBoxItem() { Content = node.Name });
            }
        }
        private async void ShowErrors()
        {
            ErrorChecker.CurrentModel = CurrentModel;

            // Generate string off the UI thread
            string[] report = await Task.Run(() => ErrorChecker.InspectAndReport(CurrentModel));

            // Now back on UI thread: update the TextBox
            ErrorPopulator.ReportErrorsWithText(Box_Errors, report[0], report[1], report[2], report[3]);

        }

        private void EditGeosetViisibilities(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; }
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
        private void createprimitiveshape(object? sender, RoutedEventArgs? e)
        {
            //  MenuPrimitiveShape.ContextMenu.IsEnabled = true;
        }
        private void MoveLayerUp(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                int index = List_Layers.SelectedIndex;
                if (index == 0) return;

                CMaterial mat = GetSelectedMAterial();
                if (mat.Layers.Count == 1) return;

                List<CMaterialLayer> tempList = new List<CMaterialLayer>();

                for (int i = 0; i < mat.Layers.Count; i++)
                {
                    if (i == index - 1)
                    {
                        // Swap the order of index-1 and index
                        tempList.Add(mat.Layers[i + 1]); // Add selected item first
                        tempList.Add(mat.Layers[i]);     // Then the one before it
                        i++; // Skip next since it's already added
                    }
                    else
                    {
                        tempList.Add(mat.Layers[i]);
                    }
                }

                mat.Layers.ObjectList = tempList;


                List_Layers.SelectedIndex = index - 1;

                CollectTexturesOpenGL();
                SetSaved(false);
            }
        }




        private void MoveLayerDown(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                int index = List_Layers.SelectedIndex;
                CMaterial mat = GetSelectedMAterial();
                if (index >= mat.Layers.Count - 1) return;

                if (mat.Layers.Count == 1) return;

                List<CMaterialLayer> tempList = new List<CMaterialLayer>();

                for (int i = 0; i < mat.Layers.Count; i++)
                {
                    if (i == index)
                    {
                        // Swap current and next
                        tempList.Add(mat.Layers[i + 1]);
                        tempList.Add(mat.Layers[i]);
                        i++; // Skip next, already added
                    }
                    else
                    {
                        tempList.Add(mat.Layers[i]);
                    }
                }

                mat.Layers.ObjectList = tempList;
                List_Layers.SelectedIndex = index + 1;
                CollectTexturesOpenGL();
                SetSaved(false);
            }
        }


        private void createnewlayer(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Textures.Count == 0) { MessageBox.Show("There are no textures"); return; }
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                layer.Alpha.MakeStatic(1);
                layer.Texture.Attach(CurrentModel.Textures[0]);
                mat.Layers.Add(layer);
                RefreshLayersList(); SetSaved(false);
            }
        }
        private void opentargetfolder(object? sender, RoutedEventArgs? e)
        {
            if (File.Exists(CurrentSaveLocation))
            {
                OpenFileLocation(CurrentSaveLocation);
            }
        }
        static void OpenFileLocation(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("File path is null or empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File was not found at that location.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string? folderPath = System.IO.Path.GetDirectoryName(filePath);
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
        private void CreateMAterial(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Textures.Count == 0) { MessageBox.Show("No textures"); return; }
            CMaterial mat = new CMaterial(CurrentModel);
            CMaterialLayer later = new CMaterialLayer(CurrentModel);
            later.Texture.Attach(CurrentModel.Textures[0]);
            mat.Layers.Add(later);
            CurrentModel.Materials.Add(mat);
            RefreshMaterialsList(); SetSaved(false);
        }
        private void createTC(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Textures.Any(x => x.ReplaceableId == 1) == false)
            {
                CTexture textue = new CTexture(CurrentModel);
                textue.ReplaceableId = 1;
                CurrentModel.Textures.Add(textue);
                RefreshTexturesList();
                RefreshLayersTextureList(); SetSaved(false);
            }
            else { MessageBox.Show("There is already TC Texture"); }
        }
        private void createTG(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Textures.Any(x => x.ReplaceableId == 2) == false)
            {
                CTexture textue = new CTexture(CurrentModel);
                textue.ReplaceableId = 2;
                CurrentModel.Textures.Add(textue);
                RefreshTexturesList(); SetSaved(false);
                RefreshLayersTextureList();
            }
            else { MessageBox.Show("There is already TG Texture"); }
        }
        private void ga_editalphas(object? sender, RoutedEventArgs? e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                transformation_editor tr = new transformation_editor(CurrentModel, ga.Alpha, true, TransformationType.Alpha);
                tr.ShowDialog();
                SetSaved(false);
            }
        }
        private void ga_editcolors(object? sender, RoutedEventArgs? e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation ga = GetSelectedGeosetAnimation();
                transformation_editor tr = new transformation_editor(CurrentModel, ga.Color, true, TransformationType.Color);
                tr.ShowDialog();
                SetSaved(false);
            }
        }
        private void editnodetranslations(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_editor tr = new transformation_editor(CurrentModel, node.Translation, false, TransformationType.Translation);

                if (tr.ShowDialog() == true)
                {
                    ListNodes_SelectedItemChanged(null, null); SetSaved(false);
                }

            }
        }
        private void editnoderotations(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_editor tr = new transformation_editor(CurrentModel, node.Rotation, false);
                if (tr.ShowDialog() == true)
                {
                    ListNodes_SelectedItemChanged(null, null);
                    SetSaved(false);
                }

            }
        }
        private void editnodescalings(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                transformation_editor tr = new transformation_editor(CurrentModel, node.Scaling, false, TransformationType.Scaling);
                if (tr.ShowDialog() == true)
                {
                    ListNodes_SelectedItemChanged(null, null); SetSaved(false);
                }
            }
        }
        private void editlayeralpha(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                transformation_editor tr = new transformation_editor(CurrentModel, layer.Alpha, true, TransformationType.Alpha);
                tr.ShowDialog();
                if (tr.DialogResult == true)
                {
                    ButtonLayerAlpha.Content = $"Edit ({layer.Alpha.Count})";
                }
                SetSaved(false);
            }
        }
        private void editlayertextureid(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                transformation_editor tr = new transformation_editor(CurrentModel, layer.TextureId, false);
                tr.ShowDialog();
                if (tr.DialogResult == true)
                {
                    ButtonLayerAlpha.Content = $"Edit ({layer.TextureId.Count})";
                }
                SetSaved(false);
            }
        }
        private void ta_edit_tr(object? sender, RoutedEventArgs? e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = GetSelectedTextureAnim();
                transformation_editor tr = new transformation_editor(CurrentModel, ta.Translation, false, TransformationType.Translation);
                tr.ShowDialog();
                Selected_TA(null, null);
                SetSaved(false);
            }
        }
        private CTextureAnimation GetSelectedTextureAnim()
        {
            return CurrentModel.TextureAnimations[List_TextureAnims.SelectedIndex];
        }
        private void ta_edit_ro(object? sender, RoutedEventArgs? e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = GetSelectedTextureAnim();
                transformation_editor tr = new transformation_editor(CurrentModel, ta.Rotation, false);
                tr.ShowDialog();
                Selected_TA(null, null);
                SetSaved(false);
            }
        }
        private void ta_edit_scaling(object? sender, RoutedEventArgs? e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                CTextureAnimation ta = GetSelectedTextureAnim();
                transformation_editor tr = new transformation_editor(CurrentModel, ta.Scaling, false, TransformationType.Scaling);
                tr.ShowDialog();
                Selected_TA(null, null);
                SetSaved(false);
            }
        }
        private void EditNodeData(object? sender, RoutedEventArgs? e)
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
                    edit.ShowDialog();
                    ListNodes_SelectedItemChanged(null, null);
                    return;
                }
                if (node is CCollisionShape cols)
                {
                    window_edit_cols edit = new window_edit_cols(cols, CurrentModel);
                    edit.ShowDialog();
                    if (RenderSettings.RenderCollisionShapes) { CurrentModel.CalculateCollisionShapeEdges(); }
                    ListNodes_SelectedItemChanged(null, null);
                    return;
                }
                if (node is CParticleEmitter emitter1)
                {
                    edit_emitter1 ei = new edit_emitter1(emitter1, CurrentModel);
                    ei.ShowDialog();
                    ListNodes_SelectedItemChanged(null, null);
                    return;
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    edit_emitter2 e2 = new edit_emitter2(emitter2, CurrentModel);
                    e2.ShowDialog();
                    ListNodes_SelectedItemChanged(null, null);
                    return;
                }
                if (node is CRibbonEmitter ribbon)
                {
                    edit_ribbon er = new edit_ribbon(CurrentModel, ribbon);
                    er.ShowDialog();
                    ListNodes_SelectedItemChanged(null, null);
                    return;
                }
                if (node is CLight light)
                {
                    Edit_light el = new Edit_light(light, CurrentModel);
                    el.ShowDialog();
                    ListNodes_SelectedItemChanged(null, null);
                    return;
                }
                if (node is CEvent ev)
                {
                    edit_eventobject edit = new edit_eventobject(CurrentModel, ev);
                    edit.ShowDialog();
                    if (edit.DialogResult == true)
                    {
                        ChangeNameOfSelectedNode(ev.Name);
                        ListNodes_SelectedItemChanged(null, null);
                    }
                }
                SetSaved(false);
            }
        }
        private void subdivide(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (var geoset in geosets)
                {
                    Calculator.SubdivideGeoset(geoset, CurrentModel);
                }
                SetSaved(false);
            }
            RefreshGeosetsList();
        }
        private void ChangeNameOfSelectedNode(string name)
        {
            TreeViewItem? item = ListNodes.SelectedItem as TreeViewItem; if (item == null) return;
            StackPanel? panel = item.Header as StackPanel; if (panel == null) return;
            TextBlock? t = panel.Children[1] as TextBlock; if (t == null) return;
            t.Text = name;
        }
        private void explain(object? sender, RoutedEventArgs? e)
        {
            Explanation h = new Explanation();
            h.ShowDialog();
        }
        private void Checked_Billlboarded(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.Billboarded = Check_Billlboarded.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_Billlboardedx(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.BillboardedLockX = Check_Billlboardedx.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_Billlboardedy(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.BillboardedLockY = Check_Billlboardedy.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_Billlboardedz(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.BillboardedLockZ = Check_Billlboardedz.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_cameraAnchored(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.CameraAnchored = Check_cameraAnchored.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_dontInhRotation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.DontInheritRotation = Check_dontInhRotation.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_DontinhScaling(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.DontInheritScaling = Check_cameraAnchored.IsChecked == true; SetSaved(false);
            }
        }
        private void Checked_dontInhTranslation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                node.DontInheritTranslation = Check_dontInhTranslation.IsChecked == true; SetSaved(false);
            }
        }
        private void SetUsedLayerTextureAnim(object? sender, SelectionChangedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void CreateTextureAnimation(object? sender, RoutedEventArgs? e)
        {
            CTextureAnimation ta = new CTextureAnimation(CurrentModel);
            CurrentModel.TextureAnimations.Add(ta);
            RefreshTextureAnims();
            RefreshLayersTextureAnimList();
            SetSaved(false);
        }


        private void SetLayerFilterMode(object? sender, SelectionChangedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                int index = List_Layers.SelectedIndex;
                mat.Layers[index].FilterMode = (EMaterialLayerFilterMode)Combo_FilterModeLayer.SelectedIndex;
                SetSaved(false);
            }
        }
        private void scr(object? sender, RoutedEventArgs? e)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            string folder = System.IO.Path.Combine(dir, "Screenshots");
            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filename = System.IO.Path.Combine(folder, "SCreenshot_" + DateTime.Now.ToString("dd mm yyyy hh mm ss") + ".png");
            CaptureScreenshot(App_window, filename);
            string? pat = System.IO.Path.GetDirectoryName(filename); if (pat == null) return;
            Process.Start("explorer.exe", pat);
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

        }
        private void scr2(object? sender, RoutedEventArgs? e)
        {
            if (ListOptions.SelectedIndex != 0) { MessageBox.Show("Select geosets tab"); return; }
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            string folder = System.IO.Path.Combine(dir, "Screenshots");
            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filename = System.IO.Path.Combine(folder, "Screenshot_" + DateTime.Now.ToString("dd mm yyyy hh mm ss") + ".png");
            CaptureScreenshot(Scene_ViewportGL, filename);
        }
        private void findTexture(object? sender, RoutedEventArgs? e)
        {
            if (TextureFinder == null) return;
            TextureFinder.ShowDialog();
        }
        private void ToggleTextures(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;

            RenderSettings.RenderTextures = !RenderSettings.RenderTextures;

            SaveSettings();
        }
        private void createcube(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            Pause = true;
            CGeoset cube = Calculator.CreateCube(CurrentModel);
            cube.Material.Attach(CurrentModel.Materials[0]);
            CreateBoneForGeoset(cube);
            CurrentModel.Geosets.Add(cube);
            RefreshGeosetsList();

            CollectTexturesOpenGL();
            RefreshGeosetsList();
            RefreshNodesTree();

            RefreshRenderData(null, null);
            Pause = false;
            RefreshSequencesList_Paths();
            RefreshPath_ModelNodes_List();
            SetSaved(false);
        }
        private void CreateBoneForGeoset(CGeoset geoset)
        {
            CBone bone = new CBone(CurrentModel);
            bone.Name = "GeneratedPrimitive_" + IDCounter.Next_;
            CGeosetGroup group = new CGeosetGroup(CurrentModel);
            CGeosetGroupNode node = new CGeosetGroupNode(CurrentModel);
            node.Node.Attach(bone);
            group.Nodes.Add(node);
            geoset.Groups.Add(group);
            foreach (var vertex in geoset.Vertices)
            {
                vertex.Group.Attach(group);
            }
            if (CurrentModel.Nodes.Contains(bone) == false) { CurrentModel.Nodes.Add(bone); }
        }
        private void createcyllinder(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            Input i = new Input("Sides");
            i.ShowDialog();
            bool parse = int.TryParse(i.Result, out int sides);
            if (!parse) { MessageBox.Show("Input not an integer"); return; }
            else
            {
                if (sides < 3) { MessageBox.Show("Sides cannot be less than 3"); return; }
                Pause = true;
                CGeoset cyllinder = Calculator.CreateCylinder(CurrentModel, 1, 2, sides);
                CreateBoneForGeoset(cyllinder);
                CurrentModel.Geosets.Add(cyllinder);
                cyllinder.Material.Attach(CurrentModel.Materials[0]);

                CollectTexturesOpenGL();
                RefreshGeosetsList();
                RefreshNodesTree();

                RefreshRenderData(null, null);
                Pause = false;
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
            }
        }
        private void createsphere(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0)
            { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            Create_Sphere cs = new Create_Sphere();
            if (cs.ShowDialog() == true)
            {
                CGeoset sphere = Calculator.CreateSphere(cs.Section, cs.Slices, CurrentModel);
                CreateBoneForGeoset(sphere);
                sphere.Material.Attach(CurrentModel.Materials[0]);
                CurrentModel.Geosets.Add(sphere);

                CollectTexturesOpenGL();
                RefreshGeosetsList();
                RefreshNodesTree();
                //
                RefreshRenderData(null, null);
                Pause = false;
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
                SetSaved(false);
            }
        }
        private void createcone(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials to apply to a new geoset"); return; }
            Input i = new Input("Sides");
            i.ShowDialog();
            bool parse = int.TryParse(i.Result, out int sides);
            if (!parse) { MessageBox.Show("Inout not and integer"); return; }
            else
            {
                if (sides < 3) { MessageBox.Show("Sides cannot be less than 3"); return; }
                Pause = true;
                CGeoset cone = Calculator.CreateCone(CurrentModel, 1, 2, sides);
                CreateBoneForGeoset(cone);
                cone.Material.Attach(CurrentModel.Materials[0]);
                CurrentModel.Geosets.Add(cone);

                CollectTexturesOpenGL();
                RefreshGeosetsList();
                RefreshNodesTree();
                //
                RefreshRenderData(null, null);
                Pause = false;
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
                SetSaved(false);
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
        private void FragmentTrianglesIntoGeosets(object? sender, RoutedEventArgs? e)
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
        private void WeldAll(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.WeldAll(geoset, CurrentModel);
                }
                RefreshGeosetsList();
                SetSaved(false);
            }
        }
        private void flattengeosetsside(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void Simplify(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.Simplify(geoset, CurrentModel);
                }
                SetSaved(false);
            }
        }
        private void showgm(object? sender, RoutedEventArgs? e)
        {
            ButtonGeosetOptions.ContextMenu.IsOpen = true;
        }
        private void showgam(object? sender, RoutedEventArgs? e)
        {
            Buttongam.ContextMenu.IsOpen = true;
        }
        private void SelectedGS(object? sender, SelectionChangedEventArgs? e)
        {
            if (ListGSequenes.SelectedItem != null)
            {
                CGlobalSequence gs = CurrentModel.GlobalSequences[ListGSequenes.SelectedIndex];
                InputGSDuration.Text = gs.Duration.ToString();
                CollectComponentsUsingGlobalSequence(gs);
            }
        }
        private void CollectComponentsUsingGlobalSequence(CGlobalSequence gs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (INode node in CurrentModel.Nodes)
            {
                if (node.Translation.GlobalSequence.Object == gs) sb.AppendLine("Node \"" + node.Name + "\"'s Translation");
                if (node.Rotation.GlobalSequence.Object == gs) sb.AppendLine("Node \"" + node.Name + "\"'s Rotation");
                if (node.Scaling.GlobalSequence.Object == gs) sb.AppendLine("Node \"" + node.Name + "\"'s Scaling");
                if (node is CParticleEmitter emitter)
                {
                    if (emitter.Visibility.GlobalSequence.Object == gs) sb.AppendLine("Emitter1 \"" + node.Name + "\"'s Visibility");
                    if (emitter.LifeSpan.GlobalSequence.Object == gs) sb.AppendLine("Emitter1 \"" + node.Name + "\"'s LifeSpan");
                    if (emitter.InitialVelocity.GlobalSequence.Object == gs) sb.AppendLine("Emitter1 \"" + node.Name + "\"'s InitialVelocity");
                    if (emitter.Latitude.GlobalSequence.Object == gs) sb.AppendLine("Emitter1 \"" + node.Name + "\"'s Latitude");
                    if (emitter.Longitude.GlobalSequence.Object == gs) sb.AppendLine("Emitter1 \"" + node.Name + "\"'s Longitude");
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    if (emitter2.Latitude.GlobalSequence.Object == gs) sb.AppendLine("Emitter2 \"" + node.Name + "\"'s Latitude");
                    if (emitter2.Width.GlobalSequence.Object == gs) sb.AppendLine("Emitter2 \"" + node.Name + "\"'s Width");
                    if (emitter2.Length.GlobalSequence.Object == gs) sb.AppendLine("Emitter2 \"" + node.Name + "\"'s Length");
                    if (emitter2.Gravity.GlobalSequence.Object == gs) sb.AppendLine("Emitter2 \"" + node.Name + "\"'s Gravity");
                    if (emitter2.Speed.GlobalSequence.Object == gs) sb.AppendLine("Emitter2 \"" + node.Name + "\"'s Speed");
                    if (emitter2.Variation.GlobalSequence.Object == gs) sb.AppendLine("Emitter2 \"" + node.Name + "\"'s Variation");
                }
                if (node is CLight light)
                {
                    if (light.AmbientColor.GlobalSequence.Object == gs) sb.AppendLine("Light \"" + node.Name + "\"'s AmbientColor");
                    if (light.Color.GlobalSequence.Object == gs) sb.AppendLine("Light \"" + node.Name + "\"'s Color");
                    if (light.Intensity.GlobalSequence.Object == gs) sb.AppendLine("Light \"" + node.Name + "\"'s Intensity");
                    if (light.AmbientIntensity.GlobalSequence.Object == gs) sb.AppendLine("Light \"" + node.Name + "\"'s AmbientIntensity");
                }
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Color.GlobalSequence.Object == gs) sb.AppendLine("Ribbon \"" + node.Name + "\"'s Color");
                    if (ribbon.HeightAbove.GlobalSequence.Object == gs) sb.AppendLine("Ribbon \"" + node.Name + "\"'s HeightAbove");
                    if (ribbon.HeightBelow.GlobalSequence.Object == gs) sb.AppendLine("Ribbon \"" + node.Name + "\"'s HeightBelow");
                    if (ribbon.TextureSlot.GlobalSequence.Object == gs) sb.AppendLine("Ribbon \"" + node.Name + "\"'s TextureSlot");
                }
            }
            foreach (var ga in CurrentModel.GeosetAnimations)
            {
                if (ga.Alpha.GlobalSequence.Object == gs) sb.AppendLine($"Geoset animation {ga.ObjectId}'s alpha");
                if (ga.Color.GlobalSequence.Object == gs) sb.AppendLine($"Geoset animation {ga.ObjectId}'s color");
            }
            foreach (var ta in CurrentModel.TextureAnimations)
            {
                if (ta.Translation.GlobalSequence.Object == gs) sb.AppendLine($"texture animation {ta.ObjectId}'s translation");
                if (ta.Rotation.GlobalSequence.Object == gs) sb.AppendLine($"texture animation {ta.ObjectId}'s rotation");
                if (ta.Scaling.GlobalSequence.Object == gs) sb.AppendLine($"texture animation {ta.ObjectId}'s scaling");
            }
            foreach (var m in CurrentModel.Materials)
            {
                foreach (var l in m.Layers)
                {
                    if (l.Alpha.GlobalSequence.Object == gs) sb.AppendLine($"Material {m.ObjectId}'s Layer {l.ObjectId}'s alpha");
                    if (l.TextureId.GlobalSequence.Object == gs) sb.AppendLine($"Material {m.ObjectId}'s Layer {l.ObjectId}'s textureID");
                }
            }
            foreach (var cam in CurrentModel.Cameras)
            {
                if (cam.Translation.GlobalSequence.Object == gs) sb.AppendLine($"camera \"{cam.Name}\"'s translation");
                if (cam.TargetTranslation.GlobalSequence.Object == gs) sb.AppendLine($"camera \"{cam.Name}\"'s target");
                if (cam.Rotation.GlobalSequence.Object == gs) sb.AppendLine($"camera \"{cam.Name}\"'s rotation");
            }
            Report_GSequence_UsedNodes.Text = sb.ToString();
        }
        private void addgs(object? sender, RoutedEventArgs? e)
        {
            bool parse = int.TryParse(InputGSDuration.Text, out int duration);
            if (!parse) { MessageBox.Show("Expected integer"); return; }
            else
            {
                if (duration <= 0) { MessageBox.Show("Expected positive integer"); return; }
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
        private void editgs(object? sender, RoutedEventArgs? e)
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
        private void delgs(object? sender, RoutedEventArgs? e)
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
                        SetSaved(false);
                    }
                }
                else
                {
                    CurrentModel.GlobalSequences.RemoveAt(ListGSequenes.SelectedIndex);
                    RefreshGlobalSequencesList();
                    SetSaved(false);
                }
            }
        }
        private static bool askDelete()
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
        private void negateusofgeosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void negavsofgeosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void fituvgeoet(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void swapusvsgeosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void gotogeosettexture(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                int index = CurrentModel.Materials.IndexOf(geosets[0].Material.Object);
                List_MAterials.SelectedIndex = index;
                ListOptions.SelectedIndex = 3;
            }
            else
            {
                MessageBox.Show("Select a single geoset");
            }
        }
        private void gotogeosetbone(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 0) { return; }
            List<CGeoset> geosets = GetSelectedGeosets();
            CBone? AttachedToBone = null;
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
                        break;
                    }
                }

                if (same)
                {
                    string name = AttachedToBone == null ? string.Empty : AttachedToBone.Name;
                    SelectNodeByName(name);
                    ListOptions.SelectedIndex = 2;
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
        private static bool SelectNodeByNameRecursive(TreeViewItem node, string name)
        {
            // Ensure the node's header is correctly structured
            if (node.Header is StackPanel container)
            {
                TextBlock? nameContainer = container.Children.OfType<TextBlock>().FirstOrDefault();
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
        private void negatenormals(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void makegassameasga(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void ImportSequences(object? sender, RoutedEventArgs? e)
        {
            CModel? TemporaryModel = GetTemporaryModel();
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
            SetSaved(false);
        }

        private void Checked_WW(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                texture.WrapWidth = Check_WW.IsChecked == true;
                SetSaved(false);
            }
        }
        private void Checked_WH(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                texture.WrapHeight = Check_WH.IsChecked == true;
                SetSaved(false);
            }
        }
        private void SelectedTexture(object? sender, SelectionChangedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                CTexture texture = GetSElectedTexture();
                Check_WH.IsChecked = texture.WrapHeight;
                Check_WW.IsChecked = texture.WrapWidth;
            }
        }
        private void SelectedSequence(object? sender, SelectionChangedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CSequence sequence = CurrentModel.Sequences[ListSequenes.SelectedIndex];
                InputRarity.Text = sequence.Rarity.ToString();
                InputMovespeed.Text = sequence.MoveSpeed.ToString();
                CheckLooping.IsChecked = sequence.NonLooping;
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
        private void Hotkeys(object? sender, KeyEventArgs e)
        {
            //  if (e.Key == Key.Escape) {
            // var focusedElement = FocusManager.GetFocusedElement(this);
            //   MessageBox.Show(focusedElement?.GetType().Name ?? "Nothing focused");
            // } // TEST
            // if (e.Key == Key.A) { InitializeSharpGL(Scene_ViewportGL, EventArgs.Empty); }
            if (e.Key == Key.LeftCtrl) { SetSelectionMode(mSelectionMode.Remove); }
            else if (e.Key == Key.LeftShift) { SetSelectionMode(mSelectionMode.Add); }

            //================================================================
            if (e.Key == Key.Delete) { DeleteBasedOnFocus(); return; }
            else if (e.Key == Key.Space) { unselectAll(null, null); return; }
            else if (e.Key == Key.Up) { CameraControl.CenterZ += 1; return; }
            else if (e.Key == Key.Down) { CameraControl.CenterZ -= 1; return; }
            else if (e.Key == Key.M) { SetWorkModeMove(null, null); }
            else if (e.Key == Key.P) { edituv_mini(null, null); }
            else if (e.Key == Key.R) { SetWorkModeRotate(null, null); }
            else if (e.Key == Key.C) { SetWorkModeScale(null, null); }
            else if (e.Key == Key.U) { setAxsModeU(null, null); }
            else if (e.Key == Key.X) { setAxsModeX(null, null); }
            else if (e.Key == Key.Y) { setAxsModeY(null, null); }
            else if (e.Key == Key.Z) { setAxsModeZ(null, null); }
            else if (e.Key == Key.V) { Menuitem_vertices.IsChecked = true; }
            else if (e.Key == Key.E) { Menuitem_edges.IsChecked = true; }
            else if (e.Key == Key.N) { Menuitem_Nodes.IsChecked = true; }
            else if (e.Key == Key.K) { Menuitem_skeleton.IsChecked = true; }
            else if (e.Key == Key.L) { Menuitem_normals.IsChecked = true; }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.S) SaveModel(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.A) selectAall(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.N) newm(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.I) selectInverse(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && e.Key == Key.S) saveas(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.O) load(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.R) reload(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.F) refreshalllists(null, null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.T)
            {
                bool c = Menuitem_Textured.IsChecked == true;
                Menuitem_Textured.IsChecked = !c;
            }
        }

        private void DeleteBasedOnFocus()
        {
            var focusedElement = FocusManager.GetFocusedElement(this);
            if (focusedElement == null) return;

            if (ListGeosets.IsAncestorOf((DependencyObject)focusedElement))
            {
                delgeoset(null, null);
            }
            else if (ListSequenes.IsAncestorOf((DependencyObject)focusedElement))
            {
                delseqalong(null, null);
            }
            else if (ListPaths.IsAncestorOf((DependencyObject)focusedElement))
            {
                Delath(null, null);
            }
            else if (ListPathNodes.IsAncestorOf((DependencyObject)focusedElement))
            {
                delPathNode(null, null);
            }
            else if (List_MAterials.IsAncestorOf((DependencyObject)focusedElement))
            {
                DelMAterial(null, null);
            }
            else if (List_TextureAnims.IsAncestorOf((DependencyObject)focusedElement))
            {
                DeleteTextureAnim(null, null);
            }
            else if (List_Textures.IsAncestorOf((DependencyObject)focusedElement))
            {

                delTexture(null, null);
            }
            else if (List_Layers.IsAncestorOf((DependencyObject)focusedElement))
            {
                DelLayer(null, null);
            }
            else if (List_Keyframes_Animator.IsAncestorOf((DependencyObject)focusedElement))
            {
                DeleteKeyframe(null, null);
            }
        }


        private void explain2(object? sender, RoutedEventArgs? e)
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
        private void CreatePresets(object? sender, RoutedEventArgs? e)
        {
            presets p = new presets(CurrentModel);
            p.ShowDialog();
            if (p.DialogResult == true)
            {
                RefreshSequencesList();
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
                History.Sequences.Add(CurrentModel);
            }
        }
        private void removesequencesfromnodetransfromations(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                if (CurrentModel.Sequences.Count == 0)
                {
                    MessageBox.Show("There are no sequences", "Nothing to work with"); return;
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
        private void duplicatenode(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                INode clone = NodeCloner.Clone(node, CurrentModel);
                clone.Name = clone.Name = "DuplcatedNode_" + IDCounter.Next_;
                CurrentModel.Nodes.Add(clone);
                RefreshNodesTree();
                SetSaved(false);
            }
        }
        private void resetkeyframes(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null)
                return;

            INode node = GetSeletedNode();

            transformation_selector ts = new transformation_selector();
            bool? result = ts.ShowDialog();

            if (result != true)
                return;

            bool resetTranslation = ts.C1.IsChecked == true;
            bool resetRotation = ts.C2.IsChecked == true;
            bool resetScaling = ts.C3.IsChecked == true;

            if (resetTranslation)
            {
                for (int i = 0; i < node.Translation.Count; i++)
                {
                    var old = node.Translation[i];
                    node.Translation[i] = new CAnimatorNode<CVector3>(old.Time, new CVector3(0, 0, 0));
                }
            }

            if (resetRotation)
            {
                for (int i = 0; i < node.Rotation.Count; i++)
                {
                    var old = node.Rotation[i];
                    node.Rotation[i] = new CAnimatorNode<CVector4>(old.Time, new CVector4(0, 0, 0, 0));
                }
            }

            if (resetScaling)
            {
                for (int i = 0; i < node.Scaling.Count; i++)
                {
                    var old = node.Scaling[i];
                    node.Scaling[i] = new CAnimatorNode<CVector3>(old.Time, new CVector3(0, 0, 0));
                }
            }
        }


        private void createpixies(object? sender, RoutedEventArgs? e)
        {
            INode cloned = NodeCloner.Clone(NodeMaker.ItemPixie, CurrentModel);
            HandleRequiredTexture((CParticleEmitter2)cloned);
            cloned.Name = "ItemPixies_" + IDCounter.Next_; ;
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
            SetSaved(false);
        }
        private void createdust(object? sender, RoutedEventArgs? e)
        {
            INode cloned = NodeCloner.Clone(NodeMaker.Dust, CurrentModel);
            cloned.Name = "Dust_" + IDCounter.Next_; ;
            HandleRequiredTexture((CParticleEmitter2)cloned);
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
            SetSaved(false);
        }
        private void HandleRequiredTexture(CParticleEmitter2 cloned)
        {
            if (cloned.RequiredTexturePath.Length > 0)
            {
                if (CurrentModel.Textures.Any(x => x.FileName == cloned.RequiredTexturePath))
                {
                    CTexture texture = CurrentModel.Textures.First(x => x.FileName == cloned.RequiredTexturePath);
                    cloned.Texture.Attach(texture);
                    RefreshTexturesList();
                }
                else
                {
                    CTexture _new = new CTexture(CurrentModel);
                    _new.FileName = cloned.RequiredTexturePath;
                    CurrentModel.Textures.Add(_new);
                    RefreshTexturesList();
                    cloned.Texture.Attach(_new);
                }
            }
        }
        private void createsmoke(object? sender, RoutedEventArgs? e)
        {
            INode cloned = NodeCloner.Clone(NodeMaker.Smoke, CurrentModel);
            HandleRequiredTexture((CParticleEmitter2)cloned);
            cloned.Name = "Smoke_" + IDCounter.Next_; ;
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
            SetSaved(false);
        }

        private void cameras(object? sender, RoutedEventArgs? e)
        {
            cameraManager cm = new cameraManager(CurrentModel);
            cm.ShowDialog();
        }
        private void findnode(object? sender, RoutedEventArgs? e)
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
        private void Checked_MatSort2(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                CMaterial material = GetSelectedMAterial();
                material.SortPrimitivesNearZ = Check_MatSort2.IsChecked == true;
            }
        }

        private void delallgas(object? sender, RoutedEventArgs? e)
        {
            CurrentModel.GeosetAnimations.Clear();
            List_GeosetAnims.Items.Clear();
            Label_GAs.Text = "0 Geoset Animations";
            SetSaved(false);
        }
        private void createmissinggas(object? sender, RoutedEventArgs? e)
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
            Optimizer.ArrangeGEosetAnimations(CurrentModel);
            RefreshGeosetAnimationsList();
            SetSaved(false);
        }
        private void separate(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                Pause = true;
                //  Scene_Viewport3D.Children.Clear();
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
                SetSaved(false);

            }
        }
        private void drawshape(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Textures.Count == 0)
            {
                MessageBox.Show("There are no textures"); return;
            }
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no textures"); return;
            }
            if (CurrentModel.Materials[0].Layers.Count == 0)
            {
                MessageBox.Show("material 0 has no layers"); return;
            }
            DrawShapeWindow ds = new DrawShapeWindow(CurrentModel);
            if (ds.ShowDialog() == true)
            {
                RefreshGeosetsList();
                SetSaved(false);
                RefreshRenderData(null, null);
            }
        }
        private void extrudedpolygon(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0 || CurrentModel.Textures.Count == 0)
            {
                MessageBox.Show("There are no materials. At least one material is needed, to be applied to a generated geoset."); return;
            }
            CreatePolygonWindow cpw = new CreatePolygonWindow(CurrentModel);
            cpw.ShowDialog();
            if (cpw.DialogResult == true)
            {
                Pause = true;
                RefreshGeosetsList();

                CollectTexturesOpenGL();
                RefreshRenderData(null, null);
                RefreshNodesTree();
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
                SetSaved(false);
                Pause = false;
            }
        }
        private void FragmentTrianglesInGeoset(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void FragmentFacesInGeoset(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);

            }
        }
        private void FragmentFacesIntoGeosets(object? sender, RoutedEventArgs? e)
        {
            // Scene_Viewport3D.Children.Clear();
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
            SetSaved(false);
        }
        private void load_from_mpqs(object? sender, RoutedEventArgs? e)
        {
            ModelBrowser browser = new ModelBrowser();
            if (browser.ShowDialog() == true)
            {
                if (browser.Selected == null)
                {
                    MessageBox.Show("null string"); return;
                }
                MPQHelper.Export(browser.Selected, AppHelper.TemporaryModelLocation, "", true);
                LoadModel(AppHelper.TemporaryModelLocation);
            }
        }
        private void ToggleShading(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderShading = Menuitem_Shading.IsChecked == true;

            SaveSettings();
        }
        private void ToggleCols(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderCollisionShapes = Menuitem_Cols.IsChecked == true;
            SetSaved(false);
            SaveSettings();
        }
        private void RotateGeosetsTogether(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void Clampuvofgeoset(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void ListGSequenes_MouseDoubleClick(object? sender, MouseButtonEventArgs e)
        {
        }
        #region Animator

        private void SelectedSEquenceInAnimator(object? sender, SelectionChangedEventArgs? e)
        {
            Animator_RefreshKeyframesList();
            if (List_Keyframes_Animator.Items.Count > 0) List_Keyframes_Animator.SelectedIndex = 0;

        }
        private void UpdateManualInputForAnimator()
        {
            if (Tabs_Geosets.TabIndex != 4) return;
            float ctrack = Animator_GetTrack();
            INode? node = animator_GetNode();
            if (node == null) { return; }
            float x = 0;
            float y = 0;
            float z = 0;
            if (ctrack != -1 && node != null)
            {
                var t = node.Translation.FirstOrDefault(x => x.Time == ctrack);
                var r = node.Rotation.FirstOrDefault(x => x.Time == ctrack);
                var s = node.Scaling.FirstOrDefault(x => x.Time == ctrack);
                if (modifyMode_current == ModifyMode.Translate && t != null)
                {

                    x = t.Value.X;
                    y = t.Value.Y;
                    z = t.Value.Z;

                }
                if (modifyMode_current == ModifyMode.Rotate && r != null)
                {
                    CVector3 euler = Calculator.QuaternionToEuler(r.Value);
                    x = euler.X;
                    y = euler.Y;
                    z = euler.Z;
                }
                if (modifyMode_current == ModifyMode.Scale && s != null)
                {
                    x = s.Value.X * 100;
                    y = s.Value.Y * 100;
                    z = s.Value.Z * 100;
                }
                InputAnimatorX.Text = x.ToString();
                InputAnimatorY.Text = y.ToString();
                InputAnimatorZ.Text = z.ToString();
            }
        }
        private INode? GetSelectedNodeInanimator()
        {
            string? name = Extractor.GetString(List_Nodes_Animator);
            if (name == null) return null;
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void ResetKeyframeTranslations(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt(List_Keyframes_Animator);
                ResetKeyframe(track, TransformationType.Translation);
                RefreshAnimatedVertexAndNodePositionsForRendering();
            }
        }
        private void DeleteKeyframe(object? sender, RoutedEventArgs? e)
        {
            int time = Extractor.GetInt(List_Keyframes_Animator);
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CHelper || node is CBone)
                {
                    node.Translation.NodeList.RemoveAll(x => x.Time == time);
                    node.Rotation.NodeList.RemoveAll(x => x.Time == time);
                    node.Scaling.NodeList.RemoveAll(x => x.Time == time);
                }
            }
            SetSaved(false);
        }
        private void ResetKeyframeRotations(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt(List_Keyframes_Animator);
                ResetKeyframe(track, TransformationType.Rotation);
                RefreshAnimatedVertexAndNodePositionsForRendering();
                SetSaved(false);
            }
        }
        private void ResetKeyframe(int track, TransformationType t)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                if (t == TransformationType.Translation)
                {
                    var f = node.Translation.FirstOrDefault(x => x.Time == track);
                    if (f != null) f.Value = new CVector3();
                }
                if (t == TransformationType.Rotation)
                {
                    var f = node.Rotation.FirstOrDefault(x => x.Time == track);
                    if (f != null) f.Value = new CVector4(0, 0, 0, 1);
                }
                if (t == TransformationType.Scaling)
                {
                    var f = node.Scaling.FirstOrDefault(x => x.Time == track);
                    if (f != null) f.Value = new CVector3(1, 1, 1);
                }
            }
            SetSaved(false);
        }
        private void ResetKeyframeScalings(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt((List_Keyframes_Animator));
                ResetKeyframe(track, TransformationType.Scaling);
                RefreshAnimatedVertexAndNodePositionsForRendering();
                SetSaved(false);
            }
        }



        private void SelectedKeyframeInAnimator(object? sender, SelectionChangedEventArgs? e)
        {
            if (List_Keyframes_Animator?.SelectedItem is not ListBoxItem item || item.Content == null)
                return;

            string content = item.Content.ToString()!;
            string[] parts = content.Split(' ');
            if (parts.Length == 0 || !int.TryParse(parts[0], out int time))
                return;

            InputCurrentFrame.Text = time.ToString();
            // LoadKeyframeInViewport(time);
            FillInputsForKeyframe(time);
            RefreshAnimatedVertexAndNodePositionsForRendering(time);
        }


        private void FillInputsForKeyframe(int time)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                var node = GetSelectedNodeInanimator();
                if (node == null) return;
                if (modifyMode_current == ModifyMode.Translate)
                {
                    var kf = node.Translation.FirstOrDefault(x => x.Time == time);
                    if (kf != null)
                    {
                        InputAnimatorX.Text = kf.Value.X.ToString();
                        InputAnimatorY.Text = kf.Value.Y.ToString();
                        InputAnimatorZ.Text = kf.Value.Z.ToString();
                    }
                    else
                    {
                        InputAnimatorX.Text = "0";
                        InputAnimatorY.Text = "0";
                        InputAnimatorZ.Text = "0";
                    }
                }
                else if (modifyMode_current == ModifyMode.Rotate)
                {
                    var kf = node.Rotation.FirstOrDefault(x => x.Time == time);
                    if (kf != null)
                    {
                        var euler = Calculator.QuaternionToEuler(kf.Value);
                        InputAnimatorX.Text = euler.X.ToString();
                        InputAnimatorY.Text = euler.Y.ToString();
                        InputAnimatorZ.Text = euler.Z.ToString();
                    }
                    else
                    {
                        InputAnimatorX.Text = "0";
                        InputAnimatorY.Text = "0";
                        InputAnimatorZ.Text = "0";
                    }
                }
                else if (modifyMode_current == ModifyMode.Scale)
                {
                    var kf = node.Scaling.FirstOrDefault(x => x.Time == time);
                    if (kf != null)
                    {


                        InputAnimatorX.Text = (kf.Value.X * 100).ToString();
                        InputAnimatorY.Text = (kf.Value.Y * 100).ToString();
                        InputAnimatorZ.Text = (kf.Value.Z * 100).ToString();
                    }
                    else
                    {
                        InputAnimatorX.Text = "100";
                        InputAnimatorY.Text = "100";
                        InputAnimatorZ.Text = "100";
                    }
                }


            }
        }


        private void RefreshBonesInAnimator()
        {
            if (List_Nodes_Animator == null) { return; }
            List_Nodes_Animator.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                if (NodesVisibleInAnimator[NodeType.Bone] && node is CBone) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Helper] && node is CHelper) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Emitter1] && node is CParticleEmitter) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Emitter2] && node is CParticleEmitter2) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Light] && node is CLight) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Event] && node is CEvent) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Cols] && node is CCollisionShape) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Ribbon] && node is CRibbonEmitter) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
                if (NodesVisibleInAnimator[NodeType.Attachment] && node is CAttachment) List_Nodes_Animator.Items.Add(new ListBoxItem() { Content = node.Name });
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
        private int CurrentlySelectedTrack = -1;
        private void gototrack(object? sender, RoutedEventArgs? e)
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
                        CreateTrackIfMissing(CurrentlySelectedTrack);
                        RefreshAnimatedVertexAndNodePositionsForRendering();
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
        private void CreateTrackIfMissing(int track)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                var bone = GetSeletedNodeInAnimator();
                bool hasTrack =
                    bone.Translation.NodeList.Any(x => x.Time == track) == true ||
                   bone.Rotation.NodeList.Any(x => x.Time == track) == true ||
                   bone.Scaling.NodeList.Any(x => x.Time == track) == true
                   ;
                if (hasTrack) return;
                if (bone.Translation.NodeList.Any(x => x.Time == track) == false)
                {
                    var kf = new CAnimatorNode<CVector3>();
                    kf.Time = track;
                    kf.Value = new CVector3(0, 0, 0);
                    bone.Translation.Add(kf);
                    bone.Translation.NodeList = bone.Translation.NodeList.OrderBy(x => x.Time).ToList();
                }
                if (bone.Rotation.NodeList.Any(x => x.Time == track) == false)
                {
                    var kf = new CAnimatorNode<CVector4>();
                    kf.Time = track;
                    kf.Value = new CVector4(0, 0, 0, 1);
                    bone.Rotation.Add(kf);
                    bone.Rotation.NodeList = bone.Rotation.NodeList.OrderBy(x => x.Time).ToList();
                }
                if (bone.Scaling.NodeList.Any(x => x.Time == track) == false)
                {
                    var kf = new CAnimatorNode<CVector3>();
                    kf.Time = track;
                    kf.Value = new CVector3(1, 1, 1);
                    bone.Scaling.Add(kf);
                    bone.Scaling.NodeList = bone.Scaling.NodeList.OrderBy(x => x.Time).ToList();
                }
                Animator_RefreshTracksList(track);
            }
        }
        private void Animator_RefreshTracksList(int select = -1)
        {
            if (List_Sequences_Animator.SelectedItem != null && List_Nodes_Animator.SelectedItem != null)
            {
                List<int> tracks = new List<int>();
                List_Keyframes_Animator.Items.Clear();
                var sequecne = GetSelectedSequenceAnimator();
                INode? node = GetSelectedNodeInanimator(); if (node == null) return;
                foreach (var kf in node.Translation)
                {
                    if (kf.Time >= sequecne.IntervalStart && kf.Time <= sequecne.IntervalEnd)
                    {
                        if (tracks.Contains(kf.Time) == false)
                        {
                            tracks.Add(kf.Time);
                        }
                    }
                }
                foreach (int t in tracks)
                {
                    string c = GetKeyframeTitle(node, t);
                    ListBoxItem listItem = new ListBoxItem() { Content = c };
                    List_Keyframes_Animator.Items.Add(listItem);
                    if (t == select) { List_Keyframes_Animator.SelectedItem = listItem; }
                }
            }
        }

        private static string GetKeyframeTitle(INode? node, int tr)
        {
            if (node == null) return string.Empty;
            bool un = true;
            string t = "";
            string r = "";
            string s = "";
            if (node.Translation.Any(x => x.Time == tr)) { t = "(T)"; un = false; }
            if (node.Rotation.Any(x => x.Time == tr)) { t = "(R)"; un = false; }
            if (node.Scaling.Any(x => x.Time == tr)) { t = "(S)"; un = false; }
            return un ? $"{tr} (Unanimated)" : $"{tr} {t}{r}{s}";
        }


        #endregion
        private void DetachBoneFromSelectedVertices(object? sender, RoutedEventArgs? e)
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

        private INode GetSelectedAttachedNode()
        {
            string? name = Extractor.GetString(ListAttachedToRiggings);
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void ClearAndATtach(object? sender, RoutedEventArgs? e)
        {
            if (ListBonesRiggings.SelectedItem == null)
            {
                MessageBox.Show("First select a bone from the list of bones", "Invalid request"); return;
            }
            else
            {
                INode? node = GetSelectedNode_Rigging();
                if (node == null) { return; }
                List<CGeosetVertex> vertices = GetSelectedVertices();
                if (vertices.Count == 0) { MessageBox.Show("Select vertices in the viewport"); return; }
                if (NodeExistsInRiggingList(node.Name)) { MessageBox.Show("this node already is attached"); return; }

                EditMatrixGroup(RiggingAction.ClearAdd, node, vertices);
                ListAttachedToRiggings.Items.Clear();
                ListAttachedToRiggings.Items.Add(new ListBoxItem() { Content = node.Name });
            }
        }

        private INode? GetSelectedNode_Rigging()
        {
            ListBoxItem? item = ListBonesRiggings.SelectedItem as ListBoxItem; if (item == null) return null;
            string? name = item.Content as string;
            return CurrentModel.Nodes.First(x => x.Name == name);

        }



        private List<CGeosetVertex> GetSelectedVertices()
        {
            List<CGeosetVertex> list = new List<CGeosetVertex>();
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    if (vertex.isSelected)
                    {
                        list.Add(vertex);
                    }
                }
            }
            return list;
        }
        private void AddAttachRiggin(object? sender, RoutedEventArgs? e)
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
                    INode? node = getselectedBoneInRigging(); if (node == null) return;
                    List<CGeosetVertex> vertices = GetSelectedVertices();
                    if (vertices.Count == 0) { MessageBox.Show("Select vertices in the viewport"); return; }
                    if (NodeExistsInRiggingList(node.Name)) { MessageBox.Show("this node already is attached"); return; }
                    EditMatrixGroup(RiggingAction.Add, node, vertices);
                    ListAttachedToRiggings.Items.Add(new ListBoxItem() { Content = node.Name });
                }
            }
        }

        private bool NodeExistsInRiggingList(string name)
        {
            foreach (var item in ListAttachedToRiggings.Items)
            {
                if (Extractor.GetString(item) == name) { return true; }
            }
            return false;
        }

        private void ReverseAllSequences(object? sender, RoutedEventArgs? e)
        {
            foreach (var sequence in CurrentModel.Sequences)
            {
                ReverseSequence(sequence);
            }
        }
        int CopedKeyframe = -1;
        private void CopyKeyframe(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt(List_Keyframes_Animator);
                CopedKeyframe = track;
            }
        }
        private void Paste_Ov_Keyframe(object? sender, RoutedEventArgs? e)
        {

            if (CopedKeyframe != -1)
            {
                Input i = new Input("New track");
                if (i.ShowDialog() == true)
                {
                    bool b = int.TryParse(i.Result, out int pastedOn);
                    if (b)
                    {
                        if (pastedOn == CopedKeyframe)
                        {
                            MessageBox.Show("Copied cannot be the same as pasted"); return;
                        }
                        else
                        {
                            select_Transformations st = new select_Transformations();
                            if (st.ShowDialog() == true)
                            {
                                bool t = st.Check_T.IsChecked == true;
                                bool r = st.Check_R.IsChecked == true;
                                bool s = st.Check_S.IsChecked == true;
                                foreach (var node in CurrentModel.Nodes)
                                {
                                    if (t)
                                    {
                                        var kf = node.Translation.FirstOrDefault(x => x.Time == CopedKeyframe);
                                        var pasted = node.Translation.FirstOrDefault(x => x.Time == pastedOn);
                                        if (kf != null)
                                            if (pasted == null)
                                            {
                                                CAnimatorNode<CVector3> n = new CAnimatorNode<CVector3>();
                                                n.Time = pastedOn;
                                                n.Value = new CVector3(kf.Value);
                                            }
                                            else
                                            {
                                                kf.Value = new CVector3(kf.Value);
                                            }
                                    }

                                    if (r)
                                    {
                                        var kf = node.Rotation.FirstOrDefault(x => x.Time == CopedKeyframe);
                                        var pasted = node.Rotation.FirstOrDefault(x => x.Time == pastedOn);
                                        if (kf != null)
                                            if (pasted == null)
                                            {
                                                CAnimatorNode<CVector4> n = new CAnimatorNode<CVector4>();
                                                n.Time = pastedOn;
                                                n.Value = new CVector4(kf.Value);
                                            }
                                            else
                                            {
                                                kf.Value = new CVector4(kf.Value);
                                            }
                                    }
                                    if (s)
                                    {
                                        var kf = node.Scaling.FirstOrDefault(x => x.Time == CopedKeyframe);
                                        var pasted = node.Scaling.FirstOrDefault(x => x.Time == pastedOn);
                                        if (kf != null)
                                            if (pasted == null)
                                            {
                                                CAnimatorNode<CVector3> n = new CAnimatorNode<CVector3>();
                                                n.Time = pastedOn;
                                                n.Value = new CVector3(kf.Value);
                                            }
                                            else
                                            {
                                                kf.Value = new CVector3(kf.Value);
                                            }
                                    }
                                }
                            }

                        }
                        SetSaved(false);
                    }
                    else
                    {
                        MessageBox.Show("Expected an integer"); return;
                    }

                }
            }
        }




        private void makeallGAUseColor(object? sender, RoutedEventArgs? e)
        {
            foreach (var ga in CurrentModel.GeosetAnimations) ga.UseColor = true;
            RefreshGeosetAnimationsList();
            SetSaved(false);
        }
        private void makeallGAUseColorNOT(object? sender, RoutedEventArgs? e)
        {
            foreach (var ga in CurrentModel.GeosetAnimations) ga.UseColor = false;
            RefreshGeosetAnimationsList();
            SetSaved(false);
        }
        //--------------------------------------------------
        // ANIMATOR
        //--------------------------------------------------
        private void Animator_ClearTranslations(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            node.Translation.Clear();
            RefreshAnimatorData();
            RefreshAnimatedVertexAndNodePositionsForRendering();
            SetSaved(false);
        }
        private void RefreshAnimatorData()
        {
            RefreshBonesInAnimator();
            RefreshSequencesInAnimator();
            UnselectAllVerticesAnimator();
            List_Keyframes_Animator.Items.Clear();
            //throw new NotImplementedException();
            SetSaved(false);
        }
        private void UnselectAllVerticesAnimator()
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var v in geoset.Vertices)
                {
                    v.isSelected = false;
                }
            }

        }
        private INode GetSeletedNodeInAnimator()
        {
            string name = GetNodeNameAnimator();
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void Animator_ClearRotations(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            node.Rotation.Clear();
            RefreshAnimatorData();
            RefreshAnimatedVertexAndNodePositionsForRendering();
            SetSaved(false);
        }
        private void Animator_ClearScalings(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            node.Scaling.Clear();
            RefreshAnimatorData();
            RefreshAnimatedVertexAndNodePositionsForRendering();
            SetSaved(false);
        }
        private int Animator_GetTrack()
        {
            bool parsed = int.TryParse(InputCurrentFrame.Text, out int value);
            if (parsed) return value;
            return -1;
        }
        private void Animator_ClearTranslations_Track(object? sender, RoutedEventArgs? e)
        {
            int track = Animator_GetTrack();
            if (track != -1)
            {
                INode node = GetSeletedNodeInAnimator();
                node.Translation.NodeList.RemoveAll(x => x.Time == track);
                RefreshAnimatorData();
                RefreshAnimatedVertexAndNodePositionsForRendering();
                SetSaved(false);
            }
        }
        private void Animator_ClearRotations_Track(object? sender, RoutedEventArgs? e)
        {
            int track = Animator_GetTrack();
            if (track != -1)
            {
                INode node = GetSeletedNodeInAnimator();
                node.Rotation.NodeList.RemoveAll(x => x.Time == track);
                RefreshAnimatorData();
                RefreshAnimatedVertexAndNodePositionsForRendering();
                SetSaved(false);
            }
        }
        private void Animator_ClearScalings_Track(object? sender, RoutedEventArgs? e)
        {
            int track = Animator_GetTrack();
            if (track != -1)
            {
                INode node = GetSeletedNodeInAnimator();
                node.Scaling.NodeList.RemoveAll(x => x.Time == track);
                RefreshAnimatorData();
                RefreshAnimatedVertexAndNodePositionsForRendering();
                SetSaved(false);
            }
        }
        private void Animator_ResetTranslations(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            foreach (var kf in node.Translation)
            {
                kf.Value.X = 0;
                kf.Value.Y = 0;
                kf.Value.Z = 0;
            }
            RefreshAnimatedVertexAndNodePositionsForRendering();
            SetSaved(false);
        }
        private void Animator_ResetRotations(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            foreach (var kf in node.Rotation)
            {
                kf.Value.X = 0;
                kf.Value.Y = 0;
                kf.Value.Z = 0;
                kf.Value.W = 1;
            }
            RefreshAnimatedVertexAndNodePositionsForRendering();
            SetSaved(false);
        }
        private void Animator_ResetScalings(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            foreach (var kf in node.Scaling)
            {
                kf.Value.X = 1;
                kf.Value.Y = 1;
                kf.Value.Z = 1;


            }
            RefreshAnimatedVertexAndNodePositionsForRendering();
            SetSaved(false);
        }
        private void Animator_ResetTranslations_Bone(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            int track = Animator_GetTrack();
            if (track != -1)
            {
                var key = node.Translation.NodeList.First(x => x.Time == track);
                key.Value.X = 0;
                key.Value.Y = 0;
                key.Value.Z = 0;
                RefreshAnimatedVertexAndNodePositionsForRendering();
                SetSaved(false);
            }
        }
        private void Animator_ResetRorations_Bone(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            int track = Animator_GetTrack();
            if (track != -1)
            {
                var key = node.Rotation.NodeList.First(x => x.Time == track);
                key.Value.X = 0;
                key.Value.Y = 0;
                key.Value.Z = 0;
                key.Value.W = 1;
                SetSaved(false);
                RefreshAnimatedVertexAndNodePositionsForRendering();
            }
        }
        private void Animator_ResetScalings_Bone(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem == null) return;
            INode node = GetSeletedNodeInAnimator();
            int track = Animator_GetTrack();
            if (track != -1)
            {
                var key = node.Scaling.NodeList.First(x => x.Time == track);
                key.Value.X = 1;
                key.Value.Y = 1;
                key.Value.Z = 1;
                SetSaved(false);
                RefreshAnimatedVertexAndNodePositionsForRendering();
            }
        }

        private void ToggleGround(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderGroundPlane = Menuitem_ground.IsChecked == true;
            SaveSettings();
        }


        private void createCols2ForTargetGeo(object? sender, RoutedEventArgs? e)
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
                    cs.Name = "GeneratedCollisionShape_" + IDCounter.Next_;
                    cs.Type = ECollisionShapeType.Sphere;
                    cs.Radius = distance;
                    cs.PivotPoint = centroid;
                    CurrentModel.Nodes.Add(cs);
                    RefreshNodesTree();

                }
            }
            //--------------------------------------------------
            //--------------------------------------------------
            //--------------------------------------------------
        }
        private void closemodel(object? sender, RoutedEventArgs? e)
        {
            CurrentModel = new CModel();
            CurrentSaveFolder = "";
            CurrentSaveLocation = "";
            refreshalllists(null, null);
            SetNewModelName();
            RefreshTitle();
            PathManager.Selected = null;
            PathManager.Paths.Clear();
            ListSequences_Paths.Items.Clear();
            ListPaths.Items.Clear();
            ListPathNodes.Items.Clear();
            ListSequences_Paths.Items.Clear();
            ListModelNodes_Paths.Items.Clear();
            SetSaved(false);
        }
        private void saveasCopy(object? sender, RoutedEventArgs? e)
        {
            if (CurrentSaveLocation
                .Length > 0 && Directory.Exists(CurrentSaveFolder))
            {
                string temp = CurrentSaveLocation;
                CurrentSaveLocation = AppendTimestampToFilePath(CurrentSaveLocation);
                SaveModel(null, null);
                CurrentSaveLocation = temp;
            }
        }
        public static string AppendTimestampToFilePath(string filePath)
        {
            string? directory = System.IO.Path.GetDirectoryName(filePath);
            string? fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string? extension = System.IO.Path.GetExtension(filePath);
            // Get current date and time
            DateTime now = DateTime.Now;
            string dayWithSuffix = AddDaySuffix(now.Day);
            string timestamp = $"{dayWithSuffix} {now:MMMM yyyy HH-mm-ss}";
            string newFileName = $"{fileNameWithoutExt} {timestamp}{extension}";
            return System.IO.Path.Combine(directory ?? "", newFileName);
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

        private void newtexturefrominput(object? sender, RoutedEventArgs? e)
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
                    RefreshTexturesList();
                    RefreshLayersTextureList();
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

        private TransformationType Animator_CopiedNodeData;
        private void Animator_CopyNodeTranslation(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                Animator_CopiedNode = GetSeletedNodeInAnimator();
                Animator_CopiedNodeData = TransformationType.Translation;
                SetSaved(false);
            }
        }
        private void Animator_CopyNodeRotation(object? sender, RoutedEventArgs? e)
        {
            Animator_CopiedNode = GetSeletedNodeInAnimator();
            Animator_CopiedNodeData = TransformationType.Rotation;
            SetSaved(false);
        }
        private void Animator_CopyNodeScaling(object? sender, RoutedEventArgs? e)
        {
            Animator_CopiedNode = GetSeletedNodeInAnimator();
            Animator_CopiedNodeData = TransformationType.Scaling;
            SetSaved(false);
        }
        private void Animator_PasteNodeMerge(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                var pasteOn = GetSeletedNodeInAnimator();
                if (Animator_CopiedNode == null) { MessageBox.Show("Nothing to paste"); return; }
                if (pasteOn == Animator_CopiedNode) { MessageBox.Show("Copied and pasted on cannot be the same"); return; }
                if (Animator_CopiedNodeData == TransformationType.Translation)
                {
                    foreach (var kf in Animator_CopiedNode.Translation)
                    {
                        if (pasteOn.Translation.Any(x => x.Time == kf.Time) == false)
                        {
                            CAnimatorNode<CVector3> c = new CAnimatorNode<CVector3>();
                            c.Value = kf.Value;
                            c.InTangent = kf.InTangent;
                            c.OutTangent = kf.OutTangent;
                            c.Time = kf.Time;
                            pasteOn.Translation.Add(c);
                        }
                    }
                }
                if (Animator_CopiedNodeData == TransformationType.Rotation)
                {
                    foreach (var kf in Animator_CopiedNode.Rotation)
                    {
                        if (pasteOn.Translation.Any(x => x.Time == kf.Time) == false)
                        {
                            CAnimatorNode<CVector4> c = new CAnimatorNode<CVector4>();
                            c.Value = kf.Value;
                            c.InTangent = kf.InTangent;
                            c.OutTangent = kf.OutTangent;
                            c.Time = kf.Time;
                            pasteOn.Rotation.Add(c);
                        }
                    }
                }
                if (Animator_CopiedNodeData == TransformationType.Scaling)
                {
                    foreach (var kf in Animator_CopiedNode.Scaling)
                    {
                        if (pasteOn.Scaling.Any(x => x.Time == kf.Time) == false)
                        {
                            CAnimatorNode<CVector3> c = new CAnimatorNode<CVector3>();
                            c.Value = kf.Value;
                            c.InTangent = kf.InTangent;
                            c.OutTangent = kf.OutTangent;
                            c.Time = kf.Time;
                            pasteOn.Scaling.Add(c);
                        }
                    }
                }
                pasteOn.Translation.NodeList = pasteOn.Translation.NodeList.OrderBy(x => x.Time).ToList();
                pasteOn.Rotation.NodeList = pasteOn.Rotation.NodeList.OrderBy(x => x.Time).ToList();
                pasteOn.Scaling.NodeList = pasteOn.Scaling.NodeList.OrderBy(x => x.Time).ToList();
            }
            SetSaved(false);
            RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private void Animator_PasteNodeOverwrite(object? sender, RoutedEventArgs? e)
        {
            if (Animator_CopiedNode == null) return;
            if (List_Nodes_Animator.SelectedItem != null)
            {
                var pasteOn = GetSeletedNodeInAnimator();
                if (pasteOn == Animator_CopiedNode) { MessageBox.Show("Copied and pasted on cannot be the same"); return; }
                if (Animator_CopiedNodeData == TransformationType.Translation)
                {
                    pasteOn.Translation.Clear();
                    foreach (var kf in Animator_CopiedNode.Translation)
                    {
                        CAnimatorNode<CVector3> c = new CAnimatorNode<CVector3>();
                        c.Value = kf.Value;
                        c.InTangent = kf.InTangent;
                        c.OutTangent = kf.OutTangent;
                        c.Time = kf.Time;
                        pasteOn.Translation.Add(c);
                    }
                }
                if (Animator_CopiedNodeData == TransformationType.Rotation)
                {
                    pasteOn.Rotation.Clear();
                    foreach (var kf in Animator_CopiedNode.Rotation)
                    {
                        CAnimatorNode<CVector4> c = new CAnimatorNode<CVector4>();
                        c.Value = kf.Value;
                        c.InTangent = kf.InTangent;
                        c.OutTangent = kf.OutTangent;
                        c.Time = kf.Time;
                        pasteOn.Rotation.Add(c);
                    }
                }
                if (Animator_CopiedNodeData == TransformationType.Scaling)
                {
                    pasteOn.Scaling.Clear();
                    foreach (var kf in Animator_CopiedNode.Scaling)
                    {
                        CAnimatorNode<CVector3> c = new CAnimatorNode<CVector3>();
                        c.Value = kf.Value;
                        c.InTangent = kf.InTangent;
                        c.OutTangent = kf.OutTangent;
                        c.Time = kf.Time;
                        pasteOn.Scaling.Add(c);
                    }
                }
                pasteOn.Translation.NodeList = pasteOn.Translation.NodeList.OrderBy(x => x.Time).ToList();
                pasteOn.Rotation.NodeList = pasteOn.Rotation.NodeList.OrderBy(x => x.Time).ToList();
                pasteOn.Scaling.NodeList = pasteOn.Scaling.NodeList.OrderBy(x => x.Time).ToList();
            }
            SetSaved(false);
            RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private void SplitMatrixGroups(object? sender, RoutedEventArgs? e)
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
                // add the groups
                geoset.Groups.Clear();
                foreach (var group in groups) geoset.Groups.Add(group);
            }
            ButtonSplitGroups.IsEnabled = false;
            ButtonAddAttach.IsEnabled = true;
            ButtonClearAttach.IsEnabled = true;
            ButtonDetach.IsEnabled = true;
            SetSaved(false);
        }
        private CGeosetGroup CloneGroup(CGeosetGroup original)
        {
            CGeosetGroup group = new CGeosetGroup(CurrentModel);
            foreach (var gnode in original.Nodes)
            {
                CGeosetGroupNode node = new CGeosetGroupNode(CurrentModel);
                group.Nodes.Add(node);
                node.Node.Attach(gnode.Node.Node);

            }
            return group;
        }
        private void ToggleSkinning(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderSkinning = Menuitem_skinning.IsChecked == true;
            SaveSettings();
        }
        private void SaveSettings()
        {
            if (Pause) return;
            string path = System.IO.Path.Combine(AppPath, "Settings.txt");
            StringBuilder settings = new StringBuilder();
            settings.AppendLine($"{nameof(CanDrag)}={CanDrag}");
            settings.AppendLine($"{nameof(RenderSettings.RenderLighing)}={RenderSettings.RenderLighing}");
            settings.AppendLine($"{nameof(RenderSettings.RenderSkinning)}={RenderSettings.RenderSkinning}");
            settings.AppendLine($"{nameof(RenderSettings.RenderEnabled)}={RenderSettings.RenderEnabled}");
            settings.AppendLine($"{nameof(RenderSettings.RenderTextures)}={RenderSettings.RenderTextures}");
            settings.AppendLine($"{nameof(RenderSettings.RenderShading)}={RenderSettings.RenderShading}");
            settings.AppendLine($"{nameof(RenderSettings.RenderCollisionShapes)}={RenderSettings.RenderCollisionShapes}");
            settings.AppendLine($"{nameof(RenderSettings.RenderGroundPlane)}={RenderSettings.RenderGroundPlane}");
            settings.AppendLine($"{nameof(RenderSettings.RenderGeosetExtents)}={RenderSettings.RenderGeosetExtents}");
            settings.AppendLine($"{nameof(RenderSettings.RenderGeosetExtentSphere)}={RenderSettings.RenderGeosetExtentSphere}");
            settings.AppendLine($"{nameof(RenderSettings.RenderNodes)}={RenderSettings.RenderNodes}");
            settings.AppendLine($"{nameof(OptimizeOnSave)}={OptimizeOnSave}");
            settings.AppendLine($"{nameof(RenderSettings.RenderGeometry)}={RenderSettings.RenderGeometry}");
            settings.AppendLine($"{nameof(RenderSettings.RenderGroundGrid)}={RenderSettings.RenderGroundGrid}");
            settings.AppendLine($"{nameof(RenderSettings.RenderGridX)}={RenderSettings.RenderGridX}");

            settings.AppendLine($"{nameof(RenderSettings.RenderGridY)}={RenderSettings.RenderGridY}");
            settings.AppendLine($"{nameof(RenderSettings.RenderVertices)}={RenderSettings.RenderVertices}");
            settings.AppendLine($"{nameof(RenderSettings.RenderEdges)}={RenderSettings.RenderEdges}");
            settings.AppendLine($"{nameof(RenderSettings.RenderSkeleton)}={RenderSettings.RenderSkeleton}");
            settings.AppendLine($"{nameof(RenderSettings.RenderNormals)}={RenderSettings.RenderNormals}");
            settings.AppendLine($"{nameof(RenderSettings.LineSpacing)}={RenderSettings.LineSpacing}");
            settings.AppendLine($"{nameof(RenderSettings.RenderAxis)}={RenderSettings.RenderAxis}");
            settings.AppendLine($"{nameof(RenderSettings.ViewportGridSizeOverlay)}={RenderSettings.ViewportGridSizeOverlay}");
            settings.AppendLine($"{nameof(RenderSettings.ViewportGridSize)}={RenderSettings.ViewportGridSize}");
            settings.AppendLine($"{nameof(ZoomIncrement)}={ZoomIncrement}");
            settings.AppendLine($"{nameof(RenderSettings.RenderFPS)}={RenderSettings.RenderFPS}");
            settings.AppendLine($"{nameof(DefaultAuthor)}={DefaultAuthor}");
            settings.AppendLine($"{nameof(MaximizeOnStart)}={MaximizeOnStart}");
            settings.AppendLine($"{nameof(ColorizeTransformations)}={ColorizeTransformations}");
            settings.AppendLine($"{nameof(RayCaster.MousePickRadius)}={RayCaster.MousePickRadius}");

            // render settingsload

            settings.AppendLine($"{nameof(RenderSettings.Color_Skinning)}={Array2S(RenderSettings.Color_Skinning)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_Vertex)}={Array2S(RenderSettings.Color_Vertex)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_VertexRigged)}={Array2S(RenderSettings.Color_VertexRigged)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_VertexRiggedSelected)}={Array2S(RenderSettings.Color_VertexRiggedSelected)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_VertexSelected)}={Array2S(RenderSettings.Color_VertexSelected)}");
            settings.AppendLine($"{nameof(RenderSettings.GridColor)}={Array2S(RenderSettings.GridColor)}");
            settings.AppendLine($"{nameof(RenderSettings.BackgroundColor)}={Array2S(RenderSettings.BackgroundColor)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_CollisionShape)}={Array2S(RenderSettings.Color_CollisionShape)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_Extent)}={Array2S(RenderSettings.Color_Extent)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_Node)}={Array2S(RenderSettings.Color_Node)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_NodeSelected)}={Array2S(RenderSettings.Color_NodeSelected)}");
            settings.AppendLine($"{nameof(RenderSettings.Path_Node)}={Array2S(RenderSettings.Path_Node)}");
            settings.AppendLine($"{nameof(RenderSettings.Path_Node_Selected)}={Array2S(RenderSettings.Path_Node_Selected)}");
            settings.AppendLine($"{nameof(RenderSettings.Path_Line)}={Array2S(RenderSettings.Path_Line)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_Edge)}={Array2S(RenderSettings.Color_Edge)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_Edge_Selected)}={Array2S(RenderSettings.Color_Edge_Selected)}");
            settings.AppendLine($"{nameof(RenderSettings.Color_Normals)}={Array2S(RenderSettings.Color_Normals)}");
            settings.AppendLine($"{nameof(RenderSettings.NodeSize)}={RenderSettings.NodeSize}");
            settings.AppendLine($"{nameof(RenderSettings.PathNodeSize)}={RenderSettings.PathNodeSize}");
            settings.AppendLine($"{nameof(RenderSettings.VertexSize)}={RenderSettings.VertexSize}");


            File.WriteAllText(path, settings.ToString());
        }

        private static string Array2S(float[] f)
        {
            return string.Join(", ", f);
        }
        private void LoadSettings()
        {
            Pause = true;
            string path = System.IO.Path.Combine(AppPath, "Settings.txt");
            if (!File.Exists(path)) { Pause = false; return; }
            string[] lines = File.ReadAllLines(path);
            if (lines.Length == 0) return;
            foreach (var line in lines)
            {
                string[] parts = line.Split("=");
                if (parts[0] == nameof(RenderSettings.RenderSkinning)) RenderSettings.RenderSkinning = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderEnabled)) RenderSettings.RenderEnabled = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderTextures)) RenderSettings.RenderTextures = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderShading)) RenderSettings.RenderShading = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderCollisionShapes)) RenderSettings.RenderCollisionShapes = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderGroundPlane)) RenderSettings.RenderGroundPlane = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderGeosetExtents)) RenderSettings.RenderGeosetExtents = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderGeosetExtentSphere)) RenderSettings.RenderGeosetExtentSphere = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderNodes)) RenderSettings.RenderNodes = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderGeometry)) RenderSettings.RenderGeometry = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderGroundGrid)) RenderSettings.RenderGroundGrid = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderGridX)) RenderSettings.RenderGridX = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.ViewportGridSize)) RenderSettings.ViewportGridSize = int.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderVertices)) RenderSettings.RenderVertices = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderEdges)) RenderSettings.RenderEdges = bool.Parse(parts[1]);
                else if (parts[0] == nameof(OptimizeOnSave)) OptimizeOnSave = bool.Parse(parts[1]);

                else if (parts[0] == nameof(RenderSettings.RenderNormals)) RenderSettings.RenderNormals = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderAxis)) RenderSettings.RenderAxis = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.LineSpacing)) RenderSettings.LineSpacing = int.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.ViewportGridSizeOverlay)) RenderSettings.ViewportGridSizeOverlay = int.Parse(parts[1]);
                else if (parts[0] == nameof(ZoomIncrement)) ZoomIncrement = float.Parse(parts[1]);
                else if (parts[0] == nameof(MaximizeOnStart)) MaximizeOnStart = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderLighing)) RenderSettings.RenderLighing = bool.Parse(parts[1]);
                else if (parts[0] == nameof(DefaultAuthor)) DefaultAuthor = parts[1];
                else if (parts[0] == nameof(History.HistoryLimit))
                {

                    Textbox_HistoryLimit.Text = parts[1];
                }

                else if (parts[0] == nameof(RayCaster.MousePickRadius)) RayCaster.MousePickRadius = float.Parse(parts[1]);
                else if (parts[0] == nameof(CanDrag))

                {
                    CanDrag = bool.Parse(parts[1]);
                    Pause = true;
                    Menuitem_allowDrag.IsChecked = CanDrag;
                    Pause = false;
                }
                else if (parts[0] == nameof(ColorizeTransformations)) ColorizeTransformations = bool.Parse(parts[1]);
                else if (parts[0] == nameof(RenderSettings.RenderFPS))
                { RenderSettings.RenderFPS = bool.Parse(parts[1]); Scene_ViewportGL.DrawFPS = RenderSettings.RenderFPS; }
                // render settngs:
                else if (parts[0] == nameof(RenderSettings.Path_Line)) { RenderSettings.Path_Line = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Path_Node)) { RenderSettings.Path_Node = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Path_Node_Selected)) { RenderSettings.Path_Node_Selected = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.PathNodeSize)) { RenderSettings.PathNodeSize = float.Parse(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.VertexSize)) { RenderSettings.VertexSize = float.Parse(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.NodeSize)) { RenderSettings.NodeSize = float.Parse(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_CollisionShape)) { RenderSettings.Color_CollisionShape = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Edge)) { RenderSettings.Color_Edge = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Edge_Selected)) { RenderSettings.Color_Edge_Selected = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Vertex)) { RenderSettings.Color_Vertex = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_VertexRigged)) { RenderSettings.Color_VertexRigged = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_VertexRiggedSelected)) { RenderSettings.Color_VertexRiggedSelected = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_VertexSelected)) { RenderSettings.Color_VertexSelected = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Node)) { RenderSettings.Color_Node = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_NodeSelected)) { RenderSettings.Color_NodeSelected = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Normals)) { RenderSettings.Color_Normals = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Skeleton)) { RenderSettings.Color_Skeleton = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Skinning)) { RenderSettings.Color_Skinning = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.GridColor)) { RenderSettings.GridColor = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.Color_Extent)) { RenderSettings.Color_Extent = GetTrio(parts[1]); }
                else if (parts[0] == nameof(RenderSettings.BackgroundColor)) { RenderSettings.BackgroundColor = GetTrio(parts[1]); }

            }
            if (MaximizeOnStart) { WindowState = WindowState.Maximized; }
            CheckAllSettings();
            Pause = false;
            RefreshGrdOverlay();
        }

        private static float[] GetTrio(string v)
        {
            return v.Split(", ")
                    .Select(float.Parse)
                    .ToArray();
        }


        private void CheckAllSettings()
        {
            InputMousePickRaius.Text = RayCaster.MousePickRadius.ToString();
            Menuitem_Lightng.IsChecked = RenderSettings.RenderLighing;
            Menuitem_colorize.IsChecked = ColorizeTransformations;
            Menuitem_Maximize.IsChecked = MaximizeOnStart;
            Menuitem_Enabled.IsChecked = RenderSettings.RenderEnabled;
            Menuitem_vertices.IsChecked = RenderSettings.RenderVertices;
            Menuitem_skinning.IsChecked = RenderSettings.RenderSkinning;
            Menuitem_Textured.IsChecked = RenderSettings.RenderTextures;
            Menuitem_Shading.IsChecked = RenderSettings.RenderShading;
            Menuitem_Cols.IsChecked = RenderSettings.RenderCollisionShapes;
            Menuitem_GAE.IsChecked = RenderSettings.RenderGeosetExtents;

            Menuitem_Nodes.IsChecked = RenderSettings.RenderNodes;
            Menuitem_ground.IsChecked = RenderSettings.RenderGroundPlane;
            Menuitem_groundGrid.IsChecked = RenderSettings.RenderGroundGrid;
            Menuitem_groundGridX.IsChecked = RenderSettings.RenderGridX;
            Menuitem_groundGridY.IsChecked = RenderSettings.RenderGridY;
            ButtonOptimizeOnSave.IsChecked = OptimizeOnSave;
            InputZoomIncrement.Text = ZoomIncrement.ToString(); ;
            Menuitem_Geometry.IsChecked = RenderSettings.RenderGeometry;
            Menuitem_edges.IsChecked = RenderSettings.RenderEdges;

            Menuitem_axis.IsChecked = RenderSettings.RenderAxis;
            ViewPortLineSpacing.Text = RenderSettings.LineSpacing.ToString();
            ViewPortGrid.Text = RenderSettings.ViewportGridSize.ToString();
            InputGridOverlay.Text = RenderSettings.ViewportGridSizeOverlay.ToString();
            Menuitem_normals.IsChecked = RenderSettings.RenderNormals;
            Menuitem_fps.IsChecked = RenderSettings.RenderFPS;
            CameraControl.ZoomIncrement = ZoomIncrement;
            RefreshGrdOverlay();
        }
        private void RemoveTrackFromAllNodes(int track, TransformationType t)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                if (t == TransformationType.Translation) node.Translation.NodeList.RemoveAll(x => x.Time == track);
                if (t == TransformationType.Rotation) node.Rotation.NodeList.RemoveAll(x => x.Time == track);
                if (t == TransformationType.Scaling) node.Scaling.NodeList.RemoveAll(x => x.Time == track);
            }
        }
        private void CopyKeyframeT(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt(List_Keyframes_Animator);
                RemoveTrackFromAllNodes(track, TransformationType.Translation);
                RefreshAnimatedVertexAndNodePositionsForRendering();
            }
        }
        private void CopyKeyframeR(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt(List_Keyframes_Animator);
                RemoveTrackFromAllNodes(track, TransformationType.Rotation);
                RefreshAnimatedVertexAndNodePositionsForRendering();
            }
        }
        private void CopyKeyframeS(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int track = Extractor.GetInt(List_Keyframes_Animator);
                RemoveTrackFromAllNodes(track, TransformationType.Scaling);
                RefreshAnimatedVertexAndNodePositionsForRendering();
            }
        }
        private List<CBone> ListBones_Rigging = new List<CBone>();

        private void SelectedBoneInRigging(object? sender, SelectionChangedEventArgs? e)
        {
            if (ListBonesRiggings.SelectedItem == null) return;
            CBone? bone = getselectedBoneInRigging(); if (bone == null) return;
            foreach (var node in ListBones_Rigging)
            {
                node.IsSelected = false;
            }
            bone.IsSelected = true;
        }
        private CBone? getselectedBoneInRigging()
        {
            string? name = Extractor.GetString(ListBonesRiggings);
            return CurrentModel.Nodes.FirstOrDefault(x => x.Name == name) as CBone;
        }
        private void SEtCeilings(object? sender, RoutedEventArgs? e)
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
                        History.Sequences.Add(CurrentModel);
                    }
                    else
                    {
                        MessageBox.Show("Expected integer or float", "Invalid input");
                    }
                }
            }
            else
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
            }
        }
        private void CalculateModelLines()
        {
            CurrentModel.CalculateCollisionShapeEdges();
            CurrentModel.CalculateGeosetBoundingBoxes();
        }
        private void scalegeosetsTogether(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            InputVector iv = new InputVector(AllowedValue.Positive, new CVector3(100, 100, 100), "Percentage");
            iv.ShowDialog();
            if (iv.DialogResult == true)
            {
                float x = iv.X, y = iv.Y, z = iv.Z;
                List<CGeoset> geosets = GetSelectedGeosets();
                List<CGeosetVertex> vertices = GetVerticesOfGeosets(geosets);
                foreach (var vertex in vertices)
                {
                    vertex.Position = new CVector3(
                        vertex.Position.X * (x / 100f),
                        vertex.Position.Y * (y / 100f),
                        vertex.Position.Z * (z / 100f)
                    );
                }

            }
        }
        private static List<CGeosetVertex> GetVerticesOfGeosets(List<CGeoset> geosets)
        {
            List<CGeosetVertex> vertices = new List<CGeosetVertex>();
            foreach (var geoset in geosets)
            {
                vertices.AddRange(geoset.Vertices);
            }
            return vertices;
        }
        private void rotateeachCP(object? sender, RoutedEventArgs? e)
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

            }
        }
        private static bool RotationInRange(float x, float y, float z)
        {
            if (x < -360) return false;
            if (x > 360) return false;
            if (y < -360) return false;
            if (y > 360) return false;
            if (z < -360) return false;
            if (z > 360) return false;
            return true;
        }
        private void rotateallCP(object? sender, RoutedEventArgs? e)
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
        private void SelectionChanged_Geosets(object? sender, SelectionChangedEventArgs? e)
        {
            if (e == null) return;
            if (e.OriginalSource is TabControl tc && tc == Tabs_Geosets)
            {
                InAnimator = Tabs_Geosets.SelectedIndex == 4 && Tab_Animator.SelectedIndex == 1;
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    Menuitem_Geometry.IsChecked = true;
                    Menuitem_Cols.IsChecked = false;
                    Menuitem_skinning.IsChecked = false;
                    RefreshGeosetsList();

                }
                if (Tabs_Geosets.SelectedIndex == 1) // triangles
                {
                    Menuitem_Geometry.IsChecked = true;
                    Menuitem_Cols.IsChecked = false;
                    Menuitem_skinning.IsChecked = false;
                    RefreshGeosets_Triangles();
                }
                if (Tabs_Geosets.SelectedIndex == 2) // vertices
                {
                    Menuitem_vertices.IsChecked = true;
                    Menuitem_Cols.IsChecked = false;
                    Menuitem_skinning.IsChecked = false;
                    RefreshGeosets_Vertices();
                }
                if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    Menuitem_vertices.IsChecked = true;
                    Menuitem_Nodes.IsChecked = true;
                    Menuitem_Cols.IsChecked = false;
                    Menuitem_skinning.IsChecked = false;
                }
                if (Tabs_Geosets.SelectedIndex == 3) // rigging
                {
                    Menuitem_vertices.IsChecked = true;
                    Menuitem_Nodes.IsChecked = true;
                    Menuitem_skinning.IsChecked = true;

                    RefreshGeosetsListRigging();
                    RefreshBonesInRigging();
                    ListAttachedToRiggings.Items.Clear();
                    // clear selection in rigging
                    foreach (var bone in ListBones_Rigging) bone.IsSelected = false;
                    bool skinningOK = CheckSkinning();
                    ButtonSplitGroups.IsEnabled = !skinningOK;
                    ButtonAddAttach.IsEnabled = skinningOK;
                    ButtonClearAttach.IsEnabled = skinningOK;
                    ButtonDetach.IsEnabled = skinningOK;
                    if (!skinningOK) MessageBox.Show("For easier working with bone-vertex relationships, each vertex must have its own matrix group. You can merge similars later. Click on 'Split groups' before using the rigging editor.");
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    Menuitem_Nodes.IsChecked = true;
                    if (Tab_Animator.SelectedIndex == 0)
                    {
                        if (CurrentModel.Nodes.Count == 0)
                        { MessageBox.Show("There are no nodes", "Nothing to animate"); return; }
                        if (CurrentModel.Geosets.Count == 0)
                        {
                            MessageBox.Show("There are no geosets", "Nothing to animate"); return;
                        }
                        if (CurrentModel.Sequences.Count == 0)
                        {
                            MessageBox.Show("There are no sequences", "Nothing to animate"); return;
                        }
                        bool b = int.TryParse(InputCurrentFrame.Text, out int track);
                        if (b)
                        {
                            if (TrackExistsInSEquences(track, null))
                            {
                                RefreshAnimatedVertexAndNodePositionsForRendering(track);
                            }
                        }
                        RefreshAnimatorData(); // no need for rfresh of lists
                    }
                }
                else if (Tabs_Geosets.SelectedIndex == 5) // cs
                {
                    RefreshCollisionShapeList();
                    Menuitem_Cols.IsChecked = true;
                }
                else if (Tabs_Geosets.SelectedIndex == 6)// nodes
                {
                    RefreshNodeEditorList(); Menuitem_Nodes.IsChecked = true;
                }
                e.Handled = true;

            }
        }
        internal void RefreshNodeEditorList()
        {
            list_Node_Editor.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                string type = node.GetType().Name;
                list_Node_Editor.Items.Add(new ListBoxItem() { Content = $"{node.Name} ({type})" });
            }
        }
        private void RefreshCollisionShapeList()
        {
            list_cs.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CCollisionShape cs)
                {
                    list_cs.Items.Add(new ListBoxItem() { Content = cs.Name });
                }
            }
        }
        private CCollisionShape? CurrentlySelectedCollisionShape;
        private float CurentCSModifyAmount = 1;
        private CCollisionShape? GetSelectedCollisionShape()
        {
            ListBoxItem? item = list_cs.SelectedItem as ListBoxItem; if (item == null) return null;
            string? name = item.Content.ToString();
            return CurrentModel.Nodes.First(x => x.Name == name) as CCollisionShape;
        }
        private void SetWorkModeMove(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.Translate;
            ButtonManual_Move.BorderBrush = Brushes.Green;
            ButtonManual_Scale.BorderBrush = Brushes.Gray;
            ButtonManual_Rotate.BorderBrush = Brushes.Gray;
            ButtonManual_More.BorderBrush = Brushes.Gray;

            UpdaManualTranslationInput();


        }

        private void UpdaManualTranslationInput()
        {
            if (Tabs_Geosets.SelectedIndex == 0) // gesooets
            {
                var geosets = GetSelectedGeosets();
                if (geosets.Count > 0)
                {
                    var centroid = Calculator.GetCentroidOfGeosets(geosets);
                    InputAnimatorX.Text = centroid.X.ToString();
                    InputAnimatorY.Text = centroid.Y.ToString();
                    InputAnimatorZ.Text = centroid.Z.ToString();

                }
            }
            else if (Tabs_Geosets.SelectedIndex == 1) //triangles
            {
                var triangles = getSelectedTriangles();
                if (triangles.Count > 0)
                {
                    var centroid = Calculator.GetCentroidofTriangles(triangles);
                    InputAnimatorX.Text = centroid.X.ToString();
                    InputAnimatorY.Text = centroid.Y.ToString();
                    InputAnimatorZ.Text = centroid.Z.ToString();
                }
            }
            else if (Tabs_Geosets.SelectedIndex == 2) //vertices
            {
                var vertices = GetSelectedVertices();
                if (vertices.Count > 0)
                {
                    var centroid = Calculator.GetCentroidOfVertices(vertices);
                    InputAnimatorX.Text = centroid.X.ToString();
                    InputAnimatorY.Text = centroid.Y.ToString();
                    InputAnimatorZ.Text = centroid.Z.ToString();
                }
            }
            else if (Tabs_Geosets.SelectedIndex == 3) //rigging
            {

            }

            else if (Tabs_Geosets.SelectedIndex == 4) //animator
            {
                UpdateManualInputForAnimator();
            }
            else if (Tabs_Geosets.SelectedIndex == 5) //cols
            {

            }
            else if (Tabs_Geosets.SelectedIndex == 6) //nodes
            {
                if (list_Node_Editor.SelectedItem == null) return;
                var node = CurrentModel.Nodes[list_Node_Editor.SelectedIndex];
                InputAnimatorX.Text = node.PivotPoint.X.ToString();
                InputAnimatorY.Text = node.PivotPoint.Y.ToString();
                InputAnimatorZ.Text = node.PivotPoint.Z.ToString();
            }
        }

        private void SetWorkModeRotate(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.Rotate;
            ButtonManual_Move.BorderBrush = Brushes.Gray;
            ButtonManual_Rotate.BorderBrush = Brushes.Green;
            ButtonManual_Scale.BorderBrush = Brushes.Gray;
            ButtonManual_More.BorderBrush = Brushes.Gray;
            InputAnimatorX.Text = "0";
            InputAnimatorY.Text = "0";
            InputAnimatorZ.Text = "0";
            UpdateManualInputForAnimator();
        }
        private void SetWorkModeScale(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.Scale;
            ButtonManual_Move.BorderBrush = Brushes.Gray;
            ButtonManual_More.BorderBrush = Brushes.Gray;
            ButtonManual_Rotate.BorderBrush = Brushes.Gray;
            ButtonManual_Scale.BorderBrush = Brushes.Green;
            InputAnimatorX.Text = "100";
            InputAnimatorY.Text = "100";
            InputAnimatorZ.Text = "100";
            UpdateManualInputForAnimator();
        }


        private void GoToTrackWithChange(int change, bool plus, bool percent = false)
        {
            bool b = int.TryParse(InputCurrentFrame.Text, out int track);
            if (!b) return;
            if (CurrentlySelectedSequenceInAnimator == null) { MessageBox.Show("Select a sequence"); return; }
            if (CurrentModel.Sequences.Contains(CurrentlySelectedSequenceInAnimator) == false) { MessageBox.Show("Select a sequence"); return; }
            int sum = 0;
            if (percent)
            {
                sum = plus ? track + (track * (change / 100)) : track - (track * (change / 100));
            }
            else
            {
                sum = plus ? track + change : track - change;
            }
            if (TrackExistsInSEquences(sum, CurrentlySelectedSequenceInAnimator))
            {
                InputCurrentFrame.Text = sum.ToString();
                gototrack(null, null);
            }
            else
            {
                MessageBox.Show("This track does not exist in the selected sequence");
            }
        }
        private void MenuItemMinus1_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(1, false);
        }
        private void MenuItemPlus1_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(1, true);
        }
        private void MenuItemMinus5_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(5, false);
        }
        private void MenuItemPlus5_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(5, true);
        }
        private void MenuItemMinus10_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(10, false);
        }
        private void MenuItemPlus10_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(10, true);
        }
        private void MenuItemMinus50_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(50, false);
        }
        private void MenuItemPlus50_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(50, true);
        }
        private void MenuItemMinus100_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(1000, false);
        }
        private void MenuItemPlus100_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(1000, true);
        }
        private void MenuItemMinus1Percent_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(1, false, true);
        }
        private void MenuItemPlus1Percent_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(1, true, true);
        }
        private void MenuItemMinus5Percent_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(5, false, true);
        }
        private void MenuItemPlus5Percent_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(5, true, true);
        }
        private void MenuItemMinus10Percent_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(10, false, true);
        }
        private void MenuItemPlus10Percent_Click(object? sender, RoutedEventArgs? e)
        {
            GoToTrackWithChange(10, true, true);
        }
        private void MenuItemZeroPercent_Click(object? sender, RoutedEventArgs? e)
        {
            if (CurrentlySelectedSequenceInAnimator == null) { MessageBox.Show("Select a sequence"); return; }
            InputCurrentFrame.Text = CurrentlySelectedSequenceInAnimator.IntervalStart.ToString();
            gototrack(null, null);
        }
        private void MenuItemHundredPercent_Click(object? sender, RoutedEventArgs? e)
        {
            if (CurrentlySelectedSequenceInAnimator == null) { MessageBox.Show("Select a sequence"); return; }
            InputCurrentFrame.Text = CurrentlySelectedSequenceInAnimator.IntervalEnd.ToString();
            gototrack(null, null);
        }
        private void ExportGeoset(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            string savePath = FileSeeker.SaveTGeoFileDialog();
            if (savePath.Length > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                string data = GeosetExporter.Write(geosets[0]);
                File.WriteAllText(savePath, data);
            }
        }
        private void ImportGeoset(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no materials. At least one material is needed to be applied to an imported geoset."); return;
            }
            string openPath = FileSeeker.OpenTGeoFileDialog();
            if (openPath.Length > 0)
            {
                CGeoset imported = GeosetExporter.Read(openPath, CurrentModel);
                ImportGeosetDialog finalize = new ImportGeosetDialog(CurrentModel);
                finalize.ShowDialog();
                if (finalize.DialogResult == true)
                {
                    Pause = true;
                    CGeosetGroup group = new CGeosetGroup(CurrentModel);
                    CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                    gnode.Node.Attach(finalize.SelectedNode);
                    group.Nodes.Add(gnode);
                    imported.Groups.Add(group);
                    imported.Material.Attach(finalize.SelectedMaterial);
                    CurrentModel.Geosets.Add(imported);
                    RefreshGeosetsList();

                    RefreshRenderData(null, null);
                    Pause = false;
                    SetSaved(false);
                }
            }
        }
        private void SetPriorityPlane(object? sender, TextChangedEventArgs e)
        {
            bool parsed = int.TryParse(PRiortyPlaneInput.Text, out int value);
            if (parsed && List_MAterials.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                mat.PriorityPlane = value;
                SetSaved(false);
            }
        }
        private void ReattachNodeGeometryToAnotherBone(object? sender, RoutedEventArgs? e)
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
                                CGeosetGroupNode? groupNode = null;
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
        private void SetSequenceLoop(object? sender, MouseButtonEventArgs? e)
        {
            if (ListSequenes.SelectedItem == null) { return; }
            CSequence sequence = GetSelectedSequence();
            sequence.NonLooping = !sequence.NonLooping;
            Pause = true;
            CheckLooping.IsChecked = sequence.NonLooping;
            Pause = false;
            RefreshSequencesList();
            History.Sequences.Add(CurrentModel);
        }
        private void ToggleGAE(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderGeosetExtents = Menuitem_GAE.IsChecked == true;
            SaveSettings();
        }

        private void ToggleNodes(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderNodes = Menuitem_Nodes.IsChecked == true;
            SaveSettings();
        }
        private void ToggleGeometry(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderGeometry = Menuitem_Geometry.IsChecked == true;
            SaveSettings();
        }
        private CVector3 CopiedPivotPoint = new CVector3();


        private void CopyPivot(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                CopiedPivotPoint = new CVector3(selected.PivotPoint);
            }
        }
        private void PastePivot(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint = new CVector3(CopiedPivotPoint);
            }
        }
        private void SetSamePPAsParent(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint = new CVector3(selected.Parent.Node.PivotPoint);
            }
        }
        private static CVector3 GetPolarOffsetPoint(CVector3 Point, float Distance, Axes axes, float angle)
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
        private void CenterAtItsAttachedVertices(object? sender, RoutedEventArgs? e)
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
        private void REsetNode(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint = new CVector3();
                SetSaved(false);
            }
        }
        private void NegatePPX(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.X = -selected.PivotPoint.X;
                SetSaved(false);
            }
        }
        private void NegatePPY(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.Y = -selected.PivotPoint.Y;
                SetSaved(false);
            }
        }
        private void NegatePPZ(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.Z = -selected.PivotPoint.Z;
                SetSaved(false);
            }
        }
        private void NegatePPA(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode selected = GetSeletedNode();
                selected.PivotPoint.X = -selected.PivotPoint.X;
                selected.PivotPoint.Y = -selected.PivotPoint.Y;
                selected.PivotPoint.Z = -selected.PivotPoint.Z;
                SetSaved(false);
            }
        }
        private void SetPolarOffsetPP(object? sender, RoutedEventArgs? e)
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
                SetSaved(false);
            }
        }
        private void ResetAllnodepp(object? sender, RoutedEventArgs? e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint = new CVector3();
        }
        private void negateallnodes_x(object? sender, RoutedEventArgs? e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint.X = -node.PivotPoint.X;
        }
        private void negateallnodes_y(object? sender, RoutedEventArgs? e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint.Y = -node.PivotPoint.Y;
        }
        private void negateallnodes_z(object? sender, RoutedEventArgs? e)
        {
            foreach (var node in CurrentModel.Nodes) node.PivotPoint.Z = -node.PivotPoint.Z;
        }
        private void negateallnodes_all(object? sender, RoutedEventArgs? e)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                node.PivotPoint.X = -node.PivotPoint.X;
                node.PivotPoint.Y = -node.PivotPoint.Y;
                node.PivotPoint.Z = -node.PivotPoint.Z;
            }
            SetSaved(false);
        }
        private void alignallnodes_x(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Nodes.Count > 1)
            {
                var first = CurrentModel.Nodes[0];
                for (int i = 1; i < CurrentModel.Nodes.Count; i++)
                {
                    CurrentModel.Nodes[i].PivotPoint.X = first.PivotPoint.X;
                }
            }
            SetSaved(false);
        }
        private void alignallnodes_y(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Nodes.Count > 1)
            {
                var first = CurrentModel.Nodes[0];
                for (int i = 1; i < CurrentModel.Nodes.Count; i++)
                {
                    CurrentModel.Nodes[i].PivotPoint.X = first.PivotPoint.Y;
                }
            }
            SetSaved(false);
        }
        private void alignallnodes_z(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Nodes.Count > 1)
            {
                var first = CurrentModel.Nodes[0];
                for (int i = 1; i < CurrentModel.Nodes.Count; i++)
                {
                    CurrentModel.Nodes[i].PivotPoint.X = first.PivotPoint.Z;
                }
            }
            SetSaved(false);
        }
        private void SetDistanceBetween2Geosets(object? sender, RoutedEventArgs? e)
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
            SetSaved(false);
        }
        private void setGeosetUnselectable(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                if (geosets.Count == 0) return;
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
                SetSaved(false);
            }
        }
        private void setGeosetSelection(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count != 1) { MessageBox.Show("Select a single geoset"); return; }
            var geoet = GetSelectedGeosets()[0];
            Geoset_Selection s = new Geoset_Selection(geoet);
            s.ShowDialog();
        }
        private void rootchildren(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                RootChildrenOf(node);
                RefreshNodesTree();
                SetSaved(false);
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
            RefreshNodesTree();

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
            RefreshNodesTree();

        }
        private void rootchildreninfinite(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode node = GetSeletedNode();
                RootChildrenOf_All(node);
                RefreshNodesTree();
                SetSaved(false);
            }
        }
        private void setallnodespoint(object? sender, RoutedEventArgs? e)
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
        private void setGeosetExtent(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                Edit_Extent ee = new Edit_Extent(geosets[0].Extent);
                ee.ShowDialog();
                SetSaved(false);
            }
        }
        private void setGeosetExtents(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                if (geosets[0].Extents.Count == 0) { MessageBox.Show("The target geoset has no geoset extents"); return; }
                GeosetExtents gx = new GeosetExtents(
                    geosets[0], CurrentModel.Sequences.Select(x => x).ToList(), CurrentModel);
                gx.ShowDialog();
                SetSaved(false);
            }
            else
            {
                MessageBox.Show("Select a single geoset");
            }
        }
        private void setSEquenceExtent(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                Edit_Extent ee = new Edit_Extent(sequence.Extent);
                ee.ShowDialog();
                if (ee.DialogResult == true) History.Sequences.Add(CurrentModel);
                SetSaved(false);

            }
        }
        private void setRarity(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                Input i = new Input(sequence.Rarity.ToString());
                if (i.ShowDialog() == true)
                {
                    bool parsed = float.TryParse(i.Result, out float value);
                    sequence.Rarity = value; SetSaved(false);
                }
            }
        }
        private void setMovespeed(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                Input i = new Input(sequence.MoveSpeed.ToString());
                if (i.ShowDialog() == true)
                {
                    bool parsed = float.TryParse(i.Result, out float value);
                    sequence.MoveSpeed = value; SetSaved(false);
                }
            }
        }
        private void S62(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderEnabled = Menuitem_Enabled.IsChecked == true;

            SaveSettings(); SetSaved(false);
        }
        private void leavesequenceatframe(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                List<int> Keyframes = GetKeyframesOfSequence(sequence);
                if (Keyframes.Count >= 2)
                {
                    Selector s = new Selector(Keyframes.Select(x => x.ToString()).ToList());
                    if (s.ShowDialog() == true)
                    {
                        int selected = 0;
                        if (s.Selected != null)
                        {
                            selected = int.Parse(s.Selected);
                        }

                        LeaveSequenceAtFrame_Finalize(sequence, selected); SetSaved(false);
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
                    CParticleEmitter? item = node as CParticleEmitter; if (item == null) continue; ;
                    LeaveSequenceAtFrame_Single(from, to, keep, item.EmissionRate);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.LifeSpan);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.InitialVelocity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Gravity);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Longitude);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2? item = node as CParticleEmitter2; if (item == null) continue;
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
                    CRibbonEmitter? item = node as CRibbonEmitter; if (item == null) continue;
                    LeaveSequenceAtFrame_Single(from, to, keep, item.HeightAbove);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.HeightBelow);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.TextureSlot);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Alpha);
                    LeaveSequenceAtFrame_Single(from, to, keep, item.Color);
                }
                if (node is CLight)
                {
                    CLight? item = node as CLight; if (item == null) continue;
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
        private static void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<float> animator)
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
        private static void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<CVector4> animator)
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
        private static void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<CVector3> animator)
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
        private static void LeaveSequenceAtFrame_Single(int from, int to, int keep, CAnimator<int> animator)
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
        private static void GetKeyframes(List<int> list_, CAnimator<int>? animator) { if (animator == null) { return; } foreach (var frame in animator) { if (!list_.Contains(frame.Time)) list_.Add(frame.Time); } }
        private static void GetKeyframes(List<int> list_, CAnimator<float>? animator) { if (animator == null) { return; } foreach (var frame in animator) { if (!list_.Contains(frame.Time)) list_.Add(frame.Time); } }
        private static void GetKeyframes(List<int> list_, CAnimator<CVector3>? animator) { if (animator == null) { return; } foreach (var frame in animator) { if (!list_.Contains(frame.Time)) list_.Add(frame.Time); } }
        private static void GetKeyframes(List<int> list_, CAnimator<CVector4>? animator) { if (animator == null) { return; } foreach (var frame in animator) { if (!list_.Contains(frame.Time)) list_.Add(frame.Time); } }
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
                    CParticleEmitter? item = node as CParticleEmitter; if (item == null) continue;
                    GetKeyframes(list, item.EmissionRate);
                    GetKeyframes(list, item.LifeSpan);
                    GetKeyframes(list, item.InitialVelocity);
                    GetKeyframes(list, item.Gravity);
                    GetKeyframes(list, item.Longitude);
                    GetKeyframes(list, item.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2? item = node as CParticleEmitter2; if (item == null) continue;
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
                    CRibbonEmitter? item = node as CRibbonEmitter; if (item == null) continue;
                    GetKeyframes(list, item.HeightAbove);
                    GetKeyframes(list, item.HeightBelow);
                    GetKeyframes(list, item.TextureSlot);
                    GetKeyframes(list, item.Alpha);
                    GetKeyframes(list, item.Color);
                }
                if (node is CLight)
                {
                    CLight? item = node as CLight; if (item == null) continue;
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
        private void deleteinbetweenkeyframes(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = GetSelectedSequence();
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;
                foreach (INode node in CurrentModel.Nodes)
                {
                    LeaveStartEnd(from, to, node.Translation);
                    LeaveStartEnd(from, to, node.Rotation);
                    LeaveStartEnd(from, to, node.Scaling);
                    if (node is CParticleEmitter)
                    {
                        CParticleEmitter? item = node as CParticleEmitter; if (item == null) return;
                        LeaveStartEnd(from, to, item.EmissionRate);
                        LeaveStartEnd(from, to, item.LifeSpan);
                        LeaveStartEnd(from, to, item.InitialVelocity);
                        LeaveStartEnd(from, to, item.Gravity);
                        LeaveStartEnd(from, to, item.Longitude);
                        LeaveStartEnd(from, to, item.Latitude);
                    }
                    if (node is CParticleEmitter2)
                    {
                        CParticleEmitter2? item = node as CParticleEmitter2; if (item == null) return;
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
                        CRibbonEmitter? item = node as CRibbonEmitter; if (item == null) return;
                        LeaveStartEnd(from, to, item.HeightAbove);
                        LeaveStartEnd(from, to, item.HeightBelow);
                        LeaveStartEnd(from, to, item.TextureSlot);
                        LeaveStartEnd(from, to, item.Alpha);
                        LeaveStartEnd(from, to, item.Color);
                    }
                    if (node is CLight)
                    {
                        CLight? item = node as CLight; if (item == null) return;
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
        private static void LeaveStartEnd(int from, int to, CAnimator<CVector3> animator) { animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to); }
        private static void LeaveStartEnd(int from, int to, CAnimator<CVector4> animator) { animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to); }
        private static void LeaveStartEnd(int from, int to, CAnimator<float> animator) { animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to); }
        private static void LeaveStartEnd(int from, int to, CAnimator<int> animator) { animator.NodeList.RemoveAll(x => x.Time > from && x.Time < to); }
        private void CentergeosetAtNode(object? sender, RoutedEventArgs? e)
        {
            List<CGeoset> gesoets = GetSelectedGeosets();
            if (gesoets.Count > 0)
            {
                List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
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
        private void spreadv(object? sender, RoutedEventArgs? e)
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
            var lists = Calculator.FindOverlappingVertexGroups(geoset, Threshold);
            // then for each group, spread them from their collective centroid, based on the given distance
            foreach (var list in lists)
            {
                SpreadVertexGroup(list, SetDistance);
            }
            SetSaved(false);
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
                float magnitude = Calculator.GetVectorMagnitude(direction);
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
            SetSaved(false);
        }
        private void editVisallseq(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count == 0) { MessageBox.Show("There are no geosets"); return; }
            if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; }
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
        private void createplane_t(object? sender, RoutedEventArgs? e)
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
                    CBone bone = new CBone(CurrentModel);
                    bone.Name = $"GeneratedTexturePlaneBone_{IDCounter.Next_}";
                    CurrentModel.Nodes.Add(bone);
                    CGeosetGroup group = new CGeosetGroup(CurrentModel);
                    CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                    gnode.Node.Attach(bone);
                    group.Nodes.Add(gnode);
                    plane.Groups.Add(group);
                    foreach (var vertex in plane.Vertices) vertex.Group.Attach(group);
                    CurrentModel.Materials.Add(material);
                    CurrentModel.Geosets.Add(plane);
                    RefreshGeosetsList();
                    RefreshMaterialsList();
                    RefreshNodesTree();
                    Pause = true;
                    RefreshRenderData(null, null);
                    Pause = false;
                    //RefreshViewPort();
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
        private static void SetCameraView(CameraView view)
        {
            switch (view)
            {
                case CameraView.Right:
                    CameraControl.eyeX = 0;
                    CameraControl.eyeY = 180;
                    CameraControl.eyeZ = 60;
                    CameraControl.UpX = 0;
                    CameraControl.UpY = 0;
                    CameraControl.UpZ = 90;
                    break;
                case CameraView.Left:
                    CameraControl.eyeX = 0;
                    CameraControl.eyeY = -180;
                    CameraControl.eyeZ = 60;
                    CameraControl.UpX = 0;
                    CameraControl.UpY = 0;
                    CameraControl.UpZ = 90;
                    break;
                case CameraView.Top:
                    CameraControl.eyeX = 10;
                    CameraControl.eyeZ = 180;
                    CameraControl.eyeY = 0;
                    CameraControl.UpX = 0;
                    CameraControl.UpY = 0;
                    CameraControl.UpZ = 90;
                    break;
                case CameraView.Bottom:
                    CameraControl.eyeX = 10;
                    CameraControl.eyeY = 0;
                    CameraControl.eyeZ = -180;
                    CameraControl.UpX = 0;
                    CameraControl.UpY = 0;
                    CameraControl.UpZ = 90;
                    break;
                case CameraView.Front:
                    CameraControl.Reset();
                    break;
                case CameraView.Back:
                    CameraControl.eyeX = -180;
                    CameraControl.eyeY = 0;
                    CameraControl.eyeZ = 60;
                    CameraControl.UpX = 0;
                    CameraControl.UpY = 0;
                    CameraControl.UpZ = 90;
                    break;
            }
        }
        private void view_top(object? sender, RoutedEventArgs? e)
        {
            SetCameraView(CameraView.Top);
        }
        private void view_bot(object? sender, RoutedEventArgs? e)
        {
            SetCameraView(CameraView.Bottom);
        }
        private void view_left(object? sender, RoutedEventArgs? e)
        {
            SetCameraView(CameraView.Left);

        }
        private void view_right(object? sender, RoutedEventArgs? e)
        {
            SetCameraView(CameraView.Right);

        }
        private void view_back(object? sender, RoutedEventArgs? e)
        {
            SetCameraView(CameraView.Back);

        }
        private void atmtag(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && CurrentModel.Geosets.Count > 0)
            {
                var mat = CurrentModel.Materials[List_MAterials.SelectedIndex];
                foreach (var geoset in CurrentModel.Geosets)
                {
                    geoset.Material.Attach(mat);
                }
            }
        }
        private void batch_convert(object? sender, RoutedEventArgs? e)
        {
            List<string> files = FileSeeker.OpenMdlMdxFiles();
            if (files.Count > 0)
            {
                BatchConverter.Convert(files);
            }
        }
        private void importObj(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0) { MessageBox.Show("There are no materials"); return; }
            ObjImporter ip = new ObjImporter(CurrentModel);
            Pause = true;
            ip.ShowDialog();
            RefreshGeosetsList();
            RefreshNodesTree();
            RefreshRenderData(null, null);
            Pause = false;
            RefreshSequencesList_Paths();
            RefreshPath_ModelNodes_List();
        }
        private void batch_convertImages(object? sender, RoutedEventArgs? e)
        {
            List<string> files = FileSeeker.openBLPs();
            BatchConverter.ConvertBLPs(files);
        }
        private void textColorizer(object? sender, RoutedEventArgs? e)
        {
            TextColorizer_Window tw = new TextColorizer_Window();
            tw.ShowDialog();
        }
        private void SwapGeoPos(object? sender, RoutedEventArgs? e)
        {
            // Swap the positions of the two selected geosets
            if (ListGeosets.SelectedItems.Count == 2)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                var centroid1 = Calculator.GetCentroidOfGeoset(geosets[0]);
                var centroid2 = Calculator.GetCentroidOfGeoset(geosets[1]);
                var offset1 = new CVector3(centroid2.X - centroid1.X, centroid2.Y - centroid1.Y, centroid2.Z - centroid1.Z);
                var offset2 = new CVector3(centroid1.X - centroid2.X, centroid1.Y - centroid2.Y, centroid1.Z - centroid2.Z);
                foreach (var vertex in geosets[0].Vertices)
                {
                    vertex.Position = new CVector3(vertex.Position.X + offset1.X, vertex.Position.Y + offset1.Y, vertex.Position.Z + offset1.Z);
                }
                foreach (var vertex in geosets[1].Vertices)
                {
                    vertex.Position = new CVector3(vertex.Position.X + offset2.X, vertex.Position.Y + offset2.Y, vertex.Position.Z + offset2.Z);
                }
            }
            else
            {
                MessageBox.Show("Select exactly 2 geosets");
            }
        }
        private void swapnodesintree(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null && CurrentModel.Nodes.Count > 1)
            {
                INode? node = GetSelectedNode(); if (node == null) { MessageBox.Show("Null node"); return; }
                List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
                nodes.Remove(node.Name);
                Selector s = new Selector(nodes);
                if (s.ShowDialog() == true)
                {
                    var node2 = CurrentModel.Nodes.First(x => x.Name == s.Selected);
                    // If both are at root
                    if (node.Parent.Node == null && node2.Parent.Node == null)
                    {
                        MessageBox.Show("Both nodes are at root. In order to swap locations in tree, one or both have to be under different parents.");
                        return;
                    }
                    // If both are under the same parent
                    if (node.Parent.Node == node2.Parent.Node)
                    {
                        MessageBox.Show("Both nodes are under the same parent. In order to swap locations in tree, one or both have to be under different parents.");
                        return;
                    }
                    // If they are related (one is the parent of the other)
                    if (node.Parent.Node == node2 || node2.Parent.Node == node)
                    {
                        MessageBox.Show("One cannot be a parent of the other. In order to swap locations in tree, one or both have to be under different parents.");
                        return;
                    }
                    // one cannot be nested under the other
                    if (IsNestedChild(node, node2) || IsNestedChild(node2, node))
                    {
                        MessageBox.Show("One cannot be a nested child of the other. In order to swap locations in tree, one or both have to be under different parents.");
                        return;
                    }
                    // Store original parents
                    var parent1 = node.Parent;
                    var parent2 = node2.Parent;
                    // Detach both nodes
                    parent1.Detach();
                    parent2.Detach();
                    // Attach them to the other’s parent
                    parent1.Attach(node2);
                    parent2.Attach(node);
                    RefreshNodesTree();
                }
            }
        }
        private static bool IsNestedChild(INode Node1, INode Node2)
        {
            // If Node2 has no parent, it's not a nested child of anything.
            if (Node2.Parent.Node == null) return false;
            // If Node2's parent is Node1, it's a direct child.
            if (Node2.Parent.Node == Node1) return true;
            // Recursively check if Node1 is an ancestor of Node2.
            return IsNestedChild(Node1, Node2.Parent.Node);
        }
        private INode? GetSelectedNode()
        {
            if (ListNodes.SelectedItem == null) return null;
            TreeViewItem? item = ListNodes.SelectedItem as TreeViewItem; if (item == null) return null;
            StackPanel? sp = item.Header as StackPanel; if (sp == null) return null;
            TextBlock? t = sp.Children[1] as TextBlock; if (t == null) return null;
            string? name = t.Text; if (name == null) return null;
            return CurrentModel.Nodes.First(x => x.Name == name);
        }
        private void swappivotswith(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null && CurrentModel.Nodes.Count > 1)
            {
                INode? node = GetSelectedNode();
                if (node == null) { return; }
                List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
                nodes.Remove(node.Name);
                Selector s = new Selector(nodes);
                if (s.ShowDialog() == true)
                {
                    var node2 = CurrentModel.Nodes.First(x => x.Name == s.Selected);
                    CVector3 temp = new CVector3(node2.PivotPoint.X, node2.PivotPoint.Y, node2.PivotPoint.Z);
                    node2.PivotPoint = node.PivotPoint;
                    node.PivotPoint = temp;
                }
            }
        }
        private void view_front(object? sender, RoutedEventArgs? e)
        {
            SetCameraView(CameraView.Front);
        }
        private void mirrorgeosets(object? sender, RoutedEventArgs? e)
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
                        Calculator.MirrorGeoset(geoset, onX, onY, onZ);
                    }

                }
            }
        }
        private void distanceGeosFromPoint(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 0) return;
            DistanceSelector ds = new DistanceSelector();
            ds.ShowDialog();
            if (ds.DialogResult == true)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                foreach (CGeoset geoset in geosets)
                {
                    Calculator.DistanceGeosetFromPoint(geoset, ds.X, ds.Y, ds.Z, ds.Distance, ds.Method);
                }
            }

        }
        private void copycentroid(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                List<CGeoset> geosets = GetSelectedGeosets();
                var centroid = Calculator.GetCentroidOfGeoset(geosets[0]);
                Clipboard.SetText($"Centroid of geoset {geosets[0].ObjectId}: {centroid.X}, {centroid.Y}, {centroid.Z}");
            }
        }
        private void lsmak(object? sender, RoutedEventArgs? e)
        {
            OpenLink("https://www.hiveworkshop.com/threads/loading-screen-maker.357087/");
        }
        static void OpenLink(string link)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Failed to open link: " + link, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void tli(object? sender, RoutedEventArgs? e)
        {
            OpenLink("https://www.hiveworkshop.com/threads/custom-pathing-texture-maker-v1-03.356305/");
        }
        private static void OpenExe(string partOfPath)
        {
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, partOfPath);
            if (File.Exists(fullPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fullPath,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("Failed to open file: " + fullPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("File not found: " + fullPath, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void txm(object? sender, RoutedEventArgs? e)
        {
            OpenExe("Tools\\cptm.exe");
        }
        private void setModelExtents(object? sender, RoutedEventArgs? e)
        {
            Edit_Extent ee = new Edit_Extent(CurrentModel.Extent);
            ee.ShowDialog();
        }
        private void custom_view(object? sender, RoutedEventArgs? e)
        {
            CameraController c = new CameraController(Scene_ViewportGL.OpenGL);
            c.ShowDialog();
            RefreshViewportCameraDEbugInfo();
        }
        private void mpqbrowser(object? sender, RoutedEventArgs? e)
        {
            MPQBrowser broswer = new MPQBrowser();
            broswer.ShowDialog();
        }
        private void batch_optimize(object? sender, RoutedEventArgs? e)
        {
            List<string> files = FileSeeker.OpenMdlMdxFiles();
            if (files.Count > 0)
            {
                CollectOptimizationSettings();
                foreach (string file in files)
                {
                    CModel? model = ModelSaverLoader.Load(file);
                    if (model == null) continue;
                    Optimizer.Optimize(model);
                    ModelSaverLoader.Save(model, file);
                }
            }
        }

        private void SetViewportGrid(object? sender, TextChangedEventArgs e)
        {
            if (Pause) return;
            bool parse = int.TryParse(ViewPortGrid.Text, out int value);
            if (parse)
            {
                if (value >= 0 && value <= 1000)
                {
                    RenderSettings.
                    ViewportGridSize = value;
                    SaveSettings();

                }
            }
        }

        private void ToggleEdges(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderEdges = Menuitem_edges.IsChecked == true;
            SaveSettings();
        }
        private void ToggleVertices(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderVertices = Menuitem_vertices.IsChecked == true;
            SaveSettings();
        }
        private void ToggleGroundGridX(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderGridX = Menuitem_groundGridX.IsChecked == true;
            SaveSettings();
        }
        private void ToggleGroundGridZ(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderGridX = Menuitem_groundGridY.IsChecked == true;
            SaveSettings();
        }
        private void ToggleGroundGrid(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderGroundGrid = Menuitem_groundGrid.IsChecked == true;
            SaveSettings();
        }
        private void ToggleOOS(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            OptimizeOnSave = ButtonOptimizeOnSave.IsChecked == true;
            SaveSettings();
        }
        private void massCreateGS(object? sender, RoutedEventArgs? e)
        {
            Mass_create_global_sequences msg = new Mass_create_global_sequences(CurrentModel);
            if (msg.ShowDialog() == true)
            {
                RefreshGlobalSequencesList();
                SetSaved(false);
            }
        }
        private void whichnodesnotanimated(object? sender, RoutedEventArgs? e)
        {
            List<string> names = new();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CBone)
                {
                    if (node.Translation.Count == 0 && node.Rotation.Count == 0 && node.Scaling.Count == 0)
                    {
                        names.Add(node.Name);
                    }
                }
            }
            if (names.Count == 0)
            {
                MessageBox.Show("There are no un-animated bones", "Info");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("These bones are not animated:");
                foreach (var name in names) sb.AppendLine(name);

                TextViewer tw = new TextViewer(sb.ToString());
                tw.ShowDialog();

            }
        }
        private void RefreshViewportCameraDEbugInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Eye: {CameraControl.eyeX} {CameraControl.eyeY} {CameraControl.eyeZ} ");
            sb.Append($"center: {CameraControl.CenterX} {CameraControl.CenterY} {CameraControl.CenterZ} ");
            sb.Append($"Roll: {CameraControl.UpX} {CameraControl.UpY} {CameraControl.UpZ}");
            DebugInfoCam.Text = sb.ToString();
        }
        private void createcam(object? sender, RoutedEventArgs? e)
        {
            // Create camera
            CCamera cam = new CCamera(CurrentModel);
            cam.Name = "GeneratedCamera_" + IDCounter.Next_;
            cam.Position = new();
            cam.Position.X = CameraControl.eyeX;
            cam.Position.Y = CameraControl.eyeY;
            cam.Position.Z = CameraControl.eyeZ;
            cam.TargetPosition = new CVector3();
            cam.TargetPosition.X = CameraControl.CenterX;
            cam.TargetPosition.Y = CameraControl.CenterY;
            cam.TargetPosition.Z = CameraControl.CenterZ;
            CurrentModel.Cameras.Add(cam);
        }
        private void ToggleSkeleton(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderSkeleton = Menuitem_skeleton.IsChecked == true;
            SaveSettings();
        }
        private CSequence? SelectSequence()
        {
            List<string> s = CurrentModel.Sequences.Select(X => X.Name).ToList();
            Selector c = new Selector(s);
            if (c.ShowDialog() == true)
            {
                return CurrentModel.Sequences[c.box.SelectedIndex];
            }
            else
            {
                return null;
            }
        }

        List<Texture> Textures_GL = new List<Texture>();

        private void LoadGroundTextureImage()
        {
            string path = "TerrainArt\\Ruins\\Ruins_Grass.blp";
            var bmp = MPQHelper.getBitmapImage(path);
            GroundTexture = new Texture();
            GroundTexture.Create(Scene_ViewportGL.OpenGL, bmp);
        }
        public void CollectTexturesOpenGL()
        {
            Pause = true;
            Textures_GL.Clear();
            foreach (var geoset in CurrentModel.Geosets)
            {
                int id = geoset.Material.Object.Layers[0].Texture.Object.ReplaceableId;
                string path = geoset.Material.Object.Layers[0].Texture.Object.FileName;
                Bitmap bmp;
                Texture t = new Texture();
                if (id == 0) { bmp = MPQHelper.getBitmapImage(path, CurrentSaveFolder); }
                else if (id == 1) bmp = MPQHelper.getBitmapImage(TeamColorPaths[CurrentTeamColor]);
                else if (id == 2) bmp = MPQHelper.getBitmapImage(TeamGlows[CurrentTeamColor]);
                else bmp = MPQHelper.getBitmapImage(White);
                t.Create(Scene_ViewportGL.OpenGL, bmp);
                Textures_GL.Add(t);
            }
            Pause = false;
        }
        private void AdjustCameraGL(OpenGL gl)
        {
            gl.Perspective(RenderSettings.FieldOfView, (double)Width / Height, RenderSettings.NearDistance, RenderSettings.FarDistance);
            gl.LookAt(
           CameraControl.eyeX,
           CameraControl.eyeY,
           CameraControl.eyeZ,
           CameraControl.CenterX,
           CameraControl.CenterY,
           CameraControl.CenterZ,
           CameraControl.UpX,
           CameraControl.UpY,
           CameraControl.UpZ);
        }
        private void DrawScene(object? sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            // MessageBox.Show($"paused; {Pause}, Enabled: {RenderEnabled}, null: {args.OpenGL == null}");
            OpenGL gl = args.OpenGL;
            if (Pause) return;
            if (!RenderSettings.RenderEnabled)
            {
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                return;
            }
            if (gl == null) return;
            // MessageBox.Show("drawing");
            CModel Model = CurrentModel;
            UpdateProjection(gl);
            // background color
            gl.ClearColor(
                RenderSettings.BackgroundColor[0],
                RenderSettings.BackgroundColor[1],
                RenderSettings.BackgroundColor[2],
                1.0f
                );
            // Clear the color and depth buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // Set up the projection and model view matrices
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // pespective
            AdjustCameraGL(gl);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            //culling
            Renderer.HandleCulling(gl);
            // anti anti aliasing

            gl.Enable(OpenGL.GL_LINE_SMOOTH);  // Enable line smoothing
            gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);  // Use the nicest option for smoothing
                                                                    //----------------------------------------------------
                                                                    // rendering
                                                                    //----------------------------------------------------

            if (RenderSettings.RenderGroundPlane) Renderer.RenderGroundTexture(gl, GroundTexture, 100);
            if (RenderSettings.RenderAxis) { Renderer.RenderAxis(gl, RenderSettings.ViewportGridSize); }
            if (RenderSettings.RenderGeosetExtents) { Renderer.RenderExtents(gl, CurrentModel); }
            if (RenderSettings.RenderCollisionShapes) { Renderer.RenderCollisionShapes(gl, CurrentModel); }
            if (RenderSettings.RenderSkinning) { Renderer.RenderRigging(gl, CurrentModel); } // attachments
            if (RenderSettings.RenderSkeleton) { Renderer.RenderSkeleton(gl, CurrentModel); }
            if (RenderSettings.RenderGroundGrid) { Renderer.RenderGrid(gl, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing); }
            if (RenderSettings.RenderGridX) { Renderer.RenderYZGrid(gl, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing); }
            if (RenderSettings.RenderGridY) { Renderer.RenderXZGrid(gl, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing); }
            //MessageBox.Show(RenderTextures.ToString());
            if (RenderSettings.RenderGeometry) Renderer.RenderTriangles(gl, CurrentModel, RenderSettings.RenderTextures, Textures_GL, InAnimator, RenderSettings.RenderShading);
            if (RenderSettings.RenderVertices) Renderer.RenderVertices(gl, Model, InAnimator);
            Renderer.RenderEdges(gl, Model, RenderSettings.RenderEdges);
            if (RenderSettings.RenderNodes) Renderer.RenderNodes(gl, Model, InAnimator);
            if (RenderSettings.RenderNormals) Renderer.RenderNormals(gl, Model);
            if (ViewingPaths) Renderer.RenderSelectedPath(gl);
            // test
            // Renderer.RenderTestLines(gl);
            //  Renderer.RendertestPoints(gl);
            //  Renderer.RenderTestExtents(gl);
            Renderer.HandleLighting(gl, RenderSettings.RenderLighing);


            //-----------------------------------------------------------

            // Renderer.HandeShading(gl, RenderShading);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            // Flush the OpenGL buffer to apply all the drawing commands
            gl.Flush();
        }
        private void UpdateProjection(OpenGL gl)
        {
            int width = (int)Scene_ViewportGL.ActualWidth;
            int height = (int)Scene_ViewportGL.ActualHeight;
            if (height == 0) height = 1; // Prevent division by zero
            float aspectRatio = (float)width / height;
            // Set viewport to match the control size
            gl.Viewport(0, 0, width, height);
            // Reset the projection matrix
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            // Apply perspective projection with corrected aspect ratio
            gl.Perspective(45.0f, aspectRatio, 0.1f, 1000.0f);
            // Switch back to modelview
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }
        private void ToggleNormals(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderNormals = Menuitem_normals.IsChecked == true;
            SaveSettings();
        }
        private void ToggleAxis(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderAxis = Menuitem_axis.IsChecked == true;
            SaveSettings();
        }
        private void SetViewportGridSpacing(object? sender, TextChangedEventArgs e)
        {
            if (Pause) return;
            bool r = int.TryParse(ViewPortLineSpacing.Text, out int result);
            if (r) RenderSettings.LineSpacing = result;
            SaveSettings();
        }
        private void Resized(object? sender, SizeChangedEventArgs e)
        {
            RefreshGrdOverlay();
        }
        private void RefreshGrdOverlay()
        {
            if (Canvas_Grid_Overlay == null) return;
            int SquareSize = RenderSettings.ViewportGridSizeOverlay;
            if (SquareSize <= 0 || SquareSize >= Canvas_Grid_Overlay.ActualWidth || SquareSize >= Canvas_Grid_Overlay.ActualHeight)
            {
                Canvas_Grid_Overlay.Children.Clear();
            }
            else
            {
                Canvas_Grid_Overlay.Children.Clear(); // Optional, in case you want to clear before drawing new grid
                double canvasWidth = Canvas_Grid_Overlay.ActualWidth;
                double canvasHeight = Canvas_Grid_Overlay.ActualHeight;
                // Draw vertical lines
                for (double x = 0; x < canvasWidth; x += SquareSize)
                {
                    Line line = new Line
                    {
                        X1 = x,
                        Y1 = 0,
                        X2 = x,
                        Y2 = canvasHeight,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection() { 2, 2 } // Optional dashed line style
                    };
                    Canvas_Grid_Overlay.Children.Add(line);
                }
                // Draw horizontal lines
                for (double y = 0; y < canvasHeight; y += SquareSize)
                {
                    Line line = new Line
                    {
                        X1 = 0,
                        Y1 = y,
                        X2 = canvasWidth,
                        Y2 = y,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection() { 2, 2 } // Optional dashed line style
                    };
                    Canvas_Grid_Overlay.Children.Add(line);
                }
            }
        }
        private void inputGridOverlaySet(object? sender, TextChangedEventArgs e)
        {
            bool p = int.TryParse(InputGridOverlay.Text, out int value);
            if (p)
            {
                if (value >= 0)
                {
                    RenderSettings.ViewportGridSizeOverlay = value;
                    RefreshGrdOverlay();
                    SaveSettings();
                }
            }
        }
        private void exportGeosetOBJ(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                var geoset = GetSelectedGeosets()[0];
                string savePath = FileSeeker.GetSavePathOBJ();
                if (savePath.Length == 0) return;
                string content = GeosetOBJConverter.GetContent(geoset);
                File.WriteAllText(savePath, content);
            }
        }
        // OR -> ||
        private void exportallobj(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count == 0) { MessageBox.Show("There are no geosets"); return; }
            CGeoset all = new CGeoset(CurrentModel);
            if (CurrentModel.Geosets.Count == 1)
            {
                all = CurrentModel.Geosets[0];
            }
            else
            {
                foreach (var g in CurrentModel.Geosets)
                {
                    all.Vertices.AddRange(g.Vertices);
                    all.Triangles.AddRange(g.Triangles);
                }
            }
            string savePath = FileSeeker.GetSavePathOBJ();
            if (savePath.Length == 0) return;
            string content = GeosetOBJConverter.GetContent(all);
            File.WriteAllText(savePath, content);
        }
        private void REmoveAllLoneNodes(object? sender, RoutedEventArgs? e)
        {
            List<INode> nodes = new List<INode>();
            foreach (var node in CurrentModel.Nodes)
            {
                if ((node.Parent == null || node.Parent.Node == null) && !CurrentModel.Nodes.Any(x => x.Parent.Node == node)) // no parent or child
                {
                    nodes.Add(node);
                }
            }
            foreach (var node in nodes) { CurrentModel.Nodes.Remove(node); }
            RefreshNodesTree();
            SetSaved(false);
        }
        private void exportstl1(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count == 0) { MessageBox.Show("There are no geosets"); return; }
            StringBuilder sb = new StringBuilder();
            string p = FileSeeker.SaveSTL();
            if (p.Length == 0) return;
            foreach (var geo in CurrentModel.Geosets)
            {
                sb.AppendLine(STL_Maker.GenerateFile(geo));
            }
            File.WriteAllText(p, sb.ToString());
        }
        private void exportstl2(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count == 0) { MessageBox.Show("There are no geosets"); return; }
            StringBuilder sb = new StringBuilder();
            string p = FileSeeker.SaveSTL();
            if (p.Length == 0) return;
            CGeoset all = CombineGeosets(CurrentModel.Geosets);
            STL_Maker.GenerateBinarySTL(all, p);
        }
        private CGeoset CombineGeosets(CObjectContainer<CGeoset> geosets)
        {
            CGeoset geo = new CGeoset(CurrentModel);
            foreach (var g in geosets)
            {
                geo.Vertices.AddRange(g.Vertices);
                geo.Triangles.AddRange(g.Triangles);
            }
            return geo;
        }
        private void importSTL(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no materials"); return;
            }
            string file = FileSeeker.openSTL();
            if (file.Length > 0)
            {
                Pause = true;
                var geosets = STL_Maker.Import(CurrentModel, file);
                foreach (var geoset in geosets)
                {
                    CurrentModel.Geosets.Add(geoset);
                }
                RefreshGeosetsList();
                RefreshNodesTree();
                RefreshRenderData(null, null);
                Pause = false;
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
                SetSaved(false);
            }
        }
        private void import3DS(object? sender, RoutedEventArgs? e)
        {

        }
        private void exportply1(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                var list = GetSelectedGeosets();
                if (list.Count == 0) { ParserPLY.ExportASCII(list, CurrentModel); return; }
                MessageBoxResult result = MessageBox.Show(
    "Yes=all as one file, No=Each as file",    // Message text
    "Export Geosets as PLY",               // Dialog title
    MessageBoxButton.YesNoCancel, // Buttons
    MessageBoxImage.Question      // Icon (optional)
);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ParserPLY.ExportASCII(list, CurrentModel);
                        break;
                    case MessageBoxResult.No:
                        ParserPLY.ExportASCII(list, CurrentModel, false);
                        break;
                    case MessageBoxResult.Cancel: break;
                }
            }
        }
        private void exportply2(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count > 0)
            {
                var list = GetSelectedGeosets();
                if (list.Count == 0) { ParserPLY.ExportBinary(list, CurrentModel); return; }
                MessageBoxResult result = MessageBox.Show(
    "Yes=all as one file, No=Each as file",    // Message text
    "Export Geosets as PLY",               // Dialog title
    MessageBoxButton.YesNoCancel, // Buttons
    MessageBoxImage.Question      // Icon (optional)
);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ParserPLY.ExportBinary(list, CurrentModel);
                        break;
                    case MessageBoxResult.No:
                        ParserPLY.ExportBinary(list, CurrentModel, false);
                        break;
                    case MessageBoxResult.Cancel: break;
                }
            }
        }
        private void adjustmdlpoa(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count == 0 && CurrentModel.Nodes.Count == 0)
            {
                MessageBox.Show("There are no geosets and no nodes");
                return;
            }
            InputVector v = new InputVector(AllowedValue.Both, "By...");
            if (v.ShowDialog() == true)
            {
                float x = v.X;
                float y = v.Y;
                float z = v.Z;
                foreach (var geoset in CurrentModel.Geosets)
                {
                    foreach (var vertex in geoset.Vertices)
                    {
                        vertex.Position.X += x;
                        vertex.Position.Y += y;
                        vertex.Position.Z += z;
                    }
                    foreach (var node in CurrentModel.Nodes)
                    {
                        node.PivotPoint.X += x;
                        node.PivotPoint.Y += y;
                        node.PivotPoint.Z += z;
                    }
                }
            }
        }
        private void renamegeoset(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            var g = GetSelectedGeosets()[0];
            Input i = new Input(g.Name);
            if (i.ShowDialog() == true)
            {
                string s = i.box.Text.Trim();
                if (s.Length > 20)
                {
                    MessageBox.Show("Maximum 20 characters!"); return;
                }
                if (CurrentModel.Geosets.Any(x => x.Name == s))
                {
                    MessageBox.Show("There is already a geoset with this name"); return;
                }
                g.Name = s;
                RefreshGeosetsList();
            }
        }

        private void batch_images(object? sender, RoutedEventArgs? e)
        {
            List<string> images = FileSeeker.OpenImages();
            if (images.Count > 0)
            {
                foreach (string image in images)
                {
                    string blp = System.IO.Path.ChangeExtension(image, ".blp");

                    BLPConverter.Convert(image, blp, this, "");
                }
            }
        }
        private void fps(object? sender, RoutedEventArgs? e)
        {
            if (Pause) { return; }
            RenderSettings.RenderFPS = Menuitem_fps.IsChecked == true;
            Scene_ViewportGL.DrawFPS = RenderSettings.RenderFPS;
            SaveSettings();
        }
        private void addw3imp(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                var t = CurrentModel.Textures[List_Textures.SelectedIndex];
                if (t.ReplaceableId == 0)
                {
                    t.FileName = prependWar3Imported(t.FileName);
                    RefreshTexturesList();
                }
                else
                {
                    MessageBox.Show("This texture's replaceable ID is not 0");
                }
            }
        }
        static string prependWar3Imported(string original)
        {
            string prepend = "War3Imported\\";
            return original.StartsWith(prepend) ? original : prepend + original;
        }
        private void stripPath(object? sender, RoutedEventArgs? e)
        {
            var t = CurrentModel.Textures[List_Textures.SelectedIndex];
            if (t.ReplaceableId == 0)
            {
                string name = System.IO.Path.GetFileName(t.FileName);
                t.FileName = name;
                RefreshTexturesList();
                SetSaved(false);
            }
            else
            {
                MessageBox.Show("This texture's replaceable ID is not 0");
            }
        }
        private void importTexture_(object? sender, RoutedEventArgs? e)
        {
            if (CurrentSaveLocation.Length == 0)
            {
                MessageBox.Show("Empty save path"); return;
            }
            if (File.Exists(CurrentSaveLocation))
            {
                List<string> Files = Directory.GetFiles(CurrentSaveFolder).Where(x => System.IO.Path.GetExtension(x).ToLower() == ".blp").ToList();
                if (Files.Count == 0)
                {
                    MessageBox.Show("There are no blp files in the directory of the currently opened model"); return;
                }
                else
                {
                    Selector s = new Selector(Files);
                    if (s.ShowDialog() == true)
                    {
                        int i = s.box.SelectedIndex;
                        string p = Files[i];
                        if (CurrentModel.Textures.Any(x => x.FileName.ToLower() == p.ToLower()))
                        {
                            MessageBox.Show("There is already a texture with this filepath present"); return;
                        }
                        CTexture t = new CTexture(CurrentModel);
                        t.FileName = p;
                        CurrentModel.Textures.Add(t);
                        RefreshTexturesList();
                        CollectTexturesOpenGL();
                        SetSaved(false);
                    }
                }
            }
            else
            {
                MessageBox.Show("The file does not exist at its initially assigned location"); return;
            }
        }
        private void calcexex(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count <= 1) { MessageBox.Show("This command requres at least 2 geosets to be present"); return; }
            List<string> list = getGeosetsAsList();
            Multiselector_Window ms = new Multiselector_Window(list);
            if (ms.ShowDialog() == true)
            {
                if (ms.list.SelectedItems.Count == list.Count)
                {
                    MessageBox.Show("Cannot select all"); return;
                }
                Optimizer.CalculateExtentsWithException(CurrentModel, ms.selectedIndexes);
            }
        }
        private List<string> getGeosetsAsList()
        {
            List<string> list = new();
            foreach (var geo in CurrentModel.Geosets)
            {
                string TitleName = geo.ObjectId.ToString() + $" ({geo.Name}) ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles)";
                list.Add(TitleName);
            }
            return list;
        }
        private void alignNodeWithParent(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null)
            {
                MessageBox.Show("Select a node"); return;
            }
            else
            {
                INode? node = GetSelectedNode();
                if (node == null)
                {
                    MessageBox.Show("Null node"); return;

                }
                if (node.Parent == null || node.Parent.Node == null)
                {
                    MessageBox.Show("This node has no parent"); return;
                }
                if (CurrentModel.Nodes.Contains(node.Parent.Node) == false)
                {
                    MessageBox.Show("This node has no parent"); return;
                }
                var parent = node.Parent.Node;
                axis_picker ap = new axis_picker("Select");
                if (ap.ShowDialog() == true)
                {
                    switch (ap.axis)
                    {
                        case Axes.X: node.PivotPoint.X = parent.PivotPoint.X; break;
                        case Axes.Y: node.PivotPoint.Y = parent.PivotPoint.Y; break;
                        case Axes.Z: node.PivotPoint.Z = parent.PivotPoint.Z; break;
                    }
                }
            }
        }
        private void stobj(object? sender, RoutedEventArgs? e)
        {
            OpenExe("Tools\\OBJ2MDL Batch Converter.exe");
        }
        private void centeratextent(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            ExtentSelector s = new ExtentSelector(CurrentModel, node);
            s.ShowDialog();
        }
        private void delTextureAndMaterials(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem == null) return;
            CTexture texture = GetSElectedTexture();
            if (CurrentModel.Materials.Count > 0 && CurrentModel.Textures.Count == 1)
            {
                MessageBox.Show("Cannot delete when there is one remaing texture and   material/s");
                return;
            }
            if (AllMaterialsUseTexture(texture))
            {
                MessageBox.Show("Cannot delete because all materials use this texture"); return;
            }
            else
            {
                List<CMaterial> toRemove = new List<CMaterial>();
                foreach (var mat in CurrentModel.Materials)
                {
                    foreach (var l in mat.Layers)
                    {
                        if (l.Texture.Object == texture) { toRemove.Add(mat); break; }
                    }
                }
                foreach (var m in toRemove)
                {
                    CurrentModel.Materials.Remove(m);
                }
                CurrentModel.Textures.Remove(texture);
                GiveMaterialToGeosetsWithout();
                RefreshLayersTextureList();
                RefreshMaterialsList();
                RefreshTexturesList();
                SetSaved(false);
            }
        }
        private void GiveMaterialToGeosetsWithout()
        {
            foreach (CGeoset geoset in CurrentModel.Geosets)
            {
                if (geoset == null) { continue; }
                if (geoset.Material == null || geoset.Material.Object == null ||
                    !CurrentModel.Materials.Contains(geoset.Material.Object))
                {
                    if (geoset.Material == null) { continue; }
                    geoset.Material.Attach(CurrentModel.Materials[0]);
                }
            }
        }
        private bool AllMaterialsUseTexture(CTexture t)
        {
            int c = 0;
            foreach (var mat in CurrentModel.Materials)
            {
                foreach (var l in mat.Layers)
                {
                    if (l.Texture.Object == t) { c++; break; ; }
                }
            }
            return CurrentModel.Materials.Count == c;
        }
        private void CopyNodeSequenceKeyframes_Translation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Selct a node"); return; }
            if (CurrentModel.Sequences.Count <= 1) { MessageBox.Show("At least two sequences must be present"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            if (node.Rotation.Count == 0 && node.Translation.Count == 0)
            {
                MessageBox.Show("This node has no translation keyframes"); return;
            }
            CopiedKeyframesData.Cut = false;
            CopiedKeyframesData.Sequence = SelectSequence();
            if (CopiedKeyframesData.Sequence != null)
            {
                int from = CopiedKeyframesData.Sequence.IntervalStart;
                int to = CopiedKeyframesData.Sequence.IntervalEnd;
                if (node.Translation.Any(x => x.Time >= from && x.Time <= to))
                {
                    CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Translation;
                    CopiedKeyframesData.CopiedNode = node;
                }
                else
                {
                    MessageBox.Show("This node has no translation keyframes in this sequence"); return;
                }
            }
        }
        private void CopyNodeSequenceKeyframes_Rotation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Sequences.Count <= 1) { MessageBox.Show("At least two sequences must be present"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode(); if (node == null) return;
            if (node.Rotation.Count == 0 && node.Rotation.Count == 0)
            {
                MessageBox.Show("This node has no rotation keyframes"); return;
            }
            CopiedKeyframesData.Cut = false;
            CopiedKeyframesData.Sequence = SelectSequence();
            if (CopiedKeyframesData.Sequence != null)
            {
                int from = CopiedKeyframesData.Sequence.IntervalStart;
                int to = CopiedKeyframesData.Sequence.IntervalEnd;
                if (node.Rotation.Any(x => x.Time >= from && x.Time <= to))
                {
                    CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Rotation;
                    CopiedKeyframesData.CopiedNode = node;
                }
                else
                {
                    MessageBox.Show("This node has no rotation keyframes in this sequence"); return;
                }
            }
        }
        private void CopyNodeSequenceKeyframes_Scaling(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Sequences.Count <= 1) { MessageBox.Show("At least two sequences must be present"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            if (node.Rotation.Count == 0 && node.Scaling.Count == 0)
            {
                MessageBox.Show("This node has no scaling keyframes"); return;
            }
            CopiedKeyframesData.Cut = false;
            CopiedKeyframesData.Sequence = SelectSequence();
            if (CopiedKeyframesData.Sequence != null)
            {
                int from = CopiedKeyframesData.Sequence.IntervalStart;
                int to = CopiedKeyframesData.Sequence.IntervalEnd;
                if (node.Scaling.Any(x => x.Time >= from && x.Time <= to))
                {
                    CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Scaling;
                    CopiedKeyframesData.CopiedNode = node;
                }
                else
                {
                    MessageBox.Show("This node has no scaling keyframes in this sequence"); return;
                }
            }
        }
        private void CutNodeSequenceKeyframes_Translation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Selct a node"); return; }
            if (CurrentModel.Sequences.Count <= 1) { MessageBox.Show("At least two sequences must be present"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode(); if (node == null) return;
            if (node.Rotation.Count == 0 && node.Translation.Count == 0)
            {
                MessageBox.Show("This node has no translation keyframes"); return;
            }
            CopiedKeyframesData.Cut = true;
            CopiedKeyframesData.Sequence = SelectSequence();
            if (CopiedKeyframesData.Sequence != null)
            {
                int from = CopiedKeyframesData.Sequence.IntervalStart;
                int to = CopiedKeyframesData.Sequence.IntervalEnd;
                if (node.Translation.Any(x => x.Time >= from && x.Time <= to))
                {
                    CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Translation;
                    CopiedKeyframesData.CopiedNode = node;
                }
                else
                {
                    MessageBox.Show("This node has no translation keyframes in this sequence"); return;
                }
            }
        }
        private void CutNodeSequenceKeyframes_Rotation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Sequences.Count <= 1) { MessageBox.Show("At least two sequences must be present"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode(); if (node == null) return;
            if (node.Rotation.Count == 0 && node.Rotation.Count == 0)
            {
                MessageBox.Show("This node has no rotation keyframes"); return;
            }
            CopiedKeyframesData.Cut = true;
            CopiedKeyframesData.Sequence = SelectSequence();
            if (CopiedKeyframesData.Sequence != null)
            {
                int from = CopiedKeyframesData.Sequence.IntervalStart;
                int to = CopiedKeyframesData.Sequence.IntervalEnd;
                if (node.Rotation.Any(x => x.Time >= from && x.Time <= to))
                {
                    CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Rotation;
                    CopiedKeyframesData.CopiedNode = node;
                }
                else
                {
                    MessageBox.Show("This node has no rotation keyframes in this sequence"); return;
                }
            }
        }
        private void CutNodeSequenceKeyframes_Scaling(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Sequences.Count <= 1) { MessageBox.Show("At least two sequences must be present"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode(); if (node == null) return;
            if (node.Rotation.Count == 0 && node.Scaling.Count == 0)
            {
                MessageBox.Show("This node has no scaling keyframes"); return;
            }
            CopiedKeyframesData.Cut = true;
            CopiedKeyframesData.Sequence = SelectSequence();
            if (CopiedKeyframesData.Sequence != null)
            {
                int from = CopiedKeyframesData.Sequence.IntervalStart;
                int to = CopiedKeyframesData.Sequence.IntervalEnd;
                if (node.Scaling.Any(x => x.Time >= from && x.Time <= to))
                {
                    CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Scaling;
                    CopiedKeyframesData.CopiedNode = node;
                }
                else
                {
                    MessageBox.Show("This node has no scaling keyframes in this sequence"); return;
                }
            }
        }
        private void PaseNodeSpecificKeyframesSEquence(object? sender, RoutedEventArgs? e)
        {
            if (CopiedKeyframesData.Sequence != null && CopiedKeyframesData.CopiedNode != null)
            {
                INode? pastedOn = GetSelectedNode();
                if (CopiedKeyframesData.CopiedNode == null) return;
                if (pastedOn == null) return;
                if (CopiedKeyframesData.CopiedNode == pastedOn)
                {
                    MessageBox.Show("Copied and pasted on nodes cannot be the same"); return;
                }
                if (CopiedKeyframesData.CopiedNodeKeyframeType == TransformationType.Translation)
                {
                    int from = CopiedKeyframesData.Sequence.IntervalStart;
                    int to = CopiedKeyframesData.Sequence.IntervalEnd;
                    var isolated = CopiedKeyframesData.CopiedNode.Translation.Where(x => x.Time >= from && x.Time <= to).ToList();
                    if (isolated.Count == 0) { MessageBox.Show("The copied node's sequence keyframes are 0. Nothing to paste."); return; }
                    foreach (var kf in isolated)
                    {
                        if (pastedOn.Translation.Any(x => x.Time == kf.Time))
                        {
                            var overwritten = pastedOn.Translation.First(x => x.Time == kf.Time);
                            overwritten.Value.X = kf.Value.X;
                            overwritten.Value.Y = kf.Value.Y;
                            overwritten.Value.Z = kf.Value.Z;
                        }
                        else
                        {
                            CAnimatorNode<CVector3> new_keyframe = new CAnimatorNode<CVector3>();
                            new_keyframe.Time = kf.Time;
                            new_keyframe.Value = new CVector3(kf.Value);
                            pastedOn.Translation.Add(new_keyframe);
                        }
                    }
                    pastedOn.Translation.NodeList = pastedOn.Translation.NodeList.OrderBy(x => x.Time).ToList();
                    if (CopiedKeyframesData.Cut)
                    {
                        foreach (var kf in isolated)
                        {
                            CopiedKeyframesData.CopiedNode.Translation.Remove(kf);
                        }
                    }
                }
                if (CopiedKeyframesData.CopiedNodeKeyframeType == TransformationType.Rotation)
                {
                    int from = CopiedKeyframesData.Sequence.IntervalStart;
                    int to = CopiedKeyframesData.Sequence.IntervalEnd;
                    var isolated = CopiedKeyframesData.CopiedNode.Rotation.Where(x => x.Time >= from && x.Time <= to).ToList();
                    if (isolated.Count == 0) { MessageBox.Show("The copied node's sequence keyframes are 0. Nothing to paste."); return; }
                    foreach (var kf in isolated)
                    {
                        if (pastedOn.Rotation.Any(x => x.Time == kf.Time))
                        {
                            var overwritten = pastedOn.Rotation.First(x => x.Time == kf.Time);
                            overwritten.Value.X = kf.Value.X;
                            overwritten.Value.Y = kf.Value.Y;
                            overwritten.Value.Z = kf.Value.Z;
                            overwritten.Value.W = kf.Value.W;
                        }
                        else
                        {
                            CAnimatorNode<CVector4> new_keyframe = new CAnimatorNode<CVector4>();
                            new_keyframe.Time = kf.Time;
                            new_keyframe.Value = new CVector4(kf.Value);
                            pastedOn.Rotation.Add(new_keyframe);
                        }
                    }
                    pastedOn.Rotation.NodeList = pastedOn.Rotation.NodeList.OrderBy(x => x.Time).ToList();
                    if (CopiedKeyframesData.Cut)
                    {
                        foreach (var kf in isolated)
                        {
                            CopiedKeyframesData.CopiedNode.Rotation.Remove(kf);
                        }
                    }
                }
                if (CopiedKeyframesData.CopiedNodeKeyframeType == TransformationType.Scaling)
                {
                    int from = CopiedKeyframesData.Sequence.IntervalStart;
                    int to = CopiedKeyframesData.Sequence.IntervalEnd;
                    var isolated = CopiedKeyframesData.CopiedNode.Scaling.Where(x => x.Time >= from && x.Time <= to).ToList();
                    if (isolated.Count == 0) { MessageBox.Show("The copied node's sequence keyframes are 0. Nothing to paste."); return; }
                    foreach (var kf in isolated)
                    {
                        if (pastedOn.Scaling.Any(x => x.Time == kf.Time))
                        {
                            var overwritten = pastedOn.Scaling.First(x => x.Time == kf.Time);
                            overwritten.Value.X = kf.Value.X;
                            overwritten.Value.Y = kf.Value.Y;
                            overwritten.Value.Z = kf.Value.Z;
                        }
                        else
                        {
                            CAnimatorNode<CVector3> new_keyframe = new CAnimatorNode<CVector3>();
                            new_keyframe.Time = kf.Time;
                            new_keyframe.Value = new CVector3(kf.Value);
                            pastedOn.Scaling.Add(new_keyframe);
                        }
                    }
                    pastedOn.Scaling.NodeList = pastedOn.Scaling.NodeList.OrderBy(x => x.Time).ToList();
                    if (CopiedKeyframesData.Cut)
                    {
                        foreach (var kf in isolated)
                        {
                            CopiedKeyframesData.CopiedNode.Scaling.Remove(kf);
                        }
                    }
                }
            }
        }
        private bool OpenGLInitialized = false;
        private void InitializeSharpGL(object? sender, EventArgs e)
        {

            if (OpenGLInitialized) return;
            // if (Scene_ViewportGL.IsInitialized || Scene_ViewportGL.OpenGL != null) { return; }
            OpenGLControl? c = sender as OpenGLControl; if (c == null) return;
            OpenGL gl = c.OpenGL;
            Dispatcher.Invoke(() =>
            {
                Scene_ViewportGL.RenderTrigger = RenderTrigger.TimerBased; // Start rendering after init
                Scene_ViewportGL.DoRender(); // Safe to render now
            });
            OpenGLInitialized = true;
            // MessageBox.Show("initialized");
        }
        private SceneInteractionState CurrentSceneInteraction = SceneInteractionState.None;
        private Point selectionStart; // Starting point for selection rectangle
        private System.Windows.Shapes.Rectangle? selectionRectangle = new System.Windows.Shapes.Rectangle(); // Fully qualified path to avoid ambiguity
        private bool isRotating = false; // Flag for camera rotation

        private void Scene_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            var Canvas_Selection = Scene_Selection_Overlay;
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                // Toggle RotatingView mode
                if (CurrentSceneInteraction == SceneInteractionState.RotatingView)
                {
                    CurrentSceneInteraction = SceneInteractionState.None;
                    Canvas_Selection.Cursor = Cursors.Arrow;
                }
                else
                {
                    CurrentSceneInteraction = SceneInteractionState.RotatingView;
                    Canvas_Selection.Cursor = Cursors.ScrollAll;
                    isRotating = true;
                    selectionStart = e.GetPosition(Canvas_Selection);
                }
            }
            /* else if (e.MiddleButton == MouseButtonState.Pressed)
             {
                 if (CurrentSceneInteraction == SceneInteractionState.RotatingView) return;
                 if (CurrentSceneInteraction == SceneInteractionState.None)
                 {
                     CurrentSceneInteraction = SceneInteractionState.DragView;
                 }
             }*/
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (CurrentSceneInteraction == SceneInteractionState.RotatingView) return;
                // Start selection only if no other interaction is active
                else if (CurrentSceneInteraction == SceneInteractionState.None)
                {
                    CurrentSceneInteraction = SceneInteractionState.DrawRectangle;
                    selectionStart = e.GetPosition(Canvas_Selection);
                    selectionRectangle = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 1,
                        Fill = Brushes.Transparent
                    };
                    Canvas.SetLeft(selectionRectangle, selectionStart.X);
                    Canvas.SetTop(selectionRectangle, selectionStart.Y);
                    Canvas_Selection.Children.Add(selectionRectangle);
                }
                if (modifyMode_current == ModifyMode.Sculpt)
                {
                    Sculpt(false);
                }
                if (modifyMode_current == ModifyMode.SculptPersonal)
                {
                    SculptPersonal(false);
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                CurrentSceneInteraction = SceneInteractionState.Modify;

                if (modifyMode_current == ModifyMode.Sculpt)
                {
                    Sculpt(true);
                }
                if (modifyMode_current == ModifyMode.SculptPersonal)
                {
                    SculptPersonal(true);
                }
            }
        }



        public static Vector2[] GetCorners(System.Windows.Shapes.Rectangle rect)
        {
            double left = Canvas.GetLeft(rect);
            double top = Canvas.GetTop(rect);
            double width = rect.Width;
            double height = rect.Height;
            return new Vector2[]
            {
            new Vector2((float)left, (float)top),                     // TopLeft
            new Vector2((float)(left + width), (float)top),           // TopRight
            new Vector2((float)left, (float)(top + height)),          // BottomLeft
            new Vector2((float)(left + width), (float)(top + height)) // BottomRight
            };
        }



        private void DrawRay(Point currentMousePos)
        {

            if (Tabs_Geosets.SelectedIndex > 3) return;
            var ray = RayCaster.GetRay((float)currentMousePos.X, (float)currentMousePos.Y, (float)Scene_ViewportGL.ActualWidth, (float)Scene_ViewportGL.ActualHeight, Scene_ViewportGL.OpenGL);


            if (Tabs_Geosets.SelectedIndex == 0)
            {
                if (CurrentModel.Geosets.Count == 0) return;

                for (int i = 0; i < CurrentModel.Geosets.Count; i++)
                {

                    bool inside = RayCaster.GeosetInsideRay(CurrentModel.Geosets[i], ray);
                    CurrentModel.Geosets[i].isSelected = inside;
                    if (inside)
                    {
                        ListGeosets.SelectedItems.Add(ListGeosets.Items[i]);
                    }
                    else
                    {
                        ListGeosets.SelectedItems.Remove(ListGeosets.Items[i]);
                    }

                }
            }

            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                List<CGeosetTriangle> selected = new List<CGeosetTriangle>();
                foreach (var g in CurrentModel.Geosets)
                {
                    foreach (var triangle in g.Triangles)
                    {
                        bool inside = RayCaster.TriangleInsideRayRadius(ray, triangle);
                        if (SelectionMode == mSelectionMode.Clear)
                        {
                            if (inside) selected.Add(triangle);
                            else { triangle.isSelected = false; }
                        }
                        else if (SelectionMode == mSelectionMode.Remove)
                        {
                            if (triangle.isSelected == false) selected.Add(triangle);
                            else { triangle.isSelected = false; }
                        }
                        else if (SelectionMode == mSelectionMode.Add && inside)
                        {
                            selected.Add(triangle);
                        }
                        // 
                    }
                }
                SelectClosestTriangle(ray, selected);
                ListSelectedTriangles();
            }
            else if (Tabs_Geosets.SelectedIndex >= 2) //vertices, rigging
            {

                List<CGeosetVertex> SelectionGroup = new List<CGeosetVertex>();
                foreach (var g in CurrentModel.Geosets)
                {
                    foreach (var vertex in g.Vertices)
                    {
                        if (g.isVisible == false) { vertex.isSelected = false; continue; }
                        bool inside = RayCaster.VertexInsideRayRadius(ray, vertex);
                        if (SelectionMode == mSelectionMode.Clear)
                        {
                            vertex.isSelected = false;
                            if (inside) { vertex.isSelected = true; }
                        }
                        else if (SelectionMode == mSelectionMode.Remove)
                        {

                            vertex.isSelected = false;
                        }
                        else if (SelectionMode == mSelectionMode.Add && inside)
                        {

                            SelectionGroup.Add(vertex);
                            vertex.isSelected = true;
                        }
                        // 
                    }
                    UpdateSelectedVerticesInfo();
                }
                // if (SelectionGroup.Count == 0) return;
                //  CGeosetVertex closest = GetClosestVertex(SelectionGroup, ray.From);

                // closest.isSelected = true;
                RefreshrigingAttachedList();




            }
            // RayCaster.Lines.Add(line);




            // SelectAffectedGeosets(lne);

        }

        private void ListSelectedTriangles()
        {

            int count = CurrentModel.Geosets.SelectMany(x => x.Triangles).Where(x => x.isSelected).Count();
            Label_SelectedTriangles.Text = $"Selected Triangles: {count}";
        }

        private static void SelectClosestTriangle(Ray ray, List<CGeosetTriangle> selected)
        {
            if (selected.Count == 0) return;
            List<Vector3> centroids = new();
            foreach (var triangle in selected)
            {
                centroids.Add(Calculator.GetCentroidofTriangleC(triangle));
            }
            var closest = Calculator.FindClosestVector(ray.From, centroids);

            for (int i = 0; i < selected.Count; i++)
            {
                if (
                    closest.X == centroids[i].X &&
                    closest.Y == centroids[i].Y &&
                    closest.Z == centroids[i].Z

                    )
                {
                    selected[i].isSelected = true;
                    return;
                }
            }
        }

        public static CGeosetVertex? GetClosestVertex(List<CGeosetVertex> vertices, Vector3 target)
        {
            if (vertices == null || vertices.Count == 0)
                return null;
            if (vertices.Count == 1) { return vertices[0]; }
            CGeosetVertex? closest = null;
            float closestDistanceSq = float.MaxValue;

            foreach (var vertex in vertices)
            {
                float dx = vertex.Position.X - target.X;
                float dy = vertex.Position.Y - target.Y;
                float dz = vertex.Position.Z - target.Z;

                float distanceSq = dx * dx + dy * dy + dz * dz;
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = vertex;
                }
            }

            return closest;
        }

        private void UpdateSelectedVerticesInfo()
        {
            int count = 0;
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    count = vertex.isSelected ? count + 1 : count;
                }
            }
            Label_SelectedVertices.Text = $"Selected vertices: {count}";
        }


        private void Scene_MouseUp(object? sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (CurrentSceneInteraction == SceneInteractionState.RotatingView)
                {
                    return;
                }

                CurrentSceneInteraction = SceneInteractionState.None;
                return;
            }

            if (e.ChangedButton != MouseButton.Left)
            {
                return; // Ignore non-left clicks
            }

            // Only proceed if we're in a selection mode or idle
            if (CurrentSceneInteraction != SceneInteractionState.None &&
                CurrentSceneInteraction != SceneInteractionState.DrawRectangle)
            {
                CurrentSceneInteraction = SceneInteractionState.None;
                return;
            }

            // ----- Selection Logic Below -----

            var Scene_Canvas_ = Scene_Selection_Overlay;
            var currentMousePos = e.GetPosition(Scene_Canvas_);

            double x1 = Math.Min(selectionStart.X, currentMousePos.X);
            double y1 = Math.Min(selectionStart.Y, currentMousePos.Y);
            double x2 = Math.Max(selectionStart.X, currentMousePos.X);
            double y2 = Math.Max(selectionStart.Y, currentMousePos.Y);

            if (Math.Abs(x2 - x1) < 1.0 && Math.Abs(y2 - y1) < 1.0)
            {
                DrawRay(currentMousePos);
            }
            else
            {
                DrawRay(x1, x2, y1, y2);
            }

            Vector2[] SelectionData = new Vector2[]
            {
        new Vector2((float)x1, (float)y1),
        new Vector2((float)x2, (float)y1),
        new Vector2((float)x2, (float)y2),
        new Vector2((float)x1, (float)y2)
            };

            if (selectionRectangle != null)
            {
                Scene_Canvas_.Children.Remove(selectionRectangle);
                selectionRectangle = null;
            }

            CurrentSceneInteraction = SceneInteractionState.None;
        }


        private void DrawRay(double x1, double x2, double y1, double y2)
        {
            if (Tabs_Geosets.SelectedIndex > 3) return;
            var SelectionExtent = RayCaster.ExtentFromMouseSelectionRectangle(x1, x2, y1, y2, (float)Scene_ViewportGL.ActualWidth, (float)Scene_ViewportGL.ActualHeight, Scene_ViewportGL.OpenGL);
            if (Tabs_Geosets.SelectedIndex == 0)
            {
                if (CurrentModel.Geosets.Count == 0) return;
                for (int i = 0; i < CurrentModel.Geosets.Count; i++)
                {
                    bool selected = RayCaster.GeosetInsideExtent(CurrentModel.Geosets[i], SelectionExtent);
                    CurrentModel.Geosets[i].isSelected = selected;
                    switch (SelectionMode)
                    {
                        case mSelectionMode.Clear:
                            if (selected)
                            {
                                ListGeosets.SelectedItems.Add(ListGeosets.Items[i]);
                            }
                            else
                            {
                                ListGeosets.SelectedItems.Remove(ListGeosets.Items[i]);
                            }
                            break;
                        case mSelectionMode.Remove:

                            ListGeosets.SelectedItems.Remove(ListGeosets.Items[i]);

                            break;
                        case mSelectionMode.Add:

                            ListGeosets.SelectedItems.Add(ListGeosets.Items[i]);


                            break;
                    }


                }
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                // unfinished
            }
            else if (Tabs_Geosets.SelectedIndex >= 2) //vertices, rigging
            {
                List<CGeosetVertex> vertices = new List<CGeosetVertex>(0);
                foreach (var g in CurrentModel.Geosets)
                {
                    foreach (var vertex in g.Vertices)
                    {
                        bool inside = RayCaster.VertexInsideSelectionExtent(SelectionExtent, vertex);
                        if (SelectionMode == mSelectionMode.Clear)
                        {
                            vertex.isSelected = inside;
                            if (inside) vertices.Add(vertex);
                        }
                        else if (SelectionMode == mSelectionMode.Remove)
                        {
                            vertex.isSelected = !inside;
                            if (inside) vertices.Add(vertex);
                        }
                        else if (SelectionMode == mSelectionMode.Add && inside)
                        {
                            vertex.isSelected = true;
                            if (inside) vertices.Add(vertex);
                        }
                        // 
                    }
                }
                RefreshrigingAttachedList();
            }

        }

        double previousY = 0;
        private void Scene_Selection_Overlay_MouseMove(object? sender, MouseEventArgs e)
        {
            var currentMousePos = e.GetPosition(Scene_Selection_Overlay);


            if (CurrentSceneInteraction == SceneInteractionState.DrawRectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                if (selectionRectangle == null) return;
                double x = Math.Min(selectionStart.X, currentMousePos.X);
                double y = Math.Min(selectionStart.Y, currentMousePos.Y);
                double width = Math.Abs(currentMousePos.X - selectionStart.X);
                double height = Math.Abs(currentMousePos.Y - selectionStart.Y);
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;
                Canvas.SetLeft(selectionRectangle, x);
                Canvas.SetTop(selectionRectangle, y);
            }

            else if (CurrentSceneInteraction == SceneInteractionState.RotatingView && isRotating)
            {
                float angle = 0.5f;
                Vector3 center = new Vector3(CameraControl.CenterX, CameraControl.CenterY, CameraControl.CenterZ);
                if (currentMousePos.Y > LastClickPositionY)
                {
                    CameraControl.RotateDown(center, angle);
                }
                else
                {
                    CameraControl.RotateUp(center, angle);
                }
                if (currentMousePos.X > LastClickPositionX)
                {
                    CameraControl.RotateLeft(center, angle);
                }
                else if (currentMousePos.X < LastClickPositionX)
                {
                    CameraControl.RotateRight(center, angle);
                }
                RefreshViewportCameraDEbugInfo();
                LastClickPositionX = currentMousePos.X;
                LastClickPositionY = currentMousePos.Y;
            }
            else if (CurrentSceneInteraction == SceneInteractionState.Modify && CanDrag && e.RightButton == MouseButtonState.Pressed)
            {
                bool positive = currentMousePos.Y < previousY;
                previousY = currentMousePos.Y;
                DebugInfoCam.Text = !positive ? "up" : "down"; //debug
                ModifySelectionByDragging(modifyMode_current, axisMode, positive);




            }

        }
        private void SetViewportZoomIncrement(object? sender, TextChangedEventArgs e)
        {
            bool get = int.TryParse(InputZoomIncrement.Text, out int val);
            if (get)
            {
                float r = val;
                if (val <= 0) r = 0.1f;
                ZoomIncrement = r;
                CameraControl.ZoomIncrement = ZoomIncrement;
                SaveSettings();
            }
        }
        private void ResetStaticColorToDefault(object? sender, RoutedEventArgs? e)
        {
            foreach (var gs in CurrentModel.GeosetAnimations)
            {
                if (gs.Color.Static)
                {
                    gs.Color.MakeStatic(new CVector3(1, 1, 1));
                }
            }
            RefreshGeosetAnimationsList();
        }
        private void shatterGeoset(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count != 1) { MessageBox.Show("select a single geoset"); return; }
            var g = GetSelectedGeosets()[0];
            ShatterAnimationMaker shm = new ShatterAnimationMaker(g, CurrentModel);
            shm.ShowDialog();
            if (shm.ShowDialog() == true)
            {
                RefreshNodesTree();
            }
        }
        private void changeoebasedonkef(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count < 1)
            {
                MessageBox.Show("Select at least one geoset"); return;
            }
            var geosets = GetSelectedGeosets();
            Input i = new Input("0", "Select a track");
            i.ShowDialog();
            if (i.DialogResult != true) { return; }
            bool p = int.TryParse(i.box.Text.Trim(), out int v);
            if (p)
            {
                if (TrackExistsInSEquences(v, null))
                {
                    foreach (var geoset in geosets)
                    {
                        if (GeosetVerticesAttachedToOneGroup(geoset))
                        {
                            var group = geoset.Vertices[0].Group.Object;
                            foreach (var gnode in group.Nodes)
                            {
                                var node = gnode.Node.Node;
                                if (node.Scaling.Any(x => x.Time == v))
                                {
                                    CVector3 value = node.Scaling.First(x => x.Time == v).Value;
                                    Calculator.ScaleGeoset(geoset, node.PivotPoint, value);
                                }
                                if (node.Rotation.Any(x => x.Time == v))
                                {
                                    CVector4 value = node.Rotation.First(x => x.Time == v).Value;
                                    CVector3 euler = Calculator.QuaternionToEuler(value);
                                    Calculator.RotateGeosetAroundPivotPoint(node.PivotPoint, geoset, euler.X, euler.Y, euler.Z);
                                }
                                if (node.Translation.Any(x => x.Time == v))
                                {
                                    CVector3 value = node.Translation.First(x => x.Time == v).Value;
                                    foreach (var vertex in geoset.Vertices)
                                    {
                                        vertex.Position.X += value.X;
                                        vertex.Position.Y += value.Y;
                                        vertex.Position.Z += value.Z;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Geoset {geoset.ObjectId}: All vertices must be attachd to one group");
                            continue; ;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This track does not exist in any sequence"); return;
                }
            }
            else
            {
                MessageBox.Show("Not an integer"); return;
            }
        }
        private static bool GeosetVerticesAttachedToOneGroup(CGeoset geoset)
        {
            if (geoset.Vertices.Count <= 1) { return true; }
            var group = geoset.Vertices[0].Group.Object;
            for (int i = 1; i < geoset.Vertices.Count; i++)
            {
                if (geoset.Vertices[0].Group.Object != group) { return false; }
            }
            return true;
            throw new NotImplementedException();
        }
        private void CopyNodeKeyframes_Translation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            CopiedKeyframesData.Cut = false;
            CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Translation;
        }
        private void CopyNodeKeyframes_Rotation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode(); if (node == null) return;
            CopiedKeyframesData.Cut = false;
            CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Rotation;
        }
        private void CopyNodeKeyframes_Scaling(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            CopiedKeyframesData.Cut = false;
            CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Scaling;
        }
        private void CutNodeKeyframes_Translation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            CopiedKeyframesData.Cut = true;
            CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Translation;
        }
        private void CutNodeKeyframes_Rotation(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            CopiedKeyframesData.Cut = true;
            CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Rotation;
        }
        private void CutNodeKeyframes_Scaling(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (CurrentModel.Nodes.Count <= 1) { MessageBox.Show("At least two nodes must be present"); return; }
            INode? node = GetSelectedNode();
            if (node == null) return;
            CopiedKeyframesData.Cut = true;
            CopiedKeyframesData.CopiedNodeKeyframeType = TransformationType.Scaling;
        }
        private void PaseNodeSpecificKeyframes(object? sender, RoutedEventArgs? e)
        {
            if (CopiedKeyframesData.CopiedNode == null) { MessageBox.Show("Copied node does not exist"); }
            if (CurrentModel.Nodes.Contains(CopiedKeyframesData.CopiedNode) == false)
            {
                MessageBox.Show("Copied node is not part of the model");
                if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
                INode? node = GetSelectedNode();
                if (node == null) return;
                if (CopiedKeyframesData.CopiedNode == null) return;
                if (CopiedKeyframesData.CopiedNode == node) { MessageBox.Show("Copied cannot be the as pasted on"); return; }
                switch (CopiedKeyframesData.CopiedNodeKeyframeType)
                {
                    case TransformationType.Translation:
                        node.Translation.Clear();
                        foreach (var kf in CopiedKeyframesData.CopiedNode.Translation)
                        {
                            node.Translation.Add(new CAnimatorNode<CVector3>(kf));
                        }
                        break;
                    case TransformationType.Rotation:
                        node.Rotation.Clear();
                        foreach (var kf in CopiedKeyframesData.CopiedNode.Rotation)
                        {
                            node.Rotation.Add(new CAnimatorNode<CVector4>(kf));
                        }
                        break;
                    case TransformationType.Scaling:
                        node.Scaling.Clear();
                        foreach (var kf in CopiedKeyframesData.CopiedNode.Scaling)
                        {
                            node.Scaling.Add(new CAnimatorNode<CVector3>(kf));
                        }
                        break;
                }
                SetSaved(false);
            }
        }
        private void PaseNodeSpecificKeyframesSEquenceMultiple(object? sender, RoutedEventArgs? e)
        {
            if (CopiedKeyframesData.CopiedNode == null) { MessageBox.Show("Copied node does not exist"); return; }

            if (CurrentModel.Nodes.Contains(CopiedKeyframesData.CopiedNode) == false)
            {
                MessageBox.Show("Copied node is not part of the model");
                if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
                List<INode> nodes = CurrentModel.Nodes.ToList();
                nodes.Remove(CopiedKeyframesData.CopiedNode);
                List<string> list = nodes.Select(x => x.Name).ToList(); ;
                Multiselector_Window ms = new Multiselector_Window(list);
                if (ms.ShowDialog() == true)
                {
                    foreach (var pastedOn in nodes)
                    {
                        switch (CopiedKeyframesData.CopiedNodeKeyframeType)
                        {
                            case TransformationType.Translation:
                                pastedOn.Translation.Clear();
                                foreach (var kf in CopiedKeyframesData.CopiedNode.Translation)
                                {
                                    pastedOn.Translation.Add(new CAnimatorNode<CVector3>(kf));
                                }
                                break;
                            case TransformationType.Rotation:
                                pastedOn.Rotation.Clear();
                                foreach (var kf in CopiedKeyframesData.CopiedNode.Rotation)
                                {
                                    pastedOn.Rotation.Add(new CAnimatorNode<CVector4>(kf));
                                }
                                break;
                            case TransformationType.Scaling:
                                pastedOn.Scaling.Clear();
                                foreach (var kf in CopiedKeyframesData.CopiedNode.Scaling)
                                {
                                    pastedOn.Scaling.Add(new CAnimatorNode<CVector3>(kf));
                                }
                                break;
                        }
                    }
                }
            }
        }
        private void RefreshRenderData(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            CollectTexturesOpenGL();
            CalculateModelLines();
            CalculateGeosetExtents();
            foreach (var g in CurrentModel.Geosets) g.RecalculateEdges();
            Pause = false;
        }

        private void renameModelFile(object? sender, RoutedEventArgs? e)
        {
            if (File.Exists(CurrentSaveLocation))
            {
                string? name = System.IO.Path.GetFileNameWithoutExtension(CurrentSaveLocation);
                Input s = new(name);
                if (s.ShowDialog() == true)
                {
                    string i = s.Result;
                    if (i.Length > 0)
                    {
                        CurrentSaveLocation = FileRenamer.RenameFile(CurrentSaveLocation, i);
                        RefreshTitle();
                    }
                }
            }
        }
        private void removeAllExtents(object? sender, RoutedEventArgs? e)
        {
            foreach (var sequence in CurrentModel.Sequences)
            {
                sequence.Extent = new CExtent();
            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.Extent = new CExtent();
                foreach (var extent in geoset.Extents)
                {
                    extent.Extent = new CExtent();
                }
            }
            RefreshRenderData(null, null);
        }
        private void view_gs_groups(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                var g = GetSelectedGeosets()[0];
                if (g.Groups.Count == 0) { MessageBox.Show("This geoset doesn't have groups!"); return; }
                GeosetGroupViewer gw = new GeosetGroupViewer(g);
                gw.ShowDialog();
            }
            else
            {
                MessageBox.Show("Select a single geoset");
            }
        }
        private void edituv_g(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                //  mapper.Show();
                //  mapper.SetData(CurrentModel, ListGeosets.Items.IndexOf(ListGeosets.SelectedItems[0]));
            }
            else
            {
                MessageBox.Show("Select a single geoset");
            }
        }
        private void rkr(object? sender, RoutedEventArgs? e)
        {
            var rk = new RawKeyframeRearranger();
            rk.ShowDialog();
        }
        private void setsqExtentsall(object? sender, RoutedEventArgs? e)
        {
            Edit_Extent ee = new Edit_Extent(CurrentModel, true);
            ee.ShowDialog();
            if (ee.DialogResult == true) History.Sequences.Add(CurrentModel);
        }
        private void tss(object? sender, RoutedEventArgs? e)
        {
            string path = System.IO.Path.Combine(AppHelper.Local, "Paths\\Tilepaths.txt");
            bigTextContainer bx = new bigTextContainer(path);
            bx.ShowDialog();
        }
        private void pastega_a(object? sender, RoutedEventArgs? e)
        {
            var selected = GetSelectedGeosetAnimation();
            if (CopiedAnimation != null && CurrentModel.GeosetAnimations.Contains(CopiedAnimation))
            {
                if (selected == CopiedAnimation)
                {
                    MessageBox.Show("Copied and pasted cannot be the same!"); return;
                }
                if (CopiedAnimation.Alpha.Static)
                {
                    selected.Alpha.MakeStatic(CopiedAnimation.Alpha.GetValue());
                }
                else
                {
                    selected.Alpha.MakeAnimated();
                    selected.Alpha.Clear();
                    foreach (var kf in CopiedAnimation.Alpha.NodeList)
                    {
                        selected.Alpha.Add(new CAnimatorNode<float>(kf));
                    }
                }
                RefreshGeosetAnimationsList();
            }
        }
        private void pastega_c(object? sender, RoutedEventArgs? e)
        {
            var selected = GetSelectedGeosetAnimation();
            if (CopiedAnimation != null && CurrentModel.GeosetAnimations.Contains(CopiedAnimation))
            {
                if (selected == CopiedAnimation)
                {
                    MessageBox.Show("Copied and pasted cannot be the same!"); return;
                }
                if (CopiedAnimation.Color.Static)
                {
                    selected.Color.MakeStatic(CopiedAnimation.Color.GetValue());
                }
                else
                {
                    selected.Color.MakeAnimated();
                    selected.Color.Clear();
                    foreach (var kf in CopiedAnimation.Color.NodeList)
                    {
                        selected.Color.Add(new CAnimatorNode<CVector3>(kf));
                    }
                }
                RefreshGeosetAnimationsList();
            }
        }
        private void pastega_ca(object? sender, RoutedEventArgs? e)
        {
            var selected = GetSelectedGeosetAnimation();
            if (CopiedAnimation != null && CurrentModel.GeosetAnimations.Contains(CopiedAnimation))
            {
                if (selected == CopiedAnimation)
                {
                    MessageBox.Show("Copied and pasted cannot be the same!"); return;
                }
                if (CopiedAnimation.Alpha.Static)
                {
                    selected.Alpha.MakeStatic(CopiedAnimation.Alpha.GetValue());
                }
                else
                {
                    selected.Alpha.MakeAnimated();
                    selected.Alpha.Clear();
                    foreach (var kf in CopiedAnimation.Alpha.NodeList)
                    {
                        selected.Alpha.Add(new CAnimatorNode<float>(kf));
                    }
                }
                if (CopiedAnimation.Color.Static)
                {
                    selected.Color.MakeStatic(CopiedAnimation.Color.GetValue());
                }
                else
                {
                    selected.Color.MakeAnimated();
                    selected.Color.Clear();
                    foreach (var kf in CopiedAnimation.Color.NodeList)
                    {
                        selected.Color.Add(new CAnimatorNode<CVector3>(kf));
                    }
                }
                RefreshGeosetAnimationsList();
            }
        }

        private void copyga(object? sender, RoutedEventArgs? e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CopiedAnimation = GetSelectedGeosetAnimation();
            }
        }
        private void CopyTexture(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                var t = CurrentModel.Textures[List_Textures.Items.IndexOf(List_Textures.SelectedItem)];
                if (t.ReplaceableId == 0)
                {
                    Clipboard.SetText(t.FileName);
                }
                else
                {
                    MessageBox.Show("Cannot copy texture that has no repalceable texture 0"); return;
                }
            }
        }
        private void PasteTexture(object? sender, RoutedEventArgs? e)
        {
            string copied = Clipboard.GetText();
            if (CurrentModel.Textures.Any(x => x.FileName.ToLower() == copied.ToLower()))
            {
                MessageBox.Show("There is already a texture with this path"); return;
            }
            else
            {
                if (MPQHelper.FileExists(copied.ToLower().Trim()))
                {
                    CTexture nt = new CTexture(CurrentModel);
                    nt.FileName = copied;
                    CurrentModel.Textures.Add(nt);
                    RefreshTexturesList();
                }
                else { MessageBox.Show("The pasted string is not a valid path within the MPQs"); return; }
            }
        }
        private void SelectedCS(object? sender, SelectionChangedEventArgs? e)
        {
            if (list_cs.SelectedItem == null) return;
            CurrentlySelectedCollisionShape = GetSelectedCollisionShape();
        }
        private void SetCurrentCSModAmount(object? sender, TextChangedEventArgs e)
        {
            bool p = float.TryParse(inputCSModifyAmmount.Text, out float val);
            if (p)
            {
                if (val > 0) { CurentCSModifyAmount = val; }
                else { CurentCSModifyAmount = 1; }
            }
            else
            {
                CurentCSModifyAmount = 1;
            }
        }
        private void ScaleUpCS(object? sender, RoutedEventArgs? e)
        {
            if (CurrentlySelectedCollisionShape != null)
            {
                if (CurrentlySelectedCollisionShape.Type == ECollisionShapeType.Box)
                {
                    CurrentlySelectedCollisionShape.Vertex1.X -= CurentCSModifyAmount;
                    CurrentlySelectedCollisionShape.Vertex1.Y -= CurentCSModifyAmount;
                    CurrentlySelectedCollisionShape.Vertex1.Z -= CurentCSModifyAmount;
                    CurrentlySelectedCollisionShape.Vertex2.X += CurentCSModifyAmount;
                    CurrentlySelectedCollisionShape.Vertex2.Y += CurentCSModifyAmount;
                    CurrentlySelectedCollisionShape.Vertex2.Z += CurentCSModifyAmount;
                }
                else
                {
                    CurrentlySelectedCollisionShape.Radius += CurentCSModifyAmount;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void ScaleDownCS(object? sender, RoutedEventArgs? e)
        {
            if (CurrentlySelectedCollisionShape != null)
            {
                if (CurrentlySelectedCollisionShape.Type == ECollisionShapeType.Box)
                {
                    var cs = CurrentlySelectedCollisionShape;
                    var m = CurentCSModifyAmount;
                    // minimum extent
                    if (
                        cs.Vertex1.X + m < 0 &&
                        cs.Vertex1.Y + m < 0 &&
                        cs.Vertex1.Z + m < 0 &&
                        cs.Vertex2.X + m > 0 &&
                        cs.Vertex2.Y + m > 0 &&
                        cs.Vertex2.Z + m > 0
                        )
                    {
                        cs.Vertex1.X += m;
                        cs.Vertex1.Y += m;
                        cs.Vertex1.Z += m;
                        cs.Vertex2.X -= m;
                        cs.Vertex2.Y -= m;
                        cs.Vertex2.Z -= m;
                    }
                }
                else
                {
                    if (CurrentlySelectedCollisionShape.Radius - CurentCSModifyAmount >= 0)
                    {
                        CurrentlySelectedCollisionShape.Radius -= CurentCSModifyAmount;
                    }
                }
                CurrentModel.CalculateCollisionShapeEdges();
            }
        }
        private void CSBack_PreviewMouseDown(object? sender, MouseEventArgs e)
        {
            float m = CurentCSModifyAmount;
            var c = CurrentlySelectedCollisionShape; if (c == null || !CurrentModel.Nodes.Contains(CurrentlySelectedCollisionShape)) { MessageBox.Show("Select a collision shape"); return; }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    c.Vertex1.X -= m;
                }
            }
            else
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    if (c.Vertex2.X + m > c.Vertex2.X) c.Vertex1.X += m;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void CSFront_PreviewMouseDown(object? sender, MouseButtonEventArgs e)
        {
            float m = CurentCSModifyAmount;
            var c = CurrentlySelectedCollisionShape; if (c == null || !CurrentModel.Nodes.Contains(CurrentlySelectedCollisionShape)) { MessageBox.Show("Select a collision shape"); return; }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    c.Vertex2.X += m;
                }
            }
            else
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    if (c.Vertex2.X - m > c.Vertex1.X) c.Vertex2.X -= m;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void CSBottom_PM(object? sender, MouseButtonEventArgs e)
        {
            float m = CurentCSModifyAmount;
            var c = CurrentlySelectedCollisionShape; if (c == null || !CurrentModel.Nodes.Contains(CurrentlySelectedCollisionShape)) { MessageBox.Show("Select a collision shape"); return; }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    c.Vertex1.Z -= m;
                }
            }
            else
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    if (c.Vertex2.Z + m > c.Vertex2.Z) c.Vertex1.Z += m;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void CSTop_PM(object? sender, MouseButtonEventArgs e)
        {
            float m = CurentCSModifyAmount;
            var c = CurrentlySelectedCollisionShape; if (c == null || !CurrentModel.Nodes.Contains(CurrentlySelectedCollisionShape)) { MessageBox.Show("Select a collision shape"); return; }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    c.Vertex2.Z += m;
                }
            }
            else
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    if (c.Vertex2.Z - m > c.Vertex1.Z) c.Vertex2.Z -= m;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void CSLeft_PM(object? sender, MouseButtonEventArgs e)
        {
            float m = CurentCSModifyAmount;
            var c = CurrentlySelectedCollisionShape; if (c == null || !CurrentModel.Nodes.Contains(CurrentlySelectedCollisionShape)) { MessageBox.Show("Select a collision shape"); return; }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    c.Vertex1.Y -= m;
                }
            }
            else
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    if (c.Vertex2.Y + m > c.Vertex2.Y) c.Vertex1.Y += m;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void CSRight_PM(object? sender, MouseButtonEventArgs e)
        {
            float m = CurentCSModifyAmount;
            var c = CurrentlySelectedCollisionShape; if (c == null || !CurrentModel.Nodes.Contains(CurrentlySelectedCollisionShape)) { MessageBox.Show("Select a collision shape"); return; }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    c.Vertex2.Y += m;
                }
            }
            else
            {
                if (c.Type == ECollisionShapeType.Box)
                {
                    if (c.Vertex2.Y - m > c.Vertex1.Y) c.Vertex2.Y -= m;
                }
            }
            CurrentModel.CalculateCollisionShapeEdges();
        }
        private void Manual_SetX(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            bool v = float.TryParse(InputAnimatorX.Text, out float value);
            if (!v) return;
            ModifyByX(value);


        }
        private void ModifyByX(float value)
        {
            if (modifyMode_current == ModifyMode.Translate)
            {
                if (Tabs_Geosets.SelectedIndex == 3) // rigging
                {
                    if (ListBonesRiggings.SelectedItem == null) return;
                    CBone? bone = getselectedBoneInRigging();
                    if (bone == null) return;
                    bone.PivotPoint.X = value;
                }
                else if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.TranslateGeosetsTo(list, Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.TranslateNodes(selected, Axes.X, value);
                }
                else if (ViewingPaths)
                {
                    TranslateSelectedPath(Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Translation, Axes.X, value);
                }

                else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.translateVertices(Axes.X, vertices, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    var verices = GetSelectedVertices();
                    Calculator.translateVertices(Axes.X, verices, value);

                }
                else if (modifyMode_current == ModifyMode.Rotate)
                {
                    if (Tabs_Geosets.SelectedIndex == 0) // geosets
                    {
                        if (ListGeosets.SelectedItems.Count == 0) return;
                        var list = GetSelectedGeosets();
                        Calculator.RotateGeosetsTogether(list, value, 0, 0);

                    }
                    else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                    {
                        var selected = GetSelectedNodes_NodeEditor();
                        Calculator.RotateNodes(selected, Axes.X, value);
                    }
                    else if (Tabs_Geosets.SelectedIndex == 4) // animator
                    {
                        int track = Animator_GetTrack();
                        var node = animator_GetNode();
                        ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Rotation, Axes.X, value);
                        return;
                    }
                    else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                    {
                        var triangles = getSelectedTriangles();
                        var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                        Calculator.RotateVertices(Axes.X, GetSelectedVertices(), value);

                    }
                    else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                    {
                        Calculator.RotateVertices(Axes.X, GetSelectedVertices(), value);
                    }
                    else if (ViewingPaths)
                    {
                        RotateSelectedPath(Axes.X, value);
                    }

                    ResetManualValues_R();
                }
                else if (modifyMode_current == ModifyMode.Scale)
                {
                    if (Tabs_Geosets.SelectedIndex == 0) // geosets
                    {
                        if (ListGeosets.SelectedItems.Count == 0) return;
                        var list = GetSelectedGeosets();
                        Calculator.ScaleGeosets(list, Axes.X, value);
                        InputAnimatorX.Text = "100";
                    }
                    else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                    {
                        var selected = GetSelectedNodes_NodeEditor();
                        Calculator.ScaleNodes(selected, Axes.X, value);
                    }
                    else if (ViewingPaths)
                    {

                        ScaleSelectedPath(Axes.X, value);
                    }
                    else if (Tabs_Geosets.SelectedIndex == 4) // animator
                    {
                        int track = Animator_GetTrack();
                        var node = animator_GetNode();
                        ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Scaling, Axes.X, value);
                    }
                    else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                    {
                        var triangles = getSelectedTriangles();
                        var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                        Calculator.RotateVertices(Axes.Z, GetSelectedVertices(), value);
                    }
                    else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                    {
                        Calculator.ScaleVertices(Axes.Z, GetSelectedVertices(), value);
                    }
                    ResetManualValues_S();
                }
            }
            else if (modifyMode_current == ModifyMode.Rotate)
            {
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.RotateGeosetsTogether(list, value, 0, 0);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.RotateNodes(selected, Axes.X, value);
                }
                else if (ViewingPaths)
                {
                    RotateSelectedPath(Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Rotation, Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.RotateVertices(Axes.X, GetSelectedVertices(), value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    Calculator.RotateVertices(Axes.X, GetSelectedVertices(), value);
                }
                ResetManualValues_R();
            }
            else if (modifyMode_current == ModifyMode.Scale)
            {
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.ScaleGeosets(list, Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.ScaleNodes(selected, Axes.X, value);
                }
                else if (ViewingPaths)
                {
                    ScaleSelectedPath(Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Scaling, Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //  triangles
                {
                    //unfinished
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    Calculator.ScaleVertices(Axes.X, GetSelectedVertices(), value);
                }
                ResetManualValues_S();
            }
        }
        private void ScaleSelectedPath(Axes x, float value)
        {
            if (ListPaths.SelectedItem != null)
            {
                int index = ListPaths.SelectedIndex;
                var path = PathManager.Paths[index];
                var list = GetSelectedPathNodes(path);
                if (list.Count > 1)
                {
                    float normalized = value / 100;
                    foreach (var node in list)
                    {
                        node.Position.X *= x == Axes.X ? normalized : 1;
                        node.Position.Y *= x == Axes.Y ? normalized : 1;
                        node.Position.Z *= x == Axes.Z ? normalized : 1;
                    }
                }
            }
        }
        private void RotateSelectedPath(Axes x, float value)
        {
            if (ListPaths.SelectedItem != null)
            {
                int index = ListPaths.SelectedIndex;
                var path = PathManager.Paths[index];
                var list = GetSelectedPathNodes(path);
                if (list.Count > 1)
                {
                    Calculator.RotateVectors(path.List.Select(x => x.Position).ToList(), x, value);
                }
            }
            throw new NotImplementedException();
        }
        private void TranslateSelectedPath(Axes x, float value)
        {
            if (ListPaths.SelectedItem != null)
            {
                int index = ListPaths.SelectedIndex;
                var path = PathManager.Paths[index];
                var list = GetSelectedPathNodes(path);
                if (list.Count == 1)
                {
                    {
                        int SelectedPathNodeIndex = ListPathNodes.SelectedIndex;
                        path.List[SelectedPathNodeIndex].Position.X = x == Axes.X ? value : path.List[SelectedPathNodeIndex].Position.X;
                        path.List[SelectedPathNodeIndex].Position.Y = x == Axes.Y ? value : path.List[SelectedPathNodeIndex].Position.Y;
                        path.List[SelectedPathNodeIndex].Position.Z = x == Axes.Z ? value : path.List[SelectedPathNodeIndex].Position.Z;
                    }
                    if (list.Count > 1)
                    {
                        Calculator.TranslateVectors(path.List.Select(x => x.Position).ToList(), x, value, true);
                    }
                }
            }
        }
        private void ModifyAnimatedPositionOfNodeAndVertices(int track, INode? node, TransformationType type, Axes axes, float value, bool increment = false)
        {
            if (node == null) return;
            //  MessageBox.Show($"type {type}, axes {axes}, value {value}");
            if (type == TransformationType.Translation)
            {
                if (node.Translation.Any(x => x.Time == track))
                {
                    var keyf = node.Translation.First(x => x.Time == track);
                    keyf.Value.X = axes == Axes.X ? value : keyf.Value.X;
                    keyf.Value.Y = axes == Axes.Y ? value : keyf.Value.Y;
                    keyf.Value.Z = axes == Axes.Z ? value : keyf.Value.Z;
                }
                else
                {
                    CAnimatorNode<CVector3> keyf = new CAnimatorNode<CVector3>();
                    keyf.Time = (int)track;
                    keyf.Value = new CVector3();
                    keyf.Value.X = axes == Axes.X ? value : keyf.Value.X;
                    keyf.Value.Y = axes == Axes.Y ? value : keyf.Value.Y;
                    keyf.Value.Z = axes == Axes.Z ? value : keyf.Value.Z;
                    node.Translation.Add(keyf);
                    node.Translation.NodeList = node.Translation.NodeList.OrderBy(x => x.Time).ToList();
                    Animator_RefreshKeyframesList();
                }
            }
            else if (type == TransformationType.Rotation)
            {
                var kf = node.Rotation.FirstOrDefault(x => x.Time == track);
                if (kf != null)
                {
                    CVector3 euler = Calculator.QuaternionToEuler(kf.Value);
                    euler.X = axes == Axes.X ? value : euler.X;
                    euler.Y = axes == Axes.Y ? value : euler.Y;
                    euler.Z = axes == Axes.Z ? value : euler.Z;
                    kf.Value = Calculator.EulerToQuaternion(euler);
                    MessageBox.Show(kf.Value.ToString()); // debug
                }
                else
                {
                    CAnimatorNode<CVector4> kf_new = new CAnimatorNode<CVector4>();
                    CVector3 euler = Calculator.QuaternionToEuler(kf_new.Value);
                    euler.X = axes == Axes.X ? value : euler.X;
                    euler.Y = axes == Axes.Y ? value : euler.Y;
                    euler.Z = axes == Axes.Z ? value : euler.Z;
                    kf_new.Value = Calculator.EulerToQuaternion(euler);
                    kf_new.Time = track;
                    node.Rotation.Add(kf_new);
                    node.Rotation.NodeList = node.Rotation.NodeList.OrderBy(x => x.Time).ToList();
                    Animator_RefreshKeyframesList();
                }



            }
            else if (type == TransformationType.Scaling)
            {
                var keyf = node.Scaling.FirstOrDefault(x => x.Time == track);
                if (keyf != null)
                {

                    keyf.Value.X = axes == Axes.X ? value / 100 : keyf.Value.X;
                    keyf.Value.Y = axes == Axes.Y ? value / 100 : keyf.Value.Y;
                    keyf.Value.Z = axes == Axes.Z ? value / 100 : keyf.Value.Z;
                    // MessageBox.Show(keyf.Value.ToString()); // debug
                }
                else
                {
                    CAnimatorNode<CVector3> keyf_new = new CAnimatorNode<CVector3>();
                    keyf_new.Time = track;
                    keyf_new.Value = new CVector3();
                    keyf_new.Value.X = axes == Axes.X ? value / 100 : keyf_new.Value.X;
                    keyf_new.Value.Y = axes == Axes.Y ? value / 100 : keyf_new.Value.Y;
                    keyf_new.Value.Z = axes == Axes.Z ? value / 100 : keyf_new.Value.Z;
                    node.Scaling.Add(keyf_new);
                    node.Scaling.NodeList = node.Scaling.NodeList.OrderBy(x => x.Time).ToList();
                    Animator_RefreshKeyframesList();
                }
            }
            RefreshAnimatedVertexAndNodePositionsForRendering(track);
        }
        private void Animator_RefreshKeyframesList()
        {
            if (List_Sequences_Animator.SelectedItem != null && List_Nodes_Animator.SelectedItem != null)
            {
                CSequence sequence = GetSelectedSequenceAnimator();
                SelectedNodeInAnimator = GetSelectedNodeInanimator(); if (SelectedNodeInAnimator == null) return;
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
                foreach (var ga in CurrentModel.GeosetAnimations)
                {
                    if (ga.Alpha.Static == false && ga.Alpha.Count > 0)
                    {
                        foreach (var kf in ga.Alpha)
                        {
                            if (kf.Time >= sequence.IntervalStart && kf.Time <= sequence.IntervalEnd)
                            {
                                if (tracks.Contains(kf.Time) == false) { tracks.Add(kf.Time); }
                            }
                        }
                    }
                }

                List_Keyframes_Animator.Items.Clear();
                foreach (int track in tracks)
                {
                    string c = GetKeyframeTitle(SelectedNodeInAnimator, track);

                    List_Keyframes_Animator.Items.Add(new ListBoxItem() { Content = c });
                }

                UpdateManualInputForAnimator();



            }
            else
            {
                List_Keyframes_Animator.Items.Clear();
            }
        }
        private INode? animator_GetNode()
        {
            if (List_Nodes_Animator.SelectedItem == null) { return null; }
            return CurrentModel.Nodes[List_Nodes_Animator.SelectedIndex];
        }
        private void Manual_SetY(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            bool v = float.TryParse(InputAnimatorY.Text, out float value);
            if (!v) return;
            ModifyByY(value);


        }
        private void ModifyByY(float value)
        {
            if (modifyMode_current == ModifyMode.Translate)
            {
                if (Tabs_Geosets.SelectedIndex == 3) // rigging
                {
                    if (ListBonesRiggings.SelectedItem == null) return;
                    CBone? bone = getselectedBoneInRigging();
                    if (bone == null) return;
                    bone.PivotPoint.Y = value;
                }
                else if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.TranslateGeosetsTo(list, Axes.Y, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.TranslateNodes(selected, Axes.Y, value);
                }
                else if (ViewingPaths)
                {
                    TranslateSelectedPath(Axes.Y, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Translation, Axes.Y, value);
                    return;
                }
                else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.translateVertices(Axes.Y, vertices, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    var verices = GetSelectedVertices();
                    Calculator.translateVertices(Axes.Y, verices, value);
                }

            }
            else if (modifyMode_current == ModifyMode.Rotate)
            {
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.RotateGeosetsTogether(list, 0, value, 0);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.RotateNodes(selected, Axes.Y, value);
                }
                else if (ViewingPaths)
                {
                    RotateSelectedPath(Axes.Y, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Rotation, Axes.X, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.translateVertices(Axes.Y, vertices, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    Calculator.RotateVertices(Axes.Y, GetSelectedVertices(), value);
                }
                ResetManualValues_R();
            }
            else if (modifyMode_current == ModifyMode.Scale)
            {
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.ScaleGeosets(list, Axes.Y, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.ScaleNodes(selected, Axes.Y, value);
                }
                else if (ViewingPaths)
                {
                    ScaleSelectedPath(Axes.Y, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    INode? node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Scaling, Axes.Y, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //  triangles
                {
                    //unfinished
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    Calculator.ScaleVertices(Axes.Y, GetSelectedVertices(), value);
                }
                ResetManualValues_S();
            }
        }
        private void Manual_SetZ(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            bool v = float.TryParse(InputAnimatorZ.Text, out float value);
            if (!v) return;
            ModifyByZ(value);
        }
        private void ModifyByZ(float value)
        {
            if (modifyMode_current == ModifyMode.Translate)
            {
                if (Tabs_Geosets.SelectedIndex == 3) // rigging
                {
                    if (ListBonesRiggings.SelectedItem == null) return;
                    CBone? bone = getselectedBoneInRigging();
                    if (bone == null) return;
                    bone.PivotPoint.Z = value;
                }
                else if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.TranslateGeosetsTo(list, Axes.Z, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.TranslateNodes(selected, Axes.Z, value);
                }
                else if (ViewingPaths)
                {
                    TranslateSelectedPath(Axes.Z, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Translation, Axes.Z, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //  triangles
                {
                    //unfinished
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    var verices = GetSelectedVertices();
                    Calculator.translateVertices(Axes.Z, verices, value);
                }
            }
            else if (modifyMode_current == ModifyMode.Rotate)
            {
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.RotateGeosetsTogether(list, 0, 0, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.RotateNodes(selected, Axes.Z, value);
                }
                else if (ViewingPaths)
                {
                    RotateSelectedPath(Axes.Z, value); return;
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Rotation, Axes.Z, value);
                    return;
                }
                else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.translateVertices(Axes.Z, vertices, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    Calculator.RotateVertices(Axes.Z, GetSelectedVertices(), value);
                }
                ResetManualValues_R();
            }
            else if (modifyMode_current == ModifyMode.Scale)
            {
                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    if (ListGeosets.SelectedItems.Count == 0) return;
                    var list = GetSelectedGeosets();
                    Calculator.ScaleGeosets(list, Axes.Z, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var selected = GetSelectedNodes_NodeEditor();
                    Calculator.ScaleNodes(selected, Axes.Z, value);
                }
                else if (ViewingPaths)
                {
                    ScaleSelectedPath(Axes.Z, value);
                }
                else if (Tabs_Geosets.SelectedIndex == 4) // animator
                {
                    int track = Animator_GetTrack();
                    var node = animator_GetNode();
                    ModifyAnimatedPositionOfNodeAndVertices(track, node, TransformationType.Scaling, Axes.Z, value);
                    return;
                }
                else if (Tabs_Geosets.SelectedIndex == 1) //  triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.ScaleVertices(Axes.Z, GetSelectedVertices(), value);
                }
                else if (Tabs_Geosets.SelectedIndex == 2) //   verticess
                {
                    Calculator.ScaleVertices(Axes.Z, GetSelectedVertices(), value);
                }
                ResetManualValues_S();
            }
        }
        private void ResetManualValues_R()
        {
            InputAnimatorX.Text = "0";
            InputAnimatorY.Text = "0";
            InputAnimatorZ.Text = "0";
        }
        private void ResetManualValues_S()
        {
            InputAnimatorX.Text = "100";
            InputAnimatorY.Text = "100";
            InputAnimatorZ.Text = "100";
        }
        private void MergeBones(object? sender, RoutedEventArgs? e)
        {
            List<string> list = GetMergeableBones();
            if (list.Count <= 1)
            {
                MessageBox.Show("There must be at least 2 childless bones in the root for this command"); return;
            }
            Multiselector_Window mw = new Multiselector_Window(list);
            if (mw.ShowDialog() == true)
            {
                var names = mw.selected;
                if (names.Count < 2) { MessageBox.Show("At least 2 bones must be selected"); return; }
                var generatedName = $"GeneratedMergedBone_{IDCounter.Next_}";
                CBone GeneratedBone = new CBone(CurrentModel);
                GeneratedBone.Name = generatedName;
                var first = CurrentModel.Nodes.First(x => x.Name == names[0]);
                GeneratedBone.Translation.NodeList = first.Translation.NodeList;
                GeneratedBone.Rotation.NodeList = first.Rotation.NodeList;
                GeneratedBone.Scaling.NodeList = first.Scaling.NodeList;
                for (int i = 1; i < names.Count; i++)
                {
                    var loopedNode = CurrentModel.Nodes.First(x => x.Name == names[i]);
                    ReattachAllVerticesOfBoneToBone(loopedNode, GeneratedBone);
                    CurrentModel.Nodes.Remove(loopedNode);
                }
                CurrentModel.Nodes.Remove(first);
                CurrentModel.Nodes.Add(GeneratedBone);
                RefreshNodesTree();
            }
        }
        private void ReattachAllVerticesOfBoneToBone(INode TargetNode, CBone ToBone)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    if (vertex.Group == null)
                    {
                        MessageBox.Show($"Null group for vertex {vertex.ObjectId} of geoset {geoset.ObjectId}");
                        continue;
                    }
                    if (vertex.Group.Object == null)
                    {
                        MessageBox.Show($"Null group object for vertex {vertex.ObjectId} of geoset {geoset.ObjectId}");
                        continue;
                    }
                    if (vertex.Group == null)
                    {
                        MessageBox.Show($"Null group for vertex {vertex.ObjectId} of geoset {geoset.ObjectId}");
                        continue;
                    }
                    if (geoset.Groups.Contains(vertex.Group.Object) == false)
                    {
                        MessageBox.Show($"Geoset does not contain group for  {vertex.ObjectId} of geoset {geoset.ObjectId}");
                        continue;
                    }
                    foreach (var gnode in vertex.Group.Object.Nodes)
                    {
                        if (gnode.Node.Node == TargetNode)
                        {
                            gnode.Node.Detach();
                            gnode.Node.Attach(ToBone);
                        }
                    }
                    /*
                    if (vertex.Group.Object == null )
                    {
                        CGeosetGroup group_new = new CGeosetGroup(CurrentModel);
                        CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                        gnode.Node.Attach(ToBone);
                        group_new.Nodes.Add(gnode);
                        geoset.Groups.Add(group_new);
                        vertex.Group.Attach(group_new);
                    }
                    else
                    {
                    }
                    */
                }
            }
        }

        private List<string> GetMergeableBones()
        {
            List<string> list = new();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node is CBone)
                {
                    if (NodeHasChildren(node) == false) { list.Add(node.Name); }
                }
            }
            return list;
        }
        private bool NodeHasChildren(INode node)
        {
            return CurrentModel.Nodes.Any(x => x.Parent.Node == node);
        }
        private List<INode> SelectedNodesInNodeEditor = new List<INode>();
        private bool InAnimator = false;
        private void SelectedNodeInNodeEditor(object? sender, SelectionChangedEventArgs? e)
        {
            SelectedNodesInNodeEditor.Clear();
            if (list_Node_Editor.Items.Count == 0 || CurrentModel.Nodes.Count < list_Node_Editor.Items.Count) { return; }

            var selectedItemsSet = new HashSet<object>(list_Node_Editor.SelectedItems.Cast<object>());

            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (selectedItemsSet.Contains(list_Node_Editor.Items[i]))
                {
                    CurrentModel.Nodes[i].IsSelected = true;
                    SelectedNodesInNodeEditor.Add(CurrentModel.Nodes[i]);
                }
                else
                {
                    CurrentModel.Nodes[i].IsSelected = false;
                }
            }



            if (SelectedNodesInNodeEditor.Count > 0)
            {
                var centroid = Calculator.GetCentroidOfVectors(SelectedNodesInNodeEditor.Select(x => x.PivotPoint).ToList());
                InputAnimatorX.Text = centroid.X.ToString();
                InputAnimatorY.Text = centroid.Y.ToString();
                InputAnimatorZ.Text = centroid.Z.ToString();
            }
            else
            {
                InputAnimatorX.Text = string.Empty;
                InputAnimatorY.Text = string.Empty;
                InputAnimatorZ.Text = string.Empty;
            }
        }

        private void NodeList_SelectAll(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.SelectAll();
        }
        private void NodeList_SelectN(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.SelectedItems.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                node.IsSelected = false;
            }
        }
        private void NodeList_SelectI(object? sender, RoutedEventArgs? e)
        {
            foreach (var item in list_Node_Editor.Items)
            {
                if (list_Node_Editor.SelectedItems.Contains(item))
                {
                    list_Node_Editor.SelectedItems.Remove(item);
                }
                else
                {
                    list_Node_Editor.SelectedItems.Add(item);
                }
            }
        }
        private void NodeEditor_AlignX(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                for (int i = 1; i < SelectedNodesInNodeEditor.Count; i++)
                {
                    SelectedNodesInNodeEditor[i].PivotPoint.X = SelectedNodesInNodeEditor[0].PivotPoint.X;
                }
            }
        }
        private void NodeEditor_AlignY(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                for (int i = 1; i < SelectedNodesInNodeEditor.Count; i++)
                {
                    SelectedNodesInNodeEditor[i].PivotPoint.Y = SelectedNodesInNodeEditor[0].PivotPoint.Y;
                }
            }
        }
        private void NodeEditor_AlignZ(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                for (int i = 1; i < SelectedNodesInNodeEditor.Count; i++)
                {
                    SelectedNodesInNodeEditor[i].PivotPoint.Z = SelectedNodesInNodeEditor[0].PivotPoint.Z;
                }
            }
        }
        private void NodeEditor_SwapLocs(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count == 2)
            {
                var temp = new CVector3(SelectedNodesInNodeEditor[0].PivotPoint);
                SelectedNodesInNodeEditor[0].PivotPoint = new CVector3(SelectedNodesInNodeEditor[1].PivotPoint);
                SelectedNodesInNodeEditor[1].PivotPoint = new CVector3(temp);
            }
            else
            {
                MessageBox.Show("Select exactly two nodes");
            }
        }
        private void NodeEditor_NegateX(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 0)
            {
                foreach (var node in SelectedNodesInNodeEditor)
                {
                    node.PivotPoint.X = -node.PivotPoint.X;
                }
            }
        }
        public class FileRenamer
        {
            public static string RenameFile(string filename, string newname)
            {
                if (!File.Exists(filename))
                    throw new FileNotFoundException("The file does not exist.", filename);
                string? directory = System.IO.Path.GetDirectoryName(filename) ?? throw new FileNotFoundException("The file does not exist.", filename);
                string? extension = System.IO.Path.GetExtension(filename) ?? throw new FileNotFoundException("The file does not exist.", filename);
                string? newFilePath = System.IO.Path.Combine(directory, newname + extension);
                File.Move(filename, newFilePath);
                return newFilePath;
            }
        }
        private void NodeEditor_NegateY(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 0)
            {
                foreach (var node in SelectedNodesInNodeEditor)
                {
                    node.PivotPoint.Y = -node.PivotPoint.Y;
                }
            }
        }
        private void NodeEditor_NegateZ(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 0)
            {
                foreach (var node in SelectedNodesInNodeEditor)
                {
                    node.PivotPoint.Z = -node.PivotPoint.Z;
                }
            }
        }
        private void NodeEditor_MirrorX(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                Calculator.MirrorNodePositions(SelectedNodesInNodeEditor, Axes.X);
            }
        }
        private void NodeEditor_MirrorY(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                Calculator.MirrorNodePositions(SelectedNodesInNodeEditor, Axes.Y);
            }
        }
        private void NodeEditor_MirrorZ(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                Calculator.MirrorNodePositions(SelectedNodesInNodeEditor, Axes.Z);
            }
        }
        private void NodeEditor_Disperce(object? sender, RoutedEventArgs? e)
        {
            if (SelectedNodesInNodeEditor.Count > 1)
            {
                Calculator.DisperceVectors(SelectedNodesInNodeEditor.Select(x => x.PivotPoint).ToList());
            }
        }
        private void removeAllgse(object? sender, RoutedEventArgs? e)
        {
            foreach (var g in CurrentModel.Geosets) g.Extents.Clear();
        }
        private void SetFullScreenMode(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            MaximizeOnStart = Menuitem_Maximize.IsChecked == true;
            SaveSettings();
        }
        private void MergeSimilarBones(object? sender, RoutedEventArgs? e)
        {
            // collect all bone groups with identical transfomrations from the root
            List<INode> Bones = CurrentModel.Nodes.Where(x => hasParent(x) == false && x is CBone).ToList();
            if (Bones.Count < 2) { MessageBox.Show("Not enough bones"); return; }
            List<List<INode>> Groups = CollectIdenticalBoneGroups(Bones);
            // MessageBox.Show(Groups.Count.ToString());
            foreach (var group in Groups)
            {
                CBone GeneratedBone = new CBone(CurrentModel);
                GeneratedBone.Name = $"GeneratedMergedBone_{IDCounter.Next_}";
                foreach (var GroupedNode in group)
                {
                    ReattachAllVerticesOfBoneToBone(GroupedNode, GeneratedBone);
                }
                GeneratedBone.Translation.NodeList = group[0].Translation.NodeList;
                GeneratedBone.Rotation.NodeList = group[0].Rotation.NodeList;
                GeneratedBone.Scaling.NodeList = group[0].Scaling.NodeList;
                CurrentModel.Nodes.Add(GeneratedBone);
            }
            foreach (var group in Groups)
            {
                foreach (var GroupedNode in group)
                {
                    CurrentModel.Nodes.Remove(GroupedNode);
                }
            }
            RefreshNodesTree();
        }

        private static List<List<INode>> CollectIdenticalBoneGroups(List<INode> bones)
        {
            var Copy = bones;
            List<List<INode>> BigList = new List<List<INode>>();
            // Collect all bones with no transformations
            List<INode> Zeroes = Copy.Where(b => b.Translation.Count == 0 &&
                                                  b.Rotation.Count == 0 &&
                                                  b.Scaling.Count == 0).ToList();
            if (Zeroes.Count > 0)
            {
                BigList.Add(Zeroes);
                Copy = Copy.Except(Zeroes).ToList(); // Remove zero-transformation bones
            }
            if (Copy.Count < 2) return BigList;
            HashSet<INode> visited = new HashSet<INode>();
            // Iterate through remaining bones and group them based on identical transformations
            foreach (var bone in Copy)
            {
                if (visited.Contains(bone)) continue;
                List<INode> group = new List<INode> { bone };
                visited.Add(bone);
                foreach (var otherBone in Copy)
                {
                    if (bone == otherBone || visited.Contains(otherBone)) continue;
                    if (AreNodeTransformationsIdentical(bone, otherBone))
                    {
                        group.Add(otherBone);
                        visited.Add(otherBone);
                    }
                }
                if (group.Count > 1) BigList.Add(group);
            }
            return BigList;
        }
        private static bool AreNodeTransformationsIdentical(INode node1, INode node2)
        {
            if (node1.Translation.Count > 0 && node1.Translation.Count == node2.Translation.Count)
            {
                for (int i = 0; i < node1.Translation.Count; i++)
                {
                    if (node1.Translation[i].Value != node2.Translation[i].Value) { return false; }
                }
            }
            if (node1.Rotation.Count > 0 && node1.Rotation.Count == node2.Rotation.Count)
            {
                for (int i = 0; i < node1.Rotation.Count; i++)
                {
                    if (node1.Rotation[i].Value != node2.Rotation[i].Value) { return false; }
                }
            }
            if (node1.Scaling.Count > 0 && node1.Scaling.Count == node2.Scaling.Count)
            {
                for (int i = 0; i < node1.Scaling.Count; i++)
                {
                    if (node1.Scaling[i].Value != node2.Scaling[i].Value) { return false; }
                }
            }
            return true;
        }
        private void ebti(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) return;
            INode? node = GetSelectedNode(); if (node == null) return;
            InputVector iv = new InputVector(AllowedValue.Both, "Percentage");
            iv.x.Text = node.WeightScaling.X.ToString();
            iv.y.Text = node.WeightScaling.Y.ToString();
            iv.z.Text = node.WeightScaling.Z.ToString();
            if (iv.ShowDialog() == true)
            {
                float x = iv.X;
                float y = iv.Y;
                float z = iv.Z;
                if (x > 100 || x < -100 || y < -100 || y > 100 || z < -100 || z > 100)
                {
                    MessageBox.Show("Only -100 to 100 values are accepted"); return;
                }
                node.WeightTranslation = new CVector3(x, y, z);
            }
        }
        private void ebtr(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) return;
            INode? node = GetSelectedNode(); if (node == null) return;
            InputVector iv = new InputVector(AllowedValue.Both, "Percentage");
            iv.x.Text = node.WeightRotation.X.ToString();
            iv.y.Text = node.WeightRotation.Y.ToString();
            iv.z.Text = node.WeightRotation.Z.ToString();
            if (iv.ShowDialog() == true)
            {
                float x = iv.X;
                float y = iv.Y;
                float z = iv.Z;
                if (x > 100 || x < -100 || y < -100 || y > 100 || z < -100 || z > 100)
                {
                    MessageBox.Show("Only -100 to 100 values are accepted"); return;
                }
                node.WeightRotation = new CVector3(x, y, z);
            }
        }
        private void ebts(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) return;
            INode? node = GetSelectedNode(); if (node == null) return;
            InputVector iv = new InputVector(AllowedValue.Both, "Percentage");
            iv.x.Text = node.WeightScaling.X.ToString();
            iv.y.Text = node.WeightScaling.Y.ToString();
            iv.z.Text = node.WeightScaling.Z.ToString();
            if (iv.ShowDialog() == true)
            {
                float x = iv.X;
                float y = iv.Y;
                float z = iv.Z;
                if (x > 100 || x < -100 || y < -100 || y > 100 || z < -100 || z > 100)
                {
                    MessageBox.Show("Only -100 to 100 values are accepted"); return;
                }
                node.WeightScaling = new CVector3(x, y, z);
            }
        }
        //------------------------------------------------------
        //--ANIMATOR FUNCTIONS START
        //------------------------------------------------------
        private void SetAllAnimatedPositionsToDefault()
        {
            foreach (var node in CurrentModel.Nodes) node.AnimatedPivotPoint = new CVector3(node.PivotPoint);
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    vertex.AnimatedPosition = new CVector3(vertex.Position);
                }
            }
        }
        /// <summary>
        /// Inside the animator, nodes and vertices are rendered based on their animated position instead of position. This function properly calculates them for the given track number;
        /// </summary>
        /// <param name="WhichKeyframe"></param>
        public void RefreshAnimatedVertexAndNodePositionsForRendering(int WhichKeyframe = -1)
        {

            int value = WhichKeyframe == -1 ? GetCurrentnpuTrack() : WhichKeyframe;
            if (value < 0) return;
            foreach (var node in CurrentModel.Nodes)
            {
                Animator.ComputeNodePosition(node, CurrentModel, WhichKeyframe);
            }
            Animator.ComputeAnimatedVertexPositions(CurrentModel, WhichKeyframe);
            Animator.ComputeGeosetVisibilities(CurrentModel, WhichKeyframe);
            Animator.ComputeNodeVisibilities(CurrentModel, WhichKeyframe);
        }
        private int GetCurrentnpuTrack()
        {
            bool v = int.TryParse(InputCurrentFrame.Text, out int val);
            if (v) { return val; }
            return -2;
        }
        private void setAxsModeX(object? sender, RoutedEventArgs? e)
        {
            axisMode = Axes.X;
            button_X.BorderBrush = Brushes.Green;
            button_Y.BorderBrush = Brushes.Gray;
            button_Z.BorderBrush = Brushes.Gray;
            button_U.BorderBrush = Brushes.Gray;
        }
        private void setAxsModeY(object? sender, RoutedEventArgs? e)
        {
            axisMode = Axes.Y;
            button_X.BorderBrush = Brushes.Gray;
            button_Y.BorderBrush = Brushes.Green;
            button_Z.BorderBrush = Brushes.Gray;
            button_U.BorderBrush = Brushes.Gray;
        }
        private void setAxsModeZ(object? sender, RoutedEventArgs? e)
        {
            axisMode = Axes.Z;
            button_X.BorderBrush = Brushes.Gray;
            button_Y.BorderBrush = Brushes.Gray;
            button_Z.BorderBrush = Brushes.Green;
            button_U.BorderBrush = Brushes.Gray;
        }
        private void setAxsModeU(object? sender, RoutedEventArgs? e)
        {
            axisMode = Axes.U;
            button_X.BorderBrush = Brushes.Gray;
            button_Y.BorderBrush = Brushes.Gray;
            button_Z.BorderBrush = Brushes.Gray;
            button_U.BorderBrush = Brushes.Green;
        }
        //------------------------------------------------------
        //--ANIMATOR FUNCTIONS END
        //------------------------------------------------------
        private void Disperse(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            Calculator.DisperceVectors(Vertices.Select(X => X.Position).ToList());
            SetSaved(false);
        }
        private void flattenVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count < 2) { MessageBox.Show("Select at least 2 vertices"); return; }
            axis_picker ap = new axis_picker("Axes?");
            if (ap.ShowDialog() == true)
            {
                var ax = ap.axis;
                for (int i = 1; i < Vertices.Count; i++)
                {
                    if (ax == Axes.X)
                    {
                        Vertices[i].Position.X = Vertices[0].Position.X;
                    }
                    if (ax == Axes.Y)
                    {
                        Vertices[i].Position.Y = Vertices[0].Position.Y;
                    }
                    if (ax == Axes.Z)
                    {
                        Vertices[i].Position.Z = Vertices[0].Position.Z;
                    }
                }
                SetSaved(false);
            }
        }
        private void CollapseVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> vertices = GetSelectedVertices();
            if (vertices.Count < 2)
            {
                MessageBox.Show("Select at least 2 vertices");
                return;
            }

            Dictionary<CGeoset, List<CGeosetVertex>> reference = new Dictionary<CGeoset, List<CGeosetVertex>>();

            // Group vertices by geoset
            foreach (var vertex in vertices)
            {
                foreach (var geoset in CurrentModel.Geosets)
                {
                    if (geoset.Vertices.Contains(vertex))
                    {
                        if (!reference.ContainsKey(geoset))
                            reference[geoset] = new List<CGeosetVertex>();
                        reference[geoset].Add(vertex);
                        break; // One vertex should belong to only one geoset
                    }
                }
            }

            foreach (var pair in reference)
            {
                if (pair.Value.Count <= 1) continue;

                var first = pair.Value.First();
                var redundantVertices = pair.Value.Skip(1).ToList();

                foreach (var triangle in pair.Key.Triangles)
                {
                    if (redundantVertices.Contains(triangle.Vertex1.Object))
                        triangle.Vertex1.Attach(first);
                    if (redundantVertices.Contains(triangle.Vertex2.Object))
                        triangle.Vertex2.Attach(first);
                    if (redundantVertices.Contains(triangle.Vertex3.Object))
                        triangle.Vertex3.Attach(first);
                }

                foreach (var v in redundantVertices)
                    pair.Key.Vertices.Remove(v);
            }
            UpdateSelectedVerticesInfo();
            SetSaved(false);
            RefreshRenderData(null, null); RefreshGeosets_Vertices();
        }

        private void floorVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            foreach (var vertex in Vertices)
            {
                vertex.Position.Floor();
            }
            SetSaved(false);
        }
        private void CelingVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            foreach (var vertex in Vertices)
            {
                vertex.Position.Ceiling();
            }
            SetSaved(false);
        }
        private void swapTrianglePositions(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetTriangle> triangles = getSelectedTriangles();
            if (triangles.Count != 2) { MessageBox.Show("Select eactly 2 triangles"); return; }

            Calculator.SwapTwoTriangles(triangles[0], triangles[1]);
            SetSaved(false);
        }
        private List<CGeosetTriangle> getSelectedTriangles()
        {
            List<CGeosetTriangle> list = new List<CGeosetTriangle>();
            foreach (var g in CurrentModel.Geosets)
            {
                list.AddRange(g.Triangles.ObjectList.Where(x => x.isSelected));
            }
            return list;
        }
        private void splitVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            foreach (var vertex in Vertices)
            {
                SplitVertex(vertex);
            }
        }
        private CGeoset? GetGeosetOfVertex(CGeosetVertex vertex)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                if (geoset.Vertices.Contains(vertex)) return geoset;
            }
            return null;
        }
        private void SplitVertex(CGeosetVertex v)
        {


            var geoset = GetGeosetOfVertex(v);
            if (geoset == null) return;
            var MatchedTriangles = geoset.Triangles.Where(x => x.Vertex1.Object == v || x.Vertex2.Object == v || x.Vertex3.Object == v).ToList();

            if (MatchedTriangles.Count > 1)
            {
                foreach (var tr in MatchedTriangles)
                {
                    if (tr.Vertex1.Object == v)
                    {
                        var vx = tr.Vertex1.Object;
                        CGeosetVertex _new = new CGeosetVertex(CurrentModel);
                        _new.Position = new CVector3(tr.Vertex1.Object.Position);
                        _new.TexturePosition = new CVector2(tr.Vertex1.Object.TexturePosition);
                        _new.Normal = new CVector3(tr.Vertex1.Object.Normal);
                        geoset.Vertices.Add(_new);
                        tr.Vertex1.Attach(_new);

                    }
                    if (tr.Vertex2.Object == v)
                    {
                        var vx = tr.Vertex2.Object;
                        CGeosetVertex _new = new CGeosetVertex(CurrentModel);
                        _new.Position = new CVector3(tr.Vertex2.Object.Position);
                        _new.TexturePosition = new CVector2(tr.Vertex2.Object.TexturePosition);
                        _new.Normal = new CVector3(tr.Vertex2.Object.Normal);
                        geoset.Vertices.Add(_new);
                        tr.Vertex1.Attach(_new);

                    }
                    if (tr.Vertex3.Object == v)
                    {
                        var vx = tr.Vertex3.Object;
                        CGeosetVertex _new = new CGeosetVertex(CurrentModel);
                        _new.Position = new CVector3(tr.Vertex3.Object.Position);
                        _new.TexturePosition = new CVector2(tr.Vertex3.Object.TexturePosition);
                        _new.Normal = new CVector3(tr.Vertex3.Object.Normal);
                        geoset.Vertices.Add(_new);
                        tr.Vertex1.Attach(_new);

                    }
                }
                geoset.Vertices.Remove(v);
            }


        }
        private CVector3 CopiedVertexPosition = new CVector3();
        private CVector3 CopiedNormalPosition = new CVector3();
        private CVector2 CopiedUVPosition = new CVector2();
        private void CopyVertexPos(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 1) { MessageBox.Show("Select exactly 1 vertex"); return; }
            CopiedVertexPosition = new CVector3(Vertices[0].Position);
        }
        private void pasteVertexPos(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 1) { MessageBox.Show("Select exactly 1 vertex"); return; }
            Vertices[0].Position = new CVector3(CopiedVertexPosition);
            SetSaved(false);
        }
        private void SwapVerticesPos(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 2) { MessageBox.Show("Select exactly 2 vertices"); return; }
            CVector3 temp = new CVector3(Vertices[0].Position);
            Vertices[0].Position = new CVector3(Vertices[1].Position);
            Vertices[1].Position = new CVector3(temp);
        }
        private void copynormal(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 1) { MessageBox.Show("Select exactly 1 vertex"); return; }
            CopiedNormalPosition = new CVector3(Vertices[0].Normal);
        }
        private void pastenormal(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 1) { MessageBox.Show("Select exactly 1 vertex"); return; }
            Vertices[0].Normal = new CVector3(CopiedNormalPosition);
            SetSaved(false);
        }
        private void copyuvv(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 1) { MessageBox.Show("Select exactly 1 vertex"); return; }
            CopiedUVPosition = new CVector2(Vertices[0].TexturePosition);
        }
        private void pasteuvv(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 1) { MessageBox.Show("Select exactly 1 vertex"); return; }
            Vertices[0].TexturePosition = new CVector2(CopiedUVPosition);
        }
        private void putongroundvertices(object? sender, RoutedEventArgs? e)
        {
            if (Tabs_Geosets.SelectedIndex == 0)
            {
                var Vertices = GetSelectedGeosets().SelectMany(x => x.Vertices.ObjectList).ToList();
                Calculator.PutOnGround(Vertices);
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                var Vertices = getSelectedTriangles().SelectMany(x => x.Vertices).Distinct().ToList();
                Calculator.PutOnGround(Vertices);
            }
            else if (Tabs_Geosets.SelectedIndex == 0) // vertices
            {
                List<CGeosetVertex> Vertices = GetSelectedVertices();
                Calculator.PutOnGround(Vertices);
            }

        }
        private void cernterverticesAtNode(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count == 0) return;
            if (CurrentModel.Nodes.Count == 0) { MessageBox.Show("There are no nodes"); return; }
            List<string> nodes = CurrentModel.Nodes.Select(x => x.Name).ToList();
            Selector s = new Selector(nodes, "Node");
            if (s.ShowDialog() == true)
            {
                int index = s.box.SelectedIndex;
                var node = CurrentModel.Nodes[index];
                Calculator.CenterVerticesAtVector(node.PivotPoint, Vertices);
            }
        }
        private void mirrorvertices(object? sender, RoutedEventArgs? e)
        {
            //unfinished
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count < 2) { MessageBox.Show("Select at least 2 vertices"); return; }
            axis_picker ap = new axis_picker("Axes?");
            if (ap.ShowDialog() == true)
            {
                var ax = ap.axis;
                Calculator.MirrorVertices(ax, Vertices);
            }
            SetSaved(false);
        }
        private void weldVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count != 2) { MessageBox.Show("Select exactly 2 vertices"); return; }
            var owner = VerticesBelongToSameGeoset(Vertices);
            if (owner == null) { MessageBox.Show("The selected vertces don't belong to the same geoset"); return; }
            var centroid = Calculator.GetCentroidOfVertices(Vertices);
            Vertices[0].Position = new CVector3(centroid);
            ReattachVertexWithVertex(Vertices[1], Vertices[0], owner);
            owner.Vertices.Remove(Vertices[1]);
            SetSaved(false);
            RefreshRenderData(null, null); RefreshGeosets_Vertices();
        }

        private static void ReattachVertexWithVertex(CGeosetVertex target, CGeosetVertex with, CGeoset owner)
        {
            foreach (var trangle in owner.Triangles)
            {
                if (trangle.Vertex1.Object == target) trangle.Vertex1.Attach(with);
                if (trangle.Vertex2.Object == target) trangle.Vertex2.Attach(with);
                if (trangle.Vertex3.Object == target) trangle.Vertex3.Attach(with);
            }
        }

        private void negateVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count < 0) { MessageBox.Show("Select at least 1 vertex"); return; }
            axis_picker ap = new axis_picker("Axes?");
            if (ap.ShowDialog() == true)
            {
                var ax = ap.axis;
                for (int i = 0; i < Vertices.Count; i++)
                {
                    if (ax == Axes.X)
                    {
                        Vertices[i].Position.X = -Vertices[i].Position.X;
                    }
                    if (ax == Axes.Y)
                    {
                        Vertices[i].Position.Y = -Vertices[i].Position.Y;
                    }
                    if (ax == Axes.Z)
                    {
                        Vertices[i].Position.Z = -Vertices[i].Position.Z;
                    }
                }
                SetSaved(false);
            }

        }
        private CGeoset? VerticesBelongToSameGeoset(List<CGeosetVertex> vertices)
        {

            foreach (var geoset in CurrentModel.Geosets)
            {

                foreach (var vertex in vertices)
                {
                    if (geoset.Vertices.Contains(vertex) == false) return null;

                }
                return geoset;
            }
            return null;
        }
        private void createTriangleinVertices(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            if (Vertices.Count < 3 || Vertices.Count > 4)
            {
                MessageBox.Show("Select exactly 3 or 4 vertices");
                return;
            }

            CGeoset? owner = VerticesBelongToSameGeoset(Vertices);
            if (owner == null)
            {
                MessageBox.Show("All selected vertices must belong to the same geoset");
                return;
            }

            if (Vertices.Count == 3)
            {
                CGeosetVertex v0 = Vertices[0];
                CGeosetVertex v1 = Vertices[1];
                CGeosetVertex v2 = Vertices[2];
                bool triangle1Exists = owner.Triangles.Any(t =>
                  (t.Vertex1.Object == v0 && t.Vertex2.Object == v1 && t.Vertex3.Object == v2) ||
                  (t.Vertex1.Object == v1 && t.Vertex2.Object == v2 && t.Vertex3.Object == v0) ||
                  (t.Vertex1.Object == v0 && t.Vertex2.Object == v2 && t.Vertex3.Object == v1)

                  );
                if (triangle1Exists)
                {
                    MessageBox.Show("These vertices already form a trangle"); return;
                }


                CGeosetTriangle triangle = new CGeosetTriangle(CurrentModel);
                triangle.Vertex1.Attach(Vertices[0]);
                triangle.Vertex2.Attach(Vertices[1]);
                triangle.Vertex3.Attach(Vertices[2]);
                owner.Triangles.Add(triangle);
            }
            else if (Vertices.Count == 4)
            {
                CGeosetVertex v0 = Vertices[0];
                CGeosetVertex v1 = Vertices[1];
                CGeosetVertex v2 = Vertices[2];
                CGeosetVertex v3 = Vertices[3];

                // Check if two triangles already exist forming a quad
                bool triangle1Exists = owner.Triangles.Any(t =>
                    (t.Vertex1.Object == v0 && t.Vertex2.Object == v1 && t.Vertex3.Object == v2) ||
                    (t.Vertex1.Object == v1 && t.Vertex2.Object == v2 && t.Vertex3.Object == v3) ||
                    (t.Vertex1.Object == v2 && t.Vertex2.Object == v3 && t.Vertex3.Object == v0) ||
                    (t.Vertex1.Object == v3 && t.Vertex2.Object == v0 && t.Vertex3.Object == v1));

                bool triangle2Exists = owner.Triangles.Any(t =>
                    (t.Vertex1.Object == v0 && t.Vertex2.Object == v2 && t.Vertex3.Object == v3) ||
                    (t.Vertex1.Object == v1 && t.Vertex2.Object == v3 && t.Vertex3.Object == v0) ||
                    (t.Vertex1.Object == v2 && t.Vertex2.Object == v0 && t.Vertex3.Object == v1) ||
                    (t.Vertex1.Object == v3 && t.Vertex2.Object == v1 && t.Vertex3.Object == v2));

                if (triangle1Exists && triangle2Exists)
                {
                    MessageBox.Show("The selected vertices already form a quadrilateral.");
                    return;
                }

                // Check if one of the two triangles exists and complete the quad
                CGeosetTriangle? existingTriangle = owner.Triangles.FirstOrDefault(t =>
                    (t.Vertex1.Object == v0 && t.Vertex2.Object == v1 && t.Vertex3.Object == v2) ||
                    (t.Vertex1.Object == v0 && t.Vertex2.Object == v2 && t.Vertex3.Object == v3) ||
                    (t.Vertex1.Object == v1 && t.Vertex2.Object == v2 && t.Vertex3.Object == v3) ||
                    (t.Vertex1.Object == v1 && t.Vertex2.Object == v3 && t.Vertex3.Object == v0));

                if (existingTriangle != null)
                {
                    // Complete the quad by adding the missing triangle
                    CGeosetTriangle newTriangle = new CGeosetTriangle(CurrentModel);
                    newTriangle.Vertex1.Attach(existingTriangle.Vertex1.Object);
                    newTriangle.Vertex2.Attach(existingTriangle.Vertex2.Object);
                    newTriangle.Vertex3.Attach(v3); // Add the missing vertex
                    owner.Triangles.Add(newTriangle);
                }
                else
                {
                    // No existing triangle, split the quad by defaulting to a diagonal
                    CGeosetTriangle triangle1 = new CGeosetTriangle(CurrentModel);
                    triangle1.Vertex1.Attach(v0);
                    triangle1.Vertex2.Attach(v1);
                    triangle1.Vertex3.Attach(v2);

                    CGeosetTriangle triangle2 = new CGeosetTriangle(CurrentModel);
                    triangle2.Vertex1.Attach(v0);
                    triangle2.Vertex2.Attach(v2);
                    triangle2.Vertex3.Attach(v3);

                    owner.Triangles.Add(triangle1);
                    owner.Triangles.Add(triangle2);
                }
            }
        }

        private void deleteVertex(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> Vertices = GetSelectedVertices();
            foreach (var v in Vertices)
            {
                foreach (var geoset in CurrentModel.Geosets)
                {
                    if (geoset.Vertices.Contains(v))
                    {
                        geoset.Triangles.ObjectList.RemoveAll(
                            x =>
                            x.Vertex1.Object == v ||
                            x.Vertex2.Object == v ||
                            x.Vertex3.Object == v);
                        geoset.Vertices.Remove(v);
                    }
                }
            }
            RefreshGeosets_Vertices();
        }
        private void DEleteTriangles(object? sender, RoutedEventArgs? e)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.Triangles.ObjectList.RemoveAll(x => x.isSelected == true);
            }
            RefreshGeosets_Triangles();
        }
        private void Animator_ClearTranslations_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Translation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
        }
        private void Animator_ClearRotations_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Rotation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
        }
        private void Animator_ClearScalings_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Scaling.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
        }
        private void Animator_ReverseTranslations_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node.Translation.Count > 1)
                {
                    var translationList = node.Translation.NodeList;
                    // Reverse only the values, but keep the times the same
                    int count = translationList.Count;
                    for (int i = 0; i < count / 2; i++)
                    {
                        var temp = translationList[i].Value;
                        translationList[i].Value = translationList[count - 1 - i].Value;
                        translationList[count - 1 - i].Value = temp;
                    }
                }
            }
            RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private void Animator_ReverseRotations_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node.Rotation.Count > 1)
                {
                    var translationList = node.Rotation.NodeList;
                    // Reverse only the values, but keep the times the same
                    int count = translationList.Count;
                    for (int i = 0; i < count / 2; i++)
                    {
                        var temp = translationList[i].Value;
                        translationList[i].Value = translationList[count - 1 - i].Value;
                        translationList[count - 1 - i].Value = temp;
                    }
                }
            }
            RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private void Animator_ReverseScalings_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node.Scaling.Count > 1)
                {
                    var translationList = node.Scaling.NodeList;
                    // Reverse only the values, but keep the times the same
                    int count = translationList.Count;
                    for (int i = 0; i < count / 2; i++)
                    {
                        var temp = translationList[i].Value;
                        translationList[i].Value = translationList[count - 1 - i].Value;
                        translationList[count - 1 - i].Value = temp;
                    }
                }
            }
            RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private void Animator_NegateTranslations_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                foreach (var kf in node.Translation)
                {
                    kf.Value.X = -kf.Value.X;
                    kf.Value.Y = -kf.Value.Y;
                    kf.Value.Z = -kf.Value.Z;
                }
            }
        }
        private void Animator_NegateRotations_Sequence(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            var sequence = GetSelectedSequenceAnimator();
            foreach (var node in CurrentModel.Nodes)
            {
                foreach (var kf in node.Rotation)
                {
                    var euler = Calculator.QuaternionToEuler(kf.Value);
                    euler.X = -euler.X;
                    euler.Y = -euler.Y;
                    euler.Z = -euler.Z;
                    kf.Value = new CVector4(Calculator.EulerToQuaternion(euler));
                }
            }
        }

        private void Animator_CopySequenceTranslation(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            CopiedAnimatorSequence = GetSelectedSequenceAnimator();
            CopiedAnimatorSequenceType = TransformationType.Translation;
        }
        private void Animator_CopySequenceRotation(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            CopiedAnimatorSequence = GetSelectedSequenceAnimator();
            CopiedAnimatorSequenceType = TransformationType.Rotation;
        }
        private void Animator_CopySequenceScaling(object? sender, RoutedEventArgs? e)
        {
            if (List_Sequences_Animator.SelectedItem == null) return;
            CopiedAnimatorSequence = GetSelectedSequenceAnimator();
            CopiedAnimatorSequenceType = TransformationType.Scaling;
        }
        private void Animator_PasteSequence(object? sender, RoutedEventArgs? e)
        {
            if (CopiedAnimatorSequence == null) { return; }
            if (CurrentModel.Sequences.Contains(CopiedAnimatorSequence) == false) { return; }
            var target = GetSelectedSequenceAnimator();
            if (target == CopiedAnimatorSequence) { MessageBox.Show("Copied and pasted cannot be the same"); return; }
            if (target.Interval <= 0) { MessageBox.Show("Invalid interval"); return; }
            if (target.Interval == CopiedAnimatorSequence.Interval)
            {
            }
            else
            {
                var result = MessageBox.Show("The sequences do not have the same interval. Try to Squish?", "Wait!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Code to handle "Yes" click
                }
            }
            foreach (var node in CurrentModel.Nodes)
            {
                if (CopiedAnimatorSequenceType == TransformationType.Translation)
                {
                }
                if (CopiedAnimatorSequenceType == TransformationType.Rotation)
                {
                }
                if (CopiedAnimatorSequenceType == TransformationType.Scaling)
                {
                }
            }
        }
        private void CopyKeyframeAll(object? sender, RoutedEventArgs? e)
        {
        }
        private void clearquads(object? sender, RoutedEventArgs? e)
        {
            QuadCollector.Clear();
        }
        private void clearquadsexcpet(object? sender, RoutedEventArgs? e)
        {
            var t = getSelectedTriangles();
            QuadCollector.ClearExcept(t);
        }
        private void addSelectedTrianglesAsQuads(object? sender, RoutedEventArgs? e)
        {
            var t = getSelectedTriangles();
            if (t.Count != 2) { MessageBox.Show("Select exactly 2 triangles"); return; }
            QuadCollector.Add(t[0], t[1]);
        }
        private void fitQuads(object? sender, RoutedEventArgs? e)
        {
            QuadFitter qf = new QuadFitter();
            qf.ShowDialog();
        }
        private void SelectAllBonesInNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CBone)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllAttachments(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CAttachment)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllCollisionShapesInNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CCollisionShape)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllEmitters1InNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CParticleEmitter)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllEmitters2InNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CParticleEmitter2)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllLightsInNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CLight)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllRibbonsInNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CRibbonEmitter)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllEventsInNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CEvent)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void SelectAllHelpersInNodeEditor(object? sender, RoutedEventArgs? e)
        {
            list_Node_Editor.UnselectAll(); // Unselect before modifying
            for (int i = 0; i < list_Node_Editor.Items.Count; i++)
            {
                if (CurrentModel.Nodes[i] is CHelper)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]); // Add properly
                }
            }
        }
        private void centerGeosetAtGeoset(object? sender, RoutedEventArgs? e)
        {
            var geosets = GetSelectedGeosets();
            if (geosets.Count == 0) { MessageBox.Show("Select at least 1 geoset"); return; }
            List<string> list = CurrentModel.Geosets.Select(x => x.ObjectId.ToString()).ToList();
            Selector s = new Selector(list);
            if (s.ShowDialog() == true)
            {
                int index = s.box.SelectedIndex;
                Calculator.CenterGeosetsAtGeoset(geosets, CurrentModel.Geosets[index]); ;
            }
        }
        private void SelectedPath(object? sender, SelectionChangedEventArgs? e)
        {
            ListPathNodes.Items.Clear();
            if (ListPaths.SelectedItem != null)
            {
                int index = ListPaths.SelectedIndex;
                var path = PathManager.Paths[index];
                PathManager.Selected = path;
                RefreshPathNodesList();
            }
            else
            {
                PathManager.Selected = null;
            }
        }
        private void newPath(object? sender, RoutedEventArgs? e)
        {
            Input i = new Input("Name");
            if (i.ShowDialog() == true)
            {
                string s = i.Result.Trim(); ;
                if (PathManager.Paths.Any(x => x.Name == s))
                {
                    MessageBox.Show("There is already a path with this name"); return;
                }
                cPath path = new cPath(s);
                PathManager.Paths.Add(path);
                ListPaths.Items.Add(new ListBoxItem() { Content = s + " [0]" });
            }
        }
        private void Delath(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem != null)
            {
                int index = ListPaths.SelectedIndex;
                ListPaths.Items.RemoveAt(index);
                PathManager.Paths.RemoveAt(index);
                ListPathNodes.Items.Clear();
            }
        }
        private void RenamePath(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem != null)
            {
                int index = ListPaths.SelectedIndex;
                var path = PathManager.Paths[index];
                string oldName = path.Name;
                Input i = new Input(oldName);
                if (i.ShowDialog() == true)
                {
                    string s = i.Result.Trim(); ;
                    if (PathManager.Paths.Any(x => x.Name == s))
                    {
                        MessageBox.Show("There is already a path with this name"); return;
                    }
                    path.Name = s;
                    RefreshPathsList();
                }
            }
        }

        private void SelectedPathNode(object? sender, SelectionChangedEventArgs? e)
        {
        }
        private void PullPath(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem != null)
            {
                InputVector v = new InputVector(AllowedValue.Both);
                if (ShowDialog() == true)
                {
                    var c = new CVector3(v.X, v.Y, v.Z);
                    int index = ListPaths.SelectedIndex;
                    var path = PathManager.Paths[index];
                    PathManager.MovePath(path, c);
                }
            }
        }
        private void ScalePath(object? sender, RoutedEventArgs? e)
        {
        }
        private void RotatePath(object? sender, RoutedEventArgs? e)
        {
        }
        private void MovePath(object? sender, RoutedEventArgs? e)
        {
        }
        private void AnimatePath(object? sender, RoutedEventArgs? e)
        {
            if (PathManager.Selected == null)
            {
                MessageBox.Show("Select a path"); return;
            }
            if (ListPaths.SelectedItem == null)
            {
                MessageBox.Show("Select a path"); return;
            }
            if (ListSequences_Paths.SelectedItem == null)
            {
                MessageBox.Show("Select a sequence"); return;
            }
            if (ListModelNodes_Paths.SelectedItem == null)
            {
                MessageBox.Show("Select a node"); return;
            }
            if (PathManager.Selected.Count <= 1)
            {
                MessageBox.Show("The path has less than 2 path nodes"); return;
            }
            var SelectedSequence = CurrentModel.Sequences[ListSequences_Paths.SelectedIndex];
            var SelectedNode = CurrentModel.Nodes[ListModelNodes_Paths.SelectedIndex];
            SelectedNode.Translation.Type = check_makeLinear.IsChecked == true ? EInterpolationType.Linear : EInterpolationType.None;
            if (Check_PathFullInterval.IsChecked == true)
            {
                PathManager.AnimateAbsolute(SelectedNode, SelectedSequence.IntervalStart, SelectedSequence.IntervalEnd, PathManager.Selected);
            }
            else
            {
                bool p = int.TryParse(InputPathCustomFrom.Text, out int from);
                bool p2 = int.TryParse(InputPathCustomFrom.Text, out int to);
                if (!p || !p2)
                {
                    MessageBox.Show("Invalid input for custom from and to"); return;
                }
                if (from >= to)
                {
                    MessageBox.Show("Invalid from-to interval input"); return;
                }
                if (from > SelectedSequence.IntervalEnd || from < SelectedSequence.IntervalStart)
                {
                    MessageBox.Show("The from input is not inside the selected sequence"); return;
                }
                if (to > SelectedSequence.IntervalEnd || to < SelectedSequence.IntervalStart)
                {
                    MessageBox.Show("The to input is not inside the selected sequence"); return;
                }
                PathManager.AnimateAbsolute(SelectedNode, from, to, PathManager.Selected);
                SelectedNode.Translation.NodeList = SelectedNode.Translation.NodeList.ToList();
            }
            transformation_editor t = new transformation_editor(CurrentModel, SelectedNode.Translation, false, TransformationType.Translation);
            t.Close();
            //  RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private void AnimatePathRelative(object? sender, RoutedEventArgs? e)
        {
            if (PathManager.Selected == null)
            {
                MessageBox.Show("Select a path"); return;
            }
            if (ListPaths.SelectedItem == null)
            {
                MessageBox.Show("Select a path"); return;
            }
            if (ListSequences_Paths.SelectedItem == null)
            {
                MessageBox.Show("Select a sequence"); return;
            }
            if (ListModelNodes_Paths.SelectedItem == null)
            {
                MessageBox.Show("Select a node"); return;
            }
            if (PathManager.Selected.Count <= 1)
            {
                MessageBox.Show("The path has less than 2 path nodes"); return;
            }
            var sequenece = CurrentModel.Sequences[ListSequences_Paths.SelectedIndex];
            var SelectedNode = CurrentModel.Nodes[ListModelNodes_Paths.SelectedIndex];
            SelectedNode.Translation.Type = check_makeLinear.IsChecked == true ? EInterpolationType.Linear : EInterpolationType.None;
            if (Check_PathFullInterval.IsChecked == true)
            {
                PathManager.AnimateRelative(SelectedNode, sequenece.IntervalStart, sequenece.IntervalEnd, PathManager.Selected);
            }
            else
            {
                bool p = int.TryParse(InputPathCustomFrom.Text, out int from);
                bool p2 = int.TryParse(InputPathCustomFrom.Text, out int to);
                if (!p || !p2)
                {
                    MessageBox.Show("Invalid input for custom from and to"); return;
                }
                if (from >= to)
                {
                    MessageBox.Show("Invalid from-to interval input"); return;
                }
                if (from > sequenece.IntervalEnd || from < sequenece.IntervalStart)
                {
                    MessageBox.Show("The from input is not inside the selected sequence"); return;
                }
                if (to > sequenece.IntervalEnd || to < sequenece.IntervalStart)
                {
                    MessageBox.Show("The to input is not inside the selected sequence"); return;
                }
                PathManager.AnimateRelative(SelectedNode, from, to, PathManager.Selected);
            }
            transformation_editor t = new transformation_editor(CurrentModel, SelectedNode.Translation, false, TransformationType.Translation);
            t.Close();
            //  RefreshAnimatedVertexAndNodePositionsForRendering();
        }
        private List<PathNode> GetSelectedPathNodes(cPath path)
        {
            List<PathNode> list = new List<PathNode>();
            foreach (var item in ListPathNodes.SelectedItems)
            {
                list.Add(path.List[ListPathNodes.Items.IndexOf(item)]);
            }
            return list;
        }
        private void SwapPathNodePos(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.SelectedItems.Count != 2) { MessageBox.Show("Select exactly 2 path nodes"); return; }
            var list = GetSelectedPathNodes(PathManager.Paths[ListPaths.SelectedIndex]);
            var temp = new CVector3(list[0].Position);
            list[0].Position = new CVector3(list[1].Position);
            list[1].Position = new CVector3(temp);
        }
        private void delPathNode(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.SelectedItems.Count == 0) { MessageBox.Show("Select   path node/s"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            List<object> selected = new List<object>();
            foreach (var item in ListPathNodes.SelectedItems)
            {
                selected.Add(item);
            }
            foreach (var item in selected)
            {
                path.RemoveAt(ListPathNodes.Items.IndexOf(item));
                ListPathNodes.Items.Remove(item);
            }
        }
        private void duplPathNode(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.SelectedItems.Count == 0) { MessageBox.Show("Select   path node/s"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            foreach (var item in ListPathNodes.SelectedItems)
            {
                var index = ListPathNodes.Items.IndexOf(item);
                path.Duplicate(index);
            }
            RefreshPathNodesList();
        }
        private void RefreshPath_ModelNodes_List()
        {
            ListModelNodes_Paths.Items.Clear();
            foreach (var node in CurrentModel.Nodes)
            {
                string name = $"{node.Name} [{node.GetType().Name}]";
                ListModelNodes_Paths.Items.Add(new ListBoxItem() { Content = name });
            }
        }
        private void RefreshPathNodesList()
        {
            if (ListPaths.SelectedItem == null) { return; }
            var path = PathManager.Selected ?? throw new FileNotFoundException("null path.");

            ListPathNodes.Items.Clear();
            string count = $"{path.Count} Nodes of selected path";
            LabelCountPathNodes.Text = count;
            if (path.Count == 0) return;
            for (int i = 0; i < path.Count; i++)
            {
                string content = $"{i} ({path.List[i].Position.X}, {path.List[i].Position.Y}, {path.List[i].Position.Z})";
                ListPathNodes.Items.Add(new ListBoxItem() { Content = content });
            }
        }
        private void NewPathNode0(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            var path = PathManager.Selected; if (path == null) return;
            path.Add(new PathNode());
            RefreshPathNodesList();
            SetSaved(false);
        }
        private void NewPathNodeAtNode(object? sender, RoutedEventArgs? e)
        {
            if (ListModelNodes_Paths.SelectedItem == null) { MessageBox.Show("Select a model node"); return; }
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.SelectedItems.Count == 0) { MessageBox.Show("Select   path node/s"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            var node = CurrentModel.Nodes[ListModelNodes_Paths.SelectedIndex];
            PathNode pnode = new PathNode(node.PivotPoint);
            path.Add(pnode);
            SetSaved(false);
            RefreshPathNodesList();
        }

        private void CopyPathNodePos(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.SelectedItems.Count != 1) { MessageBox.Show("Select 1 path node"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            var pnode = path.List[ListPathNodes.SelectedIndex];
            CopiedPathNodePosition = new CVector3(pnode.Position);
        }
        private void PastePathNodePos(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.SelectedItems.Count != 1) { MessageBox.Show("Select 1 path node"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            var pnode = path.List[ListPathNodes.SelectedIndex];
            if (CopiedPathNodePosition == null) { MessageBox.Show("Nothing is copied"); }
            pnode.Position = new CVector3(CopiedPathNodePosition);
            int index = ListPathNodes.SelectedIndex;
            ListPathNodes.Items[index] = new ListBoxItem() { Content = $"{index} [{pnode.Position.X} {pnode.Position.Y} {pnode.Position.Z}]" };
        }
        public static void SwapElements<T>(List<T> list, int one, int two)
        {
            if (one < 0 || two < 0 || one >= list.Count || two >= list.Count)
            {
                throw new ArgumentOutOfRangeException("Index is out of range.");
            }
            T temp = list[one];
            list[one] = list[two];
            list[two] = temp;
        }
        private void movePahNodeUp(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.Items.Count < 2) { MessageBox.Show("Not enough path nodes in the list"); return; }
            if (ListPathNodes.SelectedItems.Count != 1) { MessageBox.Show("Select 1 path node"); return; }
            if (ListPathNodes.SelectedIndex == 0) { MessageBox.Show("It's already at the top"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            var pnode = path.List[ListPathNodes.SelectedIndex];
            SwapElements(path.List, ListPathNodes.SelectedIndex, ListPathNodes.SelectedIndex - 1);
            RefreshPathNodesList();
        }
        private void movePahNodeDown(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.Items.Count < 2) { MessageBox.Show("Not enough path nodes in the list"); return; }
            if (ListPathNodes.SelectedItems.Count != 1) { MessageBox.Show("Select 1 path node"); return; }
            if (ListPathNodes.SelectedIndex == ListPathNodes.Items.Count - 1) { MessageBox.Show("It's already at the bottom"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            var pnode = path.List[ListPathNodes.SelectedIndex];
            SwapElements(path.List, ListPathNodes.SelectedIndex, ListPathNodes.SelectedIndex + 1);
            RefreshPathNodesList();
        }
        private void SwapNodeInOrder(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem == null) { MessageBox.Show("Select a path"); return; }
            if (ListPathNodes.Items.Count < 2) { MessageBox.Show("Not enough path nodes in the list"); return; }
            if (ListPathNodes.SelectedItems.Count != 2) { MessageBox.Show("Select 2 path nodes"); return; }
            var path = PathManager.Paths[ListPaths.SelectedIndex];
            var pnodes = GetSelectedPathNodes(path);
            int one = path.List.IndexOf(pnodes[0]);
            int two = path.List.IndexOf(pnodes[1]);
            SwapElements(path.List, one, two);
            RefreshPathNodesList();
        }

        private void Tab_Animator_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if (PlayTimer == null) return;
            if (e == null) return;
            if (e.OriginalSource is TabControl tc && tc == Tab_Animator)
            {
                //----------------------------------
                PlayTimer.Stop();
                // IconPlay.Visibility = Visibility.Collapsed;
                //  IconDontPlay.Visibility = Visibility.Hidden;
                Playback.Type = PlayBackType.Paused;
                //----------------------------------
                ViewingPaths = Tab_Animator.SelectedIndex == 0 && Tabs_Geosets.SelectedIndex == 4;
                if (Tab_Animator.SelectedIndex == 0) //paths
                {
                    RefreshPathsList();
                    RefreshSequencesList_Paths();
                    RefreshPathNodesList();
                    RefreshPath_ModelNodes_List();
                }
                if (Tab_Animator.SelectedIndex == 2)
                {
                    RefreshSequencesList_Player();
                }
                if (Tab_Animator.SelectedIndex == 1) //animate
                {
                    RefreshAnimatorData();
                }
            }
        }
        private void RefreshSequencesList_Paths()
        {
            ListSequences_Paths.Items.Clear();
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                string looping = sequence.NonLooping ? "Nonlooping" : "Looping";
                string data = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}] ({looping})";
                ListSequences_Paths.Items.Add(new ListBoxItem() { Content = data });
            }
        }
        private void RefreshPathsList()
        {
            ListPaths.Items.Clear();
            if (PathManager.Paths.Count > 0)
            {
                foreach (var path in PathManager.Paths)
                {
                    string name = $"{path.Name} [{path.Count}]";
                    ListPaths.Items.Add(new ListBoxItem() { Content = name });
                }
            }
        }

        private void Play(object? sender, RoutedEventArgs? e)
        {
            if (PlayTimer == null) return;
            if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("No sequences"); return; }
            Playback.Sequences = CurrentModel;
            if (ListSequences_Play.SelectedItem == null)
            {
                ListSequences_Play.SelectedItem = ListSequences_Play.Items[0];
            }
            Playback.CurrentSequence = CurrentModel.Sequences[ListSequences_Play.SelectedIndex];
            if (Playback.Type != PlayBackType.Paused)
            {
                Playback.Type = PlayBackType.Paused;
                PlayTimer.Stop();
                IconPlay.Visibility = Visibility.Collapsed;
                IconDontPlay.Visibility = Visibility.Visible;
            }
            else
            {
                if (Radio_PlayDef.IsChecked == true) Playback.Type = PlayBackType.Default;
                else if (Radio_PlayAlways.IsChecked == true) Playback.Type = PlayBackType.Loop;
                else if (Radio_PlayNever.IsChecked == true) Playback.Type = PlayBackType.DontLoop;
                else if (Radio_PlayCycle.IsChecked == true) Playback.Type = PlayBackType.Cycle;
                else { return; }
                PlayTimer.Start();
                IconPlay.Visibility = Visibility.Visible;
                IconDontPlay.Visibility = Visibility.Collapsed;
            }
        }
        private void ChangePlayType(object? sender, RoutedEventArgs? e)
        {
            if (Radio_PlayDef.IsChecked == true) Playback.Type = PlayBackType.Default;
            else if (Radio_PlayAlways.IsChecked == true) Playback.Type = PlayBackType.Loop;
            else if (Radio_PlayNever.IsChecked == true) Playback.Type = PlayBackType.DontLoop;
            else if (Radio_PlayCycle.IsChecked == true) Playback.Type = PlayBackType.Cycle;
            else { Playback.Type = PlayBackType.Paused; }
        }
        private void SetPlayDefault(object? sender, RoutedEventArgs? e)
        {
            playMode = PlayMode.Default;
        }
        private void setplayNever(object? sender, RoutedEventArgs? e)
        {
            playMode = PlayMode.DontLoop;
        }
        private void setplayalways(object? sender, RoutedEventArgs? e)
        {
            playMode = PlayMode.Loop;
        }
        private void SetPlayCycle(object? sender, RoutedEventArgs? e)
        {
            playMode = PlayMode.Cycle;
        }
        private void ListSequences_Play_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if (ListSequences_Play.SelectedItem != null)
            {
                Playback.CurrentSequence = CurrentModel.Sequences[ListSequences_Play.SelectedIndex];
            }
        }
        private void SelectedPathNodes(object? sender, SelectionChangedEventArgs? e)
        {
            if (ListPaths.SelectedItem != null)
            {
                if (ListPathNodes.SelectedItems.Count == 0)
                {
                    if (PathManager.Selected == null) return;
                    foreach (var node in PathManager.Selected.List) { node.IsSelected = false; }
                    InputAnimatorX.Text = "0";
                    InputAnimatorY.Text = "0";
                    InputAnimatorZ.Text = "0";
                    return;
                }
                var path = PathManager.Selected; if (path == null) return;
                // select based on listbox
                for (int i = 0; i < ListPathNodes.Items.Count; i++)
                {
                    path.List[i].IsSelected = ListPathNodes.SelectedItems.Contains(ListPathNodes.Items[i]);
                }
                // get all selected
                if (PathManager.Selected == null) return;
                List<PathNode> SelectedPathNodes = PathManager.Selected.List.Where(x => x.IsSelected).ToList();
                // set the coodinates in the manual input based on selected
                if (Selected.Count == 1)
                {
                    InputAnimatorX.Text = SelectedPathNodes[0].Position.X.ToString();
                    InputAnimatorY.Text = SelectedPathNodes[0].Position.Y.ToString();
                    InputAnimatorZ.Text = SelectedPathNodes[0].Position.Z.ToString();
                }
                if (Selected.Count > 1)
                {
                    var centroid = Calculator.GetCentroidOfVectors(SelectedPathNodes.Select(x => x.Position).ToList());
                    InputAnimatorX.Text = centroid.X.ToString();
                    InputAnimatorY.Text = centroid.Y.ToString();
                    InputAnimatorZ.Text = centroid.Z.ToString();
                }
            }
        }
        private void ExportPath(object? sender, RoutedEventArgs? e)
        {
            if (ListPaths.SelectedItem != null)
            {
                var path = PathManager.Paths[ListPaths.SelectedIndex];
                PathManager.Export(path);
            }
        }
        private void ImportPaths(object? sender, RoutedEventArgs? e)
        {
            if (PathManager.Import())
            {
                RefreshPathsList();
                ListPathNodes.Items.Clear();
            }
        }
        private void ListPathNodes_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
        }
        private void editBoneInfluence(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) return;
            var node = GetSelectedNode();
            Bone_Influence_Editor be = new Bone_Influence_Editor(node);
        }
        private void saveNative(object? sender, RoutedEventArgs? e)
        {
            NativeFormat.Save(CurrentModel);
        }
        private void load_native(object? sender, RoutedEventArgs? e)
        {
        }
        private void MoveWithArrows(Axes ax, bool positive)
        {
            bool p = float.TryParse(InputManualTransformAmount.Text, out float val);
            if (!p) { MessageBox.Show("Not an integer/float number"); return; }
            if (val <= 0) { MessageBox.Show("Input for change must be a positive number"); return; }
            if (InAnimator)
            {

                INode? node = GetSelectedNodeInanimator(); if (node == null) return;
                int track = Animator_GetTrack();
                var first = node.Translation.First(x => x.Time == track);
                first.Value.X = ax == Axes.X ? (first.Value.X + (positive ? val : -val)) : first.Value.X;
                first.Value.Y = ax == Axes.Y ? (first.Value.Y + (positive ? val : -val)) : first.Value.Y;
                first.Value.Z = ax == Axes.Z ? (first.Value.Z + (positive ? val : -val)) : first.Value.Z;

                RefreshAnimatedVertexAndNodePositionsForRendering(track);
                return;
            }

            if (Tabs_Geosets.SelectedIndex == 0) // geosets
            {
                var geosets = GetSelectedGeosets();
                foreach (var geoset in geosets)
                {
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (positive)
                        {
                            vertex.Position.X = ax == Axes.X ? vertex.Position.X + val : vertex.Position.X;
                            vertex.Position.Y = ax == Axes.Y ? vertex.Position.Y + val : vertex.Position.Y;
                            vertex.Position.Z = ax == Axes.Z ? vertex.Position.Z + val : vertex.Position.Z;
                        }
                        else
                        {
                            vertex.Position.X = ax == Axes.X ? vertex.Position.X - val : vertex.Position.X;
                            vertex.Position.Y = ax == Axes.Y ? vertex.Position.Y - val : vertex.Position.Y;
                            vertex.Position.Z = ax == Axes.Z ? vertex.Position.Z - val : vertex.Position.Z;
                        }
                    }
                }
            }
            if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                var triangles = getSelectedTriangles();
                foreach (var triangle in triangles)
                {
                    if (positive)
                    {
                        triangle.Vertex1.Object.Position.X = ax == Axes.X ? triangle.Vertex1.Object.Position.X + val : triangle.Vertex1.Object.Position.X;
                        triangle.Vertex2.Object.Position.X = ax == Axes.X ? triangle.Vertex2.Object.Position.X + val : triangle.Vertex2.Object.Position.X;
                        triangle.Vertex3.Object.Position.X = ax == Axes.X ? triangle.Vertex3.Object.Position.X + val : triangle.Vertex3.Object.Position.X;
                        triangle.Vertex1.Object.Position.Y = ax == Axes.X ? triangle.Vertex1.Object.Position.Y + val : triangle.Vertex1.Object.Position.Y;
                        triangle.Vertex2.Object.Position.Y = ax == Axes.X ? triangle.Vertex2.Object.Position.Y + val : triangle.Vertex2.Object.Position.Y;
                        triangle.Vertex3.Object.Position.Y = ax == Axes.X ? triangle.Vertex3.Object.Position.Y + val : triangle.Vertex3.Object.Position.Y;
                        triangle.Vertex1.Object.Position.Z = ax == Axes.Z ? triangle.Vertex1.Object.Position.Z + val : triangle.Vertex1.Object.Position.Z;
                        triangle.Vertex2.Object.Position.Z = ax == Axes.Z ? triangle.Vertex2.Object.Position.Z + val : triangle.Vertex2.Object.Position.Z;
                        triangle.Vertex3.Object.Position.Z = ax == Axes.Z ? triangle.Vertex3.Object.Position.Z + val : triangle.Vertex3.Object.Position.Z;
                    }
                    else
                    {
                        triangle.Vertex1.Object.Position.X = ax == Axes.X ? triangle.Vertex1.Object.Position.X - val : triangle.Vertex1.Object.Position.X;
                        triangle.Vertex2.Object.Position.X = ax == Axes.X ? triangle.Vertex2.Object.Position.X - val : triangle.Vertex2.Object.Position.X;
                        triangle.Vertex3.Object.Position.X = ax == Axes.X ? triangle.Vertex3.Object.Position.X - val : triangle.Vertex3.Object.Position.X;
                        triangle.Vertex1.Object.Position.Y = ax == Axes.X ? triangle.Vertex1.Object.Position.Y - val : triangle.Vertex1.Object.Position.Y;
                        triangle.Vertex2.Object.Position.Y = ax == Axes.X ? triangle.Vertex2.Object.Position.Y - val : triangle.Vertex2.Object.Position.Y;
                        triangle.Vertex3.Object.Position.Y = ax == Axes.X ? triangle.Vertex3.Object.Position.Y - val : triangle.Vertex3.Object.Position.Y;
                        triangle.Vertex1.Object.Position.Z = ax == Axes.Z ? triangle.Vertex1.Object.Position.Z - val : triangle.Vertex1.Object.Position.Z;
                        triangle.Vertex2.Object.Position.Z = ax == Axes.Z ? triangle.Vertex2.Object.Position.Z - val : triangle.Vertex2.Object.Position.Z;
                        triangle.Vertex3.Object.Position.Z = ax == Axes.Z ? triangle.Vertex3.Object.Position.Z - val : triangle.Vertex3.Object.Position.Z;
                    }
                }
            }
            if (Tabs_Geosets.SelectedIndex == 2) // vertuces
            {
                var vertices = GetSelectedVertices();
                foreach (var v in vertices)
                {
                    if (positive)
                    {

                        v.Position.X += ax == Axes.X ? val : 0;
                        v.Position.Y += ax == Axes.Y ? val : 0;
                        v.Position.Z += ax == Axes.Z ? val : 0;
                    }
                    else
                    {
                        v.Position.X -= ax == Axes.X ? val : 0;
                        v.Position.Y -= ax == Axes.Y ? val : 0;
                        v.Position.Z -= ax == Axes.Z ? val : 0;
                    }
                }
            }
            if (Tabs_Geosets.SelectedIndex == 4) // animator
            {
                if (Tab_Animator.SelectedIndex == 0) // paths
                {
                    if (PathManager.Selected != null)
                    {
                        foreach (var node in Selected.List)
                        {
                            if (node.IsSelected)
                            {
                                if (positive)
                                {
                                    node.Position.X = ax == Axes.X ? node.Position.X + val : node.Position.X;
                                    node.Position.Y = ax == Axes.Y ? node.Position.Y + val : node.Position.Y;
                                    node.Position.Z = ax == Axes.Z ? node.Position.Z + val : node.Position.Z;
                                }
                                else
                                {
                                    node.Position.X = ax == Axes.X ? node.Position.X - val : node.Position.X;
                                    node.Position.Y = ax == Axes.Y ? node.Position.Y - val : node.Position.Y;
                                    node.Position.Z = ax == Axes.Z ? node.Position.Z - val : node.Position.Z;
                                }
                            }
                        }
                    }
                }
            }
            if (Tabs_Geosets.SelectedIndex == 6) // nodes
            {
                var list = GetSelectedNodes_NodeEditor();
                foreach (var node in list)
                {
                    if (positive)
                    {
                        node.PivotPoint.X = ax == Axes.X ? node.PivotPoint.X + val : node.PivotPoint.X;
                        node.PivotPoint.Y = ax == Axes.Y ? node.PivotPoint.Y + val : node.PivotPoint.Y;
                        node.PivotPoint.Z = ax == Axes.Z ? node.PivotPoint.Z + val : node.PivotPoint.Z;

                    }
                    else
                    {
                        node.PivotPoint.X = ax == Axes.X ? node.PivotPoint.X - val : node.PivotPoint.X;
                        node.PivotPoint.Y = ax == Axes.Y ? node.PivotPoint.Y - val : node.PivotPoint.Y;
                        node.PivotPoint.Z = ax == Axes.Z ? node.PivotPoint.Z - val : node.PivotPoint.Z;

                    }


                }
            }
        }
        private void MoveBackward(object? sender, RoutedEventArgs? e)
        {
            MoveWithArrows(Axes.X, false);
        }
        private void ScaleSelection(bool positive, Axes AxisMode, bool each)
        {
            float value = GetTransformationIncrement() / 100; // scaling increment
            float scaleFactor = positive ? (1 + value) : (1 - value);

            if (InAnimator)
            {
                INode? node = GetSelectedNodeInanimator();
                if (node == null) return;

                int track = Animator_GetTrack();
                var first = node.Scaling.First(x => x.Time == track);

                first.Value.X *= scaleFactor;
                first.Value.Y *= scaleFactor;
                first.Value.Z *= scaleFactor;

                RefreshAnimatedVertexAndNodePositionsForRendering(track);
                return;
            }

            if (Tabs_Geosets.SelectedIndex == 0) // Geosets
            {
                var geosets = GetSelectedGeosets();
                if (each)
                {
                    foreach (var geoset in geosets)
                    {
                        CVector3 centroid = Calculator.GetCentroidOfGeoset(geoset);

                        foreach (var vertex in geoset.Vertices)
                        {
                            if (AxisMode == Axes.U || AxisMode == Axes.X)
                                vertex.Position.X = centroid.X + (vertex.Position.X - centroid.X) * scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Y)
                                vertex.Position.Y = centroid.Y + (vertex.Position.Y - centroid.Y) * scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Z)
                                vertex.Position.Z = centroid.Z + (vertex.Position.Z - centroid.Z) * scaleFactor;
                        }
                    }
                }
                else
                {
                    foreach (var geoset in geosets)
                    {
                        foreach (var vertex in geoset.Vertices)
                        {
                            if (AxisMode == Axes.U || AxisMode == Axes.X)
                                vertex.Position.X *= scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Y)
                                vertex.Position.Y *= scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Z)
                                vertex.Position.Z *= scaleFactor;
                        }
                    }
                }
            }

            if (Tabs_Geosets.SelectedIndex == 1) // Triangles
            {
                var triangles = getSelectedTriangles();
                CVector3 centroid = Calculator.GetCentroidofTriangles(triangles);

                if (each)
                {
                    foreach (var triangle in triangles)
                    {
                        foreach (var v in new[] { triangle.Vertex1, triangle.Vertex2, triangle.Vertex3 })
                        {
                            if (AxisMode == Axes.U || AxisMode == Axes.X)
                                v.Object.Position.X = centroid.X + (v.Object.Position.X - centroid.X) * scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Y)
                                v.Object.Position.Y = centroid.Y + (v.Object.Position.Y - centroid.Y) * scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Z)
                                v.Object.Position.Z = centroid.Z + (v.Object.Position.Z - centroid.Z) * scaleFactor;
                        }
                    }
                }
                else
                {
                    foreach (var triangle in triangles)
                    {
                        foreach (var v in new[] { triangle.Vertex1, triangle.Vertex2, triangle.Vertex3 })
                        {
                            if (AxisMode == Axes.U || AxisMode == Axes.X)
                                v.Object.Position.X *= scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Y)
                                v.Object.Position.Y *= scaleFactor;
                            if (AxisMode == Axes.U || AxisMode == Axes.Z)
                                v.Object.Position.Z *= scaleFactor;
                        }
                    }
                }
            }

            if (Tabs_Geosets.SelectedIndex == 2) // Vertices
            {
                var fvertices = GetSelectedVertices();
                Calculator.ScaleVerticesBy(AxisMode, fvertices, value, positive);
            }

            if (Tabs_Geosets.SelectedIndex == 6) // Nodes
            {
                var nodes = GetSelectedNodes_NodeEditor();
                foreach (var node in nodes)
                {
                    if (AxisMode == Axes.U || AxisMode == Axes.X)
                        node.PivotPoint.X *= scaleFactor;
                    if (AxisMode == Axes.U || AxisMode == Axes.Y)
                        node.PivotPoint.Y *= scaleFactor;
                    if (AxisMode == Axes.U || AxisMode == Axes.Z)
                        node.PivotPoint.Z *= scaleFactor;
                }
            }
        }

        private float GetTransformationIncrement()
        {
            bool f = float.TryParse(InputManualTransformAmount.Text, out float val);
            return f ? val : 1;
        }

        private List<INode> GetSelectedNodes_NodeEditor()
        {
            var selectedNodes = new List<INode>();
            foreach (var item in list_Node_Editor.SelectedItems)
            {
                int index = list_Node_Editor.Items.IndexOf(item);
                if (index >= 0 && index < CurrentModel.Nodes.Count)
                {
                    selectedNodes.Add(CurrentModel.Nodes[index]);
                }
            }
            return selectedNodes;
        }

        private void MoveForawrad(object? sender, RoutedEventArgs? e)
        {
            MoveWithArrows(Axes.X, true);
        }
        private void MoveLeft(object? sender, RoutedEventArgs? e)
        {
            MoveWithArrows(Axes.Y, false);
        }
        private void MoveRight(object? sender, RoutedEventArgs? e)
        {
            MoveWithArrows(Axes.Y, true);
        }
        private void MoveDown(object? sender, RoutedEventArgs? e)
        {
            MoveWithArrows(Axes.Z, false);
        }
        private void MoveUp(object? sender, RoutedEventArgs? e)
        {
            MoveWithArrows(Axes.Z, true);
        }
        private void NodeEditor_SelectByGeoset(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Geosets.Count == 0) { MessageBox.Show("There are no geosets"); return; }
            List<string> lst = CurrentModel.Geosets.Select(X => X.ObjectId.ToString()).ToList();
            Selector s = new Selector(lst);
            if (s.ShowDialog() == true)
            {
                int index = s.box.SelectedIndex;
                list_Node_Editor.Items.Clear();
                List<int> indexes = new List<int>();
                foreach (var vertex in CurrentModel.Geosets[index].Vertices)
                {
                    if (vertex.Group != null)
                    {
                        if (vertex.Group.Object != null)
                        {
                            foreach (var node in vertex.Group.Object.Nodes)
                            {
                                int NodeIndex = CurrentModel.Nodes.IndexOf(node.Node.Node);
                                if (!indexes.Contains(NodeIndex))
                                {
                                    indexes.Add(NodeIndex);
                                }
                            }
                            //list_Node_Editor
                        }
                    }
                }
                RefreshNodeEditorList();
                foreach (int i in indexes)
                {
                    list_Node_Editor.SelectedItems.Add(list_Node_Editor.Items[i]);
                }
            }
        }
        private void ExtractPathFromNow(object? sender, RoutedEventArgs? e)
        {
            if (ListModelNodes_Paths.SelectedItem == null) { MessageBox.Show("Select a model node"); return; }
            if (ListSequences_Paths.SelectedItem == null) { MessageBox.Show("Select a sequence"); return; }
            int index = ListModelNodes_Paths.SelectedIndex;
            INode node = CurrentModel.Nodes[ListModelNodes_Paths.SelectedIndex];
            CSequence s = CurrentModel.Sequences[ListSequences_Paths.SelectedIndex];
            if (CurrentModel.Nodes.Count <= index)
            {
                MessageBox.Show("The selected model node was not found in the model nodes collection. Refrsh the listbox."); return;
            }
            if (PathManager.Extract(node, s))
            {
                RefreshPathsList();
            }
        }
        private void Rigging_AttachWholGeoset(object? sender, RoutedEventArgs? e)
        {
            if (ListBonesRiggings.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            if (Listbox_Geosets_Rigging.SelectedItem == null) { MessageBox.Show("Select a geoset"); return; }
            var bone = CurrentModel.Nodes[ListBonesRiggings.SelectedIndex];
            var geoset = CurrentModel.Geosets[Listbox_Geosets_Rigging.SelectedIndex];
            //reflect
            ListAttachedToRiggings.Items.Clear();
            ListAttachedToRiggings.Items.Add(new ListBoxItem() { Content = bone.Name });
            ButtonAddAttach.IsEnabled = true;
            ButtonClearAttach.IsEnabled = true;
            ButtonDetach.IsEnabled = true;

        }

        private void createOrigin(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Nodes.Any(x => x.Name.ToLower().Trim() == "origin ref" && x is CAttachment))
            {
                MessageBox.Show("There is already an origin attachment point");
            }
            else
            {
                CAttachment att = new CAttachment(CurrentModel);
                att.Name = "Origin Ref";
                CurrentModel.Nodes.Add(att);
                RefreshNodesTree();
            }
        }


        private void CopyTrianglePosition(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            var tr = triangles[0];
            CopiedTrianglePosition1 = new CVector3(tr.Vertex1.Object.Position);
            CopiedTrianglePosition2 = new CVector3(tr.Vertex2.Object.Position);
            CopiedTrianglePosition3 = new CVector3(tr.Vertex3.Object.Position);

        }

        private void PasteTrianglePosition(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }

            triangles[0].Vertex1.Object.Position = new CVector3(CopiedTrianglePosition1);
            triangles[0].Vertex2.Object.Position = new CVector3(CopiedTrianglePosition1);
            triangles[0].Vertex3.Object.Position = new CVector3(CopiedTrianglePosition1);
        }

        private void reorderTriangleVerticesLEft(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            foreach (var triangle in triangles)
            {
                CVector3 temp2 = new CVector3(triangle.Vertex2.Object.Position);
                CVector3 temp1 = new CVector3(triangle.Vertex1.Object.Position);
                CVector3 temp3 = new CVector3(triangle.Vertex3.Object.Position);
                triangle.Vertex1.Object.Position = new CVector3(temp2);
                triangle.Vertex2.Object.Position = new CVector3(temp3);
                triangle.Vertex3.Object.Position = new CVector3(temp1);
            }

        }

        private void reorderTriangleVerticesRight(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            foreach (var triangle in triangles)
            {
                CVector3 temp2 = new CVector3(triangle.Vertex2.Object.Position);
                CVector3 temp1 = new CVector3(triangle.Vertex1.Object.Position);
                CVector3 temp3 = new CVector3(triangle.Vertex3.Object.Position);
                triangle.Vertex1.Object.Position = new CVector3(temp3);
                triangle.Vertex2.Object.Position = new CVector3(temp1);
                triangle.Vertex3.Object.Position = new CVector3(temp2);
            }
        }
        private CVector3[] CopiedNormal = new CVector3[3];

        private void CopyTriangleNormal(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count != 1) { MessageBox.Show("Select exactly 1 triangle"); return; }
            CopiedNormal[0] = new CVector3(triangles[0].Vertex1.Object.Normal);
            CopiedNormal[1] = new CVector3(triangles[0].Vertex2.Object.Normal);
            CopiedNormal[2] = new CVector3(triangles[0].Vertex3.Object.Normal);
        }

        private void PasteTriangleNormal(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count != 1) { MessageBox.Show("Select exactly 1 triangle"); return; }

            triangles[0].Vertex1.Object.Normal = new CVector3(CopiedNormal[0]);
            triangles[0].Vertex2.Object.Normal = new CVector3(CopiedNormal[1]);
            triangles[0].Vertex3.Object.Normal = new CVector3(CopiedNormal[2]);
        }
        private CVector2[] CopiedUV = new CVector2[3];
        private void CopyTriangleUV(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count != 1) { MessageBox.Show("Select exactly 1 triangle"); return; }
            CopiedUV[0] = new CVector2(triangles[0].Vertex1.Object.TexturePosition);
            CopiedUV[1] = new CVector2(triangles[0].Vertex2.Object.TexturePosition);
            CopiedUV[2] = new CVector2(triangles[0].Vertex3.Object.TexturePosition);
        }

        private void PasteTriangleUV(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count != 1) { MessageBox.Show("Select exactly 1 triangle"); return; }

            triangles[0].Vertex1.Object.TexturePosition = new CVector2(CopiedUV[0]);
            triangles[0].Vertex2.Object.TexturePosition = new CVector2(CopiedUV[1]);
            triangles[0].Vertex3.Object.TexturePosition = new CVector2(CopiedUV[2]);
        }

        private void CollapseTriangles(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            var centroid = Calculator.GetCentroidOfVertices(new List<CGeosetVertex>() { triangles[0].Vertex1.Object, triangles[0].Vertex2.Object, triangles[0].Vertex3.Object });

            foreach (var triangle in triangles)
            {
                triangle.Vertex1.Object.Position = new CVector3(centroid);
                triangle.Vertex2.Object.Position = new CVector3(centroid);
                triangle.Vertex3.Object.Position = new CVector3(centroid);
            }
            RefreshGeosets_Triangles();

        }

        private void detachTriangleInsideGeoset(object? sender, RoutedEventArgs? e)
        {

            var triangles = getSelectedTriangles();
            foreach (var triangle in triangles)
            {
                var geoset = GetGeosetOfTriangle(triangle);
                if (geoset == null)
                {
                    MessageBox.Show($"Triangle {triangle.ObjectId} is not part of any geoset"); return;
                }
                DetachVerticesOfTriangle(geoset, triangle);

            }
            RefreshGeosets_Triangles();
        }

        private void DetachVerticesOfTriangle(CGeoset geoset, CGeosetTriangle triangle, bool up = false)
        {
            var v1 = triangle.Vertex1.Object;
            var v2 = triangle.Vertex2.Object;
            var v3 = triangle.Vertex3.Object;

            foreach (var tr in geoset.Triangles)
            {
                if (tr == triangle) { continue; }
                if (tr.Vertex1.Object == v1)
                {
                    CGeosetVertex v = CopyVertex(tr.Vertex1.Object, geoset, up);
                    geoset.Vertices.Add(v);
                    tr.Vertex1.Attach(v);
                }
                if (tr.Vertex2.Object == v2)
                {
                    CGeosetVertex v = CopyVertex(tr.Vertex2.Object, geoset, up);
                    geoset.Vertices.Add(v);
                    tr.Vertex2.Attach(v);
                }
                if (tr.Vertex3.Object == v3)
                {
                    CGeosetVertex v = CopyVertex(tr.Vertex3.Object, geoset, up);
                    geoset.Vertices.Add(v);
                    tr.Vertex3.Attach(v);
                }
            }


        }

        private void DuplicateTrianglesInsideGeoset(bool up = false)
        {
            var triangles = getSelectedTriangles();
            List<CGeosetTriangle> created = new List<CGeosetTriangle>();
            foreach (var triangle in triangles)
            {
                var geoset = GetGeosetOfTriangle(triangle);
                if (geoset == null)
                {
                    MessageBox.Show($"Triangle {triangle.ObjectId} is not part of any geoset"); return;
                }

                var v1 = triangle.Vertex1.Object;
                var v2 = triangle.Vertex2.Object;
                var v3 = triangle.Vertex3.Object;

                CGeosetTriangle t = new CGeosetTriangle(CurrentModel);
                CGeosetVertex vn1 = CopyVertex(v1, geoset, up);
                CGeosetVertex vn2 = CopyVertex(v2, geoset, up);
                CGeosetVertex vn3 = CopyVertex(v3, geoset, up);
                geoset.Vertices.Add(vn1);
                geoset.Vertices.Add(vn2);
                geoset.Vertices.Add(vn3);
                t.Vertex1.Attach(vn1);
                t.Vertex2.Attach(vn2);
                t.Vertex3.Attach(vn3);
                created.Add(t);
                geoset.Triangles.Add(t);
            }
        }

        private CGeosetVertex CopyVertex(CGeosetVertex v1, CGeoset geoset, bool up)
        {
            CGeosetVertex v = new CGeosetVertex(CurrentModel);
            v.TexturePosition = new CVector2(v1.TexturePosition);
            v.Position = new CVector3(v1.Position);
            if (up) v.Position.Z += 5;
            v.Normal = new CVector3(v1.Normal);

            return v;
        }

        private CGeoset? GetGeosetOfTriangle(CGeosetTriangle triangle)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                if (geoset.Triangles.Contains(triangle)) { return geoset; }
            }
            return null;
        }

        private void DetachTraingleAsNewGeoset(object? sender, RoutedEventArgs? e)
        {
            DuplicateTrianglesAsNewGeoset(true);
        }

        private void DiplicateInsideGeoset(object? sender, RoutedEventArgs? e)
        {
            DuplicateTrianglesInsideGeoset();
        }
        private void DuplicateTrianglesAsNewGeoset(bool AndDelete = false, bool up = false)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count == 0) { return; }
            CGeoset geo = new CGeoset(CurrentModel);
            List<CGeosetTriangle> created = new List<CGeosetTriangle>();
            foreach (var triangle in triangles)
            {
                var geoset = GetGeosetOfTriangle(triangle);
                if (geoset == null)
                {
                    MessageBox.Show($"Triangle {triangle.ObjectId} is not part of any geoset"); return;
                }

                var v1 = triangle.Vertex1.Object;
                var v2 = triangle.Vertex2.Object;
                var v3 = triangle.Vertex3.Object;

                CGeosetTriangle t = new CGeosetTriangle(CurrentModel);
                CGeosetVertex vn1 = CopyVertex(v1, geoset, up);
                CGeosetVertex vn2 = CopyVertex(v2, geoset, up);
                CGeosetVertex vn3 = CopyVertex(v3, geoset, up);
                geo.Vertices.Add(vn1);
                geo.Vertices.Add(vn2);
                geo.Vertices.Add(vn3);
                t.Vertex1.Attach(vn1);
                t.Vertex2.Attach(vn2);
                t.Vertex3.Attach(vn3);
                if (AndDelete)
                {
                    geoset.Triangles.Remove(triangle);
                }

                geo.Triangles.Add(t);
            }
            CurrentModel.Geosets.Add(geo);

            RefreshGeosetsList(); RefreshGeosets_Triangles();
        }
        private void DuplicateAsNewGeoset(object? sender, RoutedEventArgs? e)
        {
            DuplicateTrianglesAsNewGeoset();
        }

        private void SubdivideTriangle(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }

            foreach (var triangle in triangles)
            {
                foreach (var geoset in CurrentModel.Geosets)
                {
                    if (geoset.Triangles.Contains(triangle))
                    {
                        geoset.Triangles.Remove(triangle);
                        CVector3 centroid = Calculator.GetCentroidofTriangle(triangle);
                        CGeosetVertex v1 = triangle.Vertex1.Object;
                        CGeosetVertex v2 = triangle.Vertex2.Object;
                        CGeosetVertex v3 = triangle.Vertex3.Object;
                        CGeosetVertex center = new CGeosetVertex(CurrentModel);
                        center.Position = new CVector3(centroid);
                        CGeosetTriangle t1 = new CGeosetTriangle(CurrentModel);
                        CGeosetTriangle t2 = new CGeosetTriangle(CurrentModel);
                        CGeosetTriangle t3 = new CGeosetTriangle(CurrentModel);
                        CVector2 centerUV = Calculator.GetCentroidUVFromTriangle(triangle);
                        CVector3 centerVector = Calculator.GetMiddleNormalOfTriangle(triangle);
                        center.TexturePosition = new CVector2(centerUV);
                        center.Normal = new CVector3(centerVector);
                        t1.Vertex1.Attach(v1);
                        t1.Vertex2.Attach(v2);
                        t1.Vertex3.Attach(center);
                        t2.Vertex1.Attach(center);
                        t2.Vertex2.Attach(v2);
                        t2.Vertex3.Attach(v3);
                        t3.Vertex1.Attach(v1);
                        t3.Vertex2.Attach(center);
                        t3.Vertex3.Attach(v3);
                        geoset.Vertices.Add(center);

                        geoset.Triangles.Add(t1);
                        geoset.Triangles.Add(t2);
                        geoset.Triangles.Add(t3);
                    }
                }
            }

            RefreshGeosets_Triangles();
        }

        private void SimplifyTriangleSelection(object? sender, RoutedEventArgs? e)
        {
            //unfinished
        }

        private void InsetTriangles(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            foreach (var triangle in triangles)
            {


                foreach (var geoset in CurrentModel.Geosets)
                {
                    if (geoset.Triangles.Contains(triangle))
                    {
                        Calculator.InsetTriangle(geoset, triangle, CurrentModel);
                    }

                }
            }
            RefreshRenderData(null, null); RefreshGeosets_Triangles();
        }

        private void Bridge2FlatSurfaces(object? sender, RoutedEventArgs? e)
        {
            //unfinished
        }

        private void MirrorFromTriangle(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            //  CGeoset geoset = VerticesBelongToSameGeoset();
            // Calculator.MirrorGeosetFromTriangles(triangles);
            //unfinished
        }

        private void FlattenTriangles(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count < 1) { MessageBox.Show("Select at least 1 triangle"); return; }

            axis_picker a = new axis_picker("Axes");
            if (a.ShowDialog() == true)
            {
                var x = a.axis;
                float firstX = triangles[0].Vertex1.Object.Position.X;
                float firstY = triangles[0].Vertex1.Object.Position.Y;
                float firstZ = triangles[0].Vertex1.Object.Position.Z;
                triangles[0].Vertex2.Object.Position.X = x == Axes.X ? firstX : triangles[0].Vertex2.Object.Position.X;
                triangles[0].Vertex2.Object.Position.Y = x == Axes.Y ? firstY : triangles[0].Vertex2.Object.Position.Y;
                triangles[0].Vertex2.Object.Position.Z = x == Axes.Z ? firstZ : triangles[0].Vertex2.Object.Position.Z;
                triangles[0].Vertex3.Object.Position.X = x == Axes.X ? firstX : triangles[0].Vertex3.Object.Position.X;
                triangles[0].Vertex3.Object.Position.Y = x == Axes.Y ? firstY : triangles[0].Vertex3.Object.Position.Y;
                triangles[0].Vertex3.Object.Position.Z = x == Axes.Z ? firstZ : triangles[0].Vertex3.Object.Position.Z;
                if (triangles.Count < 2) return;
                for (int i = 1; i < triangles.Count; i++)
                {
                    triangles[i].Vertex1.Object.Position.X = x == Axes.X ? firstX : triangles[i].Vertex1.Object.Position.X;
                    triangles[i].Vertex1.Object.Position.Y = x == Axes.Y ? firstY : triangles[i].Vertex1.Object.Position.Y;
                    triangles[i].Vertex1.Object.Position.Z = x == Axes.Z ? firstZ : triangles[i].Vertex1.Object.Position.Z;

                    triangles[i].Vertex2.Object.Position.X = x == Axes.X ? firstX : triangles[i].Vertex2.Object.Position.X;
                    triangles[i].Vertex2.Object.Position.Y = x == Axes.Y ? firstY : triangles[i].Vertex2.Object.Position.Y;
                    triangles[i].Vertex2.Object.Position.Z = x == Axes.Z ? firstZ : triangles[i].Vertex2.Object.Position.Z;
                    triangles[i].Vertex3.Object.Position.X = x == Axes.X ? firstX : triangles[i].Vertex3.Object.Position.X;
                    triangles[i].Vertex3.Object.Position.Y = x == Axes.Y ? firstY : triangles[i].Vertex3.Object.Position.Y;
                    triangles[i].Vertex3.Object.Position.Z = x == Axes.Z ? firstZ : triangles[i].Vertex3.Object.Position.Z;
                }
            }
        }

        private void InsetTrianglesConnected(object? sender, RoutedEventArgs? e)
        {

            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            foreach (var triangle in triangles)
            {


                foreach (var geoset in CurrentModel.Geosets)
                {
                    if (geoset.Triangles.Contains(triangle))
                    {
                        var inset = Calculator.InsetTriangleConnected(geoset, triangle);
                    }

                }
            }
        }

        private void ArrangeVertices(object? sender, RoutedEventArgs? e)
        {
            //unfinished
            // facing axis, distance from centroid
        }


        private void Paste_Merge_Keyframe(object? sender, RoutedEventArgs? e)
        {
            Input i = new Input("New track");
            if (i.ShowDialog() == true)
            {
                bool b = int.TryParse(i.Result, out int pastedOn);
                if (b)
                {
                    if (pastedOn == CopedKeyframe)
                    {
                        MessageBox.Show("Copied cannot be the same as pasted"); return;
                    }
                    else
                    {
                        select_Transformations st = new select_Transformations();
                        if (st.ShowDialog() == true)
                        {
                            bool t = st.Check_T.IsChecked == true;
                            bool r = st.Check_R.IsChecked == true;
                            bool s = st.Check_S.IsChecked == true;
                            foreach (var node in CurrentModel.Nodes)
                            {
                                if (t)
                                {
                                    var kf = node.Translation.FirstOrDefault(x => x.Time == CopedKeyframe);
                                    var pasted = node.Translation.FirstOrDefault(x => x.Time == pastedOn);
                                    if (kf != null)
                                        if (pasted == null)
                                        {
                                            CAnimatorNode<CVector3> n = new()
                                            {
                                                Time = pastedOn,
                                                Value = new CVector3(kf.Value)
                                            };
                                        }

                                }

                                if (r)
                                {
                                    var kf = node.Rotation.FirstOrDefault(x => x.Time == CopedKeyframe);
                                    var pasted = node.Rotation.FirstOrDefault(x => x.Time == pastedOn);
                                    if (kf != null)
                                        if (pasted == null)
                                        {
                                            CAnimatorNode<CVector4> n = new CAnimatorNode<CVector4>
                                            {
                                                Time = pastedOn,
                                                Value = new CVector4(kf.Value)
                                            };
                                        }

                                }
                                if (s)
                                {
                                    var kf = node.Scaling.FirstOrDefault(x => x.Time == CopedKeyframe);
                                    var pasted = node.Scaling.FirstOrDefault(x => x.Time == pastedOn);
                                    if (kf != null)
                                        if (pasted == null)
                                        {
                                            CAnimatorNode<CVector3> n = new CAnimatorNode<CVector3>();
                                            n.Time = pastedOn;
                                            n.Value = new CVector3(kf.Value);
                                        }

                                }
                            }
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Expected an integer"); return;
                }

            }
        }

        private void EditTrianglesUV(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count == 0) { MessageBox.Show("Select at least 1 triangle"); return; }
            InitializeMiniUVMapper(triangles, List_Textures, CurrentModel);


        }

        private void InitializeMiniUVMapper(List<CGeosetTriangle> triangles, ListBox l, CModel m)
        {
            if (MiniUVMapper == null) { MiniUVMapper = new MiniUV(triangles, l, m); }
            else
            {
                MiniUVMapper.Update(triangles, l, m);

            }
            MiniUVMapper.Show();
        }

        private void SwapkeyframesData(object? sender, RoutedEventArgs? e)
        {
            if (List_Keyframes_Animator.SelectedItem != null)
            {
                int inputTrack = Extractor.GetInt(List_Keyframes_Animator);
                Input i = new Input("Target Track");
                if (i.ShowDialog() == true)
                {
                    bool b = int.TryParse(i.Result, out int target);
                    if (b)
                    {
                        if (inputTrack == target)
                        {
                            MessageBox.Show("Both tracks cannot be the same"); return;
                        }
                        if (TrackExistsInSEquences(target, null))
                        {
                            SwapTracksData(inputTrack, target);
                            SetSaved(false);
                            RefreshAnimatedVertexAndNodePositionsForRendering();
                        }
                        else
                        {
                            MessageBox.Show("This track is not contained in any sequence"); return;
                        }
                    }
                }

            }
        }

        private void SwapTracksData(int oldTime, int newTime)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                SwapTrackKeyframes<CVector3>(node.Translation, oldTime, newTime);
                SwapTrackKeyframes<CVector4>(node.Rotation, oldTime, newTime);
                SwapTrackKeyframes<CVector3>(node.Scaling, oldTime, newTime);
            }
        }

        private void SwapTrackKeyframes<T>(CAnimator<T> animator, int oldTime, int newTime) where T : new()
        {
            var oldKey = animator.FirstOrDefault(k => k.Time == oldTime);
            var newKey = animator.FirstOrDefault(k => k.Time == newTime);

            if (oldKey != null && newKey != null)
            {
                var temp = oldKey.Value;
                oldKey.Value = newKey.Value;
                newKey.Value = temp;
            }
            else if (oldKey != null)
            {
                oldKey.Time = newTime;
            }
            else if (newKey != null)
            {
                newKey.Time = oldTime;
            }

            animator.NodeList = animator.NodeList.OrderBy(k => k.Time).ToList();
        }




        private void TextureSpecialInfo(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                var t = GetSElectedTexture();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Geosets:");
                foreach (var mat in CurrentModel.Materials)
                {
                    foreach (var l in mat.Layers)
                    {
                        if (l.Texture.Object == t)
                        {
                            var g = CurrentModel.Geosets.FirstOrDefault(x => x.Material.Object == mat);
                            if (g != null)
                            {
                                sb.AppendLine($"Geoset " + g.ObjectId.ToString());
                            }

                        }
                    }
                }
                sb.AppendLine("Nodes:");
                foreach (INode node in CurrentModel.Nodes)
                {
                    if (node is CParticleEmitter2 em)
                    {
                        if (em.Texture.Object == t)
                        {
                            sb.AppendLine($"Emitter2 '{node.Name}'");
                        }
                    }
                }
                MessageBox.Show(sb.ToString());
            }

        }

        private void mergeHB_Selected(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode? node = GetSelectedNode(); if (node == null) return;
                if (node is CHelper h)
                {

                    if (NodeHasChildren(node) == false)
                    {
                        MessageBox.Show("This node does not have children"); return;
                    }
                    if (HelperHasSingleBone(h))
                    {
                        var children = countrChildrenOfNode(h);
                        MergeHelperBoneOptons mr = new MergeHelperBoneOptons(h, children[0], CurrentModel);
                        if (mr.ShowDialog() == true)
                        {
                            RefreshNodesTree();
                        }
                    }
                    else
                    {
                        MessageBox.Show("This comand requries the target helper to have a sngle child bone"); return;
                    }

                }
                else
                {
                    MessageBox.Show("Select a helper"); return;
                }
            }
        }

        private bool HelperHasSingleBone(CHelper h)
        {
            List<INode> children = countrChildrenOfNode(h);
            if (children.Count != 1) return false;
            return children[0] is CBone;
        }

        private List<INode> countrChildrenOfNode(INode h)
        {
            List<INode> list = new List<INode>();
            foreach (var node in CurrentModel.Nodes)
            {
                if (node.Parent.Node != null)
                {
                    if (node.Parent.Node == h) { list.Add(node); }
                }
            }
            return list;
        }

        private void mergeHB_all(object? sender, RoutedEventArgs? e)
        {
            MergeHelperBoneOptons mr = new MergeHelperBoneOptons(CurrentModel);
            if (mr.ShowDialog() == true)
            {
                RefreshNodesTree();
            }
        }

        private void duplciateLayer(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                var mat = GetSelectedMAterial();
                var layer = mat.Layers[List_Layers.SelectedIndex];
                CMaterialLayer _new_layer = Duplicator.DuplicateLayer(layer, CurrentModel);
                mat.Layers.Add(_new_layer);
                RefreshLayersList();


            }
        }

        private void DuplicateMaterial(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null)
            {
                var mat = GetSelectedMAterial();
                CMaterial _new = new CMaterial(CurrentModel)
                {
                    PriorityPlane = mat.PriorityPlane,
                    SortPrimitivesNearZ = mat.SortPrimitivesNearZ,
                    SortPrimitivesFarZ = mat.SortPrimitivesFarZ,
                    ConstantColor = mat.ConstantColor
                };
                foreach (var layer in mat.Layers)
                {
                    _new.Layers.Add(Duplicator.DuplicateLayer(layer, CurrentModel));
                }

                CurrentModel.Materials.Add(_new);
                RefreshMaterialsList();
            }
        }

        private void DuplicateTa(object? sender, RoutedEventArgs? e)
        {
            if (List_TextureAnims.SelectedItem != null)
            {
                var ta = GetSelectedTextureAnim();

                CurrentModel.TextureAnimations.Add(Duplicator.DuplicateTextureAnim(ta, CurrentModel));
                RefreshTextureAnims();
            }
        }

        private void mergeHB_SelectedN(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode? node = GetSelectedNode();
                if (node is CHelper h)
                {

                    if (NodeHasChildren(node) == false)
                    {
                        MessageBox.Show("This node does not have children"); return;
                    }
                    if (HelperHasSingleBone(h))
                    {
                        var children = countrChildrenOfNode(h);
                        MergeHelperBoneOptons mr = new MergeHelperBoneOptons(h, children[0], CurrentModel, true);
                        if (mr.ShowDialog() == true)
                        {
                            RefreshNodesTree();
                        }
                    }
                    else
                    {
                        MessageBox.Show("This comand requries the target helper to have a sngle child bone"); return;
                    }

                }
                else
                {
                    MessageBox.Show("Select a helper"); return;
                }
            }
        }

        private void SetAnimatorNodeInterpolation_none(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                INode? node = GetSelectedNodeInanimator();
                if (node == null) return;
                transformation_selector ts = new transformation_selector();
                if (ts.ShowDialog() == true)
                {
                    if (ts.C1.IsChecked == true) { node.Translation.Type = EInterpolationType.None; }
                    else if (ts.C2.IsChecked == true) { node.Rotation.Type = EInterpolationType.None; }
                    else if (ts.C3.IsChecked == true) { node.Scaling.Type = EInterpolationType.None; }

                }
            }

        }

        private void SetAnimatorNodeInterpolation_linear(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                INode? node = GetSelectedNodeInanimator(); if (node == null) return;
                transformation_selector ts = new transformation_selector();
                if (ts.ShowDialog() == true)
                {
                    if (ts.C1.IsChecked == true) { node.Translation.Type = EInterpolationType.Linear; }
                    else if (ts.C2.IsChecked == true) { node.Rotation.Type = EInterpolationType.Linear; }
                    else if (ts.C3.IsChecked == true) { node.Scaling.Type = EInterpolationType.Linear; }

                }
            }
        }

        private void SetAnimatorNodeInterpolation_bezier(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                INode? node = GetSelectedNodeInanimator(); if (node == null) return;
                transformation_selector ts = new transformation_selector();
                if (ts.ShowDialog() == true)
                {
                    if (ts.C1.IsChecked == true) { node.Translation.Type = EInterpolationType.Bezier; }
                    else if (ts.C2.IsChecked == true) { node.Rotation.Type = EInterpolationType.Bezier; }
                    else if (ts.C3.IsChecked == true) { node.Scaling.Type = EInterpolationType.Bezier; }

                }
            }
        }

        private void SetAnimatorNodeVis_True(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                var node = GetSelectedNodeInanimator();
                int track = Extractor.GetInt(List_Keyframes_Animator);
                if (TrackExistsInSEquences(track, null) == false)
                {
                    MessageBox.Show("The selected track does not exist in any sequence"); return;
                }

                if (node is CAttachment att)
                {
                    ChangeVisibilityOfNodeInTrack(att.Visibility, track, true);
                }
                else if (node is CParticleEmitter em)
                {
                    ChangeVisibilityOfNodeInTrack(em.Visibility, track, true);
                }
                else if (node is CParticleEmitter2 em2)
                {
                    ChangeVisibilityOfNodeInTrack(em2.Visibility, track, true);
                }
                else if (node is CRibbonEmitter r)
                {
                    ChangeVisibilityOfNodeInTrack(r.Visibility, track, true);
                }
                else if (node is CLight l)
                {
                    ChangeVisibilityOfNodeInTrack(l.Visibility, track, true);
                }
            }
        }

        private static void ChangeVisibilityOfNodeInTrack(CAnimator<float> visibility, int track, bool v)
        {
            var f = visibility.First(x => x.Time == track);
            if (f == null)
            {
                CAnimatorNode<float> n = new CAnimatorNode<float>();
                n.Time = track;
                n.Value = v ? 1 : 0;
                visibility.Add(n);
                visibility.NodeList = visibility.NodeList.OrderBy(x => x.Time).ToList();
            }
            else
            {
                f.Value = v ? 1 : 0;
            }
        }

        private void SetAnimatorNodeVis_False(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                var node = GetSelectedNodeInanimator();
                int frame = Extractor.GetInt(List_Keyframes_Animator);
                if (TrackExistsInSEquences(frame, null) == false)
                {
                    MessageBox.Show("The selected track does not exist in any sequence"); return;
                }

                if (node is CAttachment att)
                {
                    ChangeVisibilityOfNodeInTrack(att.Visibility, frame, false);
                }
                else if (node is CParticleEmitter em)
                {
                    ChangeVisibilityOfNodeInTrack(em.Visibility, frame, false);
                }
                else if (node is CParticleEmitter2 em2)
                {
                    ChangeVisibilityOfNodeInTrack(em2.Visibility, frame, false);
                }
                else if (node is CRibbonEmitter r)
                {
                    ChangeVisibilityOfNodeInTrack(r.Visibility, frame, false);
                }
                else if (node is CLight l)
                {
                    ChangeVisibilityOfNodeInTrack(l.Visibility, frame, false);
                }
            }
        }

        private void SetAnimatorNodeInterpolation_hermite(object? sender, RoutedEventArgs? e)
        {
            if (List_Nodes_Animator.SelectedItem != null)
            {
                INode? node = GetSelectedNodeInanimator(); if (node == null) { MessageBox.Show("Select a node"); return; }
                transformation_selector ts = new transformation_selector();
                if (ts.ShowDialog() == true)
                {
                    if (ts.C1.IsChecked == true) { node.Translation.Type = EInterpolationType.Hermite; }
                    else if (ts.C2.IsChecked == true) { node.Rotation.Type = EInterpolationType.Hermite; }
                    else if (ts.C3.IsChecked == true) { node.Scaling.Type = EInterpolationType.Hermite; }

                }
            }
        }

        private void App_window_PreviewMouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is UIElement element && element.Focusable)
            {
                element.Focus(); // Force focus on the clicked control

            }
        }

        private void createblastFlare(object? sender, RoutedEventArgs? e)
        {
            INode cloned = NodeCloner.Clone(NodeMaker.BlastFlare, CurrentModel);
            HandleRequiredTexture((CParticleEmitter2)cloned);
            cloned.Name = "BlastFlare_" + IDCounter.Next_; ;
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
            SetSaved(false);
        }

        private void createFire(object? sender, RoutedEventArgs? e)
        {
            INode cloned = NodeCloner.Clone(NodeMaker.Fire, CurrentModel);
            HandleRequiredTexture((CParticleEmitter2)cloned);
            cloned.Name = "Fire_" + IDCounter.Next_; ;
            CurrentModel.Nodes.Add(cloned);
            RefreshNodesTree();
            SetSaved(false);
        }



        private void ImportGeosetm(object? sender, RoutedEventArgs? e)
        {
            string openPath = FileSeeker.OpenTGeomFileDialog();

            if (openPath.Length > 0)
            {
                Pause = true;
                CGeoset? imported = GeosetExporter.ReadGeomerge(openPath, CurrentModel);
                if (imported == null) return;
                SetSaved(false);
                refreshalllists(null, null);
                CollectTexturesOpenGL();
                Pause = false;
            }
        }

        private void ExportGeosetm(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                var g = GetSelectedGeosets();
                GeosetExporter.ExportGeomerge(CurrentModel, g[0]);
            }
            else
            {
                MessageBox.Show("select a single geoset");
            }

        }

        private void og(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_Geometry);

        }
        private List<MenuItem> RenderOptionItems = new List<MenuItem>();
        private void FillRenderItemsCollection()
        {
            RenderOptionItems.Add(Menuitem_edges);
            RenderOptionItems.Add(Menuitem_Geometry);
            RenderOptionItems.Add(Menuitem_vertices);
            RenderOptionItems.Add(Menuitem_Nodes);
            RenderOptionItems.Add(Menuitem_skeleton);
            RenderOptionItems.Add(Menuitem_skinning);
            RenderOptionItems.Add(Menuitem_Cols);
        }
        private void EnabbleOnlyRender(params MenuItem[] items)
        {
            foreach (var tem in RenderOptionItems)
            {
                tem.IsChecked = false;
            }
            for (int i = 0; i < items.Length; i++)
            {

                items[i].IsChecked = true;

            }

        }

        private void edg(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_edges, Menuitem_Geometry);
        }

        private void edg2(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_edges);
        }

        private void verts(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_vertices);
        }

        private void verts2(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_vertices, Menuitem_edges);
        }

        private void nds(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_Nodes);
        }

        private void nds2(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_Nodes, Menuitem_edges);
        }

        private void ndss(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_Nodes, Menuitem_skeleton);
        }

        private void cols4(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_Cols);
        }

        private void onlyskinning(object? sender, RoutedEventArgs? e)
        {
            EnabbleOnlyRender(Menuitem_skinning);
        }

        private void gridson(object? sender, RoutedEventArgs? e)
        {
            Menuitem_groundGrid.IsChecked = true;
            Menuitem_groundGridX.IsChecked = true;
            Menuitem_groundGridY.IsChecked = true;
        }

        private void gridsoff(object? sender, RoutedEventArgs? e)
        {
            Menuitem_groundGrid.IsChecked = false;
            Menuitem_groundGridX.IsChecked = false;
            Menuitem_groundGridY.IsChecked = false;
        }

        private void EditRenderPreferences(object? sender, RoutedEventArgs? e)
        {
            Render_Preferences rs = new Render_Preferences();
            rs.ShowDialog();
            SaveSettings();
        }

        private void ViewBig(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                ListBoxItem? i = List_Textures.SelectedItem as ListBoxItem; if (i == null) return;

                Image? m = i.ToolTip as Image; if (m == null) return;
                ImageViewer? v = new ImageViewer(m); if (v == null) return;
                v.ShowDialog();

            }
        }

        private void setColorize(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;

            ColorizeTransformations = Menuitem_colorize.IsChecked == true;
            SaveSettings();
            RefreshSequencesList();
            RefreshNodesTree();
            RefreshPathNodesList();
        }

        private void ShowUnAnimatedGeometryInfo(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
                return;
            }

            if (CurrentModel.Geosets.Count == 0)
            {
                MessageBox.Show("There are no geosets. No geometry to check");
                return;
            }

            var bones = CurrentModel.Nodes.Where(x => x is CBone).ToList();
            if (bones.Count == 0)
            {
                MessageBox.Show("There are no bones");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("These sequences don't have animated geometry:");

            List<CSequence> unAnimated = new List<CSequence>();

            foreach (var sequence in CurrentModel.Sequences)
            {
                bool found = false;

                foreach (var bone in bones)
                {
                    bool hasKeyframeInSequence = bone.Translation.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd) ||
                                                 bone.Rotation.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd) ||
                                                 bone.Scaling.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);

                    if (!hasKeyframeInSequence)
                        continue;

                    foreach (var geoset in CurrentModel.Geosets)
                    {
                        foreach (var group in geoset.Groups)
                        {
                            foreach (var gnode in group.Nodes)
                            {
                                if (gnode.Node.Node != bone)
                                    continue;

                                // This bone is used by this group
                                if (geoset.Vertices.Any(vertex => vertex.Group.Object == group))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                        if (found) break;
                    }
                    if (found) break;
                }

                if (!found)
                {
                    unAnimated.Add(sequence);
                }
            }

            foreach (var sequence in unAnimated)
            {
                sb.AppendLine(sequence.Name);
            }

            new TextViewer(sb.ToString()).ShowDialog();
        }


        private void show_w24(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Nodes.Count == 0) { MessageBox.Show("There are no nodes"); return; }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("These nodes are not animated");
            foreach (var bone in CurrentModel.Nodes)
            {

                if (bone.Translation.Count == 0 && bone.Rotation.Count == 0 && bone.Scaling.Count == 0)
                {
                    sb.AppendLine(bone.Name);
                }
            }
            TextViewer tw = new TextViewer(sb.ToString()); tw.ShowDialog();
        }

        private void show_w25(object? sender, RoutedEventArgs? e)
        {

            var bones = CurrentModel.Nodes.Where(x => x is CBone).ToList();
            if (bones.Count == 0) { MessageBox.Show("There are no bones"); return; }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("These bones are not animated");
            foreach (var bone in bones)
            {

                if (bone.Translation.Count == 0 && bone.Rotation.Count == 0 && bone.Scaling.Count == 0)
                {
                    sb.AppendLine(bone.Name);
                }
            }
            TextViewer tw = new TextViewer(sb.ToString()); tw.ShowDialog();
        }

        private void setDrag(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            CanDrag = Menuitem_allowDrag.IsChecked == true;
            SaveSettings();
        }
        private Vector3 CopiedFields = new Vector3();
        private void copy(object? sender, RoutedEventArgs? e)
        {
            bool t1 = float.TryParse(InputAnimatorX.Text, out float x);
            bool t2 = float.TryParse(InputAnimatorY.Text, out float y);
            bool t3 = float.TryParse(InputAnimatorZ.Text, out float z);
            CopiedFields.X = t1 ? x : 0;
            CopiedFields.Y = t2 ? y : 0;
            CopiedFields.Z = t3 ? z : 0;

        }

        private void paste1(object? sender, RoutedEventArgs? e)
        {
            PasteButton.ContextMenu.IsOpen = true;
        }

        private void pasteX(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.None, PasteType.None);
        }
        private Vector3 InitialFields = new Vector3();
        private void GetInitialFields()
        {
            bool t1 = float.TryParse(InputAnimatorX.Text, out float x);
            bool t2 = float.TryParse(InputAnimatorY.Text, out float y);
            bool t3 = float.TryParse(InputAnimatorZ.Text, out float z);
            InitialFields.X = t1 ? x : 0;
            InitialFields.Y = t2 ? y : 0;
            InitialFields.Z = t3 ? z : 0;
        }
        private void PasteTransformaton(PasteType x, PasteType y, PasteType z)
        {
            GetInitialFields();

            Vector3 modified = new Vector3(InitialFields.X, InitialFields.Y, InitialFields.Z);
            if (x == PasteType.Normal) { modified.X = CopiedFields.X; }
            if (y == PasteType.Normal) { modified.Y = CopiedFields.Y; }
            if (z == PasteType.Normal) { modified.Z = CopiedFields.Z; }
            if (x == PasteType.Negated) { modified.X = -CopiedFields.X; }
            if (y == PasteType.Negated) { modified.Y = -CopiedFields.Y; }
            if (z == PasteType.Negated) { modified.Z = -CopiedFields.Z; }

            InputAnimatorX.Text = modified.X.ToString();
            InputAnimatorY.Text = modified.Y.ToString();
            InputAnimatorZ.Text = modified.Z.ToString();

            ModifyByX(modified.X);
            ModifyByY(modified.Y);
            ModifyByZ(modified.Z);

        }

        private void pasteY(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.None, PasteType.Normal, PasteType.None);
        }

        private void pasteZ(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.None, PasteType.None, PasteType.Normal);
        }

        private void pasteXY(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.Normal, PasteType.None);
        }

        private void pasteZY(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.None, PasteType.Normal, PasteType.Normal);
        }

        private void pasteXZ(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.None, PasteType.Normal);
        }

        private void pasteAll(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.Normal, PasteType.Normal);
        }

        private void pasteXn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Negated, PasteType.None, PasteType.None);
        }

        private void pasteYn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.None, PasteType.Negated, PasteType.None);
        }

        private void pasteZn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.None, PasteType.None, PasteType.Negated);
        }

        private void pasteXYn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Negated, PasteType.Negated, PasteType.None);
        }

        private void pasteZYn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.None, PasteType.Negated, PasteType.Negated);
        }

        private void pasteXZn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Negated, PasteType.None, PasteType.Negated);
        }

        private void pasteXnn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Negated, PasteType.Normal, PasteType.Normal);
        }

        private void pasteYnn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.Negated, PasteType.Normal);
        }

        private void pasteZnn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.Normal, PasteType.Negated);
        }

        private void pasteXYnn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Negated, PasteType.Negated, PasteType.Normal);
        }

        private void pasteZYnn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Normal, PasteType.Negated, PasteType.Negated);
        }

        private void pasteXZnn(object? sender, RoutedEventArgs? e)
        {
            PasteTransformaton(PasteType.Negated, PasteType.Normal, PasteType.Negated);
        }

        private void copySeqTr(object? sender, RoutedEventArgs? e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                CopiedSequence = GetSelectedSequence();
            }
            else
            {
                MessageBox.Show("Select a sequence");
            }
        }

        private void PasteTransformationsOnSequence(object? sender, RoutedEventArgs? e)
        {
            if (CopiedSequence == null)
            {
                MessageBox.Show("Nothing was copied");
                return;
            }

            if (!CurrentModel.Sequences.Contains(CopiedSequence))
            {
                MessageBox.Show("Sequence is not part of the model");
                return;
            }
            var seq = GetSelectedSequence();
            if (seq == CopiedSequence)
            {
                MessageBox.Show("Copied sequence cannot be the same as the pasted sequence");
                return;
            }
            int from = CopiedSequence.IntervalStart;
            int to = CopiedSequence.IntervalEnd;
            int from2 = seq.IntervalStart;
            int to2 = seq.IntervalEnd;
            if (CopiedSequence.Interval <= 10) { MessageBox.Show("Copied sequence has invalid/too small interval"); return; }
            if (seq.Interval <= 10) { MessageBox.Show("Target sequence has invalid/too small interval"); return; }

            select_Transformations st = new select_Transformations();
            st.ShowDialog();
            if (st.DialogResult == true)
            {
                bool translation = st.Check_T.IsChecked == true;
                bool rotation = st.Check_R.IsChecked == true;
                bool scaling = st.Check_S.IsChecked == true;



                foreach (var node in CurrentModel.Nodes)
                {
                    if (translation)
                    {
                        var keyframes = node.Translation.NodeList
                            .Where(x => x.Time >= from && x.Time <= to)
                            .ToList();
                        if (keyframes.Count > 0)
                        {
                            CopySequenceKeyframesToSequence(keyframes, node.Translation.NodeList, from, to, from2, to2);
                            node.Scaling.NodeList = node.Translation.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }

                    if (rotation)
                    {
                        var keyframes = node.Rotation.NodeList
                            .Where(x => x.Time >= from && x.Time <= to)
                            .ToList();
                        if (keyframes.Count > 0)
                        {
                            CopySequenceKeyframesToSequence(keyframes, node.Rotation.NodeList, from, to, from2, to2);
                            node.Rotation.NodeList = node.Rotation.NodeList.OrderBy(x => x.Time).ToList();
                        }

                    }

                    if (scaling)
                    {
                        var keyframes = node.Scaling.NodeList
                            .Where(x => x.Time >= from && x.Time <= to)
                            .ToList();
                        if (keyframes.Count > 0)
                        {
                            CopySequenceKeyframesToSequence(keyframes, node.Scaling.NodeList, from, to, from2, to2);
                            node.Scaling.NodeList = node.Scaling.NodeList.OrderBy(x => x.Time).ToList();
                        }

                    }
                }
            }
        }

        private static void CopySequenceKeyframesToSequence(List<CAnimatorNode<CVector3>> keyframes, List<CAnimatorNode<CVector3>> nodeList, int from, int to, int from2, int to2)
        {
            int sourceDuration = to - from;
            int destDuration = to2 - from2;

            if (sourceDuration > destDuration)
            {
                // Optionally show a message:
                // MessageBox.Show("Cannot paste: copied animation is longer than the target sequence.");
                return;
            }

            float scale = (float)destDuration / sourceDuration;

            foreach (var keyframe in keyframes)
            {
                int newTime = from2 + (int)((keyframe.Time - from) * scale);
                nodeList.Add(new CAnimatorNode<CVector3>(newTime, keyframe.Value));
            }
        }


        private static void CopySequenceKeyframesToSequence(List<CAnimatorNode<CVector4>> keyframes, List<CAnimatorNode<CVector4>> nodeList, int from, int to, int from2, int to2)
        {
            int sourceDuration = to - from;
            int destDuration = to2 - from2;

            if (sourceDuration > destDuration)
            {
                // Optionally show a message:
                // MessageBox.Show("Cannot paste: copied animation is longer than the target sequence.");
                return;
            }

            float scale = (float)destDuration / sourceDuration;

            foreach (var keyframe in keyframes)
            {
                int newTime = from2 + (int)((keyframe.Time - from) * scale);
                nodeList.Add(new CAnimatorNode<CVector4>(newTime, keyframe.Value));
            }
        }

        private void copyVis(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with"); return;
            }

            SequenceToSequencesSelector s = new SequenceToSequencesSelector(CurrentModel.Sequences.ObjectList, CurrentModel.GeosetAnimations.ObjectList);
            s.ShowDialog();
        }

        private void DeleteSequencesContainingKeyword(object? sender, RoutedEventArgs? e)
        {
            if (CurrentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; }
            Input i = new Input("");
            if (i.ShowDialog() == true)
            {
                string s = i.Result.Trim().ToLower();
                var matches = CurrentModel.Sequences.Where(w => w.Name.ToLower().Contains(s)).ToList();
                foreach (var sequence in matches)
                {

                    ClearAnimations(sequence, true);


                }
                RefreshSequencesList_Paths();
                RefreshPath_ModelNodes_List();
                RefreshSequencesList();
                SetSaved(false);
            }
        }

        private void DelBonesGeometryBone(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            INode? node = GetSelectedNode(); if (node == null) { MessageBox.Show("Null node"); return; }
            if (NodeHasChildren(node)) { MessageBox.Show("Cannot delete a node with children"); return; }
            foreach (var geoset in CurrentModel.Geosets)
            {
                var vertices = geoset.Vertices.ToList();
                foreach (var vertex in vertices)
                {
                    if (vertexIsAttachedToNode(vertex, node))
                    {
                        geoset.Vertices.Remove(vertex);
                    }
                }
            }
            Optimizer.RemoveEmptyGeosets(CurrentModel);
            RefreshNodesTree();
            RefreshGeosetsList();
        }

        private static bool vertexIsAttachedToNode(CGeosetVertex vertex, INode node)
        {
            foreach (var gnode in vertex.Group.Object.Nodes)
            {
                if (gnode.Node.Node == node) return true;
            }
            return false;
        }

        private void straghten(object? sender, RoutedEventArgs? e)
        {
            var g = GetSelectedGeosets();
            if (g.Count == 0) { MessageBox.Show("Select at least 1 geoset"); return; }
            axis_picker ap = new axis_picker("Select Axes");
            if (ap.ShowDialog() == true)
            {
                var axes = ap.axis;
                foreach (var geoset in g)
                {
                    Calculator.Straighten(g, axes);
                }

            }

        }

        private void replaceTexture(object? sender, RoutedEventArgs? e)
        {
            if (List_Textures.SelectedItem != null)
            {
                var textture = GetSElectedTexture();
                var path = GetTextureFromInput();
                if (path.Length != 0)
                {
                    textture.FileName = path;
                    textture.ReplaceableId = 0;
                    RefreshTexturesList();
                    RefreshRenderData(null, null);
                }

            }
        }
        private string GetTextureFromInput()
        {
            Input i = new Input("");
            if (i.ShowDialog() == true)
            {
                string text = i.box.Text.Trim();
                if (text.Length == 0) { MessageBox.Show("Empty input"); return ""; }
                if (MPQHelper.FileExists(text))
                {
                    CTexture texture = new CTexture(CurrentModel)
                    {
                        FileName = text
                    };
                    return text;

                }
                else
                {
                    MessageBox.Show($"The texture at '{text}' was not found. Not added."); return "";
                }
            }
            return "";
        }

        private void makeGAUseColor(object? sender, RoutedEventArgs? e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                CGeosetAnimation selected = GetSelectedGeosetAnimation();
                selected.UseColor = !selected.UseColor;
                RefreshGeosetAnimationsList();
                SetSaved(false);
            }
        }

        private void mcc(object? sender, RoutedEventArgs? e)
        {
            Model_Colors_Changer mc = new Model_Colors_Changer(CurrentModel);
            if (mc.ShowDialog() == true)
            {
                RefreshTexturesList();
                RefreshRenderData(null, null);
            }
        }
        static string CopyAnimator(CAnimator<CVector3> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in animator)
            {
                sb.AppendLine($"{v.Time}: {v.Value}");
            }
            return sb.ToString();

        }
        static string CopyAnimator(CAnimator<CVector4> animator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in animator)
            {
                sb.AppendLine($"{v.Time}: {v.Value}");
            }
            return sb.ToString();

        }
        private void ckc_t(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            INode? NODE = GetSelectedNode(); if (NODE == null) { MessageBox.Show("Null node"); return; }
            Clipboard.SetText(CopyAnimator(NODE.Translation));
        }

        private void ckc_r(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            INode? NODE = GetSelectedNode(); if (NODE == null) { MessageBox.Show("Select a node"); return; }
            Clipboard.SetText(CopyAnimator(NODE.Rotation));
        }

        private void ckc_s(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            INode? NODE = GetSelectedNode(); if (NODE == null) { MessageBox.Show("Select a node"); return; }
            Clipboard.SetText(CopyAnimator(NODE.Scaling));
        }

        private void pkc_t(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            var NODE = GetSelectedNode();
            PasteKeyframes_InNode(NODE, TransformationType.Translation);
        }

        private static void PasteKeyframes_InNode(INode? node, TransformationType t)
        {
            if (node == null) return;
            string text = Clipboard.GetText();
            if (text.Length == 0) { MessageBox.Show("Nothing in the clipboard"); return; }
            var lines = text.Split('\n').ToArray();
            CAnimator<CVector3>? animator1 = null;
            CAnimator<CVector4>? animator2 = null;
            if (t == TransformationType.Translation) { animator1 = node.Translation; }
            if (t == TransformationType.Scaling) { animator1 = node.Scaling; }
            if (t == TransformationType.Rotation) { animator2 = node.Rotation; }
            if (animator1 != null)
            {
                animator1.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    CAnimatorNode<CVector3>? kf = Calculator.ReadLine3(lines[i]);
                    if (kf != null) { animator1.Add(kf); }
                }
            }
            else if (animator2 != null)
            {
                animator2.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    CAnimatorNode<CVector4>? kf = Calculator.ReadLine4(lines[i]);
                    if (kf != null) { animator2.Add(kf); }
                }

            }

        }

        private void pkc_r(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            INode? NODE = GetSelectedNode();
            PasteKeyframes_InNode(NODE, TransformationType.Rotation);
        }

        private void pkc_s(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { MessageBox.Show("Select a node"); return; }
            var NODE = GetSelectedNode();
            PasteKeyframes_InNode(NODE, TransformationType.Scaling);
        }

        private void ForceNormalsOutward(object? sender, RoutedEventArgs? e)
        {
            var geosets = GetSelectedGeosets();
            foreach (var geoset in geosets)
            {
                foreach (var trinagle in geoset.Triangles)
                {
                    Calculator.ForceNormalsDirection(trinagle, false);
                }
            }
        }

        private void autorig(object? sender, RoutedEventArgs? e)
        {
            var g = GetSelectedGeosets();
            if (g.Count == 0) { MessageBox.Show("Select at least 1 geoset"); return; }
            List<INode> bones = CurrentModel.Nodes.Where(x => x is CBone).ToList();
            if (bones.Count == 0) { MessageBox.Show("There are no bones"); return; }
            foreach (var geoses in g)
            {
                geoses.Groups.Clear();
                foreach (var verex in geoses.Vertices)
                {
                    INode? closest = Calculator.GetClosestNode(verex.Position, bones);
                    if (closest == null) continue;
                    CGeosetGroup group = new CGeosetGroup(CurrentModel);
                    CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                    gnode.Node.Attach(closest);

                    group.Nodes.Add(gnode);
                    geoses.Groups.Add(group);
                    verex.Group.Attach(group);
                }

            }
            Optimizer.MinimizeMatrixGroups_(CurrentModel);
            RefreshGeosetsList();
        }

        private void copyRig(object? sender, RoutedEventArgs? e)
        {
            var g = GetSelectedGeosets();
            if (g.Count != 1) { MessageBox.Show("Select only 1 geoset"); return; }
            CopiedRigGeoset = g[0];
        }

        private void pRig(object? sender, RoutedEventArgs? e)
        {
            var g = GetSelectedGeosets();
            if (g.Count != 1) { MessageBox.Show("Select only 1 geoset"); return; }
            var pasted = g[0];
            if (CopiedRigGeoset == pasted) { MessageBox.Show("Pasted and coped cannot be the same"); return; }
            if (CopiedRigGeoset == null) { MessageBox.Show("Nothing was copied"); return; }
            if (!CurrentModel.Geosets.Contains(CopiedRigGeoset)) { MessageBox.Show("Geoset not in model"); return; }
            if (CopiedRigGeoset.Groups.Count != 1) { MessageBox.Show("The geoset must have one  matrix group"); return; }
            CGeosetGroup copy = new CGeosetGroup(CurrentModel);
            foreach (var node in CopiedRigGeoset.Groups[0].Nodes)
            {
                CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                gnode.Node.Attach(node.Node.Node);
                copy.Nodes.Add(gnode);
            }
            pasted.Groups.Add(copy);
            RefreshGeosetsList();
        }

        private void setModeUVu(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.DragUV_U;
            HightlightMoreButton();
        }

        private void setModeUVv(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.DragUV_V;
            HightlightMoreButton();
        }

        private void HightlightMoreButton()
        {
            ButtonManual_More.BorderBrush = Brushes.Green;
            ButtonManual_Rotate.BorderBrush = Brushes.Gray;
            ButtonManual_Scale.BorderBrush = Brushes.Gray;
            ButtonManual_Move.BorderBrush = Brushes.Gray;
        }

        private void ssetModecaleUV(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.ScaleUV;
            HightlightMoreButton();
        }

        private void ssetModecaleUVu(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.ScaleUVu;
            HightlightMoreButton();
        }

        private void ssetModecaleUVv(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.ScaleUVv;
            HightlightMoreButton();
        }

        private void ForceIN(object? sender, RoutedEventArgs? e)
        {
            var geosets = GetSelectedGeosets();
            foreach (var geoset in geosets)
            {
                foreach (var trinagle in geoset.Triangles)
                {
                    Calculator.ForceNormalsDirection(trinagle, true);
                }
            }
        }

        private void seModeRotateNormals(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.RotateNormals;
            HightlightMoreButton();
        }

        private void toggleLghting(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            RenderSettings.RenderLighing = Menuitem_Lightng.IsChecked == true;

            SaveSettings();
        }

        private void seModeRotateEach(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.RotateEach;
            HightlightMoreButton();

        }

        private void seModeScaleEach(object? sender, RoutedEventArgs? e)
        {
            modifyMode_current = ModifyMode.ScaleEach;
            HightlightMoreButton();

        }
        private static float GetFloat(TextBox t)
        {
            return float.TryParse(t.Text, out float v) ? v : 0;
        }
        private static float GetFloat(string t)
        {
            return float.TryParse(t, out float v) ? v : 0;
        }
        private void copyField(object? sender, RoutedEventArgs? e)
        {
            var parent = VisualTreeHelper.GetParent(sender as MenuItem) as ContextMenu;
            if (parent == InputMenuX) { Clipboard.SetText(GetFloat(InputAnimatorX).ToString()); }
            else if (parent == InputMenuY) { Clipboard.SetText(GetFloat(InputAnimatorY).ToString()); }
            else if (parent == InputMenuZ) { Clipboard.SetText(GetFloat(InputAnimatorZ).ToString()); }

        }



        private void pasteField(object? sender, RoutedEventArgs? e)
        {
            var parent = VisualTreeHelper.GetParent(sender as MenuItem) as ContextMenu;
            if (parent == InputMenuX) { InputAnimatorX.Text = GetFloat(Clipboard.GetText()).ToString(); }
            else if (parent == InputMenuY) { InputAnimatorY.Text = GetFloat(Clipboard.GetText()).ToString(); }
            else if (parent == InputMenuZ) { InputAnimatorZ.Text = GetFloat(Clipboard.GetText()).ToString(); }

        }


        private void negateField(object? sender, RoutedEventArgs? e)
        {
            var parent = sender as MenuItem;
            if (parent == Menuitem_NegateX)
            {
                InputAnimatorX.Text = (-GetFloat(InputAnimatorX.Text)).ToString();
            }
            else if (parent == Menuitem_NegateY) { InputAnimatorY.Text = (-GetFloat(InputAnimatorY.Text)).ToString(); }
            else if (parent == Menuitem_NegateZ) { InputAnimatorZ.Text = (-GetFloat(InputAnimatorZ.Text)).ToString(); }

        }

        private void CenterField(object? sender, RoutedEventArgs? e)
        {
            var parent = sender as MenuItem;
            if (parent == Menuitem_CenterX)
            {
                InputAnimatorX.Text = "0";
            }
            else if (parent == Menuitem_CenterY) { InputAnimatorY.Text = "0"; }
            else if (parent == Menuitem_CenterZ) { InputAnimatorZ.Text = "0"; }

        }

        private void AlignField(object? sender, RoutedEventArgs? e)
        {

        }


        private void setAll(object? sender, RoutedEventArgs? e)
        {

            float x = GetFloat(InputAnimatorX);
            float y = GetFloat(InputAnimatorY);
            float z = GetFloat(InputAnimatorZ);
            ModifyByX(x);
            ModifyByY(y);
            ModifyByZ(z);
        }


        private void ClearAllAnimationsOfSequence_t(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Translation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
                node.Scaling.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
                node.Rotation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
            SetSaved(false);
            RefreshSequencesList();
        }

        private void clearsequencetranslations_t(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Translation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
            SetSaved(false);
            RefreshSequencesList();
        }

        private void clearsequencescalings_t(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Scaling.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
            SetSaved(false);
            RefreshSequencesList();
        }

        private void clearsequencerotations_t(object? sender, RoutedEventArgs? e)
        {
            var sequence = GetSelectedSequence();
            foreach (var node in CurrentModel.Nodes)
            {
                node.Rotation.NodeList.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            }
            SetSaved(false);
            RefreshSequencesList();
        }

        private void edituv_mini(object? sender, RoutedEventArgs? e)
        {
            var s = GetSelectedGeosets();

            if (s.Count == 1)
            {
                InitializeMiniUVMapper(s[0].Triangles.ObjectList, List_Textures, CurrentModel);
            }
            else
            {
                MessageBox.Show("Select a single geoset"); return;
            }
        }

        private void ImportSequence(object? sender, RoutedEventArgs? e)
        {

        }

        private void ImportTexturesm(object? sender, RoutedEventArgs? e)
        {

            CModel? TemporaryModel = GetTemporaryModel();
            if (TemporaryModel == null) { return; }

            Dictionary<int, CTexture> reference = new Dictionary<int, CTexture>();
            foreach (CTexture tx in TemporaryModel.Textures)
            {
                if (tx.ReplaceableId == 0)
                {
                    var existing = CurrentModel.Textures.First(x => x.FileName == tx.FileName);
                    if (existing == null)
                    {
                        CTexture newTexture = new CTexture(CurrentModel)
                        {
                            FileName = tx.FileName,
                            WrapWidth = tx.WrapWidth,
                            WrapHeight = tx.WrapHeight,
                            ReplaceableId = tx.ReplaceableId
                        };
                        CurrentModel.Textures.Add(newTexture);
                        reference.Add(tx.ObjectId, newTexture);
                    }
                    else
                    {
                        reference.Add(tx.ObjectId, existing);
                    }
                }
                else
                {
                    var existing = CurrentModel.Textures.First(x => x.ReplaceableId == tx.ReplaceableId);
                    if (existing == null)
                    {
                        CTexture newTexture = new CTexture(CurrentModel);
                        newTexture.ReplaceableId = tx.ReplaceableId;

                        CurrentModel.Textures.Add(newTexture);
                        reference.Add(tx.ObjectId, newTexture);
                    }
                    else
                    {
                        reference.Add(tx.ObjectId, existing);
                    }
                }



            }
            foreach (var original in TemporaryModel.Materials)
            {
                CMaterial duplicated = new CMaterial(CurrentModel);
                duplicated.SortPrimitivesFarZ = duplicated.SortPrimitivesFarZ;
                duplicated.SortPrimitivesNearZ = original.SortPrimitivesNearZ;
                duplicated.FullResolution = original.FullResolution;
                duplicated.ConstantColor = original.ConstantColor;
                duplicated.PriorityPlane = original.PriorityPlane;
                foreach (var layer in original.Layers)
                {
                    CMaterialLayer l = new(CurrentModel);
                    l.Unshaded = layer.Unshaded;
                    l.Unfogged = layer.Unfogged;
                    l.TwoSided = layer.TwoSided;
                    l.SphereEnvironmentMap = layer.SphereEnvironmentMap;
                    l.NoDepthSet = layer.NoDepthSet;
                    l.NoDepthTest = layer.NoDepthTest;
                    l.FilterMode = layer.FilterMode;
                    if (layer.Alpha.Static) { l.Alpha.MakeStatic(layer.Alpha.GetValue()); }
                    else
                    {
                        l.Alpha.MakeAnimated();
                        foreach (var kf in layer.Alpha)
                        {
                            CAnimatorNode<float> n = new CAnimatorNode<float>
                            {
                                Time = kf.Time,
                                Value = kf.Value
                            };
                            l.Alpha.Add(n);
                        }
                        l.Alpha.NodeList = l.Alpha.NodeList.OrderBy(x => x.Time).ToList();
                    }
                    if (layer.Texture.Object != null)
                    {
                        var tx = layer.Texture.Object;
                        l.Texture.Attach(reference[tx.ObjectId]);

                    }
                    if (layer.TextureId.Static)
                    {
                        l.TextureId.MakeStatic(layer.TextureId.GetValue());
                    }
                    else
                    {
                        l.TextureId.MakeAnimated();
                        foreach (var kf in layer.TextureId)
                        {
                            CAnimatorNode<int> n = new CAnimatorNode<int>();
                            n.Time = kf.Time;
                            n.Value = reference[kf.Value].ObjectId;
                            l.TextureId.Add(n);
                        }
                        l.TextureId.NodeList = l.TextureId.NodeList.OrderBy(x => x.Time).ToList();
                    }

                }
                CurrentModel.Materials.Add(duplicated);
            }
            RefreshTexturesList();
            RefreshMaterialsList();
            List_Layers.Items.Clear();
            SetSaved(false);
        }

        private void GeosetAttachmeentInfo(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            var g = GetSelectedGeosets()[0];
            List<string> names = new();
            foreach (var vertex in g.Vertices)
            {
                foreach (var gnode in vertex.Group.Object.Nodes)
                {
                    var name = gnode.Node.Node.Name;
                    if (names.Contains(name) == false)
                    {
                        names.Add(name);
                    }
                }

            }
            MessageBox.Show($"Geoset {g.ObjectId} is attached to:\n" + string.Join("\n", names));
        }



        private void DetachTrianglesInsideGeoset2(object? sender, MouseButtonEventArgs e)
        {
            DuplicateTrianglesInsideGeoset(true);

        }

        private void DupliateTriiangleInsideGeosetUp(object? sender, MouseButtonEventArgs e)
        {
            DuplicateTrianglesInsideGeoset(true);
        }

        private void DetachTraingleAsNewGeosetUp(object? sender, MouseButtonEventArgs e)
        {
            DuplicateTrianglesAsNewGeoset(true, true);
        }

        private void gotoOBJ(object? sender, RoutedEventArgs? e)
        {
            BrowserOpener.OpenLink("https://en.wikipedia.org/wiki/Wavefront_.obj_file");
        }
        private void createhs(object? sender, RoutedEventArgs? e)
        {
            // Create bones with updated coordinates (X→Z, Y→X, Z→Y)
            CBone pelvis = new CBone(CurrentModel) { Name = "Pelvis", PivotPoint = new CVector3(0, 0, 90) };
            CBone chest = new CBone(CurrentModel) { Name = "Chest", PivotPoint = new CVector3(0, 0, 120) };
            CBone head = new CBone(CurrentModel) { Name = "Head", PivotPoint = new CVector3(0, 0, 150) };

            CBone shoulderLeft = new CBone(CurrentModel) { Name = "Shoulder_Left", PivotPoint = new CVector3(0, -20, 125) };
            CBone elbowLeft = new CBone(CurrentModel) { Name = "Elbow_Left", PivotPoint = new CVector3(0, -40, 115) };
            CBone wristLeft = new CBone(CurrentModel) { Name = "Wrist_Left", PivotPoint = new CVector3(0, -60, 105) };

            CBone shoulderRight = new CBone(CurrentModel) { Name = "Shoulder_Right", PivotPoint = new CVector3(0, 20, 125) };
            CBone elbowRight = new CBone(CurrentModel) { Name = "Elbow_Right", PivotPoint = new CVector3(0, 40, 115) };
            CBone wristRight = new CBone(CurrentModel) { Name = "Wrist_Right", PivotPoint = new CVector3(0, 60, 105) };

            CBone legLeft = new CBone(CurrentModel) { Name = "Leg_Left", PivotPoint = new CVector3(0, -10, 70) };
            CBone kneeLeft = new CBone(CurrentModel) { Name = "Knee_Left", PivotPoint = new CVector3(0, -10, 40) };
            CBone ankleLeft = new CBone(CurrentModel) { Name = "Ankle_Left", PivotPoint = new CVector3(0, -10, 10) };

            CBone legRight = new CBone(CurrentModel) { Name = "Leg_Right", PivotPoint = new CVector3(0, 10, 70) };
            CBone kneeRight = new CBone(CurrentModel) { Name = "Knee_Right", PivotPoint = new CVector3(0, 10, 40) };
            CBone ankleRight = new CBone(CurrentModel) { Name = "Ankle_Right", PivotPoint = new CVector3(0, 10, 10) };

            // Attachments
            CAttachment footRightRef = new CAttachment(CurrentModel) { Name = "Foot Right Ref", PivotPoint = new CVector3(0, 10, 5) };
            footRightRef.Parent.Attach(ankleRight);

            CAttachment weaponLeft = new CAttachment(CurrentModel) { Name = "Weapon Left Ref", PivotPoint = new CVector3(0, -65, 105) };
            weaponLeft.Parent.Attach(wristLeft);

            CAttachment weaponRight = new CAttachment(CurrentModel) { Name = "Weapon Right Ref", PivotPoint = new CVector3(0, 65, 105) };
            weaponRight.Parent.Attach(wristRight);

            CAttachment chestRef = new CAttachment(CurrentModel) { Name = "Chest Ref", PivotPoint = new CVector3(0, 0, 120) };
            chestRef.Parent.Attach(chest);

            CAttachment headRef = new CAttachment(CurrentModel) { Name = "Head Ref", PivotPoint = new CVector3(0, 0, 155) };
            headRef.Parent.Attach(head);

            CAttachment overheadRef = new CAttachment(CurrentModel) { Name = "Overhead Ref", PivotPoint = new CVector3(0, 0, 170) };
            overheadRef.Parent.Attach(head);

            CAttachment footLeftRef = new CAttachment(CurrentModel) { Name = "Foot Left Ref", PivotPoint = new CVector3(0, -10, 5) };
            footLeftRef.Parent.Attach(ankleLeft);

            CAttachment handLeftRef = new CAttachment(CurrentModel) { Name = "Hand Left Ref", PivotPoint = new CVector3(0, -60, 105) };
            handLeftRef.Parent.Attach(wristLeft);

            // Hierarchy Setup
            chest.Parent.Attach(pelvis);
            head.Parent.Attach(chest);

            shoulderLeft.Parent.Attach(chest);
            elbowLeft.Parent.Attach(shoulderLeft);
            wristLeft.Parent.Attach(elbowLeft);

            shoulderRight.Parent.Attach(chest);
            elbowRight.Parent.Attach(shoulderRight);
            wristRight.Parent.Attach(elbowRight);

            legLeft.Parent.Attach(pelvis);
            kneeLeft.Parent.Attach(legLeft);
            ankleLeft.Parent.Attach(kneeLeft);

            legRight.Parent.Attach(pelvis);
            kneeRight.Parent.Attach(legRight);
            ankleRight.Parent.Attach(kneeRight);

            // Add bones to model
            CurrentModel.Nodes.Add(pelvis);
            CurrentModel.Nodes.Add(chest);
            CurrentModel.Nodes.Add(head);

            CurrentModel.Nodes.Add(shoulderLeft);
            CurrentModel.Nodes.Add(elbowLeft);
            CurrentModel.Nodes.Add(wristLeft);

            CurrentModel.Nodes.Add(shoulderRight);
            CurrentModel.Nodes.Add(elbowRight);
            CurrentModel.Nodes.Add(wristRight);

            CurrentModel.Nodes.Add(legLeft);
            CurrentModel.Nodes.Add(kneeLeft);
            CurrentModel.Nodes.Add(ankleLeft);

            CurrentModel.Nodes.Add(legRight);
            CurrentModel.Nodes.Add(kneeRight);
            CurrentModel.Nodes.Add(ankleRight);

            // Add attachments
            CurrentModel.Nodes.Add(weaponLeft);
            CurrentModel.Nodes.Add(weaponRight);
            CurrentModel.Nodes.Add(chestRef);
            CurrentModel.Nodes.Add(headRef);
            CurrentModel.Nodes.Add(overheadRef);
            CurrentModel.Nodes.Add(footLeftRef);
            CurrentModel.Nodes.Add(handLeftRef);
            CurrentModel.Nodes.Add(footRightRef);
            RefreshNodesTree();
        }


        private void ReportGeosetViisibilities(object? sender, RoutedEventArgs? e)
        {
            SelectedGeosetsList = GetSelectedGeosets();
            if (SelectedGeosetsList.Count == 1)
            {
                var first = CurrentModel.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == SelectedGeosetsList[0]);
                if (first != null)
                {
                    StringBuilder sb = new StringBuilder();
                    List<CSequence> inside = new List<CSequence>();
                    if (first.Alpha.Static)
                    {
                        float value = first.Alpha.GetValue();
                        string tag = value > 0 ? "Visible" : "Not Visible";
                        sb.AppendLine(tag); ;
                    }
                    else
                    {
                        foreach (var seq in CurrentModel.Sequences)
                        {
                            var kf = first.Alpha.NodeList.FirstOrDefault(x => x.Time >= seq.IntervalStart && x.Time <= seq.IntervalEnd);
                            if (kf == null)
                            {
                                sb.AppendLine($"{seq.Name}: ?");

                            }
                            else
                            {
                                float val = kf.Value;
                                string tag = val > 0 ? " ✓" : "";
                                sb.AppendLine($"{seq.Name}:{tag}");
                            }
                        }
                        TextViewer tv = new TextViewer(sb.ToString());
                        tv.ShowDialog();

                    }

                }
                else
                {
                    MessageBox.Show("This geoset is not used by a geoset animation");
                }
            }
        }

        private void EditGeosetViisibilitiesKeyword(object? sender, RoutedEventArgs? e)
        {
            if (ListGeosets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            var v = GetSelectedGeosets()[0];

            geoVisibilitiesKeyword g = new geoVisibilitiesKeyword(v, CurrentModel);
            if (!g.Possible) { return; }

            if (g.ShowDialog() == true)
            {
                RefreshGeosetAnimationsList();
            }

        }

        private void EditGeosetViisibilitiesKeywordNodes(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode? node = GetSelectedNode();
                if (node == null) return;
                geoVisibilitiesKeyword g = new geoVisibilitiesKeyword(node, CurrentModel);
                if (!g.Possible) return;

                if (g.ShowDialog() == true)
                {
                    SelectedNodeInNodeEditor(null, null);
                }
            }
        }

        private void check_ag(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < ListGeosets.Items.Count; i++)
            {
                var item = ListGeosets.Items[i] as ListBoxItem;
                if (item?.Content is StackPanel s && s.Children.Count > 0 && s.Children[0] is CheckBox c)
                {
                    c.IsChecked = true;
                }
            }
        }

        private void check_ng(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < ListGeosets.Items.Count; i++)
            {
                var item = ListGeosets.Items[i] as ListBoxItem;
                if (item?.Content is StackPanel s && s.Children.Count > 0 && s.Children[0] is CheckBox c)
                {
                    c.IsChecked = false;
                }
            }
        }

        private void check_rg(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < ListGeosets.Items.Count; i++)
            {
                var item = ListGeosets.Items[i] as ListBoxItem;
                if (item?.Content is StackPanel s && s.Children.Count > 0 && s.Children[0] is CheckBox c)
                {
                    c.IsChecked = !(c.IsChecked ?? false);
                }
            }
        }


        private void uncoupleVertex(object? sender, RoutedEventArgs? e)
        {
            var vertices = GetSelectedVertices();
            foreach (var geo in CurrentModel.Geosets)
            {
                var triangles = geo.Triangles.ToList();
                foreach (var triangle in triangles)
                {
                    if (
                        vertices.Contains(triangle.Vertex1.Object) ||
                        vertices.Contains(triangle.Vertex2.Object) ||
                        vertices.Contains(triangle.Vertex3.Object)

                        )
                    {
                        geo.Triangles.Remove(triangle);
                    }
                }
            }
        }

        private void SetMousePickRadius(object? sender, TextChangedEventArgs e)
        {
            if (Pause) return;

            if (float.TryParse(InputMousePickRaius.Text, out float value))
            {
                if (value > 0) { RayCaster.MousePickRadius = value; }
                else
                {
                    RayCaster.MousePickRadius = 1;
                }
            }
            else
            {
                RayCaster.MousePickRadius = 1;
            }
            SaveSettings();
        }
        private void SetSelectionMode(mSelectionMode mode)
        {
            SelectionMode = mode;
            if (mode == mSelectionMode.Clear)
            {
                Pause = true;
                selectionItemClear.IsChecked = true;
                selectionItemAdd.IsChecked = false;
                selectionItemRemove.IsChecked = false;
                Pause = false;
            }
            else if (mode == mSelectionMode.Add)
            {
                Pause = true;
                selectionItemClear.IsChecked = false;
                selectionItemAdd.IsChecked = true;
                selectionItemRemove.IsChecked = false;
                Pause = false;
            }
            else if (mode == mSelectionMode.Remove)
            {
                Pause = true;
                selectionItemClear.IsChecked = false;
                selectionItemAdd.IsChecked = false;
                selectionItemRemove.IsChecked = true;
                Pause = false;
            }
        }

        private void SetSelectionMode2(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            SetSelectionMode(mSelectionMode.Add);
        }

        private void SetSelectionMode3(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            SetSelectionMode(mSelectionMode.Remove);
        }

        private void SetSelectionMode1(object? sender, RoutedEventArgs? e)
        {
            if (Pause) return;
            if (selectionItemClear == null) return;
            SetSelectionMode(mSelectionMode.Clear);
        }

        private void Hotkeys_Up(object? sender, KeyEventArgs e)
        {

            SetSelectionMode(mSelectionMode.Clear);
        }

        private void fitVertexUV(object? sender, RoutedEventArgs? e)
        {
            var vertices = GetSelectedVertices();
            if (vertices.Count == 3)
            {
                vertices[0].TexturePosition = new CVector2(0, 0);
                vertices[1].TexturePosition = new CVector2(1, 0);
                vertices[2].TexturePosition = new CVector2(0, 1);
            }
            else if (vertices.Count == 4)
            {
                vertices[0].TexturePosition = new CVector2(0, 0);
                vertices[1].TexturePosition = new CVector2(1, 0);
                vertices[2].TexturePosition = new CVector2(0, 1);
                vertices[3].TexturePosition = new CVector2(1, 1);
            }
            else
            {
                MessageBox.Show("Select 3 or 4 vertices");
                return;
            }
        }

        private void Rigging_SelectVer(object? sender, RoutedEventArgs? e)
        {
            if (ListBonesRiggings.SelectedItem == null) return;
            var bone = getselectedBoneInRigging();

            List<INode> selected = new List<INode>();



            foreach (var geoet in CurrentModel.Geosets)
            {
                foreach (var vertex in geoet.Vertices)
                {

                    vertex.isSelected = false;
                    foreach (var gnode in vertex.Group.Object.Nodes)
                    {
                        if (vertex.Group.Object.Nodes.Any(x => x.Node.Node == selected))
                        {
                            vertex.isSelected = true; break;
                        }
                    }
                }
            }
            RefreshrigingAttachedList();

        }
        private void RefreshrigingAttachedList()
        {
            var vertices = GetSelectedVertices();
            if (Tabs_Geosets.SelectedIndex != 3) { return; }
            if (vertices.Count == 0) { ListAttachedToRiggings.Items.Clear(); return; }

            if (vertices.Count == 0) { ListAttachedToRiggings.Items.Clear(); return; }
            CGeosetGroup gr = new CGeosetGroup(CurrentModel);
            List<INode> nodes = GetNodesOfGroup(vertices[0].Group);
            if (vertices.Count == 1) { FillRiggingAttachedistWith(nodes); return; }
            bool same = true; ;
            for (int g = 0; g < vertices[0].Group.Object.Nodes.Count; g++)
            {
                nodes.Add(vertices[0].Group.Object.Nodes[g].Node.Node);
            }
            for (int v = 0; v < vertices.Count; v++)
            {
                for (int g = 0; g < vertices[v].Group.Object.Nodes.Count; g++)
                {
                    if (nodes.Contains(vertices[v].Group.Object.Nodes[g].Node.Node) == false)
                    {
                        same = false; break;
                    }
                    if (!same) { break; }
                }
                if (!same) { break; }
            }


            if (same)
            {
                LabelRiggingAttachedTo.Text = "Selected vertices are attached to: (same)";
                FillRiggingAttachedistWith(nodes);
                ButtonAddAttach.IsEnabled = true;
                ButtonClearAttach.IsEnabled = true;
                ButtonDetach.IsEnabled = true;
            }
            else
            {
                LabelRiggingAttachedTo.Text = "Selected vertices are attached to: (not same)";
                ListAttachedToRiggings.Items.Clear();
                ButtonAddAttach.IsEnabled = false;
                ButtonDetach.IsEnabled = false;
                ButtonClearAttach.IsEnabled = true;
            }
        }

        private void FillRiggingAttachedistWith(List<INode> nodes)
        {
            ListAttachedToRiggings.Items.Clear();
            List<string> names = nodes.Select(x => x.Name).ToList();
            names = names.Distinct().ToList();
            foreach (string name in names)
            {
                ListAttachedToRiggings.Items.Add(new ListBoxItem() { Content = name });
            }
        }

        private static List<INode> GetNodesOfGroup(CObjectReference<CGeosetGroup> group)
        {
            List<INode> list = new List<INode>();
            foreach (var gnode in group.Object.Nodes)
            {
                if (list.Contains(gnode.Node.Node) == false) list.Add(gnode.Node.Node);
            }
            return list;
        }

        private void SelectVerticesofBones(object? sender, RoutedEventArgs? e)
        {
            // if (ListBonesRiggings.SelectedItem == null) return;
            var bones = CurrentModel.Nodes.Where(x => x is CBone).ToList();
            var names = bones.Select(x => x.Name).ToList();
            if (bones.Count == 0)
            {
                MessageBox.Show("There are no bones");
                return;
            }
            Multiselector_Window mw = new Multiselector_Window(names, "Which bones?");

            if (mw.ShowDialog() == true)
            {

                foreach (int index in mw.selectedIndexes)
                {
                    var bone = bones[index];
                    foreach (var geoet in CurrentModel.Geosets)
                    {
                        foreach (var vertex in geoet.Vertices)
                        {

                            vertex.isSelected = false;
                            foreach (var gnode in vertex.Group.Object.Nodes)
                            {
                                if (gnode.Node.Node == bone)
                                {
                                    vertex.isSelected = true; break;
                                }
                            }
                        }
                    }
                }
            }






        }

        private void check_rag(object? sender, RoutedEventArgs? e)
        {
            foreach (var y in Listbox_Geosets_Rigging.Items)
            {
                ListBoxItem? i = y as ListBoxItem; if (i == null) continue;
                CheckBox? c = i.Content as CheckBox; if (c == null) continue;
                c.IsChecked = true;
            }
        }

        private void check_rng(object? sender, RoutedEventArgs? e)
        {
            foreach (var y in Listbox_Geosets_Rigging.Items)
            {
                ListBoxItem? i = y as ListBoxItem; if (i == null) return;
                CheckBox? c = i.Content as CheckBox; if (c == null) return;
                c.IsChecked = false;
            }
        }

        private void check_rrg(object? sender, RoutedEventArgs? e)
        {
            foreach (var y in Listbox_Geosets_Rigging.Items)
            {
                ListBoxItem? i = y as ListBoxItem; if (i == null) return;
                CheckBox? c = i.Content as CheckBox; if (c == null) return;
                c.IsChecked = !c.IsChecked;
            }
        }

        private void selectAall(object? sender, RoutedEventArgs? e)
        {
            switch (Tabs_Geosets.SelectedIndex)
            {
                case 0: SelectAllGeosets(null, null); break;
                case 1:
                    foreach (var geoset in CurrentModel.Geosets)
                    {
                        foreach (var triangle in geoset.Triangles)
                        {
                            triangle.isSelected = true;
                        }
                    }
                    break;
                case 2:
                case 3:
                    foreach (var geoset in CurrentModel.Geosets)
                    {
                        foreach (var vertex in geoset.Vertices)
                        {
                            vertex.isSelected = true;
                        }
                    }
                    break;
                case 6: NodeList_SelectN(null, null); break;
                default: break;
            }

        }

        private void unselectAll(object? sender, RoutedEventArgs? e)
        {
            DeselectAllGeosets(null, null);
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var triangle in geoset.Triangles) triangle.isSelected = false;

                foreach (var vertex in geoset.Vertices) vertex.isSelected = false;

            }

            NodeList_SelectN(null, null);

        }

        private void selectInverse(object? sender, RoutedEventArgs? e)
        {
            switch (Tabs_Geosets.SelectedIndex)
            {
                case 0: InvertSelectGeosets(null, null); break;
                case 1:
                    foreach (var geoset in CurrentModel.Geosets)
                    {
                        foreach (var triangle in geoset.Triangles)
                        {
                            triangle.isSelected = !triangle.isSelected;
                        }
                    }
                    break;
                case 2:
                case 3:
                    foreach (var geoset in CurrentModel.Geosets)
                    {
                        foreach (var vertex in geoset.Vertices)
                        {
                            vertex.isSelected = !vertex.isSelected;
                        }
                    }
                    break;
                case 6: NodeList_SelectI(null, null); break;
                default: break;
            }
        }

        private void GrowSelection(object? sender, RoutedEventArgs? e)
        {
            //unfinished
        }

        private void ShrinkSelection(object? sender, RoutedEventArgs? e)
        {
            //unfinished
        }

        private void alignx(object? sender, RoutedEventArgs? e)
        {
            AlignSelection(Axes.X);
        }


        private void aligny(object? sender, RoutedEventArgs? e)
        {
            AlignSelection(Axes.Y);

        }



        private void alignz(object? sender, RoutedEventArgs? e)
        {
            AlignSelection(Axes.Z);
        }

        private void DelNodeAndChildren(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            INode node = GetSeletedNode();

            GetAllNodesInHierarchy(node);
            foreach (INode child in TempNodeList)
            {

                CurrentModel.Nodes.Remove(child);
            }
            CurrentModel.Nodes.Remove(node);
            TempNodeList.Clear();
            RefreshNodesTree();
        }
        private List<INode> TempNodeList = new List<INode>();


        private void GetAllNodesInHierarchy(INode inputNode)
        {
            foreach (var node in CurrentModel.Nodes)
            {
                if (node.Parent?.Node == inputNode)
                {
                    TempNodeList.Add(node);
                    GetAllNodesInHierarchy(node); // Recurse only for the child
                }
            }
        }

        private void undo(object? sender, RoutedEventArgs? e)
        {
            if (ListOptions.SelectedIndex == 0)
            {
                History.Geometry.Undo(CurrentModel, this);
            }
            if (ListOptions.SelectedIndex == 1)
            {
                History.Sequences.Undo(CurrentModel, this);
            }
            if (ListOptions.SelectedIndex == 2)
            {
                History.Nodes.Undo(CurrentModel, this);
            }
        }

        private void redo(object? sender, RoutedEventArgs? e)
        {
            if (ListOptions.SelectedIndex == 0)
            {
                History.Geometry.Redo(CurrentModel, this);
            }
            if (ListOptions.SelectedIndex == 1)
            {
                History.Sequences.Redo(CurrentModel, this);
            }
            if (ListOptions.SelectedIndex == 2)
            {
                History.Nodes.Redo(CurrentModel, this);
            }
        }

        private void CenterEachBoneAtItsGeometry(object? sender, RoutedEventArgs? e)
        {
            foreach (var selected in CurrentModel.Nodes)
            {
                if (selected is CBone == false) continue;
                List<CGeosetVertex> attached = GetAttachedVerticesToNode(selected);
                CVector3 centroid = Calculator.GetCentroidOfVertices(attached);
                selected.PivotPoint = new CVector3(centroid);
            }
        }

        private void AverageSharedUV(object? sender, RoutedEventArgs? e)
        {
            var vertices = GetSelectedVertices();
            if (vertices.Count < 2)
            {
                Calculator.AverageUV(vertices);
                //unfinished
            }
            else
            {
                MessageBox.Show("Select 2 or more vertices"); return;
            }
        }
        private void AlignSelection(Axes axis)
        {
            int tab = Tabs_Geosets.SelectedIndex;

            if (tab == 0) // Geosets
            {
                var geosets = GetSelectedGeosets();
                if (geosets.Count > 1)
                {
                    // Use the first geoset's centroid as target
                    var targetCentroid = Calculator.GetCentroidOfGeoset(geosets[0]);

                    Calculator.AlignGeosets(geosets, axis == Axes.X, axis == Axes.Y, axis == Axes.Z);

                }
            }


            else if (tab == 1) // Vertices
            {
                var vertices = GetSelectedVertices();
                if (vertices.Count > 1)
                {
                    float target = GetAxisComponent(vertices[0].Position, axis);
                    for (int i = 1; i < vertices.Count; i++)
                    {
                        SetAxisComponent(vertices[i].Position, axis, target);
                    }
                }
            }
            else if (tab == 2) // Triangles
            {
                var triangleVertices = getSelectedTriangles().SelectMany(x => x.Vertices).ToList();
                if (triangleVertices.Count > 1)
                {
                    float target = GetAxisComponent(triangleVertices[0].Position, axis);
                    for (int i = 1; i < triangleVertices.Count; i++)
                    {
                        SetAxisComponent(triangleVertices[i].Position, axis, target);
                    }
                }
            }
            else if (tab == 6) // Nodes
            {
                var nodes = GetSelectedNodes_NodeEditor();
                if (nodes.Count > 1)
                {
                    float target = GetAxisComponent(nodes[0].PivotPoint, axis);
                    for (int i = 1; i < nodes.Count; i++)
                    {
                        SetAxisComponent(nodes[i].PivotPoint, axis, target);
                    }
                }
            }
        }

        private static float GetAxisComponent(CVector3 vector, Axes axis)
        {
            return axis switch
            {
                Axes.X => vector.X,
                Axes.Y => vector.Y,
                Axes.Z => vector.Z,
                _ => 0f
            };
        }

        private static void SetAxisComponent(CVector3 vector, Axes axis, float value)
        {
            switch (axis)
            {
                case Axes.X: vector.X = value; break;
                case Axes.Y: vector.Y = value; break;
                case Axes.Z: vector.Z = value; break;
            }
        }

        private void createLayer(object? sender, RoutedEventArgs? e)
        {
            if (List_MAterials.SelectedItem != null && List_Textures.SelectedItem != null)
            {
                var texture = GetSElectedTexture();
                var material = GetSelectedMAterial();
                CMaterialLayer layer = new CMaterialLayer(CurrentModel);
                layer.Texture.Attach(texture);
                material.Layers.Add(layer);
                RefreshLayersList();
            }
        }

        private void flipnormalVertex(object? sender, RoutedEventArgs? e)
        {
            var vertices = GetSelectedVertices();
            foreach (var vertex in vertices)
            {
                vertex.Normal.Negate();
            }
        }
        private void SetNewModelName()
        {
            CurrentModel.Name = "New Model";
            if (DefaultAuthor.Length > 0) CurrentModel.Name += $" by {DefaultAuthor}";
        }
        private void newm(object? sender, RoutedEventArgs? e)
        {
            model_starter ms = new model_starter();
            if (ms.ShowDialog() == true)
            {
                CurrentModel = ms.NewModel;
                refreshalllists(null, null);
                SetNewModelName();
                RefreshTitle();
                SetSaved(false);
            }
        }

        private void CollapseTrianglesAll(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count > 1) { MessageBox.Show("Select at least 1 triangle"); return; }
            var centroid = Calculator.GetCentroidOfVertices(new List<CGeosetVertex>() { triangles[0].Vertex1.Object, triangles[0].Vertex2.Object, triangles[0].Vertex3.Object });

            foreach (var triangle in triangles)
            {
                triangle.Vertex1.Object.Position = new CVector3(centroid);
                triangle.Vertex2.Object.Position = new CVector3(centroid);
                triangle.Vertex3.Object.Position = new CVector3(centroid);
            }
            RefreshGeosets_Triangles();
        }

        private void CollapseVerticesAll(object? sender, RoutedEventArgs? e)
        {
            List<CGeosetVertex> vertices = GetSelectedVertices();
            if (vertices.Count < 2)
            {
                MessageBox.Show("Select at least 2 vertices");
                return;
            }

            Dictionary<CGeoset, List<CGeosetVertex>> reference = new Dictionary<CGeoset, List<CGeosetVertex>>();

            // Group vertices by geoset
            List<CGeosetVertex> firsts = new List<CGeosetVertex>();
            foreach (var vertex in vertices)
            {
                foreach (var geoset in CurrentModel.Geosets)
                {
                    if (geoset.Vertices.Contains(vertex))
                    {
                        if (!reference.ContainsKey(geoset))
                            reference[geoset] = new List<CGeosetVertex>();
                        reference[geoset].Add(vertex);
                        break; // One vertex should belong to only one geoset
                    }
                }
            }

            foreach (var pair in reference)
            {
                if (pair.Value.Count <= 1) continue;

                var first = pair.Value.First();
                firsts.Add(first);
                var redundantVertices = pair.Value.Skip(1).ToList();

                foreach (var triangle in pair.Key.Triangles)
                {
                    if (redundantVertices.Contains(triangle.Vertex1.Object))
                        triangle.Vertex1.Attach(first);
                    if (redundantVertices.Contains(triangle.Vertex2.Object))
                        triangle.Vertex2.Attach(first);
                    if (redundantVertices.Contains(triangle.Vertex3.Object))
                        triangle.Vertex3.Attach(first);
                }

                foreach (var v in redundantVertices)
                    pair.Key.Vertices.Remove(v);
            }
            if (firsts.Count > 1)
            {
                for (int i = 1; i < firsts.Count; i++) { firsts[i].Position = new CVector3(firsts[0].Position); }

            }
            UpdateSelectedVerticesInfo();
            SetSaved(false);
            RefreshRenderData(null, null); RefreshGeosets_Vertices();
        }

        private void SnapVerticestoCentroid(object? sender, RoutedEventArgs? e)
        {
            var vertices = GetSelectedVertices();
            if (vertices.Count > 1)
            {
                var c = Calculator.GetCentroidOfVertices(vertices);
                for (int i = 0; i < vertices.Count; i++) { vertices[i].Position = new CVector3(c); }
            }
        }

        private void CreateIconFromScreenshot(object? sender, RoutedEventArgs? e)
        {
            if (File.Exists(CurrentSaveLocation) == false)
            {
                MessageBox.Show("Current save file/location does not exist"); return;
            }
            if (Directory.Exists(CurrentSaveFolder) == false)
            {
                MessageBox.Show("Current save folder does not exist"); return;
            }
            if (ListOptions.SelectedIndex != 0) { MessageBox.Show("Select the geosets tab"); return; }
            // get paths
            string? Filename = System.IO.Path.GetFileNameWithoutExtension(CurrentSaveLocation);
            string? outputFolder = System.IO.Path.Combine(CurrentSaveFolder, "Icon");
            string? outputScr = System.IO.Path.Combine(outputFolder, "scr.png");
            string? btnBorder = System.IO.Path.Combine(AppHelper.Local, "Tools\\btn.png");
            string? DiSbtnBorder = System.IO.Path.Combine(AppHelper.Local, "Tools\\disbtn.png");
            string? outputBtn = System.IO.Path.Combine(outputFolder, $"BTN_{Filename}.png");
            string? outputBtnDis = System.IO.Path.Combine(outputFolder, $"DISBTN_{Filename}.png");
            string? outputBtnBLP = System.IO.Path.Combine(outputFolder, $"BTN_{Filename}.blp");
            string? outputBtnDisBLP = System.IO.Path.Combine(outputFolder, $"DISBTN_{Filename}.blp");
            // get screenshot
            Directory.CreateDirectory(outputFolder);
            CaptureScreenshot(Scene_ViewportGL, outputScr);
            // load
            Bitmap LoadedImage = new Bitmap(outputScr);// ImageHelper.Load(outputScr);

            Bitmap btn = new Bitmap(btnBorder);
            Bitmap disbtn = new Bitmap(DiSbtnBorder);
            //resize
            Bitmap resized = ImageHelper.Resize(LoadedImage, 64, 64);


            // join
            resized.SetResolution(72, 72);
            btn.SetResolution(72, 72);
            disbtn.SetResolution(72, 72);
            Bitmap Button = ImageHelper.Join(resized, btn);
            Bitmap ButtonDisabled = ImageHelper.Join(resized, disbtn);
            // save
            Button.Save(outputBtn);
            ButtonDisabled.Save(outputBtnDis);
            // convert

            BLPConverter.Convert(outputBtn, outputBtnBLP, this, outputBtn);
            BLPConverter.Convert(outputBtnDis, outputBtnDisBLP, this, outputBtnDis);

            //dispose
            LoadedImage.Dispose();
            btn.Dispose();
            disbtn.Dispose();
            Button.Dispose();
            ButtonDisabled.Dispose();
            File.Delete(outputScr);
            //  File.Delete(outputBtn);
            //  File.Delete(outputBtnDis);
            Process.Start("explorer.exe", outputFolder);

        }

        private void setModelNameF(object? sender, RoutedEventArgs? e)
        {
            string? name = System.IO.Path.GetFileNameWithoutExtension(CurrentSaveLocation);
            if (DefaultAuthor.Length > 0)
            {
                name += " by" + DefaultAuthor;
                RefreshTitle();
                SetSaved(false);
            }
        }

        private void setAuthor(object? sender, RoutedEventArgs? e)
        {
            Input i = new Input(DefaultAuthor);
            i.ShowDialog();
            if (i.DialogResult == true)
            {
                DefaultAuthor = i.Result;
                SaveSettings();
            }
        }

        private void CenterSelectionTogether(object? sender, RoutedEventArgs? e)
        {
            axis_selector ax = new axis_selector();
            ax.ShowDialog();
            if (ax.DialogResult == true)
            {
                bool onX = ax.Check_x.IsChecked == true;
                bool onY = ax.Check_y.IsChecked == true;
                bool onZ = ax.Check_z.IsChecked == true;


                if (Tabs_Geosets.SelectedIndex == 0) // geosets
                {
                    var vertices = GetSelectedGeosets().SelectMany(x => x.Vertices).ToList();
                    Calculator.CenterVerticesTogether(vertices, onX, onY, onZ);
                }
                else if (Tabs_Geosets.SelectedIndex == 1) // triangles
                {

                    var vertices = getSelectedTriangles().SelectMany(x => x.Vertices).ToList();
                    Calculator.CenterVerticesTogether(vertices, onX, onY, onZ);
                }
                else if (Tabs_Geosets.SelectedIndex == 2 || Tabs_Geosets.SelectedIndex == 3) // vertices
                {
                    var vertices = GetSelectedVertices();
                    Calculator.CenterVerticesTogether(vertices, onX, onY, onZ);

                }
                if (Tabs_Geosets.SelectedIndex == 6) // nodes
                {
                    var nodes = GetSelectedNodes_NodeEditor();
                    Calculator.centerNodesTogether(nodes, onX, onY, onZ);
                }

                SetSaved(false);
            }


        }

        private void SnapSelectionTogether(object? sender, RoutedEventArgs? e)
        {
            SnapSettingsWindow sw = new SnapSettingsWindow();
            if (sw.ShowDialog() == true)
            {
                var type = sw.Type;
                if (Tabs_Geosets.SelectedIndex == 0) //geosets
                {
                    var vertices = GetSelectedGeosets().SelectMany(x => x.Vertices).ToList();

                    Calculator.SnapVerticesAsGroup(vertices, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);



                }
                if (Tabs_Geosets.SelectedIndex == 1) //triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();

                    Calculator.SnapVerticesAsGroup(vertices, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);

                }
                if (Tabs_Geosets.SelectedIndex == 2 || Tabs_Geosets.SelectedIndex == 3) //verties
                {
                    var vertices = GetSelectedVertices();
                    Calculator.SnapVerticesAsGroup(vertices, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);

                }
                if (Tabs_Geosets.SelectedIndex == 6) //nodes
                {
                    var nodes = GetSelectedNodes_NodeEditor();
                    Calculator.SnapNodesAsGroup(nodes, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);
                }
            }


        }

        private void SnapSelectionEach(object? sender, RoutedEventArgs? e)
        {
            SnapSettingsWindow sw = new SnapSettingsWindow();
            if (sw.ShowDialog() == true)
            {
                var type = sw.Type;
                if (Tabs_Geosets.SelectedIndex == 0) //geosets
                {
                    //   var vertices = GetSelectedGeosets().SelectMany(x => x.Vertices).ToList();
                    var geosets = GetSelectedGeosets();
                    foreach (var geoset in geosets)
                    {
                        Calculator.SnapVerticesAsGroup(geoset.Vertices.ToList(), type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);
                    }


                }
                if (Tabs_Geosets.SelectedIndex == 1) //triangles
                {
                    var triangles = getSelectedTriangles();
                    var vertices = triangles.SelectMany(x => x.Vertices).Distinct().ToList();
                    foreach (var vartex in vertices)
                    {
                        Calculator.SnapVector(vartex.Position, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);
                    }
                }
                if (Tabs_Geosets.SelectedIndex == 2 || Tabs_Geosets.SelectedIndex == 3) //verties
                {
                    var vertices = GetSelectedVertices();
                    foreach (var vartex in vertices)
                    {
                        Calculator.SnapVector(vartex.Position, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);
                    }
                }
                if (Tabs_Geosets.SelectedIndex == 6) //nodes
                {
                    var nodes = GetSelectedNodes_NodeEditor();
                    foreach (var node in nodes)
                    {
                        Calculator.SnapVector(node.PivotPoint, type, RenderSettings.ViewportGridSize, RenderSettings.LineSpacing);
                    }
                }
            }
        }

        private void RenameNodes(object? sender, RoutedEventArgs? e)
        {
            Nodes_Renamer nr = new Nodes_Renamer(CurrentModel.Nodes.ToList());
            if (nr.ShowDialog() == true)
            {
                RefreshNodesTree();
            }
        }

        private void swapnodenames(object? sender, RoutedEventArgs? e)
        {
            if (ListNodes.SelectedItem != null)
            {
                INode? node = GetSelectedNode(); if (node == null) return;
                List<string> list = CurrentModel.Nodes.Select(x => x.Name).ToList();
                list.Remove(node.Name);
                if (list.Count > 0)
                {
                    Selector s = new Selector(list);
                    if (s.ShowDialog() == true)
                    {
                        string? selected = Extractor.GetString(s.box);
                        INode target = CurrentModel.Nodes.First(x => x.Name == selected);
                        string targetName = target.Name;
                        string nodeName = node.Name;
                        target.Name = nodeName;
                        node.Name = targetName;
                        RefreshNodesTree();
                    }
                }
            }
        }

        private void explain_errorCheker(object? sender, RoutedEventArgs? e)
        {
            bigTextContainer bc = new bigTextContainer(System.IO.Path.Combine(AppHelper.Local, "errorchecklist.txt"));
            bc.ShowDialog();
        }

        private void CheckAll_Geosets_Triangles(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in Container_Geosets_Triangles.Children)
            {
                ListBoxItem? b = item as ListBoxItem; if (b == null) return;
                StackPanel? p = b.Content as StackPanel; if (p == null) return;
                CheckBox? c = p.Children[0] as CheckBox; if (c == null) return;
                c.IsChecked = true;

            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = true;
            }
            Pause = false;
        }

        private void CheckNone_Geosets_Triangles(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in Container_Geosets_Triangles.Children)
            {
                CheckBox? c = Extractor.GetCheckBoxOfListItem(item); if (c == null) return;

                c.IsChecked = false;

            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = false;
            }
            Pause = false;
        }

        private void CheckInvert_Geosets_Triangles(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in Container_Geosets_Triangles.Children)
            {
                ListBoxItem? b = item as ListBoxItem; if (b == null) return;
                StackPanel? p = b.Content as StackPanel; if (p == null) return;
                CheckBox? c = p.Children[0] as CheckBox; if (c == null) return;
                c.IsChecked = !c.IsChecked;

            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = !geoset.isVisible;
            }
            Pause = false;
        }

        private void CheckAll_Geosets_Vertices(object? sender, RoutedEventArgs? e)
        {
            Pause = true;

            foreach (var item in Container_Geosets_Vertices.Children)
            {
                ListBoxItem? b = item as ListBoxItem; if (b == null) return;
                StackPanel? p = b.Content as StackPanel; if (p == null) return;
                CheckBox? c = p.Children[0] as CheckBox; if (c == null) return;
                c.IsChecked = true;

            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = true;
            }
            Pause = false;
        }

        private void CheckNone_Geosets_Vertices(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in Container_Geosets_Vertices.Children)
            {
                ListBoxItem? b = item as ListBoxItem; if (b == null) return;
                StackPanel? p = b.Content as StackPanel; if (p == null) return;
                CheckBox? c = p.Children[0] as CheckBox; if (c == null) return;
                c.IsChecked = false;

            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = false;
            }
            Pause = false;
        }

        private void CheckInvert_Geosets_Vertices(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in Container_Geosets_Vertices.Children)
            {
                ListBoxItem? b = item as ListBoxItem; if (b == null) return;
                StackPanel? p = b.Content as StackPanel; if (p == null) return;
                CheckBox? c = p.Children[0] as CheckBox; if (c == null) return;
                c.IsChecked = !c.IsChecked;

            }
            foreach (var geoset in CurrentModel.Geosets)
            {
                geoset.isVisible = !geoset.isVisible;
            }
            Pause = false;
        }
        private void RefreshGeosets_Triangles()
        {
            Container_Geosets_Triangles.Children.Clear();

            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                ListBoxItem Item = new ListBoxItem();
                int UsedMaterialIndex = CurrentModel.Materials.IndexOf(geo.Material.Object);
                TextBlock Title = new TextBlock();
                string TitleName = geo.ObjectId.ToString() + $" ({geo.Name}) ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles) (material {UsedMaterialIndex}) (Groups {geo.Groups.Count})";
                Title.Text = TitleName;
                CheckBox CheckPart = new CheckBox();
                StackPanel Container = new StackPanel();
                Item.MouseLeave += (object? sender, MouseEventArgs e) =>
                {
                    if (Item.ToolTip is ToolTip toolTip)
                    {
                        toolTip.IsOpen = false;
                    }
                };
                Container.Orientation = Orientation.Horizontal;
                Container.Children.Add(CheckPart);
                Container.Children.Add(Title);
                CheckPart.IsChecked = true;
                //GeosetVisible.Add(geo, true);
                CheckPart.Checked += (object? sender, RoutedEventArgs? e) => geo.isVisible = true; ;
                CheckPart.Unchecked += (object? sender, RoutedEventArgs? e) => geo.isVisible = false; ;
                Item.Content = Container;
                Container_Geosets_Triangles.Children.Add(Item);
            }
        }
        private void RefreshGeosets_Vertices()
        {
            Container_Geosets_Vertices.Children.Clear();

            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                ListBoxItem Item = new ListBoxItem();
                int UsedMaterialIndex = CurrentModel.Materials.IndexOf(geo.Material.Object);
                TextBlock Title = new TextBlock();
                string TitleName = geo.ObjectId.ToString() + $" ({geo.Name}) ({geo.Vertices.Count} vertices, {geo.Triangles.Count} triangles) (material {UsedMaterialIndex}) (Groups {geo.Groups.Count})";
                Title.Text = TitleName;
                CheckBox CheckPart = new CheckBox();
                StackPanel Container = new StackPanel();
                Item.MouseLeave += (object? sender, MouseEventArgs e) =>
                {
                    if (Item.ToolTip is ToolTip toolTip)
                    {
                        toolTip.IsOpen = false;
                    }
                };
                Container.Orientation = Orientation.Horizontal;
                Container.Children.Add(CheckPart);
                Container.Children.Add(Title);
                CheckPart.IsChecked = true;
                //GeosetVisible.Add(geo, true);
                CheckPart.Checked += (object? sender, RoutedEventArgs? e) => geo.isVisible = true; ;
                CheckPart.Unchecked += (object? sender, RoutedEventArgs? e) => geo.isVisible = false; ;
                Item.Content = Container;
                Container_Geosets_Vertices.Children.Add(Item);
            }
        }





        // Helper to add a side wall between two front vertices and two back vertices




        private void report(object? sender, RoutedEventArgs? e)
        {
            MessageBox.Show("If you want to report a bug, suggest new feature or improve existing feature message me on Discord at <stan0033>", "How to report");
        }

        private void seqn(object? sender, RoutedEventArgs? e)
        {
            SequenceNamesHelper.Show();
        }

        private void createWhite(object? sender, RoutedEventArgs? e)
        {
            CTexture texture = new CTexture(CurrentModel);
            texture.FileName = White;
            CurrentModel.Textures.Add(texture);
            RefreshTexturesList();

        }

        private void createBlack(object? sender, RoutedEventArgs? e)
        {
            CTexture texture = new CTexture(CurrentModel);
            texture.FileName = Black;
            CurrentModel.Textures.Add(texture);
            RefreshTexturesList();
        }

        private void rig_SelectVerticesOfGeoset(object? sender, RoutedEventArgs? e)
        {
            if (Listbox_Geosets_Rigging.SelectedItem != null)
            {
                int index = Listbox_Geosets_Rigging.SelectedIndex;
                foreach (var vertex in CurrentModel.Geosets[index].Vertices)
                {
                    vertex.isSelected = true;
                }
            }
            else
            {
                MessageBox.Show("Selectg a geoset");
            }
        }

        private void SetHistoryLimit(object? sender, TextChangedEventArgs e)
        {
            if (Pause) return;
            if (int.TryParse(Textbox_HistoryLimit.Text, out int value))
            {
                History.SetLimit(value);
            }

        }

        private void negatetTriangles(object? sender, RoutedEventArgs? e)
        {
            var triangles = getSelectedTriangles();
            if (triangles.Count < 0) { MessageBox.Show("Select at least 1 triangle"); return; }
            axis_picker ap = new axis_picker("Axes?");
            if (ap.ShowDialog() == true)
            {
                var ax = ap.axis;
                for (int i = 0; i < triangles.Count; i++)
                {
                    if (ax == Axes.X)
                    {
                        triangles[i].Vertex1.Object.Position.X = -triangles[i].Vertex1.Object.Position.X;
                        triangles[i].Vertex2.Object.Position.X = -triangles[i].Vertex2.Object.Position.X;
                        triangles[i].Vertex3.Object.Position.X = -triangles[i].Vertex3.Object.Position.X;
                    }
                    if (ax == Axes.Y)
                    {
                        triangles[i].Vertex1.Object.Position.Y = -triangles[i].Vertex1.Object.Position.Y;
                        triangles[i].Vertex2.Object.Position.Y = -triangles[i].Vertex2.Object.Position.Y;
                        triangles[i].Vertex3.Object.Position.Y = -triangles[i].Vertex3.Object.Position.Y;
                    }
                    if (ax == Axes.Z)
                    {
                        triangles[i].Vertex1.Object.Position.Z = -triangles[i].Vertex1.Object.Position.Z;
                        triangles[i].Vertex2.Object.Position.Z = -triangles[i].Vertex2.Object.Position.Z;
                        triangles[i].Vertex3.Object.Position.Z = -triangles[i].Vertex3.Object.Position.Z;
                    }
                }
                SetSaved(false);
            }
        }





        private void SetSequenceRarity(object sender, TextChangedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {

                var seqeence = GetSelectedSequence();
                seqeence.Rarity = Extractor.GetFloat(seqeence.Rarity, InputRarity.Text, 0);
                History.Sequences.Add(CurrentModel);
            }
        }

        private void SetSequenceMoveSpeed(object sender, TextChangedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {

                var seqeence = GetSelectedSequence();

                seqeence.MoveSpeed = Extractor.GetFloat(seqeence.MoveSpeed, InputMovespeed.Text, 0);
                History.Sequences.Add(CurrentModel);
            }
        }

        private void setSequenceNonlooping(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            SetSequenceLoop(null, null);
        }

        private void Selected_TA(object? sender, SelectionChangedEventArgs? e)
        {
            if (List_TextureAnims.SelectedItem != null && CurrentModel.TextureAnimations.Count > List_TextureAnims.SelectedIndex)
            {
                int index = List_TextureAnims.SelectedIndex;
                var ta = CurrentModel.TextureAnimations[index];
                Button_TA_Translations.Content = $"Translations ({ta.Translation.Count})";
                Button_TA_Rotations.Content = $"Rotations ({ta.Rotation.Count})";
                Button_TA_Scalings.Content = $"Scalings ({ta.Scaling.Count})";
                Button_TA_Translations.IsEnabled = !ta.Translation.Static;
                Button_TA_Rotations.IsEnabled = !ta.Rotation.Static;
                Button_TA_Scalings.IsEnabled = !ta.Scaling.Static;
                Pause = true;
                Check_TA_Tr.IsChecked = !ta.Translation.Static;
                Check_TA_Ro.IsChecked = !ta.Rotation.Static;
                Check_TA_Sc.IsChecked = !ta.Scaling.Static;
                Pause = false;

            }

        }

        private void Make_Ta_Tr_Animated(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            if (List_TextureAnims.SelectedItem != null && CurrentModel.TextureAnimations.Count > List_TextureAnims.SelectedIndex)
            {
                bool animated = Check_TA_Tr.IsChecked == true;
                int index = List_TextureAnims.SelectedIndex;
                var ta = CurrentModel.TextureAnimations[index];
                if (animated)
                {
                    ta.Translation.MakeAnimated();
                }
                else
                {
                    ta.Translation.MakeStatic(new CVector3());
                }
                Button_TA_Translations.IsEnabled = !ta.Translation.Static;
            }
        }

        private void Make_Ta_Ro_Animated(object sender, RoutedEventArgs e)
        {

            if (Pause) return;
            if (List_TextureAnims.SelectedItem != null && CurrentModel.TextureAnimations.Count > List_TextureAnims.SelectedIndex)
            {
                bool animated = Check_TA_Ro.IsChecked == true;
                int index = List_TextureAnims.SelectedIndex;
                var ta = CurrentModel.TextureAnimations[index];
                if (animated)
                {
                    ta.Rotation.MakeAnimated();
                }
                else
                {
                    ta.Rotation.MakeStatic(new CVector4(0, 0, 0, 1));
                }
                Button_TA_Rotations.IsEnabled = !ta.Rotation.Static;
            }
        }

        private void Make_Ta_Sc_Animated(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            if (List_TextureAnims.SelectedItem != null && CurrentModel.TextureAnimations.Count > List_TextureAnims.SelectedIndex)
            {
                bool animated = Check_TA_Sc.IsChecked == true;
                int index = List_TextureAnims.SelectedIndex;
                var ta = CurrentModel.TextureAnimations[index];
                if (animated)
                {
                    ta.Scaling.MakeAnimated();
                }
                else
                {
                    ta.Scaling.MakeStatic(new CVector3(1, 1, 1));
                }
                Button_TA_Scalings.IsEnabled = !ta.Scaling.Static;
            }
        }

        private void SetLayerAnimatedTextureID(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                bool animated = CheckLayerTextureID.IsChecked == true;
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];

                Combo_LayerUsedTexture.IsEnabled = !animated;
                ButtonTextureID.IsEnabled = animated;
                if (animated)
                {
                    layer.TextureId.MakeAnimated();
                }
                else
                {
                    layer.TextureId.MakeStatic(layer.TextureId.GetValue());
                }
            }


        }

        private void SetLayerAnimatedAlpha(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                bool animated = CheckLayerAlpha.IsChecked == true;
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];

                InputLayerAlpha.IsEnabled = !animated;
                ButtonLayerAlpha.IsEnabled = animated;
                if (animated)
                {
                    layer.Alpha.MakeAnimated();
                }
                else
                {
                    layer.Alpha.MakeStatic(layer.Alpha.GetValue());
                }
            }
        }

        private void SetLayerAlphaStatic(object sender, TextChangedEventArgs e)
        {
            if (List_MAterials.SelectedItem != null && List_Layers.SelectedItem != null)
            {
                CMaterial mat = GetSelectedMAterial();
                CMaterialLayer layer = mat.Layers[List_Layers.SelectedIndex];
                if (float.TryParse(InputLayerAlpha.Text, out float value))
                {
                    if (value >= 0 && value <= 100)
                    {
                        float v = value / 100;
                        layer.Alpha.MakeStatic(v);
                    }


                }
            }
        }

        private void dmpg(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Geosets.Count < 2)
            {
                MessageBox.Show("There are less than 2 geosts present"); return;
            }
            if (List_MAterials.SelectedItem != null)
            {
                int index = List_MAterials.SelectedIndex;
                var mat = CurrentModel.Materials[index];
                List<CGeoset> geosets = GetGeosetsUsingMaterial(mat);
                if (geosets.Count < 2)
                {
                    MessageBox.Show("This material is used by less than 2 geosets"); return;
                }
                for (int i = 1; i < geosets.Count; i++)
                {
                    CMaterial generated = Duplicator.DuplicateMaterial(mat, CurrentModel);
                    CurrentModel.Materials.Add(mat);
                    geosets[i].Material.Attach(generated);
                }
                RefreshMaterialsList();
            }
            else
            {
                MessageBox.Show("Select a material");
            }

        }

        private List<CGeoset> GetGeosetsUsingMaterial(CMaterial mat)
        {
            return CurrentModel.Geosets.Where(x => x.Material.Object == mat).ToList();
        }

        private void edit_gs_Data(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                var geoset = GetSelectedGeosets()[0];
                geoset_data_editor gd = new geoset_data_editor(geoset);
                gd.ShowDialog();
            }
            else
            {
                MessageBox.Show("Select a single geoset"); return;
            }
        }

        private void reattachvertex(object sender, RoutedEventArgs e)
        {
            // this function will swap the vertices of the triangles 
            var vertices = GetSelectedVertices();
            if (vertices.Count != 2)
            {
                MessageBox.Show("Select exactly two vertices");
                return;
            }

            CGeoset? geoset = VerticesBelongToSameGeoset(vertices);
            if (geoset == null)
            {
                MessageBox.Show("Selected vertices do not belong to the same geoset");
                return;
            }

            var vertexA = vertices[0];
            var vertexB = vertices[1];

            foreach (var triangle in geoset.Triangles)
            {
                // Replace A with temp marker if present
                if (triangle.Vertex1.Object == vertexA) triangle.Vertex1.Attach(null);
                if (triangle.Vertex2.Object == vertexA) triangle.Vertex2.Attach(null);
                if (triangle.Vertex3.Object == vertexA) triangle.Vertex3.Attach(null);
            }

            foreach (var triangle in geoset.Triangles)
            {
                // Replace B with A
                if (triangle.Vertex1.Object == vertexB) triangle.Vertex1.Attach(vertexA);
                if (triangle.Vertex2.Object == vertexB) triangle.Vertex2.Attach(vertexA);
                if (triangle.Vertex3.Object == vertexB) triangle.Vertex3.Attach(vertexA);
            }

            foreach (var triangle in geoset.Triangles)
            {
                // Replace null (was A) with B
                if (triangle.Vertex1.Object == null) triangle.Vertex1.Attach(vertexB);
                if (triangle.Vertex2.Object == null) triangle.Vertex2.Attach(vertexB);
                if (triangle.Vertex3.Object == null) triangle.Vertex3.Attach(vertexB);
            }

            MessageBox.Show("Vertices swapped successfully.");
        }

        private void Checked_UseColor(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            bool val = CheckUseColorGA.IsChecked == true;
            if (List_GeosetAnims.SelectedItem != null)
            {
                var ga = CurrentModel.GeosetAnimations[List_GeosetAnims.SelectedIndex];
                ga.UseColor = val;
                RefreshGeosetAnimationsList();
            }
        }

        private void Checked_DropShadow(object sender, RoutedEventArgs e)
        {
            if (Pause) return;
            bool val = CheckDropShadow.IsChecked == true;
            if (List_GeosetAnims.SelectedItem != null)
            {
                var ga = CurrentModel.GeosetAnimations[List_GeosetAnims.SelectedIndex];
                ga.DropShadow = val;
                RefreshGeosetAnimationsList();
            }
        }

        private void SelectedGeosetAnim(object sender, SelectionChangedEventArgs e)
        {
            if (List_GeosetAnims.SelectedItem != null)
            {
                var ga = CurrentModel.GeosetAnimations[List_GeosetAnims.SelectedIndex];

                Pause = true;
                CheckUseColorGA.IsChecked = ga.UseColor;
                CheckDropShadow.IsChecked = ga.DropShadow;
                Pause = false;
            }
        }

        private void sentence(object sender, RoutedEventArgs e)
        {
            if (ListSequenes.SelectedItem != null)
            {
                var sequence = CurrentModel.Sequences[ListSequenes.SelectedIndex];
                sequence.Name = StringHelper.TitleCase(sequence.Name);
            }
            RefreshSequencesList();
            History.Sequences.Add(CurrentModel);
        }

        private void sentence2(object sender, RoutedEventArgs e)
        {
            foreach (var sequence in CurrentModel.Sequences)
            {
                sequence.Name = StringHelper.TitleCase(sequence.Name);
            }
            RefreshSequencesList();
            History.Sequences.Add(CurrentModel);
        }

        private void SwapGeosetAnimationsOfGeosets(object sender, RoutedEventArgs e)
        {
            // Swap the geoset animations of two geosets
            var geosets = GetSelectedGeosets();
            if (geosets.Count != 2)
            {
                MessageBox.Show("Select exactly two geosets");
                return;
            }

            // Find the geoset animations of them
            CGeosetAnimation? first = GetGeosetAnimationOfGeoset(geosets[0]);
            CGeosetAnimation? second = GetGeosetAnimationOfGeoset(geosets[1]);

            if (first != null && second != null)
            {
                // Swap
                first.Geoset.Attach(geosets[1]);
                second.Geoset.Attach(geosets[0]);
            }
            else
            {
                MessageBox.Show("Not all geosets are using a geoset animation");
                return;
            }
        }


        private CGeosetAnimation? GetGeosetAnimationOfGeoset(CGeoset cGeoset)
        {
            foreach (var ga in CurrentModel.GeosetAnimations)
            {
                if (ga.Geoset.Object == cGeoset) { return ga; }
            }
            return null;
        }

        private void swapcolors(object sender, RoutedEventArgs e)
        {

        }

        private void swapalphasGA(object sender, RoutedEventArgs e)
        {

        }

        private void SelectGaofG(object sender, RoutedEventArgs e)
        {
            var geosets = GetSelectedGeosets();
            if (geosets.Count != 1)
            {
                MessageBox.Show("Select a single geoset"); return;
            }
            var ga = CurrentModel.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == geosets[0]);
            if (ga != null)
            {
                var index = CurrentModel.GeosetAnimations.IndexOf(ga);
                List_GeosetAnims.SelectedIndex = index;
            }
        }

        private void setallextentsfixed(object sender, RoutedEventArgs e)
        {
            Edit_Extent ee = new Edit_Extent(true, CurrentModel);
            ee.ShowDialog();

        }
        private CGeosetGroup CopiedMatrixGroupAttachment;
        private NodeMaker NodeCollection;


        private void copymga(object sender, RoutedEventArgs e)
        {
            var vertices = GetSelectedVertices();
            if (vertices.Count == 0)
            {
                MessageBox.Show("Select at least one vertex"); return;
            }
            else if (vertices.Count == 1)
            {
                CopiedMatrixGroupAttachment = vertices[0].Group.Object;
            }
            else if (vertices.Count > 1)
            {
                var owner = VerticesBelongToSameGeoset(vertices);
                if (owner == null)
                {
                    MessageBox.Show("The selected vertices do not belong to the same geoset. Their matrix groups will never be the same.");
                    return;
                }
                bool same = false;
                var first = vertices[0].Group.Object;
                for (int i = 1; i < vertices.Count; i++)
                {
                    if (first != vertices[i].Group.Object)
                    {
                        MessageBox.Show("Not all selected vertiecs use the same group");
                        same = false;
                        break;
                    }
                }
                if (same)
                {
                    CopiedMatrixGroupAttachment = vertices[0].Group.Object;
                }

            }

        }

        private void pastemga(object sender, RoutedEventArgs e)
        {
            if (CopiedMatrixGroupAttachment == null)
            {
                MessageBox.Show("Nothing was copied"); return;
            }
            var vertices = GetSelectedVertices();
            foreach (var vertex in vertices)
            {
                vertices[0].Group.Attach(CopiedMatrixGroupAttachment);
            }
            RefreshrigingAttachedList();
        }

        private void debug_console(object sender, RoutedEventArgs e)
        {
            Debug_Console.Show();

        }

        private void tellModelExxtent(object sender, RoutedEventArgs e)
        {
            // Report the maximum extent of the model
            if (CurrentModel.Geosets.Count == 0)
            {
                MessageBox.Show("There are no geosets");
                return;
            }

            Extent ex = new Extent();

            // Assuming Extent initializes with extreme values:
            // ex.minX = ex.minY = ex.minZ = float.MaxValue;
            // ex.maxX = ex.maxY = ex.maxZ = float.MinValue;

            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    float x = vertex.Position.X;
                    float y = vertex.Position.Y;
                    float z = vertex.Position.Z;

                    if (x < ex.minX) ex.minX = x;
                    if (y < ex.minY) ex.minY = y;
                    if (z < ex.minZ) ex.minZ = z;

                    if (x > ex.maxX) ex.maxX = x;
                    if (y > ex.maxY) ex.maxY = y;
                    if (z > ex.maxZ) ex.maxZ = z;
                }
            }

            MessageBox.Show($"The extents are:\n" +
                $"min X: {ex.minX}\n" +
                $"min Y: {ex.minY}\n" +
                $"min Z: {ex.minZ}\n" +
                $"max X: {ex.maxX}\n" +
                $"max Y: {ex.maxY}\n" +
                $"max Z: {ex.maxZ}");
        }

        private void swapRigging(object sender, RoutedEventArgs e)
        {
            var vertices = GetSelectedVertices();
            if (vertices.Count < 2)
            {
                MessageBox.Show("Select two or more vertices");
                return;
            }

            Dictionary<CGeosetGroup, List<CGeosetVertex>> attachmentReference = new Dictionary<CGeosetGroup, List<CGeosetVertex>>();

            // Group vertices by their matrix group
            foreach (var vertex in vertices)
            {
                var group = vertex.Group.Object;
                if (!attachmentReference.TryGetValue(group, out var list))
                {
                    list = new List<CGeosetVertex>();
                    attachmentReference[group] = list;
                }
                list.Add(vertex);
            }

            if (attachmentReference.Count != 2)
            {
                MessageBox.Show("There must be exactly two groups of matrix groups in order to swap");
                return;
            }

            // Get the two distinct groups
            var groupA = attachmentReference.Keys.ElementAt(0);
            var groupB = attachmentReference.Keys.ElementAt(1);

            // Swap: store target groups to avoid interference
            foreach (var vertex in attachmentReference[groupA])
            {
                vertex.Group.Attach(groupB);
            }

            foreach (var vertex in attachmentReference[groupB])
            {
                vertex.Group.Attach(groupA);
            }
            RefreshrigingAttachedList();
        }

        private void viewbig_dc(object sender, MouseButtonEventArgs e)
        {
            ViewBig(null, null);
        }

        private void GenerateReport(object sender, RoutedEventArgs e)
        {
            ShowErrors();
        }

        private void putongroundvertices_half(object sender, RoutedEventArgs e)
        {
            if (Tabs_Geosets.SelectedIndex == 0)
            {
                var Vertices = GetSelectedGeosets().SelectMany(x => x.Vertices.ObjectList).ToList();
                Calculator.PutOnGround_Half(Vertices);
            }
            else if (Tabs_Geosets.SelectedIndex == 1) // triangles
            {
                var Vertices = getSelectedTriangles().SelectMany(x => x.Vertices).Distinct().ToList();
                Calculator.PutOnGround_Half(Vertices);
            }
            else if (Tabs_Geosets.SelectedIndex == 0) // vertices
            {
                List<CGeosetVertex> Vertices = GetSelectedVertices();
                Calculator.PutOnGround_Half(Vertices);
            }
        }

        private void gadeduplicator_click(object sender, RoutedEventArgs e)
        {
            gadeduplicator g = new gadeduplicator(CurrentModel);
            try
            {
                g.ShowDialog();
            }
            catch { return; }
            RefreshGeosetAnimationsList();
        }

        private void Scene_Selection_Overlay_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 0) return;
                string file = files[0];
                if
                   (Path.GetExtension(file).ToLower() == ".mdx"
                   ||
                   Path.GetExtension(file).ToLower() == ".mdl"
                   )
                {

                    LoadModel(file);
                }

            }
        }

        private void Scene_Selection_Overlay_DragOver(object sender, DragEventArgs e)
        {
            // Allow file drop only
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void seModeSculpt(object sender, RoutedEventArgs e)
        {
            modifyMode_current = ModifyMode.Sculpt;
            HightlightMoreButton();
        }

        private void seModeSculpt2(object sender, RoutedEventArgs e)
        {
            modifyMode_current = ModifyMode.SculptPersonal;
            HightlightMoreButton();
        }

        private void askwgmv(object sender, RoutedEventArgs e)
        {
            var l = CurrentModel.Geosets.Where(x => CurrentModel.GeosetAnimations.Any(y => y.Geoset.Object == x) == false);
            if (l.Count() == 0)
            {
                MessageBox.Show("All geosets are used by geoset animations"); return;
            }
            var d = string.Join(", ", l.Select(x => x.ObjectId.ToString()));
            MessageBox.Show(d);
        }

        private void EditNodeScalings(object sender, MouseButtonEventArgs e)
        {
            var node = GetSelectedNode();
            if (node == null)
            {
                MessageBox.Show("Select a node"); return;
            }
            else
            {
                editvisibilities_window v = new editvisibilities_window(CurrentModel, node);
                v.ShowDialog();
                SelectedNodeInNodeEditor(null, null);
            }
        }

        private void MakeParticlesDisappearAtTheEndOfTheirSequences(object sender, RoutedEventArgs e)
        {
            foreach (INode node in CurrentModel.Nodes)
            {
                if (node is CParticleEmitter2 e2 && e2.Visibility != null)
                    MakeParticleVisibilityDisappearAtEnds(e2.Visibility);

                if (node is CParticleEmitter e1 && e1.Visibility != null)
                    MakeParticleVisibilityDisappearAtEnds(e1.Visibility);

                if (node is CRibbonEmitter r && r.Visibility != null)
                    MakeParticleVisibilityDisappearAtEnds(r.Visibility);
            }
        }

        private void MakeParticleVisibilityDisappearAtEnds(CAnimator<float> visibility)
        {
            if (visibility.Animated == false)
            {
                visibility.MakeAnimated();
                foreach (var sequence in CurrentModel.Sequences)
                {
                    visibility.NodeList.Add(
                        new CAnimatorNode<float>()
                        {
                            Time = sequence.IntervalStart,
                            Value = 1
                        });
                    visibility.NodeList.Add(
                       new CAnimatorNode<float>()
                       {
                           Time = sequence.IntervalEnd,
                           Value = 0
                       });
                }
            }
            else
            {
                var existingTimes = visibility.NodeList.Select(x => x.Time).ToHashSet();
                var handledSequences = new HashSet<CSequence>();
                var newKeyframes = new List<CAnimatorNode<float>>();

                foreach (var kf in visibility.NodeList)
                {
                    var sequence = GetSequenceFromTrack(kf.Time);
                    if (sequence == null || handledSequences.Contains(sequence)) continue;

                    if (!existingTimes.Contains(sequence.IntervalEnd))
                    {
                        newKeyframes.Add(new CAnimatorNode<float>
                        {
                            Time = sequence.IntervalEnd,
                            Value = 0
                        });
                    }

                    handledSequences.Add(sequence);
                }
                visibility.NodeList.AddRange(newKeyframes);
            }



            visibility.NodeList = visibility.NodeList.OrderBy(x => x.Time).ToList();
        }


        private void editNodeVisibilities(object sender, MouseButtonEventArgs e)
        {
            if (ListNodes.SelectedItem == null) { return; }
            editvisibilities_window? ew = null;
            var node = GetSelectedNode();
            if (node is CParticleEmitter emitter)
            {
                ew = new editvisibilities_window(CurrentModel, emitter.Visibility);
            }
            else if (node is CParticleEmitter2 emitter2)
            {
                ew = new editvisibilities_window(CurrentModel, emitter2.Visibility);
            }
            else if (node is CRibbonEmitter ribbon)
            {
                ew = new editvisibilities_window(CurrentModel, ribbon.Visibility);
            }
            else if (node is CLight light)
            {
                ew = new editvisibilities_window(CurrentModel, light.Visibility);
            }
            else if (node is CAttachment att)
            {
                ew = new editvisibilities_window(CurrentModel, att.Visibility);
            }
            else if (node is CEvent ev)
            {
                ew = new editvisibilities_window(CurrentModel, ev);
            }
            else { MessageBox.Show("This node does not support visibility"); return; }

            if (ew.ShowDialog() == true)
            {
                SelectedNodeInNodeEditor(null, null);
            }
        }

        private void MakeGeosetAnimationsDisappearAtTheEndOfDeathSequence(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences");
                return;
            }

            List<string> names = CurrentModel.Sequences.Select(x => x.Name).ToList();
            Selector s = new Selector(names, "Which sequence?");

            s.ShowDialog();
            if (s.DialogResult == true)
            {
                int index = s.box.SelectedIndex;
                var sequence = CurrentModel.Sequences[index];
                foreach (var ga in CurrentModel.GeosetAnimations)
                {
                    if (ga.Alpha.Animated)
                    {

                        if (ga.Alpha.Any(x => x.Time >= sequence.IntervalStart && x.Time < sequence.IntervalEnd))
                        {
                            ga.Alpha.NodeList.Add(new CAnimatorNode<float>
                            {
                                Time = sequence.IntervalEnd,
                                Value = 0
                            });
                        }

                    }
                    else
                    {
                        ga.Alpha.MakeAnimated();
                        foreach (var loopedSequence in CurrentModel.Sequences)
                        {
                            if (loopedSequence == sequence)
                            {
                                ga.Alpha.NodeList.Add(new CAnimatorNode<float> { Time = loopedSequence.IntervalStart, Value = 1 });
                                ga.Alpha.NodeList.Add(new CAnimatorNode<float> { Time = loopedSequence.IntervalEnd, Value = 0 });
                            }
                            else
                            {
                                ga.Alpha.NodeList.Add(new CAnimatorNode<float> { Time = loopedSequence.IntervalStart, Value = 1 });
                            }
                        }
                    }

                    ga.Alpha.NodeList = ga.Alpha.NodeList.OrderBy(x => x.Time).ToList();
                }
            }



        }

        private void DetachSelectedBoneFromGeoset(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Geosets.Count == 9)
            {
                MessageBox.Show("There are no geosets"); return;
            }
            if (ListNodes.SelectedItem == null)
            {
                MessageBox.Show("Select a node");
            }
            else
            {
                var node = GetSelectedNode();
                List<string> geosets = CurrentModel.Geosets.Select(x => x.ObjectId.ToString()).ToList();
                Selector s = new Selector(geosets, "Geoset");
                s.ShowDialog();
                if (s.DialogResult == true)
                {
                    int index = s.box.SelectedIndex;
                    CGeoset geoset = CurrentModel.Geosets[index];
                    if (geoset.Groups.Count == 0) { MessageBox.Show("This geoset has no matrix groups"); return; }
                    if (GeosetUsesNode(geoset, node))
                    {
                        List<int> skipped = new List<int>();
                        int removed = 0;

                        for (int i = 0; i < geoset.Groups.Count; i++)
                        {
                            var group = geoset.Groups[i];

                            if (group.Nodes.Count == 1)
                            {
                                if (group.Nodes[0].Node.Node == node) skipped.Add(i);
                            }
                            else
                            {
                                int at = -1;
                                for (int j = 0; j < group.Nodes.Count; j++)
                                {
                                    if (group.Nodes[j].Node.Node == node) at = j; break;
                                }
                                if (at >= 0) { group.Nodes.RemoveAt(at); removed++; }
                            }
                        }


                        MessageBox.Show($"Removed {removed} instanced of node '{node.Name}' from geoset {geoset.ObjectId}");
                        if (skipped.Count > 0)
                        {
                            MessageBox.Show($"Skipped {skipped} instances of  node '{node.Name}' from geoset {geoset.ObjectId}, because some matrix groups contain only one node - this node");
                        }

                    }
                    else
                    {
                        MessageBox.Show($"Geoset {geoset.ObjectId} is not attached to node '{node.Name}'"); return;
                    }




                }

            }
        }
        private bool GeosetUsesNode(CGeoset g, INode node)
        {
            foreach (var group in g.Groups)
            {
                foreach (var gnode in group.Nodes)
                {
                    if (gnode.Node.Node == node) return true;
                }
            }
            return false;
        }

        private void convertH2B(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null)
            {
                MessageBox.Show("Select a node"); return;
            }
            else
            {
                var node = GetSelectedNode();
                if (node is CHelper helper)
                {

                    CBone bone = NodeCloner.HelperToBone(helper, CurrentModel);
                    var parent = helper.Parent.Node;
                    var children = getChildrenOfNode(node);
                    CurrentModel.Nodes.Remove(node);
                    CurrentModel.Nodes.Add(bone);
                    AddParentToNodes(children, bone);
                    if (parent != null) bone.Parent.Attach(parent);
                    RefreshNodesTree();
                }
                else
                {
                    MessageBox.Show("Selected node is not helper"); return;
                }
            }
        }

        private void AddParentToNodes(List<INode> children, INode bone)
        {
            foreach (var child in children)
            {
                child.Parent.Attach(bone);
            }
        }

        private object GetMatrixGroupsWhoUseNode(INode node)
        {
            throw new NotImplementedException();
        }

        private List<INode> getChildrenOfNode(INode node)
        {
            List<INode> list = new();
            foreach (INode n in CurrentModel.Nodes)
            {
                if (n.Parent.Node == node)
                {
                    if (n == node) continue;
                    list.Add(n);
                }

            }
            return list;
        }

        private void convertB2H(object sender, RoutedEventArgs e)
        {

            if (ListNodes.SelectedItem == null)
            {
                MessageBox.Show("Select a node"); return;
            }
            else
            {
                var node = GetSelectedNode();
                if (node is CBone bone)
                {
                    if (BoneHasAttachedVertices(bone))
                    {
                        MessageBox.Show("This bone has attached vertices. Reeattach them to another bone first");return;
                    }
                    var children = getChildrenOfNode(node);
                    CHelper h = NodeCloner.BoneToHelper(bone, CurrentModel);
                    var parent = bone.Parent.Node;
                    CurrentModel.Nodes.Remove(node);
                    CurrentModel.Nodes.Add(h);
                    AddParentToNodes(children, h);
                    if (parent!=null)  h.Parent.Attach(parent);
                    RefreshNodesTree();
                }
                else
                {
                    MessageBox.Show("Selected node is not bone"); return;
                }
            }
        }

        private bool BoneHasAttachedVertices(CBone bone)
        {
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var group in geoset.Groups)
                {
                    foreach (var node in group.Nodes)
                    {
                        if (node.Node.Node == bone) return true;
                    }
                }
            }
            return false;
        }

        private void createOverhead(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.Nodes.Any(x => x.Name.ToLower().Trim() == "overhead ref" && x is CAttachment))
            {
                MessageBox.Show("There is already an overhead attachment point");
            }
            else
            {
                CAttachment att = new CAttachment(CurrentModel);
                att.Name = "Overhead Ref";
                att.PivotPoint = new CVector3(0,0,200);
                CurrentModel.Nodes.Add(att);
                RefreshNodesTree();
            }
        }

        private void importcustomgeosetmanager(object sender, RoutedEventArgs e)
        {
            geoset_import_manager gm = new geoset_import_manager(CurrentModel);
         

            if (gm.Closed == true) return;
            Pause = true;
            gm.ShowDialog();
            if (gm.DialogResult == true)
            {
              
                SetSaved(false);
                refreshalllists(null, null);
                CollectTexturesOpenGL();
                
            }
            Pause = false;
        }

        private void saveGeomerge(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
                string path = Path.Combine(AppHelper.Local, "Geosets");
                Input i = new Input("", "Name?");
                if (i.ShowDialog() == true)
                {
                    string given = i.Result;
                    if (given.Length==0)
                    { MessageBox.Show("Empty input");return; }
                    string newPath = Path.Combine(path, given + ".tgeom");
                    if (File.Exists(newPath))
                    {
                        { MessageBox.Show("A file with that name already exists"); return; }
                    }

                    var g = GetSelectedGeosets();
                    GeosetExporter.ExportGeomergeCustom(CurrentModel, g[0], newPath);
                     
                }
            }
            else
            {
                MessageBox.Show("Select a single geoset");return;
            }
                
        }

        private void gradualgvm(object sender, RoutedEventArgs e)
        {
            GradualVisibilityMaker.Work(CurrentModel);
        }

        private void createNodePreset(object sender, RoutedEventArgs e)
        {
            INode node = null;
            if (ListNodes.SelectedItem != null) { node = GetSelectedNode(); }
            NodePresets n = new NodePresets(CurrentModel, node);
            n.ShowDialog();
            if (n.DialogResult == true)
            {
                RefreshNodesTree();
                RefreshTexturesList();
            }
        }

        private void SaveNodeAsPReset(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                var node = GetSelectedNode();
               string SourceDirectory= Path.Combine(AppHelper.Local, "NodePresets");
                string givenName = string.Empty;
                Input i = new Input("", "Name?");
                i.ShowDialog();
                if (i.DialogResult == true)
                {
                    givenName = i.Result;
                    if (PresetNameExists(givenName, SourceDirectory  ))
                    {
                        MessageBox.Show(" preset with that name exists");return;
                    }
                }
                if (node != null)
                {
                    NodePresetHandler.Save(node, givenName, SourceDirectory);
                }
               
            }
        }

        private bool PresetNameExists(string givenName, string folder)
        {
            if (string.IsNullOrWhiteSpace(givenName)) return true;

            try
            {
                string[] files = Directory.GetFiles(folder, "*.war3SFXp");

                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (string.Equals(fileName, givenName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                // Optionally log or handle exception (e.g., Directory not found or no permission)
                return false;
            }
        }

        private void mmgcc(object sender, RoutedEventArgs e)
        {
            ModelColorVariantGenerator.Generate();
        }

        private void maesatcs(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem != null)
            {
                var node = GetSelectedNode();
                if (node is CCollisionShape cs)
                {
                    CExtent ex = new CExtent();
                    ex.Max = new CVector3(cs.Vertex2);
                    ex.Min = new CVector3(cs.Vertex1);
                    ex.Radius = cs.Radius;
                    foreach (var geoset in CurrentModel.Geosets)
                    {
                        geoset.Extent = new CExtent(ex);
                        foreach (var sextent in geoset.Extents)
                        {
                            sextent.Extent = new CExtent(ex);
                        }
                    }
                    foreach (var sequence in CurrentModel.Sequences)
                    {
                        sequence.Extent = new CExtent(ex);
                    }
                }
                else
                {
                    MessageBox.Show("THe selected node is not a collision shape");
                }
            }
        }

        private void swapattachmentsBONE(object sender, RoutedEventArgs e)
        {
            if (ListNodes.SelectedItem == null)
            {
                MessageBox.Show("Select a node");return;

            }
            var node = GetSelectedNode();
                if (node is CBone bone)
            {
                var bones = CurrentModel.Nodes.Where(x => x is CBone).ToList();
                    if (bones.Count <= 1)
                {
                    MessageBox.Show("at least two bones must be present");return;
                }
                bones.Remove(bone);
                List<string> names = bones.Select(x => x.Name).ToList();
                Selector s = new Selector(names, "swap with which bone?");
                s.ShowDialog();
                    if (s.DialogResult == true){

                    string? name = s.Selected;
                    if (name == null) { MessageBox.Show("null selected string");return; }

                    CBone selected = CurrentModel.Nodes.FirstOrDefault(x => x.Name == name) as CBone;
                    if (selected == null) { MessageBox.Show("null bone"); return; }
                    SwapAttachmentsOfTwoBones(bone, selected);
                }
            }
            else
            {
                MessageBox.Show("Selected node is not a bone");
            }
        }

        private void SwapAttachmentsOfTwoBones(CBone first, CBone second)
        {
            var tempBone = new CBone(CurrentModel);

            // Temporarily reassign nodes attached to 'first' to 'temp'
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var group in geoset.Groups)
                {
                    foreach (var gnode in group.Nodes)
                    {
                        if (gnode.Node.Node == first)
                        {
                            gnode.Node.Attach(tempBone);
                        }
                    }
                }
            }

            // Reassign nodes attached to 'second' to 'first'
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var group in geoset.Groups)
                {
                    foreach (var gnode in group.Nodes)
                    {
                        if (gnode.Node.Node == second)
                        {
                            gnode.Node.Attach(first);
                        }
                    }
                }
            }

            // Reassign nodes attached to 'temp' (originally 'first') to 'second'
            foreach (var geoset in CurrentModel.Geosets)
            {
                foreach (var group in geoset.Groups)
                {
                    foreach (var gnode in group.Nodes)
                    {
                        if (gnode.Node.Node == tempBone)
                        {
                            gnode.Node.Attach(second);
                        }
                    }
                }
            }
        }

        private void swapgeosetatt(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count != 2)
            {
                var geosets = GetSelectedGeosets();
                if (GeosetsHAveONEGROup(geosets))
                {
                    SwapGeosetAttachments(geosets);
                }
                else
                {
                    MessageBox.Show("EaCh selected geoset msut have only one matrix group for this command to work");return;                }
            }
            {
                MessageBox.Show("Select exactly 2 geosets");return;
            }
        }

        private void SwapGeosetAttachments(List<CGeoset> geosets)
        {
            List<INode> nodes1 = new List<INode>();
            List<INode> nodes2 = new List<INode>();
             foreach (var node in geosets[0].Groups[0].Nodes)   nodes1.Add(node.Node.Node);
               foreach (var node in geosets[1].Groups[0].Nodes)   nodes2.Add(node.Node.Node);

            geosets[0].Groups[0].Nodes.Clear();
            geosets[1].Groups[0].Nodes.Clear();


            foreach (var node in nodes1)
            {
                CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                gnode.Node.Attach(node);
                geosets[1].Groups[0].Nodes.Add(gnode);
            }
            foreach (var node in nodes2)
            {
                CGeosetGroupNode gnode = new CGeosetGroupNode(CurrentModel);
                gnode.Node.Attach(node);
                geosets[0].Groups[0].Nodes.Add(gnode);
            }

          
        }

        private bool GeosetsHAveONEGROup(List<CGeoset> geosets)
        {
            foreach (var geoset in geosets)
            {
                if (geoset.Groups.Count != 1) return false;
                if (geoset.Groups[0] == null) return false;
                if (geoset.Groups[0].Nodes == null) return false;
                if (geoset.Groups[0].Nodes.ObjectList == null) return false;
            }



            return true;
        }

        private void improvisesprites(object sender, RoutedEventArgs e)
        {
            if (ListGeosets.SelectedItems.Count == 1)
            {
              INode[] nodes=  HandleSpriteAttachments();
                var g = GetSelectedGeosets()[0];
               var extent= Calculator.GetExtent(g);
                CVector3[] positions = Calculator.GetCeilingBurningPoints(extent);
                nodes[0].PivotPoint = new CVector3(positions[0]);
                nodes[1].PivotPoint = new CVector3(positions[1]);
                nodes[2].PivotPoint = new CVector3(positions[2]);
                nodes[3].PivotPoint = new CVector3(positions[3]);
                RefreshNodesTree();
            }
            else
            {
                MessageBox.Show("Select a single geoset");
            }
        }

        private INode[] HandleSpriteAttachments(bool ignoreCase = false)
        {
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            INode? first = CurrentModel.Nodes.FirstOrDefault(x => x is CAttachment && string.Equals(x.Name, "Sprite First Ref", comparison));
            INode? second = CurrentModel.Nodes.FirstOrDefault(x => x is CAttachment && string.Equals(x.Name, "Sprite Second Ref", comparison));
            INode? third = CurrentModel.Nodes.FirstOrDefault(x => x is CAttachment && string.Equals(x.Name, "Sprite Third Ref", comparison));
            INode? fourth = CurrentModel.Nodes.FirstOrDefault(x => x is CAttachment && string.Equals(x.Name, "Sprite Fourth Ref", comparison));

            if (first == null)
            {
                first = new CAttachment(CurrentModel) { Name = "Sprite First Ref" };
                CurrentModel.Nodes.Add(first);
            }
            if (second == null)
            {
                second = new CAttachment(CurrentModel) { Name = "Sprite Second Ref" };
                CurrentModel.Nodes.Add(second);
            }
            if (third == null)
            {
                third = new CAttachment(CurrentModel) { Name = "Sprite Third Ref" };
                CurrentModel.Nodes.Add(third);
            }
            if (fourth == null)
            {
                fourth = new CAttachment(CurrentModel) { Name = "Sprite Fourth Ref" };
                CurrentModel.Nodes.Add(fourth);
            }

            return new INode[] { first, second, third, fourth };
        }

        private void createSkeleton(object sender, RoutedEventArgs e)
        {
            CreateSkeleton_window c = new CreateSkeleton_window(CurrentModel);
            c.ShowDialog();
            if (c.DialogResult == true)
            {
                RefreshNodesTree();
            }
        }
    }
}

 