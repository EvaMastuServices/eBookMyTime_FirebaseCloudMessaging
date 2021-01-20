using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace eBookMyTime_FirebaseCloudMessaging
{

    /// <summary>
    /// developed by Jitendra using .Net Core 5.0
    /// </summary>
    class Program
    { 
        readonly string WebApiUrl = @"http://192.168.2.142/StudentKontkkt/api/StudentApi/";

        static void Main(string[] args)
        {
            Program pp = new Program();
            pp.SendNotificationOnMobile();
            pp.SendNotificationOnWeb();
        }
        public void SendNotificationOnMobile()
        {
            PushNotification obj = null;
            AndroidGCMPushNotification objAndroidGCMPushNotification = new AndroidGCMPushNotification();
            try
            {

                DataTable dt = objAndroidGCMPushNotification.GetUserPushNotification("M");
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        obj = new PushNotification();
                        obj.Sno = Convert.ToInt32(dt.Rows[i]["Sno"]);
                        obj.MessageType = Convert.ToString(dt.Rows[i]["MessageType"]);
                        obj.MessageText = Convert.ToString(dt.Rows[i]["MessageText"]);
                        obj.DeviceToken = Convert.ToString(dt.Rows[i]["DeviceToken"]);
                        obj.MessageTitle = Convert.ToString(dt.Rows[i]["MessageTitle"]);
                        obj.Data = Convert.ToString(dt.Rows[i]["Data"]);
                        obj.OtherDetails = Convert.ToString(dt.Rows[i]["OtherDetails"]);
                        obj.Data = obj.Data.Trim() + obj.OtherDetails;
                        string val1 = obj.MessageType + "-" + obj.Data;
                        obj.Data = obj.MessageText;
                        obj.MessageText = val1;
                        //end here
                        if (!string.IsNullOrEmpty(obj.DeviceToken))
                        {

                            if (dt.Rows[i]["DeviceType"].ToString().Trim().ToLower().Equals("ios"))
                            {
                                Console.WriteLine("Sending notification on Apple device for message ID: " + obj.Sno);
                                List<string> lst_errorText = new List<string>();
                                try
                                {
                                    //AppleNotification push = new AppleNotification();
                                    //push.SendAppleNotificationByPHP(obj.DeviceToken, obj.MessageText, obj.Data, obj.Data);
                                }
                                catch (Exception ex)
                                {
                                    lst_errorText.Add(ex.Message);
                                }

                                if (lst_errorText.Count > 0)
                                {
                                    //update error
                                    obj.Messageid = string.Empty;
                                    obj.Error = lst_errorText[0];
                                    objAndroidGCMPushNotification.UpdateUserPushNotification(obj);
                                    Console.WriteLine("Error :" + lst_errorText[0]);
                                }
                                else
                                {
                                    obj.Messageid = string.Empty;
                                    obj.Error = "";
                                    objAndroidGCMPushNotification.UpdateUserPushNotification(obj);
                                    Console.WriteLine("Done!");
                                }
                                //[[end here
                            }
                            else if (dt.Rows[i]["DeviceType"].ToString().Trim().ToLower().Equals("android"))
                            {
                                Console.WriteLine("Sending notification on Android device for message ID: " + obj.Sno);
                                string errorText = objAndroidGCMPushNotification.SendPushNotification(obj);
                                if (!string.IsNullOrEmpty(errorText))// added by Jitendra on 23-05-2017
                                {
                                    //update error
                                    obj.Messageid = string.Empty;
                                    obj.Error = errorText;
                                    objAndroidGCMPushNotification.UpdateUserPushNotification(obj);
                                    Console.WriteLine("Error :" + errorText);
                                }
                                else
                                    Console.WriteLine("Done!");
                            }

                        }
                    }

                }



            }
            catch (Exception ex)
            {
                obj.Messageid = string.Empty;
                obj.Error = ex.Message;
                objAndroidGCMPushNotification.UpdateUserPushNotification(obj);



            }
            finally
            {
                obj = null;

            }
        }
        public void SendNotificationOnWeb()
        {
            PushNotification obj = null;
            AndroidGCMPushNotification objAndroidGCMPushNotification = new AndroidGCMPushNotification();
            try
            {

                DataTable dt = objAndroidGCMPushNotification.GetUserPushNotification("W");
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        obj = new PushNotification();
                        obj.Sno = Convert.ToInt32(dt.Rows[i]["Sno"]);
                        obj.MessageType = Convert.ToString(dt.Rows[i]["MessageType"]);
                        obj.MessageText = Convert.ToString(dt.Rows[i]["MessageText"]);
                        obj.DeviceToken = Convert.ToString(dt.Rows[i]["DeviceToken"]);
                        obj.MessageTitle = Convert.ToString(dt.Rows[i]["MessageTitle"]);
                        obj.Data = Convert.ToString(dt.Rows[i]["Data"]);
                        obj.OtherDetails = Convert.ToString(dt.Rows[i]["OtherDetails"]);
                        obj.Data = obj.Data.Trim() + obj.OtherDetails;
                        string val1 = obj.MessageType + "-" + obj.Data;
                        obj.Data = obj.MessageText;
                        obj.MessageText = val1;
                        //end here
                        if (!string.IsNullOrEmpty(obj.DeviceToken))
                        {
                            WebNotification obj1 = new WebNotification();
                            obj1.Sno = Convert.ToInt32(dt.Rows[i]["Sno"]);
                            obj1.MessageType = Convert.ToString(dt.Rows[i]["MessageType"]);
                            obj1.MessageText = Convert.ToString(dt.Rows[i]["MessageText"]);
                            obj1.DeviceToken = Convert.ToString(dt.Rows[i]["DeviceToken"]);
                            obj1.MessageTitle = Convert.ToString(dt.Rows[i]["MessageTitle"]);
                            obj1.Data = Convert.ToString(dt.Rows[i]["Data"]);
                            obj1.LoginID = dt.Rows[i]["LoginID"].ToString().Trim();

                            Console.WriteLine("Sending notification on web for message ID: " + obj1.Sno);
                            string title = obj.MessageType + "-" + obj.Data;
                            string url = WebApiUrl + "GetChallengeNotification?connection=" + obj.DeviceToken + "&msg=" + obj.MessageText + "&title=" + title;
                            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                            {
                                webResponse.Close();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                obj.Messageid = string.Empty;
                obj.Error = ex.Message;
                objAndroidGCMPushNotification.UpdateUserPushNotification(obj);



            }
            finally
            {
                obj = null;

            }
        }

        public class WebNotification
        {
            public int Sno { get; set; }
            public string MessageType { get; set; }
            public string MessageText { get; set; }
            public string MessageTitle { get; set; }
            public string DeviceToken { get; set; }

            public string Messageid { get; set; }
            public string Error { get; set; }

            public string Data { get; set; }

            public string LoginID { get; set; }

        }
        public class PushNotification
        {
            public int Sno { get; set; }
            public string MessageType { get; set; }
            public string MessageText { get; set; }
            public string MessageTitle { get; set; }
            public string DeviceToken { get; set; }

            public string Messageid { get; set; }
            public string Error { get; set; }

            public string Data { get; set; }

            public string OtherDetails { get; set; } // added by Jitendra 2020-04-10

        }

        public class PushNotificationResponse
        {
            public string multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
            public results[] results { get; set; }

        }

        public class results
        {
            public string error { get; set; }
            public string message_id { get; set; }
        }

        public class AndroidGCMPushNotification
        {

            readonly string serverApiKey = "AAAAzIgW76g:APA91bGdnetPb3TbjjOajvFAmjc8q-WhLno3M13UGaFMoWJNqBthKhDOCwInnzdHukUSYu2qoHaZAEjdqb1ycmfCPBmdAa4_Rbb0UE-eIebccC1VnFy0S2BiJZoJvGqixyMuSH7BmJsv";
            readonly string senderId = "878456532904";
            readonly string PushNotificationEndpoint = "https://fcm.googleapis.com/fcm/send";

            public AndroidGCMPushNotification()
            {

            }


            /// <summary>
            /// <author>Pankaj Kumar</author>
            /// <desc>For sending push notification</desc>
            /// </summary>
            /// <param name="objPushNotification"></param>
            /// <returns></returns>
            public string SendPushNotification(PushNotification objPushNotification)
            {
                string result = string.Empty;
                try
                {

                    Console.WriteLine("Sending Android notification...");
                    //var value = message;
                    //HttpWebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send") as HttpWebRequest;
                    HttpWebRequest tRequest = WebRequest.Create(PushNotificationEndpoint) as HttpWebRequest;

                    //WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.UserAgent = "Android GCM Message Sender Client 1.0";
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                    //tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";
                    //string postData = "notification.title=KontaKKT&notification.body=HI Pankaj&to=" + deviceId;
                    //Byte[] byteArray = Encoding.UTF8.GetBytes(postData);


                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = objPushNotification.DeviceToken,
                        priority = "high",//high,//normal
                                          //collapse_key = "score_update",
                                          //delay_while_idle = true,
                                          //time_to_live = 108,
                                          //message = "Hi all",
                        notification = new
                        {
                            title = objPushNotification.Data, //objPushNotification.MessageType + "-" +
                            body = objPushNotification.MessageText,
                            //type = objPushNotification.MessageType
                        },
                        data = new
                        {
                            title = objPushNotification.Data,//objPushNotification.MessageType + "-" +
                            body = objPushNotification.MessageText,
                        }
                    };
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    string sResponseFromServer = tReader.ReadToEnd();
                                    var deserializer = new JavaScriptSerializer();
                                    PushNotificationResponse objPushNotificationResponse = deserializer.Deserialize<PushNotificationResponse>(sResponseFromServer);
                                    //result = sResponseFromServer;
                                    objPushNotification.Messageid = objPushNotificationResponse.results[0].message_id;
                                    objPushNotification.Error = !string.IsNullOrEmpty(objPushNotificationResponse.results[0].error) ? objPushNotificationResponse.results[0].error : string.Empty;

                                    Console.WriteLine("{Sno:\t" + objPushNotification.Sno + "} { MessageId:\t" + objPushNotification.Messageid + "}{Error:\t" + objPushNotification.Error + "}");
                                    if (!string.IsNullOrEmpty(objPushNotification.Messageid))
                                    {
                                        UpdateUserPushNotification(objPushNotification);
                                    }
                                    else
                                    {
                                        UpdateUserPushNotification(objPushNotification);
                                        //LogEntryIntoFile(sResponseFromServer);
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return result = ex.Message;
                }

                return result;
            }

            #region DB

            public DataTable GetUserPushNotification(string NotifyFor)
            {
                DataSet ds = new DataSet();
                try
                {
                    SqlConnection con = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=test;Trusted_Connection=True;");
                    using (con)
                    {
                        SqlCommand command = new SqlCommand("SELECT * FROM [UserPushNotification] WHERE [SentStatus] ='P' and [NotificationFor]= '" + NotifyFor + "'", con);
                        command.CommandType = CommandType.Text;
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LogEntryIntoFile(ex.Message);
                    //throw ex;
                    return null;
                }
            }

            public void UpdateUserPushNotification(PushNotification objPushNotification)
            {

                try
                {
                    SqlConnection con = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=test;Trusted_Connection=True;");
                    using (con)
                    {
                        SqlCommand command = new SqlCommand();
                        if (objPushNotification.Error != "")
                            command = new SqlCommand("Update  [UserPushNotification] SET [SentStatus]='E',[Error]='" + objPushNotification.Error + "' WHERE  Sno=" + objPushNotification.Sno + " and [DeviceToken]='" + objPushNotification.DeviceToken + "' and Active='Y' ", con);
                        else
                            command = new SqlCommand("Update  [UserPushNotification] SET [SentStatus]='S',[Error]='',[SentMessageId]='" + objPushNotification.Messageid + "' WHERE  Sno=" + objPushNotification.Sno + " and [DeviceToken]='" + objPushNotification.DeviceToken + "' and Active='Y' ", con);
                        command.CommandType = CommandType.Text;
                        con.Open();
                        int result = command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }

            #endregion


        }
    }
}
