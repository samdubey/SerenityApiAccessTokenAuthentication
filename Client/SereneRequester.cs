using Newtonsoft.Json;
using RestSharp;
using MyApp.ViewModels;
using Serenity.Services;
using Slps.ProtectionAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp
{
    public static class SereneRequester
    {
        [Protect]
        public static IRestResponse<bool> IsLoggedIn(string baseUrl, string token)
        {
            #region IsLoggedIn Request
            IRestResponse<bool> response = new RestResponse<bool>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.Resource = "Api/Account/IsLoggedIn";
                    request.AddHeader("Authorization", "Bearer " + token);
                    response = restClient.Execute<bool>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // If you want to used data as UserDefination, so please replace UsersDTO with UserDefinition
        // Get current user row request
        public static IRestResponse<RetrieveResponse<MyRow>> GetLoginUserData<MyRow>(string baseUrl, string resource)
        {
            #region Get Login User Request
            IRestResponse<RetrieveResponse<MyRow>> response = new RestResponse<RetrieveResponse<MyRow>>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST)
                    {
                        Resource = resource
                    };
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    response = restClient.Execute<RetrieveResponse<MyRow>>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Login user request
        public static IRestResponse<ServiceResponse> Login(string baseUrl, string username, string password)
        {
            #region Login Service Request
            IRestResponse<ServiceResponse> response = new RestResponse<ServiceResponse>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST)
                    {
                        Resource = "Api/Account/GenerateToken"
                    };
                    //Method 1 : Passing parameters in c# object
                    request.AddHeader("Content-type", "application/json");
                    request.AddParameter("Username", username);
                    request.AddParameter("Password", password);
                    var cancellationTokenSource = new CancellationTokenSource();
                    //response = restClient.Execute<ServiceResponse>(request);
                    Task<IRestResponse<ServiceResponse>> serviceResponse = Task.Run<IRestResponse<ServiceResponse>>(async () => await restClient.ExecuteTaskAsync<ServiceResponse>(request, cancellationTokenSource.Token));
                    response = serviceResponse.Result;
                    if (response.StatusCode == HttpStatusCode.OK && response.ResponseStatus == ResponseStatus.Completed)
                    {
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        VMMainModel.Instance.AuthToken = ((object)jsonResponse["token"]).ToString();
                        MyApp.Properties.Settings.Default.Token=MyApp.CommonHelper.Protect(VMMainModel.Instance.AuthToken);
                        MyApp.Properties.Settings.Default.Save();
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }


        // Create row request
        public static IRestResponse<SaveResponse> Create<MyRow>(string baseUrl, string resource, SaveRequest<MyRow> saveRequest)
        {
            #region Create Service Request
            IRestResponse<SaveResponse> response = new RestResponse<SaveResponse>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(saveRequest);
                    request.RequestFormat = DataFormat.Json;

                    response = restClient.Execute<SaveResponse>(request);
                    if (response.StatusCode == HttpStatusCode.OK && response.ResponseStatus == ResponseStatus.Completed)
                    {
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        response.Data.EntityId = (object)jsonResponse["EntityId"];
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                    //MessageBox.Show(ex.Message);
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Bulk Create row request
        public static IRestResponse<ListResponse<ResponseEntity>> BulkCreate<RequestEntity, ResponseEntity>(string baseUrl, string resource, List<SaveRequest<RequestEntity>> saveRequestList, string cookieName)
        {
            #region Bulk Create Service Request
            IRestResponse<ListResponse<ResponseEntity>> response = new RestResponse<ListResponse<ResponseEntity>>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(saveRequestList);
                    request.RequestFormat = DataFormat.Json;

                    response = restClient.Execute<ListResponse<ResponseEntity>>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                    //MessageBox.Show(ex.Message);
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Bulk Generate rows request
        public static IRestResponse<ListResponse<ResponseEntity>> BulkGenerate<RequestEntity, ResponseEntity>(string baseUrl, string resource, RequestEntity requestParam)
        {
            #region Bulk Generate Rows Service Request
            IRestResponse<ListResponse<ResponseEntity>> response = new RestResponse<ListResponse<ResponseEntity>>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(requestParam);
                    request.RequestFormat = DataFormat.Json;
                    response = restClient.Execute<ListResponse<ResponseEntity>>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                    //MessageBox.Show(ex.Message);
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Update row request
        public static IRestResponse<SaveResponse> Update<MyRow>(string baseUrl, string resource, SaveRequest<MyRow> saveRequest)
        {
            #region Update Service Request
            IRestResponse<SaveResponse> response = new RestResponse<SaveResponse>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(saveRequest);
                    request.RequestFormat = DataFormat.Json;

                    response = restClient.Execute<SaveResponse>(request);
                    if (response.StatusCode == HttpStatusCode.OK && response.ResponseStatus == ResponseStatus.Completed)
                    {
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        response.Data.EntityId = (object)jsonResponse["EntityId"];
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Update row request with fileList
        public static IRestResponse<SaveResponse> Update<MyRow>(string baseUrl, string resource, SaveRequest<MyRow> saveRequest, List<string> fileList, string cookieName)
        {
            #region Update Service Request
            IRestResponse<SaveResponse> response = new RestResponse<SaveResponse>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(saveRequest);
                    request.RequestFormat = DataFormat.Json;
                    fileList.ForEach(file =>
                    {
                        request.AddFile("receipt[receipt_file]", File.ReadAllBytes(file), "Invoice.jpg", "application/octet-stream");
                    });

                    response = restClient.Execute<SaveResponse>(request);
                    if (response.StatusCode == HttpStatusCode.OK && response.ResponseStatus == ResponseStatus.Completed)
                    {
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        response.Data.EntityId = (object)jsonResponse["EntityId"];
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                    //MessageBox.Show(ex.Message);
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Delete row request
        public static IRestResponse<DeleteResponse> Delete(string baseUrl, string resource, DeleteRequest deleteRequest, string cookieName)
        {
            #region Delete Service Request
            IRestResponse<DeleteResponse> response = new RestResponse<DeleteResponse>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(deleteRequest);
                    request.RequestFormat = DataFormat.Json;
                    response = restClient.Execute<DeleteResponse>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                    //MessageBox.Show(ex.Message);
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // Retrieve row request
        public static IRestResponse<RetrieveResponse<MyRow>> Retrieve<MyRow>(string baseUrl, string resource, RetrieveRequest retrieveRequest)
        {
            #region Retrieve Service Request
            IRestResponse<RetrieveResponse<MyRow>> response = new RestResponse<RetrieveResponse<MyRow>>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.Resource = resource;
                    request.AddJsonBody(retrieveRequest);
                    request.RequestFormat = DataFormat.Json;
                    response = restClient.Execute<RetrieveResponse<MyRow>>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        // List of rows retrieve request
        public static IRestResponse<ListResponse<MyRow>> List<MyRow>(string baseUrl, string resource, ListRequest listRequest)
        {
            #region List Service Request
            IRestResponse<ListResponse<MyRow>> response = new RestResponse<ListResponse<MyRow>>();
            if (InternetConnection.IsConnectedToInternet() == true)
            {
                try
                {
                    var restClient = new RestClient(baseUrl);
                    var request = new RestRequest(Method.POST);
                    request.Resource = resource;
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.AddHeader("Authorization", "Bearer " + VMMainModel.Instance.AuthToken);
                    request.AddJsonBody(listRequest);
                    request.RequestFormat = DataFormat.Json;
                    response = restClient.Execute<ListResponse<MyRow>>(request);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex.InnerException;
                    //MessageBox.Show(ex.Message);
                }
                return response;
            }
            else
            {
                response.ErrorMessage = "Internet connection not available. Please check connection.";
                return response;
            }
            #endregion
        }

        public static string UploadFile(string baseUrl, string uploadPath, string filePath)
        {
            string tempFilePath = string.Empty;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                //filePath = filePath.Substring(8, filePath.Length - 1);
                string fileName = Path.GetFileName(filePath);
                var client = new RestClient(baseUrl);
                var request = new RestRequest(uploadPath, Method.POST);

                // add files to upload (works with compatible verbs)
                request.AddFile(fileName, filePath);

                // execute the request
                IRestResponse response = client.Execute(request);
                var obj = JsonConvert.DeserializeObject<FileUploadResponse>(response.Content.ToString());
                //response.Content = "{"TemporaryFile":"temporary/0ea10d13dbaa40cfb5f4dbdaf1b10304.png","Size":18650,"IsImage":true,"Width":151,"Height":95}"
                tempFilePath = uploadPath + "/" + obj.TemporaryFile;

                // Async Request
                //client.ExecuteAsync(request, response =>
                //{
                //    tempFilePath = response.Content.ToString();
                //});
            }
            return tempFilePath;
        }

        // Get FileName from Path
        public static string GetFileNameFromUri(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                int pathLength = filePath.Length - 8;
                var path = filePath.Substring(8, pathLength).ToString();
                return path;
            }
            else
            {
                return filePath;
            }
        }

        // Get FilePath from Uri
        public static string GetPathFromUri(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                int pathLength = filePath.Length - 8;
                var path = filePath.Substring(8, pathLength).ToString();
                return path;
            }
            else
            {
                return filePath;
            }
        }

        // Create TempFile for uploading purpose
        public static string CreateTempFile(string filePath)
        {
            #region Create Temp File
            // Generate temporary directory name
            String directory = Path.Combine(Path.GetTempPath(), "Serene");// Path.GetRandomFileName());
            // Temporary file path
            String tempfile = Path.Combine(directory, Path.GetFileName(filePath));
            // Create directory in file system
            Directory.CreateDirectory(directory);
            // Copy input file to the temporary directory
            File.Copy(filePath, tempfile);
            FileInfo fileInfo = new FileInfo(tempfile);

            // Set the Attribute property of this file to Temporary. 
            // Although this is not completely necessary, the .NET Framework is able 
            // to optimize the use of Temporary files by keeping them cached in memory.
            fileInfo.Attributes = FileAttributes.Temporary;
            // Delete file in temporary directory
            //File.Delete(tempfile);
            return tempfile;
            #endregion
        }

        // Delete TempFile
        public static void DeleteTempFile(string tempfile)
        {
            #region Delete Temp File
            if (File.Exists(tempfile))
                File.Delete(tempfile);
            #endregion
        }
    }

    public class FileUploadResponse
    {
        public string TemporaryFile { get; set; }
        public double Size { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsImage { get; set; }
    }
}
