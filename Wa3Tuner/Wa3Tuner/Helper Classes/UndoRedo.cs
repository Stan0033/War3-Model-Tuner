
using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Wa3Tuner.Helper_Classes
{
    
    public static class History
    {
        public static int HistoryLimit = 200; // for all
        public static void SetLimit(int value) // for all
        {
            if (HistoryLimit < 0) return;
            Geometry.SetLimit(value);
            Nodes.SetLimit(value);
            Sequences.SetLimit(value);

        }
        public static class Sequences
        {
            public static List<List<CSequence>> Storage = new();
            private static int Index = 0;
            public static void Clear()
            {
                Storage.Clear();
            }
           
            public static void SetLimit(int value)
            {
                if (Index > HistoryLimit)
                {
                    Index = HistoryLimit;
                    DeleteAfterCurrentIndex();
                }
                while (Storage.Count > HistoryLimit)
                {
                    Storage.RemoveAt(0);
                    Index--;
                }
                Index = Math.Max(0, Index);


            }


            public static void Undo(CModel model, MainWindow window)
            {
                return; // unfinished
                if (Index > 0)
                {
                    Index--;
                    ReassignSequencesToModel(model, window);
                }
            }

            public static void Redo(CModel model, MainWindow window)
            {
                return; // unfinished
                if (Index + 1 < Storage.Count)
                {
                    Index++;
                    ReassignSequencesToModel(model, window);
                }
            }

            private static void ReassignSequencesToModel(CModel model, MainWindow window)
            {
                model.Sequences.Clear();
                foreach (var sequence in Storage[Index])
                {
                    var copy = Duplicator.DuplicateSequence(sequence, model);
                    model.Sequences.Add(copy);
                }
                window.RefreshSequencesList();
            }

            
            public static void Add(CModel model )  
            {
                return; // unfinished
                if (Storage.Count == HistoryLimit)
                {
                    Storage.RemoveAt(0);
                    Index--;
                }
                 DeleteAfterCurrentIndex();
                DuplicateNewSequenceListToStorage(model);
                  Index = Storage.Count - 1;
            }

            private static void DuplicateNewSequenceListToStorage(CModel model)
            {
                List<CSequence> newList = new List<CSequence>();
                foreach (var sequence in model.Sequences)
                {
                    newList.Add(Duplicator.DuplicateSequence(sequence, model));
                }
                 Storage.Add(newList);
              
            }

            private static void DeleteAfterCurrentIndex()
            {
                if (Index < Storage.Count - 1)
                {
                    Storage.RemoveRange(Index + 1, Storage.Count - Index - 1);
                }
            }

        }
        public static class Geometry
        {
            public static List<List<CGeoset>> Storage = new();

            
            private static int Index = 0;
            public static void Clear()
            {
                Storage.Clear();
            }

            public static void SetLimit(int value)
            {
                if (Index > HistoryLimit)
                {
                    Index = HistoryLimit;
                    DeleteAfterCurrentIndex();
                }
                while (Storage.Count > HistoryLimit)
                {
                    Storage.RemoveAt(0);
                    Index--;
                }
                Index = Math.Max(0, Index);


            }


            public static void Undo(CModel model, MainWindow window)
            {
                if (Index > 0)
                {
                    Index--;
                    ReassignGeosesToModel(model, window);
                }
            }

            public static void Redo(CModel model, MainWindow window)
            {
                if (Index + 1 < Storage.Count)
                {
                    Index++;
                    ReassignGeosesToModel(model, window);
                }
            }

            private static void ReassignGeosesToModel(CModel model, MainWindow window)
            {
                model.Geosets.Clear();
                foreach (var geo in Storage[Index])
                {
                    var copy = Duplicator.DuplicateGeoset(geo, model);
                    model.Geosets.Add(copy);
                }
                window.RefreshGeosetsList();
            }


            public static void Add(CModel model)
            {

                if (Storage.Count == HistoryLimit)
                {
                    Storage.RemoveAt(0);
                    Index--;
                }
                DeleteAfterCurrentIndex();
                DuplicateNewGeosetListToStorage(model);
                Index = Storage.Count - 1;
            }

            private static void DuplicateNewGeosetListToStorage(CModel model)
            {
                List<CGeoset> newList = new List<CGeoset>();
                foreach (var sequence in model.Geosets)
                {
                    newList.Add(Duplicator.DuplicateGeoset(sequence, model));
                }
                Storage.Add(newList);

            }

            private static void DeleteAfterCurrentIndex()
            {
                if (Index < Storage.Count - 1)
                {
                    Storage.RemoveRange(Index + 1, Storage.Count - Index - 1);
                }
            }
        }
        public static class Nodes
        {
            public static List<List<INode>> Storage = new();


            private static int Index = 0;
            public static void Clear()
            {
                Storage.Clear();
            }

            public static void SetLimit(int value)
            {
                if (Index > HistoryLimit)
                {
                    Index = HistoryLimit;
                    DeleteAfterCurrentIndex();
                }
                while (Storage.Count > HistoryLimit)
                {
                    Storage.RemoveAt(0);
                    Index--;
                }
                Index = Math.Max(0, Index);


            }


            public static void Undo(CModel model, MainWindow window)
            {
                if (Index > 0)
                {
                    Index--;
                    ReassignNodesToModel(model, window);
                }
            }

            public static void Redo(CModel model, MainWindow window)
            {
                if (Index + 1 < Storage.Count)
                {
                    Index++;
                    ReassignNodesToModel(model, window);
                }
            }

            private static void ReassignNodesToModel(CModel model, MainWindow window)
            {
                model.Geosets.Clear();
                foreach (var node in Storage[Index])
                {
                    var copy = NodeCloner.Clone(node, model);
                    model.Nodes.Add(copy);
                }
                window.RefreshNodesTree();
            }


            public static void Add(CModel model)
            {

                if (Storage.Count == HistoryLimit)
                {
                    Storage.RemoveAt(0);
                    Index--;
                }
                DeleteAfterCurrentIndex();
                DuplicateNewNodeListToStorage(model);
                Index = Storage.Count - 1;
            }

            private static void DuplicateNewNodeListToStorage(CModel model)
            {
                List<INode> newList = new List<INode>();
                foreach (var node in model.Nodes)
                {
                    newList.Add(NodeCloner.Clone(node, model));
                }
                Storage.Add(newList);

            }

            private static void DeleteAfterCurrentIndex()
            {
                if (Index < Storage.Count - 1)
                {
                    Storage.RemoveRange(Index + 1, Storage.Count - Index - 1);
                }
            }
        }

    }
}
 