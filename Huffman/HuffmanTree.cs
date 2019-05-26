using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    public class HuffmanTree
    {
        private BinaryTreeNode root = null;
        public readonly BinaryTreeNode newNode = null;
        private Dictionary<byte, BinaryTreeNode> set = new Dictionary<byte, BinaryTreeNode>();

        public HuffmanTree()
        {
            Add(0);
            newNode = root;
        }

        private static BinaryTreeNode GetNext(BinaryTreeNode node)
        {
            BinaryTreeNode ans = GetRight(node);
            if (ans != null) return ans;
            else
            {
                BinaryTreeNode tmp = node.parent;
                if (tmp == null) return null;

                ans = node.parent;

                while (true)
                {
                    tmp = GetLeft(ans);
                    if (tmp == null) return ans;
                    else ans = tmp;
                }
            }
        }

        private static BinaryTreeNode GetLeft(BinaryTreeNode node)
        {
            int counter = 0;
            bool downward = false;
            while (true)
            {
                if (downward)
                {
                    if (counter == 0 || node == null) break;
                    else
                    {
                        counter--;
                        node = node.right;
                    }
                }
                else
                {
                    if (node.parent != null)
                    {
                        if (node.parent.right == node)
                        {
                            node = node.parent.left;
                            downward = true;
                        }
                        else
                        {
                            node = node.parent;
                            counter++;
                        }
                    }
                    else
                    {
                        node = null;
                        break;
                    }
                }
            }
            return node;
        }

        private static BinaryTreeNode GetRight(BinaryTreeNode node)
        {
            int counter = 0;
            bool downward = false;
            while (true)
            {
                if (downward)
                {
                    if (counter == 0 || node == null) break;
                    else
                    {
                        counter--;
                        node = node.left;
                    }
                }
                else
                {
                    if (node.parent != null)
                    {
                        if (node.parent.left == node)
                        {
                            node = node.parent.right;
                            downward = true;
                        }
                        else
                        {
                            node = node.parent;
                            counter++;
                        }
                    }
                    else
                    {
                        node = null;
                        break;
                    }
                }
            }
            return node;
        }

        private void AddWeight(BinaryTreeNode node)
        {
            node.weight++;
            BinaryTreeNode i = node, tmp;
            while (true)
            {
                tmp = GetNext(i);
                if (tmp != null && tmp != node.parent)
                {
                    if (tmp.weight < node.weight)
                    {
                        i = tmp;
                    }
                    else
                    {
                        if (node != i) Swap(node, i);
                        break;
                    }
                }
                else
                {
                    if (node != i) Swap(node, i);
                    break;
                }
            }
            if (node.parent != null)
            {
                AddWeight(node.parent);
            }
        }

        private void Swap(BinaryTreeNode a, BinaryTreeNode b)
        {
            bool aleft = false, bleft = false;
            if (a.parent != null)
            {
                if (a.parent.left == a)
                {
                    aleft = true;
                }
            }

            if (b.parent != null)
            {
                if (b.parent.left == b)
                {
                    bleft = true;
                }
            }

            if (aleft)
            {
                a.parent.left = b;
            }
            else
            {
                a.parent.right = b;
            }

            if (bleft)
            {
                b.parent.left = a;
            }
            else
            {
                b.parent.right = a;
            }

            BinaryTreeNode tmp = a.parent;
            a.parent = b.parent;
            b.parent = tmp;
        }

        private void AddNewNode(byte character)
        {
            BinaryTreeNode childNodeToBeAdded = new BinaryTreeNode
            {
                character = character,
                left = null,
                right = null,
                weight = 1
            };

            BinaryTreeNode parentNodeToBeAdded = new BinaryTreeNode
            {
                character = character,
                left = newNode,
                right = childNodeToBeAdded,
                parent = newNode.parent,
                weight = 0
            };

            childNodeToBeAdded.parent = parentNodeToBeAdded;

            if (newNode.parent != null)
            {
                newNode.parent.left = parentNodeToBeAdded;
            }

            if (newNode == root)
            {
                root = parentNodeToBeAdded;
            }
            newNode.parent = parentNodeToBeAdded;

            set.Add(character, childNodeToBeAdded);
            AddWeight(parentNodeToBeAdded);
        }

        public void Add(byte character)
        {
            if (root == null)
            {
                root = new BinaryTreeNode
                {
                    character = character,
                    left = null,
                    right = null,
                    parent = null,
                    weight = 0
                };
            }
            else
            {
                if (!Exists(character))
                {
                    AddNewNode(character);
                }
                else
                {
                    AddWeight(set[character]);
                }
            }
        }

        public bool Exists(byte character)
        {
            return set.ContainsKey(character);
        }

        public int GetCode(BinaryTreeNode node, out int bitsCount)
        {
            int code = 0;
            bitsCount = 0;
            while (true)
            {
                if (node.parent != null)
                {
                    if (node.parent.left == node)
                    {
                        code <<= 1;
                    }
                    else
                    {
                        code <<= 1;
                        code |= 1;
                    }
                    bitsCount++;
                    node = node.parent;
                }
                else break;
            }
            return code;
        }

        public int GetCode(byte character, out int bitsCount)
        {
            BinaryTreeNode node = set[character];
            return GetCode(node, out bitsCount);
        }

        public byte GetCharacter(int code, out bool isNew, out uint size)
        {
            BinaryTreeNode node = root;
            size = 0;
            while (true)
            {
                if (node.left == null && node.right == null)
                {
                    isNew = node.weight == 0;
                    return node.character;
                }
                
                if((code & 1) == 0)
                {
                    node = node.left;
                }
                else
                {
                    node = node.right;
                }
                code >>= 1;
                size++;
            }
        }
    }
}
