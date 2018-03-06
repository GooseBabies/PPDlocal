using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;

namespace Tagger
{
    public class DatabaseUtil
    {        
        [Table(Name = "ImageData")]
        public class ImageDataTableDef
        {

            [Column(IsPrimaryKey = true)]
            public string Filename;
            [Column]
            public string Directory;
            [Column]
            public short Rating;
            [Column]
            public short ImageTagCount;
            [Column]
            public string Tags;
            [Column]
            public short Width;
            [Column]
            public short Height;
            [Column]
            public DateTime CreationTime;
            [Column]
            public string Filetype;
            [Column]
            public long Filesize;
            [Column]
            public string BooruURL;
            [Column]
            public bool BetterOnBooru;
            [Column]
            public int BooruSimilarityScore;

        }

        [Table(Name = "Tags")]
        public class TagsTableDef
        {

            [Column(IsPrimaryKey = true)]
            public string Tag;
            [Column]
            public string Category;
            [Column]
            public short Count;

        }

        [Table(Name = "SaveInfo")]
        public class SaveInfoTableDef
        {
            [Column(IsPrimaryKey = true)]
            public int Id;
            [Column]
            public string ProfileName;
            [Column]
            public DateTime Savetime;
            [Column]
            public string Directory;
            [Column]
            public string FileName;
        }

        public class DBTable : DataContext
        {
            public Table<ImageDataTableDef> ImageData;
            public Table<TagsTableDef> Tags;
            public Table<SaveInfoTableDef> SaveInfo;
            public DBTable(string connection) : base(connection) { }
        }

        ErrorHandling Error = new ErrorHandling();

        public DBTable ConnectToDatabase(string dbfile)
        {
            try
            {
                DBTable db = new DBTable(dbfile);
                return db;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
            
        }

        public bool DisconnectFromDatabase(DBTable db)
        {
            try
            {

                SqlConnection.ClearAllPools();
                db.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }
        public bool DeleteDatabase(DBTable db)
        {
            try
            {
                SqlConnection.ClearAllPools();
                db.DeleteDatabase();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool CreateDatabase(string dblocation, string dbname)
        {

            string DBFileExtsension = ".mdf";
            string DBFile = Path.Combine(dblocation.ApostrepheFix(), dbname.ApostrepheFix() + DBFileExtsension);
            DBTable db = new DBTable(DBFile);

            try
            {
                db.CreateDatabase();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            

        }

        public bool AddtoImageData(DBTable db, string filename, string directory, short rating, string tagadd, short tagcount, short width, short height, DateTime creationtime, string filetype, long filesize)
        {
            string tag = ";" + tagadd.ApostrepheFix() + ";";

            ImageDataTableDef ImageData = new ImageDataTableDef
            {
                Filename = filename.ApostrepheFix(),
                Directory = directory.ApostrepheFix(),
                Rating = rating,
                Tags = tag,
                ImageTagCount = tagcount,
                Width = width,
                Height = height,
                CreationTime = creationtime,
                Filetype = filetype.ApostrepheFix(),
                Filesize = filesize,
            };

            try
            {
                db.ImageData.InsertOnSubmit(ImageData);
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public bool AddTagtoImageData(DBTable db, string filename, string tag)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    foreach (ImageDataTableDef ImageData in query)
                    {
                        ImageData.Tags = ImageData.Tags + tag.ApostrepheFix() + ";";
                        ImageData.ImageTagCount++;
                    }

                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool UpdateTaginImageData(DBTable db, string filename, string tagstring)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    foreach (ImageDataTableDef ImageData in query)
                    {
                        ImageData.Tags = tagstring.ApostrepheFix();
                    }

                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool RemoveImageTag(DBTable db, string tag, string filename)
        {            
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    foreach (ImageDataTableDef ImageData in query)
                    {
                        ImageData.Tags = ImageData.Tags.Replace(tag + ";", "");
                    }
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool AddtoTags(DBTable db, string tag, string category)
        {

            TagsTableDef Tags = new TagsTableDef
            {
                Tag = tag.ApostrepheFix(),
                Count = 1,
                Category = category.ApostrepheFix()
            };

            try
            {
                db.Tags.InsertOnSubmit(Tags);
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool AddtoTags(DBTable db, string tag)
        {
            TagsTableDef Tags = new TagsTableDef
            {
                Tag = tag.ApostrepheFix(),
                Count = 1
            };

            try
            {
                db.Tags.InsertOnSubmit(Tags);
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool UpdateTagsCount(DBTable db, string tag, bool increment)
        {
            try
            {
                var query = from Tags in db.Tags where Tags.Tag == tag.ApostrepheFix() select Tags;

                if(query.Count() > 0)
                {
                    foreach (TagsTableDef Tags in query)
                    {
                        if (increment)
                        {
                            Tags.Count++;
                        }
                        else
                        {
                            Tags.Count--;
                        }

                    }
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool UpdateRating(DBTable db, string filename, short rating)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    foreach (ImageDataTableDef ImageData in query)
                    {
                        ImageData.Rating = rating;
                    }
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool UpdateBooruInfo(DBTable db, string filename, string URL, bool better, int score)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    foreach (ImageDataTableDef ImageData in query)
                    {
                        ImageData.BooruURL = URL;
                        ImageData.BetterOnBooru = better;
                        ImageData.BooruSimilarityScore = score;
                    }
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool DecrementImageDataTagCount(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    foreach (ImageDataTableDef ImageData in query)
                    {
                        ImageData.ImageTagCount--;
                    }
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool DeleteImageData(DBTable db, string filename)
        {
            List<string> results;
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query.Count() > 0)
                {
                    results = GetImageTags(db, filename);
                    if(results != null)
                    {
                        foreach (string tags in results)
                        {
                            UpdateTagsCount(db, tags, false);
                        }
                    }                    

                    foreach (var row in query)
                    {
                        db.ImageData.DeleteOnSubmit(row);
                    }
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public bool DeleteTag(DBTable db, string tag)
        {
            try
            {
                var query = from Tags in db.Tags where Tags.Tag == tag.ApostrepheFix() select Tags;
                
                if(query.Count() > 0)
                {
                    foreach (var row in query)
                    {
                        db.Tags.DeleteOnSubmit(row);
                    }
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public List<string> SearchForFiles(DBTable db, string Tag)
        {
            try
            {
                var query = db.ImageData.Where(x => x.Tags.Contains(";" + Tag.ApostrepheFix() + ";")).OrderByDescending(r => r.Rating).ThenBy(j => j.Filename).Select(t => t.Filename.ApostrepheDefix());

                if (query.Count() > 0)
                {
                    return query.ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }        

        public List<string> BooleanSearch(DBTable db, List<string[]> Ors, List<string> Ands, List<string> Nots)
        {
            string query = "select * from dbo.ImageData where ";
            string OrSequence = "(";
            List<string> oars = new List<string>();

            try
            {
                foreach (string[] j in Ors)
                {
                    for (int y = 0; y <= j.Length - 1; y++)
                    {
                        if (y == 0)
                        {
                            OrSequence += "Tags like '%;" + j[y] + ";%' ";
                        }
                        else
                        {
                            OrSequence += "or Tags like '%;" + j[y] + ";%' ";
                        }
                    }
                    OrSequence += ") ";
                    oars.Add(OrSequence);
                }

                foreach (string a in Ands)
                {
                    if (query.Length > 36)
                    {
                        query += "and Tags like '%;" + a + ";%' ";
                    }
                    else
                    {
                        query += "Tags like '%;" + a + ";%' ";
                    }
                }

                foreach (string b in oars)
                {
                    if (query.Length > 36)
                    {
                        query += "and " + OrSequence + " ";
                    }
                    else
                    {
                        query += OrSequence + " ";
                    }
                }

                foreach (string c in Nots)
                {
                    if (query.Length > 36)
                    {
                        query += "and not like '%;" + c + ";%' ";
                    }
                    else
                    {
                        query += "not like '%;" + c + ";%' ";
                    }
                }

                return db.ExecuteQuery<ImageDataTableDef>(query).OrderByDescending(r => r.Rating).ThenBy(j => j.Filename).Select(x => x.Filename.ApostrepheDefix()).ToList();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }                      
        }

        public List<string> MainSearch(DBTable db, string searchstring)
        {
            return db.ExecuteQuery<ImageDataTableDef>(searchstring).OrderByDescending(r => r.Rating).ThenBy(j => j.Filename).Select(x => x.Filename.ApostrepheDefix()).ToList();
        }

        public List<string> GetTagsContaining(DBTable db, string chars, int limit)
        {
            try
            {
                var query = from Tags in db.Tags where Tags.Tag.Contains(chars.ApostrepheFix()) select Tags.Tag.ApostrepheDefix();

                if(query.Count() > 0)
                {
                    return query.Take(limit).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public short GetImageTagCount(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.ImageTagCount;

                if(query.Count() > 0)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return -1;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public short GetImageHeight(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Height;

                if(query.Count() > 0)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return -1;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public short GetImageWidth(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Width;

                if(query.Count() > 0)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return -1;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public int GetImageArea(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select new { ImageData.Height, ImageData.Width };

                if(query.Count() > 0)
                {
                    return query.FirstOrDefault().Height * query.FirstOrDefault().Width;
                }
                else
                {
                    return -1;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public List<string> GetImageTags(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Tags.ApostrepheDefix();

                if (query.FirstOrDefault() == null || query.FirstOrDefault() == "" || query.Count() < 1)
                {
                    return new List<string>();
                }
                else
                {
                    return query.FirstOrDefault().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
            
        }

        public List<string> GetAllTags(DBTable db)
        {
            try
            {
                var query = from Tags in db.Tags select Tags.Tag.ApostrepheDefix();

                if (query.Count() < 1)
                {
                    return new List<string>();
                }
                else
                {
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public short GetRating(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Rating;

                if(query.Count() > 0)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return -1;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public string GetSearchString(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Tags.ApostrepheDefix();

                if(query.Count() > 0)
                {
                    return query.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
            
        }

        public bool TagExists(DBTable db, string tag)
        {
            try
            {
                var query = from Tags in db.Tags where Tags.Tag == tag.ApostrepheFix() select Tags.Tag;

                if (query.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public bool ImageDataExists(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if(query != null)
                {
                    if(query.Count() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public int GetTotalTagCount(DBTable db)
        {
            try
            {
                var query = from Tags in db.Tags select Tags.Tag;

                return query.Count();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public int GetIndividualTagCount(DBTable db, string tag)
        {
            try
            {
                var query = from Tags in db.Tags where Tags.Tag == tag.ApostrepheFix() select Tags.Count;

                if (query.Count() < 1)
                {
                    return -1;
                }
                else
                {
                    return query.First();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public bool? IsVideo(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Filetype;

                if (query.Count() < 0)
                {
                    return false;
                }
                else
                {
                    if (query.First() == ".mp4" || query.First() == ".mkv" || query.First() == ".mpg" || query.First() == ".wmv" || query.First() == ".avi")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public bool SaveState(DBTable db, string profilename, string directory, string filename)
        {
            int id = db.SaveInfo.Count() + 1;
            bool update;                     

            try
            {
                var query = from SaveInfo in db.SaveInfo where SaveInfo.ProfileName == profilename & SaveInfo.Directory == directory select SaveInfo;

                update = (query.Count() > 0 ? true : false);
                
                if (update)
                {
                    foreach (SaveInfoTableDef infos in query)
                    {
                        infos.ProfileName = profilename.ApostrepheFix();
                        infos.Savetime = DateTime.Now;
                        infos.Directory = directory.ApostrepheFix();
                        infos.FileName = filename.ApostrepheFix();
                    }
                }
                else
                {
                    SaveInfoTableDef Saveinfo = new SaveInfoTableDef
                    {
                        Id = id,
                        ProfileName = profilename.ApostrepheFix(),
                        Savetime = DateTime.Now,
                        Directory = directory.ApostrepheFix(),
                        FileName = filename,
                    };
                    db.SaveInfo.InsertOnSubmit(Saveinfo);
                }
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
        }

        public IQueryable<SaveInfoTableDef> LoadState(DBTable db, string profilename)
        {
            try
            {
                var query = from SaveInfo in db.SaveInfo where SaveInfo.ProfileName == profilename.ApostrepheFix() select SaveInfo;
                query = query.OrderByDescending(x => x.Savetime);
                return query;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public string LoadDirectory(DBTable db, string profilename, string directory)
        {
            try
            {
                var query = from Saveinfo in db.SaveInfo where Saveinfo.ProfileName == profilename.ApostrepheFix() & Saveinfo.Directory == directory.ApostrepheFix() select Saveinfo.FileName.ApostrepheDefix();

                if (query.Count() < 1)
                {
                    return null;
                }
                else
                {
                    return query.First();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public List<string> GetDirectories(DBTable db, string profilename)
        {
            try
            {
                var query = from SaveInfo in db.SaveInfo where SaveInfo.ProfileName == profilename.ApostrepheFix() select SaveInfo.Directory.ApostrepheDefix();

                if(query.Count() > 0)
                {
                    return query.ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public string GetImageDirectory(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.Directory.ApostrepheDefix();

                if (query.Count() > 0)
                {
                    return query.First();
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }
        
        public List<string> GetProfiles(DBTable db)
        {
            try
            {
                var query = from SaveInfo in db.SaveInfo select SaveInfo.ProfileName.ApostrepheDefix();

                if(query.Count() > 0)
                {
                    query = query.Distinct();

                    return query.ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public int GetImageTaggedCount(DBTable db, string directory)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Directory == directory.ApostrepheFix() select ImageData;

                return query.Count();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }            
        }

        public string GetBooruURL(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.BooruURL.ApostrepheDefix();

                if (query.Count() < 1)
                {
                    return null;
                }
                else if (query.First() == null)
                {
                    return null;
                }
                else
                {
                    return query.First();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public bool GetBetteronBooru(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.BetterOnBooru;

                if (query.Count() < 1)
                {
                    return false;
                }
                else
                {
                    return query.First();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public int GetBooruScore(DBTable db, string filename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData.BooruSimilarityScore;

                if (query.Count() < 1)
                {
                    return -1;
                }
                else
                {
                    return query.First();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return -1;
            }
            
        }

        public bool Rename(DBTable db, string filename, string newfilename)
        {
            try
            {
                var query = from ImageData in db.ImageData where ImageData.Filename == filename.ApostrepheFix() select ImageData;

                if (query.Count() > 0)
                {
                    foreach (ImageDataTableDef row in query)
                    {
                        ImageDataTableDef ImageData = new ImageDataTableDef
                        {
                            Filename = newfilename.ApostrepheFix(),
                            Directory = row.Directory.ApostrepheFix(),
                            Rating = row.Rating,
                            ImageTagCount = row.ImageTagCount,
                            Tags = row.Tags,
                            Width = row.Width,
                            Height = row.Height,
                            CreationTime = row.CreationTime,
                            Filetype = row.Filetype.ApostrepheFix(),
                            Filesize = row.Filesize,
                            BetterOnBooru = row.BetterOnBooru,
                            BooruSimilarityScore = row.BooruSimilarityScore,
                            BooruURL = row.BooruURL
                        };
                        db.ImageData.InsertOnSubmit(ImageData);
                        db.ImageData.DeleteOnSubmit(row);
                    }
                }
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }
            
        }

        public bool? ExportImageData(DBTable db, string file)
        {
            string data = "ImageData" + Environment.NewLine;
            string delimiter = "||";

            try
            {
                var query = from ImageData in db.ImageData select ImageData;

                if (query.Count() > 0)
                {
                    foreach (ImageDataTableDef row in query)
                    {
                        data += row.Directory + delimiter + row.Filename + delimiter + row.Rating + delimiter + row.ImageTagCount + delimiter + row.Tags + delimiter + row.Filetype + delimiter + row.Filesize + delimiter + row.Height + delimiter + row.Width + delimiter + row.CreationTime + delimiter + row.BooruURL + delimiter + row.BooruSimilarityScore + delimiter + row.BetterOnBooru + Environment.NewLine;
                    }
                    using (StreamWriter writestuff = File.AppendText(file))
                    {
                        writestuff.WriteLine(data);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public bool? ExportTags(DBTable db, string file)
        {
            string data = "Tags" + Environment.NewLine;
            string delimiter = "||";

            try
            {
                var query = from Tags in db.Tags select Tags;

                if (query.Count() > 0)
                {
                    foreach (TagsTableDef row in query)
                    {
                        data += row.Tag + delimiter + row.Count + delimiter + row.Category + Environment.NewLine;
                    }
                    using (StreamWriter writestuff = File.AppendText(file))
                    {
                        writestuff.WriteLine(data);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }            
        }

        public bool? ExportDatabase(DBTable db)
        {
            bool? ImageDataExport = false;
            bool? TagsExport = false;
            try
            {
                string SaveFile;
                var SaveFileDialog = new Microsoft.Win32.SaveFileDialog() { DefaultExt = ".txt", Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" };
                if(SaveFileDialog.ShowDialog() == true)
                {
                    SaveFile = SaveFileDialog.FileName;                    
                    if (!File.Exists(SaveFile))
                    {
                        File.Create(SaveFile).Close();
                    }
                    else
                    {
                        File.Delete(SaveFile);
                        File.Create(SaveFile).Close();
                    }
                    ImageDataExport = ExportImageData(db, SaveFile);
                    TagsExport = ExportTags(db, SaveFile);
                    if (ImageDataExport == null || TagsExport == null)
                    {
                        throw new Exception("Error Exporting Tags");
                    }
                    return true;
                }
                else
                {
                    return false;
                }               
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return false;
            }            
        }

        public bool? ImportDatabase(DBTable db, List<string[]> DataImport, int[] TableStart, string NewDirectory)
        {
            try
            {
                //ImageInfo
                List<ImageDataTableDef> ImageDataRows = new List<ImageDataTableDef>();
                for (int a = TableStart[0] + 1; a < TableStart[1]; a++)
                {
                    ImageDataTableDef ImageData = new ImageDataTableDef
                    {
                        Directory = NewDirectory,
                        Filename = DataImport[a][1],
                        Rating = Convert.ToInt16(DataImport[a][2]),
                        ImageTagCount = Convert.ToInt16(DataImport[a][3]),
                        Tags = DataImport[a][4],
                        Filetype = DataImport[a][5],
                        Filesize = Convert.ToInt64(DataImport[a][6]),
                        Height = Convert.ToInt16(DataImport[a][7]),
                        Width = Convert.ToInt16(DataImport[a][8]),
                        CreationTime = Convert.ToDateTime(DataImport[a][9]),
                        BooruURL = DataImport[a][10],
                        BooruSimilarityScore = Convert.ToInt32(DataImport[a][11]),
                        BetterOnBooru = Convert.ToBoolean(DataImport[a][12])
                    };
                    ImageDataRows.Add(ImageData);
                }

                ////ImageTags
                //List<ImageTagsTableDef> ImageTagsRows = new List<ImageTagsTableDef>();
                //for (int b = TableStart[1] + 1; b < TableStart[2]; b++)
                //{
                //    ImageTagsTableDef ImageTags = new ImageTagsTableDef
                //    {
                //        Directory = NewDirectory,
                //        filename = DataImport[b][1],
                //        rating = Convert.ToInt16(DataImport[b][2]),
                //        ImageTagCount = Convert.ToInt16(DataImport[b][3]),
                //        Tags = DataImport[b][4]
                //    };
                //    ImageTagsRows.Add(ImageTags);
                //}

                List<TagsTableDef> TagsRows = new List<TagsTableDef>();
                for (int c = TableStart[1] + 1; c <= DataImport.Count - 1; c++)
                {
                    TagsTableDef Tags = new TagsTableDef
                    {
                        Tag = DataImport[c][0],
                        Count = Convert.ToInt16(DataImport[c][1]),
                        Category = DataImport[c][2]
                    };
                    TagsRows.Add(Tags);
                }

                db.ImageData.InsertAllOnSubmit(ImageDataRows);
                //db.ImageTags.InsertAllOnSubmit(ImageTagsRows);
                db.Tags.InsertAllOnSubmit(TagsRows);
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
        }

        public bool? Flush(DBTable db)
        {
            try
            {
                var query = from Tags in db.Tags where Tags.Count == 0 select Tags;
                var query2 = from ImageInfo in db.ImageData select ImageInfo;

                if(query.Count() > 0)
                {
                    foreach (var row in query)
                    {
                        db.Tags.DeleteOnSubmit(row);
                    }
                }
                
                if(query2.Count() > 0)
                {
                    foreach (var row in query2)
                    {
                        if (!File.Exists(Path.Combine(row.Directory, row.Filename)))
                        {
                            db.ImageData.DeleteOnSubmit(row);
                        }
                    }
                }
                
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
                return null;
            }
        }
    }

    public static class StringHandleApostrephe
    {
        public static string ApostrepheFix(this string bad)
        {
            if (bad != null && bad != "")
            {
                return bad.Replace(@"'", "''");
            }
            else return bad;
        }

        public static string ApostrepheDefix(this string better)
        {
            if (better != null && better != "")
            {
                return better.Replace("''", @"'");
            }
            else return better;
        }
    }
}
