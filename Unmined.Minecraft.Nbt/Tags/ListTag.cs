using System.Collections;

namespace Unmined.Minecraft.Nbt.Tags;

public class ListTag : Tag, IList<Tag>
{
    private readonly List<Tag> _items;

    public ListTag(TagType itemType)
    {
        ItemType = itemType;
        _items = new List<Tag>();
    }

    public ListTag(TagType itemType, IEnumerable<Tag> items)
    {
        ItemType = itemType;
        _items = new List<Tag>(items);

        foreach (var item in _items)
            CheckItemType(item.Type);
    }

    public override TagType Type => TagType.List;

    public TagType ItemType { get; }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public Tag this[int index]
    {
        get => _items[index];
        set
        {
            CheckItemType(value.Type);
            _items[index] = value;
        }
    }


    public override ListTag Clone()
    {
        return new ListTag(ItemType, this.Select(t => t.Clone()));
    }

    public void Add(Tag item)
    {
        CheckItemType(item.Type);
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(Tag item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(Tag[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public bool Remove(Tag item)
    {
        return _items.Remove(item);
    }

    public IEnumerator<Tag> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public int IndexOf(Tag item)
    {
        return _items.IndexOf(item);
    }

    public void Insert(int index, Tag item)
    {
        CheckItemType(item.Type);
        _items.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    private void CheckItemType(TagType itemType)
    {
        if (itemType != ItemType)
            throw new InvalidOperationException($"Cannot add item of type {itemType} to list with ItemType {ItemType}");
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}