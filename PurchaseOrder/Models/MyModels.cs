using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public static class EnumerableExtender
    {
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }
    }
    public class DataGridModels
    {
        /// <summary>
        /// DataGrid的名稱，可ID,class
        /// </summary>
        public string DataGridName { get; set; }
        /// <summary>
        /// 是否顯示checkbox
        /// </summary>
        public bool checkbox { get; set; }
        /// <summary>
        /// 表格名稱
        /// </summary>
        public string title { get; set; }
        public string idField { get; set; }
        /// <summary>
        /// 顯示新增子項目
        /// </summary>
        public bool additem { get; set; }
        /// <summary>
        /// 是否顯修改按鈕
        /// </summary>
        public bool edititem { get; set; }
        /// <summary>
        /// 是否顯示刪除按鈕
        /// </summary>
        public bool delitem { get; set; }
        /// <summary>
        /// 是否顯示儲存按鈕
        /// </summary>
        public bool saveitem { get; set; }
        /// <summary>
        /// 顯示子表
        /// </summary>
        public bool showchilds { get; set; }
        public List<DataGridItemsModels> DataGridItems { get; set; }

    }
    public class DataGridItemsModels
    {
        public string field { get; set; }
        public string title { get; set; }
        public bool frozen { get; set; }
        public float width { get; set; }
        public string align { get; set; }
        public string sortable { get; set; }
        public bool formatter { get; set; }
        public string columnsType { get; set; }
    }
    public class SelectItem
    {
        public string id { get; set; }
        public string text { get; set; }
    }
    public class Country
    {
        private RegionInfo info;

        public string ID { get { return info.TwoLetterISORegionName; } }
        public string Name { get { return info.EnglishName; } }
        public string ChtName { get { return info.DisplayName; } }
        public string TwoCode { get { return info.TwoLetterISORegionName; } }
        public string ThreeCode { get { return info.ThreeLetterISORegionName; } }

        public Country(int LCID)
        {
            info = new RegionInfo(LCID);
        }

        public Country(string Name)
        {
            info = new RegionInfo(Name);
        }

        public string OriginName
        {
            get
            {
                switch (ID)
                {
                    case "CN":
                        return "China";
                    case "TW":
                        return "Taiwan";
                    case "US":
                        return "USA";
                    default:
                        return Name;
                }
            }
        }
    }
}