using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RaceDataService
{
    public class RetrieveRaceData : IRetrieveRaceData
    {
        ICommonUtilities _utils;
        string url = @"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate=19/03/2013%2000:00:00&track=Wimbledon";
        const string table_pattern = "<table.*?>(.*?)</table>";

        const string tr_pattern = "<tr>(.*?)</tr>";

        const string td_pattern = "<td.*?>(.*?)</td>";

        const string HTML_TAG_PATTERN = "<.*?>";

        const string LETTERS_AND_NUMBERS = @"[a-zA-Z0-9-]";

        string[] Headers = { "Fin", "Greyhound", "Trap", "SP", "Time/Sec.", "Time/Distance" };

        string TRACK;

        public RetrieveRaceData(ICommonUtilities utils)
        {
            _utils = utils;
        }

        public ICollection<ITrack> GetData(Dictionary<string, string> htmlPageList)
        {
            List<ITrack> tracksDataFromHtml = new List<ITrack>();
            foreach (var htmlPage in htmlPageList)
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(htmlPage.Key);
                TRACK = nvc["track"].Trim();
                MatchCollection table_matches = Regex.Matches(htmlPage.Value, td_pattern, RegexOptions.Singleline);
                List<string> matchList = new List<string>();
                List<string> formatedMatchList = new List<string>();
                foreach (var item in table_matches)
                {
                    string text = Regex.Replace(item.ToString(), HTML_TAG_PATTERN, string.Empty);
                    matchList.Add(text);
                }

                foreach (var item in matchList)
                {
                    formatedMatchList.Add(RemoveNewLines(item));
                }
                var meeting = ConvertToString(RemoveHeaderAndFooter(formatedMatchList));
                tracksDataFromHtml.Add(new SeedTrack { Name = TRACK, Greyhounds = GetGreyhounds(meeting) });
            }
            return tracksDataFromHtml;
        }

        private ICollection<IGreyhound> GetGreyhounds(List<Race> meeting)
        {
            var greyhoundList = new List<IGreyhound>();

            foreach (var item in meeting)
            {
                foreach (var race in item.Runners)
                {
                    if (item.Runners.Count > 0)
                    {
                        greyhoundList.Add(new SeedGreyhound
                        {
                            Name = race.Greyhound,
                            Races = GetRace(race)
                        });
                    }
                }
            }

            return greyhoundList;
        }

        private ICollection<IRace> GetRace(Runner race)
        {
            List<IRace> raceList = new List<IRace>();

            raceList.Add(new SeedRace
            {
                Comments = race.Comment,
                Trainer = race.Trainer,
                Details = race.Details,
                Finish = int.Parse(race.Finish),
                Date = DateTime.Parse(race.Date),
                Grade = GetGrade(race.Grade),
                RaceDistance = int.Parse(_utils.RemoveLetters(race.Distance)),
                Trap = int.Parse(race.Trap),
                TimeDistance1 = Math.Round(float.Parse(_utils.GetTimeDistance1(race.TimeDistance, 0).ToString()), 2),
                TimeDistance2 = _utils.GetTimeDistance1(race.TimeDistance, 1),
                TimeDistance3 = _utils.GetTimeDistance3(race.TimeDistance),
                SP1 = _utils.GetSP(race.SP, 0),
                SP2 = _utils.GetSP(race.SP, 1),
                SP3 = _utils.GetSP3(race.SP),
                TimeSec = Math.Round(float.Parse(_utils.GetFloat(race.TimeSec).ToString()), 2)
            });

            return raceList;
        }

        private string GetGrade(string p)
        {
            string pattern = "Grade";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(p, replacement).Trim();
        }

        /// <summary>
        /// this one removes all the ",,," etc and white space from the list, also it removes the title and links from the top
        /// and bottom
        /// </summary>
        /// <param name="formatedMatchList"></param>
        /// <returns></returns>
        private List<string> RemoveHeaderAndFooter(List<string> formatedMatchList)
        {
            //remove the header
            List<string> temp = new List<string>();
            temp.AddRange(formatedMatchList);

            foreach (var item in formatedMatchList)
            {
                //if (item.Equals("Wimbledon"))
                if (item.Equals(TRACK))
                {
                    break;
                }
                else
                {
                    temp.Remove(item);
                }
            }

            formatedMatchList.Reverse();
            foreach (var item in formatedMatchList)
            {
                if (item.Contains("Comment:"))
                {
                    break;
                }
                else
                {
                    temp.Remove(item);
                }
            }

            formatedMatchList.Reverse();
            Regex numbersAndLetters = new Regex(LETTERS_AND_NUMBERS);
            StringBuilder sb = new StringBuilder();
            foreach (var item in formatedMatchList)
            {
                if (!(numbersAndLetters.IsMatch(item)))
                {
                    sb.Append(item);
                    sb.Append("\n");
                    temp.Remove(item);
                }
            }
            string s = sb.ToString();
            return ReadyListForXML(temp);
            //remove footer
        }

        /// <summary>
        /// Removes all the headers: string[] Headers = { "Fin", "Greyhound", "Trap", "SP", "Time/Sec.", "Time/Distance" };
        /// from the list, also trims leading and trailing ","
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private List<string> ReadyListForXML(List<string> temp)
        {
            List<string> fullList = new List<string>();

            StringBuilder sb = new StringBuilder();
            foreach (var item in temp)
            {
                string s = item.TrimStart(',');
                sb.Append(s.TrimEnd(','));
                sb.Append("\n");
            }
            string tempList = sb.ToString().TrimEnd('\n');
            string[] tempArray = tempList.Split('\n');
            foreach (var item in tempArray)
            {
                fullList.Add(item.Trim());
            }

            var query = from string s in fullList
                        where !(Headers.Contains(s))
                        select s;

            return new List<string>(query);
        }


        /// <summary>
        /// <RACE>
        //<row>
        //  <Fin>1</Fin>
        //  <Greyhound> Cold Tea </Greyhound>
        //  <Trap>4</Trap>
        //  <SP>11/4</SP>
        //  <Time_Sec_>5.16</Time_Sec_>
        //  <Time_Distance>29.81</Time_Distance>
        //  <Details>bkw d Tyrur Ted - Miss Blue Eyes Oct-2007</Details>
        //  <Trainer>(Trainer:J S J Simpson)</Trainer>
        //  <Comments_>
        //    Comment: Mid,RnOn,LedNrLn
        //  </Comments_>
        //</row>
        /// </summary>
        /// <param name="formatedMatchList"></param>
        private List<Race> ConvertToString(List<string> formatedMatchList)
        {
            StringBuilder sb = new StringBuilder();
            char[] raceDelimiter = { '|', '~', '|' };
            foreach (var item in formatedMatchList)
            {
                if (item.Equals(TRACK))
                {
                    sb.Append("|~|");
                    sb.Append("\n");
                }
                sb.Append(item);
                sb.Append("\n");
            }

            //txtList.Text = sb.ToString(); //put the list on the UI

            string[] raceSplit = sb.ToString().Split(raceDelimiter);
            var query = from string s in raceSplit
                        where s != string.Empty
                        select s;
            return ConvertToRace(new List<string>(query));
        }

        /// <summary>
        /// raceMeet is a list of strings which contain complete races (all details - times, greyhound names, distance etc).
        /// Each of these needs to be put into an object.
        /// </summary>
        /// <param name="raceMeet"></param>
        private List<Race> ConvertToRace(List<string> raceMeet)
        {
            List<Race> meeting = new List<Race>(); //this will contain each greyhounds stats (in string format) for each race in the meeting
            foreach (var item in raceMeet) //each of these is a complete race so need to extract the runners
            {
                List<string> raceInfo = GetReaceInfo(item.Split('\n'));
                List<string> raceData = RemoveInfo(item.Split('\n'), raceInfo);
                //meeting.Add(GetRace(item));
                meeting.Add(GetRace(new List<Runner>(), raceInfo, raceData));
                //Race r = GetRace(new List<Runner>(), raceInfo, raceData);
            }
            return meeting;
        }

        private Race GetRace(List<Runner> list, List<string> raceInfo, List<string> raceData)
        {
            Runner r = new Runner();
            int integerCount = 0;
            for (int i = 0; i < 9; i++)
            {
                int j;
                bool isInt = int.TryParse(raceData[i], out j);
                if (isInt)
                {
                    integerCount++;
                }
            }
            if (integerCount > 2) //data is missing from this race
            {
                return new Race { Runners = list };
            }
            r.Track = raceInfo[0];
            r.Date = raceInfo[1];
            r.Grade = raceInfo[2];
            r.Distance = raceInfo[3];
            if (raceInfo.Count > 4) //means we've got extra race details
            {
                r.Details = raceInfo[4];
            }

            r.Finish = raceData[0];
            r.Greyhound = raceData[1];
            r.Trap = raceData[2];
            r.SP = raceData[3];
            r.TimeSec = raceData[4];
            r.TimeDistance = raceData[5];
            r.Lineage = raceData[6];
            r.Trainer = raceData[7];
            r.Comment = raceData[8];



            list.Add(r);

            try
            {
                raceData.RemoveRange(0, 9);
            }
            catch (Exception ex)
            {
                raceData.Clear();
            }

            if (raceData.Count < 2)
            {
                return new Race { Runners = list };
            }
            else
            {
                return GetRace(list, raceInfo, raceData);
            }

        }

        private List<string> RemoveInfo(string[] p, List<string> raceInfo)
        {
            List<string> pureResults = new List<string>();
            bool checkForRaceInfo = true;
            foreach (var item in p)
            {
                if (checkForRaceInfo)
                {
                    int i;
                    bool isInt = int.TryParse(item, out i);
                    if (isInt)
                    {
                        checkForRaceInfo = false;
                        pureResults.Add(item);
                    }
                }
                else
                {
                    pureResults.Add(item);
                }

            }
            return pureResults;
        }

        /// <summary>
        /// Wimbledon
        //02/11/2012   20:00
        //Grade A6
        //480m
        //1st £48, Others £22 (BGRF added £30)
        //WIMBLEDON KENNEL SWEEP STAKES
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<string> GetReaceInfo(string[] p)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var item in p)
            {
                count++;
                int i;
                bool isInt = int.TryParse(item, out i);
                if (isInt) //found the start of the actual race data so return
                {
                    break;
                }
                else //still getting race info
                {
                    if (count < 6) //if we've appended less than 6 lines of race info so far then keep appending the info with a new line
                    {
                        sb.Append(item);
                        sb.Append('\n');
                    }
                    else //race info is longer than normal (see above example) so tack any additional lines onto the last line.
                    {
                        sb.Append(" ");
                        sb.Append(item);
                    }
                }
            }
            string[] temp = sb.ToString().Trim().Split('\n');
            return temp.ToList();
        }

        private string RemoveNewLines(object item)
        {
            string s = item.ToString().Replace("\r", string.Empty);
            string p = s.Replace("\n", ",");
            string q = p.Replace("&nbsp;", " ");
            string a = q.Replace(",,,,,,,,", ",");
            string b = a.Replace(",,,", ",");
            string c = b.Replace(",,,,", ",");
            string w = c.Replace(",,", ",");
            return w;
        }

        private class Runner
        {
            public string Track { get; set; }
            public string Date { get; set; }
            public string Grade { get; set; }
            public string Distance { get; set; }
            public string Details { get; set; }
            public string Finish { get; set; }
            public string Greyhound { get; set; }
            public string Trap { get; set; }
            public string SP { get; set; }
            public string TimeSec { get; set; }
            public string TimeDistance { get; set; }
            public string Lineage { get; set; }
            public string Trainer { get; set; }
            public string Comment { get; set; }
        }

        private class Race
        {
            public List<Runner> Runners { get; set; }
        }
    }

    
}
