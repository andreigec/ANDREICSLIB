using System.Collections.Generic;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	class TreeViewUpdates : TreeView
	{

		public IEnumerable<TreeNode> allNodes()
		{
		foreach (TreeNode TN in Nodes)
		{
			if (TN.Nodes.Count == 0)
				yield return TN;
			else
				allNodes(TN);
		}
		}

		private IEnumerable<TreeNode> allNodes(TreeNode TN)
		{
			foreach (TreeNode TN2 in TN.Nodes)
			{
				if (TN2.Nodes.Count == 0)
					yield return TN2;
				else
					allNodes(TN2);
			}
		}



		public void CopyNodes(TreeView from, bool clearFirst)
		{
			if (clearFirst)
				Nodes.Clear();

			foreach (TreeNode node in from.Nodes)
			{
				if (Nodes[node.Text] != null)
				{
					CopyNodes(node, Nodes[node.Text], false);
				}
				else
					Nodes.Add((TreeNode)node.Clone());
			}
		}

		public static void CopyNodes(TreeNode from, TreeNode to, bool clearFirst)
		{
			if (clearFirst)
				to.Nodes.Clear();

			foreach (TreeNode node in from.Nodes)
			{
				if (to.Nodes[node.Text] != null)
				{
					CopyNodes(node, to.Nodes[node.Text], false);
				}
				else
					to.Nodes.Add((TreeNode)node.Clone());
			}
		}
	}
}
