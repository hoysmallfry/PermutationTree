/// Written By Joel Barba, Fall 2012.

using System;
using System.Collections.Generic;

public class PermutationTree<KeyType, DataType> where DataType : new()
{
    #region Fields
    public readonly SortedDictionary<KeyType, PermutationTree<KeyType, DataType>> children;
    #endregion

    #region Properties
    public KeyType[] Path { get; private set; }

    public DataType data { get; private set; }
    #endregion

    #region Methods
    public PermutationTree()
    {
        data = default(DataType);
        children = new SortedDictionary<KeyType, PermutationTree<KeyType, DataType>>();
    }

    public void Add(KeyType[] pKeys, DataType pData)
    {
        Add(0, pKeys, pData);
    }

    public void Add(int index, KeyType[] pKeys, DataType pData)
    {
        var length = pKeys.Length;

        // If there is no key path, then:
        if (index >= pKeys.Length)
        {
            // Do nothing else.
            return;
        }

        KeyType key = pKeys[index];

        PermutationTree<KeyType, DataType> child = null;

        // Attempt to retrieve the child if there is one in the direct children. If there is no direct child, then:
        if (!children.TryGetValue(key, out child))
        {
            // Create a child and add it to the direct children.
            child = new PermutationTree<KeyType, DataType>();
            children.Add(key, child);
        }

        // At this point, there will be a child associated with the first key.

        // If there are more than one item left in the keys, then:
        if (index != pKeys.Length - 1)
        {
            child.Add(index + 1, pKeys, pData);
            return;
        }

        // Otherwise, create an data node:

        // If this node is not already a data node, then:
        if (!EqualityComparer<DataType>.Default.Equals(child.data, default(DataType)))
        {
            // There is a data collision! throw error!
        }

        // Add the data to the node.
        child.Path = pKeys;
        child.data = pData;
    }

    private T GetDataInternal<T>(int index, KeyType[] pKeys, Func<PermutationTree<KeyType, DataType>, int, KeyType[], T> recursion, Func<PermutationTree<KeyType, DataType>, T> baseCase) where T : new()
    {
        // If there is no key path, then:
        if (index >= pKeys.Length)
        {
            // There is no list. Do nothing else.
            return default(T);
        }

        // Otherwise, there is a key list.

        KeyType key = pKeys[index];
        PermutationTree<KeyType, DataType> child = null;
        
        if (children.TryGetValue(key, out child))
        {
            // Base Case: If there is only one item left in the keys, then:
            if (index == pKeys.Length - 1)
            {
                return baseCase(child);
            }

            // We need to go deeper.

            // Search the child for the substring path.
            return recursion(child, index + 1, pKeys);
        }

        return default(T);
    }

    #region GetData
    public DataType GetData(KeyType[] pKeys)
    {
        return GetDataInternal(0, pKeys, GetDataRecursionCase, GetDataBaseCase);
    }

    private DataType GetDataBaseCase(PermutationTree<KeyType, DataType> node)
    {
        return node.data;
    }

    private DataType GetDataRecursionCase(PermutationTree<KeyType, DataType> node, int index, KeyType[] pKeys)
    {
        return node.GetDataInternal(index, pKeys, GetDataRecursionCase, GetDataBaseCase);
    }
    #endregion

    #region GetDataInChildren
    public List<DataType> GetDataInChildren()
    {
        var result = new List<DataType>();

        // If this is a leaf node, then:
        if (!EqualityComparer<DataType>.Default.Equals(data, default(DataType)))
        {
            // Add the data from the leaf node to the range.
            result.Add(data);
        }

        // Iterate through all children.
        foreach (var pair in children)
        {
            // Get the data within the entire subtree.
            result.AddRange(pair.Value.GetDataInChildren());
        }

        return result;
    }

    public List<DataType> GetDataInChildren(KeyType[] pKeys)
    {
        return GetDataInternal(0, pKeys, GetDataInChildrenRecursionCase, GetDataInChildrenBaseCase);
    }

    private List<DataType> GetDataInChildrenBaseCase(PermutationTree<KeyType, DataType> node)
    {
        return node.GetDataInChildren();
    }

    private List<DataType> GetDataInChildrenRecursionCase(PermutationTree<KeyType, DataType> node, int index, KeyType[] pKeys)
    {
        return node.GetDataInternal(index, pKeys, GetDataInChildrenRecursionCase, GetDataInChildrenBaseCase);
    }
    #endregion

    public List<KeyValuePair<KeyType[], DataType>> GetPairInChildren(KeyType[] pKeys)
    {
        return GetDataInternal(0, pKeys, GetPairInChildrenRecursionCase, GetPairInChildrenBaseCase);
    }

    private List<KeyValuePair<KeyType[], DataType>> GetPairInChildrenBaseCase(PermutationTree<KeyType, DataType> node)
    {
        var result = new List<KeyValuePair<KeyType[], DataType>>();

        // If this is a leaf node, then:
        if (!EqualityComparer<DataType>.Default.Equals(node.data, default(DataType)))
        {
            // Add the data from the leaf node to the range.
            result.Add(new KeyValuePair<KeyType[], DataType>(node.Path, node.data));
        }

        // Iterate through all children.
        foreach (var pair in node.children)
        {
            // Get the data within the entire subtree.
            result.AddRange(GetPairInChildrenBaseCase(pair.Value));
        }

        return result;
    }

    private List<KeyValuePair<KeyType[], DataType>> GetPairInChildrenRecursionCase(PermutationTree<KeyType, DataType> node, int index, KeyType[] pKeys)
    {
        return node.GetDataInternal(index, pKeys, GetPairInChildrenRecursionCase, GetPairInChildrenBaseCase);
    }
    #endregion
}

public class StringPermutationTree<DataType> : PermutationTree<char, DataType> where DataType : new()
{
    #region Methods
    public void Add(string pKeys, DataType pData, bool pCaseSensitive = false)
    {
        if (!pCaseSensitive)
        {
            pKeys = pKeys.ToLower();
        }

        Add(pKeys.ToCharArray(), pData);
    }

    public DataType GetData(string pKeys, bool pCaseSensitive = false)
    {
        if (!pCaseSensitive)
        {
            pKeys = pKeys.ToLower();
        }

        return GetData(pKeys.ToCharArray());
    }

    public List<DataType> GetDataInChildren(string pKeys, bool pCaseSensitive = false)
    {
        if (!pCaseSensitive)
        {
            pKeys = pKeys.ToLower();
        }

        return GetDataInChildren(pKeys.ToCharArray());
    }

    public List<KeyValuePair<char[], DataType>> GetPairInChildren(string pKeys, bool pCaseSensitive = false)
    {
        if (!pCaseSensitive)
        {
            pKeys = pKeys.ToLower();
        }

        return GetPairInChildren(pKeys.ToCharArray());
    }
    #endregion
}
