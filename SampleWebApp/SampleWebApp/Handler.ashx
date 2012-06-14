<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.IO;
using System.Web;

public class Handler : IHttpHandler {

	public bool IsReusable {
		get {
			return true;
		}
	}
	
	public void ProcessRequest (HttpContext context) {
		// Set up the response settings
		context.Response.ContentType = "image/jpeg";
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.BufferOutput = false;
		// Setup the Size Parameter
		PhotoSize size;
		switch (context.Request.QueryString["Size"]) {
			case "S":
				size = PhotoSize.Small;
				break;
			case "M":
				size = PhotoSize.Medium;
				break;
			case "L":
				size = PhotoSize.Large;
				break;
			default:
				size = PhotoSize.Original;
				break;
		} 
		// Setup the PhotoID Parameter
		Int32 id = -1;
		Stream stream = null;
		if (context.Request.QueryString["PhotoID"] != null && context.Request.QueryString["PhotoID"] != "") {
			id = Convert.ToInt32(context.Request.QueryString["PhotoID"]);
			stream = PhotoManager.GetPhoto(id, size);
		} else {
			id = Convert.ToInt32(context.Request.QueryString["AlbumID"]);
			stream = PhotoManager.GetFirstPhoto(id, size);
		}
		// Get the photo from the database, if nothing is returned, get the default "placeholder" photo
		if (stream == null) stream = PhotoManager.GetPhoto(size);
		// Write image stream to the response stream
		const int buffersize = 1024 * 16;
		byte[] buffer = new byte[buffersize];
		int count = stream.Read(buffer, 0, buffersize);
		while (count > 0) {
			context.Response.OutputStream.Write(buffer, 0, count);
			count = stream.Read(buffer, 0, buffersize);
		}
	}

}