using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Data.Repository
{
    public abstract class KeyValueRepository<T> : IKeyValueRepository<T> where T : DocumentEntity
    {
        //protected abstract string Url { get; }
            
        #region Get document
        /// <summary>
        /// To fetch object from database given the appropriate Url
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Get(string path)
        {
            string jsonDocument = HttpClient.Get(AppCommon.GetDocumentUrl(path));
            T result = JsonSerializer.DeserializeObject<T>(jsonDocument);
            return result;
        }
        /// <summary>
        /// Get document based on guidValue
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <param name="guidValue">Unique GUID value to fetch from repository</param>
        /// <returns></returns>
        public T Get(string path, string guidValue)
        {
            return Get(path + "/" + guidValue);
        }
        /// <summary>
        /// Get document list from the URL path
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <returns></returns>
        public List<T> GetAll(string path)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(path)));
            var resultList = JsonSerializer.DeserializeObject<Hashtable>(jsonString.ToString());
            List<T> finalResultList = new List<T>();
            if (resultList != null)
            {
                foreach (DictionaryEntry ucs in resultList)
                {
                    T templateValue = JsonSerializer.DeserializeObject<T>(ucs.Value.ToString());
                    templateValue.UniqueIdentifier = ucs.Key.ToString();
                    finalResultList.Add(templateValue);
                }
            }
            return finalResultList;
        }
        /// <summary>
        /// Get document list from the URL path
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <returns></returns>
        public string GetJsonDocument(string path)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(path)));
            string resultList = jsonString.ToString();
            return resultList;
        }
        #endregion

        #region Save or Update
        /// <summary>
        /// Insert/Update a document
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <param name="dataToSave">Dcoument to put in repository</param>
        /// <returns>Returns the URL path of the saved document</returns>
        public string SaveOrUpdate(string path, T dataToSave)
        {
            string jsonResult;
            return SaveOrUpdate(path, dataToSave, out jsonResult);
        }

        /// <summary>
        /// Insert/Update a document
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <param name="dataToSave">Dcoument to put in repository</param>
        /// <param name="outputJson"> </param>
        /// <returns>Returns the URL path of the saved document</returns>
        public string SaveOrUpdate(string path, T dataToSave, out string outputJson)
        {
            string urlPath = string.Format(path, dataToSave.GetNewGuidValue());
            dataToSave.Url = urlPath;
            outputJson = HttpClient.Put(AppCommon.GetDocumentUrl(urlPath), JsonSerializer.SerializeObject(dataToSave));
            return urlPath;
        }
        /// <summary>
        /// Insert/Update a sepcific key within document
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataToSave"></param>
        /// <returns></returns>
        public string SaveOrUpdate(string path, Dictionary<string, object> dataToSave)
        {
            //HttpClient.Put(AppCommon.GetDocumentUrl(path), JsonSerializer.SerializeObject(dataToSave));
            string jsonResult;
            return SaveOrUpdate(path, dataToSave, out jsonResult);
        }
        public string SaveOrUpdate(string path, Dictionary<string, object> dataToSave, out string outputJson)
        {
            outputJson = HttpClient.Put(AppCommon.GetDocumentUrl(path), JsonSerializer.SerializeObject(dataToSave));
            return path;
        }
        public string SaveOrUpdate(string path, Dictionary<string, T> dataToSave)
        {
            string jsonResult;
            return SaveOrUpdate(path, dataToSave, out jsonResult);
        }
        public string SaveOrUpdate(string path, Dictionary<string, T> dataToSave, out string outputJson)
        {
            outputJson = HttpClient.Put(AppCommon.GetDocumentUrl(path), JsonSerializer.SerializeObject(dataToSave));
            return path;
        }
        public string SaveOrUpdate(T dataToSave)
        {
            string jsonResult;
            return SaveOrUpdate(dataToSave, out jsonResult);
        }
        public string SaveOrUpdate(T dataToSave, out string outputJson)
        {
            string urlPath = "";
            outputJson = "";
            if (dataToSave.Url != null || dataToSave.UniqueIdentifier != null)
            {
                urlPath = dataToSave.Url;
                outputJson = HttpClient.Put(AppCommon.GetDocumentUrl(urlPath), JsonSerializer.SerializeObject(dataToSave));
            }
            return urlPath;
        }
        #endregion

        #region Delete action

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="guidValue"></param>
        /// <param name="deleteResult"> </param>
        /// <returns></returns>
        public bool Delete(string path, string guidValue, out string deleteResult)
        {
            //string jsonString = HttpClient.Delete(AppCommon.GetDocumentUrl(path + "/" + guidValue));
            return Delete(path + "/" + guidValue, out deleteResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <param name="guidValue">Unique GUID value to the node</param>
        /// <param name="keyToDelete">Key value to be deleted</param>
        /// <param name="deleteResult"> </param>
        /// <returns></returns>
        public bool Delete(string path, string guidValue, string keyToDelete, out string deleteResult)
        {
            return Delete(path + "/" + guidValue + "/" + keyToDelete, out deleteResult);
        }

        /// <summary>
        /// Delete the document from the database
        /// </summary>
        /// <param name="path">URL path value</param>
        /// <param name="deleteResult"> </param>
        /// <returns></returns>
        public bool Delete(string path, out string deleteResult)
        {
            deleteResult = HttpClient.Delete(AppCommon.GetDocumentUrl(path));
            return true;
        }
        #endregion

        #region Object Transition
        /// <summary>
        /// To convert source to destination type of object and return the destination object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void ObjectTransformation(object source, object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var targetParameter = Expression.Parameter(typeof(object), "target");

            var sourceVariable = Expression.Variable(sourceType, "castedSource");
            var targetVariable = Expression.Variable(targetType, "castedTarget");

            var expressions = new List<Expression>
                                  {
                                      Expression.Assign(sourceVariable, Expression.Convert(sourceParameter, sourceType)),
                                      Expression.Assign(targetVariable, Expression.Convert(targetParameter, targetType))
                                  };

            foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead)
                    continue;

                var targetProperty = targetType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetProperty != null
                        && targetProperty.CanWrite
                        && targetProperty.PropertyType.IsAssignableFrom(property.PropertyType))
                {
                    expressions.Add(
                        Expression.Assign(
                            Expression.Property(targetVariable, targetProperty),
                            Expression.Convert(
                                Expression.Property(sourceVariable, property), targetProperty.PropertyType)));
                }
            }

            var lambda =
                Expression.Lambda<Action<object, object>>(
                    Expression.Block(new[] { sourceVariable, targetVariable }, expressions),
                    new[] { sourceParameter, targetParameter });

            var del = lambda.Compile();

            del(source, target);
        }
        /// <summary>
        /// To transfer value of each property in source object to destination object.
        /// IF source and destination properties match, all values will be copied as it is.
        /// IF NOT, the properties of destination which doesn't have matching member with that of the source
        /// will be made as null.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget ObjectTransfer<TSource, TTarget>(TSource source)
            where TTarget : DocumentEntity, new()
        {
            var target = new TTarget();
            ObjectTransformation(source, target);
            return target;
        }
        #endregion Object Transition

        #region Move
        public string Move(string sourcePath, string destPath)
        {
            return HttpClient.Put(AppCommon.GetDocumentUrl(destPath), GetJsonDocument(AppCommon.GetDocumentUrl(sourcePath)));
        }
        #endregion

        public string SaveOrUpdate(string path, Dictionary<string, Dictionary<string, Core.Competency.Competency>> dataToSave)
        {
            return HttpClient.Put(AppCommon.GetDocumentUrl(path), JsonSerializer.SerializeObject(dataToSave));
        }

        public string SaveOrUpdate(string path, string dataToSave)
        {
            return HttpClient.Put(AppCommon.GetDocumentUrl(path), dataToSave);
        }
        //public string FormatUrlBasedOnDropbox(DropBoxLink dropbox)
        //{
        //    return string.Format("SimApp/Courses/{0}/{1}/{2}/Assignments/{3}", dropbox.CID, dropbox.UserRole, dropbox.UID, dropbox.SID);
        //}

        public string GetAssignmentUrl(DocumentPath.Module docPath, string customValue = "")
        {
            DropBoxLink dropBox = new DropBoxLink
                                      {Cid = AppConstants.AdminCourseId, UserRole = AppConstants.AdminRole};
            return GetAssignmentUrl(dropBox, docPath,customValue);
        }
        public string GetAssignmentUrl(DropBoxLink dropBox,DocumentPath.Module docPath,string customValue="")
        {
            //return AppCommon.FormatAssignmentUrl(dropBox.CID, AppCommon.GetCurrentUserRole(dropBox.UserRole), dropBox.UID, dropBox.SID) +"/"+ this.Url;// "/{0}/{1}/{2}";
            return Respository.GetDocumentPath(docPath, dropBox, customValue);
        }
        public string GetAssignmentUrl(DropBoxLink dropBox, DocumentPath.Module docPath)
        {
            //return AppCommon.FormatAssignmentUrl(dropBox.CID, AppCommon.GetCurrentUserRole(dropBox.UserRole), dropBox.UID, dropBox.SID) +"/"+ this.Url;// "/{0}/{1}/{2}";
            return Respository.GetDocumentPath(docPath, dropBox, "");
        }
        //public string GetAssignmentUrl()
        //{
        //    return "";
        //}
        protected DropBoxLink GetAdminDropBox()
        {
            DropBoxLink dropBox = new DropBoxLink
                                      {Cid = AppConstants.AdminCourseId, UserRole = AppConstants.AdminRole};
            return dropBox;
        }
    }
}
