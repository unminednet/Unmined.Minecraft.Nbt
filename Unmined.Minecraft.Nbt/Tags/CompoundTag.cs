using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Unmined.Minecraft.Nbt.Tags;

public class CompoundTag : Tag, IDictionary<string, Tag>
{
    private readonly Dictionary<string, Tag> _items;

    public CompoundTag()
    {
        _items = new Dictionary<string, Tag>();
    }

    public CompoundTag(IEnumerable<KeyValuePair<string, Tag>> tags)
    {
        _items = new Dictionary<string, Tag>(tags);
    }

    public override TagType Type => TagType.Compound;

    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public ICollection<string> Keys => _items.Keys;
    public ICollection<Tag> Values => _items.Values;

    public Tag this[string key]
    {
        get => _items[key];
        set => _items[key] = value;
    }

    public override CompoundTag Clone()
    {
        return new CompoundTag(this.Select(p => new KeyValuePair<string, Tag>(p.Key, p.Value.Clone())));
    }

    public void Clear()
    {
        _items.Clear();
    }

    public void Add(string key, Tag item)
    {
        _items.Add(key, item);
    }

    public bool ContainsKey(string key)
    {
        return _items.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return _items.Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Tag value)
    {
        return _items.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    void ICollection<KeyValuePair<string, Tag>>.Add(KeyValuePair<string, Tag> item)
    {
        ((ICollection<KeyValuePair<string, Tag>>)_items).Add(item);
    }

    bool ICollection<KeyValuePair<string, Tag>>.Contains(KeyValuePair<string, Tag> item)
    {
        return _items.Contains(item);
    }

    void ICollection<KeyValuePair<string, Tag>>.CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, Tag>>)_items).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<string, Tag>>.Remove(KeyValuePair<string, Tag> item)
    {
        return ((ICollection<KeyValuePair<string, Tag>>)_items).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}