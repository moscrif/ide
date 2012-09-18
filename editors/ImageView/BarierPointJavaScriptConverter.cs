using System;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Moscrif.IDE.Editors.ImageView
{
	public class BarierPointJavaScriptConverter : JavaScriptConverter
	{
	    private static readonly Type[] _supportedTypes = new[]
	    {
	        typeof( BarierPoint )
	    };
	
	    public override IEnumerable<Type> SupportedTypes
	    { 
	        get { return _supportedTypes; } 
	    }
	
	    public override object Deserialize( IDictionary<string, object> dictionary,Type type,JavaScriptSerializer serializer )
	    {
	        if( type == typeof( BarierPoint ) )
	        {
	            var obj = new BarierPoint();
	            if( dictionary.ContainsKey( "x" ) )
	                obj.X = serializer.ConvertToType<int>(
	                                           dictionary["x"] );
	            if( dictionary.ContainsKey( "y" ) )
	                obj.Y = serializer.ConvertToType<int>(
	                                           dictionary["y"] );
	
	            return obj;
	        }
	
	        return null;
	    }
	
	    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer )
	    {
	        var dataObj = obj as BarierPoint;
	        if( dataObj != null )
	        {
	            return new Dictionary<string,object>
	            {
	                {"x", dataObj.X },
	                {"y", dataObj.Y}
	            };
	        }
	        return new Dictionary<string, object>();
	    }
	}

}

