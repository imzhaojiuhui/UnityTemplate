using System;
using System.Collections.Generic;
using System.Linq;

public class RedDotTrie
{
    private class TrieNode
    {
        public Dictionary<string, TrieNode> Children { get; private set; }
    
        private int _count = 0;
        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                    onCountChanged?.Invoke(value);
                _count = value;
            }
        } // 当前节点的红点数量
    
        public event Action<int> onCountChanged;
        public TrieNode()
        {
            Children = new Dictionary<string, TrieNode>();
        }
    }
    
    private readonly TrieNode _root;

    public RedDotTrie()
    {
        _root = new TrieNode();
    }

    private TrieNode Ensure(string[] path)
    {
        TrieNode current = _root;

        foreach (string key in path)
        {
            if (!current.Children.ContainsKey(key))
            {
                current.Children[key] = new TrieNode();
            }
            current = current.Children[key];
        }
        return current;
    }

    public void SubscribeWithInit(Action<int> onCountChanged, params string[] path)
    {
        TrieNode node = Ensure(path);
        node.onCountChanged += onCountChanged;
        onCountChanged.Invoke(node.Count);
    }

    public void Unsubscribe(Action<int> onCountChanged, params string[] path)
    {
        // StringBuilder sb = new StringBuilder();
        // foreach (var str in path){
        //     sb.Append(str);
        // }
        // Debug.Log(sb.ToString());
        GetNode(path).onCountChanged -= onCountChanged;
    }

    private void OnSetRedDotCount(IEnumerable<string> path, int count)
    {
       // do something
    }

    // 设置红点数量
    public void SetRedDotCount(int count, params string[] path)
    {
        TrieNode node = Ensure(path);
        if (node.Children.Count > 0)
        {
            throw new InvalidOperationException($"{path} is not leaf node");
        }

        node.Count = count;
        OnSetRedDotCount(path, count);
        UpdateParentRedDotCount(path); // 更新父节点的红点数量
    }

    // 增加红点数量
    public void AddRedDotCount(int increment, params string[] path)
    {
        TrieNode node = Ensure(path);
        if (node.Children.Count > 0)
        {
            throw new InvalidOperationException($"{path} is not leaf node");
        }
        
        node.Count += increment;
        OnSetRedDotCount(path, node.Count);
        UpdateParentRedDotCount(path); // 更新父节点的红点数量
    }

    // 获取红点数量
    public int GetRedDotCount(params string[] path)
    {
        TrieNode node = GetNode(path);
        return node?.Count ?? 0;
    }

    public bool HasRedDotCount(params string[] path)
    {
        return GetRedDotCount(path) > 0;
    }

    // 辅助方法：获取某个路径对应的节点
    private TrieNode GetNode(IEnumerable<string> path)
    {
        TrieNode current = _root;

        foreach (var key in path)
        {
            if (!current.Children.ContainsKey(key))
            {
                return null;
            }
            current = current.Children[key];
        }

        return current;
    }

    // 更新父节点的红点数量
    private void UpdateParentRedDotCount(string[] path)
    {
        // 遍历路径，更新每个父节点的红点数量
        for (int i = 1; i <= path.Length; i++)
        {
            // IEnumerable<string> parentPath = path.Take(path.Length - i);
            TrieNode current = GetNode(path.Take(path.Length - i));
            // 计算当前节点的红点数量 = 所有子节点红点数量的总和
            int newCount = 0;
            foreach (var child in current.Children.Values)
            {
                newCount += child.Count;
            }

            current.Count = newCount;
            OnSetRedDotCount(path.Take(path.Length - i), newCount);
        }
    }

    public override string ToString()
    {
        return PrintTreeWithHighlight(_root);
    }

    private static string PrintTreeWithHighlight(TrieNode root, string indent = "")
    {
        if (root == null) return "NULL";
    
        // 非零红点用颜色标记（在Unity控制台中显示为黄色）
        string countStr = root.Count > 0 ? $"<color=yellow>{root.Count}</color>" : root.Count.ToString();
        string result = $"{indent}[Count: {countStr}]\n";
    
        foreach (var kvp in root.Children)
        {
            string childIndent = indent + "  ";
            result += $"{childIndent}{kvp.Key}:\n";
            result += PrintTreeWithHighlight(kvp.Value, childIndent + "  ");
        }
    
        return result;
    }
}
