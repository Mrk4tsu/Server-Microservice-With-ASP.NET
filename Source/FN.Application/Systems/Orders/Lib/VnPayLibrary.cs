using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using DnsClient;
using Org.BouncyCastle.Asn1.X9;

namespace FN.Application.Systems.Orders.Lib
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        //public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
        //{
        //    var vnPay = new VnPayLibrary();

        //    foreach (var (key, value) in collection)
        //    {
        //        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
        //        {
        //            vnPay.AddResponseData(key, value);
        //        }
        //    }

        //    var orderId = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef"));
        //    var vnPayTranId = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
        //    var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
        //    var vnpSecureHash =
        //        collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
        //    var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");

        //    var checkSignature =
        //        vnPay.ValidateSignature(vnpSecureHash, hashSecret); //check Signature

        //    if (!checkSignature)
        //        return new PaymentResponseModel()
        //        {
        //            Success = false
        //        };

        //    return new PaymentResponseModel()
        //    {
        //        Success = vnpResponseCode.Equals("00"),
        //        PaymentMethod = "VnPay",
        //        OrderDescription = orderInfo,
        //        OrderId = orderId.ToString(),
        //        PaymentId = vnPayTranId.ToString(),
        //        TransactionId = vnPayTranId.ToString(),
        //        Token = vnpSecureHash,
        //        VnPayResponseCode = vnpResponseCode
        //    };
        //}
        public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
        {
            try
            {
                var vnPay = new VnPayLibrary();

                foreach (var (key, value) in collection)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnPay.AddResponseData(key, value!);
                    }
                }

                // Kiểm tra và xử lý các tham số bắt buộc
                var orderIdStr = vnPay.GetResponseData("vnp_TxnRef");
                var vnPayTranIdStr = vnPay.GetResponseData("vnp_TransactionNo");
                var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
                var vnpSecureHash = collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value;
                var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");

                // Validate các tham số bắt buộc
                if (string.IsNullOrEmpty(orderIdStr))
                    throw new ArgumentException("Thiếu tham số vnp_TxnRef");
                if (string.IsNullOrEmpty(vnPayTranIdStr))
                    throw new ArgumentException("Thiếu tham số vnp_TransactionNo");
                if (string.IsNullOrEmpty(vnpResponseCode))
                    throw new ArgumentException("Thiếu tham số vnp_ResponseCode");
                if (string.IsNullOrEmpty(vnpSecureHash))
                    throw new ArgumentException("Thiếu tham số vnp_SecureHash");

                // Chuyển đổi an toàn
                if (!long.TryParse(orderIdStr, out var orderId))
                    throw new FormatException("vnp_TxnRef không đúng định dạng số");

                if (!long.TryParse(vnPayTranIdStr, out var vnPayTranId))
                    throw new FormatException("vnp_TransactionNo không đúng định dạng số");

                // Kiểm tra chữ ký
                if (!vnPay.ValidateSignature(vnpSecureHash, hashSecret))
                {
                    return new PaymentResponseModel()
                    {
                        Success = false,
                        PaymentMethod = "VnPay",
                        OrderDescription = orderInfo,
                        OrderId = orderId.ToString(),
                        PaymentId = vnPayTranId.ToString(),
                        TransactionId = vnPayTranId.ToString(),
                        Token = vnpSecureHash!,
                        VnPayResponseCode = vnpResponseCode
                    };
                }

                return new PaymentResponseModel()
                {
                    Success = vnpResponseCode.Equals("00"),
                    PaymentMethod = "VnPay",
                    OrderDescription = orderInfo,
                    OrderId = orderId.ToString(),
                    PaymentId = vnPayTranId.ToString(),
                    TransactionId = vnPayTranId.ToString(),
                    Token = vnpSecureHash!,
                    VnPayResponseCode = vnpResponseCode
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khi xử lý dữ liệu phản hồi từ VnPay", ex);
            }
        }
        public string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "127.0.0.1";
        }
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();

            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString();

            baseUrl += "?" + querystring;
            var signData = querystring;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnpSecureHash;

            return baseUrl;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseData();
            var myChecksum = HmacSha512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }

            foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
