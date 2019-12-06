using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using PurchaseOrderSys.AWSECommerceService;

namespace PurchaseOrderSys.AmazonWebServices
{
    public class AWS
    {
        readonly AWSECommerceServicePortTypeClient AmazonClient;

        // An alphanumeric token that uniquely identifies you as an Associate. To obtain an Associate Tag,
        readonly string AssociateTag = "";

        // Your Access Key ID which uniquely identifies you.
        readonly string AWSAccessKeyId = "";

        // A key that is used in conjunction with the Access Key ID to cryptographically sign an API request. To retrieve your Access Key ID or Secret Access Key,
        readonly string AWSSecretKey = "";

        BasicHttpBinding Binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

        public AWS(string marketplace)
        {
            Binding.MaxReceivedMessageSize = int.MaxValue;
            Binding.MaxBufferSize = int.MaxValue;
            
            string uri = string.Format("https://webservices.amazon.{0}/onca/soap?Service=AWSECommerceService", marketplace);
            AmazonClient = new AWSECommerceServicePortTypeClient();
        }

        public void ItemSearch(string keywords)
        {
            ItemSearch itemSearch = new ItemSearch()
            {
                Request = new ItemSearchRequest[]
                {
                    new ItemSearchRequest()
                    {
                        SearchIndex = "All",
                        Keywords = keywords
                    }
                },
                AWSAccessKeyId = AWSAccessKeyId,
                AssociateTag = AssociateTag,
            };

            var response = AmazonClient.ItemSearch(itemSearch);
        }
    }
}