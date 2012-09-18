using System;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Moscrif.IDE.Devices
{
	public class SkinJavaScriptConverter : JavaScriptConverter
	{
	    private static readonly Type[] _supportedTypes = new[]
	    {
	        typeof( Skin )
	    };

	    public override IEnumerable<Type> SupportedTypes
	    {
	        get { return _supportedTypes; } 
	    }
	
	    public override object Deserialize( IDictionary<string, object> dictionary,Type type,JavaScriptSerializer serializer )
	    {
	        if( type == typeof( Skin ) )
	        {
	            var obj = new Skin();
	            if( dictionary.ContainsKey( "name" ) )
	                obj.Name = serializer.ConvertToType<string>(dictionary["name"] );
	            if( dictionary.ContainsKey( "resolution" ) )
	                obj.ResolutionJson = serializer.ConvertToType<string>(dictionary["resolution"] );//Resolution
	            if( dictionary.ContainsKey( "theme" ) )
	                obj.Theme = serializer.ConvertToType<string>(dictionary["theme"] );
	            return obj;
	        }
	
	        return null;
	    }
	
	    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer )
	    {
	        var dataObj = obj as Skin;
	        if( dataObj != null )
	        {
	            return new Dictionary<string,object>
	            {
	                {"name", dataObj.Name==null?"":dataObj.Name },
	                {"resolution", dataObj.ResolutionJson==null?"":dataObj.ResolutionJson },//Resolution
			{"theme", dataObj.Theme==null?"":dataObj.Theme },
	            };
	        }
	        return new Dictionary<string, object>();
	    }
	}
}

