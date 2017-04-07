using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace PersonalTesting
{
    [TestClass]
    public class testCases
    {

        /// <summary>
        /// Tests for replacing a nodes dependents with the dependents it already has
        /// </summary>
        [TestMethod]
        public void testReplaceSame()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("a", "b");
            graph.AddDependency("b", "a");

            graph.ReplaceDependents("b", new List<string>() { "a" });

            IEnumerator<string> enumerator = graph.GetDependees("a").GetEnumerator();

            enumerator.MoveNext();

            //replacing dependents of B with A again shouldn't affect A's dependees
            Assert.IsTrue(enumerator.Current == "b");
        }

        [TestMethod]
        public void testEmptyString()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("", "a");
            graph.AddDependency("", "b");
            graph.AddDependency("", "c");

            Assert.IsTrue(graph.HasDependents(""));
            Assert.IsTrue(graph.Size == 3);
        }

        [TestMethod]
        public void testEmptyReplace()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("a", "b");
            graph.AddDependency("b", "a");

            graph.ReplaceDependents("b", new List<string>());


            Assert.IsTrue(graph["a"] == 0);
            Assert.IsTrue(graph["b"] == 1);
            Assert.IsTrue(graph.Size == 1);
        }
    }
}
