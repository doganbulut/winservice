using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Winservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RPCController : ControllerBase
    {
        protected readonly ILogger<RPCController> logger;

        public RPCController(ILogger<RPCController> logger)
        {
            this.logger = logger;
        }

        bool getDBName(string table,out string databasename)
        {
            try
            {
                var rec = DBHelper.Instance.Tables.FirstOrDefault(p=>p.Key.StartsWith(table));
                
                databasename = DBHelper.Instance.Tables.FirstOrDefault(p=>p.Key.StartsWith(table)).Value;
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("checkTable({table})", table), ex);
                databasename="";
                return false;
            }
        }


        [HttpGet("getall/{table}")]  // GET /api/getall/serie
        public String GetAll(string table)
        {
            BsonArray result = new BsonArray();
            try
            {
                string databasename ="";
                if (!getDBName(table,out databasename))
                    return result.ToString();

                using (var db = new LiteDatabase(databasename))
                {
                    var dbc = db.GetCollection(table);
                    var datas = dbc.FindAll();
                    foreach (var data in datas)
                        result.Add(data);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("GetAll({table})", table), ex);
                return result.ToString();
            }

            return result.ToString();
        }

        [HttpGet("getbyfieldname/{table}&{field}&{value}")]  // GET api/Base/getbyfield/serie&name&value
        public string GetbyFieldName(string table, string field, string value)
        {
            try
            {
                 string databasename ="";
                if (!getDBName(table,out databasename))
                    return "";

                using (var db = new LiteDatabase(databasename))
                {
                    var dbc = db.GetCollection(table);
                    var data = dbc.FindOne("$." + field + " = " + "'" + value + "'");
                    return JsonSerializer.Serialize(data);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("GetbyFieldName({table}&{field}&{value})", table, field, value), ex);
                return "";
            }
        }


        [HttpPost]
        public bool Post(string table, string jsondata)
        {
            try
            {
                if (string.IsNullOrEmpty(jsondata))
                    return false;
                
                string databasename ="";
                if (!getDBName(table,out databasename))
                    return false;

                BsonValue bson = JsonSerializer.Deserialize(jsondata);
                using (var db = new LiteDatabase(databasename))
                {
                    var dbc = db.GetCollection(table);
                    dbc.Insert(bson.AsDocument);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Post({table}&{jsondata})", table, jsondata), ex);
                return false;
            }
        }

        [HttpPut]
        public bool Put(string table, string field, string value, string jsondata)
        {
            try
            {
                string databasename ="";
                if (!getDBName(table,out databasename))
                    return false;

                using (var db = new LiteDatabase(databasename))
                {
                    var dbc = db.GetCollection(table);
                    var data = dbc.FindOne("$." + field + " = " + "'" + value + "'");
                    if (data != null)
                    {
                        try
                        {
                            BsonValue _id;
                            if (data.TryGetValue("_id", out _id))
                            {
                                bool updated = dbc.Update(_id, JsonSerializer.Deserialize(jsondata).AsDocument);
                                logger.LogInformation("Updated: " + updated.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(string.Format("Put({table}&{jsondata})", table, jsondata), ex);
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Put({table}&{jsondata})", table, jsondata), ex);
                return false;
            }
        }

        [HttpDelete]
        public bool Delete(string table, string field, string value, string jsondata)
        {
            try
            {
                string databasename ="";
                if (!getDBName(table,out databasename))
                    return false;

                using (var db = new LiteDatabase(databasename))
                {
                    var dbc = db.GetCollection(table);
                    var data = dbc.FindOne("$." + field + " = " + "'" + value + "'");
                    if (data != null)
                    {
                        try
                        {
                            BsonValue _id;
                            if (data.TryGetValue("_id", out _id))
                            {
                                bool deleted = dbc.Delete(_id);
                                logger.LogInformation("Deleted: " + deleted.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(string.Format("Delete({table}&{jsondata})", table, jsondata), ex);
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Delete({table}&{jsondata})", table, jsondata), ex);
                return false;
            }
        }
    }
}