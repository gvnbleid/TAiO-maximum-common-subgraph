﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;

namespace GeneticAlgorithm
{
    class GeneticAlgorithm
    {
        private readonly int _generationSize;
        private readonly int _generationCount;
        private readonly bool _breakWhenScoreDrops;
        private List<Graph> _generation;

        public GeneticAlgorithm(int generationSize, int generationCount, bool breakWhenScoreDrops)
        {
            _generationSize = generationSize;
            _generationCount = generationCount;
            _breakWhenScoreDrops = breakWhenScoreDrops;
        }

        public Graph FindMaximalCommonSubgraph(Graph g1, Graph g2)
        {
            var best = new Graph(new int[1, 1]);
            _generation = new List<Graph>();
            var maxSize = g1.Size < g2.Size ? g1.Size : g2.Size;
            var generationScore = int.MinValue;
            CreateFirstGeneration(_generationSize, maxSize);
            _generation.ForEach(g => g.CreateRandomMatching1(g1));
            _generation.ForEach(g => g.CreateRandomMatching2(g2));
            for (var i = 0; i < _generationCount; i++)
            {
                AssignScores(g1, g2);
                _generation.Sort((graph1, graph2) => graph1.Score.CompareTo(graph2.Score));
                if (_generation.First().Score > best.Score) best = _generation.First().Clone();
                KillHalfOfTheGeneration();
                var babies = MakeBabies(g1, g2);
                _generation.AddRange(babies);
                ApplyMutation();
                var newGenerationScore = CalculateGenerationScore();
                if (newGenerationScore < generationScore && _breakWhenScoreDrops) break;
                generationScore = newGenerationScore;
#if DEBUG
                AssignScores(g1, g2);
                Console.WriteLine($"Generation #{i}, score = {CalculateGenerationScore()}");
#endif
            }
            AssignScores(g1, g2);
            var bestScore = _generation.Max(graph => graph.Score);
            var bestInLastGeneration = _generation.First(graph => graph.Score == bestScore);
            return best.Score > bestInLastGeneration.Score ? best : bestInLastGeneration;
        }

        private void CreateFirstGeneration(int generationSize, int maxSize)
        {
            for (var i = 0; i < generationSize; i++)
            {
                _generation.Add(GraphGeneticExtensions.CreateRandomGraph(maxSize));
            }
        }

        private int CalculateGenerationScore()
        {
            return _generation.Sum(graph => graph.Score);
        }

        private void ApplyMutation()
        {
            foreach (var graph in _generation)
            {
                var sizebefore = graph.Size;
                graph.Mutate();
            }
        }

        private List<Graph> MakeBabies(Graph g1, Graph g2)
        {
            var babies = new List<Graph>();
            var minScore = _generation.Min(graph => graph.Score);
            if (minScore <= 0)
            {
                _generation.ForEach(graph => graph.NormalizedScore = graph.Score - minScore + 1);
            }
            else
            {
                _generation.ForEach(graph => graph.NormalizedScore = graph.Score);
            }

            var maxScore = _generation.Max(graph => graph.NormalizedScore);
            while (babies.Count < _generationSize / 2)
            {
                var fatherIndex = SelectParentIndex(null);
                var motherIndex = SelectParentIndex(fatherIndex);
                var babyGraph = GraphGeneticExtensions.CreateChild(_generation[motherIndex], _generation[fatherIndex], g1.Size, g2.Size);
                babies.Add(babyGraph);
            }

            return babies;
        }

        private void KillHalfOfTheGeneration()
        {
            var indicesOfGraphsToRemove = new List<int>();
            var index = 0;
            while (indicesOfGraphsToRemove.Count < _generationSize / 2)
            {
                if (GoodRandom.Next(_generationSize) > index && !indicesOfGraphsToRemove.Contains(index))
                {
                    indicesOfGraphsToRemove.Add(index);
                }

                index = (index + 1) % _generationSize;
            }

            indicesOfGraphsToRemove.Sort((index1, index2) => -index1.CompareTo(index2));
            foreach (var i in indicesOfGraphsToRemove)
            {
                _generation.RemoveAt(i);
            }
        }

        private void AssignScores(Graph g1, Graph g2)
        {
            foreach (var graph in _generation)
            {
                graph.Score = CalculateScore(g1, g2, graph);
            }
        }

        #region HelperMethods

        private int SelectParentIndex(int? blockedIndex)
        {
            var indices = new List<int>();
            for (var i = 0; i < _generation.Count; i++)
            {
                if (blockedIndex.HasValue && blockedIndex.Value == i) continue;
                for (var j = 0; j < _generation[i].NormalizedScore; j++)
                {
                    indices.Add(i);
                }
            }

            var result = GoodRandom.Next(indices.Count);
            return indices[result];
        }

        private static int CalculateScore(Graph g1, Graph g2, Graph target)
        {
            if (target.CalculateNumberOfUnconnectedSubgraphs() > 1) return -(g1.Size + g2.Size);                                   //podgraf niespójny
            if (target.Size > g1.Size || target.Size > g2.Size) return -(g1.Size + g2.Size);                            //podgraf większy od któregoś z wejściowych
            if (target.EdgesCount > g1.EdgesCount || target.EdgesCount > g2.EdgesCount) return -(g1.Size + g2.Size);    //podgraf ze zby dużą ilością krawędzi
            var n = 2 * target.EdgesCount;
            var t1 = target.NumberOfUnconnectedSubgraphsInMatching1(g1, out var unableToCalculate1);
            var t2 = target.NumberOfUnconnectedSubgraphsInMatching2(g2, out var unableToCalculate2);
            if (unableToCalculate1 || unableToCalculate2) return -(g1.Size + g2.Size);                                  //krawędź w podgrafie nie istnieje w zródłowym
            return n - (t1 + t2 - 2);
        }

        #endregion
    }
}
