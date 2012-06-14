public class Album {

	private int _id;
	private int _count;
	private string _caption;
	private bool _ispublic;

	public int AlbumID { get { return _id; } }
	public int Count { get { return _count; } }
	public string Caption { get { return _caption; } }
	public bool IsPublic { get { return _ispublic; } }

	public Album(int id, int count, string caption, bool ispublic) {
		_id = id;
		_count = count;
		_caption = caption;
		_ispublic = ispublic;
	}

}

