using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WPSendNotification.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        public ActionResult Index()
        {
            ViewBag.Return = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection c)
        {
            try
            {
                string subscriptionUri = c["uri"];
                ViewBag.Return = "";

                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);
                sendNotificationRequest.Method = "POST";

                // Create the toast message.
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<wp:Notification xmlns:wp=\"WPNotification\">" +
                   "<wp:Toast>" +
                        "<wp:Text1>" + c["title"].ToString() + "</wp:Text1>" +
                        "<wp:Text2>" + c["message"].ToString().ToString() + "</wp:Text2>" +
                        "<wp:Param>/PromocaoDetalhe.xaml?Category=promocao&amp;ItemID=" + c["Promocao"].ToString() + "&amp;NavigatedFrom=ToastNotification</wp:Param>" +
                   "</wp:Toast> " +
                "</wp:Notification>";

                // Set the notification payload to send.
                byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

                // Set the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
                sendNotificationRequest.Headers.Add("X-NotificationClass", "2");


                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                // Send the notification and get the response.
                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                ViewBag.Return = notificationStatus + " | " + deviceConnectionStatus + " | " + notificationChannelStatus;
            }
            catch (Exception ex)
            {
                ViewBag.Return = "Exception caught sending update: " + ex.ToString();
                return View(c);
            }

            return View();
        }
    }
}