using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GHRace3.Models
{
    /// <summary>
    /// One unique attribue which is a hash of track name and date eg ("Wimbledon" + "3/16/2013"), before any new data is added the track and race date is hashed and checked agaisnt this table
    /// </summary>
    public class Meeting
    {
        [Key]
        public string TrackDateHash { get; private set; }

        public void SetTrackDateHash(string url)
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(url);
            string date = DateTime.Parse(nvc[@"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate"].Trim()).ToShortDateString();
            string trackName = nvc["track"].Trim();
            byte[] hashVal;
            byte[] byteArray = Encoding.ASCII.GetBytes(trackName + date);
            MD5 md5Hash = MD5.Create();
            hashVal = md5Hash.ComputeHash(byteArray);
            this.TrackDateHash = GetHexString(hashVal);
        }

        private string GetHexString(byte[] hashVal)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in hashVal)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }
    }

 
}