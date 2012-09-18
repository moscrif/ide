using System;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Moscrif.IDE.Devices
{
	public class PublishPropertyJavaScriptConverter : JavaScriptConverter
	{
	    private static readonly Type[] _supportedTypes = new[]
	    {
	        typeof( PublishProperty )
	    };

	    public override IEnumerable<Type> SupportedTypes
	    { 
	        get { return _supportedTypes; } 
	    }
	
	    public override object Deserialize( IDictionary<string, object> dictionary,Type type,JavaScriptSerializer serializer )
	    {
	        if( type == typeof( PublishProperty ) )
	        {
	            var obj = new PublishProperty();
	            if( dictionary.ContainsKey( "name" ) )
	                obj.PublishName = serializer.ConvertToType<string>(dictionary["name"] );
	            if( dictionary.ContainsKey( "value" ) )
	                obj.PublishValue = serializer.ConvertToType<string>(dictionary["value"] );
	
	            return obj;
	        }
	
	        return null;
	    }
	
	    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer )
	    {
	        var dataObj = obj as PublishProperty;
	        if( dataObj != null )
	        {
		   //Console.WriteLine(" dataObj.PublishValue -> " + dataObj.PublishValue);

	            return new Dictionary<string,object>
	            {
	                {"name", dataObj.PublishName==null?"":dataObj.PublishName },
	                {"value", dataObj.PublishValue==null?"":dataObj.PublishValue }
	            };
	        }
	        return new Dictionary<string, object>();
	    }
	}
}

