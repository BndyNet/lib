// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net.
// Created by Bndy at 3/15/2013 14:35:20
// --------------------------------------------------------------------------
// Tree Struct.
// ==========================================================================

using System.Linq;
using System.Collections.Generic;

namespace Net.Bndy.Entities
{
	public class Tree
	{
		public Dictionary<object, TreeNode> Items { get; private set; }

		/// <summary>
		/// Gets the node object by nodeID.
		/// </summary>
		/// <param name="nodeID">The identity of the node.</param>
		/// <returns></returns>
		public TreeNode this[object nodeID]
		{
			get
			{
				if (this.Items != null)
				{
					return this.Items[nodeID];
				}

				return null;
			}
		}

		/// <summary>
		/// All nodes.
		/// </summary>
		public TreeNode[] Nodes
		{
			get
			{
				return this.Items.Values.ToArray();
			}
		}

		/// <summary>
		/// The root nodes.
		/// </summary>
		public List<TreeNode> Root
		{
			get
			{
				List<TreeNode> lst = new List<TreeNode>();

				lst = this.Nodes.Where(node => node.Parent == null).ToList();

				return lst;
			}
		}

		/// <summary>
		/// Initializes an instance of Tree class.
		/// </summary>
		/// <param name="nodes">The nodes of tree.</param>
		public Tree(params TreeNode[] nodes)
		{
			if (nodes.Count() > 0)
			{
				this.Items = nodes.ToDictionary<TreeNode, object>(node => node.ID);
			}
		}

		/// <summary>
		/// Adds sub nodes.
		/// </summary>
		/// <param name="nodes">The set of TreeNode instance.</param>
		public void AddNodes(params TreeNode[] nodes)
		{
			foreach (TreeNode tn in nodes)
			{
				if (tn != null)
				{
					this.Items[tn.ID] = tn;
				}
			}
		}
	}

	/// <summary>
	/// Node of Tree
	/// </summary>
	public class TreeNode
	{
		public object ID { get; set; }
		public TreeNode Parent { get; private set; }
		public List<TreeNode> Children { get; set; }
		public Dictionary<string, object> ExtraData { get; set; }
		public TreeNode Prev { get; private set; }
		public TreeNode Next { get; private set; }

		/// <summary>
		/// Initializes an instance of TreeNode class.
		/// </summary>
		public TreeNode()
		{
			this.Children = new List<TreeNode>();
			this.ExtraData = new Dictionary<string, object>();
		}

		/// <summary>
		/// Adds a child for current instance.
		/// </summary>
		/// <param name="node">The new instance of TreeNode class.</param>
		public void AppendChild(TreeNode node)
		{
			if (node != null)
			{
				if (this.Children.Count() > 0)
				{
					TreeNode last = this.Children.Last();
					last.Next = node;
					node.Prev = last;
				}

				node.Parent = this;
				this.Children.Add(node);
			}
		}

		/// <summary>
		/// Inserts a node before current instance.
		/// </summary>
		/// <param name="node">The new instance of TreeNode class.</param>
		public void InsertBefore(TreeNode node)
		{
			if (node != null)
			{
				// Rewrite pointers of node.
				if (this.Prev != null)
				{
					this.Prev.Next = node;
					node.Prev = this.Prev;
				}
				else
				{
					this.Prev = node;
					node.Prev = null;
				}
				node.Next = this;

				if (node.Parent != null)
					node.Parent.Children.Add(node);
			}
		}

		/// <summary>
		/// Inserts a node after current instance. 
		/// </summary>
		/// <param name="node">The new instance of TreeNode class.</param>
		public void InsertAfter(TreeNode node)
		{
			if (node != null)
			{
				// Rewrite pointers of node.
				if (this.Next != null)
				{
					this.Next.Prev = node;
					node.Next = this.Next;
				}
				else
				{
					this.Next = node;
					node.Next = null;
				}
				node.Prev = this;

				if (node.Parent != null)
					node.Parent.Children.Add(node);
			}
		}
	}
}
