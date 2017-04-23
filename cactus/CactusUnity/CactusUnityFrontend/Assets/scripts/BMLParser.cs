public class BMLParser {

	public List<Frame> loadAnimation(string filename) {
		XmlDocument doc = new XmlDocument();
		doc.Load(filename);
		XmlNode root = doc.DocumentElement;
		XmlNodeList nodeList = root.SelectNodes ("frame");
		foreach (XmlNode node in nodeList) {
			Frame f = new Frame ();
			f.duration = node.Attribute
		}
	}
}
