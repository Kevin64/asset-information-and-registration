namespace HardwareInformation
{
    public static class BIOSFileReader
    {
        //Reads txt file and parses brand, model and BIOS versions, returning the latter
        public static string readLine(string brand, string model)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"BIOSversions.txt");
            string line;

            while ((line = file.ReadLine()) != null)
            {
                if ((brand.Contains("Hewlett-Packard") || brand.Contains("Quanta")) && line.Substring(0, line.IndexOf("-")).Equals("HP"))
                {
                    if (model.Contains(line.Substring(line.IndexOf("-") + 1, line.IndexOf("=") - line.IndexOf("-") - 1)))
                        return line.Substring(line.IndexOf("=") + 1);
                }
                else if (brand.Contains("Dell") && line.Substring(0, line.IndexOf("-")).Equals("Dell"))
                {
                    if (model.Contains(line.Substring(line.IndexOf("-") + 1, line.IndexOf("=") - line.IndexOf("-") - 1)))
                        return line.Substring(line.IndexOf("=") + 1);
                }
                else if (brand.Contains("Lenovo") && line.Substring(0, line.IndexOf("-")).Equals("Lenovo"))
                {
                    if (model.Contains(line.Substring(line.IndexOf("-") + 1, line.IndexOf("=") - line.IndexOf("-") - 1)))
                        return line.Substring(line.IndexOf("=") + 1);
                }
            }
            file.Close();
            return null;
        }        
    }
}
