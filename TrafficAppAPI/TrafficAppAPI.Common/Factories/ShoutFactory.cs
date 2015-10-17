using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;

namespace TrafficAppAPI.Common.Factories
{
    public class ShoutFactory
    {
        public List<string> CreateListFromCommaSeperatedString(string fields)
        {
            try
            {
                List<string> fieldList = new List<string>();
                fieldList = fields.ToLower().Split(',').ToList();
                return fieldList;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public object CreateDataShapedObject(Shout shout, string lstOfFields)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lstOfFields))
                {
                    return shout;
                }
                else
                {
                    var fieldList = CreateListFromCommaSeperatedString(lstOfFields);

                    // create a new ExpandoObject & dynamically create the properties for this object
                    ExpandoObject objectToReturn = new ExpandoObject();
                    foreach (var field in fieldList)
                    {
                        // need to include public and instance, b/c specifying a binding flag overwrites the
                        // already-existing binding flags.

                        var fieldValue = shout.GetType()
                            .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(shout, null);

                        // add the field to the ExpandoObject
                        ((IDictionary<String, Object>)objectToReturn).Add(field, fieldValue);
                    }

                    return objectToReturn;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
