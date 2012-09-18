using System;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Moscrif.IDE.Devices
{
	public class ConditionJavaScriptConverter : JavaScriptConverter
	{
	    private static readonly Type[] _supportedTypes = new[]
	    {
	        typeof( ConditionDevice )
	    };

	    public override IEnumerable<Type> SupportedTypes
	    { 
	        get { return _supportedTypes; } 
	    }
	
	    public override object Deserialize( IDictionary<string, object> dictionary,Type type,JavaScriptSerializer serializer )
	    {
	        if( type == typeof( ConditionDevice ) )
	        {
	            var obj = new ConditionDevice();
	            if( dictionary.ContainsKey( "name" ) )
	                obj.Name = serializer.ConvertToType<string>(dictionary["name"] );
	            if( dictionary.ContainsKey( "value" ) )
	                obj.Value = serializer.ConvertToType<string>(dictionary["value"] );

	            return obj;
	        }
	
	        return null;
	    }
	
	    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer )
	    {
	        var dataObj = obj as ConditionDevice;
	        if( dataObj != null )
	        {
		  Dictionary<string,object> dct =new Dictionary<string,object>();
				dct.Add("name", dataObj.Name==null?"":dataObj.Name);
				dct.Add("value", dataObj.Value==null?"":dataObj.Value);
				if(dataObj.Height > -1){
					dct.Add("height", dataObj.Height);
					dct.Add("width", dataObj.Width);
				}
				return dct;
	          /*  return new Dictionary<string,object>
	            {
	                {"name", dataObj.Name==null?"":dataObj.Name },
	                {"value", dataObj.Value==null?"":dataObj.Value }
	            };*/
	        }
	        return new Dictionary<string, object>();
	    }
	}
}

