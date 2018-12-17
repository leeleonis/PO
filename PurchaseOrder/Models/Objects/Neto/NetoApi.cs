using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetoDeveloper;

namespace inventorySKU.NetoDeveloper
{
    public class NetoApi : NetoBase
    {
        public NetoApi()
        {

        }
        
        public GetItemResponse GetItemBySku(string sku)
        {
            GetItem request = new GetItem() {
                Filter = new GetItemFilter()
                {
                    SKU = new string[] { sku },
                    OutputSelector = Enum.GetValues(typeof(GetItemFilterOutputSelector)).Cast<GetItemFilterOutputSelector>().ToArray()
                }
            };

            return Request<GetItemResponse>("GetItem", request);
        }

        public GetItemResponse GetItemBySkus(int page = 0, int limit = 100)
        {
            GetItem request = new GetItem()
            {
                Filter = new GetItemFilter()
                {
                    DateAddedTo = DateTime.UtcNow.AddDays(1),
                    OutputSelector = Enum.GetValues(typeof(GetItemFilterOutputSelector)).Cast<GetItemFilterOutputSelector>().ToArray(),
                    Page = page,
                    Limit = limit,
                    OrderBy = GetItemFilterOrderBy.DateAdded,
                    OrderBySpecified = true,
                    OrderDirection = GetItemFilterOrderDirection.DESC,
                    OrderDirectionSpecified = true
                }
            };

            return Request<GetItemResponse>("GetItem", request);
        }

        public AddItemResponse UpdateItem(AddItemItem item)
        {
            AddItem request = new AddItem()
            {
                Item = new AddItemItem[] { item }
            };

            return Request<AddItemResponse>("AddItem", request);
        }

        public UpdateItemResponse UpdateItem(UpdateItemItem item)
        {
            UpdateItem request = new UpdateItem()
            {
                Item = new UpdateItemItem[] { item }
            };

            return Request<UpdateItemResponse>("UpdateItem", request);
        }

        public GetWarehouseResponse GetWarehouse()
        {
            GetWarehouse request = new GetWarehouse()
            {
                Filter = new GetWarehouseFilter()
                {
                    OutputSelector = Enum.GetValues(typeof(GetWarehouseFilterOutputSelector)).Cast<GetWarehouseFilterOutputSelector>().ToArray()
                }
            };

            return Request<GetWarehouseResponse>("GetWarehouse", request);
        }

        public GetCategoryResponse GetCategory()
        {
            GetCategory request = new GetCategory()
            {
                Filter = new GetCategoryFilter()
                {
                    OutputSelector = Enum.GetValues(typeof(GetCategoryFilterOutputSelector)).Cast<GetCategoryFilterOutputSelector>().ToArray()
                }
            };

            return Request<GetCategoryResponse>("GetCategory", request);
        }
    }
}