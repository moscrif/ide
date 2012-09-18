using System;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Moscrif.IDE.Devices
{
	public class DeviceJavaScriptConverter : JavaScriptConverter
	{
	    private static readonly Type[] _supportedTypes = new[]
	    {
	        typeof( Device )
	    };
	
	    public override IEnumerable<Type> SupportedTypes
	    { 
	        get { return _supportedTypes; } 
	    }
	
	    public override object Deserialize( IDictionary<string, object> dictionary,Type type,JavaScriptSerializer serializer )
	    {
	        if( type == typeof( Device ) )
	        {
	            var obj = new Device();
	            if( dictionary.ContainsKey( "publishDir" ) )
	                obj.Publish = serializer.ConvertToType<string>(dictionary["publishDir"] );
	            if( dictionary.ContainsKey( "root" ) )
	                obj.Root = serializer.ConvertToType<string>(dictionary["root"] );
	            if( dictionary.ContainsKey( "application" ) )
	                obj.Application = serializer.ConvertToType<string>(dictionary["application"] );
	            if( dictionary.ContainsKey( "outputDir" ) )
	                obj.Output_Dir = serializer.ConvertToType<string>(dictionary["outputDir"] );
	            if( dictionary.ContainsKey( "tempDir" ) )
	                obj.Temp = serializer.ConvertToType<string>(dictionary["tempDir"] );

	            if( dictionary.ContainsKey( "facebookAppID" ) )
	                obj.FacebookAppID = serializer.ConvertToType<string>(dictionary["facebookAppID"] );

	            if( dictionary.ContainsKey( "outputName" ) )
	                obj.Output_Name = serializer.ConvertToType<string>(dictionary["outputName"] );

	            if( dictionary.ContainsKey( "log" ) )
	                obj.LogDebug = serializer.ConvertToType<bool>(dictionary["log"] );

	            if( dictionary.ContainsKey( "applicationType" ) )
	                obj.ApplicationType = serializer.ConvertToType<string>(dictionary["applicationType"] );

	 	    if( dictionary.ContainsKey( "includes" ) )
	                obj.Includes = serializer.ConvertToType<Includes>(dictionary["includes"] );
	 	    //if( dictionary.ContainsKey( "publish" ) )
	              //  obj.PublishPropertisFull = serializer.ConvertToType<Moscrif.IDE.Devices.PublishProperty>(dictionary["publish"] );
	 	    //if( dictionary.ContainsKey( "conditions" ) )
	              //  obj.PublishPropertisFull = serializer.ConvertToType<Moscrif.IDE.Devices.PublishProperty>(dictionary["conditions"] );
			return obj;
	        }

	        return null;
	    }
	
	    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer )
	    {
	        var dataObj = obj as Device;
	        if( dataObj != null )
	        {
	            return new Dictionary<string,object>
	            {
	                {"publishDir", dataObj.Publish },
	                {"root", dataObj.Root},
	                {"application", dataObj.Application},
	                {"outputDir", dataObj.Output_Dir},
	                {"tempDir", dataObj.Temp},
			{"outputName", dataObj.Output_Name},
			{"publish",dataObj.PublishPropertisFull},
			{"includes",dataObj.Includes},
			{"conditions",dataObj.Conditions},
			{"log",dataObj.LogDebug},
			{"applicationType",dataObj.ApplicationType},
			{"facebookAppID",dataObj.FacebookAppID}

		    };
	        }
	        return new Dictionary<string, object>();
	    }
	}
}

