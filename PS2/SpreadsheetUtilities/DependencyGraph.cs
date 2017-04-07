//Code implementation by Derek Burns, u0907203

// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {

        /// <summary>
        /// This class represents a node on the DependencyGraph.
        /// </summary>
        private class DependencyNode
        {
            public string nodeName;
            public List<DependencyNode> dependents;
            public List<DependencyNode> dependees;
            
            /// <summary>
            /// Constructs a new node associated with the input string.
            /// </summary>
            /// <param name="nodeName">Unique string to be associated with the new node.</param>
            public DependencyNode(string nodeName)
            {
                this.nodeName = nodeName;
                this.dependents = new List<DependencyNode>();
                this.dependees = new List<DependencyNode>();
            }

            /// <summary>
            /// Returns a list of the node's dependents.
            /// </summary>
            /// <returns></returns>
            public List<string> listDependents()
            {
                List<string> stringList = new List<string>(); //TODO: How does all of this get affected by null strings?

                foreach (DependencyNode node in this.dependents)
                {
                    stringList.Add(node.nodeName);
                }
                return stringList;
            }

            /// <summary>
            /// Returns a list of the node's dependees.
            /// </summary>
            /// <returns></returns>
            public List<string> listDependees()
            {
                List<string> stringList = new List<string>();

                foreach (DependencyNode node in this.dependees)
                {
                    stringList.Add(node.nodeName);
                }
                return stringList;
            }


            /// <summary>
            /// Add a dependency where the input node is the dependent. The dependency is created both ways.
            /// (This node gets a reference to the dependent, and the dependent gets a reference to this node).
            /// 
            /// Adjusts the size of the DependencyGraph accordingly.
            /// </summary>
            /// <param name="dependent">The node that becomes the dependent of the current node.</param>
            /// <param name="dependencySize">Reference to the DependencyGraph's size count.</param>
            public void addDependent(DependencyNode dependent, ref int dependencySize)
            {
                //If the dependency doesn't exist, add it, and adjust the DependencyGraph's "size" accordingly.
                if (!dependents.Contains(dependent))
                {
                    this.dependents.Add(dependent);
                    dependent.dependees.Add(this); //Ensure that the dependent has this node as a dependee.
                    dependencySize++;
                }
            }

            /// <summary>
            /// Remove a dependency where the input node is the dependent. The dependency is removed both ways.
            /// (This node no longer has a reference to the dependent, and the dependent has no reference to this node).
            /// 
            /// Adjusts the size of the DependencyGraph accordingly.
            /// </summary>
            /// <param name="dependent">The node that is the dependent of the current node</param>
            /// <param name="dependencySize">Reference to the DependencyGraph's size count</param>
            internal void removeDependent(DependencyNode dependent, ref int dependencySize)
            {
                //If the dependency exists, remove it, and adjust the DependencyGraph's "size" accordingly.
                if (dependents.Contains(dependent))
                {
                    dependents.Remove(dependent);
                    dependent.dependees.Remove(this); //Remove the dependent's reference to this node as well.
                    dependencySize--;
                }
            }

            /// <summary>
            /// Removes the dependencies between this node and all the dependents.
            /// </summary>
            /// <param name="_size">Reference to the DependencyGraph's size count</param>
            internal void removeAllDependents(ref int _size)
            {
                //Can't use foreach loop, so iterate through the list of dependents with a while loop instead.
                while(dependents.Count > 0)
                {
                    int endDependentIndex = dependents.Count - 1;
                    removeDependent(dependents[endDependentIndex], ref _size);
                }
            }

            /// <summary>
            /// Removes the dependencies between this node and all the dependees.
            /// </summary>
            /// <param name="_size"></param>
            internal void removeAllDependees(ref int _size)
            {
                //Can't use foreach loop, so iterate through the lsit of dependees with a while loop instead.
                while (dependees.Count > 0)
                {
                    int endDependeeIndex = dependees.Count - 1;
                    //Remove each of the dependees' reference to this node. Calling the removeDependent method through the dependees ensures a 
                    //bidirectional deletion of the references.
                    dependees[endDependeeIndex].removeDependent(this, ref _size); 
                }
            }
        }

        private Dictionary<String, DependencyNode> graph;

        private int _size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            this.graph = new Dictionary<String, DependencyNode>();
            this._size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                DependencyNode node;
                graph.TryGetValue(s, out node);
                //If the node exists on the graph, return the dependees count
                if (node != null)
                {
                    return node.dependees.Count;
                }

                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            DependencyNode node;

            graph.TryGetValue(s, out node);
            
            //If the node exists on the graph, and the count is greater than 0
            if(node != null && node.dependents.Count > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            DependencyNode node;

            graph.TryGetValue(s, out node);

            //If the node exists on the graph, and the count of dependees is greater than 0
            if (node != null && node.dependees.Count > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            DependencyNode node;

            graph.TryGetValue(s, out node);

            //If the node exists, call the node's own method for returning its dependents.
            if(node != null)
            {
                return node.listDependents();
            }

            return new List<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            DependencyNode node;

            graph.TryGetValue(s, out node);

            //If the node exists on the graph, call the node's own method for returning the dependees
            if (node != null)
            {
                return node.listDependees();
            }

            return new List<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            DependencyNode dependee = retrieveNode(s);
            DependencyNode dependent = retrieveNode(t);

            dependee.addDependent(dependent, ref _size);
        }



        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            DependencyNode dependee = retrieveNode(s);
            DependencyNode dependent = retrieveNode(t);

            dependee.removeDependent(dependent, ref _size);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            DependencyNode dependee = retrieveNode(s);

            dependee.removeAllDependents(ref _size);

            foreach (string dependent in newDependents)
            {
                //Calls the overloaded method of AddDependency that takes a reference to the dependee node.
                //This way, the same node (s) doesn't have to constantly be retrieved.
                AddDependency(dependee, dependent);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            DependencyNode dependent = retrieveNode(s);

            dependent.removeAllDependees(ref _size);

            foreach (string dependee in newDependees)
            {
                //Calls the overloaded method of AddDependency that takes a reference to the dependent node.
                //This way, the same node (s) doesn't have to constantly be retrieved.
                AddDependency(dependee, dependent);
            }
        }

        /// <summary>
        /// Retrieves a node that is associated with the string. If it doesn't exist on the graph, a new node is created and placed on the graph.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private DependencyNode retrieveNode(string variable)
        {
            DependencyNode node;

            graph.TryGetValue(variable, out node);

            //If the node does not exist
            if (node == null)
            {
                node = new DependencyNode(variable);
                //Associate the node with it's string as the key
                graph.Add(variable, node);
            }

            return node;
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s">Dependee node</param>
        /// <param name="t">String associated with dependent node</param>
        private void AddDependency(DependencyNode s, string t)
        {
            //TODO: Label this overload as being slightly better, since you don't need to lookup the node for the recurring dependee node in replaceDependents
            DependencyNode dependent = retrieveNode(t);

            s.addDependent(dependent, ref _size);
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s">String associated with dependee node</param>
        /// <param name="t">Dependent node</param>
        private void AddDependency(string s, DependencyNode t)
        {
            //TODO: Label this overload as being slightly better, since you don't need to lookup the node for the recurring dependent node in replaceDependees
            DependencyNode dependee = retrieveNode(s);

            dependee.addDependent(t, ref _size);
        }

    }


}