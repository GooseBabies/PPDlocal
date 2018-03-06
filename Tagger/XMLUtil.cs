using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Tagger
{
    public class XMLUtil
    {
        const string SETTINGSFIELD = "Settings";
        const string PROFILEFIELD = "Profile";
        const string DBNAMEFIELD = "DatabaseName";
        const string VIEWCATFIELD = "ViewCatergoriesText";
        const string SHUFFLEFIELD = "Shuffle";
        const string REPEATSHUFFLEFIELD = "RepeatShuffle";
        const string SSINTERVALFIELD = "SlideshowInterval";
        const string VOLUMEFIELD = "Volume";
        const string PNG = "png";
        const string JPG = "jpg";
        const string JPEG = "jpeg";
        const string BMP = "bmp";
        const string GIF = "gif";
        const string MP4 = "mp4";
        const string WMV = "wmv";
        const string AVI = "avi";
        const string MPG = "mpg";
        const string MKV = "mkv";
        const string MOUSEDELAY = "MouseDelay";
        const string VISIBLENAV = "VisibleNav";
        const string BOORUTHRESHOLD = "BooruThreshold";
        const string BOORUSOURCE = "BooruSource";
        const string NAMEFIELD = "Name";
        const string DEFAULTFIELD = "Default";
        const string SETTINGSFILE = "taggersettings.xml";
        private string XMLLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Tagger");

        public class SaveSettings
        {
            public string Profile { get; set; }
            public string DBLocation { get; set; }
            public bool ViewCat { get; set; }
            public int Slideshowinterval { get; set; }
            public bool Shuffle { get; set; }
            public bool RepeatShuffle { get; set; }
            public double Volume { get; set; }
            public bool Png { get; set; }
            public bool Jpg { get; set; }
            public bool Jpeg { get; set; }
            public bool Bmp { get; set; }
            public bool Gif { get; set; }
            public bool Mp4 { get; set; }
            public bool Wmv { get; set; }
            public bool Avi { get; set; }
            public bool Mpg { get; set; }
            public bool Mkv { get; set; }
            public int Mousedisappeardelay { get; set; }
            public bool VisibleNav { get; set; }
            public int BooruSource { get; set; }
            public int BooruThreshold { get; set; }
            public bool SettingsError { get; set; }

        }

        ErrorHandling Error = new ErrorHandling();

        public bool CreateFile(SaveSettings settings, string profilename)
        {
            try
            {
                XDocument xmlfile = new XDocument(
                new XDeclaration("1.0", "utf-16", "true"),
                new XProcessingInstruction("test", "value"),
                new XElement(SETTINGSFIELD,
                    new XElement(PROFILEFIELD, new XAttribute(NAMEFIELD, profilename), new XAttribute(DEFAULTFIELD, "true"),
                        new XElement(DBNAMEFIELD, settings.DBLocation),
                        new XElement(VIEWCATFIELD, settings.ViewCat),
                        new XElement(SHUFFLEFIELD, settings.Shuffle),
                        new XElement(REPEATSHUFFLEFIELD, settings.RepeatShuffle),
                        new XElement(SSINTERVALFIELD, settings.Slideshowinterval),
                        new XElement(VOLUMEFIELD, settings.Volume.ToString()),
                        new XElement(PNG, settings.Png),
                        new XElement(JPG, settings.Jpg),
                        new XElement(JPEG, settings.Jpeg),
                        new XElement(BMP, settings.Bmp),
                        new XElement(GIF, settings.Gif),
                        new XElement(MP4, settings.Mp4),
                        new XElement(WMV, settings.Wmv),
                        new XElement(AVI, settings.Avi),
                        new XElement(MPG, settings.Mpg),
                        new XElement(MKV, settings.Mkv),
                        new XElement(MOUSEDELAY, settings.Mousedisappeardelay),
                        new XElement(VISIBLENAV, settings.VisibleNav),
                        new XElement(BOORUSOURCE, settings.BooruSource),
                        new XElement(BOORUTHRESHOLD, settings.BooruThreshold)
                        )
                    )
                 );

                xmlfile.Save(Path.Combine(XMLLocation, SETTINGSFILE));
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);                
                return false;
            }
            
        }

        public bool SaveFile(SaveSettings settings, string profilename)
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));
                var query = from item in profile.Elements() where item.Attributes().ElementAt(0).Value.ToString() == profilename select item;

                foreach (XElement itemElement in query)
                {
                    itemElement.SetElementValue(VIEWCATFIELD, settings.ViewCat);
                    itemElement.SetElementValue(SHUFFLEFIELD, settings.Shuffle);
                    itemElement.SetElementValue(REPEATSHUFFLEFIELD, settings.RepeatShuffle);
                    itemElement.SetElementValue(SSINTERVALFIELD, settings.Slideshowinterval);
                    itemElement.SetElementValue(VOLUMEFIELD, settings.Volume.ToString());
                    itemElement.SetElementValue(PNG, settings.Png);
                    itemElement.SetElementValue(JPG, settings.Jpg);
                    itemElement.SetElementValue(JPEG, settings.Jpeg);
                    itemElement.SetElementValue(BMP, settings.Bmp);
                    itemElement.SetElementValue(GIF, settings.Gif);
                    itemElement.SetElementValue(MP4, settings.Mp4);
                    itemElement.SetElementValue(WMV, settings.Wmv);
                    itemElement.SetElementValue(AVI, settings.Avi);
                    itemElement.SetElementValue(MPG, settings.Mpg);
                    itemElement.SetElementValue(MKV, settings.Mkv);
                    itemElement.SetElementValue(MOUSEDELAY, settings.Mousedisappeardelay);
                    itemElement.SetElementValue(VISIBLENAV, settings.VisibleNav);
                    itemElement.SetElementValue(BOORUSOURCE, settings.BooruSource);
                    itemElement.SetElementValue(BOORUTHRESHOLD, settings.BooruThreshold);
                }

                profile.Save(Path.Combine(XMLLocation, SETTINGSFILE));
                return true;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
            
        }

        public bool SaveDefaultFile(SaveSettings settings, string profilename)
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));
                var query = from item in profile.Elements() where item.Attributes().ElementAt(1).Value.ToString() == "true" select item;

                foreach (XElement itemElement in query)
                {
                    itemElement.SetAttributeValue(NAMEFIELD, profilename);
                    itemElement.SetElementValue(DBNAMEFIELD, settings.DBLocation);
                    itemElement.SetElementValue(VIEWCATFIELD, settings.ViewCat);
                    itemElement.SetElementValue(SHUFFLEFIELD, settings.Shuffle);
                    itemElement.SetElementValue(REPEATSHUFFLEFIELD, settings.RepeatShuffle);
                    itemElement.SetElementValue(SSINTERVALFIELD, settings.Slideshowinterval);
                    itemElement.SetElementValue(VOLUMEFIELD, settings.Volume.ToString());
                    itemElement.SetElementValue(PNG, settings.Png);
                    itemElement.SetElementValue(JPG, settings.Jpg);
                    itemElement.SetElementValue(JPEG, settings.Jpeg);
                    itemElement.SetElementValue(BMP, settings.Bmp);
                    itemElement.SetElementValue(GIF, settings.Gif);
                    itemElement.SetElementValue(MP4, settings.Mp4);
                    itemElement.SetElementValue(WMV, settings.Wmv);
                    itemElement.SetElementValue(AVI, settings.Avi);
                    itemElement.SetElementValue(MPG, settings.Mpg);
                    itemElement.SetElementValue(MKV, settings.Mkv);
                    itemElement.SetElementValue(MOUSEDELAY, settings.Mousedisappeardelay);
                    itemElement.SetElementValue(VISIBLENAV, settings.VisibleNav);
                    itemElement.SetElementValue(BOORUSOURCE, settings.BooruSource);
                    itemElement.SetElementValue(BOORUTHRESHOLD, settings.BooruThreshold);
                }

                profile.Save(Path.Combine(XMLLocation, SETTINGSFILE));
                return true;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public SaveSettings LoadDefaultProfile()
        {
            SaveSettings setts = new SaveSettings
            {
                DBLocation = "",
                ViewCat = false,
                Shuffle = true,
                RepeatShuffle = false,
                Slideshowinterval = 3,
                Volume = 0.3,
                Png = true,
                Jpg = true,
                Jpeg = true,
                Bmp = true,
                Gif = true,
                Mp4 = true,
                Wmv = true,
                Avi = true,
                Mpg = true,
                Mkv = true,
                Mousedisappeardelay = 4,
                VisibleNav = true,
                BooruSource = 0,
                BooruThreshold = 60,
                SettingsError = false               

            };

            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));

                var query = from item in profile.Elements() where item.Attributes().ElementAt(1).Value.ToString() == "true" select item.Elements().ElementAt(0).Value.ToString();  //dbname
                var query2 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(1).Value.ToString(); //view cat
                var query3 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(2).Value.ToString(); //shuffle
                var query4 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(3).Value.ToString(); //repeat shuffle
                var query5 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(4).Value.ToString(); //Slideshow interval
                var query6 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(5).Value.ToString();  //volume
                var query7 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Attributes().ElementAt(0).Value.ToString();  //profile
                var query8 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(6).Value.ToString(); //png
                var query9 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(7).Value.ToString(); //jpg
                var query10 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(8).Value.ToString(); //jpeg
                var query11 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(9).Value.ToString(); //bmp
                var query12 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(10).Value.ToString(); //gif
                var query13 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(11).Value.ToString(); //mp4
                var query14 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(12).Value.ToString(); //wmv
                var query15 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(13).Value.ToString(); //avi
                var query16 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(14).Value.ToString(); //mpg
                var query17 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(15).Value.ToString(); //mkv
                var query18 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(16).Value.ToString(); //mousedelay
                var query19 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(17).Value.ToString(); //visiblenav
                var query20 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(18).Value.ToString(); //BooruSource
                var query21 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(19).Value.ToString(); //BooruThreshold

                setts.DBLocation = query.First();
                setts.ViewCat = (query2.First() == "true" ? true : false);
                setts.Shuffle = (query3.First() == "true" ? true : false);
                setts.RepeatShuffle = (query4.First() == "true" ? true : false);
                setts.Slideshowinterval = Convert.ToInt32(query5.First());
                setts.Volume = Convert.ToDouble(query6.First());
                setts.Profile = query7.First();
                setts.Png = (query8.First() == "true" ? true : false);
                setts.Jpg = (query9.First() == "true" ? true : false);
                setts.Jpeg = (query10.First() == "true" ? true : false);
                setts.Bmp = (query11.First() == "true" ? true : false);
                setts.Gif = (query12.First() == "true" ? true : false);
                setts.Mp4 = (query13.First() == "true" ? true : false);
                setts.Wmv = (query14.First() == "true" ? true : false);
                setts.Avi = (query15.First() == "true" ? true : false);
                setts.Mpg = (query16.First() == "true" ? true : false);
                setts.Mkv = (query17.First() == "true" ? true : false);
                setts.Mousedisappeardelay = Convert.ToInt32(query18.First());
                setts.VisibleNav = (query19.First() == "true" ? true : false);
                setts.BooruSource = Convert.ToInt32(query20.First());
                setts.BooruThreshold = Convert.ToInt32(query21.First());
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                setts.SettingsError = true;
            }           
            
            return setts;
            
        }

        public SaveSettings LoadProfile(string profilename)
        {
            SaveSettings setts = new SaveSettings
            {
                DBLocation = "",
                ViewCat = false,
                Shuffle = true,
                RepeatShuffle = false,
                Slideshowinterval = 3,
                Volume = 0.3,
                Png = true,
                Jpg = true,
                Jpeg = true,
                Bmp = true,
                Gif = true,
                Mp4 = true,
                Wmv = true,
                Avi = true,
                Mpg = true,
                Mkv = true,
                Mousedisappeardelay = 4,
                VisibleNav = true,
                BooruSource = 0,
                BooruThreshold = 60,
                SettingsError = false
            };

            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));

                var query = from item in profile.Elements() where item.Attributes().ElementAt(0).Value.ToString() == profilename select item.Elements().ElementAt(0).Value.ToString();  //dbname
                var query2 = from item2 in profile.Elements() where item2.Attributes().ElementAt(0).Value.ToString() == profilename select item2.Elements().ElementAt(1).Value.ToString(); //view cat
                var query3 = from item2 in profile.Elements() where item2.Attributes().ElementAt(0).Value.ToString() == profilename select item2.Elements().ElementAt(2).Value.ToString(); //shuffle
                var query4 = from item2 in profile.Elements() where item2.Attributes().ElementAt(0).Value.ToString() == profilename select item2.Elements().ElementAt(3).Value.ToString(); //repeat shuffle
                var query5 = from item2 in profile.Elements() where item2.Attributes().ElementAt(0).Value.ToString() == profilename select item2.Elements().ElementAt(4).Value.ToString(); //Slideshow interval
                var query6 = from item2 in profile.Elements() where item2.Attributes().ElementAt(0).Value.ToString() == profilename select item2.Elements().ElementAt(5).Value.ToString();  //volume
                var query7 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(6).Value.ToString(); //png
                var query8 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(7).Value.ToString(); //jpg
                var query9 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(8).Value.ToString(); //jpeg
                var query10 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(9).Value.ToString(); //bmp
                var query11 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(10).Value.ToString(); //gif
                var query12 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(11).Value.ToString(); //mp4
                var query13 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(12).Value.ToString(); //wmv
                var query14 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(13).Value.ToString(); //avi
                var query15 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(14).Value.ToString(); //mpg
                var query16 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(15).Value.ToString(); //mkv
                var query17 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(16).Value.ToString(); //mousedelay
                var query18 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(17).Value.ToString(); //visiblenav
                var query19 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(18).Value.ToString(); //BooruSource
                var query20 = from item2 in profile.Elements() where item2.Attributes().ElementAt(1).Value.ToString() == "true" select item2.Elements().ElementAt(19).Value.ToString(); //BooruThreshold

                setts.DBLocation = query.First();
                setts.ViewCat = (query2.First() == "true" ? true : false);
                setts.Shuffle = (query3.First() == "true" ? true : false);
                setts.RepeatShuffle = (query4.First() == "true" ? true : false);
                setts.Slideshowinterval = Convert.ToInt32(query5.First());
                setts.Volume = Convert.ToDouble(query6.First());
                setts.Profile = profilename;
                setts.Png = (query7.First() == "true" ? true : false);
                setts.Jpg = (query8.First() == "true" ? true : false);
                setts.Jpeg = (query9.First() == "true" ? true : false);
                setts.Bmp = (query10.First() == "true" ? true : false);
                setts.Gif = (query11.First() == "true" ? true : false);
                setts.Mp4 = (query12.First() == "true" ? true : false);
                setts.Wmv = (query13.First() == "true" ? true : false);
                setts.Avi = (query14.First() == "true" ? true : false);
                setts.Mpg = (query15.First() == "true" ? true : false);
                setts.Mkv = (query16.First() == "true" ? true : false);
                setts.Mousedisappeardelay = Convert.ToInt32(query17.First());
                setts.VisibleNav = (query18.First() == "true" ? true : false);
                setts.BooruSource = Convert.ToInt32(query19.First());
                setts.BooruThreshold = Convert.ToInt32(query20.First());
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                setts.SettingsError = true;
            }            

            return setts;
        }

        public bool ChangeDefaultprofile(string profilename)
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));
                var query = from item in profile.Elements() where item.Attributes().ElementAt(1).Value.ToString() == "true" select item;
                var query2 = from item in profile.Elements() where item.Attributes().ElementAt(0).Value.ToString() == profilename select item;

                foreach (XElement itemElement in query)
                {
                    itemElement.SetAttributeValue(DEFAULTFIELD, "false");
                }

                foreach (XElement itemElement in query2)
                {
                    itemElement.SetAttributeValue(DEFAULTFIELD, "true");
                }

                profile.Save(Path.Combine(XMLLocation, SETTINGSFILE));

                return true;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public bool AddProfile(string profilename)
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));

                XElement toadd = new XElement(PROFILEFIELD, new XAttribute(NAMEFIELD, profilename), new XAttribute(DEFAULTFIELD, "false"),
                            new XElement(DBNAMEFIELD, Path.Combine(XMLLocation, profilename + ".mdf")),
                            new XElement(VIEWCATFIELD, false),
                            new XElement(SHUFFLEFIELD, true),
                            new XElement(REPEATSHUFFLEFIELD, false),
                            new XElement(SSINTERVALFIELD, 3),
                            new XElement(VOLUMEFIELD, (0.3).ToString()),
                            new XElement(PNG, true),
                            new XElement(JPG, true),
                            new XElement(JPEG, true),
                            new XElement(BMP, true),
                            new XElement(GIF, true),
                            new XElement(MP4, true),
                            new XElement(WMV, true),
                            new XElement(AVI, true),
                            new XElement(MPG, true),
                            new XElement(MKV, true),
                            new XElement(MOUSEDELAY, 4),
                            new XElement(VISIBLENAV, true),
                            new XElement(BOORUSOURCE, 0),
                            new XElement(BOORUTHRESHOLD, 60)
                            );

                profile.Add(toadd);

                profile.Save(Path.Combine(XMLLocation, SETTINGSFILE));
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public List<string> GetProfiles()
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));

                var query = from item in profile.Elements() select item.Attribute(NAMEFIELD).Value;

                return query.ToList();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
            
        }

        public string GetDefaultProfile()
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));

                var query = from item in profile.Elements() where item.Attributes().ElementAt(1).Value.ToString() == "true" select item.Attribute(NAMEFIELD).Value;

                return query.First();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
            
        }

        public bool RenameProfile(string oldname, string newname)
        {
            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));
                var query = from item in profile.Elements() where item.Attributes().ElementAt(0).Value.ToString() == oldname select item;

                foreach (XElement itemElement in query)
                {
                    itemElement.SetAttributeValue(NAMEFIELD, newname);
                }

                profile.Save(Path.Combine(XMLLocation, SETTINGSFILE));

                return true;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public string DeleteProfile(string profilename)
        {
            string dblocale = "";

            try
            {
                XElement profile = XElement.Load(Path.Combine(XMLLocation, SETTINGSFILE));
                var query = from item in profile.Elements() where item.Attributes().ElementAt(0).Value.ToString() == profilename select item;

                foreach (XElement itemElement in query)
                {
                    dblocale = itemElement.Elements().ElementAt(0).Value.ToString();
                    itemElement.RemoveAll();
                    itemElement.Remove();
                }

                profile.Save(Path.Combine(XMLLocation, SETTINGSFILE));
                return dblocale;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }
    }
}
