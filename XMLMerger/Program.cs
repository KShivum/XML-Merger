class Program
{
    public static void Main(String[] args)
    {
        //Opens and stores both XML files from argument into string arrays
        string[] xml1 = File.ReadAllLines(args[0]);
        string[] xml2 = File.ReadAllLines(args[1]);

        List<XMLFile> firstXML = new List<XMLFile>();
        List<XMLFile> secondXML = new List<XMLFile>();

        if(!xml1[11].Contains("<files>") || !xml2[11].Contains("<files>"))
        {
            Console.WriteLine("Error: XML files are not in the correct format. This program might be outdated");
            return;
        }
        System.Console.WriteLine("Parsing XML files...");
        //Adds all the files from the first XML file into a list
        for(int i = 12; i < xml1.Length - 3; i+=6)
        {
            string fullFile = xml1[i] + "\n" + xml1[i + 1] + "\n" + xml1[i + 2] + "\n" + xml1[i + 3] + "\n" + xml1[i + 4] + "\n" + xml1[i + 5];
            string path = xml1[i+4];
            int id = Convert.ToInt32(xml1[i+2].Replace("<id>", "").Replace("</id>", ""));
            firstXML.Add(new XMLFile(path, fullFile, id));
        }

        //Adds all the files from the second XML file into a list
        for(int i = 12; i < xml2.Length - 3; i+=6)
        {
            string fullFile = xml2[i] + "\n" + xml2[i + 1] + "\n" + xml2[i + 2] + "\n" + xml2[i + 3] + "\n" + xml2[i + 4] + "\n" + xml2[i + 5];
            string path = xml2[i+4];
            int id = Convert.ToInt32(xml2[i+2].Replace("<id>", "").Replace("</id>", ""));
            secondXML.Add(new XMLFile(path, fullFile, id));
        }

        //TODO: Just replace this with firstXML
        List<XMLFile> mergedXML = new List<XMLFile>();
        //Adds all the files from the first XML file into the merged XML file
        mergedXML.AddRange(firstXML);

        //Adds all the files from the second XML file into a list
        foreach(XMLFile file in secondXML)
        {
            var foundFile = mergedXML.FirstOrDefault(x => x.path.Equals(file.path));
            if(foundFile == null)
            {
                if(firstXML.Any(x => x.id == file.id))
                {
                    System.Console.WriteLine("Duplicate ID found, but different path. Adding file to merged XML with unique ID");
                    int newID = int.MaxValue;
                    while(firstXML.Any(x => x.id == newID))
                    {
                        newID--;
                    }
                    file.id = newID;
                    file.full = file.full.Replace("<id>" + file.id + "</id>", "<id>" + newID + "</id>");
                    mergedXML.Add(file);
                }
            }
            else
            {
                if(foundFile.id != file.id)
                {
                    System.Console.WriteLine("Duplicate path found, but different ID. Skipping file");
                }
            }
        }

        //Writes the merged XML file to a new file
        System.Console.WriteLine("Sorting merged XML file...");
        mergedXML = mergedXML.OrderBy(x => x.id).ToList();
        System.Console.WriteLine("Writing merged XML file...");
        using (StreamWriter sw = new StreamWriter("merged.xml"))
        {
            for(int i = 0; i < 12; i++)
            {
                sw.WriteLine(xml1[i]);
            }

            //Writes all the files from the merged XML file
            foreach(XMLFile file in mergedXML)
            {
                sw.WriteLine(file.full);
            }

            //Writes the last 3 lines of the XML file
            sw.WriteLine("  </files>");
            sw.WriteLine("</bnd4>");
            System.Console.WriteLine("Done!");
        }

    }
}

public class XMLFile
{
    public string path, full;
    public int id;

    public XMLFile(string path, string full, int id)
    {
        this.path = path;
        this.full = full;
        this.id = id;
    }

}