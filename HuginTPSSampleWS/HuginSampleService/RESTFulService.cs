using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel.Activation;

namespace HuginWS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class TPSService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "sale?okc_id={okc_id}&password={password}", Method = "POST",
            RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        string sale(string okc_id, string password, Stream strPost)
        {
            string result = string.Empty;
            try
            {
                /* okc_id and password should be done with authority control. */

                string salesinfo = new StreamReader(strPost).ReadToEnd();
                SalesInfo salesInfo = JsonConvert.DeserializeObject<SalesInfo>(salesinfo);
                string filePath = String.Format("{0}{1}_{2}_{3}.json", ECRDataFolder, okc_id, salesInfo.ZNo, salesInfo.DocumentNo);
                File.WriteAllText(filePath, salesinfo);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
                result = ex.Message;
            }
            return result;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "discount?okc_id={okc_id}&password={password}", Method = "POST",
            RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        string discount(string okc_id, string password, Stream strPost)
        {
            string result = string.Empty;
            try
            {
                /* okc_id and password should be done with authority control. */

                string salesinfo = new StreamReader(strPost).ReadToEnd();
                SalesInfo salesInfo = JsonConvert.DeserializeObject<SalesInfo>(salesinfo);
                string filePath = String.Format("{0}{1}_{2}_{3}_promo.json", ECRDataFolder, okc_id, salesInfo.ZNo, salesInfo.DocumentNo);
                File.WriteAllText(filePath, salesinfo);

                /* 
                 * If promotion applied DiscountCode has to value.
                 * If DiscountCode empty or null sended promotion is not apply.
                 */

                salesInfo.DiscountCode = "CXKU123Z";

                /* 
                 * Discount promotional information sent by the info object.
                 * If DiscountTotal sending DiscountRate have to 0.
                 * If DiscountRate sending DiscountTotal have to 0.
                 * If there isn't DiscountNotes send to empty.
                 */
                DiscountInfo discountInfo = new DiscountInfo();
                discountInfo.DiscountTotal = 1;
                discountInfo.DiscountRate = 0;
                discountInfo.DiscountNotes = new List<string>() { "3 AL 2 ODE" };

                /*
                 * If the product is desired to apply the discount salesınfo-> saleıtems [x] object should be set of the area DiscountInfo.
                 * If the product is sent to discounts should be sent subtotal discount.
                 */
                salesInfo.DiscountInfo = discountInfo;

                /*
                 * If the product is desired to apply the discount salesınfo-> saleıtems [x] object should be set of the area DiscountInfo.
                 * If the product is sent to discounts should be sent subtotal discount.
                 */

                //salesInfo.SaleItems[0].DiscountInfo = discountInfo;

                result = JsonConvert.SerializeObject(salesInfo).Replace("\"", "'");
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
                result = ex.Message;
            }
            return result;
        }

        [OperationContract]
        [WebGet(UriTemplate = "order?okc_id={okc_id}&password={password}&order_id={order_id}",
            ResponseFormat = WebMessageFormat.Json)]
        string order(string okc_id, string password, string order_id)
        {
            string result = string.Empty;
            try
            {
                /* okc_id and password should be done with authority control. */

                //Sample order content
                string orderData = File.ReadAllText(TestDataFolder + "order.json");

                //Order content includes salesinfo object.
                SalesInfo salesInfo = JsonConvert.DeserializeObject<SalesInfo>(orderData);

                result = JsonConvert.SerializeObject(salesInfo).Replace("\"", "'");

            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
                result = ex.Message;
            }
            return result.Replace("\"", "'");
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "order?okc_id={okc_id}&password={password}&order_id={order_id}&error_code={error_code}&error_desc={error_desc}",
            Method = "PUT",
            ResponseFormat = WebMessageFormat.Json)]
        string orderack(string okc_id, string password, string order_id, string error_code, string error_desc)
        {
            string result = string.Empty;
            try
            {
                /* okc_id and password should be done with authority control. */

                result = String.Format("orderid : {0}\r\n error_code : {1}\r\n error_desc : {2}", order_id, error_code, error_desc);

                //orderACK message added to file
                File.WriteAllText("orderACK.txt", result);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
                result = ex.Message;
            }
            return result;
        }

        [OperationContract]
        [WebGet(UriTemplate = "products?okc_id={okc_id}&password={password}&last_update_date={last_update_date}",
            ResponseFormat = WebMessageFormat.Json)]
        string products(string okc_id, string password, string last_update_date)
        {
            string result = string.Empty;
            try
            {
                /* okc_id and password should be done with authority control. */

                //Sample product list
                result = File.ReadAllText(TestDataFolder + "products.txt");
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
                result = ex.Message;
            }
            return result;
        }

        [OperationContract]
        [WebGet(UriTemplate = "stock?okc_id={okc_id}&password={password}&barcode={barcode}&serial={serial}&pluno={pluno}",
            ResponseFormat = WebMessageFormat.Json)]
        string stock(string okc_id, string password, string barcode, string serial, string pluno)
        {
            string result = string.Empty;
            try
            {
                /* okc_id and password should be done with authority control. */

                // do stock or serial control and return 200-SUCCESS.
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
                result = ex.Message;
            }
            return result;
        }

        #region StaticVariables
        public static string ECRDataFolder = AppDomain.CurrentDomain.BaseDirectory + "data\\";
        public static string TestDataFolder = AppDomain.CurrentDomain.BaseDirectory + "testdata\\";
        private static String strVersion = "$Revision: 8780 $";
        #endregion
    }
}