using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Security.Policy;
using static DemoApi.ViewModel.MainCode;
using Microsoft.Win32.SafeHandles;
using Microsoft.AspNetCore.Components.Forms;

namespace DemoApi.ViewModel
{
    public class MainCode
    {
        public static string accessToken = string.Empty;
        public static string userID = string.Empty;
       
        public async Task<DgftAuthResponse>AuthTokenApi(DgftAuthRequest dgftAuthRequest)
        {
            DgftAuthResponse dgftAuthResponse =new DgftAuthResponse();
            try
            {
                byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(dgftAuthRequest.password);
                dgftAuthRequest.password = Convert.ToBase64String(bytesToEncode);
                string DgftTokenUrl = Program.DgftBaseUrl + "/auth/generateAuthToken";
                
                string TokenResponse = CallAuthTokenService(DgftTokenUrl, dgftAuthRequest);
                dgftAuthResponse = JsonConvert.DeserializeObject<DgftAuthResponse>(TokenResponse);
                //Set Temp Data
                dgftAuthResponse.AccessToken = "eyJraWQiOiJUc1JLWk5ickFYTGRVZHdWRWYrOTlkWndjM0pxYm14QVM2cHljTlRycDAwPSIsImFs\r\nZyI6IlJTMjU2In0.eyJzdWIiOiIyZDM1ZmRmZC05OWU5LTRiZDctYmRmOC0zNmFkYmQ2YjJjYjkiLCJ\r\npc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtc291dGgtMS5hbWF6b25hd3MuY29tXC9hcC1zb\r\n3V0aC0xX0pyajNleVRUOSIsImNsaWVudF9pZCI6IjduYWJwYjk3bjMxNTI1dmttc2N1OGZmZnM3Iiwib\r\n3JpZ2luX2p0aSI6ImM3NzBmYzAzLWIwNWYtNDU4Ni04Y2JkLTc2NzNiNzIzMzVhZiIsImV2ZW50X2lk\r\nIjoiN2U2M2E1NDctN2NjNS00YjY4LWI4NDctYTU0MDBjMTM4YWNlIiwidG9rZW5fdXNlIjoiYWNjZXNz\r\nIiwic2NvcGUiOiJhd3MuY29nbml0by5zaWduaW4udXNlci5hZG1pbiIsImF1dGhfdGltZSI6MTY4NTE5N\r\nDU2NywiZXhwIjoxNjg1MTk4MTY3LCJpYXQiOjE2ODUxOTQ1NjcsImp0aSI6IjE3YTQ3MDAwLTY1ND\r\nktNDgyOC1hZjY0LTVkODhiOTE0NzdiZiIsInVzZXJuYW1lIjoidGVzdHVzZXIzIn0.fND_bxHwGsFWD5u\r\nYpNCqkYeEhIWKCacU25DX79tfQBebdYoIRPHQ9ILi8X6fl5RihlJDOeUT3auz4Edk7ddM_d6UIcWbP\r\nPO430SsHpz-tkIT3N0Q0uaK61mtqFMrnqbJzalubBZuh4emhQa8e3sjRbjEwg711yy7XmM5LSUAtqv0J_UaFASH0HYESUVbYGCenJwbEwmGexRdhREjmbZsToLaMPEkqnTh9n-pEkvO0R47rwkNqqfWbp_2deRMJVFQ1d8-m1yj1o8-\r\noJeylNeZ5NAlgeslrQFNRf6FOweECfU__BMDAJMnSPGZhvP2-f7lQGdVePQazPBQLftPvDsg";
                dgftAuthResponse.ExpiresIn = 3600;
                dgftAuthResponse.userID = "12323232";
                accessToken = dgftAuthResponse.AccessToken;
                userID = dgftAuthResponse.userID;

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return dgftAuthResponse;
        }
        

        public string CallAuthTokenService(string url,DgftAuthRequest dgftAuthRequest)
        {

            string responseData = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("x-api-key", Program.Api_Key_data);
                var ds = JsonConvert.SerializeObject(dgftAuthRequest);
                byte[] dataBytes = Encoding.UTF8.GetBytes(ds);
                request.ContentLength = dataBytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseData = streamReader.ReadToEnd();

                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return responseData;
        }


        public async Task<string> IRMToDGFT(JsonRequestData jsonRequest)
        {
            string SimpleResponse = string.Empty;

            JsonEncryptedData jsonEncryptedData =new JsonEncryptedData();
            JsonDecryptedData jsonDecryptedData = new JsonDecryptedData();
            try
            {
                //encrypt IRM Data Here
                var IRMEncryptData = jsonEncryptedData.GetEncryptedData(jsonRequest.Data);
                //Set IRM Encrypted data on the request model
                jsonRequest.Data = IRMEncryptData;

                var DgftSeriveUrl = Program.DgftBaseUrl + "/ebrc/pushIRMToDGFT";
                //call Webservice
                var DgftResponse = CallIRMToDGFTService(DgftSeriveUrl, jsonRequest);
                
                JsonRequestData jsonRequestData =JsonConvert.DeserializeObject<JsonRequestData>(DgftResponse);
                
                var DataResponse = jsonRequestData.Data;
                //Decrypt data here
                SimpleResponse = jsonDecryptedData.GetDecryptedData(DataResponse);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return SimpleResponse;
        }

        
        public string CallIRMToDGFTService(string url, JsonRequestData jsonRequestData)
        {

            string responseData = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("accessToken", accessToken);
                request.Headers.Add("userID", userID);
                var ds = JsonConvert.SerializeObject(jsonRequestData);
                byte[] dataBytes = Encoding.UTF8.GetBytes(ds);
                request.ContentLength = dataBytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseData = streamReader.ReadToEnd();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseData;
        }

       


    }
}
