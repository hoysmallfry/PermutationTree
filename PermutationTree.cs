using System;
using System.Collections.Generic;

public class PermutationTree<KeyType, DataType>
{
    #region Accessors
    public List<DataType> data
    {
        get
        {
            return mData;
        }
    }

    public HashSet<KeyType> childKeys
    {
        get
        {
            return mChildKeys;
        }
    }
    public SortedDictionary<KeyType, PermutationTree<KeyType, DataType>> children
    {
        get
        {
            return mChildren;
        }
    }
    #endregion

    #region Function Members
    public void Add(KeyType[] pKeys, DataType pData)
    {
        var length = pKeys.Length;

        // no keys are present, return from function.
        if (length == 0)
        {
            return;
        }

        // place all keys in a unique set of child keys.
        for (int i = 0; i < length; ++i)
        {
            mChildKeys.Add(pKeys[i]);
        }

        var key = pKeys[0];

        // retrieve child if there is one, create a child if there is none.
        PermutationTree<KeyType, DataType> child = null;
        if (!mChildren.TryGetValue(key, out child))
        {
            child = new PermutationTree<KeyType, DataType>();
            mChildren.Add(key, child);
        }

        // base case: create a leaf node.
        if (length == 1)
        {
            // create if not already created.
            if (child.mData == null)
            {
                child.mData = new List<DataType>();
            }

            child.mData.Add(pData);
            return;
        }

        //otherwise, create internal node:

        // shrink the array, since the key has been entered.
        length--;
        Array.ConstrainedCopy(pKeys, 1, pKeys, 0, length);
        Array.Resize(ref pKeys, length);

        child.Add(pKeys, pData);
    }

    private bool Contains(KeyType[] pKeys)
    {
        var result = true;
        for (int j = 0; j < pKeys.Length; ++j)
        {
            result &= mChildKeys.Contains(pKeys[j]);
        }

        return result;
    }
	
	public List<DataType> GetData(KeyType[] pKeys)
    {
        return GetData(pKeys, pKeys);
    }
	
    private List<DataType> GetData(KeyType[] pKeys, KeyType[] pFullSequence)
    {
        var length = pKeys.Length;

        if (length == 0)
            return null;

        foreach (var pair in mChildren)
        {
            var child = pair.Value;

            // the key is directly under the node.
            if (pair.Key.Equals(pKeys[0]))
            {
                if (length == 1)
                {
                    return child.data;
                }

                length--;
                Array.ConstrainedCopy(pKeys, 1, pKeys, 0, length);
                Array.Resize(ref pKeys, length);

                return child.GetData(pKeys, pFullSequence);
            }
            // the keys are underneath the children.
            else if (child.Contains(pFullSequence))
            {
                return child.GetData(pFullSequence, pFullSequence);
            }
        }

        return null;
    }

    public List<DataType> GetDataInChildren(KeyType[] pKeys)
    {
        return GetDataInChildren(pKeys, pKeys);
    }
	
	private List<DataType> GetDataInChildren(KeyType[] pKeys, KeyType[] pFullSequence)
    {
        var length = pKeys.Length;

        if (length == 0)
            return null;

        var result = new List<DataType>();
        foreach (var pair in mChildren)
        {
            var child = pair.Value;

            // the key is directly under the node.
            if (pair.Key.Equals(pKeys[0]))
            {
                // base case.
                if (length == 1)
                {
                    result.AddRange(child.GetDataInChildren());
                }
                // we need to go deeper.
                else
                {
                    length--;
                    KeyType[] keys = new KeyType[length];
                    Array.ConstrainedCopy(pKeys, 1, keys, 0, length);

                    result.AddRange(child.GetDataInChildren(keys, pFullSequence));
                }
               
            }
            // the keys are underneath the children.
            else if (child.Contains(pFullSequence))
            {
                result.AddRange(child.GetDataInChildren(pFullSequence, pFullSequence));
            }
        }

        return result;
    }
	
	public List<DataType> GetDataInChildren()
    {
        var result = new List<DataType>();

        if (mData != null)
        {
            result.AddRange(mData);
        }

        foreach (var pair in mChildren)
        {
            result.AddRange(pair.Value.GetDataInChildren());
        }

        return result;
    }
    #endregion

    #region Data Members
    private List<DataType> mData = null;
    private HashSet<KeyType> mChildKeys = new HashSet<KeyType>();
    private SortedDictionary<KeyType, PermutationTree<KeyType, DataType>> mChildren = new SortedDictionary<KeyType, PermutationTree<KeyType, DataType>>();
    #endregion
}

public class StringPermutationTree<DataType> : PermutationTree<char, DataType>
{
    #region Function Members
    public void Add(string pKeys, DataType pData, bool pCaseSensitive = false)
    {
		if(!pCaseSensitive)
		{
			pKeys = pKeys.ToLower();
		}
		
        Add(pKeys.ToCharArray(), pData);
    }
    public List<DataType> GetData(string pKeys, bool pCaseSensitive = false)
    {
		if(!pCaseSensitive)
		{
			pKeys = pKeys.ToLower();
		}
		
        return GetData(pKeys.ToCharArray());
    }
    public List<DataType> GetDataInChildren(string pKeys, bool pCaseSensitive = false)
    {
		if(!pCaseSensitive)
		{
			pKeys = pKeys.ToLower();
		}
		
        return GetDataInChildren(pKeys.ToCharArray());
    }
    #endregion
}
