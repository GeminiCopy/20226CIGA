using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private Dictionary<int, ItemDesc> _items;
    public override void Init()
    {
        base.Init();
        var textasset = ResourcesMgr.Instance.Load<TextAsset>("Dialog/tbitem");
        _items = textasset.text.FromJson<List<ItemDesc>>().ToDictionary(x => x.Id);
    }
    public ItemDesc Get(int id) => _items.GetValueOrDefault(id);
}